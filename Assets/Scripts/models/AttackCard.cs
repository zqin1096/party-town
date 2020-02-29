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

    public static void Effect(PlayerController remote) {
        Debug.LogFormat("AttackCard.Effect(), remote.displayName: {0}", remote.displayName);

        remote.hp -= 1;
    }
}
