using UnityEngine;

// 이동은 막지 않고(트리거), 탄환만 막는 엄폐물
[RequireComponent(typeof(Collider2D))]
public class Cover : MonoBehaviour, IDamageable
{
    [Header("내구도")]
    [Tooltip("0 이하면 파괴되지 않는 엄폐물")]
    [SerializeField] private int maxHealth = 0;

    [Header("시각 효과")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color hitColor = new Color(1f, 0.6f, 0.6f);
    [SerializeField] private float hitFlashTime = 0.08f;

    private int health;
    private Color baseColor = Color.white;
    private float flashEndTime;

    private void Awake()
    {
        // 이동은 통과하고 탄환 판정만 받도록 트리거로 설정
        GetComponent<Collider2D>().isTrigger = true;

        health = maxHealth;
        if (sprite != null) baseColor = sprite.color;
    }

    private void Update()
    {
        if (sprite != null && Time.time >= flashEndTime)
            sprite.color = baseColor;
    }

    public void TakeDamage(int amount)
    {
        // 탄환은 Bullet이 맞는 순간 스스로 사라지므로 여기서는 피격 연출과 내구도만 처리
        if (sprite != null)
        {
            sprite.color = hitColor;
            flashEndTime = Time.time + hitFlashTime;
        }

        if (maxHealth <= 0) return;

        health -= amount;
        if (health <= 0)
            Destroy(gameObject);
    }
}
