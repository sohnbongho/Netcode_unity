
using LittelSword.Player;
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

                // 타겟이 없거나 또는 사망했을 경우, Idle상태로 전환
                if (enemy.Target == null || enemy.Target.GetComponent<BasePlayer>()?.IsDead == true)
                {
                    Logger.Log("AttackState 갱신");
                    enemy.ChangeState<IdleState>();
                    return;
                }

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
