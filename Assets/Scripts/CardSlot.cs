using UnityEngine;

public class CardSlot : MonoBehaviour
{
     void OnDrawGizmos()
     {
         Gizmos.color = Color.white;
         Gizmos.DrawCube(transform.position, transform.localScale);
     }
}
