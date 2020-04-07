using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card {
    public Sprite suit;

    public string no;
    public string number;
    public string desc;
    public string label;
    public string type;
    public Sprite cardSprite;
    public EffectType effectType;
    private static String[] numbers = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    //public virtual void Effect(Dictionary<string, string>[] states, int callerActorNumber) { }

    public abstract void PlayCard();

    public abstract bool CanSelect();

    public static string generateNumber() {
        return numbers[UnityEngine.Random.Range(0, numbers.Length)];
    }
}
