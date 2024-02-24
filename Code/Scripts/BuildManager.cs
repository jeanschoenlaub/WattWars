using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTowerIndex;

    void Awake()
    {
      main = this;
    }

    public GameObject GetSelectedTower(){
        return towerPrefabs[selectedTowerIndex];
    }
}
