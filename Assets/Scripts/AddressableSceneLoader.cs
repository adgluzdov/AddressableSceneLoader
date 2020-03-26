using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableSceneLoader : MonoBehaviour
{
    private bool findSceneLocationPerformed = false;
    private Coroutine findSceneLocationRoutine;

    private bool getSizeScenePerformed = false;
    private Coroutine getSizeSceneRoutine;

    private bool loadScenePerformed = false;
    private Coroutine loadSceneRoutine;

    public void StartFindSceneLocation(string sceneName, Action onSuccess, Action<string> onFail)
    {
        if (!findSceneLocationPerformed)
        {
            findSceneLocationPerformed = true;
            findSceneLocationRoutine = StartCoroutine(FindSceneLocationRoutine(sceneName, onSuccess, onFail));
        }
        else
        {
            throw new Exception("FindSceneLocation is busy");
        }
    }

    public void StopFindSceneLocation()
    {
        if (findSceneLocationPerformed)
        {
            findSceneLocationPerformed = false;
            if (findSceneLocationRoutine != null)
            {
                StopCoroutine(findSceneLocationRoutine);
                findSceneLocationRoutine = null;
            }
        }
        else
        {
            throw new Exception("FindSceneLocation not start");
        }
    }

    public void StartGetSizeScene(string sceneName, Action<long> onSuccess, Action<string> onFail)
    {
        if (!getSizeScenePerformed)
        {
            getSizeScenePerformed = true;
            getSizeSceneRoutine = StartCoroutine(GetSizeSceneRoutine(sceneName, onSuccess, onFail));
        }
        else
        {
            throw new Exception("GetSizeScene is busy");
        }
    }

    public void StopGetSizeScene()
    {
        if (getSizeScenePerformed)
        {
            getSizeScenePerformed = false;
            if (getSizeSceneRoutine != null)
            {
                StopCoroutine(getSizeSceneRoutine);
                getSizeSceneRoutine = null;
            }
        }
        else
        {
            throw new Exception("GetSizeScene not start");
        }
    }

    public void StartLoadScene(string sceneName, LoadSceneMode mode, Action<SceneInstance> onSuccess, Action<float> onChangeProgress, Action<string> onFail)
    {
        if (!loadScenePerformed)
        {
            loadScenePerformed = true;
            loadSceneRoutine = StartCoroutine(LoadSceneRoutine(sceneName, mode, onSuccess, onChangeProgress, onFail));
        }
        else
        {
            throw new Exception("LoadScene is busy");
        }
    }

    public void StopLoadScene()
    {
        if (loadScenePerformed)
        {
            loadScenePerformed = false;
            if (loadSceneRoutine != null)
            {
                StopCoroutine(loadSceneRoutine);
                loadSceneRoutine = null;
            }
        }
        else
        {
            throw new Exception("LoadScene not start");
        }
    }

    private IEnumerator FindSceneLocationRoutine(string sceneName, Action onSuccess, Action<string> onFail)
    {
        var resourceLocationHandle = Addressables.LoadResourceLocationsAsync(sceneName);
        while (resourceLocationHandle.IsValid() && !resourceLocationHandle.IsDone)
        {
            Debug.Log("Awaiting scene location... " + resourceLocationHandle.PercentComplete * 100f);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        if (resourceLocationHandle.Status != AsyncOperationStatus.Succeeded || resourceLocationHandle.Result.Count == 0)
        {
            Debug.Log("Scene location for " + sceneName + " has not been found!");
            StopFindSceneLocation();
            onFail.Invoke("Scene location for " + sceneName + " has not been found!");
            yield break;
        }
        Debug.Log("Scene location found! " + resourceLocationHandle.Result[0]);
        StopFindSceneLocation();
        onSuccess.Invoke();
    }

    private IEnumerator GetSizeSceneRoutine(string sceneName, Action<long> onSuccess, Action<string> onFail)
    {
        var sizeOperationHandle = Addressables.GetDownloadSizeAsync(sceneName);
        while (sizeOperationHandle.IsValid() && !sizeOperationHandle.IsDone)
        {
            Debug.Log("Size estimation... " + sizeOperationHandle.PercentComplete * 100f);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Debug.Log(sceneName + " size = " + sizeOperationHandle.Result);
        StopGetSizeScene();
        onSuccess.Invoke(sizeOperationHandle.Result);
    }

    private IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode mode, Action<SceneInstance> onSuccess, Action<float> onChangeProgress, Action<string> onFail)
    {
        Debug.Log("LoadSceneRoutine start");
        var loadSceneOperationHandle = Addressables.LoadSceneAsync(sceneName, mode, true);
        var oldProgress = -1f;
        while (loadSceneOperationHandle.IsValid() && !loadSceneOperationHandle.IsDone)
        {
            if (loadSceneOperationHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Operation failed");
                StopLoadScene();
                onFail("Operation failed");
                yield break;
            }
            if (loadSceneOperationHandle.OperationException != null)
            {
                Debug.LogError(loadSceneOperationHandle.OperationException);
                StopLoadScene();
                onFail.Invoke(loadSceneOperationHandle.OperationException.Message);
                yield break;
            }

            if (oldProgress != loadSceneOperationHandle.PercentComplete)
            {
                oldProgress = loadSceneOperationHandle.PercentComplete;
                onChangeProgress.Invoke(oldProgress);
            }

            yield return null;
        }
        Debug.Log("LoadScene Complete!");
        StopLoadScene();
        onSuccess.Invoke(loadSceneOperationHandle.Result);
    }

}

