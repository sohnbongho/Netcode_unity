using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LittelSword.InputSystem
{
    public class InputHandler : MonoBehaviour, IInputEvents
    {
        public event Action<Vector2> OnMove;
        public event Action OnAttack;

        // InputAction ����
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

            // �̺�Ʈ ����
            moveAction.performed += HandleMove;
            attackAction.performed += HandleAttack;
        }

        private void OnDisable()
        {
            inputActions.Disable();

            // �̺�Ʈ ����
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
