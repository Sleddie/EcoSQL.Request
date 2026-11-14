using System.Data.Common;

namespace EcoSQL.Dbms;

/// <summary>Represents an interface for database management system provider adapters.</summary>
public interface IDbmsProviderAdapter
{
    /// <summary>Gets the database provider factory.</summary>
    /// <returns>The database provider factory.</returns>
    DbProviderFactory GetDbProviderFactory();
}

/// <summary>Represents an abstract base class for database management system provider adapters.</summary>
public abstract class DbmsProviderAdapter : IDbmsProviderAdapter
{
    /// <summary>Gets the database provider factory.</summary>
    /// <returns>The database provider factory.</returns>
    public abstract DbProviderFactory GetDbProviderFactory();
}