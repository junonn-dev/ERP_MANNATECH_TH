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
    public partial class frmBase_Goods_Set : Form
    {

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_2 = new cls_Grid_Base();
        private const string base_db_name = "tbl_Goods";
        private int Data_Set_Form_TF;

        Dictionary<string, TreeNode> dic_Tree_Sort_1 = new Dictionary<string, TreeNode>();  //상품 코드 분류상 대분류 관련 트리노드를 답는곳
        Dictionary<string, TreeNode> dic_Tree_Sort_2 = new Dictionary<string, TreeNode>();  //상품 코드 분류상 중분류 관려련 트리 노드를 답는곳

        

        public frmBase_Goods_Set()
        {
            InitializeComponent();
        }

        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Base_Grid_Set();

            
            Base_Goods_Grid_Set();
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            
            
            Data_Set_Form_TF = 0;            

            //상품코드 자리수에 맞추어 텍스트 박스 길이 셋팅
            if (cls_app_static_var.Item_Sort_1_Code_Length == 0)
            {
                txtNcode.Width = txtNcode.Width + txtUp.Width + 4;
                //txtNcode.MaxLength = cls_app_static_var.Item_Code_Length;
                //grB_G_Tree.Visible = false;
                trv_Item.Visible = false; 
            }
            else
            {
                //if (cls_app_static_var.Item_Sort_1_Code_Length >0 )
                //    txtNcode.MaxLength = cls_app_static_var.Item_Sort_1_Code_Length;

                //if (cls_app_static_var.Item_Sort_2_Code_Length > 0)
                //    txtNcode.MaxLength = cls_app_static_var.Item_Sort_2_Code_Length;

                //if (cls_app_static_var.Item_Sort_3_Code_Length > 0)
                //    txtNcode.MaxLength = cls_app_static_var.Item_Sort_3_Code_Length;

                txtUp.Visible = true;
                //txtUp.MaxLength = cls_app_static_var.Item_Sort_1_Code_Length
                //                + cls_app_static_var.Item_Sort_2_Code_Length
                //                + cls_app_static_var.Item_Sort_3_Code_Length - txtNcode.MaxLength;
                ////grB_G_Tree.Visible = true;
                trv_Item.Visible = true; 

                trv_Item_Set_Sort_Code();
            }

            ////txtUp.BackColor = cls_app_static_var.txt_Enable_Color;
           
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);
        }


        private void trv_Item_Set_Sort_Code()
        {                       
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string StrSql = ""; string T_base_db_name = "";
            string ItemName = ""; string ItemCode = "";
            
            DataSet ds = new DataSet();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>대분류 관련된 내역을 트리뷰에 넣는다
            T_base_db_name = "tbl_MakeItemCode1";

            StrSql = "Select ItemCode,ItemName From tbl_MakeItemCode1 ";
            StrSql = StrSql + " Order by ItemCode" ;
                        
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, T_base_db_name, ds) == false) return;
            if (Temp_Connect.DataSet_ReCount == 0) return;
                        
            trv_Item.Nodes.Clear();
            trv_Item.CheckBoxes = true;           

            for (int RowCnt = 0; RowCnt < Temp_Connect.DataSet_ReCount; RowCnt++)
            {
                ItemName = ds.Tables[T_base_db_name].Rows[RowCnt]["ItemName"].ToString();
                ItemCode = ds.Tables[T_base_db_name].Rows[RowCnt]["ItemCode"].ToString();

                TreeNode tn = trv_Item.Nodes.Add(ItemName + " - " + ItemCode );
                dic_Tree_Sort_1[ItemCode] = tn;               
            }
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<대분류 관련된 내역을 트리뷰에 넣는다
                        

            //소분류 까지 코드가 들어 와 잇다는 거는 중분류를 쓴다는 거기 때문에 중분류 코드도 넣어준다.
            if (cls_app_static_var.Item_Sort_3_Code_Length == 0) return;


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>중분류 관련된 내역을 트리뷰에 넣는다
            string UpitemCode = "";

            T_base_db_name = "tbl_MakeItemCode2";

            StrSql = "Select ItemCode,ItemName, UpitemCode From tbl_MakeItemCode2 ";
            StrSql = StrSql + " Order by UpitemCode, ItemCode ";
            
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, T_base_db_name, ds) == false) return;
            if (Temp_Connect.DataSet_ReCount == 0) return;


            for (int RowCnt = 0; RowCnt < Temp_Connect.DataSet_ReCount; RowCnt++)
            {
                ItemName = ds.Tables[T_base_db_name].Rows[RowCnt]["ItemName"].ToString();
                ItemCode = ds.Tables[T_base_db_name].Rows[RowCnt]["ItemCode"].ToString();
                UpitemCode = ds.Tables[T_base_db_name].Rows[RowCnt]["UpitemCode"].ToString();

                if (dic_Tree_Sort_1 != null)
                {                    
                    if (dic_Tree_Sort_1.ContainsKey (UpitemCode))                    
                    {
                        TreeNode tn2 = dic_Tree_Sort_1[UpitemCode];

                        if (tn2 != null)
                        {
                            TreeNode node2 = new TreeNode(ItemName + " - " + ItemCode);
                            tn2.Nodes.Add(node2);
                            tn2.Expand();
                            dic_Tree_Sort_2[UpitemCode + ItemCode] = node2;                                              
                        }
                    }
                    
                }
                
            }
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<중분류 관련된 내역을 트리뷰에 넣는다            
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
            if (e.KeyValue == 115)
                T_bt = butt_Delete;   // 삭제  F4
            if (e.KeyValue == 119)
                T_bt = butt_Excel;    //엑셀  F8    
            if (e.KeyValue == 112)
                T_bt = butt_Clear;    //엑셀  F5    

            if (T_bt.Visible == true)
            {
                EventArgs ee1 = null;
                if (e.KeyValue == 123 || e.KeyValue == 113 || e.KeyValue == 115 || e.KeyValue == 119 || e.KeyValue == 112)
                    Base_Button_Click(T_bt, ee1);
            }
        }



        private void Base_Grid_Set(string Ncode = "")
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;

            Tsql = "SELECT Good_Code , B.name ";
            Tsql = Tsql + " , Isnull (( Select Top 1 price2 From tbl_Goods_Change (nolock) Where tbl_Goods_Change.Ncode = B.Ncode  And Na_code = '' Order by ApplyDate DESC   ),B.Price2) Last_price2 ";
            Tsql = Tsql + " , Isnull (( Select Top 1 price4 From tbl_Goods_Change (nolock) Where tbl_Goods_Change.Ncode = B.Ncode  And Na_code = '' Order by ApplyDate DESC   ),B.Price4) Last_price4 ";
            Tsql = Tsql + " , Isnull (( Select Top 1 BV1 From tbl_Goods_Change (nolock) Where tbl_Goods_Change.Ncode = B.Ncode  And Na_code = '' Order by ApplyDate DESC   ),B.BV1) Last_price5 ";

            Tsql = Tsql + ", Sub_Good_Code , C.Name AS CName , Sub_Good_Cnt";
            Tsql = Tsql + " From tbl_Goods_Set (nolock) ";
            Tsql = Tsql + " LEFT Join tbl_Goods AS B (nolock) ON B.Ncode = tbl_Goods_Set.Good_Code ";
            Tsql = Tsql + " LEFT Join tbl_Goods AS C (nolock) ON C.Ncode = tbl_Goods_Set.Sub_Good_Code ";            
            Tsql = Tsql + " Where B.SET_TF = 1 " ; //셋트 상품만 불러온다.
            Tsql = Tsql + " AND B.Na_Code = '" + cls_User.gid_CountryCode + "' "; //셋트 상품만 불러온다.


            if (Ncode != "")
            {
                Tsql = Tsql + " And ( B.ncode Like '%" + Ncode.Trim() + "%'";
                Tsql = Tsql + " OR  B.name Like '%" + Ncode.Trim() + "%') ";
            }
            
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }

            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
        }



        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 8;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            cgb.grid_Merge = true;
            cgb.grid_Merge_Col_Start_index = 0;
            cgb.grid_Merge_Col_End_index = 1;

            string[] g_HeaderText = { "Set상품코드" , "Set상품명" ,"회원가" , "PV"   , "CV"
                    , "포함상품코드" , "포함상품명"   , "수량" 
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();                        
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;            
            cgb.grid_cell_format  = gr_dic_cell_format;


            int[] g_Width = { 100, 150, 70, 70, 70
                             ,100, 150, 70 
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleRight                        
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight   //5

                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft                                
                               ,DataGridViewContentAlignment.MiddleRight                               
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
                                 };
            gr_dic_text[fi_cnt + 1] = row0;
        }
        



        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, txtNcode);
            

            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {
                Data_Set_Form_TF = 1;
                DataGridView T_Gd = (DataGridView)sender;
                string t_ncode = T_Gd.CurrentRow.Cells[0].Value.ToString();
                Base_Goods_Grid_Set();
                Form_Refresh_Data(t_ncode);
                Data_Set_Form_TF = 0;
            }
        }



          

        private void txtData_Enter(object sender, EventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            if (sender is TextBox )            T_R.Text_Focus_All_Sel((TextBox)sender);
            if (sender is MaskedTextBox)        T_R.Text_Focus_All_Sel((MaskedTextBox)sender);
            TextBox tb = (TextBox)sender;
            if (tb.ReadOnly == false)
                tb.BackColor = cls_app_static_var.txt_Focus_Color;  //Color.FromArgb(239, 227, 240); 

            if (this.Controls.ContainsKey("Popup_gr"))
            {
                DataGridView T_Gd = (DataGridView)this.Controls["Popup_gr"];
                T_Gd.Visible = false;
                T_Gd.Dispose();
            }
        }

        private void txtData_Base_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.ReadOnly == false)
                tb.BackColor = Color.White;
        }

       
        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);            
            T_R.Key_Enter_13_Ncode += new Key_13_Ncode_Event_Handler(T_R_Key_Enter_13_Ncode);

            TextBox tb  = (TextBox)sender;

            if ((tb.Tag == null) || (tb.Tag.ToString () == ""))
            {
                //숫자만 입력 가능하다.
                if (T_R.Text_KeyChar_Check(e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if   
            }
            else if (tb.Tag.ToString () == "1") //숫자관련된 사항만 받아들이도록 셋팅을 함.
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e,1) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }      
            else if (tb.Tag.ToString() == "ncode") //코드관련해서 코드를치면 관련 내역이 나오도록 하기 위함.
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e,tb) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }
                                    
        }

    

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            int Sw_Tab = 0;
            if (Data_Set_Form_TF == 1) return;

            TextBox tb = (TextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
            }

            if (tb.Name == "txt_Search")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    Base_Grid_Set();
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_ItemCode")
            {
                Data_Set_Form_TF =1 ;
                if (tb.Text.Trim() == "")
                    txt_ItemName.Text = "";
                Data_Set_Form_TF = 0;
            }
        }

        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {
            if (tb.Name == "txt_Search")
            {
                if (tb.Text.Trim() != "")
                {
                    Data_Set_Form_TF = 1;
                    Base_Grid_Set(tb.Text);
                    Data_Set_Form_TF = 0;
                }
            }

            if (tb.Name == "txt_ItemCode")
            {
                Data_Set_Form_TF = 1;

                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txt_ItemName, "");
                else
                    Ncod_Text_Set_Data(tb, txt_ItemName);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtNcode")
            {
                Data_Set_Form_TF = 1;
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
                if (tb.Name == "txt_ItemCode")
                {
                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV","개별CV", "Name", "Ncode", "price2", "price4", "price5", strSql);
                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                }

            }
            else
            {          


                if (tb.Name == "txt_ItemCode")
                {
                    string Tsql;
                    Tsql = "Select Name , NCode  ,price2 , price4 , PRICE5 ";
                    Tsql = Tsql + " From [ufn_Good_Search_Web] ('" + cls_User.gid_date_time + "','') ";
                    Tsql = Tsql + " Where NCode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";

                    cgb_Pop.db_grid_Popup_Base(4, "상품명", "상품코드", "개별단가", "개별PV", "개별CV", "Name", "Ncode", "price2", "price4", "price5", Tsql);

                    cgb_Pop.Next_Focus_Control = txt_ItemCount;
                }


            }
        }





        private void Ncod_Text_Set_Data(TextBox tb, TextBox tb1_Code)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";
               
            if (tb.Name == "txt_ItemCode")
            {
                Tsql = "Select Name , NCode ,price2 ,price4, PRICE5    ";
                Tsql = Tsql + " From [ufn_Good_Search_Web] ('" + cls_User.gid_date_time + "') ";
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



        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }

        private void from_Date_Clear_()
        {
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            Base_Grid_Set();
            Base_Goods_Grid_Set();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            Data_Set_Form_TF = 1;  
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, txtNcode);
            Data_Set_Form_TF = 0;  

            txtNcode.BackColor = SystemColors.Window;
            txtNcode.ReadOnly = false;

            if (trv_Item.Visible == true)
            {
                trv_Item_Set_Sort_Code();
                //grB_G_Tree.Enabled = true;
                trv_Item.Enabled = true;
            }
        }


        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            
            if (bt.Name == "butt_Clear")
            {
                from_Date_Clear_();
            }
            

            //저장 버튼 클릭시에
            else if (bt.Name == "butt_Save")
            {
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data(ref Save_Error_Check);  //저장이 일어나는 함수

                if (Save_Error_Check > 0)
                {
                    from_Date_Clear_();       
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

            //삭제버튼 클릭시에
            else if (bt.Name == "butt_Delete")
            {
                int Del_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Delete_Base_Data(ref Del_Error_Check); //삭제가 일어남.

                if (Del_Error_Check > 0)
                {
                    from_Date_Clear_();
                }
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

            
            //엑셀 전환 버튼 클릭시에
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
            Excel_Export_File_Name = this.Text; // "Goods_Set";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;
        }


        private Boolean Check_TextBox_Error(int i)
        {
            if (i != 2)  //삭제일 경우에만 체크를 한다.
            {
                if (txtKey.Text.Trim() == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Re_Select"));
                    dGridView_Base.Focus();
                    return false;
                }
            }

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            DataSet ds = new DataSet();

            Tsql = "Select ItemCode from tbl_SalesItemDetail ";
            Tsql = Tsql + " Where ItemCode ='" + txtNcode.Text.Trim() + "'";
            
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_SalesItemDetail", ds) == false) return false;
            if (Temp_Connect.DataSet_ReCount != 0)//이미 매출 내역에 등록 된 상품이다. 그럼안됨.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Useing_Data")
                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Sell")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtNcode.Select();
                return false;
            }


            Tsql = "Select ItemCode from tbl_StockInput ";
            Tsql = Tsql + " Where ItemCode ='" + txtNcode.Text.Trim() + "'";
            
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockInput", ds) == false) return false;
            if (Temp_Connect.DataSet_ReCount != 0)//이미 재고 입고 내역에 등록 된 상품이다. 그럼안됨.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Useing_Data")
                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_InPut")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtNcode.Select();
                return false;
            }

            Tsql = "Select ItemCode from tbl_StockOutput ";
            Tsql = Tsql + " Where ItemCode ='" + txtNcode.Text.Trim() + "'";

            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            if (Temp_Connect.DataSet_ReCount != 0)//이미 재고 출고 내역에 등록 된 상품이다. 그럼안됨.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Useing_Data")
                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_OutPut")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtNcode.Select();
                return false;
            }

            Tsql = "Select M_itemCode from tbl_Stock_Move_Sub ";
            Tsql = Tsql + " Where M_itemCode ='" + txtNcode.Text.Trim() + "'";

            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Stock_Move_Sub", ds) == false) return false;
            if (Temp_Connect.DataSet_ReCount != 0)//이미 재고 출고 내역에 등록 된 상품이다. 그럼안됨.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Useing_Data")
                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Move")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtNcode.Select();
                return false;
            }
            
            

            return true;
        }


        private void Delete_Base_Data(ref int Del_Error_Check)
        {
            Del_Error_Check = 0;
            if (Check_TextBox_Error(1) == false) return;

            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();                       
            Temp_Connect.Connect_DB();
            System.Data.SqlClient.SqlConnection Conn = Temp_Connect.Conn_Conn();
            System.Data.SqlClient.SqlTransaction tran = Conn.BeginTransaction();

            string Tsql;           

            try
            {                                
                Tsql = "Insert into  tbl_Goods_Change_Mod ";
                Tsql = Tsql + " Select * , 'D' ";
                Tsql = Tsql + ",'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21) From tbl_Goods_Change ";
                Tsql = Tsql + " Where Ncode = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran );

                Tsql = "Insert into  tbl_Goods_Mod ";
                Tsql = Tsql + " Select * , 'D' ";
                Tsql = Tsql + ",'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21) From tbl_Goods ";
                Tsql = Tsql + " Where Ncode = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);


                Tsql = "Insert into  tbl_Goods_Set_Mod ";
                Tsql = Tsql + " Select * , 'D' ";
                Tsql = Tsql + ",'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21) From tbl_Goods_Set ";
                Tsql = Tsql + " Where Good_Code = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);



                Tsql = "Delete From tbl_Goods_Change ";
                Tsql = Tsql + " Where Ncode = '" + txtKey.Text.Trim() + "'";
                
                Temp_Connect.Delete_Data(Tsql, base_db_name, Conn, tran, this.Name.ToString(), this.Text) ;

                Tsql = "Delete From tbl_Goods_Set ";
                Tsql = Tsql + " Where Good_Code = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Delete_Data(Tsql, base_db_name, Conn, tran, this.Name.ToString(), this.Text);


                Tsql = "Delete From tbl_Goods ";
                Tsql = Tsql + " Where Ncode = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Delete_Data(Tsql, base_db_name, Conn, tran, this.Name.ToString(), this.Text);

                tran.Commit();                

                Del_Error_Check =1;
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






        private Boolean Check_TextBox_Error()
        {
            cls_Check_Text T_R = new cls_Check_Text();

            string me = T_R.Text_Null_Check(txtNcode); //코드
            if (me != "")
            {
                MessageBox.Show(me);        return false;
            }

            me = T_R.Text_Null_Check(txtName);    //제품명
            if (me != "")
            {
                MessageBox.Show(me);        return false;
            }


            //if ((grB_G_Tree.Visible == true) && (txtUp.Text == ""))
            if ((trv_Item.Visible == true) && (txtUp.Text == ""))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Code")
                    + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Goods_Sort")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                trv_Item.Focus(); return false;
            }


            //if (txtNcode.MaxLength != txtNcode.Text.Trim().Length)
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Code") + "\n" +
            //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    txtNcode.Focus();
            //    return false;
            //}

            int chk_cnt = 0;

            for (int i = 0; i <= dGridView_Base_2.Rows.Count - 1; i++)
            {
                if (dGridView_Base_2.Rows[i].Cells[0].Value.ToString() == "V")
                {
                    if (dGridView_Base_2.Rows[i].Cells[1].Value.ToString() == "0")
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Select_0") + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        dGridView_Base_2.Focus();   return false;
                    }
                    chk_cnt++;
                }
            }

            if (chk_cnt == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Re_Select") + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                dGridView_Base_2.Focus();   return false;
            }

            return true;
        }


        private bool  Check_TextBox_Error(string SaveCheck_2, ref int Chang_Price_TF)
        {
            SaveCheck_2 = "";   
            Chang_Price_TF= 0 ;  //상품의 금액 관련 사항들이 변경을 했는지를 체크한다 변경하면 1
            string Tsql;       
                      
            if (txtKey.Text.Trim() == "")  //처음 인설트 할때는 동일한 이름과 동일한 코드로 이미 저장된 내역이 잇는지를 체크한다.
            {
                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                Tsql = "Select Ncode, Name ";
                Tsql = Tsql + " From tbl_Goods  (nolock)  ";
                Tsql = Tsql + " Where upper(Ncode) = '" + ((txtNcode.Text).Trim()).ToUpper() + "'";
                Tsql = Tsql + " Order by Ncode ASC ";

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                if (Temp_Connect.DataSet_ReCount != 0)//동일한 코드가 있다 그럼.이거 저장하면 안되요
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Same_Code") + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                    txtNcode.Select();
                    return false;
                }
                
                Tsql = "Select Ncode, Name ";
                Tsql = Tsql + " From tbl_Goods  (nolock)  ";
                Tsql = Tsql + " Where Name = '" + (txtName.Text).Trim() + "'";
                Tsql = Tsql + " Order by Ncode ASC ";

                ds.Clear();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                if (Temp_Connect.DataSet_ReCount != 0)//동일한 이름이 있다 그럼.이거 저장하면 안되요
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Same_Name") + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                    txtName.Select();
                    return false;
                }

                //++++++++++++++++++++++++++++++++
            }
            else
            {


                //변경 저장일 경우에는 동일한 코드는 다른데 동일한 이름으로 저장된 내역이 있는지 체크한다.
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                Tsql = "Select Ncode, Name ";
                Tsql = Tsql + " From tbl_Goods  (nolock)  ";
                Tsql = Tsql + " Where upper(Ncode) <> '" + ((txtKey.Text).Trim()).ToUpper() + "'";
                Tsql = Tsql + " And  Name = '" + (txtName.Text).Trim() + "'";
                Tsql = Tsql + " Order by Ncode ASC ";

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                if (Temp_Connect.DataSet_ReCount != 0)//동일한 이름으로 코드가 있다 그럼.이거 저장하면 안되요
                {

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Same_Name") + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    txtName.Select();
                    return false;
                }                
            }                    

            return true;
        }





        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            int Chang_Price_TF = 0;
            if (Check_TextBox_Error() == false) return;
            if (Check_TextBox_Error(2) == false) return;  //상품관련 코드가 한군데에서라도 사용되었는지를 확인한다.          
            if (Check_TextBox_Error("Save_Err_Check_2", ref Chang_Price_TF) == false) return;
                      
            
            if (txtKey.Text.Trim() == "")
            {
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                Temp_Connect.Connect_DB();
                System.Data.SqlClient.SqlConnection Conn = Temp_Connect.Conn_Conn();
                System.Data.SqlClient.SqlTransaction tran = Conn.BeginTransaction();

                string Tsql;

                try
                {
                    Tsql = "insert into tbl_Goods ( ";
                    Tsql = Tsql + " ncode , Na_Code, name , GoodUse ,Set_TF , Up_itemCode ,Item_RegTime ,Recordid,Recordtime  ";
                    Tsql = Tsql + ") Values ( ";
                                      
                    Tsql = Tsql + " '" + txtNcode.Text.Trim() + "'";
                    Tsql = Tsql + ",'" + cls_User.gid_CountryCode + "'";    // tbl_Goods에 국가코드 추가. - 231016 syhuh
                    Tsql = Tsql + ",'" + txtName.Text.Trim() + "'";
                    Tsql = Tsql + ",1";
                    Tsql = Tsql + ",1";
                    Tsql = Tsql + ",'" + txtUp.Text.Trim() + "'";
                    Tsql = Tsql + ", Replace(LEFT(Convert(Varchar(25),GetDate(),21) ,10 ),'-','')  " ;
                    Tsql = Tsql + ",'" + cls_User.gid + "'";
                    Tsql = Tsql + ", Convert(Varchar(25),GetDate(),21) ";
                    Tsql = Tsql + " ) ";

                    Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);

                    int SubCnt = 0; string SubCode = "";
                    for (int i = 0; i < dGridView_Base_2.Rows.Count ; i++)
                    {
                        if (dGridView_Base_2.Rows[i].Cells[0].Value.ToString() == "V")
                        {
                            SubCnt = int.Parse(dGridView_Base_2.Rows[i].Cells[1].Value.ToString());
                            SubCode = dGridView_Base_2.Rows[i].Cells[2].Value.ToString();

                            Tsql = "insert into tbl_Goods_Set ( ";
                            Tsql = Tsql + " Good_Code, Sub_Good_Code, Sub_Good_Name, Sub_Good_Cnt,Recordid,Recordtime ";
                            Tsql = Tsql + ") Values ( ";
                                                      
                            Tsql = Tsql + " '" + txtNcode.Text.Trim() + "'";
                            Tsql = Tsql + ",'" + SubCode + "'" ;
                            Tsql = Tsql + ",''" ;
                            Tsql = Tsql + "," + SubCnt;
                            Tsql = Tsql + ",'" + cls_User.gid + "'" ;
                            Tsql = Tsql + ", Convert(Varchar(25),GetDate(),21) " ;
                            Tsql = Tsql + ")" ;

                            Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);
                        }
                    }
                          
                    tran.Commit();

                    Save_Error_Check = 1;
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                }
                catch (Exception ex )
                {
                    tran.Rollback();

                    MessageBox.Show(ex.Message);
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));

                }

                finally
                {
                    tran.Dispose();
                    Temp_Connect.Close_DB();
                }            

            }
            else //동일한 코드가 있구나 그럼 업데이트
            {
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

                if (Save_Base_Data_Up( ) == false) return;

                Save_Error_Check = 1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }

        }

        private Boolean Save_Base_Data_Up( )
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            string Tsql;

            try
            {
                Tsql = "Insert into tbl_Goods_Mod ";
                Tsql = Tsql + " Select * , 'U' ";
                Tsql = Tsql + ",'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21) From tbl_Goods ";
                Tsql = Tsql + " Where Ncode = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);


                Tsql = "Update tbl_Goods Set ";
                Tsql = Tsql + " name = '" + txtName.Text.Trim() + "'";
                Tsql = Tsql + " WHERE Ncode = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Update_Data(Tsql, Conn, tran, this.Name.ToString(), this.Text);


                Tsql = "Insert into tbl_Goods_Set_Mod ";
                Tsql = Tsql + " Select * , 'U' ";
                Tsql = Tsql + ",'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21) From tbl_Goods_Set ";
                Tsql = Tsql + " Where Good_Code = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);


                Tsql = "Delete From tbl_Goods_Set ";
                Tsql = Tsql + " Where Good_Code = '" + txtKey.Text.Trim() + "'";

                Temp_Connect.Delete_Data(Tsql, base_db_name, Conn, tran, this.Name.ToString(), this.Text);


                int SubCnt = 0; string SubCode = "";
                for (int i = 0; i < dGridView_Base_2.Rows.Count; i++)
                {
                    if (dGridView_Base_2.Rows[i].Cells[0].Value.ToString() == "V")
                    {
                        SubCnt = int.Parse(dGridView_Base_2.Rows[i].Cells[1].Value.ToString());
                        SubCode = dGridView_Base_2.Rows[i].Cells[2].Value.ToString();

                        Tsql = "insert into tbl_Goods_Set ( ";
                        Tsql = Tsql + " Good_Code, Sub_Good_Code, Sub_Good_Name, Sub_Good_Cnt,Recordid,Recordtime ";
                        Tsql = Tsql + ") Values ( ";                                               
                        Tsql = Tsql + " '" + txtNcode.Text.Trim() + "'";

                        Tsql = Tsql + ",'" + SubCode + "'";
                        Tsql = Tsql + ",''";
                        Tsql = Tsql + "," + SubCnt;
                        Tsql = Tsql + ",'" + cls_User.gid + "'";
                        Tsql = Tsql + ", Convert(Varchar(25),GetDate(),21) ";
                        Tsql = Tsql + ")";

                        Temp_Connect.Insert_Data(Tsql, base_db_name, Conn, tran);
                    }
                }

                
                tran.Commit();
                return true;

            }
            catch (Exception)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));
                return false;
            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();

            }
        }


        private void Form_Refresh_Data(string ncode)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;

            Tsql = "Select tbl_Goods.Ncode  ";
            Tsql = Tsql + " , name ";          
            Tsql = Tsql + " , Up_itemCode ";          
            Tsql = Tsql + " From tbl_Goods (nolock) ";            
            Tsql = Tsql + " Where ncode = '" + ncode + "'";
            Tsql = Tsql + " Order by Ncode ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++


            txtKey.Text = ds.Tables[base_db_name].Rows[0]["Ncode"].ToString();
            txtNcode.Text = ds.Tables[base_db_name].Rows[0]["Ncode"].ToString();
            txtUp.Text = ds.Tables[base_db_name].Rows[0]["Up_itemCode"].ToString();  

            ////대분류 중분류 선택일 경우에는 상품 코드는 대분류+중분류 + 입력 상품 코드로 해서. 저장된다.
            //if (txtUp.Visible == true)
            //{
            //    //txtUp.MaxLength
            //    string T_Code = ds.Tables[base_db_name].Rows[0]["Ncode"].ToString();
            //    txtNcode.Text = T_Code.Substring(txtUp.MaxLength, txtNcode.MaxLength )  ;
            //}
            //else //대분류 중분류 선택이 아닌 경우에는 상품 코드를 다 보여준다
            //    txtNcode.Text = ds.Tables[base_db_name].Rows[0]["Ncode"].ToString();

            txtName.Text = ds.Tables[base_db_name].Rows[0]["name"].ToString();
            

            //상품 중분류 대분류 체크 트리상에서 모든 체크내역을 다 푼다.
            foreach (string t_for_key in dic_Tree_Sort_1.Keys)
            {
                TreeNode tn2 = dic_Tree_Sort_1[t_for_key];
                tn2.Checked = false;
            }

            foreach (string t_for_key in dic_Tree_Sort_2.Keys)
            {
                TreeNode tn2 = dic_Tree_Sort_2[t_for_key];
                tn2.Checked = false;
            }

            //상품 중분류와 대분류 코드에서 선택된 내역을 트리에서 찾아서 체크한다.
            if (cls_app_static_var.Item_Sort_3_Code_Length > 0)
            {
                if (dic_Tree_Sort_2.ContainsKey(txtUp.Text))
                {
                    TreeNode tn2 = dic_Tree_Sort_2[txtUp.Text];
                    tn2.Checked = true;
                }
            }

            else if (cls_app_static_var.Item_Sort_2_Code_Length > 0)
            {
                if (dic_Tree_Sort_1.ContainsKey(txtUp.Text))
                {
                    TreeNode tn2 = dic_Tree_Sort_1[txtUp.Text];
                    tn2.Checked = true;
                }
            }


            //서브아이템 내역을 가져온다.
            Tsql = "SELECT Sub_Good_Code  ";
            Tsql = Tsql + " , Sub_Good_Cnt ";            
            Tsql = Tsql + " From tbl_Goods_Set (nolock) ";
            Tsql = Tsql + " Where Good_Code = '" + ncode + "'";
            Tsql = Tsql + " Order by Sub_Good_Code ASC ";

            ds.Clear();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
            ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt != 0)
            {
                int SubCnt = 0; string SubCode = "";
                for (int fi_cnt = 0; fi_cnt < ReCnt ; fi_cnt++)
                {
                    SubCode =ds.Tables[base_db_name].Rows[fi_cnt]["Sub_Good_Code"].ToString ();
                    SubCnt= int.Parse (ds.Tables[base_db_name].Rows[fi_cnt]["Sub_Good_Cnt"].ToString ()) ;

                    for (int i = 0; i < dGridView_Base_2.Rows.Count; i++)
                    {
                        if (dGridView_Base_2.Rows[i].Cells[2].Value.ToString() == SubCode)
                        {
                            dGridView_Base_2.Rows[i].Cells[0].Value = "V";
                            dGridView_Base_2.Rows[i].Cells[1].Value = SubCnt.ToString () ;
                        }
                    }
                }
            }
            //++++++++++++++++++++++++++++++++



            //더블클릭이나 상품 정보를 불러온 상태에선느 상품 코드의 변경이 안일어 나게 하기 위해서 상품 코드 텍스트를 락시킨다
            //추후 위의 새로 입력 버튼으로 풀수 있음.
            txtNcode.BackColor = Color.AliceBlue;
            txtNcode.ReadOnly = true;

            //grB_G_Tree.Enabled  = false;
            trv_Item.Enabled  = false; 

            
            
            
            
            
            
            
            
            txtName.Focus();

        }


        private void Base_Goods_Grid_Set()
        {
            dGridView_Base_2_Header_Reset();
            cgb_2.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;

            Tsql = "Select ";
            Tsql = Tsql + "  ''   ";          
            Tsql = Tsql + " , 0  ItemCount ";
            Tsql = Tsql + " , ncode ";
            Tsql = Tsql + " , name ";
            Tsql = Tsql + " , Price2 "; //5

            Tsql = Tsql + " , Price4 ";
            Tsql = Tsql + " , PRICE5 ";
            Tsql = Tsql + " , 0  ";
            Tsql = Tsql + " , 0 ";
            Tsql = Tsql + " , 0 "; //10         

            //Tsql = Tsql + " From tbl_Goods (nolock) ";
            Tsql = Tsql + " From ufn_Good_Search_Web ('" + cls_User.gid_date_time + "','" + cls_User.gid_CountryCode + "') ";
            Tsql = Tsql + " Where SET_TF=0 " ; //===셋트 상품이 아닌 순수 상품만 불러온다.
            Tsql = Tsql + " Order by ncode ASC ";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_2_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }

            cgb_2.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_2.db_grid_Obj_Data_Put();


            //드로박스를 넣을 2번째열을 없애 버린다.                                    
            ////dGridView_Base_2.Columns.Remove(dGridView_Base_2.Columns[1]);

            ////DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
            ////newColumn.HeaderText = "수량";
            
            ////newColumn.Name = "ItemCount";
            ////newColumn.Width = 35;            
            ////string[] typeList = { "0","1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            ////newColumn.Items.AddRange(typeList);
            ////newColumn.DefaultCellStyle.NullValue = "0";
                        
            ////dGridView_Base_2.Columns.Insert(1, newColumn);            
        }




        private void dGridView_Base_2_Header_Reset()
        {
            dGridView_Base_2.RowHeadersVisible = false;
            cgb_2.grid_col_Count = 10;            
            cgb_2.basegrid = dGridView_Base_2;
            cgb_2.grid_select_mod = DataGridViewSelectionMode.CellSelect ;
            //cgb_2.grid_Frozen_End_Count = 2;
            cgb_2.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


            string[] g_HeaderText = { "선택", "수량", "상품_코드" , "상품명" ,"회원가" , 
                                      "PV"   , "CV" , ""   ,"" , ""                                     
                                    };
            cgb_2.grid_col_header_text = g_HeaderText;

            

            int[] g_Width = { 35, 100 , 60 , 110, 70,
                              70, 70, 0, 0, 0                             
                            };
            cgb_2.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , false,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true   
                                   };
            cgb_2.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter                        
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight  //5
                               
                               ,DataGridViewContentAlignment.MiddleRight                                
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleRight //10

                              };
            cgb_2.grid_col_alignment = g_Alignment;


            DataGridViewColumnSortMode[] g_SortM =
                              {DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.NotSortable
                               //,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic  
                               ,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic  //5
                               
                               ,DataGridViewColumnSortMode.Automatic                              
                               ,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic 
                               ,DataGridViewColumnSortMode.Automatic //10                                                           
                              };
            cgb_2.grid_col_SortMode = g_SortM;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb_2.grid_cell_format = gr_dic_cell_format;
        }


        private void Set_gr_2_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
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
                                 };

            gr_dic_text[fi_cnt + 1] = row0;

        }

                
        private void trv_Item_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                //중분류가 존재하는데 대분류를 선택 한 경우에는 중분류를 선택 하도록 체크를 지워 버린다.
                if (cls_app_static_var.Item_Sort_3_Code_Length > 0)
                {
                    if (e.Node.Parent == null)
                    {
                        e.Node.Checked = false;
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Goods_Sort_2") + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }

                    int Check_Cnt = 0;
                    //중분류 선택시 중대분류 다른 곳에 체크를 했는지를 알기 위해서 했으면 지금 체크를 지우운다.
                    foreach (string t_for_key in dic_Tree_Sort_2.Keys)
                    {
                        TreeNode tn2 = dic_Tree_Sort_2[t_for_key];
                        if (tn2.Checked == true) Check_Cnt++;
                    }

                    if (Check_Cnt >= 2)
                    {
                        e.Node.Checked = false;
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Check") + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        return;
                    }

                    Check_Cnt = 0;
                    foreach (string t_for_key in dic_Tree_Sort_2.Keys)
                    {
                        TreeNode tn2 = dic_Tree_Sort_2[t_for_key];
                        if (tn2.Checked == true)
                        {
                            txtUp.Text = t_for_key;
                            Check_Cnt++;
                        }
                    }

                    if (Check_Cnt == 0) txtUp.Text = "";
                    
                  
                }
                else
                {
                    //대분류 선택시 대분류 다른 곳에 체크를 했는질ㄹ 알기 위해서 했으면 지금 체크를 지우운다.
                    int Check_Cnt = 0;
                   
                    foreach (string t_for_key in dic_Tree_Sort_1.Keys)
                    {
                        TreeNode tn2 = dic_Tree_Sort_1[t_for_key];
                        if (tn2.Checked == true) Check_Cnt++;
                    }

                    if (Check_Cnt >= 2)
                    {
                        e.Node.Checked = false;
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Sort_Check") + "\n" +
                        cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        return;
                    }

                    Check_Cnt = 0;
                    foreach (string t_for_key in dic_Tree_Sort_1.Keys)
                    {
                        TreeNode tn2 = dic_Tree_Sort_1[t_for_key];
                        if (tn2.Checked == true)
                        {
                            txtUp.Text = t_for_key;
                            Check_Cnt++;
                        }
                    }

                    if (Check_Cnt == 0) txtUp.Text = "";
                    
                }
                //if (e.Node.Nodes.Count > 0)                
            }

            txtNcode.Focus();

        }

        private void dGridView_Base_2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
              if (e.RowIndex == -1)
            {
                return;
            }

            //ColumnHeader.

                if ((sender as DataGridView).CurrentCell.ColumnIndex == 0)
                {
                    DataGridView T_DGv = (DataGridView)sender;
                    if ((T_DGv.CurrentCell.Value == null)
                    || (T_DGv.CurrentCell.Value.ToString() == ""))
                    {
                        T_DGv.CurrentCell.Value = "V";
                        T_DGv.Rows[T_DGv.CurrentRow.Index].Cells[1].Value = "1";
                    }
                    else
                    {
                        T_DGv.CurrentCell.Value = "";
                        T_DGv.Rows[T_DGv.CurrentRow.Index].Cells[1].Value = "0";
                    }
                }
        }




        private void dGridView_Base_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            
            dGridView_Base_2.EditingControl.KeyPress += new KeyPressEventHandler(textBoxPart_TextChanged);
        }

        private void textBoxPart_TextChanged(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void butt_App_Click(object sender, EventArgs e)
        {
            if (Item_Rece_Error_Check__01() == false) return;

            for (int i = 0; i <= dGridView_Base_2.Rows.Count - 1; i++)
            {
                if (dGridView_Base_2.Rows[i].Cells[2].Value.ToString() == txt_ItemCode.Text.Trim())
                {
                    dGridView_Base_2.Rows[i].Cells[0].Value = "V";                
                    dGridView_Base_2.Rows[i].Cells[1].Value = txt_ItemCount.Text.Trim();
                }                      
            }
            
            txt_ItemCode.Text = ""; txt_ItemCount.Text = "";
            txt_ItemName.Text = "";
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

            return true; 
            
        }

        private void dGridView_Base_2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }
    }
}
