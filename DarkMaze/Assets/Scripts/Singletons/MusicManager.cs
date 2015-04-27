using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource m_CurrSource;
    public AudioClip m_LastStandTrack;
    public float m_FadeSpeed;

    private AudioClip m_LastPlayedTrack;
    private int m_LastPlayedTrackPosition;

    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance != null && _instance != this)
        {
            _instance.Init();
            Destroy(this.gameObject);
            return;
        }


        _instance = this;
        transform.parent = null;
        DontDestroyOnLoad(_instance.gameObject);

        Init();
    }

    protected void Init()
    {
        
    }
    
    public void PlayBGTrack()
    {
        m_CurrSource.Play();
    }

    public void StartLastStand()
    {
        StartCoroutine(StartLastStandCo());
    }

    private IEnumerator StartLastStandCo()
    {
        // Fade out the BG track
        m_LastPlayedTrack = m_CurrSource.clip;
        m_LastPlayedTrackPosition = m_CurrSource.timeSamples;
        StartCoroutine(FadeOutVolume());

        // Fade in the Last Stand track
        yield return new WaitForSeconds(.5f);

        m_CurrSource.clip = m_LastStandTrack;
        m_CurrSource.Play();

        StartCoroutine(FadeInVolume(.5f));
    }

    public void ExitLastStand()
    {
        StartCoroutine(ExitLastStandCo());
    }

    private IEnumerator ExitLastStandCo()
    {
        // Fade out the last stand track
        StartCoroutine(FadeOutVolume());

        // Fade in the BG track
        yield return new WaitForSeconds(.5f);

        m_CurrSource.clip = m_LastPlayedTrack;
        m_CurrSource.timeSamples = m_LastPlayedTrackPosition;
        m_CurrSource.Play();
        StartCoroutine(FadeInVolume(.5f));
    }

    public IEnumerator FadeOutVolume()
    {
        float currVolume = m_CurrSource.volume;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.fixedDeltaTime * m_FadeSpeed;
            m_CurrSource.volume = Mathf.SmoothStep(currVolume, 0f, timer);
            yield return null;
        }
    }

    public IEnumerator FadeInVolume(float a_TargetVolume = .5f)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.fixedDeltaTime * m_FadeSpeed;
            m_CurrSource.volume = Mathf.SmoothStep(0f, a_TargetVolume, timer);
            yield return null;
        } 
    }

    public void OnExit()
    {
        m_CurrSource.clip = m_LastPlayedTrack;
        m_CurrSource.Stop();
    }
}