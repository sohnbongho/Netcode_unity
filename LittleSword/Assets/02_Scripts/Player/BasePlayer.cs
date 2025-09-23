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


        #region 유니티 이벤트
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

        #region 초기화
        private void InitControllers()
        {
            inputHandler = GetComponent<InputHandler>();
        }
        private void InitComponents()
        {
            rb = GetComponent<Rigidbody2D>();

        }
        #endregion

        #region 공통 메소드
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
