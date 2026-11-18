using UnityEngine;
using TMPro;

public class LobbyInventoryUI : MonoBehaviour
{
    [Tooltip("창고에 들어갈 수 있는 모든 아이템 에셋")]
    [SerializeField] private ItemData[] allItems;
    [SerializeField] private TextMeshProUGUI listText;
    [SerializeField] private TextMeshProUGUI goldText;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (goldText != null)
            goldText.text = $"Gold : {MetaProfile.Gold}";

        if (listText == null) return;

        string text = "";
        foreach (ItemData item in allItems)
        {
            int count = MetaProfile.GetStash(item);
            if (count > 0)
                text += $"{item.itemName}  x{count}\n";
        }

        listText.text = text.Length > 0 ? text : "(Empty)";
    }
}
