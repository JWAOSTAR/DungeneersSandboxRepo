using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if (UNITY_ANDROID && !UNITY_EDITOR)
using UnityEngine.Android;
#endif

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            RequestPermisions();
        }
#endif
    }

    /// <summary>
    /// Switches scene from current one to a new given scene
    /// </summary>
    /// <param name="_scene">Name of given scene to switch to</param>
    public void ChangeScene(string _scene)
    {
        //SceneManager.LoadScene(_scene, LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(_scene);
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Requests given permitions if on an Android device
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RequestPermisions()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)

        Permission.RequestUserPermission(Permission.ExternalStorageRead);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
#endif
    }
}
