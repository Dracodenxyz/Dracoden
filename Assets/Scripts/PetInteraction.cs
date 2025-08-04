using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PetInteraction : MonoBehaviour
{

    public Animator animator; 
    public PetEnergy petEnergy;
    public float playAmount = 20f;
    public Button[] playButtons; // Assign 4 buttons in Inspector
    
    private ARRaycastManager arRaycastManager;
    private Camera arCamera;
    private Touchscreen touchscreen;
    public int touchCount;

    void Start()
    {
        // Get AR components
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = Camera.main;
        
        if (arCamera == null)
        {
            Debug.LogError("No main camera found!");
        }
        
        if (arRaycastManager == null)
        {
            Debug.LogWarning("No AR Raycast Manager found. Using fallback touch detection.");
        }

        // Get touchscreen input
        touchscreen = Touchscreen.current;
        if (touchscreen == null)
        {
            Debug.LogError("No touchscreen found!");
        }
    }

    // Call this from each button's OnClick event, passing the button index (0-3)
    public void OnPlayButtonClicked(int buttonIndex)
    {
        Debug.Log("Play button clicked: " + buttonIndex);
        touchCount++;

        if(touchCount == 7)

            SceneManager.LoadScene("Transformation");
        if (buttonIndex == 0)
        {
            animator.Play("Angry");
        }
        else if (buttonIndex == 1)
        {
            animator.Play("Eat_Drink");            
        }
        else if (buttonIndex == 2)
        {
            animator.Play("Confused");
        }
        else if (buttonIndex == 3)
        {
            animator.Play("Happy");
        }

        if (petEnergy != null)
            petEnergy.PlayWithPet(playAmount);
        if (playButtons != null && buttonIndex >= 0 && buttonIndex < playButtons.Length)
            StartCoroutine(DisableButtonTemporarily(playButtons[buttonIndex], 10f));
    }

    private IEnumerator DisableButtonTemporarily(Button button, float seconds)
    {
        button.interactable = false;
        yield return new WaitForSeconds(seconds);
        button.interactable = true;
    }

    void Update()
    {
        // Handle touch input using new Input System
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = touchscreen.primaryTouch.position.ReadValue();
            
            // Only process on touch begin (first frame of press)
            if (touchscreen.primaryTouch.press.wasPressedThisFrame)
            {
                Debug.Log("Touch detected at position: " + touchPosition);
                
                // Try AR raycast first
                if (TryARTouch(touchPosition))
                {
                    return;
                }
                
                // Fallback to screen space detection
                TryScreenSpaceTouch(touchPosition);
            }
        }
    }

    bool TryARTouch(Vector2 touchPosition)
    {
        if (arRaycastManager == null || arCamera == null)
            return false;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        
        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Check if any hit is close to our pet
            foreach (ARRaycastHit hit in hits)
            {
                float distance = Vector3.Distance(hit.pose.position, transform.position);
                if (distance < 0.5f) // Within 0.5 units of pet
                {
                    Debug.Log("AR Touch detected on pet! Distance: " + distance);
                    OnPetTouched();
                    return true;
                }
            }
        }
        
        return false;
    }

    void TryScreenSpaceTouch(Vector2 touchPosition)
    {
        if (arCamera == null)
            return;

        // Convert screen position to world position
        Vector3 worldPosition = arCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, arCamera.nearClipPlane));
        
        // Cast a ray from camera through touch point
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Physics raycast hit: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Touch detected on pet via physics raycast!");
                OnPetTouched();
            }
        }
        else
        {
            // Fallback: check if touch is roughly in the pet's screen area
            Vector3 petScreenPos = arCamera.WorldToScreenPoint(transform.position);
            float distance = Vector2.Distance(touchPosition, petScreenPos);
            
            if (distance < 100f) // Within 100 pixels
            {
                Debug.Log("Touch detected near pet via screen space! Distance: " + distance);
                OnPetTouched();
            }
        }
    }

    void OnPetTouched()
    {
        Debug.Log("Pet was touched!");
        
        // React to touch (you can add animations here)
        StartCoroutine(PetReaction());
        
        // Increase energy
        if (petEnergy != null)
            petEnergy.PlayWithPet(playAmount);
    }

    IEnumerator PetReaction()
    {
        // Simple scale animation as a reaction
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1f;
        
        // Scale up
        float duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Scale back down
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
}