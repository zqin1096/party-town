using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Draw one more card at the begining
 * 
 * */
public class CharacterC : Character {
    public override string name { get { return "Character C"; } }
    public override bool hasDrawingStageSkill { get { return true; } }

    public override void DrawingStageSkill() {
        GameManager.GetLocal().InitCardWithAnimation(3);
    }
}
