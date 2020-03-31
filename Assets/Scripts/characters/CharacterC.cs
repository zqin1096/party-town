using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Draw one more card at the begining
 * 
 * */
public class CharacterC : Character
{
    public new int maxHP = 3;
    public new bool hasDrawingStageSkill = true;

    public override void DrawingStageSkill()
    {
        GameManager.GetLocal().InitializeCards(3);
    }
}
