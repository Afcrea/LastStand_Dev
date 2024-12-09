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
    private Color activeColor = Color.white; // ��ư�� ���� �� Ȱ��ȭ ����

    private void OnEnable()
    {
        // InputAction Ȱ��ȭ
        leftMenuAction.Enable();
        leftTriggerAction.Enable();
        leftPrimaryAction.Enable();
        rightTriggerAction.Enable();
    }

    private void OnDisable()
    {
        // InputAction ��Ȱ��ȭ
        leftMenuAction.Disable();
        leftTriggerAction.Disable();
        leftPrimaryAction.Disable();
        rightTriggerAction.Disable();
    }

    void Update()
    {
        // �� InputAction���� ���� �޾ƿͼ� �г� ���� ���
        TogglePanelColor(leftMenuPanel, leftMenuAction.IsPressed());
        TogglePanelColor(leftTriggerPanel, leftTriggerAction.IsPressed());
        TogglePanelColor(leftPrimaryPanel, leftPrimaryAction.IsPressed());
        TogglePanelColor(rightTriggerPanel, rightTriggerAction.IsPressed());
    }

    void TogglePanelColor(GameObject panel, bool isPressed)
    {
        if (panel.TryGetComponent<Image>(out Image panelImage))
        {
            // ��ư�� ���� ������ �� Ȱ��ȭ ����, �׷��� ���� �� ���� �������� ����
            panelImage.color = isPressed ? activeColor : originalColor;
        }
    }
}
