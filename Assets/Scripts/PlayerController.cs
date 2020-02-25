using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    // The photon view ID of player1 is 2, and the photon view ID of player2 is 3.
    // The photon view ID of the network manager is 1.


    // Summarizes a "player" within a room, identified (in that room) by ID.
    public Player photonPlayer;
    public List<CardDisplay> cards = new List<CardDisplay>();

    public static PlayerController local;
    public static PlayerController remote;

    public GameObject Hand;

    public int hp;
    public Text localHP;
    public Text remoteHP;

    [PunRPC]

    void Initialize(Player player)
    {
        Debug.Log("Enter PlayerController");
        photonPlayer = player;

        if (player.IsLocal)
        {
            local = this;
            DrawCards(4);
        }
        else
        {
            remote = this;
        }
    }

    void DrawCards(int num)
    {
        for (var i = 0; i < num; i++)
        {
            GameObject card = PhotonNetwork.Instantiate("CardContainer", new Vector3(0, 0, 0), Quaternion.identity);
            card.transform.SetParent(Hand.transform, false);
            card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
            card.GetPhotonView().RPC("Initialize", photonPlayer, true);
        }
    }

    [PunRPC]
    public void TakeDamage()
    {
        hp--;
        photonView.RPC("UpdateHealth", RpcTarget.Others, hp, false);
        photonView.RPC("UpdateHealth", photonPlayer, hp, true);
    }

    [PunRPC]
    void UpdateHealth(int hp, bool isMine)
    {
        if (!isMine)
        {
            remoteHP.text = hp.ToString();
        } else
        {
            localHP.text = hp.ToString();
        }
    }

}
