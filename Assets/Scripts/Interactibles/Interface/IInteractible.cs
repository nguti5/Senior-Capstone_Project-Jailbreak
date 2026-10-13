using UnityEngine;
 
public interface IInteractible
{
    /// <param name="interactor">The GameObject that triggered the interaction (usually the player).</param>
    void Interact(GameObject interactor);
}