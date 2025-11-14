using UnityEngine;

public class pressureButton : MonoBehaviour
{

    [SerializeField] Door door;

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
            door.CloseDoors();
        }
    }


}
