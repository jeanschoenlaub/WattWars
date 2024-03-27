using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QcmManager : MonoBehaviour
{
    public GameObject mysteryBoxPrefab;

    [Header("References")]
    [SerializeField] private Animator qcmAnimator;

    [SerializeField] private TextMeshProUGUI qcmQuestion;
    [SerializeField] private TextMeshProUGUI qcmAnswer1;
    [SerializeField] private TextMeshProUGUI qcmAnswer2;
    [SerializeField] private TextMeshProUGUI qcmAnswer3;
    [SerializeField] private TextMeshProUGUI qcmCountDown;
    [SerializeField] public TextMeshProUGUI prizeText;

    [SerializeField] public Button checkAnswers;
    [SerializeField] public List<Toggle> toggles; 

    [Header("Variables")]
    [SerializeField] private float minSpawnDelay ; // Minimum delay before a new box spawns
    [SerializeField] private float maxSpawnDelay ; // Maximum delay before a new box spawns

    // Questions
    private QuestionSO currentQuestion = null;

    // Timer variables
    private float timeRemaining = 30f; // Time in seconds for each question
    private bool timerIsRunning = false;


    // Spawn and position variables for the misteryBox
    private Vector2 spawnPoint = new Vector2(-11f, 0f); 
    private float minY = -4f; // Minimum Y offset from the spawn point
    private float maxY = 2f; // Maximum Y offset from the spawn point

    //AudioManager that we get using tag   
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    
    private void Start()
    {
        StartCoroutine(SpawnMysteryBoxAtRandomIntervals());
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                qcmCountDown.text =  Mathf.FloorToInt(timeRemaining).ToString();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;

                //TODO implement end of timer functionality
                Debug.Log("timer is out");
            }
        }
    }

     private IEnumerator SpawnMysteryBoxAtRandomIntervals()
    {
        while (true)
        {
            // If there's no Mystery Box present, start the spawn process
            if (FindObjectOfType<MysteryBox>() == null)
            {
                float elapsed = 0f; // Elapsed time since we started waiting
                float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay); // Total time we want to wait
                // Wait until enough time has elapsed
                while (elapsed < waitTime)
                {
                    yield return null; // Wait for the next frame
                    elapsed += Time.deltaTime * LevelManager.GetGameSpeed();
                }

                // Spawn the Mystery Box
                SpawnMysteryBox();
            }
            else
            {
                // If a Mystery Box is present, wait a bit before checking again
                yield return new WaitForSeconds(1f);
            }
        }

    }

    private void SpawnMysteryBox()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnPoint.x, spawnPoint.y + randomY, 0);
        Instantiate(mysteryBoxPrefab, spawnPosition, Quaternion.identity);
    }

    public void ShowQCM(){
        LevelManager.PauseGame();
        qcmAnimator.SetBool("ShowQCM", true);

        checkAnswers.interactable = true;

        // Get a random question and replace text values
        currentQuestion = LevelManager.main.currentScenario.scenarioQuestions.questions[0];
        qcmQuestion.text = currentQuestion.questionText;
        qcmAnswer1.text = currentQuestion.answers[0];
        qcmAnswer2.text = currentQuestion.answers[1];
        qcmAnswer3.text = currentQuestion.answers[2];

        // Start the timer and play music
        timerIsRunning = true;
        timeRemaining = 30; // Reset the timer for the new question
        qcmCountDown.text =  Mathf.FloorToInt(timeRemaining).ToString();
        audioManager.PlayTrack(audioManager.QuizTrack);
    }


    public void CheckAnswer(){

        int index = 0;
        bool correctAnswerChosen = false;
        timerIsRunning = false;

        foreach (Toggle toggle in toggles)
        {
            // Check if the toggle is on and compare index with the correct answer
            if (toggle.isOn)
            {
                if (index == currentQuestion.correctAnswerIndex)
                {
                    Debug.Log("Correct Answer Selected");
                    correctAnswerChosen = true;
                    // Change the toggle background to green for correct answer
                    toggle.GetComponentInChildren<Image>().color = Color.green;
                }
                else
                {
                    Debug.Log("Incorrect Answer Selected");
                    // Change the toggle background to red for incorrect answer
                    toggle.GetComponentInChildren<Image>().color = Color.red;
                }
                break;
            }
            index++;
        }

        // If the correct answer was not chosen, mark the correct one green (includes if no toggles selected)
        if (!correctAnswerChosen)
        {
            toggles[currentQuestion.correctAnswerIndex].GetComponentInChildren<Image>().color = Color.green;
            audioManager.PlaySFX(audioManager.badActionSFX);
        }else{
            StartCoroutine(CashPrizeCoroutine());
            audioManager.PlaySFX(audioManager.buildSFX);// Correct Sound 
        }
    }

    private IEnumerator CashPrizeCoroutine()
    {
        Debug.Log("started co");
        float timeElapsed = 0f;
        float animTime = 1.5f;
        int startAmount = 0; // Starting amount of the counter
        int endAmount = Mathf.FloorToInt(timeRemaining); // End amount based on timeRemaining
        qcmAnimator.SetBool("ShowPrize", true);
        audioManager.PlayLoopingSFX(audioManager.coinsPouringSFX);

        while (timeElapsed < animTime)
        {
            // Calculate the current display amount based on the elapsed time
            int currentAmount = Mathf.FloorToInt(Mathf.Lerp(startAmount, endAmount, timeElapsed / animTime));
            prizeText.text = $"+{currentAmount}";

            // Increment the elapsed time by the time since last frame
            timeElapsed += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Set the final amount in the UI
        prizeText.text = $"+{endAmount}";
        
        audioManager.StopLoopingSFX();

        // Wait a moment to let the player see the final amount
        yield return new WaitForSeconds(1.0f);
        // Increase the currency by the end amount
        LevelManager.main.IncreaseCurrency(endAmount);
        

        // Hide the prize notification
        qcmAnimator.SetBool("ShowPrize", false);
        FinishQCM();
    }


    public void FinishQCM(){
        qcmAnimator.SetBool("ShowQCM", false);
        LevelManager.ResumeGame();

        //We reset the QCM toggles to white BG and not checkes
        foreach (Toggle toggle in toggles)
        {
            toggle.GetComponentInChildren<Image>().color = Color.white;
            toggle.isOn = false;
        }

        audioManager.PlayTrack(audioManager.Track1);
    }
}