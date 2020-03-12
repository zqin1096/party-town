using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviourPun {
    public Button endTurnButton;
    public Button playButton;
    public Button responseButton;
    public Button skipButton;
    public Image timeBar;
    public Text winText;
    public Text messageBox;

    public static GameUI instance;
    void Awake() {
        instance = this;
    }

    void Update() {
        SetActiveEndTurnButton(GameManager.instance.currentPlayer == GameManager.GetLocal() && !GameManager.GetLocal().GetIsWaitingResponse() && !GameManager.isGameEnded);
        SetTimerActive((GameManager.instance.currentPlayer == GameManager.GetLocal() && !GameManager.GetLocal().GetIsWaitingResponse() && !GameManager.isGameEnded) || (GameManager.GetLocal().GetIsGettingRequest()));
        SetActivePlayButton(GameManager.instance.currentPlayer == GameManager.GetLocal() && !GameManager.GetLocal().GetIsWaitingResponse());
        TogglePlayButton(GameManager.GetLocal().getSelectedCard() != null);
        SetActiveResponseButton(GameManager.GetLocal().GetIsGettingRequest());
        ToggleResponseButton(GameManager.GetLocal().getSelectedCard() != null);
        SetActiveSkipButton(GameManager.GetLocal().GetIsGettingRequest());
    }

    // Response button.
    public void SetActiveResponseButton(bool active) {
        responseButton.gameObject.SetActive(active);
    }

    public void ToggleResponseButton(bool toggle) {
        responseButton.interactable = toggle;
    }

    public void OnResponseButton() {
        GameManager.GetLocal().getSelectedCard().DoResponse();
        GameManager.GetLocal().SendResponse(true);
    }

    // Skip button.
    public void SetActiveSkipButton(bool active) {
        skipButton.gameObject.SetActive(active);
    }

    public void OnSkipButton() {
        if (GameManager.GetLocal().getSelectedCard() != null) {
            CardContainer card = GameManager.GetLocal().getSelectedCard();
            Vector2 move = new Vector2(card.transform.position.x, card.transform.position.y - 5);
            card.transform.position = move;
            GameManager.GetLocal().setSelectedCard(null);
        }
        GameManager.GetLocal().SendResponse(false);
    }

    // End turn button.
    public void OnEndTurnButton() {
        GameManager.GetLocal().EndTurn();
    }

    public void SetActiveEndTurnButton(bool active) {
        endTurnButton.gameObject.SetActive(active);
    }

    public void ToggleEndTurnButton(bool toggle) {
        endTurnButton.interactable = toggle;
    }

    // Play button.
    public void OnPlayButton() {
        Debug.Log("Play button clicked.");
        GameManager.instance.currentPlayer.getSelectedCard().DoEffect();
    }

    public void TogglePlayButton(bool toggle) {
        Debug.Log("Toggle play button.");
        playButton.interactable = toggle;
    }

    public void SetActivePlayButton(bool active) {
        playButton.gameObject.SetActive(active);
    }

    // Timer.
    public void SetTimerActive(bool active) {
        timeBar.gameObject.SetActive(active);
    }

    public void SetWinText(string text) {
        winText.gameObject.SetActive(true);
        winText.text = text;
    }

    [PunRPC]
    public void SetMessageBox(string s) {
        messageBox.text = s;
    }
}