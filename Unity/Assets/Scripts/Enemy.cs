/////////////////////////////////
// WIP / VERY EXPERIMENTAL !!! //
/////////////////////////////////

using UnityEngine;

// An example implementation of an enemy that can be damaged.
// TODO: Could this be more generic and be an inteface?????
// TODO: Health bar?
[RequireComponent(typeof(StatController))]
public class Enemy : MonoBehaviour, IDamageable
{
    public DamagePopup damagePopupPrefab;
    public float popupHeight = 1.4f;

    private StatController m_stats;

    void Awake()
    {
        m_stats = GetComponent<StatController>();
    }
    
    public StatController GetStatController() { return m_stats; }
    public Transform GetTransform() { return transform; }

    public void TakeHit(HitContext context)
    {
        FinalDamageResult result = DamageCalculator.CalculateHit(context);
        
        Debug.Log($"{gameObject.name} took {result.TotalDamage} damage ({ (result.WasCritical ? "CRIT!" : "") }), {m_stats.GetCurrentStatValue(StatType.Health)} -> {m_stats.GetCurrentStatValue(StatType.Health) - result.TotalDamage} health remaining.");

        SpawnDamagePopup(result.TotalDamage, result.WasCritical);

        m_stats.SubtractCurrentStatValue(StatType.Health, result.TotalDamage, context.Target);

        foreach (var app in context.StatusEffects) {
            app.Effect.Apply(m_stats); 
        }

        if (m_stats.GetCurrentStatValue(StatType.Health) <= 0) {
            Destroy(gameObject);
        }
    }

    private void SpawnDamagePopup(float amount, bool crit)
    {
        if (damagePopupPrefab == null) return;

        Vector3 position = transform.position + Vector3.up * popupHeight;

        var popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        popup.SetDamage(amount, crit);
    }
}