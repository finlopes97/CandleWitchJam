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
        [Header("Volume")]
        [Range(0,1)]
        public float masterVolume;
        [Range(0, 1)]
        public float musicVolume;
        [Range(0, 1)]
        public float sfxVolume;

        private Bus masterBus;
        private Bus musicBus;
        private Bus sfxBus;


        private List<EventInstance> _eventInstances;
        private List<StudioEventEmitter> _eventEmitters;

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
            _eventEmitters = new List<StudioEventEmitter>();

            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }

        private void Start()
        {
            InitializeMusic(FMODEvents.instance.music);
        }

        private void Update()
        {
            //update the volumes with current values
            masterBus.setVolume(masterVolume);
            musicBus.setVolume(musicVolume);
            sfxBus.setVolume(sfxVolume);
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
        /// Initializes an emitter with an FMOD event.
        /// </summary>
        /// <param name="eventReference">Event to be played by the emitter.</param>
        /// <param name="emitterGamObject">Object holding the emitter.</param>
        /// <returns>the initialized emitter.</returns>
        public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGamObject) {
            StudioEventEmitter emitter = emitterGamObject.GetComponent<StudioEventEmitter>();
            emitter.EventReference = eventReference;
            _eventEmitters.Add(emitter);
            return emitter;
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
            //stop all of the event emitters so they don't continue playing in other scenes
            foreach(StudioEventEmitter emitter in _eventEmitters) {
                emitter.Stop();
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}