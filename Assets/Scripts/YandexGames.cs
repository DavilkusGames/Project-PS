using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class YandexGames : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool SDKInit();

    [DllImport("__Internal")]
    private static extern bool AuthCheck();

    [DllImport("__Internal")]
    private static extern string GetLang();

    [DllImport("__Internal")]
    private static extern void SaveToLb(int score);

    public static YandexGames Instance { get; private set; }
    public static bool IsInit { get; private set; }
    public static bool IsRus { get; private set; }
    public static bool IsAuth { get; private set; }

    private static string[] RusLangDomens = { "ru", "be", "kk", "uk", "uz" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (!Application.isEditor) StartCoroutine(nameof(WaitForSDKInit));
    }

    public void SaveToLeaderboard(int score)
    {
        if (Application.isEditor || !IsInit || !IsAuth) return;
        SaveToLb(score);
        Debug.Log("Saved to lb: " + score.ToString());
    }

    private IEnumerator WaitForSDKInit()
    {
        yield return new WaitForSeconds(1.0f);
        while (!SDKInit()) yield return new WaitForSeconds(0.3f);
        IsInit = true;
        IsRus = RusLangDomens.Contains(GetLang());
        Debug.Log("IsRus: " + IsRus.ToString());

        yield return new WaitForSeconds(2f);
        IsAuth = AuthCheck();
        Debug.Log("IsAuth: " + IsAuth.ToString());
    }
}
