using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Player : MonoBehaviourPunCallbacks
{
    [HideInInspector] public PhotonView view;
    public Hashtable playerInfo;

    private void Awake()
    {
        playerInfo = new Hashtable();
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
            GameController.GetInstance._player = this;
        playerInfo["Round"]=1;
        StorePlayerInfo();
        
    }

    public void StorePlayerInfo()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < GameCore.GetInstance.map[i].Length; j++)
            {
                string s;
                s = new Vector2(i, j).ToString();
                playerInfo[s] = JsonUtility.ToJson(GameCore.GetInstance.map[i][j]);
            }
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
    }

    public void StoreRound()
    {
        playerInfo["Round"] = GameCore.GetInstance.EndTurn();
        StorePlayerInfo();
    }

    public void SyncPlayerMap(Hashtable playerInfo,Photon.Realtime.Player player)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < GameCore.GetInstance.map[i].Length; j++)
            {
                string s = new Vector2(i, j).ToString();
                GameCore.GetInstance.map[i][j] = JsonUtility.FromJson<Card>((string)playerInfo[s]);
            }
        }
        if(!player.IsLocal)
            GameCore.GetInstance.VerticalReverse();
    }

    public void GetRound(Hashtable playerInfo)
    {
        GameCore.GetInstance.round = (int)playerInfo["Round"];
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        SyncPlayerMap(changedProps,targetPlayer);
        GetRound(changedProps);
        playerInfo = changedProps;
        GameController.GetInstance.UpdateImage();
        GameController.GetInstance.roundText.text ="回合："+((int)changedProps["Round"]);
    }
}