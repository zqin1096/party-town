using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : Card {
    public SpecialAttack() : base() {
        this.no = "4";
        this.number = Card.generateNumber();
        this.label = "Special Attack";
        this.desc = "This attacks your enemy hard";
        this.type = "Utility Card";
        this.cardSprite = Resources.Load<Sprite>("Cloud");
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
        // An Attack card can be selected when the local player has the turn and is not waiting for response.
        if (GameManager.GetLocal() == GameManager.instance.currentPlayer && !GameManager.GetLocal().GetIsWaitingResponse() && GameManager.GetLocal().numberOfAttack < 1 && !GameManager.GetLocal().isFrozen) {
            return true;
        }
        return false;
    }

    public override void PlayCard() {
        GameManager.GetLocal().numberOfAttack++;
        // When a players uses the Attack card, this player is waiting for response.
        GameManager.GetLocal().SetIsWaitingResponse(true);
        // Inform the remote player that you are playing an "Attack" card and need a "Defense" card for response.
        GameManager.GetRemote().photonView.RPC("GetRequest", GameManager.GetRemote().player, this.label, "Defense");
    }
}
