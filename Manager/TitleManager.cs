using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject createNickNamePanel;
    [SerializeField] TMP_InputField createNickNameInput;

    [Header("Loading")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] TextMeshProUGUI loadingPercentTxt;
    [SerializeField] Slider loadingProcessSlider;

    public bool isTest;
    public bool isReset;

    bool isLoading = false;
    int loadingCount = 0;


    void Awake()
    {
        if (isReset)
        {
            PlayerPrefs.DeleteAll();
        }
    }
    void Start()
    {
        createNickNamePanel.SetActive(false);
        loadingPanel.SetActive(false);
        SoundManager.Instance.PlayBGMSound(SOUND_BGM.NO_2);
    }
    private void Update()
    {
        if (isLoading)
        {
            loadingProcessSlider.value = loadingCount / 100f;
            loadingPercentTxt.text = $"{loadingCount}%";

        }
    }
    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }


    IEnumerator LoadGame()
    {
        if (!PlayerPrefs.HasKey("PlayerData"))
        {
            createNickNamePanel.SetActive(true);
            yield return new WaitUntil(() => !string.IsNullOrEmpty(AccountManager.Instance.NickName));
            createNickNamePanel.SetActive(false);
        }

        loadingPanel.SetActive(true);
        isLoading = true;
        DataTableLoader.Loaded = false;
        DataTableLoader.Load();
        loadingCount = 10;
        yield return new WaitUntil(() => DataTableLoader.Loaded);
        yield return new WaitForEndOfFrame();



        AccountManager.Instance.LoadData();
        loadingCount = 55;
        yield return new WaitUntil(() => AccountManager.Instance.isLoaded);
        yield return new WaitForEndOfFrame();



        AsyncOperation operation = SceneManager.LoadSceneAsync("InGameScene");
        operation.allowSceneActivation = false;


        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingCount = 85;

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return new WaitUntil(() => operation.isDone);
        loadingPercentTxt.text = "100%";

        isLoading = false;
    }

    public void OnClickCreateNickNameBtn()
    {
        AccountManager.Instance.NickName = createNickNameInput.text;
    }
}
