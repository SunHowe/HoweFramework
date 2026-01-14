//auto generated, do not modify it

using MessagePack;

namespace Geek.Server.Proto.Proto
{
	[MessagePackObject(true)]
	public class A 
	{
		[IgnoreMember]
		public const int Sid = -359623823;


        public int Age { get; set; }
        public TestEnum E { get; set; } = TestEnum.B;
        public TestStruct TS { get; set; }
	}
}
