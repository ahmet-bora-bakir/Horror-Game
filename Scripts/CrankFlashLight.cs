using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [SerializeField] private Light flashlightLight; 
    [SerializeField] private float drain= 10.0f;
    [SerializeField] private float recharge = 20.0f;
    [SerializeField] private float maxIntensity = 300f;
    
    private bool isOn = false;

    void Start()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = false;
    }

    void Update()
    {
        // Toggle light
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;
        }

        if (isOn)
        {
            // Natural drain over time (Time.deltaTime makes it smooth per second)
            if (flashlightLight.intensity > 0)
            {
                flashlightLight.intensity -= drain * Time.deltaTime;
            }

            // Manual recharge (Holding Right Click)
            if (Input.GetMouseButton(1) && flashlightLight.intensity < maxIntensity)
            {
                flashlightLight.intensity += recharge * Time.deltaTime;
            }
        }
    }
}
