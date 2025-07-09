using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClick : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayClick();
            }
        });
    }
}
