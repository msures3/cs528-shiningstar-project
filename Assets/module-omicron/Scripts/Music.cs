using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // Ensure AudioSource component is assigned
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is not assigned.");
            return;
        }

        // Play the audio clip
        audioSource.Play();
    }
}
