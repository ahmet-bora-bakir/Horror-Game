using UnityEngine;

public class CameraFOVEffects : MonoBehaviour
{
    public Camera playerCamera;
    public ControlPlayer Player;
    
    //babuş header öğrendi
    [Header("FOV Settings")]
    // normal hareket esnasında ya da idle duruken fov
    public float normal_FOV = 60f;
    // koşarken fov
    public float sprint_FOV = 80f;
    // normalden koşma fov sine geçme hızı
    public float fov_transition = 5f;
    // kameranın aşağı yukarı hareket etmesini sağlıyor
    [Header("Kamera 'Bob' efekti")]
    // yürürken kafa hareket hızı
    public float walk_bob_speed = 10f;
    // kafanın tepe noktası
    public float walk_bob_amount = 0.0012f;
    // koşarken " "
    public float sprint_bob_speed = 15f;
    public float sprint_bob_amount = 0.001f;
    // eğilirken " "
    public float crouch_bob_speed = 6f;
    public float crouch_bob_amount = 0.0008f;

    [Header("Kameranın pozisyonu Eğilirken")]
    // eğilmeden önce
    public float normal_height = 1.4f;
    // eğildikten sonra
    public float crouch_height = 0f;
    // geçiş hızı
    public float crouch_transition_speed = 8f;

    private float default_Pos_Y = 0;
    private float timer = 0;

    void Start()
    {
        default_Pos_Y = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        HandleFOV();
        HandleBobbingAndCrouch();
    }

    void HandleFOV()
    {
        float target_FOV = normal_FOV;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && Player.can_sprint && Player.is_hitting_wall == false)
            target_FOV = sprint_FOV;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, target_FOV, Time.deltaTime * fov_transition);
    }

    void HandleBobbingAndCrouch()
    {
        float speed = 0;
        float amount = 0;
        float target_height = normal_height;

        // Oyuncu koşuyor mu eğiliyor mu ona göre kamera bobbing yapsın. Havadayken yapmasın garip görünüyor
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && Player.can_sprint && Player.is_grounded && !Player.is_hitting_wall)
        {
            speed = sprint_bob_speed;
            amount = sprint_bob_amount;
        }
        else if (Input.GetKey(KeyCode.C) && Player.can_crouch && Player.is_grounded)
        {
            speed = crouch_bob_speed;
            amount = crouch_bob_amount;
            // oyuncu eğiliyorsa kemarayı aşağı çek
            target_height = crouch_height;
        }
        else
        {
            speed = walk_bob_speed;
            amount = walk_bob_amount;
        }

        // Kameranın yerini güncelle
        Vector3 Camera_new_pos = playerCamera.transform.localPosition;
        Camera_new_pos.y = Mathf.Lerp(Camera_new_pos.y, target_height, Time.deltaTime * crouch_transition_speed);

        // Bobbing(kamerayı sinüs fonksiyonu şeklinde bulunduğu yerden yukarı aşağı yap)
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            timer += Time.deltaTime * speed;
            Camera_new_pos.y += Mathf.Sin(timer) * amount;
        }
        else
        {
            timer = 0;
        }

        playerCamera.transform.localPosition = Camera_new_pos;
    }
}