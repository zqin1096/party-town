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

    public static Action<PlayerController, PlayerController> Effect() {
        return (PlayerController dest, PlayerController src) => {
            Debug.LogFormat(
                "HealCard.Effect(), destActorNumber: {0}, srcActorNumber: {1}",
                dest.player.ActorNumber,
                src.player.ActorNumber
            );

            dest.state["hp"] = (Int64.Parse(dest.state["hp"]) + 1).ToString();

            src.state["turn"] = (Int64.Parse(src.state["turn"]) + 1).ToString();
            dest.state["turn"] = (Int64.Parse(dest.state["turn"]) - 1).ToString();
        };
    }
}
