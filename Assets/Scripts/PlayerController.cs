using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun {
    public GameObject deck;
    public Text localHP;
    public Text remoteHP;
    public Text messageBox;
    public Text username;
    public Text remoteUsername;

    public Player player;

    private CardContainer selectedCard;
    private bool isWaitingResponse;
    private bool isGettingRequest;
    public static int maxHP = 2;
    private int currentHP;
    private string enemyCard;
    private string requestedCard;

    public string GetEnemyCard() {
        return this.enemyCard;
    }

    public int GetCurrentHP() {
        return this.currentHP;
    }

    public void SetCurrentHP(int hp) {
        this.currentHP = hp;
    }

    public void SetIsWaitingResponse(bool value) {
        this.isWaitingResponse = value;
    }

    public bool GetIsWaitingResponse() {
        return this.isWaitingResponse;
    }

    public void SetIsGettingRequest(bool value) {
        this.isGettingRequest = value;
    }

    public bool GetIsGettingRequest() {
        return this.isGettingRequest;
    }

    public void SetRequestedCard(string requestedCard) {
        this.requestedCard = requestedCard;
    }

    public string getRequestedCard() {
        return this.requestedCard;
    }

    public void setSelectedCard(CardContainer card) {
        selectedCard = card;
    }

    public CardContainer getSelectedCard() {
        return this.selectedCard;
    }

    public void EndTurn() {
        if (selectedCard != null) {
            selectedCard.transform.position = new Vector2(selectedCard.transform.position.x, selectedCard.transform.position.y - 5);
            selectedCard = null;
        }
        GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
    }

    public void StartTurn() {
        InitializeCards(2);
    }

    [PunRPC]
    void Initialize(Player player) {
        this.currentHP = maxHP;
        this.player = player;
        if (player.IsLocal) {
            this.username.text = player.NickName;
            this.remoteUsername.text = player.GetNext().NickName;
            InitializeCards(4);
        } else {
        }
    }

    [PunRPC]
    public void GetRequest(string enemyCard, string requestedCard) {
        this.isGettingRequest = true;
        this.enemyCard = enemyCard;
        this.requestedCard = requestedCard;
    }

    public void SendResponse(bool response) {
        this.isGettingRequest = false;
        if (response) {
            switch (this.enemyCard) {
                case "Attack":
                    break;
                default:
                    break;
            }
        } else {
            switch (this.enemyCard) {
                case "Attack":
                    this.currentHP--;
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", RpcTarget.Others, currentHP, false);
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", player, currentHP, true);
                    GameManager.instance.CheckWinCondition();
                    break;
                default:
                    break;
            }
        }
        this.enemyCard = null;
        this.requestedCard = null;
        GameManager.GetRemote().photonView.RPC("GetResponse", GameManager.GetRemote().player);
    }

    [PunRPC]
    void UpdateHealth(int hp, bool isMine) {
        if (!isMine) {
            remoteHP.text = hp.ToString();
        } else {
            localHP.text = hp.ToString();
        }
    }

    [PunRPC]
    public void GetResponse() {
        this.isWaitingResponse = false;
    }

    void InitializeCards(int numOfCards) {
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