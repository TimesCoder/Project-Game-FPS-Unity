using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private int currentCharacter;

    private void Awake()
    {
        Debug.Log($"Jumlah karakter: {transform.childCount}");

        // Selalu mulai dari karakter index 0
        currentCharacter = 0;
        SelectCharacter(currentCharacter);
    }

    private void SelectCharacter(int index)
    {
        previousButton.interactable = (index > 0);
        nextButton.interactable = (index < transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(i == index);

        currentCharacter = index;

        // Simpan karakter yang dipilih
        PlayerPrefs.SetInt("SelectedCharacter", currentCharacter);
        PlayerPrefs.Save();

        Debug.Log($"Karakter aktif disimpan: {currentCharacter}");
    }

    public void ChangeCharacter(int change)
    {
        currentCharacter += change;
        currentCharacter = Mathf.Clamp(currentCharacter, 0, transform.childCount - 1);
        SelectCharacter(currentCharacter);
    }

    public void SelectCharacterByButton(int index)
    {
        currentCharacter = Mathf.Clamp(index, 0, transform.childCount - 1);
        SelectCharacter(currentCharacter);
    }
}
