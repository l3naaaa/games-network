using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.IO;
using Leguar.TotalJSON;
using PlayFab.ClientModels;
using PlayFab;
using Photon.Pun.UtilityScripts;


public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public const byte GameRestartEventCode = 1;


    public Transform spawnPointsParent;
    private Transform[] spawnPoints;
    public static GameManager instance;

    public double timer = 0;
    public double startTime = 0;
    public double endTime = 100;
    public TMP_Text txtTimer;
    public GameObject winPanel, lossPanel;
    public TMP_Text winnerName;
    public TMP_Text playerLeftTxt;

    //public PlayerData playerData;
    public string filePath;
    public LeaderBoard leaderBoard;

    ExitGames.Client.Photon.Hashtable CustomData;
    void Awake(){
        instance = this;
        spawnPoints = new Transform[spawnPointsParent.childCount];

        for(int i=0; i < spawnPointsParent.childCount; i++){
            spawnPoints[i] = spawnPointsParent.GetChild(i);
        }

    }
    int pid;
    void Start()
    {
        //loads player data method
        LoadPlayerData();
        

        if(!PhotonNetwork.InRoom){Debug.LogError("Not in a room!");return;}
        for(int i=0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer) { pid = i; }
        }
        PhotonNetwork.Instantiate("Player", spawnPoints[pid].position, Quaternion.identity);

        if (PhotonNetwork.IsMasterClient)
        {
            CustomData = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            CustomData.Add("StartTime", startTime);

            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomData);
            isGameStarted = true;
        }
        else
        {
            // Debug.Log("Custom start time " + PhotonNetwork.CurrentRoom.CustomProperties["StartTime"]);
            StartCoroutine(KickstartClient());
        }
    }

    IEnumerator KickstartClient(){
        while(!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StartTime")){
            yield return new WaitForSeconds(0.1f);
        }

        startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
        isGameStarted=true;
    }

    bool isGameStarted =false;

    public void onNextRound()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameRestartEventCode,PhotonNetwork.Time , raiseEventOptions, SendOptions.SendReliable);
    }

    public void onLeave()
    {
        try
        {
            PhotonNetwork.Disconnect();
        }
        catch (Exception e) { }
        SceneManager.LoadScene("GameLobby");
    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if(eventCode == GameRestartEventCode)
        {
            startTime = (double)photonEvent.CustomData;
            winPanel.SetActive(false);lossPanel.SetActive(false);winnerName.text = "";

            Respawn();
            playerController.localPlayer.network.Reset();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        playerLeftTxt.text = $"{otherPlayer.NickName} Left the game";
        playerLeftTxt.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    bool isGameSaved = false;
    void Update()
    {
        if(!isGameStarted){return;}
        playerLeftTxt.color = new Color(1, 1, 1, Mathf.Lerp(playerLeftTxt.color.a, 0, 0.005f));
        if (startTime <= 0)
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
        }
        else 
        {
            timer = PhotonNetwork.Time - startTime;

            if(timer > endTime)
            {
                //On game end

                playerNetwork[] players = FindObjectsOfType<playerNetwork>();
                int highestScore = 0;
                playerNetwork winner = playerController.localPlayer.network;
                foreach(playerNetwork player in players) {
                    if(player.score > highestScore)
                    {
                        highestScore = player.score;
                        winner = player;
                    }
                }

                if(playerController.localPlayer.network == winner)
                {
                    //I am the winner
                    winPanel.SetActive(true);
                }
                else
                {
                    lossPanel.SetActive(true);

                }
                if(isGameSaved == false){
                    //save method best score save and leaderboard method
                    StorePersonalBest();
                    isGameSaved = true;
                }
                
                winnerName.text = $"The winner is {winner.username}";
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                txtTimer.text = (endTime - timer).ToString("n0");
            }
        }
    }

    public static void Respawn()
    {
        instance.respawn();
    }
    public void respawn()
    {
        playerController.localPlayer.transform.position = GetRandomSpawnPoint().position;
        playerController.localPlayer.network.health = 100;
    }


    Transform GetRandomSpawnPoint(){
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    public void SavePlayerData(){
        string serializedData = JSON.Serialize(PlayerData.instance).CreateString();
        
        string encrpytedData = Encrypter.encrypt(serializedData);
        File.WriteAllText(filePath , encrpytedData);
        Debug.Log("Player data saved succesfuly !");
        Debug.Log("Player Data before encryption: "+serializedData );
        Debug.Log("Player Data after encryption: " + encrpytedData);
    }

    public void LoadPlayerData(){
        if(!File.Exists(filePath)){
            // playerData = new PlayerData();
            SavePlayerData();
        }
        string fileContent = File.ReadAllText(filePath);
        // playerData = JSON.ParseString(fileContent).Deserialize<PlayerData>();
        PlayerData.instance.LoadFromSave(fileContent);

    }

    public void LoginToPlayFab(){
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest(){
            CreateAccount = true,
            CustomId = PlayerData.instance.uid,
            
        };
        PlayFabClientAPI.LoginWithCustomID(request , PlayFabLoginResult, PlayFabLoginError);
    }
    
    void PlayFabLoginResult(LoginResult loginResult){
        //
        Debug.Log("Playfab - Login succeed: " + loginResult.ToJson());
    }

    void PlayFabLoginError(PlayFabError loginError){
        Debug.Log("PlayFab - Login Faled : " + loginError.ErrorMessage);
    }

    void StorePersonalBest(){
        int CurrentScore = playerController.localPlayer.network.score;
        // PlayerData playerData = GameManager.instance.playerData;
        Debug.Log("New score " + CurrentScore);
        if(CurrentScore > PlayerData.instance.bestScore){
            Debug.Log("New score is better!");
            PlayerData.instance.userName = PhotonNetwork.LocalPlayer.NickName;
            PlayerData.instance.bestScore = CurrentScore;
            PlayerData.instance.scoreData = DateTime.UtcNow.ToString();
            PlayerData.instance.totalPlayersInGame = PhotonNetwork.CurrentRoom.PlayerCount;
            PlayerData.instance.roomName = PhotonNetwork.CurrentRoom.Name;

            LeaderBoard.instance.SubmitScore(CurrentScore);
            SavePlayerData();
        }
    }
}
