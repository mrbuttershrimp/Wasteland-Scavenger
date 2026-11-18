using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage;
    private string ownerTag;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        GetComponent<Collider2D>().isTrigger = true;
    }

    // ownerTag: 발사한 쪽의 태그 ("Player" 또는 "Enemy"). 같은 편은 맞지 않음
    public void Init(Vector2 direction, float speed, float life, int dmg, string ownerTag)
    {
        damage = dmg;
        this.ownerTag = ownerTag;
        rb.linearVelocity = direction.normalized * speed;
        transform.right = direction;
        Destroy(gameObject, life);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag)) return;

        if (other.TryGetComponent(out IDamageable target))
            target.TakeDamage(damage);

        Destroy(gameObject);
    }
}

public interface IDamageable
{
    void TakeDamage(int amount);
}
