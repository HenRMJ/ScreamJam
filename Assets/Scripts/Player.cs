using UnityEngine;

/// <summary>
/// Class <c>Player</c> represents each player in the game including
/// their personal attributes (i.e. Health), Deck, Hand and PlayArea
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private int blood;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TrySummonCard(int cost)
    {
        if (cost >= blood) return false;
        if (cost <= 0) return true;

        blood -= cost;
        return true;
    }
}
