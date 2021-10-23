using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createIF;
    public InputField joinIF;

    public void CreateRoom()
    {
        if(createIF.text!=null)
            PhotonNetwork.CreateRoom(createIF.text);
    }

    public void JoinRoom()
    {
        if(joinIF.text!=null)
            PhotonNetwork.JoinRoom(joinIF.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("InGame");
    }
}
