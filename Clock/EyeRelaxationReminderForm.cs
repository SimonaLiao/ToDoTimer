using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clock
{
    public partial class EyeRelaxationReminderForm : Form
    {
        private System.Windows.Forms.Timer confettiTimer = null!;
        private System.Windows.Forms.Timer autoCloseTimer = null!;
        private List<ConfettiLabel> confettiLabels = null!;
        private Random random = new Random();
        private Label reminderLabel = null!;
        private Label instructionLabel = null!;
        private Button okButton = null!;
        private int confettiDuration = 5000; // 5 seconds of confetti

        public EyeRelaxationReminderForm()
        {
            InitializeComponent();
            InitializeConfetti();
            StartConfettiEffect();
            StartAutoCloseTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Eye Relaxation Reminder ????";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(245, 255, 250);

            // Reminder label
            reminderLabel = new Label
            {
                Text = "?? Focus Time Complete! ??",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                AutoSize = true,
                Location = new Point(100, 80),
                BackColor = Color.Transparent
            };

            // Try to set emoji text, fall back if needed
            if (!CanDisplayEmoji())
            {
                reminderLabel.Text = "? Focus Time Complete! ?";
            }

            // Instruction label
            instructionLabel = new Label
            {
                Text = GetInstructionText(),
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(60, 120, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(500, 150),
                Location = new Point(50, 140),
                BackColor = Color.Transparent
            };

            // OK button
            okButton = new Button
            {
                Text = GetOkButtonText(),
                Font = new Font("Segoe UI", 12),
                Size = new Size(200, 40),
                Location = new Point(200, 310),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            okButton.Click += OkButton_Click;

            // Add controls
            this.Controls.AddRange(new Control[] { reminderLabel, instructionLabel, okButton });
        }

        private bool CanDisplayEmoji()
        {
            // Simple test to see if emoji can be displayed
            try
            {
                using (var testFont = new Font("Segoe UI Emoji", 12))
                {
                    return testFont.Name.Contains("Emoji") || testFont.Name.Contains("Segoe UI");
                }
            }
            catch
            {
                return false;
            }
        }

        private string GetInstructionText()
        {
            if (CanDisplayEmoji())
            {
                return "Time to relax your eyes! ??\n\nLook outside or at something far away\nfor 20 seconds to rest your eyes.\n\n?? Take a moment to appreciate nature! ??";
            }
            else
            {
                return "Time to relax your eyes!\n\nLook outside or at something far away\nfor 20 seconds to rest your eyes.\n\n? Take a moment to appreciate nature! ?";
            }
        }

        private string GetOkButtonText()
        {
            if (CanDisplayEmoji())
            {
                return "? Thanks for the reminder!";
            }
            else
            {
                return "? Thanks for the reminder!";
            }
        }

        private void InitializeConfetti()
        {
            confettiLabels = new List<ConfettiLabel>();
            
            confettiTimer = new System.Windows.Forms.Timer
            {
                Interval = 50 // Update every 50ms for smooth animation
            };
            confettiTimer.Tick += ConfettiTimer_Tick;

            autoCloseTimer = new System.Windows.Forms.Timer
            {
                Interval = confettiDuration
            };
            autoCloseTimer.Tick += AutoCloseTimer_Tick;
        }

        private void StartConfettiEffect()
        {
            // Create initial confetti pieces
            for (int i = 0; i < 20; i++)
            {
                CreateNewConfettiLabel();
            }
            confettiTimer.Start();
        }

        private void StartAutoCloseTimer()
        {
            autoCloseTimer.Start();
        }

        private void CreateNewConfettiLabel()
        {
            var emoji = GetRandomTreeEmoji();
            
            // Try different font approaches for better emoji support
            Font emojiFont;
            try
            {
                emojiFont = new Font("Segoe UI Emoji", 24, FontStyle.Regular);
            }
            catch
            {
                try
                {
                    emojiFont = new Font("Microsoft YaHei UI", 24, FontStyle.Regular);
                }
                catch
                {
                    emojiFont = new Font("Segoe UI", 24, FontStyle.Regular);
                }
            }

            var label = new Label
            {
                Text = emoji,
                Font = emojiFont,
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.Green,
                Location = new Point(random.Next(0, this.Width - 30), -50)
            };

            var confettiLabel = new ConfettiLabel
            {
                Label = label,
                VelocityX = (random.NextDouble() - 0.5) * 4,
                VelocityY = random.NextDouble() * 3 + 2,
                RotationSpeed = (random.NextDouble() - 0.5) * 10
            };

            confettiLabels.Add(confettiLabel);
            this.Controls.Add(label);
            label.BringToFront();
        }

        private string GetRandomTreeEmoji()
        {
            // Test if we can display emoji properly
            if (CanDisplayEmoji())
            {
                // Use real emoji - tree and nature related (direct emoji characters)
                var emojis = new[] { "??", "??", "??", "??", "??", "??" };
                return emojis[random.Next(emojis.Length)];
            }
            else
            {
                // Use Unicode symbols that are more widely supported
                var symbols = new[] { "?", "?", "?", "?", "?", "?", "?", "?" };
                return symbols[random.Next(symbols.Length)];
            }
        }

        private void ConfettiTimer_Tick(object sender, EventArgs e)
        {
            // Update confetti positions
            for (int i = confettiLabels.Count - 1; i >= 0; i--)
            {
                var confetti = confettiLabels[i];
                var label = confetti.Label;

                // Update position
                var newX = label.Left + (int)confetti.VelocityX;
                var newY = label.Top + (int)confetti.VelocityY;
                
                label.Location = new Point(newX, newY);

                // Add gravity
                confetti.VelocityY += 0.1;

                // Remove if off screen
                if (newY > this.Height + 50 || newX < -50 || newX > this.Width + 50)
                {
                    this.Controls.Remove(label);
                    label.Dispose();
                    confettiLabels.RemoveAt(i);
                }
            }

            // Occasionally add new pieces
            if (random.NextDouble() < 0.2 && confettiLabels.Count < 25)
            {
                CreateNewConfettiLabel();
            }
        }

        private void AutoCloseTimer_Tick(object sender, EventArgs e)
        {
            // Stop confetti after the duration
            confettiTimer.Stop();
            autoCloseTimer.Stop();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            confettiTimer?.Stop();
            autoCloseTimer?.Stop();
            
            // Clean up confetti labels
            foreach (var confetti in confettiLabels)
            {
                confetti.Label.Dispose();
            }
            confettiLabels.Clear();
            
            confettiTimer?.Dispose();
            autoCloseTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }

    public class ConfettiLabel
    {
        public Label Label { get; set; } = null!;
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public double RotationSpeed { get; set; }
    }
}