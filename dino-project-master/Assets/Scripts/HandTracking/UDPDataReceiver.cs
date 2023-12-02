using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class UDPDataReceiver : MonoBehaviour
{
    private const int portNum = 12345; // Set the port number here
    private const string receiverIP = "127.0.0.1"; // Set the IP address here

    public bool isAlive;
    // public int portNum;
    public HandTrackingControlManager handTrackingControlManager;
    public static UDPDataReceiver Instance;

    string packetReceived;
    string notification;
    static Thread receiverThread;
    static UdpClient udpClient;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        handTrackingControlManager = new();
        udpClient = new UdpClient(portNum);
        receiverThread = new Thread(UDPReceiver);
        receiverThread.Start();
    }

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        udpClient.Close();
        receiverThread.Abort();
        isAlive = false;
    }
#endif

    void UDPReceiver()
    {
        notification = "Receiver Thread Opened";
        while (isAlive)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(receiverIP), portNum); // Use receiverIP here
                byte[] data = udpClient.Receive(ref remoteEP);
                packetReceived = Encoding.ASCII.GetString(data);

                if (handTrackingControlManager != null && packetReceived != "")
                {
                    string receivedData = packetReceived;

                    // int handPresence = Convert.ToInt32(receivedData);

                    // // Debug.Log("HandCount: " + handPresence);

                    // handTrackingControlManager.SetHandCount(handPresence); 

                    if (receivedData.Length >= 1)
                    {
                        int handPresence = Convert.ToInt32(receivedData);

                        handTrackingControlManager.SetHandCount(handPresence); 
                        Debug.Log("HandCount: " + handPresence);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("UDP Receive Error: " + e.Message);
            }
        }
        udpClient.Close();
    }
}
