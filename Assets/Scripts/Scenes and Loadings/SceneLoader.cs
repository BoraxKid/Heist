using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private LoadingProperties _loadingProperties;
    [SerializeField] private FloatUnityEvent _onProgress;

    private void Awake()
    {
        SceneLoader[] sceneLoaders = GameObject.FindObjectsOfType<SceneLoader>();

        if (sceneLoaders.Length == 1)
            DontDestroyOnLoad(this.gameObject);
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadScene()
    {
        this.StartCoroutine(this.LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this._loadingProperties.loadingScreenSceneIndex, LoadSceneMode.Single);
        float progress = 0;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress != progress)
            {
                progress = asyncOperation.progress;
                this._onProgress.Invoke(progress);
            }
            yield return null;
        }
        yield break;
    }
}
