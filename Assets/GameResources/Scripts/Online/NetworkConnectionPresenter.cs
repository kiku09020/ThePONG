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
        [SerializeField] string connectingMessage = "接続中…";
        [SerializeField] string waitingMessage = "プレイヤーを待機中…";
        [SerializeField] string deletedMessage = "ホストによって、ロビーが削除されました…。";
        [SerializeField] string matchingMessage = "プレイヤーが集まりました！";

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        async void Start()
        {
            ResetConnectionUI();

            // イベントセット
            SetUIEvents();
            AddNetworkEvents();

            // 認証されるまで待機
            await UniTask.WaitUntil(() => AuthenticationService.Instance.IsAuthorized, cancellationToken: this.GetCancellationTokenOnDestroy());

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

        // ネットワークイベントの通知を受け取る
        void AddNetworkEvents()
        {
            // プレイヤー接続
            NetworkController.Instance.OnConnected += () => {
                disconnectButton.interactable = true;
                connectingText.text = waitingMessage;
            };

            // プレイヤー参加
            NetworkController.Instance.OnClientJoined += (lobby) => {
                SetLobbyPlayersCount(lobby);
            };

            // プレイヤー退室
            NetworkController.Instance.OnPlayerLeft += (lobby) => {
                SetLobbyPlayersCount(lobby);
            };

            // ホストがロビー削除
            NetworkController.Instance.OnDeletedLobbyByHost += () => {
                SetDeletedByHostDialog();
            };

            // サーバー切断
            NetworkController.Instance.OnDisconnectFromServer += () => {
                ResetConnectionUI();
                connectingText.text = connectingMessage;
            };

            // マッチング成立
            NetworkController.Instance.OnMathcing += () => {
                SetMatchingUI();
            };
        }

        //-------------------------------------------------------------------
        // プレイヤー数のテキストの変更
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
        // ロビー削除ダイアログ
        void SetDeletedByHostDialog()
        {
            dialogMessageText.text = deletedMessage;
            dialogUI.gameObject.SetActive(true);
            UIManager.ShowUIGroup<TitleMainUIGroup>();
        }

        // マッチング成立時UI
        void SetMatchingUI()
        {
            connectingText.text = matchingMessage;
            disconnectButton.interactable = false;
        }
    }
}