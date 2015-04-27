using System;
using UnityEngine;
using System.Collections;

public class EntityMovement : MonoBehaviour
{
    public enum MoveState
    {
        STILL,
        ROTLEFT,
        ROTRIGHT,
        MOVEFORWARD,
        MOVELEFT,
        MOVERIGHT,
        MOVEBACKWARD
    };

    public float        m_MoveSpeed = 1f;
    public float        m_RotateSpeed = 1f;

    public MoveState   CurrState { get; private set; }
    public MoveState   NextState { get; private set; }

    private Vector3     m_StartPos = Vector3.zero;
    private Vector3     m_TargetPos = Vector3.zero;

    private Vector3     m_StartRot = Vector3.zero;
    private Vector3     m_TargetRot = Vector3.zero;

    private float m_Timer = 0f;

    private bool m_HitObstacle = false;


    //Unity Functions

    void Awake()
    {
        CurrState = MoveState.STILL;
        NextState = MoveState.STILL;
    }

    void Update()
    {
        if (CurrState == MoveState.MOVEFORWARD
            || CurrState == MoveState.MOVEBACKWARD
            || CurrState == MoveState.MOVELEFT
            || CurrState == MoveState.MOVERIGHT)
        UpdateMovement();

        else if (CurrState == MoveState.ROTRIGHT
                 || CurrState == MoveState.ROTLEFT)
        UpdateRotation();
    }


    //Public functions

    public void Move(MoveState a_State)
    {
        SetState(a_State);
    }

    public void SetMovementSpeed(float a_Speed)
    {
        m_MoveSpeed = a_Speed;
    }

    public void SetRotationSpeed(float a_Speed)
    {
        m_RotateSpeed = a_Speed;
    }

    public float GetMovementSpeed()
    {
        return m_MoveSpeed;
    }

    public float GetRotationSpeed()
    {
        return m_RotateSpeed;
    }

    //Private Functions

    private void UpdateMovement()
    {
        m_Timer += Time.deltaTime*m_MoveSpeed;

        HandleCollisions();

        Vector3 currPos = transform.position;
        currPos.x = Mathf.SmoothStep(m_StartPos.x, m_TargetPos.x, m_Timer);
        currPos.z = Mathf.SmoothStep(m_StartPos.z, m_TargetPos.z, m_Timer);

        //rigidbody.MovePosition(currPos);
        transform.position = currPos;

        if (m_Timer >= 1f)
        {
            CurrState = NextState;
            NormalizePosition();
            SetStartTarget();
            m_HitObstacle = false;
        }

    }

    private void UpdateRotation()
    {
        m_Timer += Time.deltaTime*m_RotateSpeed;

        Vector3 currRot = transform.rotation.eulerAngles;
        currRot.y = Mathf.SmoothStep(m_StartRot.y, m_TargetRot.y, m_Timer);

        //rigidbody.MoveRotation(Quaternion.Euler(currRot));
        transform.rotation = Quaternion.Euler(currRot);

        if (m_Timer >= 1f)
        {
            CurrState = NextState;
            SetStartTarget();
        }
    }

    private void NormalizePosition()
    {
        Vector3 vPos = transform.position;
        vPos.x = Mathf.Round(vPos.x);
        vPos.z = Mathf.Round(vPos.z);
        transform.position = vPos;
    }

    private void HandleCollisions()
    {
        Vector3 vDir = (m_TargetPos - m_StartPos).normalized;
        GameObject hitObject;

        bool bHit = RaycastObstacle(vDir, out hitObject);
        if (bHit && !m_HitObstacle)
        {
            CollidedWith(hitObject);

            m_TargetPos.x = m_StartPos.x;
            m_TargetPos.z = m_StartPos.z;
            m_StartPos = transform.position;
            m_Timer = 0f;
            m_HitObstacle = true;
        }
    }

    private void CollidedWith(GameObject a_Other)
    {
        var enemy = GetComponent<Enemy>();
        if (enemy != null)
            enemy.CollidedWith(a_Other);
    }

    private bool RaycastObstacle(Vector3 a_Dir, out GameObject a_hitObject)
    {
        a_hitObject = null;
        bool bHit = false;
        RaycastHit hitInfo;

        Vector3 vOrigin = transform.position;
        BoxCollider boxCollider = GetComponent<Collider>() as BoxCollider;

        vOrigin.y = boxCollider.transform.position.y;
        bHit = Physics.Raycast(vOrigin, a_Dir, out hitInfo);

        if (bHit && hitInfo.distance < 0.40f)
        {
            a_hitObject = hitInfo.transform.gameObject;
            return true;
        }

        return false;
    }

    private void SetState(MoveState a_State)
    {
        if (CurrState == MoveState.STILL)
        {
            CurrState = a_State;
            SetStartTarget();
        }
        else if (a_State != CurrState)
        {
            NextState = a_State;
        }
    }

    private void SetStartTarget()
    {
        switch (CurrState)
        {
            case MoveState.ROTLEFT:
                NextState = MoveState.STILL;
                m_Timer = 0f;
                m_StartRot = transform.rotation.eulerAngles;
                m_TargetRot = m_StartRot + new Vector3(0f, -90f, 0f);
                break;

            case MoveState.ROTRIGHT:
                 NextState = MoveState.STILL;
                m_Timer = 0f;
                m_StartRot = transform.rotation.eulerAngles;
                m_TargetRot = m_StartRot + new Vector3(0f, 90f, 0f);
                break;
            case MoveState.MOVEFORWARD:
                NextState = MoveState.STILL;
                m_Timer = 0f;
                m_StartPos = transform.position;
                m_TargetPos = transform.position + transform.forward;
                break;
            case MoveState.MOVELEFT:
                NextState = MoveState.STILL;
                m_Timer = 0f;
                m_StartPos = transform.position;
                m_TargetPos = transform.position - transform.right;
                break;
            case MoveState.MOVERIGHT:
                NextState = MoveState.STILL;
                m_Timer = 0f;
                m_StartPos = transform.position;
                m_TargetPos = transform.position + transform.right;
                break;
            case MoveState.MOVEBACKWARD:
                NextState = MoveState.STILL;
                m_Timer = 0f;
                m_StartPos = transform.position;
                m_TargetPos = transform.position - transform.forward;
                break;
        }
    }


}
