using UnityEngine;

public class companionCube : MonoBehaviour, TeleportableObject, attachable
{
    bool attached = false;

    public void setAttached(bool attached)
    {
        this.attached = attached;
    }

    public bool isAttached()
    {
        return attached;
    }
}
