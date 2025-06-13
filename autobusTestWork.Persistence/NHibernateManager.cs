using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;

namespace autobusTestWork.Persistence;

public class NHibernateManager
{
    private ISessionFactory _sessionFactory;
    public ISessionFactory SessionFactory => _sessionFactory;

    public NHibernateManager(string connectionString)
    {
        EnsureDatabaseExists(connectionString);

        _sessionFactory = Fluently.Configure()
            .Database(MySQLConfiguration.Standard.ConnectionString(connectionString))
            .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    }

    private void BuildSchema(NHibernate.Cfg.Configuration config)
    {
        var schemaUpdate = new SchemaUpdate(config);
        schemaUpdate.Execute(false, true);
    }

    private void EnsureDatabaseExists(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        string dbName = builder.Database;
        builder.Database = "";

        using var adminConnection = new MySqlConnection(builder.ToString());
        adminConnection.Open();

        using var command = adminConnection.CreateCommand();
        command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}`";
        command.ExecuteNonQuery();
    }

    public void AddNHibernate(IServiceCollection services)
    {
        services.AddSingleton(SessionFactory);
        services.AddScoped(factory => SessionFactory.OpenSession());
    }
}
