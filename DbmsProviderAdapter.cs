using System.Data.Common;

namespace EcoSQL.Dbms;

public interface IDbmsProviderAdapter
{
    DbProviderFactory GetDbProviderFactory();
}

public abstract class DbmsProviderAdapter : IDbmsProviderAdapter
{
    public abstract DbProviderFactory GetDbProviderFactory();
}