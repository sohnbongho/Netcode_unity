
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class IdleState : IState
    {
        public void Enter(Enemy enemy)
        {
            Logger.Log("Idle 진입");
            enemy.animator.SetBool(Enemy.hashInRun, false);
        }
        public void Update(Enemy enemy)
        {
            Logger.Log("Idle 갱신");
        }
        public void Exit(Enemy enemy)
        {
            Logger.Log("Idle 종료");
        }
    }
}
