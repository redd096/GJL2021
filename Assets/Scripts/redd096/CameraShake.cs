namespace redd096
{
	using System.Collections;
	using UnityEngine;

	[AddComponentMenu("redd096/MonoBehaviours/Camera Shake")]
	public class CameraShake : MonoBehaviour
	{
		[Header("Camera - by default is main camera")]
		[Tooltip("If null, use main camera")] [SerializeField] Transform camTransform;

		[Header("Shake")]
		[Tooltip("How long should shake")] [SerializeField] float shakeDuration = 1f;
		[Tooltip("Amplitude of the shake. A larger value shakes the camera harder")] [SerializeField] float shakeAmount = 0.7f;

		[Header("Overwrite - start shake also if another is running")]
		[Tooltip("If another shake is already running, stop it and start new one")] [SerializeField] bool ovewriteShake = true;

		Coroutine shakeCoroutine;
		Vector3 originalPos;

		void Awake()
		{
			//get reference if null
			if (camTransform == null)
			{
				camTransform = Camera.main.transform;
			}

			//then save original position
			originalPos = camTransform.localPosition;
		}

		IEnumerator ShakeCoroutine(float duration, float amount)
		{
			//set shake duration
			float shake = Time.time + duration;

			//shake
			while (shake > Time.time)
			{
				camTransform.localPosition = originalPos + Random.insideUnitSphere * amount;

				yield return null;
			}

			//then reset to original position
			camTransform.localPosition = originalPos;

			shakeCoroutine = null;
		}

        #region public API

		/// <summary>
		/// Start shake, using variables in inspector
		/// </summary>
        public void StartShake()
		{
			StartShake(shakeDuration, shakeAmount);
		}

		/// <summary>
		/// Start shake, using specific duration and amount
		/// </summary>
		public void StartShake(float duration, float amount)
		{
			//do only if there is not another shake, or can overwrite it
			if (shakeCoroutine == null || ovewriteShake)
			{
				//start or restart shake coroutine
				if (shakeCoroutine != null)
					StopCoroutine(shakeCoroutine);

				shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, amount));
			}
		}

        #endregion
    }
}