using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CardContainer : MonoBehaviourPun {
    public Card card;
    public Text cardText;
    public Image suitImage;
    public Text nameText;
    public Text descriptionText;
    public GameObject Deck;
    public GameObject Enemy;

    void Start() {
        Debug.Log("CardContainer.Start()");

        this.card = CreateRandomCard();
        Debug.LogFormat(
            "CardContainer.Start(): created card, no: {0}, label: {1}",
            card.no,
            card.label
        );
    }

    [PunRPC]
    void Initialize(bool isMine) {
        Debug.LogFormat("CardContainer.Initialize(), isMine: {0}", isMine);

        // Check if this card belongs to the local player or the remote player.
        if (isMine) {
            PlayerController.local.cards.Add(this);
            //gameObject.transform.SetParent(Deck.transform, false);
        } else {
            GameManager.instance.GetOtherPlayer(PlayerController.local).cards.Add(this);
            //gameObject.transform.SetParent(Enemy.transform, false);
        }
    }

    public Card CreateRandomCard() {
        int cardType = UnityEngine.Random.Range(0, 2);

        switch (cardType) {
            case 0:
                return new AttackCard();
            case 1:
                return new HealCard();
            default:
                return new AttackCard();
        }
    }

    public void DoEffect() {
        Debug.LogFormat("CardContainer.DoEffect(), effectType: {0}", this.card.effectType);
        if (this.card.effectType == EffectType.Enemy) {
            PlayerController.remote.photonView.RPC(
                "TakeEffect",
                PlayerController.remote.photonPlayer,
                this.card.no
            );
        } else if (this.card.effectType == EffectType.Self) {
            // self effect
        }
    }
}
