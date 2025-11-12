using UnityEngine;

public class OldAttachObject : MonoBehaviour //An old version of AttachObject.cs, before the AddForce change
{
    public Transform m_AttachingPosition;
    public Rigidbody m_ObjectAttached;
    public float m_AttachingObjectSpeed=5.0f;
    private bool m_AttachedObject=false;
    private Quaternion m_AttachingObjectStartRotation;

    private bool hasObject;

    void Update()
    {
        //TODO: Input system
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!hasObject)
            {
                if (GrabObject()){
                    hasObject=true;
                    m_AttachedObject=false;
                    m_ObjectAttached.isKinematic=true;
                    m_ObjectAttached.useGravity=false;
                }
            }
            else
            {
                //Llançar l'objecte endavant
                hasObject=false;
                m_ObjectAttached.isKinematic=false;
                m_ObjectAttached.useGravity=true;
                m_ObjectAttached.AddForce(m_AttachingPosition.forward*40.0f);
                m_ObjectAttached = null;
            }
        }
        if(Input.GetMouseButtonDown(1) && hasObject)
        {
            hasObject=false;
            m_ObjectAttached.isKinematic=false;
            m_ObjectAttached.useGravity=true;
            m_ObjectAttached = null;
        }
        if(hasObject)
            UpdateAttachedObject();
    }

    void UpdateAttachedObject()
    {
        //TODO: change this to add force instead of move position so it interacts with walls and floors well
        //add a small force if near the point, add bigger force if far away
        Vector3 l_EulerAngles=m_AttachingPosition.rotation.eulerAngles;
        if(!m_AttachedObject)
        {
            Vector3 l_Direction=m_AttachingPosition.transform.position-m_ObjectAttached.transform.position;
            float l_Distance=l_Direction.magnitude;
            float l_Movement=m_AttachingObjectSpeed*Time.deltaTime;

            if(l_Movement>=l_Distance)
            {
                m_AttachedObject=true;
                m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            }
            else
            {
                l_Direction/=l_Distance;
                m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position+l_Direction*l_Movement);
                m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 1.0f-Mathf.Min(l_Distance/1.5f, 1.0f)));
            }
        }
        else
        {
            m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            m_ObjectAttached.MovePosition(m_AttachingPosition.position);
        }
    }

    bool GrabObject()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3.0f))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null && (rb.gameObject.CompareTag("Cube") || rb.gameObject.CompareTag("Enemy")))
            {
                m_ObjectAttached = rb;
                m_AttachingObjectStartRotation=m_ObjectAttached.rotation;
                return true;
            }
        }
        return false;
    }
}

