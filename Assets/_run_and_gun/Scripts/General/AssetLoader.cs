using UnityEngine;

public static class AssetLoader
{
    public static T Load<T>(string address) where T : Object
    {
        return Resources.Load<T>(address);
    }
}
