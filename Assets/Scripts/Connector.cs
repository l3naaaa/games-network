using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Connector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        RoomOptions roomOptions = new RoomOptions(){IsVisible=true, IsOpen=true};
     //   PhotonNetwork.JoinOrCreateRoom("Test", roomOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
