using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class CardClicker : MonoBehaviour
{
    Image showCard;
    UnityEvent m_event = new UnityEvent();
    void Start()
    {
        showCard = GameObject.Find("CardShow").GetComponent<Image>();
        m_event.AddListener(PlayCard);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //showCard.gameObject.SetActive(true);
            showCard.color = Color.white;
            showCard.sprite = GetComponent<SpriteRenderer>().sprite;
        } else if(Input.GetMouseButtonDown(0)){
            if(m_event != null) {
                m_event.Invoke();
            }
        }
    }

    void OnMouseExit()
    {
        showCard.color = Color.clear;
    }

    void PlayCard() {
        Networking.server.playCard(gameObject.GetComponent<CardData>().cardName);
    }
}
