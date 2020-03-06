using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {
    public Button endTurnButton;
    public Button playButton;

    public static GameUI instance;

    void Awake() {
        instance = this;
    }

    public void OnEndTurnButton() {
        GameManager.GetLocal().EndTurn();
    }

    public void ToggleEndTurnButton(bool toggle) {
        endTurnButton.interactable = toggle;
    }

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
}
