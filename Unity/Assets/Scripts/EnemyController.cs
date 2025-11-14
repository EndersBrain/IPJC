using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    private Rigidbody rb;
    private Collider col;

    [SerializeField]
    private FloatingHealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }


    public void Update()
    {

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Debug.Log("Enemy takes damage!");
            TakeDamage(25f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.centerOfMass = new Vector3(0, -0.3f, 0);

            
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0, -1f) * 5f, ForceMode.Impulse);
            rb.AddTorque(new Vector3(100f, 0f, Random.Range(-50f, 50f)));
        }


        StartCoroutine(DestroyAfterDelay(5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

