using System.Collections;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private float m_fDelta = 0f;
    private float m_fLastTSSU = 0f;
    private float m_fTimer = 0f;
    private float m_fPauseDelay = 0.15f;
    private bool m_bIsPaused = false;
    private PausePanel _pausePanel;

    private static PauseManager _instance;
    public static PauseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PauseManager>();
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
        _pausePanel = GameObject.Find("Pause Panel").GetComponent<PausePanel>();
    }

    void Update()
    {
        m_fDelta = Time.realtimeSinceStartup - m_fLastTSSU;
        m_fLastTSSU = Time.realtimeSinceStartup;
        m_fTimer += m_fDelta;

        if (Input.GetButtonDown("Pause") 
            && m_fTimer >= m_fPauseDelay 
            && Application.loadedLevelName == "MainLevel")
        {
            if (m_bIsPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }

            m_fTimer = 0f;
        }
    }


    //Public functions

    public bool IsPaused()
    {
        return m_bIsPaused;
    }

    public void PauseGame()
    {
        _pausePanel.OnPauseGame();
        StartCoroutine(MusicManager.Instance.FadeOutVolume());
        m_bIsPaused = true;
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        m_bIsPaused = false;
        StartCoroutine(MusicManager.Instance.FadeInVolume());
        _pausePanel.OnContinueGame();
    }

    public void LoadMainMenu()
    {
        if(m_bIsPaused)
            ContinueGame();

        CameraFade.Instance.FadeOut();
        MusicManager.Instance.OnExit();
        StartCoroutine(LoadMainLevelDelay(2f));
    }

    //Private Functions

    IEnumerator LoadMainLevelDelay(float a_Delay)
    {
        yield return new WaitForSeconds(a_Delay);
        Application.LoadLevel("MainMenu");
    }

}