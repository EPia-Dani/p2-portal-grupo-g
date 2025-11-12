using UnityEngine;

public class AttachObject : MonoBehaviour
{
    public Transform m_AttachingPosition;
    public Rigidbody m_ObjectAttached;
    public float springStrength = 60f; //com més gran, més ràpid s'acosta a la posició
    public float damping = 30f; //com més gran, més suau és el moviment
    public float maxSpeed = 5f;
    public float m_StopDistance = 0.25f;
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
                    m_ObjectAttached.isKinematic=false;
                    m_ObjectAttached.useGravity=false;
                    m_ObjectAttached.linearVelocity = Vector3.zero;
                    m_ObjectAttached.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                //Llançar l'objecte endavant
                hasObject=false;
                m_ObjectAttached.isKinematic=false;
                m_ObjectAttached.useGravity=true;
                m_ObjectAttached.AddForce(m_AttachingPosition.forward*70.0f);
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
        if (m_ObjectAttached == null) return;

        Vector3 targetPos = m_AttachingPosition.position;
        Vector3 toTarget = targetPos - m_ObjectAttached.position;
        float distance = toTarget.magnitude;

        if (!m_AttachedObject)
        {
            if (distance < m_StopDistance)
            {
                m_AttachedObject = true;
                m_ObjectAttached.linearVelocity = Vector3.zero;
                m_ObjectAttached.angularVelocity = Vector3.zero;
                m_ObjectAttached.MovePosition(targetPos);
                m_ObjectAttached.MoveRotation(m_AttachingPosition.rotation);
                return;
            }
        }

        Vector3 force = (toTarget * springStrength) - (m_ObjectAttached.linearVelocity * damping);

        // Limita la velocitat màxima per evitar que surti disparat
        if (m_ObjectAttached.linearVelocity.magnitude > maxSpeed)
            m_ObjectAttached.linearVelocity = m_ObjectAttached.linearVelocity.normalized * maxSpeed;

        m_ObjectAttached.AddForce(force * Time.deltaTime, ForceMode.VelocityChange);

        Quaternion targetRot = m_AttachingPosition.rotation;
        m_ObjectAttached.MoveRotation( Quaternion.Lerp(m_ObjectAttached.rotation, targetRot, Time.deltaTime * 10f));
    }


    bool GrabObject()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f))
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

