using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public delegate void InsertCardToStack(GameObject cardStack, GameObject card);
    public event InsertCardToStack insertCardToStack;
    public enum StackType
    {
        Heart, Diamond, Club, Spade
    }
    public StackType stackType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CardsManager.instance.isSettingGame) return;

        if(collision.tag.Equals("Card") && collision.transform.parent == null)
        {
            insertCardToStack?.Invoke(gameObject, collision.gameObject);
        }
    }
}
