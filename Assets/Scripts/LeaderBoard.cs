using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Security.Cryptography.X509Certificates;

public class LeaderBoard : MonoBehaviour
{
    
    int maxResults = 10;
    public leaderBoardPopUp leaderBoardPopUp;
    public static LeaderBoard instance;
   
   void Awake(){
    instance=this;
   }
   public void SubmitScore(int playerScore){

        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest(){
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate(){
                    StatisticName = "Most Kills",
                    Value = playerScore,
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request , PlayFabUpdateStatsResult ,PlayFabUpdateStatsError );
   }

   void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult updatePlayerStatisticsResult){
    Debug.Log("PlayFab - Score Submitted !");
   }

   void PlayFabUpdateStatsError(PlayFabError updatePlayerStatError){
    Debug.Log("PlayFab - Error occured while submitting stat: " + updatePlayerStatError.ErrorMessage);
   }

   public void GetLeaderBoard(){
        

        GetLeaderboardRequest request = new GetLeaderboardRequest(){
            MaxResultsCount = maxResults,
            StatisticName = "Most Kills",
        };

        PlayFabClientAPI.GetLeaderboard(request , PlayFabGetLeaderBoardResult , PlayFabGetLeaderBoardResultError);
   }
   void PlayFabGetLeaderBoardResult(GetLeaderboardResult getLeaderboardResult){
        Debug.Log("PlayFab : GetLeaderBoard Complete ! ");
        //reference leaderboard popoup script and call update ui method
        leaderBoardPopUp.UpdateUI(getLeaderboardResult.Leaderboard);
   }

   void PlayFabGetLeaderBoardResultError(PlayFabError getLeaderBoardResultError){
        Debug.Log("PlayFab: Error occured while Getting LeaderBoard Results : " + getLeaderBoardResultError.ErrorMessage);
   }
}
