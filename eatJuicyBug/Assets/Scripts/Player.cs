using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public string name;
    public List<Card> Hand = new List<Card>();
    public WinCondition winCon = null;
    public Limit limit = null;
    public string id;
}
