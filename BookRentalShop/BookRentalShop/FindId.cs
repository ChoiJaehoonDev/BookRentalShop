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

    public partial class FindId : Form
    {
        DataTable customerTable;
        bool selfCheck;
        bool pwCheck;
        string changeId;
        public FindId()
        {
            InitializeComponent();
        }

        private void FindId_Load(object sender, EventArgs e)
        {
            pwCheck = false; ;
            selfCheck = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            linkLabel1.Text = "";
            oracleConnection1.Open();
            customerTableAdapter1.Fill(dataSet11.CUSTOMER);
            customerTable = dataSet11.Tables["CUSTOMER"];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow dataRow in customerTable.Rows)
            {
                if (textBox1.Text.Equals(dataRow["C_NAME"].ToString()) && textBox2.Text.Equals(dataRow["C_PHONE"].ToString()))
                {
                    string show = textBox1.Text + "님의 아이디는 " + dataRow["C_UID"] + " 입니다.";
                    MessageBox.Show(show);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selfCheck = false;
            foreach (DataRow dataRow in customerTable.Rows)
            {
                if (textBox3.Text.Equals(dataRow["C_UID"].ToString()) && textBox4.Text.Equals(dataRow["C_NAME"].ToString()) && textBox5.Text.Equals(dataRow["C_PHONE"].ToString()))
                {
                    MessageBox.Show("확인되었습니다.");
                    textBox6.ReadOnly = false;
                    textBox7.ReadOnly = false;
                    selfCheck = true;
                    changeId = textBox3.Text;
                }
            }
            if (!selfCheck)
                MessageBox.Show("아이디, 이름, 전화번호를 다시 확인해주세요.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selfCheck)
            {
                DataRow findRow = customerTable.Rows.Find(changeId);
                int selRow = customerTable.Rows.IndexOf(findRow);
                customerTable.Rows[selRow]["C_PW"] = textBox7.Text;
                int numOfRows = customerTableAdapter1.Update(dataSet11.CUSTOMER);
                if (numOfRows < 1)
                {
                    
                }
                else
                {
                    MessageBox.Show("비밀번호를 변경했습니다!");
                    this.Close();

                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            linkLabel1.LinkColor = Color.Red;
            linkLabel1.Text = "비밀번호를 확인해주세요";
            if (!textBox6.Text.ToString().Equals("") && textBox6.Text.ToString().Equals(textBox7.Text.ToString()))
            {
                linkLabel1.LinkColor = Color.Blue;
                linkLabel1.Text = "확인되었습니다!";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            selfCheck = false;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            selfCheck = false;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            selfCheck = false;
        }
    }
}
