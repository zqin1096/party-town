using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card {
    public Sprite suit;

    public string no;
    public string desc;
    public string label;
    public EffectType effectType;

    //public virtual void Effect(Dictionary<string, string>[] states, int callerActorNumber) { }

    public abstract void PlayCard();

    public abstract bool CanSelect();
}
