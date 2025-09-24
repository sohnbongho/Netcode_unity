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
        // 상태 머시
        private StateMechine stateMechine;
        public StateMechine StateMechine => stateMechine;

        // 상태 이름(에디터용)
        public string CurrentStateName => StateMechine?.currentState?.GetType().Name ?? "None";


        // 상태를 저장할 딕셔너리 선언 초기값 설정
        private Dictionary<Type, IState> states;

        // 컴포넌트 캐싱 변수
        [NonSerialized] public Rigidbody2D rb;
        [NonSerialized] public SpriteRenderer spriteRenderer;
        [NonSerialized] public Animator animator;

        // 애니메이션 해시 추출
        public static readonly int hashIsRun = Animator.StringToHash("IsRun");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashDie = Animator.StringToHash("Die");
        public static readonly int hashHit = Animator.StringToHash("Hit");

        // Enemy Stats
        [SerializeField] private EnemyStats enemyStats;

        // 추적 대상
        [SerializeField] private Transform target;
        public LayerMask playerLayer;

        // 프로퍼티
        public Transform Target => target;
        public bool IsDead => CurrentHP <= 0;

        public int CurrentHP { get; private set; }

        #region 상태 관련 메소드
        public void ChangeState<T>() where T : IState
        {
            // 사망상태에서 다른 상태로 전이 불가
            if (IsDead && typeof(DieState) != typeof(T))
            {
                return;
            }

            if (states.TryGetValue(typeof(T), out IState newState))
            {
                stateMechine.ChangeState(newState);
            }
        }

        // 주인공을 검출하는 메소드
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

        // 추적로직
        public void MoveToPlayer()
        {
            if (target == null)
                return;

            // 이동 방향 계산
            Vector2 direction = (target.position - transform.position).normalized;

            // Target의 위치에 따라서 스프라이트 Flip
            spriteRenderer.flipX = direction.x < 0;
            rb.linearVelocity = direction * enemyStats.moveSpeed;

        }
        public void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 공격 사정거리 이내에 있는 확인
        public bool IsInAttackRange()
        {
            if (target == null)
                return false;

            float targetDistance = (transform.position - target.position).sqrMagnitude;
            return targetDistance <= (enemyStats.attackDistance * enemyStats.attackDistance);
        }

        #endregion
        #region 유니티 이벤트
        private void Awake()
        {
            InitState();
            InitComponents();
        }


        private void Start()
        {
            //상태 머신 초기화
            stateMechine = new StateMechine(this);

            // 초기 상태 설정
            ChangeState<IdleState>();
        }
        private void Update()
        {
            // 상태 머신 갯인
            stateMechine.Update();
        }
        #endregion
        #region  초기화
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

            // 물리 관련 처리
            rb.gravityScale = 0.0f;
            rb.freezeRotation = true;

            CurrentHP = enemyStats.maxHp;
        }
        #endregion

        #region 테스트용 코드
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

        #region 애니메이션 이벤트
        // 공격 애니메이션에서 호출할 메소드
        public void OnAttackAnimationEvent()
        {
            if (target == null)
                return;

            target.GetComponent<IDamageable>().TakeDamage(enemyStats.attackDamage);
        }

        #endregion

        #region 데미지 처리
        public void TakeDamage(int damage)
        {
            if (IsDead)
                return;

            // 데미지 처리
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
