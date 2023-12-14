using DesignPatterns.Singleton;
using GameController.UI;
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
		[SerializeField] NetworkConnectionPresenter presenter;
		[SerializeField] bool isTranstionScene;

		//-------------------------------------------------------------------
		/* Properties */
		public event System.Action<Lobby> OnClientJoined;
		public event System.Action<Lobby> OnClientLeft;

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
			// ���샍�r�[�Q���C�x���g�Ƀ����[�T�[�o�[�Q��������ǉ�(���r�[�ɎQ�������N���C�A���g���g�̂�)
			NetworkLobbyManager.Instance.OnJoinedLobby += async (joinCode) => await NetworkRelayManager.Instance.JoinRelay(joinCode);

			// ���r�[�Q���C�x���g
			NetworkLobbyManager.Instance.EventCallbacks.PlayerJoined += (players) => {
				print("�v���C���[���Q��������wwww");
			};

			// ���r�[���v���C���[���E��
			NetworkLobbyManager.Instance.EventCallbacks.PlayerLeft += async (players) => {
				await presenter.SetLobbyPlayersCount();
			};

			// ���r�[�폜��
			NetworkLobbyManager.Instance.EventCallbacks.LobbyDeleted += () => {
				print("�폜�C�x���g���s�`");

				// ��ɐؒf���ꂿ����āA�z�X�g���ǂ�������ł��Ȃ�
				if (!NetworkManager.Singleton.IsHost) {
					print("�z�X�g���폜���܂���");
				}
			};

			// �T�[�o�[�ڑ���
			// (LobbyEventCallbacks.PlayerJoined���ƁA���g���܂܂�Ȃ����߂���g���Ă�B)
			NetworkManager.Singleton.OnClientConnectedCallback += async (playerID) => {
				var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();

				presenter.SetLobbyPlayersCount(joinedLobby);
				OnMathcing(joinedLobby);

				print("�ڑ��C�x���g���s�`");
			};

			// �T�[�o�[�ؒf��
			NetworkManager.Singleton.OnClientDisconnectCallback += async (count) => {
				print("�ؒf�C�x���g���s�`");

				// �z�X�g����\
				if (!NetworkManager.Singleton.IsHost) {
					print("�z�X�g���ؒf�����������`�`�`");
					UIManager.ShowUIGroup<TitleMainUIGroup>();
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
				lobby = await NetworkLobbyManager.Instance.CreateLobby(joinCode);               // ���r�[�쐬
			}

			// ���r�[������ΎQ��
			else {
				// ��ԌÂ����r�[�ɎQ����ɁA�����[�Q��
				lobby = await NetworkLobbyManager.Instance.JoinLobby(result.First().Id);
			}
		}

		//-------------------------------------------------------------------
		/// <summary> �T�[�o�[����ؒf </summary>
		public async Task DisconnectFromServer(bool leftFromLobby=true)
		{
			// ���r�[�ގ�����
			if (leftFromLobby) {
				await NetworkLobbyManager.Instance.LeftFromLobby();
			}

			// ����
			NetworkManager.Singleton.Shutdown();

			presenter.ResetLobbyPlayersCount();
			print("�T�[�o�[�̐ڑ���ؒf���܂���");
		}

		//-------------------------------------------------------------------

		void OnMathcing(Lobby lobby)
		{
			// �W�܂����Ƃ�
			if (lobby.Players.Count == lobby.MaxPlayers && NetworkManager.Singleton.IsHost && isTranstionScene) {
				print("�����o�[���W�܂�܂���");
				NetworkManager.Singleton.SceneManager.LoadScene("OnlineMultiScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
			}
		}
	}
}