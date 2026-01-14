//auto generated, do not modify it

using System;
namespace Geek.Server.Proto
{
	public class MsgFactory
	{
		private static readonly System.Collections.Generic.Dictionary<int, Type> lookup;
		private static readonly System.Collections.Generic.Dictionary<Type, int> revertLookup;

        static MsgFactory()
        {
            lookup = new System.Collections.Generic.Dictionary<int, Type>(16)
            {
			    { -1907559902, typeof(Geek.Server.Proto.UserBagInfo) },
			    { -306139429, typeof(Geek.Server.Proto.BagComposePetReq) },
			    { -527122749, typeof(Geek.Server.Proto.BagComposePetResp) },
			    { -103943650, typeof(Geek.Server.Proto.BagUseItemReq) },
			    { 1493681131, typeof(Geek.Server.Proto.BagSellItemReq) },
			    { -1509970890, typeof(Geek.Server.Proto.CommonResp) },
			    { 667869091, typeof(ClientProto.NetConnectMessage) },
			    { 1245418514, typeof(ClientProto.NetDisConnectMessage) },
			    { -71242399, typeof(ClientProto.HearBeat) },
			    { -138638180, typeof(Geek.Server.Proto.Proto.TestStruct) },
			    { -359623823, typeof(Geek.Server.Proto.Proto.A) },
			    { 1375358240, typeof(Geek.Server.Proto.Proto.B) },
			    { -593677237, typeof(Geek.Server.Proto.UserInfo) },
			    { -1451735748, typeof(Geek.Server.Proto.LoginReq) },
			    { 1721248663, typeof(Geek.Server.Proto.LoginResp) },
			    { -499789670, typeof(Geek.Server.Proto.KickOut) },
            };
			
            revertLookup = new System.Collections.Generic.Dictionary<Type, int>(16)
            {
			    { typeof(Geek.Server.Proto.UserBagInfo), -1907559902 },
			    { typeof(Geek.Server.Proto.BagComposePetReq), -306139429 },
			    { typeof(Geek.Server.Proto.BagComposePetResp), -527122749 },
			    { typeof(Geek.Server.Proto.BagUseItemReq), -103943650 },
			    { typeof(Geek.Server.Proto.BagSellItemReq), 1493681131 },
			    { typeof(Geek.Server.Proto.CommonResp), -1509970890 },
			    { typeof(ClientProto.NetConnectMessage), 667869091 },
			    { typeof(ClientProto.NetDisConnectMessage), 1245418514 },
			    { typeof(ClientProto.HearBeat), -71242399 },
			    { typeof(Geek.Server.Proto.Proto.TestStruct), -138638180 },
			    { typeof(Geek.Server.Proto.Proto.A), -359623823 },
			    { typeof(Geek.Server.Proto.Proto.B), 1375358240 },
			    { typeof(Geek.Server.Proto.UserInfo), -593677237 },
			    { typeof(Geek.Server.Proto.LoginReq), -1451735748 },
			    { typeof(Geek.Server.Proto.LoginResp), 1721248663 },
			    { typeof(Geek.Server.Proto.KickOut), -499789670 },
            };
        }

        public static Type GetType(int msgId)
		{
			if (lookup.TryGetValue(msgId, out Type res))
				return res;
			else
				throw new Exception($"can not find msg type :{msgId}");
		}

		public static int GetMsgId(Type type)
		{
			if (revertLookup.TryGetValue(type, out int msgId))
				return msgId;
			else
				throw new Exception($"can not find msg id :{type.FullName}");
		}
	}
}
