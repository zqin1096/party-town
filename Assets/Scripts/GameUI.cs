using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {
    public Button endTurnButton;

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
}
