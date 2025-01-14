﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MLM_Program
{
    public partial class frmSell_RC_01 : Form
    {
        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);


        public delegate void Take_NumberDele(ref string Send_Number, ref string Send_Name, ref string Send_OrderNumber);
        public event Take_NumberDele Take_Mem_Number;

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Item___02 = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece_Add = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece_Item = new cls_Grid_Base();
        
        
               
        private Dictionary<string , cls_Sell> SalesDetail ;
        private Dictionary<int, cls_Sell_Item> SalesItemDetail = new Dictionary<int, cls_Sell_Item>();
        private Dictionary<int, cls_Sell_Rece> Sales_Rece = new Dictionary<int, cls_Sell_Rece>();
       
        private Dictionary<string, TextBox>  Ncode_dic = new Dictionary<string, TextBox>();

        private const string base_db_name = "tbl_SalesDetail";
        private int Data_Set_Form_TF;
        private string idx_Mbid = "";
        private int idx_Mbid2 = 0;
        private string idx_Na_Code = "", Temp_Ordernumber = ""; 


        public frmSell_RC_01()
        {
            InitializeComponent();
        }


        private void frmBase_From_Load(object sender, EventArgs e)
        {
            Data_Set_Form_TF = 0;

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
           

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset(1);

            dGridView_Base_Item___02_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item___02.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                        
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            
            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;            
            if (cls_User.gid_CountryCode == "KR")
            {
                mtxtSn.Mask = "999999-9999999"; //기본 셋팅은 주민번호이다.  
            }
            //else
            else if (cls_User.gid_CountryCode != "TH")  // 태국인 경우는 한국과 마찬가지로 배송내역 가림. - 240304 syhuh
            {
                //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
                groupBox2.Visible = true;
                butt_Delete.Visible = true;
            }
            mtxtSn.BackColor = cls_app_static_var.txt_Enable_Color;
            txtCenter.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSellDate.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_OrderNumber.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSellCode.BackColor = cls_app_static_var.txt_Enable_Color;
            txtCenter2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalInputPrice.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_UnaccMoney.BackColor = cls_app_static_var.txt_Enable_Color;

            txt_TotalPv.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC1.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalPrice.BackColor = cls_app_static_var.txt_Enable_Color;

            txt_OrderNumber_R.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_MCnt.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumPr.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumPV.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_SumCV.BackColor = cls_app_static_var.txt_Enable_Color;




            mtxtSellDateRe.Mask = cls_app_static_var.Date_Number_Fromat;


            mtxtTel1.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtTel2.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtZip1.Mask = cls_app_static_var.ZipCode_Number_Fromat;


            dGridView_Base_Rece_Item.Dock = DockStyle.Fill;
            if (cls_app_static_var.Rec_info_Multi_TF == 1)
                pan_Rec_Item.Visible = false;

            mtxtMbid.Focus();

            if(cls_User.gid == cls_User.SuperUserID)
            {
                butt_Delete.Visible = true;
            }

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
            }
        }

        private void InitComboZipCode_TH()
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_State_TH() ORDER BY ZIPCODE_SORT");
            sb.AppendLine("SELECT * FROM ufn_Get_ZipCode_Province_TH() ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbProvince_TH.DataBindings.Clear();
            cbProvince_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbProvince_TH.DisplayMember = "ZipCode_NM";
            cbProvince_TH.ValueMember = "ProvinceCode";

            txtZipCode_TH.Clear();
            cbDistrict_TH.SelectedIndex = -1;
            cbProvince_TH.SelectedIndex = -1;
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            //butt_Delete.Visible = true;
            //butt_Exit.Left = this.Width - butt_Exit.Width - 20;

            //butt_Clear.Left = 3;
            //butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            //butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            //butt_Delete.Left = butt_Save.Left + butt_Save.Width + 2;
            ////this.Refresh();

            //int base_w = this.Width / 4;
            //butt_Clear.Width = base_w;
            //butt_Save.Width = base_w;

            //butt_Delete.Width = base_w;
            //butt_Exit.Width = base_w;

            //butt_Clear.Left = 0;
            //butt_Save.Left = butt_Clear.Left + butt_Clear.Width;

            //butt_Delete.Left = butt_Save.Left + butt_Save.Width;
            //butt_Exit.Left = butt_Delete.Left + butt_Delete.Width;    


            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            //butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            butt_Delete.Left = butt_Save.Left + butt_Save.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);

            cfm.button_flat_change(butt_Item_Del);
            cfm.button_flat_change(butt_Item_Save);
            cfm.button_flat_change(butt_Item_Clear);
            cfm.button_flat_change(butt_Ord_Clear);

            cfm.button_flat_change(butt_Rec_Del);
            cfm.button_flat_change(butt_Rec_Save);
            cfm.button_flat_change(butt_Rec_Clear);
            cfm.button_flat_change(butt_Rec_Add);
            cfm.button_flat_change(butt_AddCode);


            




        }

        private void frm_Base_Activated(object sender, EventArgs e)
        {
        //   //19-03-11 깜빡임제거 this.Refresh();
            string Send_Number = ""; string Send_Name = ""; string Send_OrderNumber = "";
            Take_Mem_Number(ref Send_Number, ref Send_Name, ref Send_OrderNumber);


            if (Send_Number != "")
            {
                mtxtMbid.Text = Send_Number;
                Set_Form_Date(mtxtMbid.Text, "m");
            }
        }


        private void frmBase_From_KeyDown(object sender, KeyEventArgs e)
        {
            //폼일 경우에는 ESC버튼에 폼이 종료 되도록 한다
            if (sender is Form)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (!this.Controls.ContainsKey("Popup_gr") && dGridView_Base_Rece_Add.Visible == false )
                        this.Close();
                    else
                    {
                        if (dGridView_Base_Rece_Add.Visible == true)
                        {
                            dGridView_Base_Rece_Add.Visible = false;

                            cls_form_Meth cfm = new cls_form_Meth();
                            cfm.form_Group_Panel_Enable_True(this);
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
            if (e.KeyValue == 119)
                T_bt = butt_Excel;    //엑셀  F8    
            if (e.KeyValue == 112)
                T_bt = butt_Clear;    //엑셀  F5    

            if (T_bt.Visible == true)
            {
                EventArgs ee1 = null;
                if (e.KeyValue == 123 || e.KeyValue == 113 || e.KeyValue == 119 || e.KeyValue == 112)
                    Base_Button_Click(T_bt, ee1);
            }
        }



        private void txtData_Enter(object sender, EventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            if (sender is TextBox)
            {
                T_R.Text_Focus_All_Sel((TextBox)sender);
                TextBox tb = null;
                tb = (TextBox)sender;
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
            }

            if (sender is MaskedTextBox)
            {
                MaskedTextBox tb = (MaskedTextBox)sender;
                if (tb.ReadOnly == false)
                    tb.BackColor = Color.White;
            }
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
                            if (Input_Error_Check(mtb, "m") == true)
                                Set_Form_Date(mtb.Text, "m");
                            //SendKeys.Send("{TAB}");
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
            if (Input_Error_Check(mtxtMbid, "m") == true)
                Set_Form_Date(mtxtMbid.Text, "m");
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


            //마스크텍스트 박스에 입력한 내용이 있으면 그곳 다음으로 커서가 가게 한다.
            if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                mtb.SelectionStart = mtb.Text.Replace("-", "").Replace("_", "").Trim().Length + 1;
        }











        private void Set_Form_Date(string T_Mbid, string T_sort )
        {
            _From_Data_Clear();   
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

                Tsql = Tsql + ", tbl_Memberinfo.Cpno";
                Tsql = Tsql + ", tbl_Memberinfo.Na_Code ";

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
                Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";


                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text ) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                //++++++++++++++++++++++++++++++++
                Set_Form_Date(ds); //위의 DataSet객체를 가져가서 회원 정보를 넣는다

                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                if (SalesDetail != null)
                    SalesDetail.Clear();

                Set_SalesDetail();  //회원의 구매 관련 주테이블 내역을 클래스에 넣는다.

                if (SalesDetail != null)
                    Base_Grid_Set();

               

                mtxtMbid.Focus();                
            }
            
            Data_Set_Form_TF = 0;            
        }

        private void Set_Form_Date(DataSet ds)
        {
            idx_Mbid =  ds.Tables[base_db_name].Rows[0]["Mbid"].ToString();
            idx_Mbid2 = int.Parse(ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString());
            idx_Na_Code = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();
            
            mtxtMbid.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
            txtName.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
            mtxtSn.Text = encrypter.Decrypt ( ds.Tables[base_db_name].Rows[0]["Cpno"].ToString() ,"Cpno");
                  
            txtCenter.Text = ds.Tables[base_db_name].Rows[0]["B_Name"].ToString();
            txtCenter_Code.Text = ds.Tables[base_db_name].Rows[0]["businesscode"].ToString();

            txtName.ReadOnly = true;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.BackColor = cls_app_static_var.txt_Enable_Color;


            mtxtMbid.ReadOnly = true;
            mtxtMbid.BorderStyle = BorderStyle.FixedSingle;
            mtxtMbid.BackColor = cls_app_static_var.txt_Enable_Color;
        }


        private void  Set_SalesDetail ()
        {
            cls_form_Meth cm = new cls_form_Meth();
   
            string strSql = "";

            strSql = "Select tbl_SalesDetail.* ";
            strSql = strSql + " , tbl_Business.Name BusCodeName ";
            strSql = strSql + " , tbl_SellType.SellTypeName SellCodeName  ";

            strSql = strSql + " ,Case When ReturnTF = 1 Then '" + cm._chang_base_caption_search("정상") + "'";
            strSql = strSql + "  When ReturnTF = 2 Then '" + cm._chang_base_caption_search("반품") + "'";
            strSql = strSql + "  When ReturnTF = 4 Then '" + cm._chang_base_caption_search("교환") + "'";
            strSql = strSql + "  When ReturnTF = 3 Then '" + cm._chang_base_caption_search("부분반품") + "'";
            strSql = strSql + "  When ReturnTF = 5 Then '" + cm._chang_base_caption_search("취소") + "'";
            strSql = strSql + " END ReturnTFName ";


            strSql = strSql + " , Ga_Order SellTF ";
            strSql = strSql + " ,Case When Ga_Order >= 1 Then '" + cm._chang_base_caption_search("미승인") + "'";
            strSql = strSql + "  When Ga_Order = 0 Then '" + cm._chang_base_caption_search("승인") + "'";
            strSql = strSql + " ELSE '' ";
            strSql = strSql + " END SellTFName ";
            strSql = strSql + ", tbl_SalesDetail.Na_code ";

            if (cls_app_static_var.Sell_Union_Flag == "U")  //특판
            {
                strSql = strSql + " , Case When  tbl_SalesDetail.union_Seq > 0 And T_REALMLM.ERRCODE = '0000' Then ISNULL(T_REALMLM.GUARANTE_NUM,'') ";
                strSql = strSql + "        When  tbl_SalesDetail.union_Seq > 0 And T_REALMLM.ERRCODE <> '0000' Then  ISNULL(T_REALMLM_ErrCode.Er_Msg ,'' ) ";
                strSql = strSql + "        When  tbl_SalesDetail.union_Seq = 0 Then '미신고'  ";
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

                strSql = strSql + " , Case When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber  ";
                strSql = strSql + "  When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)'  ";
                strSql = strSql + "  When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)'  ";
                strSql = strSql + "  When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 2 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)'  ";
                strSql = strSql + "  When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 3 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(부분취소)'  ";
                strSql = strSql + "  When  ReturnTF = 2 then '반품처리'  ";
                strSql = strSql + "  When  ReturnTF = 3 then '부분반품처리'  ";
                strSql = strSql + "  When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '발급실패.재발급요청' + ' ' + tbl_SalesDetail.INS_Num_Err   ";
                strSql = strSql + "  ELSE tbl_SalesDetail.InsuranceNumber END InsuranceNumber2 ";
            }
            else
            {
                strSql = strSql + " , InsuranceNumber As InsuranceNumber2 ";
            }

            strSql = strSql + " , (SELECT TOP 1 ISNULL(CASE WHEN R1.Receive_Method = 1 THEN '직접수령' WHEN R1.Receive_Method = 2 THEN '배송' ELSE '' END, '') FROM tbl_Sales_Rece (NOLOCK) AS R1 WHERE tbl_SalesDetail.OrderNumber = R1.OrderNumber AND R1.RecIndex > 0) AS ReceiveName "; 

            strSql = strSql + " From tbl_SalesDetail (nolock) ";
            //strSql = strSql + " LEFT JOIN tbl_SalesDetail_TF (nolock) ON tbl_SalesDetail.OrderNumber = tbl_SalesDetail_TF.OrderNumber ";
            strSql = strSql + " LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
            strSql = strSql + " LEFT Join tbl_SellType ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode ";
            strSql = strSql + " LEFT JOIN tbl_Business (nolock) ON tbl_SalesDetail.BusCode = tbl_Business.NCode And tbl_SalesDetail.Na_code = tbl_Business.Na_code ";

            strSql = strSql + " LEFT JOIN T_REALMLM (nolock) ON T_REALMLM.SEQ = tbl_SalesDetail.union_Seq ";
            strSql = strSql + " LEFT JOIN T_REALMLM_ErrCode (nolock) ON T_REALMLM.ERRCODE = T_REALMLM_ErrCode.Er_Code ";

            if (idx_Mbid.Length == 0)
                strSql = strSql + " Where tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
            else
            {
                strSql = strSql + " Where tbl_Memberinfo.Mbid = '" + idx_Mbid + "' ";
                strSql = strSql + " And   tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
            }

            //// strSql = strSql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            strSql = strSql + " And (Ga_Order = 0 ) "; //정상내역은 승인 내역만 보여준다.
            strSql = strSql + " And tbl_SalesDetail.InputCoupon = 0 "; //쿠폰은 부분반품 불가.
            strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            strSql = strSql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";
            strSql = strSql + "  Order By  CAST(tbl_SalesDetail.RecordTime AS DATETIME) DESC";
            
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
                t_c_sell.Mbid2 = int.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["Mbid2"].ToString());
                t_c_sell.M_Name = ds.Tables[base_db_name].Rows[fi_cnt]["M_Name"].ToString();
                t_c_sell.SellDate = ds.Tables[base_db_name].Rows[fi_cnt]["SellDate"].ToString();
                t_c_sell.SellDate_2 = ds.Tables[base_db_name].Rows[fi_cnt]["SellDate_2"].ToString();
                t_c_sell.SellCode = ds.Tables[base_db_name].Rows[fi_cnt]["SellCode"].ToString();
                t_c_sell.SellCodeName = ds.Tables[base_db_name].Rows[fi_cnt]["SellCodeName"].ToString();
                t_c_sell.BusCode = ds.Tables[base_db_name].Rows[fi_cnt]["BusCode"].ToString();
                t_c_sell.BusCodeName = ds.Tables[base_db_name].Rows[fi_cnt]["BusCodeName"].ToString();
                t_c_sell.Re_BaseOrderNumber = ds.Tables[base_db_name].Rows[fi_cnt]["Re_BaseOrderNumber"].ToString();
                t_c_sell.TotalPrice = double.Parse ( ds.Tables[base_db_name].Rows[fi_cnt]["TotalPrice"].ToString());
                t_c_sell.TotalPV = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["TotalPV"].ToString());
                t_c_sell.TotalCV = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["TotalCV"].ToString());
                t_c_sell.TotalInputPrice = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["TotalInputPrice"].ToString());
                t_c_sell.Total_Sell_VAT_Price = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["Total_Sell_VAT_Price"].ToString());
                t_c_sell.Total_Sell_Except_VAT_Price = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["Total_Sell_Except_VAT_Price"].ToString());
                t_c_sell.InputCash = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["InputCash"].ToString());
                t_c_sell.InputCard = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["InputCard"].ToString());
                t_c_sell.InputCoupon = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputCoupon"].ToString());
                t_c_sell.InputPassbook = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["InputPassbook"].ToString());
                t_c_sell.InputMile = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["InputMile"].ToString());
                t_c_sell.InputPass_Pay = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["InputPass_Pay"].ToString());
                t_c_sell.UnaccMoney = double.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["UnaccMoney"].ToString());

                t_c_sell.Etc1 = ds.Tables[base_db_name].Rows[fi_cnt]["Etc1"].ToString();
                t_c_sell.Etc2 = ds.Tables[base_db_name].Rows[fi_cnt]["Etc2"].ToString();

                t_c_sell.ReturnTF = int.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["ReturnTF"].ToString());
                t_c_sell.ReturnTFName = ds.Tables[base_db_name].Rows[fi_cnt]["ReturnTFName"].ToString();

                t_c_sell.INS_Num = ds.Tables[base_db_name].Rows[fi_cnt]["InsuranceNumber2"].ToString();
                t_c_sell.InsuranceNumber_Date = ds.Tables[base_db_name].Rows[fi_cnt]["InsuranceNumber_Date"].ToString();
                t_c_sell.W_T_TF = int.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["W_T_TF"].ToString());
                t_c_sell.In_Cnt = int.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["In_Cnt"].ToString());

                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();

                t_c_sell.SellTF = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SellTF"].ToString());
                t_c_sell.SellTFName = ds.Tables[base_db_name].Rows[fi_cnt]["SellTFName"].ToString();
                t_c_sell.Na_Code = ds.Tables[base_db_name].Rows[fi_cnt]["Na_Code"].ToString();


                //t_c_sell.Us_Ord = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Us_Ord"].ToString());

                //t_c_sell.ReceiveName = ds.Tables[base_db_name].Rows[fi_cnt]["ReceiveName"].ToString();

                string  t_sellDate = t_c_sell.SellDate.Substring(0,4) ;
                t_sellDate =t_sellDate + "-" + t_c_sell.SellDate.Substring(4,2) ;
                t_sellDate =t_sellDate + "-" +  t_c_sell.SellDate.Substring(6,2) ;
                t_c_sell.SellDate = t_sellDate; 

                t_c_sell.Del_TF = "" ;

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

            int fi_cnt = 0;
            double S_cnt4 = 0; double S_cnt5 = 0; double S_cnt6 = 0, S_cnt6_1 = 0 ;
            double Sum_13 = 0; double Sum_14 = 0; double Sum_15 = 0; ; double Sum_16 = 0;
            foreach (string t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D" && SalesDetail[t_key].ReturnTF  != 2 )
                {
                    Set_gr_dic(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.

                    S_cnt4 = S_cnt4 + SalesDetail[t_key].TotalPrice;
                    S_cnt5 = S_cnt5 + SalesDetail[t_key].TotalInputPrice;
                    S_cnt6 = S_cnt6 + SalesDetail[t_key].TotalPV;
                    S_cnt6_1 = S_cnt6_1 + SalesDetail[t_key].TotalCV;

                    Sum_13 = Sum_13 + SalesDetail[t_key].InputCash;
                    Sum_14 = Sum_14 + SalesDetail[t_key].InputCard;
                    Sum_15 = Sum_15 + SalesDetail[t_key].InputPassbook;
                    //Sum_15 = Sum_15 + SalesDetail[t_key].InputCoupon;


                    Sum_16 = Sum_16 + SalesDetail[t_key].UnaccMoney;
                }

                fi_cnt++;
            }

            cls_form_Meth cm = new cls_form_Meth();

            object[] row0 = { ""
                                ,"<< " + cm._chang_base_caption_search("합계") + " >>"
                                ,""
                                ,""
                                ,S_cnt4

                                ,S_cnt5                                
                                ,S_cnt6
                                ,S_cnt6_1
                                ,""
                                ,""

                                ,Sum_13                             
                                ,Sum_14
                                ,Sum_15
                                ,Sum_16
                                ,""

                                ,""
                            };

            gr_dic_text[fi_cnt + 2] = row0;


            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
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
                                ,SalesDetail[t_key].InputCoupon
                                ,SalesDetail[t_key].UnaccMoney 
                                ,SalesDetail[t_key].RecordID 

                                ,SalesDetail[t_key].RecordTime 

                                //,SalesDetail[t_key].ReceiveName
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Header_Reset()
        {
            cgb.Grid_Base_Arr_Clear();
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_col_Count = 17;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"승인여부" , "공제번호"  , "주문번호"   , "구매일자"  , "총구매액"       
                                    , "총입금액"      , "총PV"  , "총CV"  , "구매종류"   , "구분"      
                                    , "현금"   ,"카드" ,"무통장", "쿠폰", "미결제"  
                                    ,  "기록자" ,  "기록일"
                                };

   


            if (cls_app_static_var.Sell_Union_Flag == "")  //직판특판이 아닌경우 공제번호 필드 안나오게
            {
                int[] g_Width = { 80,0, 120, 80, 80
                              , 80  ,80 , 0 , 80 , 80
                              , 80  ,80 ,80 , 80 , 80
                              , 80  ,80
                            };
                cgb.grid_col_w = g_Width;
            }
            else
            {

                int[] g_Width = { 80,cls_User.gid_CountryCode == "JP" ? 0 : 120, 120, 80, 80
                              , 80  ,80 , 0 , 80 , 80
                              , 80  ,80 ,80,80 ,80
                              ,80 , 80
                            };
                cgb.grid_col_w = g_Width;
            }

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft 
                                ,DataGridViewContentAlignment.MiddleLeft 
                                ,DataGridViewContentAlignment.MiddleCenter  
                                ,DataGridViewContentAlignment.MiddleRight//5    

                                ,DataGridViewContentAlignment.MiddleRight    
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleCenter                                  
                                ,DataGridViewContentAlignment.MiddleCenter  //10

                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight   
                                
                                 ,DataGridViewContentAlignment.MiddleCenter 
                                 ,DataGridViewContentAlignment.MiddleLeft
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

            cgb.grid_col_header_text = g_HeaderText;
            cgb.grid_cell_format = gr_dic_cell_format;
            
            cgb.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true 
                                    ,true , true,  true, true,  true      
                                    ,true , true   
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            cgb.basegrid.RowHeadersVisible = false;
        }
        //////SalesDetail___SalesDetail__SalesDetail__SalesDetail__SalesDetail__SalesDetail
        //////SalesDetail___SalesDetail__SalesDetail__SalesDetail__SalesDetail__SalesDetail







        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
     

        private void Item_Grid_Set()
        {
            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            foreach (int  t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                    if (SalesItemDetail[t_key].OrderNumber != "") 
                        Set_gr_Item(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                fi_cnt++;
            }

            cgb_Item.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Item.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Item(ref Dictionary<int, object[]> gr_dic_text, int  t_key, int fi_cnt)
        {
            object[] row0 = { SalesItemDetail[t_key].SalesItemIndex   
                                ,SalesItemDetail[t_key].ItemCode  
                                ,SalesItemDetail[t_key].ItemName   
                                ,SalesItemDetail[t_key].ItemPrice   
                                ,SalesItemDetail[t_key].ItemPV    

                                ,SalesItemDetail[t_key].ItemCV    
                                ,SalesItemDetail[t_key].ItemCount   
                                ,SalesItemDetail[t_key].ItemTotalPrice 
                                ,SalesItemDetail[t_key].ItemTotalPV                                 
                                ,SalesItemDetail[t_key].ItemTotalCV                                 

                                ,SalesItemDetail[t_key].SellStateName 
                                ,SalesItemDetail[t_key].Etc  
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Item_Header_Reset()
        {
            cgb_Item.Grid_Base_Arr_Clear();
            cgb_Item.basegrid = dGridView_Base_Item;
            cgb_Item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Item.grid_col_Count = 12;
            cgb_Item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "상품_코드"   , "상품명"  , "개별단가"   , "개별PV"        
                                , "개별CV"    , "구매_수량"   , "총상품액"    , "총상품PV" ,  "총상품CV" 
                                , "구분" , "비고"
                                };

            int[] g_Width = { 0, 90, 160, 80, 70
                                ,0 , 80 , 80 , 70 ,0
                                ,70, 200
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter 
                                ,DataGridViewContentAlignment.MiddleLeft  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight  //5    
  
                                ,DataGridViewContentAlignment.MiddleRight        
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight //10

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
            cgb_Item.grid_cell_format = gr_dic_cell_format;
            cgb_Item.grid_col_w = g_Width;
            cgb_Item.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true    
                                    ,  true ,true                    
                                   };
            cgb_Item.grid_col_Lock = g_ReadOnly;

            cgb_Item.basegrid.RowHeadersVisible = false;
        }
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail



        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail
        //////SalesItemDetail___SalesItemDetail__SalesItemDetail__SalesItemDetail     

        private void Item___02_Grid_Set()
        {
            dGridView_Base_Item___02_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item___02.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            int S_cnt6 = 0; double S_cnt7 = 0; double S_cnt8 = 0; double S_cnt9 = 0;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    if (SalesItemDetail[t_key].OrderNumber == "")
                    {
                        Set_gr_Item___02(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.

                        S_cnt6 = S_cnt6 + SalesItemDetail[t_key].ItemCount;
                        S_cnt7 = S_cnt7 + SalesItemDetail[t_key].ItemTotalPrice;
                        S_cnt8 = S_cnt8 + SalesItemDetail[t_key].ItemTotalPV;
                        S_cnt9 = S_cnt9 + SalesItemDetail[t_key].ItemTotalCV;
                    }
                }
                fi_cnt++;
            }

            txt_SumCnt.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt6);
            txt_SumPr.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt7);
            txt_SumPV.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt8);
            txt_SumCV.Text = string.Format(cls_app_static_var.str_Currency_Type, S_cnt9);

            cgb_Item___02.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Item___02.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Item___02(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt)
        {
            object[] row0 = { SalesItemDetail[t_key].SalesItemIndex   
                                ,SalesItemDetail[t_key].ItemCode  
                                ,SalesItemDetail[t_key].ItemName   
                                ,SalesItemDetail[t_key].ItemPrice   
                                ,SalesItemDetail[t_key].ItemPV    
                                ,SalesItemDetail[t_key].ItemCV    

                                ,SalesItemDetail[t_key].ItemCount   
                                ,SalesItemDetail[t_key].ItemTotalPrice 
                                ,SalesItemDetail[t_key].ItemTotalPV                                 
                                ,SalesItemDetail[t_key].ItemTotalCV                                 
                                ,SalesItemDetail[t_key].SellStateName 
                                ,SalesItemDetail[t_key].Etc  
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Item___02_Header_Reset()
        {
            cgb_Item___02.Grid_Base_Arr_Clear();
            cgb_Item___02.basegrid = dGridView_Base_Item___02;
            cgb_Item___02.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Item___02.grid_col_Count = 12;
            cgb_Item___02.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "상품_코드"   , "상품명"  , "개별단가"   , "개별PV"        
                                 , "개별CV"  , "구매_수량"   , "총상품액"    , "총상품PV"  , "총상품CV"
                                 , "구분" , "비고"
                                };

            int[] g_Width = { 0, 90, 160, 80, 70
                                ,0 , 80 , 80 , 70 ,0
                                ,70, 200
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter 
                                ,DataGridViewContentAlignment.MiddleLeft  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight  //5    
  
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight

                                ,DataGridViewContentAlignment.MiddleCenter 
                                ,DataGridViewContentAlignment.MiddleLeft  //10
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;


            cgb_Item___02.grid_col_header_text = g_HeaderText;
            cgb_Item___02.grid_cell_format = gr_dic_cell_format;
            cgb_Item___02.grid_col_w = g_Width;
            cgb_Item___02.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true        
                                    ,  true ,true                  
                                   };
            cgb_Item___02.grid_col_Lock = g_ReadOnly;

            cgb_Item___02.basegrid.RowHeadersVisible = false;
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
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
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
            SendKeys.Send("{TAB}");
        }


        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            int Sw_Tab = 0;

            if ((sender is TextBox) == false) return;

            TextBox tb = (TextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
                Sw_Tab = 1;
            }

            if (tb.Name == "txtCenter2")
            {
                if (tb.Text.Trim() == "")
                    txtCenter2_Code.Text = "";
                else if (Sw_Tab == 1)
                    Ncod_Text_Set_Data(tb, txtCenter2_Code);
            }

            if (tb.Name == "txtSellCode")
            {
                if (tb.Text.Trim() == "")
                    txtSellCode_Code.Text = "";
                else if (Sw_Tab == 1)
                    Ncod_Text_Set_Data(tb, txtSellCode_Code);
            }
            //2020-05-12 교환막음
            //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
            if (tb.Name == "txt_ItemCode" && cls_User.gid_CountryCode != "KR")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txt_ItemName.Text = "";
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_Receive_Method")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                {
                    txt_Receive_Method_Code.Text = "";
                    dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                    cgb_Rece_Item.d_Grid_view_Header_Reset();
                }
                else
                {
                    if (SalesItemDetail != null && txt_Receive_Method_Code.Text != "")
                        Rece_Item_Grid_Set();
                }
                Data_Set_Form_TF = 0;
            }

        }

        void T_R_Key_Enter_13_Name(string txt_tag, TextBox tb)
        {
            if (txt_tag != "")
            {
                int reCnt = 0;
                cls_Search_DB cds = new cls_Search_DB();
                string Search_Mbid = "";
                reCnt = cds.Member_Name_Search(ref Search_Mbid, txt_tag);

                if (reCnt == 1)
                {
                    if (tb.Name == "txtName")
                    {
                        mtxtMbid.Text = Search_Mbid; //회원명으로 검색해서 나온 사람이 한명일 경우에는 회원번호를 넣어준다.                    
                        if (Input_Error_Check(mtxtMbid, "m") == true)
                            Set_Form_Date(mtxtMbid.Text, "m");
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

            //2020-05-12 교환막음
            //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
            if (tb.Name == "txt_ItemCode" && cls_User.gid_CountryCode != "KR")
            {
                Data_Set_Form_TF = 1;

                Db_Grid_Popup(tb, txt_ItemName);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txt_ItemName, "");
                //else
                //    Ncod_Text_Set_Data(tb, txt_ItemName);

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


            //if (tb.Name == "txt_C_Bank")
            //{
            //    if (Ncode_dic != null)
            //        Ncode_dic.Clear();
            //    Ncode_dic["BankPenName"] = tb;
            //    Ncode_dic["BankCode"] = txt_C_Bank_Code;
            //    Ncode_dic["BankName"] = txt_C_Bank_Code_2;
            //    Ncode_dic["BankAccountNumber"] = txt_C_Bank_Code_3;

            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb,  "");
            //    else
            //        Ncod_Text_Set_Data(tb);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}
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

            if (tb.Name == "txtCenter")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txtCenter2")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txtBank")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txtR_Id")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txtChange")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txtSellCode")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txt_Base_Rec")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txt_Receive_Method")
                cgb_Pop.Next_Focus_Control = null;

            if (tb.Name == "txt_ItemCode")
                cgb_Pop.Next_Focus_Control = txt_ItemCount;

            //cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, cls_User.gid_CountryCode);
            cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, idx_Na_Code, mtxtSellDateRe.Text);

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
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtR_Id")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txtSellCode")
                    cgb_Pop.db_grid_Popup_Base(2, "구매_코드", "구매종류", "SellCode", "SellTypeName", strSql);


                if (tb.Name == "txt_Base_Rec")
                    cgb_Pop.db_grid_Popup_Base(2, "배송사_코드", "배송사", "ncode", "name", strSql);


                if (tb.Name == "txt_Receive_Method")
                    cgb_Pop.db_grid_Popup_Base(2, "배송_코드", "배송_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, strSql);

                if (tb.Name == "txt_C_TF")
                    cgb_Pop.db_grid_Popup_Base(2, "결제_코드", "결제_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, strSql);

                //2020-05-12 교환막음
                //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
                if (tb.Name == "txt_ItemCode" && cls_User.gid_CountryCode != "KR")
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
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
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

                    cgb_Pop.db_grid_Popup_Base(2, "구매_코드", "구매종류", "SellCode", "SellTypeName", Tsql);
                }


                if (tb.Name == "txt_Base_Rec")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Base_Rec (nolock) ";                    
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

                    cgb_Pop.db_grid_Popup_Base(2, "배송_코드", "배송_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, Tsql);
                }



                //2020-05-12 교환막음
                //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
                if (tb.Name == "txt_ItemCode" && cls_User.gid_CountryCode != "KR")
                {
                    string Tsql;
                    Tsql = "Select Name , NCode  ,price2 , price4  ";
                    Tsql = Tsql + " From ufn_Good_Search_Web ('" + mtxtSellDateRe.Text.Replace("-", "").Trim() + "','" + idx_Na_Code + "') ";
                    Tsql = Tsql + " Where NCode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";

                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV", "Name", "Ncode", "price2", "price4", Tsql);
                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                }


            }
        }


        private void Db_Grid_Popup(TextBox tb,  string strSql)
        {
    
            cls_Grid_Base_Popup cgb_Pop = new cls_Grid_Base_Popup();
            DataGridView Popup_gr = new DataGridView();
            Popup_gr.Name = "Popup_gr";
            this.Controls.Add(Popup_gr);
            cgb_Pop.basegrid = Popup_gr;
            cgb_Pop.Base_fr = this;
            cgb_Pop.Base_text_dic = Ncode_dic;            
            cgb_Pop.Base_Location_obj = tb;

            if (strSql != "")
            {
                if (tb.Name == "txt_C_Bank")
                    cgb_Pop.db_grid_Popup_Base(4, "계좌가명", "은행_코드", "은행명", "계좌번호"
                                                , "BankPenName", "BankCode", "BankName", "BankAccountNumber"
                                                , strSql);

                if (tb.Name == "txt_C_Card")
                    cgb_Pop.db_grid_Popup_Base(2, "카드_코드", "카드명"
                                                , "ncode", "CardName"
                                                , strSql);
                
            }
            else
            {
                if (tb.Name == "txt_C_Bank")
                {
                    string Tsql;
                    Tsql = "Select BankPenName , BankCode , BankName , BankAccountNumber        ";
                    Tsql = Tsql + " From tbl_BankForCompany ";
                    Tsql = Tsql + " Where BankPenName like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    BankCode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    BankName like '%" + tb.Text.Trim() + "%'";

                    cgb_Pop.db_grid_Popup_Base(4, "계좌가명", "은행_코드", "은행명", "계좌번호"
                                                , "BankPenName", "BankCode", "BankName", "BankAccountNumber"
                                                , Tsql);

                }


                if (tb.Name == "txt_C_Card")
                {
                    string Tsql;
                    Tsql = "Select  Ncode, cardname   ";
                    Tsql = Tsql + " From tbl_Card (nolock) ";
                    Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    cardname like '%" + tb.Text.Trim() + "%')";

                    cgb_Pop.db_grid_Popup_Base(2, "카드_코드", "카드명"
                                                , "ncode", "CardName"
                                                , Tsql);

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
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
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
                Tsql = "Select M_Detail , " + cls_app_static_var.Base_M_Detail_Ex ;
                Tsql = Tsql + " From tbl_Base_Change_Detail (nolock) ";
                Tsql = Tsql + " Where M_Detail_S = 'tbl_Sales_Rece' ";
                Tsql = Tsql + " And  ( M_Detail like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    " + cls_app_static_var.Base_M_Detail_Ex + " like '%" + tb.Text.Trim() + "%')";
            }

            //2020-05-12 교환막음
            //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
            if (tb.Name == "txt_ItemCode" && cls_User.gid_CountryCode != "KR")
            {
                Tsql = "Select Name , NCode ,price2 ,price4    ";
                Tsql = Tsql + " From ufn_Good_Search_Web ('" + mtxtSellDateRe.Text.Replace("-", "").Trim() + "','" + idx_Na_Code + "') ";
                Tsql = Tsql + " Where NCode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";
            }

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 1)
            {
                tb.Text = ds.Tables["t_P_table"].Rows[0][1].ToString();
                tb1_Code.Text = ds.Tables["t_P_table"].Rows[0][0].ToString();
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
                Tsql = Tsql + " From tbl_BankForCompany ";
                Tsql = Tsql + " Where BankPenName like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    BankCode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    BankName like '%" + tb.Text.Trim() + "%'";
            }


            if (tb.Name == "txt_C_Card")
            {
                Tsql = "Select  Ncode, cardname   ";
                Tsql = Tsql + " From tbl_Card (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    cardname like '%" + tb.Text.Trim() + "%')";
            }


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 1)
            {
                int fCnt = 0; 
                foreach (string  t_key in Ncode_dic.Keys)
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
            Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

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
            //++++++++++++++++++++++++++++++++            

            return true;
        }




















        private void _From_Data_Clear()
        {
            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();   
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            if (SalesDetail != null)
                SalesDetail.Clear();

            if (SalesItemDetail != null)
                SalesItemDetail.Clear();


            txtName.ReadOnly =false ;
            txtName.BackColor = SystemColors.Window;



            mtxtMbid.ReadOnly = false;
            mtxtMbid.BackColor = SystemColors.Window;
            mtxtMbid.BorderStyle = BorderStyle.Fixed3D;

            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, mtxtMbid);

            Base_Ord_Clear();

           if (cls_User.gid_CountryCode == "KR") mtxtSn.Mask = "999999-9999999";
            idx_Mbid = ""; idx_Mbid2 = 0; idx_Na_Code = "";
            mtxtMbid.Focus();
        }


        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            
            if (bt.Name == "butt_Clear")
            {                
                _From_Data_Clear();                                
            }

            else if (bt.Name == "butt_Save")
            {
                Temp_Ordernumber = "";
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {
                    Base_Ord_Clear();

                    if (SalesDetail != null)
                        SalesDetail.Clear();

                    if (SalesItemDetail != null)
                        SalesItemDetail.Clear();

                    Set_SalesDetail();  //회원의 구매 관련 주테이블 내역을 클래스에 넣는다.                    

                    if (SalesDetail != null)
                        Base_Grid_Set();
                }

                Temp_Ordernumber = "";
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

            else if (bt.Name == "butt_Delete")
            {
                int Delete_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Delete_Base_Data(ref Delete_Error_Check);

                if (Delete_Error_Check > 0)
                {
                    Base_Ord_Clear();

                    if (SalesDetail != null)
                        SalesDetail.Clear();

                    Set_SalesDetail();  //회원의 구매 관련 주테이블 내역을 클래스에 넣는다.

                    if (SalesDetail != null)
                        Base_Grid_Set();
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }


            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }

            else if (bt.Name == "butt_AddCode")
            {
                //if (cls_User.gid_CountryCode == "JP")
                //{
                //    frmBase_AddCode_JP e_f = new frmBase_AddCode_JP();
                //    e_f.Send_Address_Info += new frmBase_AddCode_JP.SendAddressDele(e_f_Send_Address_Info);
                //    e_f.ShowDialog();
                //}

                //else
                {
                    frmBase_AddCode e_f = new frmBase_AddCode();
                    e_f.Send_Address_Info += new frmBase_AddCode.SendAddressDele(e_f_Send_Address_Info);
                    e_f.ShowDialog();
                    txtAddress2.Focus();
                }
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





        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Delete_Base_Data(ref int Delete_Error_Check)
        {
            Delete_Error_Check = 0;

            //구매종류 , 회원, 구매일자 입력 안햇는지 체크
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
            //구매종류 , 회원, 구매일자 입력 안햇는지 체크
            //            if (Base_Error_Check__01() == false) return false;

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
                            txtSellDate.Focus(); return false;
                        }
                        if (SalesDetail[t_key].ReturnTF == 3)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_3")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtSellDate.Focus(); return false;
                        }

                        if (SalesDetail[t_key].ReturnTF == 4)
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_4")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            txtSellDate.Focus(); return false;
                        }
                        
                    }
                }
            }


            if (SalesDetail[Ord_N].ReturnTF.ToString() == "1")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_01")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellDate.Focus(); return false;
            }

            if (SalesDetail[Ord_N].ReturnTF.ToString() == "2")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_3")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellDate.Focus(); return false;
            }
            

            cls_Search_DB csd = new cls_Search_DB();

            //마감정산이 이루어진 판매 날짜인지 체크한다.                
            if (csd.Close_Check_SellDate("tbl_CloseTotal_02", SalesDetail[Ord_N].SellDate_2.Replace("-", "")) == false)
            {
                txtSellDate2.Focus(); return false;
            }

            //재고 관련해서 출고가 된내역인지 확인한다 출고 되었으면 삭제 되면 안됨.
            if (csd.Check_Stock_INPut(Ord_N) == false)
            {
                butt_Delete.Focus(); return false;
            }

            //재고 관련해서 출고가 된내역인지 확인한다 출고 되었으면 삭제 되면 안됨.
            if (csd.Check_Stock_OutPut (Ord_N) == false)
            {
                butt_Delete.Focus(); return false;
            }


            return true;
        }







        private bool Base_Error_Check__01()
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


            //구매일자를 넣었는지 먼저 체크한다. 안넣었으면 넣어라.
            if (mtxtSellDateRe.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDateRe.Text, mtxtSellDateRe, "Date") == false)
                {
                    mtxtSellDateRe.Focus();
                    return false;
                }
                
            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate_Re")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }


            //구매종류를 선택 안햇네 그럼 그것도 넣어라.
            if (txtSellCode_Code.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellCode")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellCode.Focus(); return false;
            }
            
            return true; 
        }


        private bool Item_Rece_Error_Check__01()
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


            //구매수량을 입력 안햇네 그럼 그것도 넣어라.
            if (txt_ItemCount.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Count")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txt_ItemCount.Focus(); return false;
            }


            //구매수량을 0  입력햇네  그럼 제대로 넣어라.
            if (int.Parse(txt_ItemCount.Text.Trim()) == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Count")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txt_ItemCount.Focus(); return false;
            }

            if (txt_SalesItemIndex.Text != "")
            {
                if (int.Parse(txt_MCnt.Text) < int.Parse(txt_ItemCount.Text.Trim()))
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re_004")                   
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txt_ItemCount.Focus(); return false;
                }
            }


            return true;
        }




        private void Base_Ord_Clear()
        {
            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset();

            
            dGridView_Base_Item___02_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item___02.d_Grid_view_Header_Reset();

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset();

            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset();


            SalesItemDetail.Clear();
            Sales_Rece.Clear();

            Base_Sub_Clear("item");
            Base_Sub_Clear("Rece");


            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(panel10, txtSellDate);
            ct.from_control_clear(panel11, mtxtSellDateRe);

            txt_SumCnt.Text = ""; txt_SumPV.Text = ""; txt_SumPr.Text = ""; txt_SumCV.Text = "";

            //cbZipCode_TH.SelectedIndex = -1;
            txtZipCode_TH.Clear();
            cbDistrict_TH.SelectedIndex = -1;
            cbProvince_TH.SelectedIndex = -1;
        }



        private void Base_Sub_Clear(string s_Tf)
        {
            cls_form_Meth ct = new cls_form_Meth();
            
            if (s_Tf == "item")
            {

                ct.from_control_clear(panel5, txt_ItemCode);
                //2020-05-12 교환막음 
                txt_ItemCode.BorderStyle = BorderStyle.Fixed3D;
                txt_ItemCode.BackColor = SystemColors.Window;
                butt_Item_Del.Visible = false; 

                butt_Item_Save.Text = ct._chang_base_caption_search("반품"); //butt_Item_Save.Text = ct._chang_base_caption_search("교환_정상");
            }

            if (s_Tf == "Rece")
            {
                dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Rece_Item.d_Grid_view_Header_Reset(1);

                ct.from_control_clear(groupBox2, txt_Receive_Method);
                butt_Rec_Del.Visible = false;
                butt_Rec_Save.Text = ct._chang_base_caption_search("추가");

                if (Sales_Rece != null)
                    Rece_Grid_Set(); //배송 그리드

                txt_Receive_Method.Focus();

            }

        }

       



        



        


        private void Base_Small_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Ord_Clear")
            {
                Base_Ord_Clear();
            }

            else if (bt.Name == "butt_Item_Clear")
            {
                Base_Sub_Clear("item");
            }
                          
            else if (bt.Name == "butt_Item_Save")
            {
                if (Base_Error_Check__01() == false) return;  //구매종류 , 회원, 구매일자 입력 안햇는지 체크

                if (Item_Rece_Error_Check__01() == false) return;

                //반품할 상품을 위쪽에서 선택해서 내린 경우에
                if (txt_SalesItemIndex.Text != "" && txt_SalesItemIndex_Re.Text == "") 
                    Base_Sub_Save_Item_Re();
                               

                //교환 상품을 선택해서 처음으로 저장해서 아래 그리드로 내리는 경우에ㅐ
                else if (txt_SalesItemIndex.Text == "" && txt_SalesItemIndex_Re.Text == "") 
                    Base_Sub_Save_Item_Re___02();


                //교환 상품을 아래쪽에서 더블 클릭해서 위로 올린 경우에.
                else if (txt_SalesItemIndex.Text == "" && txt_SalesItemIndex_Re.Text != "")
                    Base_Sub_Save_Item_Re___03();

                //반품 선택되서 아래 그리드로 내려간 내를 다시 더블 클릭해서 선택한 경우에
                else if (txt_SalesItemIndex.Text != "" && txt_SalesItemIndex_Re.Text != "")
                    Base_Sub_Save_Item_Re___04();

               
            }

            else if (bt.Name == "butt_Item_Del")
            {
                if (txt_SalesItemIndex_Re.Text == "") return;                

                Base_Sub_Delete("item");                
            }
              

        }



        private void Base_Sub_Delete(string s_Tf)
        {
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_form_Meth ct = new cls_form_Meth();

            if (s_Tf == "item")
            {
                //구매 상품 관련 딕셔너리에서 찾아서.. 삭제 표식을 해놓는다.
                SalesItemDetail[int.Parse(txt_SalesItemIndex_Re.Text)].Del_TF = "D";

                ct.from_control_clear(panel5, txt_ItemCode);
                butt_Item_Del.Visible = false;

                //2020-05-12 교환막음 txt_ItemCode.rEADONLY = FALSE;
                //2020-07-27 일본, 미국은 직판신고가 안되니 교환가동하도록함
                txt_ItemCode.BorderStyle = BorderStyle.Fixed3D;
                txt_ItemCode.BackColor = SystemColors.Window;
                butt_Item_Save.Text = ct._chang_base_caption_search("반품"); //butt_Item_Save.Text = ct._chang_base_caption_search("교환_정상");

                if (SalesItemDetail != null)
                    Item___02_Grid_Set(); //상품 그리드 
            }


            if (s_Tf == "Rece")
            {
                if (cls_app_static_var.Rec_info_Multi_TF == 0)
                {
                    //구매 상품 관련 딕셔너리에서 찾아서.. 삭제 표식을 해놓는다.
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

                ct.from_control_clear(panel37, txt_Receive_Method);
                chk_Total.Checked = false;
                butt_Rec_Del.Visible = false;
                butt_Rec_Save.Text = ct._chang_base_caption_search("추가");

                if (Sales_Rece != null)
                    Rece_Grid_Set(); //상품 그리드  

                if (SalesItemDetail != null)
                    Item_Grid_Set(); //상품 그리드     
            }

      

            ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del")
            ////       + "\n" +
            ////       cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }




        private void Base_Sub_Save_Item_Re()
        {
            cls_form_Meth ct = new cls_form_Meth();

            int New_SalesItemIndex = 0;
            int Dic_Key = 0 ;
                        
            if (SalesItemDetail != null)
            {
                foreach (int  t_key in SalesItemDetail.Keys)
                {                    
                    if (New_SalesItemIndex < SalesItemDetail[t_key].SalesItemIndex)
                    {
                        New_SalesItemIndex = t_key ;                        
                    }                     
                }
            }
            Dic_Key  = int.Parse(txt_SalesItemIndex.Text.Trim());
            New_SalesItemIndex = New_SalesItemIndex + 1;           
           
            cls_Sell_Item t_c_sell = new cls_Sell_Item();

            t_c_sell.OrderNumber = "" ;
            t_c_sell.SalesItemIndex = New_SalesItemIndex;
            t_c_sell.Real_index = int.Parse(txt_SalesItemIndex.Text.Trim());
            t_c_sell.T_OrderNumber1 = txt_OrderNumber.Text.Trim();
            t_c_sell.T_OrderNumber2 = txt_OrderNumber.Text.Trim();


            t_c_sell.ItemCode = SalesItemDetail[Dic_Key].ItemCode ;
            t_c_sell.ItemName = SalesItemDetail[Dic_Key].ItemName;
            t_c_sell.ItemCount = - int.Parse(txt_ItemCount.Text.Trim());

            t_c_sell.SellState = "R_3"; //정상:N_1  반품:R_1  교환나간거:N_3   교환들어온거:R_3
            t_c_sell.SellStateName = ct._chang_base_caption_search("반품");//"교환_반품");
            t_c_sell.Sell_VAT_TF = SalesItemDetail[Dic_Key].Sell_VAT_TF;
        


            t_c_sell.ItemPrice = - SalesItemDetail[Dic_Key].ItemPrice;
            t_c_sell.ItemPV = - SalesItemDetail[Dic_Key].ItemPV;
            t_c_sell.ItemCV = - SalesItemDetail[Dic_Key].ItemCV;
            t_c_sell.Sell_VAT_Price =  - SalesItemDetail[Dic_Key].Sell_VAT_Price;
            t_c_sell.Sell_Except_VAT_Price = - SalesItemDetail[Dic_Key].Sell_Except_VAT_Price;
            //++++++++++++++++++++++++++++++++



            t_c_sell.ItemTotalPrice = SalesItemDetail[Dic_Key].ItemPrice * t_c_sell.ItemCount;
            t_c_sell.ItemTotalPV = SalesItemDetail[Dic_Key].ItemPV * t_c_sell.ItemCount;
            t_c_sell.ItemTotalCV = SalesItemDetail[Dic_Key].ItemCV * t_c_sell.ItemCount;
            t_c_sell.Total_Sell_VAT_Price = SalesItemDetail[Dic_Key].Sell_VAT_Price * t_c_sell.ItemCount;
            t_c_sell.Total_Sell_Except_VAT_Price = SalesItemDetail[Dic_Key].Sell_Except_VAT_Price * t_c_sell.ItemCount;

            t_c_sell.ReturnDate = "";
            t_c_sell.SendDate = "";
            t_c_sell.ReturnBackDate = "";
            t_c_sell.Etc = txt_Item_Etc.Text.Trim();
            t_c_sell.RecIndex = 0;
            t_c_sell.Send_itemCount1 = 0;
            t_c_sell.Send_itemCount2 = 0;
            
            
            t_c_sell.G_Sort_Code = "";

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.Del_TF = "S";
            SalesItemDetail[New_SalesItemIndex] = t_c_sell;


            Base_Sub_Clear("item");

            if (SalesItemDetail != null)
                Item___02_Grid_Set(); //상품 그리드               

            ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
            ////            + "\n" +
            ////cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }


        private void Base_Sub_Save_Item_Re___02()
        {
            cls_form_Meth ct = new cls_form_Meth();
            
            int New_SalesItemIndex = 0;    
            if (SalesItemDetail != null)
            {
                foreach (int t_key in SalesItemDetail.Keys)
                {
                    if (New_SalesItemIndex < SalesItemDetail[t_key].SalesItemIndex)
                    {
                        New_SalesItemIndex = t_key;
                    }
                }
            }

            New_SalesItemIndex = New_SalesItemIndex + 1;


            cls_Sell_Item t_c_sell = new cls_Sell_Item();

            t_c_sell.OrderNumber = "";
            t_c_sell.SalesItemIndex = New_SalesItemIndex;
            t_c_sell.Real_index = 0;

            t_c_sell.ItemCode = txt_ItemCode.Text.Trim();
            t_c_sell.ItemName = txt_ItemName.Text.Trim();
            t_c_sell.ItemCount = int.Parse(txt_ItemCount.Text.Trim());


            t_c_sell.SellState = "N_3"; //정상:N_1  반품:R_1  교환나간거:N_3   교환들어온거:R_3
            t_c_sell.SellStateName = ct._chang_base_caption_search("교환_정상");
            t_c_sell.Sell_VAT_TF = 0;


            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql = "";
            Tsql = "Select price2 ,price4 , price5 , price6, price7, price8 ";
            Tsql = Tsql + " , Sell_VAT_Price , Except_Sell_VAT_Price   ";
            Tsql = Tsql + " From ufn_Good_Search_Web ('" + mtxtSellDateRe.Text.Replace("-", "").Trim() + "','" + idx_Na_Code + "') ";
            Tsql = Tsql + " Where NCode = '" + txt_ItemCode.Text.Trim() + "'";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

          
            t_c_sell.ItemPrice = double.Parse(ds.Tables["t_P_table"].Rows[0]["price2"].ToString());
            t_c_sell.ItemPV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price4"].ToString());

            t_c_sell.ItemCV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price5"].ToString());
            t_c_sell.Sell_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Sell_VAT_Price"].ToString());
            t_c_sell.Sell_Except_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Except_Sell_VAT_Price"].ToString());
            //++++++++++++++++++++++++++++++++

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
            t_c_sell.T_OrderNumber1 = "";
            t_c_sell.T_OrderNumber2 = "";
            
            t_c_sell.G_Sort_Code = "";

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.Del_TF = "S";
            SalesItemDetail[New_SalesItemIndex] = t_c_sell;


            Base_Sub_Clear("item");

            if (SalesItemDetail != null)
                Item___02_Grid_Set(); //상품 그리드               

            ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
            ////            + "\n" +
            ////cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }


        private void Base_Sub_Save_Item_Re___03()
        {

            int New_SalesItemIndex = 0;

            New_SalesItemIndex = int.Parse(txt_SalesItemIndex_Re.Text);


            SalesItemDetail[New_SalesItemIndex].ItemCode = txt_ItemCode.Text.Trim();
            SalesItemDetail[New_SalesItemIndex].ItemName = txt_ItemName.Text.Trim();
            SalesItemDetail[New_SalesItemIndex].ItemCount = int.Parse(txt_ItemCount.Text.Trim());


            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql = "";
            Tsql = "Select price2 ,price4 , price5 , price6 , price7, price8 ";
            Tsql = Tsql + " , Sell_VAT_Price , Except_Sell_VAT_Price   ";
            //            Tsql = Tsql + " From ufn_Good_Search_01 ('" + txtSellDate.Text.Trim() + "') ";
            Tsql = Tsql + " From ufn_Good_Search_Web ('" + mtxtSellDateRe.Text.Replace("-", "").Trim() + "','" + idx_Na_Code + "') ";
            Tsql = Tsql + " Where NCode = '" + txt_ItemCode.Text.Trim() + "'";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;



            SalesItemDetail[New_SalesItemIndex].ItemPrice = double.Parse(ds.Tables["t_P_table"].Rows[0]["price2"].ToString());
            SalesItemDetail[New_SalesItemIndex].ItemPV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price4"].ToString());

            SalesItemDetail[New_SalesItemIndex].ItemCV = double.Parse(ds.Tables["t_P_table"].Rows[0]["price5"].ToString());
            SalesItemDetail[New_SalesItemIndex].Sell_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Sell_VAT_Price"].ToString());
            SalesItemDetail[New_SalesItemIndex].Sell_Except_VAT_Price = double.Parse(ds.Tables["t_P_table"].Rows[0]["Except_Sell_VAT_Price"].ToString());
            //++++++++++++++++++++++++++++++++

            SalesItemDetail[New_SalesItemIndex].ItemTotalPrice = SalesItemDetail[New_SalesItemIndex].ItemPrice * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].ItemTotalPV = SalesItemDetail[New_SalesItemIndex].ItemPV * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].ItemTotalCV = SalesItemDetail[New_SalesItemIndex].ItemCV * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].Total_Sell_VAT_Price = SalesItemDetail[New_SalesItemIndex].Sell_VAT_Price * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].Total_Sell_Except_VAT_Price = SalesItemDetail[New_SalesItemIndex].Sell_Except_VAT_Price * SalesItemDetail[New_SalesItemIndex].ItemCount;

            SalesItemDetail[New_SalesItemIndex].Etc = txt_Item_Etc.Text.Trim();

            Base_Sub_Clear("item");

            if (SalesItemDetail != null)
                Item___02_Grid_Set(); //상품 그리드               

            ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
            ////            + "\n" +
            ////cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }




        private void Base_Sub_Save_Item_Re___04()
        {

            int New_SalesItemIndex = 0;
            int Dic_Key = int.Parse(txt_SalesItemIndex.Text);
            New_SalesItemIndex = int.Parse(txt_SalesItemIndex_Re.Text);
            
            SalesItemDetail[New_SalesItemIndex].ItemCode = txt_ItemCode.Text.Trim();
            SalesItemDetail[New_SalesItemIndex].ItemName = txt_ItemName.Text.Trim();
            SalesItemDetail[New_SalesItemIndex].ItemCount = -int.Parse(txt_ItemCount.Text.Trim());
            SalesItemDetail[New_SalesItemIndex].Real_index = int.Parse(txt_SalesItemIndex.Text);
            SalesItemDetail[New_SalesItemIndex].T_OrderNumber1 = txt_OrderNumber.Text.Trim();
            SalesItemDetail[New_SalesItemIndex].T_OrderNumber2 = txt_OrderNumber.Text.Trim();


            SalesItemDetail[New_SalesItemIndex].ItemPrice = -SalesItemDetail[Dic_Key].ItemPrice;
            SalesItemDetail[New_SalesItemIndex].ItemPV = -SalesItemDetail[Dic_Key].ItemPV;
            SalesItemDetail[New_SalesItemIndex].ItemCV = -SalesItemDetail[Dic_Key].ItemCV;
            SalesItemDetail[New_SalesItemIndex].Sell_VAT_Price = -SalesItemDetail[Dic_Key].Sell_VAT_Price;
            SalesItemDetail[New_SalesItemIndex].Sell_Except_VAT_Price = -SalesItemDetail[Dic_Key].Sell_Except_VAT_Price;
            //++++++++++++++++++++++++++++++++

            SalesItemDetail[New_SalesItemIndex].ItemTotalPrice = SalesItemDetail[Dic_Key].ItemPrice * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].ItemTotalPV = SalesItemDetail[Dic_Key].ItemPV * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].ItemTotalCV = SalesItemDetail[Dic_Key].ItemCV * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].Total_Sell_VAT_Price = SalesItemDetail[Dic_Key].Sell_VAT_Price * SalesItemDetail[New_SalesItemIndex].ItemCount;
            SalesItemDetail[New_SalesItemIndex].Total_Sell_Except_VAT_Price = SalesItemDetail[Dic_Key].Sell_Except_VAT_Price * SalesItemDetail[New_SalesItemIndex].ItemCount;
            
            SalesItemDetail[New_SalesItemIndex].Etc = txt_Item_Etc.Text.Trim();

            Base_Sub_Clear("item");

            if (SalesItemDetail != null)
                Item___02_Grid_Set(); //상품 그리드               

            ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
            ////            + "\n" +
            ////cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
        }




        private void dGridView_Base_Sub_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Name == "dGridView_Base_Item")
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0].Value != null)
                {
                    int T_key = int.Parse (dgv.CurrentRow.Cells[0].Value.ToString());


                    if (SalesItemDetail[T_key].SellState == "R_1" || SalesItemDetail[T_key].SellState == "R_3")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        dGridView_Base_Item.Focus();
                        return;
                    }


                    Put_Sub_Date(T_key,  dgv.CurrentRow.Cells[0].Value.ToString(),"item");
                }
            }

            if (dgv.Name == "dGridView_Base_Item___02")
            {
                if (dgv.CurrentRow != null &&  dgv.CurrentRow.Cells[0].Value != null)
                {
                    int T_key = int.Parse(dgv.CurrentRow.Cells[0].Value.ToString());

                    Put_Sub_Date(T_key, dgv.CurrentRow.Cells[0].Value.ToString(), "item___02");
                }
            }
        }


        private void Put_Sub_Date(int T_key,string SalesItemIndex , string t_STF)
        {
            if (t_STF == "item")
            {
                txt_SalesItemIndex_Re.Text = "";
                txt_SalesItemIndex.Text = SalesItemIndex;
                txt_MCnt.Text = "0";

                cls_form_Meth cm = new cls_form_Meth();
                butt_Item_Save.Text = cm._chang_base_caption_search("반품");
                int Salesitemindex = int.Parse(txt_SalesItemIndex.Text);
                txt_ItemCode.Text = SalesItemDetail[T_key].ItemCode;
                txt_ItemName.Text = SalesItemDetail[T_key].ItemName;
                txt_ItemCount.Text = SalesItemDetail[T_key].ItemCount.ToString();
                txt_Item_Etc.Text = SalesItemDetail[T_key].Etc;
                
                txt_ItemCode.ReadOnly = true;
                txt_ItemCode.BorderStyle = BorderStyle.FixedSingle;
                txt_ItemCode.BackColor = cls_app_static_var.txt_Enable_Color;
                butt_Item_Del.Visible = false;

                Check_Max_Re_Cnt(T_key);//반품 가능한 수량의 수를 가져온다. 텍스트 박스에

                if (int.Parse(txt_MCnt.Text) == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re_003")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    Base_Sub_Clear("item");
                    dGridView_Base_Item.Focus() ;
                    return;
                }
            }

            if (t_STF == "item___02")
            {
                txt_SalesItemIndex_Re.Text = SalesItemIndex;
                txt_MCnt.Text = "0";


                //반품 상품을 더블 클릭해서 위로 올려 놓을 시에는
                if (SalesItemDetail[T_key].Real_index > 0)
                {
                    txt_SalesItemIndex.Text = SalesItemDetail[T_key].Real_index.ToString();
                    txt_ItemCount.Text = (- SalesItemDetail[T_key].ItemCount).ToString();

                    Check_Max_Re_Cnt(SalesItemDetail[T_key].Real_index, T_key);//반품 가능한 수량의 수를 가져온다. 텍스트 박스에
                }
                else
                {
                    txt_SalesItemIndex.Text = "";
                    txt_ItemCount.Text = SalesItemDetail[T_key].ItemCount.ToString();
                }

                

                cls_form_Meth cm = new cls_form_Meth();
                butt_Item_Save.Text = cm._chang_base_caption_search("수정");                
                
                txt_ItemCode.Text = SalesItemDetail[T_key].ItemCode;
                txt_ItemName.Text = SalesItemDetail[T_key].ItemName;
                txt_Item_Etc.Text = SalesItemDetail[T_key].Etc;

                txt_ItemCode.ReadOnly = true;
                txt_ItemCode.BorderStyle = BorderStyle.FixedSingle;
                txt_ItemCode.BackColor = cls_app_static_var.txt_Enable_Color;
                butt_Item_Del.Visible = true;

                

                //if (int.Parse(txt_MCnt.Text) == 0)
                //{
                //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re_003")
                //    + "\n" +
                //    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                //    Base_Sub_Clear("item");
                //    dGridView_Base_Item.Focus();
                //    return;
                //}
            }

        }


        private void Check_Max_Re_Cnt(int T_key, int s_Tf  = 0)
        {

            int M_Cnt = 0;
            if (s_Tf == 0)
                M_Cnt = int.Parse (txt_ItemCount.Text .Trim()) ;
            else
                M_Cnt = SalesItemDetail[T_key].ItemCount ;

            int Ord_Len = txt_OrderNumber.Text.Trim().Length;

            string StrSql = "";
            StrSql = "Select ISNULL(Sum(itemCount) ,0 ) From tbl_SalesitemDetail (nolock) " ;
            StrSql = StrSql + " Where Right(OrderNumber," + Ord_Len + ") = '" + txt_OrderNumber.Text.Trim() + "'"; 
            StrSql = StrSql + " And  SellState ='R_3'" ;
            StrSql = StrSql + " And  LEN(OrderNumber) >" + Ord_Len ;
            StrSql = StrSql + " And  Real_index = " + T_key ; 

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, base_db_name, ds) == false) return ;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  
            {
                M_Cnt = M_Cnt + int.Parse (ds.Tables[base_db_name].Rows[0][0].ToString());
            }
            //++++++++++++++++++++++++++++++++    


            if (SalesItemDetail != null)
            {
                if (s_Tf == 0)
                    foreach (int t_key in SalesItemDetail.Keys)
                    {
                        if (SalesItemDetail[t_key].Del_TF != "D")
                        {
                            if (SalesItemDetail[t_key].OrderNumber == "")
                            {
                                if (SalesItemDetail[t_key].Real_index == T_key)
                                {
                                    M_Cnt = M_Cnt + SalesItemDetail[t_key].ItemCount;
                                }
                            }
                        }
                    }
                else
                {
                    foreach (int t_key in SalesItemDetail.Keys)
                    {
                        if (SalesItemDetail[t_key].Del_TF != "D")
                        {
                            if (SalesItemDetail[t_key].OrderNumber == "")
                            {                            
                                if (SalesItemDetail[t_key].Real_index == T_key && s_Tf  != t_key)
                                {
                                    M_Cnt = M_Cnt + SalesItemDetail[t_key].ItemCount;
                                }
                            }
                        }
                    }
                }
            }



            txt_MCnt.Text = M_Cnt.ToString () ;             
        }








        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
            //SendKeys.Send("{TAB}");
        }

   
  
        
        private Boolean Check_TextBox_Error()
        {
            //구매종류 , 회원, 구매일자 입력 안햇는지 체크
            if (Base_Error_Check__01() == false) return false;                        

            //회원번호 관련 관련 오류 체크 및 존재 여부 그리고 탈퇴 여부(신규 저장일 경우에)                      
            if (Input_Error_Check(mtxtMbid, "m",1) == false) return false;                                            
            
            if (Input_Error_Check_Save() == false) return false;
             
            return true;
        }



        private bool Input_Error_Check_Save()
        {

            int idx_ReturnTF = 0;           
            idx_ReturnTF = SalesDetail[txt_OrderNumber.Text.Trim()].ReturnTF;


            if (int.Parse(txtSellDate.Text.Replace("-", "").Trim()) > int.Parse(mtxtSellDateRe.Text.Replace("-", "").Trim()))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate_ReDate_Er")
                             + "\n" +
                             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }


            if (idx_ReturnTF == 2) //전체 관련해서 이화면에서 아무런 처리 못되게 한다.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_2")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }



            cls_Search_DB csd = new cls_Search_DB();

            //반품일자로 입력한 날짜가 마감 적용된 날짜인지 확인한다.
            if (csd.Close_Check_SellDate("tbl_CloseTotal_02", mtxtSellDateRe.Text.Replace("-", "").Trim()) == false)
            {
                mtxtSellDateRe.Focus(); return false;
            }



            //이미 이건으로 해서 반품한 내역이 있는지를 체크한다.
            foreach (string  t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D")
                {
                    if (SalesDetail[t_key].Re_BaseOrderNumber == txt_OrderNumber.Text.Trim())
                    {
                        if (SalesDetail[t_key].ReturnTF == 2) //전체반품한 내역이 있으면 부분이나 교환 못하게 막는다.
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                            mtxtSellDateRe.Focus();
                            return false;
                        }
                    }
                }
            }



            if (dGridView_Base_Item___02.RowCount == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Goods")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                txt_ItemCode.Focus();
                return false;
            }

           
           
                        
            return true;
        }









        private void Input_SalesDetail_dic()
        {
            cls_form_Meth ct = new cls_form_Meth();

            double Total_Sell_VAT_Price = 0; double Total_Sell_Except_VAT_Price = 0;
            double TotalPrice = 0; double TotalPv = 0; int R_TF = 0; double TotalCv = 0;
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    if (SalesItemDetail[t_key].OrderNumber == "")
                    {
                        Total_Sell_VAT_Price = Total_Sell_VAT_Price + SalesItemDetail[t_key].Total_Sell_VAT_Price;
                        Total_Sell_Except_VAT_Price = Total_Sell_Except_VAT_Price + SalesItemDetail[t_key].Total_Sell_Except_VAT_Price;
                        TotalPrice = TotalPrice + SalesItemDetail[t_key].ItemTotalPrice;
                        TotalPv = TotalPv + SalesItemDetail[t_key].ItemTotalPV;
                        TotalCv = TotalCv + SalesItemDetail[t_key].ItemTotalCV ;

                        if (SalesItemDetail[t_key].ItemCount >= 0)
                            R_TF++;
                    }
                }
            }            
            
            cls_Sell t_c_sell = new cls_Sell();

            //기존구매주문번호에다가 앞에 RR을 붙여서 전체 반품이라는 표식을 하고 주문번호를 만든다.
            t_c_sell.OrderNumber = "";
            
            t_c_sell.Mbid = idx_Mbid ;
            t_c_sell.Mbid2 = idx_Mbid2;
            t_c_sell.M_Name = txtName.Text.Trim();
            t_c_sell.Na_Code = idx_Na_Code; 
            
            t_c_sell.SellCode = txtSellCode_Code.Text.Trim();
            t_c_sell.SellCodeName = txtSellCode.Text.Trim ();

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

            t_c_sell.Re_BaseOrderNumber = txt_OrderNumber.Text.Trim(); //원 구매 관련 주문번호를 넣는다.

            t_c_sell.TotalPrice = TotalPrice;
            t_c_sell.TotalPV = TotalPv;
            t_c_sell.TotalCV = TotalCv ;
            t_c_sell.TotalInputPrice = 0;
            t_c_sell.Total_Sell_VAT_Price =  Total_Sell_VAT_Price;
            t_c_sell.Total_Sell_Except_VAT_Price =  Total_Sell_Except_VAT_Price;
            t_c_sell.InputCash = 0;
            t_c_sell.InputCard = 0;
            t_c_sell.InputCoupon = 0;
            t_c_sell.InputPassbook = 0 ;
            t_c_sell.InputMile = 0;
            t_c_sell.InputPass_Pay = 0;
            t_c_sell.UnaccMoney = 0;
            
            t_c_sell.Etc1 = txt_ETC1_R.Text.Trim();
            t_c_sell.Etc2 = txt_ETC2_R.Text.Trim();

            if (R_TF >= 1)
            {
                t_c_sell.ReturnTF = 4;
                t_c_sell.ReturnTFName = ct._chang_base_caption_search("교환");
            }
            else
            {
                t_c_sell.ReturnTF = 3;
                t_c_sell.ReturnTFName = ct._chang_base_caption_search("부분반품");
            }
            t_c_sell.INS_Num = "";
            t_c_sell.InsuranceNumber_Date = "";
            t_c_sell.W_T_TF = 0;
            t_c_sell.In_Cnt = 0;

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";
                                
            t_c_sell.SellDate = mtxtSellDateRe.Text.Replace ("-","").Trim();

            t_c_sell.Na_Code = SalesDetail[txt_OrderNumber.Text.Trim()].Na_Code;  //국가코드도 바로 넣어버린다.

            t_c_sell.Del_TF = "S";
            SalesDetail[""] = t_c_sell;
        }





        private void DB_Save_tbl_SalesDetail( ref string T_ord_N)
        {
            int rC_Cnt = 0; int R_TF = 0;
            foreach (string  t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D")
                {
                    if (t_key != "" && SalesDetail[t_key].Re_BaseOrderNumber == txt_OrderNumber.Text.Trim())
                    {
                        if (SalesDetail[t_key].ReturnTF == 3 || SalesDetail[t_key].ReturnTF == 4)
                        {
                            rC_Cnt++;
                        }
                    }                        
                }
            }

            rC_Cnt++;


            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    if (SalesItemDetail[t_key].OrderNumber == "")
                    {                   
                        if (SalesItemDetail[t_key].ItemCount >= 0)
                            R_TF++;
                    }
                }
            }

            if (R_TF >= 1) //한건이라도 나간게 있다 그럼 교환이네요
            {
                SalesDetail[T_ord_N].OrderNumber = "RC" + rC_Cnt.ToString() + txt_OrderNumber.Text.Trim();
                T_ord_N = "RC" + rC_Cnt.ToString() + txt_OrderNumber.Text.Trim();
            }
            else  //한건리아도 나간게 없다. 그럼 부분 반품이네요.
            {
                SalesDetail[T_ord_N].OrderNumber = "RP" + rC_Cnt.ToString()  + txt_OrderNumber.Text.Trim();
                T_ord_N = "RP" + rC_Cnt.ToString() + txt_OrderNumber.Text.Trim();
            }
            //++++++++++++++++++++++++++++++++
            
        }




        private void DB_Save_tbl_SalesDetail____002(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran,  string OrderNumber)
        {
            string StrSql = "";
            string Ins_Ordernumber = "";

            StrSql = "INSERT INTO tbl_SalesDetail" ;
            StrSql = StrSql + " (OrderNumber,Mbid,Mbid2,M_Name,SellDate,SellDate_2,SellCode,BusCode,";
            StrSql = StrSql + " TotalPrice,TotalPV,TotalCV,TotalInputPrice,";
            StrSql = StrSql + " Total_Sell_VAT_Price,Total_Sell_Except_VAT_Price, ";
            StrSql = StrSql + " InputCash,InputCard,InputPassbook, InputMile,UnaccMoney,";
            StrSql = StrSql + " Etc1,Etc2, ";
            StrSql = StrSql + " ReturnTF,InsuranceNumber,InsuranceNumber_Date, ";
            StrSql = StrSql + " Re_BaseOrderNumber , Na_Code,  ";
            StrSql = StrSql + " RecordID,RecordTime";

            StrSql = StrSql + " ) Values ( ";
            StrSql = StrSql + "'" + SalesDetail[Ins_Ordernumber].OrderNumber + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Mbid + "'";
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Mbid2;
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].M_Name + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellDate + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellDate + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellCode + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].BusCode + "'";
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalPrice ;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalPV;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalCV;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].TotalInputPrice;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Total_Sell_VAT_Price;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Total_Sell_Except_VAT_Price;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputCash;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputCard;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPassbook;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputMile;
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].UnaccMoney;
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Etc1 + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Etc2 + "'";
            StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].ReturnTF;
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].INS_Num + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].InsuranceNumber_Date + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Re_BaseOrderNumber   + "'";
            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Na_Code  + "'";

            StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].RecordID + "'";
            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) " ;
            StrSql = StrSql + ")";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_SalesDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;
            
        }



        private void DB_Save_tbl_SalesItemDetail(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, 
                    string OrderNumber)
        {

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                { 
                    if (SalesItemDetail[t_key].OrderNumber == "")
                        DB_Save_tbl_SalesItemDetail____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
            }
        }



        private void DB_Save_tbl_SalesItemDetail____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";

            cls_form_Meth ct = new cls_form_Meth();
            string SellState = ct._chang_base_caption_search("반품");


            StrSql = "Insert Into tbl_SalesitemDetail (";
            StrSql = StrSql + " SalesItemIndex,OrderNumber,";
            StrSql = StrSql + " ItemCode,ItemPrice,ItemPv,ItemCv,";
            StrSql = StrSql + " Sell_VAT_TF , Sell_VAT_Price, Sell_Except_VAT_Price,SellState,";
            StrSql = StrSql + " ItemCount,ItemTotalPrice,ItemTotalPV,ItemTotalcV,";
            StrSql = StrSql + " Total_Sell_VAT_Price, Total_Sell_Except_VAT_Price ";
            StrSql = StrSql + ", Real_index,";
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
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemPV;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemCV;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Sell_VAT_TF;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Sell_VAT_Price;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Sell_Except_VAT_Price;

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].SellState  + "'";

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemCount;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemTotalPrice;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemTotalPV;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].ItemTotalCV;

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Total_Sell_VAT_Price;
            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Total_Sell_Except_VAT_Price;

            StrSql = StrSql + "," + SalesItemDetail[SalesItemIndex].Real_index ;
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",''";

            StrSql = StrSql + ",''";
            StrSql = StrSql + ",0";

            StrSql = StrSql + ",0 ";
            StrSql = StrSql + ",0 ";

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].T_OrderNumber1  +"'";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].T_OrderNumber2  +"'";
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].RecordID + "'";
            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_SalesItemDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;
           
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

            StrSql = StrSql + ",Get_Tel1= '" + encrypter.Encrypt(Sales_Rece[SalesItemIndex].Get_Tel1) + "'";
            StrSql = StrSql + ",Get_Tel2= '" + encrypter.Encrypt(Sales_Rece[SalesItemIndex].Get_Tel2) + "'";

            StrSql = StrSql + ",Get_Etc1= '" + Sales_Rece[SalesItemIndex].Get_Etc1 + "'";
            StrSql = StrSql + ",Get_Etc2= '" + Sales_Rece[SalesItemIndex].Get_Etc2 + "'";

            StrSql = StrSql + ",Pass_Pay= " + Sales_Rece[SalesItemIndex].Pass_Pay;
            StrSql = StrSql + ",Pass_Number2= '" + Sales_Rece[SalesItemIndex].Pass_Number2 + "'";

            StrSql = StrSql + ",Base_Rec= '" + Sales_Rece[SalesItemIndex].Base_Rec + "'";

            StrSql = StrSql + " Where OrderNumber = '" + Sales_Rece[SalesItemIndex].OrderNumber + "'";
            StrSql = StrSql + " And SalesItemIndex = " + SalesItemIndex.ToString();

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

            //구매 상품 테이블의 변경 내역을 테이블에 넣는다.
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
            StrSql = StrSql + " Pass_Pay,Pass_Number,Base_Rec ";
            StrSql = StrSql + " ,RecordID,RecordTime ";
            StrSql = StrSql + " ) values(";

            StrSql = StrSql + "" + Sales_Rece[SalesItemIndex].SalesItemIndex;
            StrSql = StrSql + "," + Sales_Rece[SalesItemIndex].RecIndex;
            StrSql = StrSql + ",'" + OrderNumber + "'";

            StrSql = StrSql + "," + Sales_Rece[SalesItemIndex].Receive_Method;

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Date1.Replace("-", "") + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Date2.Replace("-", "") + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Name1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Name2 + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_ZipCode.Replace("-", "") + "'";
            StrSql = StrSql + ",'" +Sales_Rece[SalesItemIndex].Get_Address1 + "'";
            StrSql = StrSql + ",'" +Sales_Rece[SalesItemIndex].Get_Address2 + "'";

            StrSql = StrSql + ",'" +Sales_Rece[SalesItemIndex].Get_Tel1 + "'";
            StrSql = StrSql + ",'" +Sales_Rece[SalesItemIndex].Get_Tel2 + "'";

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Etc1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Etc2 + "'";

            StrSql = StrSql + "," + Sales_Rece[SalesItemIndex].Pass_Pay;
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Pass_Number + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Base_Rec + "'";

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
            str_Q = "Msg_Base_Save_Q";                            
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(str_Q), "", MessageBoxButtons.YesNo) == DialogResult.No) return;


            cls_Connect_DB Temp_Connect4 = new cls_Connect_DB();
            string Tsql2 = "select sellcode from tbl_salesdetail (NOLOCK) where  ordernumber = '" + txt_OrderNumber.Text + "'";
            DataSet ds2 = new DataSet();
            if (Temp_Connect4.Open_Data_Set(Tsql2, "tbl_salesdetail", ds2) == false) return;
            int ReCnt2 = Temp_Connect4.DataSet_ReCount;
            if (ReCnt2 == 1)
            {
                string primium_custom_string = ds2.Tables["tbl_salesdetail"].Rows[0][0].ToString();

                if (primium_custom_string == "04")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Partial returns are not available for Premium Custom Pack orders."
              + "\n" +
              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    else
                    {

                        MessageBox.Show("프리미엄 커스텀팩 주문은 부분반품을 할 수 없습니다."
                  + "\n" +
                  cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                    return;
                }
                else
                {
                    //txtSellCode_Code.Text = "04";
                }
            }




            if (Check_TextBox_Error() == false) return;
                       
            Input_SalesDetail_dic();   //판매 주 클래스 에 넣음 주문번호 ""으로 해서
           

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            string T_ord_N = "";
            cls_Search_DB csd = new cls_Search_DB();
            
            try
            {
                //저장할 것에 대한 주문번호를 따온다          
                DB_Save_tbl_SalesDetail( ref T_ord_N);

                //실질적인 저장,수정이 이루어지는곳. 변경시 주테이블 이전 내역도 같이 저장함
                DB_Save_tbl_SalesDetail____002(Temp_Connect, Conn, tran ,  T_ord_N );            
                
                DB_Save_tbl_SalesItemDetail(Temp_Connect, Conn, tran, T_ord_N);

                DB_Save_tbl_Sales_Rece(Temp_Connect, Conn, tran, T_ord_N);

                Temp_Ordernumber = T_ord_N; 


                tran.Commit();

                //직판 관련 조합 취소를 먼저 한다.. 이루어 지지 않았을 경우에는 메시지를 뛰우고 현 프로그램에서는 취소 과정을 그대로 진행한다.
                if (cls_app_static_var.Sell_Union_Flag == "D")
                {
                    Cancel_InsurancerNumber2(T_ord_N); //직판 관련 매출 취소가 이루어진다. 취소가 되든 안되든 우리쪽 프로그램에서는 취소를 시킨다..  직판 오류지 알아서 하라고함.                   
                }


                Save_Error_Check = 1;                
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
             
            }
            catch (Exception)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));                
            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }          
        }



        private void Cancel_InsurancerNumber2(string Ord)
        {
            string strSql = " SELECT * FROM [ufn_Get_Cancel_InsNum_Data]('" + Ord + "')";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            //공제번호가 발행되었거나 해당 원주문번호의 공제데이터가 가지않게되면 해당 RowCount 는 0가되니 보내지않아도된다.
            if (ReCnt == 0) return;


            cls_Socket csg = new cls_Socket();
            string Req = csg.Dir_Connect_Send_Cancel__2(Ord);


        }


        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            Base_Ord_Clear();

            if ((sender as DataGridView).CurrentRow.Cells[2].Value != null)
            {
                string OrderNumber = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();

                if (OrderNumber != "")
                {
                    //전체반품한 내역에 대해서는 이루어 질수 없도록 해버린다.
                    if (SalesDetail[OrderNumber].ReturnTF == 2)
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re_002")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        dGridView_Base.Focus();
                        return;
                    }

                    //이미 이건으로 해서 반품한 내역이 있는지를 체크한다.
                    foreach (string t_key in SalesDetail.Keys)
                    {
                        if (SalesDetail[t_key].Del_TF != "D")
                        {
                            if (SalesDetail[t_key].Re_BaseOrderNumber == OrderNumber)
                            {
                                if (SalesDetail[t_key].ReturnTF == 2)
                                {
                                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re")
                                    + "\n" +
                                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                                    dGridView_Base.Focus();
                                    return;
                                }
                            }
                        }
                    }


                    Set_SalesDetail(OrderNumber);
                    Set_SalesItemDetail(OrderNumber); //회원의 모든 상품 정보도 다 넣는다. 

                    if (SalesItemDetail != null)
                        Item_Grid_Set(); //상품 그리드      


                    if (SalesDetail[OrderNumber].ReturnTF == 4)  //교환 관련된 배송 정보만 나오게 한다.
                    {
                        Set_Sales_Rece(OrderNumber);  // 배송 

                        if (Sales_Rece != null)
                            Rece_Grid_Set(); //배송 그리드
                    }

                }
            }
        }


        private void Set_SalesDetail(string OrderNumber)
        {
            int idx_ReturnTF = SalesDetail[OrderNumber].ReturnTF;

            Data_Set_Form_TF  =1 ;
            //if (idx_ReturnTF == 1)
            //{
                txtSellDate.Text = SalesDetail[OrderNumber].SellDate.Replace("-", "");
                txtSellDate2.Text = SalesDetail[OrderNumber].SellDate_2.Replace("-", "");
                txtSellCode.Text = SalesDetail[OrderNumber].SellCodeName;
                txtSellCode_Code.Text = SalesDetail[OrderNumber].SellCode;
                txtCenter2.Text = SalesDetail[OrderNumber].BusCodeName;
                txtCenter2_Code.Text = SalesDetail[OrderNumber].BusCode;
                //string.Format(cls_app_static_var.str_Currency_Type, ds.Tables[base_db_name].Rows[0]["Last_price"]);
                txt_OrderNumber.Text = OrderNumber;
                txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPrice );
                txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPV);
                txt_TotalCv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalCV);

                txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);
                txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].UnaccMoney);

                txt_ETC1.Text = SalesDetail[OrderNumber].Etc1;
                txt_ETC2.Text = SalesDetail[OrderNumber].Etc2;


            //}
            //else if (idx_ReturnTF == 3 || idx_ReturnTF == 4)
            //{
            //    string Re_BaseOrderNumber = SalesDetail[OrderNumber].Re_BaseOrderNumber.Trim();
            //    txtSellDate.Text = SalesDetail[Re_BaseOrderNumber].SellDate.Replace("-", "");
            //    txtSellCode.Text = SalesDetail[Re_BaseOrderNumber].SellCodeName;
            //    txtSellCode_Code.Text = SalesDetail[Re_BaseOrderNumber].SellCode;
            //    txtCenter2.Text = SalesDetail[Re_BaseOrderNumber].BusCodeName;
            //    txtCenter2_Code.Text = SalesDetail[Re_BaseOrderNumber].BusCodeName;                
            //    txt_OrderNumber.Text = Re_BaseOrderNumber;
            //    txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalPrice);
            //    txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalPV);

            //    txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalInputPrice);
            //    txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].UnaccMoney);

            //    txt_ETC1.Text = SalesDetail[Re_BaseOrderNumber].Etc1;
            //    txt_ETC2.Text = SalesDetail[Re_BaseOrderNumber].Etc2;



            //    //////txtSellDateRe.Text = SalesDetail[OrderNumber].SellDate.Replace("-", "");
            //    //////txt_OrderNumber_R.Text = OrderNumber;
            //    //////txt_TotalPrice_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPrice);
            //    //////txt_TotalPv_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPV);

            //    ////////txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);
            //    ////////txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].UnaccMoney);

            //    //////txt_ETC1_R.Text = SalesDetail[OrderNumber].Etc1;
            //    //////txt_ETC2_R.Text = SalesDetail[OrderNumber].Etc2;
            //}

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
            strSql = strSql + "  When SellState = 'R_3' Then '" + cm._chang_base_caption_search("반품") + "'";//부분반품이다..... 기존에 "교환_반품" 이라 적혀져있었음 
            strSql = strSql + "  When SellState = 'C_1' Then '" + cm._chang_base_caption_search("취소") + "'";
            strSql = strSql + " END  SellStateName ";

            strSql = strSql + " From tbl_SalesitemDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail (nolock) ON tbl_SalesitemDetail.OrderNumber = tbl_SalesDetail.OrderNumber ";
            strSql = strSql + " LEFT JOIN tbl_Goods (nolock) ON tbl_Goods.Ncode = tbl_SalesitemDetail.ItemCode ";            
            strSql = strSql + " Where tbl_SalesitemDetail.OrderNumber = '" + OrderNumber.ToString() +"'" ;               
            strSql = strSql + " Order By tbl_SalesitemDetail.Ordernumber , SalesItemIndex ASC ";

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
                T_SalesitemDetail[ t_c_sell.SalesItemIndex] = t_c_sell;
            }
            
            SalesItemDetail  = T_SalesitemDetail;
        }


        private void Set_Sales_Rece(string OrderNumber)
        {

            string strSql = "";

            strSql = "Select tbl_Sales_Rece.*  ";
            strSql = strSql + " , Isnull(tbl_Base_Rec.name ,'' ) Base_Rec_Name ";
            strSql = strSql + " , Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " Receive_Method_Name ";
            strSql = strSql + " From tbl_Sales_Rece (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_Base_Rec (nolock) on tbl_Base_Rec.ncode = tbl_Sales_Rece.Base_Rec ";
            strSql = strSql + " LEFT JOIN tbl_Base_Change_Detail Ch_T (nolock) ON Ch_T.M_Detail_S = 'tbl_Sales_Rece' And  Ch_T.M_Detail = Convert(Varchar,tbl_Sales_Rece.Receive_Method) ";
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
                t_c_sell.Pass_Pay = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Pay"].ToString());

                t_c_sell.Pass_Number2 = ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Number2"].ToString();
                t_c_sell.Base_Rec = ds.Tables[base_db_name].Rows[fi_cnt]["Base_Rec"].ToString();
                t_c_sell.Base_Rec_Name = ds.Tables[base_db_name].Rows[fi_cnt]["Base_Rec_Name"].ToString();

                t_c_sell.Get_Etc1 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Etc1"].ToString();
                t_c_sell.Get_Etc2 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Etc2"].ToString();



                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();

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

            dGridView_Base_Rece_Add.Width = groupBox2.Width - 100;
            dGridView_Base_Rece_Add.Height = groupBox2.Height - 70;
            dGridView_Base_Rece_Add.Left = groupBox2.Left;
            dGridView_Base_Rece_Add.Top = groupBox2.Top;

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



            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";
            DataSet ds = new DataSet();
            int ReCnt = 0;

            if (t_rb.Name == "opt_Rec_Add1")
            {
                Tsql = "Select Addcode1 ,Address1 , Address2 , Address3 ";
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

                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
                ReCnt = Temp_Connect.DataSet_ReCount;
            }
            else if (t_rb.Name == "opt_Rec_Add3")
            {
                Data_Set_Form_TF = 1;
                mtxtZip1.Text = "";
                txtAddress1.Text = ""; txtAddress2.Text = "";
                mtxtTel1.Text = "";
                mtxtTel2.Text = "";
                txt_Get_Name1.Text = "";
                Data_Set_Form_TF = 0;
            }

            Data_Set_Form_TF = 1;
            mtxtZip1.Text = "";
            txtAddress1.Text = ""; txtAddress2.Text = "";
            mtxtTel1.Text = "";
            mtxtTel2.Text = "";
            txt_Get_Name1.Text = "";
            txtZipCode_TH.Text = "";
            cbDistrict_TH.SelectedIndex = -1;
            cbProvince_TH.SelectedIndex = -1;
            cbSubDistrict_TH.SelectedIndex = -1;
            Data_Set_Form_TF = 0;

            if (ReCnt == 0) return;

            Data_Set_Form_TF = 1;
            txtAddress1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address1"].ToString());
            txtAddress2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address2"].ToString());

            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
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
                if (ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Length >= 6)
                {
                    mtxtZip1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(0, 3) + "-" + ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(3, 3);
                    //txtAddCode1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(0, 3);
                    //txtAddCode2.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(3, 3);
                }
            }

            //string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
            cls_form_Meth cfm = new cls_form_Meth();
            //cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            mtxtTel1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString());

            //cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;
            mtxtTel2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString());

            if (t_rb.Name == "opt_Rec_Add2")
                txt_Get_Name1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString()); //주소테이블의 배송자명은 암호화 햇기 대문에
            else
                txt_Get_Name1.Text = ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString();  //회원 테이블의 회원명은 암호화 안햇음
            Data_Set_Form_TF = 0;
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
                //if (Recindex == 0)//삭제되지 않고 배송 정보가 없는 내역들만 뿌려준다.
                //{
                    V_Check = "V";
                    if (SalesItemDetail[t_key].Del_TF != "D" && SalesItemDetail[t_key].RecIndex == 0 
                    && SalesItemDetail[t_key].ItemCount > 0 
                    && SalesItemDetail[t_key].OrderNumber == "")
                        Set_gr_Rece_Item(ref gr_dic_text, t_key, fi_cnt, V_Check);  //데이타를 배열에 넣는다.
                //}
                //else
                //{
                //    if (SalesItemDetail[t_key].SalesItemIndex == Recindex)
                //    {
                //        V_Check = "V";
                //        Set_gr_Rece_Item(ref gr_dic_text, t_key, fi_cnt, V_Check);  //데이타를 배열에 넣는다.
                //    }
                //}

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

            string[] g_HeaderText = {"선택"  , ""   , "상품_코드"  , "상품명"   , "구매_수량"        
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


        
        private void dGridView_Base_Rece_Item_CellClick(object sender, DataGridViewCellEventArgs e)
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
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Rece_Header_Reset()
        {
            cgb_Rece.Grid_Base_Arr_Clear();
            cgb_Rece.basegrid = dGridView_Base_Rece;
            cgb_Rece.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Rece.grid_col_Count = 12;
            cgb_Rece.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "배송구분"   , "배송일"  , "수령인"   , "우편_번호"        
                                , "주소1"   , "주소2"    , "연락처_1"  , "연락처_2" , "비고"
                                , "주", "도시"
                                };

            int[] g_Width;
            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                g_Width = new int[] { 0, 90, 0, 90, 100
                                ,120 , 100 , 90 , 150 , 200
                                ,100, 100
                            };
            }
            // 그 외 국가인 경우
            else
            {
                g_Width = new int[] { 0, 90, 0, 90, 100
                                ,120 , 100 , 90 , 150 , 200
                                ,0, 0
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

                                ,DataGridViewContentAlignment.MiddleCenter  //11
                                ,DataGridViewContentAlignment.MiddleCenter
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Rece.grid_col_header_text = g_HeaderText;
            cgb_Rece.grid_cell_format = gr_dic_cell_format;
            cgb_Rece.grid_col_w = g_Width;
            cgb_Rece.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true
                                    ,true , true,
                                   };
            cgb_Rece.grid_col_Lock = g_ReadOnly;

            cgb_Rece.basegrid.RowHeadersVisible = false;
        }

        private void dGridView_Base_Rece_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

          
            if (dgv.Name == "dGridView_Base_Rece")
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0].Value != null)
                {
                    Put_Sub_Date(dgv.CurrentRow.Cells[0].Value.ToString(), "Rece");
                }
            }

  


        }

        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece


        private void Put_Sub_Date(string SalesItemIndex, string t_STF)
        {           

            if (t_STF == "Rece")
            {
                Data_Set_Form_TF = 1;
                txt_RecIndex.Text = SalesItemIndex;

                //butt_Rec_Del.Visible = true;
                cls_form_Meth cm = new cls_form_Meth();
                butt_Rec_Save.Text = cm._chang_base_caption_search("수정");
                int Salesitemindex = int.Parse(txt_RecIndex.Text);

                txt_Receive_Method.Text = Sales_Rece[Salesitemindex].Receive_Method_Name.ToString();
                txt_Receive_Method_Code.Text = Sales_Rece[Salesitemindex].Receive_Method.ToString();
                txt_Get_Name1.Text = Sales_Rece[Salesitemindex].Get_Name1;

                mtxtZip1.Text = "";

                if (Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Length >= 6)
                {
                    mtxtZip1.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(0, 3) + "-" + Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                    //txtAddCode2.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                }

                //string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
                cls_form_Meth cfm = new cls_form_Meth();
                //cfm.Phone_Number_Split(Sales_Rece[Salesitemindex].Get_Tel1.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;
                mtxtTel1.Text = Sales_Rece[Salesitemindex].Get_Tel1.ToString();

                //cfm.Phone_Number_Split(Sales_Rece[Salesitemindex].Get_Tel2.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;
                mtxtTel2.Text = Sales_Rece[Salesitemindex].Get_Tel2.ToString();


                txtAddress1.Text = Sales_Rece[Salesitemindex].Get_Address1;
                txtAddress2.Text = Sales_Rece[Salesitemindex].Get_Address2;
                txtGetDate1.Text = Sales_Rece[Salesitemindex].Get_Date1.ToString().Replace("-", "");
                txt_Pass_Number.Text = Sales_Rece[Salesitemindex].Pass_Number;
                txt_Base_Rec.Text = Sales_Rece[Salesitemindex].Base_Rec;
                txt_Base_Rec_Code.Text = Sales_Rece[Salesitemindex].Base_Rec_Name;
                txt_Get_Etc1.Text = Sales_Rece[Salesitemindex].Get_Etc1;

                Rece_Item_Grid_Set(int.Parse(SalesItemIndex));

                Data_Set_Form_TF = 0;
            }
            
        }



        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece
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
                Base_Sub_Delete("Rece");
            }


            else if (bt.Name == "butt_Rec_Save")
            {

                if (Base_Error_Check__01() == false) return;  //구매종류 , 회원, 구매일자 입력 안햇는지 체크

                if (Item_Rece_Error_Check__01("Rece") == false) return;


                if (txt_RecIndex.Text == "") //추가 일경우에 새로운 입력
                {
                    cls_form_Meth ct = new cls_form_Meth();

                    if (cls_app_static_var.Rec_info_Multi_TF == 0)
                    {
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
                    //Save_Button_Click_Cnt++;

                    //if (Save_Button_Click_Cnt == 1)
                    //{
                    //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
                    //                + "\n" +
                    //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                    //}
                }
                else
                {
                    if (Base_Error_Check__01() == false) return;  //구매종류 , 회원, 구매일자 입력 안햇는지 체크

                    if (Item_Rece_Error_Check__01("Rece") == false) return;

                    if (cls_app_static_var.Rec_info_Multi_TF == 0)                    
                        Base_Sub_Edit_Rece();
                    else
                        Base_Sub_Edit_Rece(1);

                    Base_Sub_Clear("Rece");
                    Base_Sub_Clear("item");
                    //Save_Button_Click_Cnt++;

                    //if (Save_Button_Click_Cnt == 1)
                    //{
                    //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit")
                    //                 + "\n" +
                    //    cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                    //}
                }
            }
        }




        private bool Item_Rece_Error_Check__01(string s_Tf)
        {
           

            if (s_Tf == "Rece")
            {
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
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Re_Select") + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    dGridView_Base_Rece_Item.Focus(); return false;
                }


            }


            return true;
        }



        private void Base_Sub_Save_Rece(int New_SalesItemIndex)
        {


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



            if (txtAddress1.Text.Trim() != "")
                t_c_sell.Get_Address1 = txtAddress1.Text.Trim();

            if (txtAddress2.Text.Trim() != "")
                t_c_sell.Get_Address2 = txtAddress2.Text.Trim();

            t_c_sell.Get_Etc1 = txt_Get_Etc1.Text.Trim();
            t_c_sell.Get_Etc2 = "";

            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                t_c_sell.Get_city = cbDistrict_TH.Text;
                t_c_sell.Get_state = cbProvince_TH.SelectedValue.ToString();
                if (txtZipCode_TH.Text.Replace("-", "").Trim() != "")
                {
                    t_c_sell.Get_ZipCode = txtZipCode_TH.Text.Replace("-", "");
                }
            }
            // 한국인 경우
            else
            {
                if (mtxtZip1.Text.Replace("-", "").Trim() != "")
                {
                    t_c_sell.Get_ZipCode = mtxtZip1.Text.Replace("-", "");
                }
            }

            t_c_sell.Pass_Number = txt_Pass_Number.Text.Trim();
            t_c_sell.Base_Rec_Name = txt_Base_Rec.Text.Trim();
            t_c_sell.Base_Rec = txt_Base_Rec_Code.Text.Trim();

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.Del_TF = "S";
            Sales_Rece[New_SalesItemIndex] = t_c_sell;


            SalesItemDetail[New_SalesItemIndex].RecIndex = New_SalesItemIndex;
            SalesItemDetail[New_SalesItemIndex].SendDate = txtGetDate1.Text.Trim();

            if (SalesItemDetail[New_SalesItemIndex].Del_TF == "")
                SalesItemDetail[New_SalesItemIndex].Del_TF = "U";


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

            if (Sales_Rece[SalesItemIndex].Del_TF == "")
                Sales_Rece[SalesItemIndex].Del_TF = "U";

            SalesItemDetail[SalesItemIndex].SendDate = txtGetDate1.Text.Trim();

            if (SalesItemDetail[SalesItemIndex].Del_TF == "")
                SalesItemDetail[SalesItemIndex].Del_TF = "U";
        }



        private void Base_Sub_Edit_Rece(int S_TF)
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

                    if (Sales_Rece[SalesItemIndex].Del_TF == "")
                        Sales_Rece[SalesItemIndex].Del_TF = "U";

                    SalesItemDetail[SalesItemIndex].SendDate = txtGetDate1.Text.Trim();

                    if (SalesItemDetail[SalesItemIndex].Del_TF == "")
                        SalesItemDetail[SalesItemIndex].Del_TF = "U";
                }
            }

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

        private void panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dGridView_Base_Rece_Add_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {


                txt_Get_Name1.Text = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();

                mtxtZip1.Text = "";

                if ((sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Length >= 6)
                {
                    mtxtZip1.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(0, 3) + "-" + (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(3, 3);
                    //txtAddCode2.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(3, 3);
                }

                txtAddress1.Text = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                txtAddress2.Text = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();


                mtxtTel1.Text = (sender as DataGridView).CurrentRow.Cells[4].Value.ToString();
                mtxtTel2.Text = (sender as DataGridView).CurrentRow.Cells[5].Value.ToString();

                //string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";

                cls_form_Meth cfm = new cls_form_Meth();


                //cfm.Phone_Number_Split((sender as DataGridView).CurrentRow.Cells[4].Value.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;

                //cfm.Phone_Number_Split((sender as DataGridView).CurrentRow.Cells[5].Value.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                //txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

                dGridView_Base_Rece_Add.Visible = false;
                cfm.form_Group_Panel_Enable_True(this);

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
