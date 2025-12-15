using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TravelTracker.Data;
using TravelTracker.Models;

namespace TravelTracker.Forms
{
    public partial class MainForm : Form
    {
        private readonly TripRepository _repository;
        private int _currentPage = 1;
        private const int PageSize = 10;

        // Елементи інтерфейсу
        private DataGridView dgvTrips;
        private Button btnAdd, btnEdit, btnDelete, btnSearch, btnReset, btnStats;
        private Button btnPrevPage, btnNextPage;
        private ComboBox cmbCountry, cmbTripType, cmbSort;
        private TextBox txtSearch;
        private DateTimePicker dtpStartDate, dtpEndDate;
        private CheckBox chkUseStartDate, chkUseEndDate;
        private Label lblPageInfo, lblTotalRecords;
        private Panel pnlFilters, pnlButtons, pnlPagination;

        public MainForm()
        {
            _repository = new TripRepository();
            InitializeComponent();
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Трекер Подорожей";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeCustomComponents()
        {
            // Панель фільтрів
            pnlFilters = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(10)
            };

            // Пошук
            var lblSearch = new Label { Text = "Пошук:", Location = new Point(10, 15), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(70, 12), Width = 200 };

            // Країна
            var lblCountry = new Label { Text = "Країна:", Location = new Point(290, 15), AutoSize = true };
            cmbCountry = new ComboBox 
            { 
                Location = new Point(350, 12), 
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadCountries();

            // Тип подорожі
            var lblTripType = new Label { Text = "Тип:", Location = new Point(520, 15), AutoSize = true };
            cmbTripType = new ComboBox 
            { 
                Location = new Point(560, 12), 
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadTripTypes();

            // Дата початку
            chkUseStartDate = new CheckBox { Text = "Від:", Location = new Point(10, 50), AutoSize = true };
            dtpStartDate = new DateTimePicker { Location = new Point(60, 47), Width = 200, Enabled = false };
            chkUseStartDate.CheckedChanged += (s, e) => dtpStartDate.Enabled = chkUseStartDate.Checked;

            // Дата кінця
            chkUseEndDate = new CheckBox { Text = "До:", Location = new Point(290, 50), AutoSize = true };
            dtpEndDate = new DateTimePicker { Location = new Point(340, 47), Width = 200, Enabled = false };
            chkUseEndDate.CheckedChanged += (s, e) => dtpEndDate.Enabled = chkUseEndDate.Checked;

            // Сортування
            var lblSort = new Label { Text = "Сортування:", Location = new Point(10, 85), AutoSize = true };
            cmbSort = new ComboBox 
            { 
                Location = new Point(100, 82), 
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSort.Items.AddRange(new object[] { "Дата ↓", "Дата ↑", "Назва ↓", "Назва ↑", "Вартість ↓", "Вартість ↑", "Рейтинг ↓", "Рейтинг ↑" });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += (s, e) => ApplySort();

            // Кнопки фільтрів
            btnSearch = new Button { Text = "Шукати", Location = new Point(730, 12), Width = 100 };
            btnReset = new Button { Text = "Скинути", Location = new Point(730, 47), Width = 100 };
            btnStats = new Button { Text = "Статистика", Location = new Point(730, 82), Width = 100 };

            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            btnStats.Click += BtnStats_Click;

            pnlFilters.Controls.AddRange(new Control[] 
            { 
                lblSearch, txtSearch, lblCountry, cmbCountry, lblTripType, cmbTripType,
                chkUseStartDate, dtpStartDate, chkUseEndDate, dtpEndDate,
                lblSort, cmbSort, btnSearch, btnReset, btnStats
            });

            // DataGridView
            dgvTrips = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false
            };

            // Панель кнопок
            pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnAdd = new Button { Text = "Додати", Location = new Point(10, 10), Width = 100 };
            btnEdit = new Button { Text = "Редагувати", Location = new Point(120, 10), Width = 100 };
            btnDelete = new Button { Text = "Видалити", Location = new Point(230, 10), Width = 100 };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            // Панель пагінації
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            btnPrevPage = new Button { Text = "◄ Попередня", Location = new Point(10, 8), Width = 120 };
            lblPageInfo = new Label { Text = "Сторінка: 1", Location = new Point(140, 12), AutoSize = true };
            btnNextPage = new Button { Text = "Наступна ►", Location = new Point(250, 8), Width = 120 };
            lblTotalRecords = new Label { Text = "Всього: 0", Location = new Point(400, 12), AutoSize = true };

            btnPrevPage.Click += BtnPrevPage_Click;
            btnNextPage.Click += BtnNextPage_Click;

            pnlPagination.Controls.AddRange(new Control[] { btnPrevPage, lblPageInfo, btnNextPage, lblTotalRecords });

            // Додаємо все на форму
            this.Controls.Add(dgvTrips);
            this.Controls.Add(pnlFilters);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlButtons);

            // Налаштування колонок
            SetupColumns();
        }

        private void SetupColumns()
        {
            dgvTrips.Columns.Clear();

            // ВАЖЛИВО: Name та DataPropertyName мають співпадати!
            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "TripId",
                DataPropertyName = "TripId", 
                HeaderText = "ID", 
                Width = 50 
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Destination",
                DataPropertyName = "Destination", 
                HeaderText = "Місце", 
                Width = 150 
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "CountryName",
                HeaderText = "Країна", 
                Width = 100
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "TripTypeName",
                HeaderText = "Тип", 
                Width = 100
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "AccommodationName",
                HeaderText = "Житло", 
                Width = 100
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "StartDate",
                DataPropertyName = "StartDate", 
                HeaderText = "Початок", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" }
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "EndDate",
                DataPropertyName = "EndDate", 
                HeaderText = "Кінець", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" }
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Budget",
                DataPropertyName = "Budget", 
                HeaderText = "Бюджет", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "ActualCost",
                DataPropertyName = "ActualCost", 
                HeaderText = "Витрачено", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvTrips.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Rating",
                DataPropertyName = "Rating", 
                HeaderText = "Рейтинг", 
                Width = 70 
            });
        }

        private void LoadCountries()
        {
            cmbCountry.Items.Clear();
            cmbCountry.Items.Add(new ComboBoxItem { Text = "Всі країни", Value = 0 });
            
            var countries = _repository.GetAllCountries();
            foreach (var country in countries)
            {
                cmbCountry.Items.Add(new ComboBoxItem { Text = country.Name, Value = country.CountryId });
            }
            cmbCountry.SelectedIndex = 0;
        }

        private void LoadTripTypes()
        {
            cmbTripType.Items.Clear();
            cmbTripType.Items.Add(new ComboBoxItem { Text = "Всі типи", Value = 0 });
            
            var types = _repository.GetAllTripTypes();
            foreach (var type in types)
            {
                cmbTripType.Items.Add(new ComboBoxItem { Text = type.Name, Value = type.TripTypeId });
            }
            cmbTripType.SelectedIndex = 0;
        }

        private void LoadData()
        {
            var trips = _repository.GetTrips(_currentPage, PageSize);
            dgvTrips.Rows.Clear();

            foreach (var trip in trips)
            {
                int rowIndex = dgvTrips.Rows.Add();
                var row = dgvTrips.Rows[rowIndex];
                
                row.Cells["TripId"].Value = trip.TripId;
                row.Cells["Destination"].Value = trip.Destination;
                row.Cells["CountryName"].Value = trip.Country?.Name ?? "";
                row.Cells["TripTypeName"].Value = trip.TripType?.Name ?? "";
                row.Cells["AccommodationName"].Value = trip.Accommodation?.Name ?? "";
                row.Cells["StartDate"].Value = trip.StartDate;
                row.Cells["EndDate"].Value = trip.EndDate;
                row.Cells["Budget"].Value = trip.Budget;
                row.Cells["ActualCost"].Value = trip.ActualCost;
                row.Cells["Rating"].Value = trip.Rating;
                
                row.Tag = trip; // Зберігаємо весь об'єкт для редагування
            }

            UpdatePaginationInfo();
        }

        private void UpdatePaginationInfo()
        {
            int total = _repository.GetTotalCount();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            
            lblPageInfo.Text = $"Сторінка: {_currentPage} з {totalPages}";
            lblTotalRecords.Text = $"Всього: {total}";
            
            btnPrevPage.Enabled = _currentPage > 1;
            btnNextPage.Enabled = _currentPage < totalPages;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            int? countryId = (cmbCountry.SelectedItem as ComboBoxItem)?.Value;
            if (countryId == 0) countryId = null;

            int? tripTypeId = (cmbTripType.SelectedItem as ComboBoxItem)?.Value;
            if (tripTypeId == 0) tripTypeId = null;

            DateTime? startDate = chkUseStartDate.Checked ? dtpStartDate.Value : null;
            DateTime? endDate = chkUseEndDate.Checked ? dtpEndDate.Value : null;
            string searchText = txtSearch.Text;

            var results = _repository.SearchTrips(countryId, tripTypeId, startDate, endDate, searchText);
            
            dgvTrips.Rows.Clear();
            foreach (var trip in results)
            {
                int rowIndex = dgvTrips.Rows.Add();
                var row = dgvTrips.Rows[rowIndex];
                
                row.Cells["TripId"].Value = trip.TripId;
                row.Cells["Destination"].Value = trip.Destination;
                row.Cells["CountryName"].Value = trip.Country?.Name ?? "";
                row.Cells["TripTypeName"].Value = trip.TripType?.Name ?? "";
                row.Cells["AccommodationName"].Value = trip.Accommodation?.Name ?? "";
                row.Cells["StartDate"].Value = trip.StartDate;
                row.Cells["EndDate"].Value = trip.EndDate;
                row.Cells["Budget"].Value = trip.Budget;
                row.Cells["ActualCost"].Value = trip.ActualCost;
                row.Cells["Rating"].Value = trip.Rating;
                
                row.Tag = trip;
            }
            
            lblTotalRecords.Text = $"Знайдено: {results.Count}";
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbCountry.SelectedIndex = 0;
            cmbTripType.SelectedIndex = 0;
            chkUseStartDate.Checked = false;
            chkUseEndDate.Checked = false;
            cmbSort.SelectedIndex = 0;
            _currentPage = 1;
            LoadData();
        }

        private void ApplySort()
        {
            string sortBy = "Date";
            bool ascending = false;

            switch (cmbSort.SelectedIndex)
            {
                case 0: sortBy = "Date"; ascending = false; break;
                case 1: sortBy = "Date"; ascending = true; break;
                case 2: sortBy = "Destination"; ascending = false; break;
                case 3: sortBy = "Destination"; ascending = true; break;
                case 4: sortBy = "Cost"; ascending = false; break;
                case 5: sortBy = "Cost"; ascending = true; break;
                case 6: sortBy = "Rating"; ascending = false; break;
                case 7: sortBy = "Rating"; ascending = true; break;
            }

            var sorted = _repository.GetSortedTrips(sortBy, ascending);
            
            dgvTrips.Rows.Clear();
            foreach (var trip in sorted)
            {
                int rowIndex = dgvTrips.Rows.Add();
                var row = dgvTrips.Rows[rowIndex];
                
                row.Cells["TripId"].Value = trip.TripId;
                row.Cells["Destination"].Value = trip.Destination;
                row.Cells["CountryName"].Value = trip.Country?.Name ?? "";
                row.Cells["TripTypeName"].Value = trip.TripType?.Name ?? "";
                row.Cells["AccommodationName"].Value = trip.Accommodation?.Name ?? "";
                row.Cells["StartDate"].Value = trip.StartDate;
                row.Cells["EndDate"].Value = trip.EndDate;
                row.Cells["Budget"].Value = trip.Budget;
                row.Cells["ActualCost"].Value = trip.ActualCost;
                row.Cells["Rating"].Value = trip.Rating;
                
                row.Tag = trip;
            }
        }

        private void BtnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            int total = _repository.GetTotalCount();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadData();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new TripForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTrips.SelectedRows.Count > 0)
            {
                var trip = dgvTrips.SelectedRows[0].Tag as Trip;
                if (trip != null)
                {
                    var form = new TripForm(trip.TripId);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Оберіть подорож для редагування!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTrips.SelectedRows.Count > 0)
            {
                var trip = dgvTrips.SelectedRows[0].Tag as Trip;
                if (trip != null)
                {
                    var result = MessageBox.Show($"Видалити подорож до '{trip.Destination}'?", "Підтвердження", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        _repository.DeleteTrip(trip.TripId);
                        LoadData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Оберіть подорож для видалення!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnStats_Click(object sender, EventArgs e)
        {
            var stats = _repository.GetStatistics();
            string message = $"Загальна статистика:\n\n" +
                           $"Всього подорожей: {stats.TotalTrips}\n" +
                           $"Відвідано країн: {stats.CountriesVisited}\n" +
                           $"Витрачено: {stats.TotalSpent:N2} грн\n" +
                           $"Середній рейтинг: {stats.AverageRating:F1}\n" +
                           $"Найпопулярніша країна: {stats.MostVisitedCountry}";

            MessageBox.Show(message, "Статистика", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Допоміжний клас для ComboBox
    public class ComboBoxItem
    {
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }

        public override string ToString() => Text;
    }
}