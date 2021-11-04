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
        // Non deve scattare se stiamo posizionando le carte ad inizio gioco, in quanto Last slot viene gia settato dal cardManager
        if(!CardsManager.instance.isSettingGame)
        {
            // Se la carta è un re ed è al di sopra di una colonna libera, è possibile posizionarla di sopra (tutte le altre non possono)
            if (collision.tag.Equals("Slot") && number == 13 && isDragging)
            {
                CheckCardToFlip();
                SetLastSlot(collision.transform);
            }
            // Se si trova al di sopra di un'altra carta...
            if (collision.tag.Equals("Card") && isDragging)
            {
                // ... e non è coperta, di colore diverso e di un numero superiore, allora è possibile posizionarla di sopra
                // Controllo se è diverso da DrawDeck (pila di carte pescate) per impedire alla carta di essere posizionata al di sopra di una carta nella pila
                // inoltre, controllo se la carta scoperta su cui vogliamo posizionarla non abbia carte figlie (ovvero, è scoperta e non ha una pila al di sotto)
                var slotGO = collision.GetComponent<Card>();
                if ((collision.transform.childCount == 0) && (!slotGO.isCovered) && (slotGO.color != color) && ((slotGO.number - 1) == number) && (!slotGO.GetLastSlot().tag.Equals("DrawDeck")))
                {
                    CheckCardToFlip();

                    SetLastSlot(collision.transform);
                }
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
            else if (lastSlot != null && lastSlot.tag.Equals("DrawDeck"))
            {
                //Debug.Log("Inserendo la carta " + name + " pescata da DrawnDeck sopra la carta " + slot.name);
                // Se la carta viene spostata dal drawDeck ad una colonna, è possibile incrementare il punteggio
                if (canIncreasePoint)
                {
                    increasePoint = true;
                    canIncreasePoint = false;
                }
                CardsManager.instance.DrawACardFromDrawnDeck();
            }
        }
    }

    // Posa la carta che si stava trascinando
    private void OnMouseUp()
    {
        if (lastSlot != null && isDragging)
        {
            MoveCardToLastSlot();   
            GameManager.instance.IncreaseMoves();
        }
        isDragging = false;
    }

    public void MoveCardToLastSlot()
    {
        Vector2 newPos = lastSlot.position;
        transform.parent = lastSlot;
        // Setta la nuova posizione della carta. Se deve essere posizionata al di sopra di un'altra carta, abbassala di un certo offset
        if (lastSlot.tag.Equals("Card"))
        {
            newPos = new Vector3(lastSlot.position.x, lastSlot.position.y - offset);

            // Incrementa il punteggio se la carta è stata spostata dal drawDeck ad una colonna
            if (increasePoint)
            {
                GameManager.instance.IncreasePoints(5);
                increasePoint = false;
            }
        }
        // Se è uno slot, disattiva il trigger per impedire di posizionarci altre carte
        else if (lastSlot.tag.Equals("Slot"))
        {
            lastSlot.GetComponent<BoxCollider2D>().enabled = false;
        }
        // Riposiziona la carta nella pila delle carte pescate (avviene se si rilascia il dito e non è stato trovato un nuovo slot)
        else if (lastSlot.tag.Equals("DrawDeck"))
        {
            // Rimetti a false, in quanto la carta è tornata nel drawDeck
            canIncreasePoint = false;
            increasePoint = true;
            CardsManager.instance.PutAboveDranwStack(gameObject);
        }
        // Setta la posizione dell'ultimo slot trovato
        transform.position = newPos;
        PutAboveLastSlot();
        // Se una carta coperta si trova libera dopo aver posizionato la carta selezionata, girala
        if (cardToFlip != null)
        {
            cardToFlip.GetComponent<Card>().FlipCard();
            cardToFlip = null;
        }
    }

    // Posiziona la carta selezionata al di sopra di un'altra carta scoperta incrementando il sortingOrder
    public void PutAboveLastSlot()
    {
        spriteRenderer.sortingLayerName = cardLayerName;
        if (lastSlot.tag.Equals("Card"))
        {
            spriteRenderer.sortingOrder = lastSlot.GetComponent<SpriteRenderer>().sortingOrder + 1;
            // Se la carta trascinata contiene altre carte figlie (ovvero stiamo spostando una pila di carte) allora aggiorna il sorting order di esse
            if(transform.childCount > 0)
            {
                UpdateSortingOrderOfChildCards(transform);
            }
            
        }
    }

    // Aggiorna il sorting order dell'intera pila spostata
    void UpdateSortingOrderOfChildCards(Transform card)
    {
        int order = card.GetComponent<SpriteRenderer>().sortingOrder;
        Transform childCard = card.GetChild(0);

        // Itera tutte le carte figlie per aggiornare il sorting older
        while (childCard != null)
        {
            childCard.GetComponent<SpriteRenderer>().sortingOrder = (++order);
            // Prendi la carta figlia della carta figlia, e cosi via...
            if (childCard.childCount > 0)
            {
                childCard = childCard.GetChild(0);
            }
            else
            {
                childCard = null;
            }
        }
    }

    // Setta l'ultimo slot eleggibile
    public void SetLastSlot(Transform slot)
    {
        if (slot == null)
        {
            lastSlot = null;
            return;
        }

        // Registra lo stato corrente della carta prima di cambiare lastSlot (inoltre, controlla se il setLastSlot avviene durante il dragging della carta)
        if(isDragging)
        {
            HistoryManager.instance.RegisterMoveToHistory(transform, lastSlot, false, false, false, (cardToFlip != null ? cardToFlip.GetComponent<Card>() : null), null);
        }

        lastSlot = slot;
    }

    public Transform GetLastSlot()
    {
        return lastSlot;
    }

    void CheckCardToFlip()
    {
        // Prima di cambiare slot, controlla se al di sotto si ha una carta coperta. se si, bisogna girarla
        if (lastSlot != null && lastSlot.tag.Equals("Card") && lastSlot.GetComponent<Card>().isCovered)
        {
            cardToFlip = lastSlot;
        }
    }

    // Scopri la carta
    public void FlipCard()
    {
        boxCollider.enabled = true;
        spriteRenderer.sprite = faceSprite;
        isCovered = false;
        Debug.Log("Ho scoperto la carta: " + name);
        // Incremento il punteggio se scopro una carta ma solo se non sto settando l'inizio della partita e se non è stata pescata dal deck
        if(!CardsManager.instance.isSettingGame && !lastSlot.tag.Equals("DrawDeck"))
        {
            GameManager.instance.IncreasePoints(5);
        }
    }

    // Copri la carta
    public void CoverCard()
    {
        boxCollider.enabled = false;
        spriteRenderer.sprite = backSprite;
        isCovered = true;
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
