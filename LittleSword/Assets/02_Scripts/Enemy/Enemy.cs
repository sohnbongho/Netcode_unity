using UnityEngine;
using LittelSword.Enemy.FSM;
using UnityEngine.InputSystem;

namespace LittelSword.Enemy
{
    public class Enemy : MonoBehaviour
    {
        // 상태 머시
        private StateMechine stateMechine;
        
        public void ChangeState(IState newState)
        {
            stateMechine.ChangeState(newState);
        }
        #region 유니티 이벤트
        
        private void Start()
        {
            //상태 머신 초기화
            stateMechine = new StateMechine(this);

            // 초기 상태 설정
            ChangeState(new IdleState());
        }
        private void Update()
        {
            // 상태 머신 갯인
            stateMechine.Update();

            // 테스트 코드
            TestFSM();
        }
        #endregion

        #region 테스트용 코드
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
