using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CharacterSelection : MonoBehaviourPun {
    public Character[] characters;
    public Button CharacterButtonA;
    public Button CharacterButtonB;
    public Button CharacterButtonC;

    public GameObject playWindow;
    public GameObject settingWindow;

    public const byte SELECTION_EVENT = 2;
    public const int SELECTION_DONE = 2;
    public static int numOfPlayersSelection = 0;

    public GameObject selectMessage;
    public GameObject waitMessage;

    // Start is called before the first frame update
    void Start() {
        characters = new Character[] {
            new CharacterA(),
            new CharacterB(),
            new CharacterC()
        };
    }

    // Update is called once per frame
    void Update() {
        if (numOfPlayersSelection == SELECTION_DONE) {
            playWindow.SetActive(true);
            settingWindow.SetActive(false);
            numOfPlayersSelection = 0;
            if (GameManager.GetLocal().player.IsMasterClient) {
                GameManager.instance.photonView.RPC("SetupTable", RpcTarget.AllBuffered);
            }
        }
    }
    public void CharacterSet(int index) {
        selectMessage.gameObject.SetActive(false);
        waitMessage.gameObject.SetActive(true);
        GameManager.instance.photonView.RPC("SetRemoteCharacter", GameManager.GetRemote().player, characters[index].characterName);
        GameManager.GetLocal().character = characters[index];
        CharacterButtonA.gameObject.SetActive(false);
        CharacterButtonB.gameObject.SetActive(false);
        CharacterButtonC.gameObject.SetActive(false);
        NotifyClients();
    }

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj) {
        if (obj.Code == SELECTION_EVENT) {
            numOfPlayersSelection++;
        }
    }

    private void NotifyClients() {
        RaiseEventOptions options = new RaiseEventOptions() {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(SELECTION_EVENT, null, options, sendOptions);
    }
}
