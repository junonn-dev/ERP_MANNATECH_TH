﻿namespace MLM_Program
{
    partial class frm_CSV_Import_Member
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dGridView_Base = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.butt_Search = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.butt_Excel = new System.Windows.Forms.Button();
            this.butt_Delete = new System.Windows.Forms.Button();
            this.butt_Clear = new System.Windows.Forms.Button();
            this.butt_Select = new System.Windows.Forms.Button();
            this.butt_Exit = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel17 = new System.Windows.Forms.TableLayoutPanel();
            this.panel21 = new System.Windows.Forms.Panel();
            this.mtxtSellDate2 = new System.Windows.Forms.MaskedTextBox();
            this.mtxtSellDate1 = new System.Windows.Forms.MaskedTextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.DTP_SellDate1 = new System.Windows.Forms.DateTimePicker();
            this.DTP_SellDate2 = new System.Windows.Forms.DateTimePicker();
            this.label42 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.mtxtitemcount = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_promotion_2 = new System.Windows.Forms.TextBox();
            this.mtxtMbid_2 = new System.Windows.Forms.MaskedTextBox();
            this.tableLayoutPanel62 = new System.Windows.Forms.TableLayoutPanel();
            this.panel25 = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.butt_Save = new System.Windows.Forms.Button();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.txt_promotion = new System.Windows.Forms.TextBox();
            this.txt_ItemName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel61 = new System.Windows.Forms.TableLayoutPanel();
            this.panel24 = new System.Windows.Forms.Panel();
            this.mtxtMbid = new System.Windows.Forms.MaskedTextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.butt_Order_End = new System.Windows.Forms.Button();
            this.butt_S_Not_check = new System.Windows.Forms.Button();
            this.butt_S_check = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.combo_Sheet = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel38 = new System.Windows.Forms.TableLayoutPanel();
            this.label40 = new System.Windows.Forms.Label();
            this.panel23 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dGridView_Base)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tableLayoutPanel17.SuspendLayout();
            this.panel21.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tableLayoutPanel62.SuspendLayout();
            this.panel25.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.panel10.SuspendLayout();
            this.tableLayoutPanel61.SuspendLayout();
            this.panel24.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel38.SuspendLayout();
            this.panel23.SuspendLayout();
            this.SuspendLayout();
            // 
            // dGridView_Base
            // 
            this.dGridView_Base.BackgroundColor = System.Drawing.Color.White;
            this.dGridView_Base.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("돋움", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dGridView_Base.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dGridView_Base.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("돋움", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dGridView_Base.DefaultCellStyle = dataGridViewCellStyle4;
            this.dGridView_Base.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGridView_Base.GridColor = System.Drawing.SystemColors.ButtonFace;
            this.dGridView_Base.Location = new System.Drawing.Point(0, 148);
            this.dGridView_Base.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.dGridView_Base.Name = "dGridView_Base";
            this.dGridView_Base.RowTemplate.Height = 23;
            this.dGridView_Base.Size = new System.Drawing.Size(1312, 634);
            this.dGridView_Base.TabIndex = 8;
            this.dGridView_Base.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGridView_Base_CellClick);
            this.dGridView_Base.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dGridView_Base_CellPainting);
            this.dGridView_Base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dGridView_KeyDown);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtFilePath
            // 
            this.txtFilePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(241)))), ((int)(((byte)(220)))));
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilePath.Font = new System.Drawing.Font("돋움", 9.75F);
            this.txtFilePath.Location = new System.Drawing.Point(3, 3);
            this.txtFilePath.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtFilePath.MaxLength = 30;
            this.txtFilePath.Multiline = true;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(652, 22);
            this.txtFilePath.TabIndex = 9;
            // 
            // butt_Search
            // 
            this.butt_Search.BackColor = System.Drawing.Color.White;
            this.butt_Search.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Search.Location = new System.Drawing.Point(3, 3);
            this.butt_Search.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.butt_Search.Name = "butt_Search";
            this.butt_Search.Size = new System.Drawing.Size(212, 35);
            this.butt_Search.TabIndex = 0;
            this.butt_Search.Text = "적용_파일_찾기";
            this.butt_Search.UseVisualStyleBackColor = false;
            this.butt_Search.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.butt_Excel);
            this.panel1.Controls.Add(this.butt_Delete);
            this.panel1.Controls.Add(this.butt_Clear);
            this.panel1.Controls.Add(this.butt_Select);
            this.panel1.Controls.Add(this.butt_Exit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1312, 28);
            this.panel1.TabIndex = 197;
            // 
            // butt_Excel
            // 
            this.butt_Excel.BackColor = System.Drawing.Color.White;
            this.butt_Excel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Excel.Location = new System.Drawing.Point(538, 1);
            this.butt_Excel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.butt_Excel.Name = "butt_Excel";
            this.butt_Excel.Size = new System.Drawing.Size(111, 26);
            this.butt_Excel.TabIndex = 9;
            this.butt_Excel.TabStop = false;
            this.butt_Excel.Text = "엑셀";
            this.butt_Excel.UseVisualStyleBackColor = false;
            this.butt_Excel.Visible = false;
            this.butt_Excel.Click += new System.EventHandler(this.butt_Excel_Click);
            // 
            // butt_Delete
            // 
            this.butt_Delete.BackColor = System.Drawing.Color.White;
            this.butt_Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Delete.Location = new System.Drawing.Point(135, 1);
            this.butt_Delete.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.butt_Delete.Name = "butt_Delete";
            this.butt_Delete.Size = new System.Drawing.Size(111, 26);
            this.butt_Delete.TabIndex = 8;
            this.butt_Delete.TabStop = false;
            this.butt_Delete.Text = "삭제";
            this.butt_Delete.UseVisualStyleBackColor = false;
            this.butt_Delete.Visible = false;
            // 
            // butt_Clear
            // 
            this.butt_Clear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(89)))), ((int)(((byte)(97)))));
            this.butt_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Clear.ForeColor = System.Drawing.Color.White;
            this.butt_Clear.Location = new System.Drawing.Point(276, 1);
            this.butt_Clear.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.butt_Clear.Name = "butt_Clear";
            this.butt_Clear.Size = new System.Drawing.Size(111, 26);
            this.butt_Clear.TabIndex = 7;
            this.butt_Clear.TabStop = false;
            this.butt_Clear.Text = "새로입력(F1)";
            this.butt_Clear.UseVisualStyleBackColor = false;
            this.butt_Clear.Click += new System.EventHandler(this.Base_Button_Click);
            // 
            // butt_Select
            // 
            this.butt_Select.BackColor = System.Drawing.Color.White;
            this.butt_Select.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Select.Location = new System.Drawing.Point(421, 1);
            this.butt_Select.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.butt_Select.Name = "butt_Select";
            this.butt_Select.Size = new System.Drawing.Size(111, 26);
            this.butt_Select.TabIndex = 6;
            this.butt_Select.Text = "엑셀 내용 저장";
            this.butt_Select.UseVisualStyleBackColor = false;
            this.butt_Select.Click += new System.EventHandler(this.butt_Exit_Click);
            // 
            // butt_Exit
            // 
            this.butt_Exit.BackColor = System.Drawing.Color.White;
            this.butt_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Exit.Location = new System.Drawing.Point(668, 1);
            this.butt_Exit.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.butt_Exit.Name = "butt_Exit";
            this.butt_Exit.Size = new System.Drawing.Size(111, 26);
            this.butt_Exit.TabIndex = 5;
            this.butt_Exit.TabStop = false;
            this.butt_Exit.Text = "닫기";
            this.butt_Exit.UseVisualStyleBackColor = false;
            this.butt_Exit.Click += new System.EventHandler(this.butt_Exit_Click);
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(221, 40);
            this.progress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(406, 16);
            this.progress.TabIndex = 181;
            this.progress.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel3);
            this.panel2.Controls.Add(this.tableLayoutPanel17);
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Controls.Add(this.txt_promotion_2);
            this.panel2.Controls.Add(this.mtxtMbid_2);
            this.panel2.Controls.Add(this.tableLayoutPanel62);
            this.panel2.Controls.Add(this.butt_Save);
            this.panel2.Controls.Add(this.tableLayoutPanel8);
            this.panel2.Controls.Add(this.tableLayoutPanel61);
            this.panel2.Controls.Add(this.butt_Order_End);
            this.panel2.Controls.Add(this.butt_S_Not_check);
            this.panel2.Controls.Add(this.butt_S_check);
            this.panel2.Controls.Add(this.tableLayoutPanel2);
            this.panel2.Controls.Add(this.progress);
            this.panel2.Controls.Add(this.tableLayoutPanel38);
            this.panel2.Controls.Add(this.butt_Search);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1312, 120);
            this.panel2.TabIndex = 199;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.panel5, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(747, 84);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(300, 36);
            this.tableLayoutPanel3.TabIndex = 251;
            this.tableLayoutPanel3.Visible = false;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.maskedTextBox1);
            this.panel5.Location = new System.Drawing.Point(126, 4);
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(170, 28);
            this.panel5.TabIndex = 15;
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maskedTextBox1.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.maskedTextBox1.Location = new System.Drawing.Point(3, 3);
            this.maskedTextBox1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(164, 21);
            this.maskedTextBox1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(2, 2);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "회원_번호";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel17
            // 
            this.tableLayoutPanel17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel17.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel17.ColumnCount = 2;
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Controls.Add(this.panel21, 1, 0);
            this.tableLayoutPanel17.Controls.Add(this.label42, 0, 0);
            this.tableLayoutPanel17.Location = new System.Drawing.Point(1041, 84);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            this.tableLayoutPanel17.RowCount = 1;
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel17.Size = new System.Drawing.Size(325, 36);
            this.tableLayoutPanel17.TabIndex = 250;
            this.tableLayoutPanel17.Visible = false;
            // 
            // panel21
            // 
            this.panel21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel21.BackColor = System.Drawing.Color.White;
            this.panel21.Controls.Add(this.mtxtSellDate2);
            this.panel21.Controls.Add(this.mtxtSellDate1);
            this.panel21.Controls.Add(this.label41);
            this.panel21.Controls.Add(this.DTP_SellDate1);
            this.panel21.Controls.Add(this.DTP_SellDate2);
            this.panel21.Location = new System.Drawing.Point(126, 4);
            this.panel21.Margin = new System.Windows.Forms.Padding(2);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(195, 28);
            this.panel21.TabIndex = 15;
            // 
            // mtxtSellDate2
            // 
            this.mtxtSellDate2.Location = new System.Drawing.Point(104, 3);
            this.mtxtSellDate2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.mtxtSellDate2.Name = "mtxtSellDate2";
            this.mtxtSellDate2.Size = new System.Drawing.Size(67, 21);
            this.mtxtSellDate2.TabIndex = 1;
            this.mtxtSellDate2.Enter += new System.EventHandler(this.txtData_Enter);
            this.mtxtSellDate2.Leave += new System.EventHandler(this.txtData_Base_Leave);
            // 
            // mtxtSellDate1
            // 
            this.mtxtSellDate1.Location = new System.Drawing.Point(3, 3);
            this.mtxtSellDate1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.mtxtSellDate1.Name = "mtxtSellDate1";
            this.mtxtSellDate1.Size = new System.Drawing.Size(67, 21);
            this.mtxtSellDate1.TabIndex = 0;
            this.mtxtSellDate1.Enter += new System.EventHandler(this.txtData_Enter);
            this.mtxtSellDate1.Leave += new System.EventHandler(this.txtData_Base_Leave);
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(94, 7);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(9, 15);
            this.label41.TabIndex = 60;
            this.label41.Text = "~";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DTP_SellDate1
            // 
            this.DTP_SellDate1.Location = new System.Drawing.Point(68, 3);
            this.DTP_SellDate1.Name = "DTP_SellDate1";
            this.DTP_SellDate1.Size = new System.Drawing.Size(21, 21);
            this.DTP_SellDate1.TabIndex = 1;
            this.DTP_SellDate1.TabStop = false;
            this.DTP_SellDate1.CloseUp += new System.EventHandler(this.DTP_Base_CloseUp);
            // 
            // DTP_SellDate2
            // 
            this.DTP_SellDate2.Location = new System.Drawing.Point(169, 3);
            this.DTP_SellDate2.Name = "DTP_SellDate2";
            this.DTP_SellDate2.Size = new System.Drawing.Size(21, 21);
            this.DTP_SellDate2.TabIndex = 3;
            this.DTP_SellDate2.TabStop = false;
            this.DTP_SellDate2.CloseUp += new System.EventHandler(this.DTP_Base_CloseUp);
            // 
            // label42
            // 
            this.label42.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label42.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label42.ForeColor = System.Drawing.Color.White;
            this.label42.Location = new System.Drawing.Point(2, 2);
            this.label42.Margin = new System.Windows.Forms.Padding(0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(120, 32);
            this.label42.TabIndex = 0;
            this.label42.Text = "시작일자-종료일자";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1038, 44);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 36);
            this.tableLayoutPanel1.TabIndex = 249;
            this.tableLayoutPanel1.Visible = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.mtxtitemcount);
            this.panel4.Location = new System.Drawing.Point(126, 4);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(170, 28);
            this.panel4.TabIndex = 15;
            // 
            // mtxtitemcount
            // 
            this.mtxtitemcount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtxtitemcount.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.mtxtitemcount.Location = new System.Drawing.Point(3, 3);
            this.mtxtitemcount.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.mtxtitemcount.Name = "mtxtitemcount";
            this.mtxtitemcount.Size = new System.Drawing.Size(164, 21);
            this.mtxtitemcount.TabIndex = 0;
            this.mtxtitemcount.Enter += new System.EventHandler(this.txtData_Enter);
            this.mtxtitemcount.Leave += new System.EventHandler(this.txtData_Base_Leave);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(2, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "수량";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_promotion_2
            // 
            this.txt_promotion_2.Font = new System.Drawing.Font("돋움", 9.75F);
            this.txt_promotion_2.Location = new System.Drawing.Point(569, 84);
            this.txt_promotion_2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txt_promotion_2.MaxLength = 8;
            this.txt_promotion_2.Name = "txt_promotion_2";
            this.txt_promotion_2.Size = new System.Drawing.Size(58, 22);
            this.txt_promotion_2.TabIndex = 248;
            this.txt_promotion_2.Tag = "ncode";
            this.txt_promotion_2.Visible = false;
            // 
            // mtxtMbid_2
            // 
            this.mtxtMbid_2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtxtMbid_2.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.mtxtMbid_2.Location = new System.Drawing.Point(394, 85);
            this.mtxtMbid_2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.mtxtMbid_2.Name = "mtxtMbid_2";
            this.mtxtMbid_2.Size = new System.Drawing.Size(164, 21);
            this.mtxtMbid_2.TabIndex = 247;
            this.mtxtMbid_2.Visible = false;
            // 
            // tableLayoutPanel62
            // 
            this.tableLayoutPanel62.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel62.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel62.ColumnCount = 2;
            this.tableLayoutPanel62.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel62.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel62.Controls.Add(this.panel25, 1, 0);
            this.tableLayoutPanel62.Controls.Add(this.label65, 0, 0);
            this.tableLayoutPanel62.Location = new System.Drawing.Point(304, 47);
            this.tableLayoutPanel62.Name = "tableLayoutPanel62";
            this.tableLayoutPanel62.RowCount = 1;
            this.tableLayoutPanel62.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel62.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel62.Size = new System.Drawing.Size(300, 36);
            this.tableLayoutPanel62.TabIndex = 2;
            // 
            // panel25
            // 
            this.panel25.BackColor = System.Drawing.Color.White;
            this.panel25.Controls.Add(this.txtName);
            this.panel25.Location = new System.Drawing.Point(126, 4);
            this.panel25.Margin = new System.Windows.Forms.Padding(2);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(170, 28);
            this.panel25.TabIndex = 15;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("돋움", 9.75F);
            this.txtName.Location = new System.Drawing.Point(3, 3);
            this.txtName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtName.MaxLength = 30;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(164, 22);
            this.txtName.TabIndex = 1;
            this.txtName.TabStop = false;
            this.txtName.Tag = "name";
            this.txtName.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            this.txtName.Enter += new System.EventHandler(this.txtData_Enter);
            this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtData_KeyPress);
            this.txtName.Leave += new System.EventHandler(this.txtData_Base_Leave);
            // 
            // label65
            // 
            this.label65.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label65.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label65.ForeColor = System.Drawing.Color.White;
            this.label65.Location = new System.Drawing.Point(2, 2);
            this.label65.Margin = new System.Windows.Forms.Padding(0);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(120, 32);
            this.label65.TabIndex = 0;
            this.label65.Text = "성명";
            this.label65.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butt_Save
            // 
            this.butt_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.butt_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Save.ForeColor = System.Drawing.Color.White;
            this.butt_Save.Location = new System.Drawing.Point(630, 88);
            this.butt_Save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butt_Save.Name = "butt_Save";
            this.butt_Save.Size = new System.Drawing.Size(111, 26);
            this.butt_Save.TabIndex = 246;
            this.butt_Save.Text = "저장";
            this.butt_Save.UseVisualStyleBackColor = false;
            this.butt_Save.Click += new System.EventHandler(this.butt_Save_Click);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel8.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 161F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.panel10, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(596, 47);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(414, 36);
            this.tableLayoutPanel8.TabIndex = 3;
            // 
            // panel10
            // 
            this.panel10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel10.BackColor = System.Drawing.Color.White;
            this.panel10.Controls.Add(this.txt_promotion);
            this.panel10.Controls.Add(this.txt_ItemName);
            this.panel10.Location = new System.Drawing.Point(167, 4);
            this.panel10.Margin = new System.Windows.Forms.Padding(2);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(243, 28);
            this.panel10.TabIndex = 15;
            // 
            // txt_promotion
            // 
            this.txt_promotion.Font = new System.Drawing.Font("돋움", 9.75F);
            this.txt_promotion.Location = new System.Drawing.Point(3, 3);
            this.txt_promotion.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txt_promotion.MaxLength = 8;
            this.txt_promotion.Name = "txt_promotion";
            this.txt_promotion.Size = new System.Drawing.Size(58, 22);
            this.txt_promotion.TabIndex = 1;
            this.txt_promotion.Tag = "ncode";
            this.txt_promotion.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            this.txt_promotion.Enter += new System.EventHandler(this.txtData_Enter);
            this.txt_promotion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtData_KeyPress);
            this.txt_promotion.Leave += new System.EventHandler(this.txtData_Base_Leave);
            // 
            // txt_ItemName
            // 
            this.txt_ItemName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.txt_ItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ItemName.Font = new System.Drawing.Font("돋움", 9F);
            this.txt_ItemName.ForeColor = System.Drawing.Color.White;
            this.txt_ItemName.Location = new System.Drawing.Point(61, 3);
            this.txt_ItemName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txt_ItemName.MaxLength = 30;
            this.txt_ItemName.Name = "txt_ItemName";
            this.txt_ItemName.ReadOnly = true;
            this.txt_ItemName.Size = new System.Drawing.Size(220, 21);
            this.txt_ItemName.TabIndex = 1;
            this.txt_ItemName.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "프로모션코드/프로모션명";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel61
            // 
            this.tableLayoutPanel61.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel61.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel61.ColumnCount = 2;
            this.tableLayoutPanel61.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel61.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel61.Controls.Add(this.panel24, 1, 0);
            this.tableLayoutPanel61.Controls.Add(this.label64, 0, 0);
            this.tableLayoutPanel61.Location = new System.Drawing.Point(3, 47);
            this.tableLayoutPanel61.Name = "tableLayoutPanel61";
            this.tableLayoutPanel61.RowCount = 1;
            this.tableLayoutPanel61.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel61.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel61.Size = new System.Drawing.Size(300, 36);
            this.tableLayoutPanel61.TabIndex = 1;
            // 
            // panel24
            // 
            this.panel24.BackColor = System.Drawing.Color.White;
            this.panel24.Controls.Add(this.mtxtMbid);
            this.panel24.Location = new System.Drawing.Point(126, 4);
            this.panel24.Margin = new System.Windows.Forms.Padding(2);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(170, 28);
            this.panel24.TabIndex = 15;
            // 
            // mtxtMbid
            // 
            this.mtxtMbid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtxtMbid.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.mtxtMbid.Location = new System.Drawing.Point(3, 3);
            this.mtxtMbid.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.mtxtMbid.Mask = "9999999";
            this.mtxtMbid.Name = "mtxtMbid";
            this.mtxtMbid.Size = new System.Drawing.Size(164, 21);
            this.mtxtMbid.TabIndex = 0;
            this.mtxtMbid.ValidatingType = typeof(int);
            this.mtxtMbid.TextChanged += new System.EventHandler(this.mtxtMbid_TextChanged);
            this.mtxtMbid.Enter += new System.EventHandler(this.txtData_Enter);
            this.mtxtMbid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MtxtData_KeyPress);
            this.mtxtMbid.Leave += new System.EventHandler(this.txtData_Base_Leave);
            // 
            // label64
            // 
            this.label64.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label64.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label64.ForeColor = System.Drawing.Color.White;
            this.label64.Location = new System.Drawing.Point(2, 2);
            this.label64.Margin = new System.Windows.Forms.Padding(0);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(120, 32);
            this.label64.TabIndex = 0;
            this.label64.Text = "회원_번호";
            this.label64.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butt_Order_End
            // 
            this.butt_Order_End.BackColor = System.Drawing.Color.White;
            this.butt_Order_End.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_Order_End.Location = new System.Drawing.Point(237, 85);
            this.butt_Order_End.Name = "butt_Order_End";
            this.butt_Order_End.Size = new System.Drawing.Size(111, 32);
            this.butt_Order_End.TabIndex = 240;
            this.butt_Order_End.TabStop = false;
            this.butt_Order_End.Text = "삭제";
            this.butt_Order_End.UseVisualStyleBackColor = false;
            this.butt_Order_End.Click += new System.EventHandler(this.butt_Order_End_Click);
            // 
            // butt_S_Not_check
            // 
            this.butt_S_Not_check.BackColor = System.Drawing.Color.White;
            this.butt_S_Not_check.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_S_Not_check.Location = new System.Drawing.Point(120, 85);
            this.butt_S_Not_check.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butt_S_Not_check.Name = "butt_S_Not_check";
            this.butt_S_Not_check.Size = new System.Drawing.Size(111, 32);
            this.butt_S_Not_check.TabIndex = 237;
            this.butt_S_Not_check.TabStop = false;
            this.butt_S_Not_check.Text = "전체_취소";
            this.butt_S_Not_check.UseVisualStyleBackColor = false;
            this.butt_S_Not_check.Click += new System.EventHandler(this.Base_Button_Click);
            // 
            // butt_S_check
            // 
            this.butt_S_check.BackColor = System.Drawing.Color.White;
            this.butt_S_check.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butt_S_check.Location = new System.Drawing.Point(3, 85);
            this.butt_S_check.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butt_S_check.Name = "butt_S_check";
            this.butt_S_check.Size = new System.Drawing.Size(111, 32);
            this.butt_S_check.TabIndex = 236;
            this.butt_S_check.TabStop = false;
            this.butt_S_check.Text = "전체_선택";
            this.butt_S_check.UseVisualStyleBackColor = false;
            this.butt_S_check.Click += new System.EventHandler(this.Base_Button_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1006, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(381, 36);
            this.tableLayoutPanel2.TabIndex = 187;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.combo_Sheet);
            this.panel3.Location = new System.Drawing.Point(126, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(251, 28);
            this.panel3.TabIndex = 15;
            // 
            // combo_Sheet
            // 
            this.combo_Sheet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.combo_Sheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Sheet.FormattingEnabled = true;
            this.combo_Sheet.Location = new System.Drawing.Point(3, 4);
            this.combo_Sheet.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.combo_Sheet.Name = "combo_Sheet";
            this.combo_Sheet.Size = new System.Drawing.Size(245, 20);
            this.combo_Sheet.TabIndex = 194;
            this.combo_Sheet.SelectedIndexChanged += new System.EventHandler(this.combo_Pay_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(2, 2);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 32);
            this.label7.TabIndex = 0;
            this.label7.Text = "적용_Sheet";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel38
            // 
            this.tableLayoutPanel38.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.tableLayoutPanel38.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel38.ColumnCount = 2;
            this.tableLayoutPanel38.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel38.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel38.Controls.Add(this.label40, 0, 0);
            this.tableLayoutPanel38.Controls.Add(this.panel23, 1, 0);
            this.tableLayoutPanel38.Location = new System.Drawing.Point(217, 2);
            this.tableLayoutPanel38.Name = "tableLayoutPanel38";
            this.tableLayoutPanel38.RowCount = 1;
            this.tableLayoutPanel38.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel38.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel38.Size = new System.Drawing.Size(788, 37);
            this.tableLayoutPanel38.TabIndex = 185;
            // 
            // label40
            // 
            this.label40.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label40.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(144)))), ((int)(((byte)(176)))));
            this.label40.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label40.ForeColor = System.Drawing.Color.White;
            this.label40.Location = new System.Drawing.Point(2, 2);
            this.label40.Margin = new System.Windows.Forms.Padding(0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(120, 33);
            this.label40.TabIndex = 0;
            this.label40.Text = "적용_파일_경로";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel23
            // 
            this.panel23.BackColor = System.Drawing.Color.White;
            this.panel23.Controls.Add(this.txtFilePath);
            this.panel23.Location = new System.Drawing.Point(126, 4);
            this.panel23.Margin = new System.Windows.Forms.Padding(2);
            this.panel23.Name = "panel23";
            this.panel23.Size = new System.Drawing.Size(658, 28);
            this.panel23.TabIndex = 15;
            // 
            // frm_CSV_Import_Member
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(1137, 782);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1329, 761);
            this.Controls.Add(this.dGridView_Base);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_CSV_Import_Member";
            this.Text = "프로모션 회원정보 엑셀 업로드";
            this.Activated += new System.EventHandler(this.frm_Base_Activated);
            this.Load += new System.EventHandler(this.frm_CSV_Import_Member_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBase_From_KeyDown);
            this.Resize += new System.EventHandler(this.frmBase_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dGridView_Base)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.tableLayoutPanel17.ResumeLayout(false);
            this.panel21.ResumeLayout(false);
            this.panel21.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tableLayoutPanel62.ResumeLayout(false);
            this.panel25.ResumeLayout(false);
            this.panel25.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.tableLayoutPanel61.ResumeLayout(false);
            this.panel24.ResumeLayout(false);
            this.panel24.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel38.ResumeLayout(false);
            this.panel23.ResumeLayout(false);
            this.panel23.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dGridView_Base;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button butt_Search;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button butt_Excel;
        private System.Windows.Forms.Button butt_Delete;
        private System.Windows.Forms.Button butt_Clear;
        private System.Windows.Forms.Button butt_Select;
        private System.Windows.Forms.Button butt_Exit;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel38;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Panel panel23;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox combo_Sheet;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button butt_Order_End;
        private System.Windows.Forms.Button butt_S_Not_check;
        private System.Windows.Forms.Button butt_S_check;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel61;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.MaskedTextBox mtxtMbid;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.TextBox txt_promotion;
        private System.Windows.Forms.TextBox txt_ItemName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button butt_Save;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel62;
        private System.Windows.Forms.Panel panel25;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.TextBox txt_promotion_2;
        private System.Windows.Forms.MaskedTextBox mtxtMbid_2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.MaskedTextBox mtxtitemcount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel17;
        private System.Windows.Forms.Panel panel21;
        private System.Windows.Forms.MaskedTextBox mtxtSellDate2;
        private System.Windows.Forms.MaskedTextBox mtxtSellDate1;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.DateTimePicker DTP_SellDate1;
        private System.Windows.Forms.DateTimePicker DTP_SellDate2;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.Label label3;
    }
}