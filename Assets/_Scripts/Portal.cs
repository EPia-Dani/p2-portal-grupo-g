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


    public void teleportPlayer()
    {
        Debug.Log("teleporting");

        /*
         * Vector3 l_Position = _Portal.m_VirtualPortal.transform.InverseTransformPoint(transform.position);
         * Vector3 l_Direction = _Portal.m_VirtualPortal.transform.InverseTransformDirection(-transform.forward);
         * 
         * transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_Position);
         * transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_Direction);
         * 
         * avanzar al player en dirección a su forward un offset determinado para que “atraviese” el portal y salga del trigger del otro portal
         * actualizar el mYaw del player para mantener el forward asignado
         */

    }


}
