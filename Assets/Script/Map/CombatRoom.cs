using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : MonoBehaviour
{
    [Header("방 안의 적 (자식 Enemy를 자동 수집하려면 비워 둠)")]
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();

    [Header("클리어 시 열 문")]
    [SerializeField] private MapDoor[] doorsToUnlock;

    [Header("상태")]
    [SerializeField] private bool cleared = false;

    public bool IsCleared => cleared;

    private int aliveCount;

    private void Start()
    {
        // 목록이 비어 있으면 자식 적을 자동으로 수집
        if (enemies.Count == 0)
            enemies.AddRange(GetComponentsInChildren<Enemy>());

        foreach (Enemy e in enemies)
        {
            if (e == null) continue;
            e.Died += OnEnemyDied;
            aliveCount++;
        }

        // 적이 없는 방은 바로 클리어
        if (aliveCount == 0)
            Clear(false);
        else
            LockDoors(true);
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.Died -= OnEnemyDied;
        aliveCount--;

        if (aliveCount <= 0 && !cleared)
            Clear(true);
    }

    private void Clear(bool giveLoot)
    {
        cleared = true;
        LockDoors(false);
        if (giveLoot && LootUI.Instance != null)
            LootUI.Instance.Show();
    }



    private void LockDoors(bool locked)
    {
        if (doorsToUnlock == null) return;
        foreach (MapDoor door in doorsToUnlock)
        {
            if (door != null) door.SetLocked(locked);
        }
    }
}
