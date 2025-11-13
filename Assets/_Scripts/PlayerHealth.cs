using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;
    private Rigidbody rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void die()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        
        rigidbody.isKinematic = true;
        characterController.enabled = false;
        thirdPersonController.enabled = false;

        Debug.Log("You died");
    }
}
