using UnityEngine;
using System.Collections;
using System.IO;

public class CameraFlash : MonoBehaviour
{
    public RenderTexture cameraView;
    public Light flashLight;
    public GameObject battery_ui;
    public Camera screen;
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
            screen.gameObject.SetActive(false);
            battery_ui.gameObject.SetActive(true);
        }
        else
        {
            // flaş hakkını geri kazanırsa tekrar ışık sönsün
            BatteryLight.gameObject.SetActive(false);
            battery_ui.gameObject.SetActive(false);
            screen.gameObject.SetActive(true);
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
        bool taken_shot = false;
        in_cooldown = true;
        flashLight.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < flash_duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / flash_duration;

            flashLight.intensity = max_intensity * intensity_curve.Evaluate(percent);
            if(flashLight.intensity >= max_intensity && !taken_shot)
            {
                TakeSnapshot();
                taken_shot = true;
            }
            yield return null;
        }
        // flaş hakkını azalt
        flash_amount--;
        flashLight.gameObject.SetActive(false);

        yield return new WaitForSeconds(cooldown - flash_duration);
        in_cooldown = false;
    }
    
    public void TakeSnapshot()
    {
    screen.Render(); 

    RenderTexture.active = cameraView;
    Texture2D screenshot = new Texture2D(cameraView.width, cameraView.height, TextureFormat.RGB24, false);
    screenshot.ReadPixels(new Rect(0, 0, cameraView.width, cameraView.height), 0, 0);

    RenderTexture.active = cameraView;

    screenshot.ReadPixels(new Rect(0, 0, cameraView.width, cameraView.height), 0, 0);
    screenshot.Apply();

    RenderTexture.active = null;

    byte[] bytes = screenshot.EncodeToPNG();

    string folderPath = Path.Combine(Application.persistentDataPath, "Screenshots");
    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
    
    string fileName = "Snap_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
    string filePath = Path.Combine(folderPath, fileName);

    File.WriteAllBytes(filePath, bytes);

    Debug.Log("Screenshot saved to: " + filePath);
    
    Destroy(screenshot);
    }
}
