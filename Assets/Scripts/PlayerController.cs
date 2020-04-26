using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPun {
    public GameObject deck;
    public GameObject enemy;
    public GameObject table;

    public GameObject cardPreb;

    public Text localHP;
    public Text remoteHP;
    public Text username;
    public Text remoteUsername;
    public Text localNumberOfCards;
    public Text remoteNumberOfCards;
    // This should be used to give hints
    public Text promtText;

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

    public const byte INITIALIZE_PLAYERS_DONE_EVENT = 1;
    public const byte USE_CARD_EVENT = 3;

    public GameObject playedCard;
    public GameObject canvas;

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj) {
        if (obj.Code == USE_CARD_EVENT) {
            object[] data = (object[])obj.CustomData;
            string label = (string)data[0];
            string number = (string)data[1];
            GameObject card = Instantiate(cardPreb, new Vector3(0, 400, 0), Quaternion.identity);
            card.GetComponent<CardContainer>().InitializeCard(label);
            card.GetComponent<CardContainer>().card.number = number;
            card.GetComponent<CardContainer>().card.used = true;
            card.transform.SetParent(canvas.transform, false);
            StartCoroutine(MoveCard(card));
            IEnumerator MoveCard(GameObject item) {
                iTween.MoveTo(item, iTween.Hash("position", playedCard.transform.position, "time", 0.5f));
                yield return item;
            }
        }
    }

    void Update() {
        localNumberOfCards.text = GameManager.GetLocal().numOfcards.ToString();
        if (this.remoteNumberOfCards.text != GameManager.GetRemote().numOfcards.ToString()) {
            foreach (Transform child in enemy.transform) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < GameManager.GetRemote().numOfcards; i += 1) {
                GameObject card = Instantiate(cardPreb, new Vector3(0, 0, 0), Quaternion.identity);
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
        // Debug.Log("Player's character is set to " + character.name + "; Player name: " + this.username.text);
        this.character = character;
        this.maxHP = character.maxHP;
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
        this.SetPromptText("Turn Ending!");
        this.FrozePlayer();
        Invoke("EndTurnSecond", 1);
        if (selectedCard != null) {
            selectedCard.transform.position = new Vector2(selectedCard.transform.position.x, selectedCard.transform.position.y - CardContainer.SelectedCardYOffset);
            selectedCard = null;
        }
    }

    public void EndTurnSecond() {
        this.numberOfAttack = 0;
        this.DefrozePlayer();
        string color = GameManager.GetLocal().player.IsMasterClient ? "red" : "green";
        // if the number of cards you have is more than you maxHP
        if (this.numOfcards > this.maxHP) {
            GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.instance.currentPlayer.player.NickName + " is discarding cards.", color);
            AfterDiscarding callback = delegate () {
                GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.instance.currentPlayer.player.NickName + "'s turn ends!", color);
                this.SetPromptText("Your opponent is playing...");
                GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
            };
            this.Discard(this.numOfcards - this.maxHP, null, callback);
        } else {
            GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.instance.currentPlayer.player.NickName + "'s turn ends!", color);
            this.SetPromptText("Your opponent is playing...");
            GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
        }
    }

    public void StartTurn() {
        string color = GameManager.GetLocal().player.IsMasterClient ? "red" : "green";
        GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.instance.currentPlayer.player.NickName + "'s turn starts!", color);
        this.SetPromptText("Turn starting! Drawing card...");
        if (this.character.hasDrawingStageSkill) {
            Debug.LogFormat("Character should be using skills now");
            this.character.DrawingStageSkill();
            Debug.Log("has drawing stage skill");
        } else {
            InitCardWithAnimation(2);
        }
        Invoke("StartTurnSecond", 2);  // Potential problem: should wait for drawing card animation (coroutine) to finish before executing this line.
    }

    public void StartTurnSecond() {
        this.SetPromptText("Use card wisely!");
        if (this.isFrozen) {    // Potential problem: related to StartTurn(). Player is frozen during animation and this line maybe called before animation finishes.
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
            InitCardWithAnimation(4);
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(INITIALIZE_PLAYERS_DONE_EVENT, null, raiseEventOptions, sendOptions);
        } else {
        }
    }

    [PunRPC]
    public void GetRequest(string enemyCard, string requestedCard) {
        this.isGettingRequest = true;
        this.enemyCard = enemyCard;
        this.requestedCard = requestedCard;
        this.SetPromptText("You are under attack! Defense yourself!");
    }

    public void SendResponse(bool response) {
        this.SetPromptText("");
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
            string color = GameManager.GetLocal().player.IsMasterClient ? "red" : "green";
            switch (this.enemyCard) {
                case "Attack":
                    this.currentHP--;
                    GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.GetLocal().player.NickName + " loses 1hp!", color);
                    SoundManager.PlaySound("pain");
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", RpcTarget.Others, currentHP, false);
                    GameManager.GetLocal().photonView.RPC("UpdateHealth", player, currentHP, true);
                    GameManager.instance.CheckWinCondition();
                    break;
                case "Special Attack":
                    int damage = this.isFrozen ? 2 : 1;
                    this.currentHP -= damage;
                    GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.GetLocal().player.NickName + " loses " + damage + "hp!", color);
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


    public void MoveSelectedCard() {
        this.selectedCard.transform.SetParent(canvas.transform, true);
        StartCoroutine(Wait());
        IEnumerator Wait() {
            this.FrozePlayer();
            yield return StartCoroutine(MoveCard(this.selectedCard.gameObject));
            this.DefrozePlayer();
            // GameManager.GetLocal().setSelectedCard(null);
        }

        IEnumerator MoveCard(GameObject card) {
            iTween.MoveTo(card, iTween.Hash("position", playedCard.transform.position, "time", 0.5f));
            yield return card;
        }
    }

    public void InitCardWithAnimation(int numOfCards) {
        bool temp = this.isFrozen;
        this.FrozePlayer();
        photonView.RPC("IncreaseCards", RpcTarget.Others, false, numOfCards);
        photonView.RPC("IncreaseCards", player, true, numOfCards);
        GameObject[] cards = new GameObject[numOfCards];
        for (int i = 0; i < numOfCards; i++) {
            GameObject card = Instantiate(cardPreb, new Vector3(0, 0, 0), Quaternion.identity);
            card.GetComponent<CardContainer>().InitializeCard(null);
            cards[i] = card;
        }
        StartCoroutine(Wait(cards));
        IEnumerator Wait(GameObject[] items) {
            // Wait for animation of all the cards to finish, then defroze the player.
            yield return StartCoroutine(MoveCards(items));
            if (!temp) {
                DefrozePlayer();
            }
        }

        IEnumerator MoveCards(GameObject[] items) {
            foreach (GameObject item in items) {
                item.transform.SetParent(table.transform, false);
                iTween.MoveTo(item, iTween.Hash("position", new Vector3(-800, -375, 0),
                                                "time", 0.5f,
                                                "islocal", true,
                                                "onupdate", "ChangeCardAlpha",
                                                "onupdatetarget", this.gameObject,
                                                "onupdateparams", item,
                                                "oncomplete", "SetCardParentToDeck",
                                                "oncompletetarget", this.gameObject,
                                                "oncompleteparams", item));
                yield return item;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    void SetCardParentToDeck(object item) {
        GameObject card = ((GameObject)item);
        card.transform.SetParent(deck.transform, true);
        card.GetComponent<CanvasGroup>().alpha = 1f;
    }

    void SetCardParentToEnemy(object item) {
        GameObject card = ((GameObject)item);
        card.transform.SetParent(enemy.transform, true);
        card.GetComponent<CanvasGroup>().alpha = 1f;
    }

    void ChangeCardAlpha(object item) {
        GameObject card = ((GameObject)item);
        card.GetComponent<CanvasGroup>().alpha -= 0.02f;
    }

    [PunRPC]
    public void IncreaseCards(bool isMine, int numOfCards) {
        if (isMine) {
            GameManager.GetLocal().numOfcards += numOfCards;
        } else {
            GameManager.GetRemote().numOfcards += numOfCards;
        }
    }

    [PunRPC]
    void UseCard(bool isMine) {
        if (isMine) {
            GameManager.GetLocal().numOfcards--;
        } else {
            GameManager.GetRemote().numOfcards--;
        }
    }

    [PunRPC]
    public void RemoveCard() {
        Debug.Log("called");
        int random = UnityEngine.Random.Range(0, deck.transform.childCount);
        Transform child = deck.transform.GetChild(random);
        string label = child.GetComponent<CardContainer>().card.label;
        //child.GetComponent<CardContainer>().photonView.RPC("Use", GameManager.GetRemote().player, false);
        //child.GetComponent<CardContainer>().photonView.RPC("Use", GameManager.GetLocal().player, true);
        photonView.RPC("UseCard", RpcTarget.Others, false);
        photonView.RPC("UseCard", player, true);
        Destroy(child.gameObject);
        // Animation: card move from deck to enemy.
        SoundManager.PlaySound("cardTaken");
        GameManager.GetRemote().photonView.RPC("GetCard", GameManager.GetRemote().player, label);
    }

    [PunRPC]
    public void GetCard(string label) {
        photonView.RPC("IncreaseCards", RpcTarget.Others, false, 1);
        photonView.RPC("IncreaseCards", player, true, 1);
        GameObject card = Instantiate(cardPreb, new Vector3(0, 0, 0), Quaternion.identity);
        card.GetComponent<CardContainer>().InitializeCard(label);
        //card.GetPhotonView().RPC("Initialize", RpcTarget.Others, false, label);
        //card.GetPhotonView().RPC("Initialize", player, true, label);
        SoundManager.PlaySound("cardTaken");
        // Animation: card move from enemy to deck.
        string color = GameManager.GetLocal().player.IsMasterClient ? "red" : "green";
        GameManager.instance.photonView.RPC("LogText", RpcTarget.All, GameManager.GetLocal().player.NickName + " takes 1 card from " + GameManager.GetRemote().player.NickName + "!", color);
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
        // Debug.LogFormat("PlayerController.Discard(), Num: {0}", num);
    }
}