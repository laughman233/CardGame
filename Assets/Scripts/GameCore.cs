using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameCore : BaseManager<GameCore>
{
    private int actionPoint = 4;
    private int currentAP;
    private int pointIncrease = 1;
    private int cardCount = 46;
    private int maxActionPoint = 10;
    public List<Card> playerCards = new List<Card>();
    public Card[][] map = new Card[4][];
    public int round = 1;

    public int CurrentAP
    {
        get { return currentAP; }
    }
    public int ActionPoint
    {
        get { return actionPoint; }
    }

    public int PointIncrease
    {
        get { return pointIncrease; }
    }

    public int CardCount
    {
        get { return cardCount; }
    }

    public int MaxCardPoint
    {
        get { return maxActionPoint; }
    }

    public GameCore()
    {
        StartGame();
    }

    public void StartGame()
    {
        map[0] = new Card[8];
        map[1] = new Card[4];
        map[2] = new Card[4];
        map[3] = new Card[8];
        playerCards.Add(new RedCard());
        playerCards.Add(new RedCard());
        playerCards.Add(new YellowCard());
        playerCards.Add(new YellowCard());
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                map[i][j] = new Card();
            }
        }

        currentAP = actionPoint;
    }

    public bool Embattle(int playerCardIndex, Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (x > 1)
            return false;
        if (map[x][y]._color == Card.CardColor.White && currentAP >= playerCards[playerCardIndex].cost)
        {
            map[x][y] = playerCards[playerCardIndex];
            currentAP -= playerCards[playerCardIndex].cost;
            playerCards.RemoveAt(playerCardIndex);
            playerCards.Add(GenerateCard());
            map[x][y].embattleRound = round;
            return true;
        }

        return false;
    }

    public bool Attack(Vector2 attacker, Vector2 target)
    {
        int x1 = (int)attacker.x;
        int x2 = (int)target.x;
        int y1 = (int)attacker.y;
        int y2 = (int)target.y;
        if (x1 > 1 || x2 < 2)
            return false;
        if (x2 == 3&&!map[x1][y1].rangeAttack)
        {
            foreach (var VARIABLE in map[2])
            {
                if (VARIABLE._color != Card.CardColor.White)
                    return false;
            }
        }

        if (CanAttack(attacker, target))
        {
            if (map[x1][y1].rangeAttack)
            {
                int x = Random.Range(1, 101);
                if (x > 100 * map[x1][y1].miss)
                    map[x2][y2].health -= map[x1][y1].atk;
            }
            else if (map[x2][y2].rangeAttack)
            {
                map[x1][y1].health -= map[x2][y2].atk - 1;
                map[x2][y2].health -= map[x1][y1].atk;
            }
            else
            {
                map[x2][y2].health -= map[x1][y1].atk;
                map[x1][y1].health -= map[x2][y2].atk;
            }
            if (map[x1][y1].hasAttackedOnce&&map[x1][y1].atkTwice)
            {
                map[x1][y1].hasAttackedTwice = true;
                if(!map[x1][y1].rangeAttack)
                    map[x1][y1].health += map[x2][y2].atk;
            }
            if (!map[x1][y1].hasAttackedOnce)
                map[x1][y1].hasAttackedOnce = true;
            if (map[x1][y1].health <= 0 && map[x2][y2].health <= 0)
            {
                map[x1][y1] = new Card();
                map[x2][y2] = new Card();
            }
            else if (map[x1][y1].health <= 0)
            {
                map[x2][y2]=ColorReaction(map[x1][y1]._color, map[x2][y2]._color, map[x2][y2]);
                map[x1][y1] = new Card();
            }
            else if (map[x2][y2].health <= 0)
            {
                map[x1][y1]=ColorReaction(map[x1][y1]._color, map[x2][y2]._color, map[x1][y1]);
                map[x2][y2] = new Card();
            }
            return true;
        }

        return false;
    }

    public bool CanAttack(Vector2 attacker)
    {
        int x1 = (int)attacker.x;
        int y1 = (int)attacker.y;
        if (map[x1][y1].embattleRound == round)
            return false;
        return map[x1][y1]._color != Card.CardColor.White&&
               (!map[x1][y1].hasAttackedOnce ||
                map[x1][y1].hasAttackedOnce && !map[x1][y1].hasAttackedTwice && map[x1][y1].atkTwice);
    }
    public bool CanAttack(Vector2 attacker, Vector2 target)
    {
        int x1 = (int)attacker.x;
        int x2 = (int)target.x;
        int y1 = (int)attacker.y;
        int y2 = (int)target.y;
        if (map[x1][y1].embattleRound == round)
            return false;
        return map[x1][y1]._color != Card.CardColor.White && map[x2][y2]._color != Card.CardColor.White &&
               (!map[x1][y1].hasAttackedOnce ||
                map[x1][y1].hasAttackedOnce && !map[x1][y1].hasAttackedTwice && map[x1][y1].atkTwice);
    }

    public void VerticalReverse()
    {
        Card[][] temp = new Card[4][];
        for (int i = 0; i < 4; i++)
        {
            temp[i] = map[3 - i];
        }

        map = temp;
    }

    public Card GenerateCard()
    {
        cardCount--;
        if ((Card.CardColor)Mathf.Pow(4, Random.Range(0, 4)) == Card.CardColor.Red)
            return new RedCard();
        if ((Card.CardColor)Mathf.Pow(4, Random.Range(0, 4)) == Card.CardColor.Yellow)
            return new YellowCard();
        if ((Card.CardColor)Mathf.Pow(4, Random.Range(0, 4)) == Card.CardColor.Blue)
            return new BlueCard();
        return new PurpleCard();
    }

    public bool Loss
    {
        get
        {
            if (round <=2)
                return false;
            foreach (var VARIABLE in map[0])
            {
                if (VARIABLE._color != Card.CardColor.White)
                    return false;
            }

            foreach (var VARIABLE in map[1])
            {
                if (VARIABLE._color != Card.CardColor.White)
                    return false;
            }
            return true;
        }
    }
    private Card ColorReaction(Card.CardColor color1, Card.CardColor color2,Card reactionCard)
    {
        if (color1 == Card.CardColor.Red && color2 == Card.CardColor.Yellow||color2 == Card.CardColor.Red && color1 == Card.CardColor.Yellow)
            reactionCard = new OrangeCard();
        if (color1 == Card.CardColor.Yellow && color2 == Card.CardColor.Blue||color2 == Card.CardColor.Yellow && color1 == Card.CardColor.Blue)
            reactionCard = new GreenCard();
        if (color1 == Card.CardColor.Blue && color2 == Card.CardColor.Purple||color1 == Card.CardColor.Purple&&color2== Card.CardColor.Blue)
            reactionCard = new IndigoCard();
        return reactionCard;
    }

    public int EndTurn()
    {
        if (actionPoint < maxActionPoint)
            actionPoint++;
        currentAP = actionPoint;
        foreach (var cards in map)
        {
            foreach (var VARIABLE in cards)
            {
                VARIABLE.hasAttackedOnce = false;
                VARIABLE.hasAttackedTwice = false;
            }
        }
        return ++round;
    }
}

public class Card
{
    public int cost;
    public int health;
    public int atk;
    public float miss;
    public bool rangeAttack;
    public bool atkTwice;
    public CardColor _color = CardColor.White;
    public bool hasAttackedOnce;
    public bool hasAttackedTwice;
    public int embattleRound;

    public enum CardColor
    {
        White = 0,
        Red = 1,
        Orange = 2,
        Yellow = 4,
        Green = 8,
        Blue = 16,
        Indigo = 32,
        Purple = 64
    }
}

[Serializable]
public class YellowCard : Card
{
    public YellowCard()
    {
        _color = CardColor.Yellow;
        cost = 5;
        atk = 3;
        health = 3;
    }
}

[Serializable]
public class RedCard : Card
{
    public RedCard()
    {
        _color = CardColor.Red;
        cost = 4;
        atk = 2;
        health = 6;
    }
}

[Serializable]
public class BlueCard : Card
{
    public BlueCard()
    {
        _color = CardColor.Blue;
        atk = 2;
        health = 3;
        cost = 6;
        atkTwice = true;
    }
}

[Serializable]
public class PurpleCard : Card
{
    public PurpleCard()
    {
        _color = CardColor.Purple;
        atk = 2;
        health = 3;
        cost = 7;
        rangeAttack = true;
        miss = 0.58f;
    }
}

[Serializable]
public class OrangeCard : Card
{
    public OrangeCard()
    {
        _color = CardColor.Orange;
        atk = 3;
        health = 6;
    }
}

[Serializable]
public class GreenCard : Card
{
    public GreenCard()
    {
        _color = CardColor.Green;
        atkTwice = true;
        atk = 3;
        health = 3;
    }
}

[Serializable]
public class IndigoCard : Card
{
    public IndigoCard()
    {
        _color = CardColor.Indigo;
        atkTwice = true;
        rangeAttack = true;
        miss = 0.58f;
        atk = 2;
        health = 3;
    }
}