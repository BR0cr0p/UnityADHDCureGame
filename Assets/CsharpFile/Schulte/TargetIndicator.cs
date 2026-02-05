using UnityEngine;
using TMPro;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private SchulteController controller;

    void Start()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();
        controller = FindObjectOfType<SchulteController>();
        if (controller != null)
        {
            controller.OnTargetChanged += UpdateText;
            UpdateText(controller.currentTarget);
        }

    }


    void UpdateText(int target)
    {
        if (text != null)
        {
            text.text = $"Next number: {target}";
        }
    }

    void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnTargetChanged -= UpdateText;
        }
    }

    // ÷ÿ÷√∑Ω∑®
    public void ResetIndicator()
    {
        Debug.Log("ResetIndicator called on: " + gameObject.name);

        if (text != null)
            text.text = "Next number: 1";

        // “˛≤ÿ Canvas
        gameObject.SetActive(false);
    }

}
