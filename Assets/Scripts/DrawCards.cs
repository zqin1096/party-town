using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{

    public GameObject Card1;
    public GameObject Card2;
    public GameObject Hand;
    public GameObject Enemy;

    List<GameObject> cards = new List<GameObject>();


    void Start()
    {
        cards.Add(Card1);
        cards.Add(Card2);
    }

    public void OnClick()
    {
        for (var i = 0; i < 5; i++)
        {
            // Create a Card1 (no rotation) and place it as a child of Hand.
            GameObject playerCard = Instantiate(cards[Random.Range(0, cards.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            // Every object in a Scene has a Transform. It's used to store and manipulate the position, rotation
            // and scale of the object. Every Transform can have a parent, which allows you to apply position, rotation
            // and scale hierarchically. This is the hierarchy seen in the Hierarchy pane.
            playerCard.transform.SetParent(Hand.transform, false);

            GameObject enemyCard = Instantiate(cards[Random.Range(0, cards.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            enemyCard.transform.SetParent(Enemy.transform, false);
        }
    }
}
