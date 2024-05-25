using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("Map"); // To animate the new tower on the map
    }
}
