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
        drawACard?.Invoke();
    }
}
