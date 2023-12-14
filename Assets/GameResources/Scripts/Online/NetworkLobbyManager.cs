using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using DesignPatterns.Singleton;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Network {
	public class NetworkLobbyManager : Singleton<NetworkLobbyManager> {
		/* Fields */
		const int QUERY_COUNT = 5;
		const string LOBBYS_JOIN_CODE_KEY = "JoinCode";

		[SerializeField] float heartBeatDuration = 15;

		bool isCreated;
		float heartBeatTimer;

		//-------------------------------------------------------------------
		/* Properties */
		public string JoinedLobbyID { get; private set; }

		public event System.Action<string> OnJoinedLobby;

		public LobbyEventCallbacks EventCallbacks { get; private set; } = new LobbyEventCallbacks();

		//-------------------------------------------------------------------
		/* Messages */

		//-------------------------------------------------------------------
		/* Methods */
		/// <summary> ���r�[�쐬 </summary>
		public async Task<Lobby> CreateLobby(string relayJoinCode)
		{
			string lobbyNameID = Random.Range(1, 10000).ToString("D4");     // ���r�[ID�͓K���ȗ���
			string lobbyName = "Lobby" + lobbyNameID;

			// ���r�[�ݒ�
			CreateLobbyOptions options = new CreateLobbyOptions {
				// Relay�̎Q���R�[�h�����r�[�̃f�[�^�ɓo�^����
				Data = new Dictionary<string, DataObject> {
					{LOBBYS_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode ,DataObject.IndexOptions.S1) },
				},
			};

			try {
				// ���r�[�쐬
				var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, options);

				// �C�x���g�ǉ�
				await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, EventCallbacks);

				print($"���r�[({lobby.Id})���쐬���܂���");

				// �쐬�������r�[����ۑ�
				isCreated = true;
				JoinedLobbyID = lobby.Id;

				return lobby;
			}
			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}

		/// <summary> ���r�[�Q�� </summary>
		public async Task<Lobby> JoinLobby(string lobbyID)
		{
			try {
				// ���r�[�Q��
				var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
				await LobbyService.Instance.SubscribeToLobbyEventsAsync(joinedLobby.Id, EventCallbacks);
				print("���r�[�ɎQ�����܂���");

				// �����[�Q��
				if (joinedLobby.Data.TryGetValue(LOBBYS_JOIN_CODE_KEY, out var joinCode)) {
					OnJoinedLobby?.Invoke(joinCode.Value);
				}

				JoinedLobbyID = lobbyID;
				return joinedLobby;
			}
			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}

		//-------------------------------------------------------------------
		/// <summary> �Q���������r�[�̑ގ� </summary>
		public async Task LeftFromLobby()
		{
			await LeftFromLobby(JoinedLobbyID);
		}

		/// <summary> ���r�[�̑ގ� </summary>
		public async Task LeftFromLobby(string lobbyID)
		{
			try {
				// ���r�[���쐬���Ă���΁A�폜
				if (isCreated) {
					await LobbyService.Instance.DeleteLobbyAsync(lobbyID);
					print($"���r�[({lobbyID})���폜���܂���");
				}

				else {
					var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);

					// ���r�[����ގ�
					await LobbyService.Instance.RemovePlayerAsync(lobbyID, lobby.Players.Last().Id);
				}

				// ���Z�b�g
				isCreated = false;
				JoinedLobbyID = string.Empty;
			}
			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}

		//-------------------------------------------------------------------
		/// <summary> ���r�[���� </summary>
		public async Task<IReadOnlyList<Lobby>> SearchLobbies()
		{
			// �����I�v�V�����w��
			QueryLobbiesOptions options = new QueryLobbiesOptions {
				Count = QUERY_COUNT,        // �������ʐ�

				// �t�B���^�[�w��
				Filters = new List<QueryFilter> {
					new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),        // �����̃��r�[�͌������Ȃ�
				},

				// �\�[�g���w��
				Order = new List<QueryOrder> {
					new QueryOrder(false, QueryOrder.FieldOptions.AvailableSlots),									// �󂫐l���̍~���ŕ��ѕς�
				},
			};

			try {
				// ����
				QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
				return response.Results;
			}
			catch (LobbyServiceException e) {
				throw e;
			}
		}

		//-------------------------------------------------------------------
		// �n�[�g�r�[�g����
		public async void SendLobbyHeartBeat()
		{
			if (isCreated) {
				heartBeatTimer += Time.deltaTime;

				if (heartBeatTimer >= heartBeatDuration) {
					heartBeatTimer = 0;

					try {
						await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobbyID);
						print("Send HeartBeat");
					}
					catch (LobbyServiceException e) {
						Debug.LogException(e);
					}
				}
			}
		}

		public async Task<Lobby> GetJoinedLobby()
		{
			try {
				// ���r�[ID���m�ۂ����܂őҋ@
				await UniTask.WaitWhile(() => string.IsNullOrEmpty(JoinedLobbyID),cancellationToken:this.GetCancellationTokenOnDestroy());

				// �擾
				return await LobbyService.Instance.GetLobbyAsync(JoinedLobbyID);
			}

			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}
	}
}