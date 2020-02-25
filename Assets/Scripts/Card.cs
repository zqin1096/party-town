using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string card;
    public Sprite suit;
    public string name;
    public string description;

    public Card(string card, string name, string description)
    {
        this.card = card;
        int random = UnityEngine.Random.Range(0, 4);
        // Assign the image of the suit based on the random value.
        this.name = name;
        this.description = description;
    }
}
