using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_ANDROID && !UNITY_EDITOR)
using UnityEngine.Android;
#endif
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using RedExtentions;
using static FileIcon;

namespace RedExtentions
{
    public static class StringExtentions
    {
        public static string GetSubStringBetweenStrings(this string str, string _startString, string _endString)
        {
            int startIndex = str.IndexOf(_startString) + _startString.Length;
            int endIndex = str.IndexOf(_endString, ((_startString == _endString) ? (startIndex + 1) : 0));

            return str.Substring(startIndex, (endIndex - startIndex));
        }
    }
}

public class FileBrowser : MonoBehaviour
{

    [Serializable]
    public class FileBrowserEvent : UnityEvent<string> { }

    [SerializeField]
    bool m_hideOnStart = false;
    [SerializeField]
    Sprite m_generalFile;
    [SerializeField]
    Sprite m_documentFile;
    [SerializeField]
    Sprite m_videoFile;
    [SerializeField]
    Sprite m_modelFile;
    [SerializeField]
    Sprite m_imageFile;
    [SerializeField]
    Sprite m_audioFile;
    [SerializeField]
    Sprite m_DSFile;
    [SerializeField]
    Sprite m_folder;

    [SerializeField]
    RectTransform m_mainFileArea;
    [SerializeField]
    RectTransform m_navFileArea;

    [SerializeField]
    GameObject m_scrollContent;
    [SerializeField]
    RectTransform m_scrollView;

    [SerializeField]
    InputField m_filePathAdressbar;
    [SerializeField]
    InputField m_searchBar;

    [SerializeField]
    Dropdown m_fileTypes;
    [SerializeField]
    string[] m_fileTypeFiltter;

    [SerializeField]
    RectTransform m_iconFileFormat;
    [SerializeField]
    RectTransform m_listFileFormat;

    [Space]
    [SerializeField]
    FileBrowserEvent m_onConfermation = new FileBrowserEvent();

    List<BrowserFile> m_files = new List<BrowserFile>();
    bool m_currentFileArangement = false;
    int m_currentFileCount = 0;
    Stack<string> NextStack = new Stack<string>();
    Stack<string> PrevStack = new Stack<string>();
    protected string currentAdress = string.Empty;
    protected string currentFileAddress = string.Empty;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        List<Dropdown.OptionData> temp_options = new List<Dropdown.OptionData>();
        for(int j = 0; j < m_fileTypeFiltter.Length; j++)
        {
            temp_options.Add(new Dropdown.OptionData() { text = m_fileTypeFiltter[j] });
        }
        m_fileTypes.AddOptions(temp_options);

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        m_filePathAdressbar.text = "C:\\Users\\" + Environment.UserName;
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        m_filePathAdressbar.text = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
#endif
        currentAdress = m_filePathAdressbar.text;
        string[] files = new string[0];
        if (Directory.Exists(m_filePathAdressbar.text)) {
            files = Directory.GetFiles(m_filePathAdressbar.text).Concat(Directory.GetDirectories(m_filePathAdressbar.text)).ToArray();
        }
        //StreamWriter textTest = File.CreateText(m_filePathAdressbar.text + "textTest.txt");
        //for (int t = 0; t < files.Length; t++)
        //{
        //    textTest.WriteLine(files[t]);
        //}
        //textTest.Close();

        //int m = 0;

        //string test = files[0].Split('/')[0];
        int current_file_index = 0;
        string extentionType = m_fileTypes.options[m_fileTypes.value].text.GetSubStringBetweenStrings("\"", "\"");
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Split('/', '\\')[files[i].Split('/', '\\').Length - 1];
            if (!File.GetAttributes(files[i]).HasFlag(FileAttributes.Hidden) && !fileName.StartsWith(".") && (fileName.EndsWith(extentionType) || extentionType == "."|| File.GetAttributes(files[i]).HasFlag(FileAttributes.Directory)))
            {
                BrowserFile newFile = new BrowserFile();
                newFile.path = files[i];
                newFile.isDirectory = File.GetAttributes(files[i]).HasFlag(FileAttributes.Directory);
                newFile.icon = GameObject.Instantiate(((m_currentFileArangement) ? m_listFileFormat.gameObject : m_iconFileFormat.gameObject), m_scrollContent.transform);
                newFile.icon.SetActive(true);
                newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3((-m_scrollView.rect.width / 2.0f) + (newFile.icon.GetComponent<RectTransform>().rect.width / 2.0f) + (10.0f + ((newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)*((float)(m_files.Count%(int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))))), (m_scrollView.rect.height / 2.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height * 3.0f) + 10.0f) - (17.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height + 10.0f) * (m_files.Count/(int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))), 0.0f);
                if (newFile.isDirectory)
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_folder;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });

                }
                else if (new string[]{ ".png", ".jpg", ".jpeg", ".ico", ".ai", ".bmp", ".gif", ".ps", ".psd", ".svg", ".tif", ".tiff"}.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_imageFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if(new string[] { ".doc", ".docx", ".odt", ".pdf", ".rtf", ".tex", ".txt", ".wpd" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_documentFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if(new string[] { ".3g2", ".3gp", ".avi", ".flv", ".h264", ".m4v", ".mkv", ".mov", ".mp4", ".mpg", ".mpeg", ".rm", ".swf", ".vob", ".wmv" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_videoFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".aif", ".cda", ".mid", ".midi", ".mp3", "mpa", ".ogg", ".wav", ".wma", ".wpl" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_audioFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".stl", ".obj", ".fbx", ".dae", ".3ds", ".iges", ".step", ".vrml", ".x3d"}.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_modelFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if(new string[] { ".dss", ".dsd", ".dst" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_DSFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_generalFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                newFile.icon.transform.GetChild(1).GetComponent<Text>().text = fileName;
               
                m_files.Add(newFile);
            }
        }
        m_currentFileCount = m_files.Count;
        if(m_hideOnStart)
        {
            this.gameObject.SetActive(false);
        }
    }
//    private void OnEnable()
//    {
//        List<Dropdown.OptionData> temp_options = new List<Dropdown.OptionData>();
//        for (int j = 0; j < m_fileTypeFiltter.Length; j++)
//        {
//            temp_options.Add(new Dropdown.OptionData() { text = m_fileTypeFiltter[j] });
//        }
//        m_fileTypes.AddOptions(temp_options);

//#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
//        m_filePathAdressbar.text = "C:\\Users\\" + Environment.UserName;
//#elif (UNITY_ANDROID && !UNITY_EDITOR)
//        m_filePathAdressbar.text = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
//#endif
//        currentAdress = m_filePathAdressbar.text;
//        string[] files = new string[0];
//        if (Directory.Exists(m_filePathAdressbar.text))
//        {
//            files = Directory.GetFiles(m_filePathAdressbar.text).Concat(Directory.GetDirectories(m_filePathAdressbar.text)).ToArray();
//        }
//        //StreamWriter textTest = File.CreateText(m_filePathAdressbar.text + "textTest.txt");
//        //for (int t = 0; t < files.Length; t++)
//        //{
//        //    textTest.WriteLine(files[t]);
//        //}
//        //textTest.Close();

//        //int m = 0;

//        //string test = files[0].Split('/')[0];
//        int current_file_index = 0;
//        string extentionType = m_fileTypes.options[m_fileTypes.value].text.GetSubStringBetweenStrings("\"", "\"");
//        for (int i = 0; i < files.Length; i++)
//        {
//            string fileName = files[i].Split('/', '\\')[files[i].Split('/', '\\').Length - 1];
//            if (!File.GetAttributes(files[i]).HasFlag(FileAttributes.Hidden) && !fileName.StartsWith(".") && (fileName.EndsWith(extentionType) || extentionType == "." || File.GetAttributes(files[i]).HasFlag(FileAttributes.Directory)))
//            {
//                BrowserFile newFile = new BrowserFile();
//                newFile.path = files[i];
//                newFile.isDirectory = File.GetAttributes(files[i]).HasFlag(FileAttributes.Directory);
//                newFile.icon = GameObject.Instantiate(((m_currentFileArangement) ? m_listFileFormat.gameObject : m_iconFileFormat.gameObject), m_scrollContent.transform);
//                newFile.icon.SetActive(true);
//                newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3((-m_scrollView.rect.width / 2.0f) + (newFile.icon.GetComponent<RectTransform>().rect.width / 2.0f) + (10.0f + ((newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f) * ((float)(m_files.Count % (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))))), (m_scrollView.rect.height / 2.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height * 3.0f) + 10.0f) - (17.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height + 10.0f) * (m_files.Count / (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))), 0.0f);
//                if (newFile.isDirectory)
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_folder;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });

//                }
//                else if (new string[] { ".png", ".jpg", ".jpeg", ".ico", ".ai", ".bmp", ".gif", ".ps", ".psd", ".svg", ".tif", ".tiff" }.Contains(Path.GetExtension(files[i])))
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_imageFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                else if (new string[] { ".doc", ".docx", ".odt", ".pdf", ".rtf", ".tex", ".txt", ".wpd" }.Contains(Path.GetExtension(files[i])))
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_documentFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                else if (new string[] { ".3g2", ".3gp", ".avi", ".flv", ".h264", ".m4v", ".mkv", ".mov", ".mp4", ".mpg", ".mpeg", ".rm", ".swf", ".vob", ".wmv" }.Contains(Path.GetExtension(files[i])))
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_videoFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                else if (new string[] { ".aif", ".cda", ".mid", ".midi", ".mp3", "mpa", ".ogg", ".wav", ".wma", ".wpl" }.Contains(Path.GetExtension(files[i])))
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_audioFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                else if (new string[] { ".stl", ".obj", ".fbx", ".dae", ".3ds", ".iges", ".step", ".vrml", ".x3d" }.Contains(Path.GetExtension(files[i])))
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_modelFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                else if (new string[] { ".dss", ".dsd", ".dst" }.Contains(Path.GetExtension(files[i])))
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_DSFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                else
//                {
//                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_generalFile;
//                    string dir_path = files[i];
//                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
//                }
//                newFile.icon.transform.GetChild(1).GetComponent<Text>().text = fileName;

//                m_files.Add(newFile);
//            }
//        }
//        m_currentFileCount = m_files.Count;
//    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchDirectory(string _newDirectory)
    {
        SwitchDirectory(_newDirectory, true);
    }

    public void SwitchDirectory(string _newDirectory, bool _newDir = true)
    {
        if (_newDir)
        {
            PrevStack.Push(currentAdress);
            NextStack.Clear();
        }
        currentAdress = m_filePathAdressbar.text = _newDirectory;
        currentFileAddress = string.Empty;
        RefreshBrowser();
    }

    public void RefreshBrowser()
    {
        m_scrollContent.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        string[] files = new string[0];
        if (Directory.Exists(m_filePathAdressbar.text))
        {
            files = Directory.GetFiles(m_filePathAdressbar.text).Concat(Directory.GetDirectories(m_filePathAdressbar.text)).ToArray();
        }

        //StreamWriter textTest = File.CreateText(m_filePathAdressbar.text + "textTest.txt");
        //for (int t = 0; t < files.Length; t++)
        //{
        //    textTest.WriteLine(files[t]);
        //}
        //textTest.Close();

        //int m = 0;

        //string test = files[0].Split('/')[0];
        int current_file_index = 0;
        string extentionType = m_fileTypes.options[m_fileTypes.value].text.GetSubStringBetweenStrings("\"", "\"");
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Split('/', '\\')[files[i].Split('/', '\\').Length - 1];
            if (!File.GetAttributes(files[i]).HasFlag(FileAttributes.Hidden) && !fileName.StartsWith(".") && (fileName.EndsWith(extentionType) || extentionType == "." || File.GetAttributes(files[i]).HasFlag(FileAttributes.Directory)))
            {
                BrowserFile newFile = new BrowserFile();
                newFile.path = files[i];
                newFile.isDirectory = File.GetAttributes(files[i]).HasFlag(FileAttributes.Directory);
                newFile.icon = GameObject.Instantiate(((m_currentFileArangement) ? m_listFileFormat.gameObject : m_iconFileFormat.gameObject), m_scrollContent.transform);
                newFile.icon.SetActive(true);
                newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3((-m_scrollView.rect.width / 2.0f) + (newFile.icon.GetComponent<RectTransform>().rect.width / 2.0f) + (10.0f + ((newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f) * ((float)(m_files.Count % (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))))), (m_scrollView.rect.height / 2.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height * 3.0f) + 10.0f) - (17.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height + 10.0f) * (m_files.Count / (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))), 0.0f);
                if (newFile.isDirectory)
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_folder;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });
                }
                else if (new string[] { ".png", ".jpg", ".jpeg", ".ico", ".ai", ".bmp", ".gif", ".ps", ".psd", ".svg", ".tif", ".tiff" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_imageFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".doc", ".docx", ".odt", ".pdf", ".rtf", ".tex", ".txt", ".wpd" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_documentFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".3g2", ".3gp", ".avi", ".flv", ".h264", ".m4v", ".mkv", ".mov", ".mp4", ".mpg", ".mpeg", ".rm", ".swf", ".vob", ".wmv" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_videoFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".aif", ".cda", ".mid", ".midi", ".mp3", "mpa", ".ogg", ".wav", ".wma", ".wpl" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_audioFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".stl", ".obj", ".fbx", ".dae", ".3ds", ".iges", ".step", ".vrml", ".x3d" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_modelFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".dss", ".dsd", ".dst" }.Contains(Path.GetExtension(files[i])))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_DSFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_generalFile;
                    string dir_path = files[i];
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                newFile.icon.transform.GetChild(1).GetComponent<Text>().text = fileName;
                if (current_file_index > (m_files.Count - 1))
                {
                    //m_files.Add(newFile);
                    newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3((-m_scrollView.rect.width / 2.0f) + (newFile.icon.GetComponent<RectTransform>().rect.width / 2.0f) + (10.0f + ((newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f) * ((float)(m_files.Count % (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))))) + 460, (m_scrollView.rect.height / 2.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height * 3.0f) + 10.0f) - (17.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height + 10.0f) * (m_files.Count / (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))), 0.0f);

                    m_files.Add(newFile);
                    current_file_index++;
                }
                else
                {
                    newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3(m_files[current_file_index].icon.GetComponent<RectTransform>().localPosition.x, m_files[current_file_index].icon.GetComponent<RectTransform>().localPosition.y, m_files[current_file_index].icon.GetComponent<RectTransform>().localPosition.z);
                    Destroy(m_files[current_file_index].icon);
                    m_files[current_file_index] = newFile;
                    current_file_index++;
                }
            }
        }

        m_currentFileCount = current_file_index;

        if(current_file_index < m_files.Count)
        {
            for(int t = current_file_index; t < m_files.Count; t++)
            {
                m_files[t].icon.SetActive(false);
            }
        }
    }

    public void NextAddress()
    {
        if(NextStack.Count > 0)
        {
            PrevStack.Push(m_filePathAdressbar.text);
            m_filePathAdressbar.text = NextStack.Pop();
            SwitchDirectory(m_filePathAdressbar.text, false);
        }
    }

    public void PrevAddress()
    {
        if (PrevStack.Count > 0)
        {
            NextStack.Push(m_filePathAdressbar.text);
            m_filePathAdressbar.text = PrevStack.Pop();
            SwitchDirectory(m_filePathAdressbar.text, false);
        }
    }

    public void SearchFile(string _file)
    {
        if (_file != "" && _file != " " && _file != string.Empty)
        {
            List<BrowserFile> searchList = new List<BrowserFile>();
            for (int i = 0; i < m_currentFileCount; i++)
            {
                if (m_files[i].icon.transform.GetChild(1).GetComponent<Text>().text.Contains(_file))
                {
                    searchList.Add(m_files[i]);
                }
            }

            for (int j = 0; j < searchList.Count; j++)
            {
                BrowserFile newFile = new BrowserFile();
                newFile.path = searchList[j].path;
                newFile.isDirectory = searchList[j].isDirectory;
                newFile.icon = GameObject.Instantiate(((m_currentFileArangement) ? m_listFileFormat.gameObject : m_iconFileFormat.gameObject), m_scrollContent.transform);
                newFile.icon.SetActive(true);
                newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3((-m_scrollView.rect.width / 2.0f) + (newFile.icon.GetComponent<RectTransform>().rect.width / 2.0f) + (10.0f + ((newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f) * ((float)(m_files.Count % (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))))), (m_scrollView.rect.height / 2.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height * 3.0f) + 10.0f) - (17.0f) - ((newFile.icon.GetComponent<RectTransform>().rect.height + 10.0f) * (m_files.Count / (int)(m_scrollView.rect.width / (newFile.icon.GetComponent<RectTransform>().rect.width + 10.0f)))), 0.0f);
                if (newFile.isDirectory)
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_folder;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SwitchDirectory(dir_path); });
                }
                else if (new string[] { ".png", ".jpg", ".jpeg", ".ico", ".ai", ".bmp", ".gif", ".ps", ".psd", ".svg", ".tif", ".tiff" }.Contains(Path.GetExtension(searchList[j].path)))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_imageFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".doc", ".docx", ".odt", ".pdf", ".rtf", ".tex", ".txt", ".wpd" }.Contains(Path.GetExtension(searchList[j].path)))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_documentFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".3g2", ".3gp", ".avi", ".flv", ".h264", ".m4v", ".mkv", ".mov", ".mp4", ".mpg", ".mpeg", ".rm", ".swf", ".vob", ".wmv" }.Contains(Path.GetExtension(searchList[j].path)))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_videoFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".aif", ".cda", ".mid", ".midi", ".mp3", "mpa", ".ogg", ".wav", ".wma", ".wpl" }.Contains(Path.GetExtension(searchList[j].path)))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_audioFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".stl", ".obj", ".fbx", ".dae", ".3ds", ".iges", ".step", ".vrml", ".x3d" }.Contains(Path.GetExtension(searchList[j].path)))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_modelFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else if (new string[] { ".dss", ".dsd", ".dst" }.Contains(Path.GetExtension(searchList[j].path)))
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_DSFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                else
                {
                    newFile.icon.transform.GetChild(0).GetComponent<Image>().sprite = m_generalFile;
                    string dir_path = searchList[j].path;
                    newFile.icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                    newFile.icon.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SetCurrentFile(dir_path); });
                }
                newFile.icon.transform.GetChild(1).GetComponent<Text>().text = searchList[j].path.Split('/', '\\')[searchList[j].path.Split('/', '\\').Length - 1];

                newFile.icon.GetComponent<RectTransform>().localPosition = new Vector3(m_files[j].icon.GetComponent<RectTransform>().localPosition.x, m_files[j].icon.GetComponent<RectTransform>().localPosition.y, m_files[j].icon.GetComponent<RectTransform>().localPosition.z);
                Destroy(m_files[j].icon);
                m_files[j] = newFile;
            }

            if (searchList.Count < m_files.Count)
            {
                for (int t = searchList.Count; t < m_files.Count; t++)
                {
                    m_files[t].icon.SetActive(false);
                }
            }
        }
        else
        {
            RefreshBrowser();
        }
    }

    public virtual void SetCurrentFile(string _filePath)
    {
        currentFileAddress = _filePath;
        if (m_files.Exists(f => f.path == currentFileAddress)) 
        {
            m_scrollContent.gameObject.BroadcastMessage("UpdateStatis", m_files.Find(f => f.path == currentFileAddress).icon);
        }
    }

    public virtual void OnConfermation()
    {
        if (currentFileAddress != string.Empty) {
            m_onConfermation.Invoke(currentFileAddress);
        }
    }

    public void TestConfirmCode(string adrs)
    {
        Debug.Log("This is a test to confirm confirmation. The current file selected is :" + adrs);
        //string test = "test the value of you is c";
        //string Resutl = test.GetSubStringBetweenStrings("test", "c");
    }

}

