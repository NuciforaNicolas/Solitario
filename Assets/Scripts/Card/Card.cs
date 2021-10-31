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

    public bool isCovered, canIncreasePoint, increasePoint;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        // Card is covered at the beginning
        spriteRenderer.sprite = backSprite;
        isCovered = true;
        canIncreasePoint = true;
        increasePoint = false;
        cardLayerName = "Card";
        drawnCardLayerName = "DrawnCard";

        color = (suit == Suits.Heart || suit == Suits.Diamond) ? Color.Red : Color.Black;

        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update()
    {
        // Drag and drop delle carte
        if(isDragging && !CardsManager.instance.isSettingGame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePos);
        }
    }

    // Controlla se la carta pescata finisce al di sopra di un'altra
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se la carta è un re ed è al di sopra di una colonna libera, è possibile posizionarla di sopra (tutte le altre non possono)
        if (collision.tag.Equals("Slot") && number == 13 && isDragging)
        {
            SetLastSlot(collision.transform);
        }
        // Se si trova al di sopra di un'altra carta...
        if(collision.tag.Equals("Card") && isDragging)
        {
            // ... e non è coperta, di colore diverso e di un numero superiore, allora è possibile posizionarla di sopra
            // Controllo se è diverso da DrawDeck (pila di carte pescate) per impedire alla carta di essere posizionata al di sopra di una carta nella pila
            var slotGO = collision.GetComponent<Card>();
            if((!slotGO.isCovered) && (slotGO.color != color) && ((slotGO.number-1) == number) && (!slotGO.GetLastSlot().tag.Equals("DrawDeck")))
            {
                // Per ogni carta, l'incremento del punteggio avviene una sola volta per spostamento corretto
                if(canIncreasePoint)
                {
                    increasePoint = true;
                    canIncreasePoint = false;
                }
                SetLastSlot(collision.transform);
            }
        }
    }

    // Inizia a trascinare la carta
    private void OnMouseDown()
    {

        if(!isCovered)
        {
            isDragging = true;
            spriteRenderer.sortingLayerName = drawnCardLayerName;
            //spriteRenderer.sortingOrder--;

            // Scollego la carta dalle altre (nel caso in cui la carta è gia all0interno di una pila di carte ordinate)
            if (transform.parent != null)
            {
                transform.parent = null;
            }
            // Se la carta si trova nel primo slot della colonna riattiva il trigger dello slot
            if (lastSlot != null && lastSlot.tag.Equals("Slot"))
            {
                lastSlot.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    // Posa la carta che si stava trascinando
    private void OnMouseUp()
    {
        if (lastSlot != null && isDragging)
        {
            Vector2 newPos = lastSlot.position;
            // Setta la nuova posizione della carta. Se deve essere posizionata al di sopra di un'altra carta, abbassala di un certo offset
            if (lastSlot.tag.Equals("Card"))
            {
                newPos = new Vector3(lastSlot.position.x, lastSlot.position.y - offset);
                transform.parent = lastSlot;
                if (increasePoint)
                {
                    GameManager.instance.IncreasePoints();
                    increasePoint = false;
                }
            }
            // Se è uno slot, disattiva il trigger per impedire di posizionarci altre carte
            else if(lastSlot.tag.Equals("Slot")){
                lastSlot.GetComponent<BoxCollider2D>().enabled = false;
            }
            // Riposiziona la carta nella pila delle carte pescate (avviene se si rilascia il dito e non è stato trovato un nuovo slot)
            else if(lastSlot.tag.Equals("DrawnDeck"))
            {
                CardsManager.instance.PutAboveDranwStack(gameObject);
            }
            // Setta la posizione dell'ultimo slot trovato
            transform.position = newPos;
            PutAboveLastSlot();
            // Se una carta coperta si trova libera dopo aver posizionato la carta selezionata, girala
            if(cardToFlip != null)
            {
                cardToFlip.GetComponent<Card>().FlipCard();
                cardToFlip = null;
            }
            GameManager.instance.IncreaseMoves();
        }
        isDragging = false;
    }

    // Posiziona la carta al di sopra di un'altra incrementando il sortingOrder
    public void PutAboveLastSlot()
    {
        spriteRenderer.sortingLayerName = cardLayerName;
        if (lastSlot.tag.Equals("Card"))
        {
            spriteRenderer.sortingOrder = lastSlot.GetComponent<SpriteRenderer>().sortingOrder + 1;
            //lastSlot.GetComponent<Card>().hasCardAbove = true;
        }
    }

    // Setta l'ultimo slot eleggibile
    public void SetLastSlot(Transform slot)
    {
        if (lastSlot != null && lastSlot.tag.Equals("DrawDeck"))
        {
            Debug.Log("Inserendo la carta " + name + " pescata da DrawnDeck sopra la carta " + slot.name);
            CardsManager.instance.DrawACardFromDrawnDeck();
        }
        // Prima di cambiare slot, controlla se al di sotto si ha una carta coperta. se si, bisogna girarla
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

    // Gira la carta coperta
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

    // Posiziona la carta nello stack dei semi
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
