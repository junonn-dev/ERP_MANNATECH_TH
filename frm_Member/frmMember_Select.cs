﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;

namespace MLM_Program
{
    public partial class frmMember_Select : Form
    {

        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Sub = new cls_Grid_Base();
        cls_Grid_Base cgb_Sub_Add = new cls_Grid_Base();
        private const string base_db_name = "tbl_Memberinfo";
        private int Data_Set_Form_TF;

        public delegate void SendNumberDele(string Send_Number, string Send_Name);
        public event SendNumberDele Send_Mem_Number;

        
        private Series series_Item = new Series();

        public frmMember_Select()
        {
            InitializeComponent();

            DoubleBuffered = true;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dGridView_Base, new object[] { true });
        }           


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;

            cls_Pro_Base_Function cpbf = new cls_Pro_Base_Function();
            cpbf.Put_Close_Grade_ComboBox(combo_Grade, combo_Grade_Code);
            cpbf.Put_Close_GradeP_ComboBox(combo_GradeP, combo_GradeP_Code);

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Sub.d_Grid_view_Header_Reset(1);

            dGridView_Base_Sub_Add_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Sub_Add.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);


            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtMbid2.Mask = cls_app_static_var.Member_Number_Fromat;

            mtxtMakDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtMakDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtRegDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtRegDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtEduDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtEduDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtRBODate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtRBODate2.Mask = cls_app_static_var.Date_Number_Fromat;

            Reset_Chart_Total();
            radioB_SMT.Checked = true;

            mtxtMbid.Focus();
           
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


        private void frmBase_Resize(object sender, EventArgs e)
        {

            butt_Clear.Left = 0;
            butt_Select.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Excel.Left = butt_Select.Left + butt_Select.Width + 2;
            butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Print_MembershipCard.Left = butt_Delete.Left + butt_Delete.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Select);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Print_MembershipCard);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);
            cfm.button_flat_change(butt_Excel_Sub);  
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

                            //cls_form_Meth cfm = new cls_form_Meth();
                            //cfm.form_Group_Panel_Enable_True(this);
                        }
                    }
                }// end if

            }

            ////그리드일 경우에는 DEL키로 행을 삭제하는걸 막는다.
            //if (sender is DataGridView)
            //{
            //    if (e.KeyValue == 13)
            //    {
            //        EventArgs ee =null;
            //        dGridView_Base_DoubleClick(sender, ee);
            //        e.Handled = true;
            //    } // end if
            //}

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



            if (mtxtRegDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtRegDate1.Text, mtxtRegDate1, "Date") == false)
                {
                    mtxtRegDate1.Focus();
                    return false;
                }
            }

            if (mtxtRegDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtRegDate2.Text, mtxtRegDate2, "Date") == false)
                {
                    mtxtRegDate2.Focus();
                    return false;
                }

            }


            if (mtxtRBODate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtRBODate1.Text, mtxtRBODate1, "Date") == false)
                {
                    mtxtRBODate1.Focus();
                    return false;
                }
            }

            if (mtxtRBODate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtRBODate2.Text, mtxtRBODate2, "Date") == false)
                {
                    mtxtRBODate2.Focus();
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


            if (mtxtEduDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtEduDate1.Text, mtxtEduDate1, "Date") == false)
                {
                    mtxtMakDate1.Focus();
                    return false;
                }
            }

            if (mtxtEduDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtEduDate2.Text, mtxtEduDate2, "Date") == false)
                {
                    mtxtMakDate2.Focus();
                    return false;
                }

            }


                        
           

            return true;
        }
        /// <summary> Col MBID2, SellDate </summary>
        private void Make_Base_Query_MinSellDate(ref string Tsql)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select MBID2 , convert(varchar, cast(min(selldate) as datetime), 23) SellDate");
            sb.AppendLine("FROM tbl_SalesDetail (nolock)");
            sb.AppendLine("GROUP BY MBID2 ");

            Tsql = sb.ToString();
        
        }


        private void Make_Base_Query(ref string Tsql)
        {

            //string[] g_HeaderText = {"회원_번호"  , "성명"   , "주민번호"  , "현직급"   , "위치"        
            //                    , "센타명"   , "가입일"    , "집전화"   , "핸드폰"    , "교육일"
            //                    , "후원인"   , "후원인명"  , "추천인"   , "추천인명"   ,"우편_번호"
            //                    , "주소"     , "은행명"    , "계좌번호" , "예금주"     , "구분"
            //                    , "활동_여부", "중지_여부"  , "탈퇴일"  , "라인중지일"  ,"기록자"
            //                    , "기록일", "이메일"
            //                        };

            cls_form_Meth cm = new cls_form_Meth();
            //cm._chang_base_caption_search(m_text);

            Tsql = "Select  ";
            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql += Environment.NewLine + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) ";
            else
                Tsql += Environment.NewLine + " tbl_Memberinfo.mbid2 ";

            Tsql += Environment.NewLine + " ,tbl_Memberinfo.M_Name ";
            Tsql += Environment.NewLine + " ,tbl_Memberinfo.birthday + '-' + tbl_Memberinfo.birthday_M+ '-' + tbl_Memberinfo.birthday_d ";
            //Tsql += Environment.NewLine + ",  tbl_Memberinfo.Cpno  ";
      
            Tsql += Environment.NewLine + " , case when tbl_Memberinfo.For_Kind_TF = 0 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay < 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '2' ELSE '1' END) +'******'";
            Tsql += Environment.NewLine + "        when tbl_Memberinfo.For_Kind_TF = 0 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay > 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '4' ELSE '3' END) +'******'";
            Tsql += Environment.NewLine + "        when tbl_Memberinfo.For_Kind_TF = 1 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay < 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '6' ELSE '5' END) +'******'";
            Tsql += Environment.NewLine + "        when tbl_Memberinfo.For_Kind_TF = 1 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay > 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '8' ELSE '7' END) +'******'";
            Tsql += Environment.NewLine + "        ELSE dbo.DECRYPT_AES256(tbl_Memberinfo.Cpno) END CPNO";

            Tsql += Environment.NewLine + " , case when tbl_Memberinfo.For_Kind_TF = 0 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay < 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '2' ELSE '1' END) +'******'";
            Tsql += Environment.NewLine + "        when tbl_Memberinfo.For_Kind_TF = 0 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay > 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '4' ELSE '3' END) +'******'";
            Tsql += Environment.NewLine + "        when tbl_Memberinfo.For_Kind_TF = 1 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay < 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '6' ELSE '5' END) +'******'";
            Tsql += Environment.NewLine + "        when tbl_Memberinfo.For_Kind_TF = 1 AND tbl_Memberinfo.Cpno = '' and tbl_Memberinfo.BirthDay > 2000 and len(tbl_Memberinfo.BirthDay ) = 4 then right(tbl_Memberinfo.BirthDay,2) +tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D + (case tbl_Memberinfo.Sex_FLAG WHEN 'X' THEN '8' ELSE '7' END) +'******'";
            if(cls_User.gid_Cpno_V_TF == 1)
                Tsql += Environment.NewLine + "       ELSE dbo.DECRYPT_AES256(tbl_Memberinfo.Cpno) END CPNO2";
             else
                Tsql += Environment.NewLine + "        ELSE  tbl_Memberinfo.Cpno END CPNO2";




            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.sell_mem_tf = 1 then '프리퍼드커스터머' ELSE ISNULL(C1.Grade_Name,'')  END ";

            //Tsql += Environment.NewLine + " , CC_A.G_Name ";
            //Tsql += Environment.NewLine + " , tbl_Memberinfo.LineCnt ";
            //Tsql += Environment.NewLine + " , tbl_Memberinfo.C_M_Name ";
            Tsql += Environment.NewLine + " , '' as 'FirstOrderDate'";


            Tsql += Environment.NewLine + " ,Isnull(tbl_Business.Name,'') as B_Name";
            Tsql += Environment.NewLine + " , LEFT(tbl_Memberinfo.RegTime,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.RegTime,4),2) + '-' + RIGHT(tbl_Memberinfo.RegTime,2)   ";



            Tsql += Environment.NewLine + " , tbl_Memberinfo.hometel ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.hptel ";
            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.Ed_Date <> '' Then  LEFT(tbl_Memberinfo.Ed_Date,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.Ed_Date,4),2) + '-' + RIGHT(tbl_Memberinfo.Ed_Date,2) ELSE '' End Ed_Date_2 ";


            Tsql += Environment.NewLine + " , ISNULL(CP.Grade_Name,'')   ";

            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql += Environment.NewLine + " ,tbl_Memberinfo.Nominid + '-' + Convert(Varchar,tbl_Memberinfo.Nominid2) ";
            else
                Tsql += Environment.NewLine + " ,tbl_Memberinfo.Nominid2 ";

            
            Tsql += Environment.NewLine + " , Isnull(Nom.M_Name,'') ";


            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql += Environment.NewLine + " ,tbl_Memberinfo.Saveid + '-' + Convert(Varchar,tbl_Memberinfo.Saveid2) ";
            else
                Tsql += Environment.NewLine + " ,tbl_Memberinfo.Saveid2 ";

            Tsql += Environment.NewLine + " , Isnull(Sav.M_Name,'') ";
            if (chkShowSaveDefault.Checked)
                Tsql = Tsql + ", (SELECT TOP 1 ISNULL(CAST(MBID2 AS VARCHAR), '') + '_' + ISNULL(M_NAME, '') AS SaveDefault  FROM [ufn_Up_Search_Save_Gold] ('', tbl_Memberinfo.mbid2) ) as SaveDefault " + Environment.NewLine; 
            else
                Tsql = Tsql + ", '' as SaveDefault " + Environment.NewLine;

            Tsql += Environment.NewLine + " , tbl_Memberinfo.Addcode1 ";

            Tsql += Environment.NewLine + " , tbl_Memberinfo.address1";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.address2";
            Tsql += Environment.NewLine + " , tbl_Bank.BankName";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.bankaccnt";            
            Tsql += Environment.NewLine + " , tbl_Memberinfo.bankowner";
            Tsql += Environment.NewLine + " , Case  When tbl_Memberinfo.Sell_Mem_TF = 0 then '" + cm._chang_base_caption_search("판매원") + "' ELSE  '" + cm._chang_base_caption_search("소비자") + "' End AS Sell_MEM_TF2";


            Tsql += Environment.NewLine + " , Case  " ;
            Tsql += Environment.NewLine + "  When tbl_Memberinfo.LeaveCheck = 1 Then '" + cm._chang_base_caption_search("활동") + "'" ;
            Tsql += Environment.NewLine + "  When tbl_Memberinfo.LeaveCheck = 0 Then '" + cm._chang_base_caption_search("탈퇴") + "'";
            Tsql += Environment.NewLine + "  When tbl_Memberinfo.LeaveCheck = -1 Then '" + cm._chang_base_caption_search("직권해지") + "'";
            Tsql += Environment.NewLine + "  When tbl_Memberinfo.LeaveCheck = -100 Then '" + cm._chang_base_caption_search("휴면(*자동탈퇴)") + "'";
            Tsql += Environment.NewLine + "  End AS LeaveCheck_2 ";

            //Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.LineUserDate = '' Then '" + cm._chang_base_caption_search("사용") + "' ELSE '" + cm._chang_base_caption_search("중지") + "' End ";
            //Tsql += Environment.NewLine + " , tbl_Memberinfo.GiBu_ ";
            
            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.LeaveDate <> '' Then  LEFT(tbl_Memberinfo.LeaveDate,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.LeaveDate,4),2) + '-' + RIGHT(tbl_Memberinfo.LeaveDate,2) ELSE '' End ";
            

            Tsql += Environment.NewLine + " , tbl_Memberinfo.recordid ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.recordtime ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.Email ";
            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.AgreeEmail = 'Y' then 'V' ELSE '' END ";
            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.AgreeSMS = 'Y' then 'V' ELSE '' END ";

            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.VisaDate <> '' Then  Convert(varchar(8), cast(tbl_Memberinfo.VisaDate AS Datetime) , 112) ELSE '' End ";
            Tsql += Environment.NewLine + " , Case When tbl_Memberinfo.LineUserDate <> '' Then  LEFT(tbl_Memberinfo.LineUserDate,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.LineUserDate,4),2) + '-' + RIGHT(tbl_Memberinfo.LineUserDate,2) ELSE '' End ";
            Tsql += Environment.NewLine + ", isnull(tbl_Card.cardname,'') Card_Name";
            Tsql += Environment.NewLine + ", isnull(MAuto.A_CardNumber,'') A_CardNumber ";
            Tsql += Environment.NewLine + ", isnull(MAuto.A_Month_Date,'') A_Month_Date ";
            Tsql += Environment.NewLine + ", isnull(MAuto.A_Stop_Date,'') A_Stop_Date ";

            Tsql += Environment.NewLine + " ,0 "; 




            Tsql += Environment.NewLine + " , tbl_Memberinfo.Sex_FLAG ";


            Tsql += Environment.NewLine + " , tbl_Memberinfo.BirthDay + '-' +  tbl_Memberinfo.BirthDay_M + '-' +  tbl_Memberinfo.BirthDay_D  BirthDay ";

            Tsql += Environment.NewLine + " , CASE tbl_Memberinfo.IDCARD_CONFIRM_FLAG WHEN 0 THEN 'N' WHEN 1 THEN 'Y' END ";
            Tsql += Environment.NewLine + " , CASE tbl_Memberinfo.BANKBOOK_CONFIRM_FLAG WHEN 0 THEN 'N' WHEN 1 THEN 'Y' END ";





            Tsql += Environment.NewLine + " From tbl_Memberinfo (nolock) ";
            Tsql += Environment.NewLine + " LEFT JOIN tbl_Memberinfo Sav (nolock) ON tbl_Memberinfo.Saveid = Sav.Mbid And tbl_Memberinfo.Saveid2 = Sav.Mbid2 ";
            Tsql += Environment.NewLine + " LEFT JOIN tbl_Memberinfo Nom (nolock) ON tbl_Memberinfo.Nominid = Nom.Mbid And tbl_Memberinfo.Nominid2 = Nom.Mbid2 ";

            Tsql += Environment.NewLine + " LEFT JOIN tbl_Memberinfo_A MAuto (nolock) ON MAuto.Mbid = tbl_Memberinfo.Mbid And MAuto.Mbid2 = tbl_Memberinfo.Mbid2 ";
            Tsql += Environment.NewLine + " LEFT JOIN tbl_ClosePay_04 C02  (nolock) ON C02.Mbid = tbl_Memberinfo.Mbid And C02.Mbid2 = tbl_Memberinfo.Mbid2  ";
            //Tsql += Environment.NewLine + " LEFT JOIN tbl_ClosePay_01 C01  (nolock) ON C01.Mbid = tbl_Memberinfo.Mbid And C01.Mbid2 = tbl_Memberinfo.Mbid2  ";
            
            Tsql += Environment.NewLine + " LEFT JOIN tbl_Card (nolock) ON tbl_Card.Ncode = MAuto.A_CardCode "; 

            Tsql += Environment.NewLine + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode  And tbl_Memberinfo.Na_code = tbl_Business.Na_code";
            //Tsql += Environment.NewLine + " Left Join tbl_Bank (nolock) On tbl_Memberinfo.bankcode=tbl_Bank.ncode  And tbl_Memberinfo.Na_code = tbl_Bank.Na_code ";
            Tsql += Environment.NewLine + " Left Join tbl_Bank (nolock) On tbl_Memberinfo.bankcode=tbl_Bank.ncode ";
            cls_NationService.SQL_BankNationCode(ref Tsql);
            Tsql += Environment.NewLine + " Left Join tbl_Class C1  (nolock) On tbl_Memberinfo.CurGrade=C1.Grade_Cnt ";
            Tsql += Environment.NewLine + " Left Join tbl_Class_P CP (nolock) On tbl_Memberinfo.CurPoint = CP.Grade_Cnt ";
           // Tsql += Environment.NewLine + " Left Join ufn_Mem_CurGrade_Mbid_Search ('',0) AS CC_A On CC_A.Mbid = tbl_Memberinfo.Mbid And  CC_A.Mbid2 = tbl_Memberinfo.Mbid2 ";            
        }



        private void Make_Base_Query_(ref string Tsql)
        {
            string strSql = " Where tbl_Memberinfo.Saveid2 >= 0  ";
            //// strSql += Environment.NewLine + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";

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
                    strSql += Environment.NewLine + " And tbl_Memberinfo.Mbid = '" + Mbid + "'";
                    strSql += Environment.NewLine + " And tbl_Memberinfo.Mbid2 = " + Mbid2;
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
                        strSql += Environment.NewLine + " And tbl_Memberinfo.Mbid >='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql += Environment.NewLine + " And tbl_Memberinfo.Mbid2 >= " + Mbid2;
                }

                if (csb.Member_Nmumber_Split(mtxtMbid2.Text, ref Mbid, ref Mbid2) == 1)
                {
                    if (Mbid != "")
                        strSql += Environment.NewLine + " And tbl_Memberinfo.Mbid <='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql += Environment.NewLine + " And tbl_Memberinfo.Mbid2 <= " + Mbid2;
                }
            }


            //회원명으로 검색
            if (txtName.Text.Trim() != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.M_Name Like '%" + txtName.Text.Trim() + "%'";

            //가입일자로 검색 -1

            if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() == ""))
                strSql += Environment.NewLine + " And tbl_Memberinfo.RegTime = '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";

            //가입일자로 검색 -2
            if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql += Environment.NewLine + " And tbl_Memberinfo.RegTime >= '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";
                strSql += Environment.NewLine + " And tbl_Memberinfo.RegTime <= '" + mtxtRegDate2.Text.Replace("-", "").Trim() + "'";
            }


            //기록일자로 검색 -1
            if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() == ""))
                strSql += Environment.NewLine + " And Replace(Left( tbl_Memberinfo.recordtime ,10),'-','') = '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'";

            //기록일자로 검색 -2
            if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql += Environment.NewLine + " And Replace(Left( tbl_Memberinfo.recordtime ,10),'-','') >= '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'";
                strSql += Environment.NewLine + " And Replace(Left( tbl_Memberinfo.recordtime ,10),'-','') <= '" + mtxtMakDate2.Text.Replace("-", "").Trim() + "'";
            }


            //교육일자로 검색 -1
            if ((mtxtEduDate1.Text.Replace("-", "").Trim() != "") && (mtxtEduDate2.Text.Replace("-", "").Trim() == ""))
                strSql += Environment.NewLine + " And tbl_Memberinfo.Ed_Date = '" + mtxtEduDate1.Text.Replace("-", "").Trim() + "'";

            //교육일자로 검색 -2
            if ((mtxtEduDate1.Text.Replace("-", "").Trim() != "") && (mtxtEduDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql += Environment.NewLine + " And tbl_Memberinfo.Ed_Date  >= '" + mtxtEduDate1.Text.Replace("-", "").Trim() + "'";
                strSql += Environment.NewLine + " And tbl_Memberinfo.Ed_Date  <= '" + mtxtEduDate2.Text.Replace("-", "").Trim() + "'";
            }


            if ((mtxtRBODate1.Text.Replace("-", "").Trim() != "") && (mtxtRBODate2.Text.Replace("-", "").Trim() == ""))
                strSql += Environment.NewLine + " And tbl_Memberinfo.RBO_S_Date = '" + mtxtRBODate1.Text.Replace("-", "").Trim() + "'";

            //교육일자로 검색 -2
            if ((mtxtRBODate1.Text.Replace("-", "").Trim() != "") && (mtxtRBODate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql += Environment.NewLine + " And tbl_Memberinfo.RBO_S_Date  >= '" + mtxtRBODate1.Text.Replace("-", "").Trim() + "'";
                strSql += Environment.NewLine + " And tbl_Memberinfo.RBO_S_Date  <= '" + mtxtRBODate2.Text.Replace("-", "").Trim() + "'";
            }
            

            //센타코드로으로 검색
            if (txtCenter_Code.Text.Trim() != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.BusinessCode = '" + txtCenter_Code.Text.Trim() + "'";

            if (txt_Us.Text.Trim() != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.Us_Num = '" + txt_Us.Text.Trim() + "'";

            if (txtR_Id_Code.Text.Trim() != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.recordid = '" + txtR_Id_Code.Text.Trim() + "'";

            if (txtBank_Code.Text.Trim() != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.BankCode = '" + txtBank_Code.Text.Trim() + "'";

            if (opt_Leave_2.Checked== true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.LeaveDate = ''";

            if (opt_Leave_3.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.LeaveCheck = 0 ";

            if (opt_Leave_100.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.LeaveCheck = -100";

            if (opt_Line_2.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.LineUserDate = ''";

            if (opt_Line_3.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.LineUserDate <> ''";



            if (opt_sell_2.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Sell_Mem_TF = 0 ";

            if (opt_sell_3.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Sell_Mem_TF = 1 ";


            if (radioB_R2.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.RBO_Mem_TF = 0 ";

            if (radioB_R3.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.RBO_Mem_TF = 1 ";


            if (radioB_SMT_0.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Sell_Mem_TF = 0 ";

            if (radioB_SMT_1.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Sell_Mem_TF = 1 ";

            


            if (radioB_F0.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.For_Kind_TF = 0 ";

            if (radioB_F1.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.For_Kind_TF = 1 ";
            


            if (combo_Grade_Code.Text != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.CurGrade = " + combo_Grade_Code.Text;

            if (combo_GradeP_Code.Text != "")
                strSql += Environment.NewLine + " And tbl_Memberinfo.CurPoint = " + combo_GradeP_Code.Text;
            
           
            if (radio_S2.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Us_Num >0 ";

            if (radio_S3.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Us_Num = 0 " ;



            if (opt_Ed_2.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Ed_Date <> ''";

            if (opt_Ed_3.Checked == true)
                strSql += Environment.NewLine + " And tbl_Memberinfo.Ed_Date = ''";

            //if (txtCpno.Text.Trim() != "")
            //    strSql += Environment.NewLine + " And tbl_Memberinfo.Cpno   = '" + encrypter.Encrypt(txtCpno.Text.Trim() )+ "'";
            if (txtCpno.Text.Trim() != "")
                strSql += Environment.NewLine + " And (tbl_Memberinfo.BirthDay + tbl_Memberinfo.BirthDay_M + tbl_Memberinfo.BirthDay_D ) like '%" + txtCpno.Text.Trim() + "%'";


            if (txtPNumber.Text.Trim() != "")
            {
                //tbl_Memberinfo.hometel ) 
                strSql += Environment.NewLine + " And (charindex ( '" + txtPNumber.Text.Replace(" ", "").Replace("-", "").ToString() + "', Replace(tbl_Memberinfo.hometel  ,'-','')) >0 ";
                strSql += Environment.NewLine + " OR charindex ('" + txtPNumber.Text.Replace(" ", "").Replace("-", "").ToString() + "',  Replace(tbl_Memberinfo.hptel  ,'-','')) >0 ) ";
                //strSql += Environment.NewLine + " And ( tbl_Memberinfo.hometel  = '" + encrypter.Encrypt(txtPNumber.Text.Replace(" ", "") .ToString()) + "' ";
                //strSql += Environment.NewLine + " OR   tbl_Memberinfo.hptel   = '" + encrypter.Encrypt(txtPNumber.Text.Replace(" ", "").ToString()) + "' ) ";
            }

            strSql += Environment.NewLine + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            strSql += Environment.NewLine + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

            Tsql = Tsql + strSql ;
            Tsql = Tsql + " Order by tbl_Memberinfo.Mbid, tbl_Memberinfo.Mbid2 ASC ";
        }

        bool Make_Base_DataSet(ref DataSet ds)
        {
            if(txtCpno.Text.Trim().Length > 0 )
            {
                DataTable TempTable = ds.Tables[base_db_name].Copy();
                ds.Tables[base_db_name].Rows.Clear();

                foreach(DataRow row in TempTable.Rows)
                {

                    string name = row["M_Name"].ToString();
                    string cpno = row["cpno"].ToString(); 

                    cpno = encrypter.Decrypt(cpno, "Cpno");
                 
                    if (cpno.Trim().Replace("-", "").Equals(string.Empty)) 
                        continue;
                    else if(cpno.Contains(txtCpno.Text))
                        ds.Tables[base_db_name].ImportRow(row);


                }

                
            }

            return ds.Tables[base_db_name].Rows.Count != 0;
        }


        private void Base_Grid_Set()
        {
            string Tsql = string.Empty;
            string Tsql_MinSelldate = string.Empty;
            
            Make_Base_Query(ref Tsql);

            Make_Base_Query_(ref Tsql);

            Make_Base_Query_MinSellDate(ref Tsql_MinSelldate);

            cls_form_Meth cm = new cls_form_Meth();
            //cm._chang_base_caption_search(m_text);

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();                                  
            
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name , this.Text ) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            DataSet ds_MinSellDate = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql_MinSelldate, base_db_name, ds_MinSellDate, this.Name, this.Text) == false) return;

            foreach(DataRow mainRow in ds.Tables[base_db_name].Rows)
            {
                foreach(DataRow subRow in ds_MinSellDate.Tables[base_db_name].Rows)
                {
                    if(mainRow["mbid2"].ToString() == subRow["mbid2"].ToString())
                    {
                        mainRow["FirstOrderDate"] = subRow["SellDate"].ToString();
                        break;
                    }
                }
                mainRow["Address2"] = string.Format("{0} {1}", mainRow["address1"].ToString(), mainRow["address2"].ToString());

              // mainRow[20] = encrypter.Decrypt(mainRow[20].ToString());
              // mainRow[28] = encrypter.Decrypt(mainRow[28].ToString());
                

            }

            //주민번호검색 추가
            if (Make_Base_DataSet(ref ds))
                ReCnt = ds.Tables[base_db_name].Rows.Count;
            else  
                return;

            double SellCnt_1 = 0; double SellCnt_2 = 0;
            double MemCnt_1 = 0; double MemCnt_2 = 0;
            double EdCnt_1 = 0; double EdCnt_2 = 0; 
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, double> Center_MemCnt = new Dictionary<string, double>();

            cls_Pro_Base_Function cpbf = new cls_Pro_Base_Function();
            Dictionary<string, cls_Memb_Area> dic_Pay = new Dictionary<string, cls_Memb_Area>();
            Dictionary<string, int> dic_Add = new Dictionary<string, int>();

            cls_Sn_Check csn_C = new cls_Sn_Check();

            string Per_t = "";
            int W_M_TF = 0;
            string Y_Per = "" ;

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                //Per_t = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["address1"].ToString());
                Per_t = ds.Tables[base_db_name].Rows[fi_cnt]["address1"].ToString();

                string[] T_ad = Per_t.Split(' ');

                string ADD = "";
                int ssCnt = 0;
                for (int i = 0; i < T_ad.Length; i++)
                {
                    if (T_ad[i].Replace(" ", "") != "")
                        ssCnt++; 

                    ADD = ADD  + T_ad[i].Replace(" " ,"");

                    if (ssCnt == 2)
                        break; 
                }
                //cpbf.Put_Address_Sort_Area(Per_t, ref ADD);

                if (ADD == "")
                {
                    ssCnt = 0;
                    
                    //Per_t = encrypter.Decrypt(ds.Tables[base_db_name].Rows[fi_cnt]["address2"].ToString());
                    Per_t = ds.Tables[base_db_name].Rows[fi_cnt]["address2"].ToString();
                    
                    string[] T_ad2 = Per_t.Split(' ');

                    for (int i = 0; i < T_ad2.Length; i++)
                    {
                        if (T_ad2[i].Replace(" ", "") != "")
                            ssCnt++;

                        ADD = ADD  + T_ad2[i].Replace(" ", "");

                        if (ssCnt == 2)
                            break;
                    }

                    //ADD = "";
                   // cpbf.Put_Address_Sort_Area(Per_t, ref ADD);
                }
                if (ADD != "")
                {
                    string Gun = ADD.Substring(ADD.Length - 1, 1);

                    if (Gun == "군" || Gun == "시")
                        ADD = ADD.Substring(0, ADD.Length - 1);
                }

                if (dic_Add.ContainsKey(ADD) == true)
                    dic_Add[ADD]++;
                else
                    dic_Add[ADD] = 1;
                

                ADD = "";

                W_M_TF = 0 ;

                if (ds.Tables[base_db_name].Rows[fi_cnt]["Cpno"].ToString() != "")
                {
                    Per_t = ds.Tables[base_db_name].Rows[fi_cnt]["Cpno"].ToString().Replace("-","");
                    string pert = Per_t.Substring(6, 1);
                    if (Per_t.Length >= 7)
                    {
                        if (Per_t.Replace("*", "").Length >= 7)
                           
                            if (pert == "1")
                                W_M_TF = 1;
                            else
                            W_M_TF = 2;
                            
                    }
                    else
                        W_M_TF = 3;
                }
                else
                {
                    string Sex_FLAG = ds.Tables[base_db_name].Rows[fi_cnt]["Sex_FLAG"].ToString();
                    W_M_TF = 3; 
                    if (Sex_FLAG == "X")  W_M_TF = 2;
                    if (Sex_FLAG == "Y") W_M_TF = 1; 
                }

                    

                Y_Per ="";
                if (Per_t.Length >= 7 && ds.Tables[base_db_name].Rows[fi_cnt]["Cpno"].ToString() != "")
                {
                    Y_Per = csn_C.Search_nai_Period(Per_t).ToString() + "대";
                }
                else
                {
                    string BirthDay = ds.Tables[base_db_name].Rows[fi_cnt]["BirthDay"].ToString();
                    Y_Per = csn_C.Search_nai_Period_B(BirthDay).ToString() + "대";
                }



                string Pay_c = ADD + "-" + Y_Per;

                if (dic_Pay.ContainsKey(Pay_c) == true)
                {
                    if (W_M_TF == 1) dic_Pay[Pay_c].M_Cnt++;
                    if (W_M_TF == 2) dic_Pay[Pay_c].W_Cnt++;
                    if (W_M_TF == 3 || W_M_TF == 0) dic_Pay[Pay_c].Not_Cnt  ++; 
                }
                else
                {
                    cls_Memb_Area ccP = new cls_Memb_Area();


                    ccP.Area = ADD;
                    ccP.Year_lvl = Y_Per; 
                    ccP.M_Cnt  = 0;
                    ccP.W_Cnt  = 0;
                    ccP.Not_Cnt  = 0;

                    if (W_M_TF == 1) ccP.M_Cnt++;
                    if (W_M_TF == 2) ccP.W_Cnt++;
                    if (W_M_TF == 3 || W_M_TF == 0) ccP.Not_Cnt ++;
                    
                    dic_Pay[Pay_c] = ccP;
                }                               
                //string Per_t = cm._chang_base_caption_search("판매원");
                //if (Per_t == ds.Tables[base_db_name].Rows[fi_cnt]["Sell_MEM_TF2"].ToString())
                //    SellCnt_1++;
                //else                
                //    SellCnt_2++;

                //Per_t = cm._chang_base_caption_search("활동");
                //if (Per_t == ds.Tables[base_db_name].Rows[fi_cnt]["LeaveCheck_2"].ToString())
                //    MemCnt_1++;
                //else
                //    MemCnt_2++;
                                
                //if ( ds.Tables[base_db_name].Rows[fi_cnt]["Ed_Date_2"].ToString() != "")
                //    EdCnt_1++;
                //else
                //    EdCnt_2++;

                //Per_t = ds.Tables[base_db_name].Rows[fi_cnt]["B_Name"].ToString();

                //if (Per_t != "")
                //{
                //    if (Center_MemCnt.ContainsKey(Per_t) == true)
                //        Center_MemCnt[Per_t] ++;
                //    else
                //        Center_MemCnt[Per_t] = 1;
                //}
                
            }
            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();



            Dictionary<int, object[]> gr_dic_text_Sub = new Dictionary<int, object[]>();


            var items = from pair in dic_Pay
                       orderby pair.Key ascending
                       select pair;

            int FFCnt = 0; 
            // foreach (string tkey in Year_MemCnt.Keys)
            foreach (KeyValuePair<string, cls_Memb_Area> pair in items)
            {
                //Set_gr_dic_Sub(ref ds, ref gr_dic_text_Sub, FFCnt);  //데이타를 배열에 넣는다.

                //cls_Memb_Area ccP = new cls_Memb_Area();
                //ccP = pair.Value. ; 

                object[] row0 = {  pair.Value.Area 
                                ,pair.Value.Year_lvl 
                                ,pair.Value.M_Cnt 
                                ,pair.Value.W_Cnt 
                                ,pair.Value.Not_Cnt                                 
                                 };

                gr_dic_text_Sub[FFCnt + 1] = row0;

                FFCnt++; 
            }

            cgb_Sub.grid_name_obj = gr_dic_text_Sub;  //배열을 클래스로 보낸다.
            cgb_Sub.db_grid_Obj_Data_Put();
            //////////////////////////////////////////////////////////////////////////////////////////



            Dictionary<int, object[]> gr_dic_text_Sub_Add = new Dictionary<int, object[]>();


            var items_Add = from pair in dic_Add
                        orderby pair.Key ascending
                        select pair;


            FFCnt = 0;
            // foreach (string tkey in Year_MemCnt.Keys)
            foreach (KeyValuePair<string, int> pair in items_Add)
            {
                
                object[] row0 = {  pair.Key 
                                ,pair.Value                                
                                 };

                gr_dic_text_Sub_Add[FFCnt + 1] = row0;

                FFCnt++;
            }


            cgb_Sub_Add.grid_name_obj = gr_dic_text_Sub_Add;  //배열을 클래스로 보낸다.
            cgb_Sub_Add.db_grid_Obj_Data_Put();
            //////////////////////////////////////////////////////////////////////////////////////////
            
            //차트 관련해서 뿌려주는곳 아래쪽
            //Reset_Chart_Total(SellCnt_1, SellCnt_2);
            //Reset_Chart_Total(MemCnt_1, MemCnt_2, 1);
            //Reset_Chart_Total(EdCnt_1, EdCnt_2, "1");
            //foreach (string tkey in Center_MemCnt.Keys )
            //{
            //    Push_data(series_Item, tkey, Center_MemCnt[tkey]);
            //}
        }



        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 40;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


            string[] g_HeaderText = {"회원_번호"  , "성명" , "생년월일"  , "주민번호2"  ,"주민번호"  ,    // 5
                                   "현직급"  , "최초주문일자" , "센타명"   , "가입일"    , "집전화"       // 10
                                   , "핸드폰"   , "교육일", "등급", "추천인"   , "추천인명"               // 15
                                   , "후원인"  , "후원인명"   , "후원인기준","우편번호" , "주소"          // 20
                                    , "은행명"  , "계좌번호" , "예금주"     , "구분" ,"활동_여부"        // 25
                                    , "탈퇴일","기록자", "기록일"   , "이메일" , "이메일수신여부"          // 30
                                    , "SMS수신여부", "_비자만료일", "_라인중지일",""  ,""                 // 35
                                    ,""  ,"" ,"","신분증_인증_상태", "통장_인증_상태"                    // 40
                                    };

            string[] g_Col_name = {"mbid2"  , "m_name", "birthday"  , "cpno" , "cpno2"  // 5
                    , "CurGrade"     , "FirstOrderDate"  , "CenterName"   , "regtime"    , "Tel"  // 10
                    , "HpTel"     , "StudyDate" , "Grade", "nominid2"   , "nominName" // 15
                    , "saveId2"   , "SaveidName" ,"SaveDefault"  ,"Zipcode" , "Address" // 20
                      , "BankName"  , "BankACC"  , "BankUserName"     , "Gubun", "IsLeave"   // 25
                      , "LeaveDate" ,"기록자", "기록일" , "Email", "AgreeEmail"   // 30
                      , "AgreeSMS", "_VisaEndDate", "_LineStopdate","t2"   ,"t3"   // 35
                      ,"t4"     ,"t5" ,"t6","ID Card status", "Bank account status" // 40
                                    };


            int SizeSD = chkShowSaveDefault.Checked ? 75 : 0;

            cgb.grid_col_header_text = g_HeaderText;
            cgb.grid_col_name = g_Col_name;
            int[] g_Width = { 85, 90,130,0, 130,    // 5
                100, 120  ,100, 90, 130,            // 10
                130, 100  ,80 ,cls_app_static_var.save_uging_Pr_Flag  , cls_app_static_var.save_uging_Pr_Flag           // 15
                , cls_app_static_var.nom_uging_Pr_Flag  , cls_app_static_var.nom_uging_Pr_Flag ,  SizeSD , 60  ,200     // 20
                 , 90, 0, 90 , 60 , 70  ,   // 25
                90  , 100   ,120 ,120,120   // 30
                ,120, 0, 0, 0  , 0, // 35
                0, 0 , 0, 100, 100  // 40
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true,  true,  true,  true,  true
                              ,  true ,  true ,  true,  true ,  true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //5

                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleLeft                              
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter //10

                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter   
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter//15   

                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter                              
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter //20

                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleCenter   
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter//25   

                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleCenter   //30

                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft  //35
                               
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //40

                              };
            cgb.grid_col_alignment = g_Alignment;
        }


        private void Set_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][3]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][4]
 
                                ,ds.Tables[base_db_name].Rows[fi_cnt][5]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][6]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][7]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][8]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][9]

                                ,ds.Tables[base_db_name].Rows[fi_cnt][10]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][11]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][12]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][13]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][14]

                                ,ds.Tables[base_db_name].Rows[fi_cnt][15]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][16]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][17]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][18]
                               // ,ds.Tables[base_db_name].Rows[fi_cnt][19]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][20]

                                
                                ,ds.Tables[base_db_name].Rows[fi_cnt][21]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][22]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][23]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][24]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][25]

                                ,ds.Tables[base_db_name].Rows[fi_cnt][26]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][27]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][28]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][29]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][30]

                                ,ds.Tables[base_db_name].Rows[fi_cnt][31]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][32]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][33]

                                ,ds.Tables[base_db_name].Rows[fi_cnt][34]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][35]
                                     ,ds.Tables[base_db_name].Rows[fi_cnt][36]
                                     ,ds.Tables[base_db_name].Rows[fi_cnt][37]
                                     ,ds.Tables[base_db_name].Rows[fi_cnt][38]
                                     //,ds.Tables[base_db_name].Rows[fi_cnt][39]
                                     //,ds.Tables[base_db_name].Rows[fi_cnt][40]
                                     ,ds.Tables[base_db_name].Rows[fi_cnt][41]  // IDCARD
                                     ,ds.Tables[base_db_name].Rows[fi_cnt][42]  // Bankbook
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }



        private void MtxtData_Temp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                MaskedTextBox mtb = (MaskedTextBox)sender;

                if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                {
                    string Sn = mtb.Text.Replace("-", "").Replace("_", "").Trim();
                    if (mtb.Name == "mtxtBiz1")
                    {
                        if (Sn_Number_(Sn, mtb, "biz") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtTel1")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtTel2")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtZip1")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }

                    string R4_name = mtb.Name.Substring(mtb.Name.Length - 4, 4);
                    if (R4_name == "Date" || R4_name == "ate3" || R4_name == "ate1" || R4_name == "ate2" || R4_name == "ate4")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }

                   

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
                    string[] date_a = mtb.Text.Split('-');

                    if (date_a.Length >= 3 && date_a[0].Trim() != "" && date_a[1].Trim() != "" && date_a[2].Trim() != "")
                    {
                        string Date_YYYY = "0000" + int.Parse(date_a[0]).ToString();

                        date_a[0] = Date_YYYY.Substring(Date_YYYY.Length - 4, 4);

                        if (int.Parse(date_a[1]) < 10)
                            date_a[1] = "0" + int.Parse(date_a[1]).ToString();

                        if (int.Parse(date_a[2]) < 10)
                            date_a[2] = "0" + int.Parse(date_a[2]).ToString();

                        mtb.Text = date_a[0] + '-' + date_a[1] + '-' + date_a[2];

                        cls_Check_Input_Error c_er = new cls_Check_Input_Error();
                        if (mtb.Text.Replace("-", "").Trim() != "")
                        {
                            int Ret = 0;
                            Ret = c_er.Input_Date_Err_Check(mtb);

                            if (Ret == -1)
                            {
                                mtb.Focus(); return false;
                            }
                        }

                    }
                    else
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

        private void mtxtMbid_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            MaskedTextBox tb = (MaskedTextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
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

            else if ((tb.Tag != null) && (tb.Tag.ToString() == "-"))
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, "1") == false)
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
                    txtBank_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtR_Id")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtR_Id_Code.Text = "";
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
                Db_Grid_Popup(tb, txtBank_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtBank_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txtBank_Code);

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
                cgb_Pop.Next_Focus_Control = txtR_Id;

            if (tb.Name == "txtCenter2")
                cgb_Pop.Next_Focus_Control = txtR_Id;

            if (tb.Name == "txtBank")
                cgb_Pop.Next_Focus_Control = butt_Select;

            if (tb.Name == "txtR_Id")
                cgb_Pop.Next_Focus_Control = txtBank;
            

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
                if (tb.Name == "txtCenter2")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtR_Id")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txtBank")
                    cgb_Pop.db_grid_Popup_Base(2, "은행_코드", "은행명", "Ncode", "BankName", strSql);
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
                    Tsql = Tsql + " And  ShowMemberCenter = 'Y'";
                    if (cls_User.gid_CountryCode != "") Tsql = Tsql + " And  Na_Code = '" + cls_User.gid_CountryCode + "'"; 
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
                Reset_Chart_Total(); 

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                
                dGridView_Base_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Sub.d_Grid_view_Header_Reset();

                dGridView_Base_Sub_Add_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Sub_Add.d_Grid_view_Header_Reset();
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                cls_form_Meth ct = new cls_form_Meth();
                ct.from_control_clear(this, mtxtMbid);

                opt_Ed_1.Checked = true; opt_Line_1.Checked = true; opt_Leave_1.Checked = true; opt_sell_1.Checked = true;
                //radioB_S.Checked = true; radioB_R.Checked = true;                 radioB_E.Checked = true;
                combo_Grade.SelectedIndex = -1;
                tab_Chart.SelectedIndex = 0;
                radio_S1.Checked = true;
                radioB_R1.Checked = true;
                radioB_SMT.Checked = true;
            }
            else if (bt.Name == "butt_Select")
            {
                

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                dGridView_Base_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Sub.d_Grid_view_Header_Reset();

                dGridView_Base_Sub_Add_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_Sub_Add.d_Grid_view_Header_Reset();

                Reset_Chart_Total(); 
                tab_Chart.SelectedIndex = 0; 
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                if (Check_TextBox_Error() == false) return;

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                combo_Grade_Code.SelectedIndex = combo_Grade.SelectedIndex;
                combo_GradeP_Code.SelectedIndex = combo_GradeP.SelectedIndex;
                chart_Center.Series.Clear();
                Save_Nom_Line_Chart();                
                
                Base_Grid_Set();  //뿌려주는 곳

                dGridView_Base.Columns["SaveDefault"].Width = chkShowSaveDefault.Checked ? 75 : 0;
                dGridView_Base.Columns["SaveDefault"].Visible = true;
                this.Cursor = System.Windows.Forms.Cursors.Default;

            }
           
            else if (bt.Name == "butt_Excel")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info);
                e_f.ShowDialog();
            }

            else if (bt.Name == "butt_Excel_Sub")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info_Sub);
                e_f.ShowDialog();
            }

            else if (bt.Name == "butt_Excel_Sub_Add")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info_Sub_Add);
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
            Excel_Export_File_Name = this.Text; // "Member_Select";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;
        }

        private DataGridView e_f_Send_Export_Excel_Info_Sub(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "Member_Select";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base_Sub;
        }

        private DataGridView e_f_Send_Export_Excel_Info_Sub_Add(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "Member_Select";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base_Sub_Add;
        }

        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string Send_Nubmer = ""; string Send_Name = "";
                Send_Nubmer = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                Send_Name = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                Send_Mem_Number(Send_Nubmer, Send_Name);   //부모한테 이벤트 발생 신호한다.
            }            
        }


        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
           // SendKeys.Send("{TAB}");
        }





        private void radioB_S_Base_Click(object sender, EventArgs e)
        {
            //RadioButton _Rb = (RadioButton)sender;
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtRegDate1, mtxtRegDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }


        private void radioB_R_Base_Click(object sender, EventArgs e)
        {
            //RadioButton _Rb = (RadioButton)sender;
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtMakDate1, mtxtMakDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }


        private void radioB_E_Base_Click(object sender, EventArgs e)
        {
            Data_Set_Form_TF = 1;
            //RadioButton _Rb = (RadioButton)sender;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtEduDate1, mtxtEduDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }



        private void Reset_Chart_Total()
        {
            //chart_Mem.Series.Clear();
            cls_form_Meth cm = new cls_form_Meth();
            
            double[] yValues = { 0, 0 };
            string[] xValues = { cm._chang_base_caption_search("판매원"), cm._chang_base_caption_search("소비자") };
            chart_Mem.Series["Series1"].Points.DataBindXY(xValues, yValues);
            
            chart_Mem.Series["Series1"].ChartType = SeriesChartType.Pie;
            //chart_Mem.Series["Series1"]["PieLabelStyle"] = "Disabled";

            chart_Mem.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;


            chart_Mem.Legends[0].Enabled = true;


            //chart_Leave.Series.Clear();
            double[] yValues_2 = { 0, 0 };
            string[] xValues_2 = { cm._chang_base_caption_search("활동"), cm._chang_base_caption_search("탈퇴") };
            chart_Leave.Series["Series1"].Points.DataBindXY( xValues_2, yValues_2);
            
            chart_Leave.Series["Series1"].ChartType = SeriesChartType.Pie;
            //chart_Leave.Series["Series1"]["PieLabelStyle"] = "Disabled";

            chart_Leave.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;            
            chart_Leave.Legends[0].Enabled = true;


            double[] yValues_3 = { 0, 0 };
            string[] xValues_3 = { cm._chang_base_caption_search("교육이수자"), cm._chang_base_caption_search("비이수자") };
            chart_edu.Series["Series1"].Points.DataBindXY(xValues_3, yValues_3);
            
            chart_edu.Series["Series1"].ChartType = SeriesChartType.Pie;
            //chart_edu.Series["Series1"]["PieLabelStyle"] = "Disabled";
            chart_edu.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_edu.Legends[0].Enabled = true;

            chart_Center.Series.Clear();
            series_Item.Points.Clear();
        }


        private void Reset_Chart_Total(double SellCnt_1, double SellCnt_2)
        {
            //chart_Mem.Series.Clear();
            cls_form_Meth cm = new cls_form_Meth();
            Series series_Save = new Series();                     
         
            chart_Mem.Series.Clear();
            chart_Mem.Series.Add(series_Save);

            DataPoint dp = new DataPoint();
            series_Save.ChartType = SeriesChartType.Pie;
            dp.SetValueXY(cm._chang_base_caption_search("판매원"), SellCnt_1);
            dp.Label = SellCnt_1.ToString() ;            
            dp.LabelForeColor = Color.Black;
            dp.LegendText = cm._chang_base_caption_search("판매원");
            series_Save.Points.Add(dp);

            DataPoint dp2 = new DataPoint();

            dp2.SetValueXY(cm._chang_base_caption_search("소비자"), SellCnt_2);
            dp2.Label = SellCnt_2.ToString() ;            
            dp2.LabelForeColor = Color.Black;
            dp2.LegendText = cm._chang_base_caption_search("소비자");
            series_Save.Points.Add(dp2);           
            chart_Mem.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;            
            chart_Mem.Legends[0].Enabled = true;
        }

        private void Reset_Chart_Total( double MemCnt_1, double MemCnt_2,int t_f)
        {

            cls_form_Meth cm = new cls_form_Meth();
            Series series_Save = new Series();

            chart_Leave.Series.Clear();
            chart_Leave.Series.Add(series_Save);

            DataPoint dp = new DataPoint();
            series_Save.ChartType = SeriesChartType.Pie;
            dp.SetValueXY(cm._chang_base_caption_search("활동"), MemCnt_1);
            dp.Label = MemCnt_1.ToString();            
            dp.LabelForeColor = Color.Black;
            dp.LegendText = cm._chang_base_caption_search("활동");
            series_Save.Points.Add(dp);

            DataPoint dp2 = new DataPoint();

            dp2.SetValueXY(cm._chang_base_caption_search("탈퇴"), MemCnt_2);
            dp2.Label = MemCnt_2.ToString();            
            dp2.LabelForeColor = Color.Black;
            dp2.LegendText = cm._chang_base_caption_search("탈퇴");
            series_Save.Points.Add(dp2);
            chart_Leave.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_Leave.Legends[0].Enabled = true;


         
            chart_Leave.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_Leave.Legends[0].Enabled = true;
        }

        private void Reset_Chart_Total(double EduCnt_1, double EduCnt_2, string  t_f)
        {

            cls_form_Meth cm = new cls_form_Meth();
            Series series_Save = new Series();

            chart_edu.Series.Clear();
            chart_edu.Series.Add(series_Save);

            DataPoint dp = new DataPoint();
            series_Save.ChartType = SeriesChartType.Pie;
            dp.SetValueXY(cm._chang_base_caption_search("교육이수자"), EduCnt_1);
            dp.Label = EduCnt_1.ToString();            
            dp.LabelForeColor = Color.Black;
            dp.LegendText = cm._chang_base_caption_search("교육이수자");
            series_Save.Points.Add(dp);

            DataPoint dp2 = new DataPoint();

            dp2.SetValueXY(cm._chang_base_caption_search("비이수자"), EduCnt_2);
            dp2.Label = EduCnt_2.ToString();            
            dp2.LabelForeColor = Color.Black;
            dp2.LegendText = cm._chang_base_caption_search("비이수자");
            series_Save.Points.Add(dp2);

            chart_edu.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_edu.Legends[0].Enabled = true;
        }








        private void Push_data(Series series, string p, double p_3)
        {


            if (p != "")
            {
                DataPoint dp = new DataPoint();

                if (p.Replace(" ", "").Length >= 4)
                    dp.SetValueXY(p.Replace(" ", "").Substring(0, 4), p_3);
                else
                    dp.SetValueXY(p, p_3);

                dp.Font = new System.Drawing.Font("맑은고딕", 9);
                dp.Label = string.Format(cls_app_static_var.str_Currency_Type, p_3); // p_3.ToString();                  
                series.Points.Add(dp);
            }

        }

   

        //Push_data(series_Item, nodeKey.ToString() + "Line", Save_Cnt[nodeKey]);
        private void Save_Nom_Line_Chart()
        {
            cls_form_Meth cm = new cls_form_Meth();

            chart_Center.Series.Clear();
            series_Item.Points.Clear();
            


            series_Item["DrawingStyle"] = "Emboss";
            series_Item["PointWidth"] = "0.5";
            series_Item.Name = cm._chang_base_caption_search("인원수");
            
            
            //series_Item.ChartArea = "ChartArea1";
            series_Item.ChartType = SeriesChartType.Column  ;            
            // series_Item.Legend = "Legend1";            


            chart_Center.Series.Add(series_Item);
            //chart_Center.Series.Add(series_PV);
            chart_Center.ChartAreas[0].AxisX.Interval = 1;
            chart_Center.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("맑은고딕", 9);
            chart_Center.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 8;
            chart_Center.ChartAreas[0].AxisY.Interval = 500;

            chart_Center.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            //chart_Center.ChartAreas["ChartArea1"].BackColor = Color.White;
            chart_Center.Legends[0].Enabled = true;

        }











        private void dGridView_Base_Sub_Header_Reset()
        {
            cgb_Sub.grid_col_Count = 5;
            cgb_Sub.basegrid = dGridView_Base_Sub;
            cgb_Sub.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Sub.grid_Frozen_End_Count = 2;            
            cgb_Sub.basegrid.RowHeadersVisible = false; 
            //cgb_Sub.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


            string[] g_HeaderText = {""  , "나이대"   , "남성"  , "여성"   , "모름"                                        
                                    };
            cgb_Sub.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 0, 70, 60, 60, 60  
                             
                            };
            cgb_Sub.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                             
                                   };
            cgb_Sub.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //5
                             
                              };
            cgb_Sub.grid_col_alignment = g_Alignment;
        }






        private void dGridView_Base_Sub_Add_Header_Reset()
        {
            cgb_Sub_Add.grid_col_Count = 5;
            cgb_Sub_Add.basegrid = dGridView_Base_Sub_Add;
            cgb_Sub_Add.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Sub_Add.grid_Frozen_End_Count = 2;
            cgb_Sub_Add.basegrid.RowHeadersVisible = false;
            //cgb_Sub_Add.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


            string[] g_HeaderText = {"지역"  , "인원수"   , ""  , ""   , ""                                        
                                    };
            cgb_Sub_Add.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 150, 80, 0, 0, 0  
                             
                            };
            cgb_Sub_Add.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                             
                                   };
            cgb_Sub_Add.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //5
                             
                              };
            cgb_Sub_Add.grid_col_alignment = g_Alignment;
        }













        private int but_Exp_Base_Left = 0;
        private int Parent_but_Exp_Base_Width = 0;

        private void but_Exp_Click(object sender, EventArgs e)
        {
            if (but_Exp.Text == "<<")
            {
                Parent_but_Exp_Base_Width = but_Exp.Parent.Width;
                but_Exp_Base_Left = but_Exp.Left  ;

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

        private void butt_Print_MembershipCard_Click(object sender, EventArgs e)
        {
            if (dGridView_Base.Rows.Count.Equals(0)) return;
            try
            {
                /*
                 
            string[] g_Col_name = {"mbid2"  , "m_name"   , "cpno"  , "CurGrade"   , "t1"     
                                , "CenterName"   , "regtime"    , "Tel"   , "HpTel"    , "StudyDate"
                                , "Grade", "nominid2"   , "nominName"  , "saveId2" ,""
                                 , "SaveidName"   ,"Zipcode" , "Address"     , "BankName"    , "BankACC" 
                                 , "BankUserName"     , "Gubun" , "IsLeave", "_VisaEndDate"  , "LeaveDate" 
                                 , "_LineStopdate"   ,"t2"  ,"t3" ,"t4" ,"t5" 
                                 ,"t6","기록자"  , "기록일" ,"매니져승급일","팀승급일" //다음에 번역하는걸로ㅠㅠㅍ
                                 ,"그룹승급일","마스타승급일","스타승급일" 
                                    };
                 
                 */
                cls_Connect_DB Conn = new cls_Connect_DB();
                DataSet ds = new DataSet();
                Conn.Open_Data_Set("SELECT TOP 0 * FROM Tbl_Memberinfo", "Memberinfo", ds);

                DataTable MemberInfo = ds.Tables["Memberinfo"];

                foreach(DataGridViewRow row in dGridView_Base.Rows)
                {
                    DataRow nR =  MemberInfo.NewRow();
                    nR["mbid2"] = row.Cells["mbid2"].Value;
                    nR["m_name"] = row.Cells["m_name"].Value;

                    string cpno = row.Cells["cpno"].Value.ToString();
                    if(cpno != string.Empty )
                    {
                        if(cpno.Length > 6 )
                        {
                            cpno = cpno.Substring(0, 6);
                        }
                    }
                    nR["BirthDay"] = cpno;
                    nR["RegTime"] = row.Cells["RegTime"].Value;
                    nR["Address1"] = row.Cells["Address"].Value;

                    MemberInfo.Rows.Add(nR);
                }

                frmFastReport frm = new frmFastReport();
                frm.BindingDataTables.Add("Memberinfo", MemberInfo);
                frm.ShowReport(frmFastReport.EShowReport.회원증명서);
            }
            catch (Exception ex)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Err"));
                Console.WriteLine(ex.Message);
            }
        }
    }
}
