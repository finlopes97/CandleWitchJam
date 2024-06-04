using UnityEngine;

namespace Obstacles
{
	public class FallingObjectTrigger : MonoBehaviour
	{
		private FallingObstacle parent;

		public void SetParent(FallingObstacle parent)
		{
			this.parent = parent;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (parent != null)
			{
				parent.OnTriggerAreaEnter(other);
			}
		}
	}
}