using UnityEngine;

public class pressureButton : MonoBehaviour
{

    [SerializeField] Door door;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            Debug.Log("Cube pressing the button...");
            door.OpenDoors();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            Debug.Log("No more pressing the button...");
            door.CloseDoors();
        }
    }


}
