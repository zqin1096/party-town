using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun {
    public PlayerController player1;
    public PlayerController player2;
    public PlayerController[] players;

    public static GameManager instance;

    void Start() {
        instance = this;

        this.players = new PlayerController[2]{
            this.player1,
            this.player2
        };

        Debug.LogFormat("GameManager.Start(), IsMaterClient: {0}", PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient) {
            SetPlayers();
        }
    }

    void SetPlayers() {
        player1.photonView.TransferOwnership(1);
        player2.photonView.TransferOwnership(2);

        player1.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(1));
        player2.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(2));
    }

    public PlayerController GetLocal() {
        return this.players[PhotonNetwork.LocalPlayer.ActorNumber - 1];
    }

    public PlayerController GetRemote() {
        // 1 => 1
        // 2 => 0
        return this.players[PhotonNetwork.LocalPlayer.ActorNumber % 2];
    }
}
