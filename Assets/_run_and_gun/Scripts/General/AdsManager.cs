using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;

public static class AdsManager
{
    private const string ID_GAME_ANDROID = "5163517";
    private const string ID_INTER_ANDROID = "Interstitial_Android";
    private const string ID_BANNER_ANDROID = "Banner_Android";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Advertisement.Initialize(ID_GAME_ANDROID);
    }

    public static void ShowInter()
    {
        Advertisement.Show(ID_INTER_ANDROID, new AdsListener());
    }

    public static async UniTask ShowBanner()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(ID_BANNER_ANDROID);

        await UniTask.WaitUntil(() => Advertisement.Banner.isLoaded);
        
        Advertisement.Banner.Show(ID_BANNER_ANDROID);
    }

    public static void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
}

public class AdsListener : IUnityAdsShowListener
{
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Advertisement.Load(placementId);
    }
}
