using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private Animator dayRewardsAnimator;
    [SerializeField] private GameObject dayRewardsTimeline;

    [Header("Sprites")]
    [SerializeField] private Sprite timelineDay1;
    [SerializeField] private Sprite timelineDay2;
    [SerializeField] private Sprite timelineDay3;
    [SerializeField] private Sprite timelineDay4;
     
    public int currentGameSpeed = 1;
    //Singletons
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void startRewardTransition() {
        mainCanvas.SetActive(false);
        cameraAnimator.SetTrigger("MoveUp");
    }

    public void AnimateDayReward(int dayNumber) 
    {
        dayRewardsAnimator.SetTrigger("FadeIn");

        currentGameSpeed = LevelManager.GetGameSpeed();
        LevelManager.SetGameSpeed(0);

        Image timelineImage = dayRewardsTimeline.GetComponent<Image>();
        switch(dayNumber) 
        {
            case 1:
                timelineImage.sprite = timelineDay1;
                break;
            case 2:
                timelineImage.sprite = timelineDay2;
                break;
            case 3:
                timelineImage.sprite = timelineDay3;
                break;
            case 4:
                timelineImage.sprite = timelineDay4;
                break;
            default:
                Debug.LogWarning("Invalid day number. No sprite assigned.");
                break;
        }
    }

    public void FinishDayReward() 
    {
        dayRewardsAnimator.SetTrigger("FadeOut");
        LevelManager.SetGameSpeed(currentGameSpeed);
    }
}