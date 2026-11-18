using UnityEngine;

// 무기별 강화 레벨 저장과 적용
public static class WeaponUpgradeProfile
{
    private static string Key(string weaponId, WeaponStat stat) => $"wep_{weaponId}_{stat}";

    public static int GetLevel(string weaponId, WeaponStat stat)
    {
        return PlayerPrefs.GetInt(Key(weaponId, stat), 0);
    }

    public static bool TryUpgrade(string weaponId, WeaponStat stat, WeaponUpgradeConfig.Stat def)
    {
        int level = GetLevel(weaponId, stat);
        if (level >= def.maxLevel) return false;
        if (!MetaProfile.TrySpend(def.costItem, def.CostAt(level))) return false;

        PlayerPrefs.SetInt(Key(weaponId, stat), level + 1);
        PlayerPrefs.Save();
        return true;
    }

    // 런 시작 시 복제본 WeaponData에 강화값을 더함 (원본 에셋은 변하지 않음)
    public static void Apply(string weaponId, WeaponData data, WeaponUpgradeConfig cfg)
    {
        int magLv = GetLevel(weaponId, WeaponStat.MagSize);
        int dmgLv = GetLevel(weaponId, WeaponStat.Damage);
        int rateLv = GetLevel(weaponId, WeaponStat.FireRate);

        data.magazineSize += Mathf.RoundToInt(cfg.magSize.valuePerLevel * magLv);
        data.damage += Mathf.RoundToInt(cfg.damage.valuePerLevel * dmgLv);
        data.fireRate += cfg.fireRate.valuePerLevel * rateLv;
    }
}
