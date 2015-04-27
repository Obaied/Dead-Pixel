using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityMovement))]
public class PlayerInput : MonoBehaviour
{
    public EntityMovement.MoveState LastMovementState { get; private set; }
    public EntityMovement.MoveState LastRotateState { get; private set; }

    private EntityMovement m_EntityMovement = null;
    private bool m_DisableInput = false;
    private PlayerActions _playerActions;

    void Awake()
    {
        m_EntityMovement = GetComponent<EntityMovement>();
        _playerActions = GetComponent<PlayerActions>();
        StartCoroutine(UpdateKeyboardInputCo());
    }

    private IEnumerator UpdateKeyboardInputCo()
    {
        while (enabled)
        {
            yield return null;

            if (m_DisableInput)
                continue;

            if (Input.GetButton("Shoot"))
            {
                _playerActions.Shoot();
            }

            if (Input.GetButton("MoveForward"))
            {
                LastMovementState = EntityMovement.MoveState.MOVEFORWARD;
                m_EntityMovement.Move(LastMovementState);
            }

            if (Input.GetButton("MoveBackward"))
            {
                LastMovementState = EntityMovement.MoveState.MOVEBACKWARD;
                m_EntityMovement.Move(LastMovementState);
            }

            if (Input.GetButton("MoveLeft"))
            {
                LastMovementState = EntityMovement.MoveState.MOVELEFT;
                m_EntityMovement.Move(LastMovementState);
            }

            if (Input.GetButton("MoveRight"))
            {
                LastMovementState = EntityMovement.MoveState.MOVERIGHT;
                m_EntityMovement.Move(LastMovementState);
            }

            if (Input.GetButton("RotRight"))
            {
                LastRotateState = EntityMovement.MoveState.ROTRIGHT;
                m_EntityMovement.Move(EntityMovement.MoveState.ROTRIGHT);
            }

            if (Input.GetButton("RotLeft"))
            {
                LastRotateState = EntityMovement.MoveState.ROTLEFT;
                m_EntityMovement.Move(EntityMovement.MoveState.ROTLEFT);
            }
        }
    }

    public void DisableInput()
    {
        m_DisableInput = true;
    }
}