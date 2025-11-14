using UnityEngine;
using UnityEngine.UIElements;

public class spawnerButton : MonoBehaviour
{

    [SerializeField] GameObject companionCube;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressButton;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 buttonPos = transform.position;
            buttonPos.y += 4;

            audioSource.PlayOneShot(pressButton);
            Instantiate(companionCube, buttonPos, Quaternion.identity); //rotacion default
        }
    }
}
