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

    public override void Effect(Dictionary<string, string>[] states, int callerActorNumber) {
        Debug.LogFormat(
            "AttackCard.Effect(), callerActorNumber: {0}",
            callerActorNumber
        );

        Dictionary<string, string> localState = states[callerActorNumber - 1];
        Dictionary<string, string> remoteState = states[2 - callerActorNumber];
        remoteState["hp"] = (Int64.Parse(remoteState["hp"]) - 1).ToString();

        localState["turn"] = (Int64.Parse(localState["turn"]) - 1).ToString();
        remoteState["turn"] = (Int64.Parse(remoteState["turn"]) + 1).ToString();
    }
}
