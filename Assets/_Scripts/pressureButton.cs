using UnityEngine;

public class pressureButton : MonoBehaviour
{

    [SerializeField] GameObject door;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = door.GetComponent<Animator>();
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
            animator.Play("PortalDoorAnimation");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            Debug.Log("No more pressing the button...");
            animator.Play("PortalDoorAnimation", 0, 1f);
        }
    }


}
