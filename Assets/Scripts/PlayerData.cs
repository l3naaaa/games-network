using Leguar.TotalJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
public string uid;
public string userName;
public int bestScore;
public string scoreData;
public int totalPlayersInGame;
public string roomName;

private static PlayerData m_instance;
public static PlayerData instance{
    get{
        if(m_instance == null){
            m_instance = new PlayerData();
        }

        return m_instance;
    }
}


public void LoadFromSave(PlayerData playerData){
    m_instance = playerData;
}
public void LoadFromSave(string playerSaveText)
{
    string decryptedData = Encrypter.decrypt(playerSaveText);

        PlayerData playerData = JSON.ParseString(decryptedData).Deserialize<PlayerData>();
    m_instance = playerData;
        Debug.Log("Player data Load succesfuly !");
        Debug.Log("Player Data before encryption: " + playerSaveText);
        Debug.Log("Player Data after encryption: " + decryptedData);
    }
    public PlayerData(){
    uid = Guid.NewGuid().ToString();
}

}
