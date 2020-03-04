using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPun {
    public GameObject deck;
    public Text localHP;
    public Text remoteHP;
    public Text messageBox;
    public Text username;
    public Text remoteUsername;
    public Text turn;

    public Player player;
    public Dictionary<String, String> state;

    private CardContainer selectedCard;

    public CardContainer getSelectedCard() {
        return selectedCard;
    }

    public void setSelectedCard(CardContainer card) {
        selectedCard = card;
    }

    public void EndTurn() {
        if (selectedCard != null) {
            selectedCard.transform.position = new Vector2(selectedCard.transform.position.x, selectedCard.transform.position.y - 5);
            selectedCard = null;
        }
        GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
    }

    public void StartTurn() {
        // Draw 2 more cards.
    }

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
            this.username.text = player.NickName;
            this.remoteUsername.text = player.GetNext().NickName;
            InitializeCards(6);
        } else {
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
        int originHP = Int16.Parse(GameManager.GetLocal().state["hp"]);

        Dictionary<string, string> localState = states[localActorNumber - 1];
        Dictionary<string, string> remoteState = states[2 - localActorNumber];

        if (this.player.IsLocal) {
            this.state = localState;
            localHP.text = localState["hp"].ToString();
            remoteHP.text = remoteState["hp"].ToString();
            turn.text = Int64.Parse(localState["turn"]) > 0
              ? GameManager.GetLocal().player.NickName
              : GameManager.GetRemote().player.NickName;

            EndGameIfNecessary(localState, remoteState);
        } else {
            this.state = remoteState;
        }

        int currentHP = Int16.Parse(localState["hp"]);
        if(originHP - currentHP == 1 && this.player.IsLocal)
        {
            int defenseCardIdx = -1;
            foreach (Transform child in this.deck.transform)
            {
                CardContainer cc = child.GetComponent<CardContainer>();
                if (child.GetComponent<CardContainer>().card.no == "2")
                {
                    defenseCardIdx = child.GetComponent<CardContainer>().idxOnDeck;
                    break;
                }
            }
            if (defenseCardIdx != -1)
            {
                Debug.LogFormat("A defense card can be used! Index: {0}", defenseCardIdx);

                this.RemoveCard(defenseCardIdx, 0);
            }
        }
        if(localState["hp"]=="0" || remoteState["hp"] == "0")
        {
            Debug.LogFormat("Game Ends!");
            SceneManager.LoadScene("End");
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

    [PunRPC]
    public void DeclareGameEnd(
        int winnerActorNumber,
        int callerActorNumber
    ) {
        Debug.LogFormat(
            "PlayerController.DeclareGameEnd(), winnerActorNumber: {0}, callerActorNumber: {1}",
            winnerActorNumber,
            callerActorNumber
        );

        if (this.player.IsLocal) {
            if (winnerActorNumber == 0) {
                this.messageBox.text = "Draw";
            } else if (this.player.ActorNumber == winnerActorNumber) {
                this.messageBox.text = "Player " + this.player.NickName + " has won!";
            } else {
                this.messageBox.text = "Player " + GameManager.GetRemote().player.NickName + " has won!";
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

    void EndGameIfNecessary(
        Dictionary<string, string> localState,
        Dictionary<string, string> remoteState
    ) {
        int localHPInt = (int)Int64.Parse(localState["hp"]);
        int remoteHPInt = (int)Int64.Parse(remoteState["hp"]);

        if (localHPInt < 1) {
            if (remoteHPInt > 0) {
                this.photonView.RPC(
                    "DeclareGameEnd",
                    RpcTarget.AllBuffered,
                    GameManager.GetLocalActorNumber(),
                    GameManager.GetLocalActorNumber()
                );
            } else if (remoteHPInt < 1) {
                this.photonView.RPC(
                    "DeclareGameEnd",
                    RpcTarget.AllBuffered,
                    0,
                    GameManager.GetLocalActorNumber()
                );
            }
        }
    }
}
