using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int value = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance?.AddCoins(value);
        Destroy(gameObject);
    }
}
