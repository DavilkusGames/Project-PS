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
    private static extern bool PlayerInit();

    [DllImport("__Internal")]
    private static extern bool AuthCheck();

    [DllImport("__Internal")]
    private static extern void GameReady();

    [DllImport("__Internal")]
    private static extern string GetLang();

    [DllImport("__Internal")]
    private static extern void SaveToLb(int score);

    [DllImport("__Internal")]
    private static extern void SaveCloudData(string data);

    [DllImport("__Internal")]
    private static extern void LoadCloudData();

    public static YandexGames Instance { get; private set; }
    public static bool IsInit { get; private set; }
    public static bool IsRus { get; private set; }
    public static bool IsAuth { get; private set; }

    private static string[] RusLangDomens = { "ru", "be", "kk", "uk", "uz" };
    private List<TextTranslator> translateQueue = new List<TextTranslator>();

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

    public void AddToTranslateQueue(TextTranslator sender)
    {
        translateQueue.Add(sender);
        if (IsInit || Application.isEditor)
        {
            sender.Translate(IsRus);
            translateQueue.Remove(sender);
        }
    }

    public void RemoveFromTranslateQueue(TextTranslator sender)
    {
        if (translateQueue.Contains(sender)) translateQueue.Remove(sender);
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

    public void SaveData(string str)
    {
        if (IsInit && IsAuth)
        {
            Debug.Log("Saving to cloud: " + str + "...");
            SaveCloudData(str);
        }
    }

    public bool LoadData()
    {
        if (IsInit && IsAuth)
        {
            LoadCloudData();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DataLoaded(string data)
    {
        Debug.Log("Loaded from cloud: " + data);
        GameData.CloudDataLoaded(data);
    }

    public void DataSaved()
    {
        Debug.Log("Data saved to cloud successfully");
    }

    public void GameInitialized()
    {
        if (IsInit) GameReady();
    }

    private IEnumerator WaitForSDKInit()
    {
        yield return new WaitForSeconds(0.5f);
        while (!SDKInit()) yield return new WaitForSeconds(0.2f);
        IsInit = true;
        IsRus = RusLangDomens.Contains(GetLang());
        Debug.Log("IsRus: " + IsRus.ToString());
        for (int i = 0; i < translateQueue.Count; i++)
        {
            translateQueue[i].Translate(IsRus);
        }
        translateQueue.Clear();

        while (!PlayerInit()) yield return new WaitForSeconds(0.2f);
        IsAuth = AuthCheck();
        Debug.Log("IsAuth: " + IsAuth.ToString());

        GameData.LoadData();
    }
}
