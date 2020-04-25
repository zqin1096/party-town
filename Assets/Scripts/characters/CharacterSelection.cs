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

    public GameObject PlayWindow;
    public GameObject SettingWindow;

    public Button SelectionButton;

    public const byte SELECTION_EVENT = 2;
    public const int SELECTION_DONE = 2;
    public static int numOfPlayersSelection = 0;

    // Start is called before the first frame update
    void Start() {

        SettingWindow.SetActive(true);
        PlayWindow.SetActive(false);
        characters = new Character[] {
            new CharacterA(),
            new CharacterB(),
            new CharacterC()
        };
    }

    // Update is called once per frame
    void Update() {
        if (numOfPlayersSelection == SELECTION_DONE) {
            // photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
            PlayWindow.SetActive(true);
            SettingWindow.SetActive(false);
        }
    }
    public void CharacterSet(int index) {
        Debug.Log("character set: " + index);
        GameManager.GetLocal().character = characters[index];
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

    public void Selection() {
        RaiseEventOptions options = new RaiseEventOptions() {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(SELECTION_EVENT, null, options, sendOptions);

    }
}
