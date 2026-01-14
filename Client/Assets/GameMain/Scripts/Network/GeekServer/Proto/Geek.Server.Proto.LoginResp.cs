//auto generated, do not modify it

using MessagePack;

namespace Geek.Server.Proto
{
	[MessagePackObject(true)]
	public class LoginResp : ResponseMessage
	{
		[IgnoreMember]
		public new const int Sid = 1721248663;

		[IgnoreMember]
		public const int MsgID = Sid;
		[IgnoreMember]
		public override int MsgId => MsgID;

        public UserInfo UserInfo { get; set; }
        public UserBagInfo BagInfo { get; set; }
	}
}
