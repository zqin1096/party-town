using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : Card {
    public AttackCard() : base() {
        this.no = "0";
        this.label = "Attack";
        this.desc = "This attacks your enemy";
        this.effectType = EffectType.Enemy;
    }

    public override void Effect(PlayerController dest, PlayerController src, int effectDoerNumber) {
        Debug.LogFormat(
            "AttackCard.Effect(), destActorNumber: {0}, srcActorNumber: {1}, effectDoerNumber: {2}",
            dest.player.ActorNumber,
            src.player.ActorNumber,
            effectDoerNumber
        );

        dest.state["hp"] = (Int64.Parse(dest.state["hp"]) - 1).ToString();

        this.SetNextTurn(dest, src, effectDoerNumber);
        // dest.state["turn"] = (Int64.Parse(dest.state["turn"]) + 1).ToString();
        // src.state["turn"] = (Int64.Parse(dest.state["turn"]) - 1).ToString();
    }
}
