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
				// ���蓖�č쐬
				var allocation = await RelayService.Instance.CreateAllocationAsync(allocationCount);

				// �T�[�o�[�f�[�^�ݒ�
				var relayServerData = new RelayServerData(allocation, "dtls");
				var transport = Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>();
				transport.SetRelayServerData(relayServerData);

				// �Q���R�[�h�̎擾
				var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

				// �z�X�g�Ƃ��ĊJ�n
				Unity.Netcode.NetworkManager.Singleton.StartHost();

				print($"�T�[�o�[���쐬���܂���({allocation.AllocationId})");
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
				// �R�[�h�����ƂɎQ��
				var joinedAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

				// �T�[�o�[�f�[�^�ݒ�
				RelayServerData serverData = new RelayServerData(joinedAllocation, "dtls");
				Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

				// �N���C�A���g�Ƃ��ĊJ�n
				Unity.Netcode.NetworkManager.Singleton.StartClient();
				print($"�T�[�o�[�ɎQ�����܂���({joinedAllocation.AllocationId})");
			}
			catch (RelayServiceException e) {
				Debug.LogException(e);
				throw e;
			}
		}
	}
}