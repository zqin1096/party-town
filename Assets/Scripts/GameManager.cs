using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

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

    public const byte SET_PLAYER_EVENT = 4;
    public const int SET_PLAYER_EVENT_DONE = 2;
    public static int numOfPlayersSet = 0;

    public GameObject playWindow;
    public GameObject settingWindow;

    public Image localCharacter;
    public Image remoteCharacter;

    public GameObject waitMessage;

    public GameObject textLog;

    [PunRPC]
    void SetNextTurn() {
        if (currentPlayer == null) {
            currentPlayer = player1;
        } else {
            currentPlayer = currentPlayer == player1 ? player2 : player1;
        }
        if (currentPlayer == GameManager.GetLocal()) {
            GameManager.GetLocal().StartTurn();
        } else {
            GameManager.GetLocal().SetPromptText("Wait your opponent to play a card");
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

    void WaitOne() {
        photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void SetSelection() {
        settingWindow.gameObject.SetActive(true);
    }

    void WaitTwo() {
        photonView.RPC("SetSelection", RpcTarget.AllBuffered);
    }

    void Update() {
        if (numOfPlayersSet == SET_PLAYER_EVENT_DONE) {
            Invoke("WaitTwo", 0.5f);
            numOfPlayersSet = 0;
        }
        if (numOfPlayersInitialized == INITIALIZE_PLAYERS_DONE) {
            Debug.Log(GameManager.GetLocal().numOfcards);
            Debug.Log(GameManager.GetRemote().numOfcards);
            numOfPlayersInitialized = 0;
            Invoke("WaitOne", 2f);
            // photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
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
        } else if (obj.Code == SET_PLAYER_EVENT) {
            numOfPlayersSet++;
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
        textLog.gameObject.SetActive(false);
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

    [PunRPC]
    public void SetupTable() {
        waitMessage.gameObject.SetActive(false);
        localCharacter.sprite = GameManager.GetLocal().character.characterSprite;
        remoteCharacter.sprite = GameManager.GetLocal().remoteCharacter.characterSprite;
        GameManager.GetLocal().InitCardWithAnimation(4);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(INITIALIZE_PLAYERS_DONE_EVENT, null, raiseEventOptions, sendOptions);
    }

    [PunRPC]
    public void SetRemoteCharacter(string name) {
        Debug.Log(name);
        switch (name) {
            case "Amphius":
                GameManager.GetLocal().remoteCharacter = new CharacterA();
                break;
            case "Elatus":
                GameManager.GetLocal().remoteCharacter = new CharacterB();
                break;
            case "Pyris":
                GameManager.GetLocal().remoteCharacter = new CharacterC();
                break;
        }
    }
}