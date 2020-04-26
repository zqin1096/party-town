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
            GameManager.GetRemote().photonView.RPC("RemoveCard", GameManager.GetRemote().player);
            GameManager.GetLocal().InitCardWithAnimation(1);
        } else {
            GameManager.GetLocal().InitCardWithAnimation(2);
        }
    }
}
