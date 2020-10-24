using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitTWO : Limit
{
    //cannot play dragons
    public override bool CheckLimit(CardType type, CreatureType creature) {
        if (type != CardType.Creature) {
            return true;
        } else {
            if (creature == CreatureType.Dragon) {
                return false;
            } else {
                return true;
            }
        }
    }

}
