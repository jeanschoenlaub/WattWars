using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private Animator VillageSceneTransitionAnimator;
    [SerializeField] private float villageSceneTransitionToMainMenuTime = 2f;
    [SerializeField] private Animator bbSceneTransitionAnimator;
    [SerializeField] private float bbSceneTransitionTime= 2f;

    public string previousScene = "MainMenu";

    // Singletons
    private AudioManager audioManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Make this object persistent
        }
        else if (Instance != this)
        {
            Destroy(gameObject);  // Destroy this instance because it is a duplicate
            return;
        }
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// ----------------------------------------------------- ///
    ///           Village Transition Management            /// 
    /// ----------------------------------------------------- ///


    public IEnumerator StartSceneExitToMainMenuAnimation(){
        audioManager.playButtonClickSFX();
        VillageSceneTransitionAnimator.SetBool("FogIn", true);
        
        //Wait for animation to play
        yield return new WaitForSeconds(villageSceneTransitionToMainMenuTime);
        VillageSceneTransitionAnimator.SetBool("FogIn", false);
        
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public IEnumerator StartVillageSceneEntryAnimation(){
        // We do a simson like cloud moving to the side animation
        if (previousScene == "MainMenu"){
            VillageSceneTransitionAnimator.SetBool("FogOut", true);
            yield return null; // Wait for one frame to ensure the animation star
            VillageSceneTransitionAnimator.SetBool("FogOut", false);
        }
        // We do a simson like cloud moving to the side animation
        if (previousScene == "BB"){
            VillageSceneTransitionAnimator.SetBool("FadeInBlack", true);
            yield return null; // Wait for one frame to ensure the animation star
            VillageSceneTransitionAnimator.SetBool("FadeInBlack", false);
        }
        previousScene = "Village"; // For other potential scene transi logic
    }

    /// ----------------------------------------------------- ///
    ///           BreakerBox transition Management            /// 
    /// ----------------------------------------------------- ///


    public IEnumerator GoToMapAnimation(){
        audioManager.playButtonClickSFX();
        bbSceneTransitionAnimator.SetBool("DoorsOut",true);
       
        //Wait for animation to play
        yield return new WaitForSeconds(bbSceneTransitionTime);
        bbSceneTransitionAnimator.SetBool("DoorsOut",false);

        //Then Load the right scenario
        SceneManager.LoadSceneAsync("Map");
    }

    public IEnumerator GoToScenarioAnimation(int scenarioId){
        // Play the Transition
        audioManager.playButtonClickSFX();
        bbSceneTransitionAnimator.SetBool("DoorsOut",true);
        yield return null; // Wait for one frame to ensure the animation star
        bbSceneTransitionAnimator.SetBool("DoorsOut",false);

        //Wait for animation to play
        yield return new WaitForSeconds(bbSceneTransitionTime);

        //Then Load the right scenario
        string sceneName = "Scenario"+scenarioId;
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator StartBBSceneEntryAnimation(){
        // Play the Transition
        bbSceneTransitionAnimator.SetBool("DoorsIn",true);
        yield return null; // Wait for one frame to ensure the animation star
        bbSceneTransitionAnimator.SetBool("DoorsIn",false);

        previousScene = "BB"; // For other potential scene transi logic
    }
}
