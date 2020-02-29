using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

// The photon view ID of player1 is 2, and the photon view ID of player2 is 3.
// The photon view ID of the network manager is 1.
public class PlayerController : MonoBehaviourPun {
    public static PlayerController local;
    public static PlayerController remote;

    // Summarizes a "player" within a room, identified (in that room) by ID.
    public Player photonPlayer;
    public List<CardContainer> cards = new List<CardContainer>();

    public string displayName;
    public GameObject Deck;
    public int hp;
    public Text localHP;
    public Text remoteHP;

    [PunRPC]
    void Initialize(Player player) {
        Debug.LogFormat("PlayerController.Initialize(): player.IsLocal: {0}", player.IsLocal);

        photonPlayer = player;

        if (player.IsLocal) {
            local = this;
            this.displayName = "local-1";
            DrawCards(4);
        } else {
            remote = this;
            this.displayName = "remote-1";
        }
    }

    void DrawCards(int num) {
        Debug.LogFormat("PlayerController.DrawCards, num: {0}", num);

        for (var i = 0; i < num; i++) {
            GameObject card = PhotonNetwork.Instantiate(
                "CardContainer",
                new Vector3(0, 0, 0),
                Quaternion.identity
            );
            card.transform.SetParent(Deck.transform, false);
            card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
            card.GetPhotonView().RPC("Initialize", photonPlayer, true);
        }
    }

    [PunRPC]
    public void TakeEffect(string cardNo) {
        Debug.LogFormat("PlayerContainer.TakeEffect(): cardNo: {0}", cardNo);

        switch (cardNo) {
            case "0":
                Debug.LogFormat("PlayerContainer.TakeEffect(): chosen 0 cardNo");
                AttackCard.Effect(this);
                Debug.LogFormat("PlayerContainer.TakeEffect(): now hp: {0}", this.hp);
                break;
            default:
                break;
        }

        // hp--;
        photonView.RPC("UpdateState", RpcTarget.Others, this, false);
        photonView.RPC("UpdateState", photonPlayer, this, true);
    }

    [PunRPC]
    void UpdateState(PlayerController _playerController, bool isMine) {
        Debug.LogFormat("PlayerController.UpdateState(), isMine: {0}", isMine);

        if (!isMine) {
            remoteHP.text = _playerController.hp.ToString();
        } else {
            localHP.text = _playerController.hp.ToString();
        }
    }
}
