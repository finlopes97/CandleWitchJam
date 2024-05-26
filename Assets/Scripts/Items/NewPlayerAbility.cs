using EventSystem;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Manages the unlocking of new player abilities when the player picks up an item.
    /// </summary>
    public class NewPlayerAbility : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("The ability to unlock, simple string for now, will update later")]
        private string abilityName;

        [Header("Events")] public GameEvent onPlayerPickupItem;

        /// <summary>
        /// Called when another collider enters the trigger collider attached to this GameObject.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onPlayerPickupItem.Raise(this, abilityName);
                Destroy(this.gameObject);
            }
        }
    }
}