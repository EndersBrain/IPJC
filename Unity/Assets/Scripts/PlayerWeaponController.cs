/////////////////////////////////
// WIP / VERY EXPERIMENTAL !!! //
/////////////////////////////////

using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Tooltip("The camera used for aiming.")]
    public Camera mainCamera;
    
    [Tooltip("The currently equipped weapon.")]
    public Weapon currentWeapon;

    [Tooltip("The currently equipped Item.")]
    public Item currentItem;

    [Tooltip("Max distance for the aiming raycast.")]
    public float aimRaycastDistance = 100f;

    private PlayerControls m_controls; 
    private StatController m_statController;

    private LayerMask m_layerToIgnore;

    void Awake()
    {
        m_statController = GetComponent<StatController>();
        
        m_controls = new PlayerControls();
        
        m_controls.Player.Attack.performed += OnFire;

        m_layerToIgnore = LayerMask.GetMask("Invisible_To_FPV");
    }

    void OnEnable()
    {
        m_controls.Player.Enable();
    }

    void OnDisable()
    {
        m_controls.Player.Disable();
    }

    private void OnFire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (currentWeapon != null && m_statController != null) {
            currentWeapon.Fire(GetAimDirection(), m_statController);
        }
    }

    private Vector3 GetAimDirection()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        Vector3 targetPoint;
        
        if (Physics.Raycast(ray, out RaycastHit hit, aimRaycastDistance, ~m_layerToIgnore)) {
            targetPoint = hit.point;
        } else {
            // no hit. target is far away in that direction
            targetPoint = ray.GetPoint(aimRaycastDistance);
        }

        // final direction from the *gun's spawn point* to the *target point*.
        return (targetPoint - currentWeapon.spawnPoint.position).normalized;
    }
}