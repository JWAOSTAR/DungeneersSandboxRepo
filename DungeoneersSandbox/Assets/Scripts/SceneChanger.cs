using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string _scene)
    {
        SceneManager.LoadScene(_scene, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
