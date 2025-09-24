
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class AttackState : IState
    {
        private readonly float attackCooldown;
        private float lastAttackTime;

        // 생성자
        public AttackState(float attackCooldown)
        {
            this.attackCooldown = attackCooldown;
            lastAttackTime = Time.time - attackCooldown;
        }

        public void Enter(Enemy enemy)
        {
            Logger.Log("AttackState 진입");
            enemy.animator.SetTrigger(Enemy.hashAttack);
        }
        public void Update(Enemy enemy)
        {
            Logger.Log("AttackState 갱신");

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;

                if (enemy.IsInAttackRange())
                {
                    enemy.animator.SetBool(Enemy.hashIsRun, false);
                    enemy.animator.SetTrigger(Enemy.hashAttack);
                }
                else
                {
                    enemy.ChangeState<ChaseState>();
                }
            }
        }
        public void Exit(Enemy enemy)
        {
            Logger.Log("AttackState 종료");
        }
    }
}
