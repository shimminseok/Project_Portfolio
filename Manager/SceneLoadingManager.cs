using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [SerializeField] TextMeshProUGUI loadingPercentTxt;
    [SerializeField] Slider loadingProcessSlider;

    bool isLoading;

    int loadingCountLimit = 0;
    int loadingCount = 0;

    void Update()
    {

    }



    IEnumerator LoadLoadingScene()
    {


        AsyncOperation operation = SceneManager.LoadSceneAsync("LoadingScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return null;
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                yield break;
            }
        }
        isLoading = false;
    }
}
