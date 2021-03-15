using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource m_audioSource;
    [SerializeField]
    AudioClip m_startingClip = null;
    [SerializeField]
    AudioClip m_endingClip = null;

    [SerializeField]
    int m_startingIndex = 0;
    [SerializeField]
    bool m_randomizePlay = false;
    [SerializeField]
    bool m_autoPlayNext = false;
    [SerializeField]
    bool m_playOnAwaken = false;

    bool m_playing = false;

    public int StartingIndex { get { return m_startingIndex; } set { m_startingIndex = value; } }
    public bool RandomizePlay { get { return m_randomizePlay; } set { m_randomizePlay = value; } }
    public bool AutoPlayNext { get { return m_autoPlayNext; } set { m_autoPlayNext = value; } }
    public bool PlayOnAwake { get { return m_playOnAwaken; } set { m_playOnAwaken = value; } }

    [SerializeField]
    List<AudioClip> m_audioClipList = new List<AudioClip>();

    int m_clipIndex;

    //Awake is called when the script instance is being loaded
	void Awake()
	{
        if (m_playOnAwaken) 
        {
            Play();
            PlayNext();
        }
	}

	// Start is called before the first frame update
	void Start()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        m_clipIndex = m_startingIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playing && m_autoPlayNext)
        {
            PlayNext();
        }
    }

    public void PlayNext()
	{
        if (m_startingClip != null)
        {
            m_audioSource.PlayOneShot(m_startingClip);
        }
        m_audioSource.PlayOneShot(m_audioClipList[m_clipIndex]);

        if (m_endingClip != null)
		{
            m_audioSource.PlayOneShot(m_endingClip);
        }

        m_clipIndex = (m_randomizePlay) ? UnityEngine.Random.Range(0, m_audioClipList.Count) : (((m_clipIndex + 1) >= m_audioClipList.Count) ? (m_clipIndex + 1) : 0 );
    }

    public void Play()
	{
        m_playing = true;
        PlayNext();
	}

    public void Stop()
	{
        m_playing = false;
	}
}
