using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBarManager : MonoBehaviour
{   
    public static ToolBarManager instance;
    public Item[] startItems;
    public int maxstack = 64;
    public ToolBarSlot[] toolbarSlots;
    public GameObject toolbarItemPrefab;

    int selectedSlot = -1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeSelectedSlot(0);
        foreach (var item in startItems)
        {
            AddItem(item);
        }
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number <= toolbarSlots.Length)
            {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            toolbarSlots[selectedSlot].Deselect();
        }

        toolbarSlots[newValue].Select();
        selectedSlot = newValue;

    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < toolbarSlots.Length; i++)
        {
            ToolBarSlot slot = toolbarSlots[i];
            ToolBarItem itemInSlot = slot.GetComponentInChildren<ToolBarItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxstack && itemInSlot.item.stackable == true) 
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
            else if (itemInSlot != null && itemInSlot.item == item && itemInSlot.item.stackable == false && itemInSlot.count != 0)
            {
                return false;
            }
        }

        for (int i = 0; i < toolbarSlots.Length; i++)
        {
            ToolBarSlot slot = toolbarSlots[i];
            ToolBarItem itemInSlot = slot.GetComponentInChildren<ToolBarItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    void SpawnNewItem(Item item, ToolBarSlot slot)
    {
        GameObject newItemGo = Instantiate(toolbarItemPrefab, slot.transform);
        ToolBarItem toolbarItem = newItemGo.GetComponent<ToolBarItem>();
        toolbarItem.InitialiseItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        ToolBarSlot slot = toolbarSlots[selectedSlot];
        ToolBarItem itemInSlot = slot.GetComponentInChildren<ToolBarItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }

    public Item GetSelectedItemRaw()
    {
        if (selectedSlot < 0 || selectedSlot >= toolbarSlots.Length)
            return null;

        ToolBarSlot slot = toolbarSlots[selectedSlot];
        ToolBarItem itemInSlot = slot.GetComponentInChildren<ToolBarItem>();
        return itemInSlot != null ? itemInSlot.item : null;
    }
}