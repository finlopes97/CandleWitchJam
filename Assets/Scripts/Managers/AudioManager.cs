using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Manages audio playback using FMOD, including music and sound effects.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private List<EventInstance> _eventInstances;

        public static AudioManager Instance { get; private set; }

        private EventInstance _musicEventInstance;

        private void Awake()
        {
            // Singleton
            if (Instance != null)
            {
                Debug.LogError("More than one AudioManager Instance");
            }

            Instance = this;

            _eventInstances = new List<EventInstance>();
        }

        private void Start()
        {
            InitializeMusic(FMODEvents.instance.music);
        }

        /// <summary>
        /// Initializes and starts playing the background music.
        /// </summary>
        /// <param name="musicEventReference">The FMOD event reference for the music.</param>
        private void InitializeMusic(EventReference musicEventReference)
        {
            _musicEventInstance = CreateEventInstance(musicEventReference);
            _musicEventInstance.start();
        }

        /// <summary>
        /// Plays a one-shot sound effect at the specified world position.
        /// </summary>
        /// <param name="sound">The FMOD event reference for the sound effect.</param>
        /// <param name="worldPos">The world position where the sound should be played.</param>
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }

        /// <summary>
        /// Creates and registers an FMOD event Instance.
        /// </summary>
        /// <param name="eventReference">The FMOD event reference.</param>
        /// <returns>The created FMOD event Instance.</returns>
        public EventInstance CreateEventInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }

        /// <summary>
        /// Cleans up all created FMOD event instances by stopping and releasing them.
        /// </summary>
        private void CleanUp()
        {
            //stop and release any created instances
            foreach (EventInstance eventInstance in _eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}