using UnityEngine;
using System.Collections;

public class CameraFlash : MonoBehaviour
{
    public Light flashLight;
    public Light BatteryLight;
    public float max_intensity = 1000f;
    public float flash_duration = 0.2f;
    public float cooldown = 1.0f;
    private bool in_cooldown = false;
    public int flash_amount_per_battery = 10;
    public int flash_amount;
    public AnimationCurve intensity_curve = AnimationCurve.Linear(0, 1, 1, 0);

    void Start()
    {
        // flaş sayısını max flaş sayısını ayarla ve ışıkları kapat
        flash_amount = flash_amount_per_battery;
        if (flashLight != null)
        {
            flashLight.gameObject.SetActive(false);
            BatteryLight.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && flashLight != null && flash_amount > 0 && !in_cooldown)
        {
            // flaş patlat
            StopAllCoroutines();
            StartCoroutine(ToggleLightsDelayed(0.3f));
            StartCoroutine(WorldFlash());
        }
        if (flash_amount == 0)
        {
            // eğer flaş hakkı yoksa batarya ışığı yansın
            BatteryLight.gameObject.SetActive(true);
        }
        else
        {
            // flaş hakkını geri kazanırsa tekrar ışık sönsün
            BatteryLight.gameObject.SetActive(false);
        }
    }
    // kodu beklet
    private IEnumerator ToggleLightsDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    IEnumerator WorldFlash()
    {
        // flaşı çalıştır ve oyuncuyu cooldowna sok
        in_cooldown = true;
        flashLight.gameObject.SetActive(true);
        
        float elapsed = 0f;
        while (elapsed < flash_duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / flash_duration;

            flashLight.intensity = max_intensity * intensity_curve.Evaluate(percent);
            yield return null;
        }
        // flaş hakkını azalt
        flash_amount--;
        flashLight.gameObject.SetActive(false);

        yield return new WaitForSeconds(cooldown - flash_duration);
        in_cooldown = false;
    }
}