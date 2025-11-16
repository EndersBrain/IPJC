using UnityEngine;
using System.Collections.Generic;

// This is the "recipe" for a spell (list of effects)
[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Spell Definition")]
public class SpellDefinition : ScriptableObject
{
    [Tooltip("The basic projectile prefab to spawn.")]
    public GameObject projectilePrefab;
    
    [Tooltip("The list of effects that compose this spell.")]
    public List<SpellEffect> effects = new List<SpellEffect>();
}