using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName = "아이템";
    [Min(0f)] public float weight = 1f;
    [TextArea] public string description;
    public Sprite icon;   // 나중에 인벤토리 아이콘에 사용
}
