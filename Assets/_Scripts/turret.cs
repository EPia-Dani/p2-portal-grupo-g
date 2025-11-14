using NUnit.Framework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class turret : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float maxDistance;
    [SerializeField] private float lineRendererWidth;

    private bool isActive = true;
    private Vector3 turretPos;
    private Vector3 turretForward;

    private ParticleSystem diePS;

    void Start()
    {
        setLineRenderer(lineRenderer);
        diePS = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            turretPos = transform.position + transform.forward * 0.2f;
            turretForward = transform.forward;

            lineRenderer.startWidth = lineRendererWidth;
            lineRenderer.endWidth = lineRendererWidth;

            if (Physics.Raycast(turretPos, turretForward, out RaycastHit hit, maxDistance))
            {
                lineRenderer.SetPosition(0, turretPos);
                lineRenderer.SetPosition(1, hit.point);

                switch (hit.collider.tag)
                {
                    case "Player":
                        //Debug.Log("Hit player");
                        hitPlayer(hit.collider.gameObject);
                        break;
                    case "Enemy":
                        GameObject go = hit.collider.gameObject;
                        turret turret = go.GetComponent<turret>();
                        turret.die();
                        break;
                }
            }
            else
            {
                lineRenderer.SetPosition(0, turretPos);
                lineRenderer.SetPosition(1, turretPos + turretForward * maxDistance);
            }
        }
        else
        {
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Cube") || other.gameObject.tag == "Enemy")
        {
            Debug.Log("turret deactivated");
            StartCoroutine(setInactive());
        }
    }

    private IEnumerator setInactive()
    {
        isActive = false;
        yield return new WaitForSeconds(2f);
        isActive = true;
    }

    private void setLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    private void hitPlayer(GameObject go)
    {
        PlayerHealth ph = go.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            ph.die();
        }
    }

    private IEnumerator playParticleSystem ()
    {
        diePS.Play();
        yield return null;
    }

    private void die()
    {
        if (isActive == false)
        {
            return;
        }

        diePS.Play();

        isActive = false;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        //StartCoroutine(playParticleSystem());

        //Destroy(gameObject, 2f);
        Debug.Log("turret is dead");
    }

}
