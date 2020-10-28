using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileIcon : MonoBehaviour
{
    public struct BrowserFile
    {
        public string path;
        public bool isDirectory;
        public GameObject icon;
    }
    /// <summary>
    /// OnSelect brodcasts to other FileIcon objects 
    /// </summary>
    public void OnSelect()
    {
        BroadcastMessage("UpdateStatis", gameObject);
    }

    /// <summary>
    /// Updates properites after recived brodcasted signal from currently selected FileIcon
    /// </summary>
    /// <param name="_brodcaster">The gameObject of the brodcasting FileIcon</param>
    public void UpdateStatis(GameObject _brodcaster)
    {
        transform.GetChild(2).gameObject.SetActive((gameObject == _brodcaster));
    }
}
