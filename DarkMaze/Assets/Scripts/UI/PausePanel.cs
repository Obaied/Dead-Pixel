using System.Collections;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    public float MaxAlpha = .75f;
    public float FadeSpeed = 1f;

    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.interactable = false;
    }

    public void OnPauseGame()
    {
        StartCoroutine(OnPauseGameCo());
    }

    private IEnumerator OnPauseGameCo()
    {
        if (PauseManager.Instance.IsPaused())
            yield break;

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.fixedDeltaTime*FadeSpeed;
            _canvasGroup.alpha = Mathf.SmoothStep(0f, MaxAlpha, timer);
            yield return null;
        }

        _canvasGroup.alpha = MaxAlpha;
        _canvasGroup.interactable = true;
    }

    public void OnContinueGame()
    {
        StartCoroutine(OnContinueGameCo());
    }

    private IEnumerator OnContinueGameCo()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.fixedDeltaTime * FadeSpeed;
            _canvasGroup.alpha = Mathf.SmoothStep(MaxAlpha, 0f, timer);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
    }


    public void LoadMainMenu()
    {
        _canvasGroup.interactable = false;
        PauseManager.Instance.LoadMainMenu();
    }

}