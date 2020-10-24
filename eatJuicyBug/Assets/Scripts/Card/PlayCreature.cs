using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCreature : CardEffect
{

    public override void PlayCard() {
        GameManager.instance.PlayCreature();
    }
}
