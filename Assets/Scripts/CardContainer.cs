using System;
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

    [PunRPC]
    void Initialize(bool isMine) {
        this.card = CreateRandomCard();

        Debug.LogFormat(
            "CardContainer.Initialize(): creating card, ActorNumber: {0}, isMine: {1}, cardNo: {2}, cardLabel: {3}",
            PhotonNetwork.LocalPlayer.ActorNumber,
            isMine,
            this.card.no,
            this.card.label
        );

        // Check if this card belongs to the local player or the remote player.
        if (isMine) {
            GameManager.instance.GetLocal().cards.Add(this);
            //gameObject.transform.SetParent(Deck.transform, false);
        } else {
            // GameManager.instance.GetOtherPlayer(PlayerController.local).cards.Add(this);
            GameManager.instance.GetRemote().cards.Add(this);
            //gameObject.transform.SetParent(Enemy.transform, false);
        }
    }

    public Card CreateRandomCard() {
        int cardNo = UnityEngine.Random.Range(0, 2);
        return CardMap.GetCardInstance(cardNo.ToString());
    }

    public void DoEffect() {
        Debug.LogFormat(
            "CardContainer.DoEffect(), ActorNumber: {0}, cardLabel: {1}, effectType: {2}, localActorNo: {3}, remoteActorNo: {4}",
            PhotonNetwork.LocalPlayer.ActorNumber,
            this.card.label,
            this.card.effectType,
            GameManager.instance.GetLocal().player.ActorNumber,
            GameManager.instance.GetRemote().player.ActorNumber
        );

        if (this.card.effectType == EffectType.Enemy) {
            GameManager.instance.GetRemote().photonView.RPC(
                "TakeEffect",
                GameManager.instance.GetRemote().player,
                this.card.no
            );
        } else if (this.card.effectType == EffectType.Self) {
            GameManager.instance.GetLocal().photonView.RPC(
                "TakeEffect",
                GameManager.instance.GetLocal().player,
                this.card.no
            );
        }
    }
}
