using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class CardClicker : MonoBehaviour
{
    public bool played = false;

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
        } else if(Input.GetMouseButtonDown(0) && !played){
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
        GameManager.instance.playCard(gameObject);
        //Networking.server.playCard(gameObject.GetComponent<CardData>().cardName);
    }
}
