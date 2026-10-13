using UnityEngine;
 
/// <summary>
/// Place in a scene to mark where the player appears when arriving
/// through a door that targets this spawn ID.
/// The player object must have the "Player" tag.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId;
 
    private void Start()
    {
        if (string.IsNullOrEmpty(SceneTransition.PendingSpawnId)) return;
        if (SceneTransition.PendingSpawnId != spawnId) return;
 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("SpawnPoint: no object tagged 'Player' found.", this);
            return;
        }
 
        // CharacterController overrides transform changes unless disabled first
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
 
        player.transform.SetPositionAndRotation(transform.position, transform.rotation);
 
        if (controller != null) controller.enabled = true;
 
        SceneTransition.ClearPendingSpawn();
    }
 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
