using UnityEngine;

public class DestroyObjects : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                turret turret = collision.gameObject.GetComponent<turret>();
                turret.die();
            }
            else{
                Destroy(collision.gameObject);
            }
        }
    }
}
