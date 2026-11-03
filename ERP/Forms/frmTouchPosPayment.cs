using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ERP.Forms
{
    /// <summary>
    /// Modern, touch-optimized and keyboard-first POS payment settlement dialog.
    /// Supports direct physical keyboard entry (with auto-focus and auto-selection of the net amount),
    /// on-screen touch numpad, quick cash presets, and one-tap checkout.
    /// </summary>
    public class frmTouchPosPayment : Form
    {
        public decimal NetPayable { get; private set; }
        public decimal CashReceived { get; private set; }
        public decimal CashBack { get; private set; }
        public bool ShouldPrintReceipt { get; private set; } = true;

        // UI Controls
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSub;

        private Panel pnlSummary;
        private Label lblNetPayableTitle;
        private Label lblNetPayableValue;
        private Label lblCashReceivedTitle;
        private Label lblCashReceivedValue;
        private Label lblChangeTitle;
        private Label lblChangeValue;

        private Label lblInputPrompt;
        private TextBox txtCashReceived;

        private FlowLayoutPanel pnlPresets;
        private TableLayoutPanel pnlNumpad;

        private Panel pnlBottomActions;
        private Button btnCancel;
        private Button btnSaveOnly;
        private Button btnSaveAndPrint;

        private bool _isFirstTyping = true;

        public frmTouchPosPayment(decimal netPayable)
        {
            NetPayable = Math.Max(0, netPayable);
            CashReceived = NetPayable;
            CashBack = 0;

            InitializeCodeFirstUI();
        }

        private void InitializeCodeFirstUI()
        {
            this.Text = "POS Payment & Tender";
            this.Size = new Size(580, 720);
            this.MinimumSize = new Size(540, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 250, 252); // Slate-50
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.KeyPreview = true;
            this.KeyDown += FrmTouchPosPayment_KeyDown;

            // 1. Header Banner
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(15, 23, 42), // Slate-900
                Padding = new Padding(20, 10, 20, 10)
            };

            lblHeaderTitle = new Label
            {
                Text = "💳 POS Touch Checkout",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(18, 10)
            };

            lblHeaderSub = new Label
            {
                Text = "Type tendered cash on keyboard or tap presets below (Enter to Save & Print)",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184), // Slate-400
                AutoSize = true,
                Location = new Point(20, 36)
            };

            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSub);

            // 2. Summary Dashboard (Net, Cash, Change)
            pnlSummary = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(525, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Net Payable Column
            lblNetPayableTitle = new Label
            {
                Text = "NET PAYABLE",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 12),
                Size = new Size(160, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblNetPayableValue = new Label
            {
                Text = "Rs. " + NetPayable.ToString("N0", CultureInfo.InvariantCulture),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(10, 34),
                Size = new Size(160, 32),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Cash Received Column
            lblCashReceivedTitle = new Label
            {
                Text = "CASH TENDERED",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(180, 12),
                Size = new Size(160, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblCashReceivedValue = new Label
            {
                Text = "Rs. " + NetPayable.ToString("N0", CultureInfo.InvariantCulture),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 132, 199), // Sky-600
                Location = new Point(180, 34),
                Size = new Size(160, 32),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Change Due Column
            lblChangeTitle = new Label
            {
                Text = "CHANGE DUE",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(350, 12),
                Size = new Size(160, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblChangeValue = new Label
            {
                Text = "Rs. 0",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74), // Emerald-600
                Location = new Point(350, 34),
                Size = new Size(160, 32),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlSummary.Controls.Add(lblNetPayableTitle);
            pnlSummary.Controls.Add(lblNetPayableValue);
            pnlSummary.Controls.Add(lblCashReceivedTitle);
            pnlSummary.Controls.Add(lblCashReceivedValue);
            pnlSummary.Controls.Add(lblChangeTitle);
            pnlSummary.Controls.Add(lblChangeValue);

            // 3. Cash Tender Direct Input Box
            lblInputPrompt = new Label
            {
                Text = "CASH RECEIVED (RS):",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(20, 172),
                Size = new Size(525, 20)
            };

            txtCashReceived = new TextBox
            {
                Location = new Point(20, 196),
                Size = new Size(525, 48),
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                BackColor = Color.FromArgb(240, 249, 255), // Sky-50
                ForeColor = Color.FromArgb(15, 23, 42),
                BorderStyle = BorderStyle.FixedSingle,
                Text = NetPayable.ToString("0.##", CultureInfo.InvariantCulture)
            };
            txtCashReceived.TextChanged += TxtCashReceived_TextChanged;
            txtCashReceived.KeyDown += TxtCashReceived_KeyDown;
            txtCashReceived.Enter += (s, e) => txtCashReceived.SelectAll();

            // 4. Quick Cash Preset Chips
            pnlPresets = new FlowLayoutPanel
            {
                Location = new Point(20, 256),
                Size = new Size(525, 52),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };

            Button btnExact = CreatePresetButton($"Exact (Rs. {NetPayable:N0})", Color.FromArgb(240, 253, 244), Color.FromArgb(22, 163, 74), () =>
            {
                _isFirstTyping = false;
                txtCashReceived.Text = NetPayable.ToString("0.##", CultureInfo.InvariantCulture);
                txtCashReceived.Focus();
                txtCashReceived.SelectAll();
            });
            btnExact.Width = 145;
            pnlPresets.Controls.Add(btnExact);

            int[] quickAmounts = new int[] { 50, 100, 500, 1000 };
            foreach (int addVal in quickAmounts)
            {
                int val = addVal;
                Button btnQuick = CreatePresetButton($"+{val}", Color.FromArgb(241, 245, 249), Color.FromArgb(30, 41, 59), () =>
                {
                    _isFirstTyping = false;
                    decimal current = ParseCash(txtCashReceived.Text);
                    txtCashReceived.Text = (current + val).ToString("0.##", CultureInfo.InvariantCulture);
                    txtCashReceived.Focus();
                    txtCashReceived.SelectAll();
                });
                btnQuick.Width = 68;
                pnlPresets.Controls.Add(btnQuick);
            }

            Button btnClear = CreatePresetButton("Clear", Color.FromArgb(254, 242, 242), Color.FromArgb(220, 38, 38), () =>
            {
                _isFirstTyping = false;
                txtCashReceived.Text = "0";
                txtCashReceived.Focus();
                txtCashReceived.SelectAll();
            });
            btnClear.Width = 72;
            pnlPresets.Controls.Add(btnClear);

            // 5. On-Screen Touch Numpad (4x4)
            pnlNumpad = new TableLayoutPanel
            {
                Location = new Point(20, 318),
                Size = new Size(525, 260),
                ColumnCount = 4,
                RowCount = 4,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(6)
            };
            for (int c = 0; c < 4; c++) pnlNumpad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            for (int r = 0; r < 4; r++) pnlNumpad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            // Row 1
            AddKeypadButton("1", () => HandleDigitPress("1"), 0, 0);
            AddKeypadButton("2", () => HandleDigitPress("2"), 1, 0);
            AddKeypadButton("3", () => HandleDigitPress("3"), 2, 0);
            AddSpecialKeypadButton("+500", () => AddPresetDelta(500), 3, 0, Color.FromArgb(239, 246, 255), Color.FromArgb(29, 78, 216));

            // Row 2
            AddKeypadButton("4", () => HandleDigitPress("4"), 0, 1);
            AddKeypadButton("5", () => HandleDigitPress("5"), 1, 1);
            AddKeypadButton("6", () => HandleDigitPress("6"), 2, 1);
            AddSpecialKeypadButton("+1000", () => AddPresetDelta(1000), 3, 1, Color.FromArgb(239, 246, 255), Color.FromArgb(29, 78, 216));

            // Row 3
            AddKeypadButton("7", () => HandleDigitPress("7"), 0, 2);
            AddKeypadButton("8", () => HandleDigitPress("8"), 1, 2);
            AddKeypadButton("9", () => HandleDigitPress("9"), 2, 2);
            AddSpecialKeypadButton("+5000", () => AddPresetDelta(5000), 3, 2, Color.FromArgb(240, 253, 244), Color.FromArgb(21, 128, 61));

            // Row 4
            AddKeypadButton("00", () => HandleDoubleZeroPress(), 0, 3);
            AddKeypadButton("0", () => HandleDigitPress("0"), 1, 3);
            AddSpecialKeypadButton("⌫", () => HandleBackspacePress(), 2, 3, Color.FromArgb(254, 242, 242), Color.FromArgb(220, 38, 38));
            AddSpecialKeypadButton("CLR", () => { _isFirstTyping = false; txtCashReceived.Text = "0"; txtCashReceived.Focus(); }, 3, 3, Color.FromArgb(248, 250, 252), Color.FromArgb(100, 116, 139));

            // 6. Bottom Action Bar
            pnlBottomActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 12)
            };

            btnCancel = new Button
            {
                Text = "Cancel (Esc)",
                Size = new Size(110, 50),
                Location = new Point(20, 11),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            btnSaveOnly = new Button
            {
                Text = "Save Only (F5)",
                Size = new Size(150, 50),
                Location = new Point(140, 11),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(239, 246, 255),
                ForeColor = Color.FromArgb(2, 132, 199),
                FlatStyle = FlatStyle.Flat
            };
            btnSaveOnly.FlatAppearance.BorderColor = Color.FromArgb(186, 230, 253);
            btnSaveOnly.Click += (s, e) => SubmitSale(false);

            btnSaveAndPrint = new Button
            {
                Text = "Save & Print Receipt (F6 / Enter)",
                Size = new Size(245, 50),
                Location = new Point(300, 11),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(22, 163, 74), // Emerald-600
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveAndPrint.FlatAppearance.BorderSize = 0;
            btnSaveAndPrint.Click += (s, e) => SubmitSale(true);

            pnlBottomActions.Controls.Add(btnCancel);
            pnlBottomActions.Controls.Add(btnSaveOnly);
            pnlBottomActions.Controls.Add(btnSaveAndPrint);

            // Add all controls to Form
            this.Controls.Add(pnlNumpad);
            this.Controls.Add(pnlPresets);
            this.Controls.Add(txtCashReceived);
            this.Controls.Add(lblInputPrompt);
            this.Controls.Add(pnlSummary);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBottomActions);

            // Setup Auto-Focus and Auto-Select on form show
            this.Shown += (s, e) =>
            {
                txtCashReceived.Focus();
                txtCashReceived.SelectAll();
                _isFirstTyping = true;
            };
        }

        private Button CreatePresetButton(string text, Color bg, Color textCol, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Height = 44,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = bg,
                ForeColor = textCol,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 6, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void AddKeypadButton(string text, Action onClick, int col, int row)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.Click += (s, e) => onClick();
            pnlNumpad.Controls.Add(btn, col, row);
        }

        private void AddSpecialKeypadButton(string text, Action onClick, int col, int row, Color bg, Color fg)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = bg,
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.Click += (s, e) => onClick();
            pnlNumpad.Controls.Add(btn, col, row);
        }

        private void HandleDigitPress(string digit)
        {
            if (_isFirstTyping)
            {
                _isFirstTyping = false;
                txtCashReceived.Text = digit;
            }
            else
            {
                string curr = txtCashReceived.Text;
                txtCashReceived.Text = (curr == "0" || string.IsNullOrWhiteSpace(curr)) ? digit : curr + digit;
            }
            txtCashReceived.Focus();
            txtCashReceived.SelectionStart = txtCashReceived.Text.Length;
        }

        private void HandleDoubleZeroPress()
        {
            if (_isFirstTyping)
            {
                _isFirstTyping = false;
                txtCashReceived.Text = "0";
            }
            else
            {
                string curr = txtCashReceived.Text;
                if (!string.IsNullOrWhiteSpace(curr) && curr != "0")
                {
                    txtCashReceived.Text = curr + "00";
                }
            }
            txtCashReceived.Focus();
            txtCashReceived.SelectionStart = txtCashReceived.Text.Length;
        }

        private void HandleBackspacePress()
        {
            _isFirstTyping = false;
            string curr = txtCashReceived.Text;
            if (curr.Length <= 1)
            {
                txtCashReceived.Text = "0";
            }
            else
            {
                txtCashReceived.Text = curr.Substring(0, curr.Length - 1);
            }
            txtCashReceived.Focus();
            txtCashReceived.SelectionStart = txtCashReceived.Text.Length;
        }

        private void AddPresetDelta(int delta)
        {
            _isFirstTyping = false;
            decimal curr = ParseCash(txtCashReceived.Text);
            txtCashReceived.Text = (curr + delta).ToString("0.##", CultureInfo.InvariantCulture);
            txtCashReceived.Focus();
            txtCashReceived.SelectAll();
        }

        private void TxtCashReceived_TextChanged(object sender, EventArgs e)
        {
            decimal cash = ParseCash(txtCashReceived.Text);
            CashReceived = cash;
            lblCashReceivedValue.Text = "Rs. " + cash.ToString("N0", CultureInfo.InvariantCulture);

            if (cash >= NetPayable)
            {
                CashBack = cash - NetPayable;
                lblChangeTitle.Text = "CHANGE DUE";
                lblChangeValue.ForeColor = Color.FromArgb(22, 163, 74); // Emerald
                lblChangeValue.Text = "Rs. " + CashBack.ToString("N0", CultureInfo.InvariantCulture);
            }
            else
            {
                decimal balanceDue = NetPayable - cash;
                lblChangeTitle.Text = "BALANCE DUE";
                lblChangeValue.ForeColor = Color.FromArgb(220, 38, 38); // Red
                lblChangeValue.Text = "Rs. " + balanceDue.ToString("N0", CultureInfo.InvariantCulture);
            }
        }

        private void TxtCashReceived_KeyDown(object sender, KeyEventArgs e)
        {
            _isFirstTyping = false;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SubmitSale(true);
            }
        }

        private void FrmTouchPosPayment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                e.Handled = true;
                SubmitSale(true);
            }
            else if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                SubmitSale(false);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void SubmitSale(bool printReceipt)
        {
            ShouldPrintReceipt = printReceipt;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private decimal ParseCash(string text)
        {
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                return val;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out val))
                return val;
            return 0;
        }
    }
}
