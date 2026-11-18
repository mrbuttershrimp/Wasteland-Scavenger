using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class RangedEnemy : MonoBehaviour
{
    [Header("사격")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float fireInterval = 1.5f;
    [SerializeField] private float shootRange = 10f;
    [SerializeField] private float bulletSpeed = 9f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 8;
    [Tooltip("켜면 엄폐물/지형에 가려졌을 때 쏘지 않음")]
    [SerializeField] private bool requireLineOfSight = true;
    [Tooltip("시야를 막는 레이어 (Ground, Cover)")]
    [SerializeField] private LayerMask blockMask;

    [Header("거리 유지")]
    [SerializeField] private float preferredDistance = 6f;
    [SerializeField] private float distanceTolerance = 0.5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float retreatSpeed = 2.5f;

    private Enemy enemy;
    private float nextFireTime;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.externalControl = true;
    }

    private void FixedUpdate()
    {
        Transform target = enemy.Target;
        if (target == null)
        {
            enemy.externalVx = 0f;
            return;
        }

        Vector2 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;
        float dirX = Mathf.Sign(toTarget.x);

        // 거리 유지: 너무 가까우면 후퇴, 너무 멀면 접근, 적정 거리면 정지
        float vx = 0f;
        if (dist > enemy.ChaseRange)
            vx = 0f;
        else if (dist < preferredDistance - distanceTolerance)
            vx = -dirX * retreatSpeed;
        else if (dist > preferredDistance + distanceTolerance)
            vx = dirX * moveSpeed;

        enemy.externalVx = vx;
    }

    private void Update()
    {
        if (GamePause.IsPaused) return;

        Transform target = enemy.Target;
        if (target == null || bulletPrefab == null) return;
        if (Time.time < nextFireTime) return;

        Vector2 origin = GetMuzzlePosition();
        Vector2 aimPoint = target.position;
        float dist = Vector2.Distance(origin, aimPoint);
        if (dist > shootRange) return;

        if (requireLineOfSight)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, (aimPoint - origin).normalized, dist, blockMask);
            if (hit.collider != null) return;   // 엄폐물/벽에 막힘
        }

        Fire(origin, (aimPoint - origin).normalized);
        nextFireTime = Time.time + fireInterval;
    }

    private void Fire(Vector2 origin, Vector2 dir)
    {
        GameObject go = Instantiate(bulletPrefab, origin, Quaternion.identity);
        if (go.TryGetComponent(out Bullet bullet))
            bullet.Init(dir, bulletSpeed, bulletLifetime, bulletDamage, "Enemy");
    }

    private Vector2 GetMuzzlePosition()
    {
        return muzzle != null ? (Vector2)muzzle.position : (Vector2)transform.position + Vector2.up * 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, preferredDistance);
    }
}
