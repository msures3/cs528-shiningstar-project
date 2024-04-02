using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public CreateStars createStars; // Reference to your CreateStars script

    private void Start()
    {
        // Find the CreateStars script in the scene
        createStars = FindObjectOfType<CreateStars>();
    }

    public void OnButtonClick()
    {
        // Call the method to change star colors in the CreateStars script
        if (createStars != null)
        {
            createStars.ChangeStarColors();
        }
        else
        {
            Debug.LogError("CreateStars script not found.");
        }
    }
}
