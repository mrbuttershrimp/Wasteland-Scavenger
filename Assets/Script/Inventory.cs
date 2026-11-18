using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("무게 제한")]
    [SerializeField] private float maxWeight = 20f;

    [Header("메타 업그레이드")]
    [SerializeField] private UpgradeConfig upgrades;

    private readonly List<ItemData> items = new List<ItemData>();

    public IReadOnlyList<ItemData> Items => items;
    public float MaxWeight => maxWeight;

    public float FreeWeight => maxWeight - TotalWeight;

    public float TotalWeight
    {
        get
        {
            float total = 0f;
            foreach (ItemData item in items) total += item.weight;
            return total;
        }
    }

    private void Awake()
    {
        if (upgrades != null)
            maxWeight += upgrades.MaxWeightBonus;
    }


    public event Action Changed;

    public bool CanCarry(ItemData item)
    {
        return item != null && TotalWeight + item.weight <= maxWeight + 0.0001f;
    }

    // 무게 초과 시 실패(false)
    public bool TryAdd(ItemData item)
    {
        if (!CanCarry(item)) return false;

        items.Add(item);
        Changed?.Invoke();
        return true;
    }

    public bool Remove(ItemData item)
    {
        bool removed = items.Remove(item);
        if (removed) Changed?.Invoke();
        return removed;
    }

    // 나중에 힘 업그레이드나 장비 효과로 최대 무게를 바꿀 때 사용
    public void SetMaxWeight(float value)
    {
        maxWeight = value;
        Changed?.Invoke();
    }

    public void AddMany(ItemData item, int count)
    {
        for (int i = 0; i < count; i++)
            items.Add(item);
        Changed?.Invoke();
    }
}
