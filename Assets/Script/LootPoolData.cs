using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LootPool", menuName = "Game/Loot Pool")]
public class LootPoolData : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public ItemData item;
        [Min(1)] public int minCount = 1;   // 이 전리품이 나올 때 개수 최소
        [Min(1)] public int maxCount = 2;   // 개수 최대
    }

    public Entry[] entries;
}
