using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "LittleSword/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public int maxHp = 100;
    public float moveSpeed = 3f;
    public float chaseDistance = 5f;
    public float attackDistance = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

}
