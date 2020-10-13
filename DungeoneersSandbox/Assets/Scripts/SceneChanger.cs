using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if (UNITY_ANDROID && !UNITY_EDITOR)
using UnityEngine.Android;
#endif

public class SceneChanger : MonoBehaviour
{
    void Start()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            RequestPermisions();
        }
#endif
    }

    public void ChangeScene(string _scene)
    {
        //SceneManager.LoadScene(_scene, LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(_scene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RequestPermisions()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)

        Permission.RequestUserPermission(Permission.ExternalStorageRead);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
#endif
    }
}
