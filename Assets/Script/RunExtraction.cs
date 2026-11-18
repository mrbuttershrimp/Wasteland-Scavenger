using UnityEngine;
using UnityEngine.SceneManagement;

// 런을 끝내는 두 가지 방법을 한 곳에 모음
public static class RunExtraction
{
    // 귀환: 인벤토리 아이템과 골드를 로비 창고로 옮김
    public static void Extract(Inventory inv, string lobbySceneName)
    {
        if (inv != null)
        {
            foreach (ItemData item in inv.Items)
                MetaProfile.AddStash(item, 1);
        }

        MetaProfile.AddGold(RunWallet.Gold);
        EndRun(lobbySceneName);
    }

    // 사망: 인벤토리와 골드를 모두 버리고 로비로 이동
    public static void Die(string lobbySceneName)
    {
        EndRun(lobbySceneName);
    }

    private static void EndRun(string lobbySceneName)
    {
        GamePause.ResetAll();
        RunWallet.Reset();
        SceneManager.LoadScene(lobbySceneName);
    }
}
