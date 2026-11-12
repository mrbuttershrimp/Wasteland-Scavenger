using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("체력")]
    [SerializeField] private int maxHealth = 30;

    [Header("이동")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float chaseRange = 8f;      // 이 거리 안에 플레이어가 들어오면 추적
    [SerializeField] private float stopDistance = 1f;    // 이 X 거리 안에서는 멈춤

    [Header("참조")]
    [SerializeField] private Transform target;           // 비워 두면 Tag가 Player인 오브젝트를 자동 탐색
    [SerializeField] private SpriteRenderer sprite;      // 기본 스프라이트는 오른쪽을 본다고 가정

    [Header("피격 효과")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashTime = 0.1f;

    private Rigidbody2D rb;
    private int health;
    private Color baseColor = Color.white;
    private float flashEndTime;
    public event Action<Enemy> Died;

    public int Health => health;

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
        // 피격 깜빡임 복구
        if (sprite != null && Time.time >= flashEndTime)
            sprite.color = baseColor;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            StopHorizontal();
            return;
        }

        float dx = target.position.x - transform.position.x;
        float dist = Vector2.Distance(transform.position, target.position);

        float vx = 0f;
        if (dist <= chaseRange && Mathf.Abs(dx) > stopDistance)
            vx = Mathf.Sign(dx) * moveSpeed;

        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

        // 플레이어 방향을 바라보도록 좌우 반전
        if (sprite != null && Mathf.Abs(dx) > 0.01f)
            sprite.flipX = dx < 0f;
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
        Died?.Invoke(this);
        Destroy(gameObject);
    }

    private void StopHorizontal()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
