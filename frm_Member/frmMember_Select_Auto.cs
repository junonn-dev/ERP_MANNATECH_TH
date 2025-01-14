﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Data.SqlClient;

namespace MLM_Program
{
    public partial class frmMember_Select_Auto : Form
    {

        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Cacu = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece = new cls_Grid_Base();

        private const string base_db_name = "tbl_Memberinfo_Autoship";
        private int Data_Set_Form_TF;


        public frmMember_Select_Auto()
        {
            InitializeComponent();
        }           


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Item_Header_Reset();
            cgb_Item.d_Grid_view_Header_Reset(1);

            dGridView_Base_Cacu_Header_Reset();
            cgb_Cacu.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Header_Reset();
            cgb_Rece.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            
            mtxtRegDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtRegDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;

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
                    mtxtMbid.Focus();     
                    return false;
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
                        
           

            return true;
        }


        private void Make_Base_Query(ref string Tsql)
        {
            Tsql = " SELECT ";
            Tsql += Environment.NewLine + " '' ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.Auto_Seq ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.mbid2 ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.M_Name ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_Date <> '' THEN ";
            Tsql += Environment.NewLine + " LEFT(tbl_Memberinfo_AutoShip.Req_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Req_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Req_Date, 7, 2) ELSE '' END Req_Date ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '10' THEN CASE WHEN CONVERT(VARCHAR, tbl_Memberinfo_AutoShip.Proc_Cnt) = '0' THEN '' ELSE CONVERT(VARCHAR, tbl_Memberinfo_AutoShip.Proc_Cnt) END ELSE '' END Last_Cnt ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '10' THEN ";
            Tsql += Environment.NewLine + " CASE WHEN ISNULL(A.SellDate , '') <> '' THEN LEFT(A.SellDate, 4) + '-' + SUBSTRING(A.SellDate, 5, 2) + '-' + SUBSTRING(A.SellDate, 7, 2)  ELSE '' END ";
            Tsql += Environment.NewLine + " ELSE '' END Last_Date ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN CONVERT(VARCHAR, tbl_Memberinfo_AutoShip.Proc_Cnt) ELSE '' END AS Next_Cnt ";
            //Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN CONVERT(VARCHAR, CAST(PROC_DATE AS DATETIME), 23)  ELSE '' END AS Next_Date";
            //Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN CONVERT(VARCHAR, PROC_DATE)  ELSE '' END AS Next_Date";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN LEFT(CONVERT(VARCHAR, PROC_DATE ),4)+ '-' + SUBSTRING(CONVERT(VARCHAR, PROC_DATE ),5,2)+ '-' +  SUBSTRING(CONVERT(VARCHAR, PROC_DATE ),7,2) ELSE '' END AS Next_Date ";
            //Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN LEFT(tbl_Memberinfo_AutoShip.Proc_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Proc_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Proc_Date, 7, 2) ELSE '' END AS Next_Date ";
            Tsql += Environment.NewLine + " , MC1.FlagName ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.End_Reason ";
            Tsql += Environment.NewLine + " , CASE WHEN ISNULL(tbl_Memberinfo_AutoShip.End_Date, '') <> '' THEN ";
            Tsql += Environment.NewLine + " LEFT(tbl_Memberinfo_AutoShip.End_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.End_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.End_Date, 7, 2) ";
            Tsql += Environment.NewLine + " ELSE '' END End_Date ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.TotalPrice ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.TotalPV ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.TotalCV ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.Req_State ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.RecordID ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.Proc_25_Cnt";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.Proc_25_Won_Sum";
            Tsql += Environment.NewLine + " FROM tbl_Memberinfo_AutoShip (NOLOCK) ";
            Tsql += Environment.NewLine + " INNER JOIN tbl_Memberinfo (NOLOCK) ON tbl_Memberinfo_AutoShip.mbid = tbl_Memberinfo.mbid AND tbl_Memberinfo_AutoShip.mbid2 = tbl_Memberinfo.mbid2 ";
            Tsql += Environment.NewLine + " LEFT OUTER JOIN tbl_MasterCode MC1 (NOLOCK) ON MC1.ModuleCode = 'AutoShip' AND MC1.ClassCode = '001' AND MC1.FlagCode = tbl_Memberinfo_AutoShip.Req_State ";
            Tsql += Environment.NewLine + " LEFT OUTER JOIN ( ";
// 2018-10-23 SellWeek 이뭐지?? 안쓰는거같음             Tsql += Environment.NewLine + "                 SELECT A.Auto_Seq, MAX(Sell_WeekCount) SellWeek, MAX(SellDate) SellDate ";
            Tsql += Environment.NewLine + "                 SELECT A.Auto_Seq, 0 SellWeek, MAX(SellDate) SellDate ";
            Tsql += Environment.NewLine + "                 FROM tbl_Memberinfo_AutoShip_Mod_Del (NOLOCK) A ";
            Tsql += Environment.NewLine + "                 INNER JOIN tbl_SalesDetail (NOLOCK) B ON A.OrderNumber = B.OrderNumber ";
            Tsql += Environment.NewLine + "                 Where Del_TF = 1 ";
            Tsql += Environment.NewLine + "                 GROUP BY A.Auto_Seq ";
            Tsql += Environment.NewLine + "                 ) A ON tbl_Memberinfo_AutoShip.Auto_Seq = A.Auto_Seq ";
            Tsql += Environment.NewLine + " WHERE tbl_Memberinfo_AutoShip.mbid = '' ";

            if (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                Tsql += Environment.NewLine + " AND tbl_Memberinfo_AutoShip.mbid2 = '" + mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() + "' ";
            }

            if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() == ""))
                Tsql += Environment.NewLine + " And tbl_Memberinfo_AutoShip.Req_Date = '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";

            //가입일자로 검색 -2
            if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() != ""))
            {
                Tsql += Environment.NewLine + " And tbl_Memberinfo_AutoShip.Req_Date >= '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";
                Tsql += Environment.NewLine + " And tbl_Memberinfo_AutoShip.Req_Date <= '" + mtxtRegDate2.Text.Replace("-", "").Trim() + "'";
            }
            
            Tsql += Environment.NewLine + " ORDER BY tbl_Memberinfo_AutoShip.Auto_Seq DESC ";


        }


        private void Base_Grid_Set()
        {
            
            string Tsql = "";

            //if (mtxtMbid.Text.Replace("_", "").Trim() == "")
            //{
            //    MessageBox.Show("회원번호를 입력하시기 바랍니다.");
            //    mtxtMbid.Focus();
            //    return;
            //}

            Make_Base_Query(ref Tsql);

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

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, double> Center_MemCnt = new Dictionary<string, double>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.                
            }
            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
        }



        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 19;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 4;

            string[] g_HeaderText = {"선택", "자동주문번호" , "회원번호", "회원명" , "신청일자"
                                    , "최근자동주문결제회차", "다음자동주문예정일", "다음주문회차", "다음주문일자", "상태"
                                    , "해지사유", "재결제", "총 금액", "총 PV", "총 CV"
                                    , "_상태","기록자"    , "25만원이상연속회차","25만원이상연속누적금액"
                                    }; 

            string[] g_ColsName = {"Selected", "auto_seq" , "mbid2", "m_name" , "req_date"
                                    , "LastCnt", "Lastdate", "NextCnt", "NextDate", "status"
                                    , "EndETC", "EndDate", "PR", "PV", "BV"
                                    , "_status" , "RecordName"  ,"Proc_25_Cnt","Proc_25_Won_Sum"
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 80, 120, 100, 100, 100
                            , 100, 120, 120, 100,120
                            , 100, 80, 120, 80, 80
                            , 0, 100,120,120
                            };
            cgb.grid_col_w = g_Width;
            

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true
                                    ,true , true ,true , true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft     //5

                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter                              
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter

                               ,DataGridViewContentAlignment.MiddleLeft          
                               ,DataGridViewContentAlignment.MiddleLeft          
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight

                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                                    ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[13 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[14 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[15 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[18 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[19 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb.grid_cell_format = gr_dic_cell_format;
        }


        private void Set_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                , ds.Tables[base_db_name].Rows[fi_cnt][1]
                                , ds.Tables[base_db_name].Rows[fi_cnt][2]
                                , ds.Tables[base_db_name].Rows[fi_cnt][3]
                                , ds.Tables[base_db_name].Rows[fi_cnt][4]

                                , ds.Tables[base_db_name].Rows[fi_cnt][5]
                                , ds.Tables[base_db_name].Rows[fi_cnt][6]
                                , ds.Tables[base_db_name].Rows[fi_cnt][7]
                                , ds.Tables[base_db_name].Rows[fi_cnt][8]
                                , ds.Tables[base_db_name].Rows[fi_cnt][9]

                                , ds.Tables[base_db_name].Rows[fi_cnt][10]
                                , ds.Tables[base_db_name].Rows[fi_cnt][11]
                                , ds.Tables[base_db_name].Rows[fi_cnt][12]
                                , ds.Tables[base_db_name].Rows[fi_cnt][13]
                                , ds.Tables[base_db_name].Rows[fi_cnt][14]

                                , ds.Tables[base_db_name].Rows[fi_cnt][15]
                                , ds.Tables[base_db_name].Rows[fi_cnt][16]
                                , ds.Tables[base_db_name].Rows[fi_cnt][17]
                                , ds.Tables[base_db_name].Rows[fi_cnt][18]

                            };

            gr_dic_text[fi_cnt + 1] = row0;
        }




        private void dGridView_Base_Item_Header_Reset()
        {
            cgb_Item.grid_col_Count = 6;
            cgb_Item.basegrid = dGridView_Base_Item;
            cgb_Item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Item.grid_Frozen_End_Count = 2;

            string[] g_HeaderText = {"품목코드"  , "품목명", "수량"  , "금액"   , "PV"        
                                    , "CV"
                                    };
            cgb_Item.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 120, 200, 80, 100, 100
                            , 100
                            };
            cgb_Item.grid_col_w = g_Width;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true 
                                   };
            cgb_Item.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight    //5
                               
                               ,DataGridViewContentAlignment.MiddleRight

                              };
            cgb_Item.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[4- 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Item.grid_cell_format = gr_dic_cell_format;
        }


        private void Set_gr_dic_Item(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables["AutoShip_Item"].Rows[fi_cnt][0]
                                , ds.Tables["AutoShip_Item"].Rows[fi_cnt][1]
                                , ds.Tables["AutoShip_Item"].Rows[fi_cnt][2]
                                , ds.Tables["AutoShip_Item"].Rows[fi_cnt][3]
                                , ds.Tables["AutoShip_Item"].Rows[fi_cnt][4]

                                , ds.Tables["AutoShip_Item"].Rows[fi_cnt][5]
                            };

            gr_dic_text[fi_cnt + 1] = row0;
        }



        private void dGridView_Base_Cacu_Header_Reset()
        {
            cgb_Cacu.grid_col_Count = 5;
            cgb_Cacu.basegrid = dGridView_Base_Cacu;
            cgb_Cacu.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Cacu.grid_Frozen_End_Count = 2;

            string[] g_HeaderText = {"결제수단"  , "제휴사", "카드번호"  , "할부기간"   , "결제금액"        
                                    };
            cgb_Cacu.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 80, 100, 140, 80, 100
                            };
            cgb_Cacu.grid_col_w = g_Width;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                   };
            cgb_Cacu.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight    //5
                               


                              };
            cgb_Cacu.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Cacu.grid_cell_format = gr_dic_cell_format;
        }


        private void Set_gr_dic_Cacu(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables["AutoShip_Cacu"].Rows[fi_cnt][0]
                                , ds.Tables["AutoShip_Cacu"].Rows[fi_cnt][1]
                                , ds.Tables["AutoShip_Cacu"].Rows[fi_cnt][2]
                                , ds.Tables["AutoShip_Cacu"].Rows[fi_cnt][3]
                                , ds.Tables["AutoShip_Cacu"].Rows[fi_cnt][4]

                            };

            gr_dic_text[fi_cnt + 1] = row0;
        }

        private void dGridView_Base_Rece_Header_Reset()
        {
            cgb_Rece.grid_col_Count = 5;
            cgb_Rece.basegrid = dGridView_Base_Rece;
            cgb_Rece.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Rece.grid_Frozen_End_Count = 2;

            string[] g_HeaderText = {"수령인명"  , "연락처", "우편번호"  , "주소1"   , "주소2"        
                                    };
            cgb_Rece.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 120, 120, 80, 250, 200
                            };
            cgb_Rece.grid_col_w = g_Width;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                   };
            cgb_Rece.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft    //5
                               

                              };
            cgb_Rece.grid_col_alignment = g_Alignment;
        }


        private void Set_gr_dic_Rece(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables["AutoShip_Rece"].Rows[fi_cnt][0]
                                , ds.Tables["AutoShip_Rece"].Rows[fi_cnt][1]
                                , ds.Tables["AutoShip_Rece"].Rows[fi_cnt][2]
                                , ds.Tables["AutoShip_Rece"].Rows[fi_cnt][3]
                                , ds.Tables["AutoShip_Rece"].Rows[fi_cnt][4]

                            };

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void MtxtData_Temp_KeyPress(object sender, KeyPressEventArgs e)
        {

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

        }

        

        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }


        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {          
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
                cgb.d_Grid_view_Header_Reset(1);

                dGridView_Base_Item_Header_Reset();
                cgb_Item.d_Grid_view_Header_Reset(1);

                dGridView_Base_Cacu_Header_Reset();
                cgb_Cacu.d_Grid_view_Header_Reset(1);

                dGridView_Base_Rece_Header_Reset();
                cgb_Rece.d_Grid_view_Header_Reset(1);
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                cls_form_Meth ct = new cls_form_Meth();
                ct.from_control_clear(this, mtxtMbid);

                txt_End_Reason.Text = "";
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

                txt_End_Reason.Text = "";
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

       


        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
           // SendKeys.Send("{TAB}");
        }

        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            /*하단 그리드 초기화*/
            dGridView_Base_Item_Header_Reset();
            cgb_Item.d_Grid_view_Header_Reset(1);

            dGridView_Base_Cacu_Header_Reset();
            cgb_Cacu.d_Grid_view_Header_Reset(1);

            dGridView_Base_Rece_Header_Reset();
            cgb_Rece.d_Grid_view_Header_Reset(1);

            /*ADS 번호 가지고 오기*/
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[1].Value != null))
            {
                string AutoSeq = "";
                AutoSeq = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                

                Base_Grid_Item_Set(AutoSeq);
                Base_Grid_Cacu_Set(AutoSeq);
                Base_Grid_Rece_Set(AutoSeq);
            }
             
        }


        private void Base_Grid_Item_Set(string AutoSeq)
        {

            string Tsql = "";

            Tsql = " SELECT  ";
            Tsql = Tsql + " ItemCode ";
            Tsql = Tsql + " , ItemName ";
            Tsql = Tsql + " , ItemCount ";
            Tsql = Tsql + " , ItemTotalPrice ";
            Tsql = Tsql + " , ItemTotalPV ";
            Tsql = Tsql + " , ItemTotalCV  ";
            Tsql = Tsql + " FROM tbl_Memberinfo_AutoShip_Item (NOLOCK) ";
            Tsql = Tsql + " WHERE Auto_Seq = '" + AutoSeq + "' ";

            cls_form_Meth cm = new cls_form_Meth();
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "AutoShip_Item", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_Item(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.                
            }
            
            cgb_Item.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Item.db_grid_Obj_Data_Put();
        }


        private void Base_Grid_Cacu_Set(string AutoSeq)
        {
            string Tsql = "";

            Tsql = " SELECT ";
            Tsql = Tsql + " CASE WHEN Cacu_Type = '3' THEN '카드' WHEN Cacu_Type = '4' THEN '포인트' ELSE '' END ";
            Tsql = Tsql + " , CASE WHEN Cacu_Type = '4' THEN CardName ELSE '' END ";
            Tsql = Tsql + " , CASE WHEN Cacu_Type = '3' THEN dbo.DECRYPT_AES256(CardNumber) ELSE '' END ";
            Tsql = Tsql + " , CASE WHEN Cacu_Type = '3' THEN Installment_Period ELSE '' END ";
            Tsql = Tsql + " , Payment_Amt ";
            Tsql = Tsql + " FROM tbl_Memberinfo_AutoShip_Cacu (NOLOCK) ";
            Tsql = Tsql + " WHERE Auto_Seq = '" + AutoSeq + "' ";


            cls_form_Meth cm = new cls_form_Meth();
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "AutoShip_Cacu", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_Cacu(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.                
            }

            cgb_Cacu.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Cacu.db_grid_Obj_Data_Put();
        }


        private void Base_Grid_Rece_Set(string AutoSeq)
        {

            string Tsql = "";

            Tsql = " SELECT ";
            Tsql = Tsql + " Rec_Name ";
            Tsql = Tsql + " , Rec_Tel ";
            Tsql = Tsql + " , Rec_Addcode ";
            Tsql = Tsql + " , Rec_Address1 ";
            Tsql = Tsql + " , Rec_Address2 ";
            Tsql = Tsql + " FROM tbl_Memberinfo_AutoShip_Rece (NOLOCK) ";
            Tsql = Tsql + " WHERE Auto_Seq = '" + AutoSeq + "' ";


            cls_form_Meth cm = new cls_form_Meth();
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "AutoShip_Rece", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_Rece(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.                
            }

            cgb_Rece.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Rece.db_grid_Obj_Data_Put();
        }


        private Boolean Check_ADS_End()
        {
            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {   
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V" && dGridView_Base.Rows[i].Cells[14].Value.ToString() == "99")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("There are already cancellations.");
                    }
                    else
                    {
                        MessageBox.Show("이미 해지된 건이 존재합니다.");
                    }
                    return false;
                }
            }

            string Chk = "N";

            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                {
                    Chk = "Y";
                    break;
                }
            }

            if (Chk == "N")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("There are no items selected.");
                }
                else
                {
                    MessageBox.Show("선택된 내역이 없습니다.");
                }
                return false;
            }
            
            return true;
        }

        private void butt_ADS_End_Click(object sender, EventArgs e)
        {
            if (Check_ADS_End() == false) return;

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            int Save_Error_Check = 0;

            Save_ADS_End(ref Save_Error_Check);

            if (Save_Error_Check > 0)
            {
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                Base_Grid_Set();
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Save_ADS_End(ref int Save_Error_Check)
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string StrSql = "";
                string Auto_Seq = "";
                string EndDate = "";

                cls_Search_DB csd_2 = new cls_Search_DB();
                EndDate = csd_2.Select_Today("yyyyMMdd");

                for (int i = 0; i < dGridView_Base.Rows.Count; i++)
                {
                    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                    {
                        Auto_Seq = dGridView_Base.Rows[i].Cells[1].Value.ToString();

                        StrSql = " EXEC Usp_End_Memberinfo_Autoship_CS '" + Auto_Seq + "', '" + EndDate + "', '" + cls_User.gid + "', '" + txt_End_Reason.Text + "' ";

                        Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);
                    }
                }

               
                    tran.Commit();
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


        private void dGridView_Base_CellClick(object sender, DataGridViewCellEventArgs e)
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

        private void butt_ADS_Recovery_Click(object sender, EventArgs e)
        {
            if (Check_ADS_Recvoery() == false) return;

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            int Save_Error_Check = 0;

            Save_ADS_Recovery(ref Save_Error_Check);

            if (Save_Error_Check > 0)
            {
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                Base_Grid_Set();
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private bool Check_ADS_Recvoery()
        {
            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V" && (
                        dGridView_Base.Rows[i].Cells[14].Value.ToString() == "20" ||
                        dGridView_Base.Rows[i].Cells[14].Value.ToString() == "30"))
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("There is already something in progress.");
                    }
                    else
                    {
                        MessageBox.Show("이미 진행중인 건이 존재합니다.");
                    }
                    return false;
                }
            }

            string Chk = "N";

            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                {
                    Chk = "Y";
                    break;
                }
            }

            if (Chk == "N")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("There are no items selected.");
                }
                else
                {
                    MessageBox.Show("선택된 내역이 없습니다.");
                }
                return false;
            }

            return true;
        }

        private void Save_ADS_Recovery(ref int Save_Error_Check)
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string Auto_Seq = "";
                string EndDate = "";

                cls_Search_DB csd_2 = new cls_Search_DB();
                EndDate = csd_2.Select_Today("yyyyMMdd");
                StringBuilder sb = new StringBuilder();

   

                for (int i = 0; i < dGridView_Base.Rows.Count; i++)
                {
                    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                    {
                        Auto_Seq = dGridView_Base.Rows[i].Cells[1].Value.ToString();

                        sb.Clear();
                        sb.AppendLine("INSERT INTO tbl_Memberinfo_AutoShip_Mod_Del");
                        sb.AppendLine("([Auto_Seq],[mbid],[mbid2],[Req_Type],[Req_State],[Req_Date],[Req_Month],[AutoExtend],[Start_Date],[End_Date]");
                        sb.AppendLine(",[Extend_Date],[Proc_Date],[NextProc_Date],[TotalPrice],[TotalPV],[TotalCV],[Etc],[End_Reason],[Proc_Cnt],[DeliveryCharge]");
                        sb.AppendLine(",[CustomerGroupKey],[RecordID],[RecordTime], [Del_TF], [DelRecordID], [DelRecordTime],[Send_Error])");
                        sb.AppendLine("SELECT [Auto_Seq],[mbid],[mbid2],[Req_Type],[Req_State],[Req_Date],[Req_Month],[AutoExtend],[Start_Date],[End_Date]");
                        sb.AppendLine(",[Extend_Date],[Proc_Date],[NextProc_Date],[TotalPrice],[TotalPV],[TotalCV],[Etc],[End_Reason],[Proc_Cnt],[DeliveryCharge]");
                        sb.AppendLine(",[CustomerGroupKey],[RecordID],[RecordTime], 0, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), 'Recovery'");
                        sb.AppendLine("FROM tbl_Memberinfo_AutoShip (nolock) WHERE Auto_Seq = '" + Auto_Seq + "'");
                        Temp_Connect.Insert_Data(sb.ToString(), base_db_name, Conn, tran);

                        sb.Clear();
                        sb.AppendLine("DELETE FROM tbl_Memberinfo_AutoShip  WHERE Auto_Seq = '" + Auto_Seq + "'");
                        Temp_Connect.Insert_Data(sb.ToString(), base_db_name, Conn, tran);

                        sb.Clear();
                        sb.AppendLine("INSERT INTO [dbo].[tbl_Memberinfo_AutoShip]");
                        sb.AppendLine("([Auto_Seq],[mbid],[mbid2],[Req_Type],[Req_State],[Req_Date],[Req_Month],[AutoExtend],[Start_Date],[End_Date]");
                        sb.AppendLine(",[Extend_Date],[Proc_Date],[NextProc_Date],[TotalPrice],[TotalPV],[TotalCV],[Etc],[End_Reason],[Proc_Cnt],[DeliveryCharge]");
                        sb.AppendLine(",[CustomerGroupKey],[RecordID],[RecordTime]) ");
                        sb.AppendLine("SELECT [Auto_Seq],[mbid],[mbid2],[Req_Type],[Req_State],[Req_Date],[Req_Month],[AutoExtend],[Start_Date],[End_Date]");
                        sb.AppendLine(",[Extend_Date],[Proc_Date],[NextProc_Date],[TotalPrice],[TotalPV],[TotalCV],[Etc],[End_Reason],[Proc_Cnt],[DeliveryCharge]");
                        sb.AppendLine(",[CustomerGroupKey],[RecordID],[RecordTime]");
                        sb.AppendLine(" FROM tbl_Memberinfo_AutoShip_Mod_Del (nolock) ");
                        sb.AppendLine(" WHERE Send_Error = '' AND Auto_Seq = '" + Auto_Seq + "' ");
                        sb.AppendLine(" AND DelRecordTime = (SELECT MAX(DelRecordTime) FROM tbl_Memberinfo_AutoShip_Mod_Del WHERE Send_Error = '' AND Auto_Seq = '" + Auto_Seq + "')");
                        Temp_Connect.Insert_Data(sb.ToString(), base_db_name, Conn, tran);


                        break;

                    }
                }

                    tran.Commit();
                    Save_Error_Check = 1;
              

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
    }




}
