using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("씬 오브젝트")]
    [SerializeField] private PlayerController2D player;
    [SerializeField] private CameraFollow2D cameraFollow;

    [Header("시작 맵")]
    [SerializeField] private MapPreset startMap;
    [SerializeField] private string startSpawnId = "Default";

    private GameObject currentMapObject;

    public MapRoot CurrentMap { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (player == null)
            player = FindFirstObjectByType<PlayerController2D>();
        if (cameraFollow == null && Camera.main != null)
            cameraFollow = Camera.main.GetComponent<CameraFollow2D>();
    }

    private void Start()
    {
        if (startMap != null)
            LoadMap(startMap, startSpawnId);
    }

    public void LoadMap(MapPreset preset, string spawnId)
    {
        if (preset == null || preset.mapPrefab == null)
        {
            Debug.LogWarning("MapManager: 맵 프리셋 또는 프리팹이 비어 있습니다.");
            return;
        }

        // 이전 맵 제거 (맵에 속한 적, 문, 엄폐물도 함께 사라짐)
        if (currentMapObject != null)
            Destroy(currentMapObject);

        currentMapObject = Instantiate(preset.mapPrefab, Vector3.zero, Quaternion.identity);
        CurrentMap = currentMapObject.GetComponent<MapRoot>();

        // 스폰 포인트 찾기
        Vector2 spawnPos = Vector2.zero;
        MapSpawnPoint[] spawns = currentMapObject.GetComponentsInChildren<MapSpawnPoint>();
        MapSpawnPoint found = null;
        foreach (var s in spawns)
        {
            if (s.spawnId == spawnId) { found = s; break; }
        }
        if (found == null && spawns.Length > 0) found = spawns[0];
        if (found != null) spawnPos = found.transform.position;

        // 플레이어 배치 및 맵 경계 갱신
        if (player != null && CurrentMap != null)
            player.EnterMap(CurrentMap.background, spawnPos);

        // 카메라 경계 갱신 후 즉시 플레이어 위치로 이동
        if (cameraFollow != null && CurrentMap != null && CurrentMap.background != null)
        {
            Bounds b = CurrentMap.background.bounds;
            cameraFollow.SetBounds(b.min, b.max);
            cameraFollow.Snap();
        }

        Debug.Log($"맵 로드: {preset.mapName} (스폰: {spawnId})");
    }
}
