using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    /// <summary>
    /// Represents a game event that can be raised and listened to by multiple listeners.
    /// </summary>
    [CreateAssetMenu(menuName = "GameEvent")]
    public class GameEvent : ScriptableObject
    {
        /// <summary>
        /// List of listeners that are registered to this event.
        /// </summary>
        public List<GameEventListener> listeners = new();

        /// <summary>
        /// Raises the event, notifying all registered listeners.
        /// </summary>
        /// <param name="sender">The component that is raising the event.</param>
        /// <param name="data">Additional data to be passed to the listeners.</param>
        public void Raise(Component sender, object data)
        {
            // Notify each listener about the event being raised
            foreach (GameEventListener eventListener in listeners)
            {
                eventListener.OnEventRaised(sender, data);
            }
        }

        /// <summary>
        /// Registers a listener to this event.
        /// </summary>
        /// <param name="listener">The listener to be registered.</param>
        public void RegisterListener(GameEventListener listener)
        {
            // Add the listener to the list if it's not already present
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        /// <summary>
        /// Unregisters a listener from this event.
        /// </summary>
        /// <param name="listener">The listener to be unregistered.</param>
        public void UnregisterListener(GameEventListener listener)
        {
            // Remove the listener from the list if it is present
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}