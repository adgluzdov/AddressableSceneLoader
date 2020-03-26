using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RemoteSceneLoader : MonoBehaviour
{
    public string remoteSceneName;
    public LoadSceneMode mode;
    public AddressableSceneLoader addressableSceneLoader;
    public Button OnLoad;
    public ErrorView errorViewPrefab;
    public AllowDownloadView allowDownloadViewPrefab;
    public InfoView infoViewPrefab;

    private ErrorView errorView;
    private AllowDownloadView allowDownloadView;
    private InfoView infoView;

    public UnityEvent OnFindSceneEvent;
    public UnityEventLong OnGetSizeSceneEvent;
    public UnityEvent OnAllowDownloadEvent;
    public UnityEventSceneInstance OnDownloadEvent;
    public UnityEventFloat OnChangeProgressEvent;

    void Start()
    {
        OnLoad.onClick.AddListener(() => {
            OnLoad.gameObject.SetActive(false);
            infoView = Instantiate(infoViewPrefab);
            infoView.AddStatus(" · Start the download");
            infoView.AddStatus(" · Resource Search");
            addressableSceneLoader.StartFindSceneLocation(remoteSceneName, OnFindScene, OnError);
        });
    }

    private void OnFindScene() {
        infoView.AddStatus(" · Resources found");
        infoView.AddStatus(" · We calculate the size of the loaded resources");
        addressableSceneLoader.StartGetSizeScene(remoteSceneName, OnGetSizeScene, OnError);
    }

    private void OnGetSizeScene(long size) {
        infoView.AddStatus(" · Size calculated");
        infoView.AddStatus(" · Waiting for confirmation of download");

        allowDownloadView = Instantiate(allowDownloadViewPrefab);
        allowDownloadView.SetSize(SizeToString(size));
        allowDownloadView.AddOnAllowDownloadListener(OnAllowDownload);
        allowDownloadView.AddOnCancleDownloadListener(OnCancleDownload);
    }

    private void OnAllowDownload() {
        infoView.AddStatus(" · You have confirmed the download of resources");
        infoView.AddStatus(" · Start loading the scene");
        addressableSceneLoader.StartLoadScene(remoteSceneName, mode, OnDownload, OnChangeProgress, OnError);
    }

    private void OnDownload(SceneInstance sceneInstance) {
        infoView.AddStatus(" · Scene loaded");
        infoView.AddStatus(" · Activate the scene");
    }

    private void OnChangeProgress(float progress) {
        infoView.AddStatus(" · " + ProgressToString(progress));
    }

    private void OnCancleDownload() {
        infoView.AddStatus(" · You rejected resource downloads");
        Back();
    }

    private void OnError(string err) {
        infoView.AddStatus(" · An error has occurred");
        errorView = Instantiate(errorViewPrefab);
        errorView.SetMessage(err);
        errorView.AddOnBackListener(Back);
    }

    private void Back() {
        OnLoad.gameObject.SetActive(true);
        if (infoView != null)
        {
            Destroy(infoView.gameObject);
        }

        if (allowDownloadView != null)
        {
            Destroy(allowDownloadView.gameObject);
        }

        if (errorView != null)
        {
            Destroy(errorView.gameObject);
        }
    }

    private string SizeToString(long size)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (size == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(size);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(size) * num).ToString() + suf[place];
    }

    private string ProgressToString(float progress)
    {
        return (Mathf.Clamp(Mathf.Round(progress * 100), 0, 100)).ToString("0") + "%";
    }
}
