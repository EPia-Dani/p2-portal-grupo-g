using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PortalPlacer : MonoBehaviour
{
    public static event System.Action<PortalType> OnCrosshairChange;

    public LayerMask mHitMask; //la layer PortalWall i Portal
    public GameObject mPortalsList;

    public float mTimeToRecoil = 0.5f;

    public Camera mCamera;
    private Portal portalOrange;
    private Portal portalBlue;

    [System.Flags]
    public enum PortalType
    {
        None   = 0,
        Blue   = 1,
        Orange = 2,
        Both   = Blue | Orange   // = 3
    }

    private PortalType mOpenPortal = PortalType.None; //00 None, 01 Blue, 10 Orange, 11 Both

    private float pressTime = 0f;
    private bool holdStarted = false;
    private bool shootBlueNextFixedUpdate = false;
    private bool shootOrangeNextFixedUpdate = false;

    private GameObject previewPortalOrange;
    private GameObject previewPortalBlue;
    public GameObject invalidPortalImage;

    private bool isPreviewingBlue = false;
    private bool isPreviewingOrange = false;
    private PortalType currentPreviewType = PortalType.None;
    private GameObject currentPreviewPortal;


    private float portalScale = 1f;
    private const float minPortalScale = 0.5f;
    private const float maxPortalScale = 2f;
    private const float scaleStep = 0.1f;

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

        invalidPortalImage.SetActive(false);

    }

    void Update()
    {
        //PORTAL BLAU
        if (Input.GetMouseButtonDown(0) && !isPreviewingOrange)
        {
            pressTime = 0f;
            holdStarted = false;
        }
        else if (Input.GetMouseButton(0) && !isPreviewingOrange)
        {
            pressTime += Time.deltaTime;

            if (!holdStarted && pressTime >= 0.5f)
            {
                StartPreviewPortal(PortalType.Blue);
                holdStarted = true;
                isPreviewingBlue = true;
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
                shootBlueNextFixedUpdate = true;
            }

            if (holdStarted)
            {
                StopPreviewPortal();
                shootBlueNextFixedUpdate = true;
            }
            isPreviewingBlue = false;
        }

        //PORTAL TARONJA
        if (Input.GetMouseButtonDown(1) && !isPreviewingBlue)
        {
            pressTime = 0f;
            holdStarted = false;
        }
        else if (Input.GetMouseButton(1) && !isPreviewingBlue)
        {
            pressTime += Time.deltaTime;
        
            if (!holdStarted && pressTime >= 0.5f)
            {
                StartPreviewPortal(PortalType.Orange);
                holdStarted = true;
                isPreviewingOrange = true;
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
                shootOrangeNextFixedUpdate = true;
            }
        
            if (holdStarted)
            {
                StopPreviewPortal();
                shootOrangeNextFixedUpdate = true;
            }
            isPreviewingOrange = false;
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
        currentPreviewType = portal;
        if (portal == PortalType.Blue)
            currentPreviewPortal = previewPortalBlue;
        else if (portal == PortalType.Orange)
            currentPreviewPortal = previewPortalOrange;

        // Activem el portal per veure'l durant la previsualització
        currentPreviewPortal.SetActive(true);
        invalidPortalImage.SetActive(false);
    }

    private void StopPreviewPortal()
    {
        if (currentPreviewPortal != null){
            currentPreviewPortal.transform.localScale = Vector3.one;
            currentPreviewPortal.SetActive(false); 
        }
            
        invalidPortalImage.SetActive(false);
        currentPreviewType = PortalType.None;
        currentPreviewPortal = null;
    }

    private void UpdatePreviewPortal()
    {
        if (currentPreviewPortal == null)
            return;

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            portalScale += scroll * scaleStep;
            portalScale = Mathf.Clamp(portalScale, minPortalScale, maxPortalScale);

            currentPreviewPortal.transform.localScale = Vector3.one * portalScale;
        }

        Ray ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250.0f))
        {
            bool isValidSurface = (mHitMask.value & (1 << hit.collider.gameObject.layer)) != 0; //1 << hit.collider.gameObject.layer fa un shift a l'esquerra. 000100 si layer és 2. hitmaskvalue és 000101 si té layers 0 i 2. el & fa un AND bit a bit. 0 significaria que no ha tocat cap layer vàlida

            if (!isValidSurface)
            {
                currentPreviewPortal.SetActive(false);
                invalidPortalImage.SetActive(true);
                return;
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
                invalidPortalImage.SetActive(false);
                currentPreviewPortal.transform.SetPositionAndRotation(hit.point + hit.normal * 0.01f, portalRotation);
            }
            else
            {
                currentPreviewPortal.SetActive(false);
                invalidPortalImage.SetActive(true);
            }
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
                    //TODO. revisar si ja es mira a dalt
                }
                else
                {
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
            {
                mOpenPortal |= portal; //mOpenPortal |= PortalType.Blue; // 00 | 01 = 01. si afegim el taronja, 01 | 10 = 11
                
                OnCrosshairChange?.Invoke(mOpenPortal);
                Debug.Log("Invoking OnCrosshairChange with: " + mOpenPortal);
                Portal portalScript = GetPortal(portal).GetComponent<Portal>();
                portalScript.SetScale(portalScale);
                portalScale = 1f;
            }
            else if (currentPreviewPortal != null)
            {
                currentPreviewPortal.SetActive(false);
                invalidPortalImage.SetActive(true);
            }
        }
        else
        {
            invalidPortalImage.SetActive(true);
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
            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if(hit.collider.CompareTag("Portal")) //evita col·locar portals un sobre l'altre
                {
                    string hitName = hit.collider.name.ToLower();
                    string myName = portal.name.ToLower();

                    if ((hitName.Contains("blue") && myName.Contains("blue")) ||
                        (hitName.Contains("orange") && myName.Contains("orange")))
                    {
                        continue; //se sobreescriu el portal si és el mateix color
                    }
                    else
                    {
                        // Si és un altre portal, no vàlid
                        valid = false;
                        break;
                    }
    
                }

                if ((mHitMask.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
                    valid = false;
                    break;
                }
            }
            else
            {
                valid = false;
                break;
            }
        }

        //if portal is a preview portal, skip this
        if (!portal.name.Contains("Preview"))
        {
            portal.transform.SetPositionAndRotation(originalPos, originalRot);
        }
        return valid;
    }

    private bool PlacePortal(GameObject portal, Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        if (!CheckValidPortalPosition(portal, wallCollider, pos, rot))
        {
            return false;
        }

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
