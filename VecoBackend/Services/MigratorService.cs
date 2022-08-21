using System.Text;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Npgsql;
using VecoBackend.Interfaces;
using VecoBackend.Utils;

namespace VecoBackend.Services;

public class MigratorService : IMigratorService
{
    private IMigrationRunner runner;
    private string _connectionString;

    public MigratorService(IMigrationRunner runner, IConfiguration configuration)
    {
        this.runner = runner;
        _connectionString = configuration.GetConnectionString("MainDB");
        if (_connectionString == null) throw new Exception("Connection string not specified");
    }

    public string MigrateUp()
    {
        try
        {
            // EnsureDatabase();
            runner.MigrateUp();

//        var result = String.IsNullOrEmpty(errs) ? "Success" : errs;

            return "result";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string MigrateDown(long version)
    {
        var errs = ConsoleHook(() => runner.MigrateDown(version));
        var result = String.IsNullOrEmpty(errs) ? "Success" : errs;

        return result;
    }

    private void EnsureDatabase()
    {
        NpgsqlConnection connection = null;
        try
        {
            using (connection = new NpgsqlConnection(_connectionString))
            {
                var dbName = "VecoDB";
                connection.Open();
                var records = connection.Query(@"SELECT * FROM table_name", 
                    new {name = dbName});
                if (!records.Any())
                    connection.Execute($"CREATE DATABASE {dbName}");

                connection.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            connection?.CloseAsync();
        }
    }

    private string ConsoleHook(Action action)
    {
        var saved = Console.Out;
        var sb = new StringBuilder();
        var tw = new StringWriter(sb);
        Console.SetOut(tw);

        try
        {
            action();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        tw.Close();

        // Restore the default console out.
        Console.SetOut(saved);

        var errs = sb.ToString();

        return errs;
    }
}