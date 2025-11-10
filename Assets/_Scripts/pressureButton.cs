using UnityEngine;

public class pressureButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            Debug.Log("Cube pressing the button...");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            Debug.Log("No more pressing the button...");
        }
    }


}
