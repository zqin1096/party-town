using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    
    public int maxHP = 3;

    // indicate if this character has certain skills on corresponding stages
    public bool hasDrawingStageSkill = false;
    public bool hasPlayingingStageSkill = false;
    public bool hasEndingStageSkill = false;


    virtual public void DrawingStageSkill()
    {

    }

    virtual public void PlayingStageSkill()
    {

    }

    virtual public void EndingStageSkill()
    {

    }
}
