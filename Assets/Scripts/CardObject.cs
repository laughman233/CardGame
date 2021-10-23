using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameController.GetInstance.tips.text = "";
            if (GameController.GetInstance.CanMove&&GameController.GetInstance.hasStarted)
            {
                if (transform.parent.name == "Player")
                {
                    if (GameController.GetInstance.selectedPlayerCard == gameObject)
                        GameController.GetInstance.selectedPlayerCard = null;
                    else
                    {
                        GameController.GetInstance.selectedPlayerCard = gameObject;
                        GameController.GetInstance.selectedCard = null;
                        GameController.GetInstance.tips.text = "请选择部署位置，再次选择取消选中。\n"+ShowPlayerCardInfo(GameController.GetInstance.selectedPlayerCard);
                    }
                }
                else if (transform.parent.name == "1" || transform.parent.name == "0")
                {
                    if (GameController.GetInstance.selectedCard == gameObject)
                        GameController.GetInstance.selectedCard = null;
                    else if (_image.color != Color.white &&
                             GameController.GetInstance.selectedPlayerCard == null)
                    {
                        GameController.GetInstance.selectedCard = gameObject;
                        if(GameCore.GetInstance.CanAttack(GameController.GetInstance.GetPositionFromMap(gameObject)))
                        {
                            GameController.GetInstance.tips.text =
                                "请选择要攻击的目标。\n" + ShowCardInfo(gameObject);
                        }
                        else if(GameController.GetInstance.GetCardFromMap(gameObject).embattleRound==GameCore.GetInstance.round)
                            GameController.GetInstance.tips.text="此牌在睡眠中。\n" + ShowCardInfo(GameController.GetInstance.selectedCard);
                        else
                        {
                            GameController.GetInstance.tips.text =
                                "此卡已经攻击过，无法攻击。" + ShowCardInfo(gameObject);
                        }
                    }
                    else if (GameController.GetInstance.selectedPlayerCard != null && _image.color == Color.white)
                        GameController.GetInstance.selectedPosition = gameObject;
                }
                else if (transform.parent.name == "3" || transform.parent.name == "2")
                {
                    if (GameController.GetInstance.selectedCard != null && _image.color != Color.white)
                        GameController.GetInstance.selectedTarget = gameObject;
                    else if(_image.color!=Color.white)
                    {
                        GameController.GetInstance.tips.text = ShowCardInfo(gameObject);
                    }
                }
                GameController.GetInstance.UpdateOpacity();
                GameController.GetInstance.UpdateCardAction();
                if (GameController.GetInstance.lastSelectedCard != gameObject)
                {
                    GameController.GetInstance.lastSelectedCard = gameObject;
                }
                else
                {
                    GameController.GetInstance.lastSelectedCard = null;
                }
            }
        });
    }

    private string ShowCardInfo(GameObject cardGameObject)
    {
        Card card=GameController.GetInstance.GetCardFromMap(cardGameObject);
        return "攻击：" + card.atk + " 血量：" + card.health + " 远程：" + card.rangeAttack + " 二连击：" + card.atkTwice;
    }

    private string ShowPlayerCardInfo(GameObject playerCardGO)
    {
        Card card = GameCore.GetInstance.playerCards[GameController.GetInstance.GetPlayerCardIndex(playerCardGO)];
        return "攻击：" + card.atk + " 血量：" + card.health + " 远程：" + card.rangeAttack + " 二连击：" + card.atkTwice+" 卡费："+card.cost;
    }
}