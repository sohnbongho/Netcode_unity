using LittelSword.InputSystem;
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Player
{
    public class BasePlayer : MonoBehaviour
    {
        // Controllers
        private InputHandler inputHandler;

        #region 유니티 이벤트
        protected void Awake()
        {
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
        #endregion

        #region 공통 메소드
        protected virtual void Attack()
        {
            Logger.Log($"Attack");
        }

        protected virtual void Move(Vector2 direction)
        {
            Logger.Log($"Move:" + direction);
        }

        #endregion

    }

}
