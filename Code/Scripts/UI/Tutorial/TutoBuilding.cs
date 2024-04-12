using UnityEngine;
using TMPro;

// Tutorial to show the player how to place a fuel and a diesel generator tower 
public class TutoBuilding : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public Animator MouseAnimator; // To make mouse move up and down to direct user
    
    [Header("Text References")]
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] TextMeshProUGUI TutoText; 

    public bool isTutorialActive = false;

    private TutorialManager tutoManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoManager = GetComponent<TutorialManager>();
    }

    private void Start(){
        TutoTextBox.SetActive(false);
    }

    // void Update()
    // {

    // }

}
