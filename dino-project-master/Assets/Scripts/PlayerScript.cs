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
        _arduinoCommunication = GetComponent<ArduinoCommunication>();
    }

    private void Start()
    {
        _lastArduinoValueX = _arduinoCommunication.ParseArduino('x');
        _lastArduinoValueY = _arduinoCommunication.ParseArduino('y');
        _lastArduinoValueZ = _arduinoCommunication.ParseArduino('z');
    }

    private void FixedUpdate() {
        if ((jumpCount < jumpLimit) && 
            ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))) && 
            Time.timeScale > 0 && 
            !isDead) {
                // if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) {
                //     return;
                // }
                Jump();
        }
        
        float x = _arduinoCommunication.ParseArduino('x');
        float y = _arduinoCommunication.ParseArduino('y');
        float z = _arduinoCommunication.ParseArduino('z');

        // StartCoroutine(jumpIEnumerator(y));

        if ((jumpCount < jumpLimit) && (
            (Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(_lastArduinoValueX)) >= _selisih) || 
            (Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(_lastArduinoValueY)) >= _selisih) || 
            (Mathf.Abs(Mathf.Abs(z) - Mathf.Abs(_lastArduinoValueZ)) >= _selisih))
        && Time.timeScale > 0 && !isDead) 
        {
            Jump();
            _lastArduinoValueX = x;
            _lastArduinoValueY = y;
            _lastArduinoValueZ = z;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            gameManager.PauseGame();
        }
    }

    // private IEnumerator jumpIEnumerator(float y)
    // {
    //     // Debug.Log("kontol");
    //     yield return new WaitForSeconds(0.5f);

    //     // Debug.Log("masih kontol");


    //     if ((jumpCount < jumpLimit) && Mathf.Abs(y - _lastArduinoValue) > 0.3f && Time.timeScale > 0 && !isDead) 
    //     {
    //         // Debug.Log("kontol lagi");

    //         Jump();
    //         _lastArduinoValue = y;
    //     }
    // }

    private void Jump() {
        rb.velocity = Vector2.up * jumpPower;
        jumpCount++;
        GameManager.PlaySound(GameManager.Sound.Jump);
    }

    public void ResetJump() {
        jumpCount = 0;
    }

    public void ActiveDeath() {
        isDead = true;
        _arduinoCommunication.CloseSerialPort();
    }
}
