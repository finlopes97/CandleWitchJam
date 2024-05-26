using EventSystem;
using UnityEngine;

namespace Generics
{
    /// <summary>
    /// Manages the health of an entity, including damage application and fall damage calculation.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [Header("Entity health variables")] [SerializeField, Tooltip("The max health for this entity")]
        private int maxHealth = 100;

        [SerializeField, Tooltip("The current health of the entity, for debugging purposes, largely")]
        private int currentHealth;

        [SerializeField, Tooltip("Threshold to determine if the player takes fall damage, " +
                                 "a value of 0 means the entity does not take fall damage.")]
        private float fallDamageVelocityThreshold;

        [SerializeField, Tooltip("Multiplier for fall damage (velocity.y * fallDamageMultiplier")]
        private float fallDamageMultiplier = 2.0f;

        [Header("Events")] public GameEvent onPlayerHealthChanged; // Event raised when the player's health changes

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Applies damage to the entity and raises the health changed event.
        /// </summary>
        /// <param name="amount">The amount of damage to apply.</param>
        public void ApplyDamage(int amount)
        {
            currentHealth -= amount;
            onPlayerHealthChanged.Raise(this, currentHealth);
        }

        /// <summary>
        /// Applies fall damage to the entity based on the fall velocity.
        /// </summary>
        /// <param name="velocityY">The vertical velocity of the fall.</param>
        public void ApplyFallDamage(float velocityY)
        {
            if (velocityY >= fallDamageVelocityThreshold)
                ApplyDamage(CalculateFallDamage(velocityY));
        }

        /// <summary>
        /// Calculates the amount of damage based on the fall velocity.
        /// </summary>
        /// <param name="fallVelocity">The velocity at which the entity hit the ground.</param>
        /// <returns>The amount of damage to apply.</returns>
        private int CalculateFallDamage(float fallVelocity)
        {
            int fallDamage = Mathf.RoundToInt((fallVelocity - fallDamageVelocityThreshold) * fallDamageMultiplier);
            // Debug.Log($"Damage calculated: {fallDamage}.");
            return fallDamage;
        }
    }
}