using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

//Ara és FirstPersonController
public class ThirdPersonController : MonoBehaviour, TeleportableObject
{

    [Header("References")]
    [SerializeField] private Transform MPitchController;
    [SerializeField] private CharacterController _mCharacterController;

    [Header("Movement")]
    public float maxSpeedOnGround = 10;
    public float maxSpeedRunning = 20;
    private Vector2 mDirection = Vector2.zero;
    private bool mIsRunning = false;

    [Header("Jumping")]
    public float jumpSpeed = 10;
    private float mVerticalSpeed = 0;
    public bool IsGrounded = false;

    [Header("Rotation")]
    public int rotationSpeed = 10;
    public bool invertPitch = false;

    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    public float normalFOV = 60;
    public float sprintFOV = 75;

    [Range(-89, 89)] public float minPitch = -89;
    [Range(-89, 89)] public float maxPitch = 89;

    private float currentPitch = 0;
    private float currentYaw = 0;

    private Animator m_Animator;

    void Start()
    {
        currentPitch = MPitchController.localRotation.eulerAngles.x;
        currentYaw = MPitchController.localRotation.eulerAngles.y;
        m_Animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // ROTATION
        float pitch = Input.GetAxis("Mouse Y") * rotationSpeed * (invertPitch ? 1 : -1);
        currentPitch = Mathf.Clamp(currentPitch + pitch, minPitch, maxPitch);
        
        float yaw = Input.GetAxis("Mouse X") * rotationSpeed;
        currentYaw += yaw;

        MPitchController.localRotation = Quaternion.Euler(currentPitch, 0, 0);
        transform.rotation = Quaternion.Euler(0, currentYaw, 0);

        // RUNNING
        float speed = mIsRunning ? maxSpeedRunning : maxSpeedOnGround;

        // Speed FOV
        float targetFOV = mIsRunning ? sprintFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 10); //el 10 és la velocitat d'interpolació

        // MOVEMENT
        Vector3 movement = (transform.forward * mDirection.y + transform.right * mDirection.x) * speed * Time.deltaTime;
        _mCharacterController.Move(movement);
        m_Animator.SetFloat("MovementX", mDirection.x);
        m_Animator.SetFloat("MovementZ", mDirection.y);

        // JUMP
        if (IsGrounded)
        {
            if (mVerticalSpeed < 0)
                mVerticalSpeed = -2f;
        }
        else
        {
            mVerticalSpeed += Physics.gravity.y * 2 * Time.deltaTime;
        }

        Vector3 verticalMovement = Vector3.up * mVerticalSpeed * Time.deltaTime;
        CollisionFlags collisionFlags = _mCharacterController.Move(verticalMovement);

        //IsGrounded = _mCharacterController.isGrounded; //Falla més que el collisionFlags
        IsGrounded = (collisionFlags & CollisionFlags.Below) != 0; //El & binari és per comprovar flags en bitmask     
    }

    
    public void setRotation(Vector3 yaw, Vector3 pitch)
    {
        currentYaw = Mathf.Atan2(yaw.x, yaw.z) * Mathf.Rad2Deg;

        currentPitch = Mathf.Asin(pitch.y) * Mathf.Rad2Deg;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
    }

    public float getYaw() { return currentYaw;}
    public float getPitch() { return currentPitch;}

    public void OnMove(InputAction.CallbackContext context){
        mDirection = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context){
        mIsRunning = context.performed;
    }

    public void OnJump(InputAction.CallbackContext context){
        if (!IsGrounded || !context.performed) return;
        m_Animator.SetTrigger("Jump");
        mVerticalSpeed = jumpSpeed;
        IsGrounded = false;
    }

}
