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

    public static Action<PlayerController> Effect() {
        return (PlayerController remote) => {
            Debug.LogFormat("AttackCard.Effect(), remote.displayName: {0}", remote.displayName);
            remote.state["hp"] = (Int64.Parse(remote.state["hp"]) - 1).ToString();
        };
    }
}
