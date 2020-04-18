using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class CharacterSelection : MonoBehaviour {
    // public GameObject[] SelectCharacter;
    public Character[] characters;

    public Button CharacterButtonA;
    public Button CharacterButtonB;
    public Button CharacterButtonC;

    public GameObject PlayWindow;
    public GameObject SettingWindow;


    // public Button CharacterSelectA;
    // public Button CharacterSelectB;
    // public Button CharacterSelectC;

    // Start is called before the first frame update
    void Start()
    {

        SettingWindow.SetActive(true);
        PlayWindow.SetActive(false);
        // for (int i = 0; i < SelectCharacter.Length; i++) {
        //     // SelectCharacter[i].SetActive(false);
        //     SelectCharacter[i].SetActive(true);
        // }

        characters = new Character[] {
            new CharacterA(),
            new CharacterB(),
            new CharacterC()
        };

    }

    // Update is called once per frame
    void Update()
    {
        // CharacterSet();
    }
    public void CharacterSet(int index) {
        // for (int i = 0; i < SelectCharacter.Length; i++) {
        //     SelectCharacter[i].SetActive(false);
        // }
        // SelectCharacter[index].SetActive(true);
        GameManager.GetLocal().character = characters[index];
        // if(index == 0) {
        //     GameManager.GetLocal().character = characters[index];
        // }
        // else if (index == 1){
        //     GameManager.GetLocal().character = characters[index];
        // }
        // else if(index  == 2) {
        //       GameManager.GetLocal().character = characters[index];
        // }
        

    }

    public void Selection() {
    //    Invoke("TryStartGame", 6.0f);
       PlayWindow.SetActive(true);
       SettingWindow.SetActive(false);

    }

}
