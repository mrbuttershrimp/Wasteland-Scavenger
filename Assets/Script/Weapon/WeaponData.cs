using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName = "Pistol";

    [Header("발사")]
    [Tooltip("초당 발사 횟수")]
    public float fireRate = 5f;
    [Tooltip("연사 가능 여부 (true면 마우스를 누르고 있는 동안 계속 발사)")]
    public bool automatic = false;
    [Tooltip("한 번 발사할 때 나가는 탄 수 (샷건 등)")]
    public int bulletsPerShot = 1;
    [Tooltip("탄 퍼짐 각도 (도 단위, 전체 범위)")]
    [Range(0f, 90f)] public float spreadAngle = 0f;

    [Header("탄환")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public float bulletLifetime = 2f;
    public int damage = 10;

    [Header("탄창")]
    public int magazineSize = 12;
    [Tooltip("재장전 시간(초)")]
    public float reloadTime = 1.2f;
}
    