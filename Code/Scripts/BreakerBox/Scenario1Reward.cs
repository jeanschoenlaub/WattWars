using UnityEngine;

public class Scenario1Reward : MonoBehaviour
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
