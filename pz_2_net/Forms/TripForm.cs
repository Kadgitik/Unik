using System;
using System.Drawing;
using System.Windows.Forms;
using TravelTracker.Data;
using TravelTracker.Models;

namespace TravelTracker.Forms
{
    public partial class TripForm : Form
    {
        private readonly TripRepository _repository;
        private readonly int? _tripId;
        private Trip? _currentTrip;

        // Елементи інтерфейсу
        private TextBox txtDestination, txtBudget, txtActualCost, txtNotes;
        private ComboBox cmbCountry, cmbTripType, cmbAccommodation;
        private DateTimePicker dtpStartDate, dtpEndDate;
        private NumericUpDown nudRating;
        private Button btnSave, btnCancel;

        public TripForm(int? tripId = null)
        {
            _repository = new TripRepository();
            _tripId = tripId;
            
            InitializeComponent();
            InitializeCustomComponents();
            LoadComboBoxes();

            if (_tripId.HasValue)
            {
                LoadTripData();
            }
        }

        private void InitializeComponent()
        {
            this.Text = _tripId.HasValue ? "Редагувати подорож" : "Додати подорож";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeCustomComponents()
        {
            int labelWidth = 120;
            int controlWidth = 320;
            int leftLabel = 20;
            int leftControl = 150;
            int top = 20;
            int spacing = 40;

            // Місце призначення
            var lblDestination = new Label 
            { 
                Text = "Місце призначення:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            txtDestination = new TextBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth 
            };
            top += spacing;

            // Країна
            var lblCountry = new Label 
            { 
                Text = "Країна:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            cmbCountry = new ComboBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            top += spacing;

            // Тип подорожі
            var lblTripType = new Label 
            { 
                Text = "Тип подорожі:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            cmbTripType = new ComboBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            top += spacing;

            // Житло
            var lblAccommodation = new Label 
            { 
                Text = "Тип житла:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            cmbAccommodation = new ComboBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            top += spacing;

            // Дата початку
            var lblStartDate = new Label 
            { 
                Text = "Дата початку:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            dtpStartDate = new DateTimePicker 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                Format = DateTimePickerFormat.Short
            };
            top += spacing;

            // Дата кінця
            var lblEndDate = new Label 
            { 
                Text = "Дата кінця:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            dtpEndDate = new DateTimePicker 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                Format = DateTimePickerFormat.Short
            };
            top += spacing;

            // Бюджет
            var lblBudget = new Label 
            { 
                Text = "Бюджет (грн):", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            txtBudget = new TextBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                Text = "0"
            };
            top += spacing;

            // Фактична вартість
            var lblActualCost = new Label 
            { 
                Text = "Витрачено (грн):", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            txtActualCost = new TextBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                Text = "0"
            };
            top += spacing;

            // Рейтинг
            var lblRating = new Label 
            { 
                Text = "Рейтинг (1-10):", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            nudRating = new NumericUpDown 
            { 
                Location = new Point(leftControl, top), 
                Width = 100,
                Minimum = 1,
                Maximum = 10,
                Value = 5
            };
            top += spacing;

            // Нотатки
            var lblNotes = new Label 
            { 
                Text = "Нотатки:", 
                Location = new Point(leftLabel, top), 
                Width = labelWidth 
            };
            txtNotes = new TextBox 
            { 
                Location = new Point(leftControl, top), 
                Width = controlWidth,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            top += 90;

            // Кнопки
            btnSave = new Button 
            { 
                Text = "Зберегти", 
                Location = new Point(leftControl, top), 
                Width = 100,
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button 
            { 
                Text = "Скасувати", 
                Location = new Point(leftControl + 110, top), 
                Width = 100,
                DialogResult = DialogResult.Cancel
            };

            // Додаємо контроли на форму
            this.Controls.AddRange(new Control[]
            {
                lblDestination, txtDestination,
                lblCountry, cmbCountry,
                lblTripType, cmbTripType,
                lblAccommodation, cmbAccommodation,
                lblStartDate, dtpStartDate,
                lblEndDate, dtpEndDate,
                lblBudget, txtBudget,
                lblActualCost, txtActualCost,
                lblRating, nudRating,
                lblNotes, txtNotes,
                btnSave, btnCancel
            });

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadComboBoxes()
        {
            // Завантаження країн
            var countries = _repository.GetAllCountries();
            foreach (var country in countries)
            {
                cmbCountry.Items.Add(new ComboBoxItem 
                { 
                    Text = country.Name, 
                    Value = country.CountryId 
                });
            }
            if (cmbCountry.Items.Count > 0) cmbCountry.SelectedIndex = 0;

            // Завантаження типів подорожей
            var tripTypes = _repository.GetAllTripTypes();
            foreach (var type in tripTypes)
            {
                cmbTripType.Items.Add(new ComboBoxItem 
                { 
                    Text = type.Name, 
                    Value = type.TripTypeId 
                });
            }
            if (cmbTripType.Items.Count > 0) cmbTripType.SelectedIndex = 0;

            // Завантаження типів житла
            var accommodations = _repository.GetAllAccommodations();
            foreach (var accommodation in accommodations)
            {
                cmbAccommodation.Items.Add(new ComboBoxItem 
                { 
                    Text = accommodation.Name, 
                    Value = accommodation.AccommodationId 
                });
            }
            if (cmbAccommodation.Items.Count > 0) cmbAccommodation.SelectedIndex = 0;
        }

        private void LoadTripData()
        {
            _currentTrip = _repository.GetTripById(_tripId!.Value);
            if (_currentTrip != null)
            {
                txtDestination.Text = _currentTrip.Destination;
                
                // Встановлюємо вибрану країну
                for (int i = 0; i < cmbCountry.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbCountry.Items[i]).Value == _currentTrip.CountryId)
                    {
                        cmbCountry.SelectedIndex = i;
                        break;
                    }
                }

                // Встановлюємо вибраний тип
                for (int i = 0; i < cmbTripType.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbTripType.Items[i]).Value == _currentTrip.TripTypeId)
                    {
                        cmbTripType.SelectedIndex = i;
                        break;
                    }
                }

                // Встановлюємо вибране житло
                for (int i = 0; i < cmbAccommodation.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbAccommodation.Items[i]).Value == _currentTrip.AccommodationId)
                    {
                        cmbAccommodation.SelectedIndex = i;
                        break;
                    }
                }

                dtpStartDate.Value = _currentTrip.StartDate;
                dtpEndDate.Value = _currentTrip.EndDate;
                txtBudget.Text = _currentTrip.Budget.ToString();
                txtActualCost.Text = _currentTrip.ActualCost.ToString();
                nudRating.Value = _currentTrip.Rating;
                txtNotes.Text = _currentTrip.Notes;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валідація
            if (string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                MessageBox.Show("Введіть місце призначення!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dtpEndDate.Value < dtpStartDate.Value)
            {
                MessageBox.Show("Дата кінця не може бути раніше дати початку!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtBudget.Text, out decimal budget) || budget < 0)
            {
                MessageBox.Show("Введіть коректний бюджет!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtActualCost.Text, out decimal actualCost) || actualCost < 0)
            {
                MessageBox.Show("Введіть коректну суму витрат!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var trip = new Trip
                {
                    TripId = _tripId ?? 0,
                    Destination = txtDestination.Text.Trim(),
                    CountryId = ((ComboBoxItem)cmbCountry.SelectedItem).Value,
                    TripTypeId = ((ComboBoxItem)cmbTripType.SelectedItem).Value,
                    AccommodationId = ((ComboBoxItem)cmbAccommodation.SelectedItem).Value,
                    StartDate = dtpStartDate.Value,
                    EndDate = dtpEndDate.Value,
                    Budget = budget,
                    ActualCost = actualCost,
                    Rating = (int)nudRating.Value,
                    Notes = txtNotes.Text.Trim()
                };

                if (_tripId.HasValue)
                {
                    // Оновлення
                    trip.CreatedAt = _currentTrip!.CreatedAt;
                    _repository.UpdateTrip(trip);
                    MessageBox.Show("Подорож успішно оновлено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Додавання
                    _repository.AddTrip(trip);
                    MessageBox.Show("Подорож успішно додано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}