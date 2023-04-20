using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardData : MonoBehaviour
{
    public bool InHand { get; set; }
    public bool InDeck { get; set; }
    public bool InPlay { get; set; }
    public int PositionInHand { get; set; }
    public CardType Type { get; private set;}
    public CardGroup Group { get; private set; }

    [SerializeField] private CardSO card;

    [Header("Card Fields")]
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI defenseField;
    [SerializeField] private TextMeshProUGUI bloodCostField;
    [SerializeField] private TextMeshProUGUI attackField;
    [SerializeField] private MeshRenderer cardMeshRenderer;

    [Header("Card Movement Values")]
    [SerializeField] private float speed;
    [SerializeField] private Vector3 hoverOffset;

    private int bloodCost, defense, attack;

    private Quaternion rotationalValue;
    private Vector3 targetPoint;
    private Hand hand;


    private void Awake()
    {
        foreach(Hand hand in FindObjectsOfType<Hand>())
        {
            if (hand.BelongsToPlayer)
            {
                this.hand = hand;
            }
        }
        
        // Assinging CardData
        Type = card.GetCardType();
        bloodCost = card.GetBloodCost();
        Group = card.GetCardGroup();

        // Assigning Visuals
        cardMeshRenderer.material = card.GetCardImage();
        nameField.text = card.GetName();
        descriptionField.text = card.GetDescription();
        bloodCostField.text = bloodCost.ToString();

        // Checking if Card is a monster to assign attack and defense
        if (Type == CardType.Monster)
        {
            attack = card.GetAttack();
            defense = card.GetDefense();

            attackField.text = attack.ToString();
            defenseField.text = defense.ToString();
        } else
        {
            attackField.gameObject.SetActive(false);
            defenseField.gameObject.SetActive(false);
        }

        targetPoint = transform.position;
        rotationalValue = transform.rotation;

        InHand = false;
        InDeck = true;
        InPlay = false;
    }

    private void Update()
    {
        HoverCard();
        MoveCard();
    }

    // Might want to move these methods into a different class Move Card and Move To Point
    private void MoveCard()
    {
        if (transform.position == targetPoint && transform.rotation == rotationalValue) return;

        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationalValue, Time.deltaTime * speed);
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotation)
    {
        targetPoint = pointToMoveTo;
        rotationalValue = rotation;
    }

    private void HoverCard()
    {
        if (!InHand) return;

        // Resets card position if no card is being hovered
        if (Utils.GetCardObjectUnderCursor() == null)
        {
            MoveToPoint(hand.GetPositionInHand(PositionInHand), hand.GetRotationInHand());

            return;
        }

        Transform cardTransform = Utils.GetCardObjectUnderCursor().transform;

        // if the card is being hovered it raises the position, else, it resets the position
        if (cardTransform == transform)
        {
            Vector3 hoverPoint = hand.GetPositionInHand(PositionInHand) + hoverOffset;
            Quaternion hoverRotation = Camera.main.transform.rotation;

            MoveToPoint(hoverPoint, hoverRotation);
        } else
        {
            MoveToPoint(hand.GetPositionInHand(PositionInHand), hand.GetRotationInHand());
        }
    }
}
