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

    public static PlayerController GetLocal() {
        return GameManager.instance.players[PhotonNetwork.LocalPlayer.ActorNumber - 1];
    }

    public static PlayerController GetRemote() {
        // 1 => 1
        // 2 => 0
        return GameManager.instance.players[PhotonNetwork.LocalPlayer.ActorNumber % 2];
    }

    public static PlayerController GetPlayer(int actorNumber) {
        return GameManager.instance.players[actorNumber - 1];
    }

    void Awake() {
        instance = this;
    }

    void Start() {
        this.players = new PlayerController[2]{
            this.player1,
            this.player2
        };

        Debug.LogFormat("GameManager.Start(), IsMaterClient: {0}", PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient) {
            TransferOwnerships();
            InitializePlayers();
        }
    }

    void InitializePlayers() {
        Dictionary<String, String> player1State = PlayerState.getInstance();
        player1State["turn"] = "1";

        player1.photonView.RPC(
            "Initialize",
            RpcTarget.AllBuffered,
            PhotonNetwork.CurrentRoom.GetPlayer(1),
            player1State
        );

        player2.photonView.RPC(
            "Initialize",
            RpcTarget.AllBuffered,
            PhotonNetwork.CurrentRoom.GetPlayer(2),
            PlayerState.getInstance()
        );
    }

    void TransferOwnerships() {
        player1.photonView.TransferOwnership(1);
        player2.photonView.TransferOwnership(2);
    }
}
