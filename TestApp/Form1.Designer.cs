namespace TestApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.skillDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.skillNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.callsWaitingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accptedSLDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oldestCallDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sLDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.staffDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.availDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aCWDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aUXDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.extnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.otherDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abandCallsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgAbandTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acdCallsSummaryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bcmsDashboardBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnGetHistoricalData = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.bCMSSkillBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.abandonedCallsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acceptableServiceLevelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acdCallsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgAbandonedTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgSpeedAnsweredDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgStaffDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgTalkTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowInDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowOutDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.intervalDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pctInSvcLevelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reportTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.skillDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.skillNameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalAfterCallDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalAuxOtherDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bcmsDashboardBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bCMSSkillBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(155, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Monitor Skill";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(35, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(529, 25);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Monitor All Skills";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "result :";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.skillDataGridViewTextBoxColumn,
            this.dateDataGridViewTextBoxColumn,
            this.skillNameDataGridViewTextBoxColumn,
            this.callsWaitingDataGridViewTextBoxColumn,
            this.accptedSLDataGridViewTextBoxColumn,
            this.oldestCallDataGridViewTextBoxColumn,
            this.sLDataGridViewTextBoxColumn,
            this.staffDataGridViewTextBoxColumn,
            this.availDataGridViewTextBoxColumn,
            this.aCDDataGridViewTextBoxColumn,
            this.aCWDataGridViewTextBoxColumn,
            this.aUXDataGridViewTextBoxColumn,
            this.extnDataGridViewTextBoxColumn,
            this.otherDataGridViewTextBoxColumn,
            this.abandCallsDataGridViewTextBoxColumn,
            this.avgAbandTimeDataGridViewTextBoxColumn,
            this.acdCallsSummaryDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.bcmsDashboardBindingSource;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridView1.Location = new System.Drawing.Point(35, 76);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(598, 150);
            this.dataGridView1.TabIndex = 6;
            // 
            // skillDataGridViewTextBoxColumn
            // 
            this.skillDataGridViewTextBoxColumn.DataPropertyName = "Skill";
            this.skillDataGridViewTextBoxColumn.HeaderText = "Skill";
            this.skillDataGridViewTextBoxColumn.Name = "skillDataGridViewTextBoxColumn";
            // 
            // dateDataGridViewTextBoxColumn
            // 
            this.dateDataGridViewTextBoxColumn.DataPropertyName = "Date";
            this.dateDataGridViewTextBoxColumn.HeaderText = "Date";
            this.dateDataGridViewTextBoxColumn.Name = "dateDataGridViewTextBoxColumn";
            // 
            // skillNameDataGridViewTextBoxColumn
            // 
            this.skillNameDataGridViewTextBoxColumn.DataPropertyName = "SkillName";
            this.skillNameDataGridViewTextBoxColumn.HeaderText = "SkillName";
            this.skillNameDataGridViewTextBoxColumn.Name = "skillNameDataGridViewTextBoxColumn";
            // 
            // callsWaitingDataGridViewTextBoxColumn
            // 
            this.callsWaitingDataGridViewTextBoxColumn.DataPropertyName = "CallsWaiting";
            this.callsWaitingDataGridViewTextBoxColumn.HeaderText = "CallsWaiting";
            this.callsWaitingDataGridViewTextBoxColumn.Name = "callsWaitingDataGridViewTextBoxColumn";
            // 
            // accptedSLDataGridViewTextBoxColumn
            // 
            this.accptedSLDataGridViewTextBoxColumn.DataPropertyName = "AccptedSL";
            this.accptedSLDataGridViewTextBoxColumn.HeaderText = "AccptedSL";
            this.accptedSLDataGridViewTextBoxColumn.Name = "accptedSLDataGridViewTextBoxColumn";
            // 
            // oldestCallDataGridViewTextBoxColumn
            // 
            this.oldestCallDataGridViewTextBoxColumn.DataPropertyName = "OldestCall";
            this.oldestCallDataGridViewTextBoxColumn.HeaderText = "OldestCall";
            this.oldestCallDataGridViewTextBoxColumn.Name = "oldestCallDataGridViewTextBoxColumn";
            // 
            // sLDataGridViewTextBoxColumn
            // 
            this.sLDataGridViewTextBoxColumn.DataPropertyName = "SL";
            this.sLDataGridViewTextBoxColumn.HeaderText = "SL";
            this.sLDataGridViewTextBoxColumn.Name = "sLDataGridViewTextBoxColumn";
            // 
            // staffDataGridViewTextBoxColumn
            // 
            this.staffDataGridViewTextBoxColumn.DataPropertyName = "Staff";
            this.staffDataGridViewTextBoxColumn.HeaderText = "Staff";
            this.staffDataGridViewTextBoxColumn.Name = "staffDataGridViewTextBoxColumn";
            // 
            // availDataGridViewTextBoxColumn
            // 
            this.availDataGridViewTextBoxColumn.DataPropertyName = "Avail";
            this.availDataGridViewTextBoxColumn.HeaderText = "Avail";
            this.availDataGridViewTextBoxColumn.Name = "availDataGridViewTextBoxColumn";
            // 
            // aCDDataGridViewTextBoxColumn
            // 
            this.aCDDataGridViewTextBoxColumn.DataPropertyName = "ACD";
            this.aCDDataGridViewTextBoxColumn.HeaderText = "ACD";
            this.aCDDataGridViewTextBoxColumn.Name = "aCDDataGridViewTextBoxColumn";
            // 
            // aCWDataGridViewTextBoxColumn
            // 
            this.aCWDataGridViewTextBoxColumn.DataPropertyName = "ACW";
            this.aCWDataGridViewTextBoxColumn.HeaderText = "ACW";
            this.aCWDataGridViewTextBoxColumn.Name = "aCWDataGridViewTextBoxColumn";
            // 
            // aUXDataGridViewTextBoxColumn
            // 
            this.aUXDataGridViewTextBoxColumn.DataPropertyName = "AUX";
            this.aUXDataGridViewTextBoxColumn.HeaderText = "AUX";
            this.aUXDataGridViewTextBoxColumn.Name = "aUXDataGridViewTextBoxColumn";
            // 
            // extnDataGridViewTextBoxColumn
            // 
            this.extnDataGridViewTextBoxColumn.DataPropertyName = "Extn";
            this.extnDataGridViewTextBoxColumn.HeaderText = "Extn";
            this.extnDataGridViewTextBoxColumn.Name = "extnDataGridViewTextBoxColumn";
            // 
            // otherDataGridViewTextBoxColumn
            // 
            this.otherDataGridViewTextBoxColumn.DataPropertyName = "Other";
            this.otherDataGridViewTextBoxColumn.HeaderText = "Other";
            this.otherDataGridViewTextBoxColumn.Name = "otherDataGridViewTextBoxColumn";
            // 
            // abandCallsDataGridViewTextBoxColumn
            // 
            this.abandCallsDataGridViewTextBoxColumn.DataPropertyName = "AbandCalls";
            this.abandCallsDataGridViewTextBoxColumn.HeaderText = "AbandCalls";
            this.abandCallsDataGridViewTextBoxColumn.Name = "abandCallsDataGridViewTextBoxColumn";
            // 
            // avgAbandTimeDataGridViewTextBoxColumn
            // 
            this.avgAbandTimeDataGridViewTextBoxColumn.DataPropertyName = "AvgAbandTime";
            this.avgAbandTimeDataGridViewTextBoxColumn.HeaderText = "AvgAbandTime";
            this.avgAbandTimeDataGridViewTextBoxColumn.Name = "avgAbandTimeDataGridViewTextBoxColumn";
            // 
            // acdCallsSummaryDataGridViewTextBoxColumn
            // 
            this.acdCallsSummaryDataGridViewTextBoxColumn.DataPropertyName = "AcdCallsSummary";
            this.acdCallsSummaryDataGridViewTextBoxColumn.HeaderText = "AcdCallsSummary";
            this.acdCallsSummaryDataGridViewTextBoxColumn.Name = "acdCallsSummaryDataGridViewTextBoxColumn";
            // 
            // bcmsDashboardBindingSource
            // 
            this.bcmsDashboardBindingSource.DataSource = typeof(CMDataCollector.Models.BcmsDashboard);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(35, 290);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 7;
            // 
            // btnGetHistoricalData
            // 
            this.btnGetHistoricalData.Location = new System.Drawing.Point(155, 288);
            this.btnGetHistoricalData.Name = "btnGetHistoricalData";
            this.btnGetHistoricalData.Size = new System.Drawing.Size(111, 23);
            this.btnGetHistoricalData.TabIndex = 8;
            this.btnGetHistoricalData.Text = "GetHistoricalData";
            this.btnGetHistoricalData.UseVisualStyleBackColor = true;
            this.btnGetHistoricalData.Click += new System.EventHandler(this.btnGetHistoricalData_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.abandonedCallsDataGridViewTextBoxColumn,
            this.acceptableServiceLevelDataGridViewTextBoxColumn,
            this.acdCallsDataGridViewTextBoxColumn,
            this.avgAbandonedTimeDataGridViewTextBoxColumn,
            this.avgSpeedAnsweredDataGridViewTextBoxColumn,
            this.avgStaffDataGridViewTextBoxColumn,
            this.avgTalkTimeDataGridViewTextBoxColumn,
            this.dateDataGridViewTextBoxColumn1,
            this.flowInDataGridViewTextBoxColumn,
            this.flowOutDataGridViewTextBoxColumn,
            this.intervalDataGridViewTextBoxColumn,
            this.pctInSvcLevelDataGridViewTextBoxColumn,
            this.reportTypeDataGridViewTextBoxColumn,
            this.skillDataGridViewTextBoxColumn1,
            this.skillNameDataGridViewTextBoxColumn1,
            this.switchNameDataGridViewTextBoxColumn,
            this.totalAfterCallDataGridViewTextBoxColumn,
            this.totalAuxOtherDataGridViewTextBoxColumn});
            this.dataGridView2.DataSource = this.bCMSSkillBindingSource;
            this.dataGridView2.Location = new System.Drawing.Point(35, 331);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(598, 150);
            this.dataGridView2.TabIndex = 9;
            // 
            // bCMSSkillBindingSource
            // 
            this.bCMSSkillBindingSource.DataSource = typeof(CMDataCollector.BcmsHistoricalDashboard.BCMSSkill);
            // 
            // abandonedCallsDataGridViewTextBoxColumn
            // 
            this.abandonedCallsDataGridViewTextBoxColumn.DataPropertyName = "Abandoned_Calls";
            this.abandonedCallsDataGridViewTextBoxColumn.HeaderText = "Abandoned_Calls";
            this.abandonedCallsDataGridViewTextBoxColumn.Name = "abandonedCallsDataGridViewTextBoxColumn";
            // 
            // acceptableServiceLevelDataGridViewTextBoxColumn
            // 
            this.acceptableServiceLevelDataGridViewTextBoxColumn.DataPropertyName = "Acceptable_Service_Level";
            this.acceptableServiceLevelDataGridViewTextBoxColumn.HeaderText = "Acceptable_Service_Level";
            this.acceptableServiceLevelDataGridViewTextBoxColumn.Name = "acceptableServiceLevelDataGridViewTextBoxColumn";
            // 
            // acdCallsDataGridViewTextBoxColumn
            // 
            this.acdCallsDataGridViewTextBoxColumn.DataPropertyName = "Acd_Calls";
            this.acdCallsDataGridViewTextBoxColumn.HeaderText = "Acd_Calls";
            this.acdCallsDataGridViewTextBoxColumn.Name = "acdCallsDataGridViewTextBoxColumn";
            // 
            // avgAbandonedTimeDataGridViewTextBoxColumn
            // 
            this.avgAbandonedTimeDataGridViewTextBoxColumn.DataPropertyName = "Avg_Abandoned_Time";
            this.avgAbandonedTimeDataGridViewTextBoxColumn.HeaderText = "Avg_Abandoned_Time";
            this.avgAbandonedTimeDataGridViewTextBoxColumn.Name = "avgAbandonedTimeDataGridViewTextBoxColumn";
            // 
            // avgSpeedAnsweredDataGridViewTextBoxColumn
            // 
            this.avgSpeedAnsweredDataGridViewTextBoxColumn.DataPropertyName = "Avg_Speed_Answered";
            this.avgSpeedAnsweredDataGridViewTextBoxColumn.HeaderText = "Avg_Speed_Answered";
            this.avgSpeedAnsweredDataGridViewTextBoxColumn.Name = "avgSpeedAnsweredDataGridViewTextBoxColumn";
            // 
            // avgStaffDataGridViewTextBoxColumn
            // 
            this.avgStaffDataGridViewTextBoxColumn.DataPropertyName = "Avg_Staff";
            this.avgStaffDataGridViewTextBoxColumn.HeaderText = "Avg_Staff";
            this.avgStaffDataGridViewTextBoxColumn.Name = "avgStaffDataGridViewTextBoxColumn";
            // 
            // avgTalkTimeDataGridViewTextBoxColumn
            // 
            this.avgTalkTimeDataGridViewTextBoxColumn.DataPropertyName = "Avg_Talk_Time";
            this.avgTalkTimeDataGridViewTextBoxColumn.HeaderText = "Avg_Talk_Time";
            this.avgTalkTimeDataGridViewTextBoxColumn.Name = "avgTalkTimeDataGridViewTextBoxColumn";
            // 
            // dateDataGridViewTextBoxColumn1
            // 
            this.dateDataGridViewTextBoxColumn1.DataPropertyName = "Date";
            this.dateDataGridViewTextBoxColumn1.HeaderText = "Date";
            this.dateDataGridViewTextBoxColumn1.Name = "dateDataGridViewTextBoxColumn1";
            // 
            // flowInDataGridViewTextBoxColumn
            // 
            this.flowInDataGridViewTextBoxColumn.DataPropertyName = "Flow_In";
            this.flowInDataGridViewTextBoxColumn.HeaderText = "Flow_In";
            this.flowInDataGridViewTextBoxColumn.Name = "flowInDataGridViewTextBoxColumn";
            // 
            // flowOutDataGridViewTextBoxColumn
            // 
            this.flowOutDataGridViewTextBoxColumn.DataPropertyName = "Flow_Out";
            this.flowOutDataGridViewTextBoxColumn.HeaderText = "Flow_Out";
            this.flowOutDataGridViewTextBoxColumn.Name = "flowOutDataGridViewTextBoxColumn";
            // 
            // intervalDataGridViewTextBoxColumn
            // 
            this.intervalDataGridViewTextBoxColumn.DataPropertyName = "Interval";
            this.intervalDataGridViewTextBoxColumn.HeaderText = "Interval";
            this.intervalDataGridViewTextBoxColumn.Name = "intervalDataGridViewTextBoxColumn";
            // 
            // pctInSvcLevelDataGridViewTextBoxColumn
            // 
            this.pctInSvcLevelDataGridViewTextBoxColumn.DataPropertyName = "Pct_In_Svc_Level";
            this.pctInSvcLevelDataGridViewTextBoxColumn.HeaderText = "Pct_In_Svc_Level";
            this.pctInSvcLevelDataGridViewTextBoxColumn.Name = "pctInSvcLevelDataGridViewTextBoxColumn";
            // 
            // reportTypeDataGridViewTextBoxColumn
            // 
            this.reportTypeDataGridViewTextBoxColumn.DataPropertyName = "Report_Type";
            this.reportTypeDataGridViewTextBoxColumn.HeaderText = "Report_Type";
            this.reportTypeDataGridViewTextBoxColumn.Name = "reportTypeDataGridViewTextBoxColumn";
            // 
            // skillDataGridViewTextBoxColumn1
            // 
            this.skillDataGridViewTextBoxColumn1.DataPropertyName = "Skill";
            this.skillDataGridViewTextBoxColumn1.HeaderText = "Skill";
            this.skillDataGridViewTextBoxColumn1.Name = "skillDataGridViewTextBoxColumn1";
            // 
            // skillNameDataGridViewTextBoxColumn1
            // 
            this.skillNameDataGridViewTextBoxColumn1.DataPropertyName = "Skill_Name";
            this.skillNameDataGridViewTextBoxColumn1.HeaderText = "Skill_Name";
            this.skillNameDataGridViewTextBoxColumn1.Name = "skillNameDataGridViewTextBoxColumn1";
            // 
            // switchNameDataGridViewTextBoxColumn
            // 
            this.switchNameDataGridViewTextBoxColumn.DataPropertyName = "Switch_Name";
            this.switchNameDataGridViewTextBoxColumn.HeaderText = "Switch_Name";
            this.switchNameDataGridViewTextBoxColumn.Name = "switchNameDataGridViewTextBoxColumn";
            // 
            // totalAfterCallDataGridViewTextBoxColumn
            // 
            this.totalAfterCallDataGridViewTextBoxColumn.DataPropertyName = "Total_After_Call";
            this.totalAfterCallDataGridViewTextBoxColumn.HeaderText = "Total_After_Call";
            this.totalAfterCallDataGridViewTextBoxColumn.Name = "totalAfterCallDataGridViewTextBoxColumn";
            // 
            // totalAuxOtherDataGridViewTextBoxColumn
            // 
            this.totalAuxOtherDataGridViewTextBoxColumn.DataPropertyName = "Total_Aux_Other";
            this.totalAuxOtherDataGridViewTextBoxColumn.HeaderText = "Total_Aux_Other";
            this.totalAuxOtherDataGridViewTextBoxColumn.Name = "totalAuxOtherDataGridViewTextBoxColumn";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 498);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.btnGetHistoricalData);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bcmsDashboardBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bCMSSkillBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn callsWaitingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accptedSLDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn oldestCallDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sLDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn staffDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn availDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn aCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn aCWDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn aUXDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn extnDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn otherDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn abandCallsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgAbandTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acdCallsSummaryDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource bcmsDashboardBindingSource;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btnGetHistoricalData;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn abandonedCallsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acceptableServiceLevelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acdCallsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgAbandonedTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgSpeedAnsweredDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgStaffDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgTalkTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn flowInDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn flowOutDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intervalDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pctInSvcLevelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reportTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillNameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalAfterCallDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalAuxOtherDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource bCMSSkillBindingSource;
    }
}

