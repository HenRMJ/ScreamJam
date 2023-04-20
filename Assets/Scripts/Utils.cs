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
            return hitInfo.collider.gameObject; ;
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
}
