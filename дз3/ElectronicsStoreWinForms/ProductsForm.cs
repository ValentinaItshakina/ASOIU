using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace StoreApp
{
    public class ProductsForm : Form
    {
        private DataGridView dgv;
        private TextBox txtName, txtPrice;
        private ComboBox cmbCategory;
        private Button btnAdd, btnEdit, btnDelete;
        private int? selectedId;

        public ProductsForm()
        {
            this.Text = "Товары";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            dgv = new DataGridView { Dock = DockStyle.Top, Height = 300 };
            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.SelectedRows.Count > 0)
                {
                    selectedId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    txtName.Text = dgv.SelectedRows[0].Cells[1].Value.ToString();
                    txtPrice.Text = dgv.SelectedRows[0].Cells[2].Value.ToString();
                    string catName = dgv.SelectedRows[0].Cells[3].Value.ToString();
                    foreach (var item in cmbCategory.Items)
                    {
                        var cat = (Category)item;
                        if (cat.Name == catName)
                        {
                            cmbCategory.SelectedItem = cat;
                            break;
                        }
                    }
                }
            };

            txtName = new TextBox { Location = new System.Drawing.Point(20, 320), Width = 150 };
            txtPrice = new TextBox { Location = new System.Drawing.Point(180, 320), Width = 100 };
            cmbCategory = new ComboBox { Location = new System.Drawing.Point(290, 320), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            btnAdd = new Button { Text = "Добавить", Location = new System.Drawing.Point(20, 360), Width = 100 };
            btnAdd.Click += (s, e) =>
            {
                if (ValidateForm())
                {
                    using var db = new AppDbContext();
                    db.Products.Add(new Product
                    {
                        Name = txtName.Text,
                        Price = decimal.Parse(txtPrice.Text),
                        CategoryId = ((Category)cmbCategory.SelectedItem).Id
                    });
                    db.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            };

            btnEdit = new Button { Text = "Изменить", Location = new System.Drawing.Point(130, 360), Width = 100 };
            btnEdit.Click += (s, e) =>
            {
                if (selectedId.HasValue && ValidateForm())
                {
                    using var db = new AppDbContext();
                    var item = db.Products.Find(selectedId.Value);
                    if (item != null)
                    {
                        item.Name = txtName.Text;
                        item.Price = decimal.Parse(txtPrice.Text);
                        item.CategoryId = ((Category)cmbCategory.SelectedItem).Id;
                        db.SaveChanges();
                        LoadData();
                        ClearForm();
                    }
                }
            };

            btnDelete = new Button { Text = "Удалить", Location = new System.Drawing.Point(240, 360), Width = 100 };
            btnDelete.Click += (s, e) =>
            {
                if (selectedId.HasValue && MessageBox.Show("Удалить?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using var db = new AppDbContext();
                    var item = db.Products.Find(selectedId.Value);
                    if (item != null)
                    {
                        db.Products.Remove(item);
                        db.SaveChanges();
                        LoadData();
                        ClearForm();
                    }
                }
            };

            this.Controls.Add(dgv);
            this.Controls.Add(txtName);
            this.Controls.Add(txtPrice);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);

            LoadCategories();
            LoadData();
        }

        private void LoadCategories()
        {
            using var db = new AppDbContext();
            cmbCategory.DataSource = db.Categories.ToList();
            cmbCategory.DisplayMember = "Name";
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            var data = db.Products.Include(p => p.Category)
                .Select(p => new { p.Id, p.Name, p.Price, CategoryName = p.Category != null ? p.Category.Name : "" })
                .ToList();
            dgv.DataSource = data;
            dgv.Columns["Id"].Visible = false;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название");
                return false;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Цена должна быть положительным числом");
                return false;
            }
            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию");
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtPrice.Text = "";
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            selectedId = null;
        }
    }
}