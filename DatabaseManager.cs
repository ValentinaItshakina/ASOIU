using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ElectronicsStore
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }


        public void InitializeDatabase(string mfgCsvPath, string phnCsvPath)
        {
            CreateTables();

            if (GetAllManufacturers().Count == 0 && File.Exists(mfgCsvPath))
            {
                ImportManufacturersFromCsv(mfgCsvPath);
                Console.WriteLine($"[Инициализация] Данные брендов успешно импортированы.");
            }
            if (GetAllSmartphones().Count == 0 && File.Exists(phnCsvPath))
            {
                ImportSmartphonesFromCsv(phnCsvPath);
                Console.WriteLine($"[Инициализация] Данные смартфонов успешно импортированы.");
            }
        }

        private void CreateTables()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS manufacturer (
                mfg_id INTEGER PRIMARY KEY AUTOINCREMENT,
                mfg_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS smartphone (
                phn_id INTEGER PRIMARY KEY AUTOINCREMENT,
                mfg_id INTEGER NOT NULL,
                phn_name TEXT NOT NULL,
                phn_price INTEGER NOT NULL,
                FOREIGN KEY (mfg_id) REFERENCES manufacturer(mfg_id)
            );";
            cmd.ExecuteNonQuery();
        }

        private void ImportManufacturersFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 2) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO manufacturer (mfg_id, mfg_name) VALUES (@id, @name)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@name", parts[1]);
                cmd.ExecuteNonQuery();
            }
        }

        private void ImportSmartphonesFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 4) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO smartphone (phn_id, mfg_id, phn_name, phn_price) VALUES (@id, @mfgId, @name, @price)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@mfgId", int.Parse(parts[1]));
                cmd.Parameters.AddWithValue("@name", parts[2]);
                cmd.Parameters.AddWithValue("@price", int.Parse(parts[3]));
                cmd.ExecuteNonQuery();
            }
        }

        public List<Manufacturer> GetAllManufacturers()
        {
            var result = new List<Manufacturer>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT mfg_id, mfg_name FROM manufacturer ORDER BY mfg_id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Manufacturer(reader.GetInt32(0), reader.GetString(1)));
            }
            return result;
        }

        public List<Smartphone> GetAllSmartphones()
        {
            var result = new List<Smartphone>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT phn_id, mfg_id, phn_name, phn_price FROM smartphone ORDER BY phn_id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Smartphone(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3)));
            }
            return result;
        }

        public Smartphone GetSmartphoneById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT phn_id, mfg_id, phn_name, phn_price FROM smartphone WHERE phn_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Smartphone(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
            }
            return null;
        }

        public void AddSmartphone(Smartphone phn)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO smartphone (mfg_id, phn_name, phn_price) VALUES (@mfgId, @name, @price)";
            cmd.Parameters.AddWithValue("@mfgId", phn.ManufacturerId);
            cmd.Parameters.AddWithValue("@name", phn.Name);
            cmd.Parameters.AddWithValue("@price", phn.Price);
            cmd.ExecuteNonQuery();
        }

        public void UpdateSmartphone(Smartphone phn)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE smartphone SET mfg_id = @mfgId, phn_name = @name, phn_price = @price WHERE phn_id = @id";
            cmd.Parameters.AddWithValue("@id", phn.Id);
            cmd.Parameters.AddWithValue("@mfgId", phn.ManufacturerId);
            cmd.Parameters.AddWithValue("@name", phn.Name);
            cmd.Parameters.AddWithValue("@price", phn.Price);
            cmd.ExecuteNonQuery();
        }

        public void DeleteSmartphone(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM smartphone WHERE phn_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }


        public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();

            string[] columns = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                columns[i] = reader.GetName(i);

            var rows = new List<string[]>();
            while (reader.Read())
            {
                string[] row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                    row[i] = reader.GetValue(i)?.ToString() ?? "";
                rows.Add(row);
            }
            return (columns, rows);
        }
    }
}