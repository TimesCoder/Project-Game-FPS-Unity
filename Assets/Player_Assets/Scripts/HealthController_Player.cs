using UnityEngine;

public class HealthController_Player : MonoBehaviour
{
    public static HealthController_Player Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsDead() => GetComponent<HealthController>().IsDead();
}
