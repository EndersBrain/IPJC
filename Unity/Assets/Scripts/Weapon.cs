/////////////////////////////////
// WIP / VERY EXPERIMENTAL !!! //
/////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [Tooltip("The 'base' spell, like a main attack gem.")]
    public SpellDefinition baseSpell;
    
    [Tooltip("The 'support' effects, like Noita modifiers or support gems.")]
    public List<SpellEffect> modifierEffects = new List<SpellEffect>();
    
    [Tooltip("Drag the 'BarrelEnd' or 'WandTip' empty GameObject here.")]
    public Transform spawnPoint;

    // This is called by the PlayerWeaponController
    public void Fire(Vector3 aimDirection, StatController ownerStats)
    {
        if (baseSpell == null || spawnPoint == null || ownerStats == null) {
            Debug.LogError("Weapon is not configured!");
            return;
        }

        List<SpellEffect> finalEffects = new List<SpellEffect>();
        
        if (baseSpell.effects != null) {
            finalEffects.AddRange(baseSpell.effects);
        }
        
        if (modifierEffects != null) {
            finalEffects.AddRange(modifierEffects);
        }

        GameObject projGO = Instantiate(
            baseSpell.projectilePrefab, // Prefab comes from the base spell
            spawnPoint.position, 
            Quaternion.LookRotation(aimDirection)
        );

        Projectile projectile = projGO.GetComponent<Projectile>();
        if (projectile != null) {
            projectile.Initialize(finalEffects, aimDirection, ownerStats);
        } else {
            Debug.LogError($"Prefab missing Projectile component!");
        }
    }
}