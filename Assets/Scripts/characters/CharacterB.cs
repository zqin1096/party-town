using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Insteand of drawing two cards from the deck, this charcter can draw one card from the opponent
 * 
 * */
public class CharacterB : Character {
    public override string name { get { return "Character B"; } }
    public override bool hasDrawingStageSkill { get { return true; } }

    public override void DrawingStageSkill() {
        if (GameManager.GetRemote().numOfcards != 0) {
            Debug.Log(GameManager.GetRemote().numOfcards);
            // Debug.LogFormat("Character B's skill used by {0}", GameManager.GetLocal().username.text);
            GameManager.GetRemote().photonView.RPC("RmoveCard", GameManager.GetRemote().player);
            GameManager.GetLocal().InitCardWithAnimation(1);
        } else {
            GameManager.GetLocal().InitCardWithAnimation(2);
        }
    }
}
