using UnityEngine;
using TMPro;
public class PlanTargetUI : MonoBehaviour
{
    public TextMeshProUGUI statusText; 
    public UnityEngine.UI.Button planButton;
    
    
    void Start()
    {
        if(statusText == null)
            statusText = GetComponentInChildren<TextMeshProUGUI>();

        if(planButton == null)
            planButton = GetComponentInChildren<UnityEngine.UI.Button>();

        if(planButton != null)
            planButton.onClick.AddListener(OnPlanPressed); 
    }

    public void SetStatus(string message)
    {  
        if(statusText != null)
            statusText.text = message; 
        
    }
    public void OnPlanPressed()
    {
        SetStatus("Planning..."); 
    }

    public void ShowError(string error)
    {
        if(statusText != null)
            statusText.text = $"<color=red>{error}</color>";
    }


}
