using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage;
    private float lifetime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Weapon에서 발사 직후 호출
    public void Init(Vector2 direction, float speed, float life, int dmg)
    {
        damage = dmg;
        lifetime = life;
        rb.linearVelocity = direction.normalized * speed;
        transform.right = direction;   // 탄 스프라이트가 진행 방향을 보도록
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 자신은 무시
        if (other.CompareTag("Player")) return;

        if (other.TryGetComponent(out IDamageable target))
            target.TakeDamage(damage);

        Destroy(gameObject);
    }
}

// 피해를 받을 수 있는 대상이 구현할 인터페이스
public interface IDamageable
{
    void TakeDamage(int amount);
}
