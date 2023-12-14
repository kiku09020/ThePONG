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
		/// <summary> ロビー作成 </summary>
		public async Task<Lobby> CreateLobby(string relayJoinCode)
		{
			string lobbyNameID = Random.Range(1, 10000).ToString("D4");     // ロビーIDは適当な乱数
			string lobbyName = "Lobby" + lobbyNameID;

			// ロビー設定
			CreateLobbyOptions options = new CreateLobbyOptions {
				// Relayの参加コードをロビーのデータに登録する
				Data = new Dictionary<string, DataObject> {
					{LOBBYS_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode ,DataObject.IndexOptions.S1) },
				},
			};

			try {
				// ロビー作成
				var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, options);

				// イベント追加
				await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, EventCallbacks);

				print($"ロビー({lobby.Id})を作成しました");

				// 作成したロビー情報を保存
				isCreated = true;
				JoinedLobbyID = lobby.Id;

				return lobby;
			}
			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}

		/// <summary> ロビー参加 </summary>
		public async Task<Lobby> JoinLobby(string lobbyID)
		{
			try {
				// ロビー参加
				var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
				await LobbyService.Instance.SubscribeToLobbyEventsAsync(joinedLobby.Id, EventCallbacks);
				print("ロビーに参加しました");

				// リレー参加
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
		/// <summary> 参加したロビーの退室 </summary>
		public async Task LeftFromLobby()
		{
			await LeftFromLobby(JoinedLobbyID);
		}

		/// <summary> ロビーの退室 </summary>
		public async Task LeftFromLobby(string lobbyID)
		{
			try {
				// ロビーを作成していれば、削除
				if (isCreated) {
					await LobbyService.Instance.DeleteLobbyAsync(lobbyID);
					print($"ロビー({lobbyID})を削除しました");
				}

				else {
					var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);

					// ロビーから退室
					await LobbyService.Instance.RemovePlayerAsync(lobbyID, lobby.Players.Last().Id);
				}

				// リセット
				isCreated = false;
				JoinedLobbyID = string.Empty;
			}
			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}

		//-------------------------------------------------------------------
		/// <summary> ロビー検索 </summary>
		public async Task<IReadOnlyList<Lobby>> SearchLobbies()
		{
			// 検索オプション指定
			QueryLobbiesOptions options = new QueryLobbiesOptions {
				Count = QUERY_COUNT,        // 検索結果数

				// フィルター指定
				Filters = new List<QueryFilter> {
					new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),        // 満員のロビーは検索しない
				},

				// ソート順指定
				Order = new List<QueryOrder> {
					new QueryOrder(false, QueryOrder.FieldOptions.AvailableSlots),									// 空き人数の降順で並び変え
				},
			};

			try {
				// 検索
				QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
				return response.Results;
			}
			catch (LobbyServiceException e) {
				throw e;
			}
		}

		//-------------------------------------------------------------------
		// ハートビート処理
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
				// ロビーIDが確保されるまで待機
				await UniTask.WaitWhile(() => string.IsNullOrEmpty(JoinedLobbyID),cancellationToken:this.GetCancellationTokenOnDestroy());

				// 取得
				return await LobbyService.Instance.GetLobbyAsync(JoinedLobbyID);
			}

			catch (LobbyServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}
	}
}