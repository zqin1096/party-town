using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character {

    public Sprite characterSprite;
    public string characterName;

    public virtual int maxHP { get { return 3; } }

    // indicate if this character has certain skills on corresponding stages
    public virtual bool hasDrawingStageSkill { get { return false; } }
    public virtual bool hasPlayingingStageSkill { get { return false; } }
    public virtual bool hasEndingStageSkill { get { return false; } }
    virtual public string name { get { return "default name"; } }

    virtual public void DrawingStageSkill() {

    }

    virtual public void PlayingStageSkill() {

    }

    virtual public void EndingStageSkill() {

    }
}
