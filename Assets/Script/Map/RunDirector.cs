using System.Collections.Generic;
using UnityEngine;

public class RunDirector : MonoBehaviour
{
    public static RunDirector Instance { get; private set; }

    [Header("시작 전투방 후보 (이 중 하나를 랜덤으로)")]
    [SerializeField] private MapPreset[] startCombatPresets;

    [Header("선택지 풀 (전투 3, 휴식 1, 상점 1 등)")]
    [SerializeField] private MapPreset[] roomPool;

    [Header("선택지 개수")]
    [SerializeField] private int choiceCount = 3;

    [Header("참조")]
    [SerializeField] private RoomChoiceUI choiceUI;
    [SerializeField] private PlayerController2D player;

    private bool choosing;

    private MapPreset lastPreset;

    public MapPreset CurrentPreset { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (player == null) player = FindFirstObjectByType<PlayerController2D>();
    }

    private void Start()
    {
        if (startCombatPresets == null || startCombatPresets.Length == 0)
        {
            Debug.LogWarning("RunDirector: 시작 전투방 후보가 없습니다.");
            return;
        }

        MapPreset start = startCombatPresets[Random.Range(0, startCombatPresets.Length)];
        EnterRoom(start);
    }

    // 문과 상호작용했을 때 호출 (방 클리어 조건은 MapDoor가 확인)
    public void RequestRoomChoice()
    {
        if (choosing) return;

        List<MapPreset> choices = PickChoices();
        if (choices.Count == 0)
        {
            Debug.Log("RunDirector: 더 갈 수 있는 방이 없습니다.");
            return;
        }

        choosing = true;
        player.SetInputEnabled(false);
        choiceUI.Show(choices, OnChoiceSelected);
    }

    private void OnChoiceSelected(MapPreset preset)
    {
        choosing = false;
        player.SetInputEnabled(true);
        EnterRoom(preset);
    }

    private void EnterRoom(MapPreset preset)
    {
        CurrentPreset = preset;
        lastPreset = preset;
        MapManager.Instance.LoadMap(preset, "Default");

        // 방 종류별 처리 (휴식 회복, 상점 UI 등은 이후 여기에 연결)
        switch (preset.roomType)
        {
            case RoomType.Rest: Debug.Log("휴식방 입장"); break;
            case RoomType.Shop: Debug.Log("상점 입장"); break;
        }
    }

    // 방문하지 않은 프리셋 중 가중치 랜덤으로 중복 없이 선택
    private List<MapPreset> PickChoices()
    {
        var candidates = new List<MapPreset>();
        foreach (MapPreset p in roomPool)
        {
            // 직전 방은 바로 다시 나오지 않도록 제외
            if (p != null && p != lastPreset)
                candidates.Add(p);
        }
        var result = new List<MapPreset>();
        while (result.Count < choiceCount && candidates.Count > 0)
        {
            float total = 0f;
            foreach (MapPreset c in candidates) total += Mathf.Max(0f, c.weight);
            if (total <= 0f) break;
            float roll = Random.Range(0f, total);
            int idx = 0;
            for (; idx < candidates.Count; idx++)
            {
                roll -= Mathf.Max(0f, candidates[idx].weight);
                if (roll <= 0f) break;
            }
            idx = Mathf.Min(idx, candidates.Count - 1);
            result.Add(candidates[idx]);
            candidates.RemoveAt(idx);   // 같은 선택지 안에서만 중복 방지
        }
        return result;
    }

}
