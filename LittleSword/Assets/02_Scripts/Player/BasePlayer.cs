using LittelSword.InputSystem;
using LittelSword.Interfaces;
using LittelSword.Player.Controller;
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Player
{
    public class BasePlayer : MonoBehaviour, IDamageable
    {
        // Controllers
        private InputHandler inputHandler;
        private MovementController movementController;
        private AnimationController animationController;

        // Components
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected Collider2D collider;

        // �÷��̾� ����
        public PlayerStats playerStats;

        // ������Ƽ
        public bool IsDead => CurrentHP <= 0;
        public int CurrentHP { get; set; }

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
            animator = GetComponent<Animator>();
            collider = GetComponent<Collider2D>();

            CurrentHP = playerStats.maxHp;
        }
        private void InitControllers()
        {
            inputHandler = GetComponent<InputHandler>();
            movementController = new MovementController(rb, spriteRenderer);
            animationController = new AnimationController(animator);
        }
        #endregion

        #region ���� �޼ҵ�
        protected virtual void Attack()
        {
            Logger.Log($"Attack");
            animationController.Attack();

        }

        protected virtual void Move(Vector2 direction)
        {
            Logger.Log($"Move:" + direction);
            movementController.Move(direction, playerStats.moveSpeed);
            animationController.Move(direction != Vector2.zero);
        }


        #endregion

        #region �ǰݰ��� �޼ҵ�
        public void TakeDamage(int damage)
        {
            if (IsDead)
                return;

            CurrentHP = Mathf.Max(0, CurrentHP - damage);
            if (IsDead)
            {
                Die();
            }
            else
            {
                animationController.Hit();

            }
        }

        public void Die()
        {
            animationController.Die();

            inputHandler.enabled = false;
            collider.enabled = false;
            rb.linearVelocity = Vector2.zero;
        }

        #endregion

    }

}
