using UnityEngine;

public class Limit : Card
{
    [SerializeField] private CreatureType blockedType = CreatureType.None;

    public bool Permits(Card card) {
        return !(card is Creature c && c.Type == blockedType);
    }
}