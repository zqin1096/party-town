using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Insteand of drawing two cards from the deck, this charcter can draw one card from the opponent
 * 
 * */
public class CharacterB : Character
{
    public new int maxHP = 3;
    public new bool hasDrawingStageSkill = true;

    public override void DrawingStageSkill()
    {
        if(GameManager.GetRemote().numOfcards != 0)
        {
            Debug.LogFormat("Character B's skill used by {0}", GameManager.GetLocal().username.text);
            GameManager.GetRemote().photonView.RPC("RmoveCard", GameManager.GetRemote().player);
        }
        else
        {
            GameManager.GetLocal().InitializeCards(2);
        }
    }
}
