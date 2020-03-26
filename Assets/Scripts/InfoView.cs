using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : MonoBehaviour
{
    public Text status;

    public void AddStatus(string status)
    {
        this.status.text += status + "\n";
    }
}