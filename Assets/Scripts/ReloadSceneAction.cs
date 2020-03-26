using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneAction : MonoBehaviour
{
    public void Execute() {
        SceneManager.LoadScene(gameObject.scene.buildIndex);
    }
}
