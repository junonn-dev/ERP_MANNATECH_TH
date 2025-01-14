﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace MLM_Program
{
    public partial class frmBase_Excel : Form
    {
        DataGridView Exp_dgv = new DataGridView();

        cls_Grid_Base cgb = new cls_Grid_Base();
        string Excel_Export_From_Name = "";
        string Excel_Export_File_Name = "";


        public delegate DataGridView Send_Export_Excel_Info_Dele(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name);
        public event Send_Export_Excel_Info_Dele Send_Export_Excel_Info;


        public frmBase_Excel()
        {
            InitializeComponent();
        }


        private void cmdSave_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Excel")
            {
                if (Exp_dgv.RowCount == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Excel_Not_RowCnt")
                    + "\n" +
                    cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    this.Close();
                    return;
                }

                if (cls_User.gid_Excel_Save_TF == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Excel_Not_TF")
                     + "\n" +
                     cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                Expert_Excel();
                this.Cursor = Cursors.Default;
                this.Close();

            }
            else if (bt.Name == "butt_Save")
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data();
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }

        }

        private void Expert_Excel()
        {
            int chk_cnt = 0;

            for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
            {
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                {
                    chk_cnt++;
                }
            }//  end for 그리드 상에서 엑셀 전환을 선택한 V 한 내역을 파악한다.

            if (chk_cnt == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Select") + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Select"));
                return;
            } //end if 체크를 해서 선택한 내역이 없을 경우 메시지 뛰우고나간다.

            //Excel_Export_GridDate(true);

            GridViewExcel(Exp_dgv);

        }


        private void frmBase_Excel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }// end if

            Button T_bt = butt_Exit;
            if (e.KeyValue == 123)
                T_bt = butt_Exit;    //닫기  F12
            //if (e.KeyValue == 113)
            //    T_bt = butt_Save;     //저장  F1
            if (e.KeyValue == 119)
                T_bt = butt_Excel;    //엑셀  F8    

            if (T_bt.Visible == true)
            {
                EventArgs ee1 = null;
                if (e.KeyValue == 123 || e.KeyValue == 119)
                    cmdSave_Click(T_bt, ee1);
            }

        }


        private void frmBase_Excel_Load(object sender, EventArgs e)
        {
           
            Exp_dgv = null;
            Excel_Export_From_Name = "";
            Excel_Export_File_Name = this.Text; // "";

            Exp_dgv = Send_Export_Excel_Info(ref Excel_Export_From_Name, ref Excel_Export_File_Name);

            Base_Grid_Set();
            User_Check_Load();

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Exit);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Check_01);
            cfm.button_flat_change(butt_Check_02);
            cfm.button_flat_change(button_CSV);

        }


        private void Base_Grid_Set()
        {


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


            DataGridView Temp_Grid = Exp_dgv;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, string[]> gr_dic_text = new Dictionary<int, string[]>();

            for (int i = 0; i <= Temp_Grid.Columns.Count - 1; i++)
            {
                //if (Temp_Grid.Columns[i].HeaderText != null && Temp_Grid.Columns[i].HeaderText != "" && Temp_Grid.Columns[i].Width > 0 && Temp_Grid.Columns[i].HeaderText.Substring (0,1) != "_" )
                //{
                Set_gr_dic(ref gr_dic_text, Temp_Grid.Columns[i].HeaderText.ToString(), i);  //데이타를 배열에 넣는다.                    
                //}                
            }

            cgb.grid_name = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Data_Put();


            for (int i = 0; i < Temp_Grid.Columns.Count - 1; i++)
            {
                if (Temp_Grid.Columns[i].HeaderText != null && Temp_Grid.Columns[i].HeaderText != "" && Temp_Grid.Columns[i].Visible == true && Temp_Grid.Columns[i].HeaderText.Substring(0, 1) != "_")
                {

                }
                else
                    dGridView_Base.Rows[i].Visible = false;
            }

        }

        private void dGridView_Base_Header_Reset()
        {
            dGridView_Base.RowHeadersVisible = false;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_col_Count = 2;
            cgb.basegrid = dGridView_Base;
            cgb.Sort_Mod_Auto_TF = 1;

            string[] g_HeaderText = { "선택", "제목" };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 80, 200 };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true, true };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter ,
                               DataGridViewContentAlignment.MiddleLeft
                              };
            cgb.grid_col_alignment = g_Alignment;
        }






        private void Set_gr_dic(ref Dictionary<int, string[]> gr_dic_text, string Header_Text, int fi_cnt)
        {
            string[] row0 = { "V", Header_Text };

            gr_dic_text[fi_cnt + 1] = row0;
        }

        private void dGridView_Base_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;

            //MessageBox.Show(dg.CurrentCell.ToString()); 
            //dg.CurrentCell.ColumnIndex 
            //dg.sel

            if (dg.CurrentCell.ColumnIndex != 0) return;

            if (dg.CurrentRow.Cells[0].Value.ToString() == "V")
            {
                dg.CurrentCell.Value = "";
            }

            else
            {
                dg.CurrentCell.Value = "V";
            }
        }

        private void User_Check_Load()
        {
            try
            {
                DataGridView Temp_Grid = Exp_dgv;

                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                string TSql;
                TSql = "Select CheckList from tbl_Excel_Config ";
                TSql = TSql + " Where  Excel_Name ='" + Temp_Grid.Parent.Name + "'";
                TSql = TSql + " And    upper(Excel_User) ='" + cls_User.gid.ToUpper() + "'";

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(TSql, "tbl_Excel_Config", ds) == false) return;

                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;

                int Check_Len = ds.Tables["tbl_Excel_Config"].Rows[0]["CheckList"].ToString().Length;
                string CheckList = ds.Tables["tbl_Excel_Config"].Rows[0]["CheckList"].ToString();

                if (Check_Len == 0) return;

                for (int i = 0; i < Check_Len; i++)
                {
                    if (CheckList.Substring(i, 1).ToString() == "1")
                    {
                        dGridView_Base.Rows[i].Cells[0].Value = "V";
                    }
                    else
                    {
                        dGridView_Base.Rows[i].Cells[0].Value = "";
                    }
                }

            }
            catch (System.Exception)
            {

            }
        }

        private void Save_Base_Data()
        {
            string CheckList = "";

            for (int i = 1; i <= dGridView_Base.RowCount; i++)
            {
                if (dGridView_Base.Rows[i - 1].Cells[0].Value.ToString() == "V")
                {
                    CheckList = CheckList + "1";
                }
                else
                {
                    CheckList = CheckList + "0";
                }
            }


            DataGridView Temp_Grid = Exp_dgv;

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string TSql;
            TSql = "Select CheckList from tbl_Excel_Config ";
            TSql = TSql + " Where  Excel_Name ='" + Temp_Grid.Parent.Name + "'";
            TSql = TSql + " And    upper(Excel_User) ='" + cls_User.gid.ToUpper() + "'";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(TSql, "tbl_Excel_Config", ds) == false) return;

            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
            {
                TSql = "insert into tbl_Excel_Config Values( ";
                TSql = TSql + "'" + cls_User.gid.ToUpper() + "','" + Temp_Grid.Parent.Name + "', '" + CheckList + "','') ";

                if (Temp_Connect.Insert_Data(TSql, "tbl_Excel_Config", this.Name.ToString(), this.Text) == false) return;


                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));

            }
            else
            {
                TSql = "Update tbl_Excel_Config Set ";
                TSql = TSql + " CheckList = '" + CheckList + "'";
                TSql = TSql + " Where  Excel_Name ='" + Temp_Grid.Parent.Name + "'";
                TSql = TSql + " And    upper(Excel_User) ='" + cls_User.gid.ToUpper() + "'";

                if (Temp_Connect.Update_Data(TSql, this.Name.ToString(), this.Text) == false) return;

                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }
        }



        public void GridViewExcel(DataGridView grid)
        {
            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            Excel.Range range;
            //Excel.Range oRng;

            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;
                range = null;
                //Add table headers going cell by cell.
                int k = 0;
                string[] colHeader = new string[grid.ColumnCount];
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    oSheet.Cells[1, i + 1] = grid.Columns[i].HeaderText;

                    if (i <= 25)
                    {
                        k = i + 65;
                        colHeader[i] = Convert.ToString((char)k);

                    }
                    else if (i > 25 && i <= 51)
                    {
                        k = i - 26;
                        colHeader[i] = "A" + colHeader[k];

                    }
                    else if (i >= 52)
                    {
                        k = i - 52;
                        colHeader[i] = "B" + colHeader[k];

                    }



                }

                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", colHeader[colHeader.Length - 1] + "1").Font.Bold = true;
                oSheet.get_Range("A1", colHeader[colHeader.Length - 1] + "1").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;





                // Create an array to multiple values at once.
                object[,] saNames = new object[grid.RowCount, grid.ColumnCount];

                PBar.Maximum = grid.RowCount + 1;
                string tp;
                for (int i = 0; i < grid.RowCount; i++)
                {
                    for (int j = 0; j < grid.ColumnCount; j++)
                    {

                        if (j <= 25)
                        {
                            range = oSheet.get_Range(colHeader[j] + Convert.ToString(i + 2), Missing.Value);
                        }
                        else if (j > 25 && j <= 51)
                        {
                            int tempc = j - 26;
                            range = oSheet.get_Range("A" + colHeader[tempc] + Convert.ToString(i + 2), Missing.Value);
                        }
                        else if (j >= 52)
                        {
                            int tempc = j - 52;
                            range = oSheet.get_Range("B" + colHeader[tempc] + Convert.ToString(i + 2), Missing.Value);
                        }

                        string T_string = grid.Columns[j].HeaderText.ToString();

                        if (T_string.IndexOf("코드") > 0 || T_string.IndexOf("번호") > 0 || T_string.IndexOf("일자") > 0 || T_string.IndexOf("핸드폰") > 0 || T_string.Contains("기록") == true)
                        {
                            range.NumberFormatLocal = @"@";
                        }

                        saNames[i, j] = grid.Rows[i].Cells[j].Value;

                    }

                    PBar.Value++;
                }

                oSheet.get_Range(colHeader[0] + "2", colHeader[colHeader.Length - 1] + (grid.RowCount + 1)).Value2 = saNames;


                int del_Cnt = 0;
                for (int j = 0; j < grid.ColumnCount; j++)
                {
                    string T_string = grid.Columns[j].HeaderText.ToString();

                    if (T_string == "" || T_string.Substring(0, 1) == "_")
                    {
                        oSheet.Columns[j + 1 - del_Cnt].delete();
                        del_Cnt++;
                    }
                }

                del_Cnt = 0;
                for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
                {
                    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() != "V")
                    {
                        oSheet.Columns[i + 1 - del_Cnt].delete();
                        del_Cnt++;
                    }
                }// end for  V체크 안한 내역을 알아와서 엑셀 상에서 그 셀을 지워 버린다.


                del_Cnt = 0;
                for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
                {
                    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                    {
                        string C_Ch = "";
                        if (del_Cnt <= 25)
                        {
                            k = del_Cnt + 65;
                            C_Ch = Convert.ToString((char)k);

                        }
                        else if (del_Cnt > 25 && del_Cnt <= 51)
                        {
                            k = del_Cnt - 26;
                            C_Ch = "A" + colHeader[k];

                        }
                        else if (del_Cnt >= 52)
                        {
                            k = del_Cnt - 52;
                            C_Ch = "B" + colHeader[k];

                        }


                        int grCnt = grid.RowCount + 1;
                        oSheet.Cells[grid.RowCount + 4, del_Cnt + 1].Formula = "=SUM(" + C_Ch + "2:" + C_Ch + grCnt.ToString() + ")";
                        del_Cnt++;
                    }
                }// end for  V체크 안한 내역을 알아와서 엑셀 상에서 그 셀을 지워 버린다.



                // 컬럼명(길이)에 맞추어 자동으로 Fiting
                oSheet.Columns.AutoFit();

                //oSheet.Columns.Summary();

                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                string Tsql = "";
                Tsql = "Insert Into tbl_Excel_User Values ( ";
                Tsql = Tsql + "'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21),";
                Tsql = Tsql + "'" + Excel_Export_From_Name + "',";
                Tsql = Tsql + "'') ";

                if (Temp_Connect.Insert_Data(Tsql, "tbl_Excel_User") == false) return;


                oXL.Visible = true;
                oXL.UserControl = true;
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                MessageBox.Show(errorMessage, "Error");
            }

        }




        private void Excel_Export_GridDate(bool captions)
        {
            int Excel_File_Cnt = 0;
            DataGridView Temp_Grid = Exp_dgv;

            //String strFolder = System.IO.Directory.GetCurrentDirectory();
            string strFolder = Application.StartupPath.ToString();
            string Base_File_Name = Excel_Export_File_Name;

        _Excel_File_Re_Check:
            string Temp_Name = System.IO.Path.Combine(strFolder + "\\Doc\\" + Base_File_Name + ".xls");

            //if (System.IO.File.Exists (Temp_Name) == true  )
            //{
            //    Excel_File_Cnt ++ ;
            //    Base_File_Name = Base_File_Name + "(" + Excel_File_Cnt.ToString() + ")";
            //    goto _Excel_File_Re_Check;
            //}





            //this.saveFileDialog.FileName = Base_File_Name + ".xls" ;
            //this.saveFileDialog.DefaultExt = "xls";
            //this.saveFileDialog.Filter = "Excel files (*.xls)|*.xls";


            //this.saveFileDialog.InitialDirectory = System.IO.Path.Combine(strFolder + "\\Doc");

            //DialogResult result = saveFileDialog.ShowDialog();

            //if (result != DialogResult.OK) return;


            int num = 0;
            object missingType = Type.Missing;

            Microsoft.Office.Interop.Excel.Application objApp;
            Microsoft.Office.Interop.Excel._Workbook objBook;
            Microsoft.Office.Interop.Excel.Workbooks objBooks;
            Microsoft.Office.Interop.Excel.Sheets objSheets;
            Microsoft.Office.Interop.Excel._Worksheet objSheet;
            Microsoft.Office.Interop.Excel.Range range;

            string[] headers = new string[Temp_Grid.ColumnCount];
            string[] columns = new string[Temp_Grid.ColumnCount];

            for (int c = 0; c < Temp_Grid.ColumnCount; c++)
            {
                headers[c] = Temp_Grid.Rows[0].Cells[c].OwningColumn.HeaderText.ToString();
                num = c + 65;
                columns[c] = Convert.ToString((char)num);
            }


            objApp = new Microsoft.Office.Interop.Excel.Application();
            objBooks = objApp.Workbooks;
            objBook = objBooks.Add(Missing.Value);
            objSheets = objBook.Worksheets;
            objSheet = (Microsoft.Office.Interop.Excel._Worksheet)objSheets.get_Item(1);
            range = null;

            try
            {
                if (captions)
                {
                    for (int c = 0; c < Temp_Grid.ColumnCount; c++)
                    {
                        if (c <= 25)
                        {
                            range = objSheet.get_Range(columns[c] + "1", Missing.Value);
                        }
                        else if (c > 25 && c <= 51)
                        {
                            int tempc = c - 26;
                            range = objSheet.get_Range("A" + columns[tempc] + "1", Missing.Value);
                        }

                        else if (c >= 52)
                        {
                            int tempc = c - 52;
                            range = objSheet.get_Range("B" + columns[tempc] + "1", Missing.Value);
                        }

                        range.NumberFormatLocal = @"@";

                        range.set_Value(Missing.Value, headers[c]);
                    }
                }

                string T_string = "";

                PBar.Maximum = Temp_Grid.RowCount;
                for (int i = 0; i < Temp_Grid.RowCount; i++)
                {
                    for (int j = 0; j < Temp_Grid.ColumnCount; j++)
                    {
                        if (Temp_Grid.Rows[0].Cells[j].Value != null)
                        {
                            if (j <= 25)
                            {
                                range = objSheet.get_Range(columns[j] + Convert.ToString(i + 2), Missing.Value);
                            }
                            else if (j > 25 && j <= 51)
                            {
                                int tempc = j - 26;
                                range = objSheet.get_Range("A" + columns[tempc] + Convert.ToString(i + 2), Missing.Value);
                            }
                            else if (j >= 52)
                            {
                                int tempc = j - 52;
                                range = objSheet.get_Range("B" + columns[tempc] + Convert.ToString(i + 2), Missing.Value);
                            }


                            T_string = Temp_Grid.Columns[j].HeaderText.ToString();

                            if (T_string.IndexOf("코드") > 0 || T_string.IndexOf("번호") > 0 || T_string.IndexOf("일자") > 0 || T_string.Contains("기록") == true)
                            {
                                range.NumberFormatLocal = @"@";
                            }
                            string Temp_value = "";
                            if (Temp_Grid.Rows[i].Cells[j].Value != null)
                                Temp_value = Temp_Grid.Rows[i].Cells[j].Value.ToString();

                            range.set_Value(Missing.Value, Temp_value);

                        }

                    }

                    PBar.Value++;
                }

                int del_Cnt = 0;
                for (int j = 0; j < Temp_Grid.ColumnCount; j++)
                {
                    T_string = Temp_Grid.Columns[j].HeaderText.ToString();

                    if (T_string == "" || T_string.Substring(0, 1) == "_")
                    {
                        objSheet.Columns[j + 1 - del_Cnt].delete();
                        del_Cnt++;
                    }
                }

                del_Cnt = 0;
                for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
                {
                    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() != "V")
                    {
                        objSheet.Columns[i + 1 - del_Cnt].delete();
                        del_Cnt++;
                    }
                }// end for  V체크 안한 내역을 알아와서 엑셀 상에서 그 셀을 지워 버린다.



                // 컬럼명(길이)에 맞추어 자동으로 Fiting
                objSheet.Columns.AutoFit();
                objApp.Visible = true;

                string fileName = String.Empty;
                fileName = saveFileDialog.FileName;

                //objBook.SaveAs(fileName,
                //     Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                //    missingType, missingType, missingType, missingType,
                //    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                //    missingType, missingType, missingType, missingType, missingType);                


                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                string Tsql = "";
                Tsql = "Insert Into tbl_Excel_User Values ( ";
                Tsql = Tsql + "'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21),";
                Tsql = Tsql + "'" + Excel_Export_From_Name + "',";
                Tsql = Tsql + "'" + fileName + "') ";

                if (Temp_Connect.Insert_Data(Tsql, "tbl_Excel_User") == false) return;





            }
            catch (System.Exception theException)
            {
                //MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Export_Err") ) ;

                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);



                //MessageBox.Show(errorMessage, "Error");
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Export_Err"));

                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(theException.Message);

            }
            finally
            {

                Marshal.ReleaseComObject(range);
                Marshal.ReleaseComObject(objSheet);
                Marshal.ReleaseComObject(objSheets);
                Marshal.ReleaseComObject(objBook);
                Marshal.ReleaseComObject(objBooks);
                Marshal.ReleaseComObject(objApp);

                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Export_End"));

                PBar.Value = 0;
                this.Close();
            }



        }













        private void chk_Total_MouseClick(object sender, MouseEventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            if (chk_Total.Checked == true)
            {
                button1_Click();
            }
            else
                button2_Click();

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }



        private void button1_Click()
        {
            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                dGridView_Base.Rows[i].Cells[0].Value = "V";
            }//  end for 그리드 상에서 엑셀 전환을 선택한 V 한 내역을 파악한다.         
        }

        private void button2_Click()
        {
            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                dGridView_Base.Rows[i].Cells[0].Value = "";

            }//  end for 그리드 상에서 엑셀 전환을 선택한 V 한 내역을 파악한다.         
        }

        private void butt_Check_01_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Check_01")
            {
                for (int i = 0; i < dGridView_Base.RowCount; i++)
                {
                    dGridView_Base.Rows[i].Cells[0].Value = "V";
                }
            }


            else if (bt.Name == "butt_Check_02")
            {
                for (int i = 0; i < dGridView_Base.RowCount; i++)
                {
                    dGridView_Base.Rows[i].Cells[0].Value = "";
                }

            }
        }

        private void button_CSV_Click(object sender, EventArgs e)
        {
            int chk_cnt = 0;

            for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
            {
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                {
                    chk_cnt++;
                }
            }//  end for 그리드 상에서 엑셀 전환을 선택한 V 한 내역을 파악한다.

            if (chk_cnt == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Select") + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Select"));
                return;
            } //end if 체크를 해서 선택한 내역이 없을 경우 메시지 뛰우고나간다.


            //ToCsV("");
            //ToCsV_2("");
            ToCsV_3("");
        }


        protected void ToCsV_3(string filename)
        {
            int Excel_File_Cnt = 0;
            DataGridView dGV = Exp_dgv;

            string strFolder = Application.StartupPath.ToString();
            string Base_File_Name = Excel_Export_File_Name;

        _Excel_File_Re_Check:
            string Temp_Name = System.IO.Path.Combine(strFolder + "\\Doc\\" + Base_File_Name + ".xls");

            if (System.IO.File.Exists(Temp_Name) == true)
            {
                Excel_File_Cnt++;
                Base_File_Name = Base_File_Name + "(" + Excel_File_Cnt.ToString() + ")";
                goto _Excel_File_Re_Check;
            }





            this.saveFileDialog.FileName = Base_File_Name + ".xls";
            this.saveFileDialog.DefaultExt = "xls";
            this.saveFileDialog.Filter = "Excel files (*.csv)|*.xls";


            this.saveFileDialog.InitialDirectory = System.IO.Path.Combine(strFolder + "\\Doc");

            DialogResult result = saveFileDialog.ShowDialog();

            if (result != DialogResult.OK) return;

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            string fileName = String.Empty;
            fileName = saveFileDialog.FileName;
            filename = saveFileDialog.FileName;

            // create one file gridview.csv in writing mode using streamwriter

            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                //System.IO.StreamWriter objSaveFile = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);                            
                //StreamWriter Sw = new StreamWriter(fs, System.Text.Encoding.Default);


                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                // now add the gridview header in csv file suffix with "," delimeter except last one

                int G_V_Cnt = 0;
                for (int i = 0; i < Exp_dgv.Columns.Count; i++)
                {
                    string T_string = dGV.Columns[i].HeaderText.ToString();



                    if (T_string == "" || T_string.Substring(0, 1) == "_")
                        T_string = T_string;
                    else
                    {
                        if (dGridView_Base.Rows[G_V_Cnt].Cells[0].Value.ToString() == "V")
                        {
                            sw.Write(Exp_dgv.Columns[i].HeaderText);

                            if (i != Exp_dgv.Columns.Count)
                            {
                                sw.Write("\t");
                            }
                        }

                        G_V_Cnt++;
                    }

                }

                //for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
                //{
                //    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() != "V")
                //    {
                //        objSheet.Columns[i + 1 - del_Cnt].delete();
                //        del_Cnt++;
                //    }
                //}// end for  V체크 안한 내역을 알아와서 엑셀 상에서 그 셀을 지워 버린다.
                // add new line

                sw.Write(sw.NewLine);

                cls_form_Meth cm = new cls_form_Meth();

                string B1 = cm._chang_base_caption_search("코드");
                string B2 = cm._chang_base_caption_search("번호");
                string B3 = cm._chang_base_caption_search("핸드폰");
                string B4 = cm._chang_base_caption_search("집전화");
                string B5 = cm._chang_base_caption_search("연락처");
                string B6 = cm._chang_base_caption_search("HP");
                string B7 = cm._chang_base_caption_search("일자");
                string B8 = cm._chang_base_caption_search("기록");

                PBar.Maximum = dGV.RowCount;
                G_V_Cnt = 0;
                foreach (DataGridViewRow dr in Exp_dgv.Rows)
                {
                    // iterate through all colums of specific row
                    G_V_Cnt = 0;
                    for (int i = 0; i < Exp_dgv.Columns.Count; i++)
                    {

                        // write particular cell to csv file
                        string T_string = dGV.Columns[i].HeaderText.ToString();

                        if (T_string == "" || T_string.Substring(0, 1) == "_")
                            T_string = T_string;
                        else
                        {

                            if (dGridView_Base.Rows[G_V_Cnt].Cells[0].Value.ToString() == "V")
                            {
                                if (T_string.Contains(B1) == true ||
                                    T_string.Contains(B2) == true ||
                                    T_string.Contains(B3) == true ||
                                    T_string.Contains(B4) == true ||
                                    T_string.Contains(B5) == true ||
                                    T_string.Contains(B6) == true ||
                                    T_string.Contains(B7) == true ||
                                    T_string.Contains(B8) == true
                                    )
                                    //"=\"" + myVariable + "\""

                                    if (dr.Cells[i].Value == DBNull.Value || dr.Cells[i].Value == null)
                                        sw.Write("");
                                    else
                                        sw.Write("=\"" + dr.Cells[i].Value.ToString().Replace("\t", "") + "\"");
                                else
                                {

                                    if (dr.Cells[i].Value != null)
                                        sw.Write(dr.Cells[i].Value.ToString().Replace("\t", ""));
                                    else
                                        sw.Write("");

                                }


                                if (i != Exp_dgv.Columns.Count)
                                {

                                    sw.Write("\t");

                                }

                             
                            }
                            G_V_Cnt++;
                        }



                    }
                    PBar.Value++;
                    // write new line

                    sw.Write(sw.NewLine);

                }

                // flush from the buffers.

                sw.Flush();

                // closes the file

                sw.Close();


                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                string Tsql = "";
                Tsql = "Insert Into tbl_Excel_User Values ( ";
                Tsql = Tsql + "'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21),";
                Tsql = Tsql + "'" + Excel_Export_From_Name + "',";
                Tsql = Tsql + "'" + fileName + "') ";

                if (Temp_Connect.Insert_Data(Tsql, "tbl_Excel_User") == false) return;

                this.Cursor = System.Windows.Forms.Cursors.Default;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = filename;
                process.Start();

                this.Close();
            }
            catch (System.Exception theException)
            {

                this.Cursor = System.Windows.Forms.Cursors.Default;

                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);



                //MessageBox.Show(errorMessage, "Error");
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Export_Err"));

                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(theException.Message);
            }

        }


        private void ToCsV(string filename)
        {
            int Excel_File_Cnt = 0;
            DataGridView dGV = Exp_dgv;

            string strFolder = Application.StartupPath.ToString();
            string Base_File_Name = Excel_Export_File_Name;

        _Excel_File_Re_Check:
            string Temp_Name = System.IO.Path.Combine(strFolder + "\\Doc\\" + Base_File_Name + ".xls");

            if (System.IO.File.Exists(Temp_Name) == true)
            {
                Excel_File_Cnt++;
                Base_File_Name = Base_File_Name + "(" + Excel_File_Cnt.ToString() + ")";
                goto _Excel_File_Re_Check;
            }





            this.saveFileDialog.FileName = Base_File_Name + ".xls";
            this.saveFileDialog.DefaultExt = "xls";
            this.saveFileDialog.Filter = "Excel files (*.csv)|*.xls";


            this.saveFileDialog.InitialDirectory = System.IO.Path.Combine(strFolder + "\\Doc");

            DialogResult result = saveFileDialog.ShowDialog();

            if (result != DialogResult.OK) return;

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            string fileName = String.Empty;
            fileName = saveFileDialog.FileName;
            filename = saveFileDialog.FileName;

            string stOutput = "";
            // Export titles:
            string sHeaders = "";

            for (int j = 0; j < dGV.Columns.Count; j++)
            {
                string T_string = dGV.Columns[j].HeaderText.ToString();

                if (T_string == "" || T_string.Substring(0, 1) == "_")
                    T_string = T_string;
                else
                    sHeaders = sHeaders.ToString() + dGV.Columns[j].HeaderText.ToString() + "\t";

            }
            stOutput += sHeaders + "\r\n";
            // Export data.

            cls_form_Meth cm = new cls_form_Meth();

            string B1 = cm._chang_base_caption_search("코드");
            string B2 = cm._chang_base_caption_search("번호");
            string B3 = cm._chang_base_caption_search("핸드폰");
            string B4 = cm._chang_base_caption_search("집전화");
            string B5 = cm._chang_base_caption_search("연락처");
            string B6 = cm._chang_base_caption_search("HP");
            string B7 = cm._chang_base_caption_search("일자");
            string B8 = cm._chang_base_caption_search("기록");


            PBar.Maximum = dGV.RowCount;
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                string stLine = "";
                for (int j = 0; j < dGV.Rows[i].Cells.Count; j++)
                {
                    //if (dGV.Rows[i].Cells[j].Value.ToString().Length > 0 && dGV.Rows[i].Cells[j].Value.ToString().Substring(0, 1) == "0")

                    string T_string = dGV.Columns[j].HeaderText.ToString();

                    if (T_string == "" || T_string.Substring(0, 1) == "_")
                        T_string = T_string;
                    else
                    {

                        if (T_string.IndexOf(B1) > 0 ||
                            T_string.IndexOf(B2) > 0 ||
                            T_string.IndexOf(B3) > 0 ||
                            T_string.IndexOf(B4) > 0 ||
                            T_string.IndexOf(B5) > 0 ||
                            T_string.IndexOf(B6) > 0 ||
                            T_string.IndexOf(B7) > 0 ||
                            T_string.IndexOf(B8) > 0
                            )
                            //"=\"" + myVariable + "\""

                            stLine = stLine.ToString() + "=\"" + dGV.Rows[i].Cells[j].Value.ToString() + "\"" + "\t";
                        else
                            stLine = stLine.ToString() + dGV.Rows[i].Cells[j].Value.ToString() + "\t";
                    }
                }
                stOutput += stLine + "\r\n";


                PBar.Value++;
            }

            // Encoding utf16 = Encoding.GetEncoding(1254);
            //byte[] output = System.Text.Encoding.Default.GetBytes(stOutput);
            byte[] output = System.Text.Encoding.Default.GetBytes(stOutput);
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);

            BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.Default);
            bw.Write(output, 0, output.Length); //write the encoded file
            bw.Flush();
            bw.Close();
            fs.Close();

            this.Cursor = System.Windows.Forms.Cursors.Default;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = filename;
            process.Start();

            this.Close();
        }










    }
}
