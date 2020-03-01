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

    public override void Effect(PlayerController dest, PlayerController src, int effectDoerNumber) {
        Debug.LogFormat(
            "HealCard.Effect(), destActorNumber: {0}, srcActorNumber: {1}, effectDoerNumber: {2}",
            dest.player.ActorNumber,
            src.player.ActorNumber,
            effectDoerNumber
        );

        dest.state["hp"] = (Int64.Parse(dest.state["hp"]) + 1).ToString();

        this.SetNextTurn(dest, src, effectDoerNumber);
    }
}
