using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public event EventHandler OnBothCardsFlipped = delegate { };
    public event EventHandler OnMoveEnd = delegate { };
    public event EventHandler OnGameOver = delegate { };

        [SerializeField] private GameObject grid;
    [SerializeField] private Card[] allCards;
    [SerializeField] private GameObject cardPrefab;
    

    public int MoveCount { get; private set; }              // Making this public since in UI it's required.
    [FormerlySerializedAs("CardCount")] public int cardCount = 16;

    private const int MaxFlippedCards = 2;
    private List<GameObject> _currentFlippedCards;
    private int _flippedCardsCount;
    private bool _gameOver = false;

    // Custom written functions
    public void PopulateFlippedCards(GameObject card)
    {
        if (_currentFlippedCards.Count == MaxFlippedCards)
        {
            throw new Exception("Too many flipped cards");
        }

        _currentFlippedCards.Add(card);

        if (_currentFlippedCards.Count == MaxFlippedCards)
        {
            OnBothCardsFlipped?.Invoke(this, EventArgs.Empty);
        }
    }

    private static void Shuffle<T>(List<T> ts)      // TODO: Maybe revisit this shuffle algorithm???
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

    private void CheckForSimilarity(object sender, EventArgs e)
    {
        var card0 = _currentFlippedCards[0].GetComponent<CardBehaviour>().card;
        var card1 = _currentFlippedCards[1].GetComponent<CardBehaviour>().card;

        if (card0.fruit == card1.fruit)
        {
            // Update the completed card count
            _flippedCardsCount += 2;
            _currentFlippedCards.Clear();
        }
        else
        {
            StartCoroutine(FlipBackCards());
        }
        
        OnMoveEnd?.Invoke(this, EventArgs.Empty);

        return;

        // local functions are to be defined after return statement (as a good practice)
        IEnumerator FlipBackCards()
        {
            yield return new WaitForSeconds(0.5f);
            _currentFlippedCards[0].GetComponent<CardFlipper>().FlipCard();
            _currentFlippedCards[1].GetComponent<CardFlipper>().FlipCard();
            
            _currentFlippedCards.Clear();
        }
    }

    private void MoveCountUpdate(object o, EventArgs e)
    {
        MoveCount++;
        // Debug.Log(MoveCount);
    }

    private void EndGame(object sender, EventArgs e)
    {
        Debug.Log("Game Over!!");
        Debug.Log($"It took {MoveCount} moves");
    }
    
    // Unity functions
    private void Awake()
    {
        // Keeping this permanent because there's always going to be test cards in the hierarchy
        var gridChildren = grid.GetComponentsInChildren<CardBehaviour>();
        foreach (var child in gridChildren)
        {
            Destroy(child.gameObject);
        }
    }

    private void Start()
    {
        var cards = MakeAllCards(cardCount);
        foreach (var card in cards)
        {
            cardPrefab.GetComponent<CardBehaviour>().card = card;
            Instantiate(cardPrefab, grid.transform);
        }

        _currentFlippedCards = new List<GameObject>(MaxFlippedCards);
        OnBothCardsFlipped += CheckForSimilarity;
        OnMoveEnd += MoveCountUpdate;
        OnGameOver += EndGame;
    }

    private void Update()
    {
        if (!_gameOver && _flippedCardsCount == cardCount)
        {
            _gameOver = true;
            OnGameOver.Invoke(this, EventArgs.Empty);
        }

        if (_gameOver && (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}