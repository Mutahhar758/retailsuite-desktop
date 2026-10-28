using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERP.Controls
{
    /// <summary>
    /// Compact server-side pagination bar (first / previous / next / last, page-size selector).
    /// Code-only control (no designer) so it can be dropped under any DataGridView.
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
            Height = 32;
            Dock = DockStyle.Bottom;
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(6, 3, 6, 3)
            };

            ConfigureButton(_btnFirst, "|<", "First page");
            ConfigureButton(_btnPrev, "<", "Previous page");
            ConfigureButton(_btnNext, ">", "Next page");
            ConfigureButton(_btnLast, ">|", "Last page");

            _lblInfo.AutoSize = false;
            _lblInfo.Width = 260;
            _lblInfo.Height = 24;
            _lblInfo.Margin = new Padding(4, 3, 4, 2);
            _lblInfo.TextAlign = ContentAlignment.MiddleCenter;

            _lblSize.Text = "Rows:";
            _lblSize.AutoSize = false;
            _lblSize.Width = 42;
            _lblSize.Height = 24;
            _lblSize.Margin = new Padding(8, 3, 2, 2);
            _lblSize.TextAlign = ContentAlignment.MiddleRight;

            _cmbPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPageSize.Width = 60;
            _cmbPageSize.Height = 24;
            _cmbPageSize.Margin = new Padding(2, 2, 2, 2);
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

            panel.Controls.AddRange(new Control[] { _btnFirst, _btnPrev, _lblInfo, _btnNext, _btnLast, _lblSize, _cmbPageSize });
            Controls.Add(panel);
            UpdateUi();
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

        private void UpdateUi()
        {
            _lblInfo.Text = TotalCount == 0
                ? "No records"
                : $"Page {CurrentPage} of {TotalPages}  ({TotalCount} records)";
            _btnFirst.Enabled = _btnPrev.Enabled = CurrentPage > 1;
            _btnNext.Enabled = _btnLast.Enabled = CurrentPage < TotalPages;
        }

        private static void ConfigureButton(Button b, string text, string tip)
        {
            b.Text = text;
            b.Width = 34;
            b.Height = 24;
            b.Margin = new Padding(2, 1, 2, 1);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            b.BackColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.TabStop = false;
            new ToolTip().SetToolTip(b, tip);
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