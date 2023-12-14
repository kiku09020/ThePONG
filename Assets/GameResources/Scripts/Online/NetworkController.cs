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
        [SerializeField, Tooltip("遷移までの待機時間")] float transitoinWaitDuration = 3;

        //-------------------------------------------------------------------
        /* Properties */
        /// <summary> クライアント接続時 </summary>
        public event System.Action<Lobby> OnClientJoined;

        /// <summary> ロビー退室時 </summary>
        public event System.Action<Lobby> OnPlayerLeft;

        /// <summary> プレイヤー自身がサーバー接続時 </summary>
        public event System.Action OnConnected;

        /// <summary> プレイヤー自身がサーバー切断時 </summary>
        public event System.Action OnDisconnectFromServer;

        /// <summary> ホストがロビー削除した時 </summary>
        public event System.Action OnDeletedLobbyByHost;

        /// <summary> マッチング成立時 </summary>
        public event System.Action OnMathcing;

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
            /* 自作イベント */
            // 自作ロビー参加イベントにリレーサーバー参加処理を追加(ロビーに参加したクライアント自身のみ)
            NetworkLobbyManager.Instance.OnJoinedLobby += async (joinCode) => await NetworkRelayManager.Instance.JoinRelay(joinCode);

            /* ロビーイベント */


            // ロビー参加イベント(ロビー内のプレイヤーにのみ通知)
            NetworkLobbyManager.Instance.EventCallbacks.PlayerJoined += (players) => {
                print("プレイヤーが参加したんごwwww");
            };

            // ロビー内プレイヤー退室時(ロビー内のプレイヤーにのみ通知)
            NetworkLobbyManager.Instance.EventCallbacks.PlayerLeft += async (players) => {
                var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();
                OnPlayerLeft?.Invoke(joinedLobby);

                print("プレイヤーが離脱したんごwwww");
            };

            // ロビー削除時(ロビーに参加していたプレイヤー全員に通知)
            NetworkLobbyManager.Instance.EventCallbacks.LobbyDeleted += () => {
                print("削除イベント実行～");

                // 先に切断されちゃって、ホストかどうか判定できない
                // ホスト：切断 → 削除
                // クライアント：削除 → 切断
                //if (!NetworkManager.Singleton.IsHost) {
                //	print("ホストが削除しました");
                //}
            };

            /* サーバーイベント */
            // 接続開始時
            NetworkManager.Singleton.OnClientStarted += () => {

                print("作成されました～～～～～");
            };

            // サーバー接続時
            // (LobbyEventCallbacks.PlayerJoinedだと、自身が含まれないためこれ使ってる。)
            NetworkManager.Singleton.OnClientConnectedCallback += async (playerID) => {
                var joinedLobby = await NetworkLobbyManager.Instance.GetJoinedLobby();

                OnClientJoined?.Invoke(joinedLobby);

                MathcingProcess(joinedLobby);

                print("接続イベント実行～");
            };

            // サーバー切断時
            NetworkManager.Singleton.OnClientDisconnectCallback += async (count) => {
                print("切断イベント実行～");

                // ホスト判定可能
                if (!NetworkManager.Singleton.IsHost) {
                    print("ホストが切断しちゃったよ～～～");
                    OnDeletedLobbyByHost?.Invoke();

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
                lobby = await NetworkLobbyManager.Instance.CreateLobby(joinCode);       // ロビー作成

            }

            // ロビーがあれば参加
            else {
                // 一番古いロビーに参加後に、リレー参加
                lobby = await NetworkLobbyManager.Instance.JoinLobby(result.First().Id);
            }

            OnConnected?.Invoke();
        }

        //-------------------------------------------------------------------
        /// <summary> サーバーから切断 </summary>
        public async Task DisconnectFromServer(bool leftFromLobby = true)
        {
            // ロビー退室処理
            await NetworkLobbyManager.Instance.LeftFromLobby(leftFromLobby);

            // 閉じる
            NetworkManager.Singleton.Shutdown();

            OnDisconnectFromServer?.Invoke();
            print("サーバーの接続を切断しました");
        }

        //-------------------------------------------------------------------
        // 集まったときの処理
        async void MathcingProcess(Lobby lobby)
        {
            if (lobby.AvailableSlots == 0) {
                print("メンバーが集まりました");
                OnMathcing?.Invoke();

                // シーン遷移
                if (isTranstionScene && NetworkManager.Singleton.IsHost) {
                    await UniTask.Delay(System.TimeSpan.FromSeconds(transitoinWaitDuration), cancellationToken: destroyCancellationToken);            // 待機
                    NetworkManager.Singleton.SceneManager.LoadScene("OnlineMultiScene", UnityEngine.SceneManagement.LoadSceneMode.Single);      // 遷移
                }
            }
        }
    }
}