using PolymorphicMessagePack;
namespace Geek.Server.Proto
{
	public partial class PolymorphicRegister
	{
	    static PolymorphicRegister()
        {
            System.Console.WriteLine("***PolymorphicRegister Init***"); 
            Register();
        }

		public static void Register()
        {
			PolymorphicTypeMapper.Register<Geek.Server.Proto.UserBagInfo>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.BagComposePetReq>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.BagComposePetResp>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.BagUseItemReq>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.BagSellItemReq>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.CommonResp>();
			PolymorphicTypeMapper.Register<ClientProto.NetConnectMessage>();
			PolymorphicTypeMapper.Register<ClientProto.NetDisConnectMessage>();
			PolymorphicTypeMapper.Register<ClientProto.HearBeat>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.Proto.A>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.Proto.B>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.UserInfo>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.LoginReq>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.LoginResp>();
			PolymorphicTypeMapper.Register<Geek.Server.Proto.KickOut>();
        }
	}
}
