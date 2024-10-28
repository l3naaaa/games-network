using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

public class leaderBoardPopUp : MonoBehaviour
{
   public GameObject scoreHolder;
   public GameObject noScoreHolder;
   public GameObject leaderBoardItem;

   void OnEnable(){
    //getLeaderboard method
    LeaderBoard.instance.GetLeaderBoard();
   }

   void DestroyChildrenHere(Transform parent){

        foreach(Transform child in parent ){
            Destroy(child.gameObject);
        }

   }

   public void UpdateUI(List<PlayerLeaderboardEntry> playerLeaderboardEntries){
        //
        Debug.Log(playerLeaderboardEntries.Count);
        if(playerLeaderboardEntries.Count > 0 ){
            DestroyChildrenHere(scoreHolder.transform);
            for(int i = 0; i < playerLeaderboardEntries.Count; i++ ){
                GameObject newLeaderBoardItem = Instantiate(leaderBoardItem , Vector3.zero , Quaternion.identity , scoreHolder.transform);
                newLeaderBoardItem.GetComponent<LeaderBoardItem>().SetScore( i+1 , playerLeaderboardEntries[i].DisplayName , playerLeaderboardEntries[i].StatValue);
            }

            scoreHolder.SetActive(true);
            noScoreHolder.SetActive(false);
        }
        else{
            scoreHolder.SetActive(false);
            noScoreHolder.SetActive(true);
        }

   }
}
