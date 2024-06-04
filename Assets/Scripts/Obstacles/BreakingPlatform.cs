using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Obstacles
{
	public class BreakingPlatform : MonoBehaviour
	{
		[Header("=== Settings ===")] 
		[SerializeField, Tooltip("How long before the platform breaks.")]
		private float secondsBeforeDisaster = 2.0f;
		[SerializeField, Tooltip("How long before the platform respawns, set to 0 if it shouldn't.")]
		private float secondsBeforeRespawn = 2.0f;
		[Tooltip("FEEL feedback reference, ask Kyle about it.")]
		public MMF_Player breakingFeedbacks;

		private bool _havingBreakdown;

		private void OnEnable()
		{
			_havingBreakdown = false;
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.CompareTag("Player") && !_havingBreakdown)
			{
				StartCoroutine(BreakPlatform());
			}
		}

		private IEnumerator BreakPlatform()
		{
			_havingBreakdown = true;
			breakingFeedbacks.PlayFeedbacks();
			Debug.Log("To burn it back it down...");
			yield return new WaitForSeconds(secondsBeforeDisaster);
			gameObject.SetActive(false);
			if (secondsBeforeRespawn > 0)
			{
				Debug.Log("Building it up...");
				PlatformSpawnManager.Instance.StartRespawnCoroutine(gameObject, secondsBeforeRespawn);
			}
		}
	}
}