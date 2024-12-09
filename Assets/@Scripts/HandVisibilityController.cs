using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandVisibilityController : MonoBehaviour
{
    // 레이 인터렉터
    private XRRayInteractor rayInteractor;
    
    // 핸드 모델
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


    // 오브젝트를 놓았을 때 핸드모델 활성화
    private void ActiveHandModel(SelectExitEventArgs args)
    {
        handModel?.SetActive(true);
    }

    // 오브젝트를 잡았을 때 핸드모델 비활성화
    private void DeActiveHandModel(SelectEnterEventArgs args)
    {
        handModel?.SetActive(false);
    }
}
