using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public delegate void DrawACard();
    public event DrawACard drawACard;

    // Notifica il Cardmanager che il giocatore vuole pescare una carta
    private void OnMouseUp()
    {
        // Se il gioco sta resettando il deck oppure sta facendo l'undo del reset del deck, allora bisogna attendere prima di poter pescare una carta
        if(!CardsManager.instance.isResettingDeck && !HistoryManager.instance.isUndoResetDeck)
        {
            drawACard?.Invoke();
        }
    }
}
