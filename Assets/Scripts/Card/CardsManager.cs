using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> cardsPool;
    [SerializeField] List<Transform> columns;
    [SerializeField] Dictionary<int, Transform> lastSlotInColumns;
    //[SerializeField] Dictionary<string, List<GameObject>> cardStacks;
    [SerializeField] Dictionary<string, int> stackCounters;
    [SerializeField] Stack<GameObject> deck, drawnCards;
    [SerializeField] float timeToMoveCard, timeNextDraw, maxCardsPerStack;
    [SerializeField] Transform deckSlot, drawSlot;

    float timer;
    public bool isSettingGame { get; private set; }

    public static CardsManager instance;

    private void Awake()
    {
        instance = this;
        timer = 0;

        deck = new Stack<GameObject>();
        drawnCards = new Stack<GameObject>();
        lastSlotInColumns = new Dictionary<int, Transform>();
        var stacks = GameObject.FindGameObjectsWithTag("Stack");
        //cardStacks = new Dictionary<string, List<GameObject>>();
        stackCounters = new Dictionary<string, int>();
        for(int i = 0; i < stacks.Length; i++)
        {
            //cardStacks.Add(stacks[i].name, new List<GameObject>());
            stackCounters.Add(stacks[i].name, 0);
        }
        
        foreach(GameObject go in stacks)
        {
            go.GetComponent<Stack>().insertCardToStack += InsertCardToStack;
        }

        deckSlot.GetComponent<Deck>().drawACard += DrawACardFromDeck;
    }

    // Start is called before the first frame update
    void Start()
    {
        isSettingGame = true;
        // Shuffle cards using Fisher-Yates algorithm
        cardsPool.Shuffle<GameObject>();
        for (int i = 0; i < cardsPool.Count; i++)
        {
            GameObject card = Instantiate<GameObject>(cardsPool[i], deckSlot.position, deckSlot.rotation);
            //card.GetComponent<SpriteRenderer>().sortingOrder = i;
            deck.Push(card);
        }
         
        StartCoroutine(DrawCards());
    }

    public int GetStackCounter(string stack)
    {
        return stackCounters[stack];
    }

    void InsertCardToStack(GameObject cardStack, GameObject card)
    {
        var cardComp = card.GetComponent<Card>();
        // Se lo stack counter + 1 == 1 allora va asso, se è 2 va la carta numero 2, e cosi via...
        if ((cardStack.GetComponent<Stack>().stackType.ToString() == cardComp.suit.ToString()) && ((stackCounters[cardStack.name] + 1) == cardComp.number))
        {
            // Registra lo stato corrente della carta prima di inserirla nello stack
            HistoryManager.instance.RegisterMoveToHistory(card.transform, cardComp.GetLastSlot(), false, true, false, (cardComp.GetLastSlot().tag.Equals("Card") && cardComp.GetLastSlot().GetComponent<Card>().isCovered ? cardComp.GetLastSlot().GetComponent<Card>() : null) , cardStack.name);

            //cardStacks[cardStack.name].Add(card);
            stackCounters[cardStack.name]++;
            card.GetComponent<SpriteRenderer>().sortingOrder = stackCounters[cardStack.name];
            cardComp.PutToStack();
            card.transform.position = cardStack.transform.position;
            GameManager.instance.IncreasePoints(10);
            GameManager.instance.IncreaseMoves();
            if(stackCounters[cardStack.name] >= maxCardsPerStack)
            {
                GameManager.instance.IncreaseStacksCompletedCounter();
            }
        }
    }

    public void RemoveFromStack(string stack)
    {
        stackCounters[stack]--;
        if((stackCounters[stack] + 1) == maxCardsPerStack)
        {
            GameManager.instance.DecreaseStackCompletedCounter();
        }
    }

    public void DrawACardFromDeck()
    {
        if(!isSettingGame)
        {
            if(deck.Count > 0)
            {
                GameObject card = deck.Pop();
                var cardComp = card.GetComponent<Card>();
                card.transform.position = drawSlot.position;
                cardComp.SetLastSlot(drawSlot);
                cardComp.FlipCard();
                if(drawnCards.Count > 0)
                {
                    drawnCards.Peek().GetComponent<BoxCollider2D>().enabled = false;
                }
                drawnCards.Push(card);
                drawnCards.Peek().GetComponent<SpriteRenderer>().sortingOrder = drawnCards.Count;
                
                // Registra la pesca dal mazzo nell'history solamente se non stiamo effettuando l'undo del reset del deck
                if(!HistoryManager.instance.isUndoResetDeck)
                {
                    // Registra la pesca dal mazzo nell'history
                    HistoryManager.instance.RegisterMoveToHistory(card.transform, deckSlot, true, false, false, null, null);
                }
            }
            else
            {
                // Se le carte nel deck sono terminate, posso rimetterle al posto e ripescarle
                ResetDeck();
            }

            // Incrementare il numero di mosse solamente se non si sta facendo l'undo del reset del deck
            if(!HistoryManager.instance.isUndoResetDeck)
            {
                GameManager.instance.IncreaseMoves();
            }
        }
    }

    // Rimetti la carta nel deck
    public void MoveCardToDeck(GameObject card)
    {
        deck.Push(card);
        card.transform.position = deckSlot.position;
        card.GetComponent<Card>().CoverCard();
        deck.Peek().GetComponent<SpriteRenderer>().sortingOrder = deck.Count;
    }

    // Rimetti a posto tutte le carte pescate
    void ResetDeck()
    {
        if(!isSettingGame)
        {
            while(drawnCards.Count > 0)
            {
                GameObject card = drawnCards.Pop();
                card.transform.position = deckSlot.transform.position;
                card.GetComponent<Card>().CoverCard();
                card.GetComponent<Card>().SetLastSlot(null);
                deck.Push(card);
                deck.Peek().GetComponent<SpriteRenderer>().sortingOrder = deck.Count;
            }
            // Register reset of deck to history
            HistoryManager.instance.RegisterMoveToHistory(null, null, false, false, true, null, null);
        }
    }

    public void ResetDrawStack()
    {
        while(deck.Count > 0)
        {
            DrawACardFromDeck();
        }
    }

    public void PutAboveDranwStack(GameObject card)
    {
        if (drawnCards.Count > 0)
        {
            drawnCards.Peek().GetComponent<BoxCollider2D>().enabled = false;
        }
        drawnCards.Push(card);
        drawnCards.Peek().GetComponent<SpriteRenderer>().sortingOrder = drawnCards.Count;
    }

    public void DrawACardFromDrawnDeck()
    {
        if (!isSettingGame)
        {
            if (drawnCards.Count > 0)
            {  
                drawnCards.Pop();
                if(drawnCards.Count >0 )
                {
                    drawnCards.Peek().GetComponent<SpriteRenderer>().sortingOrder = drawnCards.Count;
                    drawnCards.Peek().GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
    }

    public void MoveCardAtPosition(Transform card, Transform column)
    {
        StartCoroutine(MoveCardAtPositionCoroutine(card, column));
    }

    IEnumerator MoveCardAtPositionCoroutine(Transform card, Transform column)
    {
        Vector2 newPos = column.position;
        if(column.tag.Equals("Card"))
        {
            newPos = new Vector2(newPos.x, newPos.y - 0.3f);
        }
        timer = 0;
        while(timer < timeToMoveCard)
        {
            timer += Time.deltaTime;
            card.position = Vector2.Lerp(card.position, newPos, timer / timeToMoveCard);
            yield return null;
        }
        //transform.position = column.position;
        var cardComp = card.GetComponent<Card>();
        cardComp.SetLastSlot(column);
        cardComp.PutAboveLastSlot();
        yield return null;
    }

    IEnumerator DrawCards()
    {
        for (int i = 0; i < columns.Count; i++)
        {
            for (int j = i; j < columns.Count; j++)
            {
                yield return new WaitForSeconds(timeNextDraw);
                Transform columnTransform;
                if(i == 0)
                {
                    columnTransform = columns[j];
                }
                else
                {
                    columnTransform = lastSlotInColumns[j];
                }
                GameObject card = deck.Pop();
                if(j == i)
                {
                    //Flip the card
                    card.GetComponent<Card>().FlipCard();
                    //card.GetComponent<BoxCollider2D>().enabled = true;
                }
                MoveCardAtPosition(card.transform, columnTransform);
                // Possibile soluzione per HintManager
                //card.transform.parent = columnTransform;
                lastSlotInColumns[j] = card.transform;
            }
        }
        isSettingGame = false;
        yield return null;
    }

    public Stack<GameObject> GetDeck()
    {
        return deck;
    }
}
