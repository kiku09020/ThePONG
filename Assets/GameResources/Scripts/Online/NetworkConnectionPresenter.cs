using Cysharp.Threading.Tasks;
using GameController.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

		//-------------------------------------------------------------------
		/* Properties */

		//-------------------------------------------------------------------
		/* Messages */
		async void Start()
		{
			ResetLobbyPlayersCount();

			// UIのイベントセット
			SetUIEvents();

			// 認証されるまで待機
			await UniTask.WaitUntil(() => AuthenticationService.Instance.IsAuthorized, cancellationToken: this.GetCancellationTokenOnDestroy()) ;

			// PlayerID
			idText.text = AuthenticationService.Instance.PlayerId.ToString();

		}

		//-------------------------------------------------------------------
		/* Methods */
		void SetUIEvents()
		{
			// 開始ボタン(作成・参加)
			gameStartButton.onClick.AddListener(async () => {
				UIManager.ShowUIGroup<TitleConnectingUI>();
				await NetworkController.Instance.CreateOrJoin();
			});

			// 切断ボタン(サーバー切断・ロビー削除)
			disconnectButton.onClick.AddListener(async () => {
				UIManager.ShowUIGroup<TitleMainUIGroup>();
				await NetworkController.Instance.DisconnectFromServer();
			});
		}

		//-------------------------------------------------------------------
		// プレイヤー数のテキストの変更
		public async Task SetLobbyPlayersCount()
		{
			var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();
			playerCountText.text = $"( {joinedLobby.Players.Count} / {joinedLobby.MaxPlayers} )";
		}

		public void SetLobbyPlayersCount(Lobby joinedLobby)
		{
			playerCountText.text = $"( {joinedLobby.Players.Count} / {joinedLobby.MaxPlayers} )";
		}

		public void ResetLobbyPlayersCount()
		{
			playerCountText.text = "";
		}
	}
}