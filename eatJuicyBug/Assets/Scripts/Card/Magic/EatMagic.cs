using System.Collections.Generic;

public class EatMagic : Magic
{
    public void DoMagic(Deck deck, List<Card> hand)
    {
        Card c = deck.SkipFirstWhere(card => card is Creature creature && creature.Type == creatureType);
        if(!(c is null))
        {
            hand.Add(c);
        }
    }
}
