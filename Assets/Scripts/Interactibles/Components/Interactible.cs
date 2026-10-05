using UnityEngine;

public class Interactible : MonoBehaviour, IInteractible
{
    public void Interact()
    {
        Debug.Log("Interacting...");
        gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }

}

