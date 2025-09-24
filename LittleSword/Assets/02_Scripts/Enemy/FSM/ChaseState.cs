
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class ChaseState : IState
    {
        private readonly float detectInerval;
        private float lastDetectTime;
        public ChaseState(float detectInerval)
        {
            this.detectInerval = detectInerval;
            lastDetectTime = Time.time - detectInerval;
        }

        public void Enter(Enemy enemy)
        {
            Logger.Log("ChaseState 진입");
            enemy.animator.SetBool(Enemy.hashIsRun, true);
        }
        public void Update(Enemy enemy)
        {
            if (Time.time - lastDetectTime < detectInerval)
                return;

            lastDetectTime = Time.time;

            Logger.Log("ChaseState 갱신");
            if (enemy.DetectPlayer())
            {
                enemy.MoveToPlayer();

                if(enemy.IsInAttackRange())
                {
                    // 이동 정지
                    enemy.StopMoving();
                    enemy.ChangeState<AttackState>();
                }
            }
            else
            {
                enemy.StopMoving();
                enemy.ChangeState<IdleState>();
            }
        }
        public void Exit(Enemy enemy)
        {
            Logger.Log("ChaseState 종료");
        }
    }
}
