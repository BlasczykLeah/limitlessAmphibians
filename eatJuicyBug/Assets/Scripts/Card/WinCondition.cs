﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New WinCon", menuName = "Wincon")]
public class WinCondition : ScriptableObject
{

    public Sprite sprite;

    [Header("Frog")]
    public int frog;
    [Header("Dragon")]
    public int dragon;
    [Header("Gator")]
    public int gator;
    [Header("Azototl")]
    public int azolotl;
    [Header("Lizard")]
    public int lizard;
    [Header("Dino")]
    public int dino;

}
