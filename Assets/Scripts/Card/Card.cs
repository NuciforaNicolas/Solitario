using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    public enum Suits 
    {
        Diamond,
        Spade,
        Club,
        Heart
    }
    public Suits suit;
    public enum Color
    {
        Red, Black
    }
    public Color color;

    public int number;
    [SerializeField] bool isDragging;
    [SerializeField] float offset;

    [SerializeField] Transform lastSlot;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    [SerializeField] Transform cardToFlip;
    [SerializeField] string drawnCardLayerName;
    [SerializeField] string cardLayerName;

    [SerializeField] Sprite faceSprite, backSprite;

    public bool isCovered;
    public bool hasCardAbove;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        // Card is covered at the beginning
        spriteRenderer.sprite = backSprite;
        isCovered = true;
        cardLayerName = "Card";
        drawnCardLayerName = "DrawnCard";

        color = (suit == Suits.Heart || suit == Suits.Diamond) ? Color.Red : Color.Black;

        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update()
    {
        if(isDragging && !CardsManager.instance.isSettingGame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePos);
        }
    }

    public void SetHasCardAbove(bool val)
    {
        hasCardAbove = val;
    }
    public bool GetHasCardAbove()
    {
        return hasCardAbove;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Slot") && isDragging)
        {
            SetLastSlot(collision.transform);
        }
        if(collision.tag.Equals("Card") && isDragging)
        {
            var slotGO = collision.GetComponent<Card>();
            if(!slotGO.isCovered && (slotGO.color != color) && ((slotGO.number-1) == number) /*&& !slotGO.GetLastSlot().tag.Equals("DrawnDeck")*/)
            {
                SetLastSlot(collision.transform);
            }
        }
    }

    private void OnMouseDown()
    {

        if(!isCovered)
        {
            isDragging = true;
            spriteRenderer.sortingLayerName = drawnCardLayerName;
            //spriteRenderer.sortingOrder--;

            if (transform.parent != null)
            {
                transform.parent = null;
            }
            if (lastSlot != null && lastSlot.tag.Equals("Slot"))
            {
               /* if(lastSlot.tag.Equals("Card"))
                {
                    lastSlot.GetComponent<Card>().hasCardAbove = false;
                }*/
                lastSlot.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    private void OnMouseUp()
    {
        if (lastSlot != null && isDragging)
        {
            Vector2 newPos = lastSlot.position;
            if (lastSlot.tag.Equals("Card"))
            {
                newPos = new Vector3(lastSlot.position.x, lastSlot.position.y - offset);
                transform.parent = lastSlot;
            }
            else if(lastSlot.tag.Equals("Slot")){
                lastSlot.GetComponent<BoxCollider2D>().enabled = false;
            }
            else if(lastSlot.tag.Equals("DrawnDeck"))
            {
                CardsManager.instance.PutAboveDranwStack(gameObject);
            }
            transform.position = newPos;
            PutAboveLastSlot();
            if(cardToFlip != null)
            {
                cardToFlip.GetComponent<Card>().FlipCard();
                cardToFlip = null;
            }
        }
        isDragging = false;
    }

    public void PutAboveLastSlot()
    {
        if(lastSlot.tag.Equals("Card"))
        {
            spriteRenderer.sortingOrder = lastSlot.GetComponent<SpriteRenderer>().sortingOrder + 1;
            //lastSlot.GetComponent<Card>().hasCardAbove = true;
        }
        spriteRenderer.sortingLayerName = cardLayerName;
    }

    public void SetLastSlot(Transform slot)
    {
        if (lastSlot != null && lastSlot.tag.Equals("DrawDeck"))
        {
            Debug.Log("Inserendo la carta " + name + " pescata da DrawnDeck sopra la carta " + slot.name);
            CardsManager.instance.DrawACardFromDrawnDeck();
        }
        else if(lastSlot != null && lastSlot.tag.Equals("Card") && lastSlot.GetComponent<Card>().isCovered)
        {
            cardToFlip = lastSlot;
        }

        lastSlot = slot;
    }

    public Transform GetLastSlot()
    {
        return lastSlot;
    }

    public void FlipCard()
    {
        /*if(!boxCollider.enabled)
        {
            boxCollider.enabled = true;
        }*/
        boxCollider.enabled = true;
        //transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        //transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<SpriteRenderer>().sprite = faceSprite;
        transform.GetComponent<Card>().isCovered = false;
        Debug.Log("Ho scoperto la carta: " + name);
    }

    public void PutToStack()
    {
        isDragging = false;
        boxCollider.enabled = false;
        spriteRenderer.sortingLayerName = cardLayerName;
        if(lastSlot != null && lastSlot.tag.Equals("Card") && lastSlot.GetComponent<Card>().isCovered)
        {
            lastSlot.GetComponent<Card>().FlipCard();
        }
        if(cardToFlip != null)
        {
            cardToFlip.GetComponent<Card>().FlipCard();
        }
    }
}
