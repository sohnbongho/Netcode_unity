using LittelSword.Enemy.FSM;
using LittelSword.Interfaces;
using LittelSword.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LittelSword.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        // ���� �ӽ�
        private StateMechine stateMechine;
        public StateMechine StateMechine => stateMechine;

        // ���� �̸�(�����Ϳ�)
        public string CurrentStateName => StateMechine?.currentState?.GetType().Name ?? "None";


        // ���¸� ������ ��ųʸ� ���� �ʱⰪ ����
        private Dictionary<Type, IState> states;

        // ������Ʈ ĳ�� ����
        [NonSerialized] public Rigidbody2D rb;
        [NonSerialized] public SpriteRenderer spriteRenderer;
        [NonSerialized] public Animator animator;

        // �ִϸ��̼� �ؽ� ����
        public static readonly int hashIsRun = Animator.StringToHash("IsRun");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashDie = Animator.StringToHash("Die");
        public static readonly int hashHit = Animator.StringToHash("Hit");

        // Enemy Stats
        [SerializeField] private EnemyStats enemyStats;

        // ���� ���
        [SerializeField] private Transform target;
        public LayerMask playerLayer;

        // ������Ƽ
        public Transform Target => target;
        public bool IsDead => CurrentHP <= 0;

        public int CurrentHP { get; private set; }

        #region ���� ���� �޼ҵ�
        public void ChangeState<T>() where T : IState
        {
            // ������¿��� �ٸ� ���·� ���� �Ұ�
            if (IsDead && typeof(DieState) != typeof(T))
            {
                return;
            }

            if (states.TryGetValue(typeof(T), out IState newState))
            {
                stateMechine.ChangeState(newState);
            }
        }

        // ���ΰ��� �����ϴ� �޼ҵ�
        public bool DetectPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
                enemyStats.chaseDistance, playerLayer);
            if (colliders.Length > 0)
            {
                target = colliders
                    .OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                    .Where(c => c.GetComponent<BasePlayer>()?.IsDead == false)
                    .First()?
                    .transform ?? null;
                return target != null;
            }
            target = null;
            return false;
        }

        // ��������
        public void MoveToPlayer()
        {
            if (target == null)
                return;

            // �̵� ���� ���
            Vector2 direction = (target.position - transform.position).normalized;

            // Target�� ��ġ�� ���� ��������Ʈ Flip
            spriteRenderer.flipX = direction.x < 0;
            rb.linearVelocity = direction * enemyStats.moveSpeed;

        }
        public void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;
        }

        // ���� �����Ÿ� �̳��� �ִ� Ȯ��
        public bool IsInAttackRange()
        {
            if (target == null)
                return false;

            float targetDistance = (transform.position - target.position).sqrMagnitude;
            return targetDistance <= (enemyStats.attackDistance * enemyStats.attackDistance);
        }

        #endregion
        #region ����Ƽ �̺�Ʈ
        private void Awake()
        {
            InitState();
            InitComponents();
        }


        private void Start()
        {
            //���� �ӽ� �ʱ�ȭ
            stateMechine = new StateMechine(this);

            // �ʱ� ���� ����
            ChangeState<IdleState>();
        }
        private void Update()
        {
            // ���� �ӽ� ����
            stateMechine.Update();
        }
        #endregion
        #region  �ʱ�ȭ
        private void InitState()
        {
            states = new Dictionary<Type, IState>
            {
                [typeof(IdleState)] = new IdleState(enemyStats.detectInterval),
                [typeof(ChaseState)] = new ChaseState(enemyStats.detectInterval),
                [typeof(AttackState)] = new AttackState(enemyStats.attackCooldown),
                [typeof(DieState)] = new DieState()
            };
        }
        private void InitComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // ���� ���� ó��
            rb.gravityScale = 0.0f;
            rb.freezeRotation = true;

            CurrentHP = enemyStats.maxHp;
        }
        #endregion

        #region �׽�Ʈ�� �ڵ�
        private void TestFSM()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChangeState<IdleState>();
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ChangeState<ChaseState>();
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ChangeState<AttackState>();
            }
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,
                enemyStats.chaseDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,
                enemyStats.attackDistance);
        }
        #endregion

        #region �ִϸ��̼� �̺�Ʈ
        // ���� �ִϸ��̼ǿ��� ȣ���� �޼ҵ�
        public void OnAttackAnimationEvent()
        {
            if (target == null)
                return;

            target.GetComponent<IDamageable>().TakeDamage(enemyStats.attackDamage);
        }

        #endregion

        #region ������ ó��
        public void TakeDamage(int damage)
        {
            if (IsDead)
                return;

            // ������ ó��
            CurrentHP -= damage;
            if (IsDead)
            {
                Die();
            }
            else
            {
                animator.SetTrigger(hashHit);
            }
        }

        public void Die()
        {
            ChangeState<DieState>();
        }
        #endregion
    }
}
