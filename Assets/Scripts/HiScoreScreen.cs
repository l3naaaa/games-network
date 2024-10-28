using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiScoreScreen : MonoBehaviour
{
    public GameObject hiScoreUIHolder;
    public GameObject noScore;

    public TMP_Text playerName;
    public TMP_Text hiScore;
    public TMP_Text date;
    public TMP_Text playerCount;
    public TMP_Text roomName;

    public void UpdateHiScoreUI(){
        Debug.Log("Updating High score");
        PlayerData playerData = PlayerData.instance;
        if(playerData.bestScore > 0 ){
            playerName.text = playerData.userName;
            hiScore.text = playerData.bestScore.ToString();
            date.text = playerData.scoreData;
            playerCount.text = playerData.totalPlayersInGame.ToString();
            roomName.text = playerData.roomName;

            hiScoreUIHolder.SetActive(true);
            noScore.SetActive(false);
        }
        else{
            noScore.SetActive(true);
            hiScoreUIHolder.SetActive(false);
        }
    }

    private void OnEnable() {
        UpdateHiScoreUI();
    }
}
