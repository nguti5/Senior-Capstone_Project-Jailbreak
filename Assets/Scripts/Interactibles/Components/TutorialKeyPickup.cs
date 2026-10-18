
using UnityEngine;

public class TutorialKeyPickup : MonoBehaviour, IInteractible
{
    public void Interact(GameObject interactor)
    {
        Debug.Log("Tutorial key interaction detected!");
    }
}
