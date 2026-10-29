using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERP.Controls
{
    /// <summary>
    /// Compact, responsive server-side pagination bar.
    /// Code-only control (no designer) that auto-adapts cleanly to wide tabs and narrow split panels.
    /// </summary>
    public class PaginationControl : UserControl
    {
        private readonly Button _btnFirst = new Button();
        private readonly Button _btnPrev = new Button();
        private readonly Button _btnNext = new Button();
        private readonly Button _btnLast = new Button();
        private readonly Label _lblInfo = new Label();
        private readonly Label _lblSize = new Label();
        private readonly ComboBox _cmbPageSize = new ComboBox();
        private readonly ToolTip _toolTip = new ToolTip();

        private Control _attachedParent;
        private DataGridView _attachedGrid;
        private bool _suppressEvents;

        public event EventHandler<PageChangedEventArgs> PageChanged;

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; } = 1;
        public int TotalCount { get; private set; }
        public int PageSize { get; private set; } = 20;

        public PaginationControl()
        {
            Height = 30;
            Dock = DockStyle.Bottom;
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            ConfigureButton(_btnFirst, "«", "First page");
            ConfigureButton(_btnPrev, "<", "Previous page");
            ConfigureButton(_btnNext, ">", "Next page");
            ConfigureButton(_btnLast, "»", "Last page");

            _lblInfo.AutoSize = false;
            _lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            _lblInfo.ForeColor = Color.FromArgb(51, 65, 85);
            _lblInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);

            _lblSize.Text = "Rows:";
            _lblSize.AutoSize = false;
            _lblSize.TextAlign = ContentAlignment.MiddleRight;
            _lblSize.ForeColor = Color.FromArgb(100, 116, 139);
            _lblSize.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);

            _cmbPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPageSize.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            _cmbPageSize.Items.AddRange(new object[] { 10, 20, 50, 100, 200 });
            _cmbPageSize.SelectedItem = PageSize;
            _cmbPageSize.SelectedIndexChanged += (s, e) =>
            {
                if (_suppressEvents || !(_cmbPageSize.SelectedItem is int)) return;
                PageSize = (int)_cmbPageSize.SelectedItem;
                Raise(1);
            };

            _btnFirst.Click += (s, e) => Raise(1);
            _btnPrev.Click += (s, e) => Raise(CurrentPage - 1);
            _btnNext.Click += (s, e) => Raise(CurrentPage + 1);
            _btnLast.Click += (s, e) => Raise(TotalPages);

            Controls.AddRange(new Control[] { _btnFirst, _btnPrev, _btnNext, _btnLast, _lblInfo, _lblSize, _cmbPageSize });
            UpdateUi();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(Color.FromArgb(226, 232, 240)))
            {
                e.Graphics.DrawLine(pen, 0, 0, Width, 0);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            LayoutControls();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutControls();
        }

        private void LayoutControls()
        {
            if (_btnFirst == null || _lblInfo == null || _cmbPageSize == null) return;

            int totalWidth = ClientSize.Width;
            int totalHeight = ClientSize.Height;
            if (totalWidth <= 0 || totalHeight <= 0) return;

            int btnHeight = 22;
            int btnWidth = 32;
            int y = Math.Max(0, (totalHeight - btnHeight) / 2);

            // Left cluster: [|<] [<] [>] [>|]
            int x = 4;
            _btnFirst.SetBounds(x, y, btnWidth, btnHeight);
            x += btnWidth + 2;

            _btnPrev.SetBounds(x, y, btnWidth, btnHeight);
            x += btnWidth + 2;

            _btnNext.SetBounds(x, y, btnWidth, btnHeight);
            x += btnWidth + 2;

            _btnLast.SetBounds(x, y, btnWidth, btnHeight);
            x += btnWidth + 4;

            // Right cluster: [Rows:] [cmbPageSize]
            int rightX = totalWidth - 4;
            int comboWidth = 52;
            int lblSizeWidth = 38;

            if (rightX - comboWidth > x + 30)
            {
                _cmbPageSize.Visible = true;
                rightX -= comboWidth;
                _cmbPageSize.SetBounds(rightX, y, comboWidth, btnHeight);
                rightX -= 3;

                if (rightX - lblSizeWidth > x + 40)
                {
                    _lblSize.Visible = true;
                    rightX -= lblSizeWidth;
                    _lblSize.SetBounds(rightX, y, lblSizeWidth, btnHeight);
                    rightX -= 3;
                }
                else
                {
                    _lblSize.Visible = false;
                }
            }
            else
            {
                _cmbPageSize.Visible = false;
                _lblSize.Visible = false;
            }

            // Middle: info label
            int infoWidth = Math.Max(0, rightX - x);
            _lblInfo.SetBounds(x, y, infoWidth, btnHeight);

            UpdateUiText();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            BringToFront();
        }

        /// <summary>
        /// Attaches this pagination control cleanly to the parent container and
        /// adjusts the grid bottom anchor/size so the grid never overlaps this bar.
        /// </summary>
        public void Attach(Control parent, DataGridView grid = null)
        {
            if (parent == null) return;
            _attachedParent = parent;
            _attachedGrid = grid;

            Dock = DockStyle.Bottom;
            if (!parent.Controls.Contains(this))
            {
                parent.Controls.Add(this);
            }
            BringToFront();

            if (grid != null)
            {
                AdjustGrid(parent, grid);
                parent.SizeChanged -= Parent_SizeChanged;
                parent.SizeChanged += Parent_SizeChanged;
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            if (_attachedParent != null && _attachedGrid != null)
            {
                AdjustGrid(_attachedParent, _attachedGrid);
            }
        }

        private void AdjustGrid(Control parent, DataGridView grid)
        {
            if (parent == null || grid == null || grid.IsDisposed) return;

            BringToFront();

            // If grid is anchored to bottom (e.g., frmNormalSale), adjust height so it leaves space for the pager
            if ((grid.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
            {
                int availableBottom = parent.ClientSize.Height - Height - 6;
                int targetHeight = availableBottom - grid.Top;
                if (targetHeight > 60 && grid.Height != targetHeight)
                {
                    var origAnchor = grid.Anchor;
                    grid.Anchor = origAnchor & ~AnchorStyles.Bottom;
                    grid.Height = targetHeight;
                    grid.Anchor = origAnchor;
                }
            }
            else if (grid.Dock == DockStyle.None || grid.Dock == DockStyle.Top)
            {
                if (grid.Bottom > this.Top && this.Top > grid.Top)
                {
                    grid.Height = Math.Max(50, this.Top - grid.Top - 6);
                }
            }
        }

        /// <summary>Updates the bar from a server response without raising PageChanged.</summary>
        public void SetPagination(int currentPage, int totalPages, int totalCount, int pageSize)
        {
            _suppressEvents = true;
            try
            {
                CurrentPage = Math.Max(1, currentPage);
                TotalPages = Math.Max(1, totalPages);
                TotalCount = totalCount;
                PageSize = pageSize;
                if (!_cmbPageSize.Items.Contains(pageSize))
                    _cmbPageSize.Items.Add(pageSize);
                _cmbPageSize.SelectedItem = pageSize;
            }
            finally
            {
                _suppressEvents = false;
            }
            UpdateUi();
        }

        private void Raise(int page)
        {
            if (page < 1) page = 1;
            if (page > TotalPages) page = TotalPages;
            PageChanged?.Invoke(this, new PageChangedEventArgs(page, PageSize));
        }

        private void UpdateUiText()
        {
            bool compact = ClientSize.Width < 460;
            if (TotalCount == 0)
            {
                _lblInfo.Text = compact ? "No records" : "No records found";
            }
            else
            {
                _lblInfo.Text = compact
                    ? $"{CurrentPage}/{TotalPages} ({TotalCount})"
                    : $"Page {CurrentPage} of {TotalPages}  ({TotalCount} records)";
            }

            string fullTip = TotalCount == 0
                ? "No records"
                : $"Page {CurrentPage} of {TotalPages} ({TotalCount} records)";
            _toolTip.SetToolTip(_lblInfo, fullTip);
        }

        private void UpdateUi()
        {
            UpdateUiText();
            _btnFirst.Enabled = _btnPrev.Enabled = CurrentPage > 1;
            _btnNext.Enabled = _btnLast.Enabled = CurrentPage < TotalPages;
        }

        private void ConfigureButton(Button b, string text, string tip)
        {
            b.Text = text;
            b.Width = 32;
            b.Height = 22;
            b.Margin = new Padding(0);
            b.Padding = Padding.Empty;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            b.BackColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.TabStop = false;
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _toolTip.SetToolTip(b, tip);
        }
    }

    public class PageChangedEventArgs : EventArgs
    {
        public PageChangedEventArgs(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; }
        public int PageSize { get; }
    }
}