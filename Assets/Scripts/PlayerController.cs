using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun {
    public GameObject deck;
    public GameObject enemy;

    public Text localHP;
    public Text remoteHP;
    public Text messageBox;
    public Text username;
    public Text remoteUsername;
    public Text localNumberOfCards;
    public Text remoteNumberOfCards;
    // This should be used to give hints
    public Text promtText;
    public Text CharacterName;

    public Player player;
    public int numOfcards;
    public int numberOfAttack;
    public Character character;

    private CardContainer selectedCard;
    private bool isWaitingResponse;
    private bool isGettingRequest;
    public int maxHP = 3;
    private int currentHP;
    private string enemyCard;
    private string requestedCard;

    public bool discardMode;
    public int discardNum;
    public String[] discardLabels;
    public AfterDiscarding discardCallback;
    public ArrayList discardBucket = new ArrayList();

    public bool isFrozen = false;

    void Update() {
        localNumberOfCards.text = GameManager.GetLocal().numOfcards.ToString();
        if (this.remoteNumberOfCards.text != GameManager.GetRemote().numOfcards.ToString()) {
            foreach (Transform child in enemy.transform) {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < GameManager.GetRemote().numOfcards; i += 1) {
                GameObject card = PhotonNetwork.Instantiate(
                    "CardDisplay",
                    new Vector3(0, 0, 0),
                    Quaternion.identity
                );
                card.transform.Find("CardFront").gameObject.SetActive(false);
                card.transform.Find("CardBack").gameObject.SetActive(true);
                card.transform.SetParent(enemy.transform, false);
            }
            remoteNumberOfCards.text = GameManager.GetRemote().numOfcards.ToString();
        }
    }

    public void SetPromptText(String prompt) {
        this.promtText.text = prompt;
    }

    public void SetCharacter(Character character) {
        Debug.Log("Player's character is set to " + character.name + "; Player name: " + this.username.text);
        this.character = character;
        this.maxHP = character.maxHP;
        this.CharacterName.text = character.name;
    }

    public string GetEnemyCard() {
        return this.enemyCard;
    }

    public int GetCurrentHP() {
        return this.currentHP;
    }

    public void SetCurrentHP(int hp) {
        this.currentHP = hp;
    }

    public void SetIsWaitingResponse(bool value) {
        this.isWaitingResponse = value;
    }

    public bool GetIsWaitingResponse() {
        return this.isWaitingResponse;
    }

    public void SetIsGettingRequest(bool value) {
        this.isGettingRequest = value;
    }

    public bool GetIsGettingRequest() {
        return this.isGettingRequest;
    }

    public void SetRequestedCard(string requestedCard) {
        this.requestedCard = requestedCard;
    }

    public string getRequestedCard() {
        return this.requestedCard;
    }

    public void setSelectedCard(CardContainer card) {
        selectedCard = card;
    }

    public CardContainer getSelectedCard() {
        return this.selectedCard;
    }

    public void EndTurn() {
        if(this.isFrozen)
            this.DefrozePlayer();
        if (selectedCard != null) {
            selectedCard.transform.position = new Vector2(selectedCard.transform.position.x, selectedCard.transform.position.y - CardContainer.SelectedCardYOffset);
            selectedCard = null;
        }
        this.numberOfAttack = 0;

        // if the number of cards you have is more than you maxHP
        if (this.numOfcards > this.maxHP) {
            AfterDiscarding callback = delegate () {
                GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
            };
            this.Discard(this.numOfcards - this.maxHP, null, callback);
        } else {
            GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
        }
    }

    public void StartTurn() {
        if (this.character.hasDrawingStageSkill) {
            Debug.LogFormat("Character should be using skills now");
            this.character.DrawingStageSkill();
        } else {
            InitializeCards(1);
        }
        if (this.isFrozen) {
            this.SetPromptText("You are frozen!");
            Invoke("EndTurn", 2);
        }

    }

    [PunRPC]
    void Initialize(Player player) {
        if (player.IsMasterClient && player.IsLocal) {
            this.SetCharacter(new CharacterB());
        } else if (!player.IsMasterClient && player.IsLocal) {
            this.SetCharacter(new CharacterC());
        }
        this.currentHP = maxHP;
        this.player = player;
        if (player.IsLocal) {
            this.username.text = player.NickName;
            this.remoteUsername.text = player.GetNext().NickName;
            InitializeCards(4);
        } else {
        }
    }

    [PunRPC]
    public void GetRequest(string enemyCard, string requestedCard) {
        this.isGettingRequest = true;
        this.enemyCard = enemyCard;
        this.requestedCard = requestedCard;
    }

    public void SendResponse(bool response) {
        this.isGettingRequest = false;
        if (response) {
            switch (this.enemyCard) {
                case "Attack":
                    break;
                case "Special Attack":
                    break;
                default:
                    break;
            }
        } else {
            switch (this.enemyCard) {
                case "Attack":
                    this.currentHP--;
                    SoundManager.PlaySound("pain");
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", RpcTarget.Others, currentHP, false);
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", player, currentHP, true);
                    GameManager.instance.CheckWinCondition();
                    break;
                case "Special Attack":
                    int damage = UnityEngine.Random.Range(1, 3);
                    this.currentHP -= damage;
                    SoundManager.PlaySound("pain");
                    if (currentHP < 0) {
                        currentHP = 0;
                    }
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", RpcTarget.Others, currentHP, false);
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", player, currentHP, true);
                    GameManager.instance.CheckWinCondition();
                    break;
                default:
                    break;
            }
        }
        this.enemyCard = null;
        this.requestedCard = null;
        GameManager.GetRemote().photonView.RPC("GetResponse", GameManager.GetRemote().player);
    }

    [PunRPC]
    void UpdateHealth(int hp, bool isMine) {
        if (!isMine) {
            remoteHP.text = hp.ToString();
        } else {
            localHP.text = hp.ToString();
        }
    }

    [PunRPC]
    public void GetResponse() {
        this.isWaitingResponse = false;
    }

    [PunRPC]
    public void FrozePlayer() {
        this.isFrozen = true;
    }

    public void DefrozePlayer() {
        this.isFrozen = false;
    }

    public void InitializeCards(int numOfCards) {
        for (int i = 0; i < numOfCards; i++) {
            GameObject card = PhotonNetwork.Instantiate(
                "CardDisplay",
                new Vector3(0, 0, 0),
                Quaternion.identity
            );
            card.transform.SetParent(deck.transform, false);
            card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false, null);
            card.GetPhotonView().RPC("Initialize", player, true, null);
        }
    }

    [PunRPC]
    public void RmoveCard() {
        int random = UnityEngine.Random.Range(0, deck.transform.childCount);
        Transform child = deck.transform.GetChild(random);
        string label = child.GetComponent<CardContainer>().card.label;
        child.GetComponent<CardContainer>().photonView.RPC("Use", GameManager.GetRemote().player, false);
        child.GetComponent<CardContainer>().photonView.RPC("Use", GameManager.GetLocal().player, true);
        PhotonNetwork.Destroy(child.gameObject);
        SoundManager.PlaySound("cardTaken");
        GameManager.GetRemote().photonView.RPC("GetCard", GameManager.GetRemote().player, label);
    }

    [PunRPC]
    public void GetCard(string label) {
        GameObject card = PhotonNetwork.Instantiate("CardDisplay", new Vector3(0, 0, 0), Quaternion.identity);
        card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false, label);
        card.GetPhotonView().RPC("Initialize", player, true, label);
        SoundManager.PlaySound("cardTaken");
        card.transform.SetParent(deck.transform, false);
    }

    public delegate void AfterDiscarding();

    // Specify how many card the player should disacrd and the labels of them
    public void Discard(int num, String[] label, AfterDiscarding callback) {
        this.SetPromptText("Please discard " + num + (num == 1 ? " card" : " cards"));
        this.discardMode = true;
        this.discardLabels = label;
        this.discardNum = num;
        this.discardCallback = callback;
        Debug.LogFormat("PlayerController.Discard(), Num: {0}", num);
    }
}