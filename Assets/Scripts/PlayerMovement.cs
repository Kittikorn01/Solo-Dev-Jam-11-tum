using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles 8-directional player movement using the New Input System and procedural sine-wave animation.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Procedural Animation (Sine Wave)")]
    [SerializeField] private Transform visualTransform; // The child transform containing the sprite
    [SerializeField] private float walkAnimFrequency = 10f;
    [SerializeField] private float walkAnimAmplitude = 0.15f;
    [SerializeField] private float idleAnimFrequency = 2f;
    [SerializeField] private float idleAnimAmplitude = 0.05f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector3 initialScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure we have a reference to the visual child; fallback to self if null
        if (visualTransform == null) visualTransform = transform;
        initialScale = visualTransform.localScale;
    }

    private void Update()
    {
        // 1. Gather Input using New Input System (Simple API)
        HandleInput();

        // 2. Handle Procedural Animation
        HandleSineAnimation();
    }

    private void HandleInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // No keyboard connected

        // Reset input every frame
        moveInput = Vector2.zero;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y += 1;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y -= 1;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x -= 1;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x += 1;

        // Normalize to prevent faster diagonal movement
        if (moveInput.sqrMagnitude > 1)
        {
            moveInput.Normalize();
        }
    }

    private void FixedUpdate()
    {
        // 3. Apply Velocity
        // Note: Using rb.linearVelocity for Unity 2023+ (consistent with your current project state)
        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Applies a Squash & Stretch effect using Sine waves based on movement state.
    /// </summary>
    private void HandleSineAnimation()
    {
        float freq = moveInput.sqrMagnitude > 0.01f ? walkAnimFrequency : idleAnimFrequency;
        float amp = moveInput.sqrMagnitude > 0.01f ? walkAnimAmplitude : idleAnimAmplitude;

        // Calculate sine value
        float sineValue = Mathf.Sin(Time.time * freq);

        // Apply Squash & Stretch: When Y stretches, X squashes (maintaining volume feel)
        float squashStretchX = initialScale.x - (sineValue * amp * 0.5f);
        float squashStretchY = initialScale.y + (sineValue * amp);

        visualTransform.localScale = new Vector3(squashStretchX, squashStretchY, initialScale.z);
        
        // Slight tilt towards movement direction for extra juice
        if (moveInput.x != 0)
        {
            float targetRotation = -moveInput.x * 5f;
            visualTransform.localRotation = Quaternion.Euler(0, 0, targetRotation);
        }
        else
        {
            visualTransform.localRotation = Quaternion.Lerp(visualTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
        }
    }
}
