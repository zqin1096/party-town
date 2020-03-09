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

    public PlayerController currentPlayer;
    public float postGameTime = 30f;
    public static bool isGameEnded;

    [PunRPC]
    void SetNextTurn() {
        if (currentPlayer == null) {
            currentPlayer = player1;
        } else {
            currentPlayer = currentPlayer == player1 ? player2 : player1;
        }
        if (currentPlayer == GameManager.GetLocal()) {
            GameManager.GetLocal().StartTurn();
        }
    }

    public static int GetLocalActorNumber() {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public static int GetRemoteActorNumber() {
        return 3 - PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public static PlayerController GetLocal() {
        return GameManager.instance.players[GameManager.GetLocalActorNumber() - 1];
    }

    public static PlayerController GetRemote() {
        return GameManager.instance.players[GameManager.GetRemoteActorNumber() - 1];
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
        player1.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(1));
        player2.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(2));
        photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
    }

    void TransferOwnerships() {
        player1.photonView.TransferOwnership(1);
        player2.photonView.TransferOwnership(2);
    }

    public void CheckWinCondition() {
        if (GetLocal().GetCurrentHP() == 0) {
            photonView.RPC("WinGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void WinGame() {
        isGameEnded = true;
        if (GetLocal().GetCurrentHP() != 0) {
            GameUI.instance.SetWinText("You win!");
        } else {
            GameUI.instance.SetWinText("You lose!");
        }
        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu() {
        GameManager.isGameEnded = false;
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.CreateScene("Menu");
    }
}