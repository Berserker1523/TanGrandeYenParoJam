using UnityEngine;

/// <summary>
/// Audio source for the entire game
/// </summary>
public class GameAudioSource : MonoBehaviour
{
    /// <summary>
    /// Initializes the audio source (singleton)
    /// </summary>
    private void Awake()
    {
        if (!AudioManager.Initialized)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            AudioManager.Initialize(audioSource);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //duplicate game object, do destroy
            Destroy(gameObject);
        }
        
    }
}
