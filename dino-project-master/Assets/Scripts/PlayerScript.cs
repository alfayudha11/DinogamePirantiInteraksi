using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour {
    private const string PLAYER_JUMP_COUNT = "PlayerJumpCount";

    private ArduinoCommunication _arduinoCommunication;
    private float _lastArduinoValueX;
    private float _lastArduinoValueY;
    private float _lastArduinoValueZ;
    private const float _selisih = 0.02f;

    private UDPDataReceiver _udpDataReceiver;

    public Rigidbody2D rb;
    public float jumpPower = 10;
    private int jumpLimit;
    private bool isDead = false;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform feet;
    [SerializeField] GameManager gameManager;

    private int jumpCount = 0;


    private void Awake() {
        jumpLimit = PlayerPrefs.GetInt(PLAYER_JUMP_COUNT);
        _udpDataReceiver = UDPDataReceiver.Instance;
        if(_udpDataReceiver == null) Debug.LogWarning("No UDPDataReceiver");
    }

    private void Start()
    {
        _arduinoCommunication = GetComponent<ArduinoCommunication>();

        if(_arduinoCommunication._isReady)
        {
            _lastArduinoValueX = _arduinoCommunication.ParseArduino('x');
            _lastArduinoValueY = _arduinoCommunication.ParseArduino('y');
            _lastArduinoValueZ = _arduinoCommunication.ParseArduino('z');
        }
    }

    private void FixedUpdate()
    {
        #region HandtrackingController
        int tempHandCount = _udpDataReceiver.handTrackingControlManager.GetHandCount();
        if(tempHandCount > 0)
        {
            if(tempHandCount >= 2)
            {
                Jump();
                
                Jump();

                return;
            }

            Jump();

            return;
        }
        #endregion
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gameManager.PauseGame();
        }

        if(Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            Jump();
            return;
        }


        #region  ArduinoController
        if(!_arduinoCommunication._isReady)
        {
            return;
        }
        
        float x = _arduinoCommunication.ParseArduino('x');
        float y = _arduinoCommunication.ParseArduino('y');
        float z = _arduinoCommunication.ParseArduino('z');

        

        if (
            (Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(_lastArduinoValueX)) >= _selisih) || 
            (Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(_lastArduinoValueY)) >= _selisih) || 
            (Mathf.Abs(Mathf.Abs(z) - Mathf.Abs(_lastArduinoValueZ)) >= _selisih)
           )
        {
            Jump();
            _lastArduinoValueX = x;
            _lastArduinoValueY = y;
            _lastArduinoValueZ = z;
        }
        #endregion   
    }

   
    private void Jump() 
    {
        if((jumpCount < jumpLimit) && Time.timeScale > 0 && !isDead) 
        {
            rb.velocity = Vector2.up * jumpPower;
            jumpCount++;
            GameManager.PlaySound(GameManager.Sound.Jump);
        }
    }

    public void ResetJump() {
        jumpCount = 0;
    }

    public void ActiveDeath() {
        isDead = true;
        _arduinoCommunication.CloseSerialPort();
    }
}
