using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera playerCamera;
    public Transform reflectionPlane;
    public Camera reflectionCamera;
    public Portal mirrorPortal;
    public float offsetNearPlane;

    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 worldPosition = playerCamera.transform.position;
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(localPosition);

        Vector3 worldDirection = playerCamera.transform.forward;
        Vector3 localDirection = transform.InverseTransformDirection(worldDirection);
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);

        float distance = Vector3.Distance(mirrorPortal.reflectionCamera.transform.position, mirrorPortal.transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = distance + offsetNearPlane;
    }
}
