using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Game/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    [Serializable]
    public class Stat
    {
        public string displayName = "이동속도";
        [Tooltip("강화에 필요한 아이템 종류 (창고에서 차감됨)")]
        public ItemData costItem;
        [Tooltip("1레벨 올리는 데 필요한 기본 개수")]
        [Min(1)] public int baseCost = 2;
        [Tooltip("레벨이 오를 때마다 추가되는 개수")]
        [Min(0)] public int costPerLevel = 1;
        [Tooltip("레벨 1당 증가량")]
        public float valuePerLevel = 0.5f;
        public int maxLevel = 5;
        public string unit = "";

        // 현재 레벨에서 다음 레벨로 올리는 데 필요한 아이템 개수
        public int CostAt(int currentLevel) => baseCost + costPerLevel * currentLevel;
    }

    public Stat moveSpeed = new Stat { displayName = "이동속도", baseCost = 2, costPerLevel = 1, valuePerLevel = 0.5f, maxLevel = 5 };
    public Stat maxHealth = new Stat { displayName = "최대 체력", baseCost = 2, costPerLevel = 1, valuePerLevel = 20f, maxLevel = 5 };
    public Stat maxWeight = new Stat { displayName = "최대 무게", baseCost = 2, costPerLevel = 1, valuePerLevel = 3f, maxLevel = 5, unit = "kg" };

    public Stat Get(MetaStat stat)
    {
        switch (stat)
        {
            case MetaStat.MoveSpeed: return moveSpeed;
            case MetaStat.MaxHealth: return maxHealth;
            default: return maxWeight;
        }
    }

    // 게임플레이 스크립트가 읽는 보너스 값
    public float MoveSpeedBonus => moveSpeed.valuePerLevel * MetaProfile.GetLevel(MetaStat.MoveSpeed);
    public float MaxHealthBonus => maxHealth.valuePerLevel * MetaProfile.GetLevel(MetaStat.MaxHealth);
    public float MaxWeightBonus => maxWeight.valuePerLevel * MetaProfile.GetLevel(MetaStat.MaxWeight);
}
