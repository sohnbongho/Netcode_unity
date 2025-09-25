using LittelSword.InputSystem;
using LittelSword.Player;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private BasePlayer basePlayer;
    [SerializeField] private NetworkTransform networkTransform;

    // 컴포넌트 캐시
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private InputHandler inputHandler;
    private CinemachineCamera cmCamera;

    #region 네트워크 변수
    private NetworkVariable<bool> networkIsFacingRight = new NetworkVariable<bool>
        (
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    #endregion

    #region 유니티 이벤트
    private void Awake()
    {
        basePlayer = GetComponent<BasePlayer>();
        networkTransform = GetComponent<NetworkTransform>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputHandler = GetComponent<InputHandler>();
        cmCamera = FindFirstObjectByType<CinemachineCamera>();
    }
    private void Start()
    {
        if (IsOwner)
        {
            inputHandler.OnMove += HandleMove;
        }
    }

    private void HandleMove(Vector2 ctx)
    {
        // 현재 스프라이트의 방향 저장
        bool currentFacingRight = !spriteRenderer.flipX;

        // 방향이 변경 되었을 때 네트워크 변수를 업데이트
        if (networkIsFacingRight.Value != currentFacingRight)
        {
            networkIsFacingRight.Value = currentFacingRight;
        }
    }

    #endregion

    #region 네트워크 이벤트
    public override void OnNetworkSpawn()
    {
        // 네트워크 변수 이벤트 연결
        networkIsFacingRight.OnValueChanged += OnFacingRightChanged;

        if (IsOwner)
        {
            cmCamera.Follow = this.transform;
            inputHandler.enabled = true;
            basePlayer.enabled = true;
        }
        else
        {
            inputHandler.enabled = false;
            basePlayer.enabled = false;
        }
    }

    private void OnFacingRightChanged(bool previousValue, bool newValue)
    {
        if (!IsOwner)
        {
            spriteRenderer.flipX = !newValue;
        }
    }

    public override void OnNetworkDespawn()
    {

    }
    #endregion

}

