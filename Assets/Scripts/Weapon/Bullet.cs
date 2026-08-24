using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage = 25; // Amount of damage this bullet deals
    private Vector2 direction;

    public void Setup(Vector2 dir)
    {
        direction = dir.normalized;
        // Destroy the bullet after some time so it doesn't clutter the scene
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Move the bullet every frame
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Apply damage instead of instantly destroying
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                // Fallback to instant kill if no Health component exists yet
                Destroy(collision.gameObject);
            }
            
            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
