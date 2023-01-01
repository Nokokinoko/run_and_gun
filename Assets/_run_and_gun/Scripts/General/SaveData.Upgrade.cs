using UnityEngine;

public static partial class SaveData
{
    private const string KEY_LEVEL_MOVE = "LevelMove";
    private const string KEY_LEVEL_BULLET = "LevelBullet";

    public static int LevelMove
    {
        get => GetData(KEY_LEVEL_MOVE, 1);
        set=>SetData(KEY_LEVEL_MOVE, Mathf.Max(value, 1));
    }

    public static int LevelBullet
    {
        get => GetData(KEY_LEVEL_BULLET, 1);
        set=>SetData(KEY_LEVEL_BULLET, Mathf.Max(value, 1));
    }
}
