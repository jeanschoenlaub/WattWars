using System;
using UnityEngine;

[Serializable]
public class Tower: Structure
{
    public GameObject rangeCircle;
    public int range;

    public Tower(string _name, int _cost, GameObject _prefab, int[] _size, GameObject _rangeCircle, int _range)
        : base(_name, _cost, _prefab, _size)
    {
        this.rangeCircle = _rangeCircle;
        this.range = _range;
    }
}
