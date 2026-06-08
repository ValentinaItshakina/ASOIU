using System;
using System.Windows.Forms;

namespace StoreApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            this.Text = "Главное меню";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnCategories = new Button
            {
                Text = "Категории",
                Location = new System.Drawing.Point(100, 100),
                Size = new System.Drawing.Size(300, 60),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnCategories.Click += (s, e) => new CategoriesForm().ShowDialog();

            var btnProducts = new Button
            {
                Text = "Товары",
                Location = new System.Drawing.Point(100, 180),
                Size = new System.Drawing.Size(300, 60),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnProducts.Click += (s, e) => new ProductsForm().ShowDialog();

            var btnReports = new Button
            {
                Text = "Отчеты",
                Location = new System.Drawing.Point(100, 260),
                Size = new System.Drawing.Size(300, 60),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnReports.Click += (s, e) => new ReportsForm().ShowDialog();

            this.Controls.Add(btnCategories);
            this.Controls.Add(btnProducts);
            this.Controls.Add(btnReports);
        }
    }
}