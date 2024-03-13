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
            Debug.Log("killed");
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Mystery Box Clicked!");
        // Implement what happens when the box is clicked
        // For now, it just logs a message
    }
}