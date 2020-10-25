using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCreature : CardEffect
{
    [SerializeField] CreatureType type;
    /// <summary>
    /// When you draw a creature, this method will determine which kind of creature it is.
    /// </summary>
    public override void DetermineCreatureType(CreatureType myType) {

        type = myType;

    }
    public override void PlayCard() {
        //GameManager.instance.PlayCreature(type);
    }
}
