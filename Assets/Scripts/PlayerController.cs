using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

// The photon view ID of player1 is 2, and the photon view ID of player2 is 3.
// The photon view ID of the network manager is 1.
public class PlayerController : MonoBehaviourPun {
    public Player player;
    public List<CardContainer> cards = new List<CardContainer>();

    public GameObject Deck;
    public Text localHP;
    public Text remoteHP;
    public Text messageBox;
    public Text turn;
    public Dictionary<String, String> state;

    [PunRPC]
    void Initialize(Player player, Dictionary<String, String> playerState) {
        Debug.LogFormat(
            "PlayerController.Initialize(): IsLocal: {0}, IsMasterClient: {1}, NickName: {2}, ActorNumber: {3}",
            player.IsLocal,
            PhotonNetwork.IsMasterClient,
            player.NickName,
            player.ActorNumber
        );

        this.player = player;
        this.state = playerState;

        if (player.IsLocal) {
            DrawCards(4);
        }

        this.SetState(false);
    }

    public bool HasTurn() {
        return Int64.Parse(this.state["turn"]) > 0;
    }

    [PunRPC]
    public void TakeEffect(string cardNo) {
        Debug.LogFormat(
            "PlayerContainer.TakeEffect(): ActorNumber: {0}, cardNo: {1}",
            this.player.ActorNumber,
            cardNo
        );

        Action<PlayerController, PlayerController> effect = CardMap.GetCardEffect(cardNo);
        effect(GameManager.GetLocal(), GameManager.GetRemote());

        this.SetState(true);
    }

    [PunRPC]
    public void UpdateState(Dictionary<String, String> state, bool isMine) {
        Debug.LogFormat("PlayerController.UpdateState(), isMine: {0}", isMine);

        if (!isMine) {
            remoteHP.text = state["hp"].ToString();
        } else {
            localHP.text = state["hp"].ToString();
        }

        if (Int64.Parse(state["turn"]) > 0) {
            turn.text = this.player.ActorNumber.ToString();
        }
    }

    void DrawCards(int numOfCards) {
        Debug.LogFormat(
            "PlayerController.DrawCards, ActorNumber: {0}, numOfCards: {1}",
            this.player.ActorNumber,
            numOfCards
        );

        for (int i = 0; i < numOfCards; i++) {
            GameObject card = PhotonNetwork.Instantiate(
                "CardContainer",
                new Vector3(0, 0, 0),
                Quaternion.identity
            );

            card.transform.SetParent(Deck.transform, false);
            card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
            card.GetPhotonView().RPC("Initialize", player, true);
        }
    }

    void SetState(bool shouldUpdateRemoteState) {
        photonView.RPC("UpdateState", RpcTarget.Others, this.state, false);
        photonView.RPC("UpdateState", player, this.state, true);

        if (shouldUpdateRemoteState) {
            Dictionary<String, String> remoteState = GameManager.GetRemote().state;
            photonView.RPC("UpdateState", RpcTarget.Others, remoteState, true);
            photonView.RPC("UpdateState", player, remoteState, false);
        }
    }
}
