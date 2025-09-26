using LittelSword.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionUIView : MonoBehaviour
{
    // UI¿¬°á
    [SerializeField] private TMP_InputField sessionNameInput;
    [SerializeField] private TMP_InputField sessionCodeInput;
    [SerializeField] private Button createdSessionButton;
    [SerializeField] private Button quickJoinSessionButton;
    [SerializeField] private Button startSessionButton;

    private MultiplayerSessionManager MPSessionManager => MultiplayerSessionManager.Instance;

    private void OnEnable()
    {
        MPSessionManager.OnUpdateSessionInfo += UpdateSessionInfo;
        createdSessionButton.onClick.AddListener(
            () => MPSessionManager.CreateSessionAsync(sessionNameInput.text));

        quickJoinSessionButton.onClick.AddListener(
            () => MPSessionManager.QuickJoinSessionAsync());

        startSessionButton.onClick.AddListener(
            () => MPSessionManager.StartSession());
    }

    private void OnDisable()
    {
        MPSessionManager.OnUpdateSessionInfo -= UpdateSessionInfo;
        createdSessionButton.onClick.RemoveAllListeners();
        quickJoinSessionButton.onClick.RemoveAllListeners();
        startSessionButton.onClick.RemoveAllListeners();
    }

    private void UpdateSessionInfo(string name, string code)
    {
        sessionNameInput.text = name;
        sessionCodeInput.text = code;
    }

}
