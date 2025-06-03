using UnityEngine;

public class ObjectToHit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float ObjectHealt = 100f;

    public void ObjectHitDamage(float amount)
    {
        ObjectHealt -= amount;
        if (ObjectHealt <= 0f)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
