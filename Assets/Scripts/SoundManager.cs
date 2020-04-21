using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static AudioClip painSound;
    public static AudioClip cardTaken;
    public static AudioClip healSound;
    public static AudioClip attackSound;
    public static AudioClip shieldSound;
    public static AudioClip specialAttackSound;
    static AudioSource audioSource;
    // Start is called before the first frame update
    void Start() {
        painSound = Resources.Load<AudioClip>("Pain");
        cardTaken = Resources.Load<AudioClip>("TakeCard");
        healSound = Resources.Load<AudioClip>("Heal");
        attackSound = Resources.Load<AudioClip>("Attack");
        shieldSound = Resources.Load<AudioClip>("Shield");
        specialAttackSound = Resources.Load<AudioClip>("SpecialAttack");
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string sound) {
        switch (sound) {
            case "pain":
                audioSource.PlayOneShot(painSound);
                break;
            case "cardTaken":
                audioSource.PlayOneShot(cardTaken);
                break;
            case "heal":
                audioSource.PlayOneShot(healSound);
                break;
            case "attack":
                audioSource.PlayOneShot(attackSound);
                break;
            case "defense":
                audioSource.PlayOneShot(shieldSound);
                break;
            case "specialAttack":
                audioSource.PlayOneShot(specialAttackSound);
                break;
        }
    }
}
