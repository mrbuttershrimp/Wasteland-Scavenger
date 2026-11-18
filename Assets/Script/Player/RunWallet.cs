using System;

// 한 런 동안 유지되는 골드 (씬 전환에도 유지)
public static class RunWallet
{
    public static int Gold { get; private set; }
    public static event Action<int> Changed;

    public static void Add(int amount)
    {
        Gold += amount;
        Changed?.Invoke(Gold);
    }

    public static bool Spend(int amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        Changed?.Invoke(Gold);
        return true;
    }

    public static void Reset()
    {
        Gold = 0;
        Changed?.Invoke(Gold);
    }
}
