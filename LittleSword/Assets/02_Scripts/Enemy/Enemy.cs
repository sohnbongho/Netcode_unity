using UnityEngine;
using LittelSword.Enemy.FSM;
using UnityEngine.InputSystem;

namespace LittelSword.Enemy
{
    public class Enemy : MonoBehaviour
    {
        // ���� �ӽ�
        private StateMechine stateMechine;
        
        public void ChangeState(IState newState)
        {
            stateMechine.ChangeState(newState);
        }
        #region ����Ƽ �̺�Ʈ
        
        private void Start()
        {
            //���� �ӽ� �ʱ�ȭ
            stateMechine = new StateMechine(this);

            // �ʱ� ���� ����
            ChangeState(new IdleState());
        }
        private void Update()
        {
            // ���� �ӽ� ����
            stateMechine.Update();

            // �׽�Ʈ �ڵ�
            TestFSM();
        }
        #endregion

        #region �׽�Ʈ�� �ڵ�
        private void TestFSM()
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChangeState(new IdleState());
            }
            else if(Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ChangeState(new ChaseState());
            }
            else if(Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ChangeState(new AttackState());
            }
        }
        #endregion
    }
}
