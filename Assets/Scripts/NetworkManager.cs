using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks {
    // NetworkManager instance (singleton).
    public static NetworkManager instance;

    // Awake is used to initialize any variable or game state before the game starts.
    // Awake is called only once during the lifetime of the script instance.
    void Awake() {
        if (instance != null && instance != this) {
            // Check if an instance already exists (game scene back to the menu scene).
            gameObject.SetActive(false);
        } else {
            // Set the instance to be the script.
            instance = this;
            // gameObject is what the script is on.
            // Do not destroy the target Object when loading a new Scene.
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.LogFormat("NetworkManager.OnConnectedToMaster(): Connected to the master server");
        // CreateRoom("testRoom");
    }

    public override void OnCreatedRoom() {
        Debug.LogFormat(
            "NetworkManager.OnCreatedRoom(): roomName: {0}",
            PhotonNetwork.CurrentRoom.Name
        );
    }

    public void CreateRoom(string roomName) {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
    }

    // This function will be used as an RPC. When the host starts the game, everyone in the
    // room will call this function.
    [PunRPC]
    public void CreateScene(string sceneName) {
        Debug.LogFormat("NetworkManager.CreateScene(): roomName: {0}, sceneName: {1}",
            PhotonNetwork.CurrentRoom.Name,
            sceneName
        );
        PhotonNetwork.LoadLevel(sceneName);
    }
}
