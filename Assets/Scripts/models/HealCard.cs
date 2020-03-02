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

    public override void Effect(Dictionary<string, string>[] states, int callerActorNumber) {
        Debug.LogFormat(
            "HealCard.Effect(), callerActorNumber: {0}",
            callerActorNumber
        );

        Dictionary<string, string> localState = states[callerActorNumber - 1];
        Dictionary<string, string> remoteState = states[2 - callerActorNumber];
        localState["hp"] = (Int64.Parse(localState["hp"]) + 1).ToString();

        localState["turn"] = (Int64.Parse(localState["turn"]) - 1).ToString();
        remoteState["turn"] = (Int64.Parse(remoteState["turn"]) + 1).ToString();
    }
}
