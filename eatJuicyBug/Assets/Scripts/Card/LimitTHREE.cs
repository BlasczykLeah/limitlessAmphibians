using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitTHREE : Limit
{

    public override bool CheckLimit(CardType type, CreatureType creature) {
        if (type != CardType.Creature) {
            return true;
        } else {
            if (creature == CreatureType.Axolotl) {
                return false;
            } else {
                return true;
            }
        }
    }

}
