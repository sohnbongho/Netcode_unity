
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class IdleState : IState
    {
        private readonly float detectInerval;
        private float lastDetectTime;
        public IdleState(float detectInerval)
        {
            this.detectInerval = detectInerval;
            lastDetectTime = Time.time - detectInerval;
        }

        public void Enter(Enemy enemy)
        {
            Logger.Log("Idle 진입");
            enemy.animator.SetBool(Enemy.hashIsRun, false);
        }
        public void Update(Enemy enemy)
        {
            if (Time.time - lastDetectTime < detectInerval)
                return;

            lastDetectTime = Time.time;

            Logger.Log("Idle 갱신");
            if (enemy.DetectPlayer())
            {
                enemy.ChangeState<ChaseState>();
            }
        }
        public void Exit(Enemy enemy)
        {
            Logger.Log("Idle 종료");
        }
    }
}
