using UnityEngine;

public class pressureButton : MonoBehaviour
{

    [SerializeField] Door door;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressButton;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            audioSource.PlayOneShot(pressButton);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            door.OpenDoors();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            audioSource.PlayOneShot(pressButton);
            door.CloseDoors();
        }
    }


}
