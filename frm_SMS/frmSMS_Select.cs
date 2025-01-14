﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MLM_Program
{
    public partial class frmSMS_Select : Form
    {
        


        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        cls_Grid_Base cgb = new cls_Grid_Base();
        private const string base_db_name = "tbl_Memberinfo";
        private int Data_Set_Form_TF;

       
        public delegate void SendNumberDele(string Send_Number, string Send_Name);
        public event SendNumberDele Send_Mem_Number;

        

        public frmSMS_Select()
        {
            InitializeComponent();
        }           


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;
           

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);


            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            
            mtxtMakDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtMakDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtRegDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtRegDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtEduDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtEduDate2.Mask = cls_app_static_var.Date_Number_Fromat;


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
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Select);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);  
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


      


            if (mtxtRegDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtRegDate1.Text, mtxtRegDate1, "Date") == false)
                {
                    mtxtMakDate1.Focus();
                    return false;
                }
            }

            if (mtxtRegDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtRegDate2.Text, mtxtRegDate2, "Date") == false)
                {
                    mtxtMakDate2.Focus();
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


        private void Make_Base_Query(ref string Tsql)
        {

            //string[] g_HeaderText = {"회원_번호"  , "성명"   , "주민번호"  , "현직급"   , "위치"        
            //                    , "센타명"   , "가입일"    , "집전화"   , "핸드폰"    , "교육일"
            //                    , "후원인"   , "후원인명"  , "추천인"   , "추천인명"   ,"우편_번호"
            //                    , "주소"     , "은행명"    , "계좌번호" , "예금주"     , "구분"
            //                    , "활동_여부", "중지_여부"  , "탈퇴일"  , "라인중지일"  ,"기록자"
            //                    , "기록일"
            //                        };

            cls_form_Meth cm = new cls_form_Meth();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT* FROM");
sb.AppendLine("(");
            sb.AppendLine("Select");
            sb.AppendLine(" T_R1.mbid2");
            sb.AppendLine(", T_R1.M_Name");
            sb.AppendLine(", T_R1.M_Tel");
            sb.AppendLine(", T_R1.S_Message");
            sb.AppendLine(", Case When CAST(SendStatus AS VARCHAR) = '2' Then '문자전송완료'");
            sb.AppendLine("       when CAST(SendStatus AS VARCHAR) = '4' Then '문자전송실패'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = 'S' then '서버전송완료'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = '3' then '서버전송시도중'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = '1' then '서버전송시도중'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = '0' then '서버전송대기중' END as KORSTATE");
            sb.AppendLine(", T_R1.Recordid");
            sb.AppendLine(", T_R1.Recordtime");
            sb.AppendLine(", Case When T_R1.SMS_TF = 'S' Then 'SMS' When T_R1.SMS_TF = 'L' Then 'LMS'  When T_R1.SMS_TF = 'M' Then 'MMS' ELSE '' End AS 'GUBUN'");
            sb.AppendLine(", T_R1.Call_T_Number");
            sb.AppendLine(",  ErrorMessage.Message1 + ' ' + ErrorMessage.Message2  AS ErrorMessage");
            sb.AppendLine(", '' AS 'TEMP'");
            sb.AppendLine(", SendStatus");
            sb.AppendLine(" From tbl_SMS_mannatech_Result T_R1(nolock)");
            sb.AppendLine("  JOIN msgdata(nolock) ON msgdata.MsgNo = T_R1.T_index");
            sb.AppendLine("  LEFT JOIN ErrorMessage(nolock) ON ErrorMessage.ErrCode = msgdata.ErrorCode");
            sb.AppendLine(" UNION ALL");
            sb.AppendLine(" Select");
            sb.AppendLine(" T_R1.mbid2");
            sb.AppendLine(", T_R1.M_Name");
            sb.AppendLine(", T_R1.M_Tel");
            sb.AppendLine(", T_R1.S_Message");
            sb.AppendLine(", Case When CAST(SendStatus AS VARCHAR) = '2' Then '문자전송완료'");
            sb.AppendLine("       when CAST(SendStatus AS VARCHAR) = '4' Then '문자전송실패'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = 'S' then '서버전송완료'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = '3' then '서버전송시도중'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = '1' then '서버전송시도중'");
            sb.AppendLine("       When CAST(SendStatus AS VARCHAR) = '0' then '서버전송대기중' END as KORSTATE");
            sb.AppendLine(", T_R1.Recordid");
            sb.AppendLine(", T_R1.Recordtime");
            sb.AppendLine(", Case When T_R1.SMS_TF = 'S' Then 'SMS' When T_R1.SMS_TF = 'L' Then 'LMS'  When T_R1.SMS_TF = 'M' Then 'MMS' ELSE '' End AS 'GUBUN'");
            sb.AppendLine(", T_R1.Call_T_Number");
            sb.AppendLine(",  ErrorMessage.Message1 + ' ' + ErrorMessage.Message2  AS ErrorMessage");
            sb.AppendLine(", '' AS 'TEMP'");
            sb.AppendLine(", SendStatus");
            sb.AppendLine(" From tbl_SMS_mannatech_Result T_R1(nolock)");
            sb.AppendLine("  JOIN msgdata_Log(nolock)  ON msgdata_Log.MsgNo = T_R1.T_index");
            sb.AppendLine("  LEFT JOIN ErrorMessage(nolock) ON ErrorMessage.ErrCode = msgdata_Log.ErrorCode");
            sb.AppendLine(") A");
            Tsql = sb.ToString();
        }



        private void Make_Base_Query_(ref string Tsql)
        {
            string strSql = " Where M_Tel <>''  ";
            //// strSql = strSql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";

            string Mbid = ""; int Mbid2 = 0;
            //회원번호1로 검색
            if (
                (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "") 
                &&
                (mtxtMbid2.Text.Replace("-", "").Replace("_", "").Trim() == "") 
                )
            {

                strSql = strSql + " And Mbid2 ='" + mtxtMbid.Text + "'";
                //cls_Search_DB csb = new cls_Search_DB();
                //if (csb.Member_Nmumber_Split(mtxtMbid.Text, ref Mbid, ref Mbid2) == 1)
                //{
                //    strSql = strSql + " And Mbid = '" + Mbid + "'";
                //    strSql = strSql + " And Mbid2 = " + Mbid2;
                //}
            }

            ////회원번호2로 검색
            //if (
            //    (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
            //    &&
            //    (mtxtMbid2.Text.Replace("-", "").Replace("_", "").Trim() != "")
            //    )
            //{
            //    cls_Search_DB csb = new cls_Search_DB();
            //    if (csb.Member_Nmumber_Split(mtxtMbid.Text, ref Mbid, ref Mbid2) == 1)
            //    {
            //        if (Mbid != "")
            //            strSql = strSql + " And Mbid >='" + Mbid + "'";

            //        if (Mbid2 >= 0)
            //            strSql = strSql + " And Mbid2 >= " + Mbid2;
            //    }

            //    if (csb.Member_Nmumber_Split(mtxtMbid2.Text, ref Mbid, ref Mbid2) == 1)
            //    {
            //        if (Mbid != "")
            //            strSql = strSql + " And Mbid <='" + Mbid + "'";

            //        if (Mbid2 >= 0)
            //            strSql = strSql + " And Mbid2 <= " + Mbid2;
            //    }
            //}


            //회원명으로 검색
            if (txtName.Text.Trim() != "")
                strSql = strSql + " And M_Name Like '%" + txtName.Text.Trim() + "%'";

            //가입일자로 검색 -1

            if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And CONVERT(VARCHAR, CAST(recordtime AS DATETIME), 112) = '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";

            //가입일자로 검색 -2
            if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And CONVERT(VARCHAR, CAST(recordtime AS DATETIME), 112) >= '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And CONVERT(VARCHAR, CAST(recordtime AS DATETIME), 112) <= '" + mtxtRegDate2.Text.Replace("-", "").Trim() + "'";
            }



            //체크에따라 바꿔야함  And SendStatus IN (2) 
            if (opt_Leave_1.Checked == true)
                strSql = strSql + " And SendStatus IN ('2')  ";

            if (opt_Leave_2.Checked == true)
                strSql = strSql + " And SendStatus IN ('4')  ";


            if (opt_Leave_4.Checked == true)
                strSql = strSql + " And SendStatus NOT IN ('2', '4')   ";


            if (txtR_Id_Code.Text.Trim() != "")
                strSql = strSql + " And recordid = '" + txtR_Id_Code.Text.Trim() + "'";


            //if (txtR_Id_Code.Text.Trim() != "")
            //    strSql = strSql + " And T_R1.recordid = '" + txtR_Id_Code.Text.Trim() + "'";

            


            if (txtPNumber.Text.Trim() != "")
            {
                strSql = strSql + " And  M_Tel  = '" + txtPNumber.Text.Replace(" ", "").ToString() + "' ";                
            }

           
            Tsql = Tsql + strSql ;
            Tsql = Tsql + " Order by Recordtime desc, Mbid2 ";
        }




        private void Base_Grid_Set()
        {   
            string Tsql = "";            
            Make_Base_Query(ref Tsql);

            Make_Base_Query_(ref Tsql);

            cls_form_Meth cm = new cls_form_Meth();
            //cm._chang_base_caption_search(m_text);

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE A");
            sb.AppendLine("SET A.M_Name = B.M_Name");
            sb.AppendLine("FROM SMSDB.dbo.tbl_SMS_mannatech_Result A");
            sb.AppendLine(" JOIN TBL_MEMBERINFO B ON A.mbid2 = B.mbid2");
            sb.AppendLine("WHERE ISNULL(A.M_Name,'') = ''");

            Temp_Connect.Update_Data(sb.ToString());

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_SMSData_Set (Tsql, base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            double SellCnt_1 = 0; double SellCnt_2 = 0;
            double MemCnt_1 = 0; double MemCnt_2 = 0;
            double EdCnt_1 = 0; double EdCnt_2 = 0; 
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, double> Center_MemCnt = new Dictionary<string, double>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

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
            cgb.grid_col_Count = 11;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원_번호"  , "성명"   , "연락처"  , "메시지"   , "결과"        
                                , "전송자"   , "전송일"    , "구분"   , "회신_번호"   , "실패이유"  , ""                                
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 85, 90, 130, 300, 60  
                             ,100, 90, 130, 300, 300,0                            
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true   ,true

                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter  //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft                              
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleCenter //10

                                ,DataGridViewContentAlignment.MiddleCenter //10

                            
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
                if (tb.Name == "txtCenter")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtR_Id")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txtBank")
                    cgb_Pop.db_grid_Popup_Base(2, "은행_코드", "은행명", "Ncode", "BankName", strSql);
            }
            else
            {
                if (tb.Name == "txtCenter")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
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
               
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                cls_form_Meth ct = new cls_form_Meth();
                ct.from_control_clear(this, mtxtMbid);

            }
            else if (bt.Name == "butt_Select")
            {
                

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();
           
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                if (Check_TextBox_Error() == false) return;

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
               
                
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

           

        }


        private DataGridView e_f_Send_Export_Excel_Info(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "Member_Select";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;
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



















    }
}
