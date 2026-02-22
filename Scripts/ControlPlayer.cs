using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    public Transform playerCamera;
    private Rigidbody rb;
    private float xRotation = 0f;
    public CameraFlash camera_item;
    public FlashlightScript flashlight_item;

    public Animator CouchAnim;
    private CapsuleCollider player_collider;
    [Header("Kontrol değişkenleri")]
    public bool can_look = true;
    public bool can_sprint = true;
    public bool can_crouch = true;
    public bool is_grounded = false;
    public bool is_hitting_wall = false;
    [Header("Hız")]
    public float speed;
    public float normal_speed = 6f;
    public float sprint_speed = 10f;
    public float crouch_speed = 3f;
    public float stop_smoothness = 10f;
    public float mouse_sensitivity = 10000f;
    public float jump_force = 5.0f;
    [Header("Eğilme")]
    // bazı sorunlar oldu o yüzden daha eklemedim
    public float normal_height;
    public float crouch_height = 1.0f;
    public float crouch_transition = 10f;
    private Vector3 normal_center;
    // merdivenleri çıkmak için
    [Header("Merdiven Çıkma")]
    // çıkabileceği merdiven yüksekliği
    public float step_height = 0.4f;
    public float step_smoothness = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player_collider = GetComponent<CapsuleCollider>();
        normal_height = player_collider.height;
        normal_center = player_collider.center;
        // Mouse imlecini kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Kamerayı mouse ile ayarlama
        float mouse_X = Input.GetAxis("Mouse X") * mouse_sensitivity * Time.deltaTime;
        float mouse_Y = Input.GetAxis("Mouse Y") * mouse_sensitivity * Time.deltaTime;

        xRotation -= mouse_Y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Sol sağ haraket
        transform.Rotate(Vector3.up * mouse_X);

        //azıcık daha sorun var. ama temel olarak çalışıyor
        // oyuncu duvar benzeri yere çarpılıysa koşamayacak
        CheckForWalls();

        // hareket şekline göre hızı ayarlama 
        HandleSpeed();
        // Zıplama
        is_grounded = Physics.Raycast(transform.position, Vector3.down, normal_height + 0.1f);
        if (Input.GetButtonDown("Jump") && is_grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jump_force, ForceMode.VelocityChange);
        }

        HandleItemSwitch();
    }
    void Crouch()
    {
        /**/
    }

    void Stand()
    {
        player_collider.height = normal_height;

        player_collider.center = normal_center;
    }

    void CheckForWalls()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * (step_height + 0.1f);
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, 0.7f))
        {
            is_hitting_wall = true;
        }
        else
        {
            is_hitting_wall = false;
        }
    }
    void HandleSpeed()
    {
        // eğer oyuncu sol shift ve w ya basarsa ve oyuncu koşabilir durumdysa(eğilmiyorsa vs.)
        // oyuncu kşma hızına geçsin + eğilemesin
        if (Input.GetKey(KeyCode.LeftShift) && can_sprint && Input.GetKey(KeyCode.W) && !is_hitting_wall)
        {
            speed = sprint_speed;
            can_crouch = false;
        }
        // aynısı ama c tuşuna basıp eğilebiliyorsa
        // oyuncu yavaşlasın ve eğilsin + koşamasın
        else if (Input.GetKey(KeyCode.C) && can_crouch)
        {
            speed = crouch_speed;
            can_sprint = false;
            Crouch();
        }
        // normal hızını tutsun
        else
        {
            speed = normal_speed;
            can_crouch = true;
            can_sprint = true;
            Stand();
        }
    }

    // karakterin hareketini hesaplama ve hareketi durduğunda daha akıcı bir yavaşlama 
    void FixedUpdate()
    {
        float move_vertical = Input.GetAxisRaw("Vertical");
        float move_horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 targetDirection = (transform.forward * move_vertical) + (transform.right * move_horizontal);
        Vector3 targetVelocity = targetDirection * speed;
        
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = Vector3.Lerp(currentVelocity, new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z), stop_smoothness * Time.fixedDeltaTime);
        
        rb.linearVelocity = velocityChange;

        // karakter merdiven çıkacağı zaman
        StepClimb();
    }

    void StepClimb()
    {
        Vector3 moveDir = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;
        if (moveDir.magnitude < 0.1f) return;

        if (Physics.Raycast(transform.position + Vector3.up * 0.05f, moveDir, out RaycastHit hitLower, 0.6f))
        {
            if (!Physics.Raycast(transform.position + Vector3.up * step_height, moveDir, 0.7f))
            {
                rb.MovePosition(rb.position + new Vector3(0, step_smoothness * Time.fixedDeltaTime, 0));
                rb.linearVelocity += Vector3.up * 0.5f;
            }
        }
    }
    void HandleItemSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipCamera();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipFlashlight();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UnequipAll();
        }
    }

    void EquipCamera()
    {
        camera_item.gameObject.SetActive(true);
        flashlight_item.gameObject.SetActive(false);
    }

    void EquipFlashlight()
    {
        flashlight_item.gameObject.SetActive(true);
        camera_item.gameObject.SetActive(false);
    }

    void UnequipAll()
    {
        camera_item.gameObject.SetActive(false);
        flashlight_item.gameObject.SetActive(false);
    }
}
