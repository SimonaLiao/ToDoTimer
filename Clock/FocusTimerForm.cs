using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clock
{
    public partial class FocusTimerForm : Form
    {
        private System.Windows.Forms.Timer focusTimer = null!;
        private System.Windows.Forms.Timer displayTimer = null!;
        private Label timeDisplayLabel = null!;
        private Label focusTimeLabel = null!;
        private NumericUpDown focusTimeInput = null!;
        private Button startButton = null!;
        private Button stopButton = null!;
        private Button resetButton = null!;
        private int focusTimeMinutes = 25; // Default 25 minutes (Pomodoro technique)
        private DateTime startTime;
        private bool isRunning = false;

        public FocusTimerForm()
        {
            InitializeComponent();
            InitializeFocusTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Focus Timer ?";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 248, 255);

            // Time display label
            timeDisplayLabel = new Label
            {
                Text = "25:00",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(120, 50)
            };

            // Focus time setting label
            focusTimeLabel = new Label
            {
                Text = "Focus Time (minutes):",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(50, 140)
            };

            // Focus time input
            focusTimeInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 120,
                Value = 25,
                Location = new Point(220, 138),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 12)
            };
            focusTimeInput.ValueChanged += FocusTimeInput_ValueChanged;

            // Start button - use fallback text if emoji doesn't work
            startButton = new Button
            {
                Text = "? Start",  // Using triangle symbol instead of rocket emoji
                Font = new Font("Segoe UI", 12),
                Size = new Size(80, 40),
                Location = new Point(50, 200),
                BackColor = Color.FromArgb(50, 205, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            startButton.Click += StartButton_Click;

            // Stop button - using pause symbol
            stopButton = new Button
            {
                Text = "? Stop",  // Using actual pause symbol
                Font = new Font("Segoe UI", 12),
                Size = new Size(80, 40),
                Location = new Point(160, 200),
                BackColor = Color.FromArgb(255, 69, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            stopButton.Click += StopButton_Click;

            // Reset button - using circular arrow
            resetButton = new Button
            {
                Text = "? Reset",  // Using refresh symbol
                Font = new Font("Segoe UI", 12),
                Size = new Size(80, 40),
                Location = new Point(270, 200),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            resetButton.Click += ResetButton_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[] 
            {
                timeDisplayLabel, focusTimeLabel, focusTimeInput,
                startButton, stopButton, resetButton
            });
        }

        private void InitializeFocusTimer()
        {
            // Timer for checking if focus time is reached
            focusTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // Check every second
            };
            focusTimer.Tick += FocusTimer_Tick;

            // Timer for updating display
            displayTimer = new System.Windows.Forms.Timer
            {
                Interval = 100 // Update display every 100ms for smooth countdown
            };
            displayTimer.Tick += DisplayTimer_Tick;
        }

        private void FocusTimeInput_ValueChanged(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                focusTimeMinutes = (int)focusTimeInput.Value;
                UpdateTimeDisplay();
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                focusTimeMinutes = (int)focusTimeInput.Value;
                startTime = DateTime.Now;
                isRunning = true;
                
                focusTimer.Start();
                displayTimer.Start();
                
                startButton.Enabled = false;
                stopButton.Enabled = true;
                focusTimeInput.Enabled = false;
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            StopTimer();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            StopTimer();
            UpdateTimeDisplay();
        }

        private void StopTimer()
        {
            isRunning = false;
            focusTimer.Stop();
            displayTimer.Stop();
            
            startButton.Enabled = true;
            stopButton.Enabled = false;
            focusTimeInput.Enabled = true;
        }

        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void FocusTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - startTime;
            var targetTime = TimeSpan.FromMinutes(focusTimeMinutes);

            if (elapsed >= targetTime)
            {
                StopTimer();
                ShowEyeRelaxationReminder();
            }
        }

        private void UpdateTimeDisplay()
        {
            if (isRunning)
            {
                var elapsed = DateTime.Now - startTime;
                var remaining = TimeSpan.FromMinutes(focusTimeMinutes) - elapsed;
                
                if (remaining <= TimeSpan.Zero)
                {
                    timeDisplayLabel.Text = "00:00";
                    timeDisplayLabel.ForeColor = Color.Red;
                }
                else
                {
                    timeDisplayLabel.Text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                    timeDisplayLabel.ForeColor = remaining.TotalMinutes <= 5 ? Color.Orange : Color.FromArgb(70, 130, 180);
                }
            }
            else
            {
                timeDisplayLabel.Text = $"{focusTimeMinutes:D2}:00";
                timeDisplayLabel.ForeColor = Color.FromArgb(70, 130, 180);
            }
        }

        private void ShowEyeRelaxationReminder()
        {
            var reminderForm = new EyeRelaxationReminderForm();
            reminderForm.ShowDialog();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            focusTimer?.Stop();
            displayTimer?.Stop();
            focusTimer?.Dispose();
            displayTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}