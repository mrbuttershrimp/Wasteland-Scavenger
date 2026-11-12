using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class MapDoor : MonoBehaviour
{
    [Header("이동 대상")]
    [SerializeField] private MapPreset targetMap;
    [SerializeField] private string targetSpawnId = "Default";

    [Header("작동 방식")]
    [Tooltip("true면 E를 눌러야 이동, false면 닿는 순간 이동")]
    [SerializeField] private bool requireInteract = true;

    [Header("잠금")]
    [SerializeField] private bool locked = false;
    public bool IsLocked => locked;
    private bool playerInside;

    public void SetLocked(bool value)
    {
        locked = value;
    }


    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (!playerInside || !requireInteract) return;
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Enter();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        if (!requireInteract) Enter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Enter()
    {
        if (locked) return;

        playerInside = false;
        if (RunDirector.Instance != null)
            RunDirector.Instance.RequestRoomChoice();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
