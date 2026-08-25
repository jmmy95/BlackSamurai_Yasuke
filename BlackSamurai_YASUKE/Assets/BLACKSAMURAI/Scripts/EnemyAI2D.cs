using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private Rigidbody2D rb;
    private Transform player;
    private float nextAttackTime;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

            if (direction > 0 && !facingRight) Flip();
            if (direction < 0 && facingRight) Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (distance <= attackRange && Time.time >= nextAttackTime)
            Attack();
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(attackDamage);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}
