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
    public partial class frmClose_Sham_Pay_Real_2 : Form
    {
        


        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cg_Sub = new cls_Grid_Base();

        private const string base_db_name = "tbl_Sham_Pay_Real";
        private int Data_Set_Form_TF;

        private string idx_Na_Code = "";

        public frmClose_Sham_Pay_Real_2()
        {
            InitializeComponent();           
        }


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Sub.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            Data_Set_Form_TF = 1;            
            Data_Set_Form_TF = 0;

            cls_Pro_Base_Function cpbf = new cls_Pro_Base_Function();
            cpbf.Put_Close_Sort_ComboBox(combo_Pay2, combo_Pay_Code2);
            cpbf.Put_Close_Sort_ComboBox(combo_Pay, combo_Pay_Code);


            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtSMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtSMbid2.Mask = cls_app_static_var.Member_Number_Fromat;

            mtxtSellDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtSellDate2.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtSellDate3.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtMakeDate1.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtMakeDate2.Mask = cls_app_static_var.Date_Number_Fromat;

            if (tab_Nation.Visible == true)
            {
                cpbf.Put_NaCode_ComboBox(combo_Se, combo_Se_Code);
            }

            Put_Pay_C_Date();


            radioB_A1.Checked = true;
        }



        private void frm_Base_Activated(object sender, EventArgs e)
        {
            this.Refresh();

            if (cls_User.uSearch_MemberNumber != "")
            {
                Data_Set_Form_TF = 1;
                mtxtMbid.Text = cls_User.uSearch_MemberNumber;
                mtxtSMbid.Text = cls_User.uSearch_MemberNumber;
                cls_User.uSearch_MemberNumber = "";

                EventArgs ee1 = null; Base_Button_Click(butt_Search, ee1);
                Set_Form_Date(mtxtMbid.Text);
                Data_Set_Form_TF = 0;
                radioB_A1.Checked = true;
            }
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Delete.Left = butt_Save.Left + butt_Save.Width + 2;
            //butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);

            cfm.button_flat_change(butt_Search);
            cfm.button_flat_change(butt_Excel);
            //cfm.button_flat_change(butt_Save);

        }
       

        private void frmBase_From_KeyDown(object sender, KeyEventArgs e)
        {
            //폼일 경우에는 ESC버튼에 폼이 종료 되도록 한다
            if (sender is Form)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (!this.Controls.ContainsKey("Popup_gr"))
                    {
                        this.Close();
                        return;
                    }
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
                            return;
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
                T_bt = butt_Clear;    //리셋  F5    

            if (T_bt.Visible == true)
            {
                EventArgs ee1 = null;
                if (e.KeyValue == 123 || e.KeyValue == 113 || e.KeyValue == 115 || e.KeyValue == 119 || e.KeyValue == 112)
                    Base_Button_Click(T_bt, ee1);
            }

        }


        private void Put_Pay_C_Date()
        {
            combo_C_Date.Items.Clear();

           
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select ToEndDate  ";
            Tsql = Tsql + " From tbl_CloseTotal_02  (nolock)  ";            

            Tsql = Tsql + " Order by ToEndDate DESC  ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            Temp_Connect.Open_Data_Set(Tsql, "tbl_CloseTotal_04", ds);
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt <= 0)
                return;

            combo_C_Date.Items.Add("");            

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                combo_C_Date.Items.Add(ds.Tables["tbl_CloseTotal_04"].Rows[fi_cnt]["ToEndDate"].ToString());                
            }

            combo_C_Date.SelectedIndex = -1;            
            //++++++++++++++++++++++++++++++++
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

                if (tb.Name == "txt_Pv")
                {
                    if (tb.Text != "")
                        tb.Text = string.Format(cls_app_static_var.str_Currency_Type, double.Parse(tb.Text.Replace(",", "")));
                }

            }

            if (sender is MaskedTextBox)
            {
                MaskedTextBox tb = (MaskedTextBox)sender;
                if (tb.ReadOnly == false)
                    tb.BackColor = Color.White;
            }
        }



        //회원번호 클릿햇을때. 관련 정보들 다 리셋 시킨다.
        //추후 번호만 변경하고 엔터 안누눌러서.. 데이타가 엉키는 것을 방지하기 위함.
        private void mtxtMbid_Click(object sender, EventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)sender;

            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, mtxtMbid);
            radioB_A1.Checked = true;

            Put_Pay_C_Date();

            //마스크텍스트 박스에 입력한 내용이 있으면 그곳 다음으로 커서가 가게 한다.
            if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                mtb.SelectionStart = mtb.Text.Replace("-", "").Replace("_", "").Trim().Length + 1;

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

                MaskedTextBox mtb = (MaskedTextBox)sender;

                if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                {
                    int reCnt = 0;
                    cls_Search_DB cds = new cls_Search_DB();
                    string Search_Name = "";
                    reCnt = cds.Member_Name_Search(mtb.Text, ref Search_Name);

                    if (reCnt == 1)
                    {
                        if (mtb.Name == "mtxtMbid")
                        {
                            txtName.Text = Search_Name;
                            if (Input_Error_Check(mtb, 0) == true)
                                Set_Form_Date(mtb.Text);
                            //SendKeys.Send("{TAB}");

                        }

                    }

                    else if (reCnt > 1)  //회원번호 비슷한 사람들이 많은 경우
                    {
                        string Mbid = "";
                        int Mbid2 = 0;
                        cds.Member_Nmumber_Split(mtb.Text, ref Mbid, ref Mbid2);

                        //cls_app_static_var.Search_Member_Number_Mbid = Mbid;
                        //cls_app_static_var.Search_Member_Number_Mbid2 = Mbid2;
                        frmBase_Member_Search e_f = new frmBase_Member_Search();

                        if (mtb.Name == "mtxtMbid")
                        {
                            e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                            e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemNumber_Info);
                        }

                        e_f.ShowDialog();

                        SendKeys.Send("{TAB}");
                    }
                }
                else
                    SendKeys.Send("{TAB}");
            }

        }




        void e_f_Send_MemNumber_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            seachName = "";
            cls_Search_DB csb = new cls_Search_DB();
            csb.Member_Nmumber_Split(mtxtMbid.Text.Trim(), ref searchMbid, ref searchMbid2);
        }

        //변경할려는 대상자에 대한 회원번호에서 회원 검색창을 뛰엇을 경우에
        void e_f_Send_Mem_Number(string Send_Number, string Send_Name)
        {
            mtxtMbid.Text = Send_Number; txtName.Text = Send_Name;
            if (Input_Error_Check(mtxtMbid, 0) == true)
                Set_Form_Date(mtxtMbid.Text);
        }



        //회원번호 입력 박스의 내역이 모두 지워지면 하부 관련 회원데이타 내역을 다 리셋 시킨다. 
        private void mtxtMbid_TextChanged(object sender, EventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Text.Replace("_", "").Replace("-", "").Replace(" ", "") == "")
            {
                cls_form_Meth ct = new cls_form_Meth();
                if (mtb.Name == "mtxtMbid")
                {
                    ct.from_control_clear(this, mtb);

                }
                //    ct.from_control_clear(groupBox2, mtb);

                //ct.from_control_clear((GroupBox)mtb.Parent, mtb);
            }
        }



        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);
            T_R.Key_Enter_13_Ncode += new Key_13_Ncode_Event_Handler(T_R_Key_Enter_13_Ncode);
            T_R.Key_Enter_13_Name += new Key_13_Name_Event_Handler(T_R_Key_Enter_13_Name);

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

            else if ((tb.Tag != null) && (tb.Tag.ToString() == "-1"))
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, -1) == false)
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

            else if (tb.Tag.ToString() == "name")  //회원 정보 관련해서 이름 검색을 필요로 하는 텍스트 박스이다.
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(tb, e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

        }

        void T_R_Key_Enter_13_Name(string txt_tag, TextBox tb)
        {
            if (txt_tag != "")
            {
                int reCnt = 0;
                cls_Search_DB cds = new cls_Search_DB();
                string Search_Mbid = "";
                reCnt = cds.Member_Name_Search(ref Search_Mbid, txt_tag);

                if (reCnt == 1)
                {
                    if (tb.Name == "txtName")
                    {
                        mtxtMbid.Text = Search_Mbid; //회원명으로 검색해서 나온 사람이 한명일 경우에는 회원번호를 넣어준다.                    
                        if (Input_Error_Check(mtxtMbid, 0) == true)
                            Set_Form_Date(mtxtMbid.Text);

                        //SendKeys.Send("{TAB}");
                    }


                }
                else if (reCnt != 1)  //동명이인이 존재해서 사람이 많을 경우나 또는 이름 없이 엔터친 경우에.
                {

                    frmBase_Member_Search e_f = new frmBase_Member_Search();
                    if (tb.Name == "txtName")
                    {
                        e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                        e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemName_Info);
                    }

                    e_f.ShowDialog();

                    SendKeys.Send("{TAB}");
                }


            }
            else
                SendKeys.Send("{TAB}");

        }

        void e_f_Send_MemName_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            searchMbid = ""; searchMbid2 = 0;
            seachName = txtName.Text.Trim();
        }




        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            int Sw_Tab = 0;

            if ((sender is TextBox) == false) return;

            TextBox tb = (TextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
                Sw_Tab = 1;
            }

            ////if (tb.Name == "txtSellCode")
            ////{
            ////    if (tb.Text.Trim() == "")
            ////        txtSellCode_Code.Text = "";
            ////    else if (Sw_Tab == 1)
            ////        Ncod_Text_Set_Data(tb, txtSellCode_Code);
            ////}

            //if (tb.Name == "txtCenter2")
            //{
            //    if (tb.Text.Trim() == "")
            //        txtCenter_Code2.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txtCenter_Code2);
            //}

            if (tb.Name == "txtR_Id")
            {
                if (tb.Text.Trim() == "")
                    txtR_Id_Code.Text = "";
                else if (Sw_Tab == 1)
                    Ncod_Text_Set_Data(tb, txtR_Id_Code);
            }

            //if (tb.Name == "txtR_Id2")
            //{
            //    if (tb.Text.Trim() == "")
            //        txtR_Id_Code2.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txtR_Id_Code2);
            //}

            //if (tb.Name == "txt_ItemName2")
            //{
            //    if (tb.Text.Trim() == "")
            //        txt_ItemName_Code2.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txt_ItemName_Code2);
            //}

            
        }



        void T_R_Key_Enter_13()
        {
           
            SendKeys.Send("{TAB}");
        }


        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {
            //if (tb.Name == "txtSellCode")
            //{
            //    Data_Set_Form_TF = 1;
            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb, txtSellCode_Code, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txtSellCode_Code);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}

            //if (tb.Name == "txtCenter2")
            //{
            //    Data_Set_Form_TF = 1;
            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb, txtCenter_Code2, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txtCenter_Code2);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}

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

            //if (tb.Name == "txtR_Id2")
            //{
            //    Data_Set_Form_TF = 1;
            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb, txtR_Id_Code2, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txtR_Id_Code2);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}

            //if (tb.Name == "txt_ItemName2")
            //{
            //    Data_Set_Form_TF = 1;
            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb, txt_ItemName_Code2, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txt_ItemName_Code2);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}
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
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txtBank")
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txtR_Id")
                cgb_Pop.Next_Focus_Control = txtRemark;

            if (tb.Name == "txtChange")
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txtSellCode")
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txt_Base_Rec")
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txt_Receive_Method")
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txt_ItemCode")
                cgb_Pop.Next_Focus_Control = butt_Search;

            if (tb.Name == "txt_ItemName2")
                cgb_Pop.Next_Focus_Control = butt_Search;

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
            cgb_Pop.Base_tb_2 = tb;    //2번은 명임
            cgb_Pop.Base_Location_obj = tb;

            if (strSql != "")
            {
                if (tb.Name == "txtCenter" || tb.Name == "txtCenter2")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtR_Id" || tb.Name == "txtR_Id2")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txt_ItemName2")
                    cgb_Pop.db_grid_Popup_Base(2, "상품_코드", "상품명", "Ncode", "Name", strSql);

                if (tb.Name == "txtSellCode")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "구매_코드", "구매종류", "SellCode", "SellTypeName", strSql);
                    cgb_Pop.Next_Focus_Control = txt_Pv;
                }

            }
            else
            {
                if (tb.Name == "txtCenter" || tb.Name == "txtCenter2")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                }

                if (tb.Name == "txtR_Id" || tb.Name == "txtR_Id2")
                {
                    string Tsql;
                    Tsql = "Select user_id ,U_Name   ";
                    Tsql = Tsql + " From tbl_User (nolock) ";
                    Tsql = Tsql + " Order by user_id ";

                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", Tsql);
                }

                if (tb.Name == "txt_ItemName2")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name    ";
                    Tsql = Tsql + " From tbl_Goods (nolock) ";
                    //Tsql = Tsql + " Where GoodUse = 0 ";
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "상품_코드", "상품명", "Ncode", "Name", Tsql);
                }

                if (tb.Name == "txtSellCode")
                {
                    string Tsql;
                    Tsql = "Select SellCode ,SellTypeName    ";
                    Tsql = Tsql + " From tbl_SellType (nolock) ";
                    Tsql = Tsql + " Order by SellCode ";

                    cgb_Pop.db_grid_Popup_Base(2, "구매_코드", "구매종류", "SellCode", "SellTypeName", Tsql);
                    cgb_Pop.Next_Focus_Control = txt_Pv;
                }

            }
        }



        private void Ncod_Text_Set_Data(TextBox tb, TextBox tb1_Code)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            if (tb.Name == "txtCenter" || tb.Name == "txtCenter2")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
            }

            if (tb.Name == "txtR_Id" || tb.Name == "txtR_Id2")
            {
                Tsql = "Select user_id ,U_Name   ";
                Tsql = Tsql + " From tbl_User (nolock) ";
                Tsql = Tsql + " Where U_Name like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    user_id like '%" + tb.Text.Trim() + "%'";
            }

            if (tb.Name == "txt_ItemName2")
            {
                Tsql = "Select Ncode , Name    ";
                Tsql = Tsql + " From tbl_Goods (nolock) ";
                Tsql = Tsql + " Where Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%'";
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


















        private void Form_Clear_()
        {
            
           

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Sub.d_Grid_view_Header_Reset();            
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            
            mtxtMbid.ReadOnly = false;
            txtName.ReadOnly = false;
            mtxtSellDate.ReadOnly = false;

            mtxtMbid.BorderStyle = BorderStyle.Fixed3D;
            txtName.BorderStyle = BorderStyle.Fixed3D;
            mtxtSellDate.BorderStyle = BorderStyle.Fixed3D;

            mtxtMbid.BackColor = SystemColors.Window;
            txtName.BackColor = SystemColors.Window;
            mtxtSellDate.BackColor = SystemColors.Window;

            DTP_SellDate.Visible = true;

            combo_Pay.SelectedIndex = -1;
            tableLayoutPanel5.Enabled = true ; //마감 적용일자를  풀어놓는다.
            tableLayoutPanel6.Enabled = true; //마감 적용일자를 수정 못하게 해버린다...
            tableLayoutPanel15.Enabled = true; //마감 적용일자를 수정 못하게 해버린다...
            tableLayoutPanel13.Enabled = true;
            tableLayoutPanel14.Enabled = true;
            tab_Nation.Enabled = true;

            combo_C_Date.Items.Clear();
            idx_Na_Code = "";
            
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, mtxtMbid);
            tableLayoutPanel13.Enabled = true;
            radioB_A1.Checked = true;
            Put_Pay_C_Date();

        }






        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Clear")
            {
                Form_Clear_();    
            }

            else if (bt.Name == "butt_Save")
            {
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

               
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {
                    Form_Clear_();
                }
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            
            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }

            else if (bt.Name == "butt_Delete")
            {
                int Delete_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Delete_Base_Data(ref Delete_Error_Check);

                if (Delete_Error_Check > 0)
                {
                    Form_Clear_();
                }
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }                
            else if (bt.Name == "butt_Search")
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                combo_Pay_Code2.SelectedIndex = combo_Pay2.SelectedIndex;
                Base_Sub_Grid_Set();  //뿌려주는 곳
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
            cls_form_Meth cm = new cls_form_Meth();
            Excel_Export_File_Name = this.Text; // cm._chang_base_caption_search ( "기타_수당") ;
            Excel_Export_From_Name = this.Name;
            return dGridView_Base_Sub;
        }




















        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);            
        }





        private Boolean Search_Check_TextBox_Error()
        {

            cls_Check_Input_Error c_er = new cls_Check_Input_Error();

            if (mtxtSMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtSMbid);

                if (Ret == -1)
                {
                    mtxtSMbid.Focus(); return false;
                }
            }


            if (mtxtSMbid2.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtSMbid2);

                if (Ret == -1)
                {
                    mtxtSMbid2.Focus(); return false;
                }
            }


            if (mtxtSellDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate2.Text, mtxtSellDate2, "Date") == false)
                {
                    mtxtSellDate2.Focus(); return false;
                }
            }


            if (mtxtSellDate3.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtSellDate3.Text, mtxtSellDate3, "Date") == false)
                {
                    mtxtSellDate3.Focus(); return false;
                }
            }


            if (mtxtMakeDate1.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtMakeDate1.Text, mtxtMakeDate1, "Date") == false)
                {
                    mtxtMakeDate1.Focus(); return false;
                }
            }

            if (mtxtMakeDate2.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_(mtxtMakeDate2.Text, mtxtMakeDate2, "Date") == false)
                {
                    mtxtMakeDate2.Focus(); return false;
                }
            }


            return true;
        }



        private void Base_Sub_Grid_Set()
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Sub_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Sub.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< 

            if (Search_Check_TextBox_Error() == false) return;           
         

            string Tsql = "";

            //string[] g_HeaderText = {"입고번호"  , "입고일자"   , "상품코드"  , "상품명"   , "입고지"        
            //                        , "입고수량"   , "입고자"    , "비고"   , ""    , ""                                
            //                            };

            cls_form_Meth cf = new cls_form_Meth();
            Tsql = "Select ";
            //if (cls_app_static_var.Member_Number_1 > 0)
            //    Tsql = Tsql + " tbl_Sham_Pay_Real.mbid + '-' + Convert(Varchar,tbl_Sham_Pay_Real.mbid2) ";
            //else
            //    Tsql = Tsql + " tbl_Sham_Pay_Real.mbid2 ";

            if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 > 0)
                Tsql = Tsql + " tbl_Sham_Pay_Real.mbid + '-' + Convert(Varchar,tbl_Sham_Pay_Real.mbid2)  ";
            else if (cls_app_static_var.Member_Number_1 == 0 && cls_app_static_var.Member_Number_2 > 0)
                Tsql = Tsql + " tbl_Sham_Pay_Real.mbid2  ";
            else if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 == 0)
                Tsql = Tsql + " tbl_Sham_Pay_Real.mbid  ";


            Tsql = Tsql + " ,tbl_Sham_Pay_Real.M_Name ";            
            Tsql = Tsql + " ,LEFT(Apply_Date,4) +'-' + LEFT(RIGHT(Apply_Date,4),2) + '-' + RIGHT(Apply_Date,2) " ;
            Tsql = Tsql + " ,Apply_Pv ";
            //Tsql = Tsql + " ,'수당합계' ";
            Tsql = Tsql + " , Case ";
            Tsql = Tsql + "  When SortKind2 = '1' Then '" + cf._chang_base_caption_search("후원보너스") + "' ";
            Tsql = Tsql + "  When SortKind2 = '2' Then '" + cf._chang_base_caption_search("추천매칭보너스") + "' ";
            Tsql = Tsql + "  When SortKind2 = '3' Then '" + cf._chang_base_caption_search("추천보너스") + "' ";
            Tsql = Tsql + "  When SortKind2 = 'ETC' Then '" + cf._chang_base_caption_search("기타보너스") + "' ";            
            Tsql = Tsql + "  When SortKind2 = '0' Then '" + cf._chang_base_caption_search("수당합계") + "' ";
            Tsql = Tsql + " END ";

            Tsql = Tsql + " ,tbl_Sham_Pay_Real.ETC " ;
            Tsql = Tsql + " ,tbl_Sham_Pay_Real.RecordID " ;
            Tsql = Tsql + " ,tbl_Sham_Pay_Real.RecordTime ";
            Tsql = Tsql + " ,tbl_Sham_Pay_Real.seq ";
            Tsql = Tsql + " ,tbl_Sham_Pay_Real.SortKind2";
            Tsql = Tsql + " ,tbl_Sham_Pay_Real.Base_Pv";
            Tsql = Tsql + " ,tbl_Sham_Pay_Real.Na_Code";
            Tsql = Tsql + " From tbl_Sham_Pay_Real  (nolock) ";
            Tsql = Tsql + " Left Join tbl_Memberinfo   (nolock) On tbl_Memberinfo.Mbid  = tbl_Sham_Pay_Real.Mbid  And tbl_Memberinfo.Mbid2 = tbl_Sham_Pay_Real.Mbid2  ";
            Tsql = Tsql + " Left Join tbl_Business  (nolock)  On tbl_Memberinfo.BusinessCode  = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code  ";
            Tsql = Tsql + " LEFT Join tbl_SellType_Close   (nolock) ON tbl_Sham_Pay_Real.SortKind2 = tbl_SellType_Close.CloseCode ";

            string strSql = " Where   tbl_Sham_Pay_Real.Clo_TF = 2 ";
            
            string Mbid = ""; int Mbid2 = 0;
            //회원번호1로 검색
            if (
                (mtxtSMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
                &&
                (mtxtSMbid2.Text.Replace("-", "").Replace("_", "").Trim() == "")
                )
            {
                cls_Search_DB csb = new cls_Search_DB();
                if (csb.Member_Nmumber_Split(mtxtSMbid.Text, ref Mbid, ref Mbid2) == 1)
                {
                    if (Mbid != "")
                        strSql = strSql + " And tbl_Sham_Pay_Real.Mbid ='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql = strSql + " And tbl_Sham_Pay_Real.Mbid2 = " + Mbid2;
                }
            }


            //회원번호2로 검색
            if (
                (mtxtSMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
                &&
                (mtxtSMbid2.Text.Replace("-", "").Replace("_", "").Trim() != "")
                )
            {
                cls_Search_DB csb = new cls_Search_DB();
                if (csb.Member_Nmumber_Split(mtxtSMbid.Text, ref Mbid, ref Mbid2) == 1)
                {
                    if (Mbid != "")
                        strSql = strSql + " And tbl_Sham_Pay_Real.Mbid >='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql = strSql + " And tbl_Sham_Pay_Real.Mbid2 >= " + Mbid2;
                }

                if (csb.Member_Nmumber_Split(mtxtSMbid2.Text, ref Mbid, ref Mbid2) == 1)
                {
                    if (Mbid != "")
                        strSql = strSql + " And tbl_Sham_Pay_Real.Mbid <='" + Mbid + "'";

                    if (Mbid2 >= 0)
                        strSql = strSql + " And tbl_Sham_Pay_Real.Mbid2 <= " + Mbid2;
                }
            }

            //회원명으로 검색
            if (txtName2.Text.Trim() != "")
                strSql = strSql + " And tbl_Sham_Pay_Real.M_Name Like '%" + txtName2.Text.Trim() + "%'";

            //회원명으로 검색
            if (combo_Pay_Code2.Text.Trim() != "")
                strSql = strSql + " And tbl_Sham_Pay_Real.SortKind2 = '" + combo_Pay_Code2.Text.Trim() + "'";


            //가입일자로 검색 -1
            if ((mtxtSellDate2.Text.Replace("-", "").Trim() != "") && (mtxtSellDate3.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And tbl_Sham_Pay_Real.Apply_Date = '" + mtxtSellDate2.Text.Replace("-", "").Trim() + "'";

            //가입일자로 검색 -2
            if ((mtxtSellDate2.Text.Replace("-", "").Trim() != "") && (mtxtSellDate3.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And tbl_Sham_Pay_Real.Apply_Date >= '" + mtxtSellDate2.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And tbl_Sham_Pay_Real.Apply_Date <= '" + mtxtSellDate3.Text.Replace("-", "").Trim() + "'";
            }


            //기록일자로 검색 -1
            if ((mtxtMakeDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakeDate2.Text.Replace("-", "").Trim() == ""))
                strSql = strSql + " And Replace(Left( tbl_Sham_Pay_Real.recordtime ,10),'-','') = '" + mtxtMakeDate1.Text.Replace("-", "").Trim() + "'";

            //기록일자로 검색 -2
            if ((mtxtMakeDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakeDate2.Text.Replace("-", "").Trim() != ""))
            {
                strSql = strSql + " And Replace(Left( tbl_Sham_Pay_Real.recordtime ,10),'-','') >= '" + mtxtMakeDate1.Text.Replace("-", "").Trim() + "'";
                strSql = strSql + " And Replace(Left( tbl_Sham_Pay_Real.recordtime ,10),'-','') <= '" + mtxtMakeDate2.Text.Replace("-", "").Trim() + "'";
            }

             if (txtR_Id_Code.Text.Trim() != "")
                strSql = strSql + " And tbl_Sham_Pay_Real.recordid = '" + txtR_Id_Code.Text.Trim() + "'";





            strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            strSql = strSql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";


            Tsql = Tsql + strSql;
            Tsql = Tsql + " Order by tbl_Sham_Pay_Real.Mbid ,  tbl_Sham_Pay_Real.Mbid2   ";

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
                Set_Sub_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }

            cg_Sub.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cg_Sub.db_grid_Obj_Data_Put();
        }


        private void Set_Sub_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;
            object[] row0 = new object[cg_Sub.grid_col_Count];

            while (Col_Cnt < cg_Sub.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }

            gr_dic_text[fi_cnt + 1] = row0;
        }



        private void dGridView_Base_Sub_Header_Reset()
        {
            cg_Sub.grid_col_Count = 12;
            cg_Sub.basegrid = dGridView_Base_Sub;
            cg_Sub.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cg_Sub.grid_Frozen_End_Count = 2;
            cg_Sub.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원번호",  "성명"  , "마감_종료일"   , "적용_금액"  , "수당구분"          
                                , "비고"  , "기록자"   , "기록일자"    , "seq"   , "_SortKind2"   
                                ,"전금액"  ,"_국가코드"  
                                };
            cg_Sub.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90,  110, 90, 80, 100
                             , 110 ,70 , 100 ,  0 , 0         
                             , 110 , 0
                            };
            cg_Sub.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                                                                                             
                                     ,true,true
                                   };
            cg_Sub.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleLeft  //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft                              
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft //10      
                             
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleLeft
                          
                              };
            cg_Sub.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cg_Sub.grid_cell_format = gr_dic_cell_format;           
        }















        //private bool numericCheck(string ss)
        //{
        //     cls_Check_Text T_R = new cls_Check_Text();

        //    //쿼리문 오류관련 입력만 아니면 가능하다.
        //    if (T_R.Text_KeyChar_Check(e, 1) == false)
        //    {
        //        e.Handled = true;
        //        return;
        //    } // end if   

        //    //try
        //    //{
        //    //    int ll = Convert.ToInt32(ss);
        //    //    return true;
        //    //}
        //    //catch
        //    //{
        //    //    return false;
        //    //}
        //}




        
        private Boolean Check_TextBox_Error(ref int Not_Pay_FLAG )
        {

            cls_Check_Text T_R = new cls_Check_Text();
            string me = "";

            combo_Pay_Code.SelectedIndex = combo_Pay.SelectedIndex;
            combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex;

            //if (combo_Pay.Text == "" || combo_Pay_Code.Text == "" )
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
            //           + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_PaySort")
            //          + "\n" +
            //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    combo_Pay.Focus(); return false;
            //}

            cls_Check_Input_Error c_er = new cls_Check_Input_Error();

            if (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtMbid);

                if (Ret == -1)
                {
                    mtxtMbid.Focus(); return false;
                }
            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return false;
            }



            if (combo_C_Date.Text.Trim () == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_ApplyDate")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                combo_C_Date.Focus(); return false;
            }

            //if (radioB_A3.Checked == true)
            //{
            //    if (int.Parse(combo_C_Date.Text.Trim().Substring(0, 6)) >= 201502)
            //    {
            //        MessageBox.Show("2015년 2월 이후의 수당부터는 장려금3을 적용할 수 없습니다."
            //          + "\n" +
            //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //        combo_C_Date.Focus(); return false;
            //    }
            //}

            

            //if (combo_C_Date.Text.Trim () != "")
            //{
            //    if (Sn_Number_(mtxtSellDate.Text, mtxtSellDate, "Date") == false)
            //    {
            //        mtxtSellDate.Focus(); return false;
            //    }
            //}




            //if (combo_Se_Code.Text == "")  //다국어 지원프로그램을 사용시 국가는 필히 선택을 해야 된다.
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
            //       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Not_Na_Code")
            //      + "\n" +
            //      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

            //    combo_Se.Focus(); return false ;
            //}


            me = T_R.Text_Null_Check(txt_Pv, "Msg_Sort_Sham_Pay"); //적용 PV를
            if (me != "")
            {
                MessageBox.Show(me);
                return false;
            }


            string SortKind2 = "0";

            if (radioB_A1.Checked == true)
                SortKind2 = "1";

            if (radioB_A2.Checked == true)
                SortKind2 = "2";

            if (radioB_A3.Checked == true)
                SortKind2 = "3";

            //if (radioB_A4.Checked == true)
            //    SortKind2 = "4";

            //if (radioB_A5.Checked == true)
            //    SortKind2 = "5";

            //if (radioB_A6.Checked == true)
            //    SortKind2 = "6";

            //if (radioB_A7.Checked == true)
            //    SortKind2 = "7";

            //if (radioB_A8.Checked == true)
            //    SortKind2 = "8";

            if (radioB_ETC.Checked == true)
                SortKind2 = "ETC";

            if (SortKind2 == "0")
            {
                MessageBox.Show("적용수당을 선택해 주십시요. 확인후 다시 시도해 주십시요.");
                radioB_A1.Focus(); return false;
            }







            cls_Search_DB csd = new cls_Search_DB();
            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);


            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql = "";

            //적용 할려는 마감에 미지급액으로 해서 넘어온 내역이 있는지를 체크를한다.
            Tsql = "Select ToEndDate,mbid,mbid2,Close_FLAG,PayDate, Check_FLAG ";
            Tsql = Tsql + " From tbl_Close_Not_Pay (nolock) ";
            Tsql = Tsql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            Tsql = Tsql + " And    Mbid = '" + Mbid + "'";
            Tsql = Tsql + " And    Mbid2 = " + Mbid2;
            Tsql = Tsql + " And    Close_FLAG = 'W'";
            Tsql = Tsql + " And    Check_FLAG = 'M'";
            //++++++++++++++++++++++++++++++++

            DataSet ds2 = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds2) == false) return false;
            int ReCnt2 = Temp_Connect.DataSet_ReCount;

            if (ReCnt2 > 0)
            {
                Not_Pay_FLAG = 1;

                Tsql = "Select ToEndDate  ";
                Tsql = Tsql + " From tbl_CloseTotal_02 (nolock) ";
                Tsql = Tsql + " Where  ToEndDate > '" + combo_C_Date.Text.Trim() + "'";


                DataSet ds3 = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds3) == false) return false;
                int ReCnt3 = Temp_Connect.DataSet_ReCount;

                if (ReCnt3 > 0)   //이후에 마감이 도 내역이 잇다. 그럼 수정 할수 없다.
                {
                    MessageBox.Show("현 회원, 처리 날짜 이후에 정산처리된 마감 내역이 있습니다. 현회원의 처리 날짜에 미지급을 넘긴 내역이 존재합니다. 처리가 불가능 합니다. 확인후 다시 시도해 주십시요.");
                    txt_Pv.Focus(); return false;
                }
            }

            Tsql = "";
            Tsql = "Select Allowance1, Allowance2, Allowance3, Allowance4 , Allowance5, Allowance6, Allowance7 , Allowance8 ";
            Tsql = Tsql + " , Etc_Pay , Cur_DedCut_Pay, SumAllAllowance  , TruePayment , Etc_Pay_DedCut ,Cur_DedCut_Pay_DED    ";
            Tsql = Tsql + " , Cpno , BankAcc  ";
            Tsql = Tsql + " From tbl_ClosePay_02_Mod (nolock) ";
            Tsql = Tsql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            Tsql = Tsql + " And    Mbid = '" + Mbid + "'";
            Tsql = Tsql + " And    Mbid2 = " + Mbid2;
            //++++++++++++++++++++++++++++++++

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            double Allowance1 = 0, Allowance2 = 0, Allowance3 = 0, Allowance4 = 0, Allowance5 = 0, Allowance6 = 0, Allowance7 = 0, Allowance8 = 0
                  , Etc_Pay = 0, Cur_DedCut_Pay = 0, SumAllAllowance = 0, Cur_DedCut_Pay_DED = 0;
            if (ReCnt > 0)
            {
                Allowance1 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance1"].ToString());
                Allowance2 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance2"].ToString());
                Allowance3 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance3"].ToString());
                Allowance4 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance4"].ToString());
                Allowance5 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance5"].ToString());
                Allowance6 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance6"].ToString());
                Allowance7 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance7"].ToString());
                Allowance8 = double.Parse(ds.Tables[base_db_name].Rows[0]["Allowance8"].ToString());
                Cur_DedCut_Pay = double.Parse(ds.Tables[base_db_name].Rows[0]["Cur_DedCut_Pay"].ToString());
                Cur_DedCut_Pay_DED = double.Parse(ds.Tables[base_db_name].Rows[0]["Cur_DedCut_Pay_DED"].ToString());
                string Cpno = ds.Tables[base_db_name].Rows[0]["Cpno"].ToString();
                string BankAcc = ds.Tables[base_db_name].Rows[0]["BankAcc"].ToString();

                if ((Cpno == "" || BankAcc == "") && Not_Pay_FLAG == 0)
                {
                    MessageBox.Show("적용 마감일은 미수금이 발생되어야 하는 마감입니다. 때문에 0원 수당을 +로 처리 할 수 없습니다. 발생 수당을 미수금으로 처리 해야 하기 때문에.  적용할 수 없습니다.");
                    txt_Pv.Focus(); return false;
                }


                //double app_Pv = double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                //if (app_Pv < 0 && -app_Pv > double.Parse(ds.Tables[base_db_name].Rows[0]["SumAllAllowance"].ToString()))
                //{
                //    MessageBox.Show("현 회원의 입력 날짜에 수당 금액이 감할려고 하는 금액보다 작습니다. 확인후 다시 시도해 주십시요.");
                //    txt_Pv.Focus(); return false;
                //}

                double T_SumAllowance = 0;
                if (radioB_A1.Checked == true)
                {
                    T_SumAllowance = Allowance2 + Allowance3 + Allowance4 + Allowance5 + Allowance6 + Allowance7 + Allowance8 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                }

                if (radioB_A2.Checked == true)
                {
                    T_SumAllowance = Allowance1 + Allowance3 + Allowance4 + Allowance5 + Allowance6 + Allowance7 + Allowance8 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                }


                if (radioB_A3.Checked == true)
                {
                    T_SumAllowance = Allowance2 + Allowance1 + Allowance4 + Allowance5 + Allowance6 + Allowance7 + Allowance8 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                }


                //if (radioB_A5.Checked == true)
                //{
                //    T_SumAllowance = Allowance2 + Allowance3 + Allowance4 + Allowance1 + Allowance6 + Allowance7 + Allowance8 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                //}

                //if (radioB_A6.Checked == true)
                //{
                //    T_SumAllowance = Allowance2 + Allowance3 + Allowance4 + Allowance5 + Allowance1 + Allowance7 + Allowance8 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                //}

                //if (radioB_A7.Checked == true)
                //{
                //    T_SumAllowance = Allowance2 + Allowance3 + Allowance4 + Allowance5 + Allowance6 + Allowance1 + Allowance8 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                //}

                //if (radioB_A8.Checked == true)
                //{
                //    T_SumAllowance = Allowance2 + Allowance3 + Allowance4 + Allowance5 + Allowance6 + Allowance7 + Allowance1 + double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                //}


                //기존 반품 처리된 금액이 이번에 새롭게 계산되는 금액보다 크다 그럼 - 공제내 그럼 안됨
                if (Cur_DedCut_Pay > T_SumAllowance)
                {
                    MessageBox.Show("회원의 적용일자의 마감에 반품공제액이 존재 합니다. 이 금액이 현재 적용될려는 금액보다 큽니다. -수당이 발생됩니다.  적용할 수 없습니다.");
                    txt_Pv.Focus(); return false;
                }

                T_SumAllowance = T_SumAllowance - Cur_DedCut_Pay;

                if (Cur_DedCut_Pay_DED > T_SumAllowance)
                {
                    MessageBox.Show("회원의 적용일자의 마감에 -기타수당처리내역이 존재 합니다. 이 금액이 현재 적용될려는 금액보다 큽니다. -수당이 발생됩니다.  적용할 수 없습니다.");
                    txt_Pv.Focus(); return false;
                }


            }
            //else
            //{
            //    MessageBox.Show("현 회원의 입력 날짜에 수당 정보가 존재 하지 않습니다. 확인후 다시 시도해 주십시요.");
            //    txtName.Focus(); return false;                
            //}
            //++++++++++++++++++++++++++++++++   

            //string SortKind2 = "0";

            //if (radioB_A1.Checked == true)
            //    SortKind2 = "1";

            //if (radioB_A2.Checked == true)
            //    SortKind2 = "2";

            //if (radioB_A3.Checked == true)
            //    SortKind2 = "3";

            //if (radioB_A4.Checked == true)
            //    SortKind2 = "4";

            ////if (radioB_A5.Checked == true)
            ////    SortKind2 = "5";

            ////if (radioB_A6.Checked == true)
            ////    SortKind2 = "6";

            ////if (radioB_A7.Checked == true)
            ////    SortKind2 = "7";

            ////if (radioB_A8.Checked == true)
            ////    SortKind2 = "8";

            //if (radioB_ETC.Checked == true)
            //    SortKind2 = "ETC";


            Tsql = "Select seq  ";
            Tsql = Tsql + " From tbl_Sham_Pay_Real (nolock) ";
            Tsql = Tsql + " Where  Apply_Date ='" + combo_C_Date.Text.Trim() + "'";
            Tsql = Tsql + " And    Mbid = '" + Mbid + "'";
            Tsql = Tsql + " And    Mbid2 = " + Mbid2;
            Tsql = Tsql + " And    SortKind2 = '" + SortKind2 + "'";
            Tsql = Tsql + " And    Clo_TF = 2 ";
            //++++++++++++++++++++++++++++++++

            DataSet ds4 = new DataSet();
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds4) == false) return false;
            int ReCnt4 = Temp_Connect.DataSet_ReCount;

            if (ReCnt4 > 0)  //실제로 존재하는 회원 번호 인가.
            {
                MessageBox.Show("현 회원의 입력 날짜에 동일 수당 내역으로 등록된 내역이 존재합니다. 확인후 다시 시도해 주십시요.");
                butt_Save.Focus(); return false;
            }
            //++++++++++++++++++++++++++++++++  

            return true;
        }



        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            string str_Q = ""; 

            if (txtKey.Text == "")
                str_Q = "Msg_Base_Save_Q";
            else
                {
                    MessageBox.Show("현 화면상에서 수정은 불가능 합니다. 기존 내역을 삭제 처리 하신후에 내역을 다시 저장해 주십시요.");
                    butt_Save.Focus();
                    return;
                }

            
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString(str_Q), "", MessageBoxButtons.YesNo) == DialogResult.No) return;
            int Not_Pay_FLAG = 0;
            if (Check_TextBox_Error(ref Not_Pay_FLAG) == false) return;



            ////if (txtKey.Text != "") //수정일 경우에는 수정 프로시져로 가고 이 프로시져를 빠져나가라
            ////{
            ////    Save_Base_Data_UpDate(ref Save_Error_Check);
            ////    if (Save_Error_Check > 0)
            ////        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            ////    return;
            ////}

            tableLayoutPanel13.Enabled = false;
            string SortKind2 = "0";

            if (radioB_A1.Checked == true)
                SortKind2 = "1";

            if (radioB_A2.Checked == true)
                SortKind2 = "2";

            if (radioB_A3.Checked == true)
                SortKind2 = "3";

            //if (radioB_A4.Checked == true)
            //    SortKind2 = "4";

            //if (radioB_A5.Checked == true)
            //    SortKind2 = "5";

            //if (radioB_A6.Checked == true)
            //    SortKind2 = "6";

            //if (radioB_A7.Checked == true)
            //    SortKind2 = "7";

            if (radioB_ETC.Checked == true)
                SortKind2 = "ETC";


            string StrSql = "";
            cls_Search_DB csd = new cls_Search_DB();
            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);
                        
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();
                       
            try
            {


                cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();

                StrSql = "Select Allowance1, Allowance2, Allowance3, Allowance4 , Allowance5, Allowance6, Allowance7 , Etc_Pay ,Cur_DedCut_Pay , SumAllAllowance, Mbid,Mbid2  ";
                StrSql = StrSql + " From tbl_ClosePay_02_Mod (nolock) ";
                StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
                StrSql = StrSql + " And   Mbid ='" + Mbid  + "'";
                StrSql = StrSql + " And   Mbid2 =" + Mbid2; 


                DataSet ds2 = new DataSet();
                Temp_Connect2.Open_Data_Set(StrSql, base_db_name, ds2);

                int RowCnt = Temp_Connect2.DataSet_ReCount;

                double Allowance1 = 0, Allowance2 = 0, Allowance3 = 0, Allowance4 = 0, Allowance5 = 0, Allowance6 = 0, Allowance7 = 0, Etc_Pay = 0, Cur_DedCut_Pay = 0, SumAllAllowance = 0;
                if (RowCnt > 0)
                {
                     Allowance1 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance1"].ToString());
                     Allowance2 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance2"].ToString());
                     Allowance3 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance3"].ToString());
                     Allowance4 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance4"].ToString());
                     Allowance5 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance5"].ToString());
                     Allowance6 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance6"].ToString());
                     Allowance7 = double.Parse(ds2.Tables[base_db_name].Rows[0]["Allowance7"].ToString());
                     Etc_Pay = double.Parse(ds2.Tables[base_db_name].Rows[0]["Etc_Pay"].ToString());                   
                }
                double app_Pv =   double.Parse(txt_Pv.Text.Trim().Replace(",", ""));
                double Base_Pv = SumAllAllowance;

                if (radioB_A1.Checked == true)
                    Base_Pv = Allowance1;

                if (radioB_A2.Checked == true)
                    Base_Pv = Allowance2;

                if (radioB_A3.Checked == true)
                    Base_Pv = Allowance3;

                //if (radioB_A4.Checked == true)
                //    Base_Pv = Allowance4;

                //if (radioB_A5.Checked == true)
                //    Base_Pv = Allowance5;

                //if (radioB_A6.Checked == true)
                //    Base_Pv = Allowance6;

                //if (radioB_A7.Checked == true)
                //    Base_Pv = Allowance7;

                if (radioB_ETC.Checked == true)
                    Base_Pv = Etc_Pay;


                //if (chk_B.Checked == false)
                //{
                //    StrSql = "INSERT INTO tbl_Sham_Pay_Real ";
                //    StrSql = StrSql + " (";
                //    StrSql = StrSql + "  mbid , mbid2 , M_Name ";
                //    StrSql = StrSql + " , Apply_Date,Base_Pv, Apply_PV  ";
                //    StrSql = StrSql + " , SortKind2 , ETC , Na_Code , Clo_TF ";
                //    StrSql = StrSql + " , RecordID, RecordTime ";
                //    StrSql = StrSql + " ) ";
                //    StrSql = StrSql + " Values ";
                //    StrSql = StrSql + " (";
                //    StrSql = StrSql + "'" + Mbid + "'";
                //    StrSql = StrSql + "," + Mbid2;
                //    StrSql = StrSql + ",'" + txtName.Text + "'";
                //    StrSql = StrSql + ",'" + combo_C_Date.Text.Trim() + "'";
                //    StrSql = StrSql + ",0 ";
                //    StrSql = StrSql + "," + app_Pv;

                //    StrSql = StrSql + ",'" + SortKind2 + "'";
                //    StrSql = StrSql + ",'" + txtRemark.Text.Trim() + "'";
                //    StrSql = StrSql + ",'" + combo_Se_Code.Text.Trim() + "'";
                //    StrSql = StrSql + ",2"; //월마감   4 번 기간 마감 2번을 쓴다.
                //    StrSql = StrSql + ",'" + cls_User.gid + "'";
                //    StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
                //    StrSql = StrSql + ")";

                //    Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);
                //}


                //if (Cur_DedCut_Pay > 0)
                //{
                //    Put_Sum_Return_Remain_Pay(Temp_Connect, Conn, tran,  Mbid2);
                //}


                StrSql = "INSERT INTO tbl_Sham_Pay_Real ";
                StrSql = StrSql + " (";
                StrSql = StrSql + "  mbid , mbid2 , M_Name ";
                StrSql = StrSql + " , Apply_Date,Base_Pv, Apply_PV  ";
                StrSql = StrSql + " , SortKind2 , ETC , Na_Code , Clo_TF ";
                StrSql = StrSql + " , RecordID, RecordTime ";
                StrSql = StrSql + " ) ";
                StrSql = StrSql + " Values ";
                StrSql = StrSql + " (";
                StrSql = StrSql + "'" + Mbid + "'";
                StrSql = StrSql + "," + Mbid2;
                StrSql = StrSql + ",'" + txtName.Text + "'";
                StrSql = StrSql + ",'" + combo_C_Date.Text.Trim() + "'";
                StrSql = StrSql + ", " + Base_Pv ;
                StrSql = StrSql + "," + app_Pv;

                StrSql = StrSql + ",'" + SortKind2 + "'";
                StrSql = StrSql + ",'" + txtRemark.Text.Trim() + "'";
                StrSql = StrSql + ",'" + combo_Se_Code.Text.Trim() + "'";
                StrSql = StrSql + ",2"; //월마감   4 번 기간 마감 2번을 쓴다.
                StrSql = StrSql + ",'" + cls_User.gid + "'";
                StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
                StrSql = StrSql + ")";

                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);


                Repate_Clsoe_Cul(Temp_Connect, Conn, tran, Mbid, Mbid2, app_Pv,Cur_DedCut_Pay, Not_Pay_FLAG,  RowCnt );

                


                tbl_CloseTotal_Put2(Temp_Connect, Conn, tran);
                tbl_CloseTotal_Put3(Temp_Connect, Conn, tran);

                      
                tran.Commit();

                Save_Error_Check = 1;                
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                tableLayoutPanel13.Enabled =true ;

            }
            catch (Exception ee)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));


                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ee.ToString());
                tableLayoutPanel13.Enabled = true;
            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
                tableLayoutPanel13.Enabled = true;
            }            

        }


        private void Put_Sum_Return_Remain_Pay(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, int Mbid2)
        {
            
            string StrSql = "";

            StrSql = "Update tbl_Sales_Put_Return_Pay SET ";
            StrSql = StrSql + " Return_Pay2 =  Return_Pay2 + Isnull(B.A1,0) ";
            StrSql = StrSql + " FROM  tbl_Sales_Put_Return_Pay  A, ";

            StrSql = StrSql + " (Select Sum(Return_Pay) A1 , Base_T_index";
            StrSql = StrSql + " From tbl_Sales_Put_Return_Pay  ";

            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And   C_mbid2 ='" + Mbid2 + "'";
            StrSql = StrSql + " And   C_Na_Code ='" + combo_Se_Code.Text.Trim() + "'";

            StrSql = StrSql + " And  Base_T_index > 0 ";
            StrSql = StrSql + " And  Cl_TF = 2 ";

            StrSql = StrSql + " Group by  Base_T_index";
            StrSql = StrSql + " ) B";
            StrSql = StrSql + " Where A.T_index = B.Base_T_index ";
            Temp_Connect.Insert_Data(StrSql, Conn, tran);


            StrSql = "Delete From tbl_Sales_Put_Return_Pay ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And   C_mbid2 ='" + Mbid2 + "'";
            StrSql = StrSql + " And   C_Na_Code ='" + combo_Se_Code.Text.Trim() + "'";
            StrSql = StrSql + " And  Base_T_index > 0 ";
            StrSql = StrSql + " And  Cl_TF = 2 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);



            StrSql = "Update tbl_ClosePay_02_Mod SET ";
            StrSql = StrSql + " Sum_Return_Take_Pay = ISNULL(B.A1, 0 )   ";
            StrSql = StrSql + " FROM  tbl_ClosePay_02_Mod  A, ";

            StrSql = StrSql + " (Select    Sum(Return_Pay) A1,  C_Mbid ,C_Mbid2 , C_Na_Code  ";
            StrSql = StrSql + " From tbl_Sales_Put_Return_Pay ";
            StrSql = StrSql + " Where   Return_Pay <> 0 ";
            StrSql = StrSql + " And     OrderNumber <> '' ";
            StrSql = StrSql + " And     C_mbid2 ='" + Mbid2 + "'";
            StrSql = StrSql + " And     C_Na_Code ='" + combo_Se_Code.Text.Trim() + "'";
            StrSql = StrSql + " Group By C_Mbid ,C_Mbid2 , C_Na_Code ";
            StrSql = StrSql + " ) B";

            StrSql = StrSql + " Where  A.ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    A.Mbid  = B.C_Mbid ";
            StrSql = StrSql + " And   A.Mbid2  = B.C_Mbid2 ";
            StrSql = StrSql + " And   A.Give_Na_Code =  B.C_Na_Code ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            


            // '''--여태 까지 발생한 총 반품으로 인해서 발생된 차감액을 구한ㄷ4ㅏ.
            StrSql = "Update tbl_ClosePay_02_Mod SET ";
            StrSql = StrSql + " Sum_Return_DedCut_Pay = ISNULL(B.A1, 0 )   ";
            StrSql = StrSql + " FROM  tbl_ClosePay_02_Mod  A, ";

            StrSql = StrSql + " (Select    Sum(Return_Pay) A1,  C_Mbid ,C_Mbid2 , C_Na_Code   ";
            StrSql = StrSql + " From tbl_Sales_Put_Return_Pay  ";
            StrSql = StrSql + " Where   Return_Pay <> 0 ";
            StrSql = StrSql + " And     OrderNumber = '' ";
            StrSql = StrSql + " And     C_mbid2 ='" + Mbid2 + "'";
            StrSql = StrSql + " And     C_Na_Code ='" + combo_Se_Code.Text.Trim() + "'";
            StrSql = StrSql + " Group By C_Mbid ,C_Mbid2 , C_Na_Code ";
            StrSql = StrSql + " ) B";

            StrSql = StrSql + " Where  A.ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    A.Mbid  = B.C_Mbid ";
            StrSql = StrSql + " And   A.Mbid2  = B.C_Mbid2 ";
            StrSql = StrSql + " And   A.Give_Na_Code =  B.C_Na_Code ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);

            
            StrSql = "Update tbl_ClosePay_02_Mod SET ";
            StrSql = StrSql + " Sum_Return_Remain_Pay = Sum_Return_Take_Pay - Sum_Return_DedCut_Pay ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And   mbid2 ='" + Mbid2 + "'";
            StrSql = StrSql + " And   Give_Na_Code ='" + combo_Se_Code.Text.Trim() + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);        
        }



        private void Repate_Clsoe_Cul(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string Mbid, int Mbid2
                                        , double app_Pv, double Cur_DedCut_Pay,int Not_Pay_FLAG ,  double RowCnt = 1)
        {
            string StrSql = "";

            StrSql = " Update tbl_ClosePay_02_Mod Set ";
            //StrSql = StrSql + " SumAllAllowance =  " + app_Pv;
            //StrSql = StrSql + ",InComeTax = 0 ";
            //StrSql = StrSql + ",ResidentTax = 0 ";
            //StrSql = StrSql + ",TruePayment = 0 ";

            if (radioB_A1.Checked == true)
                StrSql = StrSql + " Allowance1 =  " + app_Pv;

            else if (radioB_A2.Checked == true)
                StrSql = StrSql + " Allowance2 =  " + app_Pv;

            else if (radioB_A3.Checked == true)
                StrSql = StrSql + " Allowance3 =  " + app_Pv;

            //if (radioB_A4.Checked == true)
            //    StrSql = StrSql + " Allowance4 =  " + app_Pv;

            //if (radioB_A5.Checked == true)
            //    StrSql = StrSql + " Allowance5 =  " + app_Pv;

            //if (radioB_A6.Checked == true)
            //    StrSql = StrSql + " Allowance6 =  " + app_Pv;

            //if (radioB_A7.Checked == true)
            //    StrSql = StrSql + " Allowance7 =  " + app_Pv;

            else if(radioB_ETC.Checked == true)
                StrSql = StrSql + " Etc_Pay =  " + app_Pv;

            else
                StrSql = StrSql + " SumAllAllowance =  " + app_Pv;

            StrSql = StrSql + " Where  Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  Mbid2 = " + Mbid2; 
            StrSql = StrSql + " And  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            

            Temp_Connect.Insert_Data(StrSql, Conn, tran);


            StrSql = " Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SumAllAllowance = Allowance1 + Allowance2 + Allowance3  + Etc_Pay - Etc_Pay_DedCut - Cur_DedCut_Pay   - Cur_DedCut_Pay_DED + SumAllAllowance_10000 + SumAllAllowance_Be_Not   ";
            StrSql = StrSql + " ,InComeTax = 0 , ResidentTax = 0 , TruePayment = 0  ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And   Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   Mbid2 = " + Mbid2;

            Temp_Connect.Insert_Data(StrSql, Conn, tran);

            if (Not_Pay_FLAG == 1)
            {
                StrSql = "Update tbl_Close_Not_Pay SET ";
                StrSql = StrSql + " SumAllAllowance = ISNULL(B.A1, 0 )   ";
                StrSql = StrSql + " FROM  tbl_Close_Not_Pay  A, ";

                StrSql = StrSql + " (Select    SumAllAllowance  - SumAllAllowance_10000 - SumAllAllowance_Be_Not A1,  Mbid ,Mbid2   ";
                StrSql = StrSql + " From tbl_ClosePay_02_Mod (nolock) ";
                StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
                StrSql = StrSql + " And   Mbid = '" + Mbid + "'";
                StrSql = StrSql + " And   Mbid2 = " + Mbid2;
                StrSql = StrSql + " ) B";

                StrSql = StrSql + " Where  A.ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
                StrSql = StrSql + " And    A.Mbid  = B.Mbid ";
                StrSql = StrSql + " And    A.Mbid2  = B.Mbid2 ";
                StrSql = StrSql + " And    A.Close_FLAG = 'W'";
                StrSql = StrSql + " And    A.Check_FLAG = 'M'";


                Temp_Connect.Insert_Data(StrSql, Conn, tran);
            }




            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " InComeTax = Round((SumAllAllowance * 0.03) ,-1,1) ";
            //StrSql = StrSql + " Where SumAllAllowance  > 0 ";
            StrSql = StrSql + " Where   Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   Mbid2 = " + Mbid2;
            StrSql = StrSql + " And  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And  SumAllAllowance >= 33340 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " ResidentTax = Round((InComeTax * 0.1) ,-1,1)  ";
            //StrSql = StrSql + " Where SumAllAllowance  > 0 ";
            StrSql = StrSql + " Where   Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   Mbid2 = " + Mbid2;
            StrSql = StrSql + " And  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And  SumAllAllowance >= 33340 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);



            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " TruePayment = SumAllAllowance - InComeTax - ResidentTax ";
            StrSql = StrSql + " Where SumAllAllowance  > 0 ";
            StrSql = StrSql + " And   Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   Mbid2 = " + Mbid2;
            StrSql = StrSql + " And  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " InComeTax = 0, ResidentTax = 0, TruePayment = 0 ";
            StrSql = StrSql + " Where  (Cpno = ''  OR BankAcc = '') ";
            StrSql = StrSql + " And    SumAllAllowance > 0 ";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And    Mbid2 = " + Mbid2;
            StrSql = StrSql + " And  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);

        }




        private void Chang_RetunPay_Table(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, int T_Mbid2)
        {
       
            string StrSql = "";
            cls_Connect_DB Search_Connect = new cls_Connect_DB();
            SqlConnection Search_Conn = null;

            int t_qu_Cnt = 0;
            Dictionary<int, string> t_qu = new Dictionary<int, string>();

            StrSql = " Select Cur_DedCut_Pay,tbl_ClosePay_02_Mod.Mbid,tbl_ClosePay_02_Mod.Mbid2 , tbl_Memberinfo.M_Name , tbl_ClosePay_02_Mod.Give_Na_Code ";
            StrSql = StrSql + " From tbl_ClosePay_02_Mod   (nolock)   ";
            StrSql = StrSql + " LEFT JOin tbl_Memberinfo (nolock) on tbl_Memberinfo.Mbid = tbl_ClosePay_02_Mod.Mbid And tbl_Memberinfo.Mbid2 = tbl_ClosePay_02_Mod.Mbid2  ";
            StrSql = StrSql + " WHERE Cur_DedCut_Pay > 0 ";
            StrSql = StrSql + " And  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid2 = '" + T_Mbid2 + "'";
            StrSql = StrSql + " And   Give_Na_Code ='" + combo_Se_Code.Text.Trim() + "'";

            int ReCnt = 0;
            SqlDataReader sr = null;
            Temp_Connect.Open_Data_Set(StrSql, Conn, tran, ref sr);
            ReCnt = Temp_Connect.DataSet_ReCount;

         
            int Mbid2 = 0, Top_SW = 0, TSw = 0, T_index = 0;
            double Cut_Pay = 0, RR_Cut_Pay = 0, T_Pay = 0;
            string Mbid = "", Re_BaseOrderNumber = "", M_Name = "", Give_Na_Code = "";


            //for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            while (sr.Read())
            {
                Cut_Pay = double.Parse(sr.GetValue(0).ToString());
                Mbid = sr.GetValue(1).ToString();
                Mbid2 = int.Parse(sr.GetValue(2).ToString());
                M_Name = sr.GetValue(3).ToString();
                Give_Na_Code = sr.GetValue(4).ToString();

                StrSql = "Select Return_Pay2,C_Mbid,C_Mbid2, T_index , Re_BaseOrderNumber ";
                StrSql = StrSql + " From tbl_Sales_Put_Return_Pay (nolock) ";
                StrSql = StrSql + " WHERE C_Mbid = '" + Mbid + "'";
                StrSql = StrSql + " And   C_Mbid2 = " + Mbid2;
                StrSql = StrSql + " And   Return_Pay2 > 0 ";
                StrSql = StrSql + " And   Base_OrderNumber = '' ";
                StrSql = StrSql + " And   C_Na_Code ='" + Give_Na_Code + "'";

                DataSet ds2 = new DataSet();
                Search_Connect.Open_Data_Set(StrSql, base_db_name, Search_Conn, ds2);
                int ReCnt2 = Search_Connect.DataSet_ReCount;

                if (ReCnt2 > 0)
                {
                    Top_SW = 0;
                    int fi_cnt2 = 0;
                    //t_qu.Clear();

                    while ((fi_cnt2 <= ReCnt2 - 1) && Top_SW == 0)
                    {
                        T_Pay = double.Parse(ds2.Tables[base_db_name].Rows[fi_cnt2]["Return_Pay2"].ToString());
                        Re_BaseOrderNumber = ds2.Tables[base_db_name].Rows[fi_cnt2]["Re_BaseOrderNumber"].ToString();
                        T_index = int.Parse(ds2.Tables[base_db_name].Rows[fi_cnt2]["T_index"].ToString());

                        TSw = 0;

                        while (Cut_Pay > 0 && TSw == 0)
                        {
                            RR_Cut_Pay = 0;

                            if (Cut_Pay > T_Pay)
                            {
                                RR_Cut_Pay = T_Pay;
                                Cut_Pay = Cut_Pay - T_Pay;
                                T_Pay = 0;
                                TSw = 1;
                            }
                            else
                            {
                                RR_Cut_Pay = Cut_Pay;
                                Cut_Pay = 0;
                            }

                            if (RR_Cut_Pay > 0)
                            {
                                StrSql = "Insert into tbl_Sales_Put_Return_Pay (ToEndDate, C_mbid,C_mbid2 , C_M_Name , Return_Pay , Base_OrderNumber , Base_T_index , Cl_TF , C_Na_Code ) ";
                                StrSql = StrSql + " Values (";
                                StrSql = StrSql + " '" + combo_C_Date.Text.Trim() + "','" + Mbid + "', " + Mbid2;
                                StrSql = StrSql + " , '" + M_Name + "', " + RR_Cut_Pay + ",";
                                StrSql = StrSql + "'" + Re_BaseOrderNumber + "'";
                                StrSql = StrSql + "," + T_index + ",2";
                                StrSql = StrSql + ",'" + Give_Na_Code + "'";
                                StrSql = StrSql + ")";
                                                               
                                t_qu[t_qu_Cnt] = StrSql;
                                t_qu_Cnt++;


                                StrSql = "Update tbl_Sales_Put_Return_Pay SET ";
                                StrSql = StrSql + " Return_Pay2 = Return_Pay2 -" + RR_Cut_Pay;
                                StrSql = StrSql + " Where   T_index  = " + T_index;
                                StrSql = StrSql + " And   C_Mbid = '" + Mbid + "'";
                                StrSql = StrSql + " And   C_Mbid2 = " + Mbid2;

                                t_qu[t_qu_Cnt] = StrSql;
                                t_qu_Cnt++;
                            }

                        }

                        if (Cut_Pay == 0) Top_SW = 1;
                        fi_cnt2++;
                    }

                }

            }
            sr.Close(); sr.Dispose();
            
            foreach (int tkey in t_qu.Keys)
            {
                StrSql = t_qu[tkey];
                Temp_Connect.Insert_Data(StrSql, Conn, tran);               
            }

        }



        private void Caul_SeMu_pay(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string Mbid)
        {
         
            string StrSql = "";

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 0 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 10000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   DayPV01 + DayPV02  > 0 ";
            StrSql = StrSql + " And   DayPV01 + DayPV02  <= 4100000 ";
            StrSql = StrSql + " And   BeforeGrade = 10 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
         


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 30000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   DayPV01 + DayPV02  > 4100000 ";
            StrSql = StrSql + " And   DayPV01 + DayPV02  <= 18900000 ";
            StrSql = StrSql + " And   BeforeGrade = 10 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 50000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  DayPV01 + DayPV02  > 18900000 ";
            StrSql = StrSql + " And   DayPV01 + DayPV02  <= 39900000 ";
            StrSql = StrSql + " And   BeforeGrade = 10 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
       

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 70000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  DayPV01 + DayPV02  > 39900000 ";
            StrSql = StrSql + " And   BeforeGrade = 10 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            

            //////////////////////////////-----------------------------------
            //////////////////////////////-----------------------------------
            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 10000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   DayPV01 + DayPV02  > 0 ";
            StrSql = StrSql + " And   DayPV01 + DayPV02  <= 4100000 ";
            StrSql = StrSql + " And   BeforeGrade >= 20 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 30000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And   DayPV01 + DayPV02  > 4100000 ";
            StrSql = StrSql + " And   DayPV01 + DayPV02  <= 16800000 ";
            StrSql = StrSql + " And   BeforeGrade >= 20 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);            

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 50000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  DayPV01 + DayPV02  > 16800000 ";
            StrSql = StrSql + " And   DayPV01 + DayPV02  <= 33600000 ";
            StrSql = StrSql + " And   BeforeGrade >= 20 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 70000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  DayPV01 + DayPV02  > 33600000 ";
            StrSql = StrSql + " And   BeforeGrade >= 20 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            

            //////////////////////////////-----------------------------------
            //////////////////////////////-----------------------------------
            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = SeMu_pay + 10000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And    SumAllAllowance  > 0 ";
            StrSql = StrSql + " And   SumAllAllowance  < 3000000 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = SeMu_pay + 30000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  SumAllAllowance  >= 3000000 ";
            StrSql = StrSql + " And   SumAllAllowance  < 7000000 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            
            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = SeMu_pay + 50000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And  SumAllAllowance  >= 7000000 ";
            StrSql = StrSql + " And   SumAllAllowance  < 13000000 ";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = SeMu_pay + 70000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And    SumAllAllowance  >= 13000000 ";
            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            


            //////////////////////////////-----------------------------------
            //////////////////////////////-----------------------------------
            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " SeMu_pay = 70000 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And    SeMu_pay > 70000 ";
            
            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            

            //////////////////////////////-----------------------------------
            //////////////////////////////-----------------------------------

            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " TruePayment = 0 ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + " TruePayment = SumAllAllowance  + SumTax - SeMu_pay  ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And    SumAllAllowance  + SumTax - SeMu_pay  > 0";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);
            


            StrSql = "Update tbl_ClosePay_02_Mod Set ";
            StrSql = StrSql + "  TruePayment = 0  ";
            StrSql = StrSql + " ,SeMu_pay = SeMu_pay - (SumAllAllowance  + SumTax)  ";
            StrSql = StrSql + " Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And    Mbid = '" + Mbid + "'";
            StrSql = StrSql + " And SumAllAllowance  + SumTax - SeMu_pay  <=  0";
            StrSql = StrSql + " And SeMu_pay > 0 ";
            StrSql = StrSql + " And SumAllAllowance  + SumTax > 0 ";


            Temp_Connect.Insert_Data(StrSql, Conn, tran);            
        }




        private void Save_Base_Data_UpDate(ref int Save_Error_Check)
        {                      
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();


            try
            {

                string StrSql = "";
                double app_Pv =   double.Parse(txt_Pv.Text.Trim().Replace(",", ""));

                StrSql = "INSERT INTO tbl_Sham_Pay_Real_Mod ";
                StrSql = StrSql + " Select  * ";
                StrSql = StrSql + ",'" + cls_User.gid + "'";
                StrSql = StrSql + ",Convert(Varchar(25),GetDate(),21) ";
                StrSql = StrSql + " From tbl_Sham_Pay_Real ";
                StrSql = StrSql + " Where seq = " + txtKey.Text; 

                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);

                StrSql = "UpDate  tbl_Sham_Pay_Real Set ";
                StrSql = StrSql + "  Apply_Pv = " + app_Pv;
                StrSql = StrSql + ", Apply_Date = '" + combo_C_Date.Text.Trim () + "'";                
                StrSql = StrSql + ", Etc = '" + txtRemark.Text.Trim() + "'";                
                StrSql = StrSql + " Where seq = " + txtKey.Text; 

                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);                 
               

                tran.Commit();

                Save_Error_Check = 1;                                
            }
            catch (Exception)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));
            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }


        }






        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Delete_Base_Data(ref int Delete_Error_Check)
        {
            Delete_Error_Check = 0;
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (txtKey.Text.Trim() == "")
            {
                butt_Delete.Focus();
                return;
            }

            ////마감정산이 이루어진 판매 날짜인지 체크한다.                
            //cls_Search_DB csd = new cls_Search_DB();
            //if (csd.Close_Check_SellDate("tbl_CloseTotal_02", combo_C_Date.Text.Trim ()) == false)
            //{
            //    mtxtSellDate.Focus(); return ;
            //}

            tableLayoutPanel13.Enabled = false;

            cls_Search_DB csd = new cls_Search_DB();
            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

           

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string StrSql = "";


                cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();

                StrSql = "Select Base_Pv, Mbid,Mbid2  ";
                StrSql = StrSql + " From tbl_Sham_Pay_Real (nolock) ";
                StrSql = StrSql + " Where seq = " + txtKey.Text;
                

                DataSet ds2 = new DataSet();
                Temp_Connect2.Open_Data_Set(StrSql, base_db_name, ds2);

                double Base_Pv = double.Parse(ds2.Tables[base_db_name].Rows[0]["Base_Pv"].ToString());
                
                

                StrSql = "Insert into  tbl_Sham_Pay_Real_Mod ";
                StrSql = StrSql + " Select *  ";
                StrSql = StrSql + ",'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21) From tbl_Sham_Pay_Real ";
                StrSql = StrSql + " Where seq = " + txtKey.Text;
                
                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);


                StrSql = "Delete From tbl_Sham_Pay_Real  ";
                StrSql = StrSql + " Where seq = " + txtKey.Text;

                Temp_Connect.Update_Data(StrSql, Conn, tran);

                Repate_Clsoe_Cul(Temp_Connect, Conn, tran, Mbid, Mbid2, Base_Pv, Base_Pv,1);

                tbl_CloseTotal_Put2(Temp_Connect, Conn, tran);
                tbl_CloseTotal_Put3(Temp_Connect, Conn, tran);

                //Caul_SeMu_pay(Temp_Connect, Conn, tran, Mbid);                

                tran.Commit();
                Delete_Error_Check = 1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del"));
                tableLayoutPanel13.Enabled =true ;
            }
            catch (Exception)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Err"));
                tableLayoutPanel13.Enabled = true;

            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
                tableLayoutPanel13.Enabled = true;
            }

        }





        //string[] g_HeaderText = {"입고번호"  , "입고일자"   , "상품코드"  , "상품명"   , "입고지"        
        //                        , "입고수량"   , "입고자"    , "비고"   , ""    , ""                                
        //                            };
        private void dGridView_Base_Sub_DoubleClick(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(panel9, mtxtMbid);      

            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[1].Value != null))
            {
                string T_Mbid = ""; string T_M_Name = ""; string T_SellDate = "";
                double T_PV = 0; string T_SellTypeName = ""; string T_ETC = "";
                int t_Seq = 0; string T_SortKind2 = "";

                Data_Set_Form_TF = 1; 
                T_Mbid = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                T_M_Name = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                T_SellDate = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                T_PV = double.Parse((sender as DataGridView).CurrentRow.Cells[3].Value.ToString());
                T_SellTypeName = (sender as DataGridView).CurrentRow.Cells[4].Value.ToString();
                T_ETC = (sender as DataGridView).CurrentRow.Cells[5].Value.ToString();
                t_Seq = int.Parse((sender as DataGridView).CurrentRow.Cells[8].Value.ToString());
                T_SortKind2 = (sender as DataGridView).CurrentRow.Cells[9].Value.ToString();
                combo_Se_Code.Text  = (sender as DataGridView).CurrentRow.Cells[11].Value.ToString();

                combo_Se.SelectedIndex = combo_Se_Code.SelectedIndex; 

                txtKey.Text = t_Seq.ToString();

                mtxtMbid.Text = T_Mbid;
                txtName.Text = T_M_Name;
                mtxtSellDate.Text = T_SellDate.Replace("-", "");

                //combo_Pay.Text = T_SellTypeName;
                //combo_Pay_Code.Text = T_SortKind2;

                if (T_SortKind2 == "1")
                    radioB_A1.Checked = true;

                if (T_SortKind2 == "2")
                    radioB_A2.Checked = true;

                if (T_SortKind2 == "3")
                    radioB_A3.Checked = true;

                if (T_SortKind2 == "4")
                    radioB_A4.Checked = true;

                //if (T_SortKind2 == "5")
                //    radioB_A5.Checked = true;

                //if (T_SortKind2 == "6")
                //    radioB_A6.Checked = true;


                //if (T_SortKind2 == "7")
                //    radioB_A7.Checked = true;

                if (T_SortKind2 == "ETC")
                    radioB_ETC.Checked = true;

                txt_Pv.Text = string.Format(cls_app_static_var.str_Currency_Type, T_PV);

                combo_C_Date.Text = T_SellDate.Replace("-", "");

                txtRemark.Text = T_ETC;

                mtxtMbid.ReadOnly = true;
                txtName.ReadOnly = true;
                mtxtMbid.BorderStyle = BorderStyle.FixedSingle;
                txtName.BorderStyle = BorderStyle.FixedSingle;
                mtxtMbid.BackColor = cls_app_static_var.txt_Enable_Color;
                txtName.BackColor = cls_app_static_var.txt_Enable_Color;

                tableLayoutPanel5.Enabled = false; //마감 적용일자를 수정 못하게 해버린다...
                tableLayoutPanel6.Enabled = false; //마감 적용일자를 수정 못하게 해버린다...
                tableLayoutPanel15.Enabled = false; //마감 적용일자를 수정 못하게 해버린다...
                tableLayoutPanel13.Enabled = false;
                tableLayoutPanel14.Enabled = false;
                tab_Nation.Enabled = false;

                //////마감정산이 이루어진 판매 날짜인지 체크한다.                
                ////cls_Search_DB csd = new cls_Search_DB();
                ////if (csd.Close_Check_SellDate("tbl_CloseTotal_02", combo_C_Date.Text.Trim ()) == false)
                ////{
                ////    mtxtSellDate.ReadOnly = true;
                ////    mtxtSellDate.BorderStyle = BorderStyle.FixedSingle;
                ////    mtxtSellDate.BackColor = cls_app_static_var.txt_Enable_Color;
                ////    DTP_SellDate.Visible = false;
                ////}

                Data_Set_Form_TF = 0;
                mtxtSellDate.Focus();
            }
        }





        private Boolean Input_Error_Check(MaskedTextBox m_tb, int s_Kind)
        {
            string T_Mbid = m_tb.Text;
            string Mbid = ""; int Mbid2 = 0;

            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == -1) //올바르게 회원번호 양식에 맞춰서 입력햇는가.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                m_tb.Focus(); return false;
            }

            string Tsql = "";
            Tsql = "Select Mbid , Mbid2, M_Name ";
            Tsql = Tsql + " , LineCnt , N_LineCnt  ";
            Tsql = Tsql + " , LeaveDate , LineUserDate  ";
            Tsql = Tsql + " , Saveid  , Saveid2  ";
            Tsql = Tsql + " , Nominid , Nominid2  ";
            Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
  

            if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 > 0)
            {
                Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
            }

            if (cls_app_static_var.Member_Number_1 == 0 && cls_app_static_var.Member_Number_2 > 0)
            {
                Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
            }

            if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 == 0)
            {
                Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid.ToString() + "'";
            }

            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)  //실제로 존재하는 회원 번호 인가.
            {

                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                m_tb.Focus(); return false;
            }
            //++++++++++++++++++++++++++++++++   


            return true;
        }







        private void Set_Form_Date(string T_Mbid)
        {
            string Mbid = ""; int Mbid2 = 0;
            Data_Set_Form_TF = 1;
            idx_Na_Code = "";

            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
            {
                string Tsql = "";
                Tsql = "Select  ";
                if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                else if (cls_app_static_var.Member_Number_1 == 0 && cls_app_static_var.Member_Number_2 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";
                else if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 == 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid AS M_Mbid ";

                Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";

                Tsql = Tsql + ",  tbl_Memberinfo.Cpno ";

                Tsql = Tsql + " , tbl_Memberinfo.LineCnt ";

                Tsql = Tsql + " , LEFT(tbl_Memberinfo.RegTime,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.RegTime,4),2) + '-' + RIGHT(tbl_Memberinfo.RegTime,2)  AS RegTime  ";

                Tsql = Tsql + "  , Add_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.hptel ";
                Tsql = Tsql + " , tbl_Memberinfo.hometel ";
                Tsql = Tsql + " , tbl_Memberinfo.address1 ";
                Tsql = Tsql + " , tbl_Memberinfo.address2 ";
                Tsql = Tsql + " , tbl_Memberinfo.Addcode1 ";
                Tsql = Tsql + " , tbl_Memberinfo.Na_code ";

                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";


                if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 > 0)
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }

                if (cls_app_static_var.Member_Number_1 == 0 && cls_app_static_var.Member_Number_2 > 0)
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }

                if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 == 0)
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid.ToString() + "'";
                }

                //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";

                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                //++++++++++++++++++++++++++++++++

                mtxtMbid.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
                txtName.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
                idx_Na_Code = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();        
            }

            Data_Set_Form_TF = 0;



        }

        private void radioB_R_Base_Click(object sender, EventArgs e)
        {
            //RadioButton _Rb = (RadioButton)sender;
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtSellDate2, mtxtSellDate3, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }

        private void radioB_S_Base_Click(object sender, EventArgs e)
        {
            Data_Set_Form_TF = 1;
            cls_form_Meth ct = new cls_form_Meth();
            ct.Search_Date_TextBox_Put(mtxtMakeDate1, mtxtMakeDate2, (RadioButton)sender);
            Data_Set_Form_TF = 0;
        }




        private void S_MtxtData_KeyPress(object sender, KeyPressEventArgs e)
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

        private void S_MtxtMbid_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            MaskedTextBox tb = (MaskedTextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void combo_Se_SelectedIndexChanged(object sender, EventArgs e)
        {
            combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex; 
            Put_Pay_C_Date();
        }









        private void tbl_CloseTotal_Put2(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran)
        {

            string StrSql = "";

            //string Give_Na_Code = "";
            //if (radioB_A3.Checked == true || radioB_A4.Checked == true || radioB_A5.Checked == true)
            //{
            //    if (combo_Se_Code.Text.Trim() == "JP")
            //        Give_Na_Code = combo_Se_Code.Text.Trim();
            //    else
            //        Give_Na_Code = combo_Se_Code.Text.Trim() + "_1";
            //}
            //else
            //    Give_Na_Code = combo_Se_Code.Text.Trim();



            StrSql = "Update tbl_CloseTotal_02 SET ";
            //StrSql = StrSql + "  Allowance1 =ISNULL(B.A1,0) ";
            //StrSql = StrSql + " ,Allowance2 =ISNULL(B.A2,0) ";
            //StrSql = StrSql + " ,Allowance3 =ISNULL(B.A3,0) ";
            //StrSql = StrSql + " ,Allowance4 =ISNULL(B.A4,0) ";
            //StrSql = StrSql + " ,Allowance5 =ISNULL(B.A5,0) ";
            //StrSql = StrSql + " ,Allowance6 =ISNULL(B.A6,0) ";
            //StrSql = StrSql + " ,Allowance7 =ISNULL(B.A7,0) ";
            //StrSql = StrSql + " ,Allowance8 =ISNULL(B.A8,0) ";
            //StrSql = StrSql + " ,Allowance9 =ISNULL(B.A9,0) ";
            //StrSql = StrSql + " ,Allowance10 =ISNULL(B.A10,0) ";

            ////StrSql = StrSql + " ,Allowance11 =ISNULL(B.A11,0) ";
            ////StrSql = StrSql + " ,Allowance12 =ISNULL(B.A12,0) ";
            ////StrSql = StrSql + " ,Allowance13 =ISNULL(B.A13,0) ";
            ////StrSql = StrSql + " ,Allowance14 =ISNULL(B.A14,0) ";
            ////StrSql = StrSql + " ,Allowance15 =ISNULL(B.A15,0) ";

            //StrSql = StrSql + " ,Allowance29 =ISNULL(B.A29,0) ";  //반품공제
            //StrSql = StrSql + " ,Allowance30 =ISNULL(B.A30,0) ";  //기타보너스

            StrSql = StrSql + " SumAllowance=ISNULL(B.AS1,0) ";
            StrSql = StrSql + " ,SumInComeTax=ISNULL(B.AS2,0) ";
            StrSql = StrSql + " ,SumResidentTax=ISNULL(B.AS3,0) ";
            StrSql = StrSql + " ,SumTruePayment=ISNULL(B.AS4,0) ";

            StrSql = StrSql + " FROM  tbl_CloseTotal_02  A, ";

            StrSql = StrSql + " (Select ";
            StrSql = StrSql + " Sum(convert(float,Allowance1)) AS A1 ,Sum(convert(float,Allowance2)) AS A2 ";
            StrSql = StrSql + ",Sum(convert(float,Allowance3)) AS A3 ,Sum(convert(float,Allowance4)) AS A4 ";
            StrSql = StrSql + ",Sum(convert(float,Allowance5)) AS A5 ,Sum(convert(float,Allowance6)) AS A6";
            StrSql = StrSql + ",Sum(convert(float,Allowance7)) AS A7 ,Sum(convert(float,Allowance8)) AS A8 ";
            StrSql = StrSql + ",Sum(convert(float,Allowance9)) AS A9 ,Sum(convert(float,Allowance10)) AS A10 ";


            ////StrSql = StrSql + ",Sum(convert(float,Allowance1_cut)) AS A28";
            StrSql = StrSql + ",Sum(convert(float,Cur_DedCut_Pay)) AS A29";


            StrSql = StrSql + ",Sum(convert(float,SumAllAllowance)) AS AS1,Sum(convert(float,InComeTax)) AS AS2 ";
            StrSql = StrSql + ",Sum(convert(float,ResidentTax)) AS AS3,Sum(convert(float,TruePayment)) AS AS4 ";
            StrSql = StrSql + " From tbl_ClosePay_02_Mod (nolock) ";
            StrSql = StrSql + " Where ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " ) B";
            StrSql = StrSql + " Where A.ToEndDate ='" + combo_C_Date.Text.Trim() + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);


        }




        private void tbl_CloseTotal_Put3(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran)
        {

            string StrSql = "";

            //string Give_Na_Code = "";
            //if (radioB_A3.Checked == true || radioB_A4.Checked == true || radioB_A5.Checked == true)
            //{
            //    if (combo_Se_Code.Text.Trim() == "JP")
            //        Give_Na_Code = combo_Se_Code.Text.Trim();
            //    else
            //        Give_Na_Code = combo_Se_Code.Text.Trim() + "_1";
            //}
            //else
            //    Give_Na_Code = combo_Se_Code.Text.Trim();

            StrSql = "Update tbl_CloseTotal_02 Set ";
            //StrSql = StrSql + "  Allowance1Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And  ToEndDate ='" + combo_C_Date.Text.Trim() + "' And   Allowance1 > 0),0) ";
            //StrSql = StrSql + " ,Allowance2Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance2 > 0),0) ";
            //StrSql = StrSql + " ,Allowance3Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance3 > 0),0) ";
            //StrSql = StrSql + " ,Allowance4Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance4 > 0),0) ";
            //StrSql = StrSql + " ,Allowance5Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance5 > 0),0) ";
            //StrSql = StrSql + " ,Allowance6Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance6 > 0),0) ";
            //StrSql = StrSql + " ,Allowance7Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance7> 0),0) ";
            //StrSql = StrSql + " ,Allowance8Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance8 > 0),0) ";
            //StrSql = StrSql + " ,Allowance9Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance9 > 0),0) ";
            //StrSql = StrSql + " ,Allowance10Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod (nolock)  Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Allowance10 > 0),0) ";

            //StrSql = StrSql + " ,Allowance11Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  K_Allowance1 > 0),0) ";
            //StrSql = StrSql + " ,Allowance12Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  K_Allowance2 > 0),0) ";
            //StrSql = StrSql + " ,Allowance13Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  K_Allowance3 > 0),0) ";
            //StrSql = StrSql + " ,Allowance14Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  K_Allowance4 > 0),0) ";
            //StrSql = StrSql + " ,Allowance15Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  K_Allowance5 > 0),0) ";
            ////'    StrSql = StrSql + " ,Allowance16Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance16 > 0),0) "  ;
            ////'    StrSql = StrSql + " ,Allowance17Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance17> 0),0) "  ;
            ////'    StrSql = StrSql + " ,Allowance18Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance18 > 0),0) "  ;
            //////'    StrSql = StrSql + " ,Allowance19Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance19 > 0),0) "  ;
            ////'    StrSql = StrSql + " ,Allowance20Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance20 > 0),0) "  ;
            ////'
            ////    StrSql = StrSql + " ,Allowance21Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance21 > 0),0) "  ;
            ////    StrSql = StrSql + " ,Allowance22Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance22 > 0),0) "  ;
            ////    StrSql = StrSql + " ,Allowance23Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance23 > 0),0) "  ;
            ////    StrSql = StrSql + " ,Allowance24Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance24 > 0),0) "  ;
            ////    StrSql = StrSql + " ,Allowance25Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance25 > 0),0) "  ;
            //    StrSql = StrSql + " ,Allowance26Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance26 > 0),0) "  ;
            //    StrSql = StrSql + " ,Allowance27Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance27> 0),0) "  ;
            ////StrSql = StrSql + " ,Allowance28Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod Where  Na_Code =  Give_Na_Code And  Allowance1_cut > 0),0) ";
            //StrSql = StrSql + " ,Allowance29Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Cur_DedCut_Pay > 0),0) ";
            //StrSql = StrSql + " ,Allowance30Cnt = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  Na_Code =  Give_Na_Code And   ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  Etc_Pay > 0),0) ";
            StrSql = StrSql + " SumAllowanceCount = ISNULL((Select Count(Mbid) From tbl_ClosePay_02_Mod  (nolock) Where  ToEndDate ='" + combo_C_Date.Text.Trim() + "' And  SumAllAllowance > 0),0) ";

            StrSql = StrSql + " Where ToEndDate ='" + combo_C_Date.Text.Trim() + "'";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);



            StrSql = "Update tbl_CloseTotal_02 Set ";
            //StrSql = StrSql + "  Allowance1Rate = (Allowance1 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance2Rate = (Allowance2 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance3Rate = (Allowance3 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance4Rate = (Allowance4 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance5Rate = (Allowance5 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance6Rate = (Allowance6 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance7Rate = (Allowance7 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance8Rate = (Allowance8 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance9Rate = (Allowance9 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance10Rate = (Allowance10 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";

            //StrSql = StrSql + " ,Allowance11Rate = (Allowance11 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance12Rate = (Allowance12 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance13Rate = (Allowance13 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance14Rate = (Allowance14 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance15Rate = (Allowance15 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            ////'    StrSql = StrSql + " ,Allowance16Rate = (Allowance16 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            ////'    StrSql = StrSql + " ,Allowance17Rate = (Allowance17 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            ////'    StrSql = StrSql + " ,Allowance18Rate = (Allowance18 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //////    StrSql = StrSql + " ,Allowance19Rate = (Allowance19 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            ////'    StrSql = StrSql + " ,Allowance20Rate = (Allowance20 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //'    StrSql = StrSql + " ,Allowance21Rate = (Allowance21 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //'    StrSql = StrSql + " ,Allowance22Rate = (Allowance22 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //'    StrSql = StrSql + " ,Allowance23Rate = (Allowance23 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //'    StrSql = StrSql + " ,Allowance24Rate = (Allowance24 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //'    StrSql = StrSql + " ,Allowance25Rate = (Allowance25 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //    StrSql = StrSql + " ,Allowance26Rate = (Allowance26 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //    StrSql = StrSql + " ,Allowance27Rate = (Allowance27 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;

            ////StrSql = StrSql + " ,Allowance28Rate = (Allowance28 /(TotalSellAmount-TotalReturnSellAmount)) * 100  "  ;
            //StrSql = StrSql + " ,Allowance29Rate = (Allowance29 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";
            //StrSql = StrSql + " ,Allowance30Rate = (Allowance30 /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";

            StrSql = StrSql + " SumAllowanceRate = (SumAllowance /(TotalSellAmount-TotalReturnSellAmount)) * 100  ";

            StrSql = StrSql + " Where ToEndDate ='" + combo_C_Date.Text.Trim() + "'";
            StrSql = StrSql + " And (TotalSellAmount-TotalReturnSellAmount) > 0";

            Temp_Connect.Insert_Data(StrSql, Conn, tran);



        }






    }
}
