using System;
using UnityEngine;

[Serializable]
public class Building : Structure
{
    public Building(string _name, int _cost, GameObject _prefab, int[] _size, GameObject _rangeCircle)
        : base(_name, _cost, _prefab, _size )
    {
    }
}