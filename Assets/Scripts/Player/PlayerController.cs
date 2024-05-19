using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

namespace Player
{
    /// <summary>
    /// Controller for the player, tracks their health, abilities, movement settings and input.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Settings")] 
        [Tooltip("Max value for player health.")]
        [SerializeField] private float maxHealth = 100.0f;
        [Tooltip("Whether or not the player takes fall damage.")]
        [SerializeField] private bool takesFallDamage = true;
        [Tooltip("Velocity at which player takes fall damage (see Fall Speed Multiplier).")] 
        [SerializeField] private float playerFallDamageVelocityThreshold = 10;

        [Header("Movement Settings")] 
        [Tooltip("Speed at which the player moves.")]
        [SerializeField] private float moveSpeed = 10.0f;
        [Tooltip("Force with which the player can jump.")]
        [SerializeField] private float jumpPower = 8.0f;
        [Tooltip("Rate at which the player's jump decreases in force (i.e. variable jump height).")]
        [SerializeField] [Range(0.1f, 1.0f)] private float jumpFallOffRate = 1.0f;

        // [Header("Ability Settings")] 
        // [Tooltip("Amount of time between key presses to register a double tap.")]
        // [SerializeField] [Range(0.10f, 1.00f)] private float doublePressThreshold = 0.25f;

        [Header("Dash Settings")]
        [Tooltip("Force applied to the character when dashing.")]
        [SerializeField] private float dashPower = 500f;
        [Tooltip("Cooldown between dashes in seconds.")]
        [SerializeField] private float dashCooldown = 3.0f;
        [Tooltip("Duration of the dash in seconds.")]
        [SerializeField] private float dashDuration = 0.3f;

        [Header("Double Jump")]
        [Tooltip("Amount of times a player can jump. Set to 2 for testing purposes.")]
        [SerializeField] private int maxJumps = 2;
        
        [Header("Ground Checks")]
        [Tooltip("Transform (point in space) for detecting ground beneath player.")]
        [SerializeField] private Transform groundCheck;
        [Tooltip("Size of that transform, should be roughly in line with the player character's sprite's feet.")]
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
        [Tooltip("Layer for detecting intersections with ground.")]
        [SerializeField] private LayerMask groundLayer;

        [Header("Gravity")] 
        [Tooltip("The base gravity applied to the player.")]
        [SerializeField] private float baseGravity = 2.0f;
        [Tooltip("The max speed at which a player can fall.")]
        [SerializeField] private float maxFallSpeed = 18.0f;
        [Tooltip("The rate at which a player's fall speed can increase while falling.")]
        [SerializeField] private float fallSpeedMultiplier = 2.0f;
        
        [Header("Debug Settings")] 
        [Tooltip("God mode.")]
        [SerializeField] private bool isInvulnerable;
        [Tooltip("Scary mode.")]
        [SerializeField] private bool isScary;
        [SerializeField] private Sprite scarySprite;
        [Tooltip("Anime Mode.")]
        [SerializeField] private GameObject dashTrailObject;
        [SerializeField] private bool leaveAnimeTrail;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private float _horizontalMovementDirection;
        private int _jumpsRemaining;
        private float _lastDashTime;
        private float _playerFacingDirection;
        private bool _isDashing;
        private bool _isGrounded;

        //audio
        private EventInstance playerFootsteps;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            dashTrailObject.SetActive(false);
            
            ScaryCheck();

        }

        private void Start()
        {
            playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.footsteps);
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
                // Debug.Log($"Player is facing: {(_playerFacingDirection < 0 ? "left" : "right")}");
            }

            _horizontalMovementDirection = context.ReadValue<float>();
        }

        /// <summary>
        /// Initiates a jump if jumps are remaining.
        /// </summary>
        /// <param name="context">Input context containing action details.</param>
        public void Jump(InputAction.CallbackContext context)
        {
            if(_jumpsRemaining > 0)
            {
                if (context.performed)
                {
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
                    _jumpsRemaining--;
                }
                else if (context.canceled)
                {
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * jumpFallOffRate);
                    _jumpsRemaining--;
                }
            }
        }
        
        /// <summary>
        /// Performs a dash in the facing direction if the dash action is performed.
        /// </summary>
        /// <param name="context">Input context containing action details.</param>
        public void Dash(InputAction.CallbackContext context)
        { 
            if (context.started && Time.time >= _lastDashTime + dashCooldown && !_isDashing)
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
            //AudioManager.instance.PlayOneShot(FMODEvents.instance.dash, transform.position);

            if(leaveAnimeTrail) { dashTrailObject.SetActive(true); }
            _isDashing = true; 
            _lastDashTime = Time.time;
            _rigidbody2D.AddForce(new Vector2((_playerFacingDirection * dashPower), _rigidbody2D.velocity.y + 1), ForceMode2D.Force);
            
            yield return new WaitForSeconds(dashDuration);
            
            if (leaveAnimeTrail) { dashTrailObject.SetActive(false); }
            _isDashing = false;
        }

        /// <summary>
        /// Checks if the player is standing on the ground and resets jump count if true.
        /// </summary>
        private void GroundCheck()
        {
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
            {
                _jumpsRemaining = maxJumps;
                _isGrounded = true;
            } else {
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
            // Debug.Log($"Player's vertical velocity: {_rigidbody2D.velocity.y}...");
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

        private void UpdateSound() {

            //start footsteps event if the player moves and is on the ground
            if(_rigidbody2D.velocity.x != 0 && _isGrounded && !_isDashing) {

                //get the playback state
                PLAYBACK_STATE playbackState;
                playerFootsteps.getPlaybackState(out playbackState);

                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                    playerFootsteps.start();

            } else {
                playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}