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
    public partial class frmSell_RC_03 : Form
    {

        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        public delegate void Take_NumberDele(ref string Send_Number, ref string Send_Name, ref string Send_OrderNumber);
        public event Take_NumberDele Take_Mem_Number;




        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Item = new cls_Grid_Base();


        cls_Grid_Base cgb_Rece = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece_Add = new cls_Grid_Base();

        private Dictionary<string , cls_Sell> SalesDetail ;
        private Dictionary<int, cls_Sell_Item> SalesItemDetail = new Dictionary<int, cls_Sell_Item>() ;
        private Dictionary<int, cls_Sell_Rece> Sales_Rece = new Dictionary<int, cls_Sell_Rece>();


        private Dictionary<string, TextBox>  Ncode_dic = new Dictionary<string, TextBox>();

        private const string base_db_name = "tbl_SalesDetail";
        private int Data_Set_Form_TF;        
        private string idx_Mbid = "";
        private int idx_Mbid2 = 0;

        public frmSell_RC_03()
        {
            InitializeComponent();
        }






        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;
            

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Item.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset(1);     
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                        
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            
            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;            
            mtxtSn.Mask = "999999-9999999"; //기본 셋팅은 주민번호이다.                                 
        }



        private void frmBase_Resize(object sender, EventArgs e)
        {

            int base_w = this.Width / 4;
            butt_Clear.Width = base_w;
            butt_Save.Width = base_w;

            butt_Delete.Width = base_w;
            butt_Exit.Width = base_w;

            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width;

            butt_Delete.Left = butt_Save.Left + butt_Save.Width;
            butt_Exit.Left = butt_Delete.Left + butt_Delete.Width;    
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
                    if (!this.Controls.ContainsKey("Popup_gr") && dGridView_Base_Rece_Add.Visible == false)
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

                              //  cls_form_Meth cfm = new cls_form_Meth();
                               // cfm.form_Group_Panel_Enable_True(this);
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

                // Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
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
            
            string strSql = "";
            cls_form_Meth cm = new cls_form_Meth();
            strSql = "Select tbl_SalesDetail.* ";
            strSql = strSql + " , tbl_Business.Name BusCodeName ";
            strSql = strSql + " , tbl_SellType.SellTypeName SellCodeName  ";

            strSql = strSql + " ,Case When ReturnTF = 1 Then '" + cm._chang_base_caption_search("정상") + "'";
            strSql = strSql + "  When ReturnTF = 2 Then '" + cm._chang_base_caption_search("반품") + "'";
            strSql = strSql + "  When ReturnTF = 4 Then '" + cm._chang_base_caption_search("교환") + "'";
            strSql = strSql + "  When ReturnTF = 3 Then '" + cm._chang_base_caption_search("부분반품") + "'";
            strSql = strSql + "  When ReturnTF = 5 Then '" + cm._chang_base_caption_search("취소") + "'";
            strSql = strSql + " END ReturnTFName ";


            strSql = strSql + " , Isnull(tbl_SalesDetail_TF.SellTF, 0 )  SellTF ";
            strSql = strSql + " ,Case When tbl_SalesDetail_TF.SellTF = 1 Then '" + cm._chang_base_caption_search("승인") + "'";
            strSql = strSql + "  When tbl_SalesDetail_TF.SellTF = 0 Then '" + cm._chang_base_caption_search("미승인") + "'";
            strSql = strSql + " ELSE '' ";
            strSql = strSql + " END SellTFName ";

            if (cls_app_static_var.Sell_Union_Flag == "U")  //특판
            {
                strSql = strSql + " , Case When  tbl_SalesDetail.union_Seq > 0 And T_REALMLM.ERRCODE = '0000' Then ISNULL(T_REALMLM.GUARANTE_NUM,'') ";
                strSql = strSql + "        When  tbl_SalesDetail.union_Seq > 0 And T_REALMLM.ERRCODE <> '0000' Then  ISNULL(T_REALMLM_ErrCode.Er_Msg ,'' ) ";
                strSql = strSql + "        When  tbl_SalesDetail.union_Seq = 0 Then '미신고'  ";
                strSql = strSql + "   End InsuranceNumber2 ";
            }
            else if (cls_app_static_var.Sell_Union_Flag == "D")  //직판
            {
                strSql = strSql + ", Case When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail AS A1 Where tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NULL And tbl_SalesDetail.InsuranceNumber <> '' Then tbl_SalesDetail.InsuranceNumber ";
                strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail AS A1 Where tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NOT NULL And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                strSql = strSql + " When  ReturnTF = 5 And InsuranceNumber_Cancel ='Y' Then tbl_SalesDetail.InsuranceNumber + '(취소상태)' ";
                strSql = strSql + " When  ReturnTF = 1 And (Select A1.SellDate From tbl_SalesDetail AS A1 Where tbl_SalesDetail.OrderNumber = A1.Re_BaseOrderNumber) IS NOT NULL And InsuranceNumber_Cancel ='' Then tbl_SalesDetail.InsuranceNumber + '(취소요청중)' ";
                strSql = strSql + " When  ReturnTF = 1 And tbl_SalesDetail.InsuranceNumber = '' Then '미승인요청' ";
                strSql = strSql + " ELSE tbl_SalesDetail.InsuranceNumber END  InsuranceNumber2 ";
            }
            else
            {
                strSql = strSql + " , InsuranceNumber As InsuranceNumber2 ";
            }


            strSql = strSql + " From tbl_SalesDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail_TF (nolock) ON tbl_SalesDetail.OrderNumber = tbl_SalesDetail_TF.OrderNumber ";
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

            // strSql = strSql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            strSql = strSql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

            //strSql = strSql + " And tbl_SalesDetail.ReturnTF = 2 ";   //---반품한 내역만 불러온다.
            //처음에는 반품한 내역만 불어 오기로 했으나 우선을 매출 다 불러오고 그리드 상에서만 반품 내역만 
            //보여지게 해주는게 더 낳을듯 함

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
            double S_cnt4 = 0; double S_cnt5 = 0; double S_cnt6 = 0;
            double Sum_13 = 0; double Sum_14 = 0; double Sum_15 = 0; ; double Sum_16 = 0;
            foreach (string t_key in SalesDetail.Keys)
            {
                if (SalesDetail[t_key].Del_TF != "D" && ( SalesDetail[t_key].ReturnTF == 4))
                {
                    Set_gr_dic(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.

                    S_cnt4 = S_cnt4 + SalesDetail[t_key].TotalPrice;
                    S_cnt5 = S_cnt5 + SalesDetail[t_key].TotalInputPrice;
                    S_cnt6 = S_cnt6 + SalesDetail[t_key].TotalPV;

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
            cgb.grid_col_Count = 15;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"승인여부" , "공제번호"  , "주문번호"   , "주문일자"  , "총주문액"       
                                    , "총입금액"      , "총PV"    , "주문종류"   , "구분"      , "현금" 
                                    ,"카드" ,"무통장", "미결제"  ,  "기록자" ,  "기록일"
                                };

           

            if (cls_app_static_var.Sell_Union_Flag == "")  //직판특판이 아닌경우 공제번호 필드 안나오게
            {
                int[] g_Width = { 80,0, 120, 80, 80
                              , 80  ,80 , 80 , 80 , 80
                              , 80  ,80 ,80,80 ,80
                            };
                cgb.grid_col_w = g_Width;
            }
            else
            {

                int[] g_Width = { 80,120, 120, 80, 80
                              , 80  ,80 , 80 , 80 , 80
                              , 80  ,80 ,80,80 ,80
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
                                ,DataGridViewContentAlignment.MiddleCenter                                  
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleRight  //10

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

            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[12 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[13 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb.grid_col_header_text = g_HeaderText;
            cgb.grid_cell_format = gr_dic_cell_format;
            
            cgb.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true 
                                    ,true , true,  true       , true,  true                  
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
                return;
            }
            
            int fi_cnt = 0; double T_Pv = 0; double T_pr = 0;

            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D")
                {
                    T_Pv = T_Pv + SalesItemDetail[t_key].ItemTotalPV  ;
                    T_pr = T_pr + SalesItemDetail[t_key].ItemTotalPrice  ;
                }
                fi_cnt++;
            }

            txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, T_pr);
            txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, T_Pv);
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
                if (SalesItemDetail[t_key].Del_TF != "D" && SalesItemDetail[t_key].ItemCount > 0)
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

                                ,SalesItemDetail[t_key].ItemCount   
                                ,SalesItemDetail[t_key].ItemTotalPrice 
                                ,SalesItemDetail[t_key].ItemTotalPV                                 
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
            cgb_Item.grid_col_Count = 10;
            cgb_Item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "상품_코드"   , "상품명"  , "개별단가"   , "개별PV"        
                                , "주문_수량"   , "총상품액"    , "총상품PV"  , "구분" , "비고"
                                };

            int[] g_Width = { 0, 90, 160, 80, 70
                                ,80 , 80 , 80 , 70 , 200
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
                                ,DataGridViewContentAlignment.MiddleCenter 
                                ,DataGridViewContentAlignment.MiddleLeft  //10
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            
            cgb_Item.grid_col_header_text = g_HeaderText;
            cgb_Item.grid_cell_format = gr_dic_cell_format;
            cgb_Item.grid_col_w = g_Width;
            cgb_Item.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true                                                            
                                   };
            cgb_Item.grid_col_Lock = g_ReadOnly;

            cgb_Item.basegrid.RowHeadersVisible = false;
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
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Rece_Header_Reset()
        {
            cgb_Rece.Grid_Base_Arr_Clear();
            cgb_Rece.basegrid = dGridView_Base_Rece;
            cgb_Rece.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Rece.grid_col_Count = 10;
            cgb_Rece.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {""  , "배송구분"   , "배송일"  , "수령인"   , "우편_번호"        
                                , "주소1"   , "주소2"    , "연락처_1"  , "연락처_2" , "비고"
                                };

            int[] g_Width = { 0, 90, 70, 90, 100
                                ,120 , 100 , 90 , 150 , 200
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

            cgb_Rece.grid_col_header_text = g_HeaderText;
            cgb_Rece.grid_cell_format = gr_dic_cell_format;
            cgb_Rece.grid_col_w = g_Width;
            cgb_Rece.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true                                                            
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
                    if (SalesItemDetail[t_key].Del_TF != "D" && SalesItemDetail[t_key].RecIndex == 0 && SalesItemDetail[t_key].ItemCount > 0)
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
            cgb_Rece_Item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

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


            if (tb.Name == "txt_Receive_Method")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txt_Receive_Method_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txt_Receive_Method_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_Base_Rec")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txt_Base_Rec_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txt_Base_Rec_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
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

            //if ((ReCnt > 1) || (ReCnt == 0)) Db_Grid_Popup(tb, Tsql);            
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
            // Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
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

            //dGridView_Base_Cacu_R_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu_R.d_Grid_view_Header_Reset();

            //dGridView_Base_Cacu_R_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu_R_Item.d_Grid_view_Header_Reset();
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

                cls_Search_DB csd = new cls_Search_DB();

                //수정하기 전에 배열에다가 내역을 받아둔다.
                csd.SalesDetail_Mod_BackUp(txt_OrderNumber_R.Text, "tbl_SalesDetail");


                string StrSql = "";
                StrSql = "EXEC Usp_Insert_tbl_Sales_CanCel_Cacu_R '" + txt_OrderNumber_R.Text + "','" + cls_User.gid + "',0";

                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);


              

                //주테이블의 변경 내역을 테이블에 넣는다.
                csd.SalesDetail_Mod(Conn, tran, txt_OrderNumber_R.Text, "tbl_SalesDetail");


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

            if (SalesDetail[Ord_N].ReturnTF.ToString() == "2")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Sell_2")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellDate.Focus(); return false;
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
            if (txtSellDateRe.Text.Trim() != "")
            {
                int Ret = 0;
                cls_Check_Input_Error c_er = new cls_Check_Input_Error();
                Ret = c_er.Input_Date_Err_Check(txtSellDateRe);

                if (Ret == -1)
                {
                    txtSellDateRe.Focus(); return false;
                }
            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_SellDate_Re")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtSellDateRe.Focus(); return false;
            }


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

            dGridView_Base_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece.d_Grid_view_Header_Reset();

            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset();     

        

            if (SalesItemDetail !=null )
                SalesItemDetail.Clear();
            if (Sales_Rece != null)
                Sales_Rece.Clear();
            
   
            Base_Sub_Clear("Rece");
            
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(panel9, txtSellDate);
            ct.from_control_clear(panel5, txtSellDateRe);            
        }



        private void Base_Sub_Clear(string s_Tf)
        {
            cls_form_Meth ct = new cls_form_Meth();


            dGridView_Base_Rece_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Rece_Item.d_Grid_view_Header_Reset();

            ct.from_control_clear(panel2, txt_Receive_Method);
            chk_Total.Checked = false;
            butt_Rec_Del.Visible = false;
            butt_Rec_Save.Text = ct._chang_base_caption_search("추가");

            if (Sales_Rece != null)
                Rece_Grid_Set(); //배송 그리드

        }

       



        



        


        private void Base_Small_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Ord_Clear")
            {
                Base_Ord_Clear();
            }





            else if (bt.Name == "butt_Rec_Clear")
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

                if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                if (Item_Rece_Error_Check__01("Rece") == false) return;


                if (txt_RecIndex.Text == "") //추가 일경우에 새로운 입력
                {
                    cls_form_Meth ct = new cls_form_Meth();
                    int Salesitemindex = 0;
                    for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                    {
                        if (dGridView_Base_Rece_Item.Rows[i].Cells[0].Value.ToString() == "V")
                        {
                            Salesitemindex = int.Parse(dGridView_Base_Rece_Item.Rows[i].Cells[1].Value.ToString());
                            Base_Sub_Save_Rece(Salesitemindex);
                        }
                    }

                    Base_Sub_Clear("Rece");
                    Base_Sub_Clear("item");

                    ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save")
                    ////            + "\n" +
                    ////cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
                }
                else
                {
                    if (Base_Error_Check__01() == false) return;  //주문종류 , 회원, 주문일자 입력 안햇는지 체크

                    if (Item_Rece_Error_Check__01("Rece") == false) return;

                    Base_Sub_Edit_Rece();

                    Base_Sub_Clear("Rece");
                    Base_Sub_Clear("item");

                    ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit")
                    ////             + "\n" +
                    ////cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
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
            txtAddCode1.Text = AddCode1; txtAddCode2.Text = AddCode2;
            txtAddress1.Text = Address1; txtAddress2.Text = Address2;

            txtAddress2.Focus();
        }




        private bool Item_Rece_Error_Check__01(string s_Tf )
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


                if ((txtAddCode1.Text.Trim() != "") || (txtAddCode2.Text.Trim() != ""))
                {
                    if ((txtAddCode1.Text.Trim() == "") || (txtAddCode2.Text.Trim() == ""))
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data")
                            + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_AddCode")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtAddCode1.Focus();
                        return false;
                    }
                }//우편번호가 다 입력이 되엇는지 체크를 한다.


                if ((txtTel_1.Text.Trim() != "") || (txtTel_2.Text.Trim() != "") || (txtTel_3.Text.Trim() != ""))
                {
                    if ((txtTel_1.Text.Trim() == "") || (txtTel_2.Text.Trim() == "") || (txtTel_3.Text.Trim() == ""))
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data")
                            + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Tel")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtTel_1.Focus();
                        return false;
                    }

                } //전화 번호가 3칸다 제대로 들어 왓는지 체크를 한다.  


                if ((txtTel2_1.Text.Trim() != "") || (txtTel2_2.Text.Trim() != "") || (txtTel2_3.Text.Trim() != ""))
                {
                    if ((txtTel2_1.Text.Trim() == "") || (txtTel2_2.Text.Trim() == "") || (txtTel2_3.Text.Trim() == ""))
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data")
                            + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Fax")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        txtTel2_1.Focus();
                        return false;
                    }
                } //팩스 번호가 제대로 들어 왓는지 체크한다.



                int chk_cnt = 0;

                for (int i = 0; i <= dGridView_Base_Rece_Item.Rows.Count - 1; i++)
                {
                    if (dGridView_Base_Rece_Item.Rows[i].Cells[0].Value.ToString() == "V")
                    {
                        chk_cnt++;
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
            if (txtTel_1.Text != "") Get_Tel1 = txtTel_1.Text.Trim() + '-' + txtTel_2.Text.Trim() + '-' + txtTel_3.Text.Trim();
            if (txtTel2_1.Text != "") Get_Tel2 = txtTel2_1.Text.Trim() + '-' + txtTel2_2.Text.Trim() + '-' + txtTel2_3.Text.Trim();

            t_c_sell.Get_Tel1 = Get_Tel1;
            t_c_sell.Get_Tel2 = Get_Tel2;

            t_c_sell.Get_ZipCode = "";
            t_c_sell.Get_Address1 = "";
            t_c_sell.Get_Address2 = "";

            if (txtAddCode1.Text.Trim() != "")
                t_c_sell.Get_ZipCode = txtAddCode1.Text.Trim() + txtAddCode2.Text.Trim();

            if (txtAddress1.Text.Trim() != "")
                t_c_sell.Get_Address1 = txtAddress1.Text.Trim();

            if (txtAddress2.Text.Trim() != "")
                t_c_sell.Get_Address2 = txtAddress2.Text.Trim();

            t_c_sell.Get_Etc1 = txt_Get_Etc1.Text.Trim();
            t_c_sell.Get_Etc2 = "";
            t_c_sell.Pass_Number = txt_Pass_Number.Text.Trim();
            t_c_sell.Base_Rec_Name = txt_Base_Rec.Text.Trim();
            t_c_sell.Base_Rec = txt_Base_Rec_Code.Text.Trim();

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";

            t_c_sell.Del_TF = "S";
            Sales_Rece[New_SalesItemIndex] = t_c_sell;


            SalesItemDetail[New_SalesItemIndex].RecIndex = New_SalesItemIndex;
            SalesItemDetail[New_SalesItemIndex].SendDate = txtGetDate1.Text.Trim();
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
            if (txtTel_1.Text != "") Get_Tel1 = txtTel_1.Text.Trim() + '-' + txtTel_2.Text.Trim() + '-' + txtTel_3.Text.Trim();
            if (txtTel2_1.Text != "") Get_Tel2 = txtTel2_1.Text.Trim() + '-' + txtTel2_2.Text.Trim() + '-' + txtTel2_3.Text.Trim();

            Sales_Rece[SalesItemIndex].Get_Tel1 = Get_Tel1;
            Sales_Rece[SalesItemIndex].Get_Tel2 = Get_Tel2;

            Sales_Rece[SalesItemIndex].Get_ZipCode = "";
            Sales_Rece[SalesItemIndex].Get_Address1 = "";
            Sales_Rece[SalesItemIndex].Get_Address2 = "";

            if (txtAddCode1.Text.Trim() != "")
                Sales_Rece[SalesItemIndex].Get_ZipCode = txtAddCode1.Text.Trim() + txtAddCode2.Text.Trim();


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
        }




        private void Base_Sub_Delete(string s_Tf)
        {
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_form_Meth ct = new cls_form_Meth();
           
            //주문 상품 관련 딕셔너리에서 찾아서.. 삭제 표식을 해놓는다.
            Sales_Rece[int.Parse(txt_RecIndex.Text)].Del_TF = "D";

            //상품관련 딕셔너리에서 배송 날짜와 배송 인덱스를 없앤다.
            SalesItemDetail[int.Parse(txt_RecIndex.Text)].SendDate = "";
            SalesItemDetail[int.Parse(txt_RecIndex.Text)].RecIndex = 0;

            ct.from_control_clear(panel2, txt_Receive_Method);
            chk_Total.Checked = false; 
            butt_Rec_Del.Visible = false;
            butt_Rec_Save.Text = ct._chang_base_caption_search("추가");

            if (Sales_Rece != null)
                Rece_Grid_Set(); //상품 그리드  

            if (SalesItemDetail != null)
                Item_Grid_Set(); //상품 그리드     
            
           
            ////MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del")
            ////       + "\n" +
            ////       cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Save"));
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

            
            if (double.Parse(txt_TotalPrice_R.Text.Trim())
                < double.Parse(txt_TotalInputPrice_R.Text.Trim()))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re_002_004")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                dGridView_Base.Focus(); return false;
            }  
           
          
            //if (double.Parse(txt_TotalInputPrice.Text.Trim())
            //        < -double.Parse(txt_TotalInputPrice_R.Text.Trim()))
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sell_Return_Re_002_004")
            //             + "\n" +
            //             cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    dGridView_Base.Focus(); return false;
            //}                 
                    
    


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

            t_c_sell.Re_BaseOrderNumber = txt_OrderNumber.Text.Trim(); //원 주문 관련 주문번호를 넣는다.

            t_c_sell.TotalPrice = - double.Parse(txt_TotalPrice.Text.Trim().Replace(",","") );
            t_c_sell.TotalPV = - double.Parse(txt_TotalPv.Text.Trim().Replace(",", ""));
            t_c_sell.TotalCV = 0;
            t_c_sell.TotalInputPrice = 0;
            t_c_sell.Total_Sell_VAT_Price = - Total_Sell_VAT_Price;
            t_c_sell.Total_Sell_Except_VAT_Price = - Total_Sell_Except_VAT_Price;
            t_c_sell.InputCash = 0;
            t_c_sell.InputCard = 0;
            t_c_sell.InputPassbook = 0 ;
            t_c_sell.InputMile = 0;
            t_c_sell.InputPass_Pay = 0;
            t_c_sell.UnaccMoney = double.Parse(txt_UnaccMoney.Text.Trim().Replace(",", ""));
            
            t_c_sell.Etc1 = txt_ETC1_R.Text.Trim();
            t_c_sell.Etc2 = txt_ETC2_R.Text.Trim();

            t_c_sell.ReturnTF = 2;
            t_c_sell.ReturnTFName = ct._chang_base_caption_search("반품");
            t_c_sell.INS_Num = "";
            t_c_sell.InsuranceNumber_Date = "";
            t_c_sell.W_T_TF = 0;
            t_c_sell.In_Cnt = 0;

            t_c_sell.RecordID = cls_User.gid;
            t_c_sell.RecordTime = "";
                                
            t_c_sell.SellDate = txtSellDateRe.Text.Trim();

            t_c_sell.Del_TF = "S";
            SalesDetail[""] = t_c_sell;
        }




    



        private void DB_Save_tbl_SalesDetail____002(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran,  string OrderNumber)
        {
            string StrSql = "";

            cls_Search_DB csd = new cls_Search_DB();

            //수정하기 전에 배열에다가 내역을 받아둔다.
            csd.SalesDetail_Mod_BackUp(OrderNumber, "tbl_SalesDetail");


            StrSql = "Update tbl_SalesDetail Set ";
            StrSql = StrSql + " SellDate = '" + SalesDetail[OrderNumber].SellDate.Replace("-","")   + "'";
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



        private void DB_Save_tbl_SalesItemDetail(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, 
                    string OrderNumber)
        {           
            
            foreach (int t_key in SalesItemDetail.Keys)
            {
                if (SalesItemDetail[t_key].Del_TF != "D") 
                {                  
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

            StrSql = StrSql + ",''";
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",''";
            StrSql = StrSql + ",'" + SalesItemDetail[SalesItemIndex].RecordID + "'";
            StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";
                        
            if (Temp_Connect.Insert_Data(StrSql,"tbl_SalesItemDetail", Conn, tran, this.Name.ToString(), this.Text) == false) return;
           
        }








        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            string str_Q = "";

            int fi_cnt = 0; int Up_Cnt = 0;
            foreach (int t_key in Sales_Rece.Keys)
            {
                if (Sales_Rece[t_key].Del_TF != "S")
                    Up_Cnt ++;
                fi_cnt++;
            }
            
            if (Up_Cnt ==0)
                str_Q = "Msg_Base_Save_Q";
            else            
                str_Q = "Msg_Base_Edit_Q";
                            
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(str_Q), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (Check_TextBox_Error() == false) return;
                      
             cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            string T_ord_N = "";
            cls_Search_DB csd = new cls_Search_DB();
            
            try
            {
             
                T_ord_N = txt_OrderNumber_R.Text.Trim();

                //실질적인 저장,수정이 이루어지는곳. 변경시 주테이블 이전 내역도 같이 저장함
                DB_Save_tbl_SalesDetail____002(Temp_Connect, Conn, tran ,  T_ord_N );




                DB_Save_tbl_Sales_Rece(Temp_Connect, Conn, tran, T_ord_N);


                tran.Commit();

                Save_Error_Check = 1;
                if (Up_Cnt ==0)
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }
            catch (Exception)
            {
                tran.Rollback();
                if (Up_Cnt == 0)
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }          
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
            StrSql = StrSql + " RecIndex= " + SalesItemDetail[SalesItemIndex].RecIndex;
            
            StrSql = StrSql + " Where OrderNumber = '" + SalesItemDetail[SalesItemIndex].OrderNumber + "'";
            StrSql = StrSql + " And SalesItemIndex = " + SalesItemIndex.ToString();

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

            //주문 상품 테이블의 변경 내역을 테이블에 넣는다.
            csd.tbl_SalesDetail_Total_Change(Conn, tran, OrderNumber, SalesItemIndex, "tbl_SalesitemDetail", T_where);
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
                    DB_Save_tbl_SalesItemDetail____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Rece[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_Sales_Rece____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                    DB_Save_tbl_SalesItemDetail____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
                }
                else if (Sales_Rece[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_Sales_Rece____S(Temp_Connect, Conn, tran, OrderNumber, t_key);
                    DB_Save_tbl_SalesItemDetail____U(Temp_Connect, Conn, tran, OrderNumber, t_key);
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

            StrSql = StrSql + ",Get_ZipCode= '" + Sales_Rece[SalesItemIndex].Get_ZipCode + "'";
            StrSql = StrSql + ",Get_Address1= '" + Sales_Rece[SalesItemIndex].Get_Address1 + "'";
            StrSql = StrSql + ",Get_Address2= '" + Sales_Rece[SalesItemIndex].Get_Address2 + "'";

            StrSql = StrSql + ",Get_Tel1= '" + Sales_Rece[SalesItemIndex].Get_Tel1 + "'";
            StrSql = StrSql + ",Get_Tel2= '" + Sales_Rece[SalesItemIndex].Get_Tel2 + "'";

            StrSql = StrSql + ",Get_Etc1= '" + Sales_Rece[SalesItemIndex].Get_Etc1 + "'";
            StrSql = StrSql + ",Get_Etc2= '" + Sales_Rece[SalesItemIndex].Get_Etc2 + "'";

            StrSql = StrSql + ",Pass_Pay= " + Sales_Rece[SalesItemIndex].Pass_Pay;
            StrSql = StrSql + ",Pass_Number2= '" + Sales_Rece[SalesItemIndex].Pass_Number2 + "'";

            StrSql = StrSql + ",Base_Rec= '" + Sales_Rece[SalesItemIndex].Base_Rec + "'";

            StrSql = StrSql + " Where OrderNumber = '" + Sales_Rece[SalesItemIndex].OrderNumber + "'";
            StrSql = StrSql + " And SalesItemIndex = " + SalesItemIndex.ToString();

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

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

            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_ZipCode + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Address1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Address2 + "'";
                                     
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Tel1 + "'";
            StrSql = StrSql + ",'" + Sales_Rece[SalesItemIndex].Get_Tel2 + "'";

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




        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            Base_Ord_Clear();

            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {
                string OrderNumber = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();

                if (OrderNumber != "")
                {
                    Set_SalesDetail(OrderNumber);

                    if (SalesItemDetail != null)
                        SalesItemDetail.Clear();

                    if (Sales_Rece != null)
                        Sales_Rece.Clear();



                    Set_SalesItemDetail(OrderNumber);  //상품                 
                    Set_Sales_Rece(OrderNumber);  // 환불의 결제 정보 

                    Item_Grid_Set(); //상품 그리드
                    Rece_Grid_Set(); //배송 그리드
                }
            }
        }


        private void Set_SalesDetail(string OrderNumber)
        {
            int idx_ReturnTF = SalesDetail[OrderNumber].ReturnTF;

            Data_Set_Form_TF = 1;
                        
            string Re_BaseOrderNumber = SalesDetail[OrderNumber].Re_BaseOrderNumber.Trim();
            txtSellDate.Text = SalesDetail[Re_BaseOrderNumber].SellDate.Replace("-", "");
            txtSellCode.Text = SalesDetail[Re_BaseOrderNumber].SellCodeName;
            txtSellCode_Code.Text = SalesDetail[Re_BaseOrderNumber].SellCode;
            txtCenter2.Text = SalesDetail[Re_BaseOrderNumber].BusCodeName;
            txtCenter2_Code.Text = SalesDetail[Re_BaseOrderNumber].BusCode;                
            txt_OrderNumber.Text = Re_BaseOrderNumber;
            txt_TotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalPrice);
            txt_TotalPv.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalPV);

            txt_TotalInputPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].TotalInputPrice);
            txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[Re_BaseOrderNumber].UnaccMoney);

            txt_ETC1.Text = SalesDetail[Re_BaseOrderNumber].Etc1;
            txt_ETC2.Text = SalesDetail[Re_BaseOrderNumber].Etc2;
                

            txtSellDateRe.Text = SalesDetail[OrderNumber].SellDate.Replace("-", "");
            txt_OrderNumber_R.Text = OrderNumber;
            txt_TotalPrice_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPrice);
            txt_TotalPv_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalPV);

            txt_TotalInputPrice_R.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].TotalInputPrice);
            //txt_UnaccMoney.Text = string.Format(cls_app_static_var.str_Currency_Type, SalesDetail[OrderNumber].UnaccMoney);

            txt_ETC1_R.Text = SalesDetail[OrderNumber].Etc1;
            txt_ETC2_R.Text = SalesDetail[OrderNumber].Etc2;
            

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







        private void dGridView_Base_Sub_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Name == "dGridView_Base_Rece")
            {
                if (dgv.CurrentRow.Cells[0].Value != null)
                {
                    //int T_key = int.Parse(dgv.CurrentRow.Cells[0].Value.ToString());

                    Put_Sub_Date( dgv.CurrentRow.Cells[0].Value.ToString(), "Rece");
                }
            }
            
        }


        private void Put_Sub_Date( string SalesItemIndex, string t_STF)
        {
            if (t_STF == "Rece")
            {
                Data_Set_Form_TF = 1;
                txt_RecIndex.Text = SalesItemIndex;

                butt_Rec_Del.Visible = true;
                cls_form_Meth cm = new cls_form_Meth();
                butt_Rec_Save.Text = cm._chang_base_caption_search("수정");
                int Salesitemindex = int.Parse(txt_RecIndex.Text);

                txt_Receive_Method.Text = Sales_Rece[Salesitemindex].Receive_Method_Name.ToString();
                txt_Receive_Method_Code.Text = Sales_Rece[Salesitemindex].Receive_Method.ToString();
                txt_Get_Name1.Text = Sales_Rece[Salesitemindex].Get_Name1;

                txtAddCode1.Text = ""; txtAddCode2.Text = "";

                if (Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Length >= 6)
                {
                    txtAddCode1.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(0, 3);
                    txtAddCode2.Text = Sales_Rece[Salesitemindex].Get_ZipCode.ToString().Substring(3, 3);
                }

                string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
                cls_form_Meth cfm = new cls_form_Meth();
                cfm.Phone_Number_Split(Sales_Rece[Salesitemindex].Get_Tel1.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;

                cfm.Phone_Number_Split(Sales_Rece[Salesitemindex].Get_Tel2.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

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




        private void dGridView_Base_Rece_Add_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {


                txt_Get_Name1.Text = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();

                txtAddCode1.Text = ""; txtAddCode2.Text = "";

                if ((sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Length >= 6)
                {
                    txtAddCode1.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(0, 3);
                    txtAddCode2.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString().Substring(3, 3);
                }

                txtAddress1.Text = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                txtAddress2.Text = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();

                string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
                cls_form_Meth cfm = new cls_form_Meth();
                cfm.Phone_Number_Split((sender as DataGridView).CurrentRow.Cells[4].Value.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;

                cfm.Phone_Number_Split((sender as DataGridView).CurrentRow.Cells[5].Value.ToString(), ref T_Num_1, ref T_Num_2, ref T_Num_3);
                txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

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
                                ,ds.Tables["TempTable"].Rows[fi_cnt][2].ToString()  
                                ,ds.Tables["TempTable"].Rows[fi_cnt][3].ToString()  
                                ,ds.Tables["TempTable"].Rows[fi_cnt][4].ToString() 
 
                                ,ds.Tables["TempTable"].Rows[fi_cnt][5].ToString()                                  
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
                txtAddCode1.Text = ""; txtAddCode2.Text = "";
                txtAddress1.Text = ""; txtAddress2.Text = "";
                txtTel_1.Text = ""; txtTel_2.Text = ""; txtTel_3.Text = "";
                txtTel2_1.Text = ""; txtTel2_2.Text = ""; txtTel2_3.Text = "";
                txt_Get_Name1.Text = "";
            }

            txtAddCode1.Text = ""; txtAddCode2.Text = "";
            txtAddress1.Text = ""; txtAddress2.Text = "";
            txtTel_1.Text = ""; txtTel_2.Text = ""; txtTel_3.Text = "";
            txtTel2_1.Text = ""; txtTel2_2.Text = ""; txtTel2_3.Text = "";
            txt_Get_Name1.Text = "";

            if (ReCnt == 0) return;
            
            txtAddress1.Text = encrypter.Decrypt(  ds.Tables["t_P_table"].Rows[0]["address1"].ToString());
            txtAddress2.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["address2"].ToString());

            if (ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Length >= 6)
            {
                txtAddCode1.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(0, 3);
                txtAddCode2.Text = ds.Tables["t_P_table"].Rows[0]["Addcode1"].ToString().Substring(3, 3);
            }

            string T_Num_1 = ""; string T_Num_2 = ""; string T_Num_3 = "";
            cls_form_Meth cfm = new cls_form_Meth();
            cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["hptel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            txtTel_1.Text = T_Num_1; txtTel_2.Text = T_Num_2; txtTel_3.Text = T_Num_3;

            cfm.Phone_Number_Split(encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["homeTel"].ToString()), ref T_Num_1, ref T_Num_2, ref T_Num_3);
            txtTel2_1.Text = T_Num_1; txtTel2_2.Text = T_Num_2; txtTel2_3.Text = T_Num_3;

            //txt_Get_Name1.Text = ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString();
            if (t_rb.Name == "opt_Rec_Add2")
                txt_Get_Name1.Text = encrypter.Decrypt(ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString()); //주소테이블의 배송자명은 암호화 햇기 대문에
            else
                txt_Get_Name1.Text = ds.Tables["t_P_table"].Rows[0]["M_Name"].ToString();  //회원 테이블의 회원명은 암호화 안햇음
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





    }
}
