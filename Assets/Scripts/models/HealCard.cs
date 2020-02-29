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
        return (PlayerController self) => {
            Debug.LogFormat("AttackCard.Effect(), remote.displayName: {0}", self.displayName);
            self.state["hp"] = (Int64.Parse(self.state["hp"]) + 1).ToString();
        };
    }
}
