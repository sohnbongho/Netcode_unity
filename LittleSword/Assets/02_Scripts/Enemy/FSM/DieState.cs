using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Enemy.FSM
{
    public class DieState : IState
    {
        public DieState()
        {
        }

        public void Enter(Enemy enemy)
        {
            Logger.Log("DieState 진입");
            enemy.animator.SetTrigger(Enemy.hashDie);
            enemy.StopMoving();
            enemy.GetComponent<Collider2D>().enabled = false;
            enemy.rb.bodyType = RigidbodyType2D.Kinematic;
            Object.Destroy(enemy.gameObject, 5.0f);
        }

        public void Update(Enemy enemy)
        {
        }
        public void Exit(Enemy enemy)
        {
        }
    }
}
