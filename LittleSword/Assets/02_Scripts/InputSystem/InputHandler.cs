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
        }

        private void OnDisable()
        {
            inputActions.Disable();

            // �̺�Ʈ ����
            moveAction.performed -= HandleMove;
        }

        private void HandleMove(InputAction.CallbackContext ctx)
        {
            Logger.Log($"Move: {ctx.ReadValue<Vector2>()}");
        }

    }

}
