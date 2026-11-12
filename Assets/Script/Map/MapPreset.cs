using UnityEngine;

public enum RoomType { Combat, Rest, Shop }

[CreateAssetMenu(fileName = "NewMap", menuName = "Game/Map Preset")]
public class MapPreset : ScriptableObject
{
    public string mapName = "Map";
    public RoomType roomType = RoomType.Combat;

    [Tooltip("선택지에 뽑힐 확률 가중치 (값이 클수록 자주 나옴)")]
    [Min(0f)] public float weight = 1f;

    [Tooltip("배경, 바닥, 벽, 엄폐물, 적, 문, 스폰 포인트가 들어 있는 맵 프리팹")]
    public GameObject mapPrefab;
}
