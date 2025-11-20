using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// These will expand with time
// NOTE: DO NOT CHANGE THE ORDER. ALL EXISTING ASSETS WILL BREAK!
public enum DamageType { Physical, Fire }
public enum StatType { Health, Armor, CritChance, CritDamage, FireDamageBonus, MoveSpeed }
public enum ModifierType { Flat, PercentAdd, PercentMultiply }

[System.Serializable]
public class DamageInstance
{
    public DamageType Type;
    public float Amount;
}

[System.Serializable]
public class StatModifierData
{
    public StatType StatToAffect;
    public float Value;
    public ModifierType Type;
}

[System.Serializable]
public struct StatusEffectApplication
{
    public StatusEffect Effect;
    public float Duration;
}

// damage that reaches the target HP
[System.Serializable]
public struct FinalDamageResult
{
    public float TotalDamage;
    public bool WasCritical;
}

// A single modifier, e.g., "+5 Armor for 10s"
// 'Source' is the object that applied it (e.g., a StatusEffect) so it can be removed by that same source
// -1 or 0 means permanent
[System.Serializable]
public class StatModifier
{
    public float Value;
    public ModifierType Type;
    public float Duration;
    public readonly object Source;

    public StatModifier(float value, ModifierType type, float duration, object source)
    {
        Value = value;
        Type = type;
        Duration = duration;
        Source = source;
    }
}

// A single stat (e.g., Armor) that manages its base value and a list of active modifiers
[System.Serializable]
public class Stat
{
    public float BaseValue;
    public List<StatModifier> m_modifiers = new List<StatModifier>();


    // TODO: FIXME: order of application


    // Calculates the final value of the stat based on all active modifiers
    public float GetValue()
    {
        float finalValue = BaseValue;
        float percentAdd = 0;
        
        m_modifiers.Sort((a, b) => a.Type.CompareTo(b.Type));

        foreach (var mod in m_modifiers) {
            if (mod.Type == ModifierType.Flat) {
                finalValue += mod.Value;
            } else if (mod.Type == ModifierType.PercentAdd) {
                percentAdd += mod.Value; // Sum all PercentAdd modifiers
            } else if (mod.Type == ModifierType.PercentMultiply) {
                finalValue *= (1f + mod.Value); // Apply each PercentMultiply individually
            }
        }

        finalValue *= (1f + percentAdd); // Apply the sum of PercentAdd modifiers
        return (float)System.Math.Round(finalValue, 4);
    }

    public void AddModifier(StatModifier mod)
    {
        m_modifiers.Add(mod);
    }

    public void RemoveModifiersFromSource(object source)
    {
        m_modifiers.RemoveAll(mod => mod.Source == source);
    }

    // Called by StatController's Update to tick down modifier durations
    public void UpdateTimers(float deltaTime)
    {
        for (int i = m_modifiers.Count - 1; i >= 0; i--) {
            // -1 or 0 means permanent
            if (m_modifiers[i].Duration > 0) {
                m_modifiers[i].Duration -= deltaTime;
                if (m_modifiers[i].Duration <= 0) {
                    m_modifiers.RemoveAt(i);
                }
            }
        }
    }
}

// This is the "packet" of information passed from a Projectile to an IDamageable. SpellEffects modify this packet in a pipeline
public class HitContext
{
    public IDamageable Target { get; }
    public StatController AttackerStats { get; }
    public List<DamageInstance> Damages { get; set; }
    public List<StatusEffectApplication> StatusEffects { get; set; }

    public HitContext(IDamageable target, StatController attacker)
    {
        Target = target;
        AttackerStats = attacker;
        Damages = new List<DamageInstance>();
        StatusEffects = new List<StatusEffectApplication>();
    }
}


// An interface for any object that can be hit by a projectile and process a HitContext
public interface IDamageable
{
    // Returns the StatController of this object
    StatController GetStatController();

    // Processes the compiled HitContext
    void TakeHit(HitContext context);

    // Gets the transform of the damageable object
    Transform GetTransform();
}




// TODO: FIXME: THIS IS JUST A STUB
// A static class that handles all damage calculation logic
// This is decoupled from the Enemy, so it can be used by any IDamageable object (Enemies, Players, Destructible environments, etc.)
public static class DamageCalculator
{
    public static FinalDamageResult CalculateHit(HitContext context)
    {
        float totalDamage = 0;
        
        StatController targetStats = context.Target.GetStatController();
        if (targetStats == null) {
            Debug.LogWarning("Hit target has no StatController!");
            return new FinalDamageResult { TotalDamage = 0, WasCritical = false };
        }

        float critChance = context.AttackerStats.GetStatValue(StatType.CritChance);
        bool isCritical = Random.value < (critChance  / 100);

        foreach (var damage in context.Damages) {
            float amount = damage.Amount;
            
            if (damage.Type == DamageType.Fire)
                amount *= (1 + context.AttackerStats.GetStatValue(StatType.FireDamageBonus));
            
            if (damage.Type == DamageType.Physical)
                amount -= targetStats.GetStatValue(StatType.Armor);
            
            totalDamage += Mathf.Max(0, amount);
        }
        
        if (isCritical) {
            float critDamage = context.AttackerStats.GetStatValue(StatType.CritDamage);
            totalDamage = totalDamage + totalDamage * (critDamage / 100);
        }

        return new FinalDamageResult {
            TotalDamage = totalDamage,
            WasCritical = isCritical
        };
    }
}

