using UnityEngine;

public class PlayerWin : MonoBehaviour
{
    public static event System.Action OnPlayerWin;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(winSound);
            PlayerWin.OnPlayerWin?.Invoke();
        }
    }
}
