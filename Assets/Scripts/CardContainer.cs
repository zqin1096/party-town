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
        this.idxOnDeck = idxOnDeck;
    }

    public Card CreateRandomCard() {
        int cardNo = UnityEngine.Random.Range(0, 3);
        return CardMap.GetCardInstance(cardNo.ToString());
    }

    // Only the current player can call this method.
    public void DoEffect() {
        this.card.PlayCard();
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.GetLocal().setSelectedCard(null);
    }

    public void DoResponse() {
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.GetLocal().setSelectedCard(null);
    }

    void Update() {
        if (this.card.CanSelect()) {
            gameObject.GetComponent<EventTrigger>().enabled = true;
            ChangeAlpha(true);
        } else {
            gameObject.GetComponent<EventTrigger>().enabled = false;
            ChangeAlpha(false);
        }
    }

    private void ChangeAlpha(bool canBePlayed) {
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
            // GameUI.instance.TogglePlayButton(true);
        } else {
            if (GameManager.GetLocal().getSelectedCard() == this) {
                transform.position = new Vector2(transform.position.x, transform.position.y - 5);
                GameManager.GetLocal().setSelectedCard(null);
                // GameUI.instance.TogglePlayButton(false);
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
