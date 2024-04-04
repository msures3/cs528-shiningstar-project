// using UnityEngine;
// using UnityEngine.UI;

// public class StarDistanceController : MonoBehaviour
// {
//     public CreateStars starManager; // Reference to the CreateStars script
//     public Slider distanceSlider; // Reference to the slider for controlling distance

//     private float minDistance = 1f; // Minimum distance between stars
//     private float maxDistance = 100f; // Maximum distance between stars

//     private void Start()
//     {
//         // Initialize the slider value to the middle
//         distanceSlider.value = 0.5f;
//     }

//     public void OnDistanceSliderChanged(float value)
//     {
//         // Calculate the distance based on slider value
//         float distance = Mathf.Lerp(minDistance, maxDistance, value);

//         // Update the distance of stars in the starManager
//         starManager.UpdateStarDistances(distance);
//     }
// }
