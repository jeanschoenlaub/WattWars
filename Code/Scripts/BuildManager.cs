using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private Tower[] towers;

    private int selectedTowerIndex = 0;

    void Awake()
    {
      main = this;
    }

    public Tower GetSelectedTower(){
        return towers[selectedTowerIndex];
    }

    public void SetSelectedTower(int _towerIndex){
       selectedTowerIndex = _towerIndex;
    }
}
