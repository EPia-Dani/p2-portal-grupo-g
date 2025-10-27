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
}
