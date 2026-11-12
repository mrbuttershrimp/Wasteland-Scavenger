using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] buttons;          // 최대 3개
    [SerializeField] private TextMeshProUGUI[] labels;  // buttons와 같은 순서

    private List<MapPreset> current;
    private Action<MapPreset> onPick;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void Show(List<MapPreset> choices, Action<MapPreset> callback)
    {
        current = choices;
        onPick = callback;
        panel.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            bool has = i < choices.Count;
            buttons[i].gameObject.SetActive(has);
            if (!has) continue;

            labels[i].text = $"[{GetTypeName(choices[i].roomType)}]\n{choices[i].mapName}";

            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => Pick(index));
        }
    }

    private void Pick(int index)
    {
        panel.SetActive(false);
        onPick?.Invoke(current[index]);
    }

    private string GetTypeName(RoomType type)
    {
        switch (type)
        {
            case RoomType.Combat: return "전투";
            case RoomType.Rest: return "휴식";
            case RoomType.Shop: return "상점";
            default: return "";
        }
    }
}
