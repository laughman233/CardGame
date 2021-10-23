using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameController:SingletonAuto<GameController>
{
    private Transform panel;
    [HideInInspector] public GameObject selectedCard;
    [HideInInspector] public GameObject selectedPlayerCard;
    [HideInInspector] public GameObject selectedPosition;
    [HideInInspector] public GameObject selectedTarget;
    [HideInInspector] public GameObject lastSelectedCard;
    [HideInInspector]public Image[][] imageMap;
    [HideInInspector]public Image[] playerCards;
    [HideInInspector]public Player _player;
    private Button endTurn;
    private PhotonView view;
    public bool hasChanged;
    public Text roundText;
    public Text tips;
    private Text actionPoint;
    public bool hasStarted;
    private Button returnMenuButton;
    private Button helpBtn;
    public GameObject helpPanel;
    private void Start()
    {
        playerCards = new Image[4];
        panel = GameObject.Find("Panel").transform;
        helpPanel=GameObject.Find("HelpPanel");
        actionPoint = panel.Find("ActionPoint").GetComponent<Text>();
        actionPoint.text = "点数："+GameCore.GetInstance.CurrentAP;
        tips = panel.Find("Tips").GetComponent<Text>();
        roundText = panel.Find("Round").GetComponent<Text>();
        endTurn = panel.Find("EndTurn").GetComponent<Button>();
        helpBtn = panel.Find("HelpBtn").GetComponent<Button>();
        returnMenuButton = panel.Find("ReturnBtn").GetComponent<Button>();
        endTurn.onClick.AddListener(() =>
        {
            if(!CanMove)
                return;
            if (!hasChanged)
            {
                tips.text = "你还没有进行任何操作。";
                return;
            }
            _player.StoreRound();
            selectedCard = null;
            selectedPlayerCard = null;
            selectedPosition = null;
            selectedTarget = null;
            lastSelectedCard = null;
            hasChanged = false;
            actionPoint.text = "点数：" + GameCore.GetInstance.CurrentAP;
            actionPoint.gameObject.SetActive(false);
            endTurn.gameObject.SetActive(false);
        });
        endTurn.gameObject.SetActive(false);
        helpBtn.onClick.AddListener((
        )=>{helpPanel.SetActive(true);});
        helpPanel.SetActive(false);
        returnMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Menu");
        });
        for (int i = 0; i < 4; i++)
        {
            playerCards[i] = panel.Find("Player").GetChild(i).GetComponent<Image>();
        }
        imageMap = new Image[4][];
        imageMap[0] = new Image[8];
        imageMap[1] = new Image[4];
        imageMap[2] = new Image[4];
        imageMap[3] = new Image[8];
        CreateMap();
        UpdateCardAction();
    }

    void Update()
    {
        if (CanMove)
        {
            endTurn.gameObject.SetActive(true);
            actionPoint.gameObject.SetActive(true);
        }

        if (PhotonNetwork.CountOfPlayers < 2)
            hasStarted = false;
        else
        {
            hasStarted = true;
        }

        if (GameCore.GetInstance.Loss)
            tips.text = "你输了";
    }

    private int GetColumn(GameObject card)
    {
        return Convert.ToInt32(card.name);
    }

    private int GetRow(GameObject card)
    {
        return Convert.ToInt32(card.transform.parent.name);
    }

    /// <summary>
    /// To tell if it is player's turn
    /// </summary>
    public bool CanMove
    {
        get
        {
            if (PhotonNetwork.MasterClient.IsLocal && GameCore.GetInstance.round % 2 != 0)
                return true;
            if (!PhotonNetwork.MasterClient.IsLocal &&GameCore.GetInstance.round % 2 == 0)
                return true;
            return false;
        }
    }
    
    public void UpdateOpacity()
    {
        Image image;
        if(lastSelectedCard!=null){        
            image = lastSelectedCard.GetComponent<Image>();
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, 1);
        }
        if(selectedPlayerCard==null&&selectedCard!=null){
            image= selectedCard.GetComponent<Image>();
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, 0.5f);
        }
        if(selectedPlayerCard!=null&&selectedCard==null){
            image = selectedPlayerCard.GetComponent<Image>();
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, 0.5f);
        }
    }
  
    
    private void CreateMap()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < imageMap[i].Length; j++)
            {
                Image image = panel.Find(i.ToString()).Find(j.ToString()).GetComponent<Image>();
                imageMap[i][j] = image;
            }
        }
    }


    public Color CardColor2Color(Card.CardColor cardColor)
    {
        if (cardColor == Card.CardColor.White)
            return Color.white;
        if (cardColor == Card.CardColor.Red)
            return Color.red;
        if (cardColor == Card.CardColor.Orange)
            return new Color(255, 122.5f, 0);
        if (cardColor == Card.CardColor.Yellow)
            return Color.yellow;
        if (cardColor == Card.CardColor.Green)
            return Color.green;
        if (cardColor == Card.CardColor.Blue)
            return new Color(0, 122.5f, 255);
        if (cardColor == Card.CardColor.Indigo)
            return Color.blue;
        return new Color(122.5f, 0, 255);
    }
    public Vector2 GetPositionFromMap(GameObject card)
    {
        int i = int.Parse(card.transform.parent.name);
        int j = int.Parse(card.name);
        return new Vector2(i, j);
    }

    public Card GetCardFromMap(GameObject card)
    {
        int a, b;
        a = (int)GetPositionFromMap(card).x;
        b = (int)GetPositionFromMap(card).y;
        return GameCore.GetInstance.map[a][b];
    }

    public int GetPlayerCardIndex(GameObject cardGO)
    {
        int index=-1;
        for (int i = 0; i < 4; i++)
        {
            if (cardGO == playerCards[i].gameObject)
                index = i;
        }
        return index;
    }

    public void UpdateCardInfo(GameObject cardGO,bool isPlayerCard)
    {
        Card card;
        if (!isPlayerCard)
            card = GetCardFromMap(cardGO);
        else
            card = GameCore.GetInstance.playerCards[GetPlayerCardIndex(cardGO)];
        cardGO.transform.Find("Atk").GetComponent<Text>().text = card.atk.ToString();
        cardGO.transform.Find("Health").GetComponent<Text>().text = card.health.ToString();
    }

    public void UpdateCardAction()
    {
        if (selectedPlayerCard != null && selectedPosition != null)
        {
            Vector2 pos=GetPositionFromMap(selectedPosition);
            GameCore.GetInstance.Embattle(GetPlayerCardIndex(selectedPlayerCard),pos);
            selectedPlayerCard = null;
            selectedPosition = null;
            _player.StorePlayerInfo();
            hasChanged =true;
            actionPoint.text = "点数：" + GameCore.GetInstance.CurrentAP;
        }
        if (selectedCard != null && selectedTarget != null)
        {
            Vector2 attacker=GetPositionFromMap(selectedCard);
            Vector2 target=GetPositionFromMap(selectedTarget);
            GameCore.GetInstance.Attack(attacker,target);
            selectedCard = null;
            selectedTarget = null;
            _player.StorePlayerInfo();
            hasChanged = true;
        }
    }

    public void UpdateImage()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < GameCore.GetInstance.map[i].Length; j++)
            {
                Card.CardColor color = GameCore.GetInstance.map[i][j]._color;
                imageMap[i][j].color = CardColor2Color(color);
                UpdateCardInfo(imageMap[i][j].gameObject,false);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            playerCards[i].color = CardColor2Color(GameCore.GetInstance.playerCards[i]._color);
            UpdateCardInfo(playerCards[i].gameObject,true);
        }
        UpdateOpacity();
    }
}
