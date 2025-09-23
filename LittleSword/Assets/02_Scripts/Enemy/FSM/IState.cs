namespace LittelSword.Enemy.FSM
{
    public interface IState
    {
        void Enter(Enemy enemy);
        void Update(Enemy enemy); // Excute
        void Exit(Enemy enemy);
    }
}

