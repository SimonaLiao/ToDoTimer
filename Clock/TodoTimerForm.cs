using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clock
{
    public partial class TodoTimerForm : Form
    {
        private List<TodoTask> todoTasks = new List<TodoTask>();
        private int nextTaskId = 1;
        private TodoTask? currentTask = null;
        
        // Timer controls
        private System.Windows.Forms.Timer focusTimer = null!;
        private System.Windows.Forms.Timer displayTimer = null!;
        private DateTime startTime;
        private bool isRunning = false;
        private int focusTimeMinutes = 25;

        // UI Controls
        private TabControl mainTabControl = null!;
        private TabPage todoTabPage = null!;
        private TabPage timerTabPage = null!;
        
        // Todo Tab Controls
        private TextBox taskNameInput = null!;
        private DateTimePicker deadlinePicker = null!;
        private CheckBox hasDeadlineCheckBox = null!;
        private Button addTaskButton = null!;
        private ListBox taskListBox = null!;
        private Button workOnTaskButton = null!;
        private Button completeTaskButton = null!;
        private Button deleteTaskButton = null!;
        
        // Timer Tab Controls
        private Label timeDisplayLabel = null!;
        private Label currentTaskLabel = null!;
        private NumericUpDown focusTimeInput = null!;
        private Button startButton = null!;
        private Button stopButton = null!;
        private Button resetButton = null!;

        public TodoTimerForm()
        {
            InitializeComponent();
            InitializeTimers();
            LoadSampleTasks(); // For demonstration
        }

        private void InitializeComponent()
        {
            this.Text = "Todo Timer App";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(700, 550);
            this.BackColor = Color.FromArgb(248, 250, 252); // Modern light background

            // Create main tab control with modern styling
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ItemSize = new Size(120, 45),
                Padding = new Point(20, 8)
            };

            // Create todo tab with modern styling
            todoTabPage = new TabPage("Tasks")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Font = new Font("Segoe UI", 10)
            };

            // Create timer tab with modern styling  
            timerTabPage = new TabPage("Timer")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Font = new Font("Segoe UI", 10)
            };

            InitializeTodoTab();
            InitializeTimerTab();

            mainTabControl.TabPages.Add(todoTabPage);
            mainTabControl.TabPages.Add(timerTabPage);
            this.Controls.Add(mainTabControl);
        }

        private void InitializeTodoTab()
        {
            // Create a modern card-like container for the input section with border
            var inputPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(630, 90),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Task name input with modern styling
            var taskNameLabel = new Label
            {
                Text = "Task Name",
                Location = new Point(15, 15),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                BackColor = Color.Transparent
            };

            taskNameInput = new TextBox
            {
                Location = new Point(15, 35),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "What do you need to work on?",
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39)
            };

            // Deadline controls with modern styling
            hasDeadlineCheckBox = new CheckBox
            {
                Text = "Set Deadline",
                Location = new Point(330, 15),
                Size = new Size(110, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat
            };
            hasDeadlineCheckBox.CheckedChanged += HasDeadlineCheckBox_CheckedChanged;

            deadlinePicker = new DateTimePicker
            {
                Location = new Point(330, 35),
                Size = new Size(180, 25),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Font = new Font("Segoe UI", 10),
                Enabled = false
            };

            // Modern add task button
            addTaskButton = new Button
            {
                Text = "+ Add Task",
                Location = new Point(520, 30),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(59, 130, 246), // Modern blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            addTaskButton.FlatAppearance.BorderSize = 0;
            addTaskButton.Click += AddTaskButton_Click;

            // Add controls to input panel
            inputPanel.Controls.AddRange(new Control[]
            {
                taskNameLabel, taskNameInput, hasDeadlineCheckBox, deadlinePicker, addTaskButton
            });

            // Task list section
            var taskListLabel = new Label
            {
                Text = "Your Tasks",
                Location = new Point(20, 130),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                BackColor = Color.Transparent
            };

            taskListBox = new ListBox
            {
                Location = new Point(20, 160),
                Size = new Size(630, 240),
                Font = new Font("Segoe UI", 10),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 50,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SelectionMode = SelectionMode.One
            };
            taskListBox.DrawItem += TaskListBox_DrawItem;
            taskListBox.SelectedIndexChanged += TaskListBox_SelectedIndexChanged;

            // Modern action buttons with better spacing
            workOnTaskButton = new Button
            {
                Text = "? Work On This",
                Location = new Point(20, 420),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(16, 185, 129), // Modern emerald
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            workOnTaskButton.FlatAppearance.BorderSize = 0;
            workOnTaskButton.Click += WorkOnTaskButton_Click;

            completeTaskButton = new Button
            {
                Text = "? Complete",
                Location = new Point(180, 420),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(34, 197, 94), // Modern green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            completeTaskButton.FlatAppearance.BorderSize = 0;
            completeTaskButton.Click += CompleteTaskButton_Click;

            deleteTaskButton = new Button
            {
                Text = "? Delete",
                Location = new Point(320, 420),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(239, 68, 68), // Modern red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            deleteTaskButton.FlatAppearance.BorderSize = 0;
            deleteTaskButton.Click += DeleteTaskButton_Click;

            // Add all controls directly to todo tab (no shadow panel)
            todoTabPage.Controls.AddRange(new Control[]
            {
                inputPanel, taskListLabel, taskListBox,
                workOnTaskButton, completeTaskButton, deleteTaskButton
            });
        }

        private void InitializeTimerTab()
        {
            // Create modern card for timer display with border instead of shadow
            var timerCard = new Panel
            {
                Location = new Point(50, 40),
                Size = new Size(580, 380),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Current task display with modern styling
            currentTaskLabel = new Label
            {
                Text = "No task selected",
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(20, 20),
                Size = new Size(540, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Large, modern time display
            timeDisplayLabel = new Label
            {
                Text = "25:00",
                Font = new Font("Segoe UI", 72, FontStyle.Regular),
                ForeColor = Color.FromArgb(59, 130, 246), // Modern blue
                Location = new Point(20, 70),
                Size = new Size(540, 120),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Focus time setting with modern layout
            var focusTimeContainer = new Panel
            {
                Location = new Point(20, 210),
                Size = new Size(540, 50),
                BackColor = Color.Transparent
            };

            var focusTimeLabel = new Label
            {
                Text = "Focus Duration",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(170, 8),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            focusTimeInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 120,
                Value = 25,
                Location = new Point(300, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.FixedSingle
            };
            focusTimeInput.ValueChanged += FocusTimeInput_ValueChanged;

            var minutesLabel = new Label
            {
                Text = "min",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(390, 8),
                Size = new Size(40, 25),
                BackColor = Color.Transparent
            };

            focusTimeContainer.Controls.AddRange(new Control[] { focusTimeLabel, focusTimeInput, minutesLabel });

            // Modern timer control buttons with better spacing
            var buttonPanel = new Panel
            {
                Location = new Point(20, 280),
                Size = new Size(540, 50),
                BackColor = Color.Transparent
            };

            startButton = new Button
            {
                Text = "▶ Start",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(120, 45),
                Location = new Point(90, 0),
                BackColor = Color.FromArgb(34, 197, 94), // Modern green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += StartButton_Click;

            stopButton = new Button
            {
                Text = "⏸ Pause",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(120, 45),
                Location = new Point(220, 0),
                BackColor = Color.FromArgb(245, 158, 11), // Modern amber
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            stopButton.FlatAppearance.BorderSize = 0;
            stopButton.Click += StopButton_Click;

            resetButton = new Button
            {
                Text = "↻ Reset",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(120, 45),
                Location = new Point(350, 0),
                BackColor = Color.FromArgb(107, 114, 128), // Modern gray
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            resetButton.FlatAppearance.BorderSize = 0;
            resetButton.Click += ResetButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { startButton, stopButton, resetButton });

            // Add all controls to timer card
            timerCard.Controls.AddRange(new Control[]
            {
                currentTaskLabel, timeDisplayLabel, focusTimeContainer, buttonPanel
            });

            // Add timer card directly to tab (no shadow)
            timerTabPage.Controls.Add(timerCard);
        }

        private void InitializeTimers()
        {
            focusTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            focusTimer.Tick += FocusTimer_Tick;

            displayTimer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            displayTimer.Tick += DisplayTimer_Tick;
        }

        private void LoadSampleTasks()
        {
            // Add some sample tasks for demonstration
            todoTasks.Add(new TodoTask
            {
                Id = nextTaskId++,
                Name = "Complete project proposal",
                Deadline = DateTime.Today.AddDays(2)
            });

            todoTasks.Add(new TodoTask
            {
                Id = nextTaskId++,
                Name = "Review code changes",
                Deadline = DateTime.Today.AddDays(1)
            });

            todoTasks.Add(new TodoTask
            {
                Id = nextTaskId++,
                Name = "Learn new programming concepts"
            });

            RefreshTaskList();
        }

        // Event handlers continued...
        
        private void HasDeadlineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            deadlinePicker.Enabled = hasDeadlineCheckBox.Checked;
            if (!hasDeadlineCheckBox.Checked)
            {
                deadlinePicker.Value = DateTime.Today;
            }
        }

        private void AddTaskButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(taskNameInput.Text))
            {
                MessageBox.Show("Please enter a task name.", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newTask = new TodoTask
            {
                Id = nextTaskId++,
                Name = taskNameInput.Text.Trim(),
                Deadline = hasDeadlineCheckBox.Checked ? deadlinePicker.Value.Date : null
            };

            todoTasks.Add(newTask);
            RefreshTaskList();

            // Clear input fields
            taskNameInput.Clear();
            hasDeadlineCheckBox.Checked = false;
            deadlinePicker.Value = DateTime.Today;
            taskNameInput.Focus();
        }

        private void TaskListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= todoTasks.Count) return;

            var task = todoTasks[e.Index];
            var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // Create modern card-like appearance for each task
            var itemRect = e.Bounds;
            var cardRect = new Rectangle(itemRect.X + 5, itemRect.Y + 3, itemRect.Width - 10, itemRect.Height - 6);

            // Draw background with modern styling
            using (var backgroundBrush = new SolidBrush(isSelected ? 
                Color.FromArgb(219, 234, 254) : task.GetBackgroundColor())) // Light blue when selected
            {
                e.Graphics.FillRectangle(backgroundBrush, cardRect);
            }

            // Draw subtle border
            using (var borderPen = new Pen(isSelected ? 
                Color.FromArgb(59, 130, 246) : Color.FromArgb(229, 231, 235), 1))
            {
                e.Graphics.DrawRectangle(borderPen, cardRect);
            }

            // Draw task text with modern typography
            var textColor = task.GetDeadlineColor();
            var font = task.IsCompleted ? 
                new Font("Segoe UI", 11, FontStyle.Strikeout) : 
                new Font("Segoe UI", 11, FontStyle.Regular);

            using (var textBrush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(cardRect.X + 15, cardRect.Y + 8, cardRect.Width - 30, 20);
                e.Graphics.DrawString(task.Name, font, textBrush, textRect);

                // Draw deadline info on second line if it exists
                if (task.Deadline.HasValue)
                {
                    var deadlineFont = new Font("Segoe UI", 9, FontStyle.Regular);
                    var deadlineColor = task.IsCompleted ? 
                        Color.FromArgb(156, 163, 175) : // Light gray for completed
                        Color.FromArgb(107, 114, 128);   // Medium gray for active
                    
                    using (var deadlineBrush = new SolidBrush(deadlineColor))
                    {
                        var deadlineRect = new Rectangle(cardRect.X + 15, cardRect.Y + 28, cardRect.Width - 30, 15);
                        var deadlineText = $"Due: {task.Deadline.Value:MMM dd, yyyy}";
                        e.Graphics.DrawString(deadlineText, deadlineFont, deadlineBrush, deadlineRect);
                    }
                    deadlineFont.Dispose();
                }

                // Draw completion checkmark or status indicator
                if (task.IsCompleted)
                {
                    var checkRect = new Rectangle(cardRect.Right - 35, cardRect.Y + 10, 20, 20);
                    using (var checkBrush = new SolidBrush(Color.FromArgb(34, 197, 94)))
                    {
                        e.Graphics.FillEllipse(checkBrush, checkRect);
                        using (var checkPen = new Pen(Color.White, 2))
                        {
                            // Draw checkmark
                            e.Graphics.DrawLines(checkPen, new Point[]
                            {
                                new Point(checkRect.X + 5, checkRect.Y + 10),
                                new Point(checkRect.X + 8, checkRect.Y + 13),
                                new Point(checkRect.X + 15, checkRect.Y + 6)
                            });
                        }
                    }
                }
            }

            if (task.IsCompleted)
                font.Dispose();
        }

        private void TaskListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var hasSelection = taskListBox.SelectedIndex >= 0;
            workOnTaskButton.Enabled = hasSelection;
            completeTaskButton.Enabled = hasSelection;
            deleteTaskButton.Enabled = hasSelection;

            if (hasSelection)
            {
                var selectedTask = todoTasks[taskListBox.SelectedIndex];
                completeTaskButton.Text = selectedTask.IsCompleted ? "? Undo" : "? Complete";
                
                // Update button colors based on task state
                if (selectedTask.IsCompleted)
                {
                    completeTaskButton.BackColor = Color.FromArgb(107, 114, 128); // Gray for undo
                    workOnTaskButton.Enabled = false; // Can't work on completed tasks
                }
                else
                {
                    completeTaskButton.BackColor = Color.FromArgb(34, 197, 94); // Green for complete
                    workOnTaskButton.Enabled = true;
                }
            }
        }

        private void WorkOnTaskButton_Click(object sender, EventArgs e)
        {
            if (taskListBox.SelectedIndex < 0) return;

            currentTask = todoTasks[taskListBox.SelectedIndex];
            currentTaskLabel.Text = $"Working on: {currentTask.Name}";
            
            // Switch to timer tab
            mainTabControl.SelectedTab = timerTabPage;
            
            MessageBox.Show($"Timer set up for: {currentTask.Name}\n\nClick Start when you're ready to begin working!", 
                "Task Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CompleteTaskButton_Click(object sender, EventArgs e)
        {
            if (taskListBox.SelectedIndex < 0) return;

            var task = todoTasks[taskListBox.SelectedIndex];
            bool wasCompleted = task.IsCompleted;
            task.IsCompleted = !task.IsCompleted;
            
            RefreshTaskList();
            
            // Play high-pitch sound and show confetti if marking as completed
            if (!wasCompleted && task.IsCompleted)
            {
                // Play a high-pitch beep (frequency: 1500Hz, duration: 200ms)
                try { System.Console.Beep(1500, 200); } catch { System.Media.SystemSounds.Exclamation.Play(); }
                // Show confetti popup
                var confettiForm = new EyeRelaxationReminderForm();
                confettiForm.ShowDialog();
            }

            // Update the current task display if this task is currently being worked on
            if (currentTask?.Id == task.Id)
            {
                var statusText = task.IsCompleted ? " (Completed)" : "";
                currentTaskLabel.Text = $"Working on: {task.Name}{statusText}";
            }
        }

        private void DeleteTaskButton_Click(object sender, EventArgs e)
        {
            if (taskListBox.SelectedIndex < 0) return;

            var task = todoTasks[taskListBox.SelectedIndex];
            var result = MessageBox.Show($"Are you sure you want to delete '{task.Name}'?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // If this task is currently being worked on, clear it
                if (currentTask?.Id == task.Id)
                {
                    currentTask = null;
                    currentTaskLabel.Text = "No task selected";
                    if (isRunning)
                    {
                        StopTimer();
                    }
                }

                todoTasks.RemoveAt(taskListBox.SelectedIndex);
                RefreshTaskList();
            }
        }

        private void RefreshTaskList()
        {
            taskListBox.Items.Clear();
            
            // Sort tasks: incomplete first, then by deadline, then by name
            var sortedTasks = todoTasks
                .OrderBy(t => t.IsCompleted)
                .ThenBy(t => t.Deadline ?? DateTime.MaxValue)
                .ThenBy(t => t.Name)
                .ToList();

            foreach (var task in sortedTasks)
            {
                taskListBox.Items.Add(task.DisplayText);
            }

            // Update the tasks list to match the sorted order for proper indexing
            todoTasks = sortedTasks;
        }

        // Timer functionality
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

                if (currentTask != null)
                {
                    currentTaskLabel.Text = $"Working on: {currentTask.Name} ??";
                }
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
            if (isRunning && currentTask != null)
            {
                // Add worked time to the task
                var workedTime = DateTime.Now - startTime;
                currentTask.WorkedTime = (currentTask.WorkedTime ?? TimeSpan.Zero).Add(workedTime);
            }

            isRunning = false;
            focusTimer.Stop();
            displayTimer.Stop();
            
            startButton.Enabled = true;
            stopButton.Enabled = false;
            focusTimeInput.Enabled = true;

            if (currentTask != null)
            {
                currentTaskLabel.Text = $"Working on: {currentTask.Name}";
            }
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
                    timeDisplayLabel.ForeColor = Color.FromArgb(239, 68, 68); // Modern red
                }
                else
                {
                    timeDisplayLabel.Text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                    // Modern color progression: blue -> amber -> red
                    if (remaining.TotalMinutes <= 2)
                        timeDisplayLabel.ForeColor = Color.FromArgb(239, 68, 68); // Red - urgent
                    else if (remaining.TotalMinutes <= 5)
                        timeDisplayLabel.ForeColor = Color.FromArgb(245, 158, 11); // Amber - warning
                    else
                        timeDisplayLabel.ForeColor = Color.FromArgb(59, 130, 246); // Blue - normal
                }
            }
            else
            {
                timeDisplayLabel.Text = $"{focusTimeMinutes:D2}:00";
                timeDisplayLabel.ForeColor = Color.FromArgb(59, 130, 246); // Modern blue
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