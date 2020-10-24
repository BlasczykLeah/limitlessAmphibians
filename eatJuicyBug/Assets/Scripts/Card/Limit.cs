using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Limit : MonoBehaviour
{
    
    public virtual bool CheckLimit(CardType type, CreatureType creature) {
        return true;
    }
    
}
