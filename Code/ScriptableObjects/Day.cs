using System.Collections.Generic;
using UnityEngine;

public enum Weather {
    Sunny,
    Overcast,
    Cloudy
}

[CreateAssetMenu(fileName = "New Day", menuName = "TowerDefense/Day")]
public class Day : ScriptableObject {
    public List<Wave> waves; // Waves scheduled for this day
    public Weather weather;
}