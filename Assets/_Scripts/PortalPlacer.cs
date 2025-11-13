using UnityEngine;
using System.Collections.Generic;

public class PortalPlacer : MonoBehaviour
{
    //public Crosshair mCrosshair;
    //list of 4 mcrosshair scripts
    //public Crosshair[] mCrosshairs = new Crosshair[4];

    public LayerMask mHitMask; //la layer PortalWall
    public GameObject mPortalsList;

    public float mTimeToRecoil = 0.5f;

    public Camera mCamera;
    private Portal portalOrange;
    private Portal portalBlue;

    [System.Flags]
    private enum PortalType
    {
        None = 0,
        Blue = 1,
        Orange = 2
    }

    private PortalType mOpenPortal = PortalType.None;

    private float pressTime = 0f;
    private bool holdStarted = false;
    private bool shootBlueNextFixedUpdate = false;
    private bool shootOrangeNextFixedUpdate = false;

    private GameObject previewPortalOrange;
    private GameObject previewPortalBlue;

    private bool isPreviewing = false;
    private PortalType currentPreviewType = PortalType.None;
    private GameObject currentPreviewPortal;

    void Awake()
    {
        portalOrange = mPortalsList.transform.Find("PortalOrange").GetComponent<Portal>();
        portalBlue = mPortalsList.transform.Find("PortalBlue").GetComponent<Portal>();

        previewPortalOrange = mPortalsList.transform.Find("PreviewPortalOrange").gameObject;
        previewPortalBlue = mPortalsList.transform.Find("PreviewPortalBlue").gameObject;


        previewPortalBlue.transform.position = Vector3.zero;
        previewPortalBlue.SetActive(false);
        previewPortalOrange.transform.position = Vector3.zero;
        previewPortalOrange.SetActive(false);

    }

/*  private void TryShootPortal(PortalType portal)
    {
        Ray ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 250.0f))
        {
            // Comprova si la capa col·lisionada és una capa vàlida per a portals
            bool isValidSurface = (mHitMask.value & (1 << hit.collider.gameObject.layer)) != 0;

            if (!isValidSurface)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal"))
                {
                    //TODO si cal, de moment no
                }
                else
                {
                    //Debug.Log("Hit invalid surface before a PortalWall — cannot place portal.");
                    return;
                }
            }

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
        // Guardem per si no és vàlid tornar a col·locar
        Vector3 originalPos = portal.transform.position;
        Quaternion originalRot = portal.transform.rotation;

        // Movem temporalment el portal per fer la comprovació dels punts
        portal.transform.SetPositionAndRotation(pos, rot);

        bool valid = true;

        foreach (Transform child in portal.transform.Find("EmplacementPoints"))
        {
            // Raycast des de cada punt fins la paret
            Ray ray = new Ray(child.position - portal.transform.forward * 0.1f, portal.transform.forward);
            RaycastHit hit;

            //Debug.DrawRay(ray.origin, ray.direction * 0.5f, Color.green, 3f);

            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                //Debug.Log($"Hit {hit.collider.name} on layer {hit.collider.gameObject.layer} ({LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                if(hit.collider.CompareTag("Portal")) //evita col·locar portals un sobre l'altre
                {
                    string hitName = hit.collider.name.ToLower();
                    string myName = portal.name.ToLower();

                    if ((hitName.Contains("blue") && myName.Contains("blue")) ||
                        (hitName.Contains("orange") && myName.Contains("orange")))
                    {
                        //Debug.Log("Overwriting same-colored portal position." + hit.collider.name + " with " + portal.name);
                        continue; //se sobreescriu el portal si és el mateix color
                    }
                    else
                    {
                        // Si és un altre portal, no vàlid
                        //Debug.Log("Cannot place portal on the opposite portal!" + hit.collider.name + " with " + portal.name);
                        valid = false;
                        break;
                    }
    
                }

                // Comprova que sigui una superfície vàlida
                if ((mHitMask.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
                    //Debug.Log("Invalid surface at point: " + child.name);
                    valid = false;
                    break;
                }
            }
            else
            {
                //Debug.Log("No hit detected at point: " + child.name);
                valid = false;
                break;
            }
        }

        if (!valid)
        {
            portal.transform.SetPositionAndRotation(originalPos, originalRot);
            //Debug.Log("Portal placement invalid — reverting position");
            return false;
        }

        // Si és vàlid, tirem endavant (per evitar z-fighting)
        portal.transform.position += portal.transform.forward * -0.01f;

        //Debug.Log("Portal placed successfully at " + pos + " with rotation " + rot.eulerAngles);
        return true;
    }

}
*/
    void Update()
    {
        // ————— PORTAL BLAU —————
        if (Input.GetMouseButtonDown(0))
        {
            pressTime = 0f;
            holdStarted = false;
        }
        else if (Input.GetMouseButton(0))
        {
            pressTime += Time.deltaTime;

            if (!holdStarted && pressTime >= 0.5f)
            {
                StartPreviewPortal(PortalType.Blue);
                holdStarted = true;
            }

            if (holdStarted)
            {
                UpdatePreviewPortal();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!holdStarted && pressTime < 0.5f)
            {
                // “tap curt” → disparar portal directament
                Debug.Log("Short tap detected - shooting blue portal");
                //TryShootPortal(PortalType.Blue);
                shootBlueNextFixedUpdate = true;
            }

            if (holdStarted)
            {
                StopPreviewPortal();
                //TryShootPortal(PortalType.Blue);
                shootBlueNextFixedUpdate = true;
            }
        }

        // ————— PORTAL TARONJA —————
        if (Input.GetMouseButtonDown(1))
        {
            pressTime = 0f;
            holdStarted = false;
        }
        else if (Input.GetMouseButton(1))
        {
            pressTime += Time.deltaTime;
        
            if (!holdStarted && pressTime >= 0.5f)
            {
                StartPreviewPortal(PortalType.Orange);
                holdStarted = true;
            }
        
            if (holdStarted)
            {
                UpdatePreviewPortal();
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (!holdStarted && pressTime < 0.5f)
            {
                // “tap curt” → disparar portal directament
                //TryShootPortal(PortalType.Orange);
                shootOrangeNextFixedUpdate = true;
            }
        
            if (holdStarted)
            {
                StopPreviewPortal();
                //TryShootPortal(PortalType.Orange);
                shootOrangeNextFixedUpdate = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (shootBlueNextFixedUpdate)
        {
            TryShootPortal(PortalType.Blue);
            shootBlueNextFixedUpdate = false;
        }

        if (shootOrangeNextFixedUpdate)
        {
            TryShootPortal(PortalType.Orange);
            shootOrangeNextFixedUpdate = false;
        }
    }


    private void StartPreviewPortal(PortalType portal)
    {
        isPreviewing = true;
        currentPreviewType = portal;
        if (portal == PortalType.Blue)
            currentPreviewPortal = previewPortalBlue;
        else if (portal == PortalType.Orange)
            currentPreviewPortal = previewPortalOrange;

        // Activem el portal per veure'l durant la previsualització
        currentPreviewPortal.SetActive(true);
    }

    private void StopPreviewPortal()
    {
        if (currentPreviewPortal != null){
            currentPreviewPortal.SetActive(false);
            Debug.Log("1111111111111111 Preview stopped");
        }
            

        isPreviewing = false;
        currentPreviewType = PortalType.None;
        currentPreviewPortal = null;
    }

    private void UpdatePreviewPortal()
    {
        if (currentPreviewPortal == null)
            return;

        Ray ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250.0f))
        {
            bool isValidSurface = (mHitMask.value & (1 << hit.collider.gameObject.layer)) != 0;

            if (!isValidSurface)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal"))
                {
                    //TODO si cal, de moment no
                }
                else
                {
                    currentPreviewPortal.SetActive(false);
                    Debug.Log("22222222222222222 Preview stopped"); //aquest no surt, correcte perque la superficie és valida
                    return;
                }
            }

            var cameraRotation = mCamera.transform.rotation;
            var portalRight = cameraRotation * Vector3.right;

            if (Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            else
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);
            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

            // Si és vàlid segons els "EmplacementPoints"
            if (CheckValidPortalPosition(currentPreviewPortal, hit.collider, hit.point, portalRotation))
            {
                currentPreviewPortal.SetActive(true);
                //currentPreviewPortal.transform.SetPositionAndRotation(hit.point, portalRotation);
                currentPreviewPortal.transform.SetPositionAndRotation(hit.point + hit.normal * 0.01f, portalRotation);
            }
            else
            {
                currentPreviewPortal.SetActive(false);
                Debug.Log("333333333333333333333 Preview stopped"); //per algun motiu no és vàlid position. ara ja sí
            }
        }
        else
        {
            //currentPreviewPortal.SetActive(false);
            Debug.Log("44444444444444444 Preview stopped"); //normal que pari perque apunto al cel i no hi ha hit. NO, doncs ara apuntant a la paret han sortit molts quatres...
        }
    }

    private void TryShootPortal(PortalType portal)
    {
        Ray ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 250.0f))
        {
            bool isValidSurface = (mHitMask.value & (1 << hit.collider.gameObject.layer)) != 0;
            if (!isValidSurface)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal"))
                {
                    //TODO si cal, de moment permetem no fer res
                }
                else
                {
                    Debug.Log("INVALID SURFACE - cannot place portal.");
                    return;
                }
            }
                

            var cameraRotation = mCamera.transform.rotation;
            var portalRight = cameraRotation * Vector3.right;

            if (Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            else
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);
            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

            if (PlacePortal(GetPortal(portal), hit.collider, hit.point, portalRotation))
                mOpenPortal |= portal;
        }
        else
        {
            Debug.Log("No hit detected - cannot place portal.");
        }
    }

    private bool CheckValidPortalPosition(GameObject portal, Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        Vector3 originalPos = portal.transform.position;
        Quaternion originalRot = portal.transform.rotation;

        portal.transform.SetPositionAndRotation(pos, rot);
        bool valid = true;

        foreach (Transform child in portal.transform.Find("EmplacementPoints"))
        {
            Ray ray = new Ray(child.position - portal.transform.forward * 0.1f, portal.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1f))
            {
                //if (hit.collider.CompareTag("Portal"))
                //{
                //    string hitName = hit.collider.name.ToLower();
                //    string myName = portal.name.ToLower();
                //    if (!(hitName.Contains("blue") && myName.Contains("blue")) &&
                //        !(hitName.Contains("orange") && myName.Contains("orange")))
                //    {
                //        valid = false;
                //        break;
                //    }
                //}

                if(hit.collider.CompareTag("Portal")) //evita col·locar portals un sobre l'altre
                {
                    string hitName = hit.collider.name.ToLower();
                    string myName = portal.name.ToLower();

                    if ((hitName.Contains("blue") && myName.Contains("blue")) ||
                        (hitName.Contains("orange") && myName.Contains("orange")))
                    {
                        //Debug.Log("Overwriting same-colored portal position." + hit.collider.name + " with " + portal.name);
                        continue; //se sobreescriu el portal si és el mateix color
                    }
                    else
                    {
                        // Si és un altre portal, no vàlid
                        //Debug.Log("Cannot place portal on the opposite portal!" + hit.collider.name + " with " + portal.name);
                        valid = false;
                        Debug.Log("5555 false invalidPoint here");
                        break;
                    }
    
                }

                if ((mHitMask.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
                    valid = false;
                    Debug.Log("6666 false invalidPoint here");
                    break;
                }
            }
            else
            {
                valid = false;
                Debug.Log("7777 false invalidPoint here");
                break;
            }
        }

        portal.transform.SetPositionAndRotation(originalPos, originalRot);
        return valid;
    }

    private bool PlacePortal(GameObject portal, Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        if (!CheckValidPortalPosition(portal, wallCollider, pos, rot))
        {
            Debug.Log("PlacePortal returning false");
            return false;
        }
            
        Debug.Log("Originally placing portal at " + pos + " and moving to " + (pos + -portal.transform.forward * 0.01f));

        portal.transform.SetPositionAndRotation(pos + -portal.transform.forward * 0.01f, rot);
        portal.SetActive(true);
        return true;
    }

    private GameObject GetPortal(PortalType portal)
    {
        switch (portal)
        {
            case PortalType.Blue:
                return portalBlue.gameObject;
            case PortalType.Orange:
                return portalOrange.gameObject;
            default:
                return null;
        }
    }
}
