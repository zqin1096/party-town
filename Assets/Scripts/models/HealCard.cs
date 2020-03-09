using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCard : Card {
    public HealCard() : base() {
        this.no = "1";
        this.label = "Heal";
        this.desc = "This heals yourself";
        this.effectType = EffectType.Self;
    }

    public override bool CanSelect() {
        if (GameManager.isGameEnded) {
            return false;
        }
        if (GameManager.GetLocal() == GameManager.instance.currentPlayer && GameManager.GetLocal().GetCurrentHP() < PlayerController.maxHP) {
            return true;
        }
        return false;
    }

    public override void PlayCard() {
        int hp = GameManager.GetLocal().GetCurrentHP() + 1;
        GameManager.GetLocal().SetCurrentHP(hp);
        GameManager.GetLocal().photonView.RPC("UpdateHealth", GameManager.GetRemote().player, hp, false);
        GameManager.GetLocal().photonView.RPC("UpdateHealth", GameManager.GetLocal().player, hp, true);
    }
}
