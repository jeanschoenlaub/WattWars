using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public float speed = 1f; // Speed at which the box moves to the right
    public float endOfGameXValue = 11.25f; // Speed at which the box moves to the right

    void Update()
    {
        // Move the box to the right
        transform.Translate(speed * Time.deltaTime * LevelManager.GetGameSpeed(), 0, 0);

        // Destroy the box if it reaches the right side of the map
        if(transform.position.x > endOfGameXValue) // Adjust this condition based on your game's coordinate system
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        QcmManager manager = FindObjectOfType<QcmManager>();
        manager.ShowQCM();
        Destroy(gameObject);
    }
}