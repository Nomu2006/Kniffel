using UnityEngine;

[ExecuteAlways]
public class SpawnAreaDebugger : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Vector3 center = transform.TransformPoint(box.center);
        Vector3 size = Vector3.Scale(box.size, transform.lossyScale);

        Gizmos.DrawWireCube(center, size);
    }
}