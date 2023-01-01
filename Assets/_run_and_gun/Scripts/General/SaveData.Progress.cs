using UnityEngine;

public static partial class SaveData
{
    private const string KEY_STAGE = "Stage";
    private const string KEY_MONEY = "Money";

    public static int Stage
    {
        get => GetData(KEY_STAGE, 1);
        set=>SetData(KEY_STAGE, Mathf.Max(value, 1));
    }

    public static int Money
    {
        get => GetData(KEY_MONEY, 0);
        set=>SetData(KEY_MONEY, Mathf.Max(value, 0));
    }
}
