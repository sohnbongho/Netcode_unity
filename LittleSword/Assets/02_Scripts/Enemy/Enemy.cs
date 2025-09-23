using LittelSword.Enemy.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LittelSword.Enemy
{
    public class Enemy : MonoBehaviour
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
        public static readonly int hashInRun = Animator.StringToHash("IsRun");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashDie = Animator.StringToHash("Die");
        public static readonly int hashHit = Animator.StringToHash("Hit");

        #region ���°��� �޼ҵ�
        public void ChangeState<T>() where T : IState
        {
            if (states.TryGetValue(typeof(T), out IState newState))
            {
                stateMechine.ChangeState(newState);
            }
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
    }
}
