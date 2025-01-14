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
    public partial class frmBase_Out_Code : Form
    {
      
        cls_Grid_Base cgb = new cls_Grid_Base();
        private const string base_db_name = "tbl_Base_Out_Code";

        public frmBase_Out_Code()
        {
            InitializeComponent();
            //this.pnl = panel1;
        }

          
  

        private void frmBase_Bank_Load(object sender, EventArgs e)
        {
           
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
                      
            Base_Grid_Set();
        }



        private void frmBase_Resize(object sender, EventArgs e)
        {
            ////butt_Exit.Left = this.Width - butt_Exit.Width - 20;

            ////butt_Clear.Left = 3;            
            ////butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            ////butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            ////butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;

            //int base_w = this.Width / 5;
            //butt_Clear.Width = base_w;
            //butt_Save.Width = base_w;
            //butt_Excel.Width = base_w;
            //butt_Delete.Width = base_w;
            //butt_Exit.Width = base_w;

            butt_Clear.Left = 0;         
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2 ;
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



        private void Base_Grid_Set()
        {
                           
            
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                        
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Ncode, T_Name, Kind_TF "; //Isnull(nationNameEng,'') nationNameEng , Na_code  ";
            Tsql = Tsql + " From tbl_Base_Out_Code  (nolock)  ";
            Tsql = Tsql + " Where Kind_TF = 'O' ";
            //Tsql = Tsql + " LEFT JOIN  tbl_Nation  (nolock) ON tbl_Nation.nationCode = tbl_Base_Out_Code.Na_Code  ";           
            Tsql = Tsql + " Order by Ncode ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;
            
            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++
            

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, string[]> gr_dic_text = new Dictionary<int, string[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt-1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }

            cgb.grid_name = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Data_Put();
            //dGridView_Base.RowCount = dGridView_Base.RowCount + 1;
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
      
                                   
            
            //dGridView_Base.columnh = System.Windows.Forms.dGridView_BaseColumnSortMode.NotSortable;                                       
            //dGridView_Base.DataSource = ds.Tables[base_db_name];            
                        
            //for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            //{
            //    dGridView_BaseRowHeaderCell headerCell = dGridView_Base.Rows[i].HeaderCell;

            //    headerCell.Value = (i + 1).ToString();
            //    headerCell.Style.Alignment = dGridView_BaseContentAlignment.MiddleCenter;
            //    headerCell.Style.Font = new Font(dGridView_Base.DefaultCellStyle.Font, FontStyle.Bold);
            //}
            
        }

        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 5;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = { "출고_코드" , "출고사유"  , "_Kind_TF"  ,""   , ""                                        
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 100, 150, 0 , 0, 0                               
                            };

            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                   };
            cgb.grid_col_Lock = g_ReadOnly;
     
            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter , 
                               DataGridViewContentAlignment.MiddleLeft , 
                               DataGridViewContentAlignment.MiddleCenter,  
                               DataGridViewContentAlignment.MiddleCenter ,
                               DataGridViewContentAlignment.MiddleRight  //5
                               //DataGridViewContentAlignment.MiddleCenter ,                               
                               //DataGridViewContentAlignment.MiddleCenter,
                               //DataGridViewContentAlignment.MiddleCenter,
                               //DataGridViewContentAlignment.MiddleRight 
                              };
            cgb.grid_col_alignment = g_Alignment;            
        }

       


                

        private void Set_gr_dic(ref DataSet ds, ref Dictionary<int, string[]> gr_dic_text, int fi_cnt)
        {
            string[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0].ToString() , 
                                ds.Tables[base_db_name].Rows[fi_cnt][1].ToString() , 
                                ds.Tables[base_db_name].Rows[fi_cnt][2].ToString() , 
                               // ds.Tables[base_db_name].Rows[fi_cnt][3].ToString()  
                                //ds.Tables[base_db_name].Rows[fi_cnt][4].ToString() , 
                                //ds.Tables[base_db_name].Rows[fi_cnt][5].ToString() , 
                                //ds.Tables[base_db_name].Rows[fi_cnt][6].ToString() ,
                                //ds.Tables[base_db_name].Rows[fi_cnt][7].ToString() ,
                                //ds.Tables[base_db_name].Rows[fi_cnt][8].ToString() ,
                                 };

            gr_dic_text[fi_cnt+1] = row0;
        }




        private void frmBase_Bank_KeyDown(object sender, KeyEventArgs e)
        {
            //폼일 경우에는 ESC버튼에 폼이 종료 되도록 한다
            if (sender is Form)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }// end if
            }

            //그리드일 경우에는 DEL키로 행을 삭제하는걸 막는다.
            if (sender is DataGridView)
            {
               
                if (e.KeyValue == 13)
                {
                    EventArgs ee = null;
                    dGridView_Base_DoubleClick(sender, ee);
                    e.Handled = true;
                } // end if
            }


            Button T_bt = butt_Exit ;
            if (e.KeyValue == 123)
                T_bt = butt_Exit ;    //닫기  F12
            if (e.KeyValue == 113)
                T_bt = butt_Save;     //저장  F2  116 
            if (e.KeyValue == 115)
                T_bt = butt_Delete;   // 삭제  F4
            if (e.KeyValue == 119)
                T_bt = butt_Excel;    //엑셀  F8    
            if (e.KeyValue == 112)
                T_bt = butt_Clear;    //엑셀  F1   112
            
            EventArgs ee1 = null;
            if (e.KeyValue == 123 || e.KeyValue == 113 || e.KeyValue == 115 || e.KeyValue == 119 || e.KeyValue == 112)
                cmdSave_Click(T_bt, ee1);            
        }




        //private void dGridView_Base_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        //{

        //    //Header인지 확인
        //    if (e.ColumnIndex < 0 & e.RowIndex >= 0)
        //    {
        //        e.Paint(e.ClipBounds, DataGridViewPaintParts.All);

        //        //행 번호를 표시할 범위를 결정
        //        Rectangle indexRect = e.CellBounds;
        //        indexRect.Inflate(-2, -2);
        //        //행번호를 표시
        //        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //                              e.CellStyle.Font, indexRect, e.CellStyle.ForeColor,
        //                              TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        //        e.Handled = true;
        //    }
        //}



        private void txtData_Enter(object sender, EventArgs e)
        {         
            cls_Check_Text T_R = new cls_Check_Text();
            T_R.Text_Focus_All_Sel((TextBox)sender);

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
                tb.BackColor = Color.White ;   
        }

        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);

            if (T_R.Text_KeyChar_Check(e) == false)
            {
                e.Handled = true;
                return;
            } // end if   
        }
        

        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }

  




        private Boolean Check_TextBox_Error()
        {      
            cls_Check_Text T_R = new cls_Check_Text();

            string me = T_R.Text_Null_Check(txtData);
            if (me != "")
            {
                MessageBox.Show(me);         
                return false;
            }

            me = T_R.Text_Null_Check(txtData2);
            if (me != "")
            {
                MessageBox.Show(me);                
                return false;
            }

            if (txtKey.Text.Trim() == "")
            {
                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                string Tsql;
                Tsql = "Select Ncode, T_Name ";
                Tsql = Tsql + " From tbl_Base_Out_Code  (nolock)  ";
                Tsql = Tsql + " Where upper(Ncode) = '" + ((txtData.Text).Trim()).ToUpper() + "'";
                Tsql = Tsql + " And  Kind_TF = 'O' ";
                //Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
                Tsql = Tsql + " Order by Ncode ASC ";

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                if (Temp_Connect.DataSet_ReCount != 0)//동일한 은행 코드가 있다 그럼.이거 저장하면 안되요
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Same_Code") + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                    txtData.Select();
                    return false;
                }


                Tsql = "Select Ncode, T_Name ";
                Tsql = Tsql + " From tbl_Base_Out_Code  (nolock)  ";
                Tsql = Tsql + " Where T_Name = '" + (txtData2.Text).Trim() + "'";
                Tsql = Tsql + " And  Kind_TF = 'O' ";
               // Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
                Tsql = Tsql + " Order by Ncode ASC ";

                ds.Clear();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                if (Temp_Connect.DataSet_ReCount != 0)//동일한 은행 코드가 있다 그럼.이거 저장하면 안되요
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Same_Name") + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                    txtData2.Select();
                    return false;
                }

                //++++++++++++++++++++++++++++++++
            }
            else
            {
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                string Tsql;
                Tsql = "Select Ncode, T_Name ";
                Tsql = Tsql + " From tbl_Base_Out_Code  (nolock)  ";
                Tsql = Tsql + " Where upper(Ncode) <> '" + ((txtData.Text).Trim()).ToUpper() + "'";
                Tsql = Tsql + " And  T_Name = '" + (txtData2.Text).Trim() + "'";
                Tsql = Tsql + " And  Kind_TF = 'O' ";
                //Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
                Tsql = Tsql + " Order by Ncode ASC ";

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
                if (Temp_Connect.DataSet_ReCount != 0)//동일한 은행 코드가 있다 그럼.이거 저장하면 안되요
                {
                    
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Same_Name") + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action") );
                    txtData2.Select();
                    return false;
                }
            }
                        
            return true;
        }


        private void cmdSave_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Clear")
            {
                cls_form_Meth ct = new cls_form_Meth();                                
                ct.from_control_clear(this, txtData);
            }
            else if (bt.Name == "butt_Save")
            {
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex;
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {
                    
                    cls_form_Meth ct = new cls_form_Meth();
                    ct.from_control_clear(this, txtData);
                    Base_Grid_Set();
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }
            else if (bt.Name == "butt_Delete")
            {
                int Del_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex;
                Delete_Base_Data(ref Del_Error_Check);
                if (Del_Error_Check > 0)
                {
                   
                    cls_form_Meth ct = new cls_form_Meth();
                    ct.from_control_clear(this, txtData);
                    Base_Grid_Set();
                }
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

            else if (bt.Name == "butt_Excel")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info);
                e_f.ShowDialog();
            }

        }


        private DataGridView e_f_Send_Export_Excel_Info(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "Bank";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;
        }



        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
             if (Check_TextBox_Error() == false) return;                        

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            Tsql = "Select Ncode, T_Name ";
            Tsql = Tsql + " From tbl_Base_Out_Code   (nolock) ";
            Tsql = Tsql + " Where upper(Ncode) = '" + ((txtData.Text).Trim()).ToUpper() + "'";
            Tsql = Tsql + " And  Kind_TF = 'O' ";
            //Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
            Tsql = Tsql + " Order by Ncode ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)//동일한 은행 코드가없네 그럼 인설트
            {
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

                Tsql = "Insert Into tbl_Base_Out_Code (Ncode , T_Name , Kind_TF ) Values (";
                Tsql = Tsql + "'" + txtData.Text.Trim() + "','" + txtData2.Text.Trim() + "','O') ";

                if (Temp_Connect.Insert_Data( Tsql, base_db_name,this.Name.ToString (), this.Text ) == false) return;

                Save_Error_Check =1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));


                ////입력할 필드 명이랑 값을 Dictionary 에 넣는다.
                //Dictionary<string, string> in_dic_1 = new Dictionary<string, string>();
                //in_dic_1["@v_Ncode"] = txtData.Text.Trim();
                //in_dic_1["@v_T_Name"] = txtData2.Text.Trim();
                
                ////입력할 필드의 형정보를 Dictionary 2에 넣는다.
                //Dictionary<string, SqlDbType> in_dic_2 = new Dictionary<string, SqlDbType>();
                //in_dic_2["@v_Ncode"] = SqlDbType.VarChar;
                //in_dic_2["@v_T_Name"] = SqlDbType.VarChar;                

                ////테이블에 맞게  insert 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                //if (Temp_Connect.Insert_Data(in_dic_1, in_dic_2, Tsql, base_db_name) == false) return;
            }
            else //동일한 은행 코드가 있구나 그럼 업데이트
            {
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

                Tsql = "Update tbl_Base_Out_Code Set ";
                Tsql = Tsql + " T_Name = '" + txtData2.Text.Trim() + "'";                
                Tsql = Tsql + " WHERE Ncode = '" + txtData.Text.Trim() + "'";
                //Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 

                if (Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text) == false) return;

                Save_Error_Check = 1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
                //DataSet 객체의 값들을 변경하는 작업을 한다.                
                //ds.Tables[base_db_name].Rows[0]["T_Name"] = txtData2.Text.Trim();


                ////커맨드 객체로 해서 보낼 내역을 쿼리로 만든다.
                //Tsql = "Update tbl_Base_Out_Code Set ";                
                //Tsql = Tsql + " T_Name = @v_T_Name ";                
                //Tsql = Tsql + " WHERE Ncode = @v_Ncode ";
                
                ////변경될 필드 명이랑 값을 Dictionary 에 넣는다.
                //Dictionary<string, string> up_dic = new Dictionary<string, string>();
                //up_dic["@v_T_Name"] = txtData2.Text.Trim();                
                //up_dic["@v_Ncode"] = txtData.Text.Trim();

                ////변경을 시키는 메소드를 호출한다.
                //Temp_Connect.Update_Data(ds, up_dic, Tsql, base_db_name);             
            }                       
            
        }




        private void Delete_Base_Data(ref int Del_Error_Check)
        {
            Del_Error_Check = 0;
            if (Check_TextBox_Error(1) == false) return;

            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            Tsql = "Delete From tbl_Base_Out_Code ";
            Tsql = Tsql + " Where Ncode = '" + txtData.Text.Trim() + "'";
            Tsql = Tsql + " And  Kind_TF = 'O' ";
            //Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 

            if (Temp_Connect.Delete_Data (Tsql, base_db_name, this.Name.ToString(), this.Text) == false) return;

            Del_Error_Check = 1;
            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del"));
        }



        private Boolean Check_TextBox_Error(int i)
        {
            if (txtKey.Text.Trim() == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Re_Select"));
                dGridView_Base.Select();
                return false;
            }

            
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            Tsql = "Select Out_Index ";
            Tsql = Tsql + " From tbl_StockOutput  (nolock) ";
            Tsql = Tsql + " Where Out_FL_Code_2 = '" + txtData.Text.Trim() + "'";
            //Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
            Tsql = Tsql + " Order by Out_Index ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
            if (Temp_Connect.DataSet_ReCount != 0)//동일한 은행 코드가 있다 그럼.이거 저장하면 안되요
            {
                MessageBox.Show( cls_app_static_var.app_msg_rm.GetString("Msg_Useing_Data") 
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtData.Select();
                return false;
            }


            return true ;
        }






        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            //int rowcnt = (sender as DataGridView).CurrentCell.RowIndex;  
            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {
                txtData.Text = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                txtData2.Text = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                //if (combo_Se.Enabled == true)
                //{
                //    combo_Se.Text = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                //    combo_Se_Code.Text = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();
                //}
                txtKey.Text = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
            }

        }


    }
}
