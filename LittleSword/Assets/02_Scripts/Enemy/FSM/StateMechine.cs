
namespace LittelSword.Enemy.FSM
{
    public class StateMechine
    {
        private Enemy enemy;

        // 생성자
        public StateMechine(Enemy enemy)
        {
            this.enemy = enemy;
        }

        // 현재 상태를 저장할 변수
        private IState currentState;

        // 상태 전환 메서드
        public void ChangeState(IState newState)
        {
            currentState?.Exit(enemy);
            currentState = newState;
            currentState.Enter(enemy);
        }

        // 현재 상태의 Update를 갱신
        public void Update()
        {
            currentState?.Update(enemy);
        }
    }
}
