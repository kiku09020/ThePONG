using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using UnityEngine;

namespace Network {
	public class NetworkRelayManager : Singleton<NetworkRelayManager> {
		/* Fields */
		[SerializeField] int allocationCount = 1;

		//-------------------------------------------------------------------
		/* Methods */
		public async Task<string> CreateRelay()
		{
			try {
				// 割り当て作成
				var allocation = await RelayService.Instance.CreateAllocationAsync(allocationCount);

				// サーバーデータ設定
				var relayServerData = new RelayServerData(allocation, "dtls");
				var transport = Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>();
				transport.SetRelayServerData(relayServerData);

				// 参加コードの取得
				var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

				// ホストとして開始
				Unity.Netcode.NetworkManager.Singleton.StartHost();

				print($"サーバーを作成しました({allocation.AllocationId})");
				return joinCode;
			}
			catch (RelayServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}

		public async Task JoinRelay(string joinCode)
		{
			try {
				// コードをもとに参加
				var joinedAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

				// サーバーデータ設定
				RelayServerData serverData = new RelayServerData(joinedAllocation, "dtls");
				Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

				// クライアントとして開始
				Unity.Netcode.NetworkManager.Singleton.StartClient();
				print($"サーバーに参加しました({joinedAllocation.AllocationId})");
			}
			catch (RelayServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}
	}
}