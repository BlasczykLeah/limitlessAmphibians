using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CardClicker : MonoBehaviour
{
    public bool played = false;
    private Vector3 initialLocalPosition;
    private int initialSortingOrder;
    public bool hoverEffectEnabled = false;
    private readonly int hoverSortingOrderGain = 10;
    private readonly float hoverTranslation = 0.4f;

    Image showCard;
    UnityEvent m_event = new UnityEvent();

    void Start()
    {
        showCard = GameObject.Find("CardShow").GetComponent<Image>();
        m_event.AddListener(PlayCard);

        if(hoverEffectEnabled)
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.size = new Vector3(boxCollider.size.x * 0.5f, boxCollider.size.y, boxCollider.size.z);   
        }
    }

    void OnMouseEnter()
    {
        if(hoverEffectEnabled)
        {
            initialLocalPosition = transform.localPosition;
            initialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
            transform.Translate(0, hoverTranslation, 0, Space.Self);
            GetComponent<SpriteRenderer>().sortingOrder = initialSortingOrder + hoverSortingOrderGain;
        }
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
        if(hoverEffectEnabled)
        {
            transform.localPosition = initialLocalPosition;
            GetComponent<SpriteRenderer>().sortingOrder = initialSortingOrder;
        }
        showCard.color = Color.clear;
    }

    void PlayCard() {
        if(!(GameManager.instance is null))
        {
            GameManager.instance.playCard(gameObject.GetComponent<Card>());
        }
        else if(!(LocalGameManager.instance is null))
        {
            LocalGameManager.instance.PlayCard(gameObject.GetComponent<Card>());
        }
        //Networking.server.playCard(gameObject.GetComponent<CardData>().cardName);
    }
}
