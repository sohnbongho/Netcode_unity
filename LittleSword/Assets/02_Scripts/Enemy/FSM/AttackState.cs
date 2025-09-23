
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class AttackState : IState
    {
        public void Enter(Enemy enemy)
        {
            Logger.Log("AttackState 진입");
            enemy.animator.SetTrigger(Enemy.hashAttack);
        }
        public void Update(Enemy enemy)
        {
            Logger.Log("AttackState 갱신");
        }
        public void Exit(Enemy enemy)
        {
            Logger.Log("AttackState 종료");
        }
    }
}
