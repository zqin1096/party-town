using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCard : Card {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public DefenseCard() : base() {
        this.no = "2";
        this.label = "Defense";
        this.desc = "This voids an attack card";
        this.effectType = EffectType.Self;
        this.isPassive = true;
    }
}
