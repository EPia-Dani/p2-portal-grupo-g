using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void die()
    {
        characterController.enabled = false;
        thirdPersonController.enabled = false;

        Debug.Log("You died");
    }
}
