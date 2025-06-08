using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance; // Singleton pattern
    public PlayerInventory inventory;

    void Awake()
    {
        instance = this; // Assign instance saat game dimulai
    }
}