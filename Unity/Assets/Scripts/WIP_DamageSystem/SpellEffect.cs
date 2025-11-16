using UnityEngine;

// The base class for ALL spell effects. New effects inherit from this
public abstract class SpellEffect : ScriptableObject
{
    // Called once when the projectile is spawned. Use for setting stats
    public virtual void Initialize(Projectile projectile) { }

    // Called every frame. Use for homing, wobbling, etc
    public virtual void OnUpdate(Projectile projectile) { }

    // Called on a fixed timer. Use for periodic effects
    public virtual void OnTick(Projectile projectile) { }

    // Called BEFORE the target's TakeHit. This is the "pipeline" where you modify the HitContext (add/convert damage, add status ...)
    public virtual void OnCompileHit(Projectile projectile, HitContext context) { }

    // Called AFTER the target's TakeHit. Use for "Hit Twice", "Explode on Hit", etc. logic
    public virtual void OnHit(Projectile projectile, HitContext context) { }

    // Called just before the projectile is destroyed by its lifetime ending
    public virtual void OnLifetimeEnd(Projectile projectile) { }
}