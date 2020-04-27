using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This charcter has two extra HP
public class CharacterA : Character {

    public CharacterA() : base() {
        this.characterSprite = Resources.Load<Sprite>("Lion");
        this.characterName = "Amphius";
    }

    public override string name { get { return "Character A"; } }
    public override int maxHP { get { return 5; } }
}
