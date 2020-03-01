using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CardContainer : MonoBehaviourPun {
    public Card card;
    public Image suitImage;
    public int idxOnDeck;

    [PunRPC]
    void Initialize(int idxOnDeck, bool isMine) {
        this.card = CreateRandomCard();

        Debug.LogFormat(
            "CardContainer.Initialize(): creating card, ActorNumber: {0}, isMine: {1}, idxOnDeck: {2}, cardNo: {3}, cardLabel: {4}",
            PhotonNetwork.LocalPlayer.ActorNumber,
            isMine,
            idxOnDeck,
            this.card.no,
            this.card.label
        );

        this.idxOnDeck = idxOnDeck;

        // Check if this card belongs to the local player or the remote player.
        if (isMine) {
            GameManager.GetLocal().cards.Add(this);
            //gameObject.transform.SetParent(Deck.transform, false);
        } else {
            // GameManager.instance.GetOtherPlayer(PlayerController.local).cards.Add(this);
            GameManager.GetRemote().cards.Add(this);
            //gameObject.transform.SetParent(Enemy.transform, false);
        }
    }

    public Card CreateRandomCard() {
        int cardNo = UnityEngine.Random.Range(0, 2);
        return CardMap.GetCardInstance(cardNo.ToString());
    }

    public void DoEffect() {
        Debug.LogFormat(
            "CardContainer.DoEffect(), HasTurn: {0}, idxOnDeck: {1}, cardLabel: {2}, effectType: {3}, localActorNo: {4}, remoteActorNo: {5}",
            GameManager.GetLocal().HasTurn(),
            this.idxOnDeck,
            this.card.label,
            this.card.effectType,
            GameManager.GetLocal().player.ActorNumber,
            GameManager.GetRemote().player.ActorNumber
        );

        if (GameManager.GetLocal().HasTurn()) {
            if (this.card.effectType == EffectType.Enemy) {
                GameManager.GetRemote().photonView.RPC(
                    "TakeEffect",
                    GameManager.GetRemote().player,
                    GameManager.GetLocal().player.ActorNumber,
                    idxOnDeck
                );
            } else if (this.card.effectType == EffectType.Self) {
                GameManager.GetLocal().photonView.RPC(
                    "TakeEffect",
                    GameManager.GetLocal().player,
                    GameManager.GetLocal().player.ActorNumber,
                    idxOnDeck
                );
            }
        }
    }
}
