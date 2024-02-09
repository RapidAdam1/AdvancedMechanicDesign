using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;


[UpdateInGroup(typeof(InitializationSystemGroup),OrderLast = true)]
public partial class PlayerInputSystem : SystemBase
{

    private PlayerControls m_input;

    protected override void OnCreate()
    {
        m_input = new PlayerControls();
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<InputMoveComponent>();
    }

    protected override void OnStartRunning()
    {
        m_input.Enable();
        m_input.DefaultMap.Move.started += Handle_MoveStarted;
        m_input.DefaultMap.Move.performed += Handle_MovePerformed;
        m_input.DefaultMap.Move.canceled += Handle_MoveCancelled;

    }

    protected override void OnStopRunning()
    {
        m_input.DefaultMap.Move.started -= Handle_MoveStarted;
        m_input.DefaultMap.Move.performed -= Handle_MovePerformed;
        m_input.DefaultMap.Move.canceled -= Handle_MoveCancelled;

        m_input.Disable();
    }

    protected override void OnUpdate()
    {
        return;
    }

    private void Handle_MoveStarted(InputAction.CallbackContext context)
    {
        foreach ((RefRO<PlayerTag> PlayerTag,Entity e) in SystemAPI.Query<RefRO<PlayerTag>>().WithDisabled<InputMoveComponent>().WithEntityAccess())
        {
            EntityManager.SetComponentEnabled<InputMoveComponent>(e, true);
        }
    }
    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        foreach (RefRW<InputMoveComponent> InputMoveComp in SystemAPI.Query<RefRW<InputMoveComponent>>().WithAll<PlayerTag>())
        {
            InputMoveComp.ValueRW.Value = context.ReadValue<Vector2>();
        }
    }
    private void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        foreach (EnabledRefRW<InputMoveComponent> InputMoveComp in SystemAPI.Query<EnabledRefRW<InputMoveComponent>>().WithAll<PlayerTag>())
        {
            InputMoveComp.ValueRW = false;
        }
    }
}
