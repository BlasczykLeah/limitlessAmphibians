using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WinCon", menuName = "Wincon")]
public class WinCondition : ScriptableObject
{
    [Header("IMAGE FOR SPRITE")]
    public UnityEngine.UI.Image sprite;

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
