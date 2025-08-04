using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject cubePrefab;
    private bool cubePlaced = false;
    private ARPlaneManager planeManager;

    void Awake()
    {
        planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!cubePlaced && args.added != null && args.added.Count > 0)
        {
            // Place the cube on the first detected plane
            ARPlane plane = args.added[0];
            Vector3 position = plane.transform.position;
            Instantiate(cubePrefab, position, Quaternion.identity);
            cubePlaced = true;

            // Disable further plane detection
            planeManager.enabled = false;

            // Hide all existing planes
            foreach (var trackedPlane in planeManager.trackables)
            {
                trackedPlane.gameObject.SetActive(false);
            }
        }
}
}