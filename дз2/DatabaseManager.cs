using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ElectronicsStore
{
    public class DatabaseManager
    {
        private readonly string _connString = "Data Source=smartphones.db";

        public void Init()
        {
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS manufacturer (
                mfg_id INTEGER PRIMARY KEY, 
                mfg_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS smartphone (
                phn_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                mfg_id INTEGER NOT NULL, 
                phn_name TEXT NOT NULL, 
                phn_price INTEGER NOT NULL
            );";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT COUNT(*) FROM manufacturer";
            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
            {
                cmd.CommandText = @"
                INSERT INTO manufacturer VALUES (1, 'Apple'), (2, 'Samsung'), (3, 'Xiaomi'), (4, 'Huawei');
                
                INSERT INTO smartphone (mfg_id, phn_name, phn_price) VALUES 
                (1, 'iPhone 15 Pro', 120000), (1, 'iPhone 14', 85000), (1, 'iPhone SE', 45000),
                (2, 'Galaxy S24 Ultra', 130000), (2, 'Galaxy A55', 38000), (2, 'Galaxy XCover 7', 42000),
                (3, 'Xiaomi 14 Ultra', 110000), (3, 'Redmi Note 13 Pro', 28000), (3, 'Poco F6', 35000),
                (4, 'Pura 70 Pro', 90000), (4, 'Nova 12 SE', 27000), (4, 'Mate 60 Pro', 105000);";
                cmd.ExecuteNonQuery();
            }
        }

        public List<Manufacturer> GetBrands()
        {
            var list = new List<Manufacturer>();
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT mfg_id, mfg_name FROM manufacturer ORDER BY mfg_id";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Manufacturer(r.GetInt32(0), r.GetString(1)));
            return list;
        }

        public List<Smartphone> GetPhones()
        {
            var list = new List<Smartphone>();
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT phn_id, mfg_id, phn_name, phn_price FROM smartphone ORDER BY phn_id";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Smartphone(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetInt32(3)));
            return list;
        }

        public Smartphone GetPhoneById(int id)
        {
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT phn_id, mfg_id, phn_name, phn_price FROM smartphone WHERE phn_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return new Smartphone(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetInt32(3));
            return null;
        }

        public void AddPhone(int mfgId, string name, int price)
        {
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO smartphone (mfg_id, phn_name, phn_price) VALUES (@mfg, @name, @price)";
            cmd.Parameters.AddWithValue("@mfg", mfgId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.ExecuteNonQuery();
        }

        public void UpdatePhone(Smartphone phn)
        {
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE smartphone SET mfg_id = @mfgId, phn_name = @name, phn_price = @price WHERE phn_id = @id";
            cmd.Parameters.AddWithValue("@id", phn.Id);
            cmd.Parameters.AddWithValue("@mfgId", phn.ManufacturerId);
            cmd.Parameters.AddWithValue("@name", phn.Name);
            cmd.Parameters.AddWithValue("@price", phn.Price);
            cmd.ExecuteNonQuery();
        }

        public void DeletePhone(int id)
        {
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM smartphone WHERE phn_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public (string[] cols, List<string[]> rows) RunSql(string sql)
        {
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();

            string[] cols = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                cols[i] = reader.GetName(i);

            var rows = new List<string[]>();
            while (reader.Read())
            {
                string[] row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                    row[i] = reader.GetValue(i)?.ToString() ?? "";
                rows.Add(row);
            }
            return (cols, rows);
        }
    }
}