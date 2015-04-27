using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityMovement))]
public abstract class Enemy : MonoBehaviour
{
    public int m_BaseDamage;
    public int m_StartHealth;
    public float m_VisibilityDistance = 20f;
    public GameObject m_DeathParticles = null;
    public ParticleSystem m_HitParticles = null;

    public AudioClip[] m_HurtSounds;
    public AudioClip[] m_DeathSounds;
    public AudioClip[] m_PlayerSeenSounds;

    protected EntityMovement m_Movement;

    protected int m_Health = 1;

    protected GameObject m_Player;

    protected bool m_FirstTimeSawPlayer = true;

    protected float m_PathTimer;
    

    //Unity Functions

    protected void Start()
    {
        m_Movement = GetComponent<EntityMovement>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Health = m_StartHealth;

        StartCoroutine(UpdateCo());
    }

    
    //Public Functions

    public abstract void TakeDamage();

    public abstract void CollidedWith(GameObject a_Other);

    public abstract void Init();


    //Private functions

    protected abstract IEnumerator UpdateCo();

    protected void PerformMovement(EntityMovement.MoveState a_State)
    {
        m_Movement.Move(a_State);
    }

    protected bool CanSeePlayer(out Vector3 a_ToPlayer, float a_MinDistance = 0f)
    {
        float fdist = a_MinDistance == 0f ? float.MaxValue : a_MinDistance;

        a_ToPlayer = m_Player.transform.position - transform.position;
        RaycastHit hitInfo;

        bool bHit = Physics.Raycast(transform.position, a_ToPlayer, out hitInfo);

        if (bHit 
            && hitInfo.collider.gameObject.tag == "Player"
            && hitInfo.distance < fdist)
            return true;

        return false;
    }


}