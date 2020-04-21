using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPun {
    public PlayerController player1;
    public PlayerController player2;
    // public PlayerController[] players;

    public static GameManager instance;

    public PlayerController currentPlayer;
    public float postGameTime = 30f;
    public static bool isGameEnded;

    public const byte INITIALIZE_PLAYERS_DONE_EVENT = 1;
    public const int INITIALIZE_PLAYERS_DONE = 2;
    public static int numOfPlayersInitialized = 0;

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

    //public static int GetLocalActorNumber() {
    //    return PhotonNetwork.LocalPlayer.ActorNumber;
    //}

    //public static int GetRemoteActorNumber() {
    //    return 3 - PhotonNetwork.LocalPlayer.ActorNumber;
    //}

    public static PlayerController GetLocal() {
        return GameManager.instance.player1.player.IsLocal ? GameManager.instance.player1 : GameManager.instance.player2;
    }

    public static PlayerController GetRemote() {
        return GameManager.instance.player1.player.IsLocal ? GameManager.instance.player2 : GameManager.instance.player1;
    }

    //public static PlayerController GetPlayer(int actorNumber) {
    //    return GameManager.instance.players[actorNumber - 1];
    //}

    void Awake() {
        instance = this;
    }

    void Start() {
        // this.players = new PlayerController[2] { this.player1, this.player2 };

        Debug.LogFormat("GameManager.Start(), IsMaterClient: {0}", PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient) {
            TransferOwnerships();
            InitializePlayers();
        }
    }

    void Update() {
        if (numOfPlayersInitialized == INITIALIZE_PLAYERS_DONE) {
            Debug.Log(GameManager.GetLocal().numOfcards);
            Debug.Log(GameManager.GetRemote().numOfcards);
            photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
            numOfPlayersInitialized = 0;
        }
    }

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj) {
        if (obj.Code == INITIALIZE_PLAYERS_DONE_EVENT) {
            numOfPlayersInitialized++;
        }
    }

    void InitializePlayers() {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in PhotonNetwork.PlayerList) {
            Debug.Log(player.NickName + " " + player.ActorNumber);
        }
        player1.photonView.RPC("Initialize", RpcTarget.AllBuffered, players[0].IsMasterClient ? players[0] : players[1]);
        player2.photonView.RPC("Initialize", RpcTarget.AllBuffered, players[0].IsMasterClient ? players[1] : players[0]);
        // photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
    }

    void TransferOwnerships() {
        // Maybe not necessary.
        Player[] players = PhotonNetwork.PlayerList;
        player1.photonView.TransferOwnership(players[0].IsMasterClient ? players[0] : players[1]);
        player2.photonView.TransferOwnership(players[0].IsMasterClient ? players[1] : players[0]);
    }

    public void CheckWinCondition() {
        if (GetLocal().GetCurrentHP() <= 0) {
            photonView.RPC("WinGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void WinGame() {
        isGameEnded = true;
        if (GetLocal().GetCurrentHP() > 0) {
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