using Generics;
using UnityEngine;

namespace Enemies.Sludge
{
    public class SludgeBehavior : MonoBehaviour
    {
        [Header("Enemy Settings")] 
        [SerializeField, Tooltip("How quickly this enemy will pursue the player.")]
        private float moveSpeed = 2.0f;
        
        [Header("Ground Checks")]
        [SerializeField, Tooltip("Transform (point in space) for detecting ground beneath an enemy.")]
        private Transform groundCheck;
        [SerializeField, Tooltip("Size of that transform, should be roughly in line with the enemy's character's sprite's feet.")]
        private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
        [SerializeField, Tooltip("Layer for detecting intersections with ground.")]
        private LayerMask groundLayer;
        
        [Header("Gravity")] [SerializeField, Tooltip("The base gravity applied to the enemy.")]
        private float baseGravity = 2.0f;
        [SerializeField, Tooltip("The max speed at which an enemy can fall.")]
        private float maxFallSpeed = 18.0f;
        [SerializeField, Tooltip("The rate at which a enemy's fall speed can increase while falling.")]
        private float fallSpeedMultiplier = 2.0f;
        
        private GameObject _player;
        private Transform _groundCheck;
        private Rigidbody2D _rigidbody2D;
        private float _previousVelocityY;
        private bool _isGrounded;
        private Vector3 _originalScale;
        
        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _originalScale = transform.localScale;
        }
        
        private void Update()
        {
            _previousVelocityY = _rigidbody2D.velocity.y; 
            GroundCheck();
            
            if (_isGrounded)
            {
                HandleMovement();
            }
            Gravity();
        }

        private void HandleMovement()
        {
            if (_player)
            {
                Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, 0).normalized;
                _rigidbody2D.velocity = new Vector2(direction.x * moveSpeed, _rigidbody2D.velocity.y);

                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(-_originalScale.x, _originalScale.y, _originalScale.z);
                }

                if (direction.x < 0)
                {
                    transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
                }
            }
        }
        
        /// <summary>
        /// Checks if the player is standing on the ground and resets jump count if true.
        /// </summary>
        private void GroundCheck()
        {
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
            {
                if (!_isGrounded)
                {
                    float fallVelocity = Mathf.Abs(_previousVelocityY);
                    HealthComponent healthComponent = GetComponent<HealthComponent>();
                    healthComponent?.ApplyFallDamage(fallVelocity);

                    _isGrounded = true;
                }
            } 
            else
            {
                _isGrounded = false;
            }
        }
        
        /// <summary>
        /// Checks if the enemy is on the ground, used for fall damage.
        /// </summary>
        private void Gravity()
        {
            if (_rigidbody2D.velocity.y < 0)
            {
                _rigidbody2D.gravityScale = baseGravity * fallSpeedMultiplier;
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,
                    Mathf.Max(_rigidbody2D.velocity.y, -maxFallSpeed));
            }
            else
            {
                _rigidbody2D.gravityScale = baseGravity;
            }
        }
    }
}
