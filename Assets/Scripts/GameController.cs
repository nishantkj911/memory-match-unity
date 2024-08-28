using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    [SerializeField] private Card[] allCards;
    [SerializeField] private GameObject cardPrefab;

    private const int MaxFlippedCards = 2;

    private List<GameObject> flippedCards;
    private event EventHandler OnBothCardsFlipped = delegate { };

    public void PopulateFlippedCards(GameObject card)
    {
        if (flippedCards.Count == MaxFlippedCards)
        {
            throw new Exception("Too many flipped cards");
        }

        flippedCards.Add(card);

        if (flippedCards.Count == MaxFlippedCards)
        {
            OnBothCardsFlipped?.Invoke(this, EventArgs.Empty);
        }
    }

    private static void Shuffle<T>(List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            (ts[i], ts[r]) = (ts[r], ts[i]);
        }
    }

    private List<Card> MakeAllCards(int count)
    {
        if (count % 2 != 0)
        {
            throw new ArgumentException("Count must be even");
        }

        List<Card> cards = new List<Card>(count);

        for (int i = 0; i < count / 2; i++)
        {
            var j = Random.Range(0, allCards.Length);
            cards.Add(allCards[j]);
            cards.Add(allCards[j]);
        }

        Shuffle(cards);
        return cards;
    }

    private void Start()
    {
        // Keeping this permanent because there's always gonna be test cards in the hierarchy
        var gridChildren = grid.GetComponentsInChildren<CardBehaviour>();
        foreach (var child in gridChildren)
        {
            Destroy(child.gameObject);
        }

        // Proper code starts here
        var cards = MakeAllCards(16);
        foreach (var card in cards)
        {
            cardPrefab.GetComponent<CardBehaviour>().card = card;
            Instantiate(cardPrefab, grid.transform);
        }

        flippedCards = new List<GameObject>(MaxFlippedCards);
        OnBothCardsFlipped += CheckForSimilarity;
    }

    private void CheckForSimilarity(object sender, EventArgs e)
    {
        var card0 = flippedCards[0].GetComponent<CardBehaviour>().card;
        var card1 = flippedCards[1].GetComponent<CardBehaviour>().card;

        if (card0.fruit == card1.fruit)
        {
            // Do nothing. keep them flipped
            flippedCards.Clear();
        }
        else
        {
            StartCoroutine(FlipBackCards());
        }

        return;

        // local functions are to be defined after return statement
        IEnumerator FlipBackCards()
        {
            yield return new WaitForSeconds(0.5f);
            flippedCards[0].GetComponent<CardFlipper>().FlipCard();
            flippedCards[1].GetComponent<CardFlipper>().FlipCard();
            
            flippedCards.Clear();
        }
    }
}