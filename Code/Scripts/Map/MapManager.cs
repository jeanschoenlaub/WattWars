using UnityEngine.SceneManagement;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public void BackToLevelSelection(){
        SceneManager.LoadScene("LvlSelection");
    }
}
