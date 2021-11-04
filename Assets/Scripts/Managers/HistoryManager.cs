using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    public Transform card, lastPos;
    public Card cardToCover;
    public string stackName;
    public bool fromDeck, fromSuitStack, resetDeck;
};

public class HistoryManager : MonoBehaviour
{
    Stack<Info> history;

    public static HistoryManager instance;
    public bool isUndoResetDeck { get; set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        history = new Stack<Info>();
    }

    public void RegisterMoveToHistory(Transform card, Transform lastPos, bool fromDeck, bool fromSuitStack, bool resetDeck, Card cardToCover, string stackName)
    {
        Info newInfo = new Info();
        newInfo.card = card;
        newInfo.lastPos = lastPos;
        newInfo.fromDeck = fromDeck;
        newInfo.fromSuitStack = fromSuitStack;
        newInfo.cardToCover = cardToCover;
        newInfo.stackName = stackName;
        newInfo.resetDeck = resetDeck;

        history.Push(newInfo);
    }

    public void Undo()
    {
        if(history.Count > 0)
        {

            Info lastInfo = history.Pop();
            // La carta è stata spostata da colonna a colonna o dal drawStack? Riportala indietro
            if (!lastInfo.fromDeck && !lastInfo.resetDeck)
            {
                lastInfo.card.GetComponent<Card>().SetLastSlot(lastInfo.lastPos);
                lastInfo.card.GetComponent<Card>().MoveCardToLastSlot();
                // Se la carta stava nello stack dei semi precedentemente, devo riabilitare il box collider e decrementare il counter dello stack
                if(lastInfo.fromSuitStack && lastInfo.stackName != null)
                {
                    lastInfo.card.GetComponent<BoxCollider2D>().enabled = true;
                    CardsManager.instance.RemoveFromStack(lastInfo.stackName);
                }
                // Se è stata scoperta una carta precedentemente, bisogna ricoprirla
                if(lastInfo.cardToCover != null)
                {
                    lastInfo.cardToCover.GetComponent<Card>().CoverCard();
                }
            }
            // Rimetti nel mazzo la carta pescata
            else if(lastInfo.fromDeck)
            {
                CardsManager.instance.DrawACardFromDrawnDeck();
                CardsManager.instance.MoveCardToDeck(lastInfo.card.gameObject);
            }
            else if(lastInfo.resetDeck)
            {
                UndoResetDeck();
            }
            GameManager.instance.IncreaseMoves();
        }
    }

    void UndoResetDeck()
    {
        isUndoResetDeck = true;
        CardsManager.instance.ResetDrawStack();
        isUndoResetDeck = false;
    }
}
