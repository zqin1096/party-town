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

    public virtual void Effect(PlayerController dest, PlayerController src, int effectDoerNumber) {
    }

    protected void SetNextTurn(PlayerController dest, PlayerController src, int effectDoerNumber) {
        int destActorNumber = dest.player.ActorNumber;
        int srcActorNumber = src.player.ActorNumber;

        Debug.LogFormat(
            "Card.SetNextTurn(): destActorNumber: {0}, srcActorNumber: {1}, effectDoerNumber: {2}",
            destActorNumber,
            srcActorNumber,
            effectDoerNumber
        );

        if (destActorNumber == effectDoerNumber) {
            dest.state["turn"] = (Int64.Parse(dest.state["turn"]) - 1).ToString();
            src.state["turn"] = (Int64.Parse(src.state["turn"]) + 1).ToString();
        } else {
            dest.state["turn"] = (Int64.Parse(dest.state["turn"]) + 1).ToString();
            src.state["turn"] = (Int64.Parse(src.state["turn"]) - 1).ToString();
        }
    }
}
