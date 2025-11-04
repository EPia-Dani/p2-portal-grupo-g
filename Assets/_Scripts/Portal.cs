using UnityEngine;

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


    public void teleportPlayer(GameObject player)
    {

        if (mirrorPortal == null) //si no hay el otro portal spawneado
        {
            return;
        }

        Debug.Log("teleporting");

        ThirdPersonController fpc = player.GetComponent<ThirdPersonController>();
        CharacterController cc = player.GetComponent<CharacterController>();
        fpc.enabled = false;
        cc.enabled = false; //disable update of position

        //convert player position and direction into entering portal's local coords
        Vector3 enterPosition = transform.InverseTransformPoint(player.transform.position);
        Vector3 enterDirection = transform.InverseTransformDirection(player.transform.forward);

        Vector3 exitPosition = mirrorPortal.transform.TransformPoint(enterPosition);
        Vector3 exitDirection = mirrorPortal.transform.TransformDirection(-enterDirection);

        //and convert to the other portal
        player.transform.position = exitPosition;
        player.transform.forward = exitDirection;
        player.transform.position += mirrorPortal.transform.forward * -1.5f; //offset to not teleport infinetely

        fpc.enabled = true;
        cc.enabled = true; //enable character controller again
    }


}
