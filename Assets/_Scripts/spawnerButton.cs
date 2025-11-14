using UnityEngine;
using UnityEngine.UIElements;

public class spawnerButton : MonoBehaviour
{

    [SerializeField] GameObject companionCube;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 buttonPos = transform.position;
            buttonPos.y += 4;

            Instantiate(companionCube, buttonPos, Quaternion.identity); //rotacion default
        }
    }
}
