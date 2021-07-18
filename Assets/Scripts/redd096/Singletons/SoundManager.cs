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
        public static AudioSource Play(AudioSource audioSource, AudioClip clip, bool forceReplay, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return null;

            //change only if different clip (so we can have same music in different scenes without stop)
            if (forceReplay || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
                return audioSource;
            }

            return null;
        }

        #endregion

        #region background music

        /// <summary>
        /// Start audio clip for background. Can set volume and loop
        /// </summary>
        public AudioSource PlayBackgroundMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //start music from this audio source
            return Play(BackgroundAudioSource, clip, false, volume, loop);
        }

        #endregion

        #region sound at point

        /// <summary>
        /// Start audio clip at point. Can set volume. Use specific pooling
        /// </summary>
        public AudioSource Play(Pooling<AudioSource> pool, AudioClip clip, Vector3 position, float volume = 1)
        {
            if (clip == null)
                return null;

            //instantiate (if didn't find deactivated, take first one in the pool)
            AudioSource audioSource = pool.Instantiate(audioPrefab);
            if (audioSource == null && pool.PooledObjects.Count > 0)
                audioSource = pool.PooledObjects[0];

            //if still null, return
            if (audioSource == null)
                return null;

            //set position, rotation and parent
            audioSource.transform.position = position;
            audioSource.transform.SetParent(SoundsParent);

            //play and start coroutine to deactivate
            StartCoroutine(DeactiveSoundAtPointCoroutine(audioSource));
            return Play(audioSource, clip, true, volume);
        }

        /// <summary>
        /// Start audio clip at point. Can set volume
        /// </summary>
        public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1)
        {
            //use this manager's pooling, instead of a specific one
            return Play(poolingSounds, clip, position, volume);
        }

        /// <summary>
        /// Start audio clip at point, with selected volume
        /// </summary>
        public AudioSource Play(AudioStruct audio, Vector3 position)
        {
            //use this manager's pooling, instead of a specific one
            return Play(poolingSounds, audio.audioClip, position, audio.volume);
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
        public AudioSource Play(AudioClip[] clips, Vector3 position, float volume = 1)
        {
            //do only if there are elements in the array
            if (clips.Length > 0)
            {
                return Play(clips[Random.Range(0, clips.Length)], position, volume);
            }

            return null;
        }

        /// <summary>
        /// Start audio clip at point. Get clip and volume random from the array
        /// </summary>
        public AudioSource Play(AudioStruct[] audios, Vector3 position)
        {
            //do only if there are elements in the array
            if (audios.Length > 0)
            {
                return Play(audios[Random.Range(0, audios.Length)], position);
            }

            return null;
        }

        #endregion
    }
}