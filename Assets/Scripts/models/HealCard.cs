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

    public static Action<PlayerController> Effect() {
        return (PlayerController target) => {
            Debug.LogFormat("AttackCard.Effect(), target's nickName: {0}", target.player.NickName);
            target.state["hp"] = (Int64.Parse(target.state["hp"]) + 1).ToString();
        };
    }
}
