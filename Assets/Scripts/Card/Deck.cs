using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public delegate void DrawACard();
    public event DrawACard drawACard;

    private void OnMouseUp()
    {
        drawACard?.Invoke();
    }
}
