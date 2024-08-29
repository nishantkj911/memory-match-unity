using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    [SerializeField] private GameObject backCard, frontCard;

    private GameController _gameController;
    private Button _button;

    private bool Flipped { get; set; }

    private void Start()
    {
        _button = GetComponent<Button>();
        backCard.SetActive(true);
        frontCard.SetActive(false);

        _gameController = FindObjectOfType<GameController>();

        _button.onClick.AddListener(delegate { _gameController.PopulateFlippedCards(gameObject); });
        _button.onClick.AddListener(FlipCard);

        _gameController.OnBothCardsFlipped += FlipCardInteractability;
        _gameController.OnMoveEnd += FlipCardInteractability;
    }

    /*
     *  Note: Although this is not used, I am keeping this since it's a simple example of a coroutine.
    private IEnumerator FlipCardCoroutine()
    {
        frontCard.SetActive(true);
        backCard.SetActive(false);
        Flipped = true;
        _button.interactable = false;

        yield return new WaitForSeconds(flipTime);

        backCard.SetActive(true);
        frontCard.SetActive(false);
        Flipped = false;
        _button.interactable = true;
    }
    */

    public void FlipCard()
    {
        frontCard.SetActive(!frontCard.activeSelf);
        backCard.SetActive(!backCard.activeSelf);
        Flipped = !Flipped;
        _button.interactable = !_button.interactable;
    }
    
    private void FlipCardInteractability(object o, EventArgs e)
    {
        _button.interactable = !_button.interactable;
    }
}