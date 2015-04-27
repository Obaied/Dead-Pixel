using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpScareManager : MonoBehaviour
{
    public float m_JumpScareDuration = .5f;
    public Sprite[] m_JumpScareSprites;
    public AudioClip[] m_JumpScareAudioClips;
    public float m_AlphaLerpSpeed = 8f;
    public float m_MaxAlpha = .75f;

    private bool _isActive = false;
    private CanvasGroup _jumpScareImageCanvasGroup;
    private Image _jumpScareImage;

    private static JumpScareManager _instance;
    public static JumpScareManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<JumpScareManager>();
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

     void Awake()
    {
        if (_instance != null && _instance != this)
        {
            //TODO: Why is Init() here ??
            _instance.Init();
            Destroy(this.gameObject);
            return;
        }


        _instance = this;
        transform.parent = null;
        DontDestroyOnLoad(_instance.gameObject);

        Init();
    }

     void Init()
    {
        var go = GameObject.Find("JumpScare Image");
        _jumpScareImageCanvasGroup = go.GetComponent<CanvasGroup>();
        _jumpScareImage = go.GetComponent<Image>();
    }

    public void StartJumpScare()
    {
        if (_isActive)
            return;

        _isActive = true;
        StartCoroutine(StartJumpScareCo());
    }

    private IEnumerator StartJumpScareCo()
    {
        var delay = new WaitForSeconds(m_JumpScareDuration);
        _jumpScareImage.sprite = m_JumpScareSprites[Random.Range(0, m_JumpScareSprites.Length)];

        AudioSource.PlayClipAtPoint(m_JumpScareAudioClips[Random.Range(0, m_JumpScareAudioClips.Length)],
            transform.position);

        //Handheld.Vibrate();
        
        //Fade In
        float destAlpha = m_MaxAlpha;
        float currAlpha = 0f;
        while (currAlpha < destAlpha)
        {
            currAlpha += Time.deltaTime * m_AlphaLerpSpeed;
            _jumpScareImageCanvasGroup.alpha = currAlpha;
            yield return null;
        }

        yield return delay;

        //Fade out & remove
        destAlpha = 0f;
        currAlpha = m_MaxAlpha;
        while (currAlpha > destAlpha)
        {
            currAlpha -= Time.deltaTime * m_AlphaLerpSpeed;
            _jumpScareImageCanvasGroup.alpha = currAlpha;
            yield return null;
        }

        _jumpScareImage.sprite = null;
        _isActive = false;
    }
}