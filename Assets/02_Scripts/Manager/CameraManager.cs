using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Input = Modoium.Service.Input;
public class CameraManager : Singleton<CameraManager>
{
    private Camera cam;

    [Header("virtual Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private Transform followTarget;

    [Header("Camera Range Data")]
    [SerializeField] private GameObject map; // 큐브로 구성된 맵
    [SerializeField] private BoxCollider boundaryCollider;

    [Header("Camera Data")]
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float zoomSpeed = 0.1f;

    [Header("Zoom Limit")]
    [SerializeField] private float minZoom;             // 최소 줌 크기
    [SerializeField] private float maxZoom;             // 최대 줌 크기

    [Header("Drag Speed Limit")]
    [SerializeField] private float minSpeed;             // 최소 드래그 속도
    [SerializeField] private float maxSpeed;             // 최대 드래그 속도

    public float buildButtonSize { get; private set; }

    protected override void Initialize()
    {
        // 초기화 코드 작성
        cam = Camera.main;
        buildButtonSize = 1;

        InitializeCamera();
    }

    private void LateUpdate()
    {
        if (UIManager.Instance.PopupBackground.IsPopupOpen)
            return;

        Drag();
        Zoom();
    }

    private void InitializeCamera()
    {
        //맵 전체 bounds 계산 (자식 큐브들의 Renderer bounds 합치기)
        Renderer[] renderers = map.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("맵에 큐브(Renderer)가 없습니다.");
            return;
        }

        Bounds mapBounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            mapBounds.Encapsulate(rend.bounds);
        }

        // 카메라, followTarget 위치 조정
        Vector3 center = mapBounds.center;
        cam.transform.position = new Vector3(center.x, center.y, cam.transform.position.z);
        followTarget.position = new Vector3(center.x, followTarget.position.y, center.z);

        // 카메라 범위 사이즈 조절
        if (boundaryCollider != null)
        {
            boundaryCollider.center = new Vector3(mapBounds.center.x, -30f, mapBounds.center.z);

            // BoxCollider는 Y 축 방향 크기도 필요하므로 약간의 기본값 유지 (ex. 10f)
            // Y축은 카메라 높이에 따라 무시되므로 적당한 값 (움직임은 XZ로 제한하니까
            boundaryCollider.size = new Vector3(mapBounds.size.x, 10f, mapBounds.size.z);
        }
    }

    private void Drag()
    {
        // 카메라 따라감
        virtualCam.transform.position = followTarget.transform.position;

        // 타워를 드래그중
        if (TowerManager.Instance.TileSelector.IsDragging)
        {
            followTarget.DOKill(); // 움직이고있으면 일단 중단시켜
            return;
        }


        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved && touch.deltaPosition.magnitude > 5f)
            {
                Vector2 delta = touch.deltaPosition;

                float timeScale = Time.timeScale;
                Vector3 move = new Vector3( -delta.x * dragSpeed * Time.unscaledDeltaTime, 0f, -delta.y * dragSpeed * Time.unscaledDeltaTime);

                Vector3 targetPos = followTarget.position + move;

                Bounds bounds = boundaryCollider.bounds;

                // Clamp 영역 안에서만 이동하도록 제한
                targetPos.x = Mathf.Clamp(targetPos.x, bounds.min.x, bounds.max.x);
                targetPos.z = Mathf.Clamp(targetPos.z, bounds.min.z, bounds.max.z);

                followTarget.DOKill(); // 기존 Tween 중단
                followTarget.DOMove(targetPos, 0.15f).SetEase(Ease.OutQuad).SetUpdate(true);
            }
        }
    }


    private void Zoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // 이전 프레임의 터치 위치 계산
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // 두 터치 간 거리 계산
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentTouchDeltaMag = (touch0.position - touch1.position).magnitude;

            // 거리 차이 계산
            float deltaMagnitudeDiff = prevTouchDeltaMag - currentTouchDeltaMag;
            // 현재 카메라 Y 위치
            float currentY = followTarget.transform.position.y;

            // Y 위치 조절 (delta에 음수 곱해서 pinch 시 가까워지고, 벌릴 때 멀어지게)
            float targetY = currentY + deltaMagnitudeDiff * zoomSpeed;
            targetY = Mathf.Clamp(targetY, minZoom, maxZoom);

            float t = Mathf.InverseLerp(minZoom, maxZoom, targetY);
            dragSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);
            buildButtonSize = Mathf.Lerp(-2.5f, -1, t) * -1;

            // 애니메이션 적용
            DOTween.Kill(followTarget.transform);
            DOTween.To(() => followTarget.transform.position.y,
                       y => {
                           Vector3 pos = followTarget.transform.position;
                           pos.y = y;
                           followTarget.transform.position = pos;
                       },
                       targetY, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
