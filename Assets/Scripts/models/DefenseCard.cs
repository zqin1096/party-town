using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCard : Card {

    public DefenseCard() : base() {
        this.no = "2";
        this.label = "Defense";
        this.desc = "This voids an attack card";
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
        // A Defense card can be selected when the local player receives a request of the Defense card.
        if (GameManager.GetLocal().GetIsGettingRequest() && GameManager.GetLocal().getRequestedCard() == this.label) {
            return true;
        }
        return false;
    }
}