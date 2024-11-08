using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly string imageBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "img");
        private string maPhongHienTai;

        public Form1()
        {
            InitializeComponent();
        }

        private void labelItem1_Click(object sender, EventArgs e)
        {
        }

        private void LoadDanhSachPhong()
        {
            using (var context = new QL_KaraokeEntities1())
            {
                var danhSachPhong = context.Phongs.ToList();

                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
                flowLayoutPanel1.WrapContents = true;
                flowLayoutPanel1.AutoScroll = true;

                foreach (var phong in danhSachPhong)
                {
                    if ((checkBox1.Checked && phong.LoaiPhong == "Thường") ||
                        (checkBox2.Checked && phong.LoaiPhong == "VIP") ||
                        (!checkBox1.Checked && !checkBox2.Checked))
                    {
                        Panel panelPhong = TaoPanelPhong(phong);
                        flowLayoutPanel1.Controls.Add(panelPhong);
                    }
                }
            }
        }

        private Panel TaoPanelPhong(Phong phong)
        {
            Panel panelPhong = new Panel();
            panelPhong.Size = new Size(flowLayoutPanel1.ClientSize.Width / 2 - 10, 100);
            panelPhong.BorderStyle = BorderStyle.FixedSingle;

            Label lblTenPhong = new Label
            {
                Text = "Tên phòng: " + phong.TenPhong,
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label lblLoaiPhong = new Label
            {
                Text = "Loại phòng: " + phong.LoaiPhong,
                Location = new Point(10, 30),
                AutoSize = true
            };

            Label lblGiaPhong = new Label
            {
                Text = "Giá phòng: " + phong.GiaPhong.ToString("C0"),
                Location = new Point(10, 50),
                AutoSize = true
            };

            Label lblTrangThai = new Label
            {
                Text = "Trạng thái: " + phong.TrangThai,
                Location = new Point(10, 70),
                AutoSize = true
            };

            panelPhong.Controls.Add(lblTenPhong);
            panelPhong.Controls.Add(lblLoaiPhong);
            panelPhong.Controls.Add(lblGiaPhong);
            panelPhong.Controls.Add(lblTrangThai);

            panelPhong.MouseClick += (sender, e) => HienThiThongTinPhong(phong);
            lblTenPhong.MouseClick += (sender, e) => HienThiThongTinPhong(phong);
            lblLoaiPhong.MouseClick += (sender, e) => HienThiThongTinPhong(phong);
            lblGiaPhong.MouseClick += (sender, e) => HienThiThongTinPhong(phong);
            lblTrangThai.MouseClick += (sender, e) => HienThiThongTinPhong(phong);

            return panelPhong;
        }

        private void HienThiThongTinPhong(Phong phong)
        {
            // Reset thông tin cũ
            listViewEx1.Items.Clear();
            textBoxX1.Clear();
            textBoxX2.Clear();
            textBoxX3.Clear();
            textBoxX4.Clear();

            // Cập nhật mã phòng hiện tại
            maPhongHienTai = phong.MaPhong;

            // Hiển thị thông tin phòng mới
            textBoxX1.Text = phong.TenPhong;
            textBoxX2.Text = phong.LoaiPhong;
            textBoxX3.Text = phong.TrangThai;
            textBoxX4.Text = phong.GiaPhong.ToString("C0");

            // Chỉ load danh sách món đã gọi nếu phòng đang sử dụng
            if (phong.TrangThai == "Đang sử dụng")
            {
                LoadDanhSachMonDaGoi(phong.MaPhong);
            }
        }

        private void LoadDanhSachMonDaGoi(string maPhong)
        {
            using (var context = new QL_KaraokeEntities1())
            {
                // Tìm phiếu đặt phòng đang active
                var phieuDatPhong = context.PhieuDatPhongs
                    .FirstOrDefault(p => p.MaPhong == maPhong && p.TinhTrang == "Đã xác nhận");

                if (phieuDatPhong == null) return;

                var hoaDon = context.HoaDons
                    .Include(h => h.ChiTietHoaDons)
                    .Include(h => h.ChiTietHoaDons.Select(c => c.DichVu))
                    .FirstOrDefault(h => h.MaPhieuDatPhong == phieuDatPhong.MaPhieuDatPhong);

                if (hoaDon == null) return;

                listViewEx1.Items.Clear();
                foreach (var chiTiet in hoaDon.ChiTietHoaDons)
                {
                    var item = new ListViewItem(chiTiet.DichVu.TenDichVu);
                    item.SubItems.Add(chiTiet.SoLuong.ToString());
                    item.SubItems.Add(chiTiet.ThanhTien.ToString());
                    listViewEx1.Items.Add(item);
                }
            }
        }

        private void LoadDanhSachDichVu()
        {
            using (var context = new QL_KaraokeEntities1())
            {
                var danhSachDichVu = context.DichVus.ToList();
                tableLayoutPanel2.SuspendLayout();
                tableLayoutPanel2.Controls.Clear();
                tableLayoutPanel2.RowCount = (int)Math.Ceiling(danhSachDichVu.Count / 3.0);
                tableLayoutPanel2.ColumnCount = 3;

                int rowIndex = 0;
                int columnIndex = 0;

                foreach (var dichVu in danhSachDichVu)
                {
                    Panel panelDichVu = TaoPanelDichVu(dichVu);
                    tableLayoutPanel2.Controls.Add(panelDichVu, columnIndex, rowIndex);

                    columnIndex++;
                    if (columnIndex >= 3)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                tableLayoutPanel2.ResumeLayout();
            }
        }

        private Panel TaoPanelDichVu(DichVu dichVu)
        {
            // Tạo panel chính
            Panel panelDichVu = new Panel
            {
                Size = new Size(180, 220),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            // Tạo panel con để chứa nội dung
            Panel contentPanel = new Panel
            {
                Size = new Size(170, 210),
                Location = new Point(5, 5),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.None
            };

            // PictureBox cho hình ảnh
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point((contentPanel.Width - 80) / 2, 05),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = GetServiceImage(dichVu.TenDichVu)
            };

            // Label cho tên dịch vụ
            Label lblTenDichVu = new Label
            {
                Text = dichVu.TenDichVu,
                Location = new Point(10, pictureBox.Bottom + 10),
                Size = new Size(contentPanel.Width - 40, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            // Label cho giá
            Label lblDonGia = new Label
            {
                Text = $"{dichVu.DonGia:N0} VNĐ",
                Location = new Point(10, lblTenDichVu.Bottom + 5),
                Size = new Size(contentPanel.Width - 30, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(192, 64, 0)
            };

            // Button "Thêm"
            Button btnThem = new Button
            {
                Text = "Thêm",
                Size = new Size(100, 30),
                Location = new Point((contentPanel.Width - 100) / 2, lblDonGia.Bottom + 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnThem.FlatAppearance.BorderSize = 0;

            // Thêm các controls vào panel
            contentPanel.Controls.AddRange(new Control[] { pictureBox, lblTenDichVu, lblDonGia, btnThem });
            panelDichVu.Controls.Add(contentPanel);

            // Xử lý sự kiện click
            void HandleClick(object sender, EventArgs e) => XacNhanThemDichVu(dichVu);

            btnThem.Click += HandleClick;
            contentPanel.Click += HandleClick;
            pictureBox.Click += HandleClick;
            lblTenDichVu.Click += HandleClick;
            lblDonGia.Click += HandleClick;

            // Hiệu ứng hover
            void HandleMouseEnter(object sender, EventArgs e)
            {
                contentPanel.BackColor = Color.FromArgb(230, 230, 230);
            }

            void HandleMouseLeave(object sender, EventArgs e)
            {
                contentPanel.BackColor = Color.FromArgb(245, 245, 245);
            }

            contentPanel.MouseEnter += HandleMouseEnter;
            contentPanel.MouseLeave += HandleMouseLeave;
            foreach (Control control in contentPanel.Controls)
            {
                control.MouseEnter += HandleMouseEnter;
                control.MouseLeave += HandleMouseLeave;
            }

            return panelDichVu;
        }

        private void XacNhanThemDichVu(DichVu dichVu)
        {
            if (string.IsNullOrEmpty(maPhongHienTai))
            {
                MessageBox.Show("Vui lòng chọn một phòng trước khi thêm dịch vụ.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo và hiển thị form nhập số lượng ngay lập tức
            using (var inputForm = new Form())
            {
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.Size = new Size(250, 150);
                inputForm.Text = $"Thêm {dichVu.TenDichVu}";

                var numericUpDown = new NumericUpDown
                {
                    Location = new Point(20, 20),
                    Size = new Size(100, 25),
                    Minimum = 1,
                    Value = 1
                };

                var btnOK = new Button
                {
                    Text = "Thêm",
                    DialogResult = DialogResult.OK,
                    Location = new Point(20, 60)
                };

                var btnCancel = new Button
                {
                    Text = "Hủy",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(120, 60)
                };

                inputForm.Controls.AddRange(new Control[] { numericUpDown, btnOK, btnCancel });
                inputForm.AcceptButton = btnOK;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    int soLuong = (int)numericUpDown.Value;
                    ThemDichVuVaoPhong(maPhongHienTai, dichVu, soLuong);
                }
            }
        }

        private void ThemDichVuVaoPhong(string maPhong, DichVu dichVu, int soLuong)
        {
            using (var context = new QL_KaraokeEntities1())
            {
                // Gọi phương thức để lấy mã hóa đơn
                string maHoaDon = LayMaHoaDonPhong(maPhong);

                if (string.IsNullOrEmpty(maHoaDon))
                {
                    MessageBox.Show("Không tìm thấy hóa đơn hiện tại của phòng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Sử dụng mã hóa đơn để truy vấn
                var hoaDon = context.HoaDons
                    .Include(h => h.ChiTietHoaDons)
                    .FirstOrDefault(hd => hd.MaHoaDon == maHoaDon);

                if (hoaDon == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo mới một ChiTietHoaDon cho dịch vụ
                var chiTietHoaDon = new ChiTietHoaDon
                {
                    MaCTHD = Guid.NewGuid().ToString("N").Substring(0, 10), // Tạo mã duy nhất
                    SoLuong = soLuong, // Số lượng dịch vụ
                    ThanhTien = dichVu.DonGia * soLuong, // Tính thành tiền
                    MaHoaDon = hoaDon.MaHoaDon, // Liên kết với hóa đơn hiện tại
                    MaDichVu = dichVu.MaDichVu
                };

                hoaDon.ChiTietHoaDons.Add(chiTietHoaDon);
                hoaDon.TongTien += (float)chiTietHoaDon.ThanhTien;

                try
                {
                    context.SaveChanges();
                    LoadDanhSachMonDaGoi(maPhong);
                    MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Xử lý lỗi xung đột dữ liệu
                    var entry = ex.Entries.Single();
                    var clientValues = (ChiTietHoaDon)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        MessageBox.Show("Hóa đơn đã bị xóa bởi người dùng khác.", "Xung đột",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        var databaseValues = (ChiTietHoaDon)databaseEntry.ToObject();
                        MessageBox.Show("Hóa đơn đã bị thay đổi bởi người dùng khác. Vui lòng tải lại dữ liệu.", "Xung đột",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi thêm dịch vụ: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string LayMaHoaDonPhong(string maPhong)
        {
            using (var context = new QL_KaraokeEntities1())
            {
                // First, find the active PhieuDatPhong for the given MaPhong
                var phieuDatPhong = context.PhieuDatPhongs
                    .FirstOrDefault(p => p.MaPhong == maPhong && p.TinhTrang == "Đã xác nhận");

                if (phieuDatPhong == null)
                {
                    return null;
                }

                // Now, find HoaDon associated with this PhieuDatPhong
                var hoaDon = context.HoaDons
                    .FirstOrDefault(hd => hd.MaPhieuDatPhong == phieuDatPhong.MaPhieuDatPhong);

                // If HoaDon not found, create a new one
                if (hoaDon == null)
                {
                    var newHoaDon = new HoaDon
                    {
                        MaHoaDon = "HD" + Guid.NewGuid().ToString("N").Substring(0, 10), // 2 + 13 = 15
                        NgayLap = DateTime.Now,
                        TongTien = 0,
                        MaPhieuDatPhong = phieuDatPhong.MaPhieuDatPhong,
                        MaNhanVien = "NV001" // Mặc định hoặc lấy từ người dùng đăng nhập
                    };
                    context.HoaDons.Add(newHoaDon);
                    context.SaveChanges(); // Lưu trước để có MaHoaDon
                    return newHoaDon.MaHoaDon;
                }

                return hoaDon.MaHoaDon;
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDanhSachPhong();
            LoadDanhSachDichVu();
            EnableDoubleBuffer(listViewEx1);
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LoadDanhSachPhong();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            LoadDanhSachPhong();
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
        }

        private void buttonItem5_Click(object sender, EventArgs e)
        {
            Form_Phong form2 = new Form_Phong();
            ShowFormInPanel(form2);
        }

        private void ShowFormInPanel(Form form)
        {
            panelEx1.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelEx1.Controls.Add(form);
            form.Show();
        }

        private void labelX7_Click(object sender, EventArgs e)
        {
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
        }

        private void labelX5_Click(object sender, EventArgs e)
        {
        }

        private void EnableDoubleBuffer(Control control)
        {
            System.Reflection.PropertyInfo doubleBufferPropertyInfo =
                control.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, true, null);
        }

        private Image GetServiceImage(string tenDichVu)
        {
            // Chuẩn hóa tên file
            string sanitizedFileName = string.Join("", tenDichVu.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine(imageBasePath, sanitizedFileName + ".png");

            try
            {
                if (File.Exists(filePath))
                {
                    using (var image = Image.FromFile(filePath))
                    {
                        // Tạo bản sao của ảnh để tránh file lock
                        return new Bitmap(image);
                    }
                }
                return CreatePlaceholderImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh '{tenDichVu}': {ex.Message}");
                return CreatePlaceholderImage();
            }
        }

        private Image CreatePlaceholderImage()
        {
            Bitmap placeholderImage = new Bitmap(60, 60);
            using (Graphics g = Graphics.FromImage(placeholderImage))
            {
                g.Clear(Color.LightGray);
                g.DrawString("No Image", new Font("Arial", 8), Brushes.Black, new PointF(5, 20));
            }
            return placeholderImage;
        }

        private void labelX4_Click(object sender, EventArgs e)
        {
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maPhongHienTai))
            {
                MessageBox.Show("Vui lòng chọn một phòng trước khi mở phòng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new QL_KaraokeEntities1())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra phòng
                        var phong = context.Phongs.FirstOrDefault(p => p.MaPhong == maPhongHienTai);
                        if (phong == null || phong.TrangThai != "Trống")
                        {
                            MessageBox.Show("Phòng đang được sử dụng hoặc không tồn tại.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Tạo mã cho phiếu đặt phòng và hóa đơn với định dạng đúng độ dài
                        string maPhieuDat = "PDP" + Guid.NewGuid().ToString("N").Substring(0, 10); // 3 + 12 = 15
                        string maHoaDon = "HD" + Guid.NewGuid().ToString("N").Substring(0, 10); // 2 + 13 = 15

                        // Sử dụng khách hàng mặc định KH001 đã có sẵn trong CSDL
                        var khach = context.Khaches.FirstOrDefault(k => k.MaKhach == "KH001");
                        if (khach == null)
                        {
                            MessageBox.Show("Không tìm thấy khách hàng mặc định (KH001) trong hệ thống.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Sử dụng nhân viên NV001 đã có sẵn trong CSDL
                        var nhanVien = context.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == "NV001");
                        if (nhanVien == null)
                        {
                            MessageBox.Show("Không tìm thấy nhân viên mặc định (NV001) trong hệ thống.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Tìm khuyến mãi hiện hành cho khách VIP (nếu có)
                        var khuyenMai = context.KhuyenMais
                            .FirstOrDefault(km =>
                                km.MaLoaiKhach == khach.MaLoaiKhach &&
                                DbFunctions.TruncateTime(km.NgayBatDau) <= DbFunctions.TruncateTime(DateTime.Now) &&
                                DbFunctions.TruncateTime(km.NgayKetThuc) >= DbFunctions.TruncateTime(DateTime.Now));

                        // Tạo phiếu đặt phòng mới
                        var phieuDatPhong = new PhieuDatPhong
                        {
                            MaPhieuDatPhong = maPhieuDat,
                            NgayDat = DateTime.Now.Date,
                            GioDat = DateTime.Now.TimeOfDay,
                            MaKhuyenMai = khuyenMai?.MaKhuyenMai, // Null nếu không có khuyến mãi
                            TinhTrang = "Đã xác nhận",
                            MaKhach = khach.MaKhach,
                            MaPhong = maPhongHienTai
                        };

                        context.PhieuDatPhongs.Add(phieuDatPhong);

                        // Tạo hóa đơn mới
                        var hoaDon = new HoaDon
                        {
                            MaHoaDon = maHoaDon,
                            NgayLap = DateTime.Now.Date,
                            TongTien = 0, // Sẽ được cập nhật khi thêm dịch vụ
                            MaPhieuDatPhong = maPhieuDat,
                            MaNhanVien = nhanVien.MaNhanVien
                        };

                        context.HoaDons.Add(hoaDon);

                        // Cập nhật trạng thái phòng
                        phong.TrangThai = "Đang sử dụng";
                        context.Entry(phong).State = EntityState.Modified;

                        // Lưu tất cả thay đổi
                        context.SaveChanges();
                        transaction.Commit();

                        // Refresh UI
                        LoadDanhSachPhong();
                        MessageBox.Show("Mở phòng thành công.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        transaction.Rollback();

                        var entry = ex.Entries.Single();
                        var clientValues = entry.CurrentValues;
                        var databaseEntry = entry.GetDatabaseValues();

                        if (databaseEntry == null)
                        {
                            MessageBox.Show("Dữ liệu đã bị xóa bởi người dùng khác.", "Xung đột",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            var databaseValues = databaseEntry.ToObject();
                            MessageBox.Show("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại dữ liệu.", "Xung đột",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Lỗi khi mở phòng: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void buttonX2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maPhongHienTai))
            {
                MessageBox.Show("Vui lòng chọn một phòng trước khi đóng phòng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new QL_KaraokeEntities1())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var phong = context.Phongs.FirstOrDefault(p => p.MaPhong == maPhongHienTai);
                        if (phong == null || phong.TrangThai != "Đang sử dụng")
                        {
                            MessageBox.Show("Phòng không tồn tại hoặc đang trống.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var phieuDatPhong = context.PhieuDatPhongs
                            .FirstOrDefault(p => p.MaPhong == maPhongHienTai && p.TinhTrang == "Đã xác nhận");

                        if (phieuDatPhong == null)
                        {
                            MessageBox.Show("Không tìm thấy phiếu đặt phòng cho phòng này.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var hoaDon = context.HoaDons
                     .Include(hd => hd.ChiTietHoaDons.Select(cthd => cthd.DichVu))
                     .FirstOrDefault(hd => hd.MaPhieuDatPhong == phieuDatPhong.MaPhieuDatPhong);



                        if (hoaDon == null)
                        {
                            MessageBox.Show("Không tìm thấy hóa đơn cho phòng này.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Tính tổng tiền dịch vụ
                        double tongTienDichVu = (double)hoaDon.ChiTietHoaDons.Sum(cthd => cthd.ThanhTien);

                        // Tính thời gian sử dụng và tiền phòng
                        double soGioSuDung = CalculateHours(phieuDatPhong.GioDat.Value, DateTime.Now.TimeOfDay);
                        decimal tienPhong = (decimal)(phong.GiaPhong * soGioSuDung);

                        // Tổng tiền cuối cùng
                        double tongTien = tongTienDichVu + (double)tienPhong;

                        var messageBoxText = $"Tên phòng: {phong.TenPhong}\nLoại phòng: {phong.LoaiPhong}\n" +
                                           $"Giá phòng: {phong.GiaPhong:C0}\n" +
                                           $"Thời gian sử dụng: {soGioSuDung} giờ\n\n" +
                                           "Danh sách dịch vụ đã sử dụng:\n";

                        foreach (var chiTietHoaDon in hoaDon.ChiTietHoaDons)
                        {
                            messageBoxText += $"{chiTietHoaDon.DichVu.TenDichVu} x {chiTietHoaDon.SoLuong} = {chiTietHoaDon.ThanhTien:C0}\n";
                        }
                        messageBoxText += $"\nTổng tiền: {tongTien:C0}";

                        var result = MessageBox.Show(messageBoxText, "Hóa đơn",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                        if (result == DialogResult.OK)
                        {
                            // Cập nhật TongTien và trạng thái
                            hoaDon.TongTien = (float)tongTien;
                            phieuDatPhong.TinhTrang = "Đã thanh toán";
                            phong.TrangThai = "Trống";

                            context.SaveChanges();
                            transaction.Commit();

                            ResetForm();
                            LoadDanhSachPhong();
                            MessageBox.Show("Đóng phòng thành công.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        transaction.Rollback();

                        var entry = ex.Entries.Single();
                        var clientValues = entry.CurrentValues;
                        var databaseEntry = entry.GetDatabaseValues();

                        if (databaseEntry == null)
                        {
                            MessageBox.Show("Dữ liệu đã bị xóa bởi người dùng khác.", "Xung đột",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            var databaseValues = databaseEntry.ToObject();
                            MessageBox.Show("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại dữ liệu.", "Xung đột",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Có lỗi xảy ra khi đóng phòng: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private double CalculateHours(TimeSpan start, TimeSpan end)
        {
            var duration = end - start;
            return Math.Ceiling(duration.TotalHours);
        }

        private void ResetForm()
        {
            maPhongHienTai = null;
            listViewEx1.Items.Clear();
            textBoxX1.Clear();
            textBoxX2.Clear();
            textBoxX3.Clear();
            textBoxX4.Clear();
        }
    }
}
