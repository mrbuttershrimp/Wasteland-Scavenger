using UnityEngine;

public enum MetaStat { MoveSpeed, MaxHealth, MaxWeight }

// 런과 런 사이에 유지되는 영구 데이터 (강화 레벨 + 아이템 창고)
public static class MetaProfile
{
    private const string KeyMove = "meta_lv_move";
    private const string KeyHp = "meta_lv_hp";
    private const string KeyWeight = "meta_lv_weight";
    private const string StashPrefix = "stash_";
    private const string KeyGold = "meta_gold";

    private static bool loaded;
    private static readonly int[] levels = new int[3];
    public static int Gold => PlayerPrefs.GetInt(KeyGold, 0);

    public static int GetLevel(MetaStat stat)
    {
        EnsureLoaded();
        return levels[(int)stat];
    }

    // 창고에 있는 아이템 개수 (ItemData 에셋 이름으로 구분)
    public static int GetStash(ItemData item)
    {
        if (item == null) return 0;
        return PlayerPrefs.GetInt(StashPrefix + item.name, 0);
    }

    public static void AddStash(ItemData item, int count)
    {
        if (item == null || count <= 0) return;
        PlayerPrefs.SetInt(StashPrefix + item.name, GetStash(item) + count);
        PlayerPrefs.Save();
    }

    // 강화 시도. 재료가 부족하면 false
    public static bool TryUpgrade(MetaStat stat, UpgradeConfig.Stat def)
    {
        EnsureLoaded();
        int level = levels[(int)stat];
        if (level >= def.maxLevel) return false;

        int cost = def.CostAt(level);
        if (def.costItem == null || GetStash(def.costItem) < cost) return false;

        PlayerPrefs.SetInt(StashPrefix + def.costItem.name, GetStash(def.costItem) - cost);

        levels[(int)stat]++;
        PlayerPrefs.SetInt(LevelKey(stat), levels[(int)stat]);
        PlayerPrefs.Save();
        return true;
    }

    private static void EnsureLoaded()
    {
        if (loaded) return;
        loaded = true;
        levels[(int)MetaStat.MoveSpeed] = PlayerPrefs.GetInt(KeyMove, 0);
        levels[(int)MetaStat.MaxHealth] = PlayerPrefs.GetInt(KeyHp, 0);
        levels[(int)MetaStat.MaxWeight] = PlayerPrefs.GetInt(KeyWeight, 0);
    }

    private static string LevelKey(MetaStat stat)
    {
        switch (stat)
        {
            case MetaStat.MoveSpeed: return KeyMove;
            case MetaStat.MaxHealth: return KeyHp;
            default: return KeyWeight;
        }
    }

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        PlayerPrefs.SetInt(KeyGold, Gold + amount);
        PlayerPrefs.Save();
    }
    // 골드, 강화 레벨, 창고 아이템을 모두 삭제
    public static void ResetAll()
    {
        // 이 게임이 저장한 PlayerPrefs 전체 삭제 (골드, 레벨, 창고 키 포함)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 메모리 캐시 초기화
        loaded = false;
        for (int i = 0; i < levels.Length; i++)
            levels[i] = 0;
    }
    // 재료를 차감. 부족하면 false
    public static bool TrySpend(ItemData item, int count)
    {
        if (item == null || count <= 0) return false;

        int have = GetStash(item);
        if (have < count) return false;

        PlayerPrefs.SetInt(StashPrefix + item.name, have - count);
        PlayerPrefs.Save();
        return true;
    }


}
