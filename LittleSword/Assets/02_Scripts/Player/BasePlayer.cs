using LittelSword.InputSystem;
using System;
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Player
{
    public class BasePlayer : MonoBehaviour
    {
        // Controllers
        private InputHandler inputHandler;

        // Components
        protected Rigidbody2D rb;


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
        private void InitControllers()
        {
            inputHandler = GetComponent<InputHandler>();
        }
        private void InitComponents()
        {
            rb = GetComponent<Rigidbody2D>();

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
            const float speed = 3.0f;
            rb.linearVelocity = direction * speed;
        }

        #endregion

    }

}
