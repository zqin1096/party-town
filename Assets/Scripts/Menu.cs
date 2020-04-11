using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks {
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button findMatchButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;
    public TextMeshProUGUI gameStartingText;

    void Start() {
        // Disable the button when the player is not connected to the server.
        findMatchButton.interactable = false;
        gameStartingText.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster() {
        // Re-enable the buttons.
        findMatchButton.interactable = true;
    }

    void SetScreen(GameObject screen) {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        screen.SetActive(true);
    }

    public void OnCreateRoomButton(TMP_InputField roomNameInput) {
        if (roomNameInput.text.Length > 0) {
            NetworkManager.instance.CreateRoom(roomNameInput.text);
        }
    }

    public void OnJoinRoomButton(TMP_InputField roomNameInput) {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnFindMatchButton() {
        NetworkManager.instance.CreateOrJoinRoom();
    }

    public void OnPlayerNameUpdate(InputField playerNameInput) {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom() {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI() {
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList) {
            playerListText.text += player.NickName + "\n";
        }

        if (PhotonNetwork.IsMasterClient) {
            startGameButton.interactable = true;
        } else {
            startGameButton.interactable = false;
        }
    }

    public void OnLeaveLobbyButton() {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton() {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1) {
            photonView.RPC("ActivateGameStartingText", RpcTarget.All, true);
            Invoke("TryStartGame", 3.0f);
        }
    }

    [PunRPC]
    public void ActivateGameStartingText(bool isActive) {
        gameStartingText.gameObject.SetActive(isActive);
    }

    void TryStartGame() {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1) {
            NetworkManager.instance.photonView.RPC("CreateScene", RpcTarget.All, "Game");
        } else {
            photonView.RPC("ActivateGameStartingText", RpcTarget.All, false);
        }
    }
}