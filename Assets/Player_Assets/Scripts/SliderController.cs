using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SliderController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private Text sliderText;

    [Header("Pengaturan")]
    [SerializeField] private float maxSliderAmount = 100.0f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float delayBeforeLoad = 2f;

    private bool sceneLoaded = false;

    private void Start()
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
        }

        // Mulai mainkan loading BGM
        AudioManager.Instance.PlayLoadingBGM();
    }

    private void Update()
    {
        if (slider == null || sceneLoaded) return;

        slider.value += (Time.deltaTime * speed) / maxSliderAmount;
        float localValue = slider.value * maxSliderAmount;
        sliderText.text = localValue.ToString("0.0");

        if (localValue >= 100f)
        {
            sceneLoaded = true;
            StartCoroutine(LoadSceneWithDelay());
        }
    }

    IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene("Main Menu");
    }
}