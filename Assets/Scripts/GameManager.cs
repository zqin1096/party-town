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

    public static int GetLocalActorNumber() {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public static int GetRemoteActorNumber() {
        // 1 => 2
        // 2 => 1
        return 3 - PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public static PlayerController GetLocal() {
        return GameManager.instance.players[GameManager.GetLocalActorNumber() - 1];
    }

    public static PlayerController GetRemote() {
        // 1 => 1
        // 2 => 0
        return GameManager.instance.players[GameManager.GetRemoteActorNumber() - 1];
    }

    public static PlayerController GetPlayer(int actorNumber) {
        return GameManager.instance.players[actorNumber - 1];
    }

    public static Dictionary<string, string>[] GetPlayerStates() {
        GameManager instance = GameManager.instance;
        Dictionary<string, string>[] states = new Dictionary<string, string>[2];
        for (int i = 0; i < instance.players.Length; i += 1) {
            states[i] = instance.players[i].state;
        }
        return states;
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

        player1.photonView.RPC(
            "UpdateState",
            RpcTarget.AllBuffered,
            GameManager.GetPlayerStates(),
            GameManager.GetLocalActorNumber()
        );

        player2.photonView.RPC(
            "UpdateState",
            RpcTarget.AllBuffered,
            GameManager.GetPlayerStates(),
            GameManager.GetLocalActorNumber()
        );
    }

    void TransferOwnerships() {
        player1.photonView.TransferOwnership(1);
        player2.photonView.TransferOwnership(2);
    }
}
