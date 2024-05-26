using EventSystem;
using UnityEngine;

namespace Generics
{
    public class HealthComponent : MonoBehaviour
    {
        [Header("Entity health variables")]
        [SerializeField, Tooltip("The max health for this entity")]
        private int maxHealth = 100;
        [SerializeField, Tooltip("The current health of the entity, for debugging purposes, largely")]
        private int currentHealth;
        [SerializeField, Tooltip("Threshold to determine if the player takes fall damage, " +
                                 "a value of 0 means the entity does not take fall damage.")]
        private float fallDamageVelocityThreshold;
        [SerializeField, Tooltip("Multiplier for fall damage (velocity.y * fallDamageMultiplier")]
        private float fallDamageMultiplier = 2.0f;

        [Header("Events")] 
        public GameEvent onPlayerHealthChanged;
        
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ApplyDamage(int amount)
        {
            currentHealth -= amount;
            onPlayerHealthChanged.Raise(this, currentHealth);
            Debug.Log($"Damage of amount: {amount} applied.");
        }
        
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