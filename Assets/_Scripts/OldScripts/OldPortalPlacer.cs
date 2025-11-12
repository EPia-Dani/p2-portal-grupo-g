using UnityEngine;
using System.Collections.Generic;

public class OldPortalPlacer : MonoBehaviour //SCRIPT PER GUARDAR EL PUNT ON FUNCIONA ABANS DE FER EL PREVISUALITZADOR. Amb tot de comentaris que a l'altre estan esborats
{
    
    //public Crosshair mCrosshair;
    //list of 4 mcrosshair scripts
    //public Crosshair[] mCrosshairs = new Crosshair[4];

    public LayerMask mHitMask; //la layer PortalWall
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
        if (Physics.Raycast(ray, out hit, 250.0f))
        {
            // Comprova si la capa col·lisionada és una capa vàlida per a portals
            bool isValidSurface = (mHitMask.value & (1 << hit.collider.gameObject.layer)) != 0;

            if (!isValidSurface)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal"))
                {
                    //Debug.Log("Hit a Portal, de moment bé.");
                    /* descomentar això i arreglar-ho si volem fer-ho encara millor, però de moment funciona ja
                    //tirem un altre raycast des de la posició del portal que hem tocat
                    Ray portalRay = new Ray(hit.point + ray.direction * 0.01f, ray.direction);
                    Debug.DrawRay(portalRay.origin, portalRay.direction * 10.0f, Color.green, 10.0f);
                    RaycastHit portalHit;
                    if (Physics.Raycast(portalRay, out portalHit, 10.0f))
                    {
                        bool isValid = (mHitMask.value & (1 << portalHit.collider.gameObject.layer)) != 0;
                        if (isValid)
                        {
                            hit = portalHit; //actualitzem el hit per col·locar el portal a la nova posició
                        }
                        else
                        {
                            Debug.Log("Hit invalid surface after passing through a Portal — cannot place portal.");
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("No valid hit after passing through a Portal — cannot place portal.");
                        return;
                    }
                    */

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
