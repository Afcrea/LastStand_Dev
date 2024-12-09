using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandVisibilityController : MonoBehaviour
{
    // ���� ���ͷ���
    private XRRayInteractor rayInteractor;
    
    // �ڵ� ��
    public GameObject handModel;

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void Start()
    {
        rayInteractor.selectExited.AddListener(ActiveHandModel);
        rayInteractor.selectEntered.AddListener(DeActiveHandModel);
    }


    // ������Ʈ�� ������ �� �ڵ�� Ȱ��ȭ
    private void ActiveHandModel(SelectExitEventArgs args)
    {
        handModel?.SetActive(true);
    }

    // ������Ʈ�� ����� �� �ڵ�� ��Ȱ��ȭ
    private void DeActiveHandModel(SelectEnterEventArgs args)
    {
        handModel?.SetActive(false);
    }
}
