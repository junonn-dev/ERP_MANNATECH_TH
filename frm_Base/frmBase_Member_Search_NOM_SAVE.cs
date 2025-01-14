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
    public partial class frmBase_Member_Search_NOM_SAVE : Form
    {
        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        cls_Grid_Base cgb = new cls_Grid_Base();
        private const string base_db_name = "tbl_Memberinfo";

        //더블 클릭한 내역을 호출한 폼으로 다시 돌려 보내기 위한 델리케이트
        public delegate void SendNumberDele(string Send_Number, string Send_Name);
        public event SendNumberDele Send_Mem_Number;


        //public delegate void Send_Search_Mem_Number_Info_Dele(ref string searchMbid, ref int searchMbid2);
        //public event Send_Search_Mem_Number_Info_Dele Send_MemNumber_Info;

        public delegate void Call_searchNumber_Info_Dele(ref string searchMbid, ref int searchMbid2, ref string searchName);
        public event Call_searchNumber_Info_Dele Call_searchNumber_Info;
        

        private string Search_Member_Number_Mbid;
        private int Search_Member_Number_Mbid2;
        private string Search_Member_Name;


        public frmBase_Member_Search_NOM_SAVE()
        {
            InitializeComponent();
        }



        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Search);


            Search_Member_Number_Mbid = ""; Search_Member_Number_Mbid2 = 0;
            Search_Member_Name = "";

            Call_searchNumber_Info(ref Search_Member_Number_Mbid, ref Search_Member_Number_Mbid2, ref Search_Member_Name);

            if (Search_Member_Name != "")
                txtMemberName.Text = Search_Member_Name;
            else
                txtMemberName.Text = "";



            Base_Grid_Set();
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);


            
            
                                   
        }


        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
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
        }


        private void frmBase_From_KeyDown(object sender, KeyEventArgs e)
        {
            //폼일 경우에는 ESC버튼에 폼이 종료 되도록 한다
            if (sender is Form)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }// end if

            }

            
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
           
            //string[] g_HeaderText = { "회원 번호" , "성명" ,"주민번호" , "등록일자"   , "센타명"  
            //                         , "추천인"   , "추천인성명"   ,"" , "후원인성명" , "활동 여부"
            //                         , "중지 여부"
            StringBuilder sb = new StringBuilder();
            if ((Search_Member_Number_Mbid == "") && (Search_Member_Number_Mbid2 != 0))
                sb.AppendLine("  exec [Usp_JDE_SELECT_NOM_SAVE_like] " + Search_Member_Number_Mbid2 + "");
            if ((txtMemberName.Text.Trim() != "") && (txtCpno.Text.Trim() == ""))
                sb.AppendLine("  exec [Usp_JDE_SELECT_NOM_SAVE_like_M_NAME] '" + txtMemberName.Text + "'");
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(sb.ToString(), base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, string[]> gr_dic_text = new Dictionary<int, string[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }

            cgb.grid_name = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Data_Put();

            
        }



        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 3;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = { "회원_번호" , "성명" ,"생년월일" 
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 100, 100, 130
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               //,DataGridViewContentAlignment.MiddleCenter  
                               //,DataGridViewContentAlignment.MiddleCenter
                               //,DataGridViewContentAlignment.MiddleCenter  //5
                               
                               //,DataGridViewContentAlignment.MiddleCenter                                
                               //,DataGridViewContentAlignment.MiddleCenter
                               //,DataGridViewContentAlignment.MiddleCenter
                               //,DataGridViewContentAlignment.MiddleCenter 
                               //,DataGridViewContentAlignment.MiddleCenter //10

                               //,DataGridViewContentAlignment.MiddleCenter                                                               
                              };
            cgb.grid_col_alignment = g_Alignment;
        }


        private void Set_gr_dic(ref DataSet ds, ref Dictionary<int, string[]> gr_dic_text, int fi_cnt)
        {
            string[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0].ToString()  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1].ToString()  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2].ToString()
                                //,ds.Tables[base_db_name].Rows[fi_cnt][3].ToString()  
                                //,ds.Tables[base_db_name].Rows[fi_cnt][4].ToString() 
 
                                //,ds.Tables[base_db_name].Rows[fi_cnt][5].ToString()                                  
                                //,ds.Tables[base_db_name].Rows[fi_cnt][6].ToString() 
                                //, ds.Tables[base_db_name].Rows[fi_cnt][7].ToString() 
                                //,ds.Tables[base_db_name].Rows[fi_cnt][8].ToString() 
                                //,ds.Tables[base_db_name].Rows[fi_cnt][9].ToString() 

                                //,ds.Tables[base_db_name].Rows[fi_cnt][10].ToString()                                 
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }

                

        private void txtData_Enter(object sender, EventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            if (sender is TextBox) T_R.Text_Focus_All_Sel((TextBox)sender);

            if (sender is MaskedTextBox) T_R.Text_Focus_All_Sel((MaskedTextBox)sender);

            if (this.Controls.ContainsKey("Popup_gr"))
            {
                DataGridView T_Gd = (DataGridView)this.Controls["Popup_gr"];
                T_Gd.Visible = false;
                T_Gd.Dispose();
            }
        }

        
        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);

            TextBox tb = (TextBox)sender;

            if ((tb.Tag == null) ||  (tb.Tag.ToString () == ""))
            {
                //숫자만 입력 가능하다.
                if (T_R.Text_KeyChar_Check(e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if   
            }
            else if (tb.Tag.ToString() == "1")
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, 1) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

            if (tb.TextLength + 1 >= tb.MaxLength)
                SendKeys.Send("{TAB}");


        }


        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }




        private void Base_Button_Click(object sender, EventArgs e)
        {
            if ((txtMemberName.Text.Trim() == "") && (txtCpno.Text.Trim() == ""))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Select_Term")                  
                   + "\n" +
                   cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));                
                txtMemberName.Focus(); return;
            }

            Base_Grid_Set();
            dGridView_Base.Focus();
        }

        private void frmBase_Member_Search_Activated(object sender, EventArgs e)
        {
            if (dGridView_Base.Rows.Count >0)
                dGridView_Base.Focus();

            Search_Member_Name = "";
        }

        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            //cls_app_static_var.Search_Member_Name_Return = "";
            //cls_app_static_var.Search_Member_Number_Return = "";

            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string Send_Nubmer = ""; string Send_Name = "";
                //cls_app_static_var.Search_Member_Number_Return = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                //cls_app_static_var.Search_Member_Name_Return = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();

                Send_Nubmer = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                Send_Name = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                Send_Mem_Number(Send_Nubmer, Send_Name);   
            }

            this.Close();
        }

     
    }
}
