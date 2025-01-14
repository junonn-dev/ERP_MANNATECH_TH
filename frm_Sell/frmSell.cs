﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Drawing.Printing;
using System.Reflection;


namespace MLM_Program
{
    public partial class frmSell : Form
    {
        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        public delegate void Take_NumberDele(ref string Send_Number, ref string Send_Name, ref string Send_OrderNumber);
        public event Take_NumberDele Take_Mem_Number;

        private string Temp_Ordernumber = "";
        private string New_Ordernumber = "";

        private string MP_YN = "";
        string couponnumber = "";
        string Sell_Mem_TF = "";
        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Cacu = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece_Add = new cls_Grid_Base();
        cls_Grid_Base cgb_Mile = new cls_Grid_Base();
        cls_Grid_Base cgb_Coupon = new cls_Grid_Base();

        private Dictionary<string, cls_Sell> SalesDetail = new Dictionary<string, cls_Sell>();
        private Dictionary<int, cls_Sell_Item> SalesItemDetail = new Dictionary<int, cls_Sell_Item>();
        private Dictionary<int, cls_Sell_Rece> Sales_Rece = new Dictionary<int, cls_Sell_Rece>();
        private Dictionary<int, cls_Sell_Cacu> Sales_Cacu = new Dictionary<int, cls_Sell_Cacu>();

        private Dictionary<string, TextBox> Ncode_dic = new Dictionary<string, TextBox>();

        private const string base_db_name = "tbl_SalesDetail";
        private int Data_Set_Form_TF;
        private string idx_Mbid = "";
        private int idx_Mbid2 = 0;
        private string idx_Na_Code = "";

        private int Form_Key_Real_TF = 1;

        private int Save_Button_Click_Cnt = 0;
        private int print_Page = 0;

        private string InsuranceNumber_Ord_Print_FLAG = "";
        //PV_CV 비져블
        private int PV_CV_Check = 0;
        private Boolean Card_Ok_Visible = true;

        Series series_Item = new Series();

        public frmSell()
        {
            InitializeComponent();

            typeof(GroupBox).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, groupBox3, new object[] { true });

            typeof(Form).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
          | BindingFlags.Instance | BindingFlags.SetProperty, null, this, new object[] { true });

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, panel8, new object[] { true });


        }

        private void frmBase_From_Load(object sender, EventArgs e)
        {
            
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            if (Environment.MachineName.Equals("LANCE1"))
                pnlCashReceipt.Visible = true;

            Data_Set_Form_TF = 0;
            Save_Button_Click_Cnt = 0;
            InsuranceNumber_Ord_Print_FLAG = "";

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset(1);

            dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Cacu.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtSn.Mask = "999999-9999999"; //기본 셋팅은 주민번호이다. 

            string[] data_Y = { ""
                              , int.Parse (cls_User.gid_date_time.Substring (0,4)).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 1 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 2 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 3 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 4 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 5 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 6 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 7 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 8 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 9 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 10 ).ToString()
                              };

            string[] data_M = { "","01", "02", "03", "04", "05"
                               , "06", "07", "08", "09", "10"
                               , "11", "12"
                              };

            string[] data_P = { ""
                               , cm._chang_base_caption_search("일시불")
                               //,"1" 1개월의 할부는 없어서 주석처리함 
                               , "2", "3", "4", "5"
                               , "6", "7", "8", "9", "10"
                               , "11", "12","13","14","15","16","17","18","19","20","21","22","23", "24"
                              };

            // 각 콤보박스에 데이타를 초기화
            combo_C_Card_Year.Items.AddRange(data_Y);
            combo_C_Card_Month.Items.AddRange(data_M);
            combo_C_Card_Per.Items.AddRange(data_P);

            combo_C_Card_Year.SelectedIndex = 0;
            combo_C_Card_Month.SelectedIndex = 0;
            combo_C_Card_Per.SelectedIndex = 0;

            //Reset_Chart_Total(); // 차트 관련해서 리셋을 시킨다.

            //상품코드 자리수에 맞추어 텍스트 박스 길이 셋팅
            if (cls_app_static_var.Item_Sort_1_Code_Length == 0)
                txt_ItemCode.MaxLength = cls_app_static_var.Item_Code_Length;

            else
            {
                if (cls_app_static_var.Item_Sort_1_Code_Length > 0)
                    txt_ItemCode.MaxLength = cls_app_static_var.Item_Sort_1_Code_Length;

                if (cls_app_static_var.Item_Sort_2_Code_Length > 0)
                    txt_ItemCode.MaxLength = cls_app_static_var.Item_Sort_2_Code_Length;

                if (cls_app_static_var.Item_Sort_3_Code_Length > 0)
                    txt_ItemCode.MaxLength = cls_app_static_var.Item_Sort_3_Code_Length;


                txt_ItemCode.MaxLength = cls_app_static_var.Item_Sort_1_Code_Length
                                + cls_app_static_var.Item_Sort_2_Code_Length
                                + cls_app_static_var.Item_Sort_3_Code_Length + txt_ItemCode.MaxLength;

            }



            //tab_Cacu.TabPages.Remove(tab_VA_Bank);
            tab_Cacu.TabPages.Remove(tab_Bank);
            tab_Cacu.TabPages.Remove(tab_Mile);

            //마일리지 사용 여부에 따라서 보여질지 안보여질지
            if (cls_app_static_var.Using_Mileage_TF == 0)
                tab_Cacu.TabPages.Remove(tab_Mile);
            else
                tableLayoutPanel20.Visible = true;

            if (cls_app_static_var.Sell_Union_Flag == "")
                tableLayoutPanel68.Visible = false;  //조합이 아니면 공제 번호 관련 보여주지 않는다.

            Form_Key_Real_TF = 0;
            radioB_DESK.Checked = true;


            mtxtSellDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtSellDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPriceDate3.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPriceDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPriceDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPriceDate4.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPriceDate6.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtTel1.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtTel2.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtZip1.Mask = cls_app_static_var.ZipCode_Number_Fromat;

            mtxtSn.BackColor = cls_app_static_var.txt_Enable_Color;
            txtCenter.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_OrderNumber.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_Ins_Number.BackColor = cls_app_static_var.txt_Enable_Color;

            txt_SumPr.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumPV.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumCV.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumCard.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumCash.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumCoupon.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumBank.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumMile.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalInputPrice.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_UnaccMoney.BackColor = cls_app_static_var.txt_Enable_Color;

            txt_C_Bank_Code.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_C_Bank_Code_2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_C_Bank_Code_3.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_Price_4_2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_C_Cash_Number2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_AVC_Cash_Number2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_Us.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_Tel.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_InputPass_Pay.BackColor = cls_app_static_var.txt_Enable_Color;

            Data_Set_Form_TF = 1;
            mtxtSellDate.Text = cls_User.gid_date_time;
            mtxtSellDate2.Text = cls_User.gid_date_time;
            txtSellCode_Code.Text = "01";
            //txtSellCode.Text = "일반주문";
            txtSellCode.Text = (cls_User.gid_CountryCode == "TH" ? "Regular_order" : "일반주문");
            //txtCenter2_Code.Text = "50000";
            //txtCenter2.Text = "물류센터";
            txtCenter2_Code.Text = (cls_User.gid_CountryCode == "TH" ? "999" : "50000");                // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
            txtCenter2.Text = (cls_User.gid_CountryCode == "TH" ? "TH_Center1" : "물류센터");    // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh

            Data_Set_Form_TF = 0;

            txt_Receive_Method.Text = "";
            txt_Receive_Method_Code.Text = "";
            txtCenter3.Text = "";
            txtCenter3_Code.Text = "";

            dGridView_Base_Rece_Item.Dock = DockStyle.Fill;
            if (cls_app_static_var.Rec_info_Multi_TF == 1)
                pan_Rec_Item.Visible = false;

            chK_PV_CV_Check.Checked = true;
            MP_YN = "N";
            mtxtMbid.Focus();

            InitComboZipCode_TH();
            // 태국버전 인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                pnlDistrict_TH.Visible = true;
                pnlProvince_TH.Visible = true;
                pnlSubDistrict_TH.Visible = true;
                pnlZipCode_TH.Visible = true;
                pnlZipCode_KR.Visible = false;
                txtAddress2.ReadOnly = true;
                cbSubDistrict_TH_SelectedIndexChanged(this, null);
                //combo_Se_Code_2.Text = "TH";

                tab_Cacu.TabPages.Remove(tab_Coupon);   // 결제내역 - 쿠폰
                tab_Cacu.TabPages.Remove(tab_VA_Bank);  // 결제내역 - 가상계좌
                pnlCashReceipt.Visible = false;         // 결재내역 - 현금 - 현금영수증
                btn_CASH_OK.Visible = false;            // 결재내역 - 현금 - 현금결제 승인
                btnCashReceiptCancel.Visible = false;   // 결재내역 - 현금 - 현금영수증 취소

                pnl_Coupon.Visible = false;
                pnl_Card_Birth.Visible = false;
                pnl_Installment.Visible = false;
                pnl_Card_CVC.Visible = true;

                button_Auth.Visible = false;    // 카드 인증 버튼
            }
            // 태국 이외 버전 인 경우
            else
            {
                pnlDistrict_TH.Visible = false;
                pnlProvince_TH.Visible = false;
                pnlSubDistrict_TH.Visible = false;
                pnlZipCode_TH.Visible = false;
                pnlZipCode_KR.Visible = true;
                txtAddress2.ReadOnly = false;
                txtAddress2.Clear();

                pnl_Card_Birth.Visible = true;
                pnl_Installment.Visible = true;
                pnl_Card_CVC.Visible = false;

                button_Auth.Visible = true;     // 카드 인증 버튼
            }
        }

        private void InitComboZipCode_TH()
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_State_TH() ORDER BY ZIPCODE_SORT ");
            sb.AppendLine("SELECT * FROM ufn_Get_ZipCode_Province_TH() ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbProvince_TH.DataBindings.Clear();
            cbProvince_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbProvince_TH.DisplayMember = "ZipCode_NM";
            cbProvince_TH.ValueMember = "ProvinceCode";

            //cbZipCode_TH.SelectedIndex = -1;
            txtZipCode_TH.Text = "";
            txtAddress2.Text = "";
            cbDistrict_TH.SelectedIndex = -1;
            cbProvince_TH.SelectedIndex = -1;
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            //int base_w = this.Width / 3;
            //butt_Clear.Width = base_w;
            //butt_Save.Width = base_w;

            ////butt_Delete.Width = base_w;
            //butt_Exit.Width = base_w;

            //butt_Clear.Left = 0;
            //butt_Save.Left = butt_Clear.Left + butt_Clear.Width;

            ////butt_Delete.Left = butt_Save.Left + butt_Save.Width;
            //butt_Exit.Left = butt_Save.Left + butt_Save.Width;    

            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            //butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Exit);

            cfm.button_flat_change(butt_Ord_Clear);
            cfm.button_flat_change(butt_Print);

            cfm.button_flat_change(butt_Item_Del);
            cfm.button_flat_change(butt_Item_Save);
            cfm.button_flat_change(butt_Item_Clear);

            cfm.button_flat_change(butt_Cacu_Del);
            cfm.button_flat_change(butt_Cacu_Save);
            cfm.button_flat_change(butt_Cacu_Clear);

            cfm.button_flat_change(butt_Rec_Del);
            cfm.button_flat_change(butt_Rec_Save);
            cfm.button_flat_change(butt_Rec_Clear);
            cfm.button_flat_change(butt_Rec_Add);

            cfm.button_flat_change(butt_Mile_Search);

            cfm.button_flat_change(butt_AddCode);

            cfm.button_flat_change(button_Ok);
            cfm.button_flat_change(button_Cancel);

            cfm.button_flat_change(buttonV_Ok);
            cfm.button_flat_change(buttonV_Cancel);





        }



        private void frmBase_Activated(object sender, EventArgs e)
        {
            ////23-03-11 깜빡임제거 this.Refresh();

            string Send_Number = ""; string Send_Name = ""; string Send_OrderNumber = "";

            Take_Mem_Number(ref Send_Number, ref Send_Name, ref Send_OrderNumber);

            if (Send_Number != "")
            {
                mtxtMbid.Text = Send_Number;
                Set_Form_Date(mtxtMbid.Text, "m");
                Coupon_Grid_Set();
                if (Send_OrderNumber != "")
                {
                    Base_Ord_Clear();
                    Put_OrderNumber_SellDate(Send_OrderNumber);
                }
            }

        }


        private void frmBase_From_KeyDown(object sender, KeyEventArgs e)
        {
            //폼일 경우에는 ESC버튼에 폼이 종료 되도록 한다
            if (sender is Form)
            {
                if (e.KeyCode == Keys.Escape)
                {

                    if (!this.Controls.ContainsKey("Popup_gr") && dGridView_Base_Rece_Add.Visible == false && dGridView_Base_Mile.Visible == false)
                        this.Close();
                    else
                    {
                        if (dGridView_Base_Rece_Add.Visible == true)
                        {
                            dGridView_Base_Rece_Add.Visible = false;

                            cls_form_Meth cfm = new cls_form_Meth();
                            cfm.form_Group_Panel_Enable_True(this);
                        }
                        else if (dGridView_Base_Mile.Visible == true)
                        {
                            dGridView_Base_Mile.Visible = false;
                            txt_Price_4.Focus();
                        }
                        else
                        {
                            DataGridView T_Gd = (DataGridView)this.Controls["Popup_gr"];

                            if (T_Gd.Name == "Popup_gr")
                            {
                                if (T_Gd.Tag != null)
                                {
                                    if (!this.Controls.ContainsKey(T_Gd.Tag.ToString()))
                                    {
                                        cls_form_Meth cfm = new cls_form_Meth();
                                        Control T_cl = cfm.from_Search_Control(this, T_Gd.Tag.ToString());
                                        if (T_cl != null)
                                            T_cl.Focus();

                                    }
                                }

                                T_Gd.Visible = false;
                                T_Gd.Dispose();

                                //cls_form_Meth cfm = new cls_form_Meth();
                                //cfm.form_Group_Panel_Enable_True(this);
                            }
                        }


                    }



                }// end if

            }



            Button T_bt = butt_Exit;
            if (e.KeyValue == 123)
                T_bt = butt_Exit;    //닫기  F12
            if (e.KeyValue == 113)
                T_bt = butt_Save;     //저장  F1
            //if (e.KeyValue == 115)
            //    T_bt = butt_Delete;   // 삭제  F4
            if (e.KeyValue == 112)
                T_bt = butt_Clear;    //엑셀  F5    

            if (T_bt.Visible == true)
            {
                EventArgs ee1 = null;
                if (e.KeyValue == 123 || e.KeyValue == 113 || e.KeyValue == 123 || e.KeyValue == 112)
                    Base_Button_Click(T_bt, ee1);
            }

        }




        private void txtData_Enter(object sender, EventArgs e)
        {
            if (Form_Key_Real_TF >= 1)
                return;

            cls_Check_Text T_R = new cls_Check_Text();

            if (sender is TextBox)
            {
                T_R.Text_Focus_All_Sel((TextBox)sender);
                TextBox tb = null;
                tb = (TextBox)sender;

                //Data_Set_Form_TF = 1;
                //if (tb.Name == "txt_Price_3")
                //{
                //    if (txt_UnaccMoney.Text.Replace(",", "") != "")
                //    {
                //        if (tb.Text == "" && double.Parse(txt_UnaccMoney.Text.Replace(",", "")) > 0)
                //            tb.Text = txt_UnaccMoney.Text;

                //        if (txt_Price_3_2.Text == "")
                //            txt_Price_3_2.Text = tb.Text.Trim();
                //    }
                //}

                //if (tb.Name == "txt_Price_1")
                //{
                //    if (txt_UnaccMoney.Text.Replace(",", "") != "")
                //    {
                //        if (tb.Text == "" && double.Parse(txt_UnaccMoney.Text.Replace(",", "")) > 0)
                //            tb.Text = txt_UnaccMoney.Text;
                //    }          
                //}

                //if (tb.Name == "txt_Price_5_2")
                //{
                //    if (txt_UnaccMoney.Text.Replace(",", "") != "")
                //    {
                //        if (tb.Text == "" && double.Parse(txt_UnaccMoney.Text.Replace(",", "")) > 0)
                //            tb.Text = txt_UnaccMoney.Text;
                //    }
                //}



                //Data_Set_Form_TF = 0;

                if (tb.ReadOnly == false)
                    tb.BackColor = cls_app_static_var.txt_Focus_Color;
            }

            if (sender is MaskedTextBox)
            {
                T_R.Text_Focus_All_Sel((MaskedTextBox)sender);
                MaskedTextBox tb = (MaskedTextBox)sender;
                if (tb.ReadOnly == false)
                    tb.BackColor = cls_app_static_var.txt_Focus_Color;
            }

            if (this.Controls.ContainsKey("Popup_gr"))
            {
                DataGridView T_Gd = (DataGridView)this.Controls["Popup_gr"];
                T_Gd.Visible = false;
                T_Gd.Dispose();
            }
        }

        private void txtData_Base_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                if (tb.ReadOnly == false)
                    tb.BackColor = Color.White;



                if (new string[] { "txtSellCode_Code", "txtSellCode" }.Any(x => x.Equals((sender as Control).Name)))
                {
                    if (!SetSellCode()) return;
                }

                if (tb.Tag != null)
                {
                    if (tb.Tag.ToString() == "2" && tb.Text != "")
                    {
                        Data_Set_Form_TF = 1;
                        double T_p = double.Parse(tb.Text.Replace(",", "").ToString());
                        tb.Text = string.Format(cls_app_static_var.str_Currency_Type, T_p);
                        Data_Set_Form_TF = 0;
                    }
                }


                if (tb.Name == "mtxtSellDate")
                {

                    if (tb.Text != "" && mtxtSellDate2.Text.Replace("-", "").Trim() == "")
                    {
                        mtxtSellDate2.Text = tb.Text;
                    }

                    if (tb.Text != "")
                    {
                        if (Base_Error_Check_Not_Sellcode__01() == false)
                        {
                            txtSellCode.Focus();
                            return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크
                        }

                        double T_p = 0;
                        string T_Mbid = mtxtMbid.Text;
                        string Mbid = ""; int Mbid2 = 0;
                        cls_Search_DB csb = new cls_Search_DB();
                        if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
                        {
                            cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                            T_p = ctm.Using_Mileage_Search(Mbid, Mbid2, tb.Text);
                            txt_Price_4_2.Text = string.Format(cls_app_static_var.str_Currency_Type, T_p);
                        }
                    }

                    txtSellCode.Focus();
                }


                if (tb.Name == "txt_Receive_Method")
                {
                    if (txt_Receive_Method_Code.Text == "3" && txtCenter2_Code.Text != "")  //센타수령일 경우에 센타 정보를 넣어준다.
                    {
                        Input_Center_Address();
                    }
                    if (tb.Text.Trim() == "직접수령")
                    {
                        mtxtZip1.Text = "";
                        txtAddress1.Text = "";
                        txtAddress2.Text = "";
                        pnlZipCode_KR.Enabled = false;
                        tableLayoutPanel42.Enabled = false;
                        tableLayoutPanel37.Enabled = false;
                        //tableLayoutPanel81.Visible = true;
                        if (cls_User.gid_CenterCode != "")
                        {
                            string Tsql = "";
                            Tsql = "Select Name ";
                            Tsql = Tsql + " From tbl_Business (nolock) ";
                            Tsql = Tsql + " Where ncode = '" + cls_User.gid_CenterCode + "' ";
                            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                            DataSet ds = new DataSet();
                            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Business", ds) == false) return;
                            int ReCnt = Temp_Connect.DataSet_ReCount;

                            if (ReCnt > 0)
                            {
                                txtCenter3.Text = ds.Tables["tbl_Business"].Rows[0]["Name"].ToString();
                            }
                            txtCenter3_Code.Text = cls_User.gid_CenterCode;
                        }


                    }
                    else
                    {
                        pnlZipCode_KR.Enabled = true;
                        tableLayoutPanel42.Enabled = true;
                        tableLayoutPanel37.Enabled = true;
                        //txtCenter3.Text = "";
                        //txtCenter3_Code.Text = "";
                        //tableLayoutPanel81.Visible = false;
                    }
                }

                if (tb.Name == "txtName" && idx_Na_Code == "" && tb.Text.Trim() != "")
                {

                    int reCnt = 0;
                    cls_Search_DB cds = new cls_Search_DB();
                    string Search_Mbid = "";
                    reCnt = cds.Member_Name_Search_S_N(ref Search_Mbid, tb.Text);

                    if (reCnt == 1)
                    {
                        mtxtMbid.Text = Search_Mbid; //회원명으로 검색해서 나온 사람이 한명일 경우에는 회원번호를 넣어준다.                    
                        if (Input_Error_Check(mtxtMbid, "m") == true)
                            Set_Form_Date(mtxtMbid.Text, "m");
                        Coupon_Grid_Set();
                    }
                    else if (reCnt != 1)  //동명이인이 존재해서 사람이 많을 경우나 또는 이름 없이 엔터친 경우에.
                    {
                        frmBase_Member_Search e_f = new frmBase_Member_Search();

                        e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                        e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemName_Info);

                        e_f.ShowDialog();

                        SendKeys.Send("{TAB}");
                    }
                }

            }

            if (sender is MaskedTextBox)
            {
                MaskedTextBox tb = (MaskedTextBox)sender;
                if (tb.ReadOnly == false)
                    tb.BackColor = Color.White;
            }
            if (txtCenter2.Text == "물류센터")
            {
                txt_Receive_Method.Text = "배송";
                txt_Receive_Method_Code.Text = "2";
                txtCenter3.Text = "물류센터";
                txtCenter3_Code.Text = "50000";
            }
            if (txtCenter2.Text == "서울픽업")
            {
                txt_Receive_Method.Text = "직접수령";
                txt_Receive_Method_Code.Text = "1";
                txtCenter3.Text = "서울픽업";
                txtCenter3_Code.Text = "50010";
            }
            if (txtCenter2.Text == "부산픽업")
            {
                txt_Receive_Method.Text = "직접수령";
                txt_Receive_Method_Code.Text = "1";
                txtCenter3.Text = "부산픽업";
                txtCenter3_Code.Text = "50011";
            }
            if (txtCenter2.Text == "인천픽업")
            {
                txt_Receive_Method.Text = "직접수령";
                txt_Receive_Method_Code.Text = "1";
                txtCenter3.Text = "인천픽업";
                txtCenter3_Code.Text = "50012";
            }

            // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
            if (txtCenter2.Text == "TH_Center1" && cls_User.gid_CountryCode == "TH")
            {
                txt_Receive_Method.Text = "Delivery";
                txt_Receive_Method_Code.Text = "2";
                txtCenter3.Text = "TH_Center1";    // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
                txtCenter3_Code.Text = "999";                // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
            }
        }

        private bool SetSellCode()
        {
            bool ret = true;
            if (dGridView_Base_Item.Rows.Count > 0 &&
                (txtSellCode_Code.Text != lblSellCode_Code.Text.ToString() || txtSellCode.Text != lblSellCode.Text.ToString())
                )
            {
                txtSellCode_Code.Text = lblSellCode_Code.Text.ToString();
                txtSellCode.Text = lblSellCode.Text.ToString();
                ret = false;
            }
            else
            {
                lblSellCode_Code.Text = txtSellCode_Code.Text;
                lblSellCode.Text = txtSellCode.Text;
            }

            return ret;
        }


        private void Input_Center_Address()
        {
            if (txtCenter2_Code.Text == "")
                return;

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";
            DataSet ds = new DataSet();
            int ReCnt = 0;


            Tsql = "Select ZipCode Addcode1 ,add1 Address1 , add2 Address2  ";
            Tsql = Tsql + " ,phone hptel ,phone homeTel , bossname M_Name ";
            Tsql = Tsql + " From tbl_Business (nolock ) ";
            Tsql = Tsql + " Where ncode = '" + txtCenter2_Code.Text + "'";


            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            ReCnt = Temp_Connect.DataSet_ReCount;


            Data_Set_Form_TF = 1;
            mtxtZip1.Text = "";
            txtAddress1.Text = ""; txtAddress2.Text = "";
            mtxtTel1.Text = "";
            mtxtTel2.Text = "";
            txt_Get_Name1.Text = "";
            Data_Set_Form_TF = 0;

            if (ReCnt == 0) return;

            Data_Set_Form_TF = 1;
            txtAddress1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address1"].ToString());
            txtAddress2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address2"].ToString());

            mtxtZip1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Replace("-", "");
            //string AddCode = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Replace("-", "");
            //if (AddCode.Length >= 6)
            //{
            //    mtxtZip1.Text = AddCode.Substring(0, 3) + "-" + AddCode.Substring(3, 3);
            //    //txtAddCode1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(0, 3);
            //    //txtAddCode2.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(3, 3);
            //}

            string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
            cls_form_Meth cfm = new cls_form_Meth();
            //cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            //mtxtTel1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString());

            //cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;
            //mtxtTel2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString());

            txt_Get_Name1.Text = ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString();  //회원 테이블의 회원명은 암호화 안햇음
            Data_Set_Form_TF = 0;
        }

        private void MtxtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            //회원번호 관련칸은 소문자를 다 대문자로 만들어 준다.
            if (e.KeyChar >= 97 && e.KeyChar <= 122)
            {
                string str = e.KeyChar.ToString().ToUpper();
                char[] ch = str.ToCharArray();
                e.KeyChar = ch[0];
            }

            //if (!(Char.IsLetter(e.KeyChar)) && e.KeyChar != 8)
            //{
            //    e.Handled = true;
            //}



            if (e.KeyChar == 13)
            {
                MaskedTextBox mtb = (MaskedTextBox)sender;

                if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                {
                    int reCnt = 0;
                    cls_Search_DB cds = new cls_Search_DB();
                    string Search_Name = "";
                    reCnt = cds.Member_Name_Search(mtb.Text, ref Search_Name);

                    if (reCnt == 1)
                    {
                        if (mtb.Name == "mtxtMbid")
                        {
                            txtName.Text = Search_Name;
                            txt_C_Name_3.Text = Search_Name;
                            if (Input_Error_Check(mtb, "m") == true)
                                Set_Form_Date(mtb.Text, "m");
                            Coupon_Grid_Set();
                            txtCenter2.Focus();
                        }
                    }

                    else if (reCnt > 1)  //회원번호 비슷한 사람들이 많은 경우
                    {
                        string Mbid = "";
                        int Mbid2 = 0;
                        cds.Member_Nmumber_Split(mtb.Text, ref Mbid, ref Mbid2);

                        frmBase_Member_Search e_f = new frmBase_Member_Search();

                        if (mtb.Name == "mtxtMbid")
                        {
                            e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                            e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemNumber_Info);
                        }

                        e_f.ShowDialog();

                        SendKeys.Send("{TAB}");
                    }
                }
                else
                    SendKeys.Send("{TAB}");
            }

        }


        void e_f_Send_MemNumber_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            seachName = "";
            cls_Search_DB csb = new cls_Search_DB();
            csb.Member_Nmumber_Split(mtxtMbid.Text.Trim(), ref searchMbid, ref searchMbid2);
        }



        //변경할려는 대상자에 대한 회원번호에서 회원 검색창을 뛰엇을 경우에
        void e_f_Send_Mem_Number(string Send_Number, string Send_Name)
        {
            mtxtMbid.Text = Send_Number; txtName.Text = Send_Name;
            txt_C_Name_3.Text = Send_Name;
            if (Input_Error_Check(mtxtMbid, "m") == true)
                Set_Form_Date(mtxtMbid.Text, "m");
            Coupon_Grid_Set();
        }


        private void mtxtMbid_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Text.Replace("_", "").Replace("-", "").Replace(" ", "") == "")
            {
                if (mtb.Name == "mtxtMbid")
                {
                    _From_Data_Clear();
                    Form_Key_Real_TF = 0;
                }

            }
        }


        //회원번호 클릿햇을때. 관련 정보들 다 리셋 시킨다.
        //추후 번호만 변경하고 엔터 안누눌러서.. 데이타가 엉키는 것을 방지하기 위함.
        private void mtxtMbid_Click(object sender, EventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Name == "mtxtMbid")
            {
                _From_Data_Clear();
            }

            Form_Key_Real_TF = 0;
            mtxtMbid.Focus();

            //마스크텍스트 박스에 입력한 내용이 있으면 그곳 다음으로 커서가 가게 한다.
            if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                mtb.SelectionStart = mtb.Text.Replace("-", "").Replace("_", "").Trim().Length + 1;

        }











        private void Set_Form_Date(string T_Mbid, string T_sort)
        {
            _From_Data_Clear();
            Form_Key_Real_TF = 0;
            //idx_Mbid = ""; idx_Mbid2 = 0;
            string Mbid = ""; int Mbid2 = 0; idx_Na_Code = "";
            Data_Set_Form_TF = 1;
            cls_Search_DB csb = new cls_Search_DB();



            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
            {
                string Tsql = "";

                Tsql = "Select  ";
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                else
                    Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";

                Tsql = Tsql + " ,tbl_Memberinfo.mbid ";
                Tsql = Tsql + " ,tbl_Memberinfo.mbid2 ";
                Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";
                Tsql = Tsql + " , tbl_Memberinfo.Na_Code ";
                Tsql = Tsql + ", tbl_Memberinfo.Cpno ";

                Tsql = Tsql + " , tbl_Memberinfo.LineCnt ";

                Tsql = Tsql + " , tbl_Memberinfo.RegTime ";
                Tsql = Tsql + " , tbl_Memberinfo.hptel ";
                Tsql = Tsql + " , tbl_Memberinfo.hometel ";

                Tsql = Tsql + " , tbl_Memberinfo.businesscode ";
                Tsql = Tsql + " ,Isnull(tbl_Business.Name,'') as B_Name";

                Tsql = Tsql + " , tbl_Memberinfo.Remarks ";

                Tsql = Tsql + " , tbl_Memberinfo.LeaveDate ";
                Tsql = Tsql + " , tbl_Memberinfo.LineUserDate ";
                Tsql = Tsql + " , tbl_Memberinfo.WebID ";
                Tsql = Tsql + " , tbl_Memberinfo.WebPassWord ";
                Tsql = Tsql + " , tbl_Memberinfo.Ed_Date ";
                Tsql = Tsql + " , tbl_Memberinfo.PayStop_Date ";

                Tsql = Tsql + " , tbl_Memberinfo.For_Kind_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.Sell_Mem_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.Us_Num ";

                Tsql = Tsql + " , tbl_Memberinfo.CurPoint ";
                Tsql = Tsql + " ,  dbo.DECRYPT_AES256(tbl_Memberinfo.bankaccnt) as bankaccnt ";


                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }

                //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
                //Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                //Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";


                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                //++++++++++++++++++++++++++++++++
                Set_Form_Date(ds); //위의 DataSet객체를 가져가서 회원 정보를 넣는다
                Sell_Mem_TF = ds.Tables[base_db_name].Rows[0]["Sell_Mem_TF"].ToString();


                //if (ds.Tables[base_db_name].Rows[0]["bankaccnt"].ToString().Trim() == "")
                //{  
                //    MessageBox.Show("가상계좌가 등록되어있지않은 회원입니다!");
                //}

                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                if (SalesDetail != null)
                    SalesDetail.Clear();

                Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

                if (SalesDetail != null)
                {
                    Base_Grid_Set();  // 위에서 배열에 넣은 내역을 그리드 상으로 옴김 판매 주 내역

                    Set_SalesItemDetail(Mbid, Mbid2);        //상품 집계 관련해서 도표에 뿌려준다.            
                }
                mtxtMbid.Focus();
            }

            Data_Set_Form_TF = 0;
        }




        private void Set_Form_Date(DataSet ds)
        {

            if (ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString() != "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Leave_Sell"));
                return;
            }

            idx_Mbid = ds.Tables[base_db_name].Rows[0]["Mbid"].ToString();
            idx_Mbid2 = int.Parse(ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString());
            idx_Na_Code = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();

            mtxtMbid.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
            txtName.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
            txt_C_Name_3.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
            mtxtSn.Text = encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Cpno"].ToString(), "Cpno");

            txtCenter.Text = ds.Tables[base_db_name].Rows[0]["B_Name"].ToString();
            txtCenter_Code.Text = ds.Tables[base_db_name].Rows[0]["businesscode"].ToString();

            txt_Us.Text = ds.Tables[base_db_name].Rows[0]["Us_Num"].ToString();


            //r김선웅씨하고 통화후에 변경함 공란으로 기본 값이구.. 저장시에 공란이면 입력하라고 하도록 변경함 2016-01-21
            //txtCenter2.Text = txtCenter.Text;
            //txtCenter2_Code.Text = txtCenter_Code.Text;

            txt_Tel.Text = ds.Tables[base_db_name].Rows[0]["hptel"].ToString(); // + " / " + ds.Tables[base_db_name].Rows[0]["hometel"].ToString(); 

            // Tsql = Tsql + " , tbl_Memberinfo.hptel ";
            //Tsql = Tsql + " , tbl_Memberinfo.hometel ";
            //txtName.ReadOnly = false;
            //txtName.BackColor = SystemColors.Window;
            //txtName.BorderStyle = BorderStyle.Fixed3D;

            txtName.ReadOnly = true;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.BackColor = cls_app_static_var.txt_Enable_Color;

        }


        private void Set_SalesDetail()
        {
            cls_form_Meth cm = new cls_form_Meth();
            string strSql = "";

            strSql = "Select tbl_SalesDetail.* ";
            strSql = strSql + " , tbl_Business.Name BusCodeName ";

            // 한국인 경우
            if (cls_NationService.GetCountryCodeOrDefault(cls_User.gid_CountryCode) == "KR")
            {
                strSql = strSql + " , tbl_SellType.SellTypeName SellCodeName  ";
            }
            // 태국인 경우
            else if (cls_NationService.GetCountryCodeOrDefault(cls_User.gid_CountryCode) == "TH")
            {
                strSql = strSql + " , tbl_SellType.SellTypeName_En SellCodeName  ";
            }

            strSql = strSql + " , Ga_Order  SellTF ";
            strSql = strSql + " ,Case When Ga_Order >= 1 Then '" + cm._chang_base_caption_search("미승인") + "'";
            strSql = strSql + "  When Ga_Order = 0 Then '" + cm._chang_base_caption_search("승인") + "'";
            strSql = strSql + " ELSE '' ";
            strSql = strSql + " END SellTFName ";

            strSql = strSql + " ,Case When ReturnTF = 1 Then '" + cm._chang_base_caption_search("정상") + "'";
            strSql = strSql + "  When ReturnTF = 2 Then '" + cm._chang_base_caption_search("반품") + "'";
            strSql = strSql + "  When ReturnTF = 4 Then '" + cm._chang_base_caption_search("교환") + "'";
            strSql = strSql + "  When ReturnTF = 3 Then '" + cm._chang_base_caption_search("부분반품") + "'";
            strSql = strSql + "  When ReturnTF = 5 Then '" + cm._chang_base_caption_search("취소") + "'";
            strSql = strSql + " END ReturnTFName ";

            if (cls_app_static_var.Sell_Union_Flag == "U")  //특판
            {
                strSql = strSql + " , Case When  tbl_SalesDetail.union_Seq > 0 And T_REALMLM.ERRCODE = '0000' Then ISNULL(T_REALMLM.GUARANTE_NUM,'') ";
                strSql = strSql + "        When  tbl_SalesDetail.union_Seq > 0 And T_REALMLM.ERRCODE <> '0000' Then  ISNULL(T_REALMLM_ErrCode.Er_Msg ,'' ) ";
                //strSql = strSql + "        When  tbl_SalesDetail.union_Seq = 0 Then '미신고'  ";
                strSql = strSql + "        When  tbl_SalesDetail.union_Seq = 0 Then '" + cm._chang_base_caption_search("미신고") + "'  ";
                strSql = strSql + "   End InsuranceNumber2 ";
            }
            else if (cls_app_static_var.Sell_Union_Flag == "D")  //직판
            {
                //strSql = strSql + ", Case When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail AS A1 Where tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber ";
                //strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail AS A1 Where tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql = strSql + " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail AS A1 Where tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)' ";
                //strSql = strSql + " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '미승인요청' ";
                //strSql = strSql + " ELSE tbl_SalesDetail.InsuranceNumber END  InsuranceNumber2 ";


                //strSql = strSql + ", Case When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber ";
                //strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql = strSql + " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)' ";
                //strSql = strSql + " When  ReturnTF = 2 then '반품처리' ";
                //strSql = strSql + " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '재발급요청요망' + ' ' + tbl_SalesDetail.INS_Num_Err  ";
                //strSql = strSql + " ELSE tbl_SalesDetail.InsuranceNumber END  InsuranceNumber2 ";


                strSql += ", Case When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber ";
                //strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql += " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 2 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)' ";
                //strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 3 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(부분취소요청중)' ";
                //strSql += " When  ReturnTF = 2 then '반품처리' ";
                //strSql += " When  ReturnTF = 3 then '부분반품처리' ";
                //strSql += " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '재발급요청요망' + ' ' + tbl_SalesDetail.INS_Num_Err  ";
                //strSql += " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(" + cm._chang_base_caption_search("취소상태") + "' ";
                strSql += " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(" + cm._chang_base_caption_search("취소상태") + "' ";
                strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 2 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(" + cm._chang_base_caption_search("취소요청중") + "' ";
                strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 3 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(" + cm._chang_base_caption_search("부분취소요청중") + "' ";
                strSql += " When  ReturnTF = 2 then '" + cm._chang_base_caption_search("반품처리") + "' ";
                strSql += " When  ReturnTF = 3 then '" + cm._chang_base_caption_search("부분반품처리") + "' ";
                strSql += " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '" + cm._chang_base_caption_search("재발급요청요망") + "' + ' ' + tbl_SalesDetail.INS_Num_Err  ";
                strSql += " ELSE tbl_SalesDetail.InsuranceNumber END InsuranceNumber2 ";
            }
            else
            {
                strSql = strSql + " , InsuranceNumber AS InsuranceNumber2 ";
            }

            strSql = strSql + " ,tbl_SalesDetail.InsuranceNumber  AS Real_InsuranceNumber  ";
            strSql = strSql + " ,tbl_sales_cacu.Associated_Card as Associated_Card";
            strSql = strSql + " ,tbl_Memberinfo.Sell_Mem_TF as Sell_Mem_TF";
            strSql = strSql + " From tbl_SalesDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
            strSql = strSql + " LEFT Join tbl_SellType ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode ";
            strSql = strSql + " LEFT JOIN tbl_Business (nolock) ON tbl_SalesDetail.BusCode = tbl_Business.NCode And tbl_SalesDetail.Na_code = tbl_Business.Na_code ";
            strSql = strSql + " LEFT JOIN T_REALMLM (nolock) ON T_REALMLM.SEQ = tbl_SalesDetail.union_Seq ";
            strSql = strSql + " LEFT JOIN T_REALMLM_ErrCode (nolock) ON T_REALMLM.ERRCODE = T_REALMLM_ErrCode.Er_Code ";
            strSql = strSql + " LEFT JOIN tbl_sales_cacu (nolock) ON tbl_SalesDetail.ordernumber = tbl_sales_cacu.ordernumber ";
            if (idx_Mbid.Length == 0)
                strSql = strSql + " Where tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
            else
            {
                strSql = strSql + " Where tbl_Memberinfo.Mbid = '" + idx_Mbid + "' ";
                strSql = strSql + " And   tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
            }

            //// strSql = strSql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
           
            //20200929구현호 이거 지워야함
            // strSql = strSql + " And tbl_SalesDetail.BusCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
            strSql = strSql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";
            strSql = strSql + " Order By  CAST(tbl_SalesDetail.RecordTime AS DATETIME) DESC";//SellDate DESC , OrderNumber DESC ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            Dictionary<string, cls_Sell> T_SalesDetail = new Dictionary<string, cls_Sell>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_Sell t_c_sell = new cls_Sell();

                t_c_sell.OrderNumber = ds.Tables[base_db_name].Rows[fi_cnt]["OrderNumber"].ToString();
                t_c_sell.Mbid = ds.Tables[base_db_name].Rows[fi_cnt]["Mbid"].ToString();
                t_c_sell.Mbid2 = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Mbid2"].ToString());
                t_c_sell.M_Name = ds.Tables[base_db_name].Rows[fi_cnt]["M_Name"].ToString();
                t_c_sell.SellDate = ds.Tables[base_db_name].Rows[fi_cnt]["SellDate"].ToString();
                t_c_sell.SellDate_2 = ds.Tables[base_db_name].Rows[fi_cnt]["SellDate_2"].ToString();
                t_c_sell.SellCode = ds.Tables[base_db_name].Rows[fi_cnt]["SellCode"].ToString();
                t_c_sell.SellCodeName = ds.Tables[base_db_name].Rows[fi_cnt]["SellCodeName"].ToString();
                t_c_sell.BusCode = ds.Tables[base_db_name].Rows[fi_cnt]["BusCode"].ToString();
                t_c_sell.BusCodeName = ds.Tables[base_db_name].Rows[fi_cnt]["BusCodeName"].ToString();
                t_c_sell.SellSort = ds.Tables[base_db_name].Rows[fi_cnt]["SellSort"].ToString();
                t_c_sell.Re_BaseOrderNumber = ds.Tables[base_db_name].Rows[fi_cnt]["Re_BaseOrderNumber"].ToString();
                t_c_sell.TotalPrice = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalPrice"].ToString());
                t_c_sell.TotalPV = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalPV"].ToString());
                t_c_sell.TotalCV = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalCV"].ToString());
                t_c_sell.TotalInputPrice = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalInputPrice"].ToString());
                t_c_sell.Total_Sell_VAT_Price = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Total_Sell_VAT_Price"].ToString());
                t_c_sell.Total_Sell_Except_VAT_Price = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Total_Sell_Except_VAT_Price"].ToString());
                t_c_sell.InputCash = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputCash"].ToString());
                t_c_sell.InputCard = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputCard"].ToString());
                t_c_sell.InputNaver = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputNaver"].ToString());
                t_c_sell.InputPassbook = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPassbook"].ToString());
                t_c_sell.InputPassbook_2 = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPassbook_2"].ToString());
                t_c_sell.InputCoupon = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputCoupon"].ToString());
                t_c_sell.InputPayment_8_TH =  double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPayment_8_TH"].ToString());
                t_c_sell.InputPayment_9_TH =  double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPayment_9_TH"].ToString());
                t_c_sell.InputPayment_10_TH = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPayment_10_TH"].ToString());


                t_c_sell.Be_InputMile = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputMile"].ToString());
                t_c_sell.InputMile = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputMile"].ToString());
                t_c_sell.InputPass_Pay = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPass_Pay"].ToString());
                t_c_sell.UnaccMoney = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["UnaccMoney"].ToString());

                t_c_sell.Etc1 = ds.Tables[base_db_name].Rows[fi_cnt]["Etc1"].ToString();
                t_c_sell.Etc2 = ds.Tables[base_db_name].Rows[fi_cnt]["Etc2"].ToString();

                t_c_sell.ReturnTF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ReturnTF"].ToString());
                t_c_sell.ReturnTFName = ds.Tables[base_db_name].Rows[fi_cnt]["ReturnTFName"].ToString();
                //t_c_sell.InsuranceNumber = ds.Tables[base_db_name].Rows[fi_cnt]["InsuranceNumber"].ToString();
                t_c_sell.INS_Num = ds.Tables[base_db_name].Rows[fi_cnt]["InsuranceNumber2"].ToString();

                t_c_sell.InsuranceNumber_Date = ds.Tables[base_db_name].Rows[fi_cnt]["InsuranceNumber_Date"].ToString();
                t_c_sell.W_T_TF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["W_T_TF"].ToString());
                t_c_sell.In_Cnt = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["In_Cnt"].ToString());

                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();
                t_c_sell.Exi_TF = ds.Tables[base_db_name].Rows[fi_cnt]["Exi_TF"].ToString();

                t_c_sell.SellTF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SellTF"].ToString());
                t_c_sell.SellTFName = ds.Tables[base_db_name].Rows[fi_cnt]["SellTFName"].ToString();

                string t_sellDate = t_c_sell.SellDate.Substring(0, 4);
                t_sellDate = t_sellDate + "-" + t_c_sell.SellDate.Substring(4, 2);
                t_sellDate = t_sellDate + "-" + t_c_sell.SellDate.Substring(6, 2);
                t_c_sell.SellDate = t_sellDate;
                t_c_sell.Us_Ord = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Us_Ord"].ToString());
                if(ds.Tables[base_db_name].Rows[fi_cnt]["Associated_Card"].ToString() == null)
                {
                    t_c_sell.Associated_Card = "";
                }
                else
                {
                    t_c_sell.Associated_Card = ds.Tables[base_db_name].Rows[fi_cnt]["Associated_Card"].ToString();

                }
                //소비자는 1 판매원은 기본 0
                if (ds.Tables[base_db_name].Rows[0]["Sell_Mem_TF"].ToString() == "1")
                    opt_sell_3.Checked = true;
                else
                    opt_sell_2.Checked = true;


                t_c_sell.Del_TF = "";
                T_SalesDetail[t_c_sell.OrderNumber] = t_c_sell;
            }


            SalesDetail = T_SalesDetail;
        }









        //////SalesDetail___SalesDetail__SalesDetail__SalesDetail__SalesDetail__SalesDetail
        //////SalesDetail___SalesDetail__SalesDetail__SalesDetail__SalesDetail__SalesDetail
        private void Base_Grid_Set()
        {
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, double> SelType_1 = new Dictionary<string, double>();

            int fi_cnt = 0;
            double Sum_TotalPrice = 0;
            double Sum_TotalInputPrice = 0;
            double Sum_PV = 0;
            double Sum_InputCash = 0;
            double Sum_InputCard = 0;
            double Sum_InputPassbook = 0, Sum_InputPassbook_2 = 0; ;
            double Sum_UnaccMoney = 0; ;
            double Sum_InputMile = 0, Sum_CV = 0;
            double Sum_TH_8 = 0, Sum_TH_9 = 0, Sum_TH_10 = 0, Sum_InputNaver = 0, Sum_InputCoupon = 0 ;
            double Sell_Cnt_1 = 0;
            double Sell_Cnt_2 = 0;
            foreach (string t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D")
                {
                    Set_gr_dic(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.

                    if (SalesDetail[t_key].SellTFName == "승인")  //합계에 승인된 것만 보여달라고 요청(명충남 대표) 2016-08-17작업
                    {
                        Sum_TotalPrice = Sum_TotalPrice + SalesDetail[t_key].TotalPrice;
                        Sum_TotalInputPrice = Sum_TotalInputPrice + SalesDetail[t_key].TotalInputPrice;
                        Sum_PV = Sum_PV + SalesDetail[t_key].TotalPV;
                        Sum_CV = Sum_CV + SalesDetail[t_key].TotalCV;

                        Sum_InputCash = Sum_InputCash + SalesDetail[t_key].InputCash;
                        Sum_InputCard = Sum_InputCard + SalesDetail[t_key].InputCard;
                        Sum_InputPassbook += SalesDetail[t_key].InputPassbook;
                        Sum_InputPassbook_2 += SalesDetail[t_key].InputPassbook_2;
                        Sum_InputMile = Sum_InputMile + SalesDetail[t_key].InputMile;
                        Sum_UnaccMoney = Sum_UnaccMoney + SalesDetail[t_key].UnaccMoney;

                        Sum_InputNaver += SalesDetail[t_key].InputNaver;
                        Sum_InputCoupon += SalesDetail[t_key].InputCoupon;

                        Sum_TH_8  += SalesDetail[t_key].InputPayment_8_TH;
                        Sum_TH_9  += SalesDetail[t_key].InputPayment_9_TH;
                        Sum_TH_10 += SalesDetail[t_key].InputPayment_10_TH;
                    }

                    string T_ver = SalesDetail[t_key].SellCodeName;
                    if (SelType_1.ContainsKey(T_ver) == true)
                    {
                        SelType_1[T_ver] = SelType_1[T_ver] + SalesDetail[t_key].TotalPrice;  //금액                    
                    }
                    else
                    {
                        SelType_1[T_ver] = SalesDetail[t_key].TotalPrice;
                    }

                    T_ver = SalesDetail[t_key].RecordID;
                    if (T_ver.Contains("WEB") != true)
                    {
                        Sell_Cnt_1 = Sell_Cnt_1 + SalesDetail[t_key].TotalPrice;
                    }
                    else
                    {
                        Sell_Cnt_2 = Sell_Cnt_2 + SalesDetail[t_key].TotalPrice;
                    }
                }

                fi_cnt++;
            }

            //Reset_Chart_Total(Sum_13, Sum_14, Sum_15,Sum_17);
            //Reset_Chart_Total(ref SelType_1);
            //Reset_Chart_Total(Sell_Cnt_1, Sell_Cnt_2);

            cls_form_Meth cm = new cls_form_Meth();

            object[] row0 = { ""
                                ,"<< " + cm._chang_base_caption_search("합계") + " >>"
                                ,""
                                ,""
                                ,Sum_TotalPrice

                                ,Sum_TotalInputPrice
                                ,Sum_PV
                                ,Sum_CV
                                ,""
                                ,""

                                , Sum_InputCash
                                , Sum_InputCard
                                , Sum_InputPassbook
                                , Sum_InputPassbook_2
                                , Sum_InputCoupon

                                , Sum_InputMile
                                , Sum_InputNaver
                                , Sum_TH_8
                                , Sum_TH_9
                                , Sum_TH_10

                                ,Sum_UnaccMoney
                                ,""
                                ,""
                                ,""
                            };

            gr_dic_text[fi_cnt + 2] = row0;

            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();

            //
            int FFCnt = 0;
            foreach (string t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D")
                {
                    if (SalesDetail[t_key].INS_Num.Contains("재발급요청요망") == true)
                    {
                        cgb.basegrid.Rows[FFCnt].DefaultCellStyle.BackColor = System.Drawing.Color.PaleGoldenrod;

                    }

                    FFCnt++;
                }

            }
        }


        private void Set_gr_dic(ref Dictionary<int, object[]> gr_dic_text, string t_key, int fi_cnt)
        {
            object[] row0 = { SalesDetail[t_key].SellTFName
                                ,SalesDetail[t_key].INS_Num
                                ,SalesDetail[t_key].OrderNumber
                                ,SalesDetail[t_key].SellDate

                                ,SalesDetail[t_key].TotalPrice
                                ,SalesDetail[t_key].TotalInputPrice
                                ,SalesDetail[t_key].TotalPV
                                ,SalesDetail[t_key].TotalCV
                                ,SalesDetail[t_key].SellCodeName
                                ,SalesDetail[t_key].ReturnTFName

                                ,SalesDetail[t_key].InputCash
                                ,SalesDetail[t_key].InputCard
                                ,SalesDetail[t_key].InputPassbook
                                ,SalesDetail[t_key].InputPassbook_2
                                ,SalesDetail[t_key].InputCoupon

                                ,SalesDetail[t_key].InputMile
                                ,SalesDetail[t_key].InputNaver
                                ,SalesDetail[t_key].InputPayment_8_TH
                                ,SalesDetail[t_key].InputPayment_9_TH
                                ,SalesDetail[t_key].InputPayment_10_TH

                                ,SalesDetail[t_key].UnaccMoney
                                ,SalesDetail[t_key].RecordID
                                ,SalesDetail[t_key].RecordTime
                                ,""
                                ,SalesDetail[t_key].Associated_Card
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Header_Reset()
        {
            cgb.Grid_Base_Arr_Clear();
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_col_Count = 25;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {
                "승인여부"      , "공제번호"   , "주문번호"       , "주문일자"      , "총주문액"
              , "총입금액"      , "총PV"       , "총CV"           , "주문종류"      , "구분"
              , "현금"          , "카드"       , "무통장입금"     , "가상계좌"      , "쿠폰"
              , "마일리지"      , "네이버페이" , "프롬프트페이_TH", "온라인뱅킹_TH" , "모바일뱅킹_TH"
              , "미결제"        ,  "기록자"    , "기록일"         ,""               ,""
            };

            bool Is_NaCode_TH = cls_User.gid_CountryCode == "TH";
            int Witdh_TH_Payment = Is_NaCode_TH ? 80 : 0;

            int Witdh_Mile = cls_app_static_var.Using_Mileage_TF != 0 ? 80 : 0;
            int Witdh_Naver = cls_User.gid_CountryCode == "KR" ? 80 : 0;

            if (cls_app_static_var.Sell_Union_Flag == "")  //직판특판이 아닌경우 공제번호 필드 안나오게
            {

                int[] g_Width = {   80 ,  0 , 120 , 0  , 0
                                  , 80 , 80 ,  80 , 80 , 80
                                  , 80 , 80 ,  80 , 80 , 80
                                  , Witdh_Mile , Witdh_Naver, Witdh_TH_Payment, Witdh_TH_Payment, Witdh_TH_Payment
                                  , 80 , 80 ,80 , 0, 0
                                };
                cgb.grid_col_w = g_Width;
            }
            else
            {

                int[] g_Width = {   80, 120, 120, 80, 80
                                  , 80,  80,  80, 80, 80
                                  , 80,  80,  80, 80, 80
                                  , Witdh_Mile , Witdh_Naver, Witdh_TH_Payment, Witdh_TH_Payment, Witdh_TH_Payment
                                  , 80 , 80 ,80 , 0, 0
                                };
                cgb.grid_col_w = g_Width;
            }

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleRight  //5   

                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter//10

                               
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight //15
                                  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight

                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
            };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[12 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[13 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[14 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[15 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[16 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[17 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[18 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[19 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[20 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb.grid_col_header_text = g_HeaderText;
            cgb.grid_cell_format = gr_dic_cell_format;

            cgb.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            cgb.basegrid.RowHeadersVisible = false;
        }
        //////SalesDetail___SalesDetail__SalesDetail__SalesDetail__SalesDetail__SalesDetail
        //////SalesDetail___SalesDetail__SalesDetail__SalesDetail__SalesDetail__SalesDetail





        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        private void Base_Sub_Sum_Item()
        {
            if (SalesItemDetail == null)
            {
                txt_TotalPrice.Text = "0";
                txt_TotalPv.Text = "0";
                txt_TotalCV.Text = "0";
                return;
            }

            int fi_cnt = 0; double T_Pv = 0; double T_pr = 0; double T_Cv = 0;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    T_Pv = T_Pv + SalesItemDetail[t_key].ItemTotalPV;
                    T_pr = T_pr + SalesItemDetail[t_key].ItemTotalPrice;
                    T_Cv = T_Cv + SalesItemDetail[t_key].ItemTotalCV;
                }
                fi_cnt++;
            }


            txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, T_pr);
            txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, T_Pv);
            txt_TotalCV.Text = string.Format(cls_app_static_var.str_Currency_Type, T_Cv);


            //if (txt_OrderNumber.Text == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    if (T_pr < cls_app_static_var.Delivery_Standard_TH)
                    {
                        txt_InputPass_Pay.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge_TH);
                    }
                    else
                        txt_InputPass_Pay.Text = "0";
                }
                else
                {
                    if (T_pr < cls_app_static_var.Delivery_Standard)
                    {
                        txt_InputPass_Pay.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge);
                    }
                    else
                        txt_InputPass_Pay.Text = "0";
                }


                if (txt_Receive_Method_Code.Text == "1") //직접수령건이다.
                    txt_InputPass_Pay.Text = "0";

            }
        }



        private void Item_Grid_Set()
        {
            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            int S_cnt4 = 0; double S_cnt5 = 0; double S_cnt6 = 0; double S_cnt7 = 0;
            double S_cnt8 = 0; double S_cnt9 = 0; double S_cnt10 = 0;
            string mp;
            int mp_count = 0;
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    Set_gr_Item(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.

                    S_cnt4 = S_cnt4 + SalesItemDetail[t_key].ItemCount;
                    S_cnt5 = S_cnt5 + SalesItemDetail[t_key].ItemPrice;
                    S_cnt6 = S_cnt6 + SalesItemDetail[t_key].ItemPV;
                    S_cnt7 = S_cnt7 + SalesItemDetail[t_key].ItemCV;
                    S_cnt8 = S_cnt8 + SalesItemDetail[t_key].ItemTotalPrice;
                    S_cnt9 = S_cnt9 + SalesItemDetail[t_key].ItemTotalPV;
                    S_cnt10 = S_cnt10 + SalesItemDetail[t_key].ItemTotalCV;

                    string Tsql;
                 
                    Tsql = "Select MP_YN FROM TBL_GOODS WHERE NCODE =  '" + SalesItemDetail[t_key].ItemCode+ "'";

                    cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                    DataSet ds = new DataSet();
                    //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                    if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;

                    int ReCnt = Temp_Connect.DataSet_ReCount;

                    if (ReCnt >= 1)
                    {
                        mp = ds.Tables["tbl_SalesDetail"].Rows[0][0].ToString();
                        if (mp == "Y")
                        {
                            mp_count = mp_count + 1;
                        }
                    }
                    }
                fi_cnt++;
            }
            //20211012 주문아이템중 MP아이템이 하나라도 들어가면이 아니고, 모두가 MP아이템이면 발동
            //if (mp_count >0)
            if (mp_count == fi_cnt)
            {
                MP_YN = "Y";
            }
            else
            {
                MP_YN = "N";
            }
            txt_SumCnt.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt4);
            txt_SumPr.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt8);
            txt_SumPV.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt9);
            txt_SumCV.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt10);

            //if (S_cnt4 != 0 || S_cnt5 != 0 || S_cnt6 != 0 || S_cnt7 != 0 || S_cnt8 != 0)
            //{
            //    cls_form_Meth cm = new cls_form_Meth();

            //    object[] row0 = { ""
            //                    ,"<< " + cm._chang_base_caption_search("합계") + " >>"
            //                    ,""
            //                    ,S_cnt4
            //                    ,S_cnt5

            //                    ,S_cnt6
            //                    ,S_cnt7
            //                    ,S_cnt8
            //                    ,""
            //                    ,""
            //                     };

            //    gr_dic_text[fi_cnt + 2] = row0;
            //}

            cgb_Item.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Item.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Item(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt)
        {
            object[] row0 = { SalesItemDetail[t_key].SalesItemIndex
                                ,SalesItemDetail[t_key].ItemCode
                                ,SalesItemDetail[t_key].ItemName
                                 ,SalesItemDetail[t_key].ItemCount
                                ,SalesItemDetail[t_key].ItemPrice
                                ,SalesItemDetail[t_key].ItemPV
                                //20230314구현호 BV값
                                 ,SalesItemDetail[t_key].ItemCV

                               
                                ,SalesItemDetail[t_key].ItemTotalPrice
                                ,SalesItemDetail[t_key].ItemTotalPV
                                ,SalesItemDetail[t_key].ItemTotalCV
                                ,SalesItemDetail[t_key].SellStateName
                                ,SalesItemDetail[t_key].Etc
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }

        //20230313 구현호 여기다
        private void dGridView_Base_Item_Header_Reset()
        {
            cgb_Item.Grid_Base_Arr_Clear();
            cgb_Item.basegrid = dGridView_Base_Item;
            cgb_Item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Item.grid_col_Count = 12;
            cgb_Item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {
                ""       , "상품_코드"   , "상품명"    , "주문_수량"    , "개별단가"   , "개별PV"
              , "개별CV"  , "총상품액"    , "총상품PV"   , "총상품CV"
              , "구분"   , "_비고"
                                };

            string[] g_ColsName = {"Col1"  , "ItemCode"   , "ItemName"  , "ItemCount" , "ItemPrice"   , "ItemPV"
                                , "ItemBV"   , "ItemTotalPrice"    , "ItemTotalPV"  , "ItemTotalBV"
                                , "Gubun" , "Etc"
                                };

            int[] g_Width = { 0, 90, 160, 80, 80, 70
                             ,90 , 80 , 80, 80
                             , 70  , 0
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleLeft
                                  ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight  //5    
  
                                ,DataGridViewContentAlignment.MiddleRight  //6    
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight

                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleLeft
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;


            cgb_Item.grid_col_header_text = g_HeaderText;
            cgb_Item.grid_col_name = g_ColsName;
            cgb_Item.grid_cell_format = gr_dic_cell_format;
            cgb_Item.grid_col_w = g_Width;
            cgb_Item.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = {
                true , true,  true,  true, true
               ,true , true,  true,  true, true
               ,true , true
                                   };
            cgb_Item.grid_col_Lock = g_ReadOnly;

            cgb_Item.basegrid.RowHeadersVisible = false;
        }
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail



        //////Sales_Cacu___Sales_Cacu__Sales_Cacu__Sales_Cacu
        //////Sales_Cacu___Sales_Cacu__Sales_Cacu__Sales_Cacu

        private void Base_Sub_Sum_Cacu()
        {
            double Sell_Pr = 0;

            double.TryParse(txt_TotalPrice.Text.Trim().Replace(",", ""), out Sell_Pr);
            if (Sales_Cacu == null)
            {
                txt_TotalInputPrice.Text = "0";
                txt_UnaccMoney.Text = txt_TotalPrice.Text.Trim();
                return;
            }

            double T_pr = 0;

            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D")
                {
                    T_pr = T_pr + Sales_Cacu[t_key].C_Price1;
                }
            }


            txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, T_pr);

            if (txt_InputPass_Pay.Text == "")
                txt_InputPass_Pay.Text = "0";

            double InputPass_Pay = double.Parse(txt_InputPass_Pay.Text.Replace(",", ""));

            //총 제품금액 + 배송비 - 결제한 금액을 빼게 되면 ... 미수금이 나온다.
            txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, (Sell_Pr + InputPass_Pay) - T_pr);

        }

        private string Cacu_Grid_Set_2()
        {
            string Tsql;
            string SuccessYN = "Y";
            Tsql = "Select MP_YN FROM TBL_GOODS (NOLOCK) WHERE NCODE =  '" + txt_ItemCode.Text + "'";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            //if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
            Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text);
            string MP_YN = ds.Tables["tbl_salesdetail"].Rows[0][0].ToString();


            //dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            double S_cnt6 = 0; double S_cnt7 = 0; double S_cnt8 = 0; double S_cnt9 = 0; double S_cnt10 = 0;
            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D")
                {
                    //Set_gr_Cacu(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                    //if (Sales_Cacu[t_key].C_TF == 1) //현금
                    //    S_cnt6 = S_cnt6 + Sales_Cacu[t_key].C_Price1;
                    //if (Sales_Cacu[t_key].C_TF == 2) //무통장
                    //    S_cnt8 = S_cnt8 + Sales_Cacu[t_key].C_Price1;
                    //if (Sales_Cacu[t_key].C_TF == 3) //카드
                    //    S_cnt7 = S_cnt7 + Sales_Cacu[t_key].C_Price1;

                    //if (Sales_Cacu[t_key].C_TF == 4) //마일리지
                    //    S_cnt9 = S_cnt9 + Sales_Cacu[t_key].C_Price1;

                    //if (Sales_Cacu[t_key].C_TF == 5) //가상계좌
                    //    S_cnt8 = S_cnt8 + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 6) //쿠폰
                    {
                        if (MP_YN == "Y")
                        {
                            //S_cnt10 = S_cnt10 + Sales_Cacu[t_key].C_Price1;
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                MessageBox.Show("It is an MP item.");
                            }
                            else
                            {
                                MessageBox.Show("MP아이템입니다.");
                            }
                            SuccessYN =  "N";
                        }
                        else
                        {
                       
                        }
                    }
                }
                fi_cnt++;
            }
            return SuccessYN;
            //txt_SumCash.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt6);
            //txt_SumCard.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt7);
            //txt_SumBank.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt8);
            //txt_SumMile.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt9);

            //txt_SumCoupon.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt10);

            //cgb_Cacu.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            //cgb_Cacu.db_grid_Obj_Data_Put();
        }
            private void Cacu_Grid_Set()
        {

            

            dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Cacu.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            double S_cnt6 = 0; double S_cnt7 = 0; double S_cnt8 = 0; double S_cnt9 = 0; double S_cnt10 = 0;
            double InputNaver = 0, InputPayment_8_TH = 0, InputPayment_9_TH = 0, InputPayment_10_TH = 0;
            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D")
                {
                    Set_gr_Cacu(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                    if (Sales_Cacu[t_key].C_TF == 1) //현금

                        S_cnt6 = S_cnt6 + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 2) //무통장

                        S_cnt8 = S_cnt8 + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 3) //카드
                        S_cnt7 = S_cnt7 + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 4) //마일리지
                        S_cnt9 = S_cnt9 + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 5) //가상계좌
                        S_cnt8 = S_cnt8 + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 6) //쿠폰
                            S_cnt10 = S_cnt10 + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 7)
                        InputNaver += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 8)
                        InputPayment_8_TH += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 9)
                        InputPayment_9_TH += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 10)
                        InputPayment_10_TH += Sales_Cacu[t_key].C_Price1;

                }
                fi_cnt++;
            }

            txt_SumCash.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt6);
            txt_SumCard.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt7);
            txt_SumBank.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt8);
            txt_SumMile.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt9);

            txt_SumCoupon.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt10);

            cgb_Cacu.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Cacu.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Cacu(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt)
        {
            object[] row0 = { Sales_Cacu[t_key].C_index
                                ,Sales_Cacu[t_key].C_TF_Name
                                ,Sales_Cacu[t_key].C_Price1
                                ,Sales_Cacu[t_key].C_AppDate1
                                ,Sales_Cacu[t_key].C_CodeName

                                ,Sales_Cacu[t_key].C_Number1
                                ,Sales_Cacu[t_key].C_Name1
                                ,Sales_Cacu[t_key].C_Name2
                                ,Sales_Cacu[t_key].C_Etc
                                ,""

                                ,Sales_Cacu[t_key].C_Coupon
                                ,Sales_Cacu[t_key].C_Installment_Period
                                ,Sales_Cacu[t_key].C_CVC
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Cacu_Header_Reset()
        {
            cgb_Cacu.Grid_Base_Arr_Clear();
            cgb_Cacu.basegrid = dGridView_Base_Cacu;
            cgb_Cacu.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Cacu.grid_col_Count = 13;
            cgb_Cacu.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "결제방법"   , "결제액"  , "결제일자"   , "카드_은행명"
                                , "카드_은행번호"   , "카드소유자_입금자"    , ""  , "_비고" , "" 
                                , "쿠폰번호", "할부_기간", "카드보안코드"

                                };
            string[] g_Cols = {"C_index"  , "C_TF_Name"   , "C_Price1"  , "C_AppDate1"   , "C_CodeName"
                                , "C_Number1"   , "C_Name1"    , "C_Name2"  , "C_Etc" , "TempCol1" 
                                ,"C_Coupon", "Installment_Period", "C_CVC"
                                };

            int[] g_Width = { 0, 90, 70, 90, 100
                                ,120 , 100 , 0 , 0 , 0
                                ,100, 80, 80
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter  //5    
  
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter  //10

                                ,DataGridViewContentAlignment.MiddleCenter  //11
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Cacu.grid_col_header_text = g_HeaderText;
            cgb_Cacu.grid_cell_format = gr_dic_cell_format;
            cgb_Cacu.grid_col_w = g_Width;
            cgb_Cacu.grid_col_name = g_Cols;
            cgb_Cacu.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true
                                   };
            cgb_Cacu.grid_col_Lock = g_ReadOnly;

            cgb_Cacu.basegrid.RowHeadersVisible = false;
        }
        //////Sales_Cacu___Sales_Cacu__Sales_Cacu__Sales_Cacu
        //////Sales_Cacu___Sales_Cacu__Sales_Cacu__Sales_Cacu





        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece
        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece
        private void Rece_Grid_Set()
        {
            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;

            if (cls_app_static_var.Rec_info_Multi_TF == 0)
            {
                foreach (int t_key in Sales_Rece.Keys)
                {
                    if (Sales_Rece[t_key].Del_TF != "D")
                        Set_gr_Rece(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                    fi_cnt++;
                }
            }
            else
            {

                foreach (int t_key in Sales_Rece.Keys)
                {
                    if (Sales_Rece[t_key].Del_TF != "D")
                    {
                        Set_gr_Rece(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                        break;
                    }
                    fi_cnt++;
                }
            }
            cgb_Rece.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Rece.db_grid_Obj_Data_Put();

            if(dGridView_Base_Rece.Rows.Count > 0)
            {
                dGridView_Base_Rece.Rows[0].Selected = true;
                dGridView_Base_Sub_DoubleClick(dGridView_Base_Rece, null);
            }
        }



        private void Set_gr_Rece(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt)
        {
            object[] row0 = { Sales_Rece[t_key].SalesItemIndex
                                ,Sales_Rece[t_key].Receive_Method_Name
                                ,Sales_Rece[t_key].Get_Date1
                                ,Sales_Rece[t_key].Get_Name1
                                ,Sales_Rece[t_key].Get_ZipCode

                                ,Sales_Rece[t_key].Get_Address1
                                ,Sales_Rece[t_key].Get_Address2
                                ,Sales_Rece[t_key].Get_Tel1
                                ,Sales_Rece[t_key].Get_Tel2
                                ,Sales_Rece[t_key].Get_Etc1

                                ,t_key
                                ,Sales_Rece[t_key].Pass_Number
                                ,Sales_Rece[t_key].Get_city     // 태국 도시
                                ,Sales_Rece[t_key].Get_state    // 태국 주
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Rece_Header_Reset()
        {
            cgb_Rece.Grid_Base_Arr_Clear();
            cgb_Rece.basegrid = dGridView_Base_Rece;
            cgb_Rece.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Rece.grid_col_Count = 14;
            cgb_Rece.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "배송구분"   , "배송일"  , "수령인"   , "우편_번호"
                                , "주소1"   , "주소2"    , "연락처_1"  , "연락처_2" , "비고"
                                ,"_t_key" , "운송장번호", "주", "도시"
                                };
            string[] g_ColsName = {"SalesItemIndex"  , "Receive_Method_Name"   , "Date1"  , "Name1"   , "ZipCode"
                                , "Address1"   , "Address2"    , "Tel1"  , "Tel2" , "Etc1"
                                ,"t_key", "Passnumber", "state", "city"
                                };

            int[] g_Width;
            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                g_Width = new int[] { 0, 90, 0, 90, 100
                                ,120 , 100 , 90 , 150 , 200
                                ,0  ,80, 80 , 80
                            };
            }
            // 그 외 국가인 경우
            else
            {
                g_Width = new int[] { 0, 90, 0, 90, 100
                                ,120 , 100 , 90 , 150 , 200
                                ,0  ,80, 0 , 0
                            };
            }


            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter  //5    
  
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter  //10

                                ,DataGridViewContentAlignment.MiddleCenter  //t_key
                                ,DataGridViewContentAlignment.MiddleLeft //Passsnumber
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Rece.grid_col_header_text = g_HeaderText;
            cgb_Rece.grid_cell_format = gr_dic_cell_format;
            cgb_Rece.grid_col_w = g_Width;
            cgb_Rece.grid_col_alignment = g_Alignment;
            cgb_Rece.grid_col_name = g_ColsName;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true,  true,  true
                                   };
            cgb_Rece.grid_col_Lock = g_ReadOnly;

            cgb_Rece.basegrid.RowHeadersVisible = false;
        }
        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece
        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece





        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        private void Rece_Item_Grid_Set(int Recindex = 0)
        {
            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0; string V_Check = "";
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (Recindex == 0)//삭제되지 않고 배송 정보가 없는 내역들만 뿌려준다.
                {
                    V_Check = "V";
                    if (SalesItemDetail[t_key].Del_TF != "D" && SalesItemDetail[t_key].RecIndex == 0)
                        Set_gr_Rece_Item(ref gr_dic_text, t_key, fi_cnt, V_Check);  //데이타를 배열에 넣는다.
                }
                else
                {
                    if (SalesItemDetail[t_key].SalesItemIndex == Recindex)
                    {
                        V_Check = "V";
                        Set_gr_Rece_Item(ref gr_dic_text, t_key, fi_cnt, V_Check);  //데이타를 배열에 넣는다.
                    }
                }

                fi_cnt++;
            }

            cgb_Rece_Item.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Rece_Item.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Rece_Item(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt, string V_Check = "")
        {
            object[] row0 = { V_Check
                                ,SalesItemDetail[t_key].SalesItemIndex
                                ,SalesItemDetail[t_key].ItemCode
                                ,SalesItemDetail[t_key].ItemName
                                ,SalesItemDetail[t_key].ItemCount

                                ,SalesItemDetail[t_key].Etc
                                ,""
                                ,""
                                ,""
                                ,""
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Rece_Item_Header_Reset()
        {
            cgb_Rece_Item.Grid_Base_Arr_Clear();
            cgb_Rece_Item.basegrid = dGridView_Base_Rece_Item;
            cgb_Rece_Item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Rece_Item.grid_col_Count = 10;

            string[] g_HeaderText = {"선택"  , ""   , "상품_코드"  , "상품명"   , "주문_수량"
                                , "비고"   , ""    , ""  , "" , ""
                                };

            int[] g_Width = { 30, 0, 60, 150, 60
                                ,200 , 0 , 0 , 0 , 0
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleLeft  //5    
  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleLeft  //10
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Rece_Item.grid_col_header_text = g_HeaderText;
            cgb_Rece_Item.grid_cell_format = gr_dic_cell_format;
            cgb_Rece_Item.grid_col_w = g_Width;
            cgb_Rece_Item.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                   };
            cgb_Rece_Item.grid_col_Lock = g_ReadOnly;

            cgb_Rece_Item.basegrid.RowHeadersVisible = false;
        }


        private void dGridView_Base_2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((sender as DataGridView).CurrentCell.ColumnIndex == 0)
            {
                DataGridView T_DGv = (DataGridView)sender;
                if ((T_DGv.CurrentCell.Value == null)
                || (T_DGv.CurrentCell.Value.ToString() == ""))
                {
                    T_DGv.CurrentCell.Value = "V";
                }
                else
                {
                    T_DGv.CurrentCell.Value = "";
                }
            }
        }
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail



        private void MtxtData_Temp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                MaskedTextBox mtb = (MaskedTextBox)sender;

                if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                {
                    Data_Set_Form_TF = 1;
                    int SW = 0;
                    string Sn = mtb.Text.Replace("-", "").Replace("_", "").Trim();
                    string R4_name = mtb.Name.Substring(mtb.Name.Length - 4, 4);
                    if (R4_name == "Date" || R4_name == "ate3" || R4_name == "ate1" || R4_name == "ate2" || R4_name == "ate4")
                    {
                        SW = 1;
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtTel1")
                    {
                        SW = 1;
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtTel2")
                    {
                        SW = 1;
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtZip1")
                    {
                        SW = 1;
                        if (Sn_Number_(Sn, mtb, "Zip") == true)
                            SendKeys.Send("{TAB}");
                    }

                    Data_Set_Form_TF = 0;
                }
                else
                    SendKeys.Send("{TAB}");


            }
        }




        private bool Sn_Number_(string Sn, MaskedTextBox mtb, string sort_TF, int t_Sort2 = 0)
        {
            if (Sn != "")
            {

                bool check_b = false;
                cls_Sn_Check csn_C = new cls_Sn_Check();

                //sort_TF = "biz";  //사업자번호체크
                //sort_TF = "Tel";  //전화번호체크
                //sort_TF = "Zip";  //우편번호체크

                if (sort_TF == "Date")
                {
                    cls_Check_Input_Error c_er = new cls_Check_Input_Error();
                    if (c_er.Input_Date_Err_Check__01(mtb) == false)
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Date")
                           + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        mtb.Focus(); return false;
                    }
                }


                check_b = csn_C.Number_NotInput_Check(mtb.Text, sort_TF);

                if (check_b == false)
                {
                    if (sort_TF == "biz")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_BuNum")
                           + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }

                    if (sort_TF == "Tel")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Tel")
                           + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }

                    if (sort_TF == "Zip")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_AddCode")
                           + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }

                    if (sort_TF == "Date")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Date")
                           + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }

                    mtb.Focus(); return false;
                }
            }

            return true;
        }





        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13_tb += new Key_13_tb_Event_Handler(T_R_Key_Enter_13_tb);
            T_R.Key_Enter_13_Ncode += new Key_13_Ncode_Event_Handler(T_R_Key_Enter_13_Ncode);
            T_R.Key_Enter_13_Name += new Key_13_Name_Event_Handler(T_R_Key_Enter_13_Name);
            TextBox tb = (TextBox)sender;

            if ((tb.Tag == null) || (tb.Tag.ToString() == ""))
            {
                //쿼리문상 오류를 잡기 위함.
                if (T_R.Text_KeyChar_Check(e, tb, tb) == false)
                {
                    e.Handled = true;
                    return;
                } // end if   
            }
            else if ((tb.Tag != null) && (tb.Tag.ToString() == "1"))
            {
                //숫자만 입력 가능
                if (T_R.Text_KeyChar_Check(e, tb, 1) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

            else if ((tb.Tag != null) && (tb.Tag.ToString() == "2"))
            {
                //숫자만 입력 가능
                if (T_R.Text_KeyChar_Check(e, tb, 1) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

            else if ((tb.Tag != null) && (tb.Tag.ToString() == "-"))
            {
                //숫자와  - 만
                if (T_R.Text_KeyChar_Check(e, tb, "-") == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }


            else if ((tb.Tag != null) && (tb.Tag.ToString() == "ncode")) //코드관련해서 코드를치면 관련 내역이 나오도록 하기 위함.
            {

                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, tb) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

            else if (tb.Tag.ToString() == "name")  //회원 정보 관련해서 이름 검색을 필요로 하는 텍스트 박스이다.
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(tb, e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }



        }

        void T_R_Key_Enter_13_tb(string txt_tag, TextBox tb)
        {

            Data_Set_Form_TF = 1;
            if (tb.Name == "txt_Price_3")
            {
                if (tb.Text != "")
                    tb.Text = string.Format(cls_app_static_var.str_Currency_Type, double.Parse(tb.Text.Replace(",", "")));

                if (txt_Price_3_2.Text == "")
                    txt_Price_3_2.Text = tb.Text.Trim();

                if (mtxtPriceDate3.Text.Replace("-", "").Trim() == "")
                    mtxtPriceDate3.Text = mtxtSellDate.Text;
            }

            if (tb.Name == "txt_Price_2")
            {
                if (tb.Text != "")
                    tb.Text = string.Format(cls_app_static_var.str_Currency_Type, double.Parse(tb.Text.Replace(",", "")));
                if (mtxtPriceDate2.Text.Replace("-", "").Trim() == "")
                    mtxtPriceDate2.Text = mtxtSellDate.Text;
            }

            if (tb.Name == "txt_Price_1")
            {
                if (tb.Text != "")
                    tb.Text = string.Format(cls_app_static_var.str_Currency_Type, double.Parse(tb.Text.Replace(",", "")));

                if (mtxtPriceDate1.Text.Replace("-", "").Trim() == "")
                    mtxtPriceDate1.Text = mtxtSellDate.Text;
            }


            if (tb.Name == "txt_Price_4")
            {
                if (tb.Text != "")
                    tb.Text = string.Format(cls_app_static_var.str_Currency_Type, double.Parse(tb.Text.Replace(",", "")));

                if (mtxtPriceDate4.Text.Replace("-", "").Trim() == "")
                    mtxtPriceDate4.Text = mtxtSellDate.Text;
            }

            if (tb.Name == "txt_Price_6")
            {
                if (tb.Text != "")
                    tb.Text = string.Format(cls_app_static_var.str_Currency_Type, double.Parse(tb.Text.Replace(",", "")));

                if (mtxtPriceDate6.Text.Replace("-", "").Trim() == "")
                    mtxtPriceDate6.Text = mtxtSellDate.Text;
            }


            if (tb.Name == "txt_ETC1")
                txt_ItemCode.Focus();
            else
                SendKeys.Send("{TAB}");

            Data_Set_Form_TF = 0;
        }



        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            int Sw_Tab = 0;

            if ((sender is TextBox) == false) return;

            TextBox tb = (TextBox)sender;
            //if (tb.TextLength >= tb.MaxLength)
            //{
            //    SendKeys.Send("{TAB}");
            //    Sw_Tab = 1;
            //}


            if (tb.Name == "txtCenter2")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                txtCenter2_Code.Text = "";
                Data_Set_Form_TF = 0;
               

            }
            if (tb.Name == "txtCenter3")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtCenter3_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtSellCode")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                {
                    txtSellCode_Code.Text = "";
                }
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_Base_Rec")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txt_Base_Rec_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

            if(tb.Name == "txt_Price_3")
            {
                txt_Price_3_2.Text = txt_Price_3.Text;
            }

            if (tb.Name == "txt_Receive_Method")


            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                {
                    txt_Receive_Method_Code.Text = "";
                    dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                    cgb_Rece_Item.d_Grid_view_Header_Reset();
                    Data_Set_Form_TF = 0;
                }
                else
                {
                    if (SalesItemDetail != null && txt_Receive_Method_Code.Text != "")
                        Rece_Item_Grid_Set();

                    Data_Set_Form_TF = 0;
                    if (tb.Text.Trim() != "2" && tb.Text.Trim() != "배송")
                    {
                        txt_InputPass_Pay.Text = "0";


                        if (tb.Text.Trim() == "센타수령" || tb.Text.Trim() == "3")
                        {
                            //Rec_Center_Change();
                            Input_Center_Address();

                        }

                        else if (tb.Text.Trim() == "직접수령" || tb.Text.Trim() == "1")
                        {
                            mtxtZip1.Text = "";
                            txtAddress1.Text = "";
                            txtAddress2.Text = "";
                            pnlZipCode_KR.Enabled = false;
                            tableLayoutPanel42.Enabled = false;
                            tableLayoutPanel37.Enabled = false;
                            //tableLayoutPanel81.Visible = true;
                            if (cls_User.gid_CenterCode != "")
                            {
                                string Tsql = "";
                                Tsql = "Select Name ";
                                Tsql = Tsql + " From tbl_Business (nolock) ";
                                Tsql = Tsql + " Where ncode = '" + cls_User.gid_CenterCode + "' ";
                                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                                DataSet ds = new DataSet();
                                if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Business", ds) == false) return;
                                int ReCnt = Temp_Connect.DataSet_ReCount;

                                if (ReCnt > 0)
                                {
                                    txtCenter3.Text = ds.Tables["tbl_Business"].Rows[0]["Name"].ToString();
                                }
                                txtCenter3_Code.Text = cls_User.gid_CenterCode;
                            }

                        }
                        else
                        {
                            pnlZipCode_KR.Enabled = true;
                            tableLayoutPanel42.Enabled = true;
                            tableLayoutPanel37.Enabled = true;
                           // txtCenter3.Text = "";
                           // txtCenter3_Code.Text = "";
                            //tableLayoutPanel81.Visible = false;
                        }



                    }
                    //else if (tb.Text.Trim() != "배송")
                    //{
                    //    txt_InputPass_Pay.Text = "0";

                    //    if (tb.Text.Trim() == "센타수령")
                    //    {
                    //        //Rec_Center_Change();
                    //        Input_Center_Address();

                    //    }

                    //}
                    else
                    {
                        //if (txt_OrderNumber.Text == "")
                        //{
                        //    //if (txt_SumPr.Text != "")
                        //    //{
                        //    //    if (double.Parse(txt_SumPr.Text.Replace(",", "")) < 50000)
                        //    //        txt_InputPass_Pay.Text = "2,500";
                        //    //    else
                        if (txt_SumPr.Text != "")
                        {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                if (double.Parse(txt_SumPr.Text.Replace(",", "")) < cls_app_static_var.Delivery_Standard_TH)
                                {
                                    txt_InputPass_Pay.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge_TH);
                                }
                                else
                                    txt_InputPass_Pay.Text = "0";
                            }
                            else
                            {
                                if (double.Parse(txt_SumPr.Text.Replace(",", "")) < cls_app_static_var.Delivery_Standard)
                                {
                                    txt_InputPass_Pay.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge);
                                }
                                else
                                    txt_InputPass_Pay.Text = "0";
                            }

                        }
                        else

                            txt_InputPass_Pay.Text = "";
                        //}
                    }
                }

            }


            if (tb.Name == "txt_ItemCode")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txt_ItemName.Text = "";
                Data_Set_Form_TF = 0;
            }


            if (tb.Name == "txt_C_Bank")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                {
                    txt_C_Bank_Code.Text = "";
                    txt_C_Bank_Code_2.Text = "";
                    txt_C_Bank_Code_3.Text = "";
                }
                Data_Set_Form_TF = 0;
                //else if (Sw_Tab == 1)
                //{
                //    if (Ncode_dic != null)
                //        Ncode_dic.Clear();
                //    Ncode_dic["BankPenName"] = tb;
                //    Ncode_dic["BankCode"] = txt_C_Bank_Code;
                //    Ncode_dic["BankName"] = txt_C_Bank_Code_2;
                //    Ncode_dic["BankAccountNumber"] = txt_C_Bank_Code_3;
                //    Ncod_Text_Set_Data(tb);
                //}
            }

            if (tb.Name == "txt_C_Card")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Replace("   ", "").Trim() == "")
                {
                    txt_C_Card_Code.Text = "";
                }
                Data_Set_Form_TF = 0;
                //else if (Sw_Tab == 1)
                //{
                //    if (Ncode_dic != null)
                //        Ncode_dic.Clear();
                //    Ncode_dic["ncode"] = tb;
                //    Ncode_dic["cardname"] = txt_C_Card_Code;                    
                //    Ncod_Text_Set_Data(tb);
                //}
            }



        }


        void T_R_Key_Enter_13_Name(string txt_tag, TextBox tb)
        {
            if (txt_tag != "")
            {
                int reCnt = 0;
                cls_Search_DB cds = new cls_Search_DB();
                string Search_Mbid = "";
                reCnt = cds.Member_Name_Search_S_N(ref Search_Mbid, txt_tag);

                if (reCnt == 1)
                {
                    if (tb.Name == "txtName")
                    {
                        mtxtMbid.Text = Search_Mbid; //회원명으로 검색해서 나온 사람이 한명일 경우에는 회원번호를 넣어준다.                    
                        if (Input_Error_Check(mtxtMbid, "m") == true)
                            Set_Form_Date(mtxtMbid.Text, "m");
                        Coupon_Grid_Set();
                    }
                }
                else if (reCnt != 1)  //동명이인이 존재해서 사람이 많을 경우나 또는 이름 없이 엔터친 경우에.
                {

                    //cls_app_static_var.Search_Member_Name = txt_tag;
                    frmBase_Member_Search e_f = new frmBase_Member_Search();
                    if (tb.Name == "txtName")
                    {
                        e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                        e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemName_Info);
                    }

                    e_f.ShowDialog();

                    SendKeys.Send("{TAB}");
                }


            }
            else
                SendKeys.Send("{TAB}");

        }

        void e_f_Send_MemName_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            searchMbid = ""; searchMbid2 = 0;
            seachName = txtName.Text.Trim();
        }



        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {
            if (Base_Error_Check__01(1) == false)
                return;



            if (tb.Name == "txtCenter2")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtCenter2_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtCenter2_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txtCenter2_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;

                if (txtCenter2.Text == "물류센터")
                {
                    txt_Receive_Method.Text = "배송";
                    txt_Receive_Method_Code.Text = "2";
                    txtCenter3.Text = "물류센터";
                    txtCenter3_Code.Text = "50000";
                }
                if (txtCenter2.Text == "서울픽업")
                {
                    txt_Receive_Method.Text = "직접수령";
                    txt_Receive_Method_Code.Text = "1";
                    txtCenter3.Text = "서울픽업";
                    txtCenter3_Code.Text = "50010";
                }
                if (txtCenter2.Text == "부산픽업")
                {
                    txt_Receive_Method.Text = "직접수령";
                    txt_Receive_Method_Code.Text = "1";
                    txtCenter3.Text = "부산픽업";
                    txtCenter3_Code.Text = "50011";
                }
                if (txtCenter2.Text == "인천픽업")
                {
                    txt_Receive_Method.Text = "직접수령";
                    txt_Receive_Method_Code.Text = "1";
                    txtCenter3.Text = "인천픽업";
                    txtCenter3_Code.Text = "50012";
                }
            }

            if (tb.Name == "txtCenter3")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtCenter3_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtCenter2_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txtCenter2_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtSellCode")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtSellCode_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtSellCode_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txtSellCode_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }




            if (tb.Name == "txt_Receive_Method")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txt_Receive_Method_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txt_Receive_Method_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txt_Receive_Method_Code);

                //SendKeys.Send("{TAB}");


                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_Base_Rec")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txt_Base_Rec_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txt_Base_Rec_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txt_Base_Rec_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }
            //20230313 구현호 여기다
            if (tb.Name == "txt_ItemCode")
            {
                Data_Set_Form_TF = 1;

                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                Db_Grid_Popup(tb, txt_ItemName);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txt_ItemName, "");
                //else
                //    Ncod_Text_Set_Data(tb, txt_ItemName);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }


            if (tb.Name == "txtBank")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtBank_Code);
                Data_Set_Form_TF = 0;
            }


            if (tb.Name == "txt_C_Bank")
            {
                if (Ncode_dic != null)
                    Ncode_dic.Clear();
                Ncode_dic["BankPenName"] = tb;
                Ncode_dic["BankCode"] = txt_C_Bank_Code;
                Ncode_dic["BankName"] = txt_C_Bank_Code_2;
                Ncode_dic["BankAccountNumber"] = txt_C_Bank_Code_3;

                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, "");
                else
                    Ncod_Text_Set_Data(tb);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_C_Card")
            {
                if (Ncode_dic != null)
                    Ncode_dic.Clear();

                txt_C_Card_Code.Text = string.Empty;
                Ncode_dic["ncode"] = txt_C_Card_Code;
                Ncode_dic["cardname"] = tb;

                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, "");
                else
                    Ncod_Text_Set_Data(tb);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }


        }



        private void Db_Grid_Popup(TextBox tb, TextBox tb1_Code)
        {
            cls_Grid_Base_Popup cgb_Pop = new cls_Grid_Base_Popup();
            DataGridView Popup_gr = new DataGridView();
            Popup_gr.Name = "Popup_gr";
            this.Controls.Add(Popup_gr);
            cgb_Pop.basegrid = Popup_gr;
            cgb_Pop.Base_fr = this;
            cgb_Pop.Base_tb = tb1_Code;  //앞에게 코드
            cgb_Pop.Base_tb_2 = tb;    //2번은 명임
            cgb_Pop.Base_Location_obj = tb;

            string And_Sql = "";
            if (tb.Name == "txtCenter")
                cgb_Pop.Next_Focus_Control = txt_ETC1;

            if (tb.Name == "txtCenter2")
                cgb_Pop.Next_Focus_Control = txt_Receive_Method;

            if (tb.Name == "txtCenter3")
            {
                cgb_Pop.Next_Focus_Control = txt_ItemCode;
                // 한국인 경우
                if (cls_NationService.GetCountryCodeOrDefault(cls_User.gid_CountryCode) == "KR")
                {
                    And_Sql = " AND LEFT(Ncode, 1) = '5' "; //한국일 때 Ncode 5로 시작하는 것만 보여줌.
                }
            }
            if (tb.Name == "txtBank")
            {
                cgb_Pop.Next_Focus_Control = buttonV_Ok;
                And_Sql = " And VAccountYN = 'Y' ";
            }
            if (tb.Name == "txtR_Id")
                cgb_Pop.Next_Focus_Control = txtCenter2;
            
            if (tb.Name == "txtChange")
                cgb_Pop.Next_Focus_Control = txtCenter2;

            if (tb.Name == "txtSellCode")
                cgb_Pop.Next_Focus_Control = txtCenter2;

            if (tb.Name == "txt_Base_Rec")
                cgb_Pop.Next_Focus_Control = mtxtZip1;

            if (tb.Name == "txt_Receive_Method")
                cgb_Pop.Next_Focus_Control = txtCenter3;

            if (tb.Name == "txt_ItemCode")
            {
                if (txtSellCode_Code.Text == "04")
                {
                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                    if (opt_sell_3.Checked == false)
                    {
                        And_Sql = " AND GeneralSell_YN = 'Y' AND PrimiumYN = 'Y'";
                    }
                    else
                    {
                    
                        //20201105 구현호 일반판매상품과 커스텀팩 상품만 가져온다.
                        //And_Sql = "AND ItemType NOT IN ('3')  AND GeneralSell_YN = 'Y' AND PrimiumYN = 'Y'";
                        //20220329 구현호 일반판매상품과 커스텀팩 상품만 가져온다. 취소한다.
                        And_Sql = "AND GeneralSell_YN = 'Y' AND PrimiumYN = 'Y'";

                    }
                    cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, idx_Na_Code, mtxtSellDate.Text, And_Sql, 1, "");
                }
                else
                {
                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                    if (opt_sell_3.Checked == false)
                    {
                    
                        And_Sql = " AND GeneralSell_YN = 'Y' ";
                    }
                    else
                    {
                        //20201105 구현호 일반판매상품과 커스텀팩 상품이 아닌걸 가져온다.
                        //And_Sql = "AND ItemType NOT IN('3')  AND GeneralSell_YN = 'Y' ";
                        //20201105 구현호 일반판매상품과 커스텀팩 상품이 아닌걸 가져온다. 취소한다.
                        And_Sql = "AND GeneralSell_YN = 'Y' ";
                    }
                    cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, idx_Na_Code, mtxtSellDate.Text, And_Sql, 1, "");
                }
                
            }
            else
            {
                cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, idx_Na_Code, mtxtSellDate.Text, And_Sql);
            }

            if (tb.Name == "txt_Receive_Method")
            {
                if (SalesItemDetail != null && txt_Receive_Method_Code.Text != "")
                    Rece_Item_Grid_Set();
            }
        }


        private void Db_Grid_Popup(TextBox tb, TextBox tb1_Code, string strSql)
        {
            cls_Grid_Base_Popup cgb_Pop = new cls_Grid_Base_Popup();
            DataGridView Popup_gr = new DataGridView();
            Popup_gr.Name = "Popup_gr";
            this.Controls.Add(Popup_gr);
            cgb_Pop.basegrid = Popup_gr;
            cgb_Pop.Base_fr = this;
            cgb_Pop.Base_tb = tb1_Code;  //앞에게 코드
            cgb_Pop.Base_tb_2 = tb;    //2번은 명임
            cgb_Pop.Base_Location_obj = tb;

            if (strSql != "")
            {
                if (tb.Name == "txtCenter2")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);
                    cgb_Pop.Next_Focus_Control = txt_ETC1;
                }

                if (tb.Name == "txtCenter3")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);
                    cgb_Pop.Next_Focus_Control = txt_Get_Name1;
                }

                if (tb.Name == "txtR_Id")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txtSellCode")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", strSql);
                    cgb_Pop.Next_Focus_Control = txtCenter2;
                }

                if (tb.Name == "txt_Base_Rec")
                    cgb_Pop.db_grid_Popup_Base(2, "배송사_코드", "배송사", "ncode", "name", strSql);


                if (tb.Name == "txt_Receive_Method")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "배송_코드", "배송_구분", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, strSql);
                    cgb_Pop.Next_Focus_Control = txt_Get_Name1;
                }

                if (tb.Name == "txt_C_TF")
                    cgb_Pop.db_grid_Popup_Base(2, "결제_코드", "결제_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, strSql);

                if (tb.Name == "txt_ItemCode")
                {
                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV", "Name", "Ncode", "price2", "price4", strSql);
                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                }

            }
            else
            {
                if (tb.Name == "txtCenter2")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
                    if (idx_Na_Code != "") Tsql = Tsql + " And  Na_Code = '" + idx_Na_Code + "'";
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " AND  ShowOrderCenter = 'Y'";
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                    cgb_Pop.Next_Focus_Control = txt_ETC1;
                }

                if (tb.Name == "txtCenter3")
                {
                    string Tsql;
                    Tsql = "Select Ncode , CASE WHEN ISNULL(UP_Ncode,'') <> '' THEN  ISNULL((SELECT TOP 1 Name FROM tbl_Business (NOLOCK) A WHERE A.Ncode = tbl_Business.UP_Ncode), '') + '-' + Name ELSE Name END Name, Delete_YN ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
                    if (idx_Na_Code != "") Tsql = Tsql + " And  Na_Code = '" + idx_Na_Code + "'";
                    if (cls_User.gid_CenterCode != "")
                    {
                        Tsql = Tsql + " And Ncode = '" + cls_User.gid_CenterCode + "' ";
                    }
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " Order by case when UP_Ncode <> '' then UP_Ncode  + Ncode else ncode + '0' end ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                    cgb_Pop.Next_Focus_Control = txt_Get_Name1;
                }

                if (tb.Name == "txtR_Id")
                {
                    string Tsql;
                    Tsql = "Select user_id ,U_Name   ";
                    Tsql = Tsql + " From tbl_User (nolock) ";
                    Tsql = Tsql + " Order by user_id ";

                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", Tsql);
                }

                if (tb.Name == "txtSellCode")
                {
                    string Tsql;
                    Tsql = "Select SellCode ,SellTypeName    ";
                    Tsql = Tsql + " From tbl_SellType (nolock) ";
                    Tsql = Tsql + " Order by SellCode ";

                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", Tsql);
                    cgb_Pop.Next_Focus_Control = txtCenter2;
                }


                if (tb.Name == "txt_Base_Rec")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Base_Rec (nolock) ";
                    if (idx_Na_Code != "") Tsql = Tsql + " Where  Na_Code = '" + idx_Na_Code + "'";
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "배송사_코드", "배송사", "ncode", "name", Tsql);

                }


                if (tb.Name == "txt_C_TF")
                {
                    string Tsql;

                    Tsql = "Select M_Detail , " + cls_app_static_var.Base_M_Detail_Ex;
                    Tsql = Tsql + " From tbl_Base_Change_Detail (nolock) ";
                    Tsql = Tsql + " Where M_Detail_S = 'tbl_Sales_Cacu' ";
                    Tsql = Tsql + " Order by M_Detail ";

                    cgb_Pop.db_grid_Popup_Base(2, "결제_코드", "결제_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, Tsql);
                }


                if (tb.Name == "txt_Receive_Method")
                {
                    string Tsql;

                    Tsql = "Select M_Detail , " + cls_app_static_var.Base_M_Detail_Ex;
                    Tsql = Tsql + " From tbl_Base_Change_Detail (nolock) ";
                    Tsql = Tsql + " Where M_Detail_S = 'tbl_Sales_Rece' ";
                    Tsql = Tsql + " Order by M_Detail ";

                    cgb_Pop.db_grid_Popup_Base(2, "배송_코드", "배송_구분", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, Tsql);
                    cgb_Pop.Next_Focus_Control = txt_Get_Name1;
                }




                if (tb.Name == "txt_ItemCode")
                {
                    string Tsql;
                    Tsql = "Select Name , NCode  ,price2 , price4  ";
                    Tsql += string.Format(" From ufn_Good_Search_Web_Sell_BySellCode ('{0}','{1}','{2}') "
                        , mtxtSellDate.Text.Replace("-", "").Trim()
                        , idx_Na_Code
                        , txtSellCode_Code.Text);
                    Tsql = Tsql + " Where NCode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";
                    if (opt_sell_3.Checked == true)
                    {

                    }
                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV", "Name", "Ncode", "price2", "price4", Tsql);

                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                }


            }
        }


        private void Db_Grid_Popup(TextBox tb, string strSql)
        {

            cls_Grid_Base_Popup cgb_Pop = new cls_Grid_Base_Popup();
            DataGridView Popup_gr = new DataGridView();
            Popup_gr.Name = "Popup_gr";
            this.Controls.Add(Popup_gr);
            cgb_Pop.basegrid = Popup_gr;
            cgb_Pop.Base_fr = this;
            cgb_Pop.Base_text_dic = Ncode_dic;
            cgb_Pop.Base_tb_2 = tb;    //2번은 명임
            cgb_Pop.Base_Location_obj = tb;

            if (strSql != "")
            {
                if (tb.Name == "txt_C_Bank")
                {
                    cgb_Pop.db_grid_Popup_Base(4, "계좌가명", "은행_코드", "은행명", "계좌번호"
                                                , "BankPenName", "BankCode", "BankName", "BankAccountNumber"
                                                , strSql);
                    cgb_Pop.Next_Focus_Control = txt_C_Etc;
                }

                if (tb.Name == "txt_C_Card")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "카드_코드", "카드명"
                                                , "ncode", "CardName"
                                                , strSql);
                    cgb_Pop.Next_Focus_Control = txt_C_Name_3;
                }

            }
            else
            {
                if (tb.Name == "txt_C_Bank")
                {
                    string Tsql;
                    Tsql = "Select BankPenName , BankCode , BankName , BankAccountNumber        ";
                    Tsql = Tsql + " From tbl_BankForCompany (NOLOCK)  ";
                    Tsql = Tsql + " Where (BankPenName like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    BankCode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    BankName like '%" + tb.Text.Trim() + "%')";
                    if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";

                    cgb_Pop.db_grid_Popup_Base(4, "계좌가명", "은행_코드", "은행명", "계좌번호"
                                                , "BankPenName", "BankCode", "BankName", "BankAccountNumber"
                                                , Tsql);

                    cgb_Pop.Next_Focus_Control = txt_C_Etc;
                }


                if (tb.Name == "txt_C_Card")
                {
                    string Tsql;
                    Tsql = "Select  Ncode, cardname   ";
                    Tsql = Tsql + " From tbl_Card (nolock) ";
                    Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    cardname like '%" + tb.Text.Trim() + "%')";
                    if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";

                    cgb_Pop.db_grid_Popup_Base(2, "카드_코드", "카드명"
                                                , "ncode", "CardName"
                                                , Tsql);

                    cgb_Pop.Next_Focus_Control = txt_C_Name_3;

                }
            }
        }



        private void Ncod_Text_Set_Data(TextBox tb, TextBox tb1_Code)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            if (tb.Name == "txtCenter2")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
                if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
            }

            if (tb.Name == "txtCenter3")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
                if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
            }

            if (tb.Name == "txtR_Id")
            {
                Tsql = "Select user_id ,U_Name   ";
                Tsql = Tsql + " From tbl_User (nolock) ";
                Tsql = Tsql + " Where U_Name like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    user_id like '%" + tb.Text.Trim() + "%'";
            }

            if (tb.Name == "txtSellCode")
            {





                Tsql = "Select SellCode ,SellTypeName    ";
                Tsql = Tsql + " From tbl_SellType (nolock) ";
                Tsql = Tsql + " Where SellCode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    SellTypeName like '%" + tb.Text.Trim() + "%'";
            }


            if (tb.Name == "txt_Base_Rec")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Base_Rec (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";
            }

            if (tb.Name == "txt_C_TF")
            {
                Tsql = "Select M_Detail , " + cls_app_static_var.Base_M_Detail_Ex;
                Tsql = Tsql + " From tbl_Base_Change_Detail (nolock) ";
                Tsql = Tsql + " Where M_Detail_S = 'tbl_Sales_Cacu' ";
                Tsql = Tsql + " And  ( M_Detail like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    " + cls_app_static_var.Base_M_Detail_Ex + " like '%" + tb.Text.Trim() + "%')";
            }



            if (tb.Name == "txt_Receive_Method")
            {
                Tsql = "Select M_Detail , " + cls_app_static_var.Base_M_Detail_Ex;
                Tsql = Tsql + " From tbl_Base_Change_Detail (nolock) ";
                Tsql = Tsql + " Where M_Detail_S = 'tbl_Sales_Rece' ";
                Tsql = Tsql + " And  ( M_Detail like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    " + cls_app_static_var.Base_M_Detail_Ex + " like '%" + tb.Text.Trim() + "%')";
            }


            if (tb.Name == "txt_ItemCode")
            {
                Tsql = "Select Name , NCode ,price2 ,price4    ";
                Tsql += string.Format(" From ufn_Good_Search_Web_Sell_BySellCode ('{0}','{1}','{2}') "
                        , mtxtSellDate.Text.Replace("-", "").Trim()
                        , idx_Na_Code
                        , txtSellCode_Code.Text);
                Tsql = Tsql + " Where NCode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";
                if (opt_sell_3.Checked == true)
                {

                }
            }

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 1)
            {
                tb.Text = ds.Tables["t_P_table"].Rows[0][1].ToString();
                tb1_Code.Text = ds.Tables["t_P_table"].Rows[0][0].ToString();

                if (tb.Name == "txt_Receive_Method")
                {
                    if (SalesItemDetail != null && txt_Receive_Method_Code.Text != "")
                        Rece_Item_Grid_Set();
                }
            }

            if ((ReCnt > 1) || (ReCnt == 0)) Db_Grid_Popup(tb, tb1_Code, Tsql);


        }


        private void Ncod_Text_Set_Data(TextBox tb)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            if (tb.Name == "txt_C_Bank")
            {
                Tsql = "Select BankPenName , BankCode , BankName , BankAccountNumber        ";
                Tsql = Tsql + " From tbl_BankForCompany (nolock)";
                Tsql = Tsql + " Where (BankPenName like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    BankCode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    BankName like '%" + tb.Text.Trim() + "%')";
                if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";
            }


            if (tb.Name == "txt_C_Card")
            {
                Tsql = "Select  Ncode, cardname   ";
                Tsql = Tsql + " From tbl_Card (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    cardname like '%" + tb.Text.Trim() + "%')";
                if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";
            }


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 1)
            {
                int fCnt = 0;
                foreach (string t_key in Ncode_dic.Keys)
                {
                    Ncode_dic[t_key].Text = ds.Tables["t_P_table"].Rows[0][fCnt].ToString();
                    fCnt++;
                }
            }

            if ((ReCnt > 1) || (ReCnt == 0)) Db_Grid_Popup(tb, Tsql);
        }




        private Boolean Input_Error_Check(MaskedTextBox m_tb, string s_Kind, int Check_Leave_TF = 0)
        {
            string T_Mbid = m_tb.Text;
            string Mbid = ""; int Mbid2 = 0;


            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == -1) //올바르게 회원번호 양식에 맞춰서 입력햇는가.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                m_tb.Focus(); return false;
            }

            if (Mbid2 == 0) //올바르게 회원번호 양식에 맞춰서 입력햇는가.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                m_tb.Focus(); return false;
            }

            string Tsql = "";
            Tsql = "Select Mbid , Mbid2, M_Name , Sell_Mem_TF  ";
            Tsql = Tsql + " , LineCnt , N_LineCnt  ";
            Tsql = Tsql + " , LeaveDate , LineUserDate  ";
            Tsql = Tsql + " , Saveid  , Saveid2  ";
            Tsql = Tsql + " , Nominid , Nominid2  ";
            Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            //Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            //Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)  //실제로 존재하는 회원 번호 인가.
            {

                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                m_tb.Focus(); return false;
            }
            else
            {
                if (Check_Leave_TF == 1)
                {
                    if (txt_OrderNumber.Text == "") //신규 저장건에 한해서.
                    {
                        //주문할려고 하는 회원이 탈퇴 회원이다
                        if (ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString() != "")
                        {

                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Leave_Sell")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            m_tb.Focus(); return false;
                        }
                    }
                }
            }
            //++++++++++++++++++++++++++++++++            

            return true;
        }




















        private void _From_Data_Clear()
        {
            tab_Cacu.Visible = true;
            panel18.Visible = true;
            Data_Set_Form_TF = 1;
            Form_Key_Real_TF++;

            if (Form_Key_Real_TF > 1)
                return;
            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            //dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Item.d_Grid_view_Header_Reset();

            //dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu.d_Grid_view_Header_Reset();

            //dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece.d_Grid_view_Header_Reset();

            //dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece_Item.d_Grid_view_Header_Reset();
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


            InsuranceNumber_Ord_Print_FLAG = "";

            txtName.ReadOnly = false;
            txtName.BackColor = SystemColors.Window;
            txtName.BorderStyle = BorderStyle.Fixed3D;

            //txtName.ReadOnly = true;
            //txtName.BackColor = cls_app_static_var.txt_Enable_Color;
            //txtName.BorderStyle = BorderStyle.FixedSingle;

            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, mtxtMbid);

            Base_Ord_Clear();

            tab_Cacu.SelectedIndex = 0;
            radioB_DESK.Checked = true;

            txt_C_B_Number.Text = "000000";
            
            mtxtSn.Mask = "999999-9999999";
            idx_Mbid = ""; idx_Mbid2 = 0; idx_Na_Code = "";

            Data_Set_Form_TF = 0;
            mtxtSellDate.Text = cls_User.gid_date_time;

            mtxtMbid.Focus();
            Sell_Mem_TF = "";
            MP_YN = "N";
        }


        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
           



            if (bt.Name == "butt_Clear")
            {

                _From_Data_Clear();
                Form_Key_Real_TF = 0;
                mtxtMbid.Focus();
            }


            else if (bt.Name == "butt_Save")
            {
              
                  if (txt_OrderNumber.Text == "")
                {
                    if (txtSellCode_Code.Text == "04")
                    {
                        cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                        string Tsql = "select sellcode from tbl_salesdetail (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "' and sellcode = '04' AND RETURNTF <>5 ";
                        DataSet ds = new DataSet();
                        if (Temp_Connect.Open_Data_Set(Tsql, "tbl_salesdetail", ds) == false) return;
                        int ReCnt = Temp_Connect.DataSet_ReCount;

                        if (ReCnt > 0)
                        {

                            cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
                            string Tsql2 = "select sellcode from tbl_salesdetail (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "' and sellcode = '04' AND ReturnTF in(2,3,4,5) ";
                            DataSet ds2 = new DataSet();
                            if (Temp_Connect2.Open_Data_Set(Tsql2, "tbl_salesdetail", ds2) == false) return;
                            int ReCnt2 = Temp_Connect2.DataSet_ReCount;
                            if (ReCnt2 == 0)
                            {
                                string primium_custom_string = ds.Tables["tbl_salesdetail"].Rows[0][0].ToString();

                                if (primium_custom_string == "04")
                                {
                                    if (cls_User.gid_CountryCode == "TH")
                                    {
                                        MessageBox.Show("The ID has already ordered a premium custom pack."
                                   + "\n" +
                                   cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                    }
                                    else
                                    {

                                        MessageBox.Show("해당아이디는 이미 프리미엄 커스텀팩이 주문됐습니다."
                                  + "\n" +
                                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                    }
                                    return;
                                }
                                else
                                {
                                    txtSellCode_Code.Text = "04";
                                }
                            }
                        }
                        int primium_custom = int.Parse(txt_SumPr.Text.Replace(",", ""));
                        if (primium_custom < 800000 || primium_custom > 1000000)
                        {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                MessageBox.Show("To apply for a premium custom pack, " + "\n" + " The order must be over 800,000 won and less than 1 million won."
                   + "\n" +
                   cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            }
                            else
                            {

                                MessageBox.Show("프리미엄 커스텀팩으로 주문신청하기 위해서는" + "\n" + " 80만원이상, 100만원이하 주문이어야 합니다."
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            }
                            return;
                        }
                        cls_Connect_DB Temp_Connect1 = new cls_Connect_DB();
                        string Tsql1 = "select sell_mem_tf from tbl_memberinfo (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "'";
                        DataSet ds1 = new DataSet();
                        if (Temp_Connect1.Open_Data_Set(Tsql1, "tbl_memberinfo", ds1) == false) return;
                        int ReCnt1 = Temp_Connect1.DataSet_ReCount;

                        if (ReCnt1 > 0)
                        {
                            string sell_mem_tf = ds1.Tables["tbl_memberinfo"].Rows[0][0].ToString();
                            if (sell_mem_tf == "1")
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("You must be a seller to apply for an order with a premium custom pack."
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                }
                                else
                                {
                                    MessageBox.Show("프리미엄 커스텀팩으로 주문신청하기 위해서는 판매자여야합니다."
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                }
                                return;
                            }
                        }
                    }
                }

                Temp_Ordernumber = "";
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {
                    cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                    string StrSql = "";//프리미엄커스텀 체크
                    StrSql = "EXEC Usp_Sell_Cacu_ReCul '" + Temp_Ordernumber + "'";
                    Temp_Connect.Update_Data(StrSql, "", "");


                    if (txtSellCode_Code.Text == "04")
                    {
                        StrSql = "EXEC  Usp_Sales_Prom_Primium_Custom '" + Temp_Ordernumber + "'";
                        Temp_Connect.Update_Data(StrSql, "", "");
                    }
                    if (txt_OrderNumber.Text == "" && (txt_UnaccMoney.Text == "0" || txt_UnaccMoney.Text == "0.00")) //주문결제된 내역의 SMS를 보낸다.
                    {
                        if (cls_User.gid_CountryCode == "TH")
                        {
                            //cls_Web Cls_Web = new cls_Web();
                            //string SuccessYN = "";
                            //SuccessYN = "";
                            //string ErrMessage = "";
                            //string SuccessYN_Card = "N";
                            //if (SuccessYN_Card == "N")
                            //{

                            //    SuccessYN_Card = Cls_Web.TH_SMS(idx_Mbid2, Temp_Ordernumber, 4, ref ErrMessage);

                            //}
                            //if (SuccessYN_Card == "Y")
                            //    SuccessYN = "Y";
                            //else
                            //    SuccessYN = "N";


                            //if (SuccessYN_Card == "N")
                            //    MessageBox.Show("문자에러");

                            //if (SuccessYN == "Y")
                            //{
                            //    MessageBox.Show("문자발송성공");
                            //}
                            //SQL내부에서 웹도메인으로 발송해야해서 프로시저를만들었다.

                                StrSql = "EXEC [Usp_TH_SMS]   " + idx_Mbid2 + ",'" + Temp_Ordernumber + "','','4'";
                            // Mail 호출 - 주문완료
                            new cls_Web().SendMail_TH(idx_Mbid2, Temp_Ordernumber, string.Empty, string.Empty, ESendMailType_TH.orderCompleteMail);

                        }
                        else
                        {
                            //EXEC Usp_Insert_SMS '2?', '회원번호1', 회원번호2, '주문번호', ''
                            StrSql = "EXEC Usp_Insert_SMS_New  '20',''," + idx_Mbid2 + ",'" + Temp_Ordernumber + "', ''";
                            //StrSql = "EXEC Usp_Insert_SMS '20',''," + idx_Mbid2 + ",'" + Temp_Ordernumber + "', ''";
                            Temp_Connect.Update_Data(StrSql, "", "");
                        }
                    }

                    ///* 직접 수령 자동 출고*/
                    //StrSql = " EXEC Usp_Sell_Auto_StockOut_Insert '" + Temp_Ordernumber + "' ";
                    //Temp_Connect.Update_Data(StrSql, "", "");
                    //주문 SMS 관련 사항.
                    //cls_Connect_DB Temp_Connect = new cls_Connect_DB();



                    Base_Ord_Clear();

                    if (SalesDetail != null)
                        SalesDetail.Clear();

                    Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

                    if (SalesDetail != null)
                        Base_Grid_Set();
                }


                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }

            else if (bt.Name == "butt_Delete")
            {
                int Delete_Error_Check = 0;

                if (cls_User.gid_Sell_Del_TF == 0) //주문취소 권한이 있는 사람만 가능하다.
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_Not_TF")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    return;
                }

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Delete_Base_Data(ref Delete_Error_Check);

                if (Delete_Error_Check > 0)
                {
                    Base_Ord_Clear();

                    if (SalesDetail != null)
                        SalesDetail.Clear();

                    Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

                    if (SalesDetail != null)
                        Base_Grid_Set();
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }



        private void Rec_Center_Change()
        {
            if (txtCenter_Code.Text == "")
                return;


            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";
            DataSet ds = new DataSet();
            int ReCnt = 0;

            Data_Set_Form_TF = 1;
            mtxtZip1.Text = "";
            txtAddress1.Text = ""; txtAddress2.Text = "";
            mtxtTel1.Text = "";
            mtxtTel2.Text = "";
            txt_Get_Name1.Text = "";
            Data_Set_Form_TF = 0;



            Tsql = "Select ZipCode ,add1 , add2 , phone ";
            Tsql = Tsql + " From tbl_Business (nolock ) ";
            Tsql = Tsql + " Where Ncode = '" + txtCenter_Code.Text.ToString() + "'";


            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            ReCnt = Temp_Connect.DataSet_ReCount;


            if (ReCnt == 0) return;

            Data_Set_Form_TF = 1;
            txtAddress1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["add1"].ToString());
            txtAddress2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["add2"].ToString());

            mtxtZip1.Text = ds.Tables["t_P_table"].Rows[0]["ZipCode"].ToString().Replace("-", "");
            //string AddCode = ds.Tables["t_P_table"].Rows[0]["ZipCode"].ToString().Replace("-", "");
            //if (AddCode.Length >= 6)
            //{
            //    mtxtZip1.Text = AddCode.Substring(0, 3) + "-" + AddCode.Substring(3, 3);              
            //}


            //string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
            //cls_form_Meth cfm = new cls_form_Meth();
            //mtxtTel1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["phone"].ToString());            
            //mtxtTel2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString());

            //if (t_rb.Name == "opt_Rec_Add2")
            //    txt_Get_Name1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString()); //주소테이블의 배송자명은 암호화 햇기 대문에
            //else
            //    txt_Get_Name1.Text = ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString();  //회원 테이블의 회원명은 암호화 안햇음
            Data_Set_Form_TF = 0;



        }

        /// <summary>
        /// 조합신고 (특판신고)
        /// </summary>
        private void Union_Send_Date()
        {
            string StrSql = "";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql = "";

            Tsql = "Select OrderNumber  From tbl_SalesDetail (nolock) ";
            Tsql = Tsql + " Where OrderNumber = '" + Temp_Ordernumber + "'";
            Tsql = Tsql + " And Ga_Order = 0 And InsuranceNumber = ''  ";

            DataSet ds_s = new DataSet();
            Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds_s);
            int ReCnt_S = Temp_Connect.DataSet_ReCount;


            if (ReCnt_S > 0)
            {

                Tsql = "Select hptel ,hometel, Address1, Address2, Mbid ";
                Tsql += ", Case When tbl_Memberinfo.Cpno <> '' then tbl_Memberinfo.Cpno  ELSE Right(BirthDay + BirthDay_M + BirthDay_D , 6) + '0000000'  End Cpno ";
                Tsql += "From tbl_Memberinfo (nolock) ";
                Tsql = Tsql + " Where Mbid = '" + idx_Mbid + "'";
                Tsql = Tsql + " And   Mbid2 = " + idx_Mbid2;

                DataSet ds = new DataSet();
                Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds);
                int ReCnt = Temp_Connect.DataSet_ReCount;

                string Cpno = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["Cpno"].ToString(), "Cpno_U");
                string hptel = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString());
                string hometel = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hometel"].ToString());
                string add_r1 = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["Address1"].ToString());
                string add_r2 = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["Address2"].ToString());

                StrSql = "EXEC p_mlmunion_Order_2 '" + cls_app_static_var.T_Company_Code + "','" + Temp_Ordernumber + "','" + Cpno + "','" + hptel + "','" + hometel + "','" + add_r1 + "','" + add_r2 + "',1";

                Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text);
            }
        }

        /// <summary>
        /// 조합신고 (특판신고) 재신고
        /// </summary>
        private void Union_Send_Date_Re()
        {
            string StrSql = "";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql = "";

            Tsql = "Select OrderNumber  From tbl_SalesDetail (nolock) ";
            Tsql = Tsql + " Where OrderNumber = '" + Temp_Ordernumber + "'";
            Tsql = Tsql + " And Ga_Order = 0 And InsuranceNumber = '' And Union_Seq = 0   ";

            DataSet ds_s = new DataSet();
            Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds_s);
            int ReCnt_S = Temp_Connect.DataSet_ReCount;


            if (ReCnt_S > 0)
            {


                Tsql = "Select hptel ,hometel, Address1, Address2, Mbid ";
                Tsql += ", Case When tbl_Memberinfo.Cpno <> '' then tbl_Memberinfo.Cpno  ELSE Right(BirthDay + BirthDay_M + BirthDay_D , 6) + '0000000'  End Cpno ";
                Tsql += "From tbl_Memberinfo (nolock) ";
                Tsql = Tsql + " Where Mbid = '" + idx_Mbid + "'";
                Tsql = Tsql + " And   Mbid2 = " + idx_Mbid2;

                DataSet ds = new DataSet();
                Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds);
                int ReCnt = Temp_Connect.DataSet_ReCount;

                string Cpno = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["Cpno"].ToString(), "Cpno_U");
                string hptel = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString());
                string hometel = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hometel"].ToString());
                string add_r1 = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["Address1"].ToString());
                string add_r2 = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["Address2"].ToString());

                StrSql = "EXEC p_mlmunion_Order_2 '" + cls_app_static_var.T_Company_Code + "','" + Temp_Ordernumber + "','" + Cpno + "','" + hptel + "','" + hometel + "','" + add_r1 + "','" + add_r2 + "',1";

                Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text);
            }
        }



        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Delete_Base_Data(ref int Delete_Error_Check)
        {
            Delete_Error_Check = 0;

            //주문종류 , 회원, 주문일자 입력 안햇는지 체크
            if (Check_Delete_TextBox_Error() == false) return;

            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string StrSql = "";
                StrSql = "EXEC Usp_Insert_tbl_Sales_CanCel_CS '" + txt_OrderNumber.Text + "','" + cls_User.gid + "',0";

                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);


                tran.Commit();
                Delete_Error_Check = 1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del"));
            }
            catch (Exception)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Err"));

            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }

        }





        private Boolean Check_Delete_TextBox_Error()
        {
            //주문종류 , 회원, 주문일자 입력 안햇는지 체크
            if (Base_Error_Check__01() == false) return false;

            //회원번호 관련 관련 오류 체크 및 존재 여부 그리고 탈퇴 여부(신규 저장일 경우에)                      
            if (Input_Error_Check(mtxtMbid, "m", 0) == false) return false;


            if (txt_OrderNumber.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Sell_OrderNumber")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                dGridView_Base.Focus();
                return false;
            }

            string Ord_N = txt_OrderNumber.Text.Trim();

            //현 내역으로 연관되서 반품이나 교환한 내역이 잇다.
            foreach (string t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D")
                {
                    if (SalesDetail[t_key].Re_BaseOrderNumber == Ord_N)
                    {
                        if (SalesDetail[t_key].ReturnTF == 2)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_2")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtCenter2.Focus(); return false;
                        }
                        if (SalesDetail[t_key].ReturnTF == 3)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_3")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtCenter2.Focus(); return false;
                        }

                        if (SalesDetail[t_key].ReturnTF == 4)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_4")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtCenter2.Focus(); return false;
                        }
                    }
                }
            }


            if (SalesDetail[Ord_N].ReturnTF.ToString() == "2")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_2")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtCenter2.Focus(); return false;
            }

            if (SalesDetail[Ord_N].ReturnTF.ToString() == "3")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_3")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtCenter2.Focus(); return false;
            }

            if (SalesDetail[Ord_N].ReturnTF.ToString() == "4")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_4")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtCenter2.Focus(); return false;
            }


            //공제번호가 있으면 삭제가 안되게 한다. 우선 먼저 공제번호를 취소한후에 다시 시도하게 한다.
            cls_form_Meth cm = new cls_form_Meth();
            if (txt_Ins_Number.Text.Trim() != "" && txt_Ins_Number.Text.Trim() != cm._chang_base_caption_search("미신고"))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Chang_Insur_Number")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                butt_Delete.Focus(); return false;
            }

            //공제번호가 있는데 수정되는 경우를 막아야한다 ...제기랄
            string Sql = string.Format("SELECT Union_Seq, InsuranceNumber FROM tbl_SalesDetail(nolock) WHERE OrderNumber = '{0}'", txt_OrderNumber.Text);
            cls_Connect_DB connect_DB = new cls_Connect_DB();
            using (DataSet ds = new DataSet())
            {
                if (connect_DB.Open_Data_Set(Sql, "공제번호", ds) == false)
                    return false;

                string Union_Seq = ds.Tables["공제번호"].Rows[0]["Union_Seq"].ToString().Trim();
                string InsuranceNumber = ds.Tables["공제번호"].Rows[0]["InsuranceNumber"].ToString().Trim();

                if (Union_Seq.Length > 1 && InsuranceNumber.Length > 1)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Chang_Insur_Number")
                   + "\n" +
                   cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    return false;
                }
            }


            cls_Search_DB csd = new cls_Search_DB();

            ////마감정산이 이루어진 판매 날짜인지 체크한다.                
            //if (csd.Close_Check_SellDate("tbl_CloseTotal_02", SalesDetail[Ord_N].SellDate_2.Replace("-", "")) == false)
            //{
            //    mtxtSellDate2.Focus(); return false;
            //}



            //재고 관련해서 출고가 된내역인지 확인한다 출고 되었으면 삭제 되면 안됨.
            if (csd.Check_Stock_OutPut(txt_OrderNumber.Text.Trim()) == false)
            {
                butt_Delete.Focus(); return false;
            }




            return true;
        }






        private bool Base_Error_Check__01(int SellCode_TF = 0)
        {
            //회원을 선택 안햇네 그럼 회원 넣어라
            if (txtName.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Mem")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return false;
            }


            if (mtxtSellDate.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate.Text, mtxtSellDate, "Date") == false)
                {
                    txtCenter2.Focus();
                    return false;
                }


            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtCenter2.Focus(); return false;
            }



            //mtxtSellDate2.Text = mtxtSellDate.Text;  //정산일자에 무조건 주문일자를 넣는다.
            //if (mtxtSellDate.Text.Replace("-", "").Trim() != "" && mtxtSellDate2.Text.Replace("-", "").Trim() == "")
            //{
            //    mtxtSellDate2.Text = mtxtSellDate.Text;
            //}


            //주문일자를 넣었는지 먼저 체크한다. 안넣었으면 넣어라.
            if (mtxtSellDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate2.Text, mtxtSellDate2, "Date") == false)
                {
                    mtxtSellDate2.Focus();
                    return false;
                }

            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate2")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDate2.Focus(); return false;
            }



            //여기에 bv 추가해야할듯 20230313 구현호
            //주문종류를 선택 안햇네 그럼 그것도 넣어라.
            if (txtSellCode_Code.Text == "" && SellCode_TF == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellCode")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellCode.Focus(); return false;
            }

            return true;
        }





        private bool Base_Error_Check_Not_Sellcode__01()
        {
            //주문일자를 넣었는지 먼저 체크한다. 안넣었으면 넣어라.
            if (mtxtSellDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate2.Text, mtxtSellDate2, "Date") == false)
                {
                    mtxtSellDate2.Focus();
                    return false;
                }
                int Ret = 0;
                cls_Check_Input_Error c_er = new cls_Check_Input_Error();
                Ret = c_er.Input_Date_Err_Check(mtxtSellDate, 1);

                if (Ret == -1)
                {
                    txtCenter2.Focus(); return false;
                }
            }
            else
            {
                txtCenter2.Focus(); return false;
            }


            return true;
        }


        private bool Item_Rece_Error_Check__01(string s_Tf, int CU_TF_Card = 0)
        {
            if (s_Tf == "item")
            {
                //상품은 선택 안햇네 그럼 그것도 넣어라.
                if (txt_ItemName.Text == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Goods")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_ItemCode.Focus(); return false;
                }


                //주문수량을 입력 안햇네 그럼 그것도 넣어라.
                if (txt_ItemCount.Text == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Count")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_ItemCount.Focus(); return false;
                }


                //주문수량을 0  입력햇네  그럼 제대로 넣어라.
                if (int.Parse(txt_ItemCount.Text.Trim()) == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Count")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_ItemCount.Focus(); return false;
                }

                if(txt_Receive_Method_Code.Text.Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                          + "-" + "배송구분"
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_Receive_Method.Focus(); return false;
                }
                else if (txt_Receive_Method_Code.Text.Trim() == "1" && txtCenter3_Code.Text.Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                          + "-" + "수령센터"
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtCenter3.Focus(); return false;
                }
                else if (txt_Receive_Method_Code.Text.Trim() == "2" && txtCenter3_Code.Text.Trim() == "")
                {
                    //txtCenter3.Text = "물류센터";
                    //txtCenter3_Code.Text = "50000";
                    txtCenter3.Text = (cls_User.gid_CountryCode == "TH" ? "TH_Center1" : "물류센터");    // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
                    txtCenter3_Code.Text = (cls_User.gid_CountryCode == "TH" ? "999" : "50000");                // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
                }

            }

            if (s_Tf == "Rece")
            {

                   if (txt_Receive_Method_Code.Text == "1")
                {
                    if (txtCenter3.Text == "" || txtCenter3_Code.Text == "")
                    {
                        if (cls_User.gid_CountryCode == "TH")
                        {
                            MessageBox.Show("Please select a pick-up center.");
                        }
                        else
                        {

                            MessageBox.Show("수령센터를 선택하시기 바랍니다.");
                        }
                        txtCenter3.Focus();
                        return false;
                    }
                }



                //배송구분 선택 안햇네 그럼 그것도 넣어라.
                if (txt_Receive_Method_Code.Text == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Rece")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_Receive_Method.Focus(); return false;
                }

                cls_Check_Input_Error c_er = new cls_Check_Input_Error();
                if (txtGetDate1.Text.Trim() != "")
                {
                    int Ret = 0;
                    Ret = c_er.Input_Date_Err_Check(txtGetDate1);

                    if (Ret == -1)
                    {
                        txtGetDate1.Focus(); return false;
                    }
                }



                string Sn = mtxtTel1.Text.Replace("-", "").Replace("_", "").Trim();
                if (Sn_Number_(Sn, mtxtTel1, "Tel") == false)
                {
                    mtxtTel1.Focus();
                    return false;
                }

                Sn = mtxtTel2.Text.Replace("-", "").Replace("_", "").Trim();
                if (Sn_Number_(Sn, mtxtTel2, "Tel") == false)
                {
                    mtxtTel2.Focus();
                    return false;
                }

                Sn = mtxtZip1.Text.Replace("-", "").Replace("_", "").Trim();
                if (Sn_Number_(Sn, mtxtZip1, "Zip") == false)
                {
                    mtxtZip1.Focus();
                    return false;
                }



                if (dGridView_Base_Rece_Item.Rows.Count == 0)
                {
                    if (SalesItemDetail != null && txt_Receive_Method_Code.Text != "")
                        Rece_Item_Grid_Set();
                }


                int chk_cnt = 0;


                if (cls_app_static_var.Rec_info_Multi_TF == 1)
                {
                    for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                    {
                        dGridView_Base_Rece_Item.Rows[i].Cells[0].Value = "V";
                        chk_cnt++;

                    }
                }
                else
                {
                    for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                    {
                        if (dGridView_Base_Rece_Item.Rows[i].Cells[0].Value.ToString() == "V")
                        {
                            chk_cnt++;
                        }
                    }

                }

                if (chk_cnt == 0)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("After adding the product to the product details, " + "\n" + "Please add the shipping details.");
                    }
                    else
                    {
                        MessageBox.Show("상품내역에 상품을 추가 한 뒤 " + "\n" + "배송내역을 추가 해 주세요");
                    }
                    dGridView_Base_Rece_Item.Focus(); return false;
                }
                if (txt_Receive_Method_Code.Text == "2")
                {
                    if (txt_Get_Name1.Text == "")
                    {
                        if (cls_User.gid_CountryCode == "TH")
                        {
                            MessageBox.Show("Please enter the recipient.");
                        }
                        else
                        {
                            MessageBox.Show("수령인을 넣어주세요.");
                        }
                        txt_Get_Name1.Focus();
                        return false;
                    }

                    if (cls_User.gid_CountryCode != "TH" && mtxtZip1.Text == "")
                    {
                        MessageBox.Show("우편번호를 넣어주세요.");
                        mtxtZip1.Focus();
                        return false;
                    }
                    else if (cls_User.gid_CountryCode == "TH")
                    {
                        if (cbProvince_TH.SelectedIndex < 0)
                        {
                            MessageBox.Show("Please enter the province.");
                            cbProvince_TH.Focus();
                            return false;
                        }
                        else if (cbDistrict_TH.SelectedIndex < 0)
                        {
                            MessageBox.Show("Please enter the district.");
                            cbDistrict_TH.Focus();
                            return false;
                        }
                        else if (cbSubDistrict_TH.SelectedIndex < 0)
                        {
                            MessageBox.Show("Please enter the subdistrict.");
                            cbSubDistrict_TH.Focus();
                            return false;
                        }
                        else if (txtZipCode_TH.Text == "")
                        {
                            MessageBox.Show("Please enter your zip code.");
                            txtZipCode_TH.Focus();
                            return false;
                        }
                    }

                    if (txtAddress1.Text == "")
                    {
                        if (cls_User.gid_CountryCode == "TH")
                        {
                            MessageBox.Show("Please enter address 1.");
                        }
                        else
                        {
                            MessageBox.Show("주소1을 넣어주세요.");
                        }
                        txtAddress1.Focus();
                        return false;
                    }
                }
            }

            if (s_Tf == "Cacu")
            {
                if (Item_Rece_Error_Check__02(CU_TF_Card) == false) return false;
            }

            return true;
        }



        private bool Item_Rece_Error_Check__02(int CU_TF_Card = 0)
        {
            if (dGridView_Base_Item.RowCount == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Goods")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txt_ItemCode.Focus(); return false;
            }

            if (txt_Price_1.Text.Trim() == "") txt_Price_1.Text = "0";
            if (txt_Price_2.Text.Trim() == "") txt_Price_2.Text = "0";
            if (txt_Price_3.Text.Trim() == "") txt_Price_3.Text = "0";
            //if (txt_Price_3_2.Text.Trim() == "") txt_Price_3_2.Text = "0";

            if (txt_Price_4.Text.Trim() == "") txt_Price_4.Text = "0";
            if (txt_Price_5_2.Text.Trim() == "") txt_Price_5_2.Text = "0";
            if (txt_Price_6.Text.Trim() == "") txt_Price_6.Text = "0";
            //0원짜리도 저장이 ㅗ디게 한다.
            if (double.Parse(txt_Price_1.Text.Trim().Replace(",", "")) == 0
                    && double.Parse(txt_Price_2.Text.Trim().Replace(",", "")) == 0
                    && double.Parse(txt_Price_3.Text.Trim().Replace(",", "")) == 0
                    && double.Parse(txt_Price_4.Text.Trim().Replace(",", "")) == 0
                && double.Parse(txt_Price_5_2.Text.Trim().Replace(",", "")) == 0
                  && double.Parse(txt_Price_6.Text.Trim().Replace(",", "")) == 0
                )
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txt_Price_1.Focus(); return false;
            }

            //주문일자를 넣었는지 먼저 체크한다. 안넣었으면 넣어라.
            if (mtxtPriceDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPriceDate1.Text, mtxtPriceDate1, "Date") == false)
                {
                    tab_Cacu.SelectedIndex = 1;
                    mtxtPriceDate1.Focus();
                    return false;
                }

                ////매출일자와 현금 결제일자가 틀리다
                //if (mtxtPriceDate1.Text.Replace("-", "").Trim() != mtxtSellDate.Text.Replace("-", "").Trim())
                //{
                //    if (MessageBox.Show("결제일자와 매출 일자가 상이 합니다. 매출 일자 셋팅을 결제 일자에 맞추시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                //    {
                //        tab_Cacu.SelectedIndex = 1;
                //        mtxtPriceDate1.Focus();
                //        return false;
                //    }
                //    else
                //    {
                //        mtxtSellDate.Text = mtxtPriceDate1.Text;
                //    }
                //}
            }


            if (mtxtPriceDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPriceDate2.Text, mtxtPriceDate2, "Date") == false)
                {
                    tab_Cacu.SelectedIndex = 2;
                    mtxtPriceDate2.Focus();
                    return false;
                }
            }


            if (mtxtPriceDate4.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPriceDate4.Text, mtxtPriceDate4, "Date") == false)
                {
                    tab_Cacu.SelectedIndex = 3;
                    mtxtPriceDate4.Focus();
                    return false;
                }
            }

            if (mtxtPriceDate6.Text.Replace("-", "").Trim() != "")
            {
                 if (Sn_Number_(mtxtPriceDate6.Text, mtxtPriceDate6, "Date") == false)
                {
                    tab_Cacu.SelectedIndex = 4;
                    mtxtPriceDate6.Focus();
                    return false;
                }
            }



            if (butt_Cacu_Save.Text == "추가" && (txt_SumCoupon.Text != "0") && tab_Cacu.SelectedIndex == 4)
            {
                if (txt_SumCoupon.Text != "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("You cannot add more than one coupon."
                   + "\n" +
                   cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("쿠폰은 한개 이상 추가 할 수 없습니다."
                  + "\n" +
                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txt_Price_6.Focus(); return false;
                }
            }


            if (mtxtPriceDate6.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPriceDate6.Text, mtxtPriceDate6, "Date") == false)
                {
                    tab_Cacu.SelectedIndex = 4;
                    mtxtPriceDate6.Focus();
                    return false;
                }
            }




            if (mtxtPriceDate3.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPriceDate3.Text, mtxtPriceDate3, "Date") == false)
                {
                    tab_Cacu.SelectedIndex = 0;
                    mtxtPriceDate3.Focus();
                    return false;
                }

                ////매출일자와 현금 결제일자가 틀리다
                //if (mtxtPriceDate3.Text.Replace("-", "").Trim() != mtxtSellDate.Text.Replace("-", "").Trim())
                //{
                //    if (MessageBox.Show("결제일자와 매출 일자가 상이 합니다. 매출 일자 셋팅을 결제 일자에 맞추시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                //    {
                //        tab_Cacu.SelectedIndex = 0;
                //        mtxtPriceDate3.Focus();
                //        return false;
                //    }
                //    else
                //    {
                //        mtxtSellDate.Text = mtxtPriceDate3.Text;
                //    }
                //}

            }




            if (double.Parse(txt_Price_1.Text) != 0)
            {
                if (mtxtPriceDate1.Text.Replace("-", "").Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_AppDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 1;
                    mtxtPriceDate1.Focus(); return false;
                }
            }


            if (double.Parse(txt_Price_2.Text) != 0)
            {
                if (mtxtPriceDate2.Text.Replace("-", "").Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_AppDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 2;
                    mtxtPriceDate2.Focus(); return false;
                }
            }

            if (double.Parse(txt_Price_3.Text) != 0)
            {
                if (mtxtPriceDate3.Text.Replace("-", "").Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_AppDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 0;
                    mtxtPriceDate3.Focus(); return false;
                }
            }

            if (double.Parse(txt_Price_4.Text) != 0)
            {
                if (mtxtPriceDate4.Text.Replace("-", "").Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_AppDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 3;
                    mtxtPriceDate4.Focus(); return false;
                }
            }

            if (double.Parse(txt_Price_6.Text) != 0)
            {
                if (mtxtPriceDate6.Text.Replace("-", "").Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_AppDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 4;
                    mtxtPriceDate6.Focus(); return false;
                }
            }



            if (double.Parse(txt_Price_2.Text) == 0)
            {
                if (mtxtPriceDate2.Text.Replace("-", "").Trim() != "" || txt_C_Name_2.Text.Trim() != ""
                    || txt_C_Bank.Text.Trim() != "" || txt_C_Bank_Code.Text.Trim() != "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_2")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 2;
                    txt_Price_2.Focus(); return false;
                }
            }




            if (double.Parse(txt_Price_1.Text) == 0)
            {
                if (mtxtPriceDate1.Text.Replace("-", "").Trim() != "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_1")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 1;
                    txt_Price_1.Focus(); return false;
                }
            }


            if (txt_C_Card_Year.Text != "")
            {
                if (int.Parse(txt_C_Card_Year.Text) < int.Parse(cls_User.gid_date_time.Substring(2, 2)) || int.Parse(txt_C_Card_Year.Text) <= 0)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("In the card expiration date, the year input is incorrect."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("카드 유효기간 에서 년 입력이 잘못 되었습니다."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txt_C_Card_Year.Focus(); return false;
                }
                string C_Card_Year = "";
                C_Card_Year = "20" + txt_C_Card_Year.Text;
                combo_C_Card_Year.Text = C_Card_Year;
            }


            if (txt_C_Card_Month.Text != "")
            {
                if (int.Parse(txt_C_Card_Month.Text) > 12 || int.Parse(txt_C_Card_Month.Text) <= 0)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("The month entered in Card Expiration Date is incorrect."
                 + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("카드 유효기간 에서 월 입력이 잘못 되었습니다."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txt_C_Card_Month.Focus(); return false;
                }

                string C_Card_Month = "";
                C_Card_Month = txt_C_Card_Month.Text;

                if (C_Card_Month.Length == 1)
                    C_Card_Month = "0" + C_Card_Month;

                combo_C_Card_Month.Text = C_Card_Month;
            }



            if (tab_Cacu.SelectedTab == tab_Card)
            {
                if (double.Parse(txt_Price_3.Text) == 0)
                {
                    if (mtxtPriceDate3.Text.Replace("-", "").Trim() != "" || txt_C_Name_3.Text.Trim() != ""
                        || txt_C_Card.Text.Trim() != "" || txt_C_Card_Code.Text.Trim() != ""
                        || txt_C_Card_Number.Text.Trim() != "" || txt_C_Card_Ap_Num.Text.Trim() != ""
                        || txt_Price_3_2.Text.Trim() != "" || combo_C_Card_Year.Text.Trim() != ""
                        || combo_C_Card_Month.Text.Trim() != "" || combo_C_Card_Per.Text.Trim() != ""
                        )
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_3")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        tab_Cacu.SelectedIndex = 0;
                        txt_Price_3.Focus(); return false;
                    }

                    if (txt_C_Name_3.Text.Trim() != "")
                    {

                        if (txt_C_Name_3.Text.Trim() == "")
                        {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                MessageBox.Show("Please enter the owner's name.");
                            }
                            else
                            {
                                MessageBox.Show("소유자명을 입력하시기 바랍니다.");
                            }
                            txt_C_Name_3.Focus();
                            return false;
                        }
                    }
                }
            }

            if (double.Parse(txt_Price_4.Text) == 0)
            {
                if (mtxtPriceDate4.Text.Replace("-", "").Trim() != "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 3;
                    txt_Price_4.Focus(); return false;
                }
            }
            if (double.Parse(txt_Price_6.Text) == 0)
            {
                if (mtxtPriceDate6.Text.Replace("-", "").Trim() != "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + "쿠폰내역"
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 3;
                    txt_Price_6.Focus(); return false;
                }
            }


            if (txt_C_index.Text != "") // 수정일 경우에는 카드나 현금 무통장 동시에 못넣게 한다.
            {
                if (txt_Price_1.Text != "0" && txt_Price_2.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_1_2")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 1;
                    txt_Price_1.Focus(); return false;
                }

                if (txt_Price_1.Text != "0" && txt_Price_3.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_1_3")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 1;
                    txt_Price_1.Focus(); return false;
                }

                if (txt_Price_1.Text != "0" && txt_Price_4.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_1_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 1;
                    txt_Price_1.Focus(); return false;
                }
                if (txt_Price_1.Text != "0" && txt_Price_6.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_1_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 1;
                    txt_Price_1.Focus(); return false;
                }
                if (txt_Price_2.Text != "0" && txt_Price_3.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_2_3")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 2;
                    txt_Price_2.Focus(); return false;
                }

                if (txt_Price_2.Text != "0" && txt_Price_4.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_2_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 2;
                    txt_Price_2.Focus(); return false;
                }
                if (txt_Price_2.Text != "0" && txt_Price_6.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_2_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 2;
                    txt_Price_2.Focus(); return false;
                }

                if (txt_Price_3.Text != "0" && txt_Price_4.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_3_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 0;
                    txt_Price_3.Focus(); return false;
                }
                if (txt_Price_3.Text != "0" && txt_Price_6.Text != "0")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Same_Not")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Cacu_Price_3_4")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    tab_Cacu.SelectedIndex = 0;
                    txt_Price_3.Focus(); return false;
                }
            }

            if (txt_Price_3_2.Text.Trim() == "") txt_Price_3_2.Text = "0";


            if (double.Parse(txt_Price_3_2.Text.Trim().Replace(",", "")) != double.Parse(txt_Price_3.Text.Trim().Replace(",", "")))
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("The approved amount you entered for the card and the payment amount are different."
                    +"\n" +
            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                else
                {
                    MessageBox.Show("카드 관련 입력하신 승인 금액과 결제 금액이 상이합니다."

                  + "\n" +
                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                tab_Cacu.SelectedIndex = 0;
                txt_Price_3.Focus(); return false;
            }


            if (CU_TF_Card == 0)
            {
                if (txt_C_Card_Ap_Num.Text.Trim() == "" && double.Parse(txt_Price_3.Text.Trim().Replace(",", "")) > 0)
                {
                    
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("If you have entered the card amount, you must enter the authorization number." + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("카드 금액을 입력하신 경우에는 승인 번호를 필히 입력 하셔야 합니다.."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                        tab_Cacu.SelectedIndex = 0;
                    txt_C_Card_Ap_Num.Focus(); return false;
                }
            }




            if (double.Parse(txt_Price_4.Text) > 0)
            {
                if (Mileage_Error_Check__01() == false)
                {
                    tab_Cacu.SelectedIndex = 3;
                    txt_Price_4.Focus();
                    return false;
                }
            }

            if (chk_app.Checked == true)
            {

                if (txt_C_Card_Number.Text.Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("When saving, you must enter the card number to automatically approve the card."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 카드 자동 승인을 하실려면 카드번호를 필히 입력 하셔야 합니다."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    tab_Cacu.SelectedIndex = 0;
                    txt_C_Card_Number.Focus(); return false;


                }
                if (txt_Price_3_2.Text.Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("To automatically approve the card when saving, you must enter the approved amount."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 카드 자동 승인을 하실려면 승인 금액을 필히 입력 하셔야 합니다."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    tab_Cacu.SelectedIndex = 0;
                    txt_Price_3_2.Focus(); return false;
                }

                if (combo_C_Card_Year.Text == "" || combo_C_Card_Month.Text == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("To automatically approve the card when saving, you must enter the expiration date."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 카드 자동 승인을 하실려면 유효기간을 필히 입력 하셔야 합니다."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    tab_Cacu.SelectedIndex = 0;
                    combo_C_Card_Year.Focus(); return false;
                }
            }


            if (check_Cash.Checked == true)
            {
                if (txt_C_Cash_Send_Nu.Text.Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("In order to process a cash receipt when saving, you must enter the report number."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 현금영수증 처리를 하실려면 신고 번호를 필히 입력 하셔야 합니다."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txt_C_Cash_Send_Nu.Focus(); return false;
                }

                if (radioB_C_Cash_Send_TF1.Checked == false && radioB_C_Cash_Send_TF2.Checked == false)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("To process cash receipts when saving, you must select whether you are an individual or a business."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 현금영수증 처리를 하실려면 개인인지 사업자 인지 선택하셔야 합니다."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    radioB_C_Cash_Send_TF1.Focus(); return false;
                }

                if (check_Cash_Pre.Checked == true)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("You checked the terminal report while checking the cash receipt report. Both cannot be checked."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("현금영수증 신고를 체크 하신 상태에서 단말기 신고도 체크 하셨습니다. 둘다 체크가 불가능 합니다."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    radioB_C_Cash_Send_TF1.Focus(); return false;
                }
            }


            if (check_AVCash.Checked == true)
            {
                if (txt_AVC_Cash_Send_Nu.Text.Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("In order to process a cash receipt when saving, you must enter the report number."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 현금영수증 처리를 하실려면 신고 번호를 필히 입력 하셔야 합니다."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txt_AVC_Cash_Send_Nu.Focus(); return false;
                }

                if (radioB_AVC_Cash_Send_TF1.Checked == false && radioB_AVC_Cash_Send_TF2.Checked == false)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("To process cash receipts when saving, you must select whether you are an individual or a business."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("저장시 현금영수증 처리를 하실려면 개인인지 사업자 인지 선택하셔야 합니다."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    radioB_AVC_Cash_Send_TF1.Focus(); return false;
                }
            }

            if (txt_C_index.Text != "") // 수정인데 현금 영수증이 발행된 내역이다 그럼 수정이 안되게 처리한다.
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    if (txt_C_Cash_Number2.Text != "")
                    {
                        MessageBox.Show("This is the history of the issuance of a cash receipt. Cannot be modified. Please cancel the cash receipt and try again.");
                        butt_Cacu_Save.Focus(); return false;
                    }
                }
                else
                {
                    if (txt_C_Cash_Number2.Text != "")
                    {
                        MessageBox.Show("현금 영수증이 발행된 내역입니다. 수정 할 수 없습니다. 현금 영수증을 취소하신 후에 다시 시도해 주십시요");
                        butt_Cacu_Save.Focus(); return false;
                    }
                }
                if (cls_User.gid_CountryCode == "TH")
                {
                    if (txt_AVC_Cash_Number2.Text != "")
                    {
                        MessageBox.Show("This is the history of the issuance of a cash receipt. Cannot be modified. Please cancel the cash receipt and try again.");
                        butt_Cacu_Save.Focus(); return false;
                    }
                }
                else
                {
                    if (txt_AVC_Cash_Number2.Text != "")
                    {
                        MessageBox.Show("현금 영수증이 발행된 내역입니다. 수정 할 수 없습니다. 현금 영수증을 취소하신 후에 다시 시도해 주십시요");
                        butt_Cacu_Save.Focus(); return false;
                    }
                }
            }




            if (txt_C_index.Text == "") //추가 일경우에 새로운 입력
            {
                if (txt_C_Card_Ap_Num.Text.Trim() != "" && txt_C_Card_Number.Text.Trim() != "")
                {
                    cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                    DataSet ds = new DataSet();

                    //txt_C_Card_Number
                    //txt_C_Card_Ap_Num

                    string Tsql = "";
                    Tsql = "Select OrderNumber   ";
                    Tsql = Tsql + " From tbl_Sales_Cacu (nolock) ";
                    Tsql = Tsql + " Where ( C_Number2 = '" + encrypter.Encrypt(txt_C_Card_Ap_Num.Text.Trim()) + "'";  //동일 승인 번호로 저장된게 잇는지르 찾는다.
                    Tsql = Tsql + " OR  C_Number2 = '" + txt_C_Card_Ap_Num.Text.Trim() + "' ) ";  //동일 승인 번호로 저장된게 잇는지르 찾는다.
                    // Tsql = Tsql + " And   C_Number1 = '" + encrypter.Encrypt(txt_C_Card_Number.Text.Trim()) + "'";  //동일 승인 번호로 저장된게 잇는지르 찾는다.

                    Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds);
                    int ReCnt = Temp_Connect.DataSet_ReCount;

                    if (ReCnt > 0)
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Same_Card_AP_Nu"));
                        //  + "\n" +
                        //  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        // butt_Cacu_Save.Focus(); return false;
                    }
                }
            }


            return true;
        }

        private bool Mileage_Error_Check__01()
        {
            if (txt_SumMile.Text == "")
                txt_SumMile.Text = "0";

            double Using_M = double.Parse(txt_Price_4_2.Text.Replace(",", ""));  //사용가능 마일리지
            double U_M = double.Parse(txt_Price_4.Text.Replace(",", ""));        //현재 결제한 마일리지
            double U_Order_M = double.Parse(txt_SumMile.Text.Replace(",", ""));        //이번 주문번호에서 이미 사용한 마일리지
            double Or_U_Order_M = 0; //이주문번호로 해서 이전에 사용한 마일리지

            if (txt_OrderNumber.Text == "") //새롭게 만들어지는 내역이다.
            {
                if (Using_M < (U_M + U_Order_M))
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Over_Point")
                     + "\n" +
                     cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_Price_4.Focus(); return false;
                }
            }
            else
            {
                cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                Or_U_Order_M = ctm.Using_Mileage_Search(txt_OrderNumber.Text); //이주문으로 해서 사용한 마일리지를 불러온다.

                if ((Using_M + Or_U_Order_M) < (U_M + U_Order_M)) //사용가능 마일 + 이주문으로 이전까지 해서 사용한 마일    이   이번에 사용된 마일리지보다 크다.. 그럼 초과임.
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Over_Point")
                     + "\n" +
                     cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_Price_4.Focus(); return false;
                }

            }

            return true;
        }

        private void Base_Ord_Clear()
        {


            //dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu.d_Grid_view_Header_Reset(1);

            //dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece.d_Grid_view_Header_Reset(1);

            //dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece_Item.d_Grid_view_Header_Reset(1);

            Data_Set_Form_TF = 1;
            if (SalesItemDetail != null)
                SalesItemDetail.Clear();
            if (Sales_Rece != null)
                Sales_Rece.Clear();
            if (Sales_Cacu != null)
                Sales_Cacu.Clear();
            Coupon_Grid_Set();
            Base_Sub_Clear("item");
            Base_Sub_Clear("Rece");
            Base_Sub_Clear("Cacu");

            tab_Cacu.SelectedIndex = 0;
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(panel8, mtxtSellDate);

            txt_OrderNumber.Text = ""; txt_TotalPv.Text = ""; txt_Ins_Number.Text = ""; txt_TotalCV.Text = "";
            txt_TotalPrice.Text = ""; txt_TotalInputPrice.Text = ""; txt_UnaccMoney.Text = "";

            txt_SumCash.Text = ""; txt_SumCard.Text = ""; txt_SumBank.Text = ""; txt_SumMile.Text = "";
            txt_SumCoupon.Text = "";
            txt_SumCnt.Text = ""; txt_SumPr.Text = ""; txt_SumPV.Text = ""; txt_SumCV.Text = "";
            txt_Pass_Number.Text = "";

            radioB_DESK.Checked = true;

            txtSellCode_Code.Text = "01";
            //txtSellCode.Text = "일반주문";
            txtSellCode.Text = (cls_User.gid_CountryCode == "TH" ? "Regular_order" : "일반주문");
            lblSellCode_Code.Text = "01";
            //lblSellCode.Text = "일반주문";
            txtSellCode.Text = (cls_User.gid_CountryCode == "TH" ? "Regular_order" : "일반주문");

            SetSellCode();

            //txtCenter2_Code.Text = "50000";
            //txtCenter2.Text = "물류센터";
            txtCenter2.Text = (cls_User.gid_CountryCode == "TH" ? "TH_Center1" : "물류센터");    // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh
            txtCenter2_Code.Text = (cls_User.gid_CountryCode == "TH" ? "999" : "50000");                // to do: 추후 매나테크에서 등록한 센터로 바꿔야함. - 230829 syhuh

            mtxtSellDate.Text = cls_User.gid_date_time;
            mtxtSellDate2.Text = cls_User.gid_date_time;
            Card_Ok_Visible = true;
            groupBox1.Enabled = true;  //배송정보 입력 그룹박스
            PV_CV_Check = 0;
            chK_PV_CV_Check.Checked = true;
            txt_Receive_Method.Select();
            Data_Set_Form_TF = 0;
            txt_Receive_Method.Text = "";
            txt_Receive_Method_Code.Text = "";
            txtCenter3.Text = "";
            txtCenter3_Code.Text = "";

            //cbZipCode_TH.SelectedIndex = -1;
            txtZipCode_TH.Text = "";
            cbDistrict_TH.SelectedIndex = -1;
            cbProvince_TH.SelectedIndex = -1;
        }


        private void Base_Sub_Clear(string s_Tf)
        {
            cls_form_Meth ct = new cls_form_Meth();
            Data_Set_Form_TF = 1;
            if (s_Tf == "item")
            {
                dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Item.d_Grid_view_Header_Reset(1);

                ct.from_control_clear(groupBox7, txt_ItemCode);
                butt_Item_Del.Visible = false;
                txt_ItemCode.ReadOnly = false;
                txt_ItemCode.BorderStyle = BorderStyle.Fixed3D;
                txt_ItemCode.BackColor = SystemColors.Window;

                butt_Item_Save.Text = ct._chang_base_caption_search("추가");

                if (SalesItemDetail != null)
                    Item_Grid_Set(); //상품 그리드

                txt_ItemCode.Focus();
            }

            if (s_Tf == "Rece")
            {
                dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Rece_Item.d_Grid_view_Header_Reset(1);

                ct.from_control_clear(groupBox1, txt_Receive_Method);
                butt_Rec_Del.Visible = false;
                butt_Rec_Save.Text = ct._chang_base_caption_search("추가");

                if (Sales_Rece != null)
                    Rece_Grid_Set(); //배송 그리드

                txt_Receive_Method.Focus();

            }


            if (s_Tf == "Cacu")
            {
                //cls_form_Meth ct = new cls_form_Meth();
                dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Cacu.d_Grid_view_Header_Reset(1);

                if (combo_C_Card_Year.SelectedIndex >= 0)
                    combo_C_Card_Year.SelectedIndex = 0;
                if (combo_C_Card_Month.SelectedIndex >= 0)
                    combo_C_Card_Month.SelectedIndex = 0;
                if (combo_C_Card_Per.SelectedIndex >= 0)
                    combo_C_Card_Per.SelectedIndex = 0;

                ct.from_control_clear(tab_Cacu, txt_Price_1);
                txt_C_Etc.Text = "";
                txt_C_index.Text = "";

                if (mtxtSellDate.Text.Replace("-", "").Trim() != "")
                {
                    if (Base_Error_Check_Not_Sellcode__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                    double T_p = 0;
                    string T_Mbid = mtxtMbid.Text;
                    string Mbid = ""; int Mbid2 = 0;
                    cls_Search_DB csb = new cls_Search_DB();
                    if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
                    {
                        cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                        T_p = ctm.Using_Mileage_Search(Mbid, Mbid2, mtxtSellDate.Text.Replace("-", "").Trim());
                        txt_Price_4_2.Text = string.Format(cls_app_static_var.str_Currency_Type, T_p);
                    }
                }


                butt_Cacu_Del.Visible = false;
                butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
                tab_Cacu.SelectedIndex = 0;
                tab_Cacu.Enabled = true;
                chk_app.Checked = false;


                enable_Card_info_txt(true);
                enable_Av_Bank_info_txt(true);
                if (Card_Ok_Visible == false)
                {
                    button_Ok.Visible = false;
                }
                else
                {

                    if (cls_app_static_var.Member_Card_Sugi_TF == 1)
                        button_Ok.Visible = true;
                    else
                        button_Ok.Visible = false;
                }
                button_Cancel.Visible = false;
                tableL_CD.Visible = false;

                buttonV_Ok.Visible = true;
                buttonV_Cancel.Visible = false;
                butt_Cacu_Save.Visible = true;

                radioB_C_Cash_Send_TF1.Checked = true;
                radioB_AVC_Cash_Send_TF1.Checked = true;

                if (Sales_Cacu != null)
                    Cacu_Grid_Set(); //배송 그리드

                txt_Price_3.Focus();
            }


            Data_Set_Form_TF = 0;

        }

        private void Base_Sub_Delete(string s_Tf)
        {
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_form_Meth ct = new cls_form_Meth();

            if (s_Tf == "item")
            {
                //주문 상품 관련 딕셔너리에서 찾아서.. 삭제 표식을 해놓는다.
                SalesItemDetail[int.Parse(txt_SalesItemIndex.Text)].Del_TF = "D";

                //배송 관련 정보도 들어간게 있으면 삭제를 표식해 버린다..
                if (Sales_Rece.ContainsKey(int.Parse(txt_SalesItemIndex.Text)) == true)
                    Sales_Rece[int.Parse(txt_SalesItemIndex.Text)].Del_TF = "D";

                ct.from_control_clear(groupBox7, txt_ItemCode);
                butt_Item_Del.Visible = false;
                txt_ItemCode.ReadOnly = false;
                txt_ItemCode.BackColor = SystemColors.Window;
                butt_Item_Save.Text = ct._chang_base_caption_search("추가");

                if (SalesItemDetail != null)
                    Item_Grid_Set(); //상품 그리드 

                if (Sales_Rece != null)
                    Rece_Grid_Set(); //상품 그리드  
            }

            if (s_Tf == "Rece")
            {

                if (cls_app_static_var.Rec_info_Multi_TF == 0)
                {
                    //주문 상품 관련 딕셔너리에서 찾아서.. 삭제 표식을 해놓는다.
                    Sales_Rece[int.Parse(txt_RecIndex.Text)].Del_TF = "D";

                    //상품관련 딕셔너리에서 배송 날짜와 배송 인덱스를 없앤다.
                    SalesItemDetail[int.Parse(txt_RecIndex.Text)].SendDate = "";
                    SalesItemDetail[int.Parse(txt_RecIndex.Text)].RecIndex = 0;

                    if (SalesItemDetail[int.Parse(txt_RecIndex.Text)].Del_TF == "")
                        SalesItemDetail[int.Parse(txt_RecIndex.Text)].Del_TF = "U";
                }
                else
                {
                    foreach (int t_key in Sales_Rece.Keys)
                    {
                        Sales_Rece[t_key].Del_TF = "D";
                    }

                    foreach (int t_key in SalesItemDetail.Keys)
                    {
                        SalesItemDetail[t_key].SendDate = "";
                        SalesItemDetail[t_key].RecIndex = 0;

                        if (SalesItemDetail[t_key].Del_TF == "")
                            SalesItemDetail[t_key].Del_TF = "U";
                    }
                }

                ct.from_control_clear(panel2, txt_Receive_Method);
                chk_Total.Checked = false;
                butt_Rec_Del.Visible = false;
                butt_Rec_Save.Text = ct._chang_base_caption_search("추가");

                if (Sales_Rece != null)
                    Rece_Grid_Set(); //상품 그리드  

                if (SalesItemDetail != null)
                    Item_Grid_Set(); //상품 그리드     
            }

            if (s_Tf == "Cacu")
            {
                //결제 관련 딕셔너리에서 찾아서.. 삭제 표식을 해놓는다.
                Sales_Cacu[int.Parse(txt_C_index.Text)].Del_TF = "D";

                ct.from_control_clear(panel_Cacu, txt_Price_1);
                butt_Cacu_Del.Visible = false;
                butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
                tab_Cacu.Enabled = true;
                enable_Card_info_txt(true);

                if (Sales_Cacu != null)
                    Cacu_Grid_Set(); //상품 그리드  


                double T_p = 0;
                string T_Mbid = mtxtMbid.Text;
                string Mbid = ""; int Mbid2 = 0;
                cls_Search_DB csb = new cls_Search_DB();
                if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
                {
                    cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                    T_p = ctm.Using_Mileage_Search(Mbid, Mbid2, mtxtSellDate.Text.Replace("-", "").Trim());
                    txt_Price_4_2.Text = string.Format(cls_app_static_var.str_Currency_Type, T_p);
                }
            }


            //MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del")
            //       + "\n" +
            //       cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }



        private void Base_Sub_Save_Item()
        {
            cls_form_Meth ct = new cls_form_Meth();

            int New_SalesItemIndex = 0;
            if (SalesItemDetail != null)
            {
                foreach (int t_key in SalesItemDetail.Keys)
                {
                    if (New_SalesItemIndex < t_key)
                        New_SalesItemIndex = t_key;
                }
            }
            New_SalesItemIndex = New_SalesItemIndex + 1;

            cls_Sell_Item t_c_sell = new cls_Sell_Item();

            t_c_sell.OrderNumber = txt_OrderNumber.Text.Trim();
            t_c_sell.SalesItemIndex = New_SalesItemIndex;

            t_c_sell.ItemCode = txt_ItemCode.Text.Trim();
            t_c_sell.ItemName = txt_ItemName.Text.Trim();
            t_c_sell.ItemCount = int.Parse(txt_ItemCount.Text.Trim());

            t_c_sell.SellState = "N_1"; //정상:N_1  반품:R_1  교환나간거:N_3   교환들어온거:R_3
            t_c_sell.SellStateName = ct._chang_base_caption_search("정상");
            t_c_sell.Sell_VAT_TF = 0;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql = "";
            Tsql = "Select price2 ,price4 , price5 , price6, price7, price8 ";
            Tsql = Tsql + " , Sell_VAT_Price , Except_Sell_VAT_Price   ";
            Tsql += string.Format(" From ufn_Good_Search_Web_Sell  ('{0}','{1}','{2}') "
                      , mtxtSellDate.Text.Replace("-", "").Trim()
                      , idx_Na_Code
                      , Sell_Mem_TF);
            Tsql = Tsql + " Where NCode = '" + txt_ItemCode.Text.Trim() + "'";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;
            //테이블 반환 함수에서 뽑아진 쿼리 인덱스를 넣는다
            t_c_sell.ItemPrice = double.Parse(ds.Tables["t_P_table"].Rows[0]["price2"].ToString());
            t_c_sell.ItemPV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price4"].ToString());
            //20230314구현호 이제 테이블반환함수를 바꿔서 새로 BV값을 넣어줘야한다.
            //이건 지우고  (ITEMBV ) CV값에 BV가 들어가도록 한다.

            t_c_sell.ItemCV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price5"].ToString());

            t_c_sell.Sell_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Sell_VAT_Price"].ToString());
            t_c_sell.Sell_Except_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Except_Sell_VAT_Price"].ToString());
            //++++++++++++++++++++++++++++++++
            //총 값을 개수에 맞춰서 넣는다.
            t_c_sell.ItemTotalPrice = t_c_sell.ItemPrice * t_c_sell.ItemCount;
            t_c_sell.ItemTotalPV = t_c_sell.ItemPV * t_c_sell.ItemCount;
            t_c_sell.ItemTotalCV = t_c_sell.ItemCV * t_c_sell.ItemCount;
            t_c_sell.Total_Sell_VAT_Price = t_c_sell.Sell_VAT_Price * t_c_sell.ItemCount;
            t_c_sell.Total_Sell_Except_VAT_Price = t_c_sell.Sell_Except_VAT_Price * t_c_sell.ItemCount;


            t_c_sell.ReturnDate = "";
            t_c_sell.SendDate = "";
            t_c_sell.ReturnBackDate = "";
            t_c_sell.Etc = txt_Item_Etc.Text.Trim();
            t_c_sell.RecIndex = 0;
            t_c_sell.Send_itemCount1 = 0;
            t_c_sell.Send_itemCount2 = 0;
            t_c_sell.T_OrderNumber1 = txt_OrderNumber.Text.Trim();
            t_c_sell.T_OrderNumber2 = "";
            t_c_sell.Real_index = 0;
            t_c_sell.G_Sort_Code = "";

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.Del_TF = "S";
            SalesItemDetail[New_SalesItemIndex] = t_c_sell;


            ct.from_control_clear((Panel)txt_ItemCode.Parent, txt_ItemCode);
            butt_Item_Del.Visible = false;
            butt_Item_Save.Text = ct._chang_base_caption_search("추가");

            if (SalesItemDetail != null)
                Item_Grid_Set(); //상품 그리드               


            //if (Save_Button_Click_Cnt == 1)
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
            //                + "\n" +
            //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
            //}

        }


        private void Base_Sub_Edit_Item()
        {
            cls_form_Meth ct = new cls_form_Meth();

            int SalesItemIndex = int.Parse(txt_SalesItemIndex.Text);

            SalesItemDetail[SalesItemIndex].ItemCode = txt_ItemCode.Text.Trim();
            SalesItemDetail[SalesItemIndex].ItemName = txt_ItemName.Text.Trim();
            SalesItemDetail[SalesItemIndex].ItemCount = int.Parse(txt_ItemCount.Text.Trim());
            SalesItemDetail[SalesItemIndex].Etc = txt_Item_Etc.Text.Trim();

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql = "";
            Tsql = "Select price2 ,price4 , price5 , price6, price7, price8 ";
            Tsql = Tsql + " , Sell_VAT_Price , Except_Sell_VAT_Price   ";
            Tsql += string.Format(" From ufn_Good_Search_Web_Sell ('{0}','{1}','{2}') "
                     , mtxtSellDate.Text.Replace("-", "").Trim()
                     , idx_Na_Code
                     , Sell_Mem_TF);
            Tsql = Tsql + " Where NCode = '" + txt_ItemCode.Text.Trim() + "'";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.



            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;


            SalesItemDetail[SalesItemIndex].ItemPrice = double.Parse(ds.Tables["t_P_table"].Rows[0]["price2"].ToString());
            SalesItemDetail[SalesItemIndex].ItemPV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price4"].ToString());
            SalesItemDetail[SalesItemIndex].ItemCV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price5"].ToString());


            SalesItemDetail[SalesItemIndex].Sell_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Sell_VAT_Price"].ToString());
            SalesItemDetail[SalesItemIndex].Sell_Except_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Except_Sell_VAT_Price"].ToString());
            //++++++++++++++++++++++++++++++++

            SalesItemDetail[SalesItemIndex].ItemTotalPrice = SalesItemDetail[SalesItemIndex].ItemPrice * SalesItemDetail[SalesItemIndex].ItemCount;
            SalesItemDetail[SalesItemIndex].ItemTotalPV = SalesItemDetail[SalesItemIndex].ItemPV * SalesItemDetail[SalesItemIndex].ItemCount;
            SalesItemDetail[SalesItemIndex].ItemTotalCV = SalesItemDetail[SalesItemIndex].ItemCV * SalesItemDetail[SalesItemIndex].ItemCount;

            SalesItemDetail[SalesItemIndex].Total_Sell_VAT_Price = SalesItemDetail[SalesItemIndex].Sell_VAT_Price * SalesItemDetail[SalesItemIndex].ItemCount;
            SalesItemDetail[SalesItemIndex].Total_Sell_Except_VAT_Price = SalesItemDetail[SalesItemIndex].Sell_Except_VAT_Price * SalesItemDetail[SalesItemIndex].ItemCount;


            if (SalesItemDetail[SalesItemIndex].Del_TF == "")
                SalesItemDetail[SalesItemIndex].Del_TF = "U";  //업데이트 되엇다고 표시한다.

            ct.from_control_clear((Panel)txt_ItemCode.Parent, txt_ItemCode);
            butt_Item_Del.Visible = false;
            butt_Item_Save.Text = ct._chang_base_caption_search("추가");

            if (SalesItemDetail != null)
                Item_Grid_Set(); //상품 그리드    

            //MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit")
            //             + "\n" +
            //cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }



        private void Base_Sub_Save_Rece(int New_SalesItemIndex)
        {



            int New_Rec_Index = 0;
            if (Sales_Rece != null)
            {
                foreach (int t_key in Sales_Rece.Keys)
                {
                    if (New_Rec_Index < t_key)
                        New_Rec_Index = t_key;
                }
            }
            New_Rec_Index = New_Rec_Index + 1;

            cls_Sell_Rece t_c_sell = new cls_Sell_Rece();

            t_c_sell.OrderNumber = txt_OrderNumber.Text.Trim();
            t_c_sell.SalesItemIndex = New_SalesItemIndex;
            t_c_sell.RecIndex = New_SalesItemIndex;
            t_c_sell.Get_Name1 = txt_Get_Name1.Text.Trim();
            t_c_sell.Get_Name2 = "";

            t_c_sell.Receive_Method = int.Parse(txt_Receive_Method_Code.Text.Trim());
            t_c_sell.Receive_Method_Name = txt_Receive_Method.Text.Trim();

            string t_sellDate = "";
            t_c_sell.Get_Date1 = "";
            t_c_sell.Get_Date2 = "";

            if (txtGetDate1.Text.Trim() != "")
            {
                t_sellDate = txtGetDate1.Text.Trim().Substring(0, 4);
                t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(4, 2);
                t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(6, 2);

                t_c_sell.Get_Date1 = t_sellDate;
            }

            string Get_Tel1 = ""; string Get_Tel2 = "";
            if (mtxtTel1.Text.Replace("-", "").Trim() != "") Get_Tel1 = mtxtTel1.Text.Trim();
            if (mtxtTel2.Text.Replace("-", "").Trim() != "") Get_Tel2 = mtxtTel2.Text.Trim();

            t_c_sell.Get_Tel1 = Get_Tel1;
            t_c_sell.Get_Tel2 = Get_Tel2;

            t_c_sell.Get_ZipCode = "";
            t_c_sell.Get_Address1 = "";
            t_c_sell.Get_Address2 = "";

            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                if (cbProvince_TH.Text.Replace("-", "").Trim() != "")
                {
                    //t_c_sell.Get_state = cbProvince_TH.Text.Trim();
                    t_c_sell.Get_state = cbProvince_TH.SelectedValue.ToString().Trim();
                }

                if (cbDistrict_TH.Text.Replace("-", "").Trim() != "")
                {
                    t_c_sell.Get_city = cbDistrict_TH.Text.Trim();
                }

                if (txtZipCode_TH.Text.Replace("-", "").Trim() != "")
                {
                    t_c_sell.Get_ZipCode = txtZipCode_TH.Text.Replace("-", "");
                }
            }
            // 태국 이외 국가
            else
            {
                if (mtxtZip1.Text.Replace("-", "").Trim() != "")
                    t_c_sell.Get_ZipCode = mtxtZip1.Text.Replace("-", "");
            }



            if (txtAddress1.Text.Trim() != "")
                t_c_sell.Get_Address1 = txtAddress1.Text.Trim();

            if (txtAddress2.Text.Trim() != "")
                t_c_sell.Get_Address2 = txtAddress2.Text.Trim();

            t_c_sell.Get_Etc1 = txt_Get_Etc1.Text.Trim();
            t_c_sell.Get_Etc2 = "";
            t_c_sell.Pass_Number = txt_Pass_Number.Text.Trim();
            t_c_sell.Base_Rec_Name = txt_Base_Rec.Text.Trim();
            t_c_sell.Base_Rec = txt_Base_Rec_Code.Text.Trim();

            t_c_sell.Receive_Center_Name = txtCenter3.Text.Trim();
            t_c_sell.Receive_Center = txtCenter3_Code.Text.Trim();

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.Del_TF = "S";
            //Sales_Rece[New_SalesItemIndex] = t_c_sell;
            Sales_Rece[New_Rec_Index] = t_c_sell;


            SalesItemDetail[New_SalesItemIndex].RecIndex = New_SalesItemIndex;
            SalesItemDetail[New_SalesItemIndex].SendDate = txtGetDate1.Text.Trim();

            if (SalesItemDetail[New_SalesItemIndex].Del_TF == "")
                SalesItemDetail[New_SalesItemIndex].Del_TF = "U";
            t_c_sell.Pass_Pay = double.Parse(txt_InputPass_Pay.Text.Trim().Replace(",", ""));
        }

        private void Base_Sub_Edit_Rece()
        {
            int SalesItemIndex = int.Parse(txt_RecIndex.Text);

            Sales_Rece[SalesItemIndex].Get_Name1 = txt_Get_Name1.Text.Trim();
            Sales_Rece[SalesItemIndex].Receive_Method = int.Parse(txt_Receive_Method_Code.Text.Trim());
            Sales_Rece[SalesItemIndex].Receive_Method_Name = txt_Receive_Method.Text.Trim();


            string t_sellDate = "";
            Sales_Rece[SalesItemIndex].Get_Date1 = "";
            if (txtGetDate1.Text.Trim() != "")
            {
                t_sellDate = txtGetDate1.Text.Trim().Substring(0, 4);
                t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(4, 2);
                t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(6, 2);

                Sales_Rece[SalesItemIndex].Get_Date1 = t_sellDate;
            }

            string Get_Tel1 = ""; string Get_Tel2 = "";
            if (mtxtTel1.Text.Replace("-", "").Trim() != "") Get_Tel1 = mtxtTel1.Text.Trim();
            if (mtxtTel2.Text.Replace("-", "").Trim() != "") Get_Tel2 = mtxtTel2.Text.Trim();

            Sales_Rece[SalesItemIndex].Get_Tel1 = Get_Tel1;
            Sales_Rece[SalesItemIndex].Get_Tel2 = Get_Tel2;

            Sales_Rece[SalesItemIndex].Get_ZipCode = "";
            Sales_Rece[SalesItemIndex].Get_Address1 = "";
            Sales_Rece[SalesItemIndex].Get_Address2 = "";

            if (mtxtZip1.Text.Replace("-", "").Trim() != "")
                Sales_Rece[SalesItemIndex].Get_ZipCode = mtxtZip1.Text.Replace("-", "");


            if (txtAddress1.Text.Trim() != "")
                Sales_Rece[SalesItemIndex].Get_Address1 = txtAddress1.Text.Trim();

            if (txtAddress2.Text.Trim() != "")
                Sales_Rece[SalesItemIndex].Get_Address2 = txtAddress2.Text.Trim();

            Sales_Rece[SalesItemIndex].Get_Etc1 = txt_Get_Etc1.Text.Trim();
            Sales_Rece[SalesItemIndex].Pass_Number = txt_Pass_Number.Text.Trim();
            Sales_Rece[SalesItemIndex].Base_Rec_Name = txt_Base_Rec.Text.Trim();
            Sales_Rece[SalesItemIndex].Base_Rec = txt_Base_Rec_Code.Text.Trim();


            Sales_Rece[SalesItemIndex].Receive_Center = txtCenter3_Code.Text.Trim();
            Sales_Rece[SalesItemIndex].Receive_Center_Name = txtCenter3.Text.Trim();

            if (Sales_Rece[SalesItemIndex].Del_TF == "")
                Sales_Rece[SalesItemIndex].Del_TF = "U";
            try
            {
                SalesItemDetail[SalesItemIndex].SendDate = txtGetDate1.Text.Trim();

                if (SalesItemDetail[SalesItemIndex].Del_TF == "")
                    SalesItemDetail[SalesItemIndex].Del_TF = "U";
            }
            catch { }
        }



        /// <summary>
        /// 2020-05-08 지성경 추가, 아이템을 추가 => 배송지 추가 => 결제금액 추가 => 아이템추가했을때
        /// 마지막으로 추가된 Item 의 배송지가 Rece에 표기되지않아서 문제되는부분을 해결
        /// </summary>
        private void Base_Sub_Save_ItemRece()
        {
            List<int> NewSalesItemIndexs = new List<int>();
            cls_Sell_Rece ReceItem = null;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF == "D")
                    continue;

                if (Sales_Rece.ContainsKey(t_key))
                {
                    ReceItem = Sales_Rece[t_key].Clone() as cls_Sell_Rece;
                    break;
                }
            }


            if (ReceItem == null)
                return;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (!Sales_Rece.ContainsKey(t_key))
                {
                    Sales_Rece[t_key] = ReceItem.Clone() as cls_Sell_Rece;
                    Sales_Rece[t_key].SalesItemIndex = t_key;
                    Sales_Rece[t_key].RecIndex = t_key;
                    Sales_Rece[t_key].Del_TF = "S";
                }
            }


            Base_Sub_Clear("Rece");

        }


        private void Base_Sub_Edit_Rece(int TF)
        {
            foreach (int t_key in Sales_Rece.Keys)
            {
                if (Sales_Rece[t_key].Del_TF != "D")
                {
                    int SalesItemIndex = t_key;

                    Sales_Rece[SalesItemIndex].Get_Name1 = txt_Get_Name1.Text.Trim();
                    Sales_Rece[SalesItemIndex].Receive_Method = int.Parse(txt_Receive_Method_Code.Text.Trim());
                    Sales_Rece[SalesItemIndex].Receive_Method_Name = txt_Receive_Method.Text.Trim();


                    string t_sellDate = "";
                    Sales_Rece[SalesItemIndex].Get_Date1 = "";
                    if (txtGetDate1.Text.Trim() != "")
                    {
                        t_sellDate = txtGetDate1.Text.Trim().Substring(0, 4);
                        t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(4, 2);
                        t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(6, 2);

                        Sales_Rece[SalesItemIndex].Get_Date1 = t_sellDate;
                    }

                    string Get_Tel1 = ""; string Get_Tel2 = "";
                    if (mtxtTel1.Text.Replace("-", "").Trim() != "") Get_Tel1 = mtxtTel1.Text.Trim();
                    if (mtxtTel2.Text.Replace("-", "").Trim() != "") Get_Tel2 = mtxtTel2.Text.Trim();

                    Sales_Rece[SalesItemIndex].Get_Tel1 = Get_Tel1;
                    Sales_Rece[SalesItemIndex].Get_Tel2 = Get_Tel2;

                    Sales_Rece[SalesItemIndex].Get_ZipCode = "";
                    Sales_Rece[SalesItemIndex].Get_Address1 = "";
                    Sales_Rece[SalesItemIndex].Get_Address2 = "";

                    Sales_Rece[SalesItemIndex].Get_city = "";
                    Sales_Rece[SalesItemIndex].Get_state = "";

                    // 태국인 경우
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        Sales_Rece[SalesItemIndex].Get_state = cbProvince_TH.SelectedValue.ToString().Trim();
                        Sales_Rece[SalesItemIndex].Get_city = cbDistrict_TH.Text.Trim();

                        if (txtZipCode_TH.Text.Trim() != "")
                        {
                            Sales_Rece[SalesItemIndex].Get_ZipCode = txtZipCode_TH.Text.Trim();
                        }
                    }
                    // 그 외 국가인 경우
                    else
                    {
                        if (mtxtZip1.Text.Replace("-", "").Trim() != "")
                            Sales_Rece[SalesItemIndex].Get_ZipCode = mtxtZip1.Text.Replace("-", "");
                    }

                    if (txtAddress1.Text.Trim() != "")
                        Sales_Rece[SalesItemIndex].Get_Address1 = txtAddress1.Text.Trim();

                    if (txtAddress2.Text.Trim() != "")
                        Sales_Rece[SalesItemIndex].Get_Address2 = txtAddress2.Text.Trim();

                    Sales_Rece[SalesItemIndex].Get_Etc1 = txt_Get_Etc1.Text.Trim();
                    Sales_Rece[SalesItemIndex].Pass_Number = txt_Pass_Number.Text.Trim();
                    Sales_Rece[SalesItemIndex].Base_Rec_Name = txt_Base_Rec.Text.Trim();
                    Sales_Rece[SalesItemIndex].Base_Rec = txt_Base_Rec_Code.Text.Trim();

                    Sales_Rece[t_key].Receive_Center = txtCenter3_Code.Text.Trim();
                    Sales_Rece[t_key].Receive_Center_Name = txtCenter3.Text.Trim();


                    if (Sales_Rece[SalesItemIndex].Del_TF == "")
                        Sales_Rece[SalesItemIndex].Del_TF = "U";

                    SalesItemDetail[SalesItemIndex].SendDate = txtGetDate1.Text.Trim();

                    if (SalesItemDetail[SalesItemIndex].Del_TF == "")
                        SalesItemDetail[SalesItemIndex].Del_TF = "U";


                }
            }
        }


        private void Base_Sub_Edit_Rece_ALL()
        {
            foreach (int t_key in Sales_Rece.Keys)
            {
                Sales_Rece[t_key].Get_Name1 = txt_Get_Name1.Text.Trim();
                Sales_Rece[t_key].Receive_Method = int.Parse(txt_Receive_Method_Code.Text.Trim());
                Sales_Rece[t_key].Receive_Method_Name = txt_Receive_Method.Text.Trim();


                string t_sellDate = "";
                Sales_Rece[t_key].Get_Date1 = "";
                if (txtGetDate1.Text.Trim() != "")
                {
                    t_sellDate = txtGetDate1.Text.Trim().Substring(0, 4);
                    t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(4, 2);
                    t_sellDate = t_sellDate + "-" + txtGetDate1.Text.Trim().Substring(6, 2);

                    Sales_Rece[t_key].Get_Date1 = t_sellDate;
                }

                string Get_Tel1 = ""; string Get_Tel2 = "";
                if (mtxtTel1.Text.Replace("-", "").Trim() != "") Get_Tel1 = mtxtTel1.Text.Trim();
                if (mtxtTel2.Text.Replace("-", "").Trim() != "") Get_Tel2 = mtxtTel2.Text.Trim();

                Sales_Rece[t_key].Get_Tel1 = Get_Tel1;
                Sales_Rece[t_key].Get_Tel2 = Get_Tel2;

                Sales_Rece[t_key].Get_ZipCode = "";
                Sales_Rece[t_key].Get_Address1 = "";
                Sales_Rece[t_key].Get_Address2 = "";

                if (mtxtZip1.Text.Replace("-", "").Trim() != "")
                    Sales_Rece[t_key].Get_ZipCode = mtxtZip1.Text.Replace("-", "");


                if (txtAddress1.Text.Trim() != "")
                    Sales_Rece[t_key].Get_Address1 = txtAddress1.Text.Trim();

                if (txtAddress2.Text.Trim() != "")
                    Sales_Rece[t_key].Get_Address2 = txtAddress2.Text.Trim();

                Sales_Rece[t_key].Get_Etc1 = txt_Get_Etc1.Text.Trim();
                Sales_Rece[t_key].Pass_Number = txt_Pass_Number.Text.Trim();
                Sales_Rece[t_key].Base_Rec_Name = txt_Base_Rec.Text.Trim();
                Sales_Rece[t_key].Base_Rec = txt_Base_Rec_Code.Text.Trim();


                if (Sales_Rece[t_key].Del_TF == "")
                    Sales_Rece[t_key].Del_TF = "U";

                SalesItemDetail[t_key].SendDate = txtGetDate1.Text.Trim();

                if (SalesItemDetail[t_key].Del_TF == "")
                    SalesItemDetail[t_key].Del_TF = "U";
            }
        }



        private void Base_Sub_Save_Cacu(int C_SF, ref int R_C_index)
        {
            cls_form_Meth ct = new cls_form_Meth();
            int New_C_index = 0;
            if (Sales_Cacu != null)
            {
                foreach (int t_key in Sales_Cacu.Keys)
                {
                    if (New_C_index < t_key)
                        New_C_index = t_key;
                }
            }
            New_C_index = New_C_index + 1;

            R_C_index = New_C_index;

            cls_Sell_Cacu t_c_sell = new cls_Sell_Cacu();

            t_c_sell.OrderNumber = txt_OrderNumber.Text.Trim();
            t_c_sell.C_index = New_C_index;

            t_c_sell.C_Price1 = 0;
            t_c_sell.C_AppDate1 = "";
            t_c_sell.C_Name1 = "";
            t_c_sell.C_Code = "";
            t_c_sell.C_CodeName = "";
            t_c_sell.C_CodeName_2 = "";
            t_c_sell.C_Number1 = "";
            t_c_sell.C_Number2 = "";
            t_c_sell.C_Number3 = "";
            t_c_sell.C_Price2 = 0;
            t_c_sell.C_Period1 = "";
            t_c_sell.C_Period2 = "";
            t_c_sell.C_Installment_Period = "";
            t_c_sell.C_CVC = "";

            t_c_sell.C_B_Number = "";
            t_c_sell.C_P_Number = "";
            t_c_sell.Sugi_TF = "";

            t_c_sell.C_Cash_Send_Nu = "";
            t_c_sell.C_Cash_Send_TF = 0;

            t_c_sell.C_Cash_Sort_TF = 0;
            t_c_sell.C_Cash_Bus_TF = 0;


            if (C_SF == 1)
            {
                t_c_sell.C_TF = 1;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("현금");
                t_c_sell.C_Price1 = double.Parse(txt_Price_1.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate1.Text.Replace("-", "").Trim();

                if (check_Not_Cash.Checked == false)
                {
                    if (check_Cash.Checked == true)
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();

                        if (radioB_C_Cash_Send_TF1.Checked == true)
                        {
                            t_c_sell.C_Cash_Send_TF = 1;
                            t_c_sell.C_Cash_Bus_TF = 0;
                        }
                        else
                        {
                            t_c_sell.C_Cash_Send_TF = 2;
                            t_c_sell.C_Cash_Bus_TF = 1;
                        }
                        t_c_sell.C_Cash_Sort_TF = 1;
                    }
                    else
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();
                        t_c_sell.C_Cash_Send_TF = 0;
                        t_c_sell.C_Cash_Sort_TF = 2;
                        t_c_sell.C_Cash_Bus_TF = 1;

                    }
                }
                else
                {
                    t_c_sell.C_Cash_Send_Nu = "";
                    t_c_sell.C_Cash_Send_TF = -1;
                    t_c_sell.C_Cash_Sort_TF = -1;
                    t_c_sell.C_Cash_Bus_TF = -1;
                }


                if (check_Cash_Pre.Checked == true || txt_C_Cash_Number2_2.Text != "")
                {
                    t_c_sell.C_Cash_Sort_TF = 100;
                    t_c_sell.C_Cash_Number = txt_C_Cash_Number2_2.Text.Trim();
                    t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                }

            }

            if (C_SF == 2)
            {
                t_c_sell.C_TF = 2;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("무통장");

                t_c_sell.C_Price1 = double.Parse(txt_Price_2.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate2.Text.Replace("-", "").Trim();
                t_c_sell.C_Name1 = txt_C_Name_2.Text.Trim();
                t_c_sell.C_Code = txt_C_Bank_Code.Text.Trim();
                t_c_sell.C_CodeName = txt_C_Bank_Code_2.Text.Trim();
                t_c_sell.C_CodeName_2 = txt_C_Bank.Text.Trim();
                t_c_sell.C_Number1 = txt_C_Bank_Code_3.Text.Trim();



                if (check_Not_Bank.Checked == false)
                {
                    if (check_Bank.Checked == true)
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();

                        if (radioB_C_Bank_Send_TF1.Checked == true)
                        {
                            t_c_sell.C_Cash_Send_TF = 1;
                            t_c_sell.C_Cash_Bus_TF = 0;
                        }
                        else
                        {
                            t_c_sell.C_Cash_Send_TF = 2;
                            t_c_sell.C_Cash_Bus_TF = 1;
                        }
                        t_c_sell.C_Cash_Sort_TF = 1;
                    }
                    else
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                        t_c_sell.C_Cash_Send_TF = 0;
                        t_c_sell.C_Cash_Sort_TF = 2;
                        t_c_sell.C_Cash_Bus_TF = 1;

                    }
                }
                else
                {
                    t_c_sell.C_Cash_Send_Nu = "";
                    t_c_sell.C_Cash_Send_TF = -1;
                    t_c_sell.C_Cash_Sort_TF = -1;
                    t_c_sell.C_Cash_Bus_TF = -1;
                }

                if (check_Cash_Pre.Checked == true)
                {
                    t_c_sell.C_Cash_Sort_TF = 100;
                    t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                }

            }


            if (C_SF == 3)
            {
                t_c_sell.C_TF = 3;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("카드");

                t_c_sell.C_Price1 = double.Parse(txt_Price_3.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate3.Text.Replace("-", "").Trim();
                t_c_sell.C_Name1 = txt_C_Name_3.Text.Trim();
                t_c_sell.C_Code = txt_C_Card_Code.Text.Trim();
                t_c_sell.C_CodeName = txt_C_Card.Text.Trim();
                t_c_sell.C_CodeName_2 = "";
                t_c_sell.C_Number1 = txt_C_Card_Number.Text.Trim();
                t_c_sell.C_Number2 = txt_C_Card_Ap_Num.Text.Trim();
                t_c_sell.C_Price2 = double.Parse(txt_Price_3_2.Text.Trim());
                t_c_sell.C_Period1 = combo_C_Card_Year.Text.Trim();
                t_c_sell.C_Period2 = combo_C_Card_Month.Text.Trim();
                t_c_sell.C_Installment_Period = combo_C_Card_Per.Text.Trim();


                t_c_sell.C_B_Number = txt_C_B_Number.Text.Trim();
                t_c_sell.C_P_Number = txt_C_P_Number.Text.Trim();

                t_c_sell.C_CVC = txt_C_CVC.Text.Trim();

                t_c_sell.Sugi_TF = "1";
            }

            if (C_SF == 4)
            {
                t_c_sell.C_TF = 4;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("마일리지");
                t_c_sell.C_Price1 = double.Parse(txt_Price_4.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate4.Text.Replace("-", "").Trim();
            }

            if (C_SF == 5)
            {
                t_c_sell.C_TF = 5;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("가상계좌");
                t_c_sell.C_Price1 = 0;
                t_c_sell.C_Price2 = double.Parse(txt_Price_5_2.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = cls_User.gid_date_time;

                t_c_sell.C_Code = txtBank_Code.Text.Trim();
                t_c_sell.C_CodeName = txtBank.Text.Trim();

                if (check_Not_AVCash.Checked == false)
                {
                    //20220914 구현호 체크없앤다

                    if (check_AVCash.Checked == true)
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_AVC_Cash_Send_Nu.Text.Trim();

                        if (radioB_AVC_Cash_Send_TF1.Checked == true)
                        {
                            t_c_sell.C_Cash_Send_TF = 1;
                            t_c_sell.C_Cash_Bus_TF = 0;
                        }
                        else
                        {
                            t_c_sell.C_Cash_Send_TF = 2;
                            t_c_sell.C_Cash_Bus_TF = 1;
                        }
                        t_c_sell.C_Cash_Sort_TF = 1;
                    }
                    else
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_AVC_Cash_Send_Nu.Text.Trim();
                        t_c_sell.C_Cash_Send_TF = 0;
                        t_c_sell.C_Cash_Sort_TF = 2;
                        t_c_sell.C_Cash_Bus_TF = 1;

                    }
                }
                else
                {
                    t_c_sell.C_Cash_Send_Nu = "";
                    t_c_sell.C_Cash_Send_TF = -1;
                    t_c_sell.C_Cash_Sort_TF = -1;
                    t_c_sell.C_Cash_Bus_TF = -1;
                }
            }

            if (C_SF == 6)
            {
                t_c_sell.C_TF = 6;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("쿠폰");
                t_c_sell.C_Price1 = double.Parse(txt_Price_6.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate6.Text.Replace("-", "").Trim();
                t_c_sell.C_Coupon = txt_Price_6_2.Text.Replace("-", "").Trim();
            }



            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.C_Etc = txt_C_Etc.Text.Trim();

            t_c_sell.Del_TF = "S";
            Sales_Cacu[New_C_index] = t_c_sell;
        }




        private void Base_Sub_Save_Cacu(int C_SF)
        {
            cls_form_Meth ct = new cls_form_Meth();
            int New_C_index = 0;
            if (Sales_Cacu != null)
            {
                foreach (int t_key in Sales_Cacu.Keys)
                {
                    if (New_C_index < t_key)
                        New_C_index = t_key;
                }
            }
            New_C_index = New_C_index + 1;

            cls_Sell_Cacu t_c_sell = new cls_Sell_Cacu();

            t_c_sell.OrderNumber = txt_OrderNumber.Text.Trim();
            t_c_sell.C_index = New_C_index;

            t_c_sell.C_Price1 = 0;
            t_c_sell.C_AppDate1 = "";
            t_c_sell.C_Name1 = "";
            t_c_sell.C_Code = "";
            t_c_sell.C_CodeName = "";
            t_c_sell.C_CodeName_2 = "";
            t_c_sell.C_Number1 = "";
            t_c_sell.C_Number2 = "";
            t_c_sell.C_Number3 = "";
            t_c_sell.C_Number4 = "";
            t_c_sell.C_Price2 = 0;
            t_c_sell.C_Period1 = "";
            t_c_sell.C_Period2 = "";
            t_c_sell.C_Installment_Period = "";
            t_c_sell.C_CVC = "";
            t_c_sell.C_B_Number = "";
            t_c_sell.C_P_Number = "";
            t_c_sell.Sugi_TF = "";

            t_c_sell.C_Cash_Send_Nu = "";
            t_c_sell.C_Cash_Send_TF = 0;
            t_c_sell.C_Cash_Sort_TF = 0;
            t_c_sell.C_Cash_Bus_TF = 0;



            if (C_SF == 1)
            {
                t_c_sell.C_TF = 1;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("현금");
                t_c_sell.C_Price1 = double.Parse(txt_Price_1.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate1.Text.Replace("-", "").Trim();

                if (check_Not_Cash.Checked == false)
                {
                    if (check_Cash.Checked == true)
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();

                        if (radioB_C_Cash_Send_TF1.Checked == true)
                        {
                            t_c_sell.C_Cash_Send_TF = 1;
                            t_c_sell.C_Cash_Bus_TF = 0;
                        }
                        else
                        {
                            t_c_sell.C_Cash_Send_TF = 2;
                            t_c_sell.C_Cash_Bus_TF = 1;
                        }
                        t_c_sell.C_Cash_Sort_TF = 1;
                    }
                    else
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();
                        t_c_sell.C_Cash_Send_TF = 0;
                        t_c_sell.C_Cash_Sort_TF = 2;
                        t_c_sell.C_Cash_Bus_TF = 1;

                    }
                }
                else
                {
                    t_c_sell.C_Cash_Send_Nu = "";
                    t_c_sell.C_Cash_Send_TF = -1;
                    t_c_sell.C_Cash_Sort_TF = -1;
                    t_c_sell.C_Cash_Bus_TF = -1;
                }

                if (check_Cash_Pre.Checked == true || txt_C_Cash_Number2_2.Text != "")
                {
                    t_c_sell.C_Cash_Sort_TF = 100;
                    t_c_sell.C_Cash_Number = txt_C_Cash_Number2_2.Text.Trim();
                    t_c_sell.C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();
                }
            }

            if (C_SF == 2)
            {
                t_c_sell.C_TF = 2;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("무통장");

                t_c_sell.C_Price1 = double.Parse(txt_Price_2.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate2.Text.Replace("-", "").Trim();
                t_c_sell.C_Name1 = txt_C_Name_2.Text.Trim();
                t_c_sell.C_Code = txt_C_Bank_Code.Text.Trim();
                t_c_sell.C_CodeName = txt_C_Bank_Code_2.Text.Trim();
                t_c_sell.C_CodeName_2 = txt_C_Bank.Text.Trim();
                t_c_sell.C_Number1 = txt_C_Bank_Code_3.Text.Trim();


                if (check_Not_Bank.Checked == false)
                {
                    if (check_Bank.Checked == true)
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();

                        if (radioB_C_Bank_Send_TF1.Checked == true)
                        {
                            t_c_sell.C_Cash_Send_TF = 1;
                            t_c_sell.C_Cash_Bus_TF = 0;
                        }
                        else
                        {
                            t_c_sell.C_Cash_Send_TF = 2;
                            t_c_sell.C_Cash_Bus_TF = 1;
                        }
                        t_c_sell.C_Cash_Sort_TF = 1;
                    }
                    else
                    {
                        t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                        t_c_sell.C_Cash_Send_TF = 0;
                        t_c_sell.C_Cash_Sort_TF = 2;
                        t_c_sell.C_Cash_Bus_TF = 1;

                    }
                }
                else
                {
                    t_c_sell.C_Cash_Send_Nu = "";
                    t_c_sell.C_Cash_Send_TF = -1;
                    t_c_sell.C_Cash_Sort_TF = -1;
                    t_c_sell.C_Cash_Bus_TF = -1;
                }

                if (check_Cash_Pre.Checked == true)
                {
                    t_c_sell.C_Cash_Sort_TF = 100;
                    t_c_sell.C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                }


            }


            if (C_SF == 3)
            {
                t_c_sell.C_TF = 3;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("카드");

                t_c_sell.C_Price1 = double.Parse(txt_Price_3.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate3.Text.Replace("-", "").Trim();
                t_c_sell.C_Name1 = txt_C_Name_3.Text.Trim();
                t_c_sell.C_Code = txt_C_Card_Code.Text.Trim();
                t_c_sell.C_CodeName = txt_C_Card.Text.Trim();
                t_c_sell.C_CodeName_2 = "";
                t_c_sell.C_Number1 = txt_C_Card_Number.Text.Trim();
                t_c_sell.C_Number2 = txt_C_Card_Ap_Num.Text.Trim();
                t_c_sell.C_Price2 = double.Parse(txt_Price_3_2.Text.Trim());
                t_c_sell.C_Period1 = combo_C_Card_Year.Text.Trim();
                t_c_sell.C_Period2 = combo_C_Card_Month.Text.Trim();
                t_c_sell.C_Installment_Period = combo_C_Card_Per.Text.Trim();
                t_c_sell.C_CVC = txt_C_CVC.Text.Trim();

                t_c_sell.C_B_Number = txt_C_B_Number.Text.Trim();
                t_c_sell.C_P_Number = txt_C_P_Number.Text.Trim();
                t_c_sell.Sugi_TF = txt_Sugi_TF.Text.Trim();

                //if (chk_app.Checked == true)
                //    t_c_sell.Sugi_TF = "1";
            }

            if (C_SF == 4)
            {
                t_c_sell.C_TF = 4;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("마일리지");
                t_c_sell.C_Price1 = double.Parse(txt_Price_4.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate4.Text.Replace("-", "").Trim();
            }

            if (C_SF == 5)
            {
                t_c_sell.C_TF = 5;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("가상계좌");
                t_c_sell.C_Price1 = 0;
                t_c_sell.C_Price2 = double.Parse(txt_Price_5_2.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = cls_User.gid_date_time;


                if (check_AVCash.Checked == true)
                {
                    t_c_sell.C_Cash_Send_Nu = txt_AVC_Cash_Send_Nu.Text.Trim();

                    if (radioB_AVC_Cash_Send_TF1.Checked == true)
                        t_c_sell.C_Cash_Send_TF = 1;
                    else
                        t_c_sell.C_Cash_Send_TF = 2;
                }
            }

            if (C_SF == 6)
            {
                t_c_sell.C_TF = 6;
                t_c_sell.C_TF_Name = ct._chang_base_caption_search("쿠폰");
                t_c_sell.C_Price1 = double.Parse(txt_Price_6.Text.Trim().Replace(",", ""));
                t_c_sell.C_AppDate1 = mtxtPriceDate6.Text.Replace("-", "").Trim();
                t_c_sell.C_Coupon = txt_Price_6_2.Text.Replace("-", "").Trim();
            }





            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.C_Etc = txt_C_Etc.Text.Trim();

            t_c_sell.Del_TF = "S";
            Sales_Cacu[New_C_index] = t_c_sell;
        }




        private void Base_Sub_Edit_Cacu()
        {
            cls_form_Meth ct = new cls_form_Meth();
            int C_index = int.Parse(txt_C_index.Text);
            Sales_Cacu[C_index].C_Etc = txt_C_Etc.Text.Trim();
            Sales_Cacu[C_index].C_TF = 0;
            Sales_Cacu[C_index].C_TF_Name = "";

            Sales_Cacu[C_index].C_Price1 = 0;
            Sales_Cacu[C_index].C_AppDate1 = "";
            Sales_Cacu[C_index].C_Name1 = "";
            Sales_Cacu[C_index].C_Code = "";
            Sales_Cacu[C_index].C_CodeName = "";
            Sales_Cacu[C_index].C_CodeName_2 = "";
            Sales_Cacu[C_index].C_Number1 = "";
            Sales_Cacu[C_index].C_Number2 = "";
            Sales_Cacu[C_index].C_Price2 = 0;
            Sales_Cacu[C_index].C_Period1 = "";
            Sales_Cacu[C_index].C_Period2 = "";
            Sales_Cacu[C_index].C_Installment_Period = "";
            Sales_Cacu[C_index].C_CVC = "";
            Sales_Cacu[C_index].C_B_Number = "";
            Sales_Cacu[C_index].C_P_Number = "";
            Sales_Cacu[C_index].Sugi_TF = "";

            Sales_Cacu[C_index].C_Cash_Send_Nu = "";
            Sales_Cacu[C_index].C_Cash_Send_TF = 0;
            Sales_Cacu[C_index].C_Cash_Sort_TF = 0;
            Sales_Cacu[C_index].C_Cash_Bus_TF = 0;

            Sales_Cacu[C_index].C_Coupon = "";
            if (double.Parse(txt_Price_1.Text.Trim()) > 0)  //현금이다
            {
                Sales_Cacu[C_index].C_TF = 1;
                Sales_Cacu[C_index].C_TF_Name = ct._chang_base_caption_search("현금");
                Sales_Cacu[C_index].C_Price1 = double.Parse(txt_Price_1.Text.Trim().Replace(",", ""));
                Sales_Cacu[C_index].C_AppDate1 = mtxtPriceDate1.Text.Replace("-", "").Trim();

                if (check_Not_Cash.Checked == false)
                {
                    if (check_Cash.Checked == true)
                    {
                        Sales_Cacu[C_index].C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();

                        if (radioB_C_Cash_Send_TF1.Checked == true)
                        {
                            Sales_Cacu[C_index].C_Cash_Send_TF = 1;  //개인
                            Sales_Cacu[C_index].C_Cash_Bus_TF = 0; //세금계산서 번호
                        }
                        else
                        {
                            Sales_Cacu[C_index].C_Cash_Send_TF = 2;  //사업자
                            Sales_Cacu[C_index].C_Cash_Bus_TF = 1; //세금계산서 번호
                        }
                        Sales_Cacu[C_index].C_Cash_Sort_TF = 1;

                        //2016-09-09 작업. 단말기로 신고했다가 단말기 신고를 안하고 수기로 신고를 하려고 할 때
                        // 기존 단말기 신고 승인번호를 리셋해주지 않기 때문에 
                        // 현금영수증 발행 화면에 나타나지 않음.
                        Sales_Cacu[C_index].C_Cash_Number = txt_C_Cash_Number2.Text.Trim();

                    }
                    else
                    {
                        Sales_Cacu[C_index].C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();
                        Sales_Cacu[C_index].C_Cash_Send_TF = 0;
                        Sales_Cacu[C_index].C_Cash_Sort_TF = 2;  //전자세금 계산서
                        Sales_Cacu[C_index].C_Cash_Bus_TF = 1;
                    }
                }
                else
                {
                    Sales_Cacu[C_index].C_Cash_Send_Nu = "";
                    Sales_Cacu[C_index].C_Cash_Send_TF = -1;
                    Sales_Cacu[C_index].C_Cash_Sort_TF = -1;  //전자세금 계산서
                    Sales_Cacu[C_index].C_Cash_Bus_TF = -1;
                }

                if (check_Cash_Pre.Checked == true || txt_C_Cash_Number2_2.Text != "")
                {
                    Sales_Cacu[C_index].C_Cash_Sort_TF = 100;
                    Sales_Cacu[C_index].C_Cash_Number = txt_C_Cash_Number2_2.Text.Trim();
                    Sales_Cacu[C_index].C_Cash_Send_Nu = txt_C_Cash_Send_Nu.Text.Trim();
                }
            }

            if (double.Parse(txt_Price_2.Text.Trim()) > 0)  //무통이다
            {
                Sales_Cacu[C_index].C_TF = 2;
                Sales_Cacu[C_index].C_TF_Name = ct._chang_base_caption_search("무통장");
                Sales_Cacu[C_index].C_Price1 = double.Parse(txt_Price_2.Text.Trim().Replace(",", ""));
                Sales_Cacu[C_index].C_AppDate1 = mtxtPriceDate2.Text.Replace("-", "").Trim();
                Sales_Cacu[C_index].C_Name1 = txt_C_Name_2.Text.Trim();
                Sales_Cacu[C_index].C_Code = txt_C_Bank_Code.Text.Trim();
                Sales_Cacu[C_index].C_CodeName = txt_C_Bank_Code_2.Text.Trim();
                Sales_Cacu[C_index].C_CodeName_2 = txt_C_Bank.Text.Trim();
                Sales_Cacu[C_index].C_Number1 = txt_C_Bank_Code_3.Text.Trim();


                if (check_Not_Bank.Checked == false)
                {
                    if (check_Bank.Checked == true)
                    {
                        Sales_Cacu[C_index].C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();

                        if (radioB_C_Bank_Send_TF1.Checked == true)
                        {
                            Sales_Cacu[C_index].C_Cash_Send_TF = 1;  //개인
                            Sales_Cacu[C_index].C_Cash_Bus_TF = 0; //세금계산서 번호
                        }
                        else
                        {
                            Sales_Cacu[C_index].C_Cash_Send_TF = 2;  //사업자
                            Sales_Cacu[C_index].C_Cash_Bus_TF = 1; //세금계산서 번호
                        }
                        Sales_Cacu[C_index].C_Cash_Sort_TF = 1;

                        //2016-09-09 작업. 단말기로 신고했다가 단말기 신고를 안하고 수기로 신고를 하려고 할 때
                        // 기존 단말기 신고 승인번호를 리셋해주지 않기 때문에 
                        // 현금영수증 발행 화면에 나타나지 않음.
                        Sales_Cacu[C_index].C_Cash_Number = txt_C_Bank_Number2.Text.Trim();

                    }
                    else
                    {
                        Sales_Cacu[C_index].C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                        Sales_Cacu[C_index].C_Cash_Send_TF = 0;
                        Sales_Cacu[C_index].C_Cash_Sort_TF = 2;  //전자세금 계산서
                        Sales_Cacu[C_index].C_Cash_Bus_TF = 1;
                    }
                }
                else
                {
                    Sales_Cacu[C_index].C_Cash_Send_Nu = "";
                    Sales_Cacu[C_index].C_Cash_Send_TF = -1;
                    Sales_Cacu[C_index].C_Cash_Sort_TF = -1;  //전자세금 계산서
                    Sales_Cacu[C_index].C_Cash_Bus_TF = -1;
                }

                if (check_Cash_Pre.Checked == true)
                {
                    Sales_Cacu[C_index].C_Cash_Sort_TF = 100;
                    Sales_Cacu[C_index].C_Cash_Send_Nu = txt_C_Bank_Send_Nu.Text.Trim();
                }

            }

            if (double.Parse(txt_Price_3.Text.Trim()) > 0 || mtxtPriceDate3.Text.Replace("-", "").Trim() != "" || txt_C_Card_Number.Text.Trim() != "")  //카드이다
            {
                Sales_Cacu[C_index].C_TF = 3;
                Sales_Cacu[C_index].C_TF_Name = ct._chang_base_caption_search("카드");
                Sales_Cacu[C_index].C_Price1 = double.Parse(txt_Price_3.Text.Trim().Replace(",", ""));
                Sales_Cacu[C_index].C_AppDate1 = mtxtPriceDate3.Text.Replace("-", "").Trim();
                Sales_Cacu[C_index].C_Name1 = txt_C_Name_3.Text.Trim();
                Sales_Cacu[C_index].C_Code = txt_C_Card_Code.Text.Trim();
                Sales_Cacu[C_index].C_CodeName = txt_C_Card.Text.Trim();
                Sales_Cacu[C_index].C_Number1 = txt_C_Card_Number.Text.Trim();
                Sales_Cacu[C_index].C_Number2 = txt_C_Card_Ap_Num.Text.Trim();
                Sales_Cacu[C_index].C_Price2 = double.Parse(txt_Price_3_2.Text.Trim());
                Sales_Cacu[C_index].C_Period1 = combo_C_Card_Year.Text.Trim();
                Sales_Cacu[C_index].C_Period2 = combo_C_Card_Month.Text.Trim();
                Sales_Cacu[C_index].C_Installment_Period = combo_C_Card_Per.Text.Trim();
                Sales_Cacu[C_index].C_CVC = txt_C_CVC.Text.Trim();
                Sales_Cacu[C_index].Sugi_TF = txt_Sugi_TF.Text.Trim();

                Sales_Cacu[C_index].C_B_Number = txt_C_B_Number.Text.Trim();
                Sales_Cacu[C_index].C_P_Number = txt_C_P_Number.Text.Trim();
                Sales_Cacu[C_index].Sugi_TF = txt_Sugi_TF.Text.Trim();



                //if (checkB_Hana_Card.Checked == true && SW == 0)
                //{
                //    Sales_Cacu[C_index].Je_Card_FLAG = "Hana";
                //}
                //if (chk_app.Checked == true)
                //    Sales_Cacu[C_index].Sugi_TF = "1";
            }

            if (double.Parse(txt_Price_4.Text.Trim()) > 0)  //현금이다
            {
                Sales_Cacu[C_index].C_TF = 4;
                Sales_Cacu[C_index].C_TF_Name = ct._chang_base_caption_search("마일리지");
                Sales_Cacu[C_index].C_Price1 = double.Parse(txt_Price_4.Text.Trim().Replace(",", ""));
                Sales_Cacu[C_index].C_AppDate1 = mtxtPriceDate4.Text.Replace("-", "").Trim();
            }

            if (double.Parse(txt_Price_5_2.Text.Trim()) > 0)  //현금이다
            {

                //StrSql = StrSql + " C_Number3  = '" + C_Number3 + "'"; //거래번호
                //StrSql = StrSql + " ,C_Number1 = '" + T_rAuthNo + "'"; //가상계좌번호
                //StrSql = StrSql + " ,C_Code = '26'"; //신한은행

                Sales_Cacu[C_index].C_TF = 5;
                Sales_Cacu[C_index].C_TF_Name = ct._chang_base_caption_search("가상계좌");
                Sales_Cacu[C_index].C_Price1 = double.Parse(txt_Price_5.Text.Trim().Replace(",", ""));
                Sales_Cacu[C_index].C_Price2 = double.Parse(txt_Price_5_2.Text.Trim().Replace(",", ""));
                Sales_Cacu[C_index].C_AppDate1 = txt_AV_C_AppDate1.Text.Replace("-", "").Trim();

                Sales_Cacu[C_index].C_Code = txtBank_Code.Text.Trim();
                Sales_Cacu[C_index].C_CodeName = txtBank.Text.Trim();

                //Sales_Cacu[C_index].C_Code = txt_AV_C_Code.Text.Trim();
                Sales_Cacu[C_index].C_Number1 = txt_AV_C_Number1.Text.Trim();
                Sales_Cacu[C_index].C_Number3 = txt_AV_C_Number3.Text.Trim();


                if (check_Not_AVCash.Checked == false)
                {
                    if (check_AVCash.Checked == true)
                    {
                        Sales_Cacu[C_index].C_Cash_Send_Nu = txt_AVC_Cash_Send_Nu.Text.Trim();

                        if (radioB_AVC_Cash_Send_TF1.Checked == true)
                        {
                            Sales_Cacu[C_index].C_Cash_Send_TF = 1;  //개인
                            Sales_Cacu[C_index].C_Cash_Bus_TF = 0; //세금계산서 번호
                        }
                        else
                        {
                            Sales_Cacu[C_index].C_Cash_Send_TF = 2;  //사업자
                            Sales_Cacu[C_index].C_Cash_Bus_TF = 1; //세금계산서 번호
                        }
                        Sales_Cacu[C_index].C_Cash_Sort_TF = 1;

                    }
                    else
                    {
                        Sales_Cacu[C_index].C_Cash_Send_Nu = txt_AVC_Cash_Send_Nu.Text.Trim();
                        Sales_Cacu[C_index].C_Cash_Send_TF = 0;
                        Sales_Cacu[C_index].C_Cash_Sort_TF = 2;  //전자세금 계산서
                        Sales_Cacu[C_index].C_Cash_Bus_TF = 1;
                    }
                }
                else
                {
                    Sales_Cacu[C_index].C_Cash_Send_Nu = "";
                    Sales_Cacu[C_index].C_Cash_Send_TF = -1;
                    Sales_Cacu[C_index].C_Cash_Sort_TF = -1;  //전자세금 계산서
                    Sales_Cacu[C_index].C_Cash_Bus_TF = -1;
                }

            }
            if (double.Parse(txt_Price_6.Text.Trim()) > 0)  //쿠폰이다
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Used coupons cannot be modified. After deleting, please issue a new one.");
                }
                else
                {

                    MessageBox.Show("사용된 쿠폰을 수정 할 수 없습니다. 삭제 후 새로 발급 받아주세요.");
                }
                couponnumber = "1";
                return;
                //Sales_Cacu[C_index].C_TF = 6;
                //Sales_Cacu[C_index].C_TF_Name = ct._chang_base_caption_search("쿠폰");
                //Sales_Cacu[C_index].C_Price1 = double.Parse(txt_Price_6.Text.Trim().Replace(",", ""));
                //Sales_Cacu[C_index].C_AppDate1 = mtxtPriceDate6.Text.Replace("-", "").Trim();
                //Sales_Cacu[C_index].C_Coupon = txt_Price_6_2.Text.Replace("-", "").Trim();


            }
            if (Sales_Cacu[C_index].Del_TF == "")
                Sales_Cacu[C_index].Del_TF = "U";
        }




        private void dGridView_KeyDown(object sender, KeyEventArgs e)
        {
            //그리드일 경우에는 DEL키로 행을 삭제하는걸 막는다.
            if (sender is DataGridView)
            {
                if (e.KeyValue == 46)
                {
                    e.Handled = true;
                } // end if

                if (e.KeyValue == 13)
                {
                    dGridView_Base_Rece_Add_DoubleClick(sender, e);
                }
            }
        }


        private void dGridView_Base_Rece_Add_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {


                txt_Get_Name1.Text = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();

                mtxtZip1.Text = "";
                mtxtZip1.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                //if ((sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Length >= 6)
                //{
                //    mtxtZip1.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(0, 3) + "-" + (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(3, 3);
                //    //txtAddCode2.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(3, 3);
                //}

                txtAddress1.Text = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                txtAddress2.Text = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();
                cls_form_Meth cfm = new cls_form_Meth();

                cfm.Home_Number_Setting((sender as DataGridView).CurrentRow.Cells[4].Value.ToString(), mtxtTel1);
                cfm.Home_Number_Setting((sender as DataGridView).CurrentRow.Cells[5].Value.ToString(), mtxtTel2);


                //string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";



                //cfm.Phone_Number_Split((sender as DataGridView).CurrentRow.Cells[4].Value.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;

                //cfm.Phone_Number_Split((sender as DataGridView).CurrentRow.Cells[5].Value.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

                dGridView_Base_Rece_Add.Visible = false;
                cfm.form_Group_Panel_Enable_True(this);

            }
        }


        private void butt_Rec_Add_Click(object sender, EventArgs e)
        {
            //회원을 선택 안햇네 그럼 회원 넣어라
            if (txtName.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Mem")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return;
            }

            if (txt_Receive_Method_Code.Text == "1")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("This is a direct receipt.");
                }
                else
                {
                    MessageBox.Show("직접수령 건 입니다.");
                }
                return;
            }

            dGridView_Base_Rece_Add.Width = groupBox1.Width - 100;
            dGridView_Base_Rece_Add.Height = groupBox1.Height - 70;
            dGridView_Base_Rece_Add.Left = groupBox1.Left;
            dGridView_Base_Rece_Add.Top = groupBox1.Top;

            Rece_Add_Grid_Set();

            cls_form_Meth cfm = new cls_form_Meth();
            cfm.form_Group_Panel_Enable_False(this);

            dGridView_Base_Rece_Add.BringToFront();
            dGridView_Base_Rece_Add.RowHeadersVisible = false;
            dGridView_Base_Rece_Add.Visible = true;
            dGridView_Base_Rece_Add.Focus();
        }


        private void Rece_Add_Grid_Set()
        {
            dGridView_Rec_Add_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Add.d_Grid_view_Header_Reset();
            string strSql = "";

            strSql = "Select Distinct Get_Name1 ,Get_ZipCode  ,  Get_Address1  , Get_Address2    ";
            strSql = strSql + " ,Get_Tel1 , Get_Tel2 ";
            strSql = strSql + " From tbl_Sales_Rece (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail (nolock) ON  tbl_SalesDetail.OrderNumber = tbl_Sales_Rece.OrderNumber ";

            if (idx_Mbid.Length == 0)
                strSql = strSql + " Where tbl_SalesDetail.Mbid2 = " + idx_Mbid2.ToString();
            else
            {
                strSql = strSql + " Where tbl_SalesDetail.Mbid = '" + idx_Mbid + "' ";
                strSql = strSql + " And   tbl_SalesDetail.Mbid2 = " + idx_Mbid2.ToString();
            }
            strSql = strSql + " And   Receive_Method = 2 ";
            strSql = strSql + " Order by  ";
            strSql = strSql + " Get_Name1 ,Get_ZipCode  ,  Get_Address1  , Get_Address2  ";
            strSql = strSql + " ,Get_Tel1 , Get_Tel2 ";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, "TempTable", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;
            if (ReCnt == 0) return;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_Rec_Add_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }

            cgb_Rece_Add.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Rece_Add.db_grid_Obj_Data_Put();
        }

        private void Set_gr_Rec_Add_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {

            string[] row0 = { ds.Tables["TempTable"].Rows[fi_cnt][0].ToString()
                                ,ds.Tables["TempTable"].Rows[fi_cnt][1].ToString()
                                ,encrypter.Decrypt ( ds.Tables["TempTable"].Rows[fi_cnt][2].ToString())
                                ,encrypter.Decrypt (ds.Tables["TempTable"].Rows[fi_cnt][3].ToString())
                                ,encrypter.Decrypt (ds.Tables["TempTable"].Rows[fi_cnt][4].ToString() )

                                ,encrypter.Decrypt (ds.Tables["TempTable"].Rows[fi_cnt][5].ToString()  )
                                ,""
                                ,""
                                ,""
                                ,""
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Rec_Add_Header_Reset()
        {
            cgb_Rece_Add.Grid_Base_Arr_Clear();
            cgb_Rece_Add.basegrid = dGridView_Base_Rece_Add;
            cgb_Rece_Add.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Rece_Add.grid_col_Count = 10;

            string[] g_HeaderText = {"수령인명"  , "우편번호"   , "주소1"  , "주소2"   , "연락처_1"
                                , "연락처_2"   , ""    , ""  , "" , ""
                                };

            int[] g_Width = { 80 ,80, 250, 200, 120
                                ,120 , 0 , 0 , 0 , 0
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft  //5    
  
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter  //10
                                };

            cgb_Rece_Add.grid_col_header_text = g_HeaderText;
            cgb_Rece_Add.grid_col_w = g_Width;
            cgb_Rece_Add.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                   };
            cgb_Rece_Add.grid_col_Lock = g_ReadOnly;

            cgb_Rece_Add.basegrid.RowHeadersVisible = false;
        }




        private void Base_Small_Item_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Item_Clear")
            {
                if (txtSellCode_Code.Text == "Auto")
                {
                    Base_Sub_Sum_Item();
                    Base_Sub_Sum_Cacu();
                }
                Base_Sub_Clear("item");
            }

            else if (bt.Name == "butt_Item_Del")
            {
                if (txt_SalesItemIndex.Text == "") return;

                if (txt_OrderNumber.Text != "")  //주문번호가 존재한다.
                {
                    cls_Search_DB csd = new cls_Search_DB();
                    //재고 관련해서 출고가 된내역인지 확인한다 출고 되었으면 삭제 되면 안됨.
                    if (csd.Check_Stock_OutPut(txt_OrderNumber.Text.Trim(), int.Parse(txt_SalesItemIndex.Text.Trim())) == false)
                    {
                        butt_Item_Del.Focus(); return;
                    }
                }

                if (txtSellCode_Code.Text == "Auto")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("It is not possible to enter or modify sales related to Autoship."
                              + "\n" +
                              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {

                        MessageBox.Show("오토쉽 관련 매출의 입력 또는 수정이 불가능 합니다."
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txtSellCode.Focus();
                    return;
                }

                Base_Sub_Delete("item");
                Base_Sub_Clear("Rece");
                Base_Sub_Sum_Item();
                Base_Sub_Sum_Cacu();

            }

            else if (bt.Name == "butt_Item_Save")
            {
                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                if (Item_Rece_Error_Check__01("item") == false) return;

                if (txtSellCode_Code.Text == "Auto")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("It is not possible to enter or modify sales related to Autoship."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("오토쉽 관련 매출의 입력 또는 수정이 불가능 합니다."
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txtSellCode.Focus();
                    return;
                }


                if (txt_SalesItemIndex.Text == "") //추가 일경우에 새로운 입력
                {
                    int Stock_cnt = 0;
                    cls_Search_DB csd = new cls_Search_DB();
                    //재고 관련해서 부족하게 되면.. 못하게 한다.
                    if (csd.Check_Stock_RealCnt(txtCenter3_Code.Text, txt_ItemCode.Text.Trim(), int.Parse(txt_ItemCount.Text)) == false)
                    {
                        txt_ItemCode.Focus(); return;
                    }
                    string successYN = "";
           
                    successYN = Cacu_Grid_Set_2();
                    if(successYN == "N")
                    {
                        return;
                    }
                    Base_Sub_Save_Item();
                    Base_Sub_Clear("item");
                    Base_Sub_Sum_Item();
                    Base_Sub_Sum_Cacu();
                    Base_Sub_Save_ItemRece();

                    Save_Button_Click_Cnt++;
                }
                else  //
                {
                   
                    Base_Sub_Edit_Item();
                    Base_Sub_Clear("item");
                    Base_Sub_Sum_Item();
                    Base_Sub_Sum_Cacu();
                    Base_Sub_Save_ItemRece();
                    Save_Button_Click_Cnt++;
                }
            }
        }


        private void Base_Small_Rece_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Rec_Clear")
            {
                Base_Sub_Clear("Rece");
            }

            else if (bt.Name == "butt_Rec_Del")
            {
                if (txt_RecIndex.Text == "") return;
                //20230418구현호 배송주소 삭제시 출고된 내역이면 삭제를 막는다
                //if (dGridView_Base_Rece.Rows.Count == 1)
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine(" select * from tbl_StockOutput where OrderNumber = '" + txt_OrderNumber.Text.Trim() + "'");
                //    cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                //    DataSet ds = new DataSet();
                //    Temp_Connect.Open_Data_Set(sb.ToString(), "tbl_StockOutput", ds);
                //    int ReCnt = Temp_Connect.DataSet_ReCount;
                //    if (ReCnt != 0)
                //    {
                //        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_ChangeRece_In_Stockout")
                //         + "\n" +
                //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                //        return;
                //    }
                //    else
                //    {

                //    }
                //}
                Base_Sub_Delete("Rece");
            }


            else if (bt.Name == "butt_Rec_Save")
            {

                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크



                if (Item_Rece_Error_Check__01("Rece") == false) return;


                if (txt_RecIndex.Text == "") //추가 일경우에 새로운 입력
                {
                    cls_form_Meth ct = new cls_form_Meth();

                    if (cls_app_static_var.Rec_info_Multi_TF == 0)
                    {

                        //for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                        //{
                        //    if (csd.Check_Stock_OutPut(txt_OrderNumber.Text.Trim()) == false)
                        //    {
                        //        butt_Rec_Save.Focus(); return;
                        //    }
                        //}
                        int Salesitemindex = 0;
                        for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                        {
                            if (dGridView_Base_Rece_Item.Rows[i].Cells[0].Value.ToString() == "V")
                            {
                                Salesitemindex = int.Parse(dGridView_Base_Rece_Item.Rows[i].Cells[1].Value.ToString());
                                Base_Sub_Save_Rece(Salesitemindex);
                            }
                        }
                    }
                    else
                    {
                        int Salesitemindex = 0;
                        for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                        {
                            Salesitemindex = int.Parse(dGridView_Base_Rece_Item.Rows[i].Cells[1].Value.ToString());
                            Base_Sub_Save_Rece(Salesitemindex);
                        }
                    }

                    Base_Sub_Clear("Rece");
                    Base_Sub_Clear("item");
                    Save_Button_Click_Cnt++;

                    //if (Save_Button_Click_Cnt == 1)
                    //{
                    //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
                    //                + "\n" +
                    //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                    ////}
                }
                else
                {
                    //20230418구현호 배송주소 수정시 출고된 내역이면 수정을 막는다
                    if (dGridView_Base_Rece.Rows.Count == 1)
                    {
                        if (txt_OrderNumber.Text != "")
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(" EXEC [USP_ORDER_DELIVERY_CHANGE_CHECK] '" + txt_OrderNumber.Text.Trim() + "'");
                            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                            DataSet ds = new DataSet();
                            Temp_Connect.Open_Data_Set(sb.ToString(), "tbl_StockOutput", ds);
                            string posetUpdateYn = ds.Tables["tbl_StockOutput"].Rows[0][1].ToString();

                            if (posetUpdateYn == "N")
                            {
                                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_ChangeRece_In_Stockout")
                                 + "\n" +
                                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                return;
                            }
                            else
                            {

                            }
                        }
                    }
                    if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                    if (Item_Rece_Error_Check__01("Rece") == false) return;

                    if (cls_app_static_var.Rec_info_Multi_TF == 0)
                    {
                        //if (csd.Check_Stock_Output_Rece(txt_OrderNumber.Text.Trim(), int.Parse(txt_RecIndex.Text.Trim())) == false)
                        //{
                        //    butt_Rec_Save.Focus(); return;
                        //}
                        Base_Sub_Edit_Rece();
                    }
                    else
                    {
                        Base_Sub_Edit_Rece(1);
                    }

                    Base_Sub_Clear("Rece");
                    Base_Sub_Clear("item");
                    Save_Button_Click_Cnt++;

                    //if (Save_Button_Click_Cnt == 1)
                    //{
                    //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit")
                    //                 + "\n" +
                    //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                    //}
                }
            }
        }



        private void Base_Small_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Ord_Clear")
            {
                Card_Ok_Visible = true;
                Base_Ord_Clear();

            }

            else if (bt.Name == "butt_Cacu_Clear")
            {
                Base_Sub_Clear("Cacu");
            }

            else if (bt.Name == "butt_Cacu_Del")
            {
                if (txt_C_index.Text == "") return;
                Base_Sub_Delete("Cacu");
                Base_Sub_Sum_Cacu();


            }

            else if (bt.Name == "butt_Cacu_Save")
            {
                int C_index = 0;
                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                if (Item_Rece_Error_Check__01("Cacu") == false) return;

                if (txt_C_index.Text == "") //추가 일경우에 새로운 입력
                {


                    if (double.Parse(txt_Price_1.Text.Trim().Replace(",", "")) > 0)  //현금이다
                        Base_Sub_Save_Cacu(1);

                    if (double.Parse(txt_Price_2.Text.Trim().Replace(",", "")) > 0)  //무통장이다
                        Base_Sub_Save_Cacu(2);

                    if (double.Parse(txt_Price_3.Text.Trim().Replace(",", "")) > 0)  //카드이다
                    {
                        if (txt_C_Card_Ap_Num.Text == "" && txt_C_P_Number.Text != "")
                        {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                if (MessageBox.Show("There is no authorization number related to the card. Is it correct to add payment information? Would you like to proceed?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                            }
                            else
                            {
                                if (MessageBox.Show("카드 관련 승인번호가 없습니다. 결제정보 추가가 맞습니까? 계속 진행하시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                            }
                        }
                        Base_Sub_Save_Cacu(3);
                    }
                    if (double.Parse(txt_Price_4.Text.Trim().Replace(",", "")) > 0)  //카드이다
                        Base_Sub_Save_Cacu(4);
                    if (double.Parse(txt_Price_6.Text.Trim().Replace(",", "")) > 0)  //쿠폰이다.
                    {
                        
                        if (MP_YN == "Y")
                        {
                            //20211012 주문아이템중 MP아이템이 하나라도 들어가면이 아니고, 모두가 MP아이템이면 발동
                            //MessageBox.Show("주문아이템들에 MP아이템이 존재합니다.");
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                MessageBox.Show("All order items are MP items, so coupons cannot be used.");
                            }
                            else
                            {
                                MessageBox.Show("모든 주문아이템들이 MP아이템이라 쿠폰을 사용 할 수 없습니다.");
                            }
                            txt_Price_6.Text = "";
                            mtxtPriceDate6.Text = "";
                            txt_Price_6_2.Text = "";
                            return;
                        }
                        else
                        {


                            cls_Connect_DB Temp_Connect5 = new cls_Connect_DB();
                            string Tsql5 = "select  overprice   from TLS_COUPON (nolock) where couponnumber =  '" + txt_Price_6_2.Text + "'   ";
                            DataSet ds4 = new DataSet();
                            if (Temp_Connect5.Open_Data_Set(Tsql5, "TLS_COUPON", ds4) == false) return;
                            int ReCnt4 = Temp_Connect5.DataSet_ReCount;

                            int overprice = int.Parse(ds4.Tables["TLS_COUPON"].Rows[0][0].ToString().Replace(".0000",""));

                            if (double.Parse(txt_SumPr.Text.Trim().Replace(",", "")) < overprice && double.Parse(txt_Price_6.Text.Trim().Replace(",", ""))  == 3000)
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("This coupon can be used for purchases of " + overprice.ToString() + " won or more.");
                                }
                                else
                                {

                                    MessageBox.Show("" + overprice.ToString() + " 원 이상 구매 시 사용 가능한 쿠폰입니다.");
                                }
                                txt_Price_6.Text = "";
                                mtxtPriceDate6.Text = "";
                                txt_Price_6_2.Text = "";
                                return;
                            }
                            if (double.Parse(txt_SumPr.Text.Trim().Replace(",", "")) < overprice && double.Parse(txt_Price_6.Text.Trim().Replace(",", "")) == 10000)
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("This coupon can be used for purchases of " + overprice.ToString() + " won or more.");
                                }
                                else
                                {
                                    MessageBox.Show("" + overprice.ToString() + " 원 이상 구매 시 사용 가능한 쿠폰입니다.");
                                }
                                txt_Price_6.Text = "";
                                mtxtPriceDate6.Text = "";
                                txt_Price_6_2.Text = "";
                                return;
                            }
                            if (double.Parse(txt_SumPr.Text.Trim().Replace(",", "")) < overprice && double.Parse(txt_Price_6.Text.Trim().Replace(",", "")) == 20000)
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("This coupon can be used for purchases of " + overprice.ToString() + " won or more.");
                                }
                                else
                                {
                                    MessageBox.Show("" + overprice.ToString() + " 원 이상 구매 시 사용 가능한 쿠폰입니다.");
                                }
                                txt_Price_6.Text = "";
                                mtxtPriceDate6.Text = "";
                                txt_Price_6_2.Text = "";
                                return;
                            }
                            

                            cls_Connect_DB Temp_Connect4 = new cls_Connect_DB();
                            string Tsql2 = "select couponnumber   from TLS_COUPON (nolock) where couponnumber =  '" + txt_Price_6_2.Text + "' and  USEENDDATE > GETDATE()   ";
                            DataSet ds2 = new DataSet();
                            if (Temp_Connect4.Open_Data_Set(Tsql2, "tbl_salesdetail", ds2) == false) return;
                            int ReCnt2 = Temp_Connect4.DataSet_ReCount;
                            if (ReCnt2 == 0)
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("The coupon has expired."
                       + "\n" + "To use it, please change the end time in Coupon Change"
                  );
                                }
                                else
                                {
                                    MessageBox.Show("해당쿠폰은 종료일이 지났습니다."
                              + "\n" + "사용하시려면 쿠폰변경에서 종료시간을 변경해주세요"
                          );
                                }
                                    return;
                             
                            }
                       

                            Base_Sub_Save_Cacu(6);
                        }
                    }
                
                    Base_Sub_Sum_Cacu();
                    Base_Sub_Clear("Cacu");
                    Save_Button_Click_Cnt++;

                    //추가후 바로 가도록 변경함  2017-03-27 요청에 의해서
                    txt_Receive_Method.Focus();

                    //if (Save_Button_Click_Cnt == 1)
                    //{
                    //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
                    //                + "\n" +
                    //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                    //}

                }
                else  //
                {
                    C_index = int.Parse(txt_C_index.Text);
                    Base_Sub_Edit_Cacu();
                    if(couponnumber == "1")
                    {
                        couponnumber = "0";
                        return;
                    }
                    Base_Sub_Clear("Cacu");
                    Base_Sub_Sum_Cacu();
                    Save_Button_Click_Cnt++;

                    //if (Save_Button_Click_Cnt == 1)
                    //{
                    //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit")
                    //                 + "\n" +
                    //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                    //}
                }
            }

            else if (bt.Name == "butt_AddCode")
            {
                frmBase_AddCode e_f = new frmBase_AddCode();
                e_f.Send_Address_Info += new frmBase_AddCode.SendAddressDele(e_f_Send_Address_Info);
                e_f.ShowDialog();
            }

        }

        private void e_f_Send_Address_Info(string AddCode1, string AddCode2, string Address1, string Address2, string Address3)
        {
            Data_Set_Form_TF = 1;
            mtxtZip1.Text = AddCode1 + "-" + AddCode2;
            txtAddress1.Text = Address1; txtAddress2.Text = Address2;
            Data_Set_Form_TF = 0;
            txtAddress2.Focus();
        }




        private void opt_Rec_Add1_MouseUp(object sender, MouseEventArgs e)
        {
            RadioButton t_rb = (RadioButton)sender;

            //회원을 선택 안햇네 그럼 회원 넣어라
            if (txtName.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Mem")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                t_rb.Checked = false;
                mtxtMbid.Focus(); return;
            }

            if (txt_Receive_Method_Code.Text == "1")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("This is direct receipt.");
                }
                else
                {
                    MessageBox.Show("직접수령 건입니다.");
                }
                return;
            }


            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";
            DataSet ds = new DataSet();
            int ReCnt = 0;

            if (t_rb.Name == "opt_Rec_Add1")
            {
                Tsql = "Select Addcode1 ,Address1 , Address2 , Address3, state, city ";
                Tsql = Tsql + " ,hptel ,homeTel , M_Name ";
                Tsql = Tsql + " From tbl_Memberinfo (nolock ) ";

                if (idx_Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + idx_Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
                }

                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
                ReCnt = Temp_Connect.DataSet_ReCount;
            }
            else if (t_rb.Name == "opt_Rec_Add2")
            {
                /*
                Tsql = "Select ETC_Addcode1 Addcode1 , ETC_Address1 Address1 , ETC_Address2 Address2 , ETC_Address3 Address3 ";
                Tsql = Tsql + " ,ETC_Tel_1 hptel ,ETC_Tel_2 homeTel , ETC_Name M_Name ";
                Tsql = Tsql + " From tbl_Memberinfo_Address (nolock ) ";

                if (idx_Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo_Address.Mbid2 = " + idx_Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo_Address.Mbid = '" + idx_Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo_Address.Mbid2 = " + idx_Mbid2.ToString();
                }
                Tsql = Tsql + " And   Sort_Add  = 'R' ";
                */

                Tsql = "select top 1 ";
                Tsql += "ISNULL(Get_ZipCode, '') as 'Addcode1', ";
                Tsql += "ISNULL(Get_Address1, '') as 'Address1', ";
                Tsql += "ISNULL(Get_Address2, '') as 'Address2', ";
                Tsql += "'' as 'Address3', ";
                Tsql += "ISNULL(Get_Tel1, '') as 'hptel', ";
                Tsql += "ISNULL(Get_Tel2, '') as 'homeTel', ";
                Tsql += "ISNULL(Get_Name1, '') as 'M_Name', ";
                Tsql += "ISNULL(Get_state, '') AS 'state', ";
                Tsql += "ISNULL(Get_city, '') AS 'city' ";
                Tsql += "from tbl_Sales_Rece (nolock) ";
                Tsql += "where OrderNumber = (select max(OrderNumber) from tbl_SalesDetail (nolock) where ReturnTF = 1 and mbid2 = '" + idx_Mbid2.ToString() + "') ";


                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
                ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0)
                {
                    Tsql = "Select ETC_Addcode1 Addcode1 , ETC_Address1 Address1 , ETC_Address2 Address2 , ETC_Address3 Address3 ";
                    Tsql = Tsql + " ,ETC_Tel_1 hptel ,ETC_Tel_2 homeTel , ETC_Name M_Name ";
                    Tsql = Tsql + " From tbl_Memberinfo_Address (nolock ) ";

                    if (idx_Mbid.Length == 0)
                        Tsql = Tsql + " Where tbl_Memberinfo_Address.Mbid2 = " + idx_Mbid2.ToString();
                    else
                    {
                        Tsql = Tsql + " Where tbl_Memberinfo_Address.Mbid = '" + idx_Mbid + "' ";
                        Tsql = Tsql + " And   tbl_Memberinfo_Address.Mbid2 = " + idx_Mbid2.ToString();
                    }
                    Tsql = Tsql + " And   Sort_Add  = 'R' ";

                    if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
                    ReCnt = Temp_Connect.DataSet_ReCount;
                }

            }
            else if (t_rb.Name == "opt_Rec_Add3")
            {
                Data_Set_Form_TF = 1;
                mtxtZip1.Text = "";
                txtAddress1.Text = ""; txtAddress2.Text = "";
                mtxtTel1.Text = "";
                mtxtTel2.Text = "";
                txt_Get_Name1.Text = "";
                //cbZipCode_TH.Text = "";
                //cbCity_TH.Text = "";
                //cbState_TH.Text = "";
                //cbZipCode_TH.SelectedIndex = -1;
                txtZipCode_TH.Text = "";
                cbDistrict_TH.SelectedIndex = -1;
                cbProvince_TH.SelectedIndex = -1;
                Data_Set_Form_TF = 0;
            }

            Data_Set_Form_TF = 1;
            mtxtZip1.Text = "";
            txtAddress1.Text = ""; txtAddress2.Text = "";
            mtxtTel1.Text = "";
            mtxtTel2.Text = "";
            txt_Get_Name1.Text = "";
            //cbZipCode_TH.Text = "";
            //cbCity_TH.Text = "";
            //cbState_TH.Text = "";
            //cbZipCode_TH.SelectedIndex = -1;
            txtZipCode_TH.Text = "";
            cbDistrict_TH.SelectedIndex = -1;
            cbProvince_TH.SelectedIndex = -1;
            cbSubDistrict_TH.SelectedIndex = -1;
            Data_Set_Form_TF = 0;

            if (ReCnt == 0) return;

            Data_Set_Form_TF = 1;

            string sNaCode = cls_User.gid_CountryCode; //ds.Tables["t_P_table"].Rows[0]["Na_Code"].ToString();

            txtAddress1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address1"].ToString());
            txtAddress2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address2"].ToString());

            // 태국인 경우
            if (sNaCode == "TH")
            {
                //cbProvince_TH.Text = ds.Tables["t_P_table"].Rows[0]["state"].ToString();
                //cbDistrict_TH.Text = ds.Tables["t_P_table"].Rows[0]["city"].ToString();
                try
                {
                    cbProvince_TH.Text = ds.Tables["t_P_table"].Rows[0]["address2"].ToString().Split(' ')[2];
                    cbDistrict_TH.Text = ds.Tables["t_P_table"].Rows[0]["address2"].ToString().Split(' ')[1];
                    cbSubDistrict_TH.Text = ds.Tables["t_P_table"].Rows[0]["address2"].ToString().Split(' ')[0];
                }
                catch (Exception)
                {
                    cbProvince_TH.Text = "";
                    cbDistrict_TH.Text = "";
                    cbSubDistrict_TH.Text = "";
                }


                txtZipCode_TH.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Replace("-", "");
            }
            // 그 외 국가인 경우
            else
            {
                mtxtZip1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Replace("-", "");
            }

            
            //string AddCode = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Replace("-", "");
            //if (AddCode.Length >= 6)
            //{
            //    mtxtZip1.Text = AddCode.Substring(0, 3) + "-" + AddCode.Substring(3, 3);
            //    //txtAddCode1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(0, 3);
            //    //txtAddCode2.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(3, 3);
            //}

            //string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
            cls_form_Meth cfm = new cls_form_Meth();
            //cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            //cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

            cfm.Home_Number_Setting(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString(), mtxtTel1);
            cfm.Home_Number_Setting(ds.Tables["t_P_table"].Rows[0]["hometel"].ToString(), mtxtTel2);

            if (t_rb.Name == "opt_Rec_Add2")
                txt_Get_Name1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString()); //주소테이블의 배송자명은 암호화 햇기 대문에
            else
                txt_Get_Name1.Text = ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString();  //회원 테이블의 회원명은 암호화 안햇음
            Data_Set_Form_TF = 0;
        }




        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;

            cls_form_Meth ct = new cls_form_Meth();

            if (dtp.Name == "DTP_SellDate")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, mtxtSellDate2);

            if (dtp.Name == "DTP_SellDate2")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, txtSellCode);


            if (dtp.Name == "DTP_PriceDate3")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, txt_C_Card);

            if (dtp.Name == "DTP_PriceDate1")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, butt_Cacu_Save);

            if (dtp.Name == "DTP_PriceDate2")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, txt_C_Name_2);

            if (dtp.Name == "DTP_PriceDate4")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, butt_Cacu_Save);
            if (dtp.Name == "DTP_PriceDate6")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender, butt_Cacu_Save);
            if (dtp.Name == "DTP_IssueDate")
                ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
        }




        private Boolean Check_TextBox_Error()
        {
            //주문종류 , 회원, 주문일자 입력 안햇는지 체크
            if (Base_Error_Check__01() == false) return false;


            //if (txtSellCode_Code.Text == "Auto")
            //{
            //    if (txt_OrderNumber.Text == "")
            //    {
            //        //MessageBox.Show("오토쉽 관련 매출의 입력 또는 수정이 불가능 합니다."
            //        //      + "\n" +
            //        //      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //        //txtSellCode.Focus();
            //        //return false;
            //    }
            //    else
            //    {


            //        foreach (int t_key in SalesItemDetail.Keys)
            //        {
            //            if (SalesItemDetail[t_key].Del_TF == "D" || SalesItemDetail[t_key].Del_TF == "S")  //직접 수령이다.
            //            {
            //                if (SalesDetail[txt_OrderNumber.Text.Trim()].SellTF == 0)
            //                {
            //                    MessageBox.Show("오토쉽 관련 매출의 입력 또는 수정이 불가능 합니다."
            //                        + "\n" +
            //                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //                    txtSellCode.Focus();
            //                    return false;
            //                }
            //            }
            //        }

            //        foreach (int t_key in Sales_Cacu.Keys)
            //        {
            //            if (Sales_Cacu[t_key].Del_TF == "D" || Sales_Cacu[t_key].Del_TF == "U" || Sales_Cacu[t_key].Del_TF == "S")  //직접 수령이다.
            //            {
            //                if (SalesDetail[txt_OrderNumber.Text.Trim()].SellTF == 0)
            //                {
            //                    MessageBox.Show("오토쉽 관련 매출의 입력 또는 수정이 불가능 합니다."
            //                        + "\n" +
            //                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //                    txtSellCode.Focus();
            //                    return false;
            //                }
            //            }
            //        }
            //    }
            //}


            //회원번호 관련 관련 오류 체크 및 존재 여부 그리고 탈퇴 여부(신규 저장일 경우에)                      
            if (Input_Error_Check(mtxtMbid, "m", 1) == false) return false;

            if (Input_Error_Check_Save() == false) return false;

            if (Input_Error_Check_Save___02() == false) return false;


            if (txt_InputPass_Pay.Text == "")
                txt_InputPass_Pay.Text = "0";



            return true;
        }

        private bool Input_Error_Check_Save()
        {
            //그리드 상에 선택한 상품이 한개라도 잇는지..
            if (dGridView_Base_Item.RowCount == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Goods")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txt_ItemCode.Focus(); return false;
            }

            if (dGridView_Base_Rece.RowCount == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                      + Environment.NewLine + "-" + "배송정보 "
                      + cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                dGridView_Base_Rece.Focus(); return false;
            }


            //foreach (int t_key in Sales_Rece.Keys)
            //{
            //    if (Sales_Rece[t_key].Del_TF != "D" || Sales_Rece[t_key].Receive_Method == 2 )  //택배이다 수령이다.
            //    {
            //        if (Sales_Rece[t_key].Get_Tel1 == "" && Sales_Rece[t_key].Get_Tel2 == "" )
            //        {
            //            MessageBox.Show("배송 선택시 연락처를 필히 입력해 주십시요."
            //                + "\n" +
            //                cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //            mtxtTel1.Focus();
            //            return false;
            //        }
            //    }
            //}

            //2016-06-07 주문한 품목수량과 배송 정보 수가 일치하지 않으면 저장안되게
            int item_cnt = 0, rece_cnt = 0;
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                    item_cnt++;
            }
            foreach (int t_key in Sales_Rece.Keys)
            {
                if (Sales_Rece[t_key].Del_TF != "D")
                    rece_cnt++;
            }

            //if (item_cnt != rece_cnt)
            //{
            //    MessageBox.Show("품목과 배송 정보 수가 일치하지 않습니다."
            //            + "\n" +
            //            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    return false;
            //}




            //if (dGridView_Base_Rece.RowCount == 0)
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
            //           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Rece_Sort")
            //          + "\n" +
            //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    txt_ItemCode.Focus(); return false;
            //}



            if (
                (txtSellCode_Code.Text == "" && txtSellCode.Text.Trim() != "")
                ||
                (txtSellCode_Code.Text != "" && txtSellCode.Text.Trim() == "")
                )
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellCode")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellCode.Focus(); return false;
            }


            ////2016-01-21 김선웅씨랑 통화후. 필수 입력으로 변경함. 주문센타를 등록 할수 잇도록.
            //if (
            //    (txtCenter2_Code.Text == "" && txtCenter2.Text.Trim() != "")
            //    ||
            //    (txtCenter2_Code.Text != "" && txtCenter2.Text.Trim() == "")
            //    ||
            //    (txtCenter2_Code.Text == "" && txtCenter2.Text.Trim() == "")
            //    )
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
            //           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellCenter")
            //          + "\n" +
            //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    txtCenter2.Focus(); return false;
            //}


            if (txt_OrderNumber.Text.Trim() != "")
            {

                //교환이나 부분반품 반품 건에 대해서는 현 화면에서 수정을 못하게함.
                string Tsql = "";
                Tsql = "select ReturnTF, Ga_Order, RecordTime from tbl_SalesDetail  (nolock) ";
                Tsql = Tsql + " Where OrderNumber = '" + txt_OrderNumber.Text.Trim() + "' ";

                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt != 0)
                {
                    //2020-08-12 주문건 수정 불가로직 추가
                    string Ga_Order = ds.Tables[base_db_name].Rows[0]["Ga_Order"].ToString();
                    string ReturnTF = ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString();
                    DateTime RecordTime = DateTime.Parse(ds.Tables[base_db_name].Rows[0]["RecordTime"].ToString());
                    DateTime NowTime = DateTime.Now;

                    //구현호  - (NowTime.Hour <= 15 && NowTime.Minute <= 30)조건은...분이 30분이상이면 다 false 처리한다..
                    string hhmm = DateTime.Now.ToString("hhmm");
                    int now_hhmm = int.Parse(hhmm);

                    bool bRecordTime_Fri_Satur_Sun_Day = false; // 주문한게 금,토,일인 경우, 3시이후 주문인경우 월요일까지 변경 가능하게끔 조치함
                    if (RecordTime.DayOfWeek == DayOfWeek.Friday && RecordTime.Hour <= 15)
                    {
                        bRecordTime_Fri_Satur_Sun_Day = true;
                    }
                    else if(RecordTime.DayOfWeek == DayOfWeek.Saturday || RecordTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        bRecordTime_Fri_Satur_Sun_Day = true;
                    }

                    if (Ga_Order == "0" && ReturnTF == "1" && cls_User.gid != cls_User.SuperUserID)
                    {
                        bool bInsertOrUpdate = false;
                        bool bPass = false;

                        foreach (int t_key in Sales_Rece.Keys)
                        {
                            if (Sales_Rece[t_key].Del_TF == "I" || Sales_Rece[t_key].Del_TF == "U")
                            {
                                bInsertOrUpdate = true;
                                break;
                            }
                        }

                        //배송지 변경이 이루어진 경우 
                        if (bInsertOrUpdate)
                        {
                            //평일 오후 3시 이전 주문건은 당일출고이루어짐, 평일 3시반까지 주소취소와 배송지변경이 가능함
                            if (RecordTime.ToString("yyyyMMdd") == NowTime.ToString("yyyyMMdd") &&
                                //RecordTime.Hour < 15 && (NowTime.Hour <= 15 && NowTime.Minute <= 30) && bRecordTime_Fri_Satur_Sun_Day == false
                                RecordTime.Hour < 15 && (now_hhmm <= 1530) && bRecordTime_Fri_Satur_Sun_Day == false
                                )
                            {
                                bPass = true;
                            }
                            else
                            {
                                if(bRecordTime_Fri_Satur_Sun_Day)
                                {
                                    //금요일 오후 3시 이후부터 토요일 일요일의 주문건은  월요일 오후 3시 반까지 주소취소와 배송지변경이 가능함
                                    //if (NowTime.DayOfWeek == DayOfWeek.Monday && NowTime.Hour <= 15 && NowTime.Minute <= 30)
                                        if (NowTime.DayOfWeek == DayOfWeek.Monday && now_hhmm <= 1530)
                                        {
                                        bPass = true;
                                    }
                                    else if (NowTime.DayOfWeek == DayOfWeek.Friday //금
                                        || NowTime.DayOfWeek == DayOfWeek.Saturday //토
                                        || NowTime.DayOfWeek == DayOfWeek.Sunday ) //일요일은 무조건 수정가능
                                    {
                                        bPass = true;
                                    }

                                }
                                else
                                {
                                    //평일 오후 3시 이후 주문건은 익일출고이루어짐, 평일 익일 3시반까지 주소취소와 배송지변경이 가능함
                                    //if (RecordTime.ToString("yyyyMMdd") == NowTime.AddDays(-1).ToString("yyyyMMdd") && (NowTime.Hour <= 15 && NowTime.Minute <= 30))
                                    if (RecordTime.ToString("yyyyMMdd") == NowTime.AddDays(-1).ToString("yyyyMMdd") && (now_hhmm <= 1530))
                                    {
                                        bPass = true;
                                    }
                                    //평일 오후 3시 이후 주문건은 익일출고이루어짐, 평일 익일 3시반까지 주소취소와 배송지변경이 가능함
                                    if (RecordTime.ToString("yyyyMMdd") == NowTime.ToString("yyyyMMdd") && RecordTime.Hour >= 15)
                                    {
                                        bPass = true;
                                    }

                                }
                            }
                        }

                        if (bPass == false)
                        {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                MessageBox.Show("* Approved orders cannot be modified. * " + Environment.NewLine +
                             "Orders placed before 3pm on weekdays are shipped the same day, so you can only cancel the address or change the delivery address by 3:30pm on weekdays" + Environment.NewLine +
                             "Orders after 3:00 PM on weekdays are shipped the next day, so you can only cancel the address and change the shipping address by 3:30 PM the next day on weekdays."
                        );
                            }
                            else
                            {

                                MessageBox.Show("* 승인된 주문건은 수정하실 수 없습니다. * " + Environment.NewLine +
                                    "평일 오후 3시 이전 주문건은 당일출고이루어기에 평일 3시반까지 주소취소와 배송지변경만이 가능하며" + Environment.NewLine +
                                    "평일 오후 3시 이후 주문건은 익일출고이루어기에 평일 익일 3시반까지 주소취소와 배송지변경만이 가능합니다"
                                );
                            }
                            return false;
                        }
                    }

                    if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "2")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_2")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtCenter2.Focus(); return false;
                    }

                    if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "3")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_3")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtCenter2.Focus(); return false;
                    }

                    if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "4")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_4")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtCenter2.Focus(); return false;
                    }

                    if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "5")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_5")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtCenter2.Focus(); return false;
                    }
                }

                //현 내역을 반품이나 부분반품 교환을 햇다. 그럼 현내역 역시 수정 못하게함.
                Tsql = "select SellDate from tbl_SalesDetail  (nolock) ";
                Tsql = Tsql + " Where Re_BaseOrderNumber = '" + txt_OrderNumber.Text.Trim() + "' ";

                //++++++++++++++++++++++++++++++++

                ds.Clear();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt != 0)
                {

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_1")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtCenter2.Focus(); return false;

                }
            }

            int Card_App_Cnt = 0;
            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D") //삭제이다
                {
                    if (Sales_Cacu[t_key].Sugi_TF != "")
                        Card_App_Cnt++;
                }
            }

            //if (Card_App_Cnt > 1 )
            //{

            //    MessageBox.Show("현 화면상에서 카드 자동 승인 신청은 한주문당 한건만 가능 합니다."
            //            + "\n" +
            //            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    dGridView_Base_Cacu.Focus(); return false;

            //}

            return true;
        }

        private bool Input_Error_Check_Save___02()
        {
            cls_Search_DB csd = new cls_Search_DB();

            //if (cls_app_static_var.Sell_Union_Flag == "U" || cls_app_static_var.Sell_Union_Flag == "D")
            //{

            //        if (txt_UnaccMoney.Text == "")
            //            txt_UnaccMoney.Text = "0";

            //        if (double.Parse(txt_UnaccMoney.Text.Trim()) > 0)
            //        {
            //            MessageBox.Show("조합 관련 신고로 미수금 또는 + 금액이 존재하면 안됩니다."
            //                                + "\n" +
            //                                cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //            dGridView_Base_Cacu.Focus();
            //            return false;
            //        }
            //}

            if (txt_OrderNumber.Text.Trim() == "")
            {
                ////마감정산이 이루어진 판매 날짜인지 체크한다.                
                //if (csd.Close_Check_SellDate("tbl_CloseTotal_02", mtxtSellDate2.Text.Trim().Replace("-", "")) == false)
                //{
                //    mtxtSellDate2.Focus(); return false;
                //}

                //if (csd.Close_Check_SellDate("tbl_CloseTotal_04", mtxtSellDate.Text.Trim().Replace("-", "")) == false)
                //{
                //    txtCenter2.Focus(); return false;
                //}
            }
            else
            {

                double Be_P = 0, Be_IN_P = 0; double Cur_P = 0, Cur_in_P = 0;


                Be_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalPrice;
                Be_IN_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalInputPrice;
                Cur_P = double.Parse(txt_TotalPrice.Text.Trim().Replace(",", ""));
                Cur_in_P = double.Parse(txt_TotalInputPrice.Text.Trim().Replace(",", ""));

                //공제번호가 발행이된 상태에서 제품의 수정이 들어 갓다.
                int idx_Chang_Item_TF = 0;
                foreach (int t_key in SalesItemDetail.Keys)
                {
                    if (SalesItemDetail[t_key].Del_TF == "D") //삭제이다
                    {
                        idx_Chang_Item_TF = 1;
                    }
                    else if (SalesItemDetail[t_key].Del_TF == "S")  //새로운 저장이다
                    {
                        idx_Chang_Item_TF = 1;
                    }
                }

                if (idx_Chang_Item_TF == 1 || Be_P != Cur_P) //금액의 변경이 이루어지거나 제품이 새롭게 추가나 삭제처리가 되엇다.
                {
                    if (csd.Check_Stock_OutPut(txt_OrderNumber.Text.Trim()) == false)  //재고출고처리가 되었다 그럼 변경 못하게 막는다.
                    {
                        butt_Save.Focus(); return false;
                    }
                }



                string Be_SellDate = SalesDetail[txt_OrderNumber.Text.Trim()].SellDate_2.Replace("-", "");

                if (Be_SellDate != mtxtSellDate.Text.Replace("-", "").Trim())
                {
                    //if (SalesDetail[txt_OrderNumber.Text.Trim()].SellTF == 0)
                    //{
                    //    if (csd.Close_Check_SellDate("tbl_CloseTotal_02", Be_SellDate) == false)
                    //    {
                    //        mtxtSellDate2.Focus(); return false;
                    //    }
                    //}

                    //if (csd.Close_Check_SellDate("tbl_CloseTotal_02", mtxtSellDate2.Text.Trim().Replace("-", "")) == false)
                    //{
                    //    mtxtSellDate2.Focus(); return false;
                    //}

                    //if (SalesDetail[txt_OrderNumber.Text.Trim()].SellTF == 0)
                    //{
                    //    if (csd.Close_Check_SellDate("tbl_CloseTotal_04", Be_SellDate) == false)
                    //    {
                    //        txtCenter2.Focus(); return false;
                    //    }
                    //}

                    //if (csd.Close_Check_SellDate("tbl_CloseTotal_04", mtxtSellDate.Text.Trim().Replace("-", "")) == false)
                    //{
                    //    txtCenter2.Focus(); return false;
                    //}
                }

                if (cls_app_static_var.Sell_Union_Flag == "U")
                {
                    //공제번호가 발급 된내역인데.. 금액 수정을 할려고 한다. 그럼 공제 취소하고 다시 하라고 알려준다.
                    cls_form_Meth cm = new cls_form_Meth();
                    if (txt_Ins_Number.Text.Trim() != "" && txt_Ins_Number.Text.Trim() != cm._chang_base_caption_search("미신고"))
                    {
                        Be_P = 0; Cur_P = 0; Cur_in_P = 0; Be_IN_P = 0;


                        Be_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalPrice;
                        Be_IN_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalInputPrice;
                        Cur_P = double.Parse(txt_TotalPrice.Text.Trim().Replace(",", ""));
                        Cur_in_P = double.Parse(txt_TotalInputPrice.Text.Trim().Replace(",", ""));








                        if (idx_Chang_Item_TF == 1 || Be_P != Cur_P || Cur_in_P != Be_IN_P) //금액의 변경이 이루어지거나 제품이 새롭게 추가나 삭제처리가 되엇다.
                        {
                            string t_Msg = "";
                            t_Msg = "조합 신고후 공제번호가 발급된 매출입니다." + "\n" +
                                "제품의 변경이나 삭제 시 조합 신고 관련해서 일치하지 않는 정보가 존재하게 됩니다." + "\n" +
                                "변경이나 삭제를 원하시면 공제 번호를 취소후에 다시 시도해 주십시요." + "\n" +
                                "확인후 다시 시도해 주십시요.";

                            MessageBox.Show(t_Msg);
                        }


                        Be_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalPrice;
                        Cur_P = double.Parse(txt_TotalPrice.Text.Trim().Replace(",", ""));
                        if (Be_P != Cur_P)
                        {
                            string S_SellDate = SalesDetail[txt_OrderNumber.Text.Trim()].SellDate.Replace("-", "");
                            S_SellDate = S_SellDate.Substring(0, 4) + '-' + S_SellDate.Substring(4, 2) + '-' + S_SellDate.Substring(6, 2);
                            string S_SellDate2 = cls_User.gid_date_time.Substring(0, 4) + '-' + cls_User.gid_date_time.Substring(4, 2) + '-' + cls_User.gid_date_time.Substring(6, 2);

                            cls_Date_G date_G = new cls_Date_G();
                            double dif = date_G.DateDiff("d", DateTime.Parse(S_SellDate), DateTime.Parse(S_SellDate2));

                            if (dif > 2)
                            {
                                while (DateTime.Parse(S_SellDate) <= DateTime.Parse(S_SellDate2))
                                {
                                    int r_d = date_G.Check_Date_HolyDay_TF(DateTime.Parse(S_SellDate));
                                    dif = dif + r_d;

                                    DateTime TodayDate = new DateTime();
                                    TodayDate = DateTime.Parse(S_SellDate);
                                    S_SellDate = TodayDate.AddDays(1).ToString("yyyy-MM-dd");
                                }
                            }

                            if (dif > 2) //2영업일이 지난내역은 걍 저장시켜준다. 대신 조합측에 알아서 하라고 메시지 뛰운다.
                            {
                                string t_Msg = "";
                                t_Msg = "현재일 기준으로 2영업일이 지난 판매 내역 입니다." + "\n" +
                                    "현재 내역은 프로그램 상으로 저장을 하나 조합측에는 신고 할 수 없습니다." + "\n" +
                                    "조합측에 별도로 문의해주시기 바랍니다.";

                                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(t_Msg), "", MessageBoxButtons.YesNo) == DialogResult.No) return false;
                            }
                            else // 2영업일이 안지낫고 매출 신고 되었다.. 그럼 매출 취소 신청하고 다시하라고한다.
                            {
                                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Chang_Insur_Number")
                                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Sell_Price")
                                        + " " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Not_Chang")
                                        + "\n" +
                                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                txtCenter2.Focus(); return false;
                            }
                        }
                    }

                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////


                if (SalesDetail[txt_OrderNumber.Text.Trim()].Ga_Order == 1)
                {
                    Boolean Cl_TF2 = csd.Close_Check_SellDate("tbl_CloseTotal_02", mtxtSellDate2.Text.Trim().Replace("-", ""), 1);
                    Boolean Cl_TF4 = csd.Close_Check_SellDate("tbl_CloseTotal_04", mtxtSellDate.Text.Trim().Replace("-", ""), 1);

                    //이미 마감돈 날짜이면 금액이나 PV 관련 수정이 되면 마감 못돌게 한다.
                    if (Cl_TF2 == false || Cl_TF4 == false)
                    {
                        Be_P = 0; Cur_P = 0;
                        Be_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalPV;
                        Cur_P = double.Parse(txt_TotalPv.Text.Trim().Replace(",", ""));


                        if (Be_P != Cur_P)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Close_Date")
                                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Sell_PV")
                                    + " " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Not_Chang")
                                    + "\n" +
                                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtCenter2.Focus(); return false;
                        }

                        Be_P = SalesDetail[txt_OrderNumber.Text.Trim()].TotalPrice;
                        Cur_P = double.Parse(txt_TotalPrice.Text.Trim().Replace(",", ""));

                        if (Be_P != Cur_P)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Close_Date")
                                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Sell_Price")
                                    + " " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Not_Chang")
                                    + "\n" +
                                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtCenter2.Focus(); return false;
                        }


                        if (SalesDetail[txt_OrderNumber.Text.Trim()].SellCode != txtSellCode_Code.Text.Trim())
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Close_Date")
                                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellCode")
                                    + " " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Not_Chang")
                                    + "\n" +
                                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtSellCode.Focus(); return false;
                        }

                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////

            }
            return true;
        }

        private bool Input_Error_Check_Save___03()
        {
            //cls_Search_DB csd = new cls_Search_DB();

            /*2016-06-23 작업
            등록된 건을 수정할때 
            미승인 건이면서 주문일자가 오늘날짜가 아니면 메세지창 띄우고 저장 못하게
            본사직원은 수정가능하게
            */
            //if (txt_OrderNumber.Text.Trim() != "")
            //{
            //    if (mtxtSellDate.Text.Replace("-", "").Trim() != cls_User.gid_date_time && cls_User.gid_CenterCode != "")
            //    {
            //        if (SalesDetail[txt_OrderNumber.Text.Trim()].SellTF == 1)
            //        {
            //            MessageBox.Show("주문일자가 당일날짜와 일치하지 않습니다.본사에 문의 바랍니다.");
            //            return false;
            //        }
            //    }
            //}
            return true;
        }



        private void Input_SalesDetail_dic()
        {
            cls_form_Meth ct = new cls_form_Meth();

            double Total_Sell_VAT_Price = 0; double Total_Sell_Except_VAT_Price = 0;
            double InputCash = 0; double InputPassbook = 0; double InputCard = 0; ; double InputMile = 0, InputPassbook_2 = 0;
            double InputCoupon = 0; ;
            double InputNaver = 0, InputPayment_8_TH = 0, InputPayment_9_TH = 0, InputPayment_10_TH = 0;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    Total_Sell_VAT_Price = Total_Sell_VAT_Price + SalesItemDetail[t_key].Total_Sell_VAT_Price;
                    Total_Sell_Except_VAT_Price = Total_Sell_Except_VAT_Price + SalesItemDetail[t_key].Total_Sell_Except_VAT_Price;
                }
            }

            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D")
                {
                    if (Sales_Cacu[t_key].C_TF == 1)
                        InputCash = InputCash + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 2)
                        InputPassbook = InputPassbook + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 3)
                        InputCard = InputCard + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 4)
                        InputMile = InputMile + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 5)
                        InputPassbook_2 = InputPassbook_2 + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 6)
                        InputCoupon = InputCoupon + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 7)
                        InputNaver += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 8)
                        InputPayment_8_TH += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 9)
                        InputPayment_9_TH += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 10)
                        InputPayment_10_TH += Sales_Cacu[t_key].C_Price1;

                }
            }

            cls_Sell t_c_sell = new cls_Sell();

            t_c_sell.OrderNumber = "";

            t_c_sell.Mbid = idx_Mbid;
            t_c_sell.Mbid2 = idx_Mbid2;
            t_c_sell.Na_Code = idx_Na_Code;
            t_c_sell.M_Name = txtName.Text.Trim();



            t_c_sell.SellCode = txtSellCode_Code.Text.Trim();
            t_c_sell.SellCodeName = txtSellCode.Text.Trim();

            t_c_sell.SellSort = "";
            if (radioB_DESK.Checked == true)
                t_c_sell.SellSort = "DESK";

            if (radioB_CALL.Checked == true)
                t_c_sell.SellSort = "CALL";



            //판매센타입력 사항이 없으면 걍 회원센타로 지정을 한다.
            if (txtCenter2_Code.Text.Trim() != "")
            {
                t_c_sell.BusCode = txtCenter2_Code.Text.Trim();
                t_c_sell.BusCodeName = txtCenter2.Text.Trim();
            }
            else
            {
                t_c_sell.BusCode = txtCenter_Code.Text.Trim();
                t_c_sell.BusCodeName = txtCenter.Text.Trim();
            }
            t_c_sell.Re_BaseOrderNumber = "";
            t_c_sell.TotalPrice = double.Parse(txt_TotalPrice.Text.Trim().Replace(",", ""));
            t_c_sell.TotalPV = double.Parse(txt_TotalPv.Text.Trim().Replace(",", ""));
            t_c_sell.TotalCV = double.Parse(txt_TotalCV.Text.Trim().Replace(",", ""));
            t_c_sell.TotalInputPrice = double.Parse(txt_TotalInputPrice.Text.Trim().Replace(",", ""));
            t_c_sell.Total_Sell_VAT_Price = Total_Sell_VAT_Price;
            t_c_sell.Total_Sell_Except_VAT_Price = Total_Sell_Except_VAT_Price;
            t_c_sell.InputCash = InputCash;
            t_c_sell.InputCard = InputCard;
            t_c_sell.InputPassbook = InputPassbook;
            t_c_sell.InputPassbook_2 = InputPassbook_2;
            t_c_sell.InputMile = InputMile;
            t_c_sell.Be_InputMile = 0;

            t_c_sell.InputPass_Pay = double.Parse(txt_InputPass_Pay.Text.Trim().Replace(",", ""));
            t_c_sell.UnaccMoney = double.Parse(txt_UnaccMoney.Text.Trim().Replace(",", ""));


            t_c_sell.InputCoupon = InputCoupon;
            t_c_sell.InputNaver = InputNaver;
            t_c_sell.InputPayment_8_TH  = InputPayment_8_TH ;
            t_c_sell.InputPayment_9_TH  = InputPayment_9_TH ;
            t_c_sell.InputPayment_10_TH = InputPayment_10_TH;


            t_c_sell.Etc1 = txt_ETC1.Text.Trim();
            t_c_sell.Etc2 = txt_ETC2.Text.Trim();

            t_c_sell.ReturnTF = 1;
            t_c_sell.ReturnTFName = ct._chang_base_caption_search("정상");
            t_c_sell.INS_Num = "";
            t_c_sell.InsuranceNumber_Date = "";
            t_c_sell.W_T_TF = 0;
            t_c_sell.In_Cnt = 0;

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.SellDate = mtxtSellDate.Text.Replace("-", "").Trim();
            t_c_sell.SellDate_2 = mtxtSellDate2.Text.Replace("-", "").Trim();
            t_c_sell.SellDate = mtxtSellDate.Text.Replace("-", "").Trim();

            t_c_sell.Del_TF = "S";
            SalesDetail[""] = t_c_sell;
        }

        private void Update_SalesDetail_dic()
        {
            double Total_Sell_VAT_Price = 0; double Total_Sell_Except_VAT_Price = 0;
            double InputCash = 0; double InputPassbook = 0; double InputCard = 0; double InputMile = 0, InputPassbook_2 = 0; double InputCoupon = 0;
            double InputNaver = 0, InputPayment_8_TH = 0, InputPayment_9_TH = 0, InputPayment_10_TH = 0;


            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    Total_Sell_VAT_Price = Total_Sell_VAT_Price + SalesItemDetail[t_key].Total_Sell_VAT_Price;
                    Total_Sell_Except_VAT_Price = Total_Sell_Except_VAT_Price + SalesItemDetail[t_key].Total_Sell_Except_VAT_Price;
                }
            }

            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D")
                {
                    if (Sales_Cacu[t_key].C_TF == 1)
                        InputCash = InputCash + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 2)
                        InputPassbook = InputPassbook + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 3)
                        InputCard = InputCard + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 4)
                        InputMile = InputMile + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 5)
                        InputPassbook_2 = InputPassbook_2 + Sales_Cacu[t_key].C_Price1;
                    if (Sales_Cacu[t_key].C_TF == 6)
                        InputCoupon = InputCoupon + Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 7)
                        InputNaver += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 8)
                        InputPayment_8_TH  += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 9)
                        InputPayment_9_TH += Sales_Cacu[t_key].C_Price1;

                    if (Sales_Cacu[t_key].C_TF == 10)
                        InputPayment_10_TH += Sales_Cacu[t_key].C_Price1;
                }
            }


            string OrderNumber = txt_OrderNumber.Text.Trim();

            SalesDetail[OrderNumber].Mbid = idx_Mbid;
            SalesDetail[OrderNumber].Mbid2 = idx_Mbid2;
            SalesDetail[OrderNumber].Na_Code = idx_Na_Code;
            SalesDetail[OrderNumber].M_Name = txtName.Text.Trim();

            SalesDetail[OrderNumber].SellCode = txtSellCode_Code.Text.Trim();
            SalesDetail[OrderNumber].SellCodeName = txtSellCode.Text.Trim();

            if (radioB_DESK.Checked == true)
                SalesDetail[OrderNumber].SellSort = "DESK";

            if (radioB_CALL.Checked == true)
                SalesDetail[OrderNumber].SellSort = "CALL";


            SalesDetail[OrderNumber].BusCode = txtCenter2_Code.Text.Trim();
            SalesDetail[OrderNumber].BusCodeName = txtCenter2.Text.Trim();
            SalesDetail[OrderNumber].Re_BaseOrderNumber = "";

            SalesDetail[OrderNumber].TotalPrice = double.Parse(txt_TotalPrice.Text.Trim().Replace(",", ""));
            SalesDetail[OrderNumber].TotalPV = double.Parse(txt_TotalPv.Text.Trim().Replace(",", ""));
            SalesDetail[OrderNumber].TotalCV = double.Parse(txt_TotalCV.Text.Trim().Replace(",", ""));
            SalesDetail[OrderNumber].TotalInputPrice = double.Parse(txt_TotalInputPrice.Text.Trim().Replace(",", ""));
            SalesDetail[OrderNumber].Total_Sell_VAT_Price = Total_Sell_VAT_Price;
            SalesDetail[OrderNumber].Total_Sell_Except_VAT_Price = Total_Sell_Except_VAT_Price;

            SalesDetail[OrderNumber].InputCash = InputCash;
            SalesDetail[OrderNumber].InputPassbook = InputPassbook;
            SalesDetail[OrderNumber].InputCard = InputCard;
            SalesDetail[OrderNumber].InputMile = InputMile;
            SalesDetail[OrderNumber].InputPassbook_2 = InputPassbook_2;

            SalesDetail[OrderNumber].InputCoupon = InputCoupon;
            SalesDetail[OrderNumber].InputNaver = InputNaver;
            SalesDetail[OrderNumber].InputPayment_8_TH = InputPayment_8_TH;
            SalesDetail[OrderNumber].InputPayment_9_TH = InputPayment_9_TH;
            SalesDetail[OrderNumber].InputPayment_10_TH = InputPayment_10_TH;

            SalesDetail[OrderNumber].InputPass_Pay = double.Parse(txt_InputPass_Pay.Text.Trim().Replace(",", ""));
            SalesDetail[OrderNumber].UnaccMoney = double.Parse(txt_UnaccMoney.Text.Trim().Replace(",", ""));

            SalesDetail[OrderNumber].Etc1 = txt_ETC1.Text.Trim();
            SalesDetail[OrderNumber].Etc2 = txt_ETC2.Text.Trim();

            SalesDetail[OrderNumber].SellDate = mtxtSellDate.Text.Replace("-", "").Trim();
            SalesDetail[OrderNumber].SellDate_2 = mtxtSellDate2.Text.Replace("-", "").Trim();

            if (SalesDetail[OrderNumber].Del_TF == "")
                SalesDetail[OrderNumber].Del_TF = "U";
        }


        private void DB_Save_tbl_SalesDetail(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, ref string T_ord_N)
        {

            if (txt_OrderNumber.Text.Trim() != "")
                T_ord_N = txt_OrderNumber.Text.Trim();

            else
            {
                DataSet ds = new DataSet();
                cls_Connect_DB Temp_Conn2 = new cls_Connect_DB();
                string StrSql = string.Format("EXEC Usp_Insert_Tbl_Sales_OrderNumber_CS '{0}', '{1}', '{2}', '{3}'",
                    idx_Mbid,
                    idx_Mbid2,
                    mtxtSellDate.Text.Replace("-", "").Trim(),
                    txtSellCode_Code.Text
                    );
                if (Temp_Conn2.Open_Data_Set(StrSql, "tbl_Sales_OrdNumber", ds) == false) return;

                if (Temp_Conn2.DataSet_ReCount == 0) return;

                T_ord_N = SalesDetail[T_ord_N].OrderNumber = ds.Tables["tbl_Sales_OrdNumber"].Rows[0]["OrderNumber"].ToString();
            }
        }

        private void DB_Save_tbl_SalesDetail____002(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran, string OrderNumber)
        {
            string StrSql = "";
            if (txt_OrderNumber.Text.Trim() == "")
            {
                string Ins_Ordernumber = "";

                StrSql = "INSERT INTO tbl_SalesDetail";
                StrSql = StrSql + " (OrderNumber,Mbid,Mbid2,M_Name,SellDate,SellDate_2,SellCode, SellSort ,BusCode,Na_Code,";
                StrSql = StrSql + " TotalPrice,TotalPV,TotalCV,TotalInputPrice,";
                StrSql = StrSql + " Total_Sell_VAT_Price,Total_Sell_Except_VAT_Price, ";
                StrSql = StrSql + " InputCash,InputCard,InputPassbook , InputPassbook_2, InputMile,UnaccMoney,InputPass_Pay,InputCoupon,";
                StrSql = StrSql + " Etc1,Etc2, ";
                StrSql = StrSql + " ReturnTF,InsuranceNumber,InsuranceNumber_Date,";
                StrSql = StrSql + " RecordID,RecordTime, Ga_Order, Primium_custom , PV_CV_Check ";
                StrSql += ", InputPayment_8_TH, InputPayment_9_TH, InputPayment_10_TH, InputNaver";
                StrSql = StrSql + " ) Values ( ";
                StrSql = StrSql + "'" + SalesDetail[Ins_Ordernumber].OrderNumber + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Mbid + "'";
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Mbid2;
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].M_Name + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellDate + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellDate_2 + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellCode + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellSort + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].BusCode + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Na_Code + "'";
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalPrice;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalPV;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalCV;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalInputPrice;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Total_Sell_VAT_Price;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Total_Sell_Except_VAT_Price;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputCash;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputCard;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPassbook;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPassbook_2;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputMile;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].UnaccMoney;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPass_Pay;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputCoupon;

                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Etc1 + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Etc2 + "'";

                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].ReturnTF;
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].INS_Num + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].InsuranceNumber_Date + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].RecordID + "'";

                StrSql = StrSql + ",Convert(Varchar(25), GetDate(), 21) ";
                if (SalesDetail[Ins_Ordernumber].UnaccMoney == 0)
                    StrSql = StrSql + ",0";
                else
                    StrSql = StrSql + ",1";
                StrSql = StrSql + ",0, ";

                if (chK_PV_CV_Check.Checked == true)
                {
                    PV_CV_Check = 1;
                }
                else
                {
                    PV_CV_Check = 0;
                }
                StrSql = StrSql + "" + PV_CV_Check + "";
                

                //2023-11-23 지성경. 왜 네이버가읍어...필드를 만들었으면 다 넣어줘야지 아무리 의미가없더라도
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPayment_8_TH;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPayment_9_TH;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPayment_10_TH;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputNaver;

                StrSql = StrSql + ")";

                //    if (chK_PV_CV_Check.Checked == true)
                //{
                //    PV_CV_Check = 1;
                //}
                //else
                //{
                //    PV_CV_Check = 0;
                //}
                //    StrSql = StrSql + ""+ PV_CV_Check+"";
                //    StrSql = StrSql + ")";

                if (Temp_Connect.Insert_Data(StrSql, "tbl_SalesDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;



                StrSql = "INSERT INTO tbl_SalesDetail_TF (OrderNumber,SellTF)";
                StrSql = StrSql + "  Values ( ";
                if (cls_app_static_var.Sell_TF_CS_Flag == "")  // ""이면 CS에서 등록되는 건들은 다 승인  그게 아니면 다 미승인으로 처리함.
                    StrSql = StrSql + "'" + SalesDetail[Ins_Ordernumber].OrderNumber + "',1)";
                else
                    StrSql = StrSql + "'" + SalesDetail[Ins_Ordernumber].OrderNumber + "',0)";

                if (Temp_Connect.Insert_Data(StrSql, "tbl_SalesDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            }
            else
            {
                cls_Search_DB csd = new cls_Search_DB();

                //수정하기 전에 배열에다가 내역을 받아둔다.
                csd.SalesDetail_Mod_BackUp(OrderNumber, "tbl_SalesDetail");


                StrSql = "Update tbl_SalesDetail Set ";
                StrSql = StrSql + " SellDate = '" + SalesDetail[OrderNumber].SellDate.Replace("-", "") + "'";
                StrSql = StrSql + ",SellDate_2 = '" + SalesDetail[OrderNumber].SellDate_2.Replace("-", "") + "'";
                StrSql = StrSql + ",TotalPrice = " + SalesDetail[OrderNumber].TotalPrice;
                StrSql = StrSql + ",TotalPV= " + SalesDetail[OrderNumber].TotalPV;
                StrSql = StrSql + ",TotalcV= " + SalesDetail[OrderNumber].TotalCV;
                StrSql = StrSql + ",TotalInputPrice= " + SalesDetail[OrderNumber].TotalInputPrice;

                StrSql = StrSql + ",Total_Sell_VAT_Price= " + SalesDetail[OrderNumber].Total_Sell_VAT_Price;
                StrSql = StrSql + ",Total_Sell_Except_VAT_Price= " + SalesDetail[OrderNumber].Total_Sell_Except_VAT_Price;

                StrSql = StrSql + ",InputCash= " + SalesDetail[OrderNumber].InputCash;
                StrSql = StrSql + ",InputCard= " + SalesDetail[OrderNumber].InputCard;
                StrSql = StrSql + ",InputPassbook= " + SalesDetail[OrderNumber].InputPassbook;
                StrSql = StrSql + ",InputPassbook_2 = " + SalesDetail[OrderNumber].InputPassbook_2;
                StrSql = StrSql + ",InputMile= " + SalesDetail[OrderNumber].InputMile;
                StrSql = StrSql + ",UnaccMoney= " + SalesDetail[OrderNumber].UnaccMoney;
                StrSql = StrSql + ",InputPass_Pay= " + SalesDetail[OrderNumber].InputPass_Pay;
                StrSql = StrSql + ",InputCoupon= " + SalesDetail[OrderNumber].InputCoupon;
                StrSql = StrSql + ",InputNaver= " + SalesDetail[OrderNumber].InputNaver;
                StrSql = StrSql + ",InputPayment_8_TH = " +  SalesDetail[OrderNumber].InputPayment_8_TH ;
                StrSql = StrSql + ",InputPayment_9_TH = " +  SalesDetail[OrderNumber].InputPayment_9_TH ;
                StrSql = StrSql + ",InputPayment_10_TH = " + SalesDetail[OrderNumber].InputPayment_10_TH;

                StrSql = StrSql + ",SellSort = '" + SalesDetail[OrderNumber].SellSort + "'";
                StrSql = StrSql + ",SellCode = '" + SalesDetail[OrderNumber].SellCode + "'";

                StrSql = StrSql + ",Etc1= '" + SalesDetail[OrderNumber].Etc1 + "'";
                StrSql = StrSql + ",Etc2= '" + SalesDetail[OrderNumber].Etc2 + "'";

                StrSql = StrSql + ",BusCode = '" + SalesDetail[OrderNumber].BusCode + "'";
                StrSql = StrSql + ",PV_CV_Check = " + PV_CV_Check + "";
                StrSql = StrSql + " Where OrderNumber = '" + SalesDetail[OrderNumber].OrderNumber + "'";

                if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

                //주테이블의 변경 내역을 테이블에 넣는다.
                csd.SalesDetail_Mod(Conn, tran, OrderNumber, "tbl_SalesDetail");



            }
        }


        private void DB_Save_tbl_Mileage____001(cls_Connect_DB Temp_Connect,
                                            SqlConnection Conn, SqlTransaction tran, string OrderNumber)
        {

            if (txt_OrderNumber.Text.Trim() == "")
            {
                string Ins_Ordernumber = "";

                if (SalesDetail[Ins_Ordernumber].InputMile > 0)
                {
                    cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                    ctm.Put_Minus_Mileage(SalesDetail[Ins_Ordernumber].Mbid, SalesDetail[Ins_Ordernumber].Mbid2, SalesDetail[Ins_Ordernumber].M_Name
                        , SalesDetail[Ins_Ordernumber].InputMile, SalesDetail[Ins_Ordernumber].OrderNumber, "12"
                        , Temp_Connect, Conn, tran, "", this.Name.ToString(), this.Text);
                }


            }
            else
            {

                if (SalesDetail[OrderNumber].InputMile > SalesDetail[OrderNumber].Be_InputMile)
                {
                    cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                    ctm.Put_Minus_Mileage(SalesDetail[OrderNumber].Mbid, SalesDetail[OrderNumber].Mbid2, SalesDetail[OrderNumber].M_Name
                        , SalesDetail[OrderNumber].InputMile - SalesDetail[OrderNumber].Be_InputMile, SalesDetail[OrderNumber].OrderNumber, "16"
                        , Temp_Connect, Conn, tran, "", this.Name.ToString(), this.Text);
                }

                if (SalesDetail[OrderNumber].InputMile < SalesDetail[OrderNumber].Be_InputMile)
                {
                    cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                    ctm.Put_Plus_Mileage(SalesDetail[OrderNumber].Mbid, SalesDetail[OrderNumber].Mbid2, SalesDetail[OrderNumber].M_Name
                        , SalesDetail[OrderNumber].Be_InputMile - SalesDetail[OrderNumber].InputMile, SalesDetail[OrderNumber].OrderNumber, "15"
                        , Temp_Connect, Conn, tran, "", this.Name.ToString(), this.Text);
                }
            }
        }



        private void DB_Save_tbl_SalesItemDetail(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber)
        {

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF == "D") //삭제이다
                {
                    //백업데이블에 백업 받고 삭제 처리한다.
                    DB_Save_tbl_SalesItemDetail____D(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (SalesItemDetail[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_SalesItemDetail____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (SalesItemDetail[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_SalesItemDetail____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
            }
        }

        private void DB_Save_tbl_SalesItemDetail____D(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";

            StrSql = "Insert into tbl_SalesitemDetail_Mod_Del  ";
            StrSql = StrSql + " Select * ,0,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) From tbl_SalesitemDetail (nolock)";
            StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            StrSql = StrSql + " And   SalesItemIndex = " + SalesItemIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_SalesitemDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = "Delete From tbl_SalesitemDetail";
            StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            StrSql = StrSql + " And   SalesItemIndex = " + SalesItemIndex;

            if (Temp_Connect.Delete_Data(StrSql, "tbl_SalesitemDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }



        private void DB_Save_tbl_SalesItemDetail____U(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";

            cls_Search_DB csd = new cls_Search_DB();
            string T_where = " And SalesItemIndex = " + SalesItemIndex.ToString();
            //수정하기 전에 배열에다가 내역을 받아둔다.
            csd.SalesDetail_Mod_BackUp(OrderNumber, "tbl_SalesitemDetail", T_where);


            StrSql = "Update tbl_SalesItemDetail Set ";

            StrSql = StrSql + " ItemCode= '" + SalesItemDetail[SalesItemIndex].ItemCode + "'";
            StrSql = StrSql + ",ItemPrice= " + SalesItemDetail[SalesItemIndex].ItemPrice;
            StrSql = StrSql + ",ItemName= '" + SalesItemDetail[SalesItemIndex].ItemName + "'";
            StrSql = StrSql + ",ItemPv= " + SalesItemDetail[SalesItemIndex].ItemPV;
            StrSql = StrSql + ",Itemcv= " + SalesItemDetail[SalesItemIndex].ItemCV;

            StrSql = StrSql + ",Sell_VAT_Price= " + SalesItemDetail[SalesItemIndex].Sell_VAT_Price;
            StrSql = StrSql + ",Sell_Except_VAT_Price= " + SalesItemDetail[SalesItemIndex].Sell_Except_VAT_Price;

            StrSql = StrSql + ",Total_Sell_VAT_Price= " + SalesItemDetail[SalesItemIndex].Total_Sell_VAT_Price;
            StrSql = StrSql + ",Total_Sell_Except_VAT_Price= " + SalesItemDetail[SalesItemIndex].Total_Sell_Except_VAT_Price;

            StrSql = StrSql + ",ItemTotalPrice= " + SalesItemDetail[SalesItemIndex].ItemTotalPrice;
            StrSql = StrSql + ",ItemTotalPV= " + SalesItemDetail[SalesItemIndex].ItemTotalPV;
            StrSql = StrSql + ",ItemTotalcV= " + SalesItemDetail[SalesItemIndex].ItemTotalCV;

            StrSql = StrSql + ",ItemCount= " + SalesItemDetail[SalesItemIndex].ItemCount;
            StrSql = StrSql + ",RecIndex= " + SalesItemDetail[SalesItemIndex].RecIndex;

            //    StrSql = StrSql + ",Send_itemCount1= " + Send_itemCount1;
            //    StrSql = StrSql + ",Send_itemCount2= " + Send_itemCount2;

            StrSql = StrSql + ",SellState= '" + SalesItemDetail[SalesItemIndex].SellState + "'";
            StrSql = StrSql + ",SendDate= '" + SalesItemDetail[SalesItemIndex].SendDate + "'";
            StrSql = StrSql + ",ETC= '" + SalesItemDetail[SalesItemIndex].Etc + "'";
            StrSql = StrSql + ",G_Sort_Code= '" + SalesItemDetail[SalesItemIndex].G_Sort_Code + "'";

            StrSql = StrSql + " Where OrderNumber = '" + SalesItemDetail[SalesItemIndex].OrderNumber + "'";
            StrSql = StrSql + " And SalesItemIndex = " + SalesItemIndex.ToString();

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

            //주문 상품 테이블의 변경 내역을 테이블에 넣는다.
            csd.tbl_SalesDetail_Total_Change(Conn, tran, OrderNumber, SalesItemIndex, "tbl_SalesitemDetail", T_where);
        }


        private void DB_Save_tbl_SalesItemDetail____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";


            StrSql = "Insert Into tbl_SalesitemDetail (";
            StrSql = StrSql + " SalesItemIndex,OrderNumber,";
            //20230314구현호
            StrSql = StrSql + " ItemCode,ItemPrice,ItemName,ItemPv,ItemCv,";
            StrSql = StrSql + " Sell_VAT_TF , Sell_VAT_Price, Sell_Except_VAT_Price,SellState,";
            StrSql = StrSql + " ItemCount,ItemTotalPrice,ItemTotalPV,ItemTotalcV,";
            StrSql = StrSql + " Total_Sell_VAT_Price, Total_Sell_Except_VAT_Price,";
            StrSql = StrSql + " ReturnDate,SendDate,ReturnBackDate,";
            StrSql = StrSql + " Etc,RecIndex,";
            StrSql = StrSql + " Send_itemCount1,Send_itemCount2, ";
            StrSql = StrSql + " T_OrderNumber1,T_OrderNumber2,G_Sort_Code";
            StrSql = StrSql + " ,RecordID,RecordTime ";
            StrSql = StrSql + " ) values(";

            StrSql = StrSql + SalesItemDetail[SalesItemIndex].SalesItemIndex;
            StrSql = StrSql + ",'" + OrderNumber + "'";

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].ItemCode + "'";
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemPrice;
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].ItemName + "'";
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemPV;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemCV;

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Sell_VAT_TF;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Sell_VAT_Price;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Sell_Except_VAT_Price;

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].SellState + "'";

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemCount;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemTotalPrice;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemTotalPV;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemTotalCV;

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Total_Sell_VAT_Price;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Total_Sell_Except_VAT_Price;

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].ReturnDate + "'";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].SendDate + "'";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].ReturnBackDate + "'";

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].Etc + "'";
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].RecIndex;

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Send_itemCount1;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Send_itemCount2;

            StrSql = StrSql + ",'" + OrderNumber + "'";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].T_OrderNumber2 + "'";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].G_Sort_Code + "'";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].RecordID + "'";
            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_SalesItemDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;

        }






        private void DB_Save_tbl_Sales_Cacu(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber)
        {

            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF == "D") //삭제이다
                {
                    //백업데이블에 백업 받고 삭제 처리한다.
                    DB_Save_tbl_Sales_Cacu____D(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Cacu[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_Sales_Cacu____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Cacu[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_Sales_Cacu____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
            }
        }






        private void DB_Save_tbl_Sales_Cacu____D(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {
            string StrSql = "";

            StrSql = "Insert into tbl_Sales_Cacu_Mod_Del  ";
            StrSql = StrSql + " Select * ,0,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) From tbl_Sales_Cacu (nolock)";
            StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            StrSql = StrSql + " And   C_index = " + C_index;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Sales_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = "Delete From tbl_Sales_Cacu";
            StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            StrSql = StrSql + " And   C_index = " + C_index;

            if (Temp_Connect.Delete_Data(StrSql, "tbl_Sales_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        } 



        private void DB_Save_tbl_Sales_Cacu____U(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {
            string StrSql = "";

            cls_Search_DB csd = new cls_Search_DB();
            string T_where = " And C_index = " + C_index.ToString();
            //수정하기 전에 배열에다가 내역을 받아둔다.
            csd.SalesDetail_Mod_BackUp(OrderNumber, "tbl_Sales_Cacu", T_where);


            StrSql = "Update tbl_Sales_Cacu Set ";

            StrSql = StrSql + " C_TF= " + Sales_Cacu[C_index].C_TF;
            StrSql = StrSql + ",C_Price1= " + Sales_Cacu[C_index].C_Price1;
            StrSql = StrSql + ",C_Price2= " + Sales_Cacu[C_index].C_Price2;

            StrSql = StrSql + ",C_AppDate1= '" + Sales_Cacu[C_index].C_AppDate1.Replace("-", "") + "'";
            StrSql = StrSql + ",C_AppDate2= '" + Sales_Cacu[C_index].C_AppDate2.Replace("-", "") + "'";

            StrSql = StrSql + ",C_CodeName= '" + Sales_Cacu[C_index].C_CodeName + "'";

            StrSql = StrSql + ",C_Number1= dbo.ENCRYPT_AES256('" + Sales_Cacu[C_index].C_Number1 + "') ";
            StrSql = StrSql + ",C_Number2= '" + Sales_Cacu[C_index].C_Number2 + "'";
            //StrSql = StrSql + ",C_Number3= '" + encrypter.Encrypt( Sales_Cacu[C_index].C_Number3) + "'";

            StrSql = StrSql + ",C_Cash_Send_Nu= '" + Sales_Cacu[C_index].C_Cash_Send_Nu + "'";
            StrSql = StrSql + ",C_Cash_Send_TF= " + Sales_Cacu[C_index].C_Cash_Send_TF;
            StrSql = StrSql + ",C_Cash_Sort_TF= " + Sales_Cacu[C_index].C_Cash_Sort_TF;
            StrSql = StrSql + ",C_Cash_Bus_TF= " + Sales_Cacu[C_index].C_Cash_Bus_TF;

            if (Sales_Cacu[C_index].C_Cash_Sort_TF == 100)
                StrSql = StrSql + ",C_Cash_Number= " + Sales_Cacu[C_index].C_Cash_Number;


            StrSql = StrSql + ",C_Name1= '" + Sales_Cacu[C_index].C_Name1 + "'";
            StrSql = StrSql + ",C_Name2= '" + Sales_Cacu[C_index].C_Name2 + "'";



            StrSql = StrSql + ",C_Code= '" + Sales_Cacu[C_index].C_Code + "'";
            StrSql = StrSql + ",C_Period1= '" + Sales_Cacu[C_index].C_Period1 + "'";
            StrSql = StrSql + ",C_Period2= '" + Sales_Cacu[C_index].C_Period2 + "'";
            StrSql = StrSql + ",C_Installment_Period= '" + Sales_Cacu[C_index].C_Installment_Period + "'";

            StrSql = StrSql + ",C_Etc= '" + Sales_Cacu[C_index].C_Etc + "'";
            StrSql = StrSql + ",C_P_Number= dbo.ENCRYPT_AES256('" + Sales_Cacu[C_index].C_P_Number + "') ";
            StrSql = StrSql + ",C_B_Number= dbo.ENCRYPT_AES256('" + Sales_Cacu[C_index].C_B_Number + "') ";

            StrSql = StrSql + ",C_CVC= '" + Sales_Cacu[C_index].C_CVC + "'";

            StrSql = StrSql + ",Sugi_TF= '" + Sales_Cacu[C_index].Sugi_TF + "'";
            if (chk_HanaMembership.Checked == true)
            {
                StrSql = StrSql + ",Associated_Card = 'hana'";
            }
            else
            {
                StrSql = StrSql + ",Associated_Card = ''";
            }
            ////StrSql = StrSql + ",C_CancelTF= " + Sales_Cacu[C_index].C_CancelTF;
            ////StrSql = StrSql + ",C_CancelDate= '" + Sales_Cacu[C_index].C_CancelDate + "'";
            ////StrSql = StrSql + ",C_CancelPrice= " + Sales_Cacu[C_index].C_CancelPrice;
            if (Sales_Cacu[C_index].C_Coupon == null)
            {
                StrSql = StrSql + ",''";
            }
            else
            {
                StrSql = StrSql + ",C_Coupon= '" + Sales_Cacu[C_index].C_Coupon + "'";
            }
            
            StrSql = StrSql + " Where OrderNumber = '" + Sales_Cacu[C_index].OrderNumber + "'";
            StrSql = StrSql + " And C_index = " + C_index.ToString();

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;
            
            //주문 상품 테이블의 변경 내역을 테이블에 넣는다.
            csd.tbl_SalesDetail_Total_Change(Conn, tran, OrderNumber, C_index, "tbl_Sales_Cacu", T_where);
       

        }


        private void DB_Save_tbl_Sales_Cacu____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {
            string StrSql = "";


            StrSql = "Insert Into tbl_Sales_Cacu (";
            StrSql = StrSql + " C_index,OrderNumber,";
            StrSql = StrSql + " C_TF,C_Code,C_CodeName,C_Name1,C_Name2,";
            StrSql = StrSql + " C_Number1 , C_Number2, ";
            StrSql = StrSql + " C_Price1,C_Price2,C_AppDate1,C_AppDate2, ";
            StrSql = StrSql + " C_CancelTF, C_CancelDate,C_CancelPrice, ";
            StrSql = StrSql + " C_Period1,C_Period2,C_Installment_Period,C_Etc";

            StrSql = StrSql + " ,C_Cash_Send_Nu,C_Cash_Send_TF,C_Cash_Sort_TF , C_Cash_Bus_TF ";

            StrSql = StrSql + " ,Sugi_TF , C_P_Number , C_B_Number   ";

            if (Sales_Cacu[C_index].C_Cash_Sort_TF == 100) StrSql = StrSql + " ,C_Cash_Number ";

            StrSql = StrSql + " ,RecordID,RecordTime ";
            StrSql = StrSql + ",Associated_Card,C_Coupon, C_CVC";
            StrSql = StrSql + " ) values(";

            StrSql = StrSql + "" + Sales_Cacu[C_index].C_index;
            StrSql = StrSql + ",'" + OrderNumber + "'";
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_TF;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Code + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_CodeName + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Name1 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Name2 + "'";

            if (Sales_Cacu[C_index].C_TF == 3)
                StrSql = StrSql + ", dbo.ENCRYPT_AES256('" + Sales_Cacu[C_index].C_Number1 + "') ";
            else
                StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Number1 + "'";

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Number2 + "'";
            //StrSql = StrSql + ",'" + encrypter.Encrypt( Sales_Cacu[C_index].C_Number2) + "'";
            //StrSql = StrSql + ",'" + encrypter.Encrypt( Sales_Cacu[C_index].C_Number3) + "'";

            StrSql = StrSql + "," + Sales_Cacu[C_index].C_Price1;
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_Price2;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_AppDate1.Replace("-", "") + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_AppDate2 + "'";

            StrSql = StrSql + "," + Sales_Cacu[C_index].C_CancelTF;
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_CancelDate + "'";
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_CancelPrice;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Period1 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Period2 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Installment_Period + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Etc + "'";

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Cash_Send_Nu + "'";
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_Cash_Send_TF;
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_Cash_Sort_TF;
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_Cash_Bus_TF;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].Sugi_TF + "'";
            StrSql = StrSql + ", dbo.ENCRYPT_AES256('" + Sales_Cacu[C_index].C_P_Number + "') ";
            StrSql = StrSql + ", dbo.ENCRYPT_AES256('" + Sales_Cacu[C_index].C_B_Number + "') ";


            if (Sales_Cacu[C_index].C_Cash_Sort_TF == 100) StrSql = StrSql + ", " + Sales_Cacu[C_index].C_Cash_Number;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].RecordID + "'";

            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            if (chk_HanaMembership.Checked == true)
            {
                StrSql = StrSql + ",'hana'";
            }
            else
            {
                StrSql = StrSql + ",''";
            }                         
            if (Sales_Cacu[C_index].C_Coupon == null)
            {
                StrSql = StrSql + ",''";
            }
            else
            {
                StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Coupon+ "'";
            }

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_CVC + "'";

            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Sales_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;

        }





        private void DB_Save_tbl_Sales_Coupon(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber)
        {

            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF == "D") //삭제이다
                {
                    //백업데이블에 백업 받고 삭제 처리한다.
                    DB_Save_tbl_Sales_Coupon____D(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Cacu[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_Sales_Coupon____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Cacu[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_Sales_Coupon____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
            }
        }
        private void DB_Save_tbl_Sales_Coupon____D(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {

     
       if (Sales_Cacu[C_index].C_TF == 6)
            {
                string StrSql = "";
                StrSql = "Delete From TLS_USE_COUPON WHERE COUPONNUMBER = '" + Sales_Cacu[C_index].C_Coupon + "'";
                if (Temp_Connect.Insert_Data(StrSql, "TLS_USE_COUPON", Conn, tran, this.Name.ToString(), this.Text) == false) return;

                string StrSqL3 = "";
                StrSqL3 = "UPDATE TLS_COUPON SET USEYN = 'N' WHERE COUPONNUMBER = '" + Sales_Cacu[C_index].C_Coupon + "' ";
                if (Temp_Connect.Insert_Data(StrSqL3, "TLS_USE_COUPON", Conn, tran, this.Name.ToString(), this.Text) == false) return;
            }
            else
            {
            }





            //string StrSql = "";

            //StrSql = "Insert into tbl_Sales_Cacu_Mod_Del  ";
            //StrSql = StrSql + " Select * ,0,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) From tbl_Sales_Cacu ";
            //StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            //StrSql = StrSql + " And   C_index = " + C_index;

            //if (Temp_Connect.Insert_Data(StrSql, "tbl_Sales_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            //StrSql = "Delete From tbl_Sales_Cacu";
            //StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            //StrSql = StrSql + " And   C_index = " + C_index;

            //if (Temp_Connect.Delete_Data(StrSql, "tbl_Sales_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }



        private void DB_Save_tbl_Sales_Coupon____U(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {
            //cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
            //string MBID2, MBID;

            //string StrSql2 = "SELECT MBID, MBID2 FROM TBL_SALESDETAIL WHERE ORDERNUMBER = '" + OrderNumber + "'";
            //DataSet ds2 = new DataSet();
            //Temp_Connect2.Open_Data_Set(StrSql2, "TBL_SALESDETAIL", ds2);
            //MBID = ds2.Tables["TBL_SALESDETAL"].Rows[0]["MBID"].ToString();
            //MBID2 = ds2.Tables["TBL_SALESDETAL"].Rows[0]["MBID2"].ToString();


            if (Sales_Cacu[C_index].C_TF == 6)
            {
                string StrSql = "";
                StrSql = "Delete From  [TLS_USE_COUPON] from [TLS_USE_COUPON] A INNER JOIN TBL_salesdetail B ON A.ordernumber = B.ordernumber WHERE B.ordernumber   = '" + OrderNumber + "'";
                if (Temp_Connect.Insert_Data(StrSql, "TLS_USE_COUPON", Conn, tran, this.Name.ToString(), this.Text) == false) return;

                string StrSqL2 = "";
                StrSqL2 = "UPDATE TLS_COUPON SET USEYN = 'N' From TLS_COUPON  A JOIN TBL_SALES_CACU B ON A.COUPONNUMBER = B.C_COUPON  WHERE B.ordernumber  = '" + OrderNumber + "' ";
                if (Temp_Connect.Update_Data(StrSqL2, Conn, tran, "", "") == false) return;

                string StrSqL4 = "";

                StrSqL4 = " UPDATE TLS_COUPON SET USEYN = 'Y'WHERE COUPONNUMBER = '" + Sales_Cacu[C_index].C_Coupon + "' ";
                if (Temp_Connect.Update_Data(StrSqL4, Conn, tran, "", "") == false) return;




                string StrSql3 = "";
                StrSql3 = "Insert Into TLS_USE_COUPON (";
                StrSql3 = StrSql3 + " MBID,MBID2,";
                StrSql3 = StrSql3 + " COUPONNUMBER,ORDERNUMBER,RECORDTIME ";
                StrSql3 = StrSql3 + " ) values(";
                StrSql3 = StrSql3 + "''";
                StrSql3 = StrSql3 + ",'" + mtxtMbid.Text.ToString() + "'";
                StrSql3 = StrSql3 + ",'" + Sales_Cacu[C_index].C_Coupon + "'";
                StrSql3 = StrSql3 + ",'" + OrderNumber + "'";
                StrSql3 = StrSql3 + ",Convert(Varchar(25),GetDate(),21) )";
                if (Temp_Connect.Insert_Data(StrSql3, "TLS_USE_COUPON", Conn, tran, this.Name.ToString(), this.Text) == false) return;

           

            }
            else
            {
            }
        }


        private void DB_Save_tbl_Sales_Coupon____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {
            //cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
            //string MBID2, MBID;

            //string StrSql2 = "SELECT MBID, MBID2 FROM TBL_SALESDETAIL WHERE ORDERNUMBER = '"+ OrderNumber + "'";
            //DataSet ds2 = new DataSet();
            //Temp_Connect2.Update_Data(StrSql2, "", "");

            //MBID = ds2.Tables["TBL_SALESDETAL"].Rows[0]["MBID"].ToString();
            //MBID2 = ds2.Tables["TBL_SALESDETAL"].Rows[0]["MBID2"].ToString();


            if (Sales_Cacu[C_index].C_TF == 6)
             {
                string StrSql = "";
                StrSql = "Insert Into TLS_USE_COUPON (";
                StrSql = StrSql + " MBID,MBID2,";
                StrSql = StrSql + " COUPONNUMBER,ORDERNUMBER,RECORDTIME ";
                StrSql = StrSql + " ) values(";
                StrSql = StrSql + "''";
                StrSql = StrSql + ",'" + mtxtMbid.Text.ToString() + "'";
                StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Coupon + "'";
                StrSql = StrSql + ",'" + OrderNumber + "'";
                StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) )";
                if (Temp_Connect.Insert_Data(StrSql, "TLS_USE_COUPON", Conn, tran, this.Name.ToString(), this.Text) == false) return;

                string StrSqL3 = "";
                StrSqL3 = "UPDATE TLS_COUPON SET USEYN = 'Y' WHERE COUPONNUMBER = '" + Sales_Cacu[C_index].C_Coupon + "' ";
                if (Temp_Connect.Insert_Data(StrSqL3, "TLS_COUPON", Conn, tran, this.Name.ToString(), this.Text) == false) return;
            }
            else
            {
            }
        }



        private void DB_Save_tbl_Sales_Rece(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber)
        {

            foreach (int t_key in Sales_Rece.Keys)
            {
                if (Sales_Rece[t_key].Del_TF == "D") //삭제이다
                {
                    //백업데이블에 백업 받고 삭제 처리한다.
                    DB_Save_tbl_Sales_Rece____D(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Rece[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_Sales_Rece____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Rece[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_Sales_Rece____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
            }
        }

        private void DB_Save_tbl_Sales_Rece____D(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";

            StrSql = "Insert into tbl_Sales_Rece_Mod_Del  ";
            StrSql = StrSql + " Select * ,0,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) From tbl_Sales_Rece ";
            StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            StrSql = StrSql + " And   SalesItemIndex = " + SalesItemIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Sales_Rece", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = "Delete From tbl_Sales_Rece";
            StrSql = StrSql + " Where OrderNumber = '" + OrderNumber + "'";
            StrSql = StrSql + " And   SalesItemIndex = " + SalesItemIndex;

            if (Temp_Connect.Delete_Data(StrSql, "tbl_Sales_Rece", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }



        private void DB_Save_tbl_Sales_Rece____U(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";

            cls_Search_DB csd = new cls_Search_DB();
            string T_where = " And SalesItemIndex = " + SalesItemIndex.ToString();
            //수정하기 전에 배열에다가 내역을 받아둔다.
            csd.SalesDetail_Mod_BackUp(OrderNumber, "tbl_Sales_Rece", T_where);


            StrSql = "Update tbl_Sales_Rece Set ";

            StrSql = StrSql + " Receive_Method= " + Sales_Rece[SalesItemIndex].Receive_Method;
            StrSql = StrSql + ",SalesItemIndex= " + Sales_Rece[SalesItemIndex].SalesItemIndex;
            StrSql = StrSql + ",Get_Name1=  '" + Sales_Rece[SalesItemIndex].Get_Name1 + "'";
            StrSql = StrSql + ",Get_Name2=  '" + Sales_Rece[SalesItemIndex].Get_Name2 + "'";

            StrSql = StrSql + ",Get_Date1= '" + Sales_Rece[SalesItemIndex].Get_Date1.Replace("-", "") + "'";
            StrSql = StrSql + ",Get_Date2= '" + Sales_Rece[SalesItemIndex].Get_Date2.Replace("-", "") + "'";

            StrSql = StrSql + ",Pass_Number= '" + Sales_Rece[SalesItemIndex].Pass_Number + "'";

            StrSql = StrSql + ",Get_ZipCode= '" + Sales_Rece[SalesItemIndex].Get_ZipCode.Replace("-", "") + "'";
            StrSql = StrSql + ",Get_Address1= '" + Sales_Rece[SalesItemIndex].Get_Address1 + "'";
            StrSql = StrSql + ",Get_Address2= '" + Sales_Rece[SalesItemIndex].Get_Address2 + "'";

            StrSql = StrSql + ",Get_Tel1= '" + Sales_Rece[SalesItemIndex].Get_Tel1 + "'";
            StrSql = StrSql + ",Get_Tel2= '" + Sales_Rece[SalesItemIndex].Get_Tel2 + "'";

            StrSql = StrSql + ",Get_Etc1= '" + Sales_Rece[SalesItemIndex].Get_Etc1 + "'";
            StrSql = StrSql + ",Get_Etc2= '" + Sales_Rece[SalesItemIndex].Get_Etc2 + "'";

            StrSql = StrSql + ",Get_city= '" + Sales_Rece[SalesItemIndex].Get_city + "'";
            StrSql = StrSql + ",Get_state= '" + Sales_Rece[SalesItemIndex].Get_state + "'";

            StrSql = StrSql + ",Pass_Pay= " + Sales_Rece[SalesItemIndex].Pass_Pay;
            StrSql = StrSql + ",Pass_Number2= '" + Sales_Rece[SalesItemIndex].Pass_Number2 + "'";

            StrSql = StrSql + ",Base_Rec= '" + Sales_Rece[SalesItemIndex].Base_Rec + "'";
            StrSql = StrSql + ",Receive_Center= '" + Sales_Rece[SalesItemIndex].Receive_Center + "'";

            StrSql = StrSql + " Where OrderNumber = '" + Sales_Rece[SalesItemIndex].OrderNumber + "'";
            StrSql = StrSql + " And SalesItemIndex = " + SalesItemIndex.ToString();

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

            //20220816 구현호 배송정보 변경시 호출mkorder로 변경내용 전달
            StrSql = " EXEC Usp_JDE_Update_MK_Ord_Rece '" + OrderNumber + "' ";
            Temp_Connect.Update_Data(StrSql, "", "");


            //주문 상품 테이블의 변경 내역을 테이블에 넣는다.
            csd.tbl_SalesDetail_Total_Change(Conn, tran, OrderNumber, SalesItemIndex, "tbl_Sales_Rece", T_where);
        }


        private void DB_Save_tbl_Sales_Rece____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";


            StrSql = "Insert Into tbl_Sales_Rece (";
            StrSql = StrSql + " RecIndex,SalesItemIndex,OrderNumber,";
            StrSql = StrSql + " Receive_Method,Get_Date1,Get_Date2,Get_Name1,Get_Name2,";
            StrSql = StrSql + " Get_ZipCode , Get_Address1, Get_Address2, ";
            StrSql = StrSql + " Get_Tel1,Get_Tel2,Get_Etc1,Get_Etc2, ";
            StrSql = StrSql + " Get_city, Get_state, ";
            StrSql = StrSql + " Pass_Pay,Pass_Number,Base_Rec, Receive_Center ";
            StrSql = StrSql + " ,RecordID,RecordTime ";
            StrSql = StrSql + " ) values(";


            StrSql = StrSql + "" + Sales_Rece[SalesItemIndex].RecIndex;

            StrSql = StrSql + "," + Sales_Rece[SalesItemIndex].SalesItemIndex;
            StrSql = StrSql + ",'" + OrderNumber + "'";

            StrSql = StrSql + "," + Sales_Rece[SalesItemIndex].Receive_Method;

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Date1.Replace("-", "") + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Date2.Replace("-", "") + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Name1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Name2 + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_ZipCode.Replace("-", "") + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Address1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Address2 + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Tel1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Tel2 + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Etc1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Etc2 + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_city + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_state + "'";

            StrSql = StrSql + "," + Sales_Rece[SalesItemIndex].Pass_Pay;
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Pass_Number + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Base_Rec + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Receive_Center + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].RecordID + "'";
            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Sales_Rece", Conn, tran, this.Name.ToString(), this.Text) == false) return;




        }



        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            string str_Q = "";

            if (txt_OrderNumber.Text == "")
                str_Q = "Msg_Base_Save_Q";
            else
                str_Q = "Msg_Base_Edit_Q";

            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(str_Q), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if(chK_PV_CV_Check.Checked == true)
            {
                PV_CV_Check = 1;
            }
            else
            {
                PV_CV_Check = 0;
            }

            if (Check_TextBox_Error() == false) return;  //각종 입력 오류를 체크한다.
            

            if (txt_OrderNumber.Text.Trim() == "")
            {
                if (idx_Mbid2 == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    mtxtMbid.Focus();
                    return;
                }
                Input_SalesDetail_dic();   //주문번호 ""으로 해서 판매 주 클래스 에 넣음
            }
            else
                Update_SalesDetail_dic();  //판매 주 클래스에 대한 수정 작업

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            int Com_TF = 0;
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();
            Com_TF = 1;
            string T_ord_N = "";

            cls_Search_DB csd = new cls_Search_DB();

            try
            {
                //저장할 것에 대한 주문번호를 따온다          
                DB_Save_tbl_SalesDetail(Temp_Connect, Conn, tran, ref T_ord_N);


                if (T_ord_N == "") //주문번호 미발급시 오류로 해서 되돌린다.  
                {
                    if (Com_TF == 1)
                        tran.Rollback();

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

                    tran.Dispose();
                    Temp_Connect.Close_DB();

                    return;
                }

                New_Ordernumber = T_ord_N;

                //실질적인 저장,수정이 이루어지는곳. 변경시 주테이블 이전 내역도 같이 저장함
                DB_Save_tbl_SalesDetail____002(Temp_Connect, Conn, tran, T_ord_N);
                
                DB_Save_tbl_SalesItemDetail(Temp_Connect, Conn, tran, T_ord_N);

                DB_Save_tbl_Sales_Cacu(Temp_Connect, Conn, tran, T_ord_N);
                                             
                DB_Save_tbl_Sales_Rece(Temp_Connect, Conn, tran, T_ord_N);
                               
                //쿠폰 내역을 저장한다
                DB_Save_tbl_Sales_Coupon(Temp_Connect, Conn, tran, T_ord_N);
                
                Temp_Ordernumber = T_ord_N;                                              

                tran.Commit();
                Com_TF = 0;

                SendCashReceipt_OK_Danal(Temp_Ordernumber);
                               

                Save_Error_Check = 1;
                if (txt_OrderNumber.Text == "")
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }
            catch (Exception)
            {
                if (Com_TF == 1)
                    tran.Rollback();

                if (txt_OrderNumber.Text == "")
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }

            if (cls_app_static_var.Sell_Union_Flag == "D" && (txt_Ins_Number.Text.Trim() == "" || txt_Ins_Number.Text.Trim().Substring(0, 7) == "재발급요청요망"))
            {
                if (idx_Na_Code.Replace(" ", "") == "KR")
                {
                    InsuranceNumber_Ord_Print_FLAG = T_ord_N ;
                    Sell_Ac_insurancenumber(T_ord_N);//직판 관련 승인 번호를 받아온다.                
                    InsuranceNumber_Ord_Print_FLAG = "";
                }
            }


        }



        private void Save_Stock_OutPut_Data_(cls_Connect_DB Temp_Connect,
                                           SqlConnection Conn, SqlTransaction tran, string OrderNumber)
        {
            int TSW = 0;
            int Clo_TF = 0;

            cls_Search_DB csd = new cls_Search_DB();


            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")  //직접 수령이다.
                {
                    //if (SalesItemDetail[t_key].Receive_Method == 1)
                    //{
                    TSW = 1;
                    //}
                }
            }



            if (Clo_TF == 0)  //마감이 안돈 내역에 대해서만
            {
                foreach (int t_key in SalesItemDetail.Keys)
                {
                    if (SalesItemDetail[t_key].Del_TF != "D")  //직접 수령이다.
                    {

                        try
                        {
                            //직접수령하고 배송일때만 자동 출고 처리를 한다.... 플라자(센터) 수령이랑 미입력일 경우에는 처리하지 않는다.
                            if (Sales_Rece[SalesItemDetail[t_key].SalesItemIndex].Receive_Method == 1)
                            {
                                if (csd.Check_Stock_OutPut(OrderNumber, SalesItemDetail[t_key].SalesItemIndex, 1) == true)
                                {
                                    Save_Stock_OutPut_Data(Temp_Connect, Conn, tran, OrderNumber, SalesItemDetail[t_key].SalesItemIndex);
                                }

                                TSW = 1;
                            }

                            //본사 직접수령인 경우에는 재고를 본사에서 떤다.
                            if (Sales_Rece[SalesItemDetail[t_key].SalesItemIndex].Receive_Method == 4)
                            {
                                if (csd.Check_Stock_OutPut(OrderNumber, SalesItemDetail[t_key].SalesItemIndex, 1) == true)
                                {
                                    Save_Stock_OutPut_Data(Temp_Connect, Conn, tran, OrderNumber, SalesItemDetail[t_key].SalesItemIndex, "001");
                                }

                                TSW = 1;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    //else
                    //{
                    //    if (SalesItemDetail[t_key].Receive_Method == 1)
                    //    {
                    //        if (csd.Check_Stock_OutPut(OrderNumber, SalesItemDetail[t_key].SalesItemIndex, 1) == false)
                    //        {
                    //            Save_Stock_OutPut_Data_Del(Temp_Connect, Conn, tran, OrderNumber, SalesItemDetail[t_key].SalesItemIndex, SalesItemDetail[SalesItemDetail[t_key].SalesItemIndex].ItemCount);  //출고된 내역이 있으면 취소 처리 한다.
                    //        }

                    //        //TSW = 1;
                    //    }
                    //}
                }

            }
        }


        private void Save_Stock_OutPut_Data(cls_Connect_DB Temp_Connect,
                                            SqlConnection Conn, SqlTransaction tran, string OrderNumber, int Salesitemindex, string out_C_Code = "")
        {
            string Out_FL = "001";   //'''---주문출고는 001 임




            string StrSql = ""; string T_Or = ""; string Out_Index = ""; string Sell_C_Code = "";
            int ItemCnt = 0; string ItemCode = ""; int Out_Price = 0; string T_index = "";
            int SalesItemIndex = 0; int Out_Pv = 0; int Out_Bv = 0; string Out_Date = "";
            int Send_itemCount1 = 0; int itemCount = 0;

            DataSet ds = new DataSet();
            StrSql = "Select  tbl_SalesItemDetail.*, tbl_SalesDetail.SellDate, tbl_SalesDetail.SellCode ";
            StrSql = StrSql + " From tbl_SalesItemDetail (nolock) ";
            StrSql = StrSql + " LEFT JOIN tbl_SalesDetail (nolock) ON tbl_SalesItemDetail.OrderNumber = tbl_SalesDetail.OrderNumber ";
            StrSql = StrSql + " Where tbl_SalesItemDetail.OrderNumber ='" + OrderNumber + "'";
            StrSql = StrSql + " And   tbl_SalesItemDetail.Salesitemindex = " + Salesitemindex;


            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {


                T_Or = OrderNumber;
                SalesItemIndex = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SalesItemIndex"].ToString());

                ItemCode = ds.Tables[base_db_name].Rows[fi_cnt]["ItemCode"].ToString();
                ItemCnt = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemCount"].ToString());
                Sell_C_Code = ds.Tables[base_db_name].Rows[fi_cnt]["SellCode"].ToString();


                Out_Price = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["itemPrice"].ToString());
                Out_Pv = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["itemPv"].ToString());
                //20230314구현호
                Out_Bv = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["itemBv"].ToString());


                Out_Date = ds.Tables[base_db_name].Rows[fi_cnt]["SellDate"].ToString();


                StrSql = "Select   ItemCount , Send_itemCount1  ";
                StrSql = StrSql + " From tbl_SalesItemDetail (nolock) ";
                StrSql = StrSql + " Where OrderNumber ='" + T_Or + "'";
                StrSql = StrSql + " And   SalesItemIndex =  " + SalesItemIndex;

                DataSet ds2 = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(StrSql, "t_P_table", ds2) == false) return;
                itemCount = int.Parse(ds2.Tables["t_P_table"].Rows[0][0].ToString());
                Send_itemCount1 = int.Parse(ds2.Tables["t_P_table"].Rows[0][1].ToString());

                if (Send_itemCount1 + ItemCnt > itemCount)
                {
                    tran.Rollback();
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Stock_Pre") + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    return;
                }


                T_index = cls_User.gid + ' ' + DateTime.UtcNow.ToString();

                StrSql = "INSERT INTO tbl_Sales_PassNumber ";
                StrSql = StrSql + " (Pass_Number2,OrderNumber,SalesItemIndex,User_TF,T_Date) ";
                StrSql = StrSql + " Select ";
                StrSql = StrSql + "'" + Out_Date.Substring(2, 6);
                //StrSql = StrSql + "'+ Right('00000' + convert(varchar(8),convert(float,Right(Count(Pass_Number2),5)) + 1),5)  ";  //카운팅 하면 안됨 가운데 비는 숫자가 나올수 있음.
                StrSql = StrSql + "'+ Right('00000' + convert(varchar(8),convert(float, isnull(Max(Right(Pass_Number2,5) ) ,0)   ) + 1),5)  ";

                StrSql = StrSql + ",'" + T_Or + "'," + SalesItemIndex + ",1,'" + T_index + "'";
                StrSql = StrSql + " From tbl_Sales_PassNumber (nolock) ";
                StrSql = StrSql + " Where LEFT(Pass_Number2,6) = '" + Out_Date.Substring(2, 6) + "'";

                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);

                StrSql = "Select Top 1  Pass_Number2   ";
                StrSql = StrSql + " From tbl_Sales_PassNumber (nolock) ";
                StrSql = StrSql + " Where  OrderNumber ='" + T_Or + "'";
                StrSql = StrSql + " And   SalesItemIndex =" + SalesItemIndex;
                StrSql = StrSql + " And   T_Date ='" + T_index + "'";
                StrSql = StrSql + " Order by Pass_Number2 DESC ";

                DataSet ds3 = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.

                if (Temp_Connect.Open_Data_Set(StrSql, "t_P_table", ds3) == false) return;
                Out_Index = ds3.Tables["t_P_table"].Rows[0]["Pass_Number2"].ToString();


                StrSql = "Insert into tbl_StockOutput (";
                StrSql = StrSql + " Out_Index,Out_FL, Out_Date  ";
                StrSql = StrSql + " , ItemCode ";
                StrSql = StrSql + " ,ItemCount";
                StrSql = StrSql + " ,Out_Price,Out_PV1, Out_SumPrice,Out_SumPV1 ";
                StrSql = StrSql + " , Out_Name ";
                StrSql = StrSql + " , Remarks1, Remarks2 ";
                StrSql = StrSql + " ,C_Code_FL ,  Out_C_Code ";
                StrSql = StrSql + " ,Base_ItemCount, Sell_C_Code ";
                StrSql = StrSql + " ,OrderNumber, Salesitemindex ";

                StrSql = StrSql + " ,RecordId, RecordTime ";
                StrSql = StrSql + " )";
                StrSql = StrSql + " Values ";
                StrSql = StrSql + " (";
                StrSql = StrSql + "'" + Out_Index + "'";   //입고번호
                StrSql = StrSql + ",'" + Out_FL + "'";   //기타입고 코드 번호
                StrSql = StrSql + ",'" + Out_Date + "'";       //상품코드

                StrSql = StrSql + ",'" + ItemCode + "'";       //상품코드
                StrSql = StrSql + "," + ItemCnt;      //입고수량
                StrSql = StrSql + "," + Out_Price;       //단위소매가
                StrSql = StrSql + "," + Out_Pv;       //단위소매가


                StrSql = StrSql + "," + Out_Price * ItemCnt;      //총입고금액
                StrSql = StrSql + "," + Out_Pv * ItemCnt;      //총입고금액

                StrSql = StrSql + ",''";      //입고자
                StrSql = StrSql + ",''";       //비고1
                StrSql = StrSql + ",''";        //비고2

                StrSql = StrSql + ",'C'";   //센타/창고 구분자 c:센타  w:창고

                if (out_C_Code == "")
                {
                    if (txt_OrderNumber.Text == "")
                        StrSql = StrSql + ",'" + SalesDetail[""].BusCode + "'";       //출고지 코드
                    else
                        StrSql = StrSql + ",'" + SalesDetail[T_Or].BusCode + "'";       //출고지 코드
                }
                else
                {
                    StrSql = StrSql + ",'" + out_C_Code + "'";
                }

                StrSql = StrSql + "," + ItemCnt;      //입고수량
                StrSql = StrSql + ",'" + Sell_C_Code + "'";      //입고수량




                

                StrSql = StrSql + ",'" + T_Or + "'";       //상품코드
                StrSql = StrSql + "," + SalesItemIndex;      //입고수량
                StrSql = StrSql + ",'" + cls_User.gid + "'";
                StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";

                StrSql = StrSql + ")";

                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);


                StrSql = "Update tbl_SalesItemDetail SET ";
                StrSql = StrSql + " Send_itemCount1 = Send_itemCount1 + " + ItemCnt;
                StrSql = StrSql + " Where OrderNumber ='" + T_Or + "'";
                StrSql = StrSql + " And   SalesItemIndex =  " + SalesItemIndex;

                Temp_Connect.Update_Data(StrSql, Conn, tran);

            }


        }


        private void Save_Stock_OutPut_Data_Del(cls_Connect_DB Temp_Connect,
                                            SqlConnection Conn, SqlTransaction tran, string OrderNumber, int Salesitemindex, int itemCount)
        {
            //string Out_FL = "001";   //'''---주문출고는 001 임


            string StrSql = "";

            StrSql = "Insert into tbl_StockOutput_DelBackup  ";
            StrSql = StrSql + " Select * ,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " From tbl_StockOutput (nolock) ";
            StrSql = StrSql + " Where OrderNumber ='" + OrderNumber + "'";
            StrSql = StrSql + " And   Salesitemindex = " + Salesitemindex;

            Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);



            StrSql = "DELETE tbl_stockOutput ";
            StrSql = StrSql + " Where OrderNumber ='" + OrderNumber + "'";
            StrSql = StrSql + " And   Salesitemindex = " + Salesitemindex;

            Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);


            StrSql = "Update tbl_SalesItemDetail SET ";
            StrSql = StrSql + " Send_itemCount1 = Send_itemCount1 - " + itemCount;
            StrSql = StrSql + " Where OrderNumber ='" + OrderNumber + "'";
            StrSql = StrSql + " And   SalesItemIndex =  " + Salesitemindex;

            Temp_Connect.Update_Data(StrSql, Conn, tran);



        }

        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check, ref string T_orderNumber)
        {
            Save_Error_Check = 0;
            string str_Q = "";

            if (txt_OrderNumber.Text == "")
                str_Q = "Msg_Base_Save_Q";
            else
                str_Q = "Msg_Base_Edit_Q";

            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(str_Q), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (Check_TextBox_Error() == false) return;  //각종 입력 오류를 체크한다.

            if (txt_OrderNumber.Text.Trim() == "")
                Input_SalesDetail_dic();   //주문번호 ""으로 해서 판매 주 클래스 에 넣음
            else
                Update_SalesDetail_dic();  //판매 주 클래스에 대한 수정 작업

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            int Com_TF = 0;
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();
            Com_TF = 1;
            string T_ord_N = "";

            cls_Search_DB csd = new cls_Search_DB();

            try
            {
                //저장할 것에 대한 주문번호를 따온다          
                DB_Save_tbl_SalesDetail(Temp_Connect, Conn, tran, ref T_ord_N);

                T_orderNumber = T_ord_N;

                if (T_ord_N == "") //주문번호 미발급시 오류로 해서 되돌린다.  
                {
                    if (Com_TF == 1)
                        tran.Rollback();

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

                    tran.Dispose();
                    Temp_Connect.Close_DB();

                    return;
                }


                //실질적인 저장,수정이 이루어지는곳. 변경시 주테이블 이전 내역도 같이 저장함
                DB_Save_tbl_SalesDetail____002(Temp_Connect, Conn, tran, T_ord_N);


                DB_Save_tbl_SalesItemDetail(Temp_Connect, Conn, tran, T_ord_N);

                DB_Save_tbl_Sales_Cacu(Temp_Connect, Conn, tran, T_ord_N);


                DB_Save_tbl_Sales_Rece(Temp_Connect, Conn, tran, T_ord_N);
                //쿠폰 내역을 저장한다
                DB_Save_tbl_Sales_Coupon(Temp_Connect, Conn, tran, T_ord_N);

                Temp_Ordernumber = T_ord_N;

                //string StrSql = "EXEC Usp_Sales_Prom '" + Temp_Ordernumber + "'";
                //Temp_Connect.Delete_Data(StrSql, "tbl_Sales_Rece", Conn, tran, this.Name.ToString(), this.Text);

                ///* 직접 수령 자동 출고*/
                //StrSql = " EXEC Usp_Sell_Auto_StockOut_Insert '" + Temp_Ordernumber + "' ";
                //Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text);

                //DB_Save_tbl_Mileage____001(Temp_Connect, Conn, tran, T_ord_N);

                //Boolean Card_App_TF = DB_Save_Card_App(Temp_Connect, Conn, tran,T_ord_N);  //카드 승인 관련해서 체크된 내역들은 승인을 낸다.

                //if (Card_App_TF == false)
                //{
                //    tran.Rollback();
                //    tran.Dispose();
                //    Temp_Connect.Close_DB();

                //    if (txt_OrderNumber.Text == "")
                //        MessageBox.Show("카드 자동 신청이 미승인 처리 되었습니다. 매출 저장 실패 입니다.");
                //    else
                //        MessageBox.Show("카드 자동 신청이 미승인 처리 되었습니다. 매출 수정 실패 입니다.");

                //    return;
                //}

                tran.Commit();
                Com_TF = 0;

                if (cls_User.gid_CountryCode != "TH")
                {
                    SendCashReceipt_OK_Danal(Temp_Ordernumber); // 현금 및 한국인 경우 현금영수증 처리.
                }
                


                Save_Error_Check = 1;
                if (txt_OrderNumber.Text == "")
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }
            catch (Exception)
            {
                if (Com_TF == 1)
                    tran.Rollback();

                if (txt_OrderNumber.Text == "")
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }

            //if (cls_app_static_var.Sell_Union_Flag == "D" && (txt_Ins_Number.Text.Trim() == "" || txt_Ins_Number.Text.Trim() == "미승인요청"))
            //{
            //    InsuranceNumber_Ord_Print_FLAG = T_ord_N;
            //    c(T_ord_N);//직판 관련 승인 번호를 받아온다.                
            //    InsuranceNumber_Ord_Print_FLAG = "";
            //}

        }


        private void Sell_Ac_insurancenumber(string T_ord_N)
        {
            string Req = "";
            /*
            bool NonGaOrder = false;
            foreach (var item in Sales_Cacu)
            {
                if (item.Value.OrderNumber == T_ord_N && item.Value.C_TF == 5)
                {
                    NonGaOrder = true;

                    break;
                }
            }
            */
            cls_Socket csg = new cls_Socket();
            Req = csg.Dir_Connect_Send(T_ord_N);

            if (Req != "Y")
            {

                if (Req == "-10000")
                    return;

                string MessageInsurance = string.Format("공제조합 발급이 실패되었습니다. 에러코드:{0}" + Environment.NewLine +
                    "https://www.macco.or.kr/it/selectListSocketErrorCode.do 접속해서 에러코드 확인후에" + Environment.NewLine +
                    "메나테크㈜ 전산담당자에게 문의하시기 바랍니다.", Req);

                MessageBox.Show(MessageInsurance);
            }
            else
            {
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                string Tsql = "";

                Tsql = "Select  InsuranceNumber  From tbl_SalesDetail  (nolock) ";
                Tsql = Tsql + " Where OrderNumber = '" + T_ord_N + "'";
                //++++++++++++++++++++++++++++++++               

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "tbl_SalesDetail", ds) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt > 0)

                    txt_Ins_Number.Text = ds.Tables["tbl_SalesDetail"].Rows[0]["InsuranceNumber"].ToString();

                MessageBox.Show("공제번호가 정상적으로 발급 되었습니다. [공제번호 : " + txt_Ins_Number.Text + "]");
                Button T_bt = butt_Print; EventArgs ee1 = null;

            }
        }

        private void dGridView_Base_Sub_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Name == "dGridView_Base_Item")
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0].Value != null)
                {
                    if (dgv.CurrentRow.Cells[0].Value.ToString() != "")
                        Put_Sub_Date(dgv.CurrentRow.Cells[0].Value.ToString(), "item");
                }
            }

            if (dgv.Name == "dGridView_Base_Rece")
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0].Value != null)
                {
                    Put_Sub_Date(dgv.CurrentRow.Cells[10].Value.ToString(), "Rece");
                }
            }

            if (dgv.Name == "dGridView_Base_Cacu")
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0].Value != null)
                {
                    cls_form_Meth ct = new cls_form_Meth();

                    if (combo_C_Card_Year.SelectedIndex >= 0)
                        combo_C_Card_Year.SelectedIndex = 0;
                    if (combo_C_Card_Month.SelectedIndex >= 0)
                        combo_C_Card_Month.SelectedIndex = 0;
                    if (combo_C_Card_Per.SelectedIndex >= 0)
                        combo_C_Card_Per.SelectedIndex = 0;

                    ct.from_control_clear(tab_Cacu, txt_Price_1);
                    txt_C_Etc.Text = "";
                    butt_Cacu_Del.Visible = false;
                    butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
                    tab_Cacu.Enabled = true;

                    enable_Card_info_txt(true);
                    enable_Av_Bank_info_txt(true);
                    if (Card_Ok_Visible == false)
                    {
                        button_Ok.Visible = false;
                    }
                    else
                    {

                        if (cls_app_static_var.Member_Card_Sugi_TF == 1)
                            button_Ok.Visible = true;
                        else
                            button_Ok.Visible = false;
                    }
                    button_Cancel.Visible = false;
                    tableL_CD.Visible = false;

                    buttonV_Ok.Visible = true;
                    buttonV_Cancel.Visible = false;

                    Put_Sub_Date(dgv.CurrentRow.Cells[0].Value.ToString(), "Cacu");
                }


                string insuranceNumber = "";
                string stockoutput = "";
                string Tsql = "";

                //20231015구현호 해당내역이 공제번호가 부여됐으면 안뜨게 한다.
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                DataSet ds = new DataSet();
                Tsql = "Select  insuranceNumber from tbl_SalesDetail (NOLOCK) where ordernumber =   '" + txt_OrderNumber.Text.Trim() + "' ";
                
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "tbl_SalesDetail", ds) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;
                if (ReCnt == 1)
                {
                    insuranceNumber = ds.Tables["tbl_SalesDetail"].Rows[0][0].ToString();
                }

                //20231015구현호 해당내역이 출고가 됐으면 안뜨게 한다.
                ds = new DataSet();
                Tsql = "Select  ordernumber  from tbl_StockOutput (NOLOCK) where ordernumber =   '" + txt_OrderNumber.Text.Trim() + "' ";

                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return;
                ReCnt = Temp_Connect.DataSet_ReCount;
                if (ReCnt >= 1)
                {
                    stockoutput = ds.Tables["tbl_StockOutput"].Rows[0][0].ToString();
                }

                if (insuranceNumber == "" && stockoutput != "")
                {
                    button_Cancel.Visible = false;
                }
                else if (insuranceNumber != "" && stockoutput == "")
                {
                    button_Cancel.Visible = false;
                }
                else if (insuranceNumber == "" && stockoutput == "")
                {
                    button_Cancel.Visible = true;
                }
                else if (insuranceNumber != "" && stockoutput != "")
                {
                    button_Cancel.Visible = false;
                }
            }
        }


        private void enable_Card_info_txt(Boolean TF_B)
        {

            tableLayoutPanel33.Enabled = TF_B;
            tableLayoutPanel50.Enabled = TF_B;

            tableLayoutPanel30.Enabled = TF_B;
            tableLayoutPanel53.Enabled = TF_B;
            tableLayoutPanel51.Enabled = TF_B;


            pnl_Installment.Enabled = TF_B;
            tableLayoutPanel52.Enabled = TF_B;
            tableLayoutPanel54.Enabled = TF_B;
            tableLayoutPanel55.Enabled = TF_B;

            pnl_Card_Birth.Enabled = TF_B;
            tableLayoutPanel72.Enabled = TF_B;
        }

        private void enable_Av_Bank_info_txt(Boolean TF_B)
        {

            tableLayoutPanel73.Enabled = TF_B;
            tableLayoutPanel46.Enabled = TF_B;
            tableLayoutPanel49.Enabled = TF_B;
        }

        private void Put_Sub_Date(string SalesItemIndex, string t_STF)
        {
            if (t_STF == "item")
            {
                Data_Set_Form_TF = 1;
                txt_SalesItemIndex.Text = SalesItemIndex;

                butt_Item_Del.Visible = true;
                cls_form_Meth cm = new cls_form_Meth();
                butt_Item_Save.Text = cm._chang_base_caption_search("수정");
                int Salesitemindex = int.Parse(txt_SalesItemIndex.Text);
                txt_ItemCode.Text = SalesItemDetail[Salesitemindex].ItemCode;
                txt_ItemName.Text = SalesItemDetail[Salesitemindex].ItemName;
                txt_ItemCount.Text = SalesItemDetail[Salesitemindex].ItemCount.ToString();
                txt_Item_Etc.Text = SalesItemDetail[Salesitemindex].Etc;

                txt_ItemCode.ReadOnly = true;
                txt_ItemCode.BorderStyle = BorderStyle.FixedSingle;
                txt_ItemCode.BackColor = cls_app_static_var.txt_Enable_Color;
                Data_Set_Form_TF = 0;
            }

            if (t_STF == "Rece")
            {
                Data_Set_Form_TF = 1;
                txt_RecIndex.Text = SalesItemIndex;

                butt_Rec_Del.Visible = false;
                cls_form_Meth cm = new cls_form_Meth();
                butt_Rec_Save.Text = cm._chang_base_caption_search("수정");
                int Salesitemindex = int.Parse(txt_RecIndex.Text);

                txt_Receive_Method.Text = Sales_Rece[Salesitemindex].Receive_Method_Name.ToString();
                txt_Receive_Method_Code.Text = Sales_Rece[Salesitemindex].Receive_Method.ToString();
                txt_Get_Name1.Text = Sales_Rece[Salesitemindex].Get_Name1;

                mtxtZip1.Text = "";

                // 태국인 경우
                if (cls_User.gid_CountryCode == "TH")
                {
                    //cbProvince_TH.Text = Sales_Rece[Salesitemindex].Get_state.Replace("-", "").ToString();
                    //cbDistrict_TH.Text = Sales_Rece[Salesitemindex].Get_city.Replace("-", "").ToString();
                    try
                    {
                        cbProvince_TH.Text = Sales_Rece[Salesitemindex].Get_Address2.Split(' ')[2];
                        cbDistrict_TH.Text = Sales_Rece[Salesitemindex].Get_Address2.Split(' ')[1];
                        cbSubDistrict_TH.Text = Sales_Rece[Salesitemindex].Get_Address2.Split(' ')[0];
                    }
                    catch (Exception)
                    {
                        cbProvince_TH.Text = "";
                        cbDistrict_TH.Text = "";
                        cbSubDistrict_TH.Text = "";
                    }


                    if (Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Length >= 5)
                    {
                        //mtxtZip1.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(0, 3) + "-" + Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                        //txtAddCode2.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                        txtZipCode_TH.Text = Sales_Rece[Salesitemindex].Get_ZipCode.Replace("-", "").ToString();
                        
                    }
                }
                // 태국 이외 국가
                else
                {
                    if (Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Length >= 5)
                    {
                        //mtxtZip1.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(0, 3) + "-" + Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                        //txtAddCode2.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                        mtxtZip1.Text = Sales_Rece[Salesitemindex].Get_ZipCode.Replace("-", "").ToString();
                    }
                }





                string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
                cls_form_Meth cfm = new cls_form_Meth();
                //cfm.Phone_Number_Split(Sales_Rece[Salesitemindex].Get_Tel1.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;
                mtxtTel1.Text = Sales_Rece[Salesitemindex].Get_Tel1.ToString();

                //cfm.Phone_Number_Split(Sales_Rece[Salesitemindex].Get_Tel2.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

                //mtxtTel2.Text = Sales_Rece[Salesitemindex].Get_Tel2.ToString();
                cfm.Home_Number_Setting(Sales_Rece[Salesitemindex].Get_Tel2.ToString(), mtxtTel2);


                txtAddress1.Text = Sales_Rece[Salesitemindex].Get_Address1;
                txtAddress2.Text = Sales_Rece[Salesitemindex].Get_Address2;
                txtGetDate1.Text = Sales_Rece[Salesitemindex].Get_Date1.ToString().Replace("-", "");
                txt_Pass_Number.Text = Sales_Rece[Salesitemindex].Pass_Number;
                txt_Base_Rec.Text = Sales_Rece[Salesitemindex].Base_Rec;
                txt_Base_Rec_Code.Text = Sales_Rece[Salesitemindex].Base_Rec_Name;
                txt_Get_Etc1.Text = Sales_Rece[Salesitemindex].Get_Etc1;

                txtCenter3.Text = Sales_Rece[Salesitemindex].Receive_Center_Name;
                txtCenter3_Code.Text = Sales_Rece[Salesitemindex].Receive_Center;

                Rece_Item_Grid_Set(Sales_Rece[Salesitemindex].SalesItemIndex);

                if (Sales_Rece[Salesitemindex].Receive_Method_Name.ToString() == "직접수령")
                {
                    //tableLayoutPanel81.Visible = true;
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        pnlZipCode_TH.Enabled = false;
                    }
                    else
                    {
                        pnlZipCode_KR.Enabled = false;
                    }
                    tableLayoutPanel42.Enabled = false;
                    tableLayoutPanel37.Enabled = false;
                }
                else
                {
                    //tableLayoutPanel81.Visible = false;
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        pnlZipCode_TH.Enabled = true;
                    }
                    else
                    {
                        pnlZipCode_KR.Enabled = true;
                    }
                    tableLayoutPanel42.Enabled = true;
                    tableLayoutPanel37.Enabled = true;
                }

                Data_Set_Form_TF = 0;
            }


            if (t_STF == "Cacu")
            {
                Data_Set_Form_TF = 1;




                txt_C_index.Text = SalesItemIndex;

                butt_Cacu_Del.Visible = true;
                cls_form_Meth cm = new cls_form_Meth();
                butt_Cacu_Save.Text = cm._chang_base_caption_search("수정");
                int C_index = int.Parse(txt_C_index.Text);

                txt_C_Etc.Text = Sales_Cacu[C_index].C_Etc.ToString();
                //= string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);

                if (Sales_Cacu[C_index].C_TF == 1)
                {
                    tab_Cacu.SelectedIndex = 1;
                    txt_Price_1.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price1);
                    mtxtPriceDate1.Text = Sales_Cacu[C_index].C_AppDate1.ToString().Replace("-", "");

                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 1)
                    {
                        check_Cash.Checked = true;

                        txt_C_Cash_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                        txt_C_Cash_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;

                        if (Sales_Cacu[C_index].C_Cash_Send_TF == 1)
                            radioB_C_Cash_Send_TF1.Checked = true;

                        if (Sales_Cacu[C_index].C_Cash_Send_TF == 2)
                            radioB_C_Cash_Send_TF2.Checked = true;

                        if (Sales_Cacu[C_index].C_Cash_Number != string.Empty)
                        {
                            btnCashReceiptCancel.Visible = false;
                        }
                        else
                            btnCashReceiptCancel.Visible = false;

                    }

                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 2)
                    {
                        check_Cash.Checked = false;

                        txt_C_Cash_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                        txt_C_Cash_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;
                    }


                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 100 || txt_C_Cash_Number2_2.Text != "")
                    {
                        check_Cash_Pre.Checked = true;
                        txt_C_Cash_Number2_2.Text = Sales_Cacu[C_index].C_Cash_Number;




                    }


                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == -1)
                    {
                        check_Cash.Checked = false;
                        check_Not_Cash.Checked = true;
                    }

                    if (txt_C_Cash_Number2.Text.Trim() != "") //현금 영수증 신고 처리가 되었다.. 그러면... 삭제와 수정 버튼을 안보이게 한다.
                    {
                        butt_Cacu_Del.Visible = false;
                    }

                    //but_Cash_Send2.Visible = true;


                }

                if (Sales_Cacu[C_index].C_TF == 2)
                {
                    tab_Cacu.SelectedIndex = 2;
                    txt_Price_2.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price1);
                    mtxtPriceDate2.Text = Sales_Cacu[C_index].C_AppDate1.ToString().Replace("-", "");
                    txt_C_Name_2.Text = Sales_Cacu[C_index].C_Name1.ToString();
                    txt_C_Bank.Text = Sales_Cacu[C_index].C_CodeName_2.ToString();
                    txt_C_Bank_Code.Text = Sales_Cacu[C_index].C_Code.ToString();
                    txt_C_Bank_Code_2.Text = Sales_Cacu[C_index].C_CodeName.ToString();
                    txt_C_Bank_Code_3.Text = Sales_Cacu[C_index].C_Number1.ToString();



                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 1)
                    {
                        check_Bank.Checked = true;

                        txt_C_Bank_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                        txt_C_Bank_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;

                        if (Sales_Cacu[C_index].C_Cash_Send_TF == 1)
                            radioB_C_Bank_Send_TF1.Checked = true;

                        if (Sales_Cacu[C_index].C_Cash_Send_TF == 2)
                            radioB_C_Bank_Send_TF2.Checked = true;
                    }

                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 2)
                    {
                        check_Bank.Checked = false;

                        txt_C_Bank_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                        txt_C_Bank_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;
                    }

                    /*현금영수증 부분취소 일때 인증번호에 보일수 있게*/
                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 99)
                    {
                        txt_C_Bank_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;
                    }


                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 100)
                    {
                        check_Cash_Pre.Checked = true;
                        txt_C_Bank_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                    }


                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == -1)
                    {
                        check_Bank.Checked = false;
                        check_Not_Bank.Checked = true;
                    }

                    //if (txt_C_Cash_Number2.Text.Trim() != "") //현금 영수증 신고 처리가 되었다.. 그러면... 삭제와 수정 버튼을 안보이게 한다.
                    //{
                    //    butt_Cacu_Del.Visible = false;
                    //}



                    tab_Cacu.SelectedIndex = 2;
                }

                if (Sales_Cacu[C_index].C_TF == 3)
                {
                    tab_Cacu.SelectedIndex = 0;

                    txt_Price_3.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price1);
                    mtxtPriceDate3.Text = Sales_Cacu[C_index].C_AppDate1.ToString().Replace("-", "");
                    txt_C_Name_3.Text = Sales_Cacu[C_index].C_Name1.ToString();
                    txt_C_Card.Text = Sales_Cacu[C_index].C_CodeName.ToString();
                    txt_C_Card_Code.Text = Sales_Cacu[C_index].C_Code.ToString();
                    txt_C_Card_Number.Text = Sales_Cacu[C_index].C_Number1.ToString();
                    txt_C_Card_Ap_Num.Text = Sales_Cacu[C_index].C_Number2.ToString();
                    txt_C_CVC.Text = Sales_Cacu[C_index].C_CVC?.ToString();

                    txt_Price_3_2.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price2);

                    if (Sales_Cacu[C_index].C_Period1.ToString().Length == 2)
                        combo_C_Card_Year.Text = "20" + Sales_Cacu[C_index].C_Period1.ToString();
                    else
                        combo_C_Card_Year.Text = Sales_Cacu[C_index].C_Period1.ToString();

                    combo_C_Card_Month.Text = Sales_Cacu[C_index].C_Period2.ToString();

                    if (combo_C_Card_Year.Text != "")
                        txt_C_Card_Year.Text = combo_C_Card_Year.Text.Substring(2, 2);

                    if (combo_C_Card_Month.Text != "")
                        txt_C_Card_Month.Text = combo_C_Card_Month.Text;


                    combo_C_Card_Per.Text = Sales_Cacu[C_index].C_Installment_Period.ToString();

                    txt_C_P_Number.Text = Sales_Cacu[C_index].C_P_Number.ToString();
                    txt_C_B_Number.Text = Sales_Cacu[C_index].C_B_Number.ToString();

                    txt_Sugi_TF.Text = Sales_Cacu[C_index].Sugi_TF.ToString();



                    if (Sales_Cacu[C_index].C_Number3.ToString() != "" && Sales_Cacu[C_index].C_Number4.ToString() == "" && Sales_Cacu[C_index].C_Price1 > 0)
                    {
                        //butt_Cacu_Del.Visible = false;
                        //tab_Card.Enabled = false;  //카드가 수기특약이나 웹상으로 승인난 내역에 대해서는 취소가 이루어지 않으면.. 수정이나 삭제가 안되게 한다.
                        enable_Card_info_txt(false);

                        button_Ok.Visible = false;
                        button_Cancel.Visible = true;
                        //20180830 지성경. lg모듈자체에 부분 금액취소가없다
                        //tableL_CD.Visible = true;
                    }
                    else
                    {
                        if (Card_Ok_Visible == false)
                        {
                            button_Ok.Visible = false;
                        }
                        else
                        {

                            if (cls_app_static_var.Member_Card_Sugi_TF == 1)
                                button_Ok.Visible = true;
                            else
                                button_Ok.Visible = false;
                        }
                        button_Cancel.Visible = false;
                        tableL_CD.Visible = false;
                    }
                }

                if (Sales_Cacu[C_index].C_TF == 4)
                {
                    tab_Cacu.SelectedIndex = 3;

                    txt_Price_4.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price1);
                    mtxtPriceDate4.Text = Sales_Cacu[C_index].C_AppDate1.ToString().Replace("-", "");

                    double T_p = 0;
                    string T_Mbid = mtxtMbid.Text;
                    string Mbid = ""; int Mbid2 = 0;
                    cls_Search_DB csb = new cls_Search_DB();
                    if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
                    {
                        cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                        T_p = ctm.Using_Mileage_Search(Mbid, Mbid2, cls_User.gid_date_time);
                        txt_Price_4_2.Text = string.Format(cls_app_static_var.str_Currency_Type, T_p);
                    }

                }

                if (Sales_Cacu[C_index].C_TF == 6)
                {
                    tab_Cacu.SelectedTab = tab_Coupon;
                    
                    txt_Price_6.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price1);
                    txt_Price_6_2.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Coupon);
                    mtxtPriceDate6.Text = Sales_Cacu[C_index].C_AppDate1.ToString();

                    double T_p = 0;
                    string T_Mbid = mtxtMbid.Text;
                    string Mbid = ""; int Mbid2 = 0;
                    //cls_Search_DB csb = new cls_Search_DB();
                    //if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
                    //{
                    //    cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                    //    T_p = ctm.Using_Mileage_Search(Mbid, Mbid2, cls_User.gid_date_time);
                    //    txt_Price_6_2.Text = Sales_Cacu[C_index].C_Coupon.ToString();
                    //}

                }

                if (Sales_Cacu[C_index].C_TF == 5)
                {
                    tab_Cacu.SelectedIndex = 2;

                    txt_Price_5_2.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price2);
                    txt_Price_5.Text = string.Format(cls_app_static_var.str_Currency_Type, Sales_Cacu[C_index].C_Price1);

                    txt_AV_C_AppDate1.Text = Sales_Cacu[C_index].C_AppDate1.ToString().Replace("-", "");
                    txtBank_Code.Text = Sales_Cacu[C_index].C_Code.ToString();
                    txtBank.Text = Sales_Cacu[C_index].C_CodeName.ToString();
                    txt_AV_C_Code.Text = Sales_Cacu[C_index].C_Code.ToString();
                    txt_AV_C_Number1.Text = Sales_Cacu[C_index].C_Number1.ToString();
                    txt_AV_C_Number3.Text = Sales_Cacu[C_index].C_Number3.ToString();

                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 1)
                    {
                        check_AVCash.Checked = true;

                        txt_AVC_Cash_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                        txt_AVC_Cash_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;

                        if (Sales_Cacu[C_index].C_Cash_Send_TF == 1)
                            radioB_AVC_Cash_Send_TF1.Checked = true;

                        if (Sales_Cacu[C_index].C_Cash_Send_TF == 2)
                            radioB_AVC_Cash_Send_TF2.Checked = true;
                    }


                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == 2)
                    {
                        check_AVCash.Checked = false;

                        txt_AVC_Cash_Send_Nu.Text = Sales_Cacu[C_index].C_Cash_Send_Nu;
                        txt_AVC_Cash_Number2.Text = Sales_Cacu[C_index].C_Cash_Number;
                    }

                    if (Sales_Cacu[C_index].C_Cash_Sort_TF == -1)
                    {
                        check_AVCash.Checked = false;
                        check_Not_AVCash.Checked = true;
                    }



                    // butt_Cacu_Save.Visible = true;
                    if (Sales_Cacu[C_index].C_Number3.ToString() != "")
                    {
                        butt_Cacu_Save.Visible = false;
                        butt_Cacu_Del.Visible = false;
                        //tab_Card.Enabled = false;  //카드가 수기특약이나 웹상으로 승인난 내역에 대해서는 취소가 이루어지 않으면.. 수정이나 삭제가 안되게 한다.
                        enable_Av_Bank_info_txt(false);
                        buttonV_Ok.Visible = false;
                        //buttonV_Cancel.Visible = true; 2018-09-11 지성경 가상계죄취소막음, 상장페이지에서 진행권유하였음
                    }
                    else
                    {
                        buttonV_Ok.Visible = true;
                        buttonV_Cancel.Visible = false;
                    }

                    if (Sales_Cacu[C_index].C_CancelTF == 1)
                    {
                        butt_Cacu_Save.Visible = true;
                        butt_Cacu_Del.Visible = true;
                        buttonV_Ok.Visible = false;
                        buttonV_Cancel.Visible = false;
                    }


                    if (txt_AVC_Cash_Number2.Text.Trim() != "") //현금 영수증 신고 처리가 되었다.. 그러면... 삭제와 수정 버튼을 안보이게 한다.
                    {
                        butt_Cacu_Del.Visible = false;
                    }

                    //2018-10-22 지성경 현금영수증 신청이 되어있지않은경우 신청할 수 잇도록 목록을 열어주자
                    if (check_AVCash.Checked == false)
                    {
                        butt_Cacu_Save.Visible = true;
                        tableLayoutPanel49.Enabled = true;
                        check_Not_AVCash.Checked = false;
                    }

                    // but_Cash_Send.Visible = true;

                }


                Data_Set_Form_TF = 0;
            }
        }







        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {

            Base_Ord_Clear();

            if ((sender as DataGridView).CurrentRow != null && (sender as DataGridView).CurrentRow.Cells[2].Value != null)
            {
                //20231204 오토쉽내역이면 결제가 아예안되게 막아버림
                if ((sender as DataGridView).CurrentRow.Cells[8].Value.ToString() == "자동주문")
                {
                    tab_Cacu.Visible = false;
                    panel18.Visible = false;
                }
                else
                {
                    tab_Cacu.Visible = true;
                    panel18.Visible = true;
                }
                if ((sender as DataGridView).CurrentRow.Cells[2].Value.ToString() != "")
                {
                    string OrderNumber = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                    string SellTF = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                    string SellDate = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();
                    //20231001
                    string Associated_Card = (sender as DataGridView).CurrentRow.Cells[18].Value.ToString();

                    



                    //2016-06-17 주문 건이 미승인이면서 오늘보다 이전 날짜 이면 센터사용자들 카드승인 버튼 안보이게
                        if (cls_User.gid_CenterCode != "" && SellTF == "미승인" && int.Parse(SellDate.Replace("-", "").ToString()) < int.Parse(cls_User.gid_date_time))
                    {
                        Card_Ok_Visible = false;
                        button_Ok.Visible = false;
                    }
                    else
                    {
                        Card_Ok_Visible = true;

                        if (cls_app_static_var.Member_Card_Sugi_TF == 1)
                            button_Ok.Visible = true;
                        else
                            button_Ok.Visible = false;
                    }

                    Put_OrderNumber_SellDate(OrderNumber);

                    if (cls_app_static_var.Sales_Rec_Ch_TF == 0)
                        groupBox1.Enabled = false; //배송정보 수정 권한이 없는 사람은 배송 내역 그룹박스 사용 못하게 막는다.
                    //MessageBox.Show("딜리트 유저아이디 기준");

                    //button_ETC.Visible = true;
                    if (Associated_Card == "hana")
                    {
                        chk_HanaMembership.Checked = true;
                    }
                    else
                    {
                        chk_HanaMembership.Checked = false;
                    }
                    cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                    string Tsql = "select sellcode , totalpv from tbl_salesdetail (NOLOCK) where ReturnTF = 1 and ordernumber = '" + OrderNumber + "'";
                    DataSet ds = new DataSet();
                    if (Temp_Connect.Open_Data_Set(Tsql, "tbl_salesdetail", ds) == false) return;
                    int ReCnt = Temp_Connect.DataSet_ReCount;

                    if (ReCnt == 0) return;

                    string primium_custom = ds.Tables["tbl_salesdetail"].Rows[0][0].ToString();
                    if (primium_custom == "04")
                    {
                        txtSellCode_Code.Text = "04";
                        txt_SumPV.Text = ds.Tables["tbl_salesdetail"].Rows[0][1].ToString();
                    }
                    else
                    {
                  
                    }
                    cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
                    string Tsql2 = "select PV_CV_Check from tbl_salesdetail (NOLOCK)  where ordernumber = '" + txt_OrderNumber.Text + "'";
                    DataSet ds2 = new DataSet();
                    if (Temp_Connect2.Open_Data_Set(Tsql2, "tbl_salesdetail", ds2) == false) return;
                    PV_CV_Check = int.Parse(ds2.Tables["tbl_salesdetail"].Rows[0][0].ToString());
                    if (PV_CV_Check ==1)
                    {
                        chK_PV_CV_Check.Checked = true;
                    }
                    else
                    {
                        chK_PV_CV_Check.Checked = false;
                    }
                }
            }
        }

        private void Put_OrderNumber_SellDate(string OrderNumber)
        {
            Set_SalesDetail(OrderNumber);

            if (SalesItemDetail != null)
                SalesItemDetail.Clear();

            if (Sales_Rece != null)
                Sales_Rece.Clear();

            if (Sales_Cacu != null)
                Sales_Cacu.Clear();

            Set_SalesItemDetail(OrderNumber);  //상품 
            Set_Sales_Cacu(OrderNumber);  // 결제 
            Set_Sales_Rece(OrderNumber);  // 배송 

            Item_Grid_Set(); //상품 그리드
            Cacu_Grid_Set(); //결제 그리드
            Rece_Grid_Set(); //배송 그리드
            VisibleCashReceiptButton(); //현금 영수증 취소여부 확인
        }


        private void Set_SalesDetail(string OrderNumber)
        {
            Data_Set_Form_TF = 1;
            try
            {
                mtxtSellDate.Text = SalesDetail[OrderNumber].SellDate.Replace("-", "");
                mtxtSellDate2.Text = SalesDetail[OrderNumber].SellDate_2.Replace("-", "");
                txtSellCode.Text = SalesDetail[OrderNumber].SellCodeName;
                txtSellCode_Code.Text = SalesDetail[OrderNumber].SellCode;
                lblSellCode.Text = SalesDetail[OrderNumber].SellCodeName;
                lblSellCode_Code.Text = SalesDetail[OrderNumber].SellCode;
                txtCenter2.Text = SalesDetail[OrderNumber].BusCodeName;
                txtCenter2_Code.Text = SalesDetail[OrderNumber].BusCode;

                radioB_DESK.Checked = false;
                radioB_CALL.Checked = false;
                if (SalesDetail[OrderNumber].SellSort == "DESK")
                    radioB_DESK.Checked = true;

                if (SalesDetail[OrderNumber].SellSort == "CALL")
                    radioB_CALL.Checked = true;


                txt_Ins_Number.Text = SalesDetail[OrderNumber].INS_Num;
                //string.Format(cls_app_static_var.str_Currency_Type, ds.Tables[base_db_name].Rows[0]["Last_price"]);
                txt_OrderNumber.Text = OrderNumber;
                txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPrice);
                txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPV);
                txt_TotalCV.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalCV);

                txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);
                txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].UnaccMoney);
                txt_InputPass_Pay.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].InputPass_Pay);

                txt_ETC1.Text = SalesDetail[OrderNumber].Etc1;
                txt_ETC2.Text = SalesDetail[OrderNumber].Etc2;

                //button_exigo.Visible = false;
                //if (SalesDetail[OrderNumber].Us_Ord == 0)
                //    button_exigo.Visible = true; 
            }
            catch (Exception ex) { }
            Data_Set_Form_TF = 0;

        }


        private void Set_SalesItemDetail(string OrderNumber)
        {

            string strSql = "";

            strSql = "Select tbl_SalesitemDetail.* ";
            strSql = strSql + " , tbl_Goods.Name Item_Name ";

            cls_form_Meth cm = new cls_form_Meth();
            strSql = strSql + " ,Case When SellState = 'N_1' Then '" + cm._chang_base_caption_search("정상") + "'";
            strSql = strSql + "  When SellState = 'N_3' Then '" + cm._chang_base_caption_search("교환_정상") + "'";
            strSql = strSql + "  When SellState = 'R_1' Then '" + cm._chang_base_caption_search("반품") + "'";
            strSql = strSql + "  When SellState = 'R_3' Then '" + cm._chang_base_caption_search("교환_반품") + "'";
            strSql = strSql + "  When SellState = 'C_1' Then '" + cm._chang_base_caption_search("취소") + "'";
            strSql = strSql + " END  SellStateName ";

            strSql = strSql + " From tbl_SalesitemDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_Goods (nolock) ON tbl_Goods.Ncode = tbl_SalesitemDetail.ItemCode ";
            strSql = strSql + " Where tbl_SalesitemDetail.OrderNumber = '" + OrderNumber.ToString() + "'";
            strSql = strSql + " Order By SalesItemIndex ASC ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            Dictionary<int, cls_Sell_Item> T_SalesitemDetail = new Dictionary<int, cls_Sell_Item>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_Sell_Item t_c_sell = new cls_Sell_Item();

                t_c_sell.OrderNumber = ds.Tables[base_db_name].Rows[fi_cnt]["OrderNumber"].ToString();

                t_c_sell.SalesItemIndex = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SalesItemIndex"].ToString());

                t_c_sell.ItemCode = ds.Tables[base_db_name].Rows[fi_cnt]["ItemCode"].ToString();
                t_c_sell.ItemName = ds.Tables[base_db_name].Rows[fi_cnt]["Item_Name"].ToString();
                t_c_sell.ItemPrice = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemPrice"].ToString());
                t_c_sell.ItemPV = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemPV"].ToString());
                t_c_sell.ItemCV = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemCV"].ToString());
                t_c_sell.Sell_VAT_TF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Sell_VAT_TF"].ToString());
                t_c_sell.Sell_VAT_Price = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Sell_VAT_Price"].ToString());
                t_c_sell.Sell_Except_VAT_Price = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Sell_Except_VAT_Price"].ToString());
                t_c_sell.SellState = ds.Tables[base_db_name].Rows[fi_cnt]["SellState"].ToString();
                t_c_sell.SellStateName = ds.Tables[base_db_name].Rows[fi_cnt]["SellStateName"].ToString();
                t_c_sell.ItemCount = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemCount"].ToString());
                t_c_sell.ItemTotalPrice = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemTotalPrice"].ToString());
                t_c_sell.ItemTotalPV = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemTotalPV"].ToString());
                t_c_sell.ItemTotalCV = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["ItemTotalCV"].ToString());
                t_c_sell.Total_Sell_VAT_Price = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Total_Sell_VAT_Price"].ToString());
                t_c_sell.Total_Sell_Except_VAT_Price = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Total_Sell_Except_VAT_Price"].ToString());
                t_c_sell.ReturnDate = ds.Tables[base_db_name].Rows[fi_cnt]["ReturnDate"].ToString();
                t_c_sell.SendDate = ds.Tables[base_db_name].Rows[fi_cnt]["SendDate"].ToString();
                t_c_sell.ReturnBackDate = ds.Tables[base_db_name].Rows[fi_cnt]["ReturnBackDate"].ToString();
                t_c_sell.Etc = ds.Tables[base_db_name].Rows[fi_cnt]["Etc"].ToString();
                t_c_sell.RecIndex = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["RecIndex"].ToString());
                t_c_sell.Send_itemCount1 = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Send_itemCount1"].ToString());
                t_c_sell.Send_itemCount2 = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Send_itemCount2"].ToString());
                t_c_sell.T_OrderNumber1 = ds.Tables[base_db_name].Rows[fi_cnt]["T_OrderNumber1"].ToString();
                t_c_sell.T_OrderNumber2 = ds.Tables[base_db_name].Rows[fi_cnt]["T_OrderNumber2"].ToString();
                t_c_sell.Real_index = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Real_index"].ToString());
                t_c_sell.G_Sort_Code = ds.Tables[base_db_name].Rows[fi_cnt]["G_Sort_Code"].ToString();


                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();

                t_c_sell.Del_TF = "";
                T_SalesitemDetail[t_c_sell.SalesItemIndex] = t_c_sell;
            }

            SalesItemDetail = T_SalesitemDetail;
        }



        private void Set_Sales_Rece(string OrderNumber)
        {

            string strSql = "";

            strSql = "Select tbl_Sales_Rece.*  ";
            strSql = strSql + " , Isnull(tbl_Base_Rec.name ,'' ) Base_Rec_Name ";
            strSql = strSql + " , Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " Receive_Method_Name ";
            strSql = strSql + " , tbl_Business.name Receive_Center_Name ";
            strSql = strSql + " From tbl_Sales_Rece (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_Base_Rec (nolock) on tbl_Base_Rec.ncode = tbl_Sales_Rece.Base_Rec ";
            strSql = strSql + " LEFT JOIN tbl_Base_Change_Detail Ch_T (nolock) ON Ch_T.M_Detail_S = 'tbl_Sales_Rece' And  Ch_T.M_Detail = Convert(Varchar,tbl_Sales_Rece.Receive_Method) ";
            strSql = strSql + " LEFT JOIN tbl_Business (nolock) ON tbl_Sales_Rece.Receive_Center = tbl_Business.ncode ";
            strSql = strSql + " Where tbl_Sales_Rece.OrderNumber = '" + OrderNumber.ToString() + "'";
            strSql = strSql + " Order By SalesItemIndex ASC ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++
            cls_form_Meth cm = new cls_form_Meth();

            Dictionary<int, cls_Sell_Rece> T_Sales_Rece = new Dictionary<int, cls_Sell_Rece>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_Sell_Rece t_c_sell = new cls_Sell_Rece();

                t_c_sell.OrderNumber = ds.Tables[base_db_name].Rows[fi_cnt]["OrderNumber"].ToString();
                t_c_sell.SalesItemIndex = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SalesItemIndex"].ToString());
                t_c_sell.RecIndex = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["RecIndex"].ToString());
                t_c_sell.Receive_Method = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Receive_Method"].ToString());
                t_c_sell.Receive_Method_Name = ds.Tables[base_db_name].Rows[fi_cnt]["Receive_Method_Name"].ToString();


                t_c_sell.Get_Date1 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Date1"].ToString();
                t_c_sell.Get_Date2 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Date2"].ToString();
                t_c_sell.Get_Name1 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Name1"].ToString();
                t_c_sell.Get_Name2 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Name2"].ToString();
                t_c_sell.Get_ZipCode = ds.Tables[base_db_name].Rows[fi_cnt]["Get_ZipCode"].ToString();
                t_c_sell.Get_Address1 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["Get_Address1"].ToString());
                t_c_sell.Get_Address2 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["Get_Address2"].ToString());

                t_c_sell.Get_Tel1 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["Get_Tel1"].ToString());
                t_c_sell.Get_Tel2 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["Get_Tel2"].ToString());

                t_c_sell.Pass_Number = ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Number"].ToString();
                t_c_sell.Pass_Pay = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Pay"].ToString());

                t_c_sell.Pass_Number2 = ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Number2"].ToString();
                t_c_sell.Base_Rec = ds.Tables[base_db_name].Rows[fi_cnt]["Base_Rec"].ToString();
                t_c_sell.Base_Rec_Name = ds.Tables[base_db_name].Rows[fi_cnt]["Base_Rec_Name"].ToString();

                t_c_sell.Get_Etc1 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Etc1"].ToString();
                t_c_sell.Get_Etc2 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Etc2"].ToString();

                t_c_sell.Get_state = ds.Tables[base_db_name].Rows[fi_cnt]["Get_state"].ToString();  // 태국 주
                t_c_sell.Get_city = ds.Tables[base_db_name].Rows[fi_cnt]["Get_city"].ToString();  // 태국 도시

                t_c_sell.Receive_Center = ds.Tables[base_db_name].Rows[fi_cnt]["Receive_center"].ToString();
                t_c_sell.Receive_Center_Name = ds.Tables[base_db_name].Rows[fi_cnt]["Receive_Center_Name"].ToString();

                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();
                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["recordid"].ToString();

                if (t_c_sell.Get_Date1 != "")
                {
                    string t_sellDate = t_c_sell.Get_Date1.Substring(0, 4);
                    t_sellDate = t_sellDate + "-" + t_c_sell.Get_Date1.Substring(4, 2);
                    t_sellDate = t_sellDate + "-" + t_c_sell.Get_Date1.Substring(6, 2);

                    t_c_sell.Get_Date1 = t_sellDate;
                }

                if (t_c_sell.Get_Date2 != "")
                {
                    string t_sellDate = t_c_sell.Get_Date1.Substring(0, 4);
                    t_sellDate = t_sellDate + "-" + t_c_sell.Get_Date2.Substring(4, 2);
                    t_sellDate = t_sellDate + "-" + t_c_sell.Get_Date2.Substring(6, 2);

                    t_c_sell.Get_Date2 = t_sellDate;
                }



                t_c_sell.Del_TF = "";
                T_Sales_Rece[t_c_sell.SalesItemIndex] = t_c_sell;
            }

            Sales_Rece = T_Sales_Rece;
        }




        private void Set_Sales_Cacu(string OrderNumber)
        {

            string strSql = "";

            strSql = "Select tbl_Sales_Cacu.* ";
            strSql = strSql + " , Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " C_TF_Name ";
            strSql = strSql + " , Isnull(tbl_BankForCompany.BankPenName , '')  C_CodeName_2 ";
            strSql = strSql + " , Isnull(tbl_Bank.bankname , '')  AV_BankName ";

            strSql = strSql + " From tbl_Sales_Cacu (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail (nolock) ON tbl_SalesDetail.OrderNumber = tbl_Sales_Cacu.OrderNumber ";
            strSql = strSql + " LEFT JOIN tbl_BankForCompany (nolock) ON tbl_Sales_Cacu.C_Code = tbl_BankForCompany.BankCode And  tbl_Sales_Cacu.C_Number1 = tbl_BankForCompany.BankAccountNumber And tbl_SalesDetail.Na_Code = tbl_BankForCompany.Na_Code ";
            strSql = strSql + " LEFT JOIN tbl_Base_Change_Detail Ch_T (nolock) ON Ch_T.M_Detail_S = 'tbl_Sales_Cacu' And  Ch_T.M_Detail = Convert(Varchar,tbl_Sales_Cacu.C_TF) ";

            strSql = strSql + " LEFT JOIN tbl_Bank (nolock) ON Right(tbl_Sales_Cacu.C_Code,2)  = Right(tbl_Bank.Ncode,2)  And tbl_Sales_Cacu.C_TF = 5   ";
            cls_NationService.SQL_BankNationCode(ref strSql);

            strSql = strSql + " Where tbl_Sales_Cacu.OrderNumber = '" + OrderNumber.ToString() + "'";
            strSql = strSql + " Order By C_index ASC ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            Dictionary<int, cls_Sell_Cacu> T_Sales_Cacu = new Dictionary<int, cls_Sell_Cacu>();
            cls_form_Meth cm = new cls_form_Meth();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_Sell_Cacu t_c_sell = new cls_Sell_Cacu();

                t_c_sell.OrderNumber = ds.Tables[base_db_name].Rows[fi_cnt]["OrderNumber"].ToString();
                t_c_sell.C_index = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_index"].ToString());

                t_c_sell.C_TF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_TF"].ToString());
                t_c_sell.C_TF_Name = ds.Tables[base_db_name].Rows[fi_cnt]["C_TF_Name"].ToString();

                t_c_sell.C_Code = ds.Tables[base_db_name].Rows[fi_cnt]["C_Code"].ToString();

                if (ds.Tables[base_db_name].Rows[fi_cnt]["AV_BankName"].ToString() != "")
                {
                    t_c_sell.C_CodeName = ds.Tables[base_db_name].Rows[fi_cnt]["AV_BankName"].ToString();
                }
                else
                {
                    t_c_sell.C_CodeName = ds.Tables[base_db_name].Rows[fi_cnt]["C_CodeName"].ToString();
                }
                t_c_sell.C_Coupon = ds.Tables[base_db_name].Rows[fi_cnt]["C_Coupon"].ToString();
                t_c_sell.C_CodeName_2 = ds.Tables[base_db_name].Rows[fi_cnt]["C_CodeName_2"].ToString();

                t_c_sell.C_Name1 = ds.Tables[base_db_name].Rows[fi_cnt]["C_Name1"].ToString();
                t_c_sell.C_Name2 = ds.Tables[base_db_name].Rows[fi_cnt]["C_Name2"].ToString();
                t_c_sell.C_Number1 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["C_Number1"].ToString());
                // t_c_sell.C_Number1 = encrypter.Decrypt(t_c_sell.C_Number1);
                t_c_sell.C_Number2 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["C_Number2"].ToString());
                t_c_sell.C_Number3 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["C_Number3"].ToString());
                t_c_sell.C_Number4 = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["C_Number4"].ToString());

                t_c_sell.C_Price1 = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Price1"].ToString());
                t_c_sell.C_Price2 = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Price2"].ToString());


                t_c_sell.C_AppDate1 = ds.Tables[base_db_name].Rows[fi_cnt]["C_AppDate1"].ToString();
                t_c_sell.C_AppDate2 = ds.Tables[base_db_name].Rows[fi_cnt]["C_AppDate2"].ToString();
                t_c_sell.C_CancelTF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_CancelTF"].ToString());
                t_c_sell.C_CancelDate = ds.Tables[base_db_name].Rows[fi_cnt]["C_CancelDate"].ToString();
                t_c_sell.C_CancelPrice = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_CancelPrice"].ToString());

                t_c_sell.C_Period1 = ds.Tables[base_db_name].Rows[fi_cnt]["C_Period1"].ToString();
                t_c_sell.C_Period2 = ds.Tables[base_db_name].Rows[fi_cnt]["C_Period2"].ToString();
                t_c_sell.C_Installment_Period = ds.Tables[base_db_name].Rows[fi_cnt]["C_Installment_Period"].ToString();
                t_c_sell.C_CVC = ds.Tables[base_db_name].Rows[fi_cnt]["C_CVC"].ToString();
                t_c_sell.C_Etc = ds.Tables[base_db_name].Rows[fi_cnt]["C_Etc"].ToString();

                t_c_sell.Sugi_TF = ds.Tables[base_db_name].Rows[fi_cnt]["Sugi_TF"].ToString();
                t_c_sell.C_P_Number = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["C_P_Number"].ToString());
                t_c_sell.C_B_Number = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["C_B_Number"].ToString());

                t_c_sell.C_Base_Index = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Base_Index"].ToString());

                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();

                t_c_sell.C_Cash_Number = ds.Tables[base_db_name].Rows[fi_cnt]["C_Cash_Number"].ToString();
                t_c_sell.C_Cash_Send_Nu = ds.Tables[base_db_name].Rows[fi_cnt]["C_Cash_Send_Nu"].ToString();
                t_c_sell.C_Cash_Send_TF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Cash_Send_TF"].ToString());
                t_c_sell.C_Cash_Sort_TF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Cash_Sort_TF"].ToString());
                t_c_sell.C_Cash_Bus_TF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Cash_Bus_TF"].ToString());


                string t_sellDate = "";
                if (t_c_sell.C_AppDate1 != "")
                {
                    t_sellDate = t_c_sell.C_AppDate1.Substring(0, 4);
                    t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate1.Substring(4, 2);
                    t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate1.Substring(6, 2);
                }
                t_c_sell.C_AppDate1 = t_sellDate;

                t_sellDate = "";
                if (t_c_sell.C_AppDate2 != "")
                {
                    t_sellDate = t_c_sell.C_AppDate2.Substring(0, 4);
                    t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate2.Substring(4, 2);
                    t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate2.Substring(6, 2);
                }
                t_c_sell.C_AppDate2 = t_sellDate;



                t_c_sell.Del_TF = "";
                T_Sales_Cacu[t_c_sell.C_index] = t_c_sell;
            }

            Sales_Cacu = T_Sales_Cacu;
        }



        private void Set_SalesItemDetail(string Mbid, int Mbid2)
        {
            cls_form_Meth cm = new cls_form_Meth();
            string strSql = "";

            strSql = "Select Isnull(Sum(tbl_SalesitemDetail.ItemCount), 0 )   ";
            strSql = strSql + " , tbl_Goods.Name Item_Name ";
            //strSql = strSql + " ,Case When SellState = 'N_1' Then '" + cm._chang_base_caption_search("정상") + "'";
            //strSql = strSql + "  When SellState = 'N_3' Then '" + cm._chang_base_caption_search("교환_정상") + "'";
            //strSql = strSql + "  When SellState = 'R_1' Then '" + cm._chang_base_caption_search("반품") + "'";
            //strSql = strSql + "  When SellState = 'R_3' Then '" + cm._chang_base_caption_search("교환_반품") + "'";
            //strSql = strSql + " END  SellStateName ";

            strSql = strSql + " From tbl_SalesitemDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_Goods (nolock) ON tbl_Goods.Ncode = tbl_SalesitemDetail.ItemCode ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail (nolock) ON tbl_SalesDetail.OrderNumber = tbl_SalesitemDetail.OrderNumber ";
            strSql = strSql + " Where tbl_SalesDetail.Mbid = '" + Mbid.ToString() + "'";
            strSql = strSql + " And   tbl_SalesDetail.Mbid2 = " + Mbid2;
            strSql = strSql + " And   ItemCount > 0 ";
            strSql = strSql + " Group By tbl_Goods.Name ";
            strSql = strSql + " Order By tbl_Goods.Name ASC ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++
            //Dictionary<string, int> T_SalesitemDetail = new Dictionary<string, int>();
            int ItemCnt = 0; string ItemCode = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                ItemCode = ds.Tables[base_db_name].Rows[fi_cnt]["Item_Name"].ToString();
                ItemCnt = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt][0].ToString());
                //Push_data(series_Item, ItemCode.Replace(" ", "").Substring(0, 5), ItemCnt);
            }


        }



        private void chk_Total_MouseClick(object sender, MouseEventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            dGridView_Base_Rece_Item.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;

            if (chk_Total.Checked == true)
            {
                for (int i = 0; i < dGridView_Base_Rece_Item.RowCount; i++)
                {
                    dGridView_Base_Rece_Item.Rows[i].Cells[0].Value = "V";
                }
            }
            else
            {
                for (int i = 0; i < dGridView_Base_Rece_Item.RowCount; i++)
                {
                    dGridView_Base_Rece_Item.Rows[i].Cells[0].Value = "";
                }
            }

            dGridView_Base_Rece_Item.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.Cursor = System.Windows.Forms.Cursors.Default;

        }

        private void tab_Cacu_SelectedIndexChanged(object sender, EventArgs e)
        {

            mtxtPriceDate3.Text = "";
            txt_C_Name_3.Text = "";
            txt_C_B_Number.Text = "";
            txt_C_Card.Text = "";
            txt_C_Card_Code.Text = "";
            txt_C_Card_Number.Text = "";
            txt_C_Card_Ap_Num.Text = "";
            txt_Price_3_2.Text = "";
            combo_C_Card_Year.Text = "";
            combo_C_Card_Month.Text = "";
            combo_C_Card_Per.Text = "";
            txt_Price_1.Text = "";
            mtxtPriceDate1.Text = "";
            mtxtPriceDate3.Text = "";

            txtBank.Text = "";
            txtBank_Code.Text = "";
            txt_Price_5_2.Text = "";
            txt_Price_3.Text = "";

            check_Cash.Checked = false;
            radioB_C_Cash_Send_TF1.Checked = true;
            txt_C_Cash_Send_Nu.Text = "";
            txt_C_Cash_Number2_2.Text = "";
            btnCashReceiptCancel.Visible = false;

            if (tab_Cacu.SelectedIndex == 0)
                txt_Price_3.Focus();
            if (tab_Cacu.SelectedIndex == 1)
                txt_Price_1.Focus();
            if (tab_Cacu.SelectedIndex == 2)
                txt_Price_2.Focus();

            if (tab_Cacu.SelectedIndex == 3)
                txt_Price_5_2.Focus();
            if (tab_Cacu.SelectedIndex == 4)
                txt_Price_6.Focus();
        }

        private void txt_SOrd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string T_Orde = txt_SOrd.Text;
                string strSql = "";

                strSql = "Select Mbid,Mbid2 ";
                strSql = strSql + " From tbl_SalesDetail (nolock) ";
                strSql = strSql + " Where tbl_SalesDetail.OrderNumber = '" + txt_SOrd.Text.ToString() + "'";
                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_OrderNumber_Not_Exist")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    return;
                }
                //++++++++++++++++++++++++++++++++
                string Send_Number = "";
                if (cls_app_static_var.Member_Number_1 > 0)
                    Send_Number = ds.Tables[base_db_name].Rows[0]["Mbid"].ToString() + "-" + ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString();
                else
                    Send_Number = ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString();


                mtxtMbid.Text = Send_Number;
                Set_Form_Date(mtxtMbid.Text, "m");
                Coupon_Grid_Set();


                if (T_Orde != "")
                {
                    Base_Ord_Clear();
                    txt_SOrd.Text = T_Orde;
                    Put_OrderNumber_SellDate(T_Orde);
                }
            }

        }




        private void butt_Mile_Search_Click(object sender, EventArgs e)
        {
            //회원을 선택 안햇네 그럼 회원 넣어라
            if (txtName.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Mem")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return;
            }

            dGridView_Base_Mile.Width = groupBox3.Width - 10;
            dGridView_Base_Mile.Height = groupBox3.Height - 18;
            dGridView_Base_Mile.Left = groupBox3.Left + 5;
            dGridView_Base_Mile.Top = groupBox3.Top + 15;

            Mile_Grid_Set();

            dGridView_Base_Mile.BringToFront();
            //dGridView_Base_Mile.RowHeadersVisible = false;
            dGridView_Base_Mile.Visible = true;
            dGridView_Base_Mile.Focus();
        }



        private void Mile_Grid_Set()
        {
            dGridView_Mile_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Mile.d_Grid_view_Header_Reset();
            string strSql = "";

            strSql = "SELECT T_Time,PlusValue, MinusValue, ";
            strSql = strSql + " Case  When PlusValue > 0 then C1.T_Name When MinusValue >0  then C2.T_Name End ";
            strSql = strSql + " ,Plus_OrderNumber ";

            strSql = strSql + " ,Minus_OrderNumber,User_id ,'', '', ''";
            strSql = strSql + " From tbl_Member_Mileage (nolock) ";
            strSql = strSql + " LEFT Join tbl_Member_Mileage_Code C1  (nolock) ON tbl_Member_Mileage.PlusKind = C1.T_Code ";
            strSql = strSql + " LEFT Join tbl_Member_Mileage_Code C2  (nolock) ON tbl_Member_Mileage.MinusKind = C2.T_Code ";

            if (idx_Mbid.Length == 0)
                strSql = strSql + " Where tbl_Member_Mileage.Mbid2 = " + idx_Mbid2.ToString();
            else
            {
                strSql = strSql + " Where tbl_Member_Mileage.Mbid = '" + idx_Mbid + "' ";
                strSql = strSql + " And   tbl_Member_Mileage.Mbid2 = " + idx_Mbid2.ToString();
            }

            strSql = strSql + " Order by T_Time DESC";



            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, "TempTable", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;
            if (ReCnt == 0) return;

            double S_cnt1 = 0; double S_cnt2 = 0; int fi_cnt2 = 0;
            cls_form_Meth cm = new cls_form_Meth();

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_Mile_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                S_cnt1 = S_cnt1 + double.Parse(ds.Tables["TempTable"].Rows[fi_cnt][1].ToString());
                S_cnt2 = S_cnt2 + double.Parse(ds.Tables["TempTable"].Rows[fi_cnt][2].ToString());

                fi_cnt2 = fi_cnt;
            }


            object[] row0 = { "<< " + cm._chang_base_caption_search("합계") + " >>"
                                ,S_cnt1
                                ,S_cnt2
                                ,string.Format(cls_app_static_var.str_Currency_Type, S_cnt1 - S_cnt2 )
                                ,""

                                ,""
                                ,""
                                ,""
                                ,""
                                ,""
                            };

            gr_dic_text[fi_cnt2 + 2] = row0;

            cgb_Mile.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Mile.db_grid_Obj_Data_Put();
        }

        private void Set_gr_Mile_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {

            object[] row0 = { ds.Tables["TempTable"].Rows[fi_cnt][0]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][1]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][2]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][3]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][4]

                                ,ds.Tables["TempTable"].Rows[fi_cnt][5]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][6]
                                ,""
                                ,""
                                ,""
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Mile_Header_Reset()
        {
            cgb_Mile.Grid_Base_Arr_Clear();
            cgb_Mile.basegrid = dGridView_Base_Mile;
            cgb_Mile.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Mile.grid_col_Count = 10;
            cgb_Mile.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            cgb_Mile.Sort_Mod_Auto_TF = 1;

            string[] g_HeaderText = {"기록일"  , "적립액"   , "사용액"  , "구분"   , "적립_주문번호"
                                , "사용_주문번호"   , "기록자"    , ""  , "" , ""
                                };

            int[] g_Width = { 80 ,80, 250, 200, 120
                                ,120 , 100 , 0 , 0 , 0
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft  //5    
  
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter  //10
                                };

            cgb_Mile.grid_col_header_text = g_HeaderText;
            cgb_Mile.grid_col_w = g_Width;
            cgb_Mile.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                   };
            cgb_Mile.grid_col_Lock = g_ReadOnly;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[2 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb_Mile.grid_cell_format = gr_dic_cell_format;

            //cgb_Mile.basegrid.RowHeadersVisible = false;
        }
        public double IntRound(double Value, int Digit)
        {
            double Temp = Math.Pow(10.0, Digit);
            return Math.Round(Value * Temp) / Temp;
        }

        private void butt_Print_Click(object sender, EventArgs e)
        {
            if (txt_OrderNumber.Text == "" && InsuranceNumber_Ord_Print_FLAG == "") return;

            frmFastReport frm = new frmFastReport();
            frm.Parameter.Add("회원명", txtName.Text);
            frm.Parameter.Add("회원번호", mtxtMbid.Text);
            frm.Parameter.Add("주문일자", mtxtSellDate.Text);
            frm.Parameter.Add("주문번호", txt_OrderNumber.Text);
            frm.Parameter.Add("공제번호", txt_Ins_Number.Text);
            frm.Parameter.Add("주문유형", txtSellCode.Text);
            frm.Parameter.Add("연락처", txt_Tel.Text);

            string GetName = string.Empty;
            string PassNumber = string.Empty;
            string FullAddress = "{0}" + Environment.NewLine + "{1}";
            string ReceiveMethodName = string.Empty;
            string addr1 = string.Empty;
            string addr2 = string.Empty;
            string zipcode = string.Empty;

            if (dGridView_Base_Rece.Rows.Count > 0)
            {
                DataGridViewRow row = dGridView_Base_Rece.Rows[0];
                GetName = row.Cells["Name1"].Value.ToString();
                PassNumber = row.Cells["Passnumber"].Value.ToString();
                zipcode = row.Cells["ZipCode"].Value.ToString();
                addr1 = row.Cells["Address1"].Value.ToString();
                addr2 = row.Cells["Address2"].Value.ToString();
                ReceiveMethodName = row.Cells["Receive_Method_Name"].Value.ToString();

                FullAddress = string.Format(FullAddress, zipcode, addr1 + " " + addr2);
            }
            else
            {
                FullAddress = string.Format(FullAddress, string.Empty, string.Empty);
            }

            frm.Parameter.Add("받는사람", GetName);
            frm.Parameter.Add("운송장번호", PassNumber);
            frm.Parameter.Add("수령방법", ReceiveMethodName);
            frm.Parameter.Add("주소", FullAddress);

   

            string BV = "";
            BV = DateTime.Now.ToString("yyyyMM");
            frm.Parameter.Add("BV", BV);
            //cs는 데이터에 따른 인쇄여부가 아니라 그냥 즉각적인 여부로 한단다
            //cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
            //string Tsql2 = "select PV_CV_Check from tbl_salesdetail where ordernumber = '" + txt_OrderNumber.Text + "'";
            //DataSet ds2 = new DataSet();
            //if (Temp_Connect2.Open_Data_Set(Tsql2, "tbl_salesdetail", ds2) == false) return;
            //int PV_CV_Check_print = int.Parse(ds2.Tables["tbl_salesdetail"].Rows[0][0].ToString());

      

            double txt_TotalInputPrice_int = double.Parse(txt_TotalInputPrice.Text.Replace(",",""));
            double txt_TotalInputPrice_min = txt_TotalInputPrice_int / 11;
            Math.Round(txt_TotalInputPrice_min);
            string txt_TotalInputPrice_min_string = string.Format("{0:###,###,###,###}", txt_TotalInputPrice_min);
         

            double txt_TotalInputPrice_min_final = txt_TotalInputPrice_int - txt_TotalInputPrice_min;
            Math.Round(txt_TotalInputPrice_min_final);
            string txt_TotalInputPrice_min_final_string = string.Format("{0:###,###,###,###}", txt_TotalInputPrice_min_final);
           


            if (chK_PV_CV_Check.Checked == true)
            {
                frm.Parameter.Add("현금합산", txt_SumCash.Text);
                frm.Parameter.Add("신용카드합산", txt_SumCard.Text);
                frm.Parameter.Add("가상계좌합산", txt_SumBank.Text);
                frm.Parameter.Add("쿠폰합산", txt_SumCoupon.Text);
                frm.Parameter.Add("PV", txt_SumPV.Text);
                frm.Parameter.Add("배송료", txt_InputPass_Pay.Text);
                frm.Parameter.Add("총입금액", txt_TotalInputPrice.Text);
                frm.Parameter.Add("세금", txt_TotalInputPrice_min_string);
                frm.Parameter.Add("총입금액세금차액", txt_TotalInputPrice_min_final_string);
                
            }
            else
            {
                frm.Parameter.Add("현금합산", "");
                frm.Parameter.Add("신용카드합산", "");
                frm.Parameter.Add("가상계좌합산", "");
                frm.Parameter.Add("쿠폰합산", "");
                frm.Parameter.Add("PV", "");
                frm.Parameter.Add("배송료", "");
                frm.Parameter.Add("총입금액", "");
                frm.Parameter.Add("세금", "");
                frm.Parameter.Add("총입금액세금차액", "");
            }



            DataTable Products = new DataTable();
            Products.Columns.Add("ItemCode", typeof(string));
            Products.Columns.Add("Name", typeof(string));
            Products.Columns.Add("ItemCount", typeof(int));
            Products.Columns.Add("ItemPrice", typeof(int));
            Products.Columns.Add("ItemTotalPrice", typeof(int));
            Products.Columns.Add("ItemPV", typeof(int));
     
            Products.Columns.Add("Etc", typeof(string));
            //추가된 세트아이템 코드
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select A.Good_Code, A.Sub_Good_Code, B.name, A.Sub_Good_Cnt");
            sb.AppendLine("from tbl_Goods_Set (NOLOCK)  A");
            sb.AppendLine(" join tbl_goods (NOLOCK) b on a.Sub_Good_Code = b.ncode order by  A.Sub_Good_Code ");

            DataSet ds = new DataSet();
            DataTable dtSetItems = new DataTable();
            DataRow[] FindRow;
            if ((new cls_Connect_DB()).Open_Data_Set(sb.ToString(), "SetItems", ds) == false)
                return;

            dtSetItems = ds.Tables["SetItems"];
            int SetCnt = 0;
            //추가된 세트아이템 코드
            foreach (DataGridViewRow row in dGridView_Base_Item.Rows)
            {
                DataRow Product = Products.NewRow();
                Product["ItemCode"] = row.Cells["Itemcode"].Value.ToString();
                Product["Name"] = row.Cells["ItemName"].Value.ToString();
                Product["ItemCount"] = row.Cells["ItemCount"].Value.ToString();

                if (chK_PV_CV_Check.Checked == true)
                {
                    Product["ItemPV"] = row.Cells["ItemPV"].Value.ToString();
                    Product["ItemPrice"] = row.Cells["ItemPrice"].Value.ToString();
                    Product["ItemTotalPrice"] = row.Cells["ItemTotalPrice"].Value.ToString();
                }
                else
                {
                    Product["ItemPV"] =0;
                    Product["ItemPrice"] = 0;
                    Product["ItemTotalPrice"] = 0;
                }
           
                Product["Etc"] = row.Cells["Etc"].Value.ToString();

                Products.Rows.Add(Product);
                //--세트아이템 찾아 넣어주기
                FindRow = dtSetItems.Select("Good_Code = '" + Product["ItemCode"].ToString() + "'");
                SetCnt = 0;

                foreach (DataRow SetRow in FindRow)
                {
                    SetCnt = Convert.ToInt32(SetRow["Sub_Good_Cnt"]) * Convert.ToInt32(Product["ItemCount"]);

                    DataRow SetProduct = Products.NewRow();
                    SetProduct["ItemCode"] = SetRow["Sub_Good_Code"].ToString();
                    SetProduct["Name"] = "ㄴ(SET) " +  SetRow["name"].ToString();
                    SetProduct["ItemCount"] = SetCnt.ToString();

                  
                    Products.Rows.Add(SetProduct);
                }
            }

            if (Products.Rows.Count < 22)
            {
                for (int i = Products.Rows.Count; i < 22; i++)
                {
                    DataRow Product = Products.NewRow();
                    Products.Rows.Add(Product);
                }
            }
            //else if (Products.Rows.Count / 22 > 0)
            //{
            //    int n = ((Products.Rows.Count / 22) + 1) * 28;
            //    for (int i = Products.Rows.Count; i < n; i++)
            //    {
            //        DataRow Product = Products.NewRow();
            //        Products.Rows.Add(Product);
            //    }
            //}


            frm.BindingDataTables.Add("Products", Products);
            frm.ShowReport(frmFastReport.EShowReport.거래명세표);
        }

        private Boolean Cacu_Card_Error_Check__01()
        {
            //교환이나 부분반품 반품 건에 대해서는 현 화면에서 수정을 못하게함.
            string Tsql = "";
            Tsql = "select ReturnTF, InsuranceNumber  from tbl_SalesDetail  (nolock) ";
            Tsql = Tsql + " Where OrderNumber = '" + txt_OrderNumber.Text.Trim() + "' ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt != 0)
            {
                if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "2")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_2")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtCenter2.Focus(); return false;
                }

                if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "3")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_3")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtCenter2.Focus(); return false;
                }

                if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "4")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_4")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtCenter2.Focus(); return false;
                }

                if (ds.Tables[base_db_name].Rows[0]["ReturnTF"].ToString() == "5")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_5")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtCenter2.Focus(); return false;
                }


                if (ds.Tables[base_db_name].Rows[0]["InsuranceNumber"].ToString() != "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("There is an exemption number. Please try again after canceling the deduction number."
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {
                        MessageBox.Show("공제번호가 존재합니다. 공제번호 취소후에 다시 시도해 주십시요."
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    txtCenter2.Focus(); return false;
                }
            }

            return true;
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {

            //회원번호 관련 관련 오류 체크 및 존재 여부 그리고 탈퇴 여부(신규 저장일 경우에)                      
            if (Input_Error_Check(mtxtMbid, "m", 1) == false) return ;
            

            if (txt_OrderNumber.Text == "")
            {
                if (txtSellCode_Code.Text == "04")
                {
                    cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
                    string Tsql = "select sellcode from tbl_salesdetail (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "' and sellcode = '04' AND RETURNTF <>5 ";
                    DataSet ds = new DataSet();
                    if (Temp_Connect2.Open_Data_Set(Tsql, "tbl_salesdetail", ds) == false) return;
                    int ReCnt = Temp_Connect2.DataSet_ReCount;

                    if (ReCnt > 0)
                    {
                        cls_Connect_DB Temp_Connect4 = new cls_Connect_DB();
                        string Tsql2 = "select sellcode from tbl_salesdetail (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "' and sellcode = '04' AND ReturnTF in(2,3,4,5) ";
                        DataSet ds2 = new DataSet();
                        if (Temp_Connect4.Open_Data_Set(Tsql2, "tbl_salesdetail", ds2) == false) return;
                        int ReCnt2 = Temp_Connect4.DataSet_ReCount;
                        if (ReCnt2 == 0)
                        {
                            string primium_custom_string = ds.Tables["tbl_salesdetail"].Rows[0][0].ToString();

                            if (primium_custom_string == "04")
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("The ID has already ordered a premium custom pack."
                                  + "\n" +
                                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                }
                                else
                                {
                                    MessageBox.Show("해당아이디는 이미 프리미엄 커스텀팩이 주문됐습니다."
                              + "\n" +
                              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                }
                                return;
                            }
                            else
                            {
                                txtSellCode_Code.Text = "04";
                            }
                        }
                    }
                    int primium_custom = int.Parse(txt_SumPr.Text.Replace(",", ""));
                    if (primium_custom < 800000 || primium_custom > 1000000)
                    {
                        if (cls_User.gid_CountryCode == "TH")
                        {
                            MessageBox.Show("To apply for a premium custom pack, " + "\n" + " The order must be over 800,000 won and less than 1 million won."
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        }
                        else
                        {
                            MessageBox.Show("프리미엄 커스텀팩으로 주문신청하기 위해서는" + "\n" + " 80만원이상, 100만원이하 주문이어야 합니다."
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        }
                        return;
                    }
                    cls_Connect_DB Temp_Connect1 = new cls_Connect_DB();
                    string Tsql1 = "select sell_mem_tf from tbl_memberinfo (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "'";
                    DataSet ds1 = new DataSet();
                    if (Temp_Connect1.Open_Data_Set(Tsql1, "tbl_memberinfo", ds1) == false) return;
                    int ReCnt1 = Temp_Connect1.DataSet_ReCount;

                    if (ReCnt1 > 0)
                    {
                        string sell_mem_tf = ds1.Tables["tbl_memberinfo"].Rows[0][0].ToString();
                        if (sell_mem_tf == "1")
                        {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                MessageBox.Show("You must be a seller to apply for an order with a premium custom pack."
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            }
                            else
                            {
                                MessageBox.Show("프리미엄 커스텀팩으로 주문신청하기 위해서는 판매자여야합니다."
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            }
                            return;
                        }
                    }
                }
            }
            //20201012 김대리요청 배송내역 없으면 승인 안되도록 한다
            if (Item_Rece_Error_Check__01("Rece") == false) return;
            //20230312구현호 1 카드승인시 돈다.
            if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

            if (Item_Rece_Error_Check__01("Cacu", 1) == false) return;

            //20230312구현호 2 각종 카드승인관련 빈값체크
            if (txt_C_Card_Number.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("To automatically authorize your card, you must enter the approved amount."
             + "\n" +
             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                else
                {
                    MessageBox.Show("카드 자동 승인을 하실려면 카드번호를 필히 입력 하셔야 합니다."
                  + "\n" +
                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                tab_Cacu.SelectedIndex = 0;
                txt_C_Card_Number.Focus(); return;
            }
            if (txt_Price_3_2.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("To automatically authorize your card, you must enter the approved amount."
             + "\n" +
             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                else
                {
                    MessageBox.Show("카드 자동 승인을 하실려면 승인 금액을 필히 입력 하셔야 합니다."
                  + "\n" +
                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                tab_Cacu.SelectedIndex = 0;
                txt_Price_3_2.Focus(); return;
            }

            if (combo_C_Card_Year.Text == "" || combo_C_Card_Month.Text == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("For automatic card approval, you must enter the expiration date."
                     + "\n" +
                     cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                else
                {
                    MessageBox.Show("카드 자동 승인을 하실려면 유효기간을 필히 입력 하셔야 합니다."
                     + "\n" +
                     cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                tab_Cacu.SelectedIndex = 0;
                txt_C_Card_Year.Focus(); return;
            }

            if (txt_UnaccMoney.Text == "")
                txt_UnaccMoney.Text = "0";

            double P_r = double.Parse(txt_Price_3.Text.Replace(",", ""));


            // 한국에서만 비교
            if (cls_User.gid_CountryCode != "TH" && tableLayoutPanel72.Visible == true)
            {
                //if (txt_C_P_Number.MaxLength != txt_C_P_Number.Text.Length)
                //{
                //    MessageBox.Show("카드 비밀번호 앞 2자리를 올바르게 입력해 주십시요"
                //               + "\n" +
                //              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                //    return;
                //}

                if (txt_C_B_Number.Text.Length < 6)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Please enter the correct 6-digit date of birth. EX:4/8/2372 -> 720408"
                              + "\n" +
                             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {

                        MessageBox.Show("생년월일 6자리를 올바르게 입력해 주십시요. EX:2372년 4월 8일  -> 720408"
                               + "\n" +
                              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    return;
                }
            }

            // 태국에서만 비교
            if (cls_User.gid_CountryCode == "TH" && txt_C_CVC.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please enter your card security code.");
                }
                else
                {

                    MessageBox.Show("카드 보안코드를 입력해 주시기 바랍니다.");
                }
                txt_C_CVC.Focus();
                return;
            }


            if (txt_OrderNumber.Text.Trim() != "")
            {
                if (Cacu_Card_Error_Check__01() == false)
                    return;
            }

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            int C_index = 0;

            if (txt_C_index.Text == "") //추가 일경우에 새로운 입력
            {
                Base_Sub_Save_Cacu(3, ref C_index);
                Base_Sub_Clear("Cacu");
                Base_Sub_Sum_Cacu();
                Save_Button_Click_Cnt++;
            }
            else  //
            {
                
                        C_index = int.Parse(txt_C_index.Text);
                txt_Sugi_TF.Text = "1";
                Base_Sub_Edit_Cacu();
                if (couponnumber == "1")
                {
                    couponnumber = "0";
                    return;
                }
                Base_Sub_Clear("Cacu");
                Base_Sub_Sum_Cacu();
                Save_Button_Click_Cnt++;
            }

            int Save_Error_Check = 0;
            string OrderNumber = "";

            if (txt_OrderNumber.Text.Trim() != "")
            {
                OrderNumber = txt_OrderNumber.Text.Trim();
                Save_Base_Data(ref Save_Error_Check, ref OrderNumber);
            }
            else
            {
                OrderNumber = "";
                Save_Base_Data(ref Save_Error_Check, ref OrderNumber);
            }


            //매출 저장중이나 수정중에 오류가 발생 되어 있다 그럼 나머지 작업 하지 말고 걍 나가라
            if (Save_Error_Check == 0)
            {
                Sales_Cacu[C_index].C_Price1 = 0;
                foreach (DataGridViewRow row in dGridView_Base_Cacu.Rows)
                {
                    if (row.Cells["C_index"].Value.ToString() == C_index.ToString())
                    {
                        row.Cells["C_Price1"].Value = "0";
                    }
                }

                Base_Sub_Sum_Cacu();
                this.Cursor = System.Windows.Forms.Cursors.Default;
                return;
            }
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            cls_Web Cls_Web = new cls_Web();

            string SuccessYN = "";
            string Err_M = "";

            SuccessYN = Cls_Web.Dir_Card_Approve_OK_Err(OrderNumber, C_index, ref Err_M);




            if (SuccessYN != "Y" && Err_M != "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Card authorization error : "
                        + "\n" +
                       Err_M);
                }
                else
                {

                    MessageBox.Show("카드승인 오류 : "
                               + "\n" +
                              Err_M);
                }
            }

            //           다돌고 온다. sp_helptext Usp_Sell_Cacu_ReCul 
            if (Save_Error_Check > 0)
            {

                string StrSql = "";//프리미엄커스텀 체크
                StrSql = "EXEC Usp_Sell_Cacu_ReCul '" + OrderNumber + "'";
                Temp_Connect.Update_Data(StrSql, "", "");


                if (txtSellCode_Code.Text == "04")
                {
                    StrSql = "EXEC  Usp_Sales_Prom_Primium_Custom '" + OrderNumber + "'";
                    Temp_Connect.Update_Data(StrSql, "", "");
                }
                //20210319 구현호 승인안됐으면 문자보내지않는다
                if (SuccessYN == "Y") //주문결제된 내역의 SMS를 보낸다.
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        //SuccessYN = "";
                        //string ErrMessage = "";
                        //string SuccessYN_Card = "N";
                        //if (SuccessYN_Card == "N")
                        //{

                        //    SuccessYN_Card = Cls_Web.TH_SMS(idx_Mbid2, Temp_Ordernumber, 4, ref ErrMessage);

                        //}
                        //if (SuccessYN_Card == "Y")
                        //    SuccessYN = "Y";
                        //else
                        //    SuccessYN = "N";


                        //if (SuccessYN_Card == "N")
                        //    ErrMessage = ErrMessage + " 문자에러";

                        //if (SuccessYN == "Y")
                        //{
                        //    MessageBox.Show("문자발송성공");
                        //}

                        StrSql = "EXEC [Usp_TH_SMS]   " + idx_Mbid2 + ",'" + Temp_Ordernumber + "','','4'";
                        // Mail 호출 - 주문완료
                        new cls_Web().SendMail_TH(idx_Mbid2, Temp_Ordernumber, string.Empty, string.Empty, ESendMailType_TH.orderCompleteMail);
                    }
                    else
                    {
                        //EXEC Usp_Insert_SMS '2?', '회원번호1', 회원번호2, '주문번호', ''
                        StrSql = "EXEC Usp_Insert_SMS_New  '20',''," + idx_Mbid2 + ",'" + Temp_Ordernumber + "', ''";
                        //StrSql = "EXEC Usp_Insert_SMS '20',''," + idx_Mbid2 + ",'" + Temp_Ordernumber + "', ''";
                        Temp_Connect.Update_Data(StrSql, "", "");
                    }
                }


                System.Threading.Thread.Sleep(1000);
                if (cls_app_static_var.Sell_Union_Flag == "D" && (txt_Ins_Number.Text.Trim() == "" || txt_Ins_Number.Text.Trim().Substring(0, 7) == "재발급요청요망"))
                {
                    if (idx_Na_Code.Replace(" ", "") == "KR")
                    {
                        InsuranceNumber_Ord_Print_FLAG = OrderNumber;
                        Sell_Ac_insurancenumber(OrderNumber);//직판 관련 승인 번호를 받아온다.                
                        InsuranceNumber_Ord_Print_FLAG = "";
                    }
                }

                /* 직접 수령 자동 출고*/
                //StrSql = " EXEC Usp_Sell_Auto_StockOut_Insert '" + OrderNumber + "' ";
                //Temp_Connect.Update_Data(StrSql, "", "");


                Base_Ord_Clear();

                if (SalesDetail != null)
                    SalesDetail.Clear();

                Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

                if (SalesDetail != null)
                    Base_Grid_Set();

                Put_OrderNumber_SellDate(OrderNumber);
            }


            if (SuccessYN != "Y")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("There was a problem authorizing your card. Please contact the company.");
                }
                else
                {
                    MessageBox.Show("카드 승인중에 문제가 발생했습니다.  업체에 문의해 주십시요.");
                }
                this.Cursor = System.Windows.Forms.Cursors.Default;
                return;
            }

            cls_form_Meth ct = new cls_form_Meth();

            combo_C_Card_Year.SelectedIndex = 0;
            txt_C_Etc.Text = "";
            butt_Cacu_Del.Visible = false;
            butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
            tab_Cacu.Enabled = true;

            enable_Card_info_txt(true);
            enable_Av_Bank_info_txt(true);

            if (cls_app_static_var.Member_Card_Sugi_TF == 1)
                button_Ok.Visible = true;
            else
                button_Ok.Visible = false;

            button_Cancel.Visible = false;
            tableL_CD.Visible = false;

            Put_Sub_Date(C_index.ToString(), "Cacu");


            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Work_End"));

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

            if (txt_OrderNumber.Text.Trim() != "")
            {
                if (Cacu_Card_Error_Check__01() == false)
                    return;


                //공제번호가 있으면 삭제가 안되게 한다. 우선 먼저 공제번호를 취소한후에 다시 시도하게 한다.
                cls_form_Meth cm = new cls_form_Meth();
                if (SalesDetail[txt_OrderNumber.Text.Trim()].Union_Seq > 0 && cls_app_static_var.Sell_Union_Flag == "U")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Chang_Insur_Number")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    button_Cancel.Focus(); return;

                }
            }


            int C_index = int.Parse(txt_C_index.Text);

            cls_Web Cls_Web = new cls_Web();
            string ErrMessage = "";
            string SuccessYN = "";


            SuccessYN = Cls_Web.Dir_Card_Approve_Cancel(txt_OrderNumber.Text, C_index, ref ErrMessage);
            

            if (SuccessYN == "N")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("A problem occurred while canceling the card.\nPlease check with the computer staff." + Environment.NewLine +
                         "PG Message : " + ErrMessage);
                }
                else
                {

                    MessageBox.Show("카드 취소중 문제가 발생했습니다.\n전산담당자에게 확인 부탁드립니다." + Environment.NewLine +
                    "PG Message : " + ErrMessage);
                }
                return;
            }

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string OrderNumber = txt_OrderNumber.Text;


            string StrSql = "";//프리미엄커스텀 체크
            StrSql = "EXEC Usp_Sell_Cacu_ReCul '" + OrderNumber + "'";
            Temp_Connect.Update_Data(StrSql, "", "");


            if (txtSellCode_Code.Text == "04")
            {
                StrSql = "EXEC  Usp_Sales_Prom_Primium_Custom '" + OrderNumber + "'";
                Temp_Connect.Update_Data(StrSql, "", "");

            }


            System.Threading.Thread.Sleep(1000);

            Base_Ord_Clear();

            if (SalesDetail != null)
                SalesDetail.Clear();

            Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

            if (SalesDetail != null)
                Base_Grid_Set();

            Put_OrderNumber_SellDate(OrderNumber);


            cls_form_Meth ct = new cls_form_Meth();

            combo_C_Card_Year.SelectedIndex = 0;
            txt_C_Etc.Text = "";
            butt_Cacu_Del.Visible = false;
            butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
            tab_Cacu.Enabled = true;

            enable_Card_info_txt(true);
            enable_Av_Bank_info_txt(true);
            if (Card_Ok_Visible == false)
            {
                button_Ok.Visible = false;
            }
            else
            {
                if (cls_app_static_var.Member_Card_Sugi_TF == 1)
                    button_Ok.Visible = true;
                else
                    button_Ok.Visible = false;
            }
            button_Cancel.Visible = false;
            tableL_CD.Visible = false;

            Put_Sub_Date(C_index.ToString(), "Cacu");
            
            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Work_End"));
        }

        private void butt_Excel_Click(object sender, EventArgs e)
        {

        }

        private void buttonV_Ok_Click(object sender, EventArgs e)
        {
            if (txt_AVC_Cash_Send_Nu.Text == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Cash receipt application number has not been entered.");
                }
                else
                {
                    MessageBox.Show("현금영수증 신청번호가 입력되지 않았습니다.");
                }
                return;
            }
            if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

            if (Item_Rece_Error_Check__01("Cacu") == false) return;


            if (txt_Price_5_2.Text.Trim() == "") txt_Price_5_2.Text = "0";

            if (double.Parse(txt_Price_5_2.Text) == 0)
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Virtual Account_Please enter the requested amount. Please check and try again.");
                }
                else
                {

                    MessageBox.Show("가상계좌_요청 금액을 입력해 주십시요.  확인후 다시 시도해 주십시요.");
                }
                txt_Price_5_2.Focus();
                return;
            }

            //if (txtBank_Code.Text.Trim() == "")
            //{
            //    MessageBox.Show("신청 은행을 선택해 주십시요.  확인후 다시 시도해 주십시요.");
            //    txtBank.Focus();
            //    return;
            //}

            if (txt_OrderNumber.Text.Trim() != "")
            {
                if (Cacu_Card_Error_Check__01() == false)
                    return;
            }

            string VA_DEST_TEL = txt_VA_DEST_TEL.Text;
            if (txt_UnaccMoney.Text == "")
                txt_UnaccMoney.Text = "0";

            double P_r = double.Parse(txt_Price_5_2.Text.Replace(",", ""));

            if (P_r > double.Parse(txt_UnaccMoney.Text.Replace(",", "")))
            {
                //MessageBox.Show("가상계좌요청 금액이 결제해야할 금액보다 큽니다. 계속 진행하시겠습니까?"
                //  + "\n" +
                //  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                //txt_Price_5_2.Focus(); return;

                string m_Sg = "";


                if (cls_User.gid_CountryCode == "TH")
                {
                    m_Sg = "The virtual account request amount is greater than the amount to be paid. Would you like to proceed?";
                    m_Sg = m_Sg + "\n";
                    m_Sg = m_Sg + "Accounts receivables may occur. If you proceed, please check it out later!!";
                }
                else
                {

                    m_Sg = "가상계좌요청 금액이 결제해야할 금액보다 큽니다. 계속 진행하시겠습니까?";
                    m_Sg = m_Sg + "\n";
                    m_Sg = m_Sg + "미수금 내역이 발생 할 수 있습니다. 진행하기게 되면 추후 필히 확인해 주십시요!!";
                }
                if (MessageBox.Show(m_Sg, "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    txt_Price_5_2.Focus(); return;
                }

            }




            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            int C_index = 0;

            if (txt_C_index.Text == "") //추가 일경우에 새로운 입력
            {
                Base_Sub_Save_Cacu(5, ref C_index);
                Base_Sub_Clear("Cacu");
                Base_Sub_Sum_Cacu();
                Save_Button_Click_Cnt++;
            }
            else  //
            {
                C_index = int.Parse(txt_C_index.Text);
                Base_Sub_Edit_Cacu();
                if (couponnumber == "1")
                {
                    couponnumber = "0";
                    return;
                }
                Base_Sub_Clear("Cacu");
                Base_Sub_Sum_Cacu();
                Save_Button_Click_Cnt++;
            }



            int Save_Error_Check = 0;
            string OrderNumber = "";
            bool FirstStart = txt_OrderNumber.Text.Trim().Equals(string.Empty);

            if (FirstStart.Equals(false))
            {
                OrderNumber = txt_OrderNumber.Text.Trim();
                Save_Base_Data(ref Save_Error_Check, ref OrderNumber);
            }
            else
            {
                OrderNumber = "";
                Save_Base_Data(ref Save_Error_Check, ref OrderNumber);
            }
            //매출 저장중이나 수정중에 오류가 발생 되어 있다 그럼 나머지 작업 하지 말고 걍 나가라
            if (Save_Error_Check == 0)
            {
                Base_Sub_Sum_Cacu();
                this.Cursor = System.Windows.Forms.Cursors.Default;
                return;
            }


            //cls_Cash_Card_Admin_Cancel cccA = new cls_Cash_Card_Admin_Cancel();
            //int ret = cccA.Cash_Card_Send_Singo_OK(OrderNumber, mtxtMbid.Text.Trim(), "Bank", C_index);

            string SuccessYN = "";
            string SuccessMessage = "";

            cls_Web Cls_Web = new cls_Web();
            string StrSql = string.Empty;
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            SuccessYN = Cls_Web.Dir_VR_Account_Approve_OK(OrderNumber, C_index, idx_Mbid2, ref SuccessMessage);

            if (Save_Error_Check > 0 && SuccessYN.Equals("Y"))
            {
               

                StrSql = "EXEC Usp_Sell_Cacu_ReCul '" + OrderNumber + "'";
                Temp_Connect.Update_Data(StrSql, "", "");


                if (txtSellCode_Code.Text == "04")
                {
                    StrSql = "EXEC  Usp_Sales_Prom_Primium_Custom '" + OrderNumber + "'";
                    Temp_Connect.Update_Data(StrSql, "", "");
                }

                
                System.Threading.Thread.Sleep(1000);
                if (cls_app_static_var.Sell_Union_Flag == "D" && (txt_Ins_Number.Text.Trim() == "" || txt_Ins_Number.Text.Trim().Substring(0, 7) == "재발급요청요망"))
                {
                    if (idx_Na_Code.Replace(" ", "") == "KR")
                    {
                        InsuranceNumber_Ord_Print_FLAG = OrderNumber;
                        Sell_Ac_insurancenumber(OrderNumber);//직판 관련 승인 번호를 받아온다.                
                        InsuranceNumber_Ord_Print_FLAG = "";
                    }
                }
                                              

                Base_Ord_Clear();

                if (SalesDetail != null)
                    SalesDetail.Clear();

                Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

                if (SalesDetail != null)
                    Base_Grid_Set();

                Put_OrderNumber_SellDate(OrderNumber);
            }
                        

            if (SuccessYN.Equals("Y"))
            {
                StrSql = "EXEC Usp_Insert_SMS_New  '25',''," + idx_Mbid2  + ",'" + OrderNumber + "', ''";  //가상계좌 관련 알리톡을 내보낸다
                //StrSql = "EXEC Usp_Insert_SMS '25',''," + idx_Mbid2 + ",'" + OrderNumber + "', ''";  //가상계좌 관련 알리톡을 내보낸다
                Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text);
            }

            if (SuccessYN == "N")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("A problem occurred while requesting a virtual account. Please contact the company." + Environment.NewLine +
                   "PG Message : " + SuccessMessage);
                }
                else
                {

                    MessageBox.Show("가상계좌 요청중에 문제가 발생했습니다.  업체에 문의해 주십시요." + Environment.NewLine +
                    "PG Message : " + SuccessMessage);
                }
                return;
            }



            cls_form_Meth ct = new cls_form_Meth();

            txt_C_Etc.Text = "";
            butt_Cacu_Del.Visible = false;
            butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
            tab_Cacu.Enabled = true;

            enable_Card_info_txt(true);
            enable_Av_Bank_info_txt(true);
            buttonV_Ok.Visible = true;
            buttonV_Cancel.Visible = false;

            Put_Sub_Date(C_index.ToString(), "Cacu");


            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Work_End"));

            this.Cursor = System.Windows.Forms.Cursors.Default;

        }

        private void e_f_Send_Accnt_Info(string SuccessYN, string Message)
        {
            if (SuccessYN == "Y")
            {
                //MessageBox.Show("");
            }
            else
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("A problem occurred while requesting a virtual account. Please contact the company.");
                }
                else
                {

                    MessageBox.Show("가상계좌 요청중에 문제가 발생했습니다. 업체에 문의해 주십시요.");
                }
                return;
            }
        }
        void e_f_Call_Accnt_Info(ref string OrderNumber, ref int C_Index)
        {

        }
        private void buttonV_Cancel_Click(object sender, EventArgs e)
        {
            if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

            if (txt_OrderNumber.Text.Trim() != "")
            {
                if (Cacu_Card_Error_Check__01() == false)
                    return;
            }

            int C_index = int.Parse(txt_C_index.Text);

            ////cls_Kicc_Send cks = new cls_Kicc_Send();
            ////int ret_C1 = cks.Cash_Card_Send_Singo_Cancel(txt_OrderNumber.Text, mtxtMbid.Text.Trim(), "Bank", C_index);

            cls_Cash_Card_Admin_Cancel cccA = new cls_Cash_Card_Admin_Cancel();
            int ret_C1 = cccA.Cash_Card_Send_Singo_Cancel(txt_OrderNumber.Text, mtxtMbid.Text.Trim(), "Bank", C_index);

            if (ret_C1 == 1)
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("There was a problem canceling the virtual account. Please contact the company.");
                }
                else
                {
                    MessageBox.Show("가상계좌 취소중에 문제가 발생했습니다. 업체에 문의해 주십시요.");
                }
                return;
            }

            if (ret_C1 == 100)
            {
                string m_Sg = "";
                if (cls_User.gid_CountryCode == "TH")
                {
                    m_Sg = "There was a problem canceling the virtual account. Would you like to proceed?";
                    m_Sg = m_Sg + "\n";
                    m_Sg = m_Sg + "If you continue, you must manually cancel the virtual account.";
                }
                else
                {
                    m_Sg = "가상계좌 취소중에 문제가 발생했습니다. 계속 진행하시겠습니까?";
                    m_Sg = m_Sg + "\n";
                    m_Sg = m_Sg + "계속 진행시 가상계좌는 메뉴얼로 직접 취소하셔야 합니다.";
                }
                if (MessageBox.Show(m_Sg, "", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }


            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string OrderNumber = txt_OrderNumber.Text;


            string StrSql = "";//프리미엄커스텀 체크
            StrSql = "EXEC Usp_Sell_Cacu_ReCul '" + OrderNumber + "'";
            Temp_Connect.Update_Data(StrSql, "", "");


            if (txtSellCode_Code.Text == "04")
            {
                StrSql = "EXEC  Usp_Sales_Prom_Primium_Custom '" + OrderNumber + "'";
                Temp_Connect.Update_Data(StrSql, "", "");

            }



            System.Threading.Thread.Sleep(2000);

            Base_Ord_Clear();

            if (SalesDetail != null)
                SalesDetail.Clear();

            Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

            if (SalesDetail != null)
                Base_Grid_Set();

            Put_OrderNumber_SellDate(OrderNumber);


            cls_form_Meth ct = new cls_form_Meth();

            txt_C_Etc.Text = "";
            butt_Cacu_Del.Visible = false;
            butt_Cacu_Save.Text = ct._chang_base_caption_search("추가");
            tab_Cacu.Enabled = true;

            enable_Card_info_txt(true);
            enable_Av_Bank_info_txt(true);
            buttonV_Ok.Visible = true;
            buttonV_Cancel.Visible = false;

            Put_Sub_Date(C_index.ToString(), "Cacu");

            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Work_End"));

        }

        private void txt_InputPass_Pay_TextChanged(object sender, EventArgs e)
        {

            if (Data_Set_Form_TF == 1)
                return;
            Base_Sub_Sum_Cacu();
        }

        private void MtxtSellDate2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSellCode_Code_Leave(object sender, EventArgs e)
        {
            if (new string[] { "txtSellCode_Code", "txtSellCode" }.Any(x => x.Equals((sender as Control).Name)))
            {
                if (!SetSellCode()) return;
            }
        }

        private void mtxtMbid_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void btnGetMemberHPTel_Click(object sender, EventArgs e)
        {
            this.txt_VA_DEST_TEL.Text = this.txt_Tel.Text;
        }

        private void btn_CASH_OK_Click(object sender, EventArgs e)
        {
            if(txt_C_Cash_Send_Nu.Text == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Cash receipt application number has not been entered.");
                }
                else
                {

                    MessageBox.Show("현금영수증 신청번호가 입력되지 않았습니다.");
                }
                return;
            }
            if (butt_Cacu_Save.Visible)
            {
                this.Base_Small_Button_Click(butt_Cacu_Save, null);


                this.Base_Button_Click(butt_Save, null);

            }
        }

        private void SendCashReceipt_OK_Danal(string OrderNumber)
        {
            string Tsql = " Select OrderNumber, C_Index , C_CASH_SEND_TF  from tbl_Sales_Cacu (nolock) ";
            Tsql = Tsql + " Where C_TF = 1 AND OrderNumber = '" + OrderNumber + "' AND C_Cash_Number = '' AND ( C_Cash_Send_TF = 1 or C_Cash_Send_TF = 2 ) and C_CancelTF = 0 ";
            //20220914구현호 가상계좌도..
            //Tsql = Tsql + " Where  OrderNumber = '" + OrderNumber + "' AND C_Cash_Number = '' AND ( C_Cash_Send_TF = 1 or C_Cash_Send_TF = 2 ) and C_CancelTF = 0 ";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            cls_Web web = new cls_Web();

            if (Temp_Connect.Open_Data_Set(Tsql, "Cash", ds) && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables["Cash"].Rows)
                {
                    string rOrderNumber = row["OrderNumber"].ToString();
                    int rC_index = int.Parse(row["C_Index"].ToString());
                    string rSend_Number = row["C_CASH_SEND_TF"].ToString().Replace("-", "").Replace(" ", "");

                    web.Dir_Cash_Receipt_Approve(OrderNumber, rC_index, rSend_Number);
                }

            }
        }


        private void SendCashReceipt_Cancel_Danal(string OrderNumber)
        {
            string Tsql = "select OrderNumber, C_Index  from tbl_Sales_Cacu (nolock)  where C_TF = 1 AND OrderNumber = '" + OrderNumber + "' AND C_Cash_Number <> '' AND C_Cash_Send_TF = 1 and C_CancelTF = 0  ";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            cls_Web web = new cls_Web();

            if (Temp_Connect.Open_Data_Set(Tsql, "Cash", ds) && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables["Cash"].Rows)
                {
                    string rOrderNumber = row["OrderNumber"].ToString();
                    int rC_index = int.Parse(row["C_Index"].ToString());
                    //string rSend_Number = row["C_CASH_SEND_TF"].ToString().Replace("-", "").Replace(" ", "");

                    web.Dir_VR_Cash_Receipt_All_Cancel(OrderNumber, rC_index);
                }

            }
        }

        private void btnCashReceiptCancel_Click(object sender, EventArgs e)
        {
            SendCashReceipt_Cancel_Danal(txt_OrderNumber.Text);
            VisibleCashReceiptButton();
        }

        private void VisibleCashReceiptButton()
        {

            string Tsql = "select * from tbl_Sales_Cacu (nolock)  where C_TF = 1 AND OrderNumber = '" + txt_OrderNumber + "' AND C_Cash_Number <> '' ";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            cls_Web web = new cls_Web();

            if (Temp_Connect.Open_Data_Set(Tsql, "Cash", ds) && ds.Tables[0].Rows.Count > 0)
            {
                btnCashReceiptCancel.Visible = false;
            }
            else
            {
                btnCashReceiptCancel.Visible = false;
            }


        }

        private void mtxtSellDate_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void butt_Coupon_Search_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Mem")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return;
            }

            //dGridView_Base_Coupon.Width = groupBox3.Width - 10;
            //dGridView_Base_Coupon.Height = groupBox3.Height - 18;
            //dGridView_Base_Coupon.Left = groupBox3.Left + 5;
            //dGridView_Base_Coupon.Top = groupBox3.Top + 15;

            Coupon_Grid_Set();

            //dGridView_Base_Coupon.BringToFront();
            ////dGridView_Base_Coupon.RowHeadersVisible = false;
            //dGridView_Base_Coupon.Visible = true;
            //dGridView_Base_Coupon.Focus();
        }

        private void Coupon_Grid_Set()
        {
            dGridView_Coupon_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Coupon.d_Grid_view_Header_Reset();
            string strSql = "";

            strSql = "select a.mbid2,b.M_Name,a.COUPONNUMBER, a.SALEPRICE, a.RECORDTIME,CASE When USEYN = 'Y'THEN '사용불가' When USEYN = 'N' AND USEENDDATE < CONVERT(CHAR(23), GETDATE(), 21)     THEN '만료'  When USEYN = 'N' AND USEENDDATE >CONVERT(CHAR(23), GETDATE(), 21)     THEN '사용가능'  end  ,a.USEENDDATE from TLS_COUPON a  join tbl_memberinfo  b on a.mbid2 = b.mbid2  ";


            if (txt_mbid2_6.Text != "")
            {
                strSql = strSql + " where a.mbid2 like '%" + txt_mbid2_6.Text.ToString().Replace("-", "").Replace("_", "").Replace(" ", "") + "%'";
            }
            else
            {
                if(mtxtMbid.Text != "")
                {
                    strSql = strSql + " where a.mbid2 like '%" + mtxtMbid.Text.ToString().Replace("-", "").Replace("_", "").Replace(" ", "") + "%'";
                }
                else
                {
                    return;
                }
                
            }

            //strSql = strSql + " Where TLS_COUPON.Mbid2 = '" + mtxtMbid.Text.ToString().Replace("-", "").Replace("_", "") + "'";

            strSql = strSql + " Order by RECORDTIME";



            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, "TempTable", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;
            if (ReCnt == 0) return;

            cls_form_Meth cm = new cls_form_Meth();

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_Coupon_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }


            cgb_Coupon.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Coupon.db_grid_Obj_Data_Put();
        }

        private void Set_gr_Coupon_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {

            object[] row0 = { ds.Tables["TempTable"].Rows[fi_cnt][0]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][1]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][2]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][3]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][4]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][5]
                                ,ds.Tables["TempTable"].Rows[fi_cnt][6]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Coupon_Header_Reset()
        {
            cgb_Coupon.Grid_Base_Arr_Clear();
            cgb_Coupon.basegrid = dGridView_Base_Coupon;
            cgb_Coupon.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Coupon.grid_col_Count = 7;
            cgb_Coupon.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            cgb_Coupon.Sort_Mod_Auto_TF = 1;

            string[] g_HeaderText = {"회원번호","이름",  "쿠폰번호", "금액" , "기록일"   
                                   , "사용구분", "종료일"
                                };

            int[] g_Width = { 80 ,80, 250, 200,100,100,100
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleLeft //5    
                                              ,DataGridViewContentAlignment.MiddleLeft //5    
                                                            ,DataGridViewContentAlignment.MiddleLeft //5    
                                                            ,DataGridViewContentAlignment.MiddleLeft //5    
                                };

            cgb_Coupon.grid_col_header_text = g_HeaderText;
            cgb_Coupon.grid_col_w = g_Width;
            cgb_Coupon.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true,  true,  true  ,true
                                   };
            cgb_Coupon.grid_col_Lock = g_ReadOnly;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb_Coupon.grid_cell_format = gr_dic_cell_format;

            //cgb_Mile.basegrid.RowHeadersVisible = false;
        }

        private void dGridView_Base_Coupon_DoubleClick(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs ee = (System.Windows.Forms.MouseEventArgs)e;
            if (ee.Button == MouseButtons.Left)
            {
                

                DataGridView dgv = (DataGridView)sender;
                if (mtxtMbid.Text == "") return;
                String Coupon_use;
                Coupon_use = dgv.CurrentRow.Cells[5].Value.ToString();
                if (Coupon_use == "사용불가" || Coupon_use == "만료")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Coupons that have already been used or expired. can not use."
       + "\n" +
       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {

                        MessageBox.Show("이미 사용하거나 종료일이 지난 쿠폰입니다. 사용 할 수 없습니다."
          + "\n" +
          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    tab_Cacu.SelectedIndex = 4;
                    mtxtPriceDate6.Focus(); return;
                }

                txt_Price_6_2.Text = dgv.CurrentRow.Cells[2].Value.ToString();
                txt_Price_6.Text = dgv.CurrentRow.Cells[3].Value.ToString().Replace(".0000","");
            }
        }
        

        private void chk_Primium_Custom_CheckedChanged(object sender, EventArgs e)
        {
            if (txt_OrderNumber.Text != "")
            {
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                string Tsql = "select sellcode from tbl_salesdetail (NOLOCK) where  ReturnTF = 1 and  ordernumber = '" + txt_OrderNumber.Text + "'";
                DataSet ds = new DataSet();
                if (Temp_Connect.Open_Data_Set(Tsql, "tbl_salesdetail", ds) == false) return;
                string primium_custom = ds.Tables["tbl_salesdetail"].Rows[0][0].ToString();

                if (primium_custom == "04")
                {
                    txtSellCode_Code.Text = "04";
                }
                else
                {
                }

            }
        }


        //tbl_SalesDetail_Be_Prim_Sell 
        private void txtSellCode_Code_TextChanged(object sender, EventArgs e)
        {
            if (txt_OrderNumber.Text != "")
            {

                if (txtSellCode_Code.Text =="04")
                {
                    cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                    string Tsql = "select sellcode from tbl_salesdetail (NOLOCK)  where  ReturnTF = 1 and  mbid2 = '" + mtxtMbid.Text + "' and sellcode = '04'";
                    DataSet ds = new DataSet();
                    if (Temp_Connect.Open_Data_Set(Tsql, "tbl_salesdetail", ds) == false) return;
                    int ReCnt = Temp_Connect.DataSet_ReCount;

                    if (ReCnt > 0)
                    {
                        cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
                        string Tsql2 = "select sellcode from tbl_salesdetail (NOLOCK) where  mbid2 = '" + mtxtMbid.Text + "' and sellcode = '04' AND ReturnTF in(2,3,4,5) ";
                        DataSet ds3 = new DataSet();
                        if (Temp_Connect2.Open_Data_Set(Tsql2, "tbl_salesdetail", ds3) == false) return;
                        int ReCnt4 = Temp_Connect2.DataSet_ReCount;
                        if (ReCnt4 == 0)
                        {
                            string primium_custom = ds.Tables["tbl_salesdetail"].Rows[0][0].ToString();

                            if (primium_custom == "04")
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("The ID has already ordered a premium custom pack."
                                             + "\n" +
                                             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                }
                                else
                                {
                                    MessageBox.Show("해당아이디는 이미 프리미엄 커스텀팩이 주문됐습니다."
                                  + "\n" +
                                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                                }
                                return;
                            }
                            else
                            {
                                txtSellCode_Code.Text = "04";
                            }
                        }
                    }


                    //이전에 회원명단 받아서 넣기로함.. 2020-09-27 김영수 팀장.
                    Tsql = " Select Mbid from tbl_SalesDetail_Be_Prim_Sell (NOLOCK)  ";
                    Tsql = Tsql + "  Where  mbid2 = " + mtxtMbid.Text.Trim();

                    DataSet ds2 = new DataSet();
                    if (Temp_Connect.Open_Data_Set(Tsql, "tbl_salesdetail", ds2) == false) return;
                    int ReCnt2 = Temp_Connect.DataSet_ReCount;

                    if (ReCnt2 > 0)
                    {
                        if (cls_User.gid_CountryCode == "TH")
                        {
                            MessageBox.Show("The ID has already ordered a premium custom pack."
                              + "\n" +
                              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        }
                        else
                        {
                            MessageBox.Show("해당아이디는 이미 프리미엄 커스텀팩이 주문됐습니다."
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        }
                        return;                        
                    }


                    

                }
            }
        }

        private void tab_Cacu_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void tab_Cacu_Click(object sender, EventArgs e)
        {

        }

        private void txt_OrderNumber_TextChanged(object sender, EventArgs e)
        {
            //if(txt_OrderNumber.Text.Trim() == string.Empty)
            //{
            //    txtCenter3.Enabled = true;
            //    txt_Receive_Method.Enabled = true;
            //}
            //if (txt_OrderNumber.Text.Trim() == string.Empty)
            //{
            //    txtCenter3.Enabled = false;
            //    txt_Receive_Method.Enabled = false;
            //}

        }

        private void panel51_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dGridView_Base_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button_Auth_Click(object sender, EventArgs e)
        {
            string C_Card_Number = "";
            string C_Card_Year = "";
            string C_Card_Month = "";
            string C_B_Number = "";
            C_Card_Number = txt_C_Card_Number.Text.Trim();
            C_Card_Year = txt_C_Card_Year.Text.Trim();
            C_Card_Month = txt_C_Card_Month.Text.Trim();
            C_B_Number = txt_C_B_Number.Text.Trim();
            int C_index = 0;

            cls_Web Cls_Web = new cls_Web();

            string SuccessYN = "";
            string Err_M = "";
            string rAcquirerNm = "";
            //20230312구현호 3 카드결제관련 성공여부에 관련된 클래스, ordernumber = 주문번호, c_index = 0,Err_M = ""
            SuccessYN = Cls_Web.Dir_Card_Approve_OK_Err_2(C_Card_Number, C_Card_Year, C_Card_Month, C_B_Number,ref Err_M, ref rAcquirerNm);
            

            if (SuccessYN != "Y")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Card authentication has not been completed. \n card : " + rAcquirerNm + " \n reason : " + Err_M + "");
                }
                else
                {
                    MessageBox.Show("카드인증이 완료되지 않았습니다. \n 카드사 : " + rAcquirerNm + " \n 사유 : " + Err_M + "");
                }
            }
            else
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Card authentication is complete.");
                }
                else
                {

                    MessageBox.Show("카드인증이 완료됐습니다.");
                }
            }
        }

        private void butt_All_Card_Click(object sender, EventArgs e)
        {
            string T_Mbid = mtxtMbid.Text;
            string Mbid = ""; int Mbid2 = 0;

            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == -1) //올바르게 회원번호 양식에 맞춰서 입력햇는가.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return;
            }

            if (txt_SumPr.Text.Replace(",", "") == "")
            {
                string sMsgText = (cls_User.gid_CountryCode == "TH") ? "Please select the product first." : "상품을 먼저 선택하여 주십시오.";
                MessageBox.Show(sMsgText);
                txt_ItemCode.Focus();
                return;
            }

            //int SumPr = int.Parse(txt_SumPr.Text.Replace(",", ""));
            //int InputPass_Pay = int.Parse(txt_InputPass_Pay.Text.Replace(",", ""));
            //int total = SumPr + InputPass_Pay;
            //string final_total = string.Format("{0:###,###,###,###}", total);

            float SumPr = (txt_SumPr.Text.Replace(",", "") == "") ? 0 : float.Parse(txt_SumPr.Text.Replace(",",""));
            float InputPass_Pay = (txt_InputPass_Pay.Text.Replace(",", "") == "") ? 0 : float.Parse(txt_InputPass_Pay.Text.Replace(",", ""));
            float total = SumPr + InputPass_Pay;
            string final_total = string.Format(cls_app_static_var.str_Currency_Money_Type, total);

            txt_Price_3.Text = final_total.ToString();
        }

        private void butt_All_Money_Click(object sender, EventArgs e)
        {
            //int SumPr = int.Parse(txt_SumPr.Text.Replace(",", ""));
            float SumPr = (txt_SumPr.Text.Replace(",", "") == "") ? 0 : float.Parse(txt_SumPr.Text.Replace(",", ""));

            float InputPass_Pay = (txt_InputPass_Pay.Text.Replace(",", "") == "") ? 0 : float.Parse(txt_InputPass_Pay.Text.Replace(",", ""));
            float total = SumPr + InputPass_Pay;
            //string final_total = string.Format("{0:###,###,###,###}", total);
            string final_total = string.Format(cls_app_static_var.str_Currency_Money_Type, total);

            txt_Price_1.Text = final_total.ToString();
        }

        private void butt_All_Bank_Click(object sender, EventArgs e)
        {
            int SumPr = int.Parse(txt_SumPr.Text.Replace(",", ""));
            int InputPass_Pay = int.Parse(txt_InputPass_Pay.Text.Replace(",", ""));
            int total = SumPr + InputPass_Pay;
            string final_total = string.Format("{0:###,###,###,###}", total);

            txt_Price_2.Text = final_total.ToString();
        }

        private void butt_All_Bank2_Click(object sender, EventArgs e)
        {
            int SumPr = int.Parse(txt_SumPr.Text.Replace(",", ""));
            int InputPass_Pay = int.Parse(txt_InputPass_Pay.Text.Replace(",", ""));
            int total = SumPr + InputPass_Pay;
            string final_total = string.Format("{0:###,###,###,###}", total);

            txt_Price_5_2.Text = final_total.ToString();
        }



        private void check_Cash_CheckedChanged(object sender, EventArgs e)
        {
            if (check_Cash.Checked == true)
            {
                txt_C_Cash_Send_Nu.Text = "0100001234";
            }
            if (check_Cash.Checked == false)
            {
                txt_C_Cash_Send_Nu.Text = "";
            }
        }

        private void check_AVCash_CheckedChanged(object sender, EventArgs e)
        {
            if (check_AVCash.Checked == true)
            {
                txt_AVC_Cash_Send_Nu.Text = "0100001234";
            }
            if (check_AVCash.Checked == false)
            {
                txt_AVC_Cash_Send_Nu.Text = "";
            }
        }

        private void dGridView_Base_KeyDown(object sender, KeyEventArgs e)
        {
            //구현호 20230412 그리드 복붙
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) // Ctrl + C
            {
                if (dGridView_Base.SelectedCells.Count > 0)
                {
                    dGridView_Base.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
                    Clipboard.SetDataObject(dGridView_Base.GetClipboardContent());
                    e.Handled = true;
                }
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) // Ctrl + V
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    // 붙여넣기를 처리할 로직을 작성합니다.
                    // 예를 들어, clipboardText를 파싱하여 DataGridView에 데이터를 추가하는 등의 작업을 수행할 수 있습니다.
                    e.Handled = true;
                }
            }
        }

        private void cbProvince_TH_SelectedIndexChanged(object sender, EventArgs e)
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_City_TH('" + cbProvince_TH.Text + "') ORDER BY ZIPCODE_SORT ");
            sb.AppendLine("SELECT ZIPCODE_NM FROM ufn_Get_ZipCode_District_TH('" + cbProvince_TH.Text + "') ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbDistrict_TH.DataBindings.Clear();
            cbDistrict_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbDistrict_TH.DisplayMember = "ZipCode_NM";
        }



        private void cbDistrict_TH_SelectedIndexChanged(object sender, EventArgs e)
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT * FROM dbo.ufn_Get_ZipCode_TH('" + cbDistrict_TH.Text + "') ");
            sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_SubDistrict_TH('" + cbDistrict_TH.Text + "') ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbSubDistrict_TH.DataBindings.Clear();
            cbSubDistrict_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbSubDistrict_TH.DisplayMember = "ZipCode_NM";
        }

        private void cbSubDistrict_TH_SelectedIndexChanged(object sender, EventArgs e)
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT [ZIPCODE_NM] = PostCode FROM TLS_ZIPCODE_CS WITH(NOLOCK) WHERE SubDistrictThaiShort = '" + cbSubDistrict_TH.Text + "' ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            if (Temp_conn.DataSet_ReCount <= 0) return;

            txtZipCode_TH.Text = "";
            txtZipCode_TH.Text = ds.Tables["ZipCode_NM"].Rows[0][0].ToString();

            txtAddress2.Text = cbSubDistrict_TH.Text + " " + cbDistrict_TH.Text + " " + cbProvince_TH.Text;
        }
    }
}
