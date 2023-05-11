using UnityEngine;

public static class Utils
{
    public static GameObject GetCardObjectUnderCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hitInfo;
        if (Physics.Raycast(
                ray,
                out hitInfo,
                Mathf.Infinity,
                LayerMask.GetMask("Card")
            )
        )
        {
            return hitInfo.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit);
        return raycastHit.point;
    }

    public static Transform GetTransformUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.collider.transform;
        }

        return null;
    }

    public static void EveryoneDrawACard()
    {
        foreach (Hand hand in GetHands())
        {
            hand.AddCardToHand();
        }
    }
    
    public static void PlayerDrawACard()
    {
        foreach (Hand hand in GetHands())
        {
            if (hand.BelongsToPlayer)
            {
                hand.AddCardToHand();
            }
        }
    }

    public static void EnemyDrawACard()
    {
        foreach (Hand hand in GetHands())
        {
            if (!hand.BelongsToPlayer)
            {
                hand.AddCardToHand();
            }
        }
    }

    public static Hand[] GetHands()
    {
        return GameObject.FindObjectsOfType<Hand>();
    }
}
