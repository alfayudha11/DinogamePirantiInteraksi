using UnityEngine;
using System.IO.Ports;

public class ArduinoCommunication : MonoBehaviour
{
    private string _portName = "COM3"; // Change this to match your Arduino's port
    private int _baudRate = 9600;

    private SerialPort _serialPort;

    private void Awake()
    {
        // Get a list of available COM ports
        string[] ports = SerialPort.GetPortNames();

        foreach (string port in ports)
        {
            // Try opening each port and check if it's your Arduino
            _serialPort = new SerialPort(port, 9600); // Use the correct baud rate

            try
            {
                _serialPort.Open();
                string signature = _serialPort.ReadLine();
                _serialPort.Close();

                if (signature.Contains("ArduinoSignature")) // Replace with your Arduino's signature
                {
                    Debug.Log("Arduino found on port: " + port);
                    // Now you know which COM port to use
                    // You can store this port in a variable or PlayerPrefs for later use
                    break;
                }
            }
            catch (System.Exception e)
            {
                // Handle errors when opening the port
                Debug.LogWarning("Error opening " + port + ": " + e.Message);
            }
        }
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
        if(_serialPort == null)
        {
            _serialPort = new SerialPort(_portName, _baudRate);
            _serialPort.Open();
            Debug.Log("serialPort is null");
        }
        
        if (_serialPort.IsOpen)
        {
            string data = _serialPort.ReadLine().Trim(); // Remove leading/trailing whitespace

            // Split the data into X, Y, and Z values
            string[] values = data.Split(' ');
            
            if (values.Length == 3 &&
                float.TryParse(values[0], out float xValue) &&
                float.TryParse(values[1], out float yValue) &&
                float.TryParse(values[2], out float zValue))
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

        return -1;
    }
}
