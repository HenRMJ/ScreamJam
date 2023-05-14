using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField]
    private Vector2Int _gridDimensions = new Vector2Int(2, 2);
    [SerializeField]
    private float _padding = 0f;
    [SerializeField]
    private GameObject _cardSlotPrefab = null;

    private List<CardSlot> AllCardSlots = new List<CardSlot>();

    // _cardSlotDimensions defaults to 0,0 for the first slot, then changes to
    // the _cardSlotPrefab's Collider dimensions for the remaining slots
    private Vector2 _cardSlotDimensions = new Vector2(0, 0);
    private GameObject[,] _cardSlots;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Another GridManager exists in the scene");
            return;
        }
        Instance = this;

        _cardSlots = new GameObject[_gridDimensions.x, _gridDimensions.y];
        Build(_cardSlotPrefab, _gridDimensions, _padding);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns>Vector3 World Coordinates of center at 2D coordiante position</returns>
    private Vector3 CoordinatesToPosition(Vector2Int coordinates)
    {
        return new Vector3(
             (coordinates.x * _cardSlotDimensions.x) + (coordinates.x * _padding),
            0f,
             (coordinates.y * _cardSlotDimensions.y) + (coordinates.y * _padding)
            );
    }

    public GameObject InstantiateOnGrid(GameObject original, Vector2Int coordinates, Quaternion rotation)
    {
        Vector3 position = CoordinatesToPosition(coordinates);
        GameObject instantiated = (GameObject)Instantiate(
                original,
                transform,
                instantiateInWorldSpace: false
            );
        instantiated.transform.localPosition = position;
        return instantiated;
    }

    private void Build(GameObject cardSlot, Vector2Int dimensions, float padding)
    {
        // We can't get the Bounds of a CardSlot until it's instantiated so we
        // create one first, read its bounds and destroy it then create the
        // real slots in the loops
        GameObject instantiated = Instantiate(cardSlot);

        // Bounds.extents are 1/2 the dimensions, multiply by 2 to get the full card slot width
        Vector3 cardSlotBounds = instantiated.GetComponent<BoxCollider>().bounds.extents * 2;
        _cardSlotDimensions = new Vector2(cardSlotBounds.x, cardSlotBounds.z);
        Destroy(instantiated);

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                GameObject instantiatedCard = InstantiateOnGrid(cardSlot, new Vector2Int(x, y), Quaternion.identity);
                _cardSlots[x,y] = instantiatedCard;

                AllCardSlots.Add(instantiatedCard.GetComponent<CardSlot>());
            }
        }
    }

    public CardSlot CardAt(int x, int y)
    {
        return CardAt(new Vector2Int(x,y));
    }

    public CardSlot CardAt(Vector2Int coordinates)
    {
        if ((coordinates.x < 0 || coordinates.x >= _gridDimensions.x)
            || (coordinates.x < 0 || coordinates.x >= _gridDimensions.x))
            return null;

        if ((coordinates.y < 0 || coordinates.y >= _gridDimensions.y)
            || (coordinates.y < 0 || coordinates.y >= _gridDimensions.y))
            return null;

        return _cardSlots[coordinates.x, coordinates.y].GetComponent<CardSlot>();
    }

    public bool ThereAreCardsInPlay()
    {
        bool cardsInPlay = false;

        foreach(CardSlot cardSlot in AllCardSlots)
        {
            if (cardSlot.Card != null)
            {
                cardsInPlay = true;
                break;
            }
        }

        return cardsInPlay;
    }

    public List<CardSlot> GetAllCardSlots() => AllCardSlots;
    public Vector2Int GetGridDimensions() => _gridDimensions;
}
