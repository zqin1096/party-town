using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class CharacterSelection : MonoBehaviourPun {
    // public GameObject[] SelectCharacter;
    public Character[] characters;
    public Button CharacterButtonA;
    public Button CharacterButtonB;
    public Button CharacterButtonC;

    // public Button btn;

    public GameObject PlayWindow;
    public GameObject SettingWindow;
    // public GameObject SelectionButton;
    public Button SelectionButton;

    public const byte SELECTION_EVENT = 2;
    public const int SELECTION_DONE = 2;
    public static int numOfPlayersSelection = 0;

    // Start is called before the first frame update
    void Start()
    {

        SettingWindow.SetActive(true);
        PlayWindow.SetActive(false);
        characters = new Character[] {
            new CharacterA(),
            new CharacterB(),
            new CharacterC()
        };
    }

    // Update is called once per frame
    void Update()
    {
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
        // Button btn = SelectionButton.GetComponent<Button>();
        // btn.GetComponent<Button> ().onClick.AddListener (() => {
        //     ClickEvent ();
        // });

        
        // byte evCode = 2;
        RaiseEventOptions options= new RaiseEventOptions(){
                    // GameManager.GetLocal().Selection(),
                // GameManager.GetRemote().Selection()
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(SELECTION_EVENT,null,options, sendOptions);
      
    
        // if (player.IsLocal) {
        //     this.username.text = player.NickName;
        //     this.remoteUsername.text = player.GetNext().NickName;
        //     InitCardWithAnimation(4);
        //     RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        //     SendOptions sendOptions = new SendOptions { Reliability = true };
        //     PhotonNetwork.RaiseEvent(INITIALIZE_PLAYERS_DONE_EVENT, null, raiseEventOptions, sendOptions);
        // } else {
        // }

        // GetNextScene();
        // PlayWindow.SetActive(true);
        // SettingWindow.SetActive(false);
    }

	// private void ClickEvent(){
	// 	Debug.Log ("Button Clicked. ClickHandler.");
	// }

    // public void ToggleResponse(bool toggle) {
    //     SelectionButton.interactable = toggle;
    // }

    // void GetNextScene(){
    //     StartCoroutine(LoadPlayScene0());
    // }

    // IEnumerator LoadPlayScene0() {
    //     yield return new WaitForSeconds(3f);
    //     PlayWindow.SetActive(true);
    //     SettingWindow.SetActive(false);
    // }
        
//     private void OnEvent( byte eventCode, object content, int senderID)
//     {
//         if ( (eventCode == 1) && (senderID <= 0) ) 
//         {
// //          PhotonPlayer sender = PhotonPlayer.Find(senderid);
//             int counter = (int)content;
//             Debug.Log( string.Format("Counter from Server: {0}", counter ));
//         }
//     }


}
