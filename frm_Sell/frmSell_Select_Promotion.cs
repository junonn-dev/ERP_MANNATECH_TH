﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
//using System.Collections;

namespace MLM_Program
{
    public partial class frmSell_Select_Promotion : Form
    {
        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);



        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cg_sub = new cls_Grid_Base();
        private const string base_db_name = "tbl_Promotion";
        private int Data_Set_Form_TF;

        //public delegate void SendNumberDele(string Send_Number, string Send_Name);
        //public event SendNumberDele Send_Mem_Number;
        Series series_Day = new Series();
        Dictionary<string, int> chart_dic = new Dictionary<string, int>();


        public frmSell_Select_Promotion()
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

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_sub.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;  

        }


        private void frm_Base_Activated(object sender, EventArgs e)
        {
           //19-03-11 깜빡임제거 this.Refresh();
        }


        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Select.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Excel.Left = butt_Select.Left + butt_Select.Width + 2;
            //butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Select);
            //cfm.button_flat_change(butt_Delete);
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

            //그리드일 경우에는 DEL키로 행을 삭제하는걸 막는다.
            if (sender is DataGridView)
            {
               

                if (e.KeyValue == 13)
                {
                    EventArgs ee =null;
                    dGridView_Base_DoubleClick(sender, ee);
                    e.Handled = true;
                } // end if
            }

            Button T_bt = butt_Exit;
            if (e.KeyValue == 123)
                T_bt = butt_Exit;    //닫기  F12
            if (e.KeyValue == 113)
                T_bt = butt_Select;     //조회  F1
            // if (e.KeyValue == 115)
            //     T_bt = butt_Delete;   // 삭제  F4
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
           return true;
        }


        private void Make_Base_Query(ref string Tsql)
        {

            Tsql = "Select ";
            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + " Mbid + '-' + Convert(Varchar,Mbid2) mbid2 ";
            else
                Tsql = Tsql + " Mbid2  mbid2";
            Tsql = Tsql + " ,M_Name ";
            Tsql = Tsql + " ,Self_PV ";
            Tsql = Tsql + " ,Self_Point ";
            Tsql = Tsql + " ,Down_PV ";
            Tsql = Tsql + " ,Down_Point ";
            Tsql = Tsql + ", Self_Pv + Down_PV  Sum_PV ";
            Tsql = Tsql + ", Down_G_110 + Down_G_100 + Down_G_90 + Down_G_80 + Down_G_70 ";
            Tsql = Tsql + "  + Down_G_60 + Down_G_50 + Down_G_40 + Down_G_30  Grade ";
            Tsql = Tsql + " , Down_Grade_Sum ";
            Tsql = Tsql + " From ufn_Mem_Promo_Search_20160318 () ";
        }



        private void Make_Base_Query_(ref string Tsql)
        {
            string strSql = " ";

            strSql = strSql + " Where Self_Point + Down_Point + Down_G_110 + Down_G_100 + Down_G_90 + Down_G_80 + Down_G_70 + Down_G_60 + Down_G_50 + Down_G_40 + Down_G_30  + Down_Grade_Sum > 0 ";

            if (mtxtMbid.Text.Trim() != "")
                strSql = "And  mbid2 = " + mtxtMbid.Text.Trim();

            if (mtxtMbid.Text.Trim() != "" && txtName.Text.Trim() != "")
                strSql = strSql = " And M_Name = '" + txtName.Text.Trim() + "' ";
            else if (mtxtMbid.Text.Trim() == "" && txtName.Text.Trim() != "")
                strSql = " And M_Name = '" + txtName.Text.Trim() + "' ";
                

            Tsql = Tsql + strSql;
    
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
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name , this.Text ) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            DataSet ds_View = new DataSet();
            DataTable tbl_Promotion = new DataTable("Promotion");

            tbl_Promotion.Columns.Add("mbid2", typeof(string));
            tbl_Promotion.Columns.Add("M_Name", typeof(string));
            tbl_Promotion.Columns.Add("Gubun", typeof(string));
            tbl_Promotion.Columns.Add("PV", typeof(float));
            tbl_Promotion.Columns.Add("Point", typeof(int));
            tbl_Promotion.Columns.Add("Grade", typeof(int));
            
            for (int i = 0; i < ReCnt; i++)
            {
                //본인매출
                tbl_Promotion.Rows.Add(ds.Tables[base_db_name].Rows[i]["mbid2"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["M_Name"].ToString()
                                    , "본인매출"
                                    , ds.Tables[base_db_name].Rows[i]["Self_PV"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["Self_Point"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["Grade"].ToString()
                                    );
                //신규추천매출
                tbl_Promotion.Rows.Add(ds.Tables[base_db_name].Rows[i]["mbid2"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["M_Name"].ToString()
                                    , "신규추천매출"
                                    , ds.Tables[base_db_name].Rows[i]["Down_PV"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["Down_Point"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["Down_Grade_Sum"].ToString()
                                    );
                //합계
                tbl_Promotion.Rows.Add(ds.Tables[base_db_name].Rows[i]["mbid2"].ToString()
                                    , ds.Tables[base_db_name].Rows[i]["M_Name"].ToString()
                                    , "합계"
                                    , ds.Tables[base_db_name].Rows[i]["Sum_PV"].ToString()
                                    , (double.Parse(ds.Tables[base_db_name].Rows[i]["Self_Point"].ToString())
                                     + double.Parse(ds.Tables[base_db_name].Rows[i]["Down_Point"].ToString())).ToString()
                                    , (double.Parse(ds.Tables[base_db_name].Rows[i]["Grade"].ToString() )
                                     + double.Parse(ds.Tables[base_db_name].Rows[i]["Down_Grade_Sum"].ToString())).ToString()
                                    );
                
            }
            ds_View.Tables.Add(tbl_Promotion);


                       

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ds_View.Tables["Promotion"].Rows.Count - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds_View, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   

        }




        private void Set_gr_dic(ref DataSet ds_View, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds_View.Tables["Promotion"].Rows[fi_cnt][0]
                                ,ds_View.Tables["Promotion"].Rows[fi_cnt][1]  
                                ,ds_View.Tables["Promotion"].Rows[fi_cnt][2]  
                                ,ds_View.Tables["Promotion"].Rows[fi_cnt][3]  
                                ,ds_View.Tables["Promotion"].Rows[fi_cnt][4]  
                                ,ds_View.Tables["Promotion"].Rows[fi_cnt][5]  
                                 };

            gr_dic_text[fi_cnt + 1] = row0;

            
        }





        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 6;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 3;
            //cg_sub.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            cgb.grid_Merge = true;
            cgb.grid_Merge_Col_Start_index = 0;
            cgb.grid_Merge_Col_End_index = 1;

            string[] g_HeaderText = {"회원번호"  , "성명"   , "구분"  , "PV"   , "Point"    
                                    , "승급현황"
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 100, 100, 100, 100, 100  
                             , 100
                            };
            cgb.grid_col_w = g_Width;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb.grid_cell_format = gr_dic_cell_format;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                                                         
                                    ,true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight  //5    
                           
                               ,DataGridViewContentAlignment.MiddleRight
                              };
            cgb.grid_col_alignment = g_Alignment;

            dGridView_Base.RowHeadersVisible = false;
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





        private void MtxtData_Temp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }

        
        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);            
            
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
                                   
        }

        

        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
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

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cg_sub.d_Grid_view_Header_Reset();
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


                cls_form_Meth ct = new cls_form_Meth();
                mtxtMbid.Text = "";
                txtName.Text = "";
            }
            else if (bt.Name == "butt_Select")
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset();
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<3
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cg_sub.d_Grid_view_Header_Reset();
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
            Excel_Export_File_Name = this.Text;
            Excel_Export_From_Name = this.Name;
            return dGridView_Sub_001;
        }

       
        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
        }



        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string Send_Code = ""; //string Send_Name = "";
                Send_Code = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                //Send_Name = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Sub_Grid_Set(Send_Code);
                this.Cursor = System.Windows.Forms.Cursors.Default; 
            }            
        }




        private void Make_Sub_Query(ref string StrSql, string search_Code )
        {         
            
            StrSql = "Select  ";
            if (cls_app_static_var.Member_Number_1 > 0)
                StrSql = StrSql + " Mbid + '-' + Convert(Varchar,Mbid2) mbid2";
            else
                StrSql = StrSql + " Mbid2 mbid2";

            StrSql = StrSql + ", M_Name ";
            StrSql = StrSql + ", Self_PV ";
            StrSql = StrSql + ", Down_G_110 + Down_G_100 + Down_G_90 + Down_G_80 + Down_G_70 ";
            StrSql = StrSql + "  + Down_G_60 + Down_G_50 + Down_G_40 + Down_G_30  Down_G ";
            StrSql = StrSql + ", Self_Point ";
            StrSql = StrSql + " from ufn_Mem_Promo_Search_20160318_Detail ('" + search_Code + "') ";

        }



        private void Make_Sub_Query_(ref string Tsql,string search_Code)
        {
            string strSql = " ";       
            strSql = " Where RegTime Between '20160201' And '20160531' ";
            strSql = strSql + " And Nominid2 = " + search_Code;
            strSql = strSql + " And Self_Point + Down_G_110 + Down_G_100 + Down_G_90 + Down_G_80 + Down_G_70 + Down_G_60 + Down_G_50 + Down_G_40 + Down_G_30 > 0 ";
            
            Tsql = Tsql + strSql;
            Tsql = Tsql + " Order by Mbid2  ";
        }




        private void Sub_Grid_Set(string search_Code)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_sub.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


            string Tsql = "";
            Make_Sub_Query(ref Tsql, search_Code);

            Make_Sub_Query_(ref Tsql, search_Code);

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;                        
            
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_Sub_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cg_sub.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cg_sub.db_grid_Obj_Data_Put();
        }


      

        private void dGridView_Sub_Header_Reset()
        {


            cg_sub.grid_col_Count = 5;
            cg_sub.basegrid = dGridView_Sub_001;
            cg_sub.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cg_sub.grid_Frozen_End_Count = 2;
            //cg_sub.grid_Frozen_End_Count = 2;
            cg_sub.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원_번호"  , "성명"   , "PV합"  , "승급현황"   , "포인트"                                        
                                    };
            cg_sub.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 85, 90, 130, 120,120
                            };
            cg_sub.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                   };
            cg_sub.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight  //5
                              };
            cg_sub.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cg_sub.grid_cell_format = gr_dic_cell_format;
        }


        private void Set_Sub_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][3]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][4]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }












    }
}
