using UnityEngine;

public class Limit : MonoBehaviour
{
    [SerializeField] private CreatureType blockedType;

    public bool Permits(CardType type, CreatureType creature) {
        return type != CardType.Creature || creature != blockedType;
    }
}