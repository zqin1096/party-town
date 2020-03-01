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
        return (PlayerController target) => {
            Debug.LogFormat("AttackCard.Effect(), target's nickName: {0}", target.player.NickName);
            target.state["hp"] = (Int64.Parse(target.state["hp"]) - 1).ToString();
        };
    }
}
