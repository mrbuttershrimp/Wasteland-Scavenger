using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("총기 데이터 (스크립터블 오브젝트)")]
    [SerializeField] private WeaponData data;

    [Header("총구 (탄이 나가는 위치)")]
    [SerializeField] private Transform muzzle;

    [Header("참조")]
    [SerializeField] private Transform player;  // 플레이어 루트 (탄 충돌 무시용 태그 확인 외 선택)

    [Header("강화")]
    [SerializeField] private string weaponId = "pistol";
    [SerializeField] private WeaponUpgradeConfig upgradeConfig;

    private Camera cam;
    private float nextFireTime;
    private int currentAmmo;
    private bool isReloading;
    private float reloadEndTime;

    public WeaponData Data => data;
    public int CurrentAmmo => currentAmmo;

    private void Awake()
    {
        if (data != null)
        {
            // 원본 에셋을 건드리지 않도록 복제한 뒤 강화값 적용
            data = Instantiate(data);
            if (upgradeConfig != null)
                WeaponUpgradeProfile.Apply(weaponId, data, upgradeConfig);
        }
        cam = Camera.main;
        currentAmmo = data != null ? data.magazineSize : 0;
    }

    private void Update()
    {
        if (GamePause.IsPaused) return;

        if (data == null || Mouse.current == null || Keyboard.current == null) return;

        // 재장전 처리
        if (isReloading)
        {
            if (Time.time >= reloadEndTime)
            {
                isReloading = false;
                currentAmmo = data.magazineSize;
            }
            return;
        }

        // R 키로 수동 재장전
        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < data.magazineSize)
        {
            StartReload();
            return;
        }

        // 발사 입력 (자동 / 단발 구분)
        bool fireInput = data.automatic
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.leftButton.wasPressedThisFrame;

        if (fireInput && Time.time >= nextFireTime)
        {
            if (currentAmmo <= 0)
            {
                StartReload();
                return;
            }
            Fire();
        }
    }

    private void Fire()
    {
        if (data.bulletPrefab == null || muzzle == null) return;

        nextFireTime = Time.time + 1f / Mathf.Max(0.01f, data.fireRate);
        currentAmmo--;

        Vector2 baseDir = GetAimDirection();

        for (int i = 0; i < data.bulletsPerShot; i++)
        {
            // 퍼짐 각도를 랜덤으로 적용
            float angle = Random.Range(-data.spreadAngle * 0.5f, data.spreadAngle * 0.5f);
            Vector2 dir = Quaternion.Euler(0f, 0f, angle) * baseDir;

            GameObject go = Instantiate(data.bulletPrefab, muzzle.position, Quaternion.identity);
            if (go.TryGetComponent(out Bullet bullet))
                bullet.Init(dir, data.bulletSpeed, data.bulletLifetime, data.damage, "Player");

        }
    }

    // 총구 기준으로 마우스 방향 계산 (VisualRoot 반전과 무관하게 월드 좌표로 계산)
    private Vector2 GetAimDirection()
    {
        Vector3 screen = new Vector3(
            Mouse.current.position.ReadValue().x,
            Mouse.current.position.ReadValue().y,
            -cam.transform.position.z);
        Vector2 mouseWorld = cam.ScreenToWorldPoint(screen);
        return (mouseWorld - (Vector2)muzzle.position).normalized;
    }

    private void StartReload()
    {
        isReloading = true;
        reloadEndTime = Time.time + data.reloadTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (muzzle == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(muzzle.position, 0.05f);
    }
    public void RefillAmmo()
    {
        isReloading = false;
        currentAmmo = data.magazineSize;
    }

}
