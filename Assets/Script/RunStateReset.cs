using UnityEngine;

public static class RunStateReset
{
    // 게임 실행 후, 첫 씬이 로드되기 전에 한 번 호출됨
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        GamePause.ResetAll();
        RunWallet.Reset();
    }
}
