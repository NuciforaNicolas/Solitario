using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField] List<Transform> columns;

    HintManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Hint()
    {
        for(int i = 0; i < columns.Count; i++)
        {
            Transform slotToCheck = columns[i];
            while(slotToCheck.childCount > 0)
            {
                if (slotToCheck.tag.Equals("Card"))
                {
                    var cardComp = slotToCheck.GetComponent<Card>();
                    if (!cardComp.isCovered)
                    {
                        // Dovrebbe andare in uno degli stack?
                        int stackCounter = CardsManager.instance.GetStackCounter(cardComp.suit.ToString());
                        if (cardComp.number == (stackCounter + 1))
                        {
                            // va nello stack
                            Debug.Log("La carta va nello stack: " + cardComp.suit.ToString());
                            return;
                        }

                        // Dovrebbe andare nell'ultima carta di una delle pile, con colore diverso e numero superiore?
                        for(int j = i + 1; j < columns.Count; j++)
                        {
                            //var lastCardInColumn = GetLastCardInColumn(columns[j]);
                        }
                        
                    }
                }
            }
        }
    }
}
