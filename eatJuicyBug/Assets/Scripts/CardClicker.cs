using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardClicker : MonoBehaviour
{
    Image showCard;

    void Start()
    {
        showCard = GameObject.Find("CardShow").GetComponent<Image>();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //showCard.gameObject.SetActive(true);
            showCard.color = Color.white;
            showCard.sprite = GetComponent<SpriteRenderer>().sprite;
        }
    }

    void OnMouseExit()
    {
        showCard.color = Color.clear;
    }
}
