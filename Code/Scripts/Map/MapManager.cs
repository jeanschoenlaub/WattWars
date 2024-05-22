using UnityEngine.SceneManagement;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public void BackToLevelSelection(){
        Debug.Log("aaa");
        SceneManager.LoadScene("LvlSelection");
    }
}
