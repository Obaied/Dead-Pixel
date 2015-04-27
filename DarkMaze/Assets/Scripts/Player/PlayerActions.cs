using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerActions : MonoBehaviour
{
    public AudioClip[] m_HitSounds;
    public AudioClip[] m_ShootSounds;
    public AudioClip m_DeathSound;
    public GameObject m_HealthBar;
    public float m_ShootFreq = .1f;
    public GameObject m_PlayerShotPrefab;

    private float m_Health = 100f;
    private float m_TargetHealth;
    private float m_ShootTimer;

    private bool m_bInvincible = false;
    private bool m_bLastStand = false;
    private LightFlicker m_LightFlicker;


    //Unity Functions

    void Awake()
    {
        m_LightFlicker = GetComponent<LightFlicker>();
        m_TargetHealth = m_Health;

        StartCoroutine(UpdateCo());
    }

    private IEnumerator UpdateCo()
    {
        while (enabled)
        {
            yield return null;

            m_ShootTimer += Time.deltaTime;

            if (Math.Abs(m_TargetHealth - m_Health) < .001f)
                continue;

            m_Health = Mathf.Lerp(m_Health, m_TargetHealth, Time.deltaTime * 3.0f);
            Vector3 vScale = new Vector3(m_Health / 100f, m_HealthBar.transform.localScale.y,
                    m_HealthBar.transform.localScale.z);
            m_HealthBar.transform.localScale = vScale;
        }
    }

    public void Shoot()
    {
        if (m_ShootTimer < m_ShootFreq
            || PauseManager.Instance.IsPaused() )
            return;

        m_ShootTimer = 0f;

        var go = Instantiate(m_PlayerShotPrefab) as GameObject;
        var vShotPos = transform.position;
        vShotPos.y -= .3f;

        go.GetComponent<PlayerShot>().Init(
            vShotPos,
            vShotPos + transform.forward*10f);

        AudioSource.PlayClipAtPoint(
            m_ShootSounds[Random.Range(0, m_ShootSounds.Length)]
            , transform.position, .60f);

        RaycastHit hitInfo;
        Vector3 vOrigin = transform.position;
        bool bHit = Physics.Raycast(vOrigin, transform.forward, out hitInfo);

        if (bHit && hitInfo.collider.gameObject.tag == "Enemy")
        {
            Enemy hitEnemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
            hitEnemy.TakeDamage();
        }
    }

    //public functions

    public void TakeDamage(float a_Health)
    {
        if (m_bInvincible)
            return;

        m_TargetHealth -= a_Health;

        //Take regular damage
        if (m_TargetHealth > 0)
        {
            AudioSource.PlayClipAtPoint(
                    m_HitSounds[Random.Range(0, m_HitSounds.Length)], transform.position);
        }
        
        //Take critical damage
        else
        {
            //Enter last stand
            if (!m_bLastStand)
            {
                m_bLastStand = true;
                m_TargetHealth = 1f;
                //m_LightFlicker.enabled = true;
                //GetComponent<Light>().intensity *= .5f;
                MusicManager.Instance.StartLastStand();
            }
            else
            {
                //Die for real
                m_bInvincible = true;
                m_TargetHealth = 0f;

                AudioSource.PlayClipAtPoint(
                     m_DeathSound, transform.position);

                GetComponent<PlayerInput>().DisableInput();
                CameraFade.Instance.FadeOut();
                PauseManager.Instance.LoadMainMenu();
            }
        }

    }

    public void Heal(float a_Health)
    {
        if (m_bInvincible)
            return;

        m_TargetHealth += a_Health;

        //Exit last stand, if any
        if (m_bLastStand)
        {
            m_bLastStand = false;
            m_LightFlicker.enabled = false;
            GetComponent<Light>().intensity *= 2f;
            MusicManager.Instance.ExitLastStand();
        }
    }

    public void SetInvin(bool a_Set)
    {
        m_bInvincible = a_Set;
    }

    public void SetHealth(float a_Health)
    {
        m_TargetHealth = a_Health;
    }

    public float GetHealth()
    {
        return m_Health;
    }

}