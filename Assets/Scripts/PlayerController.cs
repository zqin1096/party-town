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
    public GameObject deck;
    public Text localHP;
    public Text remoteHP;
    public Text messageBox;
    public Text turn;

    public Player player;
    public Dictionary<String, String> state;

    [PunRPC]
    void Initialize(Player player, Dictionary<String, String> playerState) {
        Debug.LogFormat(
            "PlayerController.Initialize(): localActorNumber: {0}, playerActorNumber: {1}, IsLocal: {2}, IsMasterClient: {3}, NickName: {4}",
            GameManager.GetLocalActorNumber(),
            player.ActorNumber,
            player.IsLocal,
            PhotonNetwork.IsMasterClient,
            player.NickName
        );

        this.player = player;
        this.state = playerState;

        if (player.IsLocal) {
            InitializeCards(4);
        }
    }

    public bool HasTurn() {
        return Int64.Parse(this.state["turn"]) > 0;
    }

    [PunRPC]
    public void UpdateState(
        Dictionary<String, String>[] states,
        int callerActorNumber
    ) {
        string statesInString = "";
        for (int i = 0; i < states.Length; i += 1) {
            statesInString += "[player: " + i + "] ";
            if (states[i] != null) {
                foreach (KeyValuePair<string, string> entry in states[i]) {
                    statesInString += entry.Key + ": " + entry.Value + ", ";
                }
            }
            statesInString += "/ ";
        }

        int localActorNumber = GameManager.GetLocalActorNumber();

        Debug.LogFormat(
            "PlayerController.UpdateState(), player: {0}, isLocal: {1}, callerActorNumber: {2}, localActorNumber: {3}, states: {4}",
            this.player,
            this.player.IsLocal,
            callerActorNumber,
            localActorNumber,
            statesInString
        );

        Dictionary<string, string> localState = states[localActorNumber - 1];
        Dictionary<string, string> remoteState = states[2 - localActorNumber];

        if (this.player.IsLocal) {
            this.state = localState;
            localHP.text = localState["hp"].ToString();
            remoteHP.text = remoteState["hp"].ToString();
            turn.text = Int64.Parse(localState["turn"]) > 0
              ? localActorNumber.ToString()
              : (3 - localActorNumber).ToString();
        } else {
            this.state = remoteState;
        }
    }

    [PunRPC]
    public void RemoveCard(
        int idxOnDeck,
        int callerActorNumber
    ) {
        Debug.LogFormat(
            "PlayerController.RemoveCard(), ActorNumber: {0}, idxOnDeck: {1}, callerActorNumber: {2}",
            this.player.ActorNumber,
            idxOnDeck,
            callerActorNumber
        );

        if (this.player.IsLocal) {
            CardContainer cardContainer = null;
            foreach (Transform child in this.deck.transform) {
                CardContainer cc = child.GetComponent<CardContainer>();
                if (child.GetComponent<CardContainer>().idxOnDeck == idxOnDeck) {
                    cardContainer = cc;
                    break;
                }
            }

            if (cardContainer != null) {
                Debug.LogFormat(
                    "PlayerController.RemoveCard, found, remove card of idx: {0}",
                    idxOnDeck
                );

                Destroy(cardContainer.gameObject);
            }

        } else {
        }
    }

    void InitializeCards(int numOfCards) {
        Debug.LogFormat(
            "PlayerController.InitializeCards, ActorNumber: {0}, numOfCards: {1}",
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
}
