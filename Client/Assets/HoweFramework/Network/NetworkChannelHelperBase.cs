using System.IO;

namespace HoweFramework
{
    /// <summary>
    /// 网络通道辅助器基类。
    /// </summary>
    public abstract class NetworkChannelHelperBase : INetworkChannelHelper
    {
        public abstract int PacketHeaderLength { get; }

        public IRemoteRequestDispatcher RequestDispatcher { get; private set; }

        public INetworkChannel NetworkChannel { get; private set; }

        public abstract Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData);

        public abstract IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData);

        public abstract void PrepareForConnecting();

        public abstract bool SendHeartBeat();

        public abstract bool Serialize<T>(T packet, Stream destination) where T : Packet;

        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        public void Initialize(INetworkChannel networkChannel)
        {
            NetworkChannel = networkChannel;
            RequestDispatcher = RemoteRequestModule.Instance.CreateRemoteRequestDispatcher();
            NetworkChannel.SetDefaultHandler(OnReceivePacket);

            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();

            RequestDispatcher.Dispose();
            RequestDispatcher = null;
            NetworkChannel = null;
        }

        private bool OnReceivePacket(object sender, GameEventArgs e)
        {
            var packet = (Packet)e;

            if (packet is IRemoteRequest remoteRequest && packet is IResponse response)
            {
                packet.SetIsReleaseAfterFire(false);

                var requestId = remoteRequest.RequestId;
                RequestDispatcher.SetResponse(requestId, response);
                return true;
            }

            return false;
        }
    }
}
