using System;
using UnityEngine;

public enum WeaponStat { MagSize, Damage, FireRate }

[CreateAssetMenu(fileName = "WeaponUpgradeConfig", menuName = "Game/Weapon Upgrade Config")]
public class WeaponUpgradeConfig : ScriptableObject
{
    [Serializable]
    public class Stat
    {
        public string displayName = "탄창";
        [Tooltip("강화에 소모하는 아이템 (창고에서 차감)")]
        public ItemData costItem;
        [Min(1)] public int baseCost = 2;
        [Min(0)] public int costPerLevel = 1;
        [Tooltip("레벨 1당 증가량 (탄창은 발 수, 데미지는 피해량, 발사 속도는 초당 발 수)")]
        public float valuePerLevel = 2f;
        public int maxLevel = 5;
        public string unit = "";

        public int CostAt(int currentLevel) => baseCost + costPerLevel * currentLevel;
    }

    public Stat magSize = new Stat { displayName = "탄창", valuePerLevel = 2f, unit = "발" };
    public Stat damage = new Stat { displayName = "데미지", valuePerLevel = 2f, unit = "" };
    public Stat fireRate = new Stat { displayName = "발사 속도", valuePerLevel = 0.5f, unit = "/초" };

    public Stat Get(WeaponStat stat)
    {
        switch (stat)
        {
            case WeaponStat.MagSize: return magSize;
            case WeaponStat.Damage: return damage;
            default: return fireRate;
        }
    }
}
