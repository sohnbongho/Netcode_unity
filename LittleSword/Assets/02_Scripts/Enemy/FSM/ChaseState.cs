
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class ChaseState : IState
    {
        public void Enter(Enemy enemy)
        {
            Logger.Log("ChaseState 진입");
            enemy.animator.SetBool(Enemy.hashInRun, true);
        }
        public void Update(Enemy enemy)
        {
            Logger.Log("ChaseState 갱신");
        }
        public void Exit(Enemy enemy)
        {
            Logger.Log("ChaseState 종료");
        }
    }
}
