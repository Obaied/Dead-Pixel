using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource m_TitleAudioSource;
    public Text m_PlayButtonText;

    private bool m_DisableInput = false;
    private bool m_ShouldFlashPlayButton = true;

    private float m_MusicTarget = 1f;

    //Private functions

    void Start()
    {
        CameraFade.Instance.SetAlphaGoal(0f, 2f);
        StartCoroutine(FlashPlayButtonCo());
    }

    private IEnumerator FlashPlayButtonCo()
    {
        float timer = 0;
        float timerFreq = .25f;
        float fadeFreq = .10f;
        float alpha = 1f;
        while (m_ShouldFlashPlayButton)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer >= timerFreq)
            {
                timer = 0f;
                alpha = alpha == 1 ? 0 : 1;
                m_PlayButtonText.CrossFadeAlpha(alpha, fadeFreq, false);
            }
        }
    }

    private IEnumerator LoadLevelDelay(float a_Delay, string a_SceneName)
    {
        yield return new WaitForSeconds(a_Delay);
        Application.LoadLevel(a_SceneName);
    }

    void Update()
    {
        m_TitleAudioSource.volume = Mathf.Lerp(m_TitleAudioSource.volume, m_MusicTarget, Time.deltaTime);
    }


    //Public Functions

    public void PlayGame()
    {
        if (m_DisableInput)
            return;

        m_ShouldFlashPlayButton = false;
        m_DisableInput = true;
        //AudioSource.PlayClipAtPoint(
        //    m_PlayGameSound, transform.position);

        CameraFade.Instance.FadeOut();
        m_MusicTarget = 0f;

        StartCoroutine(LoadLevelDelay(2f, "LoadingScreen"));
    }
}