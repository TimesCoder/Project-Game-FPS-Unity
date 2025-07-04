using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject player0; // Player biasa
    public GameObject player1; // Player alternatif

    void Start()
    {
        int selected = PlayerPrefs.GetInt("SelectedCharacter", 0);

        if (selected == 0)
        {
            player0.SetActive(true);
            player1.SetActive(false);
        }
        else if (selected == 1)
        {
            player0.SetActive(false);
            player1.SetActive(true);
        }
        else
        {
            // Default fallback (kalau datanya rusak)
            player0.SetActive(true);
            player1.SetActive(false);
        }

        Debug.Log("Karakter aktif: " + selected);
    }
}
