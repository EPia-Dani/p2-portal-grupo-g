using UnityEngine;
using System.Collections;


public class Portal : MonoBehaviour
{
    public Camera playerCamera;
    public Transform reflectionTransform;
    public Camera reflectionCamera;
    public Portal mirrorPortal;
    public float offsetNearPlane;

    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
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
            if (mirrorPortal != null) //si el otro portal existe
            {
                switch (other.tag)
                {
                    case "Player":
                        Debug.Log("Teleporting player");
                        teleportPlayer(other.gameObject);
                        break;
                    case "Cube":
                        //todo cube tp -resize too-
                        break;
                }
            }
        }
    }


    private void teleportPlayer(GameObject player)
    {
        ThirdPersonController fpc = player.GetComponent<ThirdPersonController>();
        CharacterController cc = player.GetComponent<CharacterController>();

        Transform MPitchController = player.transform.Find("PitchController");

        fpc.enabled = false;
        cc.enabled = false; //disable update of player


        //convert player position and direction into entering portal's local coords
        Vector3 enterPosition = transform.InverseTransformPoint(player.transform.position);
        Vector3 enterDirection = transform.InverseTransformDirection(player.transform.forward);

        Vector3 exitPosition = mirrorPortal.transform.TransformPoint(enterPosition);
        Vector3 exitDirection = mirrorPortal.transform.TransformDirection(-enterDirection);
        

        //and convert to the other portal
        player.transform.position = exitPosition;
        player.transform.forward = exitDirection;
        player.transform.position += mirrorPortal.transform.forward * -0.1f; //offset to not teleport infinetely

        //override rotation from fpc
        Vector3 flatForward = exitDirection;
        flatForward.y = 0;
        fpc.setRotation(flatForward, exitDirection);
        
        player.transform.rotation = Quaternion.Euler(0, fpc.getYaw(), 0);
        MPitchController.localRotation = Quaternion.Euler(fpc.getPitch(), 0, 0);

        fpc.enabled = true;
        cc.enabled = true; //enable character controller again
    }


}
