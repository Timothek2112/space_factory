using Godot;
using System;

public interface IInventory
{
    public IPutable origin { get; set; }

    public void ShowInventory();
    public void UpdateSlots();
}
