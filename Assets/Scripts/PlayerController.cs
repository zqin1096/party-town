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

    public GameObject deck;
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

        this.SetState(player.ActorNumber);
    }

    public bool HasTurn() {
        return Int64.Parse(this.state["turn"]) > 0;
    }

    [PunRPC]
    public void TakeEffect(int effectDoerNumber, int idxOnDeck) {
        Debug.LogFormat(
            "PlayerContainer.TakeEffect(): ActorNumber: {0}, idxOnDeck: {1}",
            this.player.ActorNumber,
            idxOnDeck
        );

        CardContainer cardContainer = null;
        foreach (Transform child in GameManager.GetPlayer(effectDoerNumber).deck.transform) {
            CardContainer cc = child.GetComponent<CardContainer>();
            if (child.GetComponent<CardContainer>().idxOnDeck == idxOnDeck) {
                cardContainer = cc;
                break;
            }
        }

        if (cardContainer != null) {
            cardContainer.card.Effect(this, GameManager.GetRemote(), effectDoerNumber);
            this.SetState(effectDoerNumber);
        }
    }

    [PunRPC]
    public void UpdateState(
        Dictionary<String, String> state,
        Dictionary<String, String> remoteState,
        bool isMine,
        int effectDoerNumber
    ) {
        string stateInString = "";
        foreach (KeyValuePair<string, string> entry in state) {
            stateInString += entry.Key + ": " + entry.Value + ", ";
        }

        string remoteStateInString = "";
        if (remoteState != null) {
            foreach (KeyValuePair<string, string> entry in remoteState) {
                remoteStateInString += entry.Key + ": " + entry.Value + ", ";
            }
        }

        Debug.LogFormat(
            "PlayerController.UpdateState(), effectDoerNumber: {0}, isMine: {1}, state: {2}, remoteState: {3}",
            effectDoerNumber,
            isMine,
            stateInString,
            remoteStateInString
        );

        if (isMine) {
            localHP.text = state["hp"].ToString();
        } else {
            remoteHP.text = state["hp"].ToString();
        }

        int actorNumber = -1;
        if (Int64.Parse(state["turn"]) > 0) {
            actorNumber = isMine ? this.player.ActorNumber : (this.player.ActorNumber % 2) + 1;
        } else if (Int64.Parse(remoteState["turn"]) > 0) {
            actorNumber = isMine ? (this.player.ActorNumber % 2) + 1 : this.player.ActorNumber;
        }
        turn.text = actorNumber.ToString();
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

            card.transform.SetParent(deck.transform, false);
            card.GetPhotonView().RPC("Initialize", RpcTarget.Others, i, false);
            card.GetPhotonView().RPC("Initialize", player, i, true);
        }
    }

    void SetState(int effectDoerNumber) {
        Dictionary<String, String> remoteState = GameManager.GetRemote().state;

        photonView.RPC(
            "UpdateState",
            RpcTarget.Others,
            this.state,
            remoteState,
            false,
            effectDoerNumber
        );

        photonView.RPC(
            "UpdateState",
            player,
            this.state,
            remoteState,
            true,
            effectDoerNumber
        );

        // Dictionary<String, String> remoteState = GameManager.GetRemote().state;
        // if (remoteState != null) {
        //     photonView.RPC("UpdateState", RpcTarget.Others, remoteState, true, effectDoerNumber);
        //     photonView.RPC("UpdateState", player, remoteState, false, effectDoerNumber);
        // }
    }
}
