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

    // ������Ʈ ĳ��
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private InputHandler inputHandler;
    private CinemachineCamera cmCamera;

    #region ��Ʈ��ũ ����
    private NetworkVariable<bool> networkIsFacingRight = new NetworkVariable<bool>
        (
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    #endregion

    #region ����Ƽ �̺�Ʈ
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
        // ���� ��������Ʈ�� ���� ����
        bool currentFacingRight = !spriteRenderer.flipX;

        // ������ ���� �Ǿ��� �� ��Ʈ��ũ ������ ������Ʈ
        if (networkIsFacingRight.Value != currentFacingRight)
        {
            networkIsFacingRight.Value = currentFacingRight;
        }
    }

    #endregion

    #region ��Ʈ��ũ �̺�Ʈ
    public override void OnNetworkSpawn()
    {
        // ��Ʈ��ũ ���� �̺�Ʈ ����
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

