using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.InputSystem
{
    public class InputHandler : MonoBehaviour, IInputEvents
    {
        public event Action<Vector2> OnMove;
        public event Action OnAttack;

        // InputAction 연결
        private InputSystem_Actions inputActions;

        private InputAction moveAction;
        private InputAction attackAction;

        private void Awake()
        {
            inputActions = new InputSystem_Actions();
            moveAction = inputActions.Player.Move;
            attackAction = inputActions.Player.Attack;
        }

        private void OnEnable()
        {
            inputActions.Enable();

            // 이벤트 연결
            moveAction.performed += HandleMove;
        }

        private void OnDisable()
        {
            inputActions.Disable();

            // 이벤트 해지
            moveAction.performed -= HandleMove;
        }

        private void HandleMove(InputAction.CallbackContext ctx)
        {
            Logger.Log($"Move: {ctx.ReadValue<Vector2>()}");
        }

    }

}
