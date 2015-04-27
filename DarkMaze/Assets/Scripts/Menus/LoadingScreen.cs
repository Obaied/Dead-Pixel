using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private bool _isSlowLoading;

    void Awake()
    {
        CameraFade.Instance.SetAlphaGoal(0f, 2f);
        StartCoroutine(SlowLoadLevel());
    }

    void Update()
    {
        if (Input.anyKeyDown && _isSlowLoading)
        {
            _isSlowLoading = false;
            StopCoroutine("SlowLoadLevel");
            StartCoroutine(FastLoadLevel());
        }
    }

    private IEnumerator FastLoadLevel()
    {
        CameraFade.Instance.FadeOut();
        yield return new WaitForSeconds(2f);
        Application.LoadLevel("MainLevel");
    }

    private IEnumerator SlowLoadLevel()
    {
        _isSlowLoading = true;
        yield return new WaitForSeconds(5f);

        _isSlowLoading = false;
        CameraFade.Instance.FadeOut();
        yield return new WaitForSeconds(2f);
        Application.LoadLevel("MainLevel");
    }
}