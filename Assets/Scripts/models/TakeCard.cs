using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCard : Card {
    public TakeCard() : base() {
        this.no = "3";
        this.number = Card.generateNumber();
        this.label = "Take Card";
        this.desc = "Take one card from the opponent";
        this.type = "Utility Card";
        this.cardSprite = Resources.Load<Sprite>("Knight");
        this.effectType = EffectType.Enemy;
    }
    public override bool CanSelect() {
        if (GameManager.isGameEnded) {
            return false;
        }
        // This is used in discard mode
        if (GameManager.GetLocal().discardMode == true && GameManager.GetLocal() == GameManager.instance.currentPlayer &&
                (GameManager.GetLocal().discardLabels == null || Array.IndexOf(GameManager.GetLocal().discardLabels, this.label) > -1)) {
            return true;
        }
        if (GameManager.GetLocal() == GameManager.instance.currentPlayer && !GameManager.GetLocal().GetIsWaitingResponse() && GameManager.GetRemote().numOfcards != 0  && !GameManager.GetLocal().isFrozen) {
            return true;
        }
        return false;
    }
    public override void PlayCard() {
        GameManager.GetRemote().photonView.RPC("RmoveCard", GameManager.GetRemote().player);
    }
}
