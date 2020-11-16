using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public static readonly int placedLimit = 6;

    public string name;
    public List<Card> hand = new List<Card>();
    public List<Card> placed = new List<Card>();
    public WinCondition winCon = null;
    public Limit limit = null;
    public string id;
    public bool myTurn = false;
    public int cardsOnTable = 0;
    public bool disconnected = false;

    public Dictionary<CreatureType, int> creatureAmounts = new Dictionary<CreatureType, int>();

    public bool IsWinner => !(winCon is null) && creatureAmounts.Fulfills(winCon.requirements);

    public void Play(int index)
    {
        if(index < 0 || index >= hand.Count)
            return;

        if(!(hand[index] is Creature c))
            return;

        if(placed.Count >= placedLimit)
            return;

        placed.Add(hand[index]);
        hand.RemoveAt(index);
        creatureAmounts.Increment(c.Type);
    }

    public void DiscardFromHand(int index)
    {
        if(index < 0 || index >= hand.Count)
            return;

        hand.RemoveAt(index);
    }

    public void DiscardFromHand(CreatureType type)
    {
        int index = hand.FindIndex(card => card is Creature c && c.Type == type);
        if(index < 0)
            return;

        hand.RemoveAt(index);
    }

    public void DiscardFromPlaced(CreatureType type)
    {
        int index = placed.FindIndex(card => card is Creature c && c.Type == type);
        if(index < 0)
            return;

        placed.RemoveAt(index);
        creatureAmounts[type]--;
    }

    public void DiscardAllFromPlaced(CreatureType type)
    {
        creatureAmounts[type] -= placed.RemoveAll(card => card is Creature c && c.Type == type);
    }
}
