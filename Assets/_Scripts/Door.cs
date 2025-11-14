using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    private GameObject subDoor1;
    private GameObject subDoor2;
    private Vector3 originalPosition1;
    private Vector3 originalPosition2;

    private bool isOpening = false;
    private Coroutine currentCoroutine = null;
    private float doorMoveDistance = 2f;
    private float openCloseDuration = 3f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoor;

    void Start()
    {
        Transform t1 = transform.Find("SubDoor1");
        Transform t2 = transform.Find("SubDoor2");
        if (t1 == null || t2 == null)
        {
            Debug.LogError("SubDoor1 or SubDoor2 not found");
            return;
        }

        subDoor1 = t1.gameObject;
        subDoor2 = t2.gameObject;
        originalPosition1 = t1.localPosition;
        originalPosition2 = t2.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OpenDoors();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        CloseDoors();
    }

    public void OpenDoors()
    {
        if (isOpening) return;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(MoveDoors(true));
    }

    public void CloseDoors()
    {
        if (!isOpening) return;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(MoveDoors(false));
    }

    private IEnumerator MoveDoors(bool open)
    {
        isOpening = open;

        Vector3 start1 = subDoor1.transform.localPosition;
        Vector3 start2 = subDoor2.transform.localPosition;

        Vector3 target1 = open ? originalPosition1 + new Vector3(0f, -doorMoveDistance, 0f) : originalPosition1;
        Vector3 target2 = open ? originalPosition2 + new Vector3(0f, doorMoveDistance, 0f) : originalPosition2;

        float t = 0f;

        audioSource.pitch = 0.7f;
        audioSource.PlayOneShot(openDoor);

        while (t < openCloseDuration)
        {
            t += Time.deltaTime;
            float normalizedT = t / openCloseDuration;
            subDoor1.transform.localPosition = Vector3.Lerp(start1, target1, normalizedT);
            subDoor2.transform.localPosition = Vector3.Lerp(start2, target2, normalizedT);
            yield return null;
        }

        subDoor1.transform.localPosition = target1;
        subDoor2.transform.localPosition = target2;
        currentCoroutine = null;
    }
}
