using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class ShakeShuffleTree : MonoBehaviour
{
    [Header("Detection Settings")]
    public float shakeThreshold = 2.0f;
    public float cooldown = 1.0f;

    private float lastShakeTime;

    void Start()
    {
        // Check if Accelerometer exists and enable it
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("<color=white>[SHAKE SYSTEM]</color> Accelerometer Enabled.");
        }
        else
        {
            Debug.LogWarning("Accelerometer not found on this device.");
        }
    }

    void Update()
    {
        // 1. SIMULATED SHAKE (Editor Testing)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("<color=yellow>[EDITOR]</color> Space pressed. Simulating shake...");
            TriggerReset(new Vector3(0, 1.5f, 0));
        }

        // 2. PHYSICAL SHAKE (Mobile)
        DetectShake();
    }

    private void DetectShake()
    {
        if (Accelerometer.current == null) return;

        // Get the current acceleration vector
        Vector3 acceleration = Accelerometer.current.acceleration.ReadValue();

        // We use sqrMagnitude for better mobile performance
        float force = acceleration.sqrMagnitude;

        if (force >= (shakeThreshold * shakeThreshold))
        {
            if (Time.time - lastShakeTime > cooldown)
            {
                lastShakeTime = Time.time;
                TriggerReset(acceleration);
            }
        }
    }

    private void TriggerReset(Vector3 accelData)
    {
        // Debugging angles for Logcat
        float angleX = Mathf.Atan2(accelData.y, accelData.z) * Mathf.Rad2Deg;
        float angleY = Mathf.Atan2(accelData.x, accelData.z) * Mathf.Rad2Deg;

        Debug.Log($"<color=orange>[SHAKE EVENT]</color> Force: {accelData.magnitude:F2} | AngleX: {angleX:F1}");

        // --- UPDATED LOGIC TO USE SNAPSHOT RESET ---

        // Find and Reset Search Visualizer
        TreeSearchVisualizer search = Object.FindFirstObjectByType<TreeSearchVisualizer>();
        if (search != null)
        {
            Debug.Log("<color=cyan>Shake System:</color> Rebuilding Search Tree from Snapshot.");
            // Call the snapshot method we added to keep same values
            search.ResetToSnapshot();
        }

        // Find and Reset Sort Visualizer
        TreeSortVisualizer sort = Object.FindFirstObjectByType<TreeSortVisualizer>();
        if (sort != null)
        {
            Debug.Log("<color=green>Shake System:</color> Rebuilding Sort Tree from Snapshot.");
            // Call the snapshot method we added to keep same values
            sort.ResetToSnapshot();
        }
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}