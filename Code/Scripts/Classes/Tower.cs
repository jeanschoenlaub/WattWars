using System;
using UnityEngine;

[Serializable]
public class Tower: Structure
{
    public Tower(string _name, int _cost, GameObject _prefab, int[] _size)
        : base(_name, _cost, _prefab, _size)
    {
    }
}
