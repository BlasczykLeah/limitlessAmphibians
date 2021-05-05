
public class Deck : SelectArray<Card>
{
    public Deck(Card[] cards) : base(cards)
    {
    }

    public Card Draw()
    {
        return SkipFirst();
    }
}
