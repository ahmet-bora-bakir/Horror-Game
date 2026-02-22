using UnityEngine;

public class Drink : MonoBehaviour
{
    public ControlPlayer Player;
    private bool is_over_drink = false;

    void Update()
    {
        // oyuncu içecek alanı içindeyken. e ye basarsa çalışır
        if (is_over_drink && Input.GetKeyDown(KeyCode.E))
        {
            CollectDrink();
        }
    }

    void CollectDrink()
    {
        // oyuncunun herhangi bir özelliği artar(şimdilik hız)
        Player.normal_speed += 3f;

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // oyuncunun içeceğin tetik alanına girip girmediğini kontrol eder
        if (other.CompareTag("Player"))
        {
            is_over_drink = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            is_over_drink = false;
        }
    }
}
