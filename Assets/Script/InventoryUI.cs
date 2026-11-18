using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController2D player;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI listText;
    [SerializeField] private TextMeshProUGUI weightText;

    [Header("무게 경고 색")]
    [Tooltip("최대 무게 대비 이 비율 이상이면 경고 색으로 표시")]
    [Range(0f, 1f)][SerializeField] private float warnRatio = 0.9f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warnColor = Color.red;

    private bool open;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Start()
    {
        if (inventory == null && player != null)
            inventory = player.GetComponent<Inventory>();

        if (inventory != null)
            inventory.Changed += Refresh;
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.Changed -= Refresh;
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.tabKey.wasPressedThisFrame) return;

        if (open)
            Close();
        else if (!GamePause.IsPaused)   // 다른 팝업(상점 등)이 떠 있으면 열지 않음
            Open();
    }

    private void Open()
    {
        open = true;
        panel.SetActive(true);
        GamePause.Set(true);
        if (player != null) player.SetInputEnabled(false);
        Refresh();
    }

    private void Close()
    {
        open = false;
        panel.SetActive(false);
        GamePause.Set(false);
        if (player != null) player.SetInputEnabled(true);
    }

    private void Refresh()
    {
        if (inventory == null) return;

        // 같은 아이템은 개수로 묶어서 표시
        var sb = new StringBuilder();
        var listed = new HashSet<ItemData>();
        IReadOnlyList<ItemData> items = inventory.Items;

        foreach (ItemData item in items)
        {
            if (!listed.Add(item)) continue;

            int count = 0;
            foreach (ItemData other in items)
                if (other == item) count++;

            sb.AppendLine($"{item.itemName}  x{count}  ({item.weight * count:F1})");
        }

        if (items.Count == 0)
            sb.Append("(비어 있음)");

        if (listText != null) listText.text = sb.ToString();

        if (weightText != null)
        {
            float total = inventory.TotalWeight;
            float max = inventory.MaxWeight;
            bool warn = max > 0f && total / max >= warnRatio;

            weightText.text = $"무게  {total:F1} / {max:F1}";
            weightText.color = warn ? warnColor : normalColor;
        }
    }
}
