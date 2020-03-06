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

    // If a card is passive, it can only be used to response.
    public bool isPassive;

    public virtual void Effect(Dictionary<string, string>[] states, int callerActorNumber) { }
}
