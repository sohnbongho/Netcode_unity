using LittelSword.Interfaces;
using UnityEngine;

namespace LittelSword.Player
{
    public class Warrior : BasePlayer
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Vector2 size = new Vector2(1.0f, 2.0f);
        [SerializeField] private float offset = 0.5f;


        // 애니메이션 이벤트에서 호출
        public void OnWarriorAttack()
        {
            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            Vector2 center = (Vector2)transform.position + direction * offset;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0, enemyLayer);
            foreach (var collider in colliders)
            {
                collider.GetComponent<IDamageable>()?.TakeDamage(playerStats.attackDamage);
            }
        }

        private void OnDrawGizmos()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            Vector2 center = (Vector2)transform.position + direction * offset;

            Gizmos.color = new Color(1f, 0, 0, 0.3f);
            Gizmos.DrawCube(center, new Vector3(size.x, size.y, 0));
        }

    }

}
