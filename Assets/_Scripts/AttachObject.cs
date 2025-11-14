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
                m_ObjectAttached.AddForce(m_AttachingPosition.forward*500.0f);
                releaseObject();
                m_ObjectAttached = null;
            }
        }
        if(Input.GetMouseButtonDown(1) && hasObject)
        {
            hasObject=false;
            m_ObjectAttached.isKinematic=false;
            m_ObjectAttached.useGravity=true;
            releaseObject();
            m_ObjectAttached = null;
        }
        if(hasObject)
            UpdateAttachedObject();
    }


    private void releaseObject()
    {
        if (m_ObjectAttached.gameObject.CompareTag("Cube"))
        {
            companionCube cc = m_ObjectAttached.gameObject.GetComponent<companionCube>();
            cc.setAttached(false);
        }
        else if (m_ObjectAttached.gameObject.CompareTag("Enemy"))
        {
            turret turret = m_ObjectAttached.gameObject.GetComponent<turret>();
            turret.setAttached(false);
        }

        Debug.Log("Object attached is false");
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
        if (Physics.Raycast(ray, out hit, 15.0f))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            attachable attachable = hit.collider.GetComponent<attachable>();

            if (rb != null && attachable != null)
            {
                m_ObjectAttached = rb;

                if (m_ObjectAttached.gameObject.CompareTag("Cube")){
                    companionCube cc = m_ObjectAttached.gameObject.GetComponent<companionCube>();
                    cc.setAttached(true);
                }
                else if (m_ObjectAttached.gameObject.CompareTag("Enemy")){
                    turret turret = m_ObjectAttached.gameObject.GetComponent<turret>();
                    turret.setAttached(true);
                }
                Debug.Log("Object attached is true");

                m_AttachingObjectStartRotation = m_ObjectAttached.rotation;
                return true;
            }
        }
        return false;
    }

    public bool itHasObject()
    {
        return hasObject;
    }

}

