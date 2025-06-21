using UnityEngine;

public class FoodStepSound : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("FootSteps Sources")]
    public AudioClip[] footStepsSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private AudioClip GetRandomFootStep()
    {
        return footStepsSound[UnityEngine.Random.Range(0, footStepsSound.Length)];
    }
    private void Step()
    {
        AudioClip clip = GetRandomFootStep();
        audioSource.PlayOneShot(clip);
    }
}
