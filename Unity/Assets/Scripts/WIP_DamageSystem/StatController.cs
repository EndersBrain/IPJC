using UnityEngine;
using System.Collections.Generic;

public class StatController : MonoBehaviour
{
    [System.Serializable]
    public class StatEntry {
        public StatType type;
        public Stat stat;
    }

    public List<StatEntry> statEntries = new List<StatEntry>();


    public Dictionary<StatType, Stat> Stats = new Dictionary<StatType, Stat>();

    void Awake()
    {
        Stats.Clear();
        foreach (var entry in statEntries) {
            Stats[entry.type] = entry.stat;
        }
    }

    void Update()
    {
        foreach (var stat in Stats.Values) {
            stat.UpdateTimers(Time.deltaTime);
        }
    }

    // NOTE: this getter is meant for stats that do not track their value. ex: armor, damage, crit
    public float GetStatValue(StatType type)
    {
        if (Stats.TryGetValue(type, out Stat stat)) {
            return stat.GetValue();
        }
        
        Debug.LogWarning($"Stat {type} not found on {gameObject.name}");
        return 0;
    }

    // NOTE: this getter is meant for stats with dynamic values. ex: health, mana
    public float GetCurrentStatValue(StatType type)
    {
        if (Stats.TryGetValue(type, out Stat stat)) {
            return stat.GetValue();
        }
        
        Debug.LogWarning($"Stat {type} not found on {gameObject.name}");
        return 0;
    }

    public void SubtractCurrentStatValue(StatType type, float value, object Source)
    {
        if (Stats.TryGetValue(type, out Stat stat)) {
            // TODO: make stat able to track their own value for stats such as health, mana and so on. This will allow us to:
            // 1. correctly track current value (ex: current health is different than max health)
            // 2. apply tick effects (ex: poison = periodic damage. regen = periodic heal)

            // FIXME: THIS IS A TEMPORARY HACK. this basically just subtracts max health
            stat.AddModifier(new StatModifier(-value, ModifierType.Flat, 0, Source));
        }
    }

    public void AddCurrentStatValue(StatType type, float value, object Source)
    {
        if (Stats.TryGetValue(type, out Stat stat)) {
            // Same goes here. ex: insntant heal from other sources
            // Might need overloads / special for certain stats
        }
    }

    public void AddModifier(StatType stat, StatModifier mod)
    {
        if (Stats.TryGetValue(stat, out Stat s)) {
            s.AddModifier(mod);
        }
    }

    public void RemoveModifiersFromSource(object source)
    {
        foreach (var stat in Stats.Values) {
            stat.RemoveModifiersFromSource(source);
        }
    }
}
