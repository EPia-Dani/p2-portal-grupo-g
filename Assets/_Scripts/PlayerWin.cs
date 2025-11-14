using UnityEngine;

public class PlayerWin : MonoBehaviour
{
    public static event System.Action OnPlayerWin;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWin.OnPlayerWin?.Invoke();
        }
    }
}
