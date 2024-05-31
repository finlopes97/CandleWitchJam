using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Generics;
using FMOD.Studio;
using Managers;

namespace Player
{
    /// <summary>
    /// Controller for the player's movement, abilities and input settings and
    /// behaviors.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")] 
        [SerializeField, Tooltip("Speed at which the player moves.")]
        private float moveSpeed = 10.0f;
        
        [Header("Jump Settings")]
        [SerializeField, Tooltip("Force with which the player can jump.")]
        private float jumpPower = 8.0f;
        [SerializeField, Tooltip("Rate at which the player's jump decreases in " +
                                 "force (i.e. variable jump height)."), Range(0.1f, 1.0f)]
        private float jumpFallOffRate = 1.0f;
        [SerializeField, Tooltip("The particle system to spawn on a jump.")]
        private ParticleSystem jumpParticleSystem;
        
        [Header("Dash Settings")] [SerializeField, Tooltip("Whether or not the player can dash")]
        private bool canDash;
        [SerializeField, Tooltip("Force applied to the character when dashing.")]
        private float dashPower = 500f;
        [SerializeField, Tooltip("Cooldown between dashes in seconds.")]
        private float dashCooldown = 3.0f;
        [SerializeField, Tooltip("Duration of the dash in seconds.")]
        private float dashDuration = 0.3f;

        [Header("Double Jump")]
        [SerializeField, Tooltip("Amount of times a player can jump. Set to 2 for testing purposes.")]
        private int maxJumps = 1;
        
        [Header("Ground Checks")]
        [SerializeField, Tooltip("Transform (point in space) for detecting ground beneath player.")]
        private Transform groundCheck;
        [SerializeField, Tooltip("Size of that transform, should be roughly in line with the player character's sprite's feet.")]
        private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
        [SerializeField, Tooltip("Layer for detecting intersections with ground.")]
        private LayerMask groundLayer;

        [Header("Gravity")] [SerializeField, Tooltip("The base gravity applied to the player.")]
        private float baseGravity = 2.0f;
        [SerializeField, Tooltip("The max speed at which a player can fall.")]
        private float maxFallSpeed = 18.0f;
        [SerializeField, Tooltip("The rate at which a player's fall speed can increase while falling.")]
        private float fallSpeedMultiplier = 2.0f;
        
        [Header("Debug Tools")]
        [SerializeField, Tooltip("Scary mode.")]
        private bool isScary;
        [SerializeField] 
        private Sprite scarySprite;
        [SerializeField, Tooltip("Anime Mode.")]
        private GameObject dashTrailObject;

        [SerializeField] private bool leaveAnimeTrail;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private float _horizontalMovementDirection;
        private int _jumpsRemaining;
        private float _lastDashTime;
        private float _playerFacingDirection;
        private bool _isDashing;
        private bool _isGrounded;
        private float _previousVelocityY;

        private EventInstance _playerFootsteps;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _jumpsRemaining = maxJumps;
            // dashTrailObject.SetActive(false);
            ScaryCheck();
            _playerFootsteps = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.footsteps);
        }

        /// <summary>
        /// Handles player movement input.
        /// </summary>
        /// <param name="context">Input context containing action details.</param>
        public void Move(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _playerFacingDirection = context.ReadValue<float>();
            }

            _horizontalMovementDirection = context.ReadValue<float>();
        }

        /// <summary>
        /// Initiates a jump if jumps are remaining.
        /// </summary>
        /// <param name="context">Input context containing action details.</param>
        public void Jump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log($"Jumps remaining before HandleJump(): {_jumpsRemaining}");
                HandleJump();
                Debug.Log($"Jumps remaining after HandleJump(): {_jumpsRemaining}");
            }
            else if (context.canceled)
            {
                if (_rigidbody2D.velocity.y > 0)
                {
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * jumpFallOffRate);
                }
            }
        }
    
        private void HandleJump()
        {
            if (_jumpsRemaining <= 0) return;
            
            Instantiate(jumpParticleSystem, groundCheck);
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
            _jumpsRemaining--;
            // Play jump sound
            if (_isGrounded) 
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.jump, transform.position);
            else
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.doubleJump, transform.position);
        }

        /// <summary>
        /// Performs a dash in the facing direction if the dash action is performed.
        /// </summary>
        /// <param name="context">Input context containing action details.</param>
        public void Dash(InputAction.CallbackContext context)
        {
            if (canDash && context.started && Time.time >= _lastDashTime + dashCooldown && !_isDashing)
            {
                StartCoroutine(DashCoroutine());
            }
        }

        /// <summary>
        /// Coroutine to handle the dash duration and reset the dash state.
        /// </summary>
        private IEnumerator DashCoroutine()
        {
            //play dash sound here
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.dash, transform.position);

            if (leaveAnimeTrail)
            {
                dashTrailObject.SetActive(true);
            }

            _isDashing = true;
            _lastDashTime = Time.time;
            _rigidbody2D.AddForce(new Vector2((_playerFacingDirection * dashPower), _rigidbody2D.velocity.y + 1),
                ForceMode2D.Force);

            yield return new WaitForSeconds(dashDuration);

            if (leaveAnimeTrail)
            {
                dashTrailObject.SetActive(false);
            }

            _isDashing = false;
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

                    // Play landing sound (maybe check for vertical velocity > threshold?)
                    AudioManager.Instance.PlayOneShot(FMODEvents.instance.landing, transform.position);
                   
                    Debug.Log("Player just landed. Resetting jumps.");
                    _jumpsRemaining = maxJumps;
                    _isGrounded = true;
                }
            } 
            else
            {
                _isGrounded = false;
            }
        }

        /// <summary>
        /// Checks if the player is standing on the ground and resets jump count if true.
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

        /// <summary>
        /// Updates the player's abilities based on the event data.
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="data">The data associated with the event.</param>
        public void UpdateAbilities(Component sender, object data)
        {
            Debug.Log($"Event from {sender} has been called.");
            if (data is string abilityName)
            {
                switch (abilityName)
                {
                    case "DoubleJump":
                        maxJumps = 2;
                        _jumpsRemaining = maxJumps;
                        break;
                    case "Dash":
                        canDash = true;
                        break;
                    default:
                        return;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!_isDashing)
            {
                _rigidbody2D.velocity = new Vector2(_horizontalMovementDirection * moveSpeed, _rigidbody2D.velocity.y);
            }

            UpdateSound();
        }

        private void Update()
        {
            _previousVelocityY = _rigidbody2D.velocity.y;
            GroundCheck();
            Gravity();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        private void ScaryCheck()
        {
            if (isScary)
            {
                _spriteRenderer.sprite = scarySprite;
            }
        }

        /// <summary>
        /// Updates the sound effects based on the player's movement state.
        /// </summary>
        private void UpdateSound()
        {
            // Start footsteps event if the player moves and is on the ground
            if (_rigidbody2D.velocity.x != 0 && _isGrounded && !_isDashing)
            {
                // Get the playback state
                _playerFootsteps.getPlaybackState(out var playbackState);

                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                    _playerFootsteps.start();
            }
            else
            {
                _playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}