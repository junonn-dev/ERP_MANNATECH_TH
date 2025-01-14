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
    public partial class frmSell_R_01 : Form
    {
        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        public delegate void Take_NumberDele(ref string Send_Number, ref string Send_Name, ref string Send_OrderNumber);
        public event Take_NumberDele Take_Mem_Number;

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Cacu = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece = new cls_Grid_Base();
        
        private Dictionary<string , cls_Sell> SalesDetail ;
        private Dictionary<int, cls_Sell_Item> SalesItemDetail = new Dictionary<int, cls_Sell_Item>() ;
        private Dictionary<int, cls_Sell_Rece> Sales_Rece = new Dictionary<int, cls_Sell_Rece>();
        private Dictionary<int, cls_Sell_Cacu> Sales_Cacu = new Dictionary<int, cls_Sell_Cacu>();

        private Dictionary<string, TextBox>  Ncode_dic = new Dictionary<string, TextBox>();

        private const string base_db_name = "tbl_SalesDetail";
        private int Data_Set_Form_TF;
        private string idx_Mbid = "";
        private int idx_Mbid2 = 0;

        public frmSell_R_01()
        {
            InitializeComponent();
        }





        private void frmBase_From_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
           
            Data_Set_Form_TF = 0;
  

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset(1);

            dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Cacu.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset(1);         
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                        
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            
            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;            
            mtxtSn.Mask = "999999-9999999"; //기본 셋팅은 주민번호이다.      

            mtxtSn.BackColor = cls_app_static_var.txt_Enable_Color;
            txtCenter.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSellDate.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_OrderNumber.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSellCode.BackColor = cls_app_static_var.txt_Enable_Color;
            txtCenter2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalInputPrice.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_UnaccMoney.BackColor = cls_app_static_var.txt_Enable_Color;

            txt_TotalPv.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalBv.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC1.BackColor = cls_app_static_var.txt_Enable_Color;            
            txt_TotalPrice.BackColor = cls_app_static_var.txt_Enable_Color;

            txt_OrderNumber_R.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalPv_R.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalBv_R.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalPrice_R.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalCv_R.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_TotalCv.BackColor = cls_app_static_var.txt_Enable_Color;

            mtxtSellDateRe.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtSellDate3.Mask = cls_app_static_var.Date_Number_Fromat;


            if (cls_User.gid == cls_User.SuperUserID)
            {
                butt_Delete.Visible = true;
            }
        }



        private void frmBase_Resize(object sender, EventArgs e)
        {
            //butt_Exit.Left = this.Width - butt_Exit.Width - 20;

            //butt_Clear.Left = 3;
            //butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            //butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            //butt_Delete.Left = butt_Save.Left + butt_Save.Width + 2;
            ////this.Refresh();

            ////int base_w = this.Width / 4;
            ////butt_Clear.Width = base_w;
            ////butt_Save.Width = base_w;

            ////butt_Delete.Width = base_w;
            ////butt_Exit.Width = base_w;

            ////butt_Clear.Left = 0;
            ////butt_Save.Left = butt_Clear.Left + butt_Clear.Width;

            ////butt_Delete.Left = butt_Save.Left + butt_Save.Width;
            ////butt_Exit.Left = butt_Delete.Left + butt_Delete.Width;

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
            cfm.button_flat_change(butt_Ord_Clear);
            
        }


        private void frm_Base_Activated(object sender, EventArgs e)
        {
           //19-03-11 깜빡임제거 this.Refresh();

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
                    if (!this.Controls.ContainsKey("Popup_gr"))
                        this.Close();
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

                           // cls_form_Meth cfm = new cls_form_Meth();
                           // cfm.form_Group_Panel_Enable_True(this);
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
            string Mbid = ""; int Mbid2 = 0;
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
                             

                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";
                
                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }

                ////Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
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

                Set_SalesDetail();  //회원의 주문 관련 주테이블 내역을 클래스에 넣는다.

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
            
            mtxtMbid.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
            txtName.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
            mtxtSn.Text = encrypter.Decrypt ( ds.Tables[base_db_name].Rows[0]["Cpno"].ToString() ,"Cpno");
                  
            txtCenter.Text = ds.Tables[base_db_name].Rows[0]["B_Name"].ToString();
            txtCenter_Code.Text = ds.Tables[base_db_name].Rows[0]["businesscode"].ToString();

            txtName.ReadOnly = true;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.BackColor = cls_app_static_var.txt_Enable_Color;
        }


        private void  Set_SalesDetail ()
        {
            cls_form_Meth cm = new cls_form_Meth();
            string strSql = "";

            strSql = "Select tbl_SalesDetail.* ";
            strSql = strSql + " , tbl_Business.Name BusCodeName ";
            //strSql = strSql + " , tbl_SellType.SellTypeName SellCodeName  ";

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
            
            strSql = strSql + " ,Case When ReturnTF = 1 Then '" + cm._chang_base_caption_search("정상") + "'";
            strSql = strSql + "  When ReturnTF = 2 Then '" + cm._chang_base_caption_search("반품") + "'";
            strSql = strSql + "  When ReturnTF = 4 Then '" + cm._chang_base_caption_search("교환") + "'";
            strSql = strSql + "  When ReturnTF = 3 Then '" + cm._chang_base_caption_search("부분반품") + "'";
            strSql = strSql + "  When ReturnTF = 5 Then '" + cm._chang_base_caption_search("취소") + "'";
            strSql = strSql + " END ReturnTFName ";


            strSql = strSql + " , Ga_Order  SellTF ";
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

                //strSql = strSql + ", Case When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber ";
                //strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql = strSql + " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                //strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)' ";
                //strSql = strSql + " When  ReturnTF = 2 then '반품처리' ";
                //strSql = strSql + " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '재발급요청요망' + ' ' + tbl_SalesDetail.INS_Num_Err  ";
                //strSql = strSql + " ELSE tbl_SalesDetail.InsuranceNumber END  InsuranceNumber2 ";


                strSql += ", Case When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber ";
                strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail  (nolock)  AS A1 Where A1.Re_BaseOrderNumber <> '' And tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber Order by OrderNumber ) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                strSql += " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 2 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)' ";
                strSql += " When  ReturnTF = 1 And (Select top 1  A1.SellDate From tbl_SalesDetail (nolock) AS A1 Where A1.Re_BaseOrderNumber <> '' And  tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber And ReturnTF = 3 Order by OrderNumber  ) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(부분취소요청중)' ";
                strSql += " When  ReturnTF = 2 then '반품처리' ";
                strSql += " When  ReturnTF = 3 then '부분반품처리' ";
                strSql += " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '재발급요청요망' + ' ' + tbl_SalesDetail.INS_Num_Err  ";
                strSql += " ELSE tbl_SalesDetail.InsuranceNumber END InsuranceNumber2 ";

            }
            else
            {
                strSql = strSql + " , InsuranceNumber As InsuranceNumber2 ";
            }




            strSql = strSql + " From tbl_SalesDetail (nolock) ";
            //strSql = strSql + " LEFT JOIN tbl_SalesDetail_TF (nolock) ON tbl_SalesDetail.OrderNumber = tbl_SalesDetail_TF.OrderNumber ";
            strSql = strSql + " LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
            strSql = strSql + " LEFT Join tbl_SellType ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode ";
            strSql = strSql + " LEFT JOIN tbl_Business (nolock) ON tbl_SalesDetail.BusCode = tbl_Business.NCode And tbl_SalesDetail.Na_code = tbl_Business.Na_code  ";

            strSql = strSql + " LEFT JOIN T_REALMLM (nolock) ON T_REALMLM.SEQ = tbl_SalesDetail.union_Seq ";
            strSql = strSql + " LEFT JOIN T_REALMLM_ErrCode (nolock) ON T_REALMLM.ERRCODE = T_REALMLM_ErrCode.Er_Code ";

            if (idx_Mbid.Length == 0)
                strSql = strSql + " Where tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
            else
            {
                strSql = strSql + " Where tbl_Memberinfo.Mbid = '" + idx_Mbid + "' ";
                strSql = strSql + " And   tbl_Memberinfo.Mbid2 = " + idx_Mbid2.ToString();
            }

            strSql = strSql + " And (Ga_Order >= 0) "; //정상내역은 승인 내역만 보여준다.
            strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            strSql = strSql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";
            strSql = strSql + " Order By OrderNumber DESC ";

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
        private void Base_Grid_Set(  )
        {
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            double S_cnt4 = 0; double S_cnt5 = 0; double S_cnt6 = 0; double S_cnt7 = 0;
            double Sum_13 = 0; double Sum_14 = 0; double Sum_15 = 0; ; double Sum_16 = 0;            
            foreach (string t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D")
                {
                    Set_gr_dic(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.

                    S_cnt4 = S_cnt4 + SalesDetail[t_key].TotalPrice;
                    S_cnt5 = S_cnt5 + SalesDetail[t_key].TotalInputPrice;
                    S_cnt6 = S_cnt6 + SalesDetail[t_key].TotalPV;
                    S_cnt7 = S_cnt7 + SalesDetail[t_key].TotalCV;

                    Sum_13 = Sum_13 + SalesDetail[t_key].InputCash;
                    Sum_14 = Sum_14 + SalesDetail[t_key].InputCard;
                    Sum_15 = Sum_15 + SalesDetail[t_key].InputPassbook;
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
                                ,S_cnt7
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
                                ,SalesDetail[t_key].UnaccMoney 
                                ,SalesDetail[t_key].RecordID 

                                ,SalesDetail[t_key].RecordTime 
                                 };
            
            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Header_Reset()
        {
            cgb.Grid_Base_Arr_Clear();
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_col_Count = 16;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"승인여부" , "공제번호"  , "주문번호"   , "주문일자"  , "총주문액"       
                                    , "총입금액"      , "총PV" , "총CV"     , "주문종류"   , "구분"     
                                    , "현금"    ,"카드" ,"무통장", "미결제"  ,  "기록자" 
                                    ,  "기록일"
                                };

            


            if (cls_app_static_var.Sell_Union_Flag == "")  //직판특판이 아닌경우 공제번호 필드 안나오게
            {
                int[] g_Width = { 80,0, 120, 80, 80
                              , 80  ,80 , 80 , 80 , 80
                              ,80, 80  ,80 ,80,80 
                              ,80
                            };
                cgb.grid_col_w = g_Width;
            }
            else
            {

                int[] g_Width = { 80,120, 120, 80, 80
                              , 80  ,80 , 80 , 80 , 80
                              , 80  ,80 ,80,80 ,80
                              ,80
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
                                ,DataGridViewContentAlignment.MiddleCenter //10

                                ,DataGridViewContentAlignment.MiddleRight  
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight 
                                ,DataGridViewContentAlignment.MiddleRight 
                                 ,DataGridViewContentAlignment.MiddleCenter //15
                                 
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

            cgb.grid_col_header_text = g_HeaderText;
            cgb.grid_cell_format = gr_dic_cell_format;
            
            cgb.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true 
                                    ,true , true,  true       , true,  true   
                                    ,  true 
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
                txt_TotalBv.Text = "0";
                return;
            }

            int fi_cnt = 0; double T_Pv = 0; double T_pr = 0; double T_Cv = 0;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    T_Pv = T_Pv + SalesItemDetail[t_key].ItemTotalPV  ;
                    T_pr = T_pr + SalesItemDetail[t_key].ItemTotalPrice  ;
                    T_Cv = T_Cv + SalesItemDetail[t_key].ItemTotalCV;
                }
                fi_cnt++;
            }

            txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, T_pr);
            txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, T_Pv);
            txt_TotalBv.Text = string.Format(cls_app_static_var.str_Currency_Type, T_Cv);
        }


        private void Item_Grid_Set()
        {
            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                    Set_gr_Item(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                fi_cnt++;
            }

            cgb_Item.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Item.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Item(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt)
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
                                , "개별BV"  , "주문_수량"   , "총상품액"    , "총상품PV"  , "총상품CV"
                                , "구분", "비고"
                                };



            int[] g_Width = { 0, 90, 160, 80, 70
                                ,80 , 80 , 80 , 70 ,70
                                ,70,200
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

            
            cgb_Item.grid_col_header_text = g_HeaderText;
            cgb_Item.grid_cell_format = gr_dic_cell_format;
            cgb_Item.grid_col_w = g_Width;
            cgb_Item.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true                                                            
                                     ,true       ,true                                                         
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
            double Sell_Pr  = double.Parse(txt_TotalPrice.Text.Trim().Replace (",","")) ;
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
            txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, Sell_Pr - T_pr);
        }


        private void Cacu_Grid_Set()
        {
            dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Cacu.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            foreach (int t_key in Sales_Cacu.Keys)
            {
                if (Sales_Cacu[t_key].Del_TF != "D")
                    Set_gr_Cacu(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                fi_cnt++;
            }

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
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Cacu_Header_Reset()
        {
            cgb_Cacu.Grid_Base_Arr_Clear();
            cgb_Cacu.basegrid = dGridView_Base_Cacu;
            cgb_Cacu.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Cacu.grid_col_Count = 10;
            cgb_Cacu.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "결제방법"   , "결제액"  , "결제일자"   , "카드_은행명"        
                                , "카드_은행번호"   , "카드소유자"    , "입금자"  , "비고" , ""
                                };

            int[] g_Width = { 0, 90, 70, 90, 100
                                ,120 , 100 , 90 , 150 , 0
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
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            
            cgb_Cacu.grid_col_header_text = g_HeaderText;
            cgb_Cacu.grid_cell_format = gr_dic_cell_format;
            cgb_Cacu.grid_col_w = g_Width;
            cgb_Cacu.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true                                                            
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
            foreach (int t_key in Sales_Rece.Keys)
            {
                if (Sales_Rece[t_key].Del_TF != "D")
                    Set_gr_Rece(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                fi_cnt++;
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

                                ,Sales_Rece[t_key].Get_city
                                ,Sales_Rece[t_key].Get_state
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
                                , "태국_도시", "태국_주"
                                };

            int[] g_Width;

            if (cls_User.gid_CountryCode == "TH")
            {
                g_Width = new int[] {
                                0, 90, 70, 90, 100
                                ,120 , 100 , 90 , 150 , 200
                                ,100, 100
                            };
            }
            else
            {
                g_Width = new int[] {
                                0, 90, 70, 90, 100
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

                                ,DataGridViewContentAlignment.MiddleCenter
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
                                    ,true , true
                                   };
            cgb_Rece.grid_col_Lock = g_ReadOnly;

            cgb_Rece.basegrid.RowHeadersVisible = false;
        }
        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece
        //////Sales_Rece___Sales_Rece__Sales_Rece__Sales_Rece






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
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txtCenter2_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txtCenter2_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtSellCode")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txtSellCode_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txtSellCode_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }
                  

            //if (tb.Name == "txt_ItemCode")
            //{
            //    Data_Set_Form_TF = 1;

            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb, txt_ItemName, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txt_ItemName);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}


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
                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", strSql);


                if (tb.Name == "txt_Base_Rec")
                    cgb_Pop.db_grid_Popup_Base(2, "배송사_코드", "배송사", "ncode", "name", strSql);


                if (tb.Name == "txt_Receive_Method")
                    cgb_Pop.db_grid_Popup_Base(2, "배송_코드", "배송_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, strSql);

                if (tb.Name == "txt_C_TF")
                    cgb_Pop.db_grid_Popup_Base(2, "결제_코드", "결제_종류", "M_Detail", cls_app_static_var.Base_M_Detail_Ex, strSql);
                
                if (tb.Name == "txt_ItemCode")
                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV", "Name", "Ncode", "price2", "price4", strSql);

                             
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

                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", Tsql);
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


                

                if (tb.Name == "txt_ItemCode")
                {
                    string Tsql;
                    Tsql = "Select Name , NCode  ,price2 , price4  ";
                    Tsql = Tsql + " From ufn_Good_Search_01 ('" + cls_User.gid_date_time + "') ";
                    Tsql = Tsql + " Where NCode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";

                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV", "Name", "Ncode", "price2", "price4", Tsql);
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


            if (tb.Name == "txt_ItemCode")
            {
                Tsql = "Select Name , NCode ,price2 ,price4    ";
                Tsql = Tsql + " From ufn_Good_Search_01 ('" + cls_User.gid_date_time + "') ";
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
           // // Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
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
            
            //dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Item.d_Grid_view_Header_Reset();

            //dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu.d_Grid_view_Header_Reset();

            //dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece.d_Grid_view_Header_Reset();

            //dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece_Item.d_Grid_view_Header_Reset();
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<



            txtName.ReadOnly =false ;
            txtName.BackColor = SystemColors.Window;

            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, mtxtMbid);

            Base_Ord_Clear();

            mtxtSn.Mask = "999999-9999999";
            idx_Mbid = ""; idx_Mbid2 = 0;
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
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
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
                StrSql = "EXEC Usp_Insert_tbl_Sales_CanCel_CS '" + txt_OrderNumber_R.Text + "','" + cls_User.gid + "',0";

                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);

                Input_Mileage_Table_CanCel_Cancel(Temp_Connect, Conn, tran);

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


            if (txt_OrderNumber_R.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Sell_OrderNumber")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                dGridView_Base.Focus();
                return false;
            }

            string Ord_N = txt_OrderNumber_R.Text.Trim();

            ////////현 내역으로 연관되서 반품이나 교환한 내역이 잇다.
            //////foreach (string t_key in SalesDetail.Keys)
            //////{
            //////    if (SalesDetail[t_key].Del_TF != "D")
            //////    {
            //////        if (SalesDetail[t_key].Re_BaseOrderNumber == Ord_N)
            //////        {
            //////            if (SalesDetail[t_key].ReturnTF == 2)
            //////            {
            //////                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_2")
            //////                + "\n" +
            //////                cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //////            }
            //////            if (SalesDetail[t_key].ReturnTF == 3)
            //////            {
            //////                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_3")
            //////                + "\n" +
            //////                cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //////            }

            //////            if (SalesDetail[t_key].ReturnTF == 4)
            //////            {
            //////                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_4")
            //////                + "\n" +
            //////                cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //////            }
            //////            dGridView_Base.Focus();
            //////            break;
            //////        }
            //////    }
            //////}


            if (SalesDetail[Ord_N].ReturnTF.ToString() == "1")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_01")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellDate.Focus(); return false;
            }

            if (SalesDetail[Ord_N].ReturnTF.ToString() == "3")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_3")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellDate.Focus(); return false;
            }

            if (SalesDetail[Ord_N].ReturnTF.ToString() == "4")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_4")
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

            
            //주문일자를 넣었는지 먼저 체크한다. 안넣었으면 넣어라.
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


            //if (mtxtSellDateRe.Text.Replace("-", "").Trim() != "" && mtxtSellDate3.Text.Replace("-", "").Trim() == "")
            //{
            //    mtxtSellDate3.Text = mtxtSellDateRe.Text; 
            //}
            //
            //
            //if (mtxtSellDate3.Text.Replace("-", "").Trim() != "")
            //{
            //    if (Sn_Number_(mtxtSellDate3.Text, mtxtSellDate3, "Date") == false)
            //    {
            //        mtxtSellDate3.Focus();
            //        return false;
            //    }
            //
            //}




            //주문종류를 선택 안햇네 그럼 그것도 넣어라.
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



        private void Base_Ord_Clear()
        {
            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset();

            dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Cacu.d_Grid_view_Header_Reset();

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset();

        

            if (SalesItemDetail !=null )
                SalesItemDetail.Clear();
            if (Sales_Rece != null)
                Sales_Rece.Clear();
            if (Sales_Cacu != null)
                Sales_Cacu.Clear();

            Base_Sub_Clear("item");
            Base_Sub_Clear("Rece");
            Base_Sub_Clear("Cacu");


            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(panel7, txtSellDate);
            ct.from_control_clear(panel8, mtxtSellDateRe);            
        }



        private void Base_Sub_Clear(string s_Tf)
        {
            cls_form_Meth ct = new cls_form_Meth();
            
            if (s_Tf == "item")
            {             

                if (SalesItemDetail != null)
                    Item_Grid_Set(); //상품 그리드
            }

            if (s_Tf == "Rece")
            {               
                if (Sales_Rece != null)
                    Rece_Grid_Set(); //배송 그리드
            }


            if (s_Tf == "Cacu")
            {
                dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Cacu.d_Grid_view_Header_Reset();              

                if (Sales_Cacu != null)
                    Cacu_Grid_Set(); //배송 그리드
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

            else if (bt.Name == "butt_Item_Del")
            {
                //if (txt_SalesItemIndex.Text == "") return;

                //if (txt_OrderNumber.Text != "")  //주문번호가 존재한다.
                //{
                //    cls_Search_DB csd = new cls_Search_DB();
                //    //재고 관련해서 출고가 된내역인지 확인한다 출고 되었으면 삭제 되면 안됨.
                //    if (csd.Check_Stock_OutPut(txt_OrderNumber.Text.Trim(), int.Parse(txt_SalesItemIndex.Text.Trim())) == false)
                //    {
                //        butt_Item_Del.Focus(); return;
                //    }
                //}

               
                Base_Sub_Clear("Rece");
                Base_Sub_Sum_Item();
                Base_Sub_Sum_Cacu();
            }


            else if (bt.Name == "butt_Item_Save")
            {
                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                //if (Item_Rece_Error_Check__01("item") == false) return;

                //if (txt_SalesItemIndex.Text == "") //추가 일경우에 새로운 입력
                //{
                //    Base_Sub_Save_Item();
                //    Base_Sub_Clear("Rece");
                //    Base_Sub_Sum_Item();
                //    Base_Sub_Sum_Cacu();
                //}
                //else  //
                //{                   
                //    Base_Sub_Clear("Rece");
                //    Base_Sub_Sum_Item();
                //    Base_Sub_Sum_Cacu();
                //}
            }



            else if (bt.Name == "butt_Rec_Clear")
            {
                Base_Sub_Clear("Rece");
            }

           


            else if (bt.Name == "butt_Rec_Save")
            {

                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크


                    cls_form_Meth ct = new cls_form_Meth();
                                   
                    Base_Sub_Clear("Rece");
                    Base_Sub_Clear("item");

                    //MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
                    //            + "\n" +
                    //cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
              
            }


            else if (bt.Name == "butt_Cacu_Clear")
            {
                Base_Sub_Clear("Cacu");
            }



            else if (bt.Name == "butt_Cacu_Save")
            {
                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

            }
            //    if (txt_C_index.Text == "") //추가 일경우에 새로운 입력
            //    {
            //        if (double.Parse(txt_Price_1.Text.Trim().Replace(",", "")) > 0)  //현금이다
            //            Base_Sub_Save_Cacu(1);

            //        if (double.Parse(txt_Price_2.Text.Trim().Replace(",", "")) > 0)  //무통장이다
            //            Base_Sub_Save_Cacu(2);

            //        if (double.Parse(txt_Price_3.Text.Trim().Replace(",", "")) > 0)  //카드이다
            //            Base_Sub_Save_Cacu(3);

            //        Base_Sub_Clear("Cacu");
            //        Base_Sub_Sum_Cacu();

            //        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
            //                    + "\n" +
            //        cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
            //    }
            //    else  //
            //    {
               
            //        Base_Sub_Clear("Cacu");
            //        Base_Sub_Sum_Cacu();

            //        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit")
            //                     + "\n" +
            //        cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
            //    }
            //}

    
        }


        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
            //SendKeys.Send("{TAB}");
        }

   
  
        
        private Boolean Check_TextBox_Error()
        {
            //주문종류 , 회원, 주문일자 입력 안햇는지 체크
            if (Base_Error_Check__01() == false) return false;                        

            //회원번호 관련 관련 오류 체크 및 존재 여부 그리고 탈퇴 여부(신규 저장일 경우에)                      
            if (Input_Error_Check(mtxtMbid, "m",1) == false) return false;                                            
            
            if (Input_Error_Check_Save() == false) return false;
             
            return true;
        }



        private bool Input_Error_Check_Save()
        {

            int idx_ReturnTF = 0;
            int idx_SellTF = -1; 

            if (txt_OrderNumber_R.Text == "")
            {
                idx_ReturnTF = SalesDetail[txt_OrderNumber.Text.Trim()].ReturnTF;
                idx_SellTF = SalesDetail[txt_OrderNumber.Text.Trim()].SellTF  ;
            }
            else
                idx_ReturnTF = SalesDetail[txt_OrderNumber_R.Text.Trim()].ReturnTF;

            ////if (idx_SellTF == 0 )
            ////{
            ////    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate_ReDate_Er")
            ////                 + "\n" +
            ////                 cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            ////    mtxtSellDateRe.Focus(); return false;
            ////}

            if (int.Parse(txtSellDate.Text.Replace("-", "").Trim()) > int.Parse(mtxtSellDateRe.Text.Replace("-", "").Trim()))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate_ReDate_Er")
                             + "\n" +
                             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }


            if (mtxtSellDateRe.Text.Replace("-", "").Trim() != "" && mtxtSellDate3.Text.Replace("-", "").Trim() == "")
            {
                mtxtSellDate3.Text = mtxtSellDateRe.Text;
            }




            if (idx_ReturnTF == 3) //부분반품 관련해서 이화면에서 아무런 처리 못되게 한다.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_3")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }

            if (idx_ReturnTF == 4) //교환 관련해서 이화면에서 아무런 처리 못되게 한다.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_4")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }

            if (idx_ReturnTF == 5) //취소처리된 내역은 처리 못한다.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_5")
                        + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSellDateRe.Focus(); return false;
            }

            cls_Search_DB csd = new cls_Search_DB();

            //if (idx_ReturnTF == 1 || idx_ReturnTF == 4)  //왜 여기에 4가 잇을까 이게 몹시 궁금함 ㅠㅠ
            if (idx_ReturnTF == 1 )
            {                               

                //반품일자로 입력한 날짜가 마감 적용된 날짜인지 확인한다.
                if (csd.Close_Check_SellDate("tbl_CloseTotal_02", mtxtSellDate3.Text.Replace("-", "").Trim()) == false)
                {
                    mtxtSellDate3.Focus(); return false;
                }

                //이미 이건으로 해서 반품한 내역이 있는지를 체크한다. 교환이나 부분반품한 내역이 있는지
                foreach (string  t_key in SalesDetail.Keys)
                {
                    if (SalesDetail[t_key].Del_TF != "D")
                    {
                        if (SalesDetail[t_key].Re_BaseOrderNumber == txt_OrderNumber.Text.Trim())
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Del_3_4")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                            mtxtSellDateRe.Focus();
                            return false;
                        }
                    }
                }

                if (dGridView_Base_Item.RowCount == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Goods")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));                    
                }

            }
            else if (idx_ReturnTF == 2)  //반품인데 수정을 한다.
            {
                string Be_SellDate = SalesDetail[txt_OrderNumber_R.Text.Trim()].SellDate_2.Replace("-", "");

                if (Be_SellDate != mtxtSellDate3.Text.Replace("-", "").Trim())
                {
                    if (csd.Close_Check_SellDate("tbl_CloseTotal_02", Be_SellDate) == false)
                    {
                        mtxtSellDate3.Focus(); return false;
                    }

                    if (csd.Close_Check_SellDate("tbl_CloseTotal_02", mtxtSellDate3.Text.Replace("-","").Trim()) == false)
                    {
                        mtxtSellDate3.Focus(); return false;
                    }
                }
            }
                        
            return true;
        }









        private void Input_SalesDetail_dic()
        {
            cls_form_Meth ct = new cls_form_Meth();

            double Total_Sell_VAT_Price = 0; double Total_Sell_Except_VAT_Price = 0;
            
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    Total_Sell_VAT_Price = Total_Sell_VAT_Price + SalesItemDetail[t_key].Total_Sell_VAT_Price;
                    Total_Sell_Except_VAT_Price = Total_Sell_Except_VAT_Price + SalesItemDetail[t_key].Total_Sell_Except_VAT_Price;
                }
            }            
            
            cls_Sell t_c_sell = new cls_Sell();

            //기존주문주문번호에다가 앞에 RR을 붙여서 전체 반품이라는 표식을 하고 주문번호를 만든다.
            t_c_sell.OrderNumber = "";
            
            t_c_sell.Mbid = idx_Mbid ;
            t_c_sell.Mbid2 = idx_Mbid2;
            t_c_sell.M_Name = txtName.Text.Trim();
            
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

            t_c_sell.SellSort = SalesDetail[txt_OrderNumber.Text.Trim()].SellSort; 
            t_c_sell.Re_BaseOrderNumber = txt_OrderNumber.Text.Trim(); //원 주문 관련 주문번호를 넣는다.

            t_c_sell.TotalPrice = - double.Parse(txt_TotalPrice.Text.Trim().Replace(",","") );
            t_c_sell.TotalPV = - double.Parse(txt_TotalPv.Text.Trim().Replace(",", ""));
            t_c_sell.TotalCV = -double.Parse(txt_TotalBv.Text.Trim().Replace(",", ""));
            t_c_sell.TotalInputPrice = 0;
            t_c_sell.Total_Sell_VAT_Price = - Total_Sell_VAT_Price;
            t_c_sell.Total_Sell_Except_VAT_Price = - Total_Sell_Except_VAT_Price;
            t_c_sell.InputCash = -SalesDetail[txt_OrderNumber.Text.Trim()].InputCash;
            t_c_sell.InputCard = -SalesDetail[txt_OrderNumber.Text.Trim()].InputCard;
            t_c_sell.InputPassbook = -SalesDetail[txt_OrderNumber.Text.Trim()].InputPassbook;
            t_c_sell.InputPassbook_2 = -SalesDetail[txt_OrderNumber.Text.Trim()].InputPassbook_2;
            t_c_sell.InputMile = - SalesDetail[txt_OrderNumber.Text.Trim()].InputMile ;  //현금이나 무통장 같은 경우와는 다르게 마일리지 같은 경우에는 바로 마일리지 테이블에 바로 + 되므로 여기서도 바로 - 넣는다.
            t_c_sell.InputPass_Pay = - SalesDetail[txt_OrderNumber.Text.Trim()].InputPass_Pay ;
            t_c_sell.UnaccMoney = double.Parse(txt_UnaccMoney.Text.Trim().Replace(",", ""));
            
            t_c_sell.Etc1 = txt_ETC1_R.Text.Trim();
            t_c_sell.Etc2 = txt_ETC2_R.Text.Trim();

            t_c_sell.Na_Code = SalesDetail[txt_OrderNumber.Text.Trim()].Na_Code;  //국가코드도 바로 넣어버린다.

            t_c_sell.ReturnTF = 2;
            t_c_sell.ReturnTFName = ct._chang_base_caption_search("반품");
            t_c_sell.INS_Num = "";
            t_c_sell.InsuranceNumber_Date = "";
            t_c_sell.W_T_TF = 0;
            t_c_sell.In_Cnt = 0;

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";
                                
            t_c_sell.SellDate = mtxtSellDateRe.Text.Replace("-","") .Trim();
            t_c_sell.SellDate_2 = mtxtSellDate3.Text.Replace("-", "").Trim();

            t_c_sell.Del_TF = "S";

            SalesDetail[""] = t_c_sell;
        }


        private void Update_SalesDetail_dic()
        {
            string OrderNumber = txt_OrderNumber_R.Text.Trim();           
                                    
            SalesDetail[OrderNumber].Etc1 = txt_ETC1_R.Text.Trim();
            SalesDetail[OrderNumber].Etc2 = txt_ETC2_R.Text.Trim();
            SalesDetail[OrderNumber].SellDate = mtxtSellDateRe.Text.Replace("-", "").Trim();
            SalesDetail[OrderNumber].SellDate_2 = mtxtSellDate3.Text.Replace("-", "").Trim();
            if (SalesDetail[OrderNumber].Del_TF == "")
                SalesDetail[OrderNumber].Del_TF = "U";

        }

        private void DB_Save_tbl_SalesDetail( ref string T_ord_N)
        {         
            if (txt_OrderNumber_R.Text.Trim() != "")
                T_ord_N = txt_OrderNumber_R.Text.Trim();
            else
            {
                SalesDetail[T_ord_N].OrderNumber = "RR" + txt_OrderNumber.Text.Trim();
                T_ord_N = "RR" + txt_OrderNumber.Text.Trim();
                //++++++++++++++++++++++++++++++++
            }
        }




        private void DB_Save_tbl_SalesDetail____002(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran,  string OrderNumber)
        {
            string StrSql = "";
            if (txt_OrderNumber_R.Text.Trim() == "")
            {
                string Ins_Ordernumber = "";

                StrSql = "INSERT INTO tbl_SalesDetail" ;
                StrSql = StrSql + " (OrderNumber,Mbid,Mbid2,M_Name,SellDate,SellDate_2,SellCode, SellSort ,BusCode,";
                StrSql = StrSql + " TotalPrice,TotalPV,TotalCV,TotalInputPrice,";
                StrSql = StrSql + " Total_Sell_VAT_Price,Total_Sell_Except_VAT_Price, ";
                StrSql = StrSql + " InputCash,InputCard,InputPassbook,InputPassbook_2, InputMile,UnaccMoney, InputPass_Pay ,";
                StrSql = StrSql + " Etc1,Etc2, ";
                StrSql = StrSql + " ReturnTF,InsuranceNumber,InsuranceNumber_Date, ";
                StrSql = StrSql + " Re_BaseOrderNumber , Na_Code, ";
                StrSql = StrSql + " RecordID,RecordTime";

                StrSql = StrSql + " ) Values ( ";
                StrSql = StrSql + "'" + SalesDetail[Ins_Ordernumber].OrderNumber + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].Mbid + "'";
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].Mbid2;
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].M_Name + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellDate + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellDate_2 + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellCode + "'";
                StrSql = StrSql + ",'" + SalesDetail[Ins_Ordernumber].SellSort  + "'";
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
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPassbook_2;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputMile;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].UnaccMoney;
                StrSql = StrSql + "," + SalesDetail[Ins_Ordernumber].InputPass_Pay;
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
            else
            {
                cls_Search_DB csd = new cls_Search_DB();

                //수정하기 전에 배열에다가 내역을 받아둔다.
                csd.SalesDetail_Mod_BackUp(OrderNumber, "tbl_SalesDetail");


                StrSql = "Update tbl_SalesDetail Set ";
                StrSql = StrSql + " SellDate = '" + SalesDetail[OrderNumber].SellDate.Replace("-","")   + "'";
                StrSql = StrSql + ",SellDate_2 = '" + SalesDetail[OrderNumber].SellDate_2.Replace("-", "") + "'";
                StrSql = StrSql + ",TotalPrice = " + SalesDetail[OrderNumber].TotalPrice ;
                StrSql = StrSql + ",TotalPV= " + SalesDetail[OrderNumber].TotalPV;
                StrSql = StrSql + ",TotalcV= " + SalesDetail[OrderNumber].TotalCV;
                StrSql = StrSql + ",TotalInputPrice= " + SalesDetail[OrderNumber].TotalInputPrice;

                StrSql = StrSql + ",Total_Sell_VAT_Price= " + SalesDetail[OrderNumber].Total_Sell_VAT_Price;
                StrSql = StrSql + ",Total_Sell_Except_VAT_Price= " + SalesDetail[OrderNumber].Total_Sell_Except_VAT_Price;

                StrSql = StrSql + ",InputCash= " + SalesDetail[OrderNumber].InputCash;
                StrSql = StrSql + ",InputCard= " + SalesDetail[OrderNumber].InputCard;
                StrSql = StrSql + ",InputPassbook= " + SalesDetail[OrderNumber].InputPassbook;
                StrSql = StrSql + ",InputMile= " + SalesDetail[OrderNumber].InputMile;
                StrSql = StrSql + ",UnaccMoney= " + SalesDetail[OrderNumber].UnaccMoney;

                StrSql = StrSql + ",Etc1= '" + SalesDetail[OrderNumber].Etc1 + "'";
                StrSql = StrSql + ",Etc2= '" + SalesDetail[OrderNumber].Etc2 + "'";

                StrSql = StrSql + " Where OrderNumber = '" + SalesDetail[OrderNumber].OrderNumber  + "'";

                if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

                //주테이블의 변경 내역을 테이블에 넣는다.
                csd.SalesDetail_Mod(Conn, tran,OrderNumber, "tbl_SalesDetail");
            }
        }



        private void DB_Save_tbl_SalesItemDetail(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, 
                    string OrderNumber)
        {           
            //아이템을 환불처리한다.
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D") 
                {                  
                    DB_Save_tbl_SalesItemDetail____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
            }

            //결제를 환불처리한다.
            //..2018-09-03 지성경 < 환불을 막습니다요 > 
            //////foreach (int t_key in Sales_Cacu.Keys)
            //////{
            //////    if (Sales_Cacu[t_key].Del_TF != "D")
            //////    {
            //////        DB_Save_tbl_Sales_Cacu____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
            //////    }
            //////}
        }



        private void DB_Save_tbl_SalesItemDetail____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int SalesItemIndex)
        {
            string StrSql = "";
            
            cls_form_Meth ct = new cls_form_Meth();
            string SellState = "R_1";

            StrSql = "Insert Into tbl_SalesitemDetail (";            
            StrSql = StrSql + " SalesItemIndex,OrderNumber,";
            StrSql = StrSql + " ItemCode,ItemPrice,ItemPv,ItemCv,";
            StrSql = StrSql + " Sell_VAT_TF , Sell_VAT_Price, Sell_Except_VAT_Price,SellState,";
            StrSql = StrSql + " ItemCount,ItemTotalPrice,ItemTotalPV,ItemTotalcV,";
            StrSql = StrSql + " Total_Sell_VAT_Price, Total_Sell_Except_VAT_Price,";
            StrSql = StrSql + " ReturnDate,SendDate,ReturnBackDate,";
            StrSql = StrSql + " Etc,RecIndex,";                    
             StrSql = StrSql + " Send_itemCount1,Send_itemCount2, ";
            StrSql = StrSql + " T_OrderNumber1,T_OrderNumber2,G_Sort_Code ";
            StrSql = StrSql + " ,RecordID,RecordTime ";
            StrSql = StrSql + " ) values("  ;

            StrSql = StrSql +  SalesItemDetail[SalesItemIndex].SalesItemIndex ;
            StrSql = StrSql + ",'" + OrderNumber + "'";

            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].ItemCode + "'";
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemPrice;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemPV;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemCV;
            StrSql = StrSql + "," +  SalesItemDetail[SalesItemIndex].Sell_VAT_TF;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].Sell_VAT_Price;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].Sell_Except_VAT_Price;

            StrSql = StrSql + ",'" + SellState + "'";

            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemCount;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemTotalPrice;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemTotalPV;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].ItemTotalCV;

            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].Total_Sell_VAT_Price;
            StrSql = StrSql + "," + - SalesItemDetail[SalesItemIndex].Total_Sell_Except_VAT_Price;

            StrSql = StrSql + ",''";
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",''";

            StrSql = StrSql + ",''";
            StrSql = StrSql + ",0";

            StrSql = StrSql + ",0 " ;
            StrSql = StrSql + ",0 " ;

            StrSql = StrSql + ",'" + txt_OrderNumber.Text.Trim() + "'";
            StrSql = StrSql + ",'" + txt_OrderNumber.Text.Trim() + "'";
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].RecordID + "'";
            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";
                        
            if (Temp_Connect.Insert_Data(StrSql,"tbl_SalesItemDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;
           
        }



        private void DB_Save_tbl_Sales_Cacu____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string OrderNumber, int C_index)
        {
            string StrSql = "";


            StrSql = "Insert Into tbl_Sales_Cacu (";
            StrSql = StrSql + " C_index,OrderNumber,";
            StrSql = StrSql + " C_TF,C_Code,C_CodeName,C_Name1,C_Name2,";
            StrSql = StrSql + " C_Number1 , C_Number2, C_Number3, ";
            StrSql = StrSql + " C_Price1,C_Price2,C_AppDate1,C_AppDate2, ";
            StrSql = StrSql + " C_CancelTF, C_CancelDate,C_CancelPrice, ";
            StrSql = StrSql + " C_Period1,C_Period2,C_Installment_Period,C_Etc";
            StrSql = StrSql + " ,RecordID,RecordTime ";
            StrSql = StrSql + " ) values(";

            StrSql = StrSql + "" + Sales_Cacu[C_index].C_index;
            StrSql = StrSql + ",'" + OrderNumber + "'";
            StrSql = StrSql + "," + Sales_Cacu[C_index].C_TF;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Code + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_CodeName + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Name1 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Name2 + "'";

            StrSql = StrSql + ",'" + encrypter.Encrypt(Sales_Cacu[C_index].C_Number1) + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Number2 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Number3 + "'";

            StrSql = StrSql + "," + - Sales_Cacu[C_index].C_Price1;
            StrSql = StrSql + "," + - Sales_Cacu[C_index].C_Price2;

            StrSql = StrSql + ",'" + mtxtSellDateRe.Text.Replace("-", "").Trim() + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_AppDate2.Replace("-", "") + "'";

            StrSql = StrSql + ",1 " ;
            StrSql = StrSql + ",'" + mtxtSellDateRe.Text.Replace ("-","").Trim () + "'";
            StrSql = StrSql + "," + - Sales_Cacu[C_index].C_Price1;

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Period1 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Period2 + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Installment_Period + "'";
            StrSql = StrSql + ",'" + Sales_Cacu[C_index].C_Etc + "'";

            StrSql = StrSql + ",'" + Sales_Cacu[C_index].RecordID + "'";

            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Sales_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;

        }






        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            string str_Q = "";

            if (txt_OrderNumber_R.Text == "")            
                str_Q = "Msg_Base_Save_Q";
            else            
                str_Q = "Msg_Base_Edit_Q";
                            
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(str_Q), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (Check_TextBox_Error() == false) return;

            if (txt_OrderNumber_R.Text.Trim() == "")
            {

                //cls_Cash_Card_Admin_Cancel cccA = new cls_Cash_Card_Admin_Cancel();
                //int ret_C1 = cccA.Cash_Card_Send_Singo_Cancel(txt_OrderNumber.Text, mtxtMbid.Text.Trim(), "Cash");
                //if (ret_C1 == 1)
                //{
                //    MessageBox.Show("현금 영수증 취소중에 문제가 발생했습니다. 반품 처리는 더이상 진행 돼지 않습니다. 업체에 문의해 주십시요.");
                //    return;
                //}

                //if (ret_C1 == 100)
                //{
                //    MessageBox.Show("현금 영수증 취소중에 문제가 발생했습니다. 반품 처리는 더이상 진행 돼지 않습니다.");
                //    return;
                //}


                //ret_C1 = cccA.Cash_Card_Send_Singo_Cancel(txt_OrderNumber.Text, mtxtMbid.Text.Trim(), "Card", 0, "R");

                //if (ret_C1 >= 1)
                //{
                //    MessageBox.Show("웹 카드 승인 내역 취소중에 문제가 발생했습니다. 반품 처리는 더이상 진행 돼지 않습니다. 업체에 문의해 주십시요.");
                //    return;
                //}


                //if (ret_C1 == 100)
                //{
                //    MessageBox.Show("웹 카드 승인 내역 취소중에 문제가 발생했습니다. 반품 처리는 더이상 진행 돼지 않습니다.");
                //    return;
                //}

                Input_SalesDetail_dic();   //판매 주 클래스 에 넣음 주문번호 ""으로 해서
            }
            else
                Update_SalesDetail_dic();  //판매 주 클래스에 대한 수정 작업

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


                if (txt_OrderNumber_R.Text.Trim() == "")
                {
                    DB_Save_tbl_SalesItemDetail(Temp_Connect, Conn, tran, T_ord_N); //상품과 결제의 환불까지 처리한다
                }

                Input_Mileage_Table_CanCel(Temp_Connect, Conn, tran);


               

                tran.Commit();

                Save_Error_Check = 1;
                if (txt_OrderNumber_R.Text == "")
                {
                    string StrSql = "";
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        //cls_Web Cls_Web = new cls_Web();
                        //string SuccessYN = "";
                        //SuccessYN = "";
                        //string ErrMessage = "";
                        //string SuccessYN_Card = "N";
                        //if (SuccessYN_Card == "N")
                        //{

                        //    SuccessYN_Card = Cls_Web.TH_SMS(idx_Mbid2, txt_OrderNumber.Text.Trim(), 6, ref ErrMessage);

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
                        StrSql = "EXEC [Usp_TH_SMS]   " + idx_Mbid2 + ",'" + txt_OrderNumber.Text + "','','6'";  //주문반품등록
                    }
                    else
                    {
                      
                        StrSql = "EXEC Usp_Insert_SMS_New  '21',''," + idx_Mbid2 + ",'" + txt_OrderNumber.Text.Trim() + "', ''";  //반품처리되엇다고 알림톡을 보낸다 원주문번호를 보낸다 알림톡에
                                                                                                                                  //StrSql = "EXEC Usp_Insert_SMS '21',''," + idx_Mbid2 + ",'" + txt_OrderNumber.Text.Trim() + "', ''";  //반품처리되엇다고 알림톡을 보낸다 원주문번호를 보낸다 알림톡에
                        Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text);
                    }

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                }
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }
            catch (Exception)
            {
                tran.Rollback();
                if (txt_OrderNumber_R.Text == "")
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();

                //직판 관련 조합 취소를 먼저 한다.. 이루어 지지 않았을 경우에는 메시지를 뛰우고 현 프로그램에서는 취소 과정을 그대로 진행한다.
                if (Save_Error_Check == 1 && cls_app_static_var.Sell_Union_Flag == "D")
                {
                    Cancel_InsurancerNumber(); //직판 관련 매출 취소가 이루어진다. 취소가 되든 안되든 우리쪽 프로그램에서는 취소를 시킨다..  직판 오류지 알아서 하라고함.                   
                }
            }          
        }

        private void Cancel_InsurancerNumber()
        {
            string strSql = "SELECT * FROM [ufn_Get_Cancel_InsNum_Data] ('" + txt_OrderNumber.Text.Trim() + "')";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            cls_Socket csg = new cls_Socket();
            string Req = csg.Dir_Connect_Send_Cancel(txt_OrderNumber.Text);

            if (Req != "Y")
            {
                MessageBox.Show("조합 관련 Error 입니다. 주문취소는 이루어 지지만 조합관련 취소는 별도로 신청 하셔야 합니다.");
            }
            else
            {
                MessageBox.Show("공제번호가 정상적으로 취소 되었습니다.");
            }


        }

        private void Input_Mileage_Table_CanCel(cls_Connect_DB Temp_Connect,
                                            SqlConnection Conn, SqlTransaction tran)
        {
            if (SalesDetail[txt_OrderNumber.Text].InputMile > 0)
            {
                string OrderNumber = txt_OrderNumber.Text;

                cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                ctm.Put_Plus_Mileage(SalesDetail[OrderNumber].Mbid, SalesDetail[OrderNumber].Mbid2, SalesDetail[OrderNumber].M_Name
                    , SalesDetail[OrderNumber].InputMile, SalesDetail[OrderNumber].OrderNumber, "32"
                    , Temp_Connect, Conn, tran, "", this.Name.ToString(), this.Text);
            }
        }

        private void Input_Mileage_Table_CanCel_Cancel (cls_Connect_DB Temp_Connect,
                                            SqlConnection Conn, SqlTransaction tran)
        {
            //반품의 원주문으로 가서 그 내역에 마일리지 내역으로 결제한게 있다. 그럼 반품할때 + 되었으니 이번에는 다시  - 시켜준다.
            if (SalesDetail[txt_OrderNumber.Text].InputMile > 0)
            {
                string OrderNumber = txt_OrderNumber.Text;

                cls_tbl_Mileage ctm = new cls_tbl_Mileage();
                ctm.Put_Minus_Mileage (SalesDetail[OrderNumber].Mbid, SalesDetail[OrderNumber].Mbid2, SalesDetail[OrderNumber].M_Name
                    , SalesDetail[OrderNumber].InputMile, SalesDetail[OrderNumber].OrderNumber, "34"
                    , Temp_Connect, Conn, tran, "", this.Name.ToString(), this.Text);
            }
        }



        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            Base_Ord_Clear();

            if ((sender as DataGridView).CurrentRow.Cells[2].Value != null)
            {
                string OrderNumber = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();

                if (OrderNumber != "")
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
                }
            }
        }


        private void Set_SalesDetail(string OrderNumber)
        {
            int idx_ReturnTF = SalesDetail[OrderNumber].ReturnTF;

            Data_Set_Form_TF = 1;

            if (idx_ReturnTF == 3 || idx_ReturnTF == 1 || idx_ReturnTF == 5)
            {
                txtSellDate.Text = SalesDetail[OrderNumber].SellDate.Replace("-", "");
                txtSellDate2.Text = SalesDetail[OrderNumber].SellDate_2.Replace("-", "");
                txtSellCode.Text = SalesDetail[OrderNumber].SellCodeName;
                txtSellCode_Code.Text = SalesDetail[OrderNumber].SellCode;
                txtCenter2.Text = SalesDetail[OrderNumber].BusCodeName;
                txtCenter2_Code.Text = SalesDetail[OrderNumber].BusCode;
                //string.Format(cls_app_static_var.str_Currency_Type, ds.Tables[base_db_name].Rows[0]["Last_price"]);
                txt_OrderNumber.Text = OrderNumber;
                txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPrice);
                txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPV);
                txt_TotalBv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalCV);

                txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);
                txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].UnaccMoney);

                txt_ETC1.Text = SalesDetail[OrderNumber].Etc1;
                txt_ETC2.Text = SalesDetail[OrderNumber].Etc2;
            }
            else if (idx_ReturnTF == 2 )
            {
                string Re_BaseOrderNumber = SalesDetail[OrderNumber].Re_BaseOrderNumber.Trim();
                txtSellDate.Text = SalesDetail[Re_BaseOrderNumber].SellDate.Replace("-", "");
                txtSellDate2.Text = SalesDetail[Re_BaseOrderNumber].SellDate_2.Replace("-", "");
                txtSellCode.Text = SalesDetail[Re_BaseOrderNumber].SellCodeName;
                txtSellCode_Code.Text = SalesDetail[Re_BaseOrderNumber].SellCode;
                txtCenter2.Text = SalesDetail[Re_BaseOrderNumber].BusCodeName;
                txtCenter2_Code.Text = SalesDetail[Re_BaseOrderNumber].BusCode;                
                txt_OrderNumber.Text = Re_BaseOrderNumber;
                txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalPrice);
                txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalPV);
                txt_TotalBv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalCV);

                txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalInputPrice);
                txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].UnaccMoney);

                txt_ETC1.Text = SalesDetail[Re_BaseOrderNumber].Etc1;
                txt_ETC2.Text = SalesDetail[Re_BaseOrderNumber].Etc2;



                mtxtSellDateRe.Text = SalesDetail[OrderNumber].SellDate.Replace("-", "");
                mtxtSellDate3.Text = SalesDetail[OrderNumber].SellDate_2.Replace("-", "");
                txt_OrderNumber_R.Text = OrderNumber;
                txt_TotalPrice_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPrice);
                txt_TotalPv_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPV);
                txt_TotalBv_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalCV);

                //txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);
                //txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].UnaccMoney);

                txt_ETC1_R.Text = SalesDetail[OrderNumber].Etc1;
                txt_ETC2_R.Text = SalesDetail[OrderNumber].Etc2;
            }

            Data_Set_Form_TF = 0;
        }




        private void Set_SalesItemDetail(string OrderNumber )
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
            strSql = strSql + " Where tbl_SalesitemDetail.OrderNumber = '" + OrderNumber.ToString() +"'" ;
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
                t_c_sell.Get_Address1 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["Get_Address1"].ToString());
                t_c_sell.Get_Address2 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["Get_Address2"].ToString());

                t_c_sell.Get_Tel1 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["Get_Tel1"].ToString());
                t_c_sell.Get_Tel2 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["Get_Tel2"].ToString());

                t_c_sell.Pass_Number = ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Number"].ToString();
                t_c_sell.Pass_Pay = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Pay"].ToString());
                                
                t_c_sell.Pass_Number2 = ds.Tables[base_db_name].Rows[fi_cnt]["Pass_Number2"].ToString();                
                t_c_sell.Base_Rec = ds.Tables[base_db_name].Rows[fi_cnt]["Base_Rec"].ToString();
                t_c_sell.Base_Rec_Name = ds.Tables[base_db_name].Rows[fi_cnt]["Base_Rec_Name"].ToString();

                t_c_sell.Get_Etc1 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Etc1"].ToString();
                t_c_sell.Get_Etc2 = ds.Tables[base_db_name].Rows[fi_cnt]["Get_Etc2"].ToString();

                if (cls_User.gid_CountryCode == "TH")
                {
                    t_c_sell.Get_city = ds.Tables[base_db_name].Rows[fi_cnt]["Get_city"].ToString();
                    t_c_sell.Get_state = ds.Tables[base_db_name].Rows[fi_cnt]["Get_state"].ToString();
                }
                
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




        private void Set_Sales_Cacu(string OrderNumber)
        {

            string strSql = "";

            strSql = "Select tbl_Sales_Cacu.* ";
            strSql = strSql + " , Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " C_TF_Name ";
            strSql = strSql + " , Isnull(tbl_BankForCompany.BankPenName , '')  C_CodeName_2 ";
            strSql = strSql + " From tbl_Sales_Cacu (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail (nolock) ON tbl_SalesDetail.OrderNumber = tbl_Sales_Cacu.OrderNumber ";
            strSql = strSql + " LEFT JOIN tbl_BankForCompany (nolock) ON tbl_Sales_Cacu.C_Code = tbl_BankForCompany.BankCode And  tbl_Sales_Cacu.C_Number1 = tbl_BankForCompany.BankAccountNumber  And tbl_SalesDetail.Na_Code = tbl_BankForCompany.Na_Code ";
            strSql = strSql + " LEFT JOIN tbl_Base_Change_Detail Ch_T (nolock) ON Ch_T.M_Detail_S = 'tbl_Sales_Cacu' And  Ch_T.M_Detail = Convert(Varchar,tbl_Sales_Cacu.C_TF) ";
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
                t_c_sell.C_CodeName = ds.Tables[base_db_name].Rows[fi_cnt]["C_CodeName"].ToString();
                t_c_sell.C_CodeName_2 = ds.Tables[base_db_name].Rows[fi_cnt]["C_CodeName_2"].ToString();

                t_c_sell.C_Name1 = ds.Tables[base_db_name].Rows[fi_cnt]["C_Name1"].ToString();
                t_c_sell.C_Name2 = ds.Tables[base_db_name].Rows[fi_cnt]["C_Name2"].ToString();
                t_c_sell.C_Number1 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["C_Number1"].ToString());
                t_c_sell.C_Number2 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["C_Number2"].ToString());
                t_c_sell.C_Number3 = encrypter.Decrypt (ds.Tables[base_db_name].Rows[fi_cnt]["C_Number3"].ToString());

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
                t_c_sell.C_Etc = ds.Tables[base_db_name].Rows[fi_cnt]["C_Etc"].ToString();

                t_c_sell.C_Base_Index = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["C_Base_Index"].ToString());

                t_c_sell.RecordID = ds.Tables[base_db_name].Rows[fi_cnt]["RecordID"].ToString();
                t_c_sell.RecordTime = ds.Tables[base_db_name].Rows[fi_cnt]["RecordTime"].ToString();

                string t_sellDate = t_c_sell.C_AppDate1.Substring(0, 4);
                t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate1.Substring(4, 2);
                t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate1.Substring(6, 2);

                t_c_sell.C_AppDate1 = t_sellDate;

                if (t_c_sell.C_AppDate2 != "")
                {
                    t_sellDate = t_c_sell.C_AppDate2.Substring(0, 4);
                    t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate2.Substring(4, 2);
                    t_sellDate = t_sellDate + "-" + t_c_sell.C_AppDate2.Substring(6, 2);

                    t_c_sell.C_AppDate2 = t_sellDate;
                }




                t_c_sell.Del_TF = "";
                T_Sales_Cacu[t_c_sell.C_index] = t_c_sell;
            }

            Sales_Cacu = T_Sales_Cacu;
        }

























    }
}
