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
    #endregion

    #region 네트워크 이벤트
    public override void OnNetworkSpawn()
    {
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
    public override void OnNetworkDespawn()
    {

    }
    #endregion

}

