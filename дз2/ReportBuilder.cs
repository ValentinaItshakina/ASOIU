using System;
using System.Collections.Generic;
using System.Text;

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

        public ReportBuilder(DatabaseManager db)
        {
            _db = db;
        }

        public ReportBuilder Query(string sql)
        {
            _sql = sql;
            return this;
        }

        public ReportBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ReportBuilder Header(params string[] columns)
        {
            _headers = columns;
            return this;
        }

        public ReportBuilder ColumnWidths(params int[] widths)
        {
            _widths = widths;
            return this;
        }


        public ReportBuilder Footer(string label)
        {
            _footerLabel = label;
            return this;
        }

        public string Build()
        {
            var (columns, rows) = _db.ExecuteQuery(_sql);
            var sb = new StringBuilder();


            if (!string.IsNullOrEmpty(_title))
            {
                sb.AppendLine();
                sb.AppendLine($"=== {_title} ===");
            }


            string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
            int colCount = displayHeaders.Length;

            int[] widths = new int[colCount];
            if (_widths.Length >= colCount)
                widths = _widths;
            else
                for (int i = 0; i < colCount; i++) widths[i] = 25; 


            for (int i = 0; i < colCount; i++)
                sb.Append(displayHeaders[i].PadRight(widths[i]));
            sb.AppendLine();


            int totalTableWidth = 0;
            for (int i = 0; i < colCount; i++) totalTableWidth += widths[i];

            sb.AppendLine(new string('─', totalTableWidth));


            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Length && c < colCount; c++)
                {
                    sb.Append(rows[r][c].PadRight(widths[c]));
                }
                sb.AppendLine();
            }

            
            if (!string.IsNullOrEmpty(_footerLabel))
            {
                sb.AppendLine(new string('─', totalTableWidth));
                sb.AppendLine($"{_footerLabel}: {rows.Count}");
            }

            return sb.ToString();
        }

        
        public void Print()
        {
            Console.Write(Build());
        }
    }
}