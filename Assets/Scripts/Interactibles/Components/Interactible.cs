using UnityEngine;
 
public class Interactible : MonoBehaviour, IInteractible
{
    public void Interact(GameObject interactor)
    {   // test comment
        Debug.Log("Interacting...");
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }
}
