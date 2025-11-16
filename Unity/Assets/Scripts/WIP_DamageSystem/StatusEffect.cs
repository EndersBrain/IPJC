using UnityEngine;

// This is the "package" for a set of stat modifiers (status effect (buff/debuff))
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Spells/Status Effect")]
public class StatusEffect : ScriptableObject
{
    public string EffectName;
    public float Duration;
    public StatModifierData[] Modifiers;

    public void Apply(StatController target)
    {
        if (target == null) return;
        foreach (var modData in Modifiers) {
            target.AddModifier(modData.StatToAffect, new StatModifier(modData.Value, modData.Type, Duration, this));
        }
    }

    public void Remove(StatController target)
    {
        if (target == null) return;
        target.RemoveModifiersFromSource(this);
    }
}