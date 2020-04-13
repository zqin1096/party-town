using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCard : Card {
    public HealCard() : base() {
        this.no = "1";
        this.number = Card.generateNumber();
        this.label = "Heal";
        this.desc = "This heals yourself";
        this.type = "Basic Card";
        this.cardSprite = Resources.Load<Sprite>("Health");
        this.effectType = EffectType.Self;
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
        if (GameManager.GetLocal() == GameManager.instance.currentPlayer && GameManager.GetLocal().GetCurrentHP() < GameManager.GetLocal().maxHP && !GameManager.GetLocal().GetIsWaitingResponse() && !GameManager.GetLocal().isFrozen) {
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
