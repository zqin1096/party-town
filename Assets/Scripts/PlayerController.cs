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
    public static PlayerController local;
    public static PlayerController remote;

    // Summarizes a "player" within a room, identified (in that room) by ID.
    public Player photonPlayer;
    public List<CardContainer> cards = new List<CardContainer>();

    public string displayName;
    public GameObject Deck;
    public Text localHP;
    public Text remoteHP;
    public Dictionary<String, String> state;

    [PunRPC]
    void Initialize(Player player) {
        Debug.LogFormat("PlayerController.Initialize(): player.IsLocal: {0}, {1}", player.IsLocal, this.localHP.text);

        this.photonPlayer = player;
        this.state = new Dictionary<String, String>();
        state.Add("hp", "5");
        state.Add("turn", "1");

        if (player.IsLocal) {
            PlayerController.local = this;
            this.displayName = "local-1";
            this.localHP.text += this.state["hp"];
            DrawCards(4);
        } else {
            PlayerController.remote = this;
            this.displayName = "remote-1";
            this.remoteHP.text += this.state["hp"];
        }
    }

    void DrawCards(int num) {
        Debug.LogFormat("PlayerController.DrawCards, num: {0}", num);

        for (var i = 0; i < num; i++) {
            GameObject card = PhotonNetwork.Instantiate(
                "CardContainer",
                new Vector3(0, 0, 0),
                Quaternion.identity
            );
            card.transform.SetParent(Deck.transform, false);
            card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
            card.GetPhotonView().RPC("Initialize", photonPlayer, true);
        }
    }

    [PunRPC]
    public void TakeEffect(string cardNo) {
        Debug.LogFormat("PlayerContainer.TakeEffect(): cardNo: {0}", cardNo);

        switch (cardNo) {
            case "0":
                Action<PlayerController> effect = AttackCard.Effect();
                effect(this);
                break;
            default:
                break;
        }

        photonView.RPC("UpdateState", RpcTarget.Others, this.state, false);
        photonView.RPC("UpdateState", photonPlayer, this.state, true);
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
