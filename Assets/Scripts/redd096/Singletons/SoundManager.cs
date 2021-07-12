namespace redd096
{
    using UnityEngine;
    using System.Collections;

    [System.Serializable]
    public struct AudioStruct
    {
        public AudioClip audioClip;
        [Range(0f, 1f)] public float volume;
    }

    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        #region variables

        [Header("Instantiate sound at point")]
        [SerializeField] AudioSource audioPrefab = default;

        //sound at point
        private Transform soundsParent;
        Transform SoundsParent
        {
            get
            {
                if (soundsParent == null)
                    soundsParent = new GameObject("Sounds Parent").transform;

                return soundsParent;
            }
        }

        Pooling<AudioSource> poolingSounds = new Pooling<AudioSource>();

        //background
        private AudioSource backgroundAudioSource;
        AudioSource BackgroundAudioSource
        {
            get
            {
                //create audio source if null
                if (backgroundAudioSource == null)
                    backgroundAudioSource = gameObject.AddComponent<AudioSource>();

                //return audio source
                return backgroundAudioSource;
            }
        }

        #endregion

        #region static Play

        /// <summary>
        /// Start audio clip. Can set volume and loop
        /// </summary>
        public static void Play(AudioSource audioSource, AudioClip clip, bool forceReplay, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return;

            //change only if different clip (so we can have same music in different scenes without stop)
            if (forceReplay || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
            }
        }

        #endregion

        #region background music

        /// <summary>
        /// Start audio clip for background. Can set volume and loop
        /// </summary>
        public void PlayBackgroundMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //start music from this audio source
            Play(BackgroundAudioSource, clip, false, volume, loop);
        }

        #endregion

        #region sound at point

        /// <summary>
        /// Start audio clip at point. Can set volume. Use specific pooling
        /// </summary>
        public void Play(Pooling<AudioSource> pool, AudioClip clip, Vector3 position, float volume = 1)
        {
            if (clip == null)
                return;

            //instantiate (if didn't find deactivated, take first one in the pool)
            AudioSource audioSource = pool.Instantiate(audioPrefab);
            if (audioSource == null && pool.PooledObjects.Count > 0)
                audioSource = pool.PooledObjects[0];

            //if still null, return
            if (audioSource == null)
                return;

            //set position, rotation and parent
            audioSource.transform.position = position;
            audioSource.transform.SetParent(SoundsParent);

            //play and start coroutine to deactivate
            Play(audioSource, clip, true, volume);
            StartCoroutine(DeactiveSoundAtPointCoroutine(audioSource));
        }

        /// <summary>
        /// Start audio clip at point. Can set volume
        /// </summary>
        public void Play(AudioClip clip, Vector3 position, float volume = 1)
        {
            //use this manager's pooling, instead of a specific one
            Play(poolingSounds, clip, position, volume);
        }

        IEnumerator DeactiveSoundAtPointCoroutine(AudioSource audioToDeactivate)
        {
            //wait to end the clip
            if (audioToDeactivate)
                yield return new WaitForSeconds(audioToDeactivate.clip.length);

            //and deactive
            if (audioToDeactivate)
                audioToDeactivate.gameObject.SetActive(false);
        }

        /// <summary>
        /// Start audio clip at point. Can set volume. Get clip random from the array
        /// </summary>
        public void Play(AudioClip[] clips, Vector3 position, float volume = 1)
        {
            //do only if there are elements in the array
            if (clips.Length > 0)
            {
                Play(clips[Random.Range(0, clips.Length)], position, volume);
            }
        }

        /// <summary>
        /// Start audio clip at point. Get clip and volume random from the array
        /// </summary>
        public void Play(AudioStruct[] audios, Vector3 position)
        {
            //do only if there are elements in the array
            if (audios.Length > 0)
            {
                AudioStruct audio = audios[Random.Range(0, audios.Length)];

                Play(audio.audioClip, position, audio.volume);
            }
        }

        #endregion
    }
}