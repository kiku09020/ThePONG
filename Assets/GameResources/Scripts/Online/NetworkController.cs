using Cysharp.Threading.Tasks;
using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Network {
    public class NetworkController : Singleton<NetworkController> {
        /* Fields */
        [SerializeField] bool isTranstionScene;
        [SerializeField, Tooltip("�J�ڂ܂ł̑ҋ@����")] float transitoinWaitDuration = 3;

        //-------------------------------------------------------------------
        /* Properties */
        /// <summary> �N���C�A���g�ڑ��� </summary>
        public event System.Action<Lobby> OnClientJoined;

        /// <summary> ���r�[�ގ��� </summary>
        public event System.Action<Lobby> OnPlayerLeft;

        /// <summary> �v���C���[���g���T�[�o�[�ڑ��� </summary>
        public event System.Action OnConnected;

        /// <summary> �v���C���[���g���T�[�o�[�ؒf�� </summary>
        public event System.Action OnDisconnectFromServer;

        /// <summary> �z�X�g�����r�[�폜������ </summary>
        public event System.Action OnDeletedLobbyByHost;

        /// <summary> �}�b�`���O������ </summary>
        public event System.Action OnMathcing;

        //-------------------------------------------------------------------
        /* Messages */

        protected override async void Awake()
        {
            base.Awake();

            // �T�[�r�X������
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () => {
                print($"{AuthenticationService.Instance.PlayerId} was signedin.");
            };

#if UNITY_EDITOR
            // ���Z�b�g(MPPM�p)
            AuthenticationService.Instance.ClearSessionToken();
#endif

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            SetEvents();
        }

        private void FixedUpdate()
        {
            // �n�[�g�r�[�g���M
            NetworkLobbyManager.Instance.SendLobbyHeartBeat();
        }

        //-------------------------------------------------------------------
        // �T�[�o�[�ڑ�����ؒf���Ȃǂ̃C�x���g�B���������ɌĂяo��
        void SetEvents()
        {
            /* ����C�x���g */
            // ���샍�r�[�Q���C�x���g�Ƀ����[�T�[�o�[�Q��������ǉ�(���r�[�ɎQ�������N���C�A���g���g�̂�)
            NetworkLobbyManager.Instance.OnJoinedLobby += async (joinCode) => await NetworkRelayManager.Instance.JoinRelay(joinCode);

            /* ���r�[�C�x���g */


            // ���r�[�Q���C�x���g(���r�[���̃v���C���[�ɂ̂ݒʒm)
            NetworkLobbyManager.Instance.EventCallbacks.PlayerJoined += (players) => {
                print("�v���C���[���Q��������wwww");
            };

            // ���r�[���v���C���[�ގ���(���r�[���̃v���C���[�ɂ̂ݒʒm)
            NetworkLobbyManager.Instance.EventCallbacks.PlayerLeft += async (players) => {
                var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();
                OnPlayerLeft?.Invoke(joinedLobby);

                print("�v���C���[�����E������wwww");
            };

            // ���r�[�폜��(���r�[�ɎQ�����Ă����v���C���[�S���ɒʒm)
            NetworkLobbyManager.Instance.EventCallbacks.LobbyDeleted += () => {
                print("�폜�C�x���g���s�`");

                // ��ɐؒf���ꂿ����āA�z�X�g���ǂ�������ł��Ȃ�
                // �z�X�g�F�ؒf �� �폜
                // �N���C�A���g�F�폜 �� �ؒf
                //if (!NetworkManager.Singleton.IsHost) {
                //	print("�z�X�g���폜���܂���");
                //}
            };

            /* �T�[�o�[�C�x���g */
            // �ڑ��J�n��
            NetworkManager.Singleton.OnClientStarted += () => {

                print("�쐬����܂����`�`�`�`�`");
            };

            // �T�[�o�[�ڑ���
            // (LobbyEventCallbacks.PlayerJoined���ƁA���g���܂܂�Ȃ����߂���g���Ă�B)
            NetworkManager.Singleton.OnClientConnectedCallback += async (playerID) => {
                var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();

                OnClientJoined?.Invoke(joinedLobby);

                MathcingProcess(joinedLobby);

                print("�ڑ��C�x���g���s�`");
            };

            // �T�[�o�[�ؒf��
            NetworkManager.Singleton.OnClientDisconnectCallback += async (count) => {
                print("�ؒf�C�x���g���s�`");

                // �z�X�g����\
                if (!NetworkManager.Singleton.IsHost) {
                    print("�z�X�g���ؒf�����������`�`�`");
                    OnDeletedLobbyByHost?.Invoke();

                    await DisconnectFromServer(false);
                }
            };
        }

        //-------------------------------------------------------------------
        /* Methods */
        public async Task CreateOrJoin()
        {
            // �A�N�e�B�u�ȃ��r�[������
            var result = await NetworkLobbyManager.Instance.SearchLobbies();
            Lobby lobby = null;

            // ���r�[��1���Ȃ���΁A���r�[�쐬
            if (result.Count == 0) {
                string joinCode = await NetworkRelayManager.Instance.CreateRelay();     // �����[�T�[�o�[�쐬
                lobby = await NetworkLobbyManager.Instance.CreateLobby(joinCode);       // ���r�[�쐬

            }

            // ���r�[������ΎQ��
            else {
                // ��ԌÂ����r�[�ɎQ����ɁA�����[�Q��
                lobby = await NetworkLobbyManager.Instance.JoinLobby(result.First().Id);
            }

            OnConnected?.Invoke();
        }

        //-------------------------------------------------------------------
        /// <summary> �T�[�o�[����ؒf </summary>
        public async Task DisconnectFromServer(bool leftFromLobby = true)
        {
            // ���r�[�ގ�����
            await NetworkLobbyManager.Instance.LeftFromLobby(leftFromLobby);

            // ����
            NetworkManager.Singleton.Shutdown();

            OnDisconnectFromServer?.Invoke();
            print("�T�[�o�[�̐ڑ���ؒf���܂���");
        }

        //-------------------------------------------------------------------
        // �W�܂����Ƃ��̏���
        async void MathcingProcess(Lobby lobby)
        {
            if (lobby.AvailableSlots == 0) {
                print("�����o�[���W�܂�܂���");
                OnMathcing?.Invoke();

                // �V�[���J��
                if (isTranstionScene && NetworkManager.Singleton.IsHost) {
                    await UniTask.Delay(System.TimeSpan.FromSeconds(transitoinWaitDuration), cancellationToken: destroyCancellationToken);            // �ҋ@
                    NetworkManager.Singleton.SceneManager.LoadScene("OnlineMultiScene", UnityEngine.SceneManagement.LoadSceneMode.Single);      // �J��
                }
            }
        }
    }
}