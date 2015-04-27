using System.Collections;
using UnityEngine;

public class Enemy_Evolver : Enemy
{
    public GameObject m_EnemyShot = null;
    public AudioClip m_ShootSound;

    private int m_EvolutionStage = 0;
    private float m_ShootTimer;
    private float m_ShootFreq = 2f;
    
    //Public Functions

    public int GetEvolutionStage()
    {
        return m_EvolutionStage;
    }

    public void ForceEvolve()
    {
        if (m_EvolutionStage >= 2)
            return;

        m_EvolutionStage++;
    }

    public override void CollidedWith(GameObject a_Other)
    {
        switch (a_Other.tag)
        {
            case "Player":
                int evo = m_EvolutionStage + 1;
                a_Other.GetComponent<PlayerActions>().TakeDamage(m_BaseDamage * evo);
                break;
            case "Enemy":
                Destroy(a_Other);
                EnemyManager.Instance.EvolveEnemy(this.gameObject);
                break;
        }
    }

    public override void TakeDamage()
    {
        m_Health--;
        if (m_Health <= 0)
        {
            GetComponent<AudioSource>().PlayOneShot(
                m_DeathSounds[m_EvolutionStage]);

            Vector3 vPos = transform.position;
            vPos.y = -0.5f;
            Instantiate(m_DeathParticles, vPos, transform.rotation);
            Destroy(gameObject);
        }

        else
        {
            GetComponent<AudioSource>().PlayOneShot(
                m_HurtSounds[Random.Range(0, m_HurtSounds.Length)]);
            m_HitParticles.Play();
        }
    }

    public override void Init()
    {
        int level = DungeonManager.Instance.Level;
        if (level > 4)
        {
            if (Random.value > .7f)
                ForceEvolve();
        }
        else if (level > 6)
        {
            if (Random.value > .5f)
                ForceEvolve();
        }
    }


    //Private & protected Functions

    protected override IEnumerator UpdateCo()
    {
        while (enabled)
        {
            yield return null;
            m_ShootTimer += Time.deltaTime;
            m_PathTimer += Time.deltaTime*m_Movement.m_MoveSpeed;

            if (m_PathTimer < 1f || m_Player == null)
                continue;

            Vector3 vToPlayer = Vector3.zero;
            if (!CanSeePlayer(out vToPlayer, m_VisibilityDistance))
            {
                m_PathTimer = 0f;
                continue;
            }

            if (m_FirstTimeSawPlayer)
            {
                m_FirstTimeSawPlayer = false;
                GetComponent<AudioSource>().PlayOneShot(
                    m_PlayerSeenSounds[Random.Range(0, m_PlayerSeenSounds.Length)]);
            }

            //Debug.Log(gameObject + " | Can See Player");
            Vector3 vToTarget = Vector3.zero;
            Vector3 vToFriendly = Vector3.zero;

            if (CloseToOtherEnemyOfSameEvo(out vToFriendly))
            {
                vToTarget = vToFriendly;
            }
            else
            {
                vToTarget = vToPlayer;
            }

            vToTarget.y = 0f;
            bool bTurnOverride = false;

            //Zero out the other axis
            if (Mathf.Abs((Mathf.Abs(vToTarget.x) - Mathf.Abs(vToTarget.z))) < .1f)
            {
                //Square distance
                int x = 0;
                int z = 0;
                DungeonManager.Instance.GetClosestCell(transform.position + transform.forward, out x, out z);
                if (DungeonManager.Instance.GetCellType(x, z) == Cell.CellType.WALL)
                {
                    bTurnOverride = true;
                    //Zero out the other axis to make the movement grid-like
                    if (Mathf.Abs(transform.forward.x) > Mathf.Abs(transform.forward.z))
                    {
                        vToTarget.x = 0f;
                    }
                    else
                    {
                        vToTarget.z = 0f;
                    }
                }

                //Zero out the other axis to make the movement grid-like
                if (Mathf.Abs(transform.forward.x) > Mathf.Abs(transform.forward.z))
                {
                    vToTarget.z = 0f;
                }
                else
                {
                    vToTarget.x = 0f;
                }
            }
            else
            {
                if (Mathf.Abs(vToTarget.x) > Mathf.Abs(vToTarget.z) + .1f)
                {
                    vToTarget.z = 0f;
                }
                else
                {
                    vToTarget.x = 0f;
                }
            }
            
            //Do we need to rotate ??
            if (bTurnOverride || Vector3.Angle(transform.forward, vToTarget) > 20f)
            {
                if (Vector3.Dot(transform.right, vToTarget) > 0)
                {
                    PerformMovement(EntityMovement.MoveState.ROTRIGHT);
                }
                else
                {
                    PerformMovement(EntityMovement.MoveState.ROTLEFT);
                }
            }
            else
            {
                if (m_EvolutionStage == 2
                    && Vector3.Angle(transform.forward, vToPlayer) < 5f)
                {
                    if (m_ShootTimer >= m_ShootFreq)
                    {
                        m_ShootTimer = 0f;
                        FireShot();
                    }
                }
                else
                {
                    PerformMovement(EntityMovement.MoveState.MOVEFORWARD);
                }
            }

            m_PathTimer = 0f;
        }
    }

    protected bool CloseToOtherEnemyOfSameEvo(out Vector3 a_ToFriendly)
    {
        a_ToFriendly = Vector3.zero;

        //Max evo
        if (m_EvolutionStage >= 2) return false;

        GameObject[] otherEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (otherEnemies.Length == 0)
            return false;

        float fDist = 0f;
        float fMinDist = float.MaxValue;
        int iEvoStage = 0;
        GameObject closestEnemy = null;
        foreach (GameObject enemy in otherEnemies)
        {
            fDist = Vector3.SqrMagnitude(transform.position - enemy.transform.position);

            if (enemy == this.gameObject
                || enemy.GetComponent<Enemy_Evolver>() == null)
                continue;

            iEvoStage = enemy.GetComponent<Enemy_Evolver>().GetEvolutionStage();
            if (iEvoStage != m_EvolutionStage)
                continue;

            if (fDist < fMinDist)
            {
                fMinDist = fDist;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
            return false;

        var origin = transform.position;
        a_ToFriendly = closestEnemy.transform.position - origin;

        //Range check
        if (a_ToFriendly.magnitude > 4f)
            return false;

        RaycastHit hitInfo;
        bool bHit = Physics.Raycast(origin, a_ToFriendly, out hitInfo);

        if (bHit && hitInfo.collider.gameObject == closestEnemy.gameObject)
        {
            return true;
        }

        return false;
    }

    private void FireShot()
    {
        GetComponent<AudioSource>().PlayOneShot(m_ShootSound);
        GameObject go = Instantiate(m_EnemyShot) as GameObject;
        go.GetComponent<EnemyShot>().Init(transform.position, transform.forward, 2f);
    }

}