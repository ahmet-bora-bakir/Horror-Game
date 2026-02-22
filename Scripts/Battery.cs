using UnityEngine;
// kamera için batarya toplama
public class Battery : MonoBehaviour
{
    public CameraFlash cameraFlash;
    private bool is_over_battery = false;

    void Update()
    {
        // oyuncu batarya alanı içindeyken. e ye basarsa çalışır
        if (is_over_battery && Input.GetKeyDown(KeyCode.E))
        {
            CollectBattery();
        }
    }

    void CollectBattery()
    {
        // oyuncu flaş sayısı yenilenir ve batarya kaybolur
        cameraFlash.flash_amount = cameraFlash.flash_amount_per_battery;

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // oyuncunun bataryanın tetik alanına girip girmediğini kontrol eder
        if (other.CompareTag("Player"))
        {
            is_over_battery = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            is_over_battery = false;
        }
    }
}