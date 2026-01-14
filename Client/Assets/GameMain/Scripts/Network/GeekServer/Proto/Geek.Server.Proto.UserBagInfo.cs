//auto generated, do not modify it

using MessagePack;
using System.Collections.Generic;

namespace Geek.Server.Proto
{
	[MessagePackObject(true)]
	public class UserBagInfo 
	{
		[IgnoreMember]
		public const int Sid = -1907559902;


        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();
	}
}
