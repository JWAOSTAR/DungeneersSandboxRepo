using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string _scene)
    {
        //SceneManager.LoadScene(_scene, LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(_scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
