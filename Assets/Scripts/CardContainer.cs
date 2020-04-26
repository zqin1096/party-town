using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CardContainer : MonoBehaviour {
    public Card card;
    // public Image suitImage;
    public Text number;
    public Text label;
    public Text description;
    public Text typeText;
    public Image cardImage;
    public static readonly int SelectedCardYOffset = 15;

    public const byte USE_CARD_EVENT = 3;

    public void InitializeCard(string label, string number) {
        if (label == null) {
            this.card = CreateRandomCard();
            this.label.text = this.card.label;
            this.number.text = this.card.number;
            this.description.text = this.card.desc;
            this.typeText.text = this.card.type;
            this.cardImage.sprite = this.card.cardSprite;
        } else {
            switch (label) {
                case "Attack":
                    this.card = new AttackCard();
                    break;
                case "Defense":
                    this.card = new DefenseCard();
                    break;
                case "Heal":
                    this.card = new HealCard();
                    break;
                case "Take Card":
                    this.card = new TakeCard();
                    break;
                case "Special Attack":
                    this.card = new SpecialAttack();
                    break;
                case "Billizard":
                    this.card = new BillizardCard();
                    break;
                default:
                    break;
            }
            if (number != null) {
                this.card.number = number;
            }
            this.label.text = this.card.label;
            this.number.text = this.card.number;
            this.description.text = this.card.desc;
            this.typeText.text = this.card.type;
            this.cardImage.sprite = this.card.cardSprite;
        }
    }

    public Card CreateRandomCard() {
        int randomValue = UnityEngine.Random.Range(0, 110);
        string cardNo = "";
        if (0 <= randomValue && randomValue < 42) {
            cardNo = "0";
        } else if (42 <= randomValue && randomValue < 52) {
            cardNo = "1";
        } else if (52 <= randomValue && randomValue < 68) {
            cardNo = "2";
        } else if (68 <= randomValue && randomValue < 90) {
            cardNo = "3";
        } else if (90 <= randomValue && randomValue < 100) {
            cardNo = "4";
        } else if (100 <= randomValue && randomValue < 110) {
            cardNo = "5";
        }
        return CardMap.GetCardInstance(cardNo);
    }

    // Only the current player can call this method.
    public void DoEffect() {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        object[] data = new object[] { this.card.label, this.card.number };
        PhotonNetwork.RaiseEvent(USE_CARD_EVENT, data, raiseEventOptions, sendOptions);
        this.card.used = true;
        string color = GameManager.GetLocal().player.IsMasterClient ? "red" : "green";
        GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.instance.currentPlayer.player.NickName + " uses " + GameManager.GetLocal().getSelectedCard().card.label + ".", color);
        this.card.PlayCard();
        GameManager.GetLocal().MoveSelectedCard();
        GameManager.GetLocal().photonView.RPC("UseCard", GameManager.GetRemote().player, false);
        GameManager.GetLocal().photonView.RPC("UseCard", GameManager.GetLocal().player, true);
        // Destroy(this.gameObject);
        GameManager.GetLocal().setSelectedCard(null);
    }

    public void DoResponse() {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        object[] data = new object[] { this.card.label, this.card.number };
        PhotonNetwork.RaiseEvent(USE_CARD_EVENT, data, raiseEventOptions, sendOptions);
        this.card.used = true;
        if (this.card.label == "Defense") {
            SoundManager.PlaySound("defense");
        }
        string nickname = GameManager.GetLocal().player.NickName;
        string color = GameManager.GetLocal().player.IsMasterClient ? "red" : "green";
        GameManager.instance.photonView.RPC("LogText", RpcTarget.All, nickname + " responses with " + GameManager.GetLocal().getSelectedCard().card.label + ".", color);
        GameManager.GetLocal().MoveSelectedCard();
        GameManager.GetLocal().photonView.RPC("UseCard", GameManager.GetRemote().player, false);
        GameManager.GetLocal().photonView.RPC("UseCard", GameManager.GetLocal().player, true);
        // Destroy(this.gameObject);
        GameManager.GetLocal().setSelectedCard(null);
    }

    void Update() {
        if (this.card != null && this.card.CanSelect()) {
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
        //if (canBePlayed) {
        //    gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        //} else {
        //    gameObject.GetComponent<CanvasGroup>().alpha = 0.45f;
        //}
    }

    public void ToggleSelect() {

        // Discard related code
        if (GameManager.GetLocal().discardMode == true) {
            if (GameManager.GetLocal().discardBucket.Contains(this)) {
                GameManager.GetLocal().discardBucket.Remove(this);
                transform.position = new Vector2(transform.position.x, transform.position.y - SelectedCardYOffset);
            } else {
                transform.position = new Vector2(transform.position.x, transform.position.y + SelectedCardYOffset);
                GameManager.GetLocal().discardBucket.Add(this);
                if (GameManager.GetLocal().discardBucket.Count == GameManager.GetLocal().discardNum) {
                    GameManager.GetLocal().SetPromptText("");
                    GameManager.GetLocal().discardMode = false;
                    foreach (CardContainer card in GameManager.GetLocal().discardBucket) {
                        GameManager.GetLocal().photonView.RPC("UseCard", GameManager.GetRemote().player, false);
                        GameManager.GetLocal().photonView.RPC("UseCard", GameManager.GetLocal().player, true);
                        Destroy(card.gameObject);
                    }
                    GameManager.GetLocal().discardBucket.Clear();
                    GameManager.GetLocal().discardCallback();
                }
            }
            return;
        }
        if (GameManager.GetLocal().getSelectedCard() == null) {
            transform.position = new Vector2(transform.position.x, transform.position.y + SelectedCardYOffset);
            GameManager.GetLocal().setSelectedCard(this);
        } else {
            if (GameManager.GetLocal().getSelectedCard() == this) {
                transform.position = new Vector2(transform.position.x, transform.position.y - SelectedCardYOffset);
                GameManager.GetLocal().setSelectedCard(null);
            } else {
                GameManager.GetLocal().getSelectedCard().transform.position =
                    new Vector2(GameManager.GetLocal().getSelectedCard().transform.position.x,
                               GameManager.GetLocal().getSelectedCard().transform.position.y - SelectedCardYOffset);

                transform.position = new Vector2(transform.position.x, transform.position.y + SelectedCardYOffset);
                GameManager.GetLocal().setSelectedCard(this);
            }
        }
        Debug.Log("Current player: " + GameManager.instance.currentPlayer.player.NickName);
        if (GameManager.instance.currentPlayer.getSelectedCard() != null) {
            Debug.Log("Current card: " + GameManager.instance.currentPlayer.getSelectedCard().card.label);
        }
    }
}