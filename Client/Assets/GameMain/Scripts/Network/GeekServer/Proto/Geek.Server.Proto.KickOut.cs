//auto generated, do not modify it

using MessagePack;

namespace Geek.Server.Proto
{
	[MessagePackObject(true)]
	public class KickOut : Message
	{
		[IgnoreMember]
		public const int Sid = -499789670;

		[IgnoreMember]
		public const int MsgID = Sid;
		[IgnoreMember]
		public override int MsgId => MsgID;

        public KickOutReason Reason { get; set; }
	}
}
