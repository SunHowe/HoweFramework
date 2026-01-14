//auto generated, do not modify it

using MessagePack;

namespace ClientProto
{
	[MessagePackObject(true)]
	public class HearBeat : Message
	{
		[IgnoreMember]
		public const int Sid = -71242399;

		[IgnoreMember]
		public const int MsgID = Sid;
		[IgnoreMember]
		public override int MsgId => MsgID;

        /// <summary>
        /// 当前时间
        /// </summary>
        public long TimeTick { get; set; }
	}
}
