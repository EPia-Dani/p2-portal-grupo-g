using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event System.Action OnPlayerDeath;

    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;
    //private Rigidbody rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        //rigidbody = GetComponent<Rigidbody>();
    }

    public void die()
    {
        //rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        //
        //rigidbody.isKinematic = true;
        characterController.enabled = false;
        thirdPersonController.enabled = false;

        Debug.Log("You died");
        OnPlayerDeath?.Invoke();
    }
}
