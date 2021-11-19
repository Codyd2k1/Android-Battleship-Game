using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createButtonInput;
    public InputField joinButtonInput;

    public Text joinRoomFailedMessage;

    public Text NoRoomNameEnteredMessage;
    public void createRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.PlayerTtl = 60000;
        roomOptions.EmptyRoomTtl = 60000;
        string roomName = createButtonInput.text;
        if(string.IsNullOrEmpty(roomName))
        {
            joinRoomFailedMessage.gameObject.SetActive(false);
            NoRoomNameEnteredMessage.gameObject.SetActive(true);
            return;
        }
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void joinRoom()
    {
        Debug.Log("Attempting to join room");
        string roomName = joinButtonInput.text;
        if(string.IsNullOrEmpty(roomName))
        {
            
            roomName = "noRoomNameEntered1095";
        }
        
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        NoRoomNameEnteredMessage.gameObject.SetActive(false);
        joinRoomFailedMessage.gameObject.SetActive(false);
        PhotonNetwork.LoadLevel("game");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("join room failed");
        NoRoomNameEnteredMessage.gameObject.SetActive(false);
        joinRoomFailedMessage.gameObject.SetActive(true);
        base.OnJoinRoomFailed(returnCode, message);
    }
}
