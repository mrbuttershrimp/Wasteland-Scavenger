using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    public event Action<Enemy> Died;

    [Header("체력")]
    [SerializeField] private int maxHealth = 30;

    [Header("이동 (기본 근접 추적)")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float stopDistance = 1f;

    [Header("접촉 피해")]
    [Tooltip("플레이어와 닿아 있을 때 주는 피해")]
    [SerializeField] private int contactDamage = 10;
    [Tooltip("접촉 피해 간격 (초)")]
    [SerializeField] private float contactCooldown = 0.8f;

    [Header("보상")]
    [SerializeField] private int goldReward = 10;

    [Header("참조")]
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer sprite;

    [Header("피격 효과")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashTime = 0.1f;

    private Rigidbody2D rb;
    private int health;
    private Color baseColor = Color.white;
    private float flashEndTime;
    private float nextContactTime;

    // 원거리 스크립트 등 외부에서 이동을 직접 제어할 때 사용
    [HideInInspector] public bool externalControl;
    [HideInInspector] public float externalVx;

    public int Health => health;
    public Transform Target => target;
    public float ChaseRange => chaseRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.freezeRotation = true;

        health = maxHealth;
        if (sprite != null) baseColor = sprite.color;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void Update()
    {
        if (sprite != null && Time.time >= flashEndTime)
            sprite.color = baseColor;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dx = target.position.x - transform.position.x;

        float vx;
        if (externalControl)
        {
            vx = externalVx;
        }
        else
        {
            float dist = Vector2.Distance(transform.position, target.position);
            vx = 0f;
            if (dist <= chaseRange && Mathf.Abs(dx) > stopDistance)
                vx = Mathf.Sign(dx) * moveSpeed;
        }

        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

        if (sprite != null && Mathf.Abs(dx) > 0.01f)
            sprite.flipX = dx < 0f;
    }

    // 플레이어와 닿아 있으면 쿨다운마다 접촉 피해
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextContactTime) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        if (collision.gameObject.TryGetComponent(out IDamageable player))
        {
            player.TakeDamage(contactDamage);
            nextContactTime = Time.time + contactCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        health -= amount;

        if (sprite != null)
        {
            sprite.color = hitColor;
            flashEndTime = Time.time + hitFlashTime;
        }

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        RunWallet.Add(goldReward);
        Died?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
