using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;

public class CardContainer : MonoBehaviourPun {
    public Card card;
    public Image suitImage;
    public int idxOnDeck;
    public Text label;

    [PunRPC]
    void Initialize(int idxOnDeck, bool isMine) {
        this.card = CreateRandomCard();
        this.label.text = this.card.label;

        Debug.LogFormat(
            "CardContainer.Initialize(): creating card, ActorNumber: {0}, isMine: {1}, idxOnDeck: {2}, cardNo: {3}, cardLabel: {4}",
            PhotonNetwork.LocalPlayer.ActorNumber,
            isMine,
            idxOnDeck,
            this.card.no,
            this.card.label
        );

        this.idxOnDeck = idxOnDeck;
    }

    public Card CreateRandomCard() {
        int cardNo = UnityEngine.Random.Range(0, 3);
        return CardMap.GetCardInstance(cardNo.ToString());
    }

    public void DoEffect() {
        /*
        Debug.LogFormat(
            "CardContainer.DoEffect(), HasTurn: {0}, idxOnDeck: {1}, cardLabel: {2}, effectType: {3}, localActorNo: {4}, remoteActorNo: {5}",
            GameManager.GetLocal().HasTurn(),
            this.idxOnDeck,
            this.card.label,
            this.card.effectType,
            GameManager.GetLocal().player.ActorNumber,
            GameManager.GetRemote().player.ActorNumber
        );
        */
        Debug.Log("DoEffect() is called.");

        // Defense card should not be used
        /*
        if (this.card.no == "2") {
            return;
        }
        */


        this.card.Effect(
            GameManager.GetPlayerStates(),
            GameManager.GetLocalActorNumber()
        );

        GameManager.GetLocal().photonView.RPC(
            "UpdateState",
            RpcTarget.AllBuffered,
            GameManager.GetPlayerStates(),
            GameManager.GetLocalActorNumber()
        );

        GameManager.GetRemote().photonView.RPC(
            "UpdateState",
            RpcTarget.AllBuffered,
            GameManager.GetPlayerStates(),
            GameManager.GetLocalActorNumber()
        );

        GameManager.GetLocal().photonView.RPC(
            "RemoveCard",
            RpcTarget.AllBuffered,
            this.idxOnDeck,
            GameManager.GetLocalActorNumber()
        );
    }

    void Update() {
        if (GameManager.instance.currentPlayer == GameManager.GetLocal() && !this.card.isPassive) {
            gameObject.GetComponent<EventTrigger>().enabled = true;
            changeAlpha(true);
        } else {
            gameObject.GetComponent<EventTrigger>().enabled = false;
            changeAlpha(false);
        }
    }

    private void changeAlpha(bool canBePlayed) {
        Color color = gameObject.GetComponent<Image>().color;
        if (canBePlayed) {
            color.a = 1f;
        } else {
            color.a = 0.45f;
        }
        gameObject.GetComponent<Image>().color = color;
    }

    public void ToggleSelect() {
        Debug.Log("Toggle select card.");
        if (GameManager.GetLocal().getSelectedCard() == null) {
            transform.position = new Vector2(transform.position.x, transform.position.y + 5);
            GameManager.GetLocal().setSelectedCard(this);
            GameUI.instance.TogglePlayButton(true);
        } else {
            if (GameManager.GetLocal().getSelectedCard() == this) {
                transform.position = new Vector2(transform.position.x, transform.position.y - 5);
                GameManager.GetLocal().setSelectedCard(null);
                GameUI.instance.TogglePlayButton(false);
            } else {
                GameManager.GetLocal().getSelectedCard().transform.position =
                    new Vector2(GameManager.GetLocal().getSelectedCard().transform.position.x,
                               GameManager.GetLocal().getSelectedCard().transform.position.y - 5);

                transform.position = new Vector2(transform.position.x, transform.position.y + 5);
                GameManager.GetLocal().setSelectedCard(this);
            }
        }
        Debug.Log("Current player: " + GameManager.instance.currentPlayer.player.NickName);
        if (GameManager.instance.currentPlayer.getSelectedCard() != null) {
            Debug.Log("Current card: " + GameManager.instance.currentPlayer.getSelectedCard().card.label);
        }
    }

}
