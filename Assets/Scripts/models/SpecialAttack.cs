using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : Card {
    public SpecialAttack() : base() {
        this.no = "4";
        this.label = "Special Attack";
        this.desc = "This attacks your enemy hard";
        this.effectType = EffectType.Enemy;
    }

    public override bool CanSelect() {
        if (GameManager.isGameEnded) {
            return false;
        }
        // An Attack card can be selected when the local player has the turn and is not waiting for response.
        if (GameManager.GetLocal() == GameManager.instance.currentPlayer && !GameManager.GetLocal().GetIsWaitingResponse() && GameManager.GetLocal().numberOfAttack < 1) {
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
