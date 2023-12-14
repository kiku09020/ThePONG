using Cysharp.Threading.Tasks;
using GameController.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Network {
    public class NetworkConnectionPresenter : MonoBehaviour {
        /* Fields */
        [Header("GameStart")]
        [SerializeField] Button gameStartButton;
        [SerializeField] TextMeshProUGUI idText;

        [Header("Connecting")]
        [SerializeField] TextMeshProUGUI connectingText;
        [SerializeField] TextMeshProUGUI playerCountText;
        [SerializeField] Button disconnectButton;

        [Header("Dialog")]
        [SerializeField] RectTransform dialogUI;
        [SerializeField] TextMeshProUGUI dialogMessageText;

        [Header("Messages")]
        [SerializeField] string connectingMessage = "�ڑ����c";
        [SerializeField] string waitingMessage = "�v���C���[��ҋ@���c";
        [SerializeField] string deletedMessage = "�z�X�g�ɂ���āA���r�[���폜����܂����c�B";
        [SerializeField] string matchingMessage = "�v���C���[���W�܂�܂����I";

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        async void Start()
        {
            ResetConnectionUI();

            // �C�x���g�Z�b�g
            SetUIEvents();
            AddNetworkEvents();

            // �F�؂����܂őҋ@
            await UniTask.WaitUntil(() => AuthenticationService.Instance.IsAuthorized, cancellationToken: this.GetCancellationTokenOnDestroy());

            // PlayerID
            idText.text = AuthenticationService.Instance.PlayerId.ToString();
        }

        //-------------------------------------------------------------------
        /* Methods */
        void SetUIEvents()
        {
            // �J�n�{�^��(�쐬�E�Q��)
            gameStartButton.onClick.AddListener(async () => {
                UIManager.ShowUIGroup<TitleConnectingUI>();
                await NetworkController.Instance.CreateOrJoin();
            });

            // �ؒf�{�^��(�T�[�o�[�ؒf�E���r�[�폜)
            disconnectButton.onClick.AddListener(async () => {
                UIManager.ShowUIGroup<TitleMainUIGroup>();
                await NetworkController.Instance.DisconnectFromServer();
            });
        }

        // �l�b�g���[�N�C�x���g�̒ʒm���󂯎��
        void AddNetworkEvents()
        {
            // �v���C���[�ڑ�
            NetworkController.Instance.OnConnected += () => {
                disconnectButton.interactable = true;
                connectingText.text = waitingMessage;
            };

            // �v���C���[�Q��
            NetworkController.Instance.OnClientJoined += (lobby) => {
                SetLobbyPlayersCount(lobby);
            };

            // �v���C���[�ގ�
            NetworkController.Instance.OnPlayerLeft += (lobby) => {
                SetLobbyPlayersCount(lobby);
            };

            // �z�X�g�����r�[�폜
            NetworkController.Instance.OnDeletedLobbyByHost += () => {
                SetDeletedByHostDialog();
            };

            // �T�[�o�[�ؒf
            NetworkController.Instance.OnDisconnectFromServer += () => {
                ResetConnectionUI();
                connectingText.text = connectingMessage;
            };

            // �}�b�`���O����
            NetworkController.Instance.OnMathcing += () => {
                SetMatchingUI();
            };
        }

        //-------------------------------------------------------------------
        // �v���C���[���̃e�L�X�g�̕ύX
        void SetLobbyPlayersCount(Lobby joinedLobby)
        {
            playerCountText.text = $"( {joinedLobby.Players.Count} / {joinedLobby.MaxPlayers} )";
        }

        void ResetConnectionUI()
        {
            disconnectButton.interactable = false;
            playerCountText.text = "";
        }

        //--------------------------------------------------
        // ���r�[�폜�_�C�A���O
        void SetDeletedByHostDialog()
        {
            dialogMessageText.text = deletedMessage;
            dialogUI.gameObject.SetActive(true);
            UIManager.ShowUIGroup<TitleMainUIGroup>();
        }

        // �}�b�`���O������UI
        void SetMatchingUI()
        {
            connectingText.text = matchingMessage;
            disconnectButton.interactable = false;
        }
    }
}