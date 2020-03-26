using System;
using UnityEngine;
using UnityEngine.UI;

public class AllowDownloadView : MonoBehaviour
{
    public Button allow;
    public Button cancle;
    public Text size;

    private Action allowListener;
    private Action cancleListener;
    public void AddOnAllowDownloadListener(Action listener)
    {
        allowListener = listener;
    }

    public void AddOnCancleDownloadListener(Action listener)
    {
        cancleListener = listener;
    }

    public void SetSize(string size)
    {
        this.size.text = size;
    }

    private void Start()
    {
        allow.onClick.AddListener(() => {
            if (allowListener != null)
            {
                allowListener.Invoke();
            }
        });

        cancle.onClick.AddListener(() => {
            if (cancleListener != null)
            {
                cancleListener.Invoke();
            }
        });
    }

    
}