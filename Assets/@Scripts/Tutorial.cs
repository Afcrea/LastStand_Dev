using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    public GameObject leftMenuPanel;
    public GameObject leftTriggerPanel;
    public GameObject leftPrimaryPanel;
    public GameObject rightTriggerPanel;

    public InputAction leftMenuAction;
    public InputAction leftTriggerAction;
    public InputAction leftPrimaryAction;
    public InputAction rightTriggerAction;

    private Color originalColor = Color.black;
    private Color activeColor = Color.white; // 버튼을 누를 때 활성화 색상

    private void OnEnable()
    {
        // InputAction 활성화
        leftMenuAction.Enable();
        leftTriggerAction.Enable();
        leftPrimaryAction.Enable();
        rightTriggerAction.Enable();
    }

    private void OnDisable()
    {
        // InputAction 비활성화
        leftMenuAction.Disable();
        leftTriggerAction.Disable();
        leftPrimaryAction.Disable();
        rightTriggerAction.Disable();
    }

    void Update()
    {
        // 각 InputAction에서 값을 받아와서 패널 색상 토글
        TogglePanelColor(leftMenuPanel, leftMenuAction.IsPressed());
        TogglePanelColor(leftTriggerPanel, leftTriggerAction.IsPressed());
        TogglePanelColor(leftPrimaryPanel, leftPrimaryAction.IsPressed());
        TogglePanelColor(rightTriggerPanel, rightTriggerAction.IsPressed());
    }

    void TogglePanelColor(GameObject panel, bool isPressed)
    {
        if (panel.TryGetComponent<Image>(out Image panelImage))
        {
            // 버튼이 눌린 상태일 때 활성화 색상, 그렇지 않을 때 원래 색상으로 설정
            panelImage.color = isPressed ? activeColor : originalColor;
        }
    }
}
