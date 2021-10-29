using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> cardsPool;
    [SerializeField] List<Transform> columns;
    [SerializeField] Dictionary<int, Transform> lastSlotInColumns;
    [SerializeField] Dictionary<string, List<GameObject>> cardStacks;
    [SerializeField] Dictionary<string, int> stackCounters;
    Stack<GameObject> deck, drawnCards;
    [SerializeField] float timeToMoveCard, timeNextDraw;
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
        cardStacks = new Dictionary<string, List<GameObject>>();
        stackCounters = new Dictionary<string, int>();
        for(int i = 0; i < stacks.Length; i++)
        {
            cardStacks.Add(stacks[i].name, new List<GameObject>());
            stackCounters.Add(stacks[i].name, 0);
        }
        
        foreach(GameObject go in stacks)
        {
            go.GetComponent<Stack>().insertCardToStack += InsertCardToStack;
        }

        deckSlot.GetComponent<Deck>().drawACard += DrawACard;
    }

    // Start is called before the first frame update
    void Start()
    {
        isSettingGame = true;
        // Shuffle cards using Fisher-Yates algorithm
        cardsPool.Shuffle<GameObject>();
        for (int i = 0; i < cardsPool.Count; i++)
            deck.Push(Instantiate<GameObject>(cardsPool[i], deckSlot.position, deckSlot.rotation));

        StartCoroutine(DrawCards());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InsertCardToStack(GameObject cardStack, GameObject card)
    {
        var cardComp = card.GetComponent<Card>();
        // Se lo stack counter + 1 == 1 allora va asso, se è 2 va la carta numero 2, e cosi via...
        if ((cardStack.GetComponent<Stack>().stackType.ToString() == cardComp.suit.ToString()) && ((stackCounters[cardStack.name] + 1) == cardComp.number))
        {
            cardStacks[cardStack.name].Add(card);
            stackCounters[cardStack.name]++;
            cardComp.PutToStack();
            card.transform.position = cardStack.transform.position;
        }
    }

    void DrawACard()
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
                    card.GetComponent<SpriteRenderer>().sortingOrder = drawnCards.Peek().GetComponent<SpriteRenderer>().sortingOrder + 1;
                    drawnCards.Peek().GetComponent<BoxCollider2D>().enabled = false;
                }
                drawnCards.Push(card);
            }
        }
    }

    IEnumerator MoveCardAtPosition(Transform card, Transform column)
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
                    card.GetComponent<BoxCollider2D>().enabled = true;
                }
                StartCoroutine(MoveCardAtPosition(card.transform, columnTransform));
                lastSlotInColumns[j] = card.transform;
            }
        }
        isSettingGame = false;
        yield return null;
    }
}
