using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootUI : MonoBehaviour
{
    public static LootUI Instance { get; private set; }

    [Header("전리품 풀")]
    [SerializeField] private LootPoolData pool;
    [SerializeField] private int choiceCount = 3;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] buttons;          // choiceCount 개수만큼
    [SerializeField] private TextMeshProUGUI[] labels;  // buttons와 같은 순서

    [Header("참조")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController2D player;
    [SerializeField] private ItemPickup pickupPrefab;   // 드롭용 프리팹
    [SerializeField] private float dropSpread = 0.3f;   // 드롭 시 좌우로 흩어지는 폭

    private readonly List<LootPoolData.Entry> current = new List<LootPoolData.Entry>();
    private readonly List<int> counts = new List<int>();
    private bool open;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => Pick(index));
        }
    }

    public void Show()
    {
        if (open || pool == null) return;

        open = true;
        RollChoices();
        RefreshLabels();
        panel.SetActive(true);
        GamePause.Push();
        player.SetInputEnabled(false);
    }

    private void RollChoices()
    {
        current.Clear();
        counts.Clear();

        // 풀에서 중복 없이 choiceCount개 선택
        var candidates = new List<LootPoolData.Entry>();
        foreach (var e in pool.entries)
            if (e != null && e.item != null) candidates.Add(e);

        int n = Mathf.Min(choiceCount, candidates.Count);
        for (int i = 0; i < n; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            LootPoolData.Entry e = candidates[idx];
            candidates.RemoveAt(idx);

            current.Add(e);
            counts.Add(Random.Range(e.minCount, Mathf.Max(e.minCount, e.maxCount) + 1));
        }
    }

    private void RefreshLabels()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            bool has = i < current.Count;
            buttons[i].gameObject.SetActive(has);
            if (!has) continue;

            ItemData item = current[i].item;
            int count = counts[i];
            labels[i].text = $"{item.itemName} x{count}\n총 무게 {item.weight * count:F1}";
        }
    }

    private void Pick(int index)
    {
        if (!open || index >= current.Count) return;

        ItemData item = current[index].item;
        int count = counts[index];
        float totalWeight = item.weight * count;

        if (inventory.FreeWeight + 0.0001f >= totalWeight)
        {
            inventory.AddMany(item, count);
            Debug.Log($"전리품 획득: {item.itemName} x{count}");
        }
        else
        {
            // 무게 여유가 부족하면 전부 플레이어 위치에 드롭
            Drop(item, count);
            Debug.Log($"무게 부족: {item.itemName} x{count} 드롭");
        }

        Close();
    }

    private void Drop(ItemData item, int count)
    {
        if (pickupPrefab == null) return;

        Vector3 basePos = player.transform.position + Vector3.up * 0.5f;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = basePos + Vector3.right * Random.Range(-dropSpread, dropSpread);
            ItemPickup p = Instantiate(pickupPrefab, pos, Quaternion.identity);
            p.SetItem(item);
        }
    }

    private void Close()
    {
        open = false;
        panel.SetActive(false);
        GamePause.Pop();
        player.SetInputEnabled(true);
    }
}
