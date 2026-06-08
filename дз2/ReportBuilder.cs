using System;
using System.Collections.Generic;

namespace ElectronicsStore
{
    public class ReportBuilder
    {
        private readonly DatabaseManager _db;
        private string _sql = "";
        private string _title = "";
        private string[] _headers = Array.Empty<string>();
        private int[] _widths = Array.Empty<int>();
        private string _footerLabel = "";

        public ReportBuilder(DatabaseManager db) { _db = db; }

        public ReportBuilder Query(string sql) { _sql = sql; return this; }
        public ReportBuilder Title(string title) { _title = title; return this; }
        public ReportBuilder Header(params string[] columns) { _headers = columns; return this; }
        public ReportBuilder ColumnWidths(params int[] widths) { _widths = widths; return this; }
        public ReportBuilder Footer(string label) { _footerLabel = label; return this; }

        public void Print()
        {
            var (columns, rows) = _db.RunSql(_sql);

            if (!string.IsNullOrEmpty(_title))
                Console.WriteLine($"\n=== {_title} ===");

            string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
            int colCount = displayHeaders.Length;

            int[] widths = _widths.Length >= colCount ? _widths : new int[colCount];
            if (_widths.Length < colCount)
            {
                for (int i = 0; i < colCount; i++) widths[i] = 25;
            }

            for (int i = 0; i < colCount; i++)
                Console.Write(displayHeaders[i].PadRight(widths[i]));
            Console.WriteLine();

            int totalWidth = 0;
            for (int i = 0; i < colCount; i++) totalWidth += widths[i];
            Console.WriteLine(new string('─', totalWidth));

            foreach (var row in rows)
            {
                for (int c = 0; c < row.Length && c < colCount; c++)
                    Console.Write(row[c].PadRight(widths[c]));
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(_footerLabel))
            {
                Console.WriteLine(new string('─', totalWidth));
                Console.WriteLine($"{_footerLabel}: {rows.Count}");
            }
        }
    }
}