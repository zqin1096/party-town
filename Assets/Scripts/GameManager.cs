using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    public PlayerController player1;
    public PlayerController player2;

    public PlayerController currentPlayer;

    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    void Start() {
        Debug.LogFormat("GameManager.Start()");
        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("GameManager.Start(): Master, setting the players");
            SetPlayers();
        }
    }

    void SetPlayers() {
        Debug.LogFormat("GameManager.SetPlayers()");
        player1.photonView.TransferOwnership(1);
        player2.photonView.TransferOwnership(2);

        player1.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(1));
        player2.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(2));
    }

    // [PunRPC]
    // void SetNextTurn() {
    //     if (currentPlayer == null) {
    //         currentPlayer = player1;
    //     } else {
    //         currentPlayer = currentPlayer == player1 ? player2 : player1;
    //     }
    // }

    public PlayerController GetOtherPlayer(PlayerController player) {
        return player == player1 ? player2 : player1;
    }
}
