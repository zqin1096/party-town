using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static AudioClip painSound;
    static AudioSource audioSource;
    // Start is called before the first frame update
    void Start() {
        painSound = Resources.Load<AudioClip>("Pain");
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string sound) {
        switch (sound) {
            case "pain":
                audioSource.PlayOneShot(painSound);
                break;
        }
    }
}
