using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace BookRentalShop
{
    public partial class Manager : Form
    {
        DataTable itemTable;
        DataTable ownTable;
        DataTable customerTable;
        DataTable rentTable;

        int r;
        bool reduction, isfilter1, isfilter2, isfilter3, isPanel, isfilter4;
        int ret1, ret2;

        static int dif;


        public Manager()
        {
            InitializeComponent();
        }

        private void Manager_Load(object sender, EventArgs e)
        {
            // TODO: 이 코드는 데이터를 'dataSet11.RENT' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.rENTTableAdapter.FillByLate(this.dataSet11.RENT, DateTime.Today);
            this.cUSTOMERTableAdapter.FillByBlack(this.dataSet11.CUSTOMER);
            // TODO: 이 코드는 데이터를 'dataSet11.CUSTOMER' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            string[] c_gerne = { "역사", "문학", "어학", "예술", "기술과학", "자연과학", "사회과학", "종교", "철학", "백과사전", "기타" };
            string[] c_loc = { "1-1", "1-2", "2-1", "2-2", "2-3", "3-1" };
            r = 0;
            isfilter1 = false;
            isfilter2 = false;
            isfilter3 = false;
            isPanel = false;
            isfilter4 = false;        
            comboBox1.Items.AddRange(c_loc);
            comboBox2.Items.AddRange(c_gerne);
            textBox11.Text = "";
            textBox10.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            itemTableAdapter1.Fill(dataSet11.ITEM);
            ownTableAdapter1.Fill(dataSet11.OWN);
            itemTable = dataSet11.Tables["ITEM"];
            ownTable = dataSet11.Tables["OWN"];
            customerTable = dataSet11.Tables["CUSTOMER"];
            reduction = false;
            dataGridView8.Columns[1].ReadOnly = true;
            dataGridView8.Columns[0].ReadOnly= true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("폐기하시겠습니까?", "물품 폐기", MessageBoxButtons.YesNo) == DialogResult.No)
            {

            }
            else
            {
                oWNBindingSource.RemoveCurrent();
                ownTableAdapter1.Update(dataSet11.OWN);
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("폐기하시겠습니까?", "물품 폐기", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                ownTableAdapter1.Update(dataSet11.OWN);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            isfilter1 = false;
            button2.Text = "검색";
            oWNBindingSource.RemoveFilter();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ownTable.Rows.Count; ++i)
            {
                DataRow dataRow = itemTable.Rows.Find(ownTable.Rows[i][0]);
                switch (ownTable.Rows[i][2].ToString())
                {            
                    
                    case "B":
                        ownTable.Rows[i][4] = Convert.ToInt32(Convert.ToDouble(dataRow[5]) * 0.9);
                        ownTable.Rows[i][5] = Convert.ToInt32(Convert.ToDouble(dataRow[6]) * 0.9);


                        break;
                    case "C":
                        ownTable.Rows[i][4] = Convert.ToInt32(Convert.ToDouble(dataRow[5]) * 0.8);
                        ownTable.Rows[i][5] = Convert.ToInt32(Convert.ToDouble(dataRow[6]) * 0.8);


                        break;
                }                
            }
            ownTableAdapter1.Update(dataSet11.OWN);
            itemTableAdapter1.Update(dataSet11.ITEM);
            MessageBox.Show("저장되었습니다!");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!isfilter3)
            {
                switch (comboBox5.Text.ToString())
                {
                    case "UID":
                        cUSTOMERBindingSource.Filter = "C_UID LIKE '%" + textBox3.Text + "%'";
                        break;
                    case "이름":
                        cUSTOMERBindingSource.Filter = "C_NAME LIKE '%" + textBox3.Text + "%'";
                        break;
                }
                button6.Text = "필터 해제";
                isfilter3 = true;
            }
            else
            {
                cUSTOMERTableAdapter.FillByBlack(dataSet11.CUSTOMER);
                isfilter3 = false;
                button6.Text = "검색";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string temp = dataGridView4.CurrentRow.Cells[0].Value.ToString();

            DataRow dataRows = customerTable.Rows.Find(temp);
            dataRows["C_GRADE"] = -1;
            cUSTOMERTableAdapter.Update(dataSet11.CUSTOMER);
            MessageBox.Show("변경되었습니다!");
            cUSTOMERTableAdapter.FillByBlack(dataSet11.CUSTOMER);
            panel1.Visible = false;
            dataGridView2.Visible = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string temp = dataGridView3.CurrentRow.Cells[0].Value.ToString();

            DataRow dataRows = customerTable.Rows.Find(temp);
            dataRows["C_GRADE"] = 0;
            cUSTOMERTableAdapter.Update(dataSet11.CUSTOMER);
            MessageBox.Show("변경되었습니다!");
            cUSTOMERTableAdapter.FillByBlack(dataSet11.CUSTOMER);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            bool aaa = false;
            foreach (DataRow dataRow in itemTable.Rows)
            {
                if (dataRow["I_NAME"].Equals(textBox4.Text))
                {
                    aaa = true;
                }
            }

            if (aaa)
            {
                DataRow newOwn = ownTable.NewRow();

                Random random = new Random();

                while (true)
                {
                    r = random.Next(0, 500);
                    foreach (DataRow data in ownTable.Rows)
                    {
                        if (r.ToString().Equals(data["O_SERIALNUM"]))
                            continue;
                    }
                    goto EXIT;
                }
            EXIT:
                newOwn["O_SERIALNUM"] = r.ToString();
                newOwn["O_NAME"] = textBox4.Text;
                newOwn["o_status"] = "A";
                newOwn["o_isown"] = 0;      //0:  보유, 1: 대여, 2:예약
                DataRow find = itemTable.Rows.Find(textBox4.Text.ToString());
                newOwn["O_FEE"] = find["I_FEE"];
                newOwn["O_LATEFEE"] = find["I_LATEFEE"];
                ownTable.Rows.Add(newOwn);
                ret2 = ownTableAdapter1.Update(dataSet11.OWN);
                MessageBox.Show("등록되었습니다!");
            }
            else
            {
                MessageBox.Show("등록된 책이 아닙니다!");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            for(int i =0; i < dataGridView5.RowCount-1; i++)
            {
                    //DataRow[] find = rentTable.Select("R_NAME = '" + dataGridView5.Rows[i].Cells[0].Value.ToString() + "' and R_UID = '" + dataGridView5.Rows[i].Cells[2].Value.ToString() + "' and R_SERIALNUM = '" + dataGridView5.Rows[i].Cells[1].Value.ToString() + "'");
                    TimeSpan timeSpan = DateTime.Today - Convert.ToDateTime(dataGridView5.Rows[i].Cells[4].Value.ToString());
                    Manager.dif = timeSpan.Days;
                    string book = dataGridView5.Rows[i].Cells[0].Value.ToString();
                    string id = dataGridView5.Rows[i].Cells[2].Value.ToString();
                    DataRow[] dataRow = ownTable.Select("O_NAME = '" + book + "' and O_SERIALNUM = '" + dataGridView5.Rows[i].Cells[1].Value.ToString() + "'");
                    int late = Convert.ToInt16(dataRow[0]["O_LATEFEE"]);
                    SendMail(id, book, late, dif);  //UID, 책 제목, 해당 책의 연체료
                
            }           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!isPanel)
            {
                panel1.Visible = true;
                cUSTOMERTableAdapter.FillByWhite(dataSet11.CUSTOMER);
                isPanel = true;
            }
            else
            {
                panel1.Visible = false;
                cUSTOMERTableAdapter.FillByBlack(dataSet11.CUSTOMER);
                isPanel = false;

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedTab.Text.ToString())
            {
                case "등록":
                    this.rENTTableAdapter.FillByLate(this.dataSet11.RENT, DateTime.Today);
                    this.cUSTOMERTableAdapter.FillByBlack(this.dataSet11.CUSTOMER);
                    break;
                case "폐기":
                    this.rENTTableAdapter.FillByLate(this.dataSet11.RENT, DateTime.Today);
                    this.cUSTOMERTableAdapter.FillByBlack(this.dataSet11.CUSTOMER);
                    break;
                case "물품관리":
                    break;
                case "블랙리스트 관리":
                    this.rENTTableAdapter.FillByLate(this.dataSet11.RENT, DateTime.Today);
                    this.cUSTOMERTableAdapter.FillByBlack(this.dataSet11.CUSTOMER);
                    break;
                case "주별 대여현황":
                    break;
                case "기간별 매출":
                    break;
                case "연체 목록":
                    this.cUSTOMERTableAdapter.Fill(dataSet11.CUSTOMER);
                    break;
            }
        }

        private void dataGridView6_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string name = dataGridView6.CurrentRow.Cells[0].Value.ToString();
            panel2.Visible = true;
            rENTTableAdapter.FillByNAME(dataSet11.RENT, name);

        }

        private void button13_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Chart chart = new Chart();
            chart.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //DataRow[] dataRow = ownTable.Select("O_NAME = '" + dataGridView2.CurrentRow.Cells[0].Value.ToString() + "'");
            ownTableAdapter1.FillByNAME(dataSet11.OWN, dataGridView2.CurrentRow.Cells[0].Value.ToString());
            panel3.Visible = true;

        }

        private void button17_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            ownTableAdapter1.Fill(dataSet11.OWN);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ownTableAdapter1.Update(dataSet11.OWN);
            ownTableAdapter1.Fill(dataSet11.OWN);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.ColumnIndex == 3)
            {
                if(e.Value != null)
                {
                    switch (e.Value.ToString())
                    {
                        case "0":
                            e.Value = "보유";
                            break;
                        case "1":
                            e.Value = "대여";
                            break;
                    }
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (!isfilter4)
            {
                iTEMBindingSource1.Filter = "I_NAME LIKE '%" + textBox6.Text + "%'";
                button14.Text = "필터 해제";
                isfilter4 = true;
            }
            else
            {
                iTEMBindingSource1.RemoveFilter();
                button14.Text = "검색";
                textBox6.Text = "";
            }
        }

        private void dataGridView2_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show("삭제할 수 없습니다.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!isfilter2)
            {
                switch (comboBox4.Text.ToString())
                {
                    case "책 제목":
                        iTEMBindingSource.Filter = "I_NAME LIKE '%" + textBox2.Text.ToString() + "%'";
                        break;
                    case "장르":
                        iTEMBindingSource.Filter = "I_GENRE LIKE '%" + textBox2.Text.ToString() + "%'";
                        break;
                    case "위치":
                        iTEMBindingSource.Filter = "I_LOCATION = '" + textBox1.Text.ToString() + "'";
                        break;
                }
                button2.Text = "필터 해제";
                isfilter1 = true;
            }
            else
            {
                oWNBindingSource.RemoveFilter();
                button2.Text = "검색";
                textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isfilter1)
            {
                switch (comboBox3.Text.ToString())
                {
                    case "책 제목":
                        oWNBindingSource.Filter = "O_NAME LIKE '%" + textBox1.Text.ToString() + "%'";
                        break;
                    case "일련번호":
                        oWNBindingSource.Filter = "O_SERIALNUM LIKE '%" + textBox1.Text.ToString() + "%'";
                        break;
                    case "등급":
                        oWNBindingSource.Filter = "O_NAME = '" + textBox1.Text.ToString() + "'";
                        break;
                }
                button2.Text = "필터 해제";
                isfilter1 = true;
            }
            else
            {
                oWNBindingSource.RemoveFilter();
                button2.Text = "검색";
                textBox1.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DataRow newItem = itemTable.NewRow();
            DataRow newOwn = ownTable.NewRow();
            foreach (DataRow dataRow in itemTable.Rows)
            {
                if (dataRow["I_NAME"].Equals(textBox4.Text))
                {
                    reduction = true;
                    goto RE;

                }
            }

            newItem["I_NAME"] = textBox4.Text;
            newItem["I_PAGE"] = textBox5.Text;
            newItem["I_GENRE"] = comboBox2.Text;
            newItem["I_PUBLISHER"] = textBox7.Text;
            newItem["I_AUTHORITY"] = textBox8.Text;
            newItem["I_FEE"] = Convert.ToInt32(textBox9.Text);
            newItem["I_LATEFEE"] = Convert.ToInt32(textBox10.Text);
            newItem["I_DEADLINE"] = Convert.ToInt32(textBox11.Text);
            newItem["I_LOCATION"] = comboBox1.Text;
            itemTable.Rows.Add(newItem);
            ret1 = itemTableAdapter1.Update(dataSet11.ITEM);

        RE:
            Random random = new Random();

            while (true)
            {
                r = random.Next(0, 500);
                foreach (DataRow data in ownTable.Rows)
                {
                    if (r.ToString().Equals(data["O_SERIALNUM"]))
                        continue;
                }
                goto EXIT;
            }
        EXIT:
            newOwn["O_SERIALNUM"] = r.ToString();
            newOwn["O_NAME"] = textBox4.Text;
            newOwn["o_status"] = "A";
            newOwn["o_isown"] = 0;      //0:  보유, 1: 대여, 2:예약
            ownTable.Rows.Add(newOwn);
            ret2 = ownTableAdapter1.Update(dataSet11.OWN);
            MessageBox.Show("등록되었습니다!");

        }
        void SendMail(string ID, string name, int late, int dif)
        {
            DataRow latecutomer = customerTable.Rows.Find(ID);
            string toAddress = latecutomer["C_MAIL"].ToString();
            string sendAddress = "seodanggoldb@gmail.com";
            string sendPassword = "tjekdrhf1!!";
            SmtpClient smtp = null;
            MailMessage message = null;
            string latefee = latecutomer["C_LATEFEE"].ToString();
            string subject = "미반납 도서 공지입니다.";
            string body = "안녕하세요, 책 대여점입니다. \n" +
                    "고객님이 대여하신 " + name + "(이)가 반납 예정일이 지났습니다.\n" +
                    "연체료는 하루에" +late.ToString()+ "원 만큼 부과되고, " + dif.ToString()+ "일 연체되어 " + Convert.ToString(late * dif) + "원의 연체료가 발생 하였으며, \n"+" 총" + latefee + "원 만큼 납부해주시기 바랍니다.\n" +
                    "연체료가 일정 금액 이상일 시 이용에 제한이 있으니 주의해주시기 바랍니다.\n-대여점 서당골";
            try
            {
                smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(sendAddress, sendPassword),
                    Timeout = 20000
                };
                message = new MailMessage(sendAddress, toAddress)
                {
                    Subject = subject,
                    Body = body              
                };
                smtp.Send(message);
                MessageBox.Show(ID + "님 에게 메일을 보냈습니다.");
            }
            catch(Exception e)
            {
                MessageBox.Show(ID + "님 에게 메일을 보내지 못했습니다.");
                MessageBox.Show(e.ToString());
            }
            finally
            {
                if (smtp != null)
                    smtp.Dispose();
                if (message != null)
                    message.Dispose();
            }
        }
    }
}
