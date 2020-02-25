using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CardDisplay : MonoBehaviourPun
{
    public Card card;
    public Text cardText;
    public Image suitImage;
    public Text nameText;
    public Text descriptionText;
    public GameObject Hand;
    public GameObject Enemy;

    void Start()
    {
        card = new Card("8", "Attack", "");
        cardText.text = card.card;
        //suitImage.sprite = card.suit;
        nameText.text = card.name;
        descriptionText.text = card.description;
    }

    [PunRPC]
    void Initialize(bool isMine)
    {
        Debug.Log("Enter CardDisplay");
        // Check if this card belongs to the local player or the remote player.
        if (isMine)
        {
            PlayerController.local.cards.Add(this);
            //gameObject.transform.SetParent(Hand.transform, false);

        }
        else
        {
            GameManager.instance.GetOtherPlayer(PlayerController.local).cards.Add(this);
            //gameObject.transform.SetParent(Enemy.transform, false);
        }
    }

    public void Attack()
    {
        PlayerController.remote.photonView.RPC("TakeDamage", PlayerController.remote.photonPlayer);
    }
}
