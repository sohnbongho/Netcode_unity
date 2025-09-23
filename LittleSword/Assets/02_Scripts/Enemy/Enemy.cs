using LittelSword.Enemy.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LittelSword.Enemy
{
    public class Enemy : MonoBehaviour
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
        public static readonly int hashInRun = Animator.StringToHash("IsRun");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashDie = Animator.StringToHash("Die");
        public static readonly int hashHit = Animator.StringToHash("Hit");

        #region 상태관련 메소드
        public void ChangeState<T>() where T : IState
        {
            if (states.TryGetValue(typeof(T), out IState newState))
            {
                stateMechine.ChangeState(newState);
            }
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
                [typeof(IdleState)] = new IdleState(),
                [typeof(ChaseState)] = new ChaseState(),
                [typeof(AttackState)] = new AttackState()
            };
        }
        private void InitComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
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
    }
}
