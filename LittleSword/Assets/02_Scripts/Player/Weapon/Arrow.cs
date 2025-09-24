using LittelSword.Interfaces;
using UnityEngine;

namespace LittleSword.Player.Weapon
{
    public class Arrow : MonoBehaviour
    {
        private Rigidbody2D rb;

        public float force = 10.0f;
        public int damage = 30;

        public void Init(float force, int damage)
        {
            this.force = force;
            this.damage = damage;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            // ¹Ù·Î ÈûÀÌ ÆÅ µé¾î°¡°Ô
            rb.AddRelativeForce(transform.right * force, ForceMode2D.Impulse);
            Destroy(gameObject, 3.0f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<IDamageable>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}


