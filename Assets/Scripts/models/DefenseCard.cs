using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCard : Card {

    public DefenseCard() : base() {
        this.no = "2";
        this.number = Card.generateNumber();
        this.label = "Defense";
        this.desc = "Avoid the effect of Attack";
        this.type = "Basic Card";
        this.cardSprite = Resources.Load<Sprite>("Guard");
        this.effectType = EffectType.Self;
    }

    public override void PlayCard() {
        // Only used to respond to the attack.
        throw new System.NotImplementedException();
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
        // A Defense card can be selected when the local player receives a request of the Defense card.
        if (GameManager.GetLocal().GetIsGettingRequest() && GameManager.GetLocal().getRequestedCard() == this.label && !GameManager.GetLocal().isFrozen) {
            return true;
        }
        return false;
    }
}