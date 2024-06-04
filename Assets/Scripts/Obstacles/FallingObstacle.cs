using System;
using Generics;
using UnityEngine;

namespace Obstacles
{
	public class FallingObstacle : MonoBehaviour
	{
		[Header("=== Settings ===")]
		[SerializeField, Tooltip("How much static damage the falling object will do.")]
		private int damageAmount = 20;
		
		[SerializeField] 
		private GameObject triggerAreaPrefab;
		[SerializeField]
		private LayerMask groundLayerMask;

		private Rigidbody2D _rigidbody;
		private GameObject _triggerArea;

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			_rigidbody.isKinematic = true;
			CreateTriggerArea();
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
			{
				other.gameObject.GetComponent<HealthComponent>().ApplyDamage(damageAmount);
			}
			Destroy(gameObject);
		}

		private void CreateTriggerArea()
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayerMask);

			if (hit.collider != null)
			{
				// Calculate distance from obj to floor
				float distanceToFloor = hit.distance;

				// Create a trigger area
				_triggerArea = Instantiate(triggerAreaPrefab, transform, true);

				// Set pos, then size
				_triggerArea.transform.position = new Vector2(transform.position.x, transform.position.y - distanceToFloor / 2);
				BoxCollider2D boxCollider = _triggerArea.GetComponent<BoxCollider2D>();
				boxCollider.size = new Vector2(boxCollider.size.x, distanceToFloor);
				
				// Call OnTriggerEnter2D when the trigger area is instantiated
				FallingObjectTrigger fallingObjectTrigger = _triggerArea.AddComponent<FallingObjectTrigger>();
				fallingObjectTrigger.SetParent(this);
			}
			else
			{
				Debug.LogWarning("No floor found beneath object...");
			}
		}
		
		public void OnTriggerAreaEnter(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				Debug.Log($"{other.gameObject.name} detected...");
				Destroy(_triggerArea);
				_rigidbody.isKinematic = false;
			}
		}
	}
}
