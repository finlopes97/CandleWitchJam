using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "GameEvent")]
    public class GameEvent : ScriptableObject
    {
        public List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise(Component sender, object data)
        {
            foreach (GameEventListener eventListener in listeners)
            {
                eventListener.OnEventRaised(sender, data);
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}