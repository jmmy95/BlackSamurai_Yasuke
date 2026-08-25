using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 0.35f;

    private Animator animator;
    private float nextAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
            Attack();
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
