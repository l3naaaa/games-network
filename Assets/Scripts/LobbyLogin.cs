using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leguar.TotalJSON;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyLogin : MonoBehaviourPunCallbacks
{
    
    public TMP_InputField PlayerNameInput;
    string playerName;

    public Transform loginPanel;
    public Transform selectionPanel;
    public Transform createRoomPanel;
    public Transform insideRoomPanel;
    public Transform listRoomPanel;
    public Transform chatPanel;

    public TMP_InputField roomName;

    public GameObject textNamePrefab;
    public Transform insideRoomPlayerList;
    public Transform roomListnew;
    public GameObject roomListPrefab;
    public Transform roomListContent;

    public Button StartGameButton;
    Dictionary<string , RoomInfo> cachedRoomList;

    public chat pChat;
    void Awake(){
        StartGameButton.onClick.AddListener(OnStartGame);
    }

    private void Start(){

        loginPanel.gameObject.SetActive(false);
        PlayerNameInput.text = playerName = string.Format("Player {0}" , UnityEngine.Random.Range(1,10000));
        cachedRoomList = new Dictionary<string, RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;

        LoadPlayerData();
        if(string.IsNullOrEmpty(PlayerData.instance.userName)){
            loginPanel.gameObject.SetActive(true);
            return;
        }
        
        PlayerNameInput.text = PlayerData.instance.userName;
        LoginButtonCLick();
    }

    public string filePath;
     public void LoadPlayerData(){
        if(!File.Exists(filePath)){
            // playerData = new PlayerData();
            SavePlayerData();
        }
        string fileContent = File.ReadAllText(filePath);
        // playerData = JSON.ParseString(fileContent).Deserialize<PlayerData>();
        PlayerData.instance.LoadFromSave(fileContent);
    }

    public void SavePlayerData(){
        string serializedData = JSON.Serialize(PlayerData.instance).CreateString();
        string savedEncryptedData = Encrypter.encrypt(serializedData);
        File.WriteAllText(filePath , savedEncryptedData);
    }
    public void ActivatePanel(string panelName){
        loginPanel.gameObject.SetActive(false);
        selectionPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
        insideRoomPanel.gameObject.SetActive(false);
        listRoomPanel.gameObject.SetActive(false);
        chatPanel.gameObject.SetActive(false);

            if(panelName == loginPanel.gameObject.name)
                loginPanel.gameObject.SetActive(true);
            else if(panelName == selectionPanel.gameObject.name)
                selectionPanel.gameObject.SetActive(true);
            else if(panelName == createRoomPanel.gameObject.name)
                createRoomPanel.gameObject.SetActive(true);
            else if(panelName == insideRoomPanel.gameObject.name)
                insideRoomPanel.gameObject.SetActive(true);
            else if(panelName == listRoomPanel.gameObject.name)
                listRoomPanel.gameObject.SetActive(true);
            else if(panelName == chatPanel.gameObject.name)
                chatPanel.gameObject.SetActive(true);
    }
    public void LoginButtonCLick(){
        if(PlayerNameInput.text.Trim() != ""){
            PhotonNetwork.LocalPlayer.NickName = playerName = PlayerNameInput.text;
            PlayerData.instance.userName = playerName;
            LoginToPlayFab();
            PhotonNetwork.ConnectUsingSettings();
        }
        else{
            Debug.Log("player Name is Invalid");
        }
       
    }

    #region PlayFab
    public void LoginToPlayFab(){
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest(){
            CreateAccount = true,
            CustomId = PlayerData.instance.uid,
            
            
        };
        PlayFabClientAPI.LoginWithCustomID(request , PlayFabLoginResult, PlayFabLoginError);
    }

    void UpdatePlayFabUserName(string name){
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest{
            DisplayName = name,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request , PlayFabNameUpdateResult , PlayFabNameUpdateError );
    }

    void PlayFabNameUpdateResult(UpdateUserTitleDisplayNameResult updateUserTitleDisplayNameResult){
        Debug.Log("Playfab- Player name update success");
    }
    void PlayFabNameUpdateError(PlayFabError userNameUpdateError){
        Debug.Log("PlayFab- Error occured while updatingUserName: " + userNameUpdateError.ErrorMessage);
    }
    
    void PlayFabLoginResult(LoginResult loginResult){
        //
        Debug.Log("Playfab - Login succeed: " + loginResult.ToJson());
        UpdatePlayFabUserName(PlayerData.instance.userName);
    }

    void PlayFabLoginError(PlayFabError loginError){
        Debug.Log("PlayFab - Login Faled : " + loginError.ErrorMessage);
    }
    #endregion

    public override void OnConnectedToMaster()
    {
       Debug.Log("Succesfully Connected to Master Server");
       ActivatePanel("Selection");
    }
    public void DisconnectButtonClick(){
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
       Debug.Log("Disconnected from Master Server ! ");
       ActivatePanel("Login");
    }

    public void RandomJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public void CreateRoom(){
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomName.text , roomOptions );
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Game Room Has been created ! ");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a Room");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinedRoom()
    {
        var authenticationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        pChat.userName = PhotonNetwork.LocalPlayer.NickName;
        pChat.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,"1.0",authenticationValues); 
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        Debug.Log("Joined to a Game Room ");
        ActivatePanel("InsideRoom");

        UpdatePlayerList();  
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        //PURGE
        for (int i = 0; i < insideRoomPlayerList.childCount; i++)
        {
            Destroy(insideRoomPlayerList.GetChild(i).gameObject);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntrry = Instantiate(textNamePrefab, insideRoomPlayerList);
            playerListEntrry.GetComponent<Text>().text = player.NickName;
        }
    }

    public void OnStartGame(){
        SceneManager.LoadScene("GameScene 1");
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        pChat.chatClient.Disconnect();
    
        Debug.Log("You Left the Room ..");
        ActivatePanel("CreateRoom");
        DestroyChildren(insideRoomPlayerList);
    }
    public void DestroyChildren(Transform parent){
            foreach(Transform child in parent){
                Destroy(child.gameObject);
            }
    }

    public void ListRoomClick(){
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to The Lobby! ");
        ActivatePanel("ListRooms");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);

        UpdateCacheRoomList(roomList);
        Debug.Log(cachedRoomList.Count);

        //PURGE
        for(int i=0; i < roomListContent.childCount; i++){
            Destroy(roomListContent.GetChild(i).gameObject);
        }

        foreach(var room in cachedRoomList){
            var newRoomEntry = Instantiate(roomListPrefab, roomListContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Key;
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1}/{2}) ]" , room.Key, room.Value.PlayerCount , room.Value.MaxPlayers);
        }
    }
    public void LeaveLobbyClick(){
        PhotonNetwork.LeaveLobby();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Left the Lobby");
        DestroyChildren(roomListContent);
        cachedRoomList.Clear();
        ActivatePanel("Selection");
    }
    public void UpdateCacheRoomList(List<RoomInfo> roomList){
        foreach(var room in roomList){
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList){
                    cachedRoomList.Remove(room.Name);
                    //Debug.Log($"Removed room {room.Name}, isOpen:{room.IsOpen}, isVisible:{room.IsVisible}, removed?:{room.RemovedFromList}");
            }else{
                cachedRoomList[room.Name] = room;
            }
        }
    }

}
