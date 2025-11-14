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

    
    private bool canTpPlayer = true;
    private bool canTpCube = true;

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
            if (mirrorPortal != null) //si el otro portal existe
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
        Debug.Log("Teleporting cube");

        canTpCube = false;

        Rigidbody rb = cube.GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = true;

        Vector3 l_Velocity = transform.InverseTransformDirection(rb.linearVelocity);


        Vector3 enterPosition = transform.InverseTransformPoint(cube.transform.position);
        Vector3 exitPosition = mirrorPortal.transform.TransformPoint(enterPosition);

        Vector3 enterDirection = transform.InverseTransformDirection(cube.transform.forward);
        Vector3 exitDirection = mirrorPortal.transform.TransformDirection(-enterDirection);

        cube.transform.position = exitPosition;
        cube.transform.forward = exitDirection;
        cube.transform.position += mirrorPortal.transform.forward * -0.2f;

        /*
         *  Solo permitiermos que se teletransporte cuando no lo estemos llevando, por lo que tendremos que comprobar esa condici�n
         *  
         *  mover e orientar el objeto igual que al player,
         *  tendremos que modificarle la velocidad y la escala del objeto para orientarlo en la direcci�n del portal y para que le afecte el factor de escala entre los dos portales
         * 
         * Vector3 l_Velocity=_Portal.m_VirtualPortal.transform.InverseTransformDirection(l_Rigidbody.velocity);
         * l_Rigidbody.velocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_Velocity);
         * transform.localScale *= (_Portal.m_MirrorPortal.transform.localScale.x/_Portal.transform.localScale.x);
         */

        yield return new WaitForSeconds(1f);
        canTpCube = true;
    }

    private void teleportPlayer(GameObject player)
    {
        ThirdPersonController fpc = player.GetComponent<ThirdPersonController>();
        CharacterController cc = player.GetComponent<CharacterController>();
        
        //Rigidbody rb = player.GetComponent<Rigidbody>();
        //rb.linearVelocity = Vector3.zero;
        //rb.isKinematic = true;

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
        player.transform.position += mirrorPortal.transform.forward * -0.2f; //offset to not teleport infinetely

        //override rotation from fpc
        Vector3 flatForward = exitDirection;
        flatForward.y = 0;
        fpc.setRotation(flatForward, exitDirection);

        player.transform.rotation = Quaternion.Euler(0, fpc.getYaw(), 0);
        MPitchController.localRotation = Quaternion.Euler(fpc.getPitch(), 0, 0);


        //rb.isKinematic = false;
        //rb.linearVelocity = Vector3.zero;
        fpc.enabled = true;
        cc.enabled = true; //enable character controller again
    }

    private IEnumerator waitTeleportPlayer(GameObject player)
    {
        //Rigidbody rb = player.GetComponent<Rigidbody>();

        canTpPlayer = false;
        mirrorPortal.canTpPlayer = false;

        //rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        //rb.isKinematic = true;

        teleportPlayer(player);

        yield return new WaitForSeconds(1f);
        canTpPlayer = true;
        mirrorPortal.canTpPlayer = true;
        //rb.isKinematic = false;
    }

    public void SetScale(float scale)
    {
        portalSize = scale;
        transform.localScale = Vector3.one * portalSize;
    }


}
