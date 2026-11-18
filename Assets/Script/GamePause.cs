using UnityEngine;

// 여러 팝업(인벤토리, 전리품, 상점 등)이 동시에 요청해도 안전하도록 요청 횟수로 관리
public static class GamePause
{
    private static int requestCount;

    public static bool IsPaused => requestCount > 0;

    public static void Push()
    {
        requestCount++;
        Apply();
    }

    public static void Pop()
    {
        requestCount = Mathf.Max(0, requestCount - 1);
        Apply();
    }

    // 기존 코드 호환용 (Set(true) = Push, Set(false) = Pop)
    public static void Set(bool paused)
    {
        if (paused) Push();
        else Pop();
    }

    public static void ResetAll()
    {
        requestCount = 0;
        Apply();
    }

    private static void Apply()
    {
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
