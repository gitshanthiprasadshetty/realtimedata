using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {             
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReference2.BcmsDashboardServiceClient obj = new ServiceReference2.BcmsDashboardServiceClient();
                CMDataCollector.Models.BcmsDashboard result = obj.MonitorBcmsForSkill(textBox1.Text);
                //dataGridView1.datab
                List<BCMS> dataObj = new List<BCMS>();
                BCMS data = new BCMS();
                if (result != null)
                {
                    data.AccptedSL = result.AccptedSL;
                    data.ACD = result.ACD;
                    data.ACW = result.ACW;
                    data.AUX = result.AUX;
                    data.Avail = result.Avail;
                    data.CallsWaiting = result.CallsWaiting;
                    data.Date = result.Date;
                    data.Extn = result.Extn;
                    data.OldestCall = result.OldestCall;
                    data.Other = result.Other;
                    data.Skill = result.Skill;
                    data.SkillName = result.SkillName;
                    data.SL = result.SL;
                    data.Staff = result.Staff;
                    data.AbandCalls = result.AbandCalls;
                    data.ACDCallsSummary = result.AcdCallsSummary;
                    dataObj.Add(data);
                    dataGridView1.DataSource = dataObj;
                }
                else
                    label1.Text = "No data to show";
            }
            catch (Exception ex)
            {
                label1.Text = "Error : " + ex.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReference2.BcmsDashboardServiceClient obj = new ServiceReference2.BcmsDashboardServiceClient();

                var resultList = obj.MonitorBcms();               
                List<BCMS> dataObj = new List<BCMS>();
                foreach (var result in resultList)
                {
                    BCMS data = new BCMS();
                    data.AccptedSL = result.AccptedSL;
                    data.ACD = result.ACD;
                    data.ACW = result.ACW;
                    data.AUX = result.AUX;
                    data.Avail = result.Avail;
                    data.CallsWaiting = result.CallsWaiting;
                    data.Date = result.Date;
                    data.Extn = result.Extn;
                    data.OldestCall = result.OldestCall;
                    data.Other = result.Other;
                    data.Skill = result.Skill;
                    data.SkillName = result.SkillName;
                    data.SL = result.SL;
                    data.Staff = result.Staff;
                    data.AbandCalls = result.AbandCalls;
                    data.ACDCallsSummary = result.AcdCallsSummary;
                    dataObj.Add(data);
                }
                dataGridView1.DataSource = dataObj;
            }
            catch (Exception ex)
            {
                label1.Text = "Error : " + ex.Message;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReference2.BcmsDashboardServiceClient obj = new ServiceReference2.BcmsDashboardServiceClient();
               
                //obj.StartBCMS();
                CMDataCollector.BCMSDashboardManager.Start();               
            }
            catch (Exception ex)
            {
                label1.Text = "Service start failed!";
                CMDataCollector.BCMSDashboardManager.Stop();
            }
        }

        private void btnGetHistoricalData_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReference3.BcmsHistoricalServiceClient obj = new ServiceReference3.BcmsHistoricalServiceClient();
                CMDataCollector.BcmsHistoricalDashboard.BCMSSkill resultdata = obj.GetBcmsHistoricalData(textBox2.Text);
                BCMSSkill data = new BCMSSkill();
                List<BCMSSkill> dataList = new List<BCMSSkill>();
                if (resultdata != null)
                {
                    data.Abandoned_Calls = resultdata.Abandoned_Calls;
                    data.Acceptable_Service_Level = resultdata.Acceptable_Service_Level;
                    data.Acd_Calls = resultdata.Acd_Calls;
                    data.Avg_Abandoned_Time = resultdata.Avg_Abandoned_Time;
                    data.Avg_Speed_Answered = resultdata.Avg_Speed_Answered;
                    data.Avg_Staff = resultdata.Avg_Staff;
                    data.Avg_Talk_Time = resultdata.Avg_Talk_Time;
                    data.Date = resultdata.Date;
                    data.Flow_In = resultdata.Flow_In;
                    data.Flow_Out = resultdata.Flow_Out;
                    data.Interval = resultdata.Interval;
                    data.Pct_In_Svc_Level = resultdata.Pct_In_Svc_Level;
                    data.Report_Type = resultdata.Report_Type;
                    data.Skill = resultdata.Skill;
                    data.Total_After_Call = resultdata.Total_After_Call;
                    data.Total_Aux_Other = resultdata.Total_Aux_Other;
                    dataList.Add(data);
                    dataGridView2.DataSource = dataList;
                }
                else
                    label1.Text = "No data to show";
            }
            catch (Exception ex)
            {
                label1.Text = "Error : " + ex;
            }
        }
    }
}
