using System;
using UnityEngine;

[Serializable]
public class Building
{
    public string name;
    public int cost;
    public int[] size; // 2x1 array in terms of plot size eg. [2,2] - [x,y]
    public GameObject prefab;

    // Additional properties for the new functionality
    public bool isActive;
    public int energyReceived;

    // Constructor
    public Building(string _name, int _cost, GameObject _prefab, int[] _size)
    {
        name = _name;
        cost = _cost;
        prefab = _prefab;
        size = _size;
        isActive = false; // Buildings start as inactive
        energyReceived = 0; // Initial energy received is 0
    }

    // Method to toggle the building on/off
    public void ToggleActive()
    {
        isActive = !isActive;

        // You can add additional logic here to visually indicate the building's state
        // For example, changing the color or enabling/disabling a light
        if (prefab != null)
        {
            var renderer = prefab.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = isActive ? Color.green : Color.red;
            }
        }
    }

    // Method to simulate receiving energy
    public void ReceiveEnergy(int amount)
    {
        if (isActive)
        {
            energyReceived += amount;
            // Here you can add logic for what happens when the building receives energy
            // For example, performing some action once enough energy has been accumulated
        }
    }
}