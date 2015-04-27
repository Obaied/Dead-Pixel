using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy_Jumper : Enemy
{
    public float m_JumpSpeed = 2f;
    public float m_SpawnOffset = 2f;
    public float m_SeenPlayerDelay = 1f;
    public Image m_JumpScareImage;

    //Public Functions

    public override void CollidedWith(GameObject a_Other)
    {
        switch (a_Other.tag)
        {
            case "Player":
                a_Other.GetComponent<PlayerActions>().TakeDamage(m_BaseDamage);
                break;
        }
    }

    public override void TakeDamage()
    {
        m_Health--;
        if (m_Health <= 0)
        {
            AudioSource.PlayClipAtPoint(
                m_DeathSounds[Random.Range(0, m_DeathSounds.Length)], transform.position);

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
        GetComponent<Collider>().enabled = false;
        //Bury yourself underground
        transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);
    }


    //Private & protected Functions

    protected override IEnumerator UpdateCo()
    {
        while (enabled)
        {
            yield return null;
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
                yield return StartCoroutine(JumpPlayerCo());
            }

            Vector3 vToTarget = vToPlayer;

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
                //Debug.LogError("Angle: " + Vector3.Angle(transform.forward, vToTarget) + " | ToPlayer: " + vToTarget);
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
                PerformMovement(EntityMovement.MoveState.MOVEFORWARD);
            }

            m_PathTimer = 0f;
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
    //}

    private IEnumerator JumpPlayerCo()
    {
        //Change your location to infront of the player, 
        EntityMovement.MoveState lastMoveState = m_Player.GetComponent<PlayerInput>().LastMovementState;
        m_VisibilityDistance *= 10f;

        Vector3 vPos = m_Player.transform.position;
        Vector3 vRot = m_Player.transform.rotation.eulerAngles;
        vRot.y -= 180f;
        switch (lastMoveState)
        {
            case EntityMovement.MoveState.MOVEFORWARD:
                vPos += m_Player.transform.forward * m_SpawnOffset;
                break;
            case EntityMovement.MoveState.MOVELEFT:
                vPos += -m_Player.transform.right * m_SpawnOffset;
                break;
            case EntityMovement.MoveState.MOVERIGHT:
                vPos += m_Player.transform.right * m_SpawnOffset;
                break;
            case EntityMovement.MoveState.MOVEBACKWARD:
                vPos += -m_Player.transform.forward * m_SpawnOffset;
                break;
        }

        vPos.y = transform.position.y;

        var type = DungeonManager.Instance.GetClosestCell(vPos);
        if (type == Cell.CellType.WALL
            || type == Cell.CellType.NULL)
        {
            //Early exit without any further confusion
            Debug.Log(this + " | killed itself because of bad positioning");
            Destroy(this.gameObject);
        }

        transform.position = vPos;
        transform.rotation = Quaternion.Euler(vRot);

        //Start the jump scare
        JumpScareManager.Instance.StartJumpScare();

        // Rise up
        float timer = 0f;
        Vector3 startPos = vPos;
        Vector3 targetPos = vPos;
        targetPos.y = 0f;

        //yield return new WaitForSeconds(2f);
        while (vPos.y != targetPos.y)
        {
            timer += Time.deltaTime * m_JumpSpeed;
            vPos.y = Mathf.SmoothStep(startPos.y, targetPos.y, timer);
           // Debug.Log(this + ": vPos: " + vPos);

            transform.position = vPos;
            yield return new WaitForEndOfFrame();
        }

        GetComponent<Collider>().enabled = true;

        //yield return new WaitForSeconds(2f);
        // play a scary sound, 
        GetComponent<AudioSource>().PlayOneShot(
                m_PlayerSeenSounds[Random.Range(0, m_PlayerSeenSounds.Length)]);

        // wait 1 -> 1.5f seconds
        yield return new WaitForSeconds(m_SeenPlayerDelay);

        // Continue to Search & Destroy
    }
}