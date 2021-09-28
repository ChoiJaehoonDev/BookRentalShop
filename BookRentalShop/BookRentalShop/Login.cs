using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookRentalShop
{
    public partial class Login : Form
    {
        public static string Uid;
        DataTable customerTable, rentTable, ownTable;
        bool idCheck;
        bool idCheck1;
        bool pwCheck;
        bool islogin;
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            linkLabel3.Text = "";
            idCheck = false;
            idCheck1 = true;
            pwCheck = false;
            islogin = true;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            panel1.Visible = false;
            rentTableAdapter1.Fill(dataSet11.RENT);
            customerTableAdapter1.Fill(dataSet11.CUSTOMER);
            ownTableAdapter1.Fill(dataSet11.OWN);
            customerTable = dataSet11.Tables["CUSTOMER"];
            rentTable = dataSet11.Tables["RENT"];
            ownTable = dataSet11.Tables["own"];
            //------------------- 처음에 화면 띄울 때, 모든 정보 업데이트

            foreach(DataRow dataRow in customerTable.Rows)
            {
                dataRow["C_LATEFEE"] = 0;
            }
            customerTableAdapter1.Update(dataSet11.CUSTOMER);
            foreach(DataRow dataRow in rentTable.Rows)
            {
                if(dataRow["R_ISRETURN"].ToString().Equals("0"))
                    dataRow["R_LATEFEE"] = 0;
            }
            rentTableAdapter1.Update(dataSet11.RENT);

            foreach (DataRow dataRow in rentTable.Rows) {       ///own 테이블에서 연체료 정보 받아서 렌트테이블에 연체료 더해주고, 누적합을 회원테이블에 저장
                TimeSpan a = DateTime.Today - Convert.ToDateTime(dataRow["R_DEADLINE"]);
                int daydif = a.Days;
                if(daydif >= 0)
                {
                    DataRow[] own = ownTable.Select("O_NAME = '" + dataRow["R_NAME"] + "' and O_SERIALNUM = '" + dataRow["R_SERIALNUM"] + "'");
                    if (Convert.ToInt16(dataRow["R_ISRETURN"]) == 0)
                    {
                        dataRow["R_LATEFEE"] = Convert.ToInt16(own[0]["O_LATEFEE"]) * daydif;

                        DataRow cus = customerTable.Rows.Find(dataRow["R_UID"]);
                        cus["C_LATEFEE"] = Convert.ToInt32(cus["C_LATEFEE"]) + Convert.ToInt16(dataRow["R_LATEFEE"]);
                    }
                }

             }
            customerTableAdapter1.Update(dataSet11.CUSTOMER);
            rentTableAdapter1.Update(dataSet11.RENT);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            islogin = true;
            foreach (DataRow mydataRow in customerTable.Rows)
            {
                if (textBox1.Text.Equals(mydataRow["C_UID"].ToString()) && textBox2.Text.Equals(mydataRow["C_PW"].ToString()))
                {
                    DataRow[] foundRows = customerTable.Select("C_UID = '" + textBox1.Text + "'");
                    Uid = textBox1.Text;
                    foreach(DataRow dataRow2 in foundRows)
                    {
                        int grade = Convert.ToInt16(dataRow2["C_GRADE"]);
                        switch (grade)
                        {
                            case -1:                                
                            case 0:
                                Customer customer = new Customer();
                                customer.Show();
                                islogin = false;
                                this.Visible = false;
                                break;
                            case 1:
                                Staff staff = new Staff();
                                staff.Show();
                                islogin = false;
                                this.Visible = false;
                                break;
                            case 2:
                                Manager manager = new Manager();
                                manager.Show();
                                islogin = false;
                                this.Visible = false;
                                break;
                        }
                    }
                }                
            }
            if(islogin)
                MessageBox.Show("아이디 혹은 비밀번호가 잘못 되었습니다.");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FindId findId = new FindId();
            findId.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            idCheck1 = true;
            for(int i =0; i< customerTable.Rows.Count; ++i)
            {
                if (textBox3.Text.ToString().Equals(customerTable.Rows[i]["C_UID"]))
                {
                    MessageBox.Show("중복된 아이디 입니다!");
                    idCheck1 = false;
                }
            }
            if (textBox3.Text.ToString().Equals(""))
            {
                MessageBox.Show("아이디를 입력해주세요.");
            }
            else if(idCheck1)
            {
                MessageBox.Show("사용가능한 아이디입니다!");
                idCheck = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(Char.IsLetter(e.KeyChar)) && !(Char.IsDigit(e.KeyChar)) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            linkLabel3.LinkColor = Color.Red;
            linkLabel3.Text = "비밀번호를 확인해주세요";
            pwCheck = false;
            if(!textBox5.Text.ToString().Equals("") && textBox5.Text.ToString().Equals(textBox6.Text.ToString()))
            {
                pwCheck = true;
                linkLabel3.LinkColor = Color.Blue;
                linkLabel3.Text = "확인되었습니다!";
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (idCheck && pwCheck)
            {
                DataRow newRow = customerTable.NewRow();
                newRow["C_UID"] = textBox3.Text;
                newRow["C_NAME"] = textBox4.Text;
                newRow["C_PW"] = textBox6.Text;
                newRow["C_mail"] = textBox7.Text;
                newRow["C_PHONE"] = textBox8.Text;
                newRow["C_GRADE"] = 0;
                newRow["C_LATEFEE"] = 0;
                customerTable.Rows.Add(newRow);
                customerTableAdapter1.Update(dataSet11.CUSTOMER);
                MessageBox.Show("축하합니다! 가입되었습니다!");
                panel1.Visible = false;
            }
            else
            {
                MessageBox.Show("ID 및 PW를 확인하세요!");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            idCheck = false;
        }
    }
}
