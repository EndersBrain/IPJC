using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform handPosition;
    private GameObject currentWeapon;

    public void Equip(Item item)
    {
        if (item.prefabToEquip == null)
        {
            Debug.LogWarning("Itemul nu are prefabToEquip setat!");
            return;
        }

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(item.prefabToEquip, handPosition);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }
}
