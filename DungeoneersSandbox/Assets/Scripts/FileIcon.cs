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

    public void OnSelect()
    {
        BroadcastMessage("UpdateStatis", gameObject);
    }

    public void UpdateStatis(GameObject _brodcaster)
    {
        transform.GetChild(2).gameObject.SetActive((gameObject == _brodcaster));
    }
}
