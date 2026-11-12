using UnityEngine;

public class MapSpawnPoint : MonoBehaviour
{
    [Tooltip("문의 targetSpawnId와 같은 값이어야 함")]
    public string spawnId = "Default";

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.6f, 1.2f, 0f));
    }
}
