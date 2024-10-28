using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class chat : MonoBehaviour, IChatClientListener
{
    public ChatClient chatClient;
    public string userName;
    public TMP_InputField inputField;
    public Text chatContent;

    void Start() {
        chatClient = new ChatClient(this);
    }

    void Update() {
        chatClient.Service();
    }

    public void setMessages(){
        if(inputField.text == ""){
            return;
        }
        
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name , inputField.text);
        inputField.text = "";


    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Chat - " + level + " - " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat - OnChatState " + state);
    }

    public void OnConnected()
    {
        Debug.Log("Chat - User:" + userName + "Has been conncected!");
        chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name, creationOptions: new ChannelCreationOptions(){PublishSubscribers = true});
    }

    public void OnDisconnected()
    {
        Debug.Log("Chat - User:" + userName + "has disconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ChatChannel currentChat;
        if(chatClient.TryGetChannel(PhotonNetwork.CurrentRoom.Name, out currentChat))
        {
            chatContent.text = currentChat.ToStringMessages();
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i < channels.Length; i++ ){
            if(results[i])
            {
                Debug.Log("chat - Subscribed to " + channels[i] + "channel" );
                chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, "has Joined the Chat ! ");
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }

    
}
