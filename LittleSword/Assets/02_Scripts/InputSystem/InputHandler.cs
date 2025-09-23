using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
            attackAction.performed += HandleAttack;
        }

        private void OnDisable()
        {
            inputActions.Disable();

            // 이벤트 해지
            moveAction.performed -= HandleMove;
            attackAction.performed -= HandleAttack;
        }

        private void HandleMove(InputAction.CallbackContext ctx)
        {
            OnMove?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void HandleAttack(InputAction.CallbackContext obj)
        {
            OnAttack?.Invoke();
        }
    }

}
