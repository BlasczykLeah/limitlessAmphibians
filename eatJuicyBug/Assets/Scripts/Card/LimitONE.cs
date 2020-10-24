using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitONE : Limit
{
    //NO FROGS. MUAHAHAHA
    public override bool CheckLimit(CardType type, CreatureType creature) {
        if (type != CardType.Creature) {
            return true;
        } else {
            if (creature == CreatureType.Frog) {
                return false;
            } else {
                return true;
            }
        }
    }

}
