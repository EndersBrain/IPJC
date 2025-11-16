using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1f;
    public TextMeshProUGUI text;

    private float _timer;

    private void Update()
    {
        // Always face the camera
        if (Camera.main != null) {
            transform.LookAt(Camera.main.transform);
            transform.forward = -transform.forward;
        }
        
        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Auto-destroy
        _timer += Time.deltaTime;
        if (_timer >= lifetime) {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float amount, bool crit)
    {
        text.text = crit ? $"<color=yellow>{amount:F0}!</color>" : amount.ToString("F0");
    }
}