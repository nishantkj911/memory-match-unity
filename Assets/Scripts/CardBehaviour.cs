using TMPro;
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public Card card;
    [SerializeField] private TMP_Text cardText;

    private void Start()
    {
        cardText.text = card.fruit;
    }
}