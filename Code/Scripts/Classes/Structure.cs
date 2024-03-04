using UnityEngine;
using System;

[Serializable]
public abstract class Structure
{
    public string name;
    public int cost;
    public int[] size;
    public GameObject prefab;

    protected Structure(string _name, int _cost, GameObject _prefab, int[] _size)
    {
        name = _name;
        cost = _cost;
        prefab = _prefab;
        size = _size;
    }
}