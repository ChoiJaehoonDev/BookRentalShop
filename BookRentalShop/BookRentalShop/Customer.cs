using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookRentalShop
{
    public partial class Customer : Form
    {
        bool isfilter1, isfilter2, check, isregi, isbook;
        DataTable rentTable, itemTable, reviewTable, bookingTable, ownTable, customerTable;
        public Customer()
        {
            InitializeComponent();
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            isbook = false;
            check = false;
            //Login.Uid = "customer2";
            label8.Text = "[ " + Login.Uid + " ] 님 환영합니다!";
            // TODO: 이 코드는 데이터를 'dataSet11.RENT' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            rENTTableAdapter.Fill(dataSet11.RENT);
            rentTable = dataSet11.Tables["RENT"];
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            this.oWNTableAdapter.Fill(this.dataSet11.OWN);
            // TODO: 이 코드는 데이터를 'dataSet11.ITEM' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.iTEMTableAdapter.Fill(this.dataSet11.ITEM);
            customerTableAdapter1.Fill(dataSet11.CUSTOMER);
            bookingTableAdapter1.FillByID(dataSet11.BOOKING, Login.Uid);
            itemTable = dataSet11.Tables["ITEM"];
            reviewTable = dataSet11.Tables["REVIEW"];
            rENTTableAdapter.Fill(dataSet11.RENT);
            bookingTable = dataSet11.Tables["BOOKING"];
            customerTable = dataSet11.Tables["CUSTOMER"];
            ownTable = dataSet11.Tables["OWN"];
            isfilter1 = false;
            isfilter2 = false;
            panel1.Visible = false;
            isregi = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isfilter1)
            {
                string name = textBox2.Text;
                string page = textBox3.Text;
                string genre = comboBox1.Text;
                string publi = textBox4.Text;
                string fee = textBox5.Text;
                string loc = comboBox2.Text;
                string filter = "";

                if (name.Equals("")) ;
                else
                    filter += "I_NAME LIKE '%" + name + "%'";

                if (page.Equals("")) ;
                else
                {
                    if (filter.Equals(""))
                    {
                        filter += "I_PAGE < " + page;

                    }
                    else
                    {
                        filter += " and I_PAGE < " + page;
                    }
                }
                if (genre.Equals("")) ;
                else
                {
                    if (filter.Equals(""))
                    {
                        filter += "I_GENRE LIKE '%" + genre + "%'";

                    }
                    else
                    {
                        filter += " and I_GENRE LIKE '%" + genre + "%'";
                    }
                }

                if (publi.Equals("")) ;
                else
                {
                    if (filter.Equals(""))
                    {
                        filter += "I_PUBLISHER LIKE '%" + publi + "%'";

                    }
                    else
                    {
                        filter += " and I_PUBLISHER LIKE '%" + publi + "%'";
                    }
                }
                if (fee.Equals("")) ;
                else
                {
                    if (filter.Equals(""))
                    {
                        filter += "I_FEE < " + fee;

                    }
                    else
                    {
                        filter += " and I_FEE < " + fee;
                    }
                }
                if (loc.Equals("")) ;
                else
                {
                    if (filter.Equals(""))
                    {
                        filter += "I_LOCATION LIKE '%" + loc + "%'";

                    }
                    else
                    {
                        filter += " and I_LOCATION LIKE '%" + loc + "%'";
                    }
                }
                if (filter.Equals("")) ;
                else
                {
                    iTEMBindingSource.Filter = filter;
                    button1.Text = "필터 해제";
                    isfilter1 = true;
                }
            }
            else
            {
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                comboBox1.Text = "";
                comboBox2.Text = "";
                iTEMBindingSource.RemoveFilter();
                button1.Text = "검색";
                isfilter1 = false;
            }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            string temp = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            this.oWNTableAdapter.FillByNAME(this.dataSet11.OWN, dataGridView1.CurrentRow.Cells[0].Value.ToString());
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            rENTTableAdapter.Fill(dataSet11.RENT);
            switch (comboBox3.Text.ToString())
            {
                case "현재 대여 목록":
                    rENTTableAdapter.FillByNow(dataSet11.RENT, Login.Uid);
                    break;
                case "이전 대여 목록":
                    rENTTableAdapter.FillByPre(dataSet11.RENT, Login.Uid);

                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (check)
            {
                DataRow[] findData = rentTable.Select("R_NAME = '" + dataGridView4.CurrentRow.Cells[0].Value.ToString() + "' and R_UID = '" + Login.Uid + "'");
                DataRow newRow = reviewTable.NewRow();
                newRow["RE_CONTENT"] = textBox6.Text;
                newRow["RE_NAME"] = dataGridView4.CurrentRow.Cells[0].Value.ToString();
                newRow["RE_UID"] = Login.Uid;
                newRow["RE_RENTDATE"] = findData[0]["R_RENTDATE"];
                newRow["RE_SERIALNUM"] = findData[0]["R_SERIALNUM"];
                newRow["RE_REDATE"] = DateTime.Today;
                reviewTable.Rows.Add(newRow);
                reviewTableAdapter1.Update(dataSet11.REVIEW);
                MessageBox.Show("등록되었습니다!");
                textBox6.Text = "";
                panel1.Visible = false;
                isregi = false;
                check = false;
            }

        }

        private void dataGridView5_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            listBox1.Items.Clear();
            panel2.Visible = true;
            string ID = dataGridView5.CurrentRow.Cells[0].Value.ToString();
            DateTime dateTime = Convert.ToDateTime(dataGridView5.CurrentRow.Cells[1].Value);
            string content = dataGridView5.CurrentRow.Cells[2].Value.ToString();

            listBox1.Items.Add("작 성 자 : " + ID);
            listBox1.Items.Add("\n\n" + "작성시간 : " + dateTime.ToString());
            listBox1.Items.Add("\n\n" + "내    용 : " + content);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("예약 취소하시겠습니까?", "예약 취소", MessageBoxButtons.YesNo) == DialogResult.No)
            {

            }
            else
            {
                bOOKINGBindingSource.RemoveCurrent();
                bookingTableAdapter1.Update(dataSet11.BOOKING);

            }
        }

        private void dataGridView6_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("예약 취소하시겠습니까?", "예약 취소", MessageBoxButtons.YesNo) == DialogResult.No)
            {

            }
            else
            {
                bOOKINGBindingSource.RemoveCurrent();
                bookingTableAdapter1.Update(dataSet11.BOOKING);

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataRow dataRow = customerTable.Rows.Find(Login.Uid);

            MessageBox.Show(dataRow["C_NAME"] + "님의 연체료는 " + dataRow["C_LATEFEE"].ToString() + "원 입니다.");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
            if (e.ColumnIndex == 3)

            {
                if (e.Value != null)
                {
                    bool bookcheck = true;

                    switch (e.Value.ToString())
                    {
                        case "0":
                            e.Value = "대여 가능";
                            break;
                        case "1":
                            int idx = e.RowIndex;
                            bookcheck = true;
                            foreach (DataRow dataRow in bookingTable.Rows)
                            {
                                if (dataRow["B_SERIALNUM"].ToString().Equals(ownTable.Rows[idx][1].ToString()))
                                    bookcheck = false;

                                if (bookcheck)
                                {
                                    e.Value = "예약하기";
                                    e.CellStyle.BackColor = Color.Gray;
                                }
                                else
                                {
                                    e.Value = "예약 불가";
                                    e.CellStyle.BackColor = Color.LightCoral;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataRow[] row = rentTable.Select("R_NAME = '" + dataGridView2.CurrentRow.Cells[0].Value.ToString() + "' and R_SERIALNUM = '" + dataGridView2.CurrentRow.Cells[1].Value.ToString() + "' and R_ISRETURN = 0");
            DataRow cusRow = customerTable.Rows.Find(Login.Uid);

            if (dataGridView2.CurrentRow.Cells[3].Value.ToString().Equals("1"))
            {
                if (MessageBox.Show("예약하시겠습니까?", "예약", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (cusRow["C_GRADE"].ToString().Equals("-1"))
                    {
                        MessageBox.Show("예약할 수 없습니다.\n관리자에게 문의해주십시오.");
                    }
                    else
                    {
                        foreach (DataRow dataRow in bookingTable.Rows)
                        {
                            if (dataRow["B_NAME"].ToString().Equals(dataGridView2.CurrentRow.Cells[0].Value.ToString()))
                            {
                                isbook = true;
                            }
                        }
                        if (!isbook)
                        {
                            DataRow bookRow = bookingTable.NewRow();
                            bookRow["B_NAME"] = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                            bookRow["B_ID"] = Login.Uid;
                            bookRow["B_SERIALNUM"] = dataGridView2.CurrentRow.Cells[1].Value.ToString();
                            bookRow["B_BOOKINGDATE"] = Convert.ToDateTime(row[0][4]);
                            bookingTable.Rows.Add(bookRow);
                            bookingTableAdapter1.Update(dataSet11.BOOKING);

                            MessageBox.Show("예약되었습니다!");
                        }
                        else
                        {
                            MessageBox.Show("이미 예약된 책입니다.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("대여중인 책만 예약이 가능합니다.");
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedTab.Text.ToString())
            {
                case "도서 검색":
                    this.iTEMTableAdapter.Fill(this.dataSet11.ITEM);
                    this.oWNTableAdapter.Fill(this.dataSet11.OWN);
                    rENTTableAdapter.Fill(dataSet11.RENT);
                    break;
                case "대여 목록":
                    rENTTableAdapter.FillByNow(dataSet11.RENT, Login.Uid);
                    break;
                case "후기":
                    iTEMTableAdapter.Fill(dataSet11.ITEM);
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            check = false;
            if (!isregi)
            {
                rENTTableAdapter.Fill(dataSet11.RENT);
                foreach (DataRow dataRow in rentTable.Rows)
                {
                    if (dataRow["R_UID"].Equals(Login.Uid) && dataRow["R_NAME"].Equals(dataGridView4.CurrentRow.Cells[0].Value.ToString()))
                    {
                        check = true;
                    }
                }
                if (!check)
                    MessageBox.Show("대여 기록이 없습니다.");
                else
                {
                    panel1.Visible = true;
                    isregi = true;
                }
            }
            else
            {
                textBox6.Text = "";
                panel1.Visible = false;
                isregi = false;
            }
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            reviewTableAdapter1.FillByName(dataSet11.REVIEW, dataGridView4.CurrentRow.Cells[0].Value.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt16(dataGridView3.CurrentRow.Cells[5].Value) == 0 && Convert.ToInt16(dataGridView3.CurrentRow.Cells[4].Value) == 0)
            {
                DataRow[] dataRow = rentTable.Select("R_NAME = '" + dataGridView3.CurrentRow.Cells[0].Value.ToString() + "' and R_SERIALNUM = '" + dataGridView3.CurrentRow.Cells[1].Value.ToString() + "' and R_RENTDATE = '" + Convert.ToDateTime(dataGridView3.CurrentRow.Cells[2].Value) + "'");
                DataRow[] findRow = itemTable.Select("I_NAME = '" + dataGridView3.CurrentRow.Cells[0].Value.ToString() + "'");
                dataRow[0]["R_ISEXTEND"] = 1;
                dataRow[0]["R_DEADLINE"] = Convert.ToDateTime(dataRow[0]["R_DEADLINE"]).AddDays(Convert.ToInt16(findRow[0]["I_DEADLINE"]));

                rENTTableAdapter.Update(dataSet11.RENT);
                MessageBox.Show("연장되었습니다!");
            }
            else
            {
                if (Convert.ToInt16(dataGridView3.CurrentRow.Cells[5].Value) != 0)
                {
                    MessageBox.Show("이미 연장되었습니다.");
                }
                else if (Convert.ToInt16(dataGridView3.CurrentRow.Cells[4].Value) != 0)
                {
                    MessageBox.Show("반납된 책입니다.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isfilter2)
            {
                switch (comboBox4.Text.ToString())
                {
                    case "책 제목":
                        rENTBindingSource.Filter = "R_NAME LIKE '%" + textBox1.Text + "%'";
                        break;

                }
                button2.Text = "필터해제";
                isfilter2 = true;
            }
            else
            {
                rENTBindingSource.RemoveFilter();
                button2.Text = "검색";
                isfilter2 = true;
            }

        }
    }
}
