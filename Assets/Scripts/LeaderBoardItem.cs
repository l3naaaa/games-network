using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardItem : MonoBehaviour
{
   public Text order;
   public Text userName;
   public Text score;


   public void SetScore(int _order , string _username, int _score){
        order.text = _order.ToString() + ")";
        userName.text = _username ;
        score.text = _score.ToString();
   }

}
