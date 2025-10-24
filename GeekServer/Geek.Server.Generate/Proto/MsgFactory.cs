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
            lookup = new System.Collections.Generic.Dictionary<int, Type>(19)
            {
			    { 1435193915, typeof(Geek.Server.Proto.ReqBagInfo) },
			    { -1872884227, typeof(Geek.Server.Proto.ResBagInfo) },
			    { 225320501, typeof(Geek.Server.Proto.ReqComposePet) },
			    { 750865816, typeof(Geek.Server.Proto.ResComposePet) },
			    { 1686846581, typeof(Geek.Server.Proto.ReqUseItem) },
			    { -1395845865, typeof(Geek.Server.Proto.ReqSellItem) },
			    { 901279609, typeof(Geek.Server.Proto.ResItemChange) },
			    { 667869091, typeof(ClientProto.NetConnectMessage) },
			    { 1245418514, typeof(ClientProto.NetDisConnectMessage) },
			    { 299119425, typeof(Geek.Server.Proto.TestStruct) },
			    { 1250601847, typeof(Geek.Server.Proto.A) },
			    { -899515946, typeof(Geek.Server.Proto.B) },
			    { -593677237, typeof(Geek.Server.Proto.UserInfo) },
			    { 1267074761, typeof(Geek.Server.Proto.ReqLogin) },
			    { 785960738, typeof(Geek.Server.Proto.ResLogin) },
			    { 1587576546, typeof(Geek.Server.Proto.ResLevelUp) },
			    { 1575482382, typeof(Geek.Server.Proto.HearBeat) },
			    { 1179199001, typeof(Geek.Server.Proto.ResErrorCode) },
			    { 537499886, typeof(Geek.Server.Proto.ResPrompt) },
            };
			
            revertLookup = new System.Collections.Generic.Dictionary<Type, int>(19)
            {
			    { typeof(Geek.Server.Proto.ReqBagInfo), 1435193915 },
			    { typeof(Geek.Server.Proto.ResBagInfo), -1872884227 },
			    { typeof(Geek.Server.Proto.ReqComposePet), 225320501 },
			    { typeof(Geek.Server.Proto.ResComposePet), 750865816 },
			    { typeof(Geek.Server.Proto.ReqUseItem), 1686846581 },
			    { typeof(Geek.Server.Proto.ReqSellItem), -1395845865 },
			    { typeof(Geek.Server.Proto.ResItemChange), 901279609 },
			    { typeof(ClientProto.NetConnectMessage), 667869091 },
			    { typeof(ClientProto.NetDisConnectMessage), 1245418514 },
			    { typeof(Geek.Server.Proto.TestStruct), 299119425 },
			    { typeof(Geek.Server.Proto.A), 1250601847 },
			    { typeof(Geek.Server.Proto.B), -899515946 },
			    { typeof(Geek.Server.Proto.UserInfo), -593677237 },
			    { typeof(Geek.Server.Proto.ReqLogin), 1267074761 },
			    { typeof(Geek.Server.Proto.ResLogin), 785960738 },
			    { typeof(Geek.Server.Proto.ResLevelUp), 1587576546 },
			    { typeof(Geek.Server.Proto.HearBeat), 1575482382 },
			    { typeof(Geek.Server.Proto.ResErrorCode), 1179199001 },
			    { typeof(Geek.Server.Proto.ResPrompt), 537499886 },
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
