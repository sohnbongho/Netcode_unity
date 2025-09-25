using LittelSword.InputSystem;
using LittelSword.Player;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private BasePlayer basePlayer;
        [SerializeField] private NetworkTransform networkTransform;

        // ������Ʈ ĳ��
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private InputHandler inputHandler;

        #region ����Ƽ �̺�Ʈ
        private void Awake()
        {
            basePlayer = GetComponent<BasePlayer>();
            networkTransform = GetComponent<NetworkTransform>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            inputHandler = GetComponent<InputHandler>();
        }
        #endregion

        #region ��Ʈ��ũ �̺�Ʈ
        public override void OnNetworkSpawn()
        {
            if(IsOwner)
            {
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
}

