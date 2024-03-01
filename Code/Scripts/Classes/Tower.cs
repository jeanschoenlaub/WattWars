using System;
using UnityEngine;

[Serializable]
public class Tower
{
    public string name;
    public int cost;
    public int[] size; //2x1 array in terms of plot size eg. [2,2] - [x,y]
    public GameObject prefab;

    public Tower (string _name, int _cost, GameObject _prefab, int[] _size){
        name = _name;
        cost = _cost;
        prefab = _prefab;
        size = _size;
    }

}
