using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event System.Action OnPlayerDeath;

    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    public void die()
    {
        audioSource.PlayOneShot(deathSound);
        characterController.enabled = false;
        thirdPersonController.enabled = false;
        OnPlayerDeath?.Invoke();
    }
}
