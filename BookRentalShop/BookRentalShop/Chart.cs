using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using Devart.Data.Oracle;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace BookRentalShop
{
    public partial class Chart : Form
    {
        DataTable rentTable;
        public Chart()
        {
            InitializeComponent();
        }

        private static int CalcWeekNumberFromDate(DateTime dateTime)

        {

            int iWeekNumber = 0;

            //DateTime dTime = new DateTime(year, month, day);

            string FirstOfMonth = DateTime.Now.ToString("yyMM01");

            DateTime dt = new DateTime(); DateTime.TryParse(FirstOfMonth, out dt);

            CultureInfo myCl = new CultureInfo("ko-KR");

            Calendar myCal = myCl.Calendar;

            CalendarWeekRule myCWR = myCl.DateTimeFormat.CalendarWeekRule;

            DayOfWeek myFirstDOW = myCl.DateTimeFormat.FirstDayOfWeek;


            int WeekOfToday = myCal.GetWeekOfYear(dateTime, myCWR, myFirstDOW);



            int WeekOfFirstday = myCal.GetWeekOfYear(dt, myCWR, myFirstDOW);

            int WeekOfMonth = WeekOfToday - WeekOfFirstday;

            iWeekNumber = WeekOfMonth + 1;

            return iWeekNumber;

        }

        public DateTime GetFirstDateOfWeek(int year, int week)
        {
            DateTime firstDateOfYear = new DateTime(year, 1, 1);
            DateTime firstDateOfFirstWeek = firstDateOfYear.AddDays(7 - (int)(firstDateOfYear.DayOfWeek) + 1);
            return firstDateOfFirstWeek.AddDays(7 * (week - 1));

        }

        private void Chart_Load(object sender, EventArgs e)
        {
            oracleConnection1.Open();
            rentTableAdapter1.Fill(dataSet11.RENT);
            rentTable = dataSet11.Tables["RENT"];
            

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Interval = 1;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Interval = 1;
            Series se = new Series();
            //se.ChartType = SeriesChartType.Bar;
            se.IsValueShownAsLabel = true;
            chart1.Series.Clear();
            se.Name = "대여량";
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox1.Items.Add("책 제목  빌린사람  대여날짜");
            string d1 = dateTimePicker1.Value.ToString("yyMMdd");
            int count = 0;

            switch (comboBox1.Text)
            {
                case "일간 대여 현황":
                    oracleCommand1.CommandText = "Select R_NAME, COUNT(*)borrow from rent where r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d1 + "' , 'yyMMdd') group by R_NAME Order By borrow DESC";
                    oracleCommand2.CommandText = "Select R_NAME, R_UID, R_RENTDATE from rent where r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d1 + "' , 'yyMMdd')";
                    Devart.Data.Oracle.OracleDataReader rdr = oracleCommand1.ExecuteReader();

                    while (rdr.Read())
                    {
                        DataPoint dataPoint = new DataPoint();
                        ToolTip toolTip = new ToolTip();
                        //chart1.Series[0].Points.AddXY(rdr["R_NAME"], rdr["borrow"]);
                        dataPoint.SetValueXY(rdr["R_NAME"], rdr["borrow"]);
                        dataPoint.ToolTip = rdr["R_NAME"].ToString();
                        se.Points.Add(dataPoint);

                        if (count < 5)
                        {
                            listBox2.Items.Add((++count).ToString() + "위 : " + rdr["R_NAME"].ToString() + '\n'.ToString());
                        }
                    }

                    rdr = oracleCommand2.ExecuteReader();
                    while (rdr.Read())
                    {
                        listBox1.Items.Add(rdr["R_NAME"].ToString() +" " +  rdr["R_UID"].ToString() + " " + rdr["R_RENTDATE"].ToString());
                    }

                    
                    rdr.Close();
                    break;
                case "주간 대여 현황":

                    int week = CalcWeekNumberFromDate(dateTimePicker1.Value) - 1;
                    int year = Convert.ToInt16(dateTimePicker1.Value.Year);
                    DateTime d2 = GetFirstDateOfWeek(year, week);
                    d1 = d2.ToString("yyMMdd");
                    DateTime d3 = d2.AddDays(6);
                    string d4 = d3.ToString("yyMMdd");

                    oracleCommand1.CommandText = "Select R_NAME, COUNT(*)borrow from rent where r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d4 + "' , 'yyMMdd') group by R_NAME Order By borrow DESC";
                    oracleCommand2.CommandText = "Select R_NAME, R_UID, R_RENTDATE from rent where r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d4 + "' , 'yyMMdd')";

                    rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        DataPoint dataPoint = new DataPoint();
                        ToolTip toolTip = new ToolTip();
                        dataPoint.SetValueXY(rdr["R_NAME"], rdr["borrow"]);
                        dataPoint.ToolTip = rdr["R_NAME"].ToString();
                        se.Points.Add(dataPoint);
                        if (count < 5)
                        {
                            listBox2.Items.Add((++count).ToString() + "위 : " + rdr["R_NAME"].ToString() + '\n'.ToString());
                        }
                    }
                    rdr = oracleCommand2.ExecuteReader();

                    while (rdr.Read())
                    {
                        listBox1.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + rdr["R_RENTDATE"].ToString());
                    }
                    rdr.Close();
                    break;

                case "월간 대여 현황":
                    DateTime today = dateTimePicker1.Value.Date;
                    DateTime firstDay = today.AddDays(1 - today.Day);
                    DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                    string d6 = firstDay.ToString("yyMMdd");
                    string d7 = lastDay.ToString("yyMMdd");

                    oracleCommand1.CommandText = "Select R_NAME, COUNT(*)borrow from rent where r_rentdate between to_date('" + d6 + "' , 'yyMMdd') and to_date('" + d7 + "' , 'yyMMdd') group by R_NAME Order By borrow DESC";
                    oracleCommand2.CommandText = "Select R_NAME, R_UID, R_RENTDATE from rent where r_rentdate between to_date('" + d6 + "' , 'yyMMdd') and to_date('" + d7 + "' , 'yyMMdd')";

                    rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        DataPoint dataPoint = new DataPoint();
                        ToolTip toolTip = new ToolTip();
                        dataPoint.SetValueXY(rdr["R_NAME"], rdr["borrow"]);
                        dataPoint.ToolTip = rdr["R_NAME"].ToString();
                        se.Points.Add(dataPoint);
                        if (count < 5)
                        {
                            listBox2.Items.Add((++count).ToString() + "위 : " + rdr["R_NAME"].ToString() + '\n'.ToString());
                        }
                    }
                    rdr = oracleCommand2.ExecuteReader();

                    while (rdr.Read())
                    {
                        listBox1.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + rdr["R_RENTDATE"].ToString());
                    }

                    
                    rdr.Close();
                    break;
                case "선택 날짜":

                    break;


            }
            chart1.Series.Add(se);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "일간 대여 현황":
                case "주간 대여 현황":
                case "월간 대여 현황":
                    panel1.Visible = false;
                    break;
                case "선택 날짜":
                    panel1.Visible = true;
                    break;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Interval = 1;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Interval = 1;
            listBox1.Items.Clear();
            Series se = new Series();
            //se.ChartType = SeriesChartType.Bar;
            se.IsValueShownAsLabel = true;
            chart1.Series.Clear();
            se.Name = "대여량";
            chart1.Series.Add(se);
            
            listBox2.Items.Clear();
            listBox1.Items.Clear();
            listBox1.Items.Add("책 제목  빌린사람  대여날짜");

            int count = 0;
            if (comboBox1.Text.Equals("선택 날짜"))
            {

                string d8 = dateTimePicker1.Value.Date.ToString("yyMMdd");
                string d9 = dateTimePicker2.Value.Date.ToString("yyMMdd");
                if (Convert.ToInt64(d8) > Convert.ToInt64(d9))
                {
                    MessageBox.Show("시작 날짜가 마지막 날짜보다 큽니다!");
                }
                else
                {
                    oracleCommand1.CommandText = "Select R_NAME, COUNT(*)borrow from rent where r_rentdate between to_date('" + d8 + "' , 'yyMMdd') and to_date('" + d9 + "' , 'yyMMdd') group by R_NAME Order By borrow DESC";
                    oracleCommand2.CommandText = "Select R_NAME, R_UID, R_RENTDATE from rent where r_rentdate between to_date('" + d8 + "' , 'yyMMdd') and to_date('" + d9 + "' , 'yyMMdd')";

                    Devart.Data.Oracle.OracleDataReader rdr = oracleCommand1.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart1.Series[0].Points.AddXY(rdr["R_NAME"], rdr["borrow"]);
                        if (count < 5)
                        {
                            listBox2.Items.Add((++count).ToString() + "위 : " + rdr["R_NAME"].ToString() + '\n'.ToString());
                        }
                    }
                    rdr = oracleCommand2.ExecuteReader();

                    while (rdr.Read())
                    {
                        listBox1.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + rdr["R_RENTDATE"].ToString());
                    }
                    rdr.Close();
                }
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Interval = 1;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Interval = 1;
            Series se = new Series();
            Series se1 = new Series();
            se.Sort(PointSortOrder.Ascending);
            se1.Sort(PointSortOrder.Ascending);
            //se.ChartType = SeriesChartType.Bar;
            se.IsValueShownAsLabel = true;
            se1.IsValueShownAsLabel = true;

            chart2.Series.Clear();
            se.Name = "매출";
            se1.Name = "연체료";
            chart2.Series.Add(se);
            chart2.Series.Add(se1);

            listBox3.Items.Clear();
            //listBox3.Items.Add("책 제목  빌린사람  대여날짜");
            string d1 = dateTimePicker3.Value.ToString("yyMMdd");

            int rentTotal = 0;
            int lateTotal = 0;


            switch (comboBox2.Text)
            {
                case "일간 매출 현황":
                    oracleCommand1.CommandText = "Select R_RENTDATE, SUM(O_FEE) FEE FROM OWN, RENT WHERE r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d1 + "', 'yyMMdd') and to_date('" + d1 + "' , 'yyMMdd') group by R_RENTDATE order by r_rentdate";
                    oracleCommand2.CommandText = "Select R_DEADLINE R_RENTDATE, SUM(R_LATEFEE) LATEFEE FROM RENT WHERE r_isreturn = 1 and r_deadline between to_date('" + d1 + "', 'yyMMdd') and to_date('" + d1 + "' , 'yyMMdd') group by R_DEADLINE order by r_deadline";

                    oracleCommand3.CommandText = "Select R_NAME, R_UID, R_RENTDATE, O_FEE from rent, Own where r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d1 + "' , 'yyMMdd') order by r_rentdate";
                    oracleCommand4.CommandText = "Select R_NAME, R_UID, R_DEADLINE, R_LATEFEE from rent where r_deadline between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d1 + "' , 'yyMMdd') order by r_deadline";

                    Devart.Data.Oracle.OracleDataReader rdr = oracleCommand1.ExecuteReader();

                    while (rdr.Read())
                    {
                        chart2.Series[0].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["FEE"]);
                        rentTotal += Convert.ToInt32(rdr["FEE"]);
                        
                    }

                    rdr = oracleCommand2.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart2.Series[1].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["LATEFEE"]);
                        lateTotal += Convert.ToInt32(rdr["LATEFEE"]);
                    }
                    rdr = oracleCommand3.ExecuteReader();
                    listBox3.Items.Add("대여 매출 : 책 제목 - ID - 대여 일 - 대여료");
                    while (rdr.Read())
                    {
                        listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString() + " " + rdr["O_FEE"].ToString());
                    }
                    listBox3.Items.Add("\n\n\n");
                    listBox3.Items.Add("연체료 매출 : 책 제목 - ID - 반납 일 - 연체료\n\n\n");
                    rdr = oracleCommand4.ExecuteReader();
                    while (rdr.Read())
                    {
                        listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_DEADLINE"]).ToShortDateString() + " " + rdr["R_LATEFEE"].ToString());
                    }
                    listBox3.Items.Add("\n\n\n");

                    rdr.Close();
                    break;

                case "주간 매출 현황":

                    int week = CalcWeekNumberFromDate(dateTimePicker3.Value) - 1;
                    int year = Convert.ToInt16(dateTimePicker3.Value.Year);
                    DateTime d2 = GetFirstDateOfWeek(year, week);
                    d1 = d2.ToString("yyMMdd");
                    DateTime d3 = d2.AddDays(6);
                    string d4 = d3.ToString("yyMMdd");

                    oracleCommand1.CommandText = "Select R_RENTDATE, SUM(O_FEE) FEE FROM OWN, RENT WHERE r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d1 + "', 'yyMMdd') and to_date('" + d4 + "' , 'yyMMdd') group by R_RENTDATE order by r_rentdate";
                    oracleCommand2.CommandText = "Select R_DEADLINE R_RENTDATE, SUM(R_LATEFEE) LATEFEE FROM RENT WHERE r_isreturn = 1 and r_deadline between to_date('" + d1 + "', 'yyMMdd') and to_date('" + d4 + "' , 'yyMMdd') group by R_DEADLINE order by r_deadline";

                    oracleCommand3.CommandText = "Select R_NAME, R_UID, R_RENTDATE, O_FEE from rent, Own where r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d4 + "' , 'yyMMdd') order by r_rentdate";
                    oracleCommand4.CommandText = "Select R_NAME, R_UID, R_DEADLINE, R_LATEFEE from rent where r_deadline between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d4 + "' , 'yyMMdd') order by r_deadline";

                    rdr = oracleCommand1.ExecuteReader();

                    while (rdr.Read())
                    {
                        chart2.Series[0].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["FEE"]);
                        rentTotal += Convert.ToInt32(rdr["FEE"]);

                    }

                    rdr = oracleCommand2.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart2.Series[1].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["LATEFEE"]);
                        lateTotal += Convert.ToInt32(rdr["LATEFEE"]);

                    }
                    rdr = oracleCommand3.ExecuteReader();

                    listBox3.Items.Add("대여 매출 : 책 제목 - ID - 대여 일 - 대여료");
                    while (rdr.Read())
                    {
                        listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString() + " " + rdr["O_FEE"].ToString());
                    }
                    listBox3.Items.Add("\n\n\n");
                    listBox3.Items.Add("연체료 매출 : 책 제목 - ID - 반납 일 - 연체료\n\n\n");
                    rdr = oracleCommand4.ExecuteReader();
                    while (rdr.Read())
                    {
                        listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_DEADLINE"]).ToShortDateString() + " " + rdr["R_LATEFEE"].ToString());
                    }
                    listBox3.Items.Add("\n\n\n");

                    rdr.Close();
                    break;

                case "월간 매출 현황":
                    DateTime today = dateTimePicker3.Value.Date;
                    DateTime firstDay = today.AddDays(1 - today.Day);
                    DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                    
                    string d6 = firstDay.ToString("yyMMdd");
                    string d7 = lastDay.ToString("yyMMdd");
                    oracleCommand1.CommandText = "Select R_RENTDATE, SUM(O_FEE) FEE FROM OWN, RENT WHERE r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d6 + "', 'yyMMdd') and to_date('" + d7 + "' , 'yyMMdd') group by R_RENTDATE order by r_rentdate";
                    oracleCommand2.CommandText = "Select R_DEADLINE R_RENTDATE, SUM(R_LATEFEE) LATEFEE FROM RENT WHERE r_isreturn = 1 and r_deadline between to_date('" + d6 + "', 'yyMMdd') and to_date('" + d7 + "' , 'yyMMdd') group by R_DEADLINE order by r_deadline";

                    oracleCommand3.CommandText = "Select R_NAME, R_UID, R_RENTDATE, O_FEE from rent, Own where r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d6 + "' , 'yyMMdd') and to_date('" + d7 + "' , 'yyMMdd') order by r_rentdate";
                    oracleCommand4.CommandText = "Select R_NAME, R_UID, R_DEADLINE, R_LATEFEE from rent where r_deadline between to_date('" + d6 + "' , 'yyMMdd') and to_date('" + d7 + "' , 'yyMMdd') order by r_deadline";
                    rdr = oracleCommand1.ExecuteReader();

                    while (rdr.Read())
                    {
                        chart2.Series[0].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["FEE"]);
                        rentTotal += Convert.ToInt32(rdr["FEE"]);

                    }

                    rdr = oracleCommand2.ExecuteReader();
                    while (rdr.Read())
                    {
                        chart2.Series[1].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["LATEFEE"]);
                        lateTotal += Convert.ToInt32(rdr["LATEFEE"]);

                    }
                    rdr = oracleCommand3.ExecuteReader();
                    listBox3.Items.Add("대여 매출 : 책 제목 - ID - 대여 일 - 대여료");
                    while (rdr.Read())
                    {
                        listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString() + " " + rdr["O_FEE"].ToString());
                    }
                    listBox3.Items.Add("\n\n\n");
                    listBox3.Items.Add("연체료 매출 : 책 제목 - ID - 반납 일 - 연체료\n\n\n");
                    rdr = oracleCommand4.ExecuteReader();
                    while (rdr.Read())
                    {
                        listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_DEADLINE"]).ToShortDateString() + " " + rdr["R_LATEFEE"].ToString());
                    }
                    listBox3.Items.Add("\n\n\n");

                    rdr.Close();
                    break;
                case "선택 날짜":
                    break;
            }
            label4.Text = rentTotal.ToString();
            label6.Text = lateTotal.ToString();
            label7.Text = "총 " + (rentTotal + lateTotal).ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.Text)
            {
                case "일간 매출 현황":
                case "주간 매출 현황":
                case "월간 매출 현황":
                    panel2.Visible = false;
                    break;
                case "특정 날짜":
                    panel2.Visible = true;
                    break;
            }
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            chart2.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            
            Series se = new Series();
            Series se1 = new Series();
            se.Sort(PointSortOrder.Ascending);
            se1.Sort(PointSortOrder.Ascending);

            //se.ChartType = SeriesChartType.Bar;
            se.IsValueShownAsLabel = true;
            se1.IsValueShownAsLabel = true;

            chart2.Series.Clear();
            se.Name = "매출";
            se1.Name = "연체료";
            chart2.Series.Add(se);
            chart2.Series.Add(se1);

            listBox3.Items.Clear();
            string d1 = dateTimePicker3.Value.ToString("yyMMdd");
            string d2 = dateTimePicker4.Value.ToString("yyMMdd");

            int rentTotal = 0;
            int lateTotal = 0;

            oracleCommand1.CommandText = "Select R_RENTDATE, SUM(O_FEE) FEE FROM OWN, RENT WHERE r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d1 + "', 'yyMMdd') and to_date('" + d2 + "' , 'yyMMdd') group by R_RENTDATE order by r_rentdate";
            oracleCommand2.CommandText = "Select R_DEADLINE R_RENTDATE, SUM(R_LATEFEE) LATEFEE FROM RENT WHERE r_isreturn = 1 and r_deadline between to_date('" + d1 + "', 'yyMMdd') and to_date('" + d2 + "' , 'yyMMdd') group by R_DEADLINE order by r_deadline";

            oracleCommand3.CommandText = "Select R_NAME, R_UID, R_RENTDATE, O_FEE from rent, Own where r_serialnum = o_serialnum and r_name = o_name and r_rentdate between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d2 + "' , 'yyMMdd') order by r_rentdate";
            oracleCommand4.CommandText = "Select R_NAME, R_UID, R_DEADLINE, R_LATEFEE from rent where r_deadline between to_date('" + d1 + "' , 'yyMMdd') and to_date('" + d2 + "' , 'yyMMdd') order by r_deadline";

            Devart.Data.Oracle.OracleDataReader rdr = oracleCommand1.ExecuteReader();

            while (rdr.Read())
            {
                chart2.Series[0].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["FEE"]);
                rentTotal += Convert.ToInt32(rdr["FEE"]);

            }

            rdr = oracleCommand2.ExecuteReader();
            while (rdr.Read())
            {
                chart2.Series[1].Points.AddXY(Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString(), rdr["LATEFEE"]);
                lateTotal += Convert.ToInt32(rdr["LATEFEE"]);

            }
            rdr = oracleCommand3.ExecuteReader();
            listBox3.Items.Add("대여 매출 : 책 제목 - ID - 대여 일 - 대여료");
            while (rdr.Read())
            {
                listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_RENTDATE"]).ToShortDateString() + " " + rdr["O_FEE"].ToString());
            }
            listBox3.Items.Add("\n\n\n");
            listBox3.Items.Add("연체료 매출 : 책 제목 - ID - 반납 일 - 연체료\n\n\n");
            rdr = oracleCommand4.ExecuteReader();
            while (rdr.Read())
            {
                listBox3.Items.Add(rdr["R_NAME"].ToString() + " " + rdr["R_UID"].ToString() + " " + Convert.ToDateTime(rdr["R_DEADLINE"]).ToShortDateString() + " " + rdr["R_LATEFEE"].ToString());
            }
            listBox3.Items.Add("\n\n\n");

            label4.Text = rentTotal.ToString();
            label6.Text = lateTotal.ToString();
            label7.Text = "총 " + (rentTotal + lateTotal).ToString();
            rdr.Close();
        }
    }
}