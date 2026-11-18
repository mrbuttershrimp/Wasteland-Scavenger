using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private SpriteRenderer sprite;
    [Tooltip("이 거리 안에 플레이어가 있으면 E로 줍기 가능")]
    [SerializeField] private float pickupRadius = 1.5f;

    private Inventory playerInventory;
    private Transform player;

    private void Awake()
    {
        if (sprite != null && item != null && item.icon != null)
            sprite.sprite = item.icon;
    }

    private void Update()
    {
        if (GamePause.IsPaused) return;

        // 플레이어 탐색 (맵 전환으로 플레이어가 바뀌지 않도록 필요할 때만 찾음)
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            player = p.transform;
            playerInventory = p.GetComponent<Inventory>();
        }

        if (playerInventory == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > pickupRadius) return;

        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;

        if (playerInventory.TryAdd(item))
            Destroy(gameObject);
        else
            Debug.Log($"무게 초과로 {item.itemName}을(를) 주울 수 없습니다.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
    public void SetItem(ItemData data)
    {
        item = data;
        if (sprite != null && item != null && item.icon != null)
            sprite.sprite = item.icon;
    }

}
