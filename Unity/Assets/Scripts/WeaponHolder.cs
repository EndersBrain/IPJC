// Weapon Holder script for equipping and unequipping weapons in Unity

using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform handPosition;
    private GameObject currentWeapon;
    public Item equippedItem;

    public void Equip(Item item)
    {
        if (item.prefabToEquip == null)
        {
            Debug.LogWarning("Itemul doesn't have prefabToEquip set!");
            return;
        }

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(item.prefabToEquip, handPosition);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        equippedItem = item;
    }

    public void Unequip()
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = null;
        equippedItem = null;
    }
}