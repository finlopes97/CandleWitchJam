using System.Collections;
using UnityEngine;

namespace Obstacles
{
	public class PlatformSpawnManager : MonoBehaviour
	{
		public static PlatformSpawnManager Instance;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void StartRespawnCoroutine(GameObject platform, float respawnTime)
		{
			StartCoroutine(RespawnPlatform(platform, respawnTime));
		}

		private IEnumerator RespawnPlatform(GameObject platform, float respawnTime)
		{
			yield return new WaitForSeconds(respawnTime);
			platform.SetActive(true);
		}
	}
}