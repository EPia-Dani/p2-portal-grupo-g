using UnityEngine;
using UnityEngine.UIElements;

public class spawnerButton : MonoBehaviour
{

    [SerializeField] GameObject companionCube;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 buttonPos = transform.position;
            buttonPos.y += 4;

            Instantiate(companionCube, buttonPos, Quaternion.identity); //rotacion default

            //Debug.Log("spawning cube");
        }
    }
}
