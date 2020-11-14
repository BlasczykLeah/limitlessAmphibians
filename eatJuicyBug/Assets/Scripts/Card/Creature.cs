using UnityEngine;

public class Creature : Card
{
    [SerializeField] private CreatureType type = CreatureType.None;

    public CreatureType Type => type;
}
