using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace StoreApp
{
    public class ReportsForm : Form
    {
        private DataGridView dgv1, dgv2, dgv3;

        public ReportsForm()
        {
            this.Text = "Отчеты";
            this.Size = new System.Drawing.Size(800, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            dgv1 = new DataGridView { Dock = DockStyle.Top, Height = 200 };
            dgv2 = new DataGridView { Dock = DockStyle.Top, Height = 200 };
            dgv3 = new DataGridView { Dock = DockStyle.Top, Height = 200 };

            this.Controls.Add(dgv3);
            this.Controls.Add(dgv2);
            this.Controls.Add(dgv1);

            LoadReports();
        }

        private void LoadReports()
        {
            using var db = new AppDbContext();

            // Отчет 1: Все товары с категориями
            var report1 = db.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .Select(p => new {
                    Товар = p.Name,
                    Цена = p.Price,
                    Категория = p.Category != null ? p.Category.Name : ""
                })
                .ToList();
            dgv1.DataSource = report1;

            // Отчет 2: Количество товаров по категориям
            var report2 = db.Products
                .GroupBy(p => p.Category != null ? p.Category.Name : "Без категории")
                .Select(g => new {
                    Категория = g.Key,
                    Количество = g.Count()
                })
                .OrderBy(r => r.Категория)
                .ToList();
            dgv2.DataSource = report2;

            // Отчет 3: Средняя цена по категориям (исправлено!)
            // Получаем данные, конвертируем decimal в double на стороне клиента
            var products = db.Products
                .Include(p => p.Category)
                .ToList();

            var report3 = products
                .GroupBy(p => p.Category != null ? p.Category.Name : "Без категории")
                .Select(g => new {
                    Категория = g.Key,
                    СредняяЦена = Math.Round(g.Average(p => (double)p.Price), 2)
                })
                .OrderByDescending(r => r.СредняяЦена)
                .ToList();
            dgv3.DataSource = report3;
        }
    }
}