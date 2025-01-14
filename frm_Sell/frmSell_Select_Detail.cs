﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace MLM_Program
{
    public partial class frmSell_Select_Detail : Form
    {

        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgbb = new cls_Grid_Base();

        ////public delegate void SendNumberDele(string Send_Number, string Send_Name, string Send_OrderNumber);
        ////public event SendNumberDele Send_Sell_Number;

        public delegate void SendNumberDele(string Send_Number, string Send_Name, string Send_OrderNumber);
        public event SendNumberDele Send_Sell_Number;

        public delegate void Send_Mem_NumberDele(string Send_Number, string Send_Name);
        public event Send_Mem_NumberDele Send_Mem_Number;


        private const string base_db_name = "tbl_SalesDetail";
        private int Data_Set_Form_TF;


        public frmSell_Select_Detail()
        {
            InitializeComponent();

            DoubleBuffered = true;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.SetProperty, null, dGridView_Base, new object[] { true });
        }

      


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_gold2(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgbb.d_Grid_view_Header_Reset();

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            cls_Pro_Base_Function cpbf = new cls_Pro_Base_Function();
            cpbf.Put_SellCode_ComboBox(combo_Se, combo_Se_Code);

            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtMbid2.Mask = cls_app_static_var.Member_Number_Fromat;


            mtxtMbid.Focus();
            //grB_Search.Height = mtxtMbid.Top + mtxtMbid.Height + 3;  

            mtxtSellDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtSellDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtMakDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtMakDate2.Mask = cls_app_static_var.Date_Number_Fromat;


            txt_P_1.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_2_2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_3.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_4.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_5.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_6.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_P_7.BackColor = cls_app_static_var.txt_Enable_Color;
          //  txt_P_8.BackColor = cls_app_static_var.txt_Enable_Color;

            ////txt_SumCnt.BackColor = cls_app_static_var.txt_Enable_Color;
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Select.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Excel.Left = butt_Select.Left + butt_Select.Width + 2;
            butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Select);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);  
        }

        private void frm_Base_Activated(object sender, EventArgs e)
        {
           //19-03-11 깜빡임제거 this.Refresh();

            if (cls_User.uSearch_MemberNumber != "")
            {
                Data_Set_Form_TF = 1;
                mtxtMbid.Text = cls_User.uSearch_MemberNumber;
                // mtxtSMbid.Text = cls_User.uSearch_MemberNumber;
                cls_User.uSearch_MemberNumber = "";

                EventArgs ee1 = null; Base_Button_Click(butt_Select, ee1);  //butt_Search
                //EventArgs ee1 = null; Select_Button_Click(butt_Select, ee1);

                //Set_Form_Date(mtxtMbid.Text);
                Data_Set_Form_TF = 0;
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
                            //cfm.form_Group_Panel_Enable_True(this);
                        }
                    }
                }// end if

            }

           

            Button T_bt = butt_Exit;
            if (e.KeyValue == 123)
                T_bt = butt_Exit;    //닫기  F12
            if (e.KeyValue == 113)
                T_bt = butt_Select;     //조회  F1
            if (e.KeyValue == 115)
                T_bt = butt_Delete;   // 삭제  F4
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


        private Boolean Check_TextBox_Error()
        {
           
            cls_Check_Input_Error c_er = new cls_Check_Input_Error();

            if (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")               
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtMbid);

                if (Ret == -1)
                {                    
                    mtxtMbid.Focus();     return false;
                }   
            }


            if (mtxtMbid2.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtMbid2);

                if (Ret == -1)
                {
                    mtxtMbid2.Focus(); return false;
                }   
            }


            if (mtxtSellDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate1.Text, mtxtSellDate1, "Date") == false)
                {
                    mtxtSellDate1.Focus();
                    return false;
                }

            }

            if (mtxtSellDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate2.Text, mtxtSellDate2, "Date") == false)
                {
                    mtxtSellDate2.Focus();
                    return false;
                }
            }

            if (mtxtMakDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtMakDate1.Text, mtxtMakDate1, "Date") == false)
                {
                    mtxtMakDate1.Focus();
                    return false;
                }
            }

            if (mtxtMakDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtMakDate2.Text, mtxtMakDate2, "Date") == false)
                {
                    mtxtMakDate2.Focus();
                    return false;
                }
            }


                   

            return true;
        }


        private void Make_Base_Query(ref string Tsql)
        {

            //string[] g_HeaderText = {"주문번호"  , "주문_일자"   , "반품_교환_일자"  , "회원_번호"   , "성명"        
            //                    , "주민번호"   , "등록_센타명"    , "주문_센타명"   , "주문_종류"    , "총주문액"
            //                    , "총PV"   , "총결제액"  , "현금"   , "카드"   ,"무통장"
            //                    , "미수금"     , "구분"    , "비고1" , "비고2"     , "기록자"
            //                    , "기록일", ""  , ""  , ""  ,""
            //                    , ""
            //                        };


            Tsql = "Select  ";
            Tsql = Tsql + "  LEFT(tbl_SalesDetail.SellDate,4) +'-' + LEFT(RIGHT(tbl_SalesDetail.SellDate,4),2) + '-' + RIGHT(tbl_SalesDetail.SellDate,2)   ";
            Tsql = Tsql + ", tbl_SalesDetail.OrderNumber  ";            
            
            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + ", tbl_SalesDetail.mbid + '-' + Convert(Varchar,tbl_SalesDetail.mbid2) ";
            else
                Tsql = Tsql + ", tbl_SalesDetail.mbid2 ";

            Tsql = Tsql + " ,tbl_SalesDetail.M_Name ";           
       
            Tsql = Tsql + " ,Isnull(tbl_Business.Name,'') as B_Name";

            Tsql = Tsql + " ,Isnull(S_Bus.Name,'') as S_B_Name";
            Tsql = Tsql + " , tbl_SellType.SellTypeName SellCodeName  ";
            Tsql = Tsql + ", TotalInputPrice ";
            Tsql = Tsql + ",UnaccMoney ";
            Tsql = Tsql + " From tbl_SalesDetail (nolock) ";
            Tsql = Tsql + " LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
            Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode  And tbl_Memberinfo.Na_code = tbl_Business.Na_code  ";
            Tsql = Tsql + " LEFT JOIN tbl_Business S_Bus (nolock) ON tbl_SalesDetail.BusCode = S_Bus.NCode  And tbl_SalesDetail.Na_code = S_Bus.Na_code  ";
            Tsql = Tsql + " LEFT Join tbl_SellType (nolock) ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode ";
            //20200316 구현호
            //Tsql = Tsql + " LEFT JOIN tbl_Sales_Cacu (nolock) ON tbl_SalesDetail.OrderNumber = tbl_Sales_Cacu.OrderNumber";

        }

        private void Make_Base_Query___i(ref string Tsql)
        {
            //string[] g_HeaderText = {"주문_일자"  , "주문번호"   , "회원_번호"   , "성명"   , "등록_센타명"     
            //                    , "주문_센타명"   , "주문_종류"    , "상품명"   , "금액"    , "PV"
            //                    , "수량"   , "합계금액"  , "합계PV"   , "현금결제액"   ,"현금결제일"
            //                    , "무통장결제액"  , "무통장결제일"    , "카드결제액" , "카드결제일" , "카드사명"
            //                    , "카드번호", "승인번호"  , "승인일자"  , "할부"  ,"실입금액"
            //                    , "비고"
            //                        };
          
            Tsql = "Select  ";
            Tsql = Tsql + "  tbl_Goods.Name   ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemPrice  ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemPv  ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemCv  ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemCount  ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemTotalPrice  ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemTotalPv  ";
            Tsql = Tsql + ", tbl_SalesitemDetail.ItemTotalCv  ";
            Tsql = Tsql + " ,tbl_SalesitemDetail.Etc ";
            Tsql = Tsql + ", tbl_SalesDetail.OrderNumber "; 
            Tsql = Tsql + " From tbl_SalesitemDetail (nolock) ";
            Tsql = Tsql + " LEFT JOIN  tbl_SalesDetail (nolock) ON tbl_SalesitemDetail.OrderNumber = tbl_SalesDetail.OrderNumber ";
            Tsql = Tsql + " LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
            Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code  ";
            Tsql = Tsql + " LEFT JOIN tbl_Business S_Bus (nolock) ON tbl_SalesDetail.BusCode = S_Bus.NCode And tbl_SalesDetail.Na_code = S_Bus.Na_code  ";
            Tsql = Tsql + " LEFT Join tbl_SellType (nolock) ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode ";
            Tsql = Tsql + " LEFT JOIN tbl_Goods  (nolock) ON tbl_SalesitemDetail.ItemCode = tbl_Goods.NCode ";

            //20200316 궇녀호
           // Tsql = Tsql + " LEFT JOIN tbl_Sales_Cacu (nolock) ON tbl_SalesitemDetail.OrderNumber = tbl_Sales_Cacu.OrderNumber";
        }

        private void Make_Base_Query___C(ref string Tsql)
        {
            //string[] g_HeaderText = {"주문_일자"  , "주문번호"   , "회원_번호"   , "성명"   , "등록_센타명"     
            //                    , "주문_센타명"   , "주문_종류"    , "상품명"   , "금액"    , "PV"
            //                    , "수량"   , "합계금액"  , "합계PV"   , "현금결제액"   ,"현금결제일"
            //                    , "무통장결제액"  , "무통장결제일"    , "카드결제액" , "카드결제일" , "카드사명"
            //                    , "카드번호", "승인번호"  , "승인일자"  , "할부"  ,"실입금액"
            //                    , "비고"
            //                        };

            Tsql = "Select  ";
            Tsql = Tsql + "  Case When C_TF = 1 Then C_Price1 ELSE 0 End " ;
            Tsql = Tsql + " ,Case When C_TF = 1 Then  LEFT(C_AppDate1,4) +'-' + LEFT(RIGHT(C_AppDate1,4),2) + '-' + RIGHT(C_AppDate1,2) ELSE '' End   ";

            //Tsql = Tsql + " ,Case When C_TF = 2 Then C_Price1 ELSE 0 End ";
            //Tsql = Tsql + " ,Case When C_TF = 2 Then  LEFT(C_AppDate1,4) +'-' + LEFT(RIGHT(C_AppDate1,4),2) + '-' + RIGHT(C_AppDate1,2) ELSE '' End   ";
            //2018-10-04 지성경 가상계좌로 확인되도록 수정
            Tsql = Tsql + " ,Case When C_TF = 5 Then C_Price1 ELSE 0 End ";
            Tsql = Tsql + " ,Case When C_TF = 5 Then  LEFT(C_AppDate1,4) +'-' + LEFT(RIGHT(C_AppDate1,4),2) + '-' + RIGHT(C_AppDate1,2) ELSE '' End   ";

            Tsql = Tsql + " ,Case When C_TF = 3 Then C_Price1 ELSE 0 End ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then  LEFT(C_AppDate1,4) +'-' + LEFT(RIGHT(C_AppDate1,4),2) + '-' + RIGHT(C_AppDate1,2) ELSE '' End   ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then Isnull(tbl_Card.cardname,'') ELSE '' End ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then C_Number1 ELSE '' End ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then C_Number2 ELSE '' End ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then  LEFT(C_AppDate1,4) +'-' + LEFT(RIGHT(C_AppDate1,4),2) + '-' + RIGHT(C_AppDate1,2) ELSE '' End   ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then C_Installment_Period ELSE '' End ";
            Tsql = Tsql + " ,Case When C_TF = 3 Then C_Price2 ELSE 0 End ";
            Tsql = Tsql + " ,tbl_Sales_Cacu.C_Etc ";
            
            Tsql = Tsql + ", tbl_SalesDetail.OrderNumber  ";
            
            Tsql = Tsql + " From tbl_Sales_Cacu (nolock) ";
            Tsql = Tsql + " LEFT JOIN  tbl_SalesDetail (nolock) ON tbl_Sales_Cacu.OrderNumber = tbl_SalesDetail.OrderNumber ";           
            Tsql = Tsql + " LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
            Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";
            Tsql = Tsql + " LEFT JOIN tbl_Business S_Bus (nolock) ON tbl_SalesDetail.BusCode = S_Bus.NCode  And tbl_SalesDetail.Na_code = S_Bus.Na_code ";
            Tsql = Tsql + " LEFT Join tbl_SellType ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode ";
            Tsql = Tsql + " LEFT JOIN tbl_BankForCompany (nolock) ON tbl_Sales_Cacu.C_Code = tbl_BankForCompany.BankCode And  tbl_Sales_Cacu.C_Number1 = tbl_BankForCompany.BankAccountNumber  And tbl_Sales_Cacu.C_TF =2 ";
            Tsql = Tsql + " LEFT JOIN tbl_Card (nolock) ON tbl_Sales_Cacu.C_Code = tbl_Card.Ncode And tbl_Sales_Cacu.C_TF = 3 And tbl_SalesDetail.Na_Code = tbl_Card.Na_Code ";
        }



        private void Make_Base_Query_(ref string Tsql)
        {
            string strSql = " Where tbl_SalesDetail.Mbid2 >= 0  ";
            
                        string Mbid = ""; int Mbid2 = 0;
            //회원번호1로 검색
            if (
                (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "") 
                &&
                (mtxtMbid2.Text.Replace("-", "").Replace("_", "").Trim() == "") 
                )
            {
                cls_Search_DB csb = new cls_Search_DB();
                if (csb.Member_Nmumber_Split(mtxtMbid.Text, ref Mbid, ref Mbid2) == 1)
                {
                    strSql = strSql + " And tbl_SalesDetail.Mbid = '" + Mbid + "'";
                    strSql = strSql + " And tbl_SalesDetail.Mbid2 = " + Mbid2;
                }
            }

            //회원번호2로 검색
            if (
                (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
                &&
                (mtxtMbid2.Text.Replace("-", "").Replace("_", "").Trim() != "")
                )
            {
                cls_Search_DB csb = new cls_Search_DB();
                if (csb.Member_Nmumber_Split(mtxtMbid.Text, ref Mbid, ref Mbid2) == 1)
                {
                    if (Mbid != "")
                        strSql = strSql + " And tbl_SalesDetail.Mbid >='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql = strSql + " And tbl_SalesDetail.Mbid2 >= " + Mbid2;
                }

                if (csb.Member_Nmumber_Split(mtxtMbid2.Text, ref Mbid, ref Mbid2) == 1)
                {
                    if (Mbid != "")
                        strSql = strSql + " And tbl_SalesDetail.Mbid <='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql = strSql + " And tbl_SalesDetail.Mbid2 <= " + Mbid2;
                }
            }


            //회원명으로 검색
            if (txtName.Text.Trim() != "")
                strSql = strSql + " And tbl_SalesDetail.M_Name Like '%" + txtName.Text.Trim() + "%'";

            //가입일자로 검색 -1
            if ((mtxtSellDate1.Text.Replace("-", "").Trim() != "") && (mtxtSellDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And tbl_SalesDetail.SellDate = '" + mtxtSellDate1.Text.Replace("-", "").Trim() + "'";

            //가입일자로 검색 -2
            if ((mtxtSellDate1.Text.Replace("-", "").Trim() != "") && (mtxtSellDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And tbl_SalesDetail.SellDate >= '" + mtxtSellDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And tbl_SalesDetail.SellDate <= '" + mtxtSellDate2.Text.Replace("-", "").Trim() + "'";
            }


            //기록일자로 검색 -1
            if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And Replace(Left( tbl_SalesDetail.recordtime ,10),'-','') = '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'";

            //기록일자로 검색 -2
            if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And Replace(Left( tbl_SalesDetail.recordtime ,10),'-','') >= '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And Replace(Left( tbl_SalesDetail.recordtime ,10),'-','') <= '" + mtxtMakDate2.Text.Replace("-", "").Trim() + "'";
            }


           

            //센타코드로으로 검색
            if (txtCenter_Code.Text.Trim() != "")
                strSql = strSql + " And tbl_Memberinfo.BusinessCode = '" + txtCenter_Code.Text.Trim() + "'";

            if (txtCenter2_Code.Text.Trim() != "")
                strSql = strSql + " And tbl_SalesDetail.BusCode = '" + txtCenter2_Code.Text.Trim() + "'";

            if (txtR_Id_Code.Text.Trim() != "")
                strSql = strSql + " And tbl_SalesDetail.recordid = '" + txtR_Id_Code.Text.Trim() + "'";


            if (txtSellCode_Code.Text.Trim() != "")
                strSql = strSql + " And tbl_SalesDetail.SellCode = '" + txtSellCode_Code.Text.Trim() + "'";

            if (txtOrderNumber.Text.Trim() != "")
                strSql = strSql + " And tbl_SalesDetail.OrderNumber = '" + txtOrderNumber.Text.Trim() + "'";

            



            if (opt_sell_2.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.ReturnTF = 1 ";

            if (opt_sell_3.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.ReturnTF = 2 ";

            if (opt_sell_4.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.ReturnTF = 3 ";

            if (opt_sell_5.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.ReturnTF = 4 ";

            if (opt_sell_6.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.ReturnTF = 5 ";

           


            

            if (opt_Ed_2.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.UnaccMoney = 0 ";

            if (opt_Ed_3.Checked == true)
                strSql = strSql + " And tbl_SalesDetail.UnaccMoney <> 0 ";



            //strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            strSql = strSql + " And tbl_SalesDetail.BusCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";

            strSql = strSql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

            string CardNumber = txtCardNumber.Text.Trim();
            string CardApproveNumber = txtCardApproveNumber.Text.Trim();
            if(CardNumber.Length > 0 && CardApproveNumber.Length > 0)
            {
                strSql += " AND dbo.DECRYPT_AES256(tbl_Sales_Cacu.C_Number1) LIKE '%" + CardNumber + "%' ";
                strSql += " AND tbl_Sales_Cacu.C_Number2 LIKE '%" + CardApproveNumber + "%' ";
            }
            else if (CardNumber.Length > 0)
            {
                strSql += " AND dbo.DECRYPT_AES256(tbl_Sales_Cacu.C_Number1) LIKE '%" + CardNumber + "%' ";
            }
            else if (CardApproveNumber.Length > 0)
            {
                strSql += " AND tbl_Sales_Cacu.C_Number2 LIKE '%" + CardApproveNumber + "%' ";
            }

            if (CardNumber.Length > 0 || CardApproveNumber.Length > 0)
                strSql += " AND tbl_Sales_Cacu.C_TF = 3";


            Tsql = Tsql + strSql ;
            Tsql = Tsql + " Order by tbl_SalesDetail.SellDate DESC, tbl_SalesDetail.OrderNumber ";
            Tsql = Tsql + ",tbl_SalesDetail.Mbid, tbl_SalesDetail.Mbid2  ";
        }


        private void Base_Grid_Set2(string mbid2)
        {
            dGridView_gold2(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgbb.d_Grid_view_Header_Reset();

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine(" Select  top 1  T_AA.Lvl, T_AA.mbid2  ");
            //sb.AppendLine(",Isnull(CC_A.G_Name,'') as  gold");
            //sb.AppendLine(",A.M_Name  ");
            //sb.AppendLine(" From ufn_GetSubTree_MemGroup('', " + mbid2 + ") T_AA ");
            //sb.AppendLine(" LEFT JOIN tbl_Memberinfo AS A  (nolock) ON A.Mbid = T_AA.mbid And A.Mbid2 = T_AA.Mbid2  ");
            //sb.AppendLine(" LEFT JOIN tbl_Memberinfo AS B  (nolock) ON a.Saveid = b.mbid And a.Saveid2 = b.mbid2 ");
            //sb.AppendLine(" LEFT JOIN tbl_Memberinfo AS C  (nolock) ON a.Nominid=c.mbid And a.Nominid2 = c.mbid2   ");
            //sb.AppendLine(" LEFT Join tbl_Business  (nolock) On a.businesscode=tbl_Business.ncode And a.Na_code = tbl_Business.Na_code ");
            //sb.AppendLine(" Left Join tbl_Class C1  (nolock) On A.CurGrade=C1.Grade_Cnt ");
            //sb.AppendLine(" Left Join ufn_Mem_CurGrade_Mbid_Search ('',0) AS CC_A On CC_A.Mbid = A.Mbid And  CC_A.Mbid2 = A.Mbid2  ");
            //sb.AppendLine(" Where T_AA.Lvl > 0  ");
            //sb.AppendLine(" and C1.grade_cnt>=70");
            //sb.AppendLine(" and  A.LeaveDate = '' ");
            //sb.AppendLine(" ORder by Lvl ASC, ");
            //sb.AppendLine(" LEFT(SaveCur,3) ASC   , SaveCur ASC ");

            sb.AppendLine("Select  ");
            sb.AppendLine(" T_AA.Lvl ");
            sb.AppendLine(" , T_AA.mbid2");
            sb.AppendLine("   ,Isnull(CC_A.G_Name,'') ");
            sb.AppendLine("    ,A.M_Name ");
            sb.AppendLine("	 , Case When A.Regtime <> '' Then  LEFT(A.Regtime,4) +'-' + LEFT(RIGHT(A.Regtime,4),2) + '-' + RIGHT(A.Regtime,2) ELSE '' End  ");
            sb.AppendLine("	 , Case When A.LeaveDate <> '' Then  LEFT(A.LeaveDate,4) +'-' + LEFT(RIGHT(A.LeaveDate,4),2) + '-' + RIGHT(A.LeaveDate,2) ELSE '' End ");
            sb.AppendLine("	 , Isnull( tbl_Business.name,'')  ,A.Saveid2  , Isnull(b.M_Name,'')  ,A.Nominid2  , Isnull(C.M_Name,'')  , A.hometel  , A.hptel  , '' ");
            sb.AppendLine("	  , A.LineCnt  From ufn_matrix_mem('',"+mbid2+ ", '*') T_AA  LEFT JOIN tbl_Memberinfo AS A  (nolock) ON A.Mbid = T_AA.mbid And A.Mbid2 = T_AA.Mbid2    ");
            sb.AppendLine("	  LEFT JOIN tbl_Memberinfo AS B  (nolock) ON a.Saveid = b.mbid And a.Saveid2 = b.mbid2    LEFT JOIN tbl_Memberinfo AS C  (nolock) ON a.Nominid=c.mbid And a.Nominid2 = c.mbid2   ");
            sb.AppendLine("	   LEFT Join tbl_Business  (nolock) On a.businesscode=tbl_Business.ncode  And a.Na_code = tbl_Business.Na_code");
            sb.AppendLine("	    Left Join tbl_Class C1  (nolock) On A.CurGrade=C1.Grade_Cnt  ");
            sb.AppendLine("		Left Join ufn_Mem_CurGrade_Mbid_Search ('',0) AS CC_A On CC_A.Mbid = A.Mbid And  CC_A.Mbid2 = A.Mbid2  ");
            sb.AppendLine("		where  T_AA.mbid2 <> '" + mbid2 + "'");
            sb.AppendLine("	   ORder by Lvl ");

           DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(sb.ToString(), base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                string test = ds.Tables[base_db_name].Rows[fi_cnt][2].ToString();
                if (test == "골드" || test == "루비" || test == "사파이어" || test == "에메랄드" || test == "다이아몬드" || test == "블루다이아몬드" || test == "레드다디아몬드" || test == "크라운" || test == "엠페리얼")
                {
                    Set_gr_dic2(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
                    break;
                }
               
            }

            cgbb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgbb.db_grid_Obj_Data_Put();
        }
        private void Set_gr_dic2(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            
                object[] row0 = {
                 ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][3]
                                 };


                gr_dic_text[fi_cnt + 1] = row0;
               
          
        }
        private void Base_Grid_Set()
        {
            string Tsql = ""; string Tsql_C = ""; string Tsql_i = "";            
            Make_Base_Query(ref Tsql);
            Make_Base_Query___i(ref Tsql_i);
            Make_Base_Query___C(ref Tsql_C);

            Make_Base_Query_(ref Tsql);
            Make_Base_Query_(ref Tsql_i);
            Make_Base_Query_(ref Tsql_C);

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();                                  
            
            DataSet ds = new DataSet();                        
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name , this.Text ) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            cls_Connect_DB Temp_Connect_C = new cls_Connect_DB();  
            DataSet ds_C = new DataSet();            
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect_C.Open_Data_Set(Tsql_C, "tbl_Sales_Cacu", ds_C, this.Name, this.Text) == false) return;
            int ReCnt_C = Temp_Connect_C.DataSet_ReCount;
            //++++++++++++++++++++++++++++++++

            cls_Connect_DB Temp_Connect_i = new cls_Connect_DB();  
            DataSet ds_i = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect_i.Open_Data_Set(Tsql_i, "tbl_SalesItemDetail", ds_i, this.Name, this.Text) == false) return;
            int ReCnt_i = Temp_Connect_i.DataSet_ReCount;
            //++++++++++++++++++++++++++++++++

            
            //double Sum_10 = 0; double Sum_11 = 0; double Sum_12 = 0;
            //double Sum_13 = 0; double Sum_14 = 0; double Sum_15 = 0;
            //double Sum_16 = 0;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            int cnt_i = 0; int cnt_C = 0; int fi_cnt = 0; int Ord_first = 0;
            int Af_Cnt_C = 0; int Af_Cnt_i = 0; int Arr_Cnt = 0;

            int Total_i_1 = 0; int Total_i_2 = 0; int Total_i_3 = 0, Total_i_4 = 0 ;
            int Total_C_1 = 0; int Total_C_2 = 0; int Total_C_3 = 0; int Total_C_4 = 0;

            int S_Total_i_1 = 0; int S_Total_i_2 = 0; int S_Total_i_3 = 0, S_Total_i_4 = 0 ;
            int S_Total_C_1 = 0; int S_Total_C_2 = 0; int S_Total_C_3 = 0; int S_Total_C_4 = 0;

            double Sum_20 = 0; double Sum_21 = 0;

            string S_Date = ds.Tables[base_db_name].Rows[0][0].ToString();
            for (fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_totalinputprice(ref ds, ref gr_dic_text, fi_cnt);
                Sum_20 = Sum_20 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["totalinputprice"].ToString());  //판매수량
                Sum_21 = Sum_21 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["unaccmoney"].ToString());  //출고수량
            }
            for ( fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                
                string OrderNumber = ds.Tables[base_db_name].Rows[fi_cnt][1].ToString() ;

                cnt_i = 0; cnt_C = 0; Ord_first = 0;

            done:
                object[] row0 = new object[cgb.grid_col_Count];

                Af_Cnt_C= 0;  Af_Cnt_i = 0;

                Set_gr_dic(ref row0, ref ds, fi_cnt, Ord_first);  //데이타를 배열에 넣는다.


               



                Set_gr_dic_i(ref row0, ref ds_i, OrderNumber, ReCnt_i, ref cnt_i, ref Af_Cnt_i,
                    ref Total_i_1, ref Total_i_2, ref Total_i_3, ref Total_i_4                  
                    );  //데이타를 배열에 넣는다.

                Set_gr_dic_C(ref row0, ref ds_C, OrderNumber, ReCnt_C, ref cnt_C, ref Af_Cnt_C, 
                    ref Total_C_1, ref Total_C_2, ref Total_C_3, ref Total_C_4                    
                    );  //데이타를 배열에 넣는다.

               


                gr_dic_text[Arr_Cnt + 1] = row0;
                Arr_Cnt++;
                if (Af_Cnt_i + Af_Cnt_C > 0 )
                {
                    Ord_first++;
                    goto done;
                }

                if (fi_cnt + 1  <= ReCnt - 1)
                {
                    if (S_Date != ds.Tables[base_db_name].Rows[fi_cnt +1][0].ToString())
                    {
                        Set_gr_dic_Total(ref gr_dic_text, ref Arr_Cnt, ref Total_i_1, ref Total_i_2, ref Total_i_3, ref Total_i_4, ref Total_C_1, ref Total_C_2, ref Total_C_3, ref Total_C_4,
                            ref S_Total_i_1, ref S_Total_i_2, ref S_Total_i_3,ref S_Total_i_4, ref S_Total_C_1, ref S_Total_C_2, ref S_Total_C_3, ref S_Total_C_4);
                                                
                        S_Date = ds.Tables[base_db_name].Rows[fi_cnt + 1][0].ToString();
                    }
                }
            }

            Set_gr_dic_Total(ref gr_dic_text, ref Arr_Cnt, ref Total_i_1, ref Total_i_2, ref Total_i_3, ref Total_i_4, ref Total_C_1, ref Total_C_2, ref Total_C_3, ref Total_C_4,
               ref S_Total_i_1, ref S_Total_i_2, ref S_Total_i_3, ref S_Total_i_4, ref S_Total_C_1, ref S_Total_C_2, ref S_Total_C_3, ref S_Total_C_4);

            Set_gr_dic_Total(ref gr_dic_text, ref Arr_Cnt, ref S_Total_i_1, ref S_Total_i_2, ref S_Total_i_3, ref  S_Total_i_4, ref S_Total_C_1, ref S_Total_C_2, ref S_Total_C_3, ref S_Total_C_4);


            txt_P_3.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_20);
            txt_P_7.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_21);


            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
        }

        private void Set_gr_dic_totalinputprice(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][6],
                               ds.Tables[base_db_name].Rows[fi_cnt][7]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_gold2()
        {
            cgbb.grid_col_Count = 4;
            cgbb.basegrid = dGridView_gold;
            cgbb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;

            string[] g_HeaderText = {"lv","최초골드이상 회원번호","최초골드이상 회원 직위"  ,"최초골드이상 회원명"
                                    };
            cgbb.grid_col_header_text = g_HeaderText;
            int[] g_Width = { 120,120,120,120
                            };
            cgbb.grid_col_w = g_Width;


            Boolean[] g_ReadOnly = { true , true, true, true
                                   };
            cgbb.grid_col_Lock = g_ReadOnly;
            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                             ,DataGridViewContentAlignment.MiddleCenter
                             ,DataGridViewContentAlignment.MiddleCenter
                                };
            cgbb.grid_col_alignment = g_Alignment;
            

         

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();


            //gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgbb.grid_cell_format = gr_dic_cell_format;
        }


        private void dGridView_Base_Header_Reset()
        {
            
            cgb.grid_col_Count = 29;            
            cgb.basegrid = dGridView_Base;            
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;

            string[] g_HeaderText = {"주문_일자"  , "주문번호"   , "회원_번호"   , "성명"   , "등록_센타명"     
                                , "주문_센타명"   , "주문_종류"    , "상품명"   , "금액"    , "PV"

                                , "CV", "수량"   , "합계금액"  , "합계PV" , "합계BV"  
                               ,"상품비고"  , "현금결제액"     ,"현금결제일"   , "무통장결제액"  , "무통장결제일"

                                , "카드결제액"   , "카드결제일" , "카드사명", "카드번호", "승인번호"
                                , "승인일자"    , "할부"  ,"실입금액"       , "결제비고"
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 130, 90, 110, 90, 90  
                             ,130, 130, 130, 90, 80  
                             ,80 , 80, 80, 80, 80
                              ,80,80 , 90, 130 , 130
                           , 90  ,130 , 90 , 90 , 90
                            , 90 ,90, 90, 90
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true ,  true ,true  ,true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft                              
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight //10

                               ,DataGridViewContentAlignment.MiddleRight   
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight//15                             

                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleRight                              
                               ,DataGridViewContentAlignment.MiddleCenter //20


                               ,DataGridViewContentAlignment.MiddleRight //21
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter   
                               ,DataGridViewContentAlignment.MiddleCenter 

                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleCenter  
                              };
            cgb.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[12 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[13 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[14 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[15 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            gr_dic_cell_format[17 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[19 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[21 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[28 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb.grid_cell_format = gr_dic_cell_format;
            
        }


      


        private void Set_gr_dic(ref object[] row0 ,ref DataSet ds,  int fi_cnt, int S_TF = 0)
        {
            

            int Col_Cnt = 0;

            if (S_TF == 0)
            {
                while (Col_Cnt < 7)
                {
                    row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                    Col_Cnt++;
                }
            }
            else
            {
                while (Col_Cnt < 7)
                {
                    row0[Col_Cnt] = "";
                    Col_Cnt++;
                }
            }

            row0[7] = "";            row0[8] = 0;
            row0[9] = 0;            row0[10] = 0;
            row0[11] = 0;
            row0[12] = 0; row0[13] = 0; row0[14] = 0;  row0[15] = ""; // 여까지 상품임

            row0[16] = 0;            row0[17] = "";  //현금
            row0[18] = 0;            row0[19] = "";  //무통장
            row0[20] = 0;            row0[21] = "";  //하부 카드 관련

            row0[22] = "";            row0[23] = "";
            row0[24] = "";            row0[25] = "";
            row0[26] = "";

            row0[27] = 0;            row0[28] = "";

           

            //object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][1]  
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][2]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][3]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][4]
 
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][5]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][6]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][7]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][8]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][9]

            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][10]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][11]  
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][12]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][13]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][14]

            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][15]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][16]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][17]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][18]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][19]

            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][20]
            //                    //,ds.Tables[base_db_name].Rows[fi_cnt][21]  
            //                    //,ds.Tables[base_db_name].Rows[fi_cnt][22]
            //                    //,ds.Tables[base_db_name].Rows[fi_cnt][23]
            //                    //,ds.Tables[base_db_name].Rows[fi_cnt][24]

            //                    //,ds.Tables[base_db_name].Rows[fi_cnt][25]
            //                     };

            
        }



        private void Set_gr_dic_i(ref object[] row0, ref DataSet ds,
            string OrderNumber, int ReCnt, ref int F_cnt, ref int Af_Cnt_i ,
            ref int Total_i_1, ref int Total_i_2, ref int Total_i_3, ref int Total_i_4           
            )
        {
            int For_Cnt = 0;
            for (int Cnt = 0; Cnt <= ReCnt - 1; Cnt++)
            {
                if (ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][9].ToString() == OrderNumber)
                {
                    For_Cnt++;

                    if (For_Cnt > F_cnt)
                    {
                        row0[7] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][0];
                        row0[8] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][1];
                        row0[9] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][2];
                        row0[10] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][3];
                        row0[11] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][4];
                        row0[12] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][5];
                        row0[13] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][6];
                        row0[14] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][7];
                        row0[15] = ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][8];

                        Total_i_1 = Total_i_1 + int.Parse(ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][4].ToString());
                        Total_i_2 = Total_i_2 + int.Parse(ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][5].ToString());
                        Total_i_3 = Total_i_3 + int.Parse(ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][6].ToString());
                        Total_i_4 = Total_i_4 + int.Parse(ds.Tables["tbl_SalesItemDetail"].Rows[Cnt][7].ToString());



                        F_cnt++;

                        if  (Cnt + 1 <= ReCnt - 1)
                        {
                            if (ds.Tables["tbl_SalesItemDetail"].Rows[Cnt+1][9].ToString() == OrderNumber)
                            {
                                Af_Cnt_i++;
                            }
                        }

                        break;
                    }
                }

            }
        }


        private void Set_gr_dic_C(ref object[] row0, ref DataSet ds,
                string OrderNumber, int ReCnt, ref int F_cnt, ref int Af_Cnt_C ,
                ref int Total_C_1, ref int Total_C_2, ref int Total_C_3, ref int Total_C_4                
            )
        {
            int For_Cnt = 0;
            for (int Cnt = 0; Cnt <= ReCnt - 1; Cnt++)
            {
                if (ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][13].ToString() == OrderNumber)
                {
                     For_Cnt++;

                     if (For_Cnt > F_cnt)
                     {
                         row0[16] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][0];
                         row0[17] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][1];
                         row0[18] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][2];
                         row0[19] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][3];
                         row0[20] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][4];
                         row0[21] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][5];

                         row0[22] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][6];
                         row0[23] = encrypter.Decrypt ( ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][7].ToString () );
                         row0[24] = encrypter.Decrypt ( ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][8].ToString () );
                         row0[25] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][9];
                         row0[26] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][10];

                         row0[27] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][11];
                         row0[28] = ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][12];

                         Total_C_1 = Total_C_1 + int.Parse(ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][0].ToString());
                         Total_C_2 = Total_C_2 + int.Parse(ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][2].ToString());
                         Total_C_3 = Total_C_3 + int.Parse(ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][4].ToString());
                         Total_C_4 = Total_C_4 + int.Parse(ds.Tables["tbl_Sales_Cacu"].Rows[Cnt][11].ToString());


                         F_cnt++;
                         if (Cnt + 1 <= ReCnt - 1)
                         {
                             if (ds.Tables["tbl_Sales_Cacu"].Rows[Cnt + 1][13].ToString() == OrderNumber)
                             {
                                 Af_Cnt_C++;
                             }
                         }
                         
                         break;
                     }

                }
            }
        }




        private void Set_gr_dic_Total(ref  Dictionary<int, object[]> gr_dic_text, ref int Arr_Cnt,
          ref int Total_i_1, ref int Total_i_2, ref int Total_i_3, ref int Total_i_4,
          ref int Total_C_1, ref int Total_C_2, ref int Total_C_3, ref int Total_C_4 ,

          ref int S_Total_i_1, ref int S_Total_i_2, ref int S_Total_i_3 ,ref int  S_Total_i_4 ,
          ref int S_Total_C_1, ref int S_Total_C_2, ref int S_Total_C_3, ref int S_Total_C_4
          )
        {
            cls_form_Meth cm = new cls_form_Meth();

            object[] row_T = new object[cgb.grid_col_Count];

            int T_Col_Cnt = 0;
            while (T_Col_Cnt < 3)
            {
                row_T[T_Col_Cnt] = "";
                T_Col_Cnt++;
            }
            
            row_T[4] = "<< " + cm._chang_base_caption_search("소계") + " >>";
            
            T_Col_Cnt = 5;
            while (T_Col_Cnt < 7)
            {
                row_T[T_Col_Cnt] = "";
                T_Col_Cnt++;
            }

            row_T[7] = ""; row_T[8] = 0;
            row_T[9] = 0; 
            row_T[11] = Total_i_1;
            row_T[12] = Total_i_2;
            row_T[13] = Total_i_3;
            row_T[14] = Total_i_4; 
            row_T[15] = ""; // 여까지 상품임

            row_T[16] = Total_C_1; row_T[17] = "";  //현금
            row_T[18] = Total_C_2; row_T[19] = "";  //무통장
            row_T[20] = Total_C_3; row_T[21] = "";  //하부 카드 관련

            row_T[22] = ""; row_T[23] = "";
            row_T[24] = ""; row_T[25] = "";
            row_T[26] = "";

            row_T[27] = Total_C_4; row_T[28] = "";

            S_Total_i_1 = S_Total_i_1 + Total_i_1;
            S_Total_i_2 = S_Total_i_2 + Total_i_2;
            S_Total_i_3 = S_Total_i_3 + Total_i_3;
            S_Total_i_4 = S_Total_i_4 + Total_i_4;

            S_Total_C_1 = S_Total_C_1 + Total_C_1;
            S_Total_C_2 = S_Total_C_2 + Total_C_2;
            S_Total_C_3 = S_Total_C_3 + Total_C_3;
            S_Total_C_4 = S_Total_C_4 + Total_C_4;

            Total_C_1 = 0; Total_C_2 = 0; Total_C_3 = 0; Total_C_4 = 0;
            Total_i_1 = 0; Total_i_2 = 0; Total_i_3 = 0; Total_i_4 = 0;
            gr_dic_text[Arr_Cnt + 1] = row_T;
            Arr_Cnt++;
        }



        private void Set_gr_dic_Total(ref  Dictionary<int, object[]> gr_dic_text, ref int Arr_Cnt,
          ref int Total_i_1, ref int Total_i_2, ref int Total_i_3, ref int Total_i_4 ,
          ref int Total_C_1, ref int Total_C_2, ref int Total_C_3, ref int Total_C_4
          )
        {
            cls_form_Meth cm = new cls_form_Meth();

            object[] row_T = new object[cgb.grid_col_Count];

            int T_Col_Cnt = 0;
            while (T_Col_Cnt < 3)
            {
                row_T[T_Col_Cnt] = "";
                T_Col_Cnt++;
            }
            
            row_T[4] = "[[ " + cm._chang_base_caption_search("합계") + " ]]";

            T_Col_Cnt = 5;
            while (T_Col_Cnt < 7)
            {
                row_T[T_Col_Cnt] = "";
                T_Col_Cnt++;
            }

            row_T[7] = ""; row_T[8] = 0;
            row_T[9] = 0;
            row_T[10] = 0;
            row_T[11] = Total_i_1;
            row_T[12] = Total_i_2;
            row_T[13] = Total_i_3;
            row_T[14] = Total_i_4;

            txt_P_1.Text = string.Format(cls_app_static_var.str_Currency_Type, Total_i_2);
            txt_P_2.Text = string.Format(cls_app_static_var.str_Currency_Type, Total_i_3);
            txt_P_2_2.Text = string.Format(cls_app_static_var.str_Currency_Type, Total_i_4);
            
            // 여까지 상품임

            row_T[16] = Total_C_1; row_T[17] = "";  //현금
            row_T[18] = Total_C_2; row_T[19] = "";  //무통장
            row_T[20] = Total_C_3; row_T[21] = "";  //하부 카드 관련

            row_T[22] = ""; row_T[23] = "";
            row_T[24] = ""; row_T[25] = "";
            row_T[26] = "";

            row_T[27] = Total_C_4; row_T[28] = "";

            txt_P_4.Text = string.Format(cls_app_static_var.str_Currency_Type, Total_C_1);
            txt_P_6.Text = string.Format(cls_app_static_var.str_Currency_Type, Total_C_2);
            txt_P_5.Text = string.Format(cls_app_static_var.str_Currency_Type, Total_C_4);

            Total_C_1 = 0; Total_C_2 = 0; Total_C_3 = 0; Total_C_4 = 0;
            Total_i_1 = 0; Total_i_2 = 0; Total_i_3 = 0; Total_i_4 = 0;
            gr_dic_text[Arr_Cnt + 1] = row_T;
            Arr_Cnt++;

            
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
                SendKeys.Send("{TAB}");
            }
        }



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




        private void mtxtMbid_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            MaskedTextBox tb = (MaskedTextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
            }
        }


        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);            
            T_R.Key_Enter_13_Ncode += new Key_13_Ncode_Event_Handler(T_R_Key_Enter_13_Ncode);

            TextBox tb = (TextBox)sender;

            if ((tb.Tag == null) || (tb.Tag.ToString() == ""))
            {
                //숫자만 입력 가능하다.
                if (T_R.Text_KeyChar_Check(e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if   
            }
            else if ((tb.Tag != null) && (tb.Tag.ToString() == "1"))
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, 1) == false)
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

        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            int Sw_Tab = 0;

            if ((sender is TextBox) == false)  return;

            TextBox tb = (TextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
                Sw_Tab = 1;
            }

            if (tb.Name == "txtCenter")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtCenter_Code.Text = "";
                Data_Set_Form_TF = 0 ; 
            }

            if (tb.Name == "txtBank")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtSellCode_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtR_Id")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtR_Id_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtCenter2")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtCenter2_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtSellCode")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtSellCode_Code.Text = "";
                Data_Set_Form_TF = 0;
            }
        }

        

        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }


        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {            
            if (tb.Name == "txtCenter")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtCenter_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtCenter_Code,"");
                //else
                //    Ncod_Text_Set_Data(tb, txtCenter_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtR_Id")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtR_Id_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtR_Id_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txtR_Id_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtBank")
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
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txtCenter2")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txtBank")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txtR_Id")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txtChange")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txtSellCode")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txt_Base_Rec")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txt_Receive_Method")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txt_ItemCode")
                cgb_Pop.Next_Focus_Control = butt_Select;

            cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, cls_User.gid_CountryCode);
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
            cgb_Pop.Base_tb_2 = tb ;    //2번은 명임
            cgb_Pop.Base_Location_obj = tb;

            if (strSql != "")
            {
                if (tb.Name == "txtCenter")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtR_Id")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txtBank")
                    cgb_Pop.db_grid_Popup_Base(2, "은행_코드", "은행명", "Ncode", "BankName", strSql);

                if (tb.Name == "txtCenter2")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);
           
                if (tb.Name == "txtSellCode")
                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", strSql);
            }
            else
            {
                if (tb.Name == "txtCenter")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                    if (cls_User.gid_CountryCode != "") Tsql = Tsql + " And  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " And  ShowMemberCenter = 'Y' ";
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

                if (tb.Name == "txtBank")
                {
                    string Tsql;
                    Tsql = "Select Ncode ,BankName    ";
                    Tsql = Tsql + " From tbl_Bank (nolock) ";
                    if (cls_User.gid_CountryCode != "") Tsql = Tsql + " Where  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "은행_코드", "은행명", "Ncode", "BankName", Tsql);
                }

                if (tb.Name == "txtCenter2")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                    if (cls_User.gid_CountryCode != "") Tsql = Tsql + " And  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " And  ShowOrderCenter = 'Y' ";
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                }                                

                if (tb.Name == "txtSellCode")
                {
                    string Tsql;
                    Tsql = "Select SellCode ,SellTypeName    ";
                    Tsql = Tsql + " From tbl_SellType (nolock) ";
                    Tsql = Tsql + " Order by SellCode ";

                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", Tsql);
                }

            }
        }



        private void Ncod_Text_Set_Data(TextBox tb, TextBox tb1_Code)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql="";
            
            if (tb.Name == "txtCenter")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                if (cls_User.gid_CountryCode != "") Tsql = Tsql + " And  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
            }

            if (tb.Name == "txtR_Id")
            {
                Tsql = "Select user_id ,U_Name   ";
                Tsql = Tsql + " From tbl_User (nolock) ";
                Tsql = Tsql + " Where U_Name like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    user_id like '%" + tb.Text.Trim() + "%'";
            }

            if (tb.Name == "txtBank")
            {
                Tsql = "Select Ncode , BankName   ";
                Tsql = Tsql + " From tbl_Bank (nolock) ";
                Tsql = Tsql + " Where (Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    BankName like '%" + tb.Text.Trim() + "%')";
                if (cls_User.gid_CountryCode != "") Tsql = Tsql + " And  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
            }


            if (tb.Name == "txtCenter2")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                if (cls_User.gid_CountryCode != "") Tsql = Tsql + " And  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
            }
          

            if (tb.Name == "txtSellCode")
            {
                Tsql = "Select SellCode ,SellTypeName    ";
                Tsql = Tsql + " From tbl_SellType (nolock) ";
                Tsql = Tsql + " Where SellCode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    SellTypeName like '%" + tb.Text.Trim() + "%'";
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







        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Clear")
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();            
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                cls_form_Meth ct = new cls_form_Meth();
                ct.from_control_clear(this, mtxtMbid);

                opt_Ed_1.Checked = true;  opt_sell_1.Checked = true;
                combo_Se.SelectedIndex = -1;

                
            }
            else if (bt.Name == "butt_Select")
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();
                txt_P_1.Text = ""; txt_P_2.Text = ""; txt_P_2_2.Text = "";
                txt_P_3.Text = ""; txt_P_4.Text = ""; txt_P_5.Text = "";
                txt_P_6.Text = ""; txt_P_7.Text = "";
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                if (Check_TextBox_Error() == false) return;

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex; 
                Base_Grid_Set();  //뿌려주는 곳
                this.Cursor = System.Windows.Forms.Cursors.Default;

            }
           
            else if (bt.Name == "butt_Excel")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info);
                e_f.ShowDialog();
            }

            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }

            else if (bt.Name  == "butt_Exp")
            {
                if (bt.Text == "...")
                {
                    grB_Search.Height = button_base.Top + button_base.Height + 3;
                    bt.Text = ".";
                }
                else
                {
                    grB_Search.Height = butt_Exp.Top + butt_Exp.Height + 3;
                    bt.Text = "...";
                }
            }

        }


        private DataGridView e_f_Send_Export_Excel_Info(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "Sell_Select";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;
        }

       

 

        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
            //SendKeys.Send("{TAB}");
        }




        private void radioB_S_Base_Click(object sender, EventArgs e)
        {
            //RadioButton _Rb = (RadioButton)sender;
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtSellDate1, mtxtSellDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }


        private void radioB_R_Base_Click(object sender, EventArgs e)
        {
            Data_Set_Form_TF = 1;
            //RadioButton _Rb = (RadioButton)sender;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtMakDate1, mtxtMakDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }





        //private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        //{
        //    if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
        //    {
        //        string Send_Nubmer = ""; string Send_Name = ""; ; string Send_OrderNumber = "";
        //        Send_OrderNumber = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
        //        Send_Nubmer = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();
        //        Send_Name = (sender as DataGridView).CurrentRow.Cells[4].Value.ToString();
        //        Send_Mem_Number(Send_Nubmer, Send_Name, Send_OrderNumber);   //부모한테 이벤트 발생 신호한다.
        //    }
        //}





        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[1].Value != null))
            {
                string Send_Nubmer = ""; string Send_Name = ""; ; string Send_OrderNumber = "";
                Send_OrderNumber = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                Send_Nubmer = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                Send_Name = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();

                if (Send_OrderNumber == "") return; 

                Send_Sell_Number(Send_Nubmer, Send_Name, Send_OrderNumber);   //부모한테 이벤트 발생 신호한다.
            }
        }

        private int but_Exp_Base_Left = 0;
        private int Parent_but_Exp_Base_Width = 0;

        private void but_Exp_Click(object sender, EventArgs e)
        {
            if (but_Exp.Text == "<<")
            {
                Parent_but_Exp_Base_Width = but_Exp.Parent.Width;
                but_Exp_Base_Left = but_Exp.Left;

                but_Exp.Parent.Width = but_Exp.Width;
                but_Exp.Left = 0;
                but_Exp.Text = ">>";
            }
            else
            {
                but_Exp.Parent.Width = Parent_but_Exp_Base_Width;
                but_Exp.Left = but_Exp_Base_Left;
                but_Exp.Text = "<<";
            }
        }

        private void dGridView_Base_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string mbid2 = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
            if (mbid2 == "")
            {
                return;
            }
            Base_Grid_Set2(mbid2);  //뿌려주는 곳

        }
    }
}
