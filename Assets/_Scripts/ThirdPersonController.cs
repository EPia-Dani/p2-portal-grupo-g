using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Weapon")]
    [SerializeField] private Weapon weapon;
    private bool isShooting = false;

    //private bool isRespawning = false;

    [Range(-89, 89)] public float minPitch = -89;
    [Range(-89, 89)] public float maxPitch = 89;

    private float currentPitch = 0;
    private float currentYaw = 0;
    private float gravity = -20f;

    private Animator m_Animator;

    void OnEnable()
    {
        //GameManager.OnPlayerRespawn += HandlePlayerRespawn; //per desactivar el movement un moment
        //GameManager.OnGameRestart += HandlePlayerRespawn;
        //GameManager.OnPlayerDeath += HandlePlayerRespawn;
    }

    void Start()
    {
        currentPitch = MPitchController.localRotation.eulerAngles.x;
        currentYaw = MPitchController.localRotation.eulerAngles.y;
        m_Animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //if (isRespawning) return;


        //A l'update millor modificar només paràmetres, fer els càlculs a part.
        //Del profe: existeix Vector2 pos = Input.mousePositionDelta; per no haver de guardar la posició del frame anterior
        
        // ROTATION
        float pitch = Input.GetAxis("Mouse Y") * rotationSpeed * (invertPitch ? 1 : -1);
        //deltaMove.y * rotationSpeed * time.deltaTime per fer-ho frame independent
        currentPitch = Mathf.Clamp(currentPitch + pitch, minPitch, maxPitch);
        
        //float yaw = Input.GetAxis("Mouse X") * rotationSpeed*2 * Time.deltaTime;
        float yaw = Input.GetAxis("Mouse X") * rotationSpeed;
        currentYaw += yaw;

        MPitchController.localRotation = Quaternion.Euler(currentPitch, 0, 0);
        //localRotation seria només al component, rotation és respecte al món. En aquest cas funcionen els dos. Millor rotation.
        transform.rotation = Quaternion.Euler(0, currentYaw, 0); //Aquí és local perquè el yaw afecta al cos sencer del Player, no només a la càmera.

        // RUNNING
        float speed = mIsRunning ? maxSpeedRunning : maxSpeedOnGround;

        // Speed FOV
        //Quan estigui corrent, ampliar el camp de visió (FOV) de la càmera per donar sensació de velocitat
        float targetFOV = mIsRunning ? sprintFOV : normalFOV;
        //També es podria fer amb una interpolació lineal Lerp perquè sigui més suau el canvi -> A+(B-A)*t
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 10); //el 10 és la velocitat d'interpolació

        // MOVEMENT
        Vector3 movement = (transform.forward * mDirection.y + transform.right * mDirection.x) * speed * Time.deltaTime;
        _mCharacterController.Move(movement);
        m_Animator.SetFloat("MovementX", mDirection.x);
        m_Animator.SetFloat("MovementZ", mDirection.y);

        // JUMP
        //MRUA: y = y0 + v0*t + 0.5*a*t^2    (Physics.gravity millor que posar -9.81)
        mVerticalSpeed += gravity * Time.deltaTime; //la velocitat amb el temps decreix i amb el temps arriba a ser 0 (i podria ser negativa)
        Vector3 verticalMovement = Vector3.up * mVerticalSpeed * Time.deltaTime;
        CollisionFlags collisionFlags = _mCharacterController.Move(verticalMovement);

        //IsGrounded = _mCharacterController.isGrounded; //Falla més que el collisionFlags
        IsGrounded = (collisionFlags & CollisionFlags.Below) != 0; //El & binari és per comprovar flags en bitmask
        if (IsGrounded && mVerticalSpeed > 0.0f)
            mVerticalSpeed = 0f;
        // SHOOT
        if (isShooting && weapon != null)
        {
            //weapon.Shoot();
        }
        
    }

    /*
    public void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            item.Pick(this.gameObject);
        }
    }*/


    /*
    Input Value: Per moviment continu, rotació de càmera, controls analògics
    CallbackContext: Per accions discretes com saltar, disparar, interactuar, canviar armes

    No es poden fer servir els dos alhora perquè InputValue necessita que el PlayerInput sigui Send Messages i CallbackContext necessita que sigui InvokeEvents.
    */

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
        mIsRunning = context.performed; //started quan es prem, performed quan es manté premut i canceled quan es deixa de prémer
    }

    public void OnJump(InputAction.CallbackContext context){
        if (!IsGrounded || !context.performed) return;
        m_Animator.SetTrigger("Jump");
        mVerticalSpeed = jumpSpeed;
        IsGrounded = false;
    }

    public void OnShoot(InputAction.CallbackContext context){
        isShooting = context.performed && weapon != null;
    }

    public void OnReload(InputAction.CallbackContext context){
        //if (context.started && weapon != null)
        //    weapon.Reload();
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    private void HandlePlayerRespawn()
    {
        //isRespawning = true;
        StartCoroutine(ResetRespawnVariable());
    }

    private IEnumerator ResetRespawnVariable()
    {
        yield return new WaitForSeconds(1f);
        //isRespawning = false;
    }



    /*
    public Camera GameCamera;
    public float playerSpeed = 2.0f;
    private float JumpForce = 1.0f;
    
    private CharacterController m_Controller;
    private Animator m_Animator;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float gravityValue = -9.81f;

    private void Start()
    {
        m_Controller = gameObject.GetComponent<CharacterController>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        groundedPlayer = m_Controller.isGrounded;
        
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f;
        }

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //trasnform input into camera space
        var forward = GameCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        var right = Vector3.Cross(Vector3.up, forward);
        
        Vector3 move = forward * input.z + right * input.x;
        move.y = 0;
        
        m_Controller.Move(move * Time.deltaTime * playerSpeed);

        m_Animator.SetFloat("MovementX", input.x);
        m_Animator.SetFloat("MovementZ", input.z);

        if (input != Vector3.zero)
        {
            gameObject.transform.forward = forward;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(JumpForce * -3.0f * gravityValue);
            m_Animator.SetTrigger("Jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        m_Controller.Move(playerVelocity * Time.deltaTime);
    }
    */
}
