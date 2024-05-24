using UnityEngine;

public class Scenario3Reward : MonoBehaviour
{
    public GameObject objectToActivate;
    
     // This function will be called by the animation event
    public void ActivateLvlRewardUI()
    {
        objectToActivate.SetActive(true);
    }

    public void DeActivateLvlRewardUI()
    {
        objectToActivate.SetActive(false);
    }
}
