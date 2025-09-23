using LittelSword.InputSystem;
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Player
{
    public class BasePlayer : MonoBehaviour
    {
        // Controllers
        private InputHandler inputHandler;

        #region ����Ƽ �̺�Ʈ
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

        #region �ʱ�ȭ
        private void InitControllers()
        {
            inputHandler = GetComponent<InputHandler>();
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
        }

        #endregion

    }

}
