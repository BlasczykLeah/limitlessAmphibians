using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLimit : CardEffect
{
    public override void PlayCard() {
        GameManager.instance.PlayLimit();
    }


}
