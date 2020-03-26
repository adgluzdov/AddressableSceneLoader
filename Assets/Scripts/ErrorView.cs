using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorView : MonoBehaviour
{
    public Text text;
    public Button back;

    public void SetMessage(string err) {
        text.text = err;
    }

    internal void AddOnBackListener(Action listener)
    {
        back.onClick.AddListener(() => {
            listener.Invoke();
        });
    }
}
