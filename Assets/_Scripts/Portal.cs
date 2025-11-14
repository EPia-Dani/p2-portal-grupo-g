using UnityEngine;
using System.Collections;


public class Portal : MonoBehaviour
{
    public Camera playerCamera;
    public Transform reflectionTransform;
    public Camera reflectionCamera;
    public Portal mirrorPortal;
    public float offsetNearPlane;

    private float portalSize = 1.0f;

    private bool canTp = false;
    private bool canTpPlayer = true;
    private bool canTpCube = true;


    void OnEnable()
    {
        PortalPlacer.OnCrosshairChange += HandleCrosshairChange;
    }

    void OnDisable()
    {
        PortalPlacer.OnCrosshairChange -= HandleCrosshairChange;
    }

    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 worldPosition = playerCamera.transform.position;
        Vector3 localPosition = reflectionTransform.InverseTransformPoint(worldPosition);
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(localPosition);

        Vector3 worldDirection = playerCamera.transform.forward;
        Vector3 localDirection = reflectionTransform.InverseTransformDirection(worldDirection);
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);

        float distance = Vector3.Distance(mirrorPortal.reflectionCamera.transform.position, mirrorPortal.transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = Mathf.Max(0.0f, distance) + offsetNearPlane;
    }


    private void OnTriggerEnter(Collider other)
    {
        TeleportableObject teleportable = other.GetComponent<TeleportableObject>();
        if (teleportable != null) //si es teleportable
        {
            if (canTp)
            {
                switch (other.tag)
                {
                    case "Player":
                        if (canTpPlayer)
                        {
                            StartCoroutine(waitTeleportPlayer(other.gameObject));
                        }
                        break;
                    case "Cube":
                        if (canTpCube)
                        {
                            StartCoroutine(teleportCube(other.gameObject));
                        }
                        break;
                }
            }
        }
    }


    private IEnumerator teleportCube(GameObject cube)
    {
        companionCube cc = cube.GetComponent<companionCube>();

        if (cc.isAttached())
        {
            Debug.Log("cannot tp cube");
            yield return null;
        }
        else
        {
            Debug.Log("Teleporting cube");

            canTpCube = false;
            mirrorPortal.canTpCube = false;

            Rigidbody rb = cube.GetComponent<Rigidbody>();

            Vector3 enterVelocity = rb.linearVelocity;
            rb.isKinematic = true;

            Vector3 enterPosition = transform.InverseTransformPoint(cube.transform.position);
            Vector3 exitPosition = mirrorPortal.transform.TransformPoint(enterPosition);

            Vector3 enterDirection = transform.InverseTransformDirection(cube.transform.forward);
            Vector3 exitDirection = mirrorPortal.transform.TransformDirection(-enterDirection);

            enterVelocity = transform.InverseTransformDirection(enterVelocity);
            Vector3 exitVelocity = mirrorPortal.transform.TransformDirection(-enterVelocity);

            cube.transform.position = exitPosition;
            cube.transform.forward = exitDirection;
            cube.transform.position += mirrorPortal.transform.forward * -0.2f;
            cube.transform.localScale *= (mirrorPortal.transform.localScale.x / transform.localScale.x);

            rb.isKinematic = false;
            rb.linearVelocity = exitVelocity;

            yield return new WaitForSeconds(0.5f);
            canTpCube = true;
            mirrorPortal.canTpCube = true;
        }
    }

    private void teleportPlayer(GameObject player)
    {
        ThirdPersonController fpc = player.GetComponent<ThirdPersonController>();
        CharacterController cc = player.GetComponent<CharacterController>();

        Transform MPitchController = player.transform.Find("PitchController");

        fpc.enabled = false;
        cc.enabled = false;

        //convert player position and direction into entering portal's local coords
        Vector3 enterPosition = transform.InverseTransformPoint(player.transform.position);
        Vector3 enterDirection = transform.InverseTransformDirection(player.transform.forward);

        //and convert to the other portal
        Vector3 exitPosition = mirrorPortal.transform.TransformPoint(enterPosition);
        Vector3 exitDirection = mirrorPortal.transform.TransformDirection(-enterDirection);

        
        player.transform.position = exitPosition;
        player.transform.forward = exitDirection;
        player.transform.position += mirrorPortal.transform.forward * -0.2f; //offset to not teleport infinetely

        //override rotation from fpc
        Vector3 flatForward = exitDirection;
        flatForward.y = 0;
        fpc.setRotation(flatForward, exitDirection);

        player.transform.rotation = Quaternion.Euler(0, fpc.getYaw(), 0);
        MPitchController.localRotation = Quaternion.Euler(fpc.getPitch(), 0, 0);

        fpc.enabled = true;
        cc.enabled = true;
    }

    private IEnumerator waitTeleportPlayer(GameObject player)
    {

        AttachObject attachObject = player.GetComponent<AttachObject>();
        if (attachObject.itHasObject())
        {
            yield return null;
        }
        else
        {
            canTpPlayer = false;
            mirrorPortal.canTpPlayer = false;

            teleportPlayer(player);

            yield return new WaitForSeconds(0.5f);
            canTpPlayer = true;
            mirrorPortal.canTpPlayer = true;
        }
    }

    public void SetScale(float scale)
    {
        portalSize = scale;
        transform.localScale = Vector3.one * portalSize;
    }


    private void HandleCrosshairChange(PortalPlacer.PortalType type)
    {
        switch (type)
        {
            case PortalPlacer.PortalType.None:
                canTp = false;
                break;
            case PortalPlacer.PortalType.Blue:
                canTp = false;
                break;
            case PortalPlacer.PortalType.Orange:
                canTp = false;
                break;
            case PortalPlacer.PortalType.Both:
                canTp = true;
                break;
        }
    }


}
