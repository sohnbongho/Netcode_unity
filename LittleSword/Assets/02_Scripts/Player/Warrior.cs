using LittelSword.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace LittelSword.Player
{
    public class Warrior : BasePlayer
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Vector2 size = new Vector2(1.0f, 2.0f);
        [SerializeField] private Vector2 offset = new Vector2(0.5f, 0.0f);


        // 애니메이션 이벤트에서 호출
        public void OnWarriorAttack()
        {
            Vector2 center = transform.position;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center + offset, size, 0, enemyLayer);
            foreach(var collider in colliders)
            {
                collider.GetComponent<IDamageable>()?.TakeDamage(playerStats.attackDamage);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position + new Vector3(offset.x, offset.y, 0), 
                new Vector3(size.x, size.y, 0));
        }

    }

}
