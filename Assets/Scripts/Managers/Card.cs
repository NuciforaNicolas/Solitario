using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    public enum Suits{
        Diamond,
        Spade,
        Club,
        Heart
    }
    public Suits suit;
    public int number;
    [SerializeField] bool isDragging, hasCardAbove;
    [SerializeField] float offset;

    [SerializeField] Transform lastSlot;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(isDragging)
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
        if(collision.tag.Equals("Slot") || (collision.tag.Equals("Card") && isDragging /*&& !collision.GetComponent<Card>().GetHasCardAbove()*/))
        {
            Debug.Log("Collision detected from " + name + ": " + collision.name);
            lastSlot = collision.transform;
        }
    }

    private void OnMouseDown()
    {
        /*if(!hasCardAbove)
        {
            
        }*/

        isDragging = true;
        if (spriteRenderer.sortingOrder > 1)
        {
            spriteRenderer.sortingOrder--;
        }
        if (lastSlot != null)
        {
            /*if (lastSlot.tag.Equals("Card"))
            {
                lastSlot.GetComponent<Card>().SetHasCardAbove(false);
            }*/
            lastSlot.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void OnMouseUp()
    {
        if (lastSlot != null && isDragging)
        {
            Vector2 newPos = lastSlot.position;
            if(lastSlot.tag.Equals("Card")){
                newPos = new Vector2(lastSlot.position.x, lastSlot.position.y - offset);
                spriteRenderer.sortingOrder = lastSlot.GetComponent<SpriteRenderer>().sortingOrder + 1;
                //lastSlot.GetComponent<Card>().SetHasCardAbove(true);
            }
            transform.position = newPos;
            lastSlot.GetComponent<BoxCollider2D>().enabled = false;
        }
        isDragging = false;
    }
}
