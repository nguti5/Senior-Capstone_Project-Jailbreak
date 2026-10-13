using UnityEngine;
 
public class Interactible : MonoBehaviour, IInteractible
{
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting...");
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }
}