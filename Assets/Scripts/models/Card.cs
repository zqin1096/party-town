using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {
    public Sprite suit;

    public string no;
    public string desc;
    public string label;
    public EffectType effectType;

    public virtual void Effect(Dictionary<string, string>[] states, int callerActorNumber) {}
}
