using UnityEngine;

public static partial class SaveData
{
    private const string KEY_LEVEL_MOVE = "LevelMove";
    private const string KEY_LEVEL_THROW = "LevelThrow";

    public static int LevelMove
    {
        get => GetData(KEY_LEVEL_MOVE, 1);
        set=>SetData(KEY_LEVEL_MOVE, Mathf.Max(value, 1));
    }

    public static int LevelThrow
    {
        get => GetData(KEY_LEVEL_THROW, 1);
        set=>SetData(KEY_LEVEL_THROW, Mathf.Max(value, 1));
    }
}
