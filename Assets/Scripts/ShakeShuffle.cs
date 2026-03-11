using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections.Generic;

public class ShakeShuffle : MonoBehaviour
{
    [Header("Detection Settings")]
    public float shakeThreshold = 2.0f;
    public float cooldown = 1.5f;

    private float lastShakeTime;

    void Start()
    {
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("<color=white>[SHAKE SYSTEM]</color> Array Reset System Active.");
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleReset();
        }

        if (Accelerometer.current != null)
        {
            Vector3 accel = Accelerometer.current.acceleration.ReadValue();

            if (accel.sqrMagnitude > 1.0f && Time.frameCount % 30 == 0)
            {
                float angle = Mathf.Atan2(accel.y, accel.z) * Mathf.Rad2Deg;
                Debug.Log($"Logcat: Current Device Tilt Angle: {angle:F1}");
            }

            if (accel.sqrMagnitude >= (shakeThreshold * shakeThreshold))
            {
                if (Time.time - lastShakeTime > cooldown)
                {
                    lastShakeTime = Time.time;
                    HandleReset();
                }
            }
        }
    }

    private void HandleReset()
    {
        Debug.Log("<color=orange>[SHAKE DETECTED]</color> Resetting Active Linear Visualizer...");

        var linear = Object.FindFirstObjectByType<LinearSearchVisualizer>();
        if (linear != null) linear.ResetToSnapshot();

        var jump = Object.FindFirstObjectByType<JumpSearchVisualizer>();
        if (jump != null) jump.ResetToSnapshot();

        var radix = Object.FindFirstObjectByType<RadixSortVisualizer>();
        if (radix != null) radix.ResetToSnapshot();

        var merge = Object.FindFirstObjectByType<MergeSortVisualizer>();
        if (merge != null) merge.ResetToSnapshot();

#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}