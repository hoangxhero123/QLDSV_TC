using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLDSV
{
    public partial class frmDangKy : DevExpress.XtraEditors.XtraForm
    {
        private BindingSource bdsSinhVien = new BindingSource();
        private BindingSource bdsLopTinchi = new BindingSource();
        private BindingSource bdsDSLTC_HUY = new BindingSource();
        public frmDangKy()
        {
            InitializeComponent();
        }
        // comment đang kí 3
        private void frmDangKy_Load(object sender, EventArgs e)
        {
            loadcbNienkhoa();
            cbNienKhoa.SelectedIndex = 0;
            txbMASV.Text = Program.username;
        }

        private void btnTimSV_Click(object sender, EventArgs e)
        {
            if (txbMASV.Text.Trim() == "")
            {
                MessageBox.Show("Mã sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                txbMASV.Focus();
                return;
            }
            if (txbMASV.Text != Program.username)
            {
                MessageBox.Show("Bạn không phải là tài khoản sinh viên này!", "", MessageBoxButtons.OK);
                txbMASV.Focus();
                return;
            }
            txbMaSVDK.Text = txbMASV.Text;
            string cmd = "EXEC dbo.SP_getInfoSVDKI '" + txbMASV.Text + "'";
            string cmd1 = "EXEC dbo.SP_LIST_SVHUYDANGKY '" + txbMASV.Text + "'";
            DataTable tableSV = Program.ExecSqlDataTable(cmd);
            DataTable tableDSLTC_HUY = Program.ExecSqlDataTable(cmd1);

            this.bdsSinhVien.DataSource = tableSV;
            this.bdsDSLTC_HUY.DataSource = tableDSLTC_HUY;
            this.gridSV.DataSource = this.bdsSinhVien;
            this.gridLTCHuy.DataSource = this.bdsDSLTC_HUY;
        }
        void loadcbNienkhoa()
        {
            DataTable dt = new DataTable();
            string cmd = "EXEC dbo.GetAllNienKhoa";
            dt = Program.ExecSqlDataTable(cmd);

            BindingSource bdsNienKhoa = new BindingSource();
            bdsNienKhoa.DataSource = dt;
            cbNienKhoa.DataSource = bdsNienKhoa;
            cbNienKhoa.DisplayMember = "NIENKHOA";
            cbNienKhoa.ValueMember = "NIENKHOA";
        }
        void loadcbHocKi(string nienkhoa)
        {
            DataTable dt = new DataTable();
            string cmd = "EXEC dbo.GetAllHocKy '" + nienkhoa + "'";
            dt = Program.ExecSqlDataTable(cmd);

            BindingSource bdsHocKi = new BindingSource();
            bdsHocKi.DataSource = dt;
            cbHocKy.DataSource = bdsHocKi;
            cbHocKy.DisplayMember = "HOCKY";
            cbHocKy.ValueMember = "HOCKY";
        }

        private void btnTimLTC_Click(object sender, EventArgs e)
        {
            string cmd = "EXEC [dbo].[SP_InDanhSachLopTinChi] '" + cbNienKhoa.Text + "', '" + cbHocKy.Text + "'";
            DataTable tableLopTC = Program.ExecSqlDataTable(cmd);
            this.bdsLopTinchi.DataSource = tableLopTC;
            this.gridLTC.DataSource = this.bdsLopTinchi;
        }

        private void cbNienKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadcbHocKi(cbNienKhoa.Text);
            //cbHocKy.SelectedIndex = 0;
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            if (txbMaSVDK.Text.Trim() == "")
            {
                MessageBox.Show("Mã sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                txbMaSVDK.Focus();
                return;
            }
            if (txbMALTCDK.Text.Trim() == "")
            {
                MessageBox.Show("Mã lớp tín chỉ không được thiếu!", "", MessageBoxButtons.OK);
                txbMALTCDK.Focus();
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng kí lớp học này ?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string cmd = "EXEC [dbo].[SP_XULY_LTC] '" + txbMaSVDK.Text + "' , '" + txbMALTCDK.Text + "', " + 1;
                if (Program.ExecSqlNonQuery(cmd) == 0)
                {
                    MessageBox.Show("Đăng kí thành công!");
                    string cmd1 = "EXEC dbo.SP_LIST_SVHUYDANGKY '" + txbMASV.Text + "'";
                    DataTable tableDSLTC_HUY = Program.ExecSqlDataTable(cmd1);
                    this.bdsDSLTC_HUY.DataSource = tableDSLTC_HUY;
                    this.gridLTCHuy.DataSource = this.bdsDSLTC_HUY;

                    btnTimLTC.PerformClick();
                }
                else
                {
                    MessageBox.Show("Đăng kí thất bại");
                }
            }
            else
            {
                return;
            }
        }

        private void btnHuyDangKy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txbMaSVDK.Text.Trim() == "")
            {
                MessageBox.Show("Mã sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                txbMaSVDK.Focus();
                return;
            }
            if (bdsDSLTC_HUY.Position < 0)
            {
                MessageBox.Show("Bạn chưa chọn lớp tín chỉ để hủy");
                gridLTCHuy.Focus();
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy đăng kí lớp học này ?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string maltc = "";
                if (((DataRowView)bdsDSLTC_HUY[bdsDSLTC_HUY.Position])["MALTC"] != null)
                {
                    maltc = ((DataRowView)bdsDSLTC_HUY[bdsDSLTC_HUY.Position])["MALTC"].ToString();
                }

                string cmd = "EXEC [dbo].[SP_XULY_LTC] '" + txbMaSVDK.Text + "' , '" + maltc + "', " + 2;
                if (Program.ExecSqlNonQuery(cmd) == 0)
                {
                    MessageBox.Show("Hủy đăng kí thành công!");
                    string cmd1 = "EXEC dbo.SP_LIST_SVHUYDANGKY '" + txbMASV.Text + "'";
                    DataTable tableDSLTC_HUY = Program.ExecSqlDataTable(cmd1);
                    this.bdsDSLTC_HUY.DataSource = tableDSLTC_HUY;
                    this.gridLTCHuy.DataSource = this.bdsDSLTC_HUY;

                    btnTimLTC.PerformClick();
                }
                else
                {
                    MessageBox.Show("Hủy đăng kí thất bại");
                }
            }
            else
            {
                return;
            }
        }

        private void btnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gridLTC_MouseClick(object sender, MouseEventArgs e)
        {
            if (bdsLopTinchi.Count > 0)
            {
                txbMALTCDK.Text = ((DataRowView)bdsLopTinchi[bdsLopTinchi.Position])["MALTC"].ToString();
            }
        }
    }
}