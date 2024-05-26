using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    [System.Serializable]
    public class CustomGameEvent : UnityEvent<Component, object> {}
    
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent gameEvent;

        public CustomGameEvent response;

        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised(Component sender, object data)
        {
            response.Invoke(sender, data);
        }
    }
}