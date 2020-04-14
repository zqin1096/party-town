using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillizardCard : Card {
    public BillizardCard() : base() {
        this.no = "5";
        this.number = Card.generateNumber();
        this.label = "Billizard";
        this.desc = "Froze your opponent for one round";
        this.type = "Utility Card";
        this.cardSprite = Resources.Load<Sprite>("Snowflake");
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
        if (GameManager.GetLocal() == GameManager.instance.currentPlayer && !GameManager.GetLocal().GetIsWaitingResponse() && GameManager.GetRemote().isFrozen == false && !GameManager.GetLocal().isFrozen) {
            return true;
        }
        return false;
    }
    public override void PlayCard() {
        GameManager.GetRemote().photonView.RPC("FrozePlayer", GameManager.GetRemote().player);
    }
}
