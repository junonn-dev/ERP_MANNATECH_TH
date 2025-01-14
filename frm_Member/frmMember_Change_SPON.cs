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
    public partial class frmMember_Change_SPON : Form
    {
       
        
        private string base_db_name = "frmMember_Change_SPON";
        private int Data_Set_Form_TF = 0;
        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_Excel = new cls_Grid_Base();
        cls_Grid_Base cgb_Login = new cls_Grid_Base();

        Dictionary<string, cls_tbl_User> dic_tbl_User = new Dictionary<string, cls_tbl_User>();  //사용자 관련 정보를 클래스를 통해서. 넣는다.

        Dictionary<string, TreeNode> dic_Tree_Sort_1 = new Dictionary<string, TreeNode>();  //상품 코드 분류상 대분류 관련 트리노드를 답는곳
        Dictionary<string, TreeNode> dic_Tree_Sort_2 = new Dictionary<string, TreeNode>();  //상품 코드 분류상 중분류 관려련 트리 노드를 답는곳

        Dictionary<string, TreeView> dic_Tree_view = new Dictionary<string, TreeView>();  //상품 코드 분류상 대분류 관련 트리노드를 답는곳

        //Dictionary<string, Boolean> Main_Menu = new Dictionary<string, Boolean>();

        //public delegate void Send_MainMenu_Info_Dele(ref Dictionary<string, Boolean> Main_Menu);
        //public event Send_MainMenu_Info_Dele Send_MainMenu_Info;


        private int User_Select_Current_Row = 0;


        public frmMember_Change_SPON()
        {
            InitializeComponent();
        }




        private void frm_Base_Activated(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;

            cls_Pro_Base_Function cpbf = new cls_Pro_Base_Function();
            cpbf.Put_NaCode_ComboBox(combo_Se, combo_Se_Code);


            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            User_Select_Current_Row = -1; 
            
            //dGridView_Login_Header_Reset();
            //cgb_Login.d_Grid_view_Header_Reset();

            //dGridView_Excel_Header_Reset();
            //cgb_Excel.d_Grid_view_Header_Reset();


            //Send_MainMenu_Info(ref  Main_Menu);

            //trv_Item_Set_Sort_Code();

            if (dic_tbl_User != null)
                dic_tbl_User.Clear();


            if (dic_tbl_User != null)
                Base_Grid_Set();

            if (cls_app_static_var.Program_User_Center_Sort == 0) //센타 프로그램을 사용하지 못하면.  소속 센타 관련해서 보여주지 않는다.
                tableLayoutPanel21.Visible = false;

            mtxtTel_Dir.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtRegDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtLeaveDate.Mask = cls_app_static_var.Date_Number_Fromat;

            txtD1.BackColor = cls_app_static_var.txt_Enable_Color;
            txtD2.BackColor = cls_app_static_var.txt_Enable_Color;

            radioB_User_FLAG_M.Checked = true; 

        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            //butt_Exit.Left = this.Width - butt_Exit.Width - 20;

            //butt_Clear.Left = 3;
            //butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            ////butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            //butt_Delete.Left = butt_Save.Left + butt_Save.Width + 2;
            ////this.Refresh();


            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            //butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            butt_Delete.Left = butt_Save.Left + butt_Save.Width + 2;
            button_D_Select.Left = butt_Save.Left + butt_Save.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);
            cfm.button_flat_change(button_LogOut);
            cfm.button_flat_change(button_D_Select);
            
            

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
                    cmdSave_Click(T_bt, ee1);
            }
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

                    if (mtb.Name == "mtxtTel_Dir")
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

                    if (mtb.Name == "mtxtRegDate")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtLeaveDate")
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

            if ((sender is TextBox) == false) return;

            TextBox tb = (TextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
                Sw_Tab = 1;
            }

            if (tb.Name == "txtCenter")
            {
                if (tb.Text.Trim() == "")
                    txtCenter_Code.Text = "";
                else if (Sw_Tab == 1)
                    Ncod_Text_Set_Data(tb, txtCenter_Code);
            }
            if (tb.Name == "txtGubun")
            {
                if (tb.Text.Trim() == "")
                    txtGubun_Code.Text = "";
                else if (Sw_Tab == 1)
                    Ncod_Text_Set_Data(tb, txtGubun_Code);
            }
            if (tb.Name == "txtStatus")
            {
                if (tb.Text.Trim() == "")
                    txtStatus_Code.Text = "";
                else if (Sw_Tab == 1)
                    Ncod_Text_Set_Data(tb, txtStatus_Code);
            }

            //if (tb.Name == "txtBank")
            //{
            //    if (tb.Text.Trim() == "")
            //        txtSellCode_Code.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txtSellCode_Code);
            //}

            //if (tb.Name == "txtR_Id")
            //{
            //    if (tb.Text.Trim() == "")
            //        txtR_Id_Code.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txtR_Id_Code);
            //}

            //if (tb.Name == "txtCenter2")
            //{
            //    if (tb.Text.Trim() == "")
            //        txtCenter2_Code.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txtCenter2_Code);
            //}

            //if (tb.Name == "txtSellCode")
            //{
            //    if (tb.Text.Trim() == "")
            //        txtSellCode_Code.Text = "";
            //    else if (Sw_Tab == 1)
            //        Ncod_Text_Set_Data(tb, txtSellCode_Code);
            //}
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
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txtCenter_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txtCenter_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }
            if (tb.Name == "txtGubun")
            {

                Data_Set_Form_TF = 1;
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txtGubun_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txtGubun_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
             
                Data_Set_Form_TF = 0;
            }
            if (tb.Name == "txtStatus")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txtStatus_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txtStatus_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
                Data_Set_Form_TF = 0;
            }

            //if (tb.Name == "txtR_Id")
            //{
            //    Data_Set_Form_TF = 1;
            //    if (tb.Text.ToString() == "")
            //        Db_Grid_Popup(tb, txtR_Id_Code, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txtR_Id_Code);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}

            //if (tb.Name == "txtBank")
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
            //        Db_Grid_Popup(tb, txtCenter2_Code, "");
            //    else
            //        Ncod_Text_Set_Data(tb, txtCenter2_Code);

            //    SendKeys.Send("{TAB}");
            //    Data_Set_Form_TF = 0;
            //}

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
                if (tb.Name == "txtCenter")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtGubun")
                    cgb_Pop.db_grid_Popup_Base(2, "신청코드", "신청상태", "code", "name", strSql);

                if (tb.Name == "txtStatus")
                    cgb_Pop.db_grid_Popup_Base(2, "처리상태코드", "처리상태", "code", "name", strSql);

                if (tb.Name == "txtCenter2")
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);

                if (tb.Name == "txtSellCode")
                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", strSql);
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
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                }

                if (tb.Name == "txtGubun")
                {
                    string Tsql;
                    Tsql = "Select code ,name   ";
                    Tsql = Tsql + " From TBL_MEMBER_CHANGE_SPON_GUBUNCODE (nolock) ";
                    Tsql = Tsql + " Order by code ";

                    cgb_Pop.db_grid_Popup_Base(2, "신청코드", "신청상태", "code", "name", Tsql);
                }

                if (tb.Name == "txtStatus")
                {
                    string Tsql;
                    Tsql = "Select code ,name   ";
                    Tsql = Tsql + " From TBL_MEMBER_CHANGE_SPON_STATUSCODE (nolock) ";
                    Tsql = Tsql + " Order by code ";

                    cgb_Pop.db_grid_Popup_Base(2, "처리상태코드", "처리상태", "code", "name", Tsql);
                }

                if (tb.Name == "txtCenter2")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                }

                if (tb.Name == "txtSellCode")
                {
                    string Tsql;
                    Tsql = "Select SellCode ,SellTypeName    ";
                    Tsql = Tsql + " From tbl_SellType (nolock) ";
                    Tsql = Tsql + " Order by SellCode ";

                    cgb_Pop.db_grid_Popup_Base(2, "주문_코드", "주문종류", "SellCode", "SellTypeName", Tsql);
                }

            }
        }



        private void Ncod_Text_Set_Data(TextBox tb, TextBox tb1_Code)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            if (tb.Name == "txtCenter")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
            }

            if (tb.Name == "txtGubun")
            {
                Tsql = "Select  code ,name     ";
                Tsql = Tsql + " From TBL_MEMBER_CHANGE_SPON_GUBUNCODE (nolock) ";
                Tsql = Tsql + " Where code like '%" + txtGubun.Text.Trim() + "%'";
                Tsql = Tsql + " OR    name like '%" + txtGubun.Text.Trim() + "%'";
            }

            if (tb.Name == "txtStatus")
            {
                Tsql = "Select  code ,name    ";
                Tsql = Tsql + " From TBL_MEMBER_CHANGE_SPON_STATUSCODE (nolock) ";
                Tsql = Tsql + " Where code like '%" + txtStatus.Text.Trim() + "%'";
                Tsql = Tsql + " OR    name like '%" + txtStatus.Text.Trim() + "%'";
            }


            if (tb.Name == "txtCenter2")
            {
                Tsql = "Select  Ncode, Name   ";
                Tsql = Tsql + " From tbl_Business (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    Name like '%" + tb.Text.Trim() + "%')";
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
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



        private void trv_Item_Set_Sort_Code()
        {
            string ItemName = ""; string ItemCode = "";
    
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>상위 메뉴 관련된 내역을 트리뷰에 넣는다
            
            MenuStrip temp_menu = cls_app_static_var.Mdi_Base_Menu; //((MDIMain)(this.MdiParent)).menuStrip;
            //MenuStrip temp_menu = ((MDIMain)(this.MdiParent)).menuStrip;
            int fCnt = 0;
            foreach (ToolStripMenuItem Baes_1_Menu in temp_menu.Items)
            {
                if ("Exit_Menu" != Baes_1_Menu.Name && Baes_1_Menu.Visible == true)
                {
                    ItemName = Baes_1_Menu.Text.ToString();
                    ItemCode = Baes_1_Menu.Name.ToString();


                    if (ItemCode != "")
                    {
                        if (fCnt == 0)
                        {
                            trv_Item.Nodes.Clear();
                            trv_Item.CheckBoxes = true;

                            tab_Menu.TabPages[0].Text = ItemName;
                            TreeNode tn = trv_Item.Nodes.Add(ItemName);
                            dic_Tree_Sort_1[ItemCode] = tn;
                        }
                        else
                        {
                            TabPage t_tp = new TabPage();
                            TreeView t_v = new TreeView();

                            t_v.Nodes.Clear();
                            t_v.CheckBoxes = true;
                            t_v.AfterCheck += new TreeViewEventHandler(trv_Item_AfterCheck);
                            
                            t_tp.Text = ItemName;
                            t_tp.BackColor = tab_Menu.TabPages[0].BackColor;
                            t_tp.Controls.Add(t_v);

                            t_v.Dock = DockStyle.Fill;

                            TreeNode tn = t_v.Nodes.Add(ItemName);
                            dic_Tree_Sort_1[ItemCode] = tn;
                            //dic_Tree_view[ItemName] = t_v;

                            tab_Menu.Controls.Add(t_tp);
                        }

                        fCnt++;
                    }

                    
                }
            }
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< 상위 메뉴 관련된 내역을 트리뷰에 넣는다
                        


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>하위메뉴 관련된 내역을 트리뷰에 넣는다
            string UpitemCode = "";            
            string Tool_Tip = "";
            foreach (ToolStripMenuItem Baes_1_Menu in temp_menu.Items)
            {
                //dic_Tree_view[Main_ItemName] = t_v;
               // Main_ItemName = Baes_1_Menu.Text.ToString();

                for (int cnt = 0; cnt < Baes_1_Menu.DropDownItems.Count; cnt++)
                {                  

                    if (Baes_1_Menu.DropDownItems[cnt] is ToolStripMenuItem)
                    {
                        //ToolStripItem sub_menu = Baes_1_Menu.DropDownItems[cnt];
                                                                      
                        ItemName = Baes_1_Menu.DropDownItems[cnt].Text.ToString();
                        ItemCode = Baes_1_Menu.DropDownItems[cnt].Name.ToString();
                        UpitemCode = Baes_1_Menu.Name.ToString();

                        Tool_Tip = "";
                        if (Baes_1_Menu.DropDownItems[cnt].ToolTipText != null)
                            Tool_Tip = Baes_1_Menu.DropDownItems[cnt].ToolTipText.ToString();                        
                        

                        //if (ItemCode == "m_Member_Delete")
                        //    return;

                        

                        if (dic_Tree_Sort_1 != null &&
                            ItemCode != "m_Base_User" &&
                            ItemCode != "m_Base_User_Log" &&
                            ItemCode != "m_Base_Config_1" &&
                            Tool_Tip != "-"
                            //Baes_1_Menu.DropDownItems[cnt].Enabled == true   //Visible 속성을 이곳에서 체크하면 다 Flase 나와서  Enabled로 해서 안보이는메뉴를 결정 하기로함.
                            //&& Baes_1_Menu.DropDownItems[cnt].Visible == true
                            )
                        {

                            if (dic_Tree_Sort_1.ContainsKey(UpitemCode))
                            {
                                TreeNode tn2 = dic_Tree_Sort_1[UpitemCode];

                                if (tn2 != null)
                                {
                                    TreeNode node2 = new TreeNode(ItemName);
                                    tn2.Nodes.Add(node2);
                                    tn2.Expand();
                                    dic_Tree_Sort_2[UpitemCode + "/" + ItemCode] = node2;
                                }
                            }

                        }
                       

                    }
                }
                
            }

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<하위메뉴 관련된 내역을 트리뷰에 넣는다                        
        }



        private void trv_Item_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
               
                if (e.Node.Parent == null)
                {          
                    foreach (string t_for_key in dic_Tree_Sort_2.Keys)
                    {
                        TreeNode tn2 = dic_Tree_Sort_2[t_for_key];
                        if (e.Node.Text.ToString() == tn2.Parent.Text.ToString())
                        {
                            tn2.Checked = e.Node.Checked  ;
                        }                       
                    }
                }                          
            }
        }
        private void From_Clear_()
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, txtNcode);
            chkb_Leave.Checked = false;            
            chk_Excel_Save.Checked = false;            
            chk_Cpno_V.Checked = false;

            //dGridView_Login_Header_Reset();
            //cgb_Login.d_Grid_view_Header_Reset();

            //dGridView_Excel_Header_Reset();
            //cgb_Excel.d_Grid_view_Header_Reset();


            tabControl_Tab_Dispose();
            trv_Item_Set_Sort_Code();

            txtID.ReadOnly = false;
            txtID.BorderStyle = BorderStyle.Fixed3D ;
            txtID.BackColor = SystemColors.Window; 

            txtNcode.ReadOnly = false;
            txtNcode.BorderStyle = BorderStyle.Fixed3D;
            txtNcode.BackColor = SystemColors.Window; 

            if (dic_tbl_User != null)
                dic_tbl_User.Clear();


            if (dic_tbl_User != null)
                Base_Grid_Set();

            radioB_User_FLAG_M.Checked = true;
            txtID_Update.Text = "";

            txtID.Focus();

        }


        private void cmdSave_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Clear")
            {
                From_Clear_();
                User_Select_Current_Row = -1; 
            }
            else if (bt.Name == "butt_Save")
            {
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                //저장이 이루어진다.
                combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex; 
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {
                    From_Clear_();  
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
                Delete_Base_Data(ref Del_Error_Check);
                if (Del_Error_Check > 0)
                {
                    From_Clear_();  
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
            Excel_Export_File_Name = this.Text; // "Purchase";
            Excel_Export_From_Name = this.Name;
            return dGridView_Base;
        }



        private void Delete_Base_Data(ref int Del_Error_Check)
        {
            Del_Error_Check = 0;
            if (txtID.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_User_Ncode")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtID.Focus(); return ;
            }


            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;




            //if (txtID.Text == "admin")
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Del_UserID")                     
            //          + "\n" +
            //          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    txtID.Focus(); return;
            //}
            

            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            
            Tsql = "Insert into tbl_mail_request_mod Select * , '" + cls_User.gid  + "', convert(varchar,getdate(),21)  From tbl_mail_request (nolock)   ";
            Tsql = Tsql + " Where MemID = '" + txtID.Text.Trim() + "'";

            if (Temp_Connect.Delete_Data(Tsql, base_db_name, this.Name.ToString(), this.Text) == false) return;


            Tsql = "Delete From tbl_mail_request   ";
            Tsql = Tsql + " Where MemID = '" + txtID.Text.Trim() + "'";

            if (Temp_Connect.Delete_Data(Tsql, base_db_name, this.Name.ToString(), this.Text) == false) return;

            Del_Error_Check = 1;
            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del"));
        }




        private bool Base_Error_Check__01()
        {
            //회원을 선택 안햇네 그럼 회원 넣어라
            if (txtName.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_User_Name")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtName.Focus(); return false;
            }
            if (txtID.Text == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_User_ID")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                txtID.Focus(); return false;
            }
     

            return true;
        }


        private void Save_Base_Data(ref int Save_Error_Check)
        {

           //if (Base_Error_Check__01() == false) return;
                string OrderCancel = "0"; string ReceChange = "0"; string OrderRR = "0"; string TakeRR = "0";
            if (chk_C.Checked == true)
                OrderCancel = "1";
            if (chk_X.Checked == true)
                ReceChange = "1";
          
            string u_user_Ncode = txtID.Text.Trim();
            //++++++++++++++++++++++++++++++++

            //if (txtID_Update.Text == "")
            //{
            //    //MessageBox.Show("변경할 내용을 먼저 선택해주세요."
            //    //           + "\n" +
            //    //           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    //return;
            //}
            //else
            //{
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                string mbid2 = "";  string NOMIN_Index = ""; string SAVE_Index = "";
                for (int i = 0; i < dGridView_Base.Rows.Count; i++)
                {
                    if (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "V")
                    {
                        mbid2 = dGridView_Base.Rows[i].Cells[1].Value.ToString();
                        NOMIN_Index = dGridView_Base.Rows[i].Cells[16].Value.ToString();
                        SAVE_Index = dGridView_Base.Rows[i].Cells[17].Value.ToString();
                        string Tsql = "";

                        if(NOMIN_Index != SAVE_Index) //모두변경
                        {
                            //이전내역을 백업저장한다.
                            Tsql = "Insert into TBL_MEMBER_CHANGE_SPON_mod  ";
                            Tsql = Tsql + " Select * ,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) From TBL_MEMBER_CHANGE_SPON (nolock)  ";
                            Tsql = Tsql + " Where SEQ = '" + NOMIN_Index + "' or SEQ = '" + SAVE_Index + "'  ";
                            if (Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text) == false) return;

                            //수정한다.
                            Tsql = "Update TBL_MEMBER_CHANGE_SPON Set ";
                            if (chk_C.Checked == true)
                            {
                                Tsql = Tsql + " STATUS = 'C'";
                            }
                            if (chk_X.Checked == true)
                            {
                                Tsql = Tsql + " STATUS = 'X'";
                            }
                            Tsql = Tsql + " ,RECORD_ID= '" + cls_User.gid + "'";
                            Tsql = Tsql + " ,MOD_DATE =  convert(varchar,getdate(),21) ";
                            Tsql = Tsql + " Where SEQ = '" + NOMIN_Index + "' or SEQ = '" + SAVE_Index + "' ";

                            if (Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text) == false) return;




                            if (chk_C.Checked == true)
                            {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                Tsql = "EXEC [Usp_TH_SMS]   " + mbid2 + ",'','','9'";
                                // Mail 호출 - 추천인/후원인 변경 처리 완료
                                new cls_Web().SendMail_TH(int.Parse(mbid2), string.Empty, string.Empty, mtxtMakDate1.Text, ESendMailType_TH.changeNominSaveMail);
                            }
                            else
                            {
                                Tsql = "EXEC Usp_Insert_SMS_New  '31',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 완료
                                                                                                                    //Tsql = "EXEC Usp_Insert_SMS '31',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 완료
                            }
                            Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text);
                            }
                            if (chk_X.Checked == true)
                            {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                Tsql = "EXEC [Usp_TH_SMS]   " + mbid2 + ",'','','10'";
                            }
                            else
                            {
                                Tsql = "EXEC Usp_Insert_SMS_New  '32',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 실패
                                                                                                                    //Tsql = "EXEC Usp_Insert_SMS '32',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 실패
                            }
                            Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text);
                            }
                        }
                        else//하나만변경
                        {
                            //이전내역을 백업저장한다.
                            Tsql = "Insert into TBL_MEMBER_CHANGE_SPON_mod  ";
                            Tsql = Tsql + " Select * ,'" + cls_User.gid + "',Convert(Varchar(25),GetDate(),21) From TBL_MEMBER_CHANGE_SPON (nolock)  ";
                            Tsql = Tsql + " Where SEQ = '" + NOMIN_Index + "' ";
                            if (Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text) == false) return;


                            //수정한다.
                            Tsql = "Update TBL_MEMBER_CHANGE_SPON Set ";
                            if (chk_C.Checked == true)
                            {
                                Tsql = Tsql + " STATUS = 'C'";
                            }
                            if (chk_X.Checked == true)
                            {
                                Tsql = Tsql + " STATUS = 'X'";
                            }

                            Tsql = Tsql + " ,RECORD_ID= '" + cls_User.gid + "'";
                            Tsql = Tsql + " ,MOD_DATE =  convert(varchar,getdate(),21) ";


                            Tsql = Tsql + " Where SEQ = '" + NOMIN_Index + "' ";

                            if (Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text) == false) return;




                            if (chk_C.Checked == true)
                            {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                Tsql = "EXEC [Usp_TH_SMS]   " + mbid2 + ",'','','9'";
                            }
                            else
                            {
                                //Tsql = "EXEC Usp_Insert_SMS '31',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 완료
                                Tsql = "EXEC Usp_Insert_SMS_New  '31',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 완료
                            }
                            Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text);
                            }
                            if (chk_X.Checked == true)
                            {
                            if (cls_User.gid_CountryCode == "TH")
                            {
                                Tsql = "EXEC [Usp_TH_SMS]   " + mbid2 + ",'','','10'";
                            }
                            else
                            {
                                Tsql = "EXEC Usp_Insert_SMS_New '32',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 실패
                                                                                                                   //Tsql = "EXEC Usp_Insert_SMS '32',''," + mbid2 + ",'" + NOMIN_Index + "', ''";  //후원인추천인변경신청 실패
                            }
                            Temp_Connect.Update_Data(Tsql, this.Name.ToString(), this.Text);
                            }
                        }
                    }
                //}
                
                
            }
            Save_Error_Check = 1;
            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
        }








        private void Base_Grid_Set()
        {
              dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();


            //string Tsql = "";

            //StringBuilder sb = new StringBuilder();


            ////sb.AppendLine("     ( ");
            //sb.AppendLine("	SELECT '', A.MBID2 AS MBID2, ");
            //sb.AppendLine("C.M_Name, ");
            //sb.AppendLine(" '모두변경', ");
            //sb.AppendLine("ISNULL(A.E_MBID2,'') AS BEFOR_NOMINID2, ");
            //sb.AppendLine("ISNULL(D.M_Name,'') AS BEFOR_NOMIN_M_NAME, ");
            //sb.AppendLine("ISNULL(A.C_MBID2,'') AS AFTER_NOMINID2, ");
            //sb.AppendLine("ISNULL(E.M_NAME,'') AS AFTER_NOMIN_M_NAME, ");
            //sb.AppendLine("ISNULL(B.E_MBID2,'') AS BEFOR_SAVEID2, ");
            //sb.AppendLine("ISNULL(F.M_NAME,'') AS BEFOR_SAVE_M_NAME, ");
            //sb.AppendLine("ISNULL(B.C_MBID2,'') AS AFTER_SAVEID2, ");
            //sb.AppendLine("ISNULL(G.M_Name,'') AS AFTER_SAVE_M_NAME, ");
            //sb.AppendLine("case when A.STATUS = 'R' then '신청상태' WHEN A.STATUS= 'C' then '변경완료' else '거부' end, ");
            //sb.AppendLine(" A.REG_DATE, ");
            //sb.AppendLine(" A.MOD_DATE, ");
            //sb.AppendLine(" A.RECORD_ID, ");
            //sb.AppendLine(" A.SEQ AS NOMIN_SEQ, ");
            //sb.AppendLine(" B.SEQ AS SAVE_SEQ, ");
            //sb.AppendLine(" A.REG_DATE, ");
            //sb.AppendLine(" B.REG_DATE, ");
            //sb.AppendLine(" A.STATUS, ");
            //sb.AppendLine(" B.STATUS ");
            //sb.AppendLine(" FROM  (SELECT * FROM  TBL_MEMBER_CHANGE_SPON WHERE GUBUN = 'N') A  ");
            //sb.AppendLine("  JOIN (SELECT * FROM TBL_MEMBER_CHANGE_SPON WHERE GUBUN ='S') B ON A.MBID2 = B.MBID2  ");
            //sb.AppendLine(" AND LEFT (CONVERT(CHAR(23), A.REG_DATE, 21),19) = LEFT (CONVERT(CHAR(23), B.REG_DATE, 21),19)  ");
            //sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO C ON A.MBID2 = C.MBID2 ");
            //sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO D ON A.E_MBID2 = D.MBID2 ");
            //sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO E ON A.C_MBID2 = E.MBID2 ");
            //sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO F ON B.E_MBID2 = F.MBID2 ");
            //sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO G ON B.C_MBID2 = G.MBID2 ");
            //if (txtID.Text.Trim() != "")
            //    sb.AppendLine(" And A.MBID2 LIKE '%" + txtID.Text.Trim() + "%'");

            //if (txtName.Text.Trim() != "")
            //    sb.AppendLine(" And  C.M_NAME LIKE '%" + txtName.Text.Trim() + "%'");
            ////기록일자로 검색 -1
            //if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() == ""))
            //    sb.AppendLine("And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') = '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'");

            ////기록일자로 검색 -2
            //if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() != ""))
            //{
            //    sb.AppendLine("And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') >= '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'");
            //    sb.AppendLine("And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') <= '" + mtxtMakDate2.Text.Replace("-", "").Trim() + "'");
            //}

            //if (txtGubun_Code.Text.Trim() != "")
            //    sb.AppendLine(" And  A.GUBUN LIKE '%" + txtGubun_Code.Text.Trim() + "%'");
            //if (txtStatus_Code.Text.Trim() != "")
            //    sb.AppendLine(" And  A.STATUS LIKE '%" + txtStatus_Code.Text.Trim() + "%'");
            ////sb.AppendLine(") ");
            //sb.AppendLine("UNION ALL ");
            ////sb.AppendLine("( ");
            //sb.AppendLine("SELECT '', A.MBID2 AS MBID2,C.M_Name, ");
            //sb.AppendLine("case when GUBUN = 'N' then '추천인변경요청' else '후원인변경요청' end, ");
            //sb.AppendLine("case when GUBUN = 'N' then ISNULL(A.E_MBID2,'') ELSE '' END AS BEFOR_NOMINID2, ");
            //sb.AppendLine("case when GUBUN = 'N' then ISNULL(D.M_Name,'')  ELSE '' END AS BEFOR_NOMIN_M_NAME, ");
            //sb.AppendLine("case when GUBUN = 'N' then ISNULL(A.C_MBID2,'')  ELSE '' END AS AFTER_NOMINID2, ");
            //sb.AppendLine("case when GUBUN = 'N' then ISNULL(E.M_NAME,'') ELSE '' END  AS AFTER_NOMIN_M_NAME, ");
            //sb.AppendLine("case when GUBUN = 'S' then ISNULL(A.E_MBID2,'') ELSE '' END  AS BEFOR_SAVEID2, ");
            //sb.AppendLine("case when GUBUN = 'S' then ISNULL(D.M_Name,'') ELSE '' END  AS BEFOR_SAVE_M_NAME, ");
            //sb.AppendLine("case when GUBUN = 'S' then ISNULL(A.C_MBID2,'') ELSE '' END AS AFTER_SAVEID2, ");
            //sb.AppendLine("case when GUBUN = 'S' then ISNULL(E.M_NAME,'') ELSE '' END AS AFTER_SAVE_M_NAME, ");
            //sb.AppendLine("case when A.STATUS = 'R' then '신청상태' WHEN A.STATUS= 'C' then '변경완료' else '거부' end, ");
            //sb.AppendLine("A.REG_DATE, ");
            //sb.AppendLine("A.MOD_DATE, ");
            //sb.AppendLine("A.RECORD_ID, ");
            //sb.AppendLine("A.SEQ AS NOMIN_SEQ, ");
            //sb.AppendLine("A.SEQ AS SAVE_SEQ, ");
            //sb.AppendLine("A.REG_DATE, ");
            //sb.AppendLine("A.REG_DATE, ");
            //sb.AppendLine("A.STATUS, ");
            //sb.AppendLine("A.STATUS ");
            //sb.AppendLine("FROM TBL_MEMBER_CHANGE_SPON A ");
            //sb.AppendLine("LEFT JOIN TBL_MEMBERINFO C ON A.MBID2 = C.MBID2 ");
            //sb.AppendLine("LEFT JOIN TBL_MEMBERINFO D ON A.E_MBID2 = D.MBID2 ");
            //sb.AppendLine("LEFT JOIN TBL_MEMBERINFO E ON A.C_MBID2 = E.MBID2 ");

            //sb.AppendLine("WHERE  A.SEQ IN( ");
            //sb.AppendLine("SELECT SEQ FROM TBL_MEMBER_CHANGE_SPON WHERE SEQ NOT IN  ");
            //sb.AppendLine("( ");
            //sb.AppendLine("(SELECT   ");
            //sb.AppendLine("A.SEQ AS NOMIN_SEQ ");
            //sb.AppendLine("FROM  (SELECT * FROM  TBL_MEMBER_CHANGE_SPON WHERE GUBUN = 'N') A  ");
            //sb.AppendLine(" JOIN (SELECT * FROM TBL_MEMBER_CHANGE_SPON WHERE GUBUN ='S') B ON A.MBID2 = B.MBID2  ");
            //sb.AppendLine("AND LEFT (CONVERT(CHAR(23), A.REG_DATE, 21),19) = LEFT (CONVERT(CHAR(23), B.REG_DATE, 21),19) ) ");
            //sb.AppendLine(" UNION ALL ");
            //sb.AppendLine("(SELECT   ");
            //sb.AppendLine("B.SEQ AS NOMIN_SEQ ");
            //sb.AppendLine("FROM  (SELECT * FROM  TBL_MEMBER_CHANGE_SPON WHERE GUBUN = 'N') A  ");
            //sb.AppendLine(" JOIN (SELECT * FROM TBL_MEMBER_CHANGE_SPON WHERE GUBUN ='S') B ON A.MBID2 = B.MBID2  ");
            //sb.AppendLine("AND LEFT (CONVERT(CHAR(23), A.REG_DATE, 21),19) = LEFT (CONVERT(CHAR(23), B.REG_DATE, 21),19) ) ");
            //sb.AppendLine(") ");

            //if (txtID.Text.Trim() != "")
            //    sb.AppendLine(" And A.MBID2 LIKE '%" + txtID.Text.Trim() + "%'");

            //if (txtName.Text.Trim() != "")
            //    sb.AppendLine(" And  C.M_NAME LIKE '%" + txtName.Text.Trim() + "%'");
            ////기록일자로 검색 -1
            //if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() == ""))
            //    sb.AppendLine(" And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') = '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'");

            ////기록일자로 검색 -2
            //if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() != ""))
            //{
            //    sb.AppendLine(" And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') >= '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'");
            //    sb.AppendLine(" And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') <= '" + mtxtMakDate2.Text.Replace("-", "").Trim() + "'");
            //}

            //if (txtGubun_Code.Text.Trim() != "")
            //    sb.AppendLine(" And  A.GUBUN LIKE '%" + txtGubun_Code.Text.Trim() + "%'");
            //if (txtStatus_Code.Text.Trim() != "")
            //    sb.AppendLine(" And  A.STATUS LIKE '%" + txtStatus_Code.Text.Trim() + "%'");

            //sb.AppendLine(") ");

            //sb.AppendLine(" ORDER BY A.REG_DATE DESC  ");

            //Tsql = sb.ToString();
            ////++++++++++++++++++++++++++++++++
            //cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            //DataSet ds = new DataSet();
            ////테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            //if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
            //int ReCnt = Temp_Connect.DataSet_ReCount;

            //if (ReCnt == 0) return;
            ////++++++++++++++++++++++++++++++++
            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            //for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            //{
            //    Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            //}
            //cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            //cgb.db_grid_Obj_Data_Put();
        }
        private void Base_Grid_Set_Serach()
        {
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();


            string Tsql = "";
            //Tsql = "    select '', B.M_NAME, A.MBID2, C.M_NAME,A.E_MBID2, D.M_NAME,A.C_MBID2,   ";
            //Tsql = Tsql + "case when GUBUN = 'N' then '추천인변경요청' else '후원인변경요청' end,";
            //Tsql = Tsql + "case when STATUS = 'R' then '신청상태' WHEN STATUS = 'C' then '변경완료' else '거부' end,";
            //Tsql = Tsql + "REG_DATE,";
            //Tsql = Tsql + "MOD_DATE,";
            //Tsql = Tsql + "RECORD_ID,";
            //Tsql = Tsql + "A.SEQ";

            //Tsql = Tsql + " From TBL_MEMBER_CHANGE_SPON (nolock) A JOIN TBL_MEMBERINFO B ON A.MBID2 = B.MBID2 ";
            //Tsql = Tsql + " LEFT JOIN  TBL_MEMBERINFO C ON A.E_MBID2 = C.MBID2 ";
            //Tsql = Tsql + " LEFT JOIN  TBL_MEMBERINFO D ON A.C_MBID2 = D.MBID2 ";
            //Tsql = Tsql + " WHERE SEQ <> '' ";

            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("     ( ");

            sb.AppendLine("SELECT * FROM ");
            sb.AppendLine("(  ");
            sb.AppendLine(" SELECT '' as a , A.MBID2 AS MBID2,  ");
            sb.AppendLine("C.M_Name,  ");
            sb.AppendLine(" '모두변경'as b,  ");
            sb.AppendLine("ISNULL(A.E_MBID2,'') AS BEFOR_NOMINID2,  ");
            sb.AppendLine("ISNULL(D.M_Name,'') AS BEFOR_NOMIN_M_NAME,  ");
            sb.AppendLine("ISNULL(A.C_MBID2,'') AS AFTER_NOMINID2,  ");
            sb.AppendLine("ISNULL(E.M_NAME,'') AS AFTER_NOMIN_M_NAME,  ");
            sb.AppendLine("ISNULL(B.E_MBID2,'') AS BEFOR_SAVEID2,  ");
            sb.AppendLine("ISNULL(F.M_NAME,'') AS BEFOR_SAVE_M_NAME,  ");
            sb.AppendLine("ISNULL(B.C_MBID2,'') AS AFTER_SAVEID2,  ");
            sb.AppendLine("ISNULL(G.M_Name,'') AS AFTER_SAVE_M_NAME,  ");
            sb.AppendLine("case when A.STATUS = 'R' then '신청상태' WHEN A.STATUS= 'C' then '변경완료' else '거부' end as c ,  ");
            sb.AppendLine(" A.REG_DATE AS REG_DATE1,  ");
            sb.AppendLine(" A.MOD_DATE,  ");
            sb.AppendLine(" A.RECORD_ID,  ");
            sb.AppendLine(" A.SEQ AS NOMIN_SEQ, ");
            sb.AppendLine(" B.SEQ AS SAVE_SEQ,  ");
            sb.AppendLine(" A.REG_DATE AS REG_DATE2,  ");
            sb.AppendLine(" B.REG_DATE AS REG_DATE3,  ");
            sb.AppendLine(" A.STATUS AS STATUS1 ,  ");
            //sb.AppendLine(" B.STATUS AS STATUS2  ");
            sb.AppendLine(" B.STATUS AS STATUS2,  ");   // 240307 태국 구분 관련 추가 - syhuh
            sb.AppendLine(" C.Na_Code  ");  // 240307 태국 구분 관련 추가 - syhuh
            sb.AppendLine(" FROM  (SELECT * FROM  TBL_MEMBER_CHANGE_SPON WHERE GUBUN = 'N') A   ");
            sb.AppendLine("  JOIN (SELECT * FROM TBL_MEMBER_CHANGE_SPON WHERE GUBUN ='S') B ON A.MBID2 = B.MBID2   ");
            sb.AppendLine(" AND LEFT (CONVERT(CHAR(23), A.REG_DATE, 21),19) = LEFT (CONVERT(CHAR(23), B.REG_DATE, 21),19)   ");
            sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO C ON A.MBID2 = C.MBID2  ");
            sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO D ON A.E_MBID2 = D.MBID2  ");
            sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO E ON A.C_MBID2 = E.MBID2  ");
            sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO F ON B.E_MBID2 = F.MBID2  ");
            sb.AppendLine(" LEFT JOIN TBL_MEMBERINFO G ON B.C_MBID2 = G.MBID2  ");
            sb.AppendLine("UNION ALL  ");
            sb.AppendLine("SELECT ''as a , A.MBID2 AS MBID2,C.M_Name,  ");
            sb.AppendLine("case when GUBUN = 'N' then '추천인변경요청' else '후원인변경요청' end as b,  ");
            sb.AppendLine("case when GUBUN = 'N' then ISNULL(A.E_MBID2,'') ELSE '' END AS BEFOR_NOMINID2,  ");
            sb.AppendLine("case when GUBUN = 'N' then ISNULL(D.M_Name,'')  ELSE '' END AS BEFOR_NOMIN_M_NAME,  ");
            sb.AppendLine("case when GUBUN = 'N' then ISNULL(A.C_MBID2,'')  ELSE '' END AS AFTER_NOMINID2,  ");
            sb.AppendLine("case when GUBUN = 'N' then ISNULL(E.M_NAME,'') ELSE '' END  AS AFTER_NOMIN_M_NAME,  ");
            sb.AppendLine("case when GUBUN = 'S' then ISNULL(A.E_MBID2,'') ELSE '' END  AS BEFOR_SAVEID2,  ");
            sb.AppendLine("case when GUBUN = 'S' then ISNULL(D.M_Name,'') ELSE '' END  AS BEFOR_SAVE_M_NAME,  ");
            sb.AppendLine("case when GUBUN = 'S' then ISNULL(A.C_MBID2,'') ELSE '' END AS AFTER_SAVEID2,  ");
            sb.AppendLine("case when GUBUN = 'S' then ISNULL(E.M_NAME,'') ELSE '' END AS AFTER_SAVE_M_NAME,  ");
            sb.AppendLine("case when A.STATUS = 'R' then '신청상태' WHEN A.STATUS= 'C' then '변경완료' else '거부' end as c,  ");
            sb.AppendLine("A.REG_DATE AS REG_DATE1,  ");
            sb.AppendLine("A.MOD_DATE,  ");
            sb.AppendLine("A.RECORD_ID,  ");
            sb.AppendLine("A.SEQ AS NOMIN_SEQ,  ");
            sb.AppendLine("A.SEQ AS SAVE_SEQ,  ");
            sb.AppendLine("A.REG_DATE AS REG_DATE2,  ");
            sb.AppendLine("A.REG_DATE AS REG_DATE3,  ");
            sb.AppendLine("A.STATUS AS STATUS1 ,  ");
            //sb.AppendLine("A.STATUS AS STATUS2 ");
            sb.AppendLine("A.STATUS AS STATUS2, ");     // 240307 태국 구분 관련 추가 - syhuh
            sb.AppendLine("C.Na_Code ");    // 240307 태국 구분 관련 추가 - syhuh
            sb.AppendLine("FROM TBL_MEMBER_CHANGE_SPON A  ");
            sb.AppendLine("LEFT JOIN TBL_MEMBERINFO C ON A.MBID2 = C.MBID2  ");
            sb.AppendLine("LEFT JOIN TBL_MEMBERINFO D ON A.E_MBID2 = D.MBID2  ");
            sb.AppendLine("LEFT JOIN TBL_MEMBERINFO E ON A.C_MBID2 = E.MBID2  ");
            sb.AppendLine("WHERE  A.SEQ IN(  ");
            sb.AppendLine("SELECT SEQ FROM TBL_MEMBER_CHANGE_SPON WHERE SEQ NOT IN   ");
            sb.AppendLine("(  ");
            sb.AppendLine("(SELECT    ");
            sb.AppendLine("A.SEQ AS NOMIN_SEQ  ");
            sb.AppendLine("FROM  (SELECT * FROM  TBL_MEMBER_CHANGE_SPON WHERE GUBUN = 'N') A   ");
            sb.AppendLine(" JOIN (SELECT * FROM TBL_MEMBER_CHANGE_SPON WHERE GUBUN ='S') B ON A.MBID2 = B.MBID2   ");
            sb.AppendLine("AND LEFT (CONVERT(CHAR(23), A.REG_DATE, 21),19) = LEFT (CONVERT(CHAR(23), B.REG_DATE, 21),19) )  ");
            sb.AppendLine(" UNION ALL  ");
            sb.AppendLine("(SELECT    ");
            sb.AppendLine("B.SEQ AS NOMIN_SEQ  ");
            sb.AppendLine("FROM  (SELECT * FROM  TBL_MEMBER_CHANGE_SPON WHERE GUBUN = 'N') A   ");
            sb.AppendLine(" JOIN (SELECT * FROM TBL_MEMBER_CHANGE_SPON WHERE GUBUN ='S') B ON A.MBID2 = B.MBID2   ");
            sb.AppendLine("AND LEFT (CONVERT(CHAR(23), A.REG_DATE, 21),19) = LEFT (CONVERT(CHAR(23), B.REG_DATE, 21),19) )  ");
            sb.AppendLine(") ");
            sb.AppendLine(") ");
            sb.AppendLine(") ");
            sb.AppendLine("AS LASTTABLE  WHERE LASTTABLE.MBID2 <> '' ");

            cls_NationService.SQL_NationCode(ref sb, "LASTTABLE", "AND ", true);    // 240307 태국 구분 관련 추가 - syhuh

            if (txtID.Text.Trim() != "")
                sb.AppendLine(" And LASTTABLE.MBID2 LIKE '%" + txtID.Text.Trim() + "%'");

            if (txtName.Text.Trim() != "")
                sb.AppendLine(" And  LASTTABLE.M_NAME LIKE '%" + txtName.Text.Trim() + "%'");
            //기록일자로 검색 -1
            if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() == ""))
                sb.AppendLine(" And Replace(Left(CONVERT(CHAR(8),  LASTTABLE.REG_DATE1, 112),10),'-','') = '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'");

            //기록일자로 검색 -2
            if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() != ""))
            {
                sb.AppendLine(" And Replace(Left(CONVERT(CHAR(8),  LASTTABLE.REG_DATE1, 112),10),'-','') >= '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'");
                sb.AppendLine(" And Replace(Left(CONVERT(CHAR(8),  LASTTABLE.REG_DATE1, 112),10),'-','') <= '" + mtxtMakDate2.Text.Replace("-", "").Trim() + "'");
            }

            if (txtGubun_Code.Text.Trim() != "")
                sb.AppendLine(" And  LASTTABLE.b LIKE '%" + txtGubun.Text.Trim() + "%'");
            if (txtStatus_Code.Text.Trim() != "")
                sb.AppendLine(" And  LASTTABLE.STATUS1 LIKE '%" + txtStatus_Code.Text.Trim() + "%'");

            //sb.AppendLine(") ");

            sb.AppendLine(" ORDER BY LASTTABLE.REG_DATE1 DESC  ");



            //if (txtID.Text.Trim() != "")
            //Tsql = Tsql + " And A.MBID2 LIKE '%" + txtID.Text.Trim() + "%'";

            //if (txtName.Text.Trim() != "")
            // Tsql = Tsql + " And  B.M_NAME LIKE '%" + txtName.Text.Trim() + "%'";
            ////기록일자로 검색 -1
            //if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() == ""))
            //    Tsql = Tsql + " And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') = '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'";

            ////기록일자로 검색 -2
            //if ((mtxtMakDate1.Text.Replace("-", "").Trim() != "") && (mtxtMakDate2.Text.Replace("-", "").Trim() != ""))
            //{
            //    Tsql = Tsql + " And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') >= '" + mtxtMakDate1.Text.Replace("-", "").Trim() + "'";
            //    Tsql = Tsql + " And Replace(Left(CONVERT(CHAR(8),  A.REG_DATE, 112),10),'-','') <= '" + mtxtMakDate2.Text.Replace("-", "").Trim() + "'";
            //}

            //if (txtGubun_Code.Text.Trim() != "")
            //    Tsql = Tsql + " And  A.GUBUN LIKE '%" + txtGubun_Code.Text.Trim() + "%'";
            //if (txtStatus_Code.Text.Trim() != "")
            //    Tsql = Tsql + " And  A.STATUS LIKE '%" + txtStatus_Code.Text.Trim() + "%'";

            Tsql = sb.ToString();
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
            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
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
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }



        private void dGridView_Base_Header_Reset()
        {

            cgb.grid_col_Count = 18;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            //cgb.grid_Frozen_End_Count = 2;            
            //cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            
           
            string[] g_HeaderText = { "선택","회원번호"    , "이름"  ,"신청내용","기존추천인번호" ,"기존추천인이름"
                                  ,"변경추천인번호" ,  "변경추천인이름",  "기존후원인번호",  "기존후원인이름",  "변경후원인번호"
                                  ,"변경후원인이름" ,"상태","신청일","처리일","처리자","",""

                                };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = {  100,100, 100, 150, 150,150
                            ,150  ,100 ,100,150,150
                            ,100,100,100,100,100,0,0
                        };
            cgb.grid_col_w = g_Width;
           

            

            Boolean[] g_ReadOnly = {  true,true,  true,  true ,true      , true
                                    ,true , true    , true, true, true
                                    , true  , true, true, true, true, true, true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {   DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft   //5    
                               
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              ,DataGridViewContentAlignment.MiddleLeft

                              ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                    ,DataGridViewContentAlignment.MiddleLeft
                                        ,DataGridViewContentAlignment.MiddleLeft
                                            ,DataGridViewContentAlignment.MiddleLeft

                                        ,DataGridViewContentAlignment.MiddleLeft
                                            ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb.grid_col_alignment = g_Alignment;

            //cgb.basegrid.RowHeadersVisible = false;
        }




        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {

            DataGridView dgv = (DataGridView)sender;
         
            txtName.Text = dgv.CurrentRow.Cells[1].Value.ToString();
            txtID.Text = dgv.CurrentRow.Cells[2].Value.ToString();  
            txtID_Update.Text = dgv.CurrentRow.Cells[2].Value.ToString();
            txtSEQ.Text = dgv.CurrentRow.Cells[12].Value.ToString();
            String string_chk_OrderCancel = dgv.CurrentRow.Cells[8].Value.ToString();
            if(string_chk_OrderCancel == "변경완료")
            {
                chk_C.Checked = true;
            }
            else
            {
                chk_C.Checked = false;
            }
            String string_chk_ReceChange = dgv.CurrentRow.Cells[8].Value.ToString();
            if (string_chk_ReceChange == "거부")
            {
                chk_X.Checked = true;
            }
            else
            {
                chk_X.Checked = false;
            }
        }
          



        private void User_node_Check(string Menu1, int s_TF)
        {
            string[] t_Memu;
            t_Memu = Menu1.Split('%');
            for (int cnt = 0; cnt < t_Memu.Length; cnt++)
            {
                if (t_Memu[cnt] != "")
                {
                    foreach (string t_for_key in dic_Tree_Sort_2.Keys)
                    {
                        TreeNode tn2 = dic_Tree_Sort_2[t_for_key];
                        if (t_for_key == t_Memu[cnt])
                            tn2.Checked = true;                    
                    }
                }
            }

        }









        private void Set_gr_Excel(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;

            object[] row0 = new object[cgb_Excel.grid_col_Count];

            while (Col_Cnt < cgb_Excel.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }


            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Excel_Header_Reset()
        {
            cgb_Excel.Grid_Base_Arr_Clear();
            cgb_Excel.basegrid = dGridView_Excel;
            cgb_Excel.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Excel.grid_col_Count = 10;

            //cgb_Excel.grid_Frozen_End_Count = 3;
            cgb_Excel.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"전환_화면"  , "저장_이름"   , "저장_시간"  , ""   , ""                                        
                                ,"" , "" , ""  ,   ""  , "" 
                                };

            int[] g_Width = { 100, 150, 120, 0, 0
                            ,0 , 0 , 0 , 0 , 0                      
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft 
                                ,DataGridViewContentAlignment.MiddleLeft  
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter  //5    
  
                                ,DataGridViewContentAlignment.MiddleCenter 
                                ,DataGridViewContentAlignment.MiddleRight  
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleCenter 
                                ,DataGridViewContentAlignment.MiddleCenter  //10

                                };


            cgb_Excel.grid_col_header_text = g_HeaderText;
            cgb_Excel.grid_col_w = g_Width;
            cgb_Excel.grid_col_alignment = g_Alignment;


            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true   
                                   };
            cgb_Excel.grid_col_Lock = g_ReadOnly;

        }



        private void tabControl_Tab_Dispose()
        {

           
            for (int fcnt = tab_Menu.TabCount - 1; fcnt > 0; fcnt--)
            {
                tab_Menu.TabPages[fcnt].Dispose();
            }
           
            tab_Menu.TabPages[0].Text = "";
            
        }

        private void radioB_U_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtID.Text.Trim() == "" && txtID.ReadOnly == true)
            {
                txtID.ReadOnly = false;
                txtID.BorderStyle = BorderStyle.Fixed3D;
                txtID.BackColor = SystemColors.Window;
            }
        }

        private void button_D_Select_Click(object sender, EventArgs e)
        {
            //if (txt_Search.Text.Trim() != "")
            //{
                if (dic_tbl_User != null)
                    dic_tbl_User.Clear();

                //if (radioB_User_FLAG_S_M.Checked == false && radioB_User_FLAG_S_T.Checked == false && radioB_User_FLAG_S_E.Checked == false)
                //{
                //    Set_Tbl_User(txt_Search.Text.Trim());
                //}
                //else
                //{
                //    Set_Tbl_User("","SS");
                //}

                if (dic_tbl_User != null)
                    Base_Grid_Set();
            //}
            
        }

        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
        }

        private void dGridView_Base_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button_D_Select_Click_1(object sender, EventArgs e)
        {
            Base_Grid_Set_Serach();
        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            
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

        private void butt_S_check_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
            {
                dGridView_Base.Rows[i].Cells[0].Value = "V";
            }
        }

        private void butt_S_Not_check_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= dGridView_Base.Rows.Count - 1; i++)
            {
                dGridView_Base.Rows[i].Cells[0].Value = "";
            }
        }
    }
}
