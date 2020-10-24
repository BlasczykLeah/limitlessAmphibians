using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitSIX : Limit
{

    public override bool CheckLimit(CardType type, CreatureType creature) {
        if (type != CardType.Creature) {
            return true;
        } else {
            if (creature == CreatureType.Dino) {
                return false;
            } else {
                return true;
            }
        }
    }

}
