using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> cardsPool;
    [SerializeField] List<Transform> columns;
    [SerializeField] Transform deckSlot;
    // Start is called before the first frame update
    void Start()
    {
        cardsPool.Shuffle<GameObject>();
        for (int i = 0; i < cardsPool.Count; i++)
            Instantiate<GameObject>(cardsPool[i], deckSlot.position, deckSlot.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
