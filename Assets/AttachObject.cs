using UnityEngine;

public class AttachObject : MonoBehaviour
{
    public Transform m_AttachingPosition;
    public Rigidbody m_ObjectAttached;
    public float m_AttachingObjectSpeed=5.0f;
    private bool m_AttachedObject=false;
    private Quaternion m_AttachingObjectStartRotation;

    private bool hasObject;

    void Start()
    {
        m_AttachingObjectStartRotation=m_ObjectAttached.rotation;
    }

    void Update()
    {
        //TODO: Input system
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!hasObject)
            {
                //TODO: check distance and raycast to cube
                hasObject=true;
                m_AttachedObject=false;
                m_ObjectAttached.isKinematic=true;
                m_ObjectAttached.useGravity=false;
            }
            else
            {
                //Llançar l'objecte endavant
                hasObject=false;
                m_ObjectAttached.isKinematic=false;
                m_ObjectAttached.useGravity=true;
                m_ObjectAttached.AddForce(m_AttachingPosition.forward*40.0f);
            }
        }
        if(Input.GetMouseButtonDown(1) && hasObject)
        {
            hasObject=false;
            m_ObjectAttached.isKinematic=false;
            m_ObjectAttached.useGravity=true;
        }
        if(hasObject)
            UpdateAttachedObject();
    }

    void UpdateAttachedObject()
    {
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
}

