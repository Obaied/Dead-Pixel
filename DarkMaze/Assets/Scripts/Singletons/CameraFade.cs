using UnityEngine;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    public Texture2D m_FadeTexture = null;

    private int m_DrawDepth = 10;
    private float m_FadeSpeed = 0.5f;
    private float m_Alpha = 1f;
    private float m_AlphaGoal = 0f;

    // Own Delta Time
    private float m_fLastTSSU = 0f;
    private float m_fDelta = 0f;

    private static CameraFade _instance;
    public static CameraFade Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraFade>();
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
        m_fLastTSSU = Time.realtimeSinceStartup;
        m_fDelta = Time.realtimeSinceStartup - m_fLastTSSU;
        m_Alpha = 1f;
    }

    void Update()
    {
        m_fDelta = Time.realtimeSinceStartup - m_fLastTSSU;
        m_fLastTSSU = Time.realtimeSinceStartup;
    }

    void OnGUI()
    {
        if (m_Alpha != m_AlphaGoal)
        {
            if (m_Alpha > m_AlphaGoal)
            {
                m_Alpha -= m_FadeSpeed * m_fDelta;
            }
            else
            {
                m_Alpha += m_FadeSpeed * m_fDelta;
            }

            if (Mathf.Abs(m_Alpha - m_AlphaGoal) < 0.02f)
            {
                m_Alpha = m_AlphaGoal;
            }
        }

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, m_Alpha);
        GUI.depth = m_DrawDepth;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_FadeTexture);
    }

    public void SetAlphaGoal(float a_AlphaGoal, float a_FadeSpeed)
    {
        m_AlphaGoal = a_AlphaGoal;
        m_FadeSpeed = a_FadeSpeed;
    }

    public void FadeIn()
    {
        SetAlphaGoal(0f, 0.5f);
    }

    public void FadeOut()
    {
        SetAlphaGoal(1f, 0.5f);
    }

    public float GetAlphaValue()
    {
        return m_Alpha;
    }
}
