using System;
using System.Linq;
using System.Windows.Forms;

namespace StoreApp
{
    public class CategoriesForm : Form
    {
        private DataGridView dgv;
        private TextBox txtName;
        private Button btnAdd, btnEdit, btnDelete;
        private int? selectedId;

        public CategoriesForm()
        {
            this.Text = "Категории";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;

            dgv = new DataGridView { Dock = DockStyle.Top, Height = 250 };
            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.SelectedRows.Count > 0)
                {
                    selectedId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    txtName.Text = dgv.SelectedRows[0].Cells[1].Value.ToString();
                }
            };

            txtName = new TextBox { Location = new System.Drawing.Point(20, 280), Width = 200 };

            btnAdd = new Button { Text = "Добавить", Location = new System.Drawing.Point(20, 320), Width = 100 };
            btnAdd.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtName.Text))
                {
                    using var db = new AppDbContext();
                    db.Categories.Add(new Category { Name = txtName.Text });
                    db.SaveChanges();
                    LoadData();
                    txtName.Text = "";
                }
            };

            btnEdit = new Button { Text = "Изменить", Location = new System.Drawing.Point(130, 320), Width = 100 };
            btnEdit.Click += (s, e) =>
            {
                if (selectedId.HasValue && !string.IsNullOrWhiteSpace(txtName.Text))
                {
                    using var db = new AppDbContext();
                    var item = db.Categories.Find(selectedId.Value);
                    if (item != null)
                    {
                        item.Name = txtName.Text;
                        db.SaveChanges();
                        LoadData();
                        txtName.Text = "";
                    }
                }
            };

            btnDelete = new Button { Text = "Удалить", Location = new System.Drawing.Point(240, 320), Width = 100 };
            btnDelete.Click += (s, e) =>
            {
                if (selectedId.HasValue)
                {
                    using var db = new AppDbContext();
                    if (db.Products.Any(p => p.CategoryId == selectedId.Value))
                    {
                        MessageBox.Show("Есть товары в этой категории!");
                        return;
                    }
                    var item = db.Categories.Find(selectedId.Value);
                    if (item != null)
                    {
                        db.Categories.Remove(item);
                        db.SaveChanges();
                        LoadData();
                        txtName.Text = "";
                    }
                }
            };

            this.Controls.Add(dgv);
            this.Controls.Add(txtName);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            dgv.DataSource = db.Categories.Select(c => new { c.Id, c.Name }).ToList();
            dgv.Columns["Id"].Visible = false;
        }
    }
}