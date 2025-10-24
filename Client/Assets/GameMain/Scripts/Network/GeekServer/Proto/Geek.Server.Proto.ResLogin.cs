//auto generated, do not modify it

using MessagePack;

namespace Geek.Server.Proto
{
	[MessagePackObject(true)]
	public class ResLogin : ResponseMessage
	{
		[IgnoreMember]
		public new const int Sid = 785960738;

		[IgnoreMember]
		public const int MsgID = Sid;
		[IgnoreMember]
		public override int MsgId => MsgID;

        public UserInfo UserInfo { get; set; }
	}
}
