using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event System.Action OnPlayerDeath;

    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    public void die()
    {
        characterController.enabled = false;
        thirdPersonController.enabled = false;
        OnPlayerDeath?.Invoke();
    }
}
