﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MLM_Program
{
    public partial class frmClose_1_Select_01 : Form
    {
        
        private const string base_db_name = "tbl_DB";
        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Sum = new cls_Grid_Base();
        private int Data_Set_Form_TF = 0 ; 
        private int Form_Load_TF = 0;

        public frmClose_1_Select_01()
        {
            InitializeComponent();
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

        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Form_Load_TF = 0;

            txt_ETC1.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC2.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC3.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC4.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC5.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC6.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC7.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC8.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_ETC9.BackColor = cls_app_static_var.txt_Enable_Color;


            mtxtFromDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtFromDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtToDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtToDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPayDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtPayDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            //txtPayDate.BackColor = cls_app_static_var.txt_Enable_Color;

            dGridView_Base.Width = this.Width / 2 + 50;
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
        }

        private void frm_Base_Activated(object sender, EventArgs e)
        {
           //19-03-11 깜빡임제거 this.Refresh();

            if (Form_Load_TF == 0)
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                dGridView_Sum_Base_Header_Reset();
                cgb_Sum.d_Grid_view_Header_Reset(1);
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                Form_Load_TF = 1;
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
                            //  cfm.form_Group_Panel_Enable_True(this);
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



        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Clear")
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                dGridView_Sum_Base_Header_Reset();
                cgb_Sum.d_Grid_view_Header_Reset(1);

                chart_Pay.Series.Clear();
                chart_Cnt.Series.Clear();        
                tabControl1.SelectedIndex = 0;

                textToEndDate.Text = "";
                tab_Pay_Tab_Dispose();
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                cls_form_Meth ct = new cls_form_Meth();
                
                ct.from_control_clear(tabControl2);
                ct.from_control_clear(this, mtxtFromDate1);
                
                
            }
            else if (bt.Name == "butt_Select")
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();

                dGridView_Sum_Base_Header_Reset();
                cgb_Sum.d_Grid_view_Header_Reset(1);

                chart_Pay.Series.Clear();
                chart_Cnt.Series.Clear();                
                Save_Nom_Line_Chart();
                tabControl1.SelectedIndex = 0;

                cls_form_Meth ct = new cls_form_Meth();
                ct.from_control_clear(tabControl2);
                tab_Pay_Tab_Dispose();                
                
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

            else if (bt.Name == "butt_Exp")
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
            cls_form_Meth cm = new cls_form_Meth();
            Excel_Export_File_Name = this.Text; // cm._chang_base_caption_search ("주간_마감별_집계");
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;


        }






        private void Make_Base_Query(ref string Tsql)
        {


            //string[] g_HeaderText = {"웹적용여부","마감_시작일"  ,"마감_종료일"  ,"지급_일자" , "기간판매액"        
            //                   , "기간판매PV" , "기간반품액"     , "기간반품PV"  , "직접판매수당"  ,"추천매칭"      
            //                   , "추천보너스"  , "직접판매수당-대"  , "추천매칭-후원", ""   , ""   
            //                    , ""  , ""   , "" , ""     , ""     
            //                     , "후원공제"   , "반품공제액"  , "기타수당" , "기간수당합계"  , "기간소득세"  
            //                    , "기간주민세"  ,"기간실지급액"        , "기간지급률" , "", "" 

            //                    ,"" , ""  , "", ""   , ""  
            //                    , ""  , "", ""   , ""  , ""   
            //                    ,"" , ""   
            //                        };
            cls_form_Meth cm = new cls_form_Meth();

            //스타보너스 ,  바이너리보너스 , 추천매칭
            Tsql = "Select  ";
            Tsql = Tsql + " Case When Web_V_TF =1 then '적용' ELSE '' End  ";
            Tsql = Tsql + " ,LEFT(FromEndDate,4) +'-' + LEFT(RIGHT(FromEndDate,4),2) + '-' + RIGHT(FromEndDate,2) FromEndDate ";
            Tsql = Tsql + " , LEFT(ToEndDate,4) +'-' + LEFT(RIGHT(ToEndDate,4),2) + '-' + RIGHT(ToEndDate,2) ToEndDate ";
            Tsql = Tsql + " , LEFT(PayDate,4) +'-' + LEFT(RIGHT(PayDate,4),2) + '-' + RIGHT(PayDate,2) PayDate";
            Tsql = Tsql + " , TotalSellAmount , TotalSellPv ";

            Tsql = Tsql + " , TotalReturnSellAmount , TotalReturnSellPV,TotalSellCV,TotalReturnSellCV, Allowance1 , Allowance2 , Allowance3  ";

            Tsql = Tsql + " , Allowance4, Allowance5 ,  Allowance6 , Allowance7 , Allowance8 ";

            Tsql = Tsql + " ,Allowance9 ,  Isnull(Allowance10,0) + Isnull(Allowance11,0) +Isnull(Allowance12,0)  Allowance10 , Allowance28 ";

            Tsql = Tsql + " , Isnull(Allowance29,0)  Allowance19 , Allowance30 , SumAllowance , SumInComeTax , SumResidentTax "; 
            Tsql = Tsql + " , SumTruePayment , SumAllowanceRate ,'','',''  ";


            Tsql = Tsql + " , Allowance1Rate , Allowance2Rate ,Allowance3Rate , Allowance4Rate ,Allowance5Rate ";
            Tsql = Tsql + " , Allowance6Rate , Allowance7Rate ,Allowance8Rate , Allowance9Rate ,Allowance10Rate ";

            Tsql = Tsql + " , Web_V_TF ";

            Tsql = Tsql + " From tbl_CloseTotal_01 (nolock) ";            
        }



        private void Make_Base_Query_(ref string Tsql)
        {
            string strSql = " Where ToEndDate <> ''  ";

            //string Mbid = ""; int Mbid2 = 0;
            ////회원번호1로 검색
            //if (
            //    (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
            //    &&
            //    (mtxtMbid2.Text.Replace("-", "").Replace("_", "").Trim() == "")
            //    )
            //{
            //    cls_Search_DB csb = new cls_Search_DB();
            //    if (csb.Member_Nmumber_Split(mtxtMbid.Text, ref Mbid, ref Mbid2) == 1)
            //    {
            //        if (Mbid != "")
            //            strSql = strSql + " And tbl_SalesDetail.Mbid ='" + Mbid + "'";

            //        if (Mbid2 >= 0)
            //            strSql = strSql + " And tbl_SalesDetail.Mbid2 = " + Mbid2;
            //    }


            //}

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
            //            strSql = strSql + " And tbl_SalesDetail.Mbid >='" + Mbid + "'";

            //        if (Mbid2 >= 0)
            //            strSql = strSql + " And tbl_SalesDetail.Mbid2 >= " + Mbid2;
            //    }

            //    if (csb.Member_Nmumber_Split(mtxtMbid2.Text, ref Mbid, ref Mbid2) == 1)
            //    {
            //        if (Mbid != "")
            //            strSql = strSql + " And tbl_SalesDetail.Mbid <='" + Mbid + "'";

            //        if (Mbid2 >= 0)
            //            strSql = strSql + " And tbl_SalesDetail.Mbid2 <= " + Mbid2;
            //    }
            //}


            ////회원명으로 검색
            //if (txtName.Text.Trim() != "")
            //    strSql = strSql + " And tbl_SalesDetail.M_Name Like '%" + txtName.Text.Trim() + "%'";

            //가입일자로 검색 -1
            if ((mtxtFromDate1.Text.Replace("-", "").Trim() != "") && (mtxtFromDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And FromEndDAte = '" + mtxtFromDate1.Text.Replace("-", "").Trim() + "'";

            //가입일자로 검색 -2
            if ((mtxtFromDate1.Text.Replace("-", "").Trim() != "") && (mtxtFromDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And FromEndDAte >= '" + mtxtFromDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And FromEndDate <= '" + mtxtFromDate2.Text.Replace("-", "").Trim() + "'";
            }


            //기록일자로 검색 -1
            if ((mtxtToDate1.Text.Replace("-", "").Trim() != "") && (mtxtToDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And ToEndDate = '" + mtxtToDate1.Text.Replace("-", "").Trim() + "'";

            //기록일자로 검색 -2
            if ((mtxtToDate1.Text.Replace("-", "").Trim() != "") && (mtxtToDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And ToEndDate >= '" + mtxtToDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And ToEndDate <= '" + mtxtToDate2.Text.Replace("-", "").Trim() + "'";
            }


            //기록일자로 검색 -1
            if ((mtxtPayDate1.Text.Replace("-", "").Trim() != "") && (mtxtPayDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And PayDate = '" + mtxtPayDate1.Text.Replace("-", "").Trim() + "'";

            //기록일자로 검색 -2
            if ((mtxtPayDate1.Text.Replace("-", "").Trim() != "") && (mtxtPayDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And PayDate >= '" + mtxtPayDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And PayDate <= '" + mtxtPayDate2.Text.Replace("-", "").Trim() + "'";
            }



            Tsql = Tsql + strSql;
            Tsql = Tsql + " Order by ToEndDAte DESC ";            
        }




        private void Base_Grid_Set()
        {
            string Tsql = "";
            Make_Base_Query(ref Tsql);

            Make_Base_Query_(ref Tsql);

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++


            
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();           

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

               
            }

            //Reset_Chart_Total(Sum_13, Sum_14, Sum_15, Sum_17);
            //Reset_Chart_Total(ref SelType_1);
            //Reset_Chart_Total(Sell_Cnt_1, Sell_Cnt_2);



            

            if (gr_dic_text.Count > 0)
            {
                put_Sum_Dataview(ds, ReCnt);
                
                //put_Chart(ds, ReCnt);               
            }

            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
        }

       


        private void dGridView_Base_Header_Reset()
        {

            cgb.grid_col_Count = 42;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //스타보너스 ,  바이너리보너스 , 추천매칭
            string[] g_HeaderText = {"웹적용여부","마감_시작일"  ,"마감_종료일"  ,"지급_일자" , "기간판매액"        
                               , "기간판매PV" , "기간반품액"     , "기간반품PV"    , "_기간판매CV"    , "_기간반품CV"    
                               , "직접판매수당"  ,"추천수당"    , "직접판매수당_B"  , "추천수당_B"  , "비긴즈수당"
                               , "후원수당"   , "추천매칭수당"    , "추천관리수당"  , "동급적용수당"   , "직급수당" 
                                 , ""   , "반품공제액"  , "기타수당" , "기간수당합계"  , "기간소득세"  
                                , "기간주민세"  ,"기간실지급액"        , "기간지급률" , "", "" 

                                ,"" , ""  , "", ""   , ""  
                                , ""  , "", ""   , ""  , ""   
                                ,"" , ""   
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 80, 130 , 130, 90, 110
                            , 90, 90   ,101, 0, 0
                            , 10 , 10  ,10 ,10, 10
                             , 10, 10,10 , 10, 10
                             , 0 , 100,100 , 100 , 100 
                             , 100, 100 ,100,0,0

                             , 0 , 0  ,0 , 0, 0
                             , 0 , 0  ,0 , 0, 0
                             ,0 , 0 
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  

                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  

                                    ,true  ,true  
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight//5    
  
                               ,DataGridViewContentAlignment.MiddleRight                            
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight                              
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight//10

                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight   
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight  //15   

                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight                            
                               ,DataGridViewContentAlignment.MiddleRight                              
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight//20

                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight   
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight  //25   

                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  //30

                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  //35

                                ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  //40

                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter  
                              };
            cgb.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;
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
            gr_dic_cell_format[21 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[22 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[23 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[24 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[25 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[26 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[27 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb.grid_cell_format = gr_dic_cell_format;

        }


        private void Set_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;
            object[] row0 = new object[cgb.grid_col_Count];

            while (Col_Cnt < cgb.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }

            gr_dic_text[fi_cnt + 1] = row0;


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
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][21]  
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][22]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][23]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][24]

            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][25]
            //                    ,ds.Tables[base_db_name].Rows[fi_cnt][26]
            //                     };

            //gr_dic_text[fi_cnt + 1] = row0;
        }


        private void put_Sum_Dataview(DataSet ds, int ReCnt)
        {
            Dictionary<int, object[]> gr_dic_text_Sum = new Dictionary<int, object[]>();
            Dictionary<string, double> Sum_dic = new Dictionary<string, double>();
            cls_form_Meth cm = new cls_form_Meth();
                      
            
            Sum_dic["직접판매수당"] = 0;            
            Sum_dic["추천수당"] = 0;
            Sum_dic["직접판매수당_B"] = 0;
            Sum_dic["추천수당_B"] = 0;
            Sum_dic["비긴즈수당"] = 0;
            Sum_dic["후원수당"] = 0;
            Sum_dic["추천매칭수당"] = 0;
            Sum_dic["추천관리수당"] = 0;
            Sum_dic["동급적용수당"] = 0;
            Sum_dic["직급수당"] = 0;
            Sum_dic["기타수당"] = 0;
            
            Sum_dic["반품공제액"] = 0;
            Sum_dic["기타수당"] = 0;
            Sum_dic["수당합"] = 0;
            Sum_dic["소득세합"] = 0;
            Sum_dic["주민세합"] = 0;
            Sum_dic["실지급액합"] = 0;

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Sum_dic["직접판매수당"] = Sum_dic["직접판매수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance1"].ToString());
                Sum_dic["추천수당"] = Sum_dic["추천수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance2"].ToString());
                Sum_dic["직접판매수당_B"] = Sum_dic["직접판매수당_B"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance3"].ToString());
                Sum_dic["추천수당_B"] = Sum_dic["추천수당_B"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance4"].ToString());
                Sum_dic["비긴즈수당"] = Sum_dic["비긴즈수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance5"].ToString());
                Sum_dic["후원수당"] = Sum_dic["후원수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance6"].ToString());
                Sum_dic["추천매칭수당"] = Sum_dic["추천매칭수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance7"].ToString());
                Sum_dic["추천관리수당"] = Sum_dic["추천관리수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance8"].ToString());
                Sum_dic["동급적용수당"] = Sum_dic["동급적용수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance9"].ToString());
                Sum_dic["직급수당"] = Sum_dic["직급수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance10"].ToString());
                
                Sum_dic["반품공제액"] = Sum_dic["반품공제액"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance19"].ToString());
                Sum_dic["기타수당"] = Sum_dic["기타수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance30"].ToString());
                Sum_dic["수당합"] = Sum_dic["수당합"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SumAllowance"].ToString());
                Sum_dic["소득세합"] = Sum_dic["소득세합"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SumInComeTax"].ToString());
                Sum_dic["주민세합"] = Sum_dic["주민세합"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SumResidentTax"].ToString());
                Sum_dic["실지급액합"] = Sum_dic["실지급액합"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SumTruePayment"].ToString());
            }

            int f_cnt = 0 ;
            foreach (string t_key in Sum_dic.Keys )
            {
                object[] row0 = { cm._chang_base_caption_search (t_key)
                                , Sum_dic[t_key]
                                ,""
                                ,""
                                ,""                                
                           
                            };

                gr_dic_text_Sum[f_cnt] = row0;
                f_cnt++;
            }
            

            cgb_Sum.grid_name_obj = gr_dic_text_Sum;  //배열을 클래스로 보낸다.
            cgb_Sum.db_grid_Obj_Data_Put();            
        }


        private void dGridView_Sum_Base_Header_Reset()
        {
            cgb_Sum.grid_col_Count = 5;
            cgb_Sum.basegrid = dGridView_Base_Sum;
            cgb_Sum.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;            
            cgb_Sum.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            cgb_Sum.basegrid.RowHeadersVisible = false;
            cgb_Sum.Sort_Mod_Auto_TF =1 ;
            //스타보너스 ,  바이너리보너스 , 추천매칭
            string[] g_HeaderText = {"구분"  ,"합계금액"  ,"" , ""   , ""                                      
                                    };
            cgb_Sum.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 100, 150 , 0, 0, 0                            
                            };
            cgb_Sum.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                                                         
                                   };
            cgb_Sum.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight  
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight//5      
                                                          
                              };
            cgb_Sum.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[2 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Sum.grid_cell_format = gr_dic_cell_format;  
        }



        private Boolean Check_TextBox_Error()
        {

            cls_Check_Input_Error c_er = new cls_Check_Input_Error();



            if (mtxtFromDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtFromDate1.Text, mtxtFromDate1, "Date") == false)
                {
                    mtxtFromDate1.Focus();
                    return false;
                }
            }

            if (mtxtFromDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtFromDate2.Text, mtxtFromDate2, "Date") == false)
                {
                    mtxtFromDate2.Focus();
                    return false;
                }
            }


            if (mtxtToDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtToDate1.Text, mtxtToDate1, "Date") == false)
                {
                    mtxtToDate1.Focus();
                    return false;
                }
            }

            if (mtxtToDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtToDate2.Text, mtxtToDate2, "Date") == false)
                {
                    mtxtToDate2.Focus();
                    return false;
                }
            }

            if (mtxtPayDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPayDate1.Text, mtxtPayDate1, "Date") == false)
                {
                    mtxtPayDate1.Focus();
                    return false;
                }
            }

            if (mtxtPayDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtPayDate2.Text, mtxtPayDate2, "Date") == false)
                {
                    mtxtPayDate2.Focus();
                    return false;
                }
            }         
            
            return true;
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
            //T_R.Key_Enter_13_Ncode += new Key_13_Ncode_Event_Handler(T_R_Key_Enter_13_Ncode);

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

        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
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
            ct.Search_Date_TextBox_Put(mtxtFromDate1, mtxtFromDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }



        private void radioB_R_Base_Click(object sender, EventArgs e)
        {
            //RadioButton _Rb = (RadioButton)sender;
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtToDate1, mtxtToDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }

        private void radioB_P_Base_Click(object sender, EventArgs e)
        {
            //RadioButton _Rb = (RadioButton)sender;
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtPayDate1, mtxtPayDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }




        private void put_Chart(DataSet ds, int ReCnt)
        {
            Dictionary<string, double> dic_Pay_1 = new Dictionary<string, double>();
            Dictionary<string, double> dic_Pay_2 = new Dictionary<string, double>();
            Dictionary<string, double> dic_Pay_3 = new Dictionary<string, double>();
            Dictionary<string, double> dic_Pay_tur = new Dictionary<string, double>();

            Dictionary<string, double> dic_Cnt1 = new Dictionary<string, double>();
            Dictionary<string, double> dic_Cnt2 = new Dictionary<string, double>();
            Dictionary<string, double> dic_Cnt3 = new Dictionary<string, double>();
            Dictionary<string, double> dic_Cnt_tur = new Dictionary<string, double>();

            double Pay1 = 0, Pay2 = 0, Pay3 = 0, tur = 0;
            int Cnt1 = 0, Cnt2 = 0, Cnt3 = 0, turCnt = 0;
            string ToDate = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                ToDate = ds.Tables[base_db_name].Rows[fi_cnt]["ToEndDate"].ToString();

                Pay1 = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance1"].ToString());
                Pay2 = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance2"].ToString());
                Pay3 = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance3"].ToString());
                tur = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SumTruePayment"].ToString());

                Cnt1 = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance1Cnt"].ToString());
                Cnt2 = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance2Cnt"].ToString());
                Cnt3 = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance3Cnt"].ToString());
                turCnt = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["SumAllowanceCount"].ToString());


                dic_Pay_1[ToDate] = Pay1;
                dic_Pay_2[ToDate] = Pay2;
                dic_Pay_3[ToDate] = Pay3;
                dic_Pay_tur[ToDate] = tur;

                dic_Cnt1[ToDate] = Cnt1;
                dic_Cnt2[ToDate] = Cnt2;
                dic_Cnt3[ToDate] = Cnt3;
                dic_Cnt_tur[ToDate] = turCnt;
            }

            Series series_Pay = new Series();
            Series series_Cnt = new Series();
            Put_Series_Pay(series_Pay, "스타트");
            Put_Series_Cnt(series_Cnt, "스타트");
            foreach (string t_key in dic_Pay_1.Keys)
            {
                Push_data(series_Pay, t_key, dic_Pay_1[t_key]);
                Push_data(series_Cnt, t_key, dic_Cnt1[t_key]);
            }

            Series series_Pay2 = new Series();
            Series series_Cnt2 = new Series();
            Put_Series_Pay(series_Pay2, "바이너리");
            Put_Series_Cnt(series_Cnt2, "바이너리");
            foreach (string t_key in dic_Pay_2.Keys)
            {
                Push_data(series_Pay2, t_key, dic_Pay_2[t_key]);
                Push_data(series_Cnt2, t_key, dic_Cnt2[t_key]);
            }

            Series series_Pay3 = new Series();
            Series series_Cnt3 = new Series();
            Put_Series_Pay(series_Pay3, "매칭");
            Put_Series_Cnt(series_Cnt3, "매칭");
            foreach (string t_key in dic_Pay_3.Keys)
            {
                Push_data(series_Pay3, t_key, dic_Pay_3[t_key]);
                Push_data(series_Cnt3, t_key, dic_Cnt3[t_key]);
            }


            Series series_Pay4 = new Series();
            Series series_Cnt4 = new Series();
            Put_Series_Pay(series_Pay4, "실지급액");
            Put_Series_Cnt(series_Cnt4, "실지급액");
            foreach (string t_key in dic_Pay_tur.Keys)
            {
                Push_data(series_Pay4, t_key, dic_Pay_tur[t_key]);
                Push_data(series_Cnt4, t_key, dic_Cnt_tur[t_key]);
            }

        }



        private void Push_data(Series series, string p, double  p_3)
        {
            DataPoint dp = new DataPoint();
            dp.SetValueXY(p, p_3);
            dp.Label = string.Format(cls_app_static_var.str_Currency_Type, p_3); // p_3.ToString(); //p_3.ToString();
            series.Points.Add(dp);
        }



        private void Put_Series_Pay(Series series_Pay, string PayName)
        {
            cls_form_Meth cm = new cls_form_Meth();

            series_Pay.Points.Clear();
            series_Pay["DrawingStyle"] = "Emboss";
            series_Pay["PointWidth"] = "0.5";
            series_Pay.Name = cm._chang_base_caption_search(PayName);
            series_Pay.ChartType = SeriesChartType.Column;
            series_Pay.Legend = "Legend1";
            chart_Pay.Series.Add(series_Pay);
        }

        private void Put_Series_Cnt(Series series_Pay, string PayName)
        {
            cls_form_Meth cm = new cls_form_Meth();

            series_Pay.Points.Clear();
            series_Pay["DrawingStyle"] = "Emboss";
            series_Pay["PointWidth"] = "0.5";
            series_Pay.Name = cm._chang_base_caption_search(PayName);
            series_Pay.ChartType = SeriesChartType.Column;
            series_Pay.Legend = "Legend1";
            chart_Cnt.Series.Add(series_Pay);
        }


        private void Save_Nom_Line_Chart()
        {
            chart_Pay.Series.Clear();
            chart_Pay.ChartAreas[0].AxisX.Interval = 1;
            chart_Pay.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("맑은고딕", 9);
            chart_Pay.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 7;

            chart_Cnt.Series.Clear();
            chart_Cnt.ChartAreas[0].AxisX.Interval = 1;
            chart_Cnt.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("맑은고딕", 9);
            chart_Cnt.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 7;
        }


                    
            //Sum_dic["스타트보너스"] = 0;
            //Sum_dic["바이너리보너스"] = 0;
            //Sum_dic["추천매칭"] = 0;
            //Sum_dic["바이너리공제"] = 0;

            //Sum_dic["반품공제액"] = 0;
            //Sum_dic["기타수당"] = 0;
            //Sum_dic["수당합"] = 0;
            //Sum_dic["소득세합"] = 0;
            //Sum_dic["주민세합"] = 0;
            //Sum_dic["실지급액합"] = 0;

        private void dGridView_Base_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            tab_Pay_Tab_Dispose();
            

            //"회원번호", "성명", "마감_시작일"  ,"마감_종료일"  ,"지급_일자"  
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string  ToEndDate = "";
                
                ToEndDate = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                ToEndDate = ToEndDate.Replace("-", "");
                textToEndDate.Text = ToEndDate;

                txt_ETC1.Text = (sender as DataGridView).CurrentRow.Cells[31].Value.ToString();
                txt_ETC2.Text = (sender as DataGridView).CurrentRow.Cells[32].Value.ToString();
                txt_ETC3.Text = (sender as DataGridView).CurrentRow.Cells[33].Value.ToString();
                txt_ETC4.Text = (sender as DataGridView).CurrentRow.Cells[34].Value.ToString();
                txt_ETC5.Text = (sender as DataGridView).CurrentRow.Cells[35].Value.ToString();
                txt_ETC6.Text = (sender as DataGridView).CurrentRow.Cells[36].Value.ToString();
                txt_ETC7.Text = (sender as DataGridView).CurrentRow.Cells[37].Value.ToString();
                txt_ETC8.Text = (sender as DataGridView).CurrentRow.Cells[38].Value.ToString();
                txt_ETC9.Text = (sender as DataGridView).CurrentRow.Cells[39].Value.ToString();

                if ((sender as DataGridView).CurrentRow.Cells[41].Value.ToString() == "1")
                {
                    chk_Web.Checked = true;
                }
                else
                {
                    chk_Web.Checked = false;
                }

                Allowance_Detail(ToEndDate);
                

            }
        }

        private void Allowance_Detail(string ToEndDate)
        {

            //Sum_dic["후원공제"] = Sum_dic["후원공제"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Allowance1_Cut"].ToString());

            //Sum_dic["반품공제액"] = Sum_dic["반품공제액"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Cur_DedCut_Pay"].ToString());
            //Sum_dic["기타수당"] = Sum_dic["기타수당"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Etc_Pay"].ToString());
            //Sum_dic["나눔기부"] = Sum_dic["나눔기부"] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["Sum_Gibu"].ToString());

            cls_form_Meth cm = new cls_form_Meth();

            string Pay_c = "",  fild_name = "" ;
            for (int f_cnt = 1; f_cnt <= 13; f_cnt++)
            {
                if (f_cnt == 1)
                {
                    Pay_c = "직접판매수당";
                    fild_name = " Allowance1 " ;
                }

                if (f_cnt == 2) 
                {
                    Pay_c = "추천수당";
                    fild_name = " Allowance2 " ;
                }


                if (f_cnt == 3)
                {
                    Pay_c = "직접판매수당_B";
                    fild_name = " Allowance1 ";
                }

                if (f_cnt == 4)
                {
                    Pay_c = "추천수당_B";
                    fild_name = " Allowance2 ";
                }

                if (f_cnt == 5)
                {
                    Pay_c = "비긴즈수당";
                    fild_name = " Allowance2 ";
                }


                if (f_cnt == 6)
                {
                    Pay_c = "후원수당";
                    fild_name = " Allowance2 ";
                }

                if (f_cnt == 7)
                {
                    Pay_c = "추천매칭수당";
                    fild_name = " Allowance2 ";
                }


                if (f_cnt == 8)
                {
                    Pay_c = "추천관리수당";
                    fild_name = " Allowance2 ";
                }

                if (f_cnt == 9)
                {
                    Pay_c = "동급적용수당";
                    fild_name = " Allowance2 ";
                }

                if (f_cnt == 10)
                {
                    Pay_c = "직급수당";
                    fild_name = " Allowance10 + Allowance11 + Allowance12 ";
                }
               

                if (f_cnt == 11)
                {
                    Pay_c = "반품공제액";
                    fild_name = " Cur_DedCut_Pay ";
                }

                if (f_cnt == 12)
                {
                    Pay_c = "기타수당";
                    fild_name = " Etc_Pay ";
                }


                if (f_cnt == 13)
                {
                    Pay_c = cm._chang_base_caption_search("수당합");
                    fild_name = " SumAllAllowance " ;
                }


                if (f_cnt == 1) 
                {
                    tab_Pay.TabPages[0].Text = Pay_c;
                                        
                    cls_Grid_Base cgb_P1 = new cls_Grid_Base();
                    dGridView_Base_Header_Reset(dGridView_Pay, cgb_P1); //디비그리드 헤더와 기본 셋팅을 한다.
                    cgb_P1.d_Grid_view_Header_Reset();
                    Real_Allowance_Detail(ToEndDate, fild_name, cgb_P1); 
                }
                else
                {
                    DataGridView t_DGV = new DataGridView();
                    TabPage t_tp = new TabPage();
                   
                    t_DGV.Name = Pay_c;
                    t_tp.Text = Pay_c;
                    t_tp.BackColor = tab_Pay.TabPages[0].BackColor;
                    t_tp.Controls.Add(t_DGV);
                    
                    t_DGV.Dock = DockStyle.Fill;
                    t_DGV.BackgroundColor = dGridView_Pay.BackgroundColor;

                    cls_Grid_Base cgb_P1 = new cls_Grid_Base();
                    dGridView_Base_Header_Reset(t_DGV, cgb_P1); //디비그리드 헤더와 기본 셋팅을 한다.
                    cgb_P1.d_Grid_view_Header_Reset();
                    Real_Allowance_Detail(ToEndDate, fild_name, cgb_P1); 

                    tab_Pay.Controls.Add(t_tp);     
                }

                tab_Pay.Refresh();
            }
        }




        private void Real_Allowance_Detail(string ToEndDate, string fild_name,  cls_Grid_Base cgb_P)
        {
            string StrSql = "";

            StrSql = "Select ";
            if (cls_app_static_var.Member_Number_1 > 0)
                StrSql = StrSql + " Mbid + '-' + Convert(Varchar,Mbid2) ";
            else
                StrSql = StrSql + " Mbid2 ";

            StrSql = StrSql + ",M_Name ,  " +  fild_name + " , '' , '' ";
            StrSql = StrSql + " From  tbl_ClosePay_01_Mod (nolock) ";
            StrSql = StrSql + " Where ToEndDate = '" + ToEndDate + "'";
            StrSql = StrSql + " And " + fild_name + " > 0 ";
            StrSql = StrSql + " Order By Mbid, Mbid2 ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_Pay_gr_dic(ref ds, ref gr_dic_text, fi_cnt, cgb_P);  //데이타를 배열에 넣는다.
            }

            if (gr_dic_text.Count > 0)
            {
                //put_Sum_Dataview(ds, ReCnt);                
            }

            cgb_P.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_P.db_grid_Obj_Data_Put();

        }



        private void dGridView_Base_Header_Reset(DataGridView dGridView, cls_Grid_Base cgb_P)
        {

            cgb_P.grid_col_Count = 5;
            cgb_P.basegrid = dGridView;
            cgb_P.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_P.grid_Frozen_End_Count = 3;
            cgb_P.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원번호","성명", "금액", ""  ,""                             
                                };
            cgb_P.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 100, 100 , 100, 0, 0
                            };
            cgb_P.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                                                         
                                   };
            cgb_P.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter//5      
                               
                              };
            cgb_P.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;            
            cgb_P.grid_cell_format = gr_dic_cell_format;
        }




        private void Set_Pay_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt, cls_Grid_Base cgb_P)
        {
            int Col_Cnt = 0;
            object[] row0 = new object[cgb_P.grid_col_Count];

            while (Col_Cnt < cgb_P.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }

            gr_dic_text[fi_cnt + 1] = row0;
        }



        private void tab_Pay_Tab_Dispose()
        {
            for (int fcnt = tab_Pay.TabCount - 1; fcnt > 0; fcnt--)
            {
                tab_Pay.TabPages[fcnt].Dispose();
            }

            tab_Pay.TabPages[0].Text = "";
            tab_Pay.Refresh();

            cls_Grid_Base cgb_P1 = new cls_Grid_Base();
            dGridView_Base_Header_Reset(dGridView_Pay, cgb_P1); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_P1.d_Grid_view_Header_Reset();
        }


        private void chk_Web_MouseClick(object sender, MouseEventArgs e)
        {
            string StrSql  = "";

            StrSql = "Update tbl_CloseTotal_01 Set " ;
            if (chk_Web.Checked == true )
            {
                StrSql = StrSql + " Web_V_TF =   1 ";
                dGridView_Base.CurrentRow.Cells[40].Value = "1";
            }
            else
            {
                StrSql = StrSql + " Web_V_TF =   0 ";
                dGridView_Base.CurrentRow.Cells[40].Value = "0";
            }

            StrSql = StrSql + " Where ToendDate = '" + textToEndDate.Text.Trim() + "'";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            

            Temp_Connect.Insert_Data(StrSql, "",this.Name, this.Text );

            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
        }





    }
}
