using UnityEngine;

public class PortalPlacer : MonoBehaviour
{
    //public Crosshair mCrosshair;
    //list of 4 mcrosshair scripts
    //public Crosshair[] mCrosshairs = new Crosshair[4];
    //public mhitmask
    public LayerMask mHitMask;
    //public portalslist (un gameobject amb array de portals)
    public GameObject mPortalsList;

    public float mTimeToRecoil = 0.5f;

    public Camera mCamera;
    private Portal portalRight;
    private Portal portalLeft;

    [System.Flags]
    private enum PortalType
    {
        None = 0,
        Blue = 1,
        Orange = 2
    }

    private PortalType mOpenPortal = PortalType.None;

    void Awake()
    {
        portalRight = mPortalsList.transform.Find("PortalOrange").GetComponent<Portal>();
        portalLeft = mPortalsList.transform.Find("PortalBlue").GetComponent<Portal>();
    }

    void Update()
    {
        //TODO input system
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click - shoot blue portal");
            TryShootPortal(PortalType.Blue);
        }

        if (Input.GetMouseButtonDown(1))
            TryShootPortal(PortalType.Orange);
    }


    private void TryShootPortal(PortalType portal)
    {
        Ray ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 250.0f, mHitMask))
        {
            var cameraRotation = mCamera.transform.rotation;
            var portalRight = cameraRotation * Vector3.right;

            if(Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);

            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);
            Debug.DrawLine(hit.point, hit.point + portalForward * 2.0f, Color.red, 10.0f);

            if(PlacePortal(GetPortal(portal), hit.collider, hit.point, portalRotation))
            {
                //play sound
                //mcrosshair.SetCrosshairState(MCrosshair.CrosshairState.PortalShoot);
                mOpenPortal |= portal; // això és un bitwise OR que afegeix el portal a mOpenPortal, per treure és un AND negatiu i es faria: mOpenPortal &= ~portal
            }

        }
    }

    private bool PlacePortal(GameObject portal, Collider wallCollider, Vector3 pos, Quaternion rot)
    {

        //Comprovar que el portal no es col·lisiona amb res. LATER
        //List<GameObject> validPoints = //child from portal named EmplacementPoints
        //    new List<GameObject>();
        //foreach(Transform child in portal.transform.Find("EmplacementPoints"))
        //{
        //    validPoints.Add(child.gameObject);
        //}

        //de moment permetrem col·locar el portal sempre
        portal.transform.position = pos;
        portal.transform.rotation = rot;
        //una mica més endavant per evitar z-fighting
        portal.transform.position += portal.transform.forward * -0.01f;



        Debug.Log("Portal placed at " + pos + " with rotation " + rot.eulerAngles);

    


        return true;
    

    }

    private GameObject GetPortal(PortalType portal)
    {
        switch (portal)
        {
            case PortalType.Blue:
                return portalLeft.gameObject;
            case PortalType.Orange:
                return portalRight.gameObject;
            default:
                return null;
        }
    }
}
