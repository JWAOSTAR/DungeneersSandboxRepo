using RedExtentions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileBrowser : FileBrowser
{
    [SerializeField]
    InputField m_currentFileNameBar;
    [SerializeField]
    GameObject m_replaceConfirmation;

    string m_fileName = string.Empty;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// SetCurrentFile sets the currentlly selected file 
    /// </summary>
    /// <param name="_filePath">Path of the currentlly selected file</param>
    public override void SetCurrentFile(string _filePath)
    {
        base.SetCurrentFile(_filePath);
        m_fileName = m_currentFileNameBar.text = _filePath.Split('/', '\\')[_filePath.Split('/', '\\').Length - 1];
    }

    /// <summary>
    /// SetCurrnetFileName sets the path and file name for the file to be saved
    /// </summary>
    /// <param name="_fileName">The name for the file to be saved</param>
    public void SetCurrnetFileName(string _fileName)
    {
        //m_fileName = _fileName;
        string extentionType = m_fileTypes.options[m_fileTypes.value].text.GetSubStringBetweenStrings("\"", "\"");
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        SetCurrentFile((currentAdress + ((currentAdress.EndsWith("\\") || currentAdress.EndsWith("/")) ? "" : "\\") + _fileName + ((_fileName.Contains(".dst")) ? "" : extentionType)));
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        SetCurrentFile((currentAdress + ((currentAdress.EndsWith("\\") || currentAdress.EndsWith("/")) ? "" : "/") + _fileName + ((_fileName.Contains(".dst")) ? "" : extentionType)));
#endif
    }

    /// <summary>
    /// OnConfermation is called when the confermation button for file save is pushed to conferm the is a file to save
    /// </summary>
    public override void OnConfermation()
    {
        if(!File.Exists(currentFileAddress))
        {
            base.OnConfermation();
        }
        else
        {
            m_replaceConfirmation.SetActive(true);
        }
    }

    /// <summary>
    /// OnReplace is called when a file being saved is itself going to replacing an existing file
    /// </summary>
    public void OnReplace()
    {
        base.OnConfermation();
    }
}
