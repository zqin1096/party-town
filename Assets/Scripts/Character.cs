using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHealth;
    public Texture2D card;
    //public string gender;
    //public string[] skills;

    public Character()
    {

    }
    public Character(int maxHealth, string cardPath)
    {
        this.maxHealth = maxHealth;
        this.card = Resources.Load<Texture2D>(cardPath);
    }
}

public class Character1 : Character
{
    public Character1() : base(4, "")
    {
    }
}

public class Character2 : Character
{
    public Character2() : base(3, "")
    {
    }
}

public class Character3 : Character
{
    public Character3() : base(4, "")
    {
    }
}

public class Character4 : Character
{
    public Character4() : base(4, "")
    {
    }
}
