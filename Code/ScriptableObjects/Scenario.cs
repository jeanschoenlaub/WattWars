using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scenario", menuName = "TowerDefense/Scenario")]
public class Scenario : ScriptableObject {
    public string scenarioName;
    public int Lives;
    public List<Day> days; // Days in the scenario
}