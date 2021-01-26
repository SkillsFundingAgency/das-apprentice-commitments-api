using System;
using System.Data.Common;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests.Database
{

    public static class CreateAcceptanceTestData
    {
        public const string AcceptanceTestsDatabaseName = "AcceptanceTestsDb";

        public static void DropDatabase(string connectionString)
        {
            File.Delete(AcceptanceTestsDatabaseName);
        }

        public static void CreateTables(string connectionString)
        {

            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            
            using SqliteCommand registrationCommand = new SqliteCommand(
                @"
                    CREATE TABLE Registration(
                        Id uniqueidentifier NOT NULL PRIMARY KEY, 
                        ApprenticeshipId INTEGER NOT NULL,
                        Email nvarchar(255) NOT NULL,
                        CreatedOn TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
            ", connection);
            registrationCommand.ExecuteNonQuery();

            using SqliteCommand clientOutBoxDataCommand = new SqliteCommand(
                @"
                    CREATE TABLE ClientOutboxData(
                        MessageId uniqueidentifier NOT NULL PRIMARY KEY, 
                        EndpointName nvarchar(150) NOT NULL, 
                        CreatedAt TIMESTAMP NOT NULL, 
                        Dispatched INTEGER NOT NULL,
                        DispatchedAt TIMESTAMP NULL, 
                        Operations nvarchar(255) NOT NULL,
                        PersistenceVersion nvarchar(23) NOT NULL
                );
            ", connection);
            clientOutBoxDataCommand.ExecuteNonQuery();

            using SqliteCommand outBoxDataCommand = new SqliteCommand(
                @"
                    CREATE TABLE OutboxData(
                        MessageId uniqueidentifier NOT NULL PRIMARY KEY, 
                        Dispatched INTEGER NOT NULL,
                        DispatchedAt TIMESTAMP NULL, 
                        PersistenceVersion nvarchar(23) NOT NULL,
                        Operations nvarchar(255) NOT NULL
                );
            ", connection);
            outBoxDataCommand.ExecuteNonQuery();


            connection.Close();
        }
    }
}