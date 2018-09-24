using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private LoadingProperties _loadingProperties;
    [SerializeField] private FloatUnityEvent _onProgress;

    private void OnEnable()
    {
        this.StartCoroutine(this.LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this._loadingProperties.sceneIndex, LoadSceneMode.Single);
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
