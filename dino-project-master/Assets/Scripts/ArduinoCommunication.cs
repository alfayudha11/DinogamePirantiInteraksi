using UnityEngine;
using System.IO.Ports;

public class ArduinoCommunication : MonoBehaviour
{
    private int _baudRate = 9600;
    public bool _isReady { get; private set; }
    private SerialPort _serialPort;

    private void Awake()
    {
        // Debug.Log("arduino awaken");
        // _isReady = false;
        SearchAndOpenPort();
    }

    // private void SearchAndOpenPort()
    // {
    //     string[] ports = SerialPort.GetPortNames();

    //     // Debug.Log("Available Ports:");
    //     // foreach (string port in ports)
    //     // {
    //     //     Debug.Log(port);
    //     // }

    //     foreach (string port in ports)
    //     {
    //         _serialPort = new SerialPort(port, _baudRate);

    //         try
    //         {
    //             _serialPort.Open();
    //             string data = _serialPort.ReadLine().Trim(); // Remove leading/trailing whitespace

    //             // Split the data into X, Y, and Z values
    //             string[] values = data.Split(' ');
                
    //             string signature = values[0];
    //             _serialPort.Close();

    //             if (signature.Contains("ArduinoSignature"))
    //             {
    //                 Debug.Log("Arduino found on port: " + port);
    //                 _serialPort = new SerialPort(port, _baudRate);
    //                 _serialPort.Open();
    //                 _isReady = true;
    //                 return;
    //             }
    //         }
    //         catch (System.Exception e)
    //         {
    //             _isReady = false;
    //             Debug.LogWarning("Error opening " + port + ": " + e.Message);
    //         }
    //     }
    // }
    private void SearchAndOpenPort()
    {
        string[] ports = SerialPort.GetPortNames();
        int maxAttempts = 1;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            foreach (string port in ports)
            {
                try
                {
                    _serialPort = new SerialPort(port, _baudRate);
                    _serialPort.ReadTimeout = 1000; // Set a read timeout
                    _serialPort.Open();
                    string data = _serialPort.ReadLine().Trim();

                    // Check for Arduino signature and handle accordingly
                    // Split the data into X, Y, and Z values
                    string[] values = data.Split(' ');
                    
                    string signature = values[0];
                    _serialPort.Close();

                    if (signature.Contains("ArduinoSignature"))
                    {
                        Debug.Log("Arduino found on port: " + port);
                        _serialPort.Close();
                        _serialPort = new SerialPort(port, _baudRate);
                        _serialPort.Open();
                        _isReady = true;
                        return;
                    }
                    else
                    {
                        _serialPort.Close();
                    }
                }
                catch (System.Exception e)
                {
                    _isReady = false;
                    Debug.LogWarning("Error opening " + port + ": " + e.Message);
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                }
            }

            attempts++;
        }

        Debug.LogWarning("Arduino not found after " + maxAttempts + " attempts.");
        _isReady = false;
    }



    private void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    public void CloseSerialPort()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
        }
    }

    public float ParseArduino(char axis)
    {
        if (_isReady)
        {
            string data = _serialPort.ReadLine().Trim(); // Remove leading/trailing whitespace

            // Split the data into X, Y, and Z values
            string[] values = data.Split(' ');
            
            if (values.Length == 4 &&
                // float.TryParse(values[0], out float xValue) &&
                float.TryParse(values[1], out float xValue) &&
                float.TryParse(values[2], out float yValue) &&
                float.TryParse(values[3], out float zValue))
            {
                if(axis == 'x')
                {
                    Debug.Log("xValue: "+xValue);
                    return xValue;
                }
                else if(axis == 'y')
                {
                    Debug.Log("yValue: "+yValue);

                    return yValue;
                }
                else if(axis == 'z')
                {
                    Debug.Log("zValue: "+zValue);
                    return zValue;
                }
            }
            else
            {
                // Handle parsing failure or provide a warning message
                Debug.LogWarning("Data is not in the expected format: " + data);
                return -1;
            }
        }
        else
        {
            Debug.Log("no arduino");
        }

        return -1;
    }
}
