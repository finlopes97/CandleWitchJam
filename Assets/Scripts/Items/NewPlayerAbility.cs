using System;
using EventSystem;
using UnityEngine;

namespace Items
{
    public class NewPlayerAbility : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("The ability to unlock, simple string for now, will update later")]
        private string abilityName;
        [Header("Events")] 
        public GameEvent onPlayerPickupItem;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Collided with {other.gameObject.name}!");
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("You got a new ability!");
                onPlayerPickupItem.Raise(this, abilityName);
                Destroy(this.gameObject);
            }
        }
    }
}
