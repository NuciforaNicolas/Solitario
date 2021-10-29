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

    public bool isCovered;
    public bool hasCardAbove;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        // Card is covered at the beginning
        spriteRenderer.enabled = false;
        isCovered = true;

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
        if(collision.tag.Equals("Slot") && isDragging)
        {
            SetLastSlot(collision.transform);
        }
        if(collision.tag.Equals("Card") && isDragging)
        {
            var slotGO = collision.GetComponent<Card>();
            if(!slotGO.isCovered && (slotGO.color != color) && ((slotGO.number-1) == number))
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
            //spriteRenderer.sortingOrder = 1;

            if (transform.parent != null)
            {
                transform.parent = null;
            }
            if (lastSlot != null)
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
                newPos = new Vector2(lastSlot.position.x, lastSlot.position.y - offset);
                transform.parent = lastSlot;
            }
            else{
                lastSlot.GetComponent<BoxCollider2D>().enabled = false;
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
    }

    public void SetLastSlot(Transform slot)
    {
        if(lastSlot != null && lastSlot.tag.Equals("Card") && lastSlot.GetComponent<Card>().isCovered)
        {
            cardToFlip = lastSlot;
        }
        lastSlot = slot;
    }

    public void FlipCard()
    {
        if(!boxCollider.enabled)
        {
            boxCollider.enabled = true;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<Card>().isCovered = false;
        Debug.Log("Ho scoperto la carta: " + name);
    }

    public void PutToStack()
    {
        isDragging = false;
        boxCollider.enabled = false;
        spriteRenderer.sortingOrder = 0;
        if(lastSlot != null && lastSlot.tag.Equals("Card") && lastSlot.GetComponent<Card>().isCovered)
        {
            lastSlot.GetComponent<Card>().FlipCard();
        }
    }
}
