using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    /// <summary>
    /// Custom UnityEvent that passes a Component and an object as parameters.
    /// Allows for greater flexibility when raising events such as being able to
    /// know what Component raised the event and any parameters it may need to pass.
    /// </summary>
    [System.Serializable]
    public class CustomGameEvent : UnityEvent<Component, object>
    {
    }

    /// <summary>
    /// Listens for a specified GameEvent and invokes a response when the event is raised.
    /// </summary>
    public class GameEventListener : MonoBehaviour
    {
        /// <summary>
        /// The GameEvent to listen for.
        /// </summary>
        [Tooltip("The GameEvent to listen for")]
        public GameEvent gameEvent;

        /// <summary>
        /// The response to invoke when the event is raised.
        /// </summary>
        [Tooltip("The response to invoke when the event is raised")]
        public CustomGameEvent response;

        /// <summary>
        /// Registers this listener with the GameEvent when the object is enabled.
        /// </summary>
        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        /// <summary>
        /// Unregisters this listener from the GameEvent when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        /// <summary>
        /// Invokes the response when the event is raised.
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="data">Additional data associated with the event.</param>
        public void OnEventRaised(Component sender, object data)
        {
            // Invoke the custom response, passing the sender and additional data
            response.Invoke(sender, data);
        }
    }
}