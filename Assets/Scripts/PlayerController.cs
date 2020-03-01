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
    // Summarizes a "player" within a room, identified (in that room) by ID.
    public Player player;
    public List<CardContainer> cards = new List<CardContainer>();

    public GameObject Deck;
    public Text localHP;
    public Text remoteHP;
    public Dictionary<String, String> state;

    [PunRPC]
    void Initialize(Player player) {
        Debug.LogFormat(
            "PlayerController.Initialize(): IsLocal: {0}, IsMasterClient: {1}, NickName: {2}, ActorNumber: {3}",
            player.IsLocal,
            PhotonNetwork.IsMasterClient,
            player.NickName,
            player.ActorNumber
        );

        this.player = player;
        this.state = new Dictionary<String, String>();
        state.Add("hp", "5");
        state.Add("turn", "0");

        if (player.IsLocal) {
            this.localHP.text += this.state["hp"];
            this.state["turn"] = "1";
            DrawCards(4);
        } else {
            this.remoteHP.text += this.state["hp"];
        }
    }

    void DrawCards(int numOfCards) {
        Debug.LogFormat("PlayerController.DrawCards, numOfCards: {0}", numOfCards);

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

    [PunRPC]
    public void TakeEffect(string cardNo) {
        Debug.LogFormat(
            "PlayerContainer.TakeEffect(): ActorNumber: {0}, NickName: {1}, cardNo: {2}",
            this.player.ActorNumber,
            this.player.NickName,
            cardNo
        );

        Action<PlayerController> effect = CardMap.GetCardEffect(cardNo);
        effect(this);

        photonView.RPC("UpdateState", RpcTarget.Others, this.state, false);
        photonView.RPC("UpdateState", player, this.state, true);
    }

    [PunRPC]
    void UpdateState(Dictionary<String, String> state, bool isMine) {
        Debug.LogFormat("PlayerController.UpdateState(), isMine: {0}", isMine);

        if (!isMine) {
            remoteHP.text = state["hp"].ToString();
        } else {
            localHP.text = state["hp"].ToString();
        }
    }
}
