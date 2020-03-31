using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public Image timeBar;
    public float maxTime = 30f;
    public float timeLeft;

    // Start is called before the first frame update
    void Start() {
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update() {
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            timeBar.fillAmount = timeLeft / maxTime;
        } else {
            if (GameManager.GetLocal().discardMode == true){
                return;
            }
            if (GameManager.instance.currentPlayer == GameManager.GetLocal()) {
                GameUI.instance.OnEndTurnButton();
                ResetTimer();
            } else if (GameManager.GetLocal().GetIsGettingRequest()) {
                GameUI.instance.OnSkipButton();
                ResetTimer();
            }
        }
    }

    public void ResetTimer() {
        timeLeft = 20f;
    }
}
