using Luban;

namespace Geek.Server.Config
{
    /// <summary>
    /// 同步加载指定名字的配置表数据。
    /// </summary>
    public delegate ByteBuf LoadDataTableFunc(string dataTableName);
    
    /// <summary>
    /// 异步加载指定名字的配置表数据。
    /// </summary>
    public delegate Task<ByteBuf> LoadDataTableAsyncFunc(string dataTableName);
}