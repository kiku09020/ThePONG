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

			// サービス初期化
			await UnityServices.InitializeAsync();

			AuthenticationService.Instance.SignedIn += () => {
				print($"{AuthenticationService.Instance.PlayerId} was signedin.");
			};

#if UNITY_EDITOR
			// リセット(MPPM用)
			AuthenticationService.Instance.ClearSessionToken();
#endif

			await AuthenticationService.Instance.SignInAnonymouslyAsync();

			SetEvents();
		}

		private void FixedUpdate()
		{
			// ハートビート送信
			NetworkLobbyManager.Instance.SendLobbyHeartBeat();
		}

		//-------------------------------------------------------------------
		// サーバー接続時や切断時などのイベント。初期化時に呼び出す
		void SetEvents()
		{
			// 自作ロビー参加イベントにリレーサーバー参加処理を追加(ロビーに参加したクライアント自身のみ)
			NetworkLobbyManager.Instance.OnJoinedLobby += async (joinCode) => await NetworkRelayManager.Instance.JoinRelay(joinCode);

			// ロビー参加イベント
			NetworkLobbyManager.Instance.EventCallbacks.PlayerJoined += (players) => {
				print("プレイヤーが参加したんごwwww");
			};

			// ロビー内プレイヤー離脱時
			NetworkLobbyManager.Instance.EventCallbacks.PlayerLeft += async (players) => {
				await presenter.SetLobbyPlayersCount();
			};

			// ロビー削除時
			NetworkLobbyManager.Instance.EventCallbacks.LobbyDeleted += () => {
				print("削除イベント実行～");

				// 先に切断されちゃって、ホストかどうか判定できない
				if (!NetworkManager.Singleton.IsHost) {
					print("ホストが削除しました");
				}
			};

			// サーバー接続時
			// (LobbyEventCallbacks.PlayerJoinedだと、自身が含まれないためこれ使ってる。)
			NetworkManager.Singleton.OnClientConnectedCallback += async (playerID) => {
				var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();

				presenter.SetLobbyPlayersCount(joinedLobby);
				OnMathcing(joinedLobby);

				print("接続イベント実行～");
			};

			// サーバー切断時
			NetworkManager.Singleton.OnClientDisconnectCallback += async (count) => {
				print("切断イベント実行～");

				// ホスト判定可能
				if (!NetworkManager.Singleton.IsHost) {
					print("ホストが切断しちゃったよ～～～");
					UIManager.ShowUIGroup<TitleMainUIGroup>();
					await DisconnectFromServer(false);
				}
			};
		}

		//-------------------------------------------------------------------
		/* Methods */
		public async Task CreateOrJoin()
		{
			// アクティブなロビーを検索
			var result = await NetworkLobbyManager.Instance.SearchLobbies();
			Lobby lobby = null;

			// ロビーが1つもなければ、ロビー作成
			if (result.Count == 0) {
				string joinCode = await NetworkRelayManager.Instance.CreateRelay();     // リレーサーバー作成
				lobby = await NetworkLobbyManager.Instance.CreateLobby(joinCode);               // ロビー作成
			}

			// ロビーがあれば参加
			else {
				// 一番古いロビーに参加後に、リレー参加
				lobby = await NetworkLobbyManager.Instance.JoinLobby(result.First().Id);
			}
		}

		//-------------------------------------------------------------------
		/// <summary> サーバーから切断 </summary>
		public async Task DisconnectFromServer(bool leftFromLobby=true)
		{
			// ロビー退室処理
			if (leftFromLobby) {
				await NetworkLobbyManager.Instance.LeftFromLobby();
			}

			// 閉じる
			NetworkManager.Singleton.Shutdown();

			presenter.ResetLobbyPlayersCount();
			print("サーバーの接続を切断しました");
		}

		//-------------------------------------------------------------------

		void OnMathcing(Lobby lobby)
		{
			// 集まったとき
			if (lobby.Players.Count == lobby.MaxPlayers && NetworkManager.Singleton.IsHost && isTranstionScene) {
				print("メンバーが集まりました");
				NetworkManager.Singleton.SceneManager.LoadScene("OnlineMultiScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
			}
		}
	}
}