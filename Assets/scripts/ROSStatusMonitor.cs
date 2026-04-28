using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using System.Net.Sockets;
using System.Reflection;

/// <summary>
/// A dedicated monitor for the ROS Connection.
/// Uses "Sticky" logic to prevent flickering: 
/// Once Green, it stays Green unless an actual Error is reported.
/// </summary>
public class ROSStatusMonitor : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Text component displaying ROS status (e.g. top right of screen)")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool _hasConnectedOnce = false;

    private void Start()
    {
        StartCoroutine(CheckROSConnection());
    }

    private IEnumerator CheckROSConnection()
    {
        ROSConnection ros = ROSConnection.GetOrCreateInstance();
        
        // Initial state
        UpdateText("ROS: <color=yellow>Connecting...</color>");

        while (true)
        {
            // 1. Check for Hard Failure first
            if (ros.HasConnectionError)
            {
                UpdateText("ROS: <color=red>Disconnected</color>");
                _hasConnectedOnce = false; // Reset if we fail
            }
            // 2. If we are already stable, stay Green (Sticky Mode)
            else if (_hasConnectedOnce)
            {
                UpdateText($"ROS: <color=green>Connected ({ros.RosIPAddress})</color>");
            }
            // 3. If not stable yet, perform the deep check
            else 
            {
                bool isSocketCreated = false;
                try 
                {
                    // Reflection to check if the TCP Client object exists
                    var field = typeof(ROSConnection).GetField("tcpClient", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        var client = field.GetValue(ros) as TcpClient;
                        // We only check if client is NOT NULL. We ignore .Connected which can flicker.
                        if (client != null) isSocketCreated = true;
                    }
                }
                catch { }

                if (isSocketCreated)
                {
                    _hasConnectedOnce = true; // Latch to Green
                    UpdateText($"ROS: <color=green>Connected ({ros.RosIPAddress})</color>");
                }
                else
                {
                    UpdateText("ROS: <color=yellow>Connecting...</color>");
                }
            }
            
            // Check every 1 second (slower check reduces flickering feel)
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void UpdateText(string msg)
    {
        if (statusText) statusText.text = msg;
    }
}