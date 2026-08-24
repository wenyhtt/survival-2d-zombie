using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
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
            // Destroy the enemy
            Destroy(collision.gameObject);
            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
