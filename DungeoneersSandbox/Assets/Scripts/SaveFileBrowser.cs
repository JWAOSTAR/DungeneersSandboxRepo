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

    public override void SetCurrentFile(string _filePath)
    {
        base.SetCurrentFile(_filePath);
        m_fileName = m_currentFileNameBar.text = _filePath.Split('/', '\\')[_filePath.Split('/', '\\').Length - 1];
    }

    public void SetCurrnetFileName(string _fileName)
    {
        //m_fileName = _fileName;
        SetCurrentFile((currentAdress + ((currentAdress.EndsWith("\\") || currentAdress.EndsWith("/")) ? "" : "\\") + _fileName + ((_fileName.Contains(".dst")) ? "" : ".dst")));
    }

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

    public void OnReplace()
    {
        base.OnConfermation();
    }
}
