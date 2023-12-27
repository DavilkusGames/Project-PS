using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Plugins.Audio.Core;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Text verTxt;
    public GameObject loadingPanel;
    public static MainMenuManager Instance;

    private void Awake()
    {
        if (Instance != null) DestroyImmediate(gameObject);
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        verTxt.text = Application.version;

        if (GameData.dataLoaded) DataLoaded(false);
        else if (Application.isEditor) GameData.LoadData();
        else loadingPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void DataLoaded(bool firstTime)
    {
        loadingPanel.SetActive(false);
        if (firstTime) YandexGames.Instance.GameInitialized();
    }
}
