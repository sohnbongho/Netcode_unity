using LittelSword.InputSystem;
using LittelSword.Player.Controller;
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Player
{
    public class BasePlayer : MonoBehaviour
    {
        // Controllers
        private InputHandler inputHandler;
        private MovementController movementController;

        // Components
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;


        #region ����Ƽ �̺�Ʈ
        protected void Awake()
        {
            InitComponents();
            InitControllers();
        }



        protected void OnEnable()
        {
            inputHandler.OnMove += Move;
            inputHandler.OnAttack += Attack;
        }

        protected void OnDisable()
        {
            inputHandler.OnMove -= Move;
            inputHandler.OnAttack -= Attack;
        }

        #endregion

        #region �ʱ�ȭ
        private void InitComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        private void InitControllers()
        {
            inputHandler = GetComponent<InputHandler>();
            movementController = new MovementController(rb, spriteRenderer);
        }
        #endregion

        #region ���� �޼ҵ�
        protected virtual void Attack()
        {
            Logger.Log($"Attack");
        }

        protected virtual void Move(Vector2 direction)
        {
            Logger.Log($"Move:" + direction);
            const float speed = 5.0f;
            movementController.Move(direction, speed);
        }

        #endregion

    }

}
