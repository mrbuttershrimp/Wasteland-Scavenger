using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public enum ItemType { Heal, AmmoRefill, MaxHealthUp }

    [Serializable]
    public class ShopItem
    {
        public string itemName = "회복약";
        public ItemType type = ItemType.Heal;
        public int price = 20;
        public int amount = 30;     // Heal: 회복량, MaxHealthUp: 증가량, AmmoRefill: 무시
    }

    [Header("상품 풀 (방문마다 중복 없이 랜덤으로 뽑음)")]
    [SerializeField] private ShopItem[] itemPool;
    [SerializeField] private int itemsPerVisit = 3;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] buyButtons;          // itemsPerVisit 개수만큼
    [SerializeField] private TextMeshProUGUI[] buyLabels;  // buyButtons와 같은 순서
    [SerializeField] private Button leaveButton;
    [SerializeField] private TextMeshProUGUI goldText;

    private PlayerHealth health;
    private Weapon weapon;
    private Action onClose;
    private readonly List<ShopItem> current = new List<ShopItem>();
    private readonly List<bool> sold = new List<bool>();

    private void Awake()
    {
        panel.SetActive(false);
        leaveButton.onClick.AddListener(Close);
        RunWallet.Changed += OnGoldChanged;
    }

    private void OnDestroy()
    {
        RunWallet.Changed -= OnGoldChanged;
    }

    public void Open(PlayerHealth playerHealth, Weapon playerWeapon, Action closeCallback)
    {
        health = playerHealth;
        weapon = playerWeapon;
        onClose = closeCallback;

        RollItems();
        panel.SetActive(true);
        Refresh();
    }

    private void OnGoldChanged(int _)
    {
        // 파괴된 뒤 호출되지 않도록 방어
        if (this == null) return;
        Refresh();
    }

    private void RollItems()
    {
        current.Clear();
        sold.Clear();

        var pool = new List<ShopItem>(itemPool);
        int count = Mathf.Min(itemsPerVisit, pool.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            current.Add(pool[idx]);
            sold.Add(false);
            pool.RemoveAt(idx);
        }
    }

    private void Refresh()
    {
        if (!panel.activeSelf) return;

        if (goldText != null)
            goldText.text = $"골드: {RunWallet.Gold}";

        for (int i = 0; i < buyButtons.Length; i++)
        {
            bool has = i < current.Count;
            buyButtons[i].gameObject.SetActive(has);
            if (!has) continue;

            ShopItem item = current[i];
            buyLabels[i].text = sold[i]
                ? $"{item.itemName}\n(품절)"
                : $"{item.itemName}\n{item.price}G";

            buyButtons[i].interactable = !sold[i] && RunWallet.Gold >= item.price;

            int index = i;
            buyButtons[i].onClick.RemoveAllListeners();
            buyButtons[i].onClick.AddListener(() => Buy(index));
        }
    }

    private void Buy(int index)
    {
        ShopItem item = current[index];
        if (sold[index] || !RunWallet.Spend(item.price)) return;

        switch (item.type)
        {
            case ItemType.Heal:
                health.Heal(item.amount);
                break;
            case ItemType.AmmoRefill:
                weapon.RefillAmmo();
                break;
            case ItemType.MaxHealthUp:
                health.IncreaseMax(item.amount);
                break;
        }

        sold[index] = true;
        Refresh();
    }

    private void Close()
    {
        panel.SetActive(false);
        onClose?.Invoke();
    }
}
