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
    public partial class frmMember_Save_Change : Form
    {

        cls_Grid_Base cgb = new cls_Grid_Base();

        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        //cls_Grid_Base cgb = new cls_Grid_Base();
        private const string base_db_name = "tbl_Memberinfo";
        private int Data_Set_Form_TF;

        public delegate void Take_NumberDele(ref string Send_Number, ref string Send_Name);
        public event Take_NumberDele Take_Mem_Number;

        public frmMember_Save_Change()
        {
            InitializeComponent();
        }

        
        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            Data_Set_Form_TF = 0;
           
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtMbid2.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtMbid3.Mask = cls_app_static_var.Member_Number_Fromat;

            txtCpno.BackColor = cls_app_static_var.txt_Enable_Color;
            txtRDate.BackColor = cls_app_static_var.txt_Enable_Color;
            txtHptel.BackColor = cls_app_static_var.txt_Enable_Color;

            mtxtMbid2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtName2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtCpno2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtRDate2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtHptel2.BackColor = cls_app_static_var.txt_Enable_Color;

            txtCpno3.BackColor = cls_app_static_var.txt_Enable_Color;
            txtRDate3.BackColor = cls_app_static_var.txt_Enable_Color;
            txtHptel3.BackColor = cls_app_static_var.txt_Enable_Color;
        }



        private void frm_Base_Activated(object sender, EventArgs e)
        {
           //19-03-11 깜빡임제거 this.Refresh();

            string Send_Number = ""; string Send_Name = "";
            Take_Mem_Number(ref Send_Number, ref Send_Name);

            if (Send_Number != "")
            {
                mtxtMbid.Text = Send_Number;
                Set_Form_Date(mtxtMbid.Text);
            }
        }
        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            //butt_Excel.Left = butt_Save.Left + butt_Save.Width + 2;
            //butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);
            cfm.button_flat_change(butt_Top);
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
                            if (Input_Error_Check(mtb,0) == true)
                                Set_Form_Date(mtb.Text);
                            //SendKeys.Send("{TAB}");

                        }

                        if (mtb.Name == "mtxtMbid3")
                        {
                            txtName3.Text = Search_Name;
                            if (Input_Error_Check(mtb, 3) == true)
                                Set_Form_Date(mtb.Text,3);

                            SendKeys.Send("{TAB}");
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

                        if (mtb.Name == "mtxtMbid3")
                        {
                            e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number_3);
                            e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemNumber_Info_3);
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

        void e_f_Send_MemNumber_Info_3(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            seachName = "";
            cls_Search_DB csb = new cls_Search_DB();
            csb.Member_Nmumber_Split(mtxtMbid3.Text.Trim(), ref searchMbid, ref searchMbid2);
        }



        //변경할려는 대상자에 대한 회원번호에서 회원 검색창을 뛰엇을 경우에
        void e_f_Send_Mem_Number(string Send_Number, string Send_Name)
        {
            mtxtMbid.Text = Send_Number;            txtName.Text = Send_Name;
            if (Input_Error_Check(mtxtMbid, 0) == true)
                Set_Form_Date(mtxtMbid.Text);
        }

        //새로운 후원인 관련 회원 검색창을 뛰엇을 경우에 검색창에서 이벤트 실행시..
        void e_f_Send_Mem_Number_3(string Send_Number, string Send_Name)
        {
            mtxtMbid3.Text = Send_Number;            txtName3.Text = Send_Name;
            if (Input_Error_Check(mtxtMbid3, 3) == true)
                Set_Form_Date(mtxtMbid3.Text, 3);
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
                    ct.from_control_clear(groupBox1, mtb);
                    ct.from_control_clear(groupBox2, mtb);
                }

                ct.from_control_clear(groupBox3, mtb);

                //if (mtb.Name == "mtxtMbid")
                //{
                //    txtName.Text = ""; txtCpno.Text = ""; txtRDate.Text = "";
                //    mtxtMbid2.Text = ""; txtName2.Text = ""; txtCpno2.Text = ""; txtRDate2.Text = "";
                //}

                //if (mtb.Name == "mtxtMbid3")
                //{
                //    txtName3.Text = ""; txtCpno3.Text = ""; txtRDate3.Text = "";
                //    txtHptel3.Text = ""; txtAdd3.Text = "";
                //}
            }
        }


        //텍스트 박스들 포커스가 갓을때 전제 선택이 되도록함.
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


        //텍스트 박스 키 프레스 이벤트
        private void txtData_KeyPress(object sender, KeyPressEventArgs e)
        {
            cls_Check_Text T_R = new cls_Check_Text();

            //엔터키를 눌럿을 경우에 탭을 다음 으로 옴기기 위한 이벤트 추가
            T_R.Key_Enter_13 += new Key_13_Event_Handler(T_R_Key_Enter_13);
            T_R.Key_Enter_13_Name += new Key_13_Name_Event_Handler(T_R_Key_Enter_13_Name);
          
            TextBox tb = (TextBox)sender;

            if ((tb.Tag == null) || (tb.Tag.ToString() == ""))
            {                
                if (T_R.Text_KeyChar_Check(e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if   
            }
            else if (tb.Tag.ToString() == "1") //숫자만 입력 가능하다.
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, 1) == false)
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


        //텍스트 박스의 기준 길이만큰 다 차면 다음 지정된 탭으로 이동을 해라 전화번호 관려련 사항때문에 첨가됨.
        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;

            TextBox tb = (TextBox)sender;
            if (tb.TextLength >= tb.MaxLength)
            {
                SendKeys.Send("{TAB}");
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

                    if (tb.Name == "txtName3")
                    {
                        mtxtMbid3.Text = Search_Mbid; //회원명으로 검색해서 나온 사람이 한명일 경우에는 회원번호를 넣어준다.                    
                        if (Input_Error_Check(mtxtMbid3, 3) == true)
                            Set_Form_Date(mtxtMbid3.Text,3);
                        SendKeys.Send("{TAB}");
                    }
                }
                else if (reCnt != 1)  //동명이인이 존재해서 사람이 많을 경우나 또는 이름 없이 엔터친 경우에.
                {
                    //cls_app_static_var.Search_Member_Name = txt_tag;
                    frmBase_Member_Search e_f = new frmBase_Member_Search();
                    if (tb.Name == "txtName")
                    {
                        e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                        e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemName_Info);
                    }

                    if (tb.Name == "txtName3")
                    {
                        e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number_3);
                        e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemName_Info_3);
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
           seachName = txtName.Text.Trim() ;
        }

        void e_f_Send_MemName_Info_3(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            searchMbid = ""; searchMbid2 = 0;
           seachName = txtName3.Text.Trim() ;;
        }
        
        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }

      

        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Top")
            {
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data_Top(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {
                    cls_form_Meth ct = new cls_form_Meth();
                    ct.from_control_clear(this, mtxtMbid);

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                    cgb.d_Grid_view_Header_Reset(1);
                    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }


            if (bt.Name == "butt_Clear")
            {                
                cls_form_Meth ct = new cls_form_Meth();
                ct.from_control_clear(this, mtxtMbid);

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb.d_Grid_view_Header_Reset(1);
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            }


            else if (bt.Name == "butt_Save")
            {
                if (mtxtMbid3.Text.Replace("-", "").Replace("_", "").Replace(" ", "") == "" && txtName3.Text == "" && txtCpno3.Text == "" && txtRDate3.Text == "")
                {
                    Button T_bt = butt_Top;
                    EventArgs ee1 = null;
                    Base_Button_Click(T_bt, ee1);
                    return;
                }

                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)
                {                    
                    cls_form_Meth ct = new cls_form_Meth();
                    ct.from_control_clear(this, mtxtMbid);

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                    cgb.d_Grid_view_Header_Reset(1);
                    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }


            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }                         
        }





        private Boolean  Input_Error_Check(MaskedTextBox  m_tb, int s_Kind )
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
                m_tb.Focus();                return false;
            }
            
            string Tsql = "";
            Tsql = "Select Mbid , Mbid2, M_Name ,  Sell_Mem_TF, RBO_Mem_TF ";
            Tsql = Tsql + " , LineCnt , N_LineCnt  ";
            Tsql = Tsql + " , LeaveDate , LineUserDate  " ;
            Tsql = Tsql + " , Saveid  , Saveid2  ";
            Tsql = Tsql + " , Nominid , Nominid2  ";
            Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
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
                m_tb.Focus();                return false;
            }
            //++++++++++++++++++++++++++++++++   


            if (s_Kind == 3) //3인 경우는 새로운 지정 후원인인데.. 탈퇴나 라인중자가 아닌지를 체크한다.
            {
                //if (ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString() != "")
                //{

                //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Leave_")
                //            + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                //           + "\n" +
                //           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                //    m_tb.Focus(); return false;
                //}

                if (ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString() != "")
                {
                    if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Input_Mem_Leave_") + "\n" + cls_app_static_var.app_msg_rm.GetString("Msg_Action_Ing"), "", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        m_tb.Focus(); return false;
                    }
                }

                if (ds.Tables[base_db_name].Rows[0]["LineUserDate"].ToString() != "")
                {

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Delete_")
                            + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    m_tb.Focus(); return false;
                }

                //후원인은 소비자로 할수 없다. . 추천인이라면 모를까.
                if (int.Parse(ds.Tables[base_db_name].Rows[0]["RBO_Mem_TF"].ToString()) == 1)
                {

                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Sell_TF_0")
                            + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    m_tb.Focus(); return false;
                }


                //나의 하부 사람이 내 위로 갈려고 하는건 아닌지 체크한다
                if (Saveid_Loop_Check(Mbid, Mbid2) == false)
                {
                    m_tb.Focus(); return false;
                }
            }

            return true ;
        }


        //새로운 후원인으로 지정된 사람의 변경할려는 대상자 아래 있는 사람인지를 체크한다. 
        //그럴경우 후원이 루트를 찾아 쫓아올라가는 수당로직 같은 경유 무한 루프에 빠지므로
        private Boolean Saveid_Loop_Check(string Mbid, int Mbid2)
        {
            string T_Mbid = mtxtMbid.Text;
            string TTMbid = ""; int TTMbid2 = 0;

            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref TTMbid, ref TTMbid2) == -1) return false ;


            string Tsql = "";
            Tsql = "Select  ";           
            Tsql = "Select Mbid , Mbid2 , M_Name , lvl , Save_Cur " ;
            Tsql = Tsql + " From ufn_SaveUp_Member_Search ('" + Mbid + "'," + Mbid2 + ") ";
            Tsql = Tsql + " Where Mbid = '" + TTMbid + "'" ;
            Tsql = Tsql + " And   Mbid2 = " + TTMbid2.ToString ();

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return false ;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) 
                return true;
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Down_")
                        + " - " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));                
                return false ;
            }
            //++++++++++++++++++++++++++++++++            
        }



        private void Set_Form_Date(string  T_Mbid)
        {
            Data_Set_Form_TF = 1;
            string Mbid = ""; int Mbid2 = 0;

            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
            {
                string Tsql = "";
                Tsql = "Select  ";
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                else
                    Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";

                Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";
                Tsql = Tsql + ", tbl_Memberinfo.Cpno   AS Cpno";

                Tsql = Tsql + " , tbl_Memberinfo.LineCnt ";

                Tsql = Tsql + " , LEFT(tbl_Memberinfo.RegTime,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.RegTime,4),2) + '-' + RIGHT(tbl_Memberinfo.RegTime,2)  AS RegTime  ";                
                Tsql = Tsql + " , tbl_Memberinfo.hptel ";
                Tsql = Tsql + " , tbl_Memberinfo.address1 +'-' + tbl_Memberinfo.address2 ";


                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " ,tbl_Memberinfo.Saveid + '-' + Convert(Varchar,tbl_Memberinfo.Saveid2) AS T_Saveid ";
                else
                    Tsql = Tsql + " ,tbl_Memberinfo.Saveid2 AS T_Saveid ";

                Tsql = Tsql + " , Isnull(Sav.M_Name,'') AS Save_Name ";
                Tsql = Tsql + ",  Sav.Cpno   AS Save_Cpno";
                
                Tsql = Tsql + " , LEFT(Sav.RegTime,4) +'-' + LEFT(RIGHT(Sav.RegTime,4),2) + '-' + RIGHT(Sav.RegTime,2)  As Save_RegTime  ";
                Tsql = Tsql + " , Sav.hptel AS Save_hptel ";
                Tsql = Tsql + " , Sav.address1 +'-' + Sav.address2 AS Save_Address ";
                
                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                Tsql = Tsql + " LEFT JOIN tbl_Memberinfo Sav (nolock) ON tbl_Memberinfo.Saveid = Sav.Mbid And tbl_Memberinfo.Saveid2 = Sav.Mbid2 ";
                //Tsql = Tsql + " LEFT JOIN tbl_Memberinfo Nom (nolock) ON tbl_Memberinfo.Nominid = Nom.Mbid And tbl_Memberinfo.Nominid2 = Nom.Mbid2 ";
                //Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode ";
                //Tsql = Tsql + " Left Join tbl_Bank On tbl_Memberinfo.bankcode=tbl_Bank.ncode ";
                //Tsql = Tsql + " Left Join tbl_Class C1 On tbl_Memberinfo.CurGrade=C1.Grade_Cnt ";          

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }
               // // Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
                Tsql = Tsql + " And   tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

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
                txtCpno.Text = encrypter.Decrypt ( ds.Tables[base_db_name].Rows[0]["Cpno"].ToString(),"Cpno");
                txtRDate.Text = ds.Tables[base_db_name].Rows[0]["RegTime"].ToString();
                txtHptel.Text = encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hptel"].ToString());

                mtxtMbid2.Text = ds.Tables[base_db_name].Rows[0]["T_Saveid"].ToString();
                txtName2.Text = ds.Tables[base_db_name].Rows[0]["Save_Name"].ToString();
                txtCpno2.Text = encrypter.Decrypt (ds.Tables[base_db_name].Rows[0]["Save_Cpno"].ToString(),"Cpno");
                txtRDate2.Text = ds.Tables[base_db_name].Rows[0]["Save_RegTime"].ToString();

                txtHptel2.Text = encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Save_hptel"].ToString());

                Base_Grid_Set();
            }
            Data_Set_Form_TF = 0;
            mtxtMbid3.Focus();
        }



        private void Set_Form_Date(string T_Mbid, int T_sort)
        {
            Data_Set_Form_TF = 1;
            string Mbid = ""; int Mbid2 = 0;

            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
            {
                string Tsql = "";
                Tsql = "Select  ";
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                else
                    Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";

                Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";

                Tsql = Tsql + ", tbl_Memberinfo.Cpno  AS Cpno";

                Tsql = Tsql + " , tbl_Memberinfo.LineCnt ";

                Tsql = Tsql + " , LEFT(tbl_Memberinfo.RegTime,4) +'-' + LEFT(RIGHT(tbl_Memberinfo.RegTime,4),2) + '-' + RIGHT(tbl_Memberinfo.RegTime,2)  AS RegTime  ";
                Tsql = Tsql + " , tbl_Memberinfo.hptel ";
                Tsql = Tsql + " , tbl_Memberinfo.address1 ";
                Tsql = Tsql + " ,tbl_Memberinfo.address2  ";


                //if (cls_app_static_var.Member_Number_1 > 0)
                //    Tsql = Tsql + " ,tbl_Memberinfo.Saveid + '-' + Convert(Varchar,tbl_Memberinfo.Saveid2) AS Saveid ";
                //else
                //    Tsql = Tsql + " ,tbl_Memberinfo.Saveid2 AS Saveid ";

                //Tsql = Tsql + " , Isnull(Sav.M_Name,'') AS Save_Name ";

                //if (cls_app_static_var.Member_Cpno_Visible_TF == 1)
                //    Tsql = Tsql + ", Case When  Sav.Cpno <> '' Then LEFT(Sav.Cpno,6) +'-' + RIGHT(Sav.Cpno,7)  ELSE '' End AS Save_Cpno";
                //else
                //    Tsql = Tsql + ", Case When  Sav.Cpno <> '' Then LEFT(Sav.Cpno,6) +'-' + '*******'  ELSE '' End  AS Save_Cpno";

                //Tsql = Tsql + " , LEFT(Sav.RegTime,4) +'-' + LEFT(RIGHT(Sav.RegTime,4),2) + '-' + RIGHT(Sav.RegTime,2)  As Save_RegTime  ";
                //Tsql = Tsql + " , Sav.hptel AS Save_hptel ";
                //Tsql = Tsql + " , Sav.address1 +'-' + Sav.address2 AS Save_Address ";

                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                //Tsql = Tsql + " LEFT JOIN tbl_Memberinfo Sav (nolock) ON tbl_Memberinfo.Saveid = Sav.Mbid And tbl_Memberinfo.Saveid2 = Sav.Mbid2 ";
                //Tsql = Tsql + " LEFT JOIN tbl_Memberinfo Nom (nolock) ON tbl_Memberinfo.Nominid = Nom.Mbid And tbl_Memberinfo.Nominid2 = Nom.Mbid2 ";
                //Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode ";
                //Tsql = Tsql + " Left Join tbl_Bank On tbl_Memberinfo.bankcode=tbl_Bank.ncode ";
                //Tsql = Tsql + " Left Join tbl_Class C1 On tbl_Memberinfo.CurGrade=C1.Grade_Cnt ";          

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }
                //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
                //Tsql = Tsql + " And   tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                //Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                //++++++++++++++++++++++++++++++++

                mtxtMbid3.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
                txtName3.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
                txtCpno3.Text = encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Cpno"].ToString());
                txtRDate3.Text = ds.Tables[base_db_name].Rows[0]["RegTime"].ToString();

                txtHptel3.Text = encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hptel"].ToString());
                txtAdd3.Text = encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["address1"].ToString()) + " " + encrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["address2"].ToString()) ;               
            }
            Data_Set_Form_TF = 0;
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


            if (mtxtMbid3.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtMbid3);

                if (Ret == -1)
                {
                    mtxtMbid3.Focus(); return false;
                }
            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid3.Focus(); return false;
            }

            string Self_Mbid = ""; int Self_Mbid2 = 0;
            string New_Saveid = ""; int New_Saveid2 = 0;
            string Old_Saveid = ""; int Old_Saveid2 = 0;

            string T_Mbid = mtxtMbid2.Text;
            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Old_Saveid, ref Old_Saveid2) != 1)//입력된 회원번호 내역이 올바르게 입력 되었는지를 체크함
            {
                //mtxtMbid.Focus(); return false;
            }


            T_Mbid = mtxtMbid3.Text;
            if (csb.Member_Nmumber_Split(T_Mbid, ref New_Saveid, ref New_Saveid2) != 1)
            {
                mtxtMbid3.Focus(); return false;
            }

            T_Mbid = mtxtMbid.Text;
            if (csb.Member_Nmumber_Split(T_Mbid, ref Self_Mbid, ref Self_Mbid2) != 1)
            {
                mtxtMbid.Focus(); return false;
            }



            //if ((New_Saveid == Old_Saveid) && (New_Saveid2 == Old_Saveid2) )
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Down_Same")
            //            + " - " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
            //           + "\n" +
            //           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    mtxtMbid.Focus(); return false;
            //}
            
            if ((New_Saveid == Self_Mbid) && (New_Saveid2 == Self_Mbid2))
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Down_Self")
                       + " - " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return false;
            }


            return true;
        }


        private Boolean Check_TextBox_Error(string Save_Err_Check_2)
        {
            if (Input_Error_Check(mtxtMbid, 0) == false) return false;

            // -- * 회원번호 체크 *
            if (Save_Err_Check_2 == "Save_Err_Check_2")
            {
                if (Input_Error_Check(mtxtMbid3, 3) == false) return false;

                if (mtxtMbid2.Text == mtxtMbid3.Text)
                {

                }
                else
                {

                    cls_Search_DB csb = new cls_Search_DB();
                    if (csb.Member_Down_Save_TF(mtxtMbid3.Text.Trim()) == false)
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Down_Full")
                               + " - " + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                              + "\n" +
                              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                        mtxtMbid3.Focus(); return false;
                    }
                }
            }
            // -- * 좌/우에 누가있는지 체크 *
            else if (Save_Err_Check_2 == "Save_Err_Check_4")
            {
                int LineCnt = rdoLineLeft.Checked ? 1 : 2;
                string Qry = "SELECT mbid2, m_name FROM tbl_Memberinfo (nolock) WHERE SaveId2 = " + mtxtMbid3.Text.Replace("_", "").Trim();
                Qry += " AND LineCnt = " + LineCnt;
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                DataSet ds = new DataSet();

                Temp_Connect.Open_Data_Set(Qry, "tbl_Memberinfo", ds);
                if(Temp_Connect.DataSet_ReCount > 0 )
                {
                    string Msg = string.Format("{0}측에 기존회원 {1}_{2} 회원이 존재합니다." + Environment.NewLine
                        + "변경하시겠습니까?"
                        , (LineCnt == 1 ? "좌" : "우")
                        , ds.Tables["tbl_Memberinfo"].Rows[0]["mbid2"].ToString()
                        , ds.Tables["tbl_Memberinfo"].Rows[0]["m_name"].ToString()
                        );

                    DialogResult ret = MessageBox.Show(Msg, "확인", MessageBoxButtons.YesNo);
                    return ret == DialogResult.Yes;
                }
                

            }



            return true;
        }


       
        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;

            //int Chang_Price_TF = 0;
            if (Check_TextBox_Error() == false) return;
            if (Check_TextBox_Error("Save_Err_Check_2") == false) return;
            if (Check_TextBox_Error("Save_Err_Check_4") == false) return;
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;


            cls_Search_DB csd = new cls_Search_DB();
            csd.Member_Mod_BackUp(mtxtMbid.Text.Trim(), "tbl_Memberinfo");

            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

            T_Mbid = mtxtMbid3.Text.Trim();
            string NEW_idx = ""; 
            int NEW_idx2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref NEW_idx, ref NEW_idx2);
            int LineCnt_Tmp = csd.LineCnt_Search_Save(NEW_idx, NEW_idx2);

            //int LineCnt = csd.LineCnt_Search_Save(NEW_idx, NEW_idx2);
            int LineCnt = rdoLineLeft.Checked ? 1 : (LineCnt_Tmp == 1 ? 2 : LineCnt_Tmp);//csd.LineCnt_Search_Save(Saveid, Saveid2);

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string StrSql = "";

            //이미 좌가있으면 우로빼주고
            if (rdoLineLeft.Checked)
            {
                StrSql = "select top 1 mbid2 from tbl_memberinfo where Saveid2 = '" + NEW_idx2 + "' AND LineCnt = 1 ";

                DataSet ds = new DataSet();
                Temp_Connect.Open_Data_Set(StrSql, base_db_name, ds);
                if (Temp_Connect.DataSet_ReCount > 0)
                {
                    cls_Search_DB csd2 = new cls_Search_DB();
                    string TempMbid2 = ds.Tables[base_db_name].Rows[0][0].ToString();
                    csd2.Member_Mod_BackUp(TempMbid2, "tbl_Memberinfo");

                    StrSql = "Update tbl_Memberinfo Set ";
                    StrSql = StrSql + " LineCnt = 2 ";
                    StrSql = StrSql + " Where saveid2 = '" + NEW_idx2 + "'";
                    if (Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text) == false) return;

                    csd2.tbl_Memberinfo_Mod(TempMbid2, txt_ETC.Text);
                }

                LineCnt = 1;
            }
            //이미 우가있으면 좌로 빼줍니다
            else if(rdoLineRight.Checked)
            {
                StrSql = "select top 1 mbid2 from tbl_memberinfo where Saveid2 = '" + NEW_idx2 + "' AND LineCnt = 2 ";

                DataSet ds = new DataSet();
                Temp_Connect.Open_Data_Set(StrSql, base_db_name, ds);
                if (Temp_Connect.DataSet_ReCount > 0)
                {
                    cls_Search_DB csd2 = new cls_Search_DB();
                    string TempMbid2 = ds.Tables[base_db_name].Rows[0][0].ToString();
                    csd2.Member_Mod_BackUp(TempMbid2, "tbl_Memberinfo");

                    StrSql = "Update tbl_Memberinfo Set ";
                    StrSql = StrSql + " LineCnt = 1 ";
                    StrSql = StrSql + " Where saveid2 = '" + NEW_idx2 + "'";
                    if (Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text) == false) return;

                    csd2.tbl_Memberinfo_Mod(TempMbid2, txt_ETC.Text);
                }

                LineCnt = 2;
            }
            

            StrSql = "Update tbl_Memberinfo Set ";
            StrSql = StrSql + "  saveid = '" + NEW_idx + "'";
            StrSql = StrSql + " ,saveid2 = " + NEW_idx2.ToString();
            StrSql = StrSql + " ,LineCnt = " + LineCnt;
            StrSql = StrSql + " Where mbid = '" + Mbid + "'";
            StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();

            if (Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text) == false) return;



            csd.tbl_Memberinfo_Mod(mtxtMbid.Text.Trim(), txt_ETC.Text);


            Save_Error_Check = 1;
            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));

        }



        //최상위로 변경할 경우 버튼 클릭시 실행되는 메소드
        private void Save_Base_Data_Top(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            //최상위로 변경할 경우에는 변경할려는 회원의 회원번호만 체크를 한다.
            cls_Check_Input_Error c_er = new cls_Check_Input_Error();
            if (mtxtMbid.Text.Replace("-", "").Replace("_", "").Trim() != "")
            {
                int Ret = 0;
                Ret = c_er._Member_Nmumber_Split(mtxtMbid);

                if (Ret == -1)
                {
                    mtxtMbid.Focus(); return;
                }
            }
            else
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtMbid.Focus(); return;
            }


            //최상위로 변경할 경우에는 변경할려는 회원의 회원번호만 체크를 한다.
            if (Check_TextBox_Error("Save_Err_Check_3") == false) return;
            
            cls_Search_DB csd = new cls_Search_DB();
            csd.Member_Mod_BackUp(mtxtMbid.Text.Trim(), "tbl_Memberinfo");  //회원 내역을 임시로 백업 받는다.

            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2); //회원번호를 분할 받아온다.

            string NEW_idx = "**"; int NEW_idx2 = 0;
            int LineCnt = 1;

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string StrSql = "";
            StrSql = "Update tbl_Memberinfo Set ";
            StrSql = StrSql + "  saveid = '" + NEW_idx + "'";
            StrSql = StrSql + " ,saveid2 = " + NEW_idx2.ToString();
            StrSql = StrSql + " ,LineCnt = " + LineCnt;
            StrSql = StrSql + " Where mbid = '" + Mbid + "'";
            StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();

            if (Temp_Connect.Update_Data(StrSql, this.Name.ToString(), this.Text) == false) return;

            csd.tbl_Memberinfo_Mod(mtxtMbid.Text.Trim(), txt_ETC.Text ); // 위에서 백업 받은 임시 내역이랑 변경한 후의 내역이랑 비교해서. 차이점을 저장한다.
            
            Save_Error_Check = 1;  //제대로 저장이 되엇는지 체크하는 변수
            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
        }



        //회원번호 클릿햇을때. 관련 정보들 다 리셋 시킨다.
        //추후 번호만 변경하고 엔터 안누눌러서.. 데이타가 엉키는 것을 방지하기 위함.
        private void mtxtMbid_Click(object sender, EventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)sender;

            cls_form_Meth ct = new cls_form_Meth();
            if (mtb.Name == "mtxtMbid")
            {
                ct.from_control_clear(groupBox2, mtb);
                ct.from_control_clear(groupBox1, mtb);
            }

            ct.from_control_clear(groupBox3, mtb);

            //if (mtb.Name == "mtxtMbid")
            //{
            //    txtName.Text = ""; txtCpno.Text = ""; txtRDate.Text = "";
            //    mtxtMbid2.Text = ""; txtName2.Text = ""; txtCpno2.Text = ""; txtRDate2.Text = "";
                
            //}

            //if (mtb.Name == "mtxtMbid3")
            //{
            //    txtName3.Text = ""; txtCpno3.Text = ""; txtRDate3.Text = "";
            //    txtHptel3.Text = ""; txtAdd3.Text = "";
            //}

            //마스크텍스트 박스에 입력한 내용이 있으면 그곳 다음으로 커서가 가게 한다.
            if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                mtb.SelectionStart = mtb.Text.Replace("-", "").Replace("_", "").Trim().Length+1;

        }






        private void Make_Base_Query(ref string Tsql)
        {
            cls_form_Meth cm = new cls_form_Meth();
            string save_C = cm._chang_base_caption_search("후원인_변경");
            string nom_C = cm._chang_base_caption_search("추천인_변경");

            Tsql = "Select  ";
            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) ";
            else
                Tsql = Tsql + " tbl_Memberinfo.mbid2 ";

            Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";


            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + ", A.Old_mbid + '-' + Convert(Varchar,A.Old_mbid2) ";
            else
                Tsql = Tsql + ", A.Old_mbid2 ";

            Tsql = Tsql + " ,C.M_Name ";


            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + ", A.New_mbid + '-' + Convert(Varchar,A.New_mbid2) ";
            else
                Tsql = Tsql + ", A.New_mbid2 ";

            Tsql = Tsql + " ,D.M_Name ";

            Tsql = Tsql + " , A.Recordid ";
            Tsql = Tsql + " , A.Recordtime ";
            Tsql = Tsql + " , Case When Save_Nomin_SW = 'Sav' Then '" + save_C + "' ELSE '" + nom_C + "' END";
            Tsql = Tsql + " , A.Remark   ";
            Tsql = Tsql + " ,Isnull(tbl_Business.Name,'') as B_Name";

            Tsql = Tsql + " From tbl_Memberinfo_Save_Nomin_Change As A (nolock) ";
            Tsql = Tsql + " LEFT JOIN tbl_Memberinfo  (nolock) On A.Mbid= tbl_Memberinfo.Mbid And A.Mbid2 = tbl_Memberinfo.Mbid2  ";
            Tsql = Tsql + " LEFT JOIN tbl_Memberinfo AS C (nolock) On A.Old_mbid=C.Mbid And A.Old_mbid2=C.Mbid2  ";
            Tsql = Tsql + " LEFT JOIN tbl_Memberinfo AS D (nolock) On A.New_mbid=D.Mbid And A.New_mbid2=D.Mbid2 ";

            Tsql = Tsql + " LEFT JOIN tbl_Business  (nolock)   ON tbl_Memberinfo.BusinessCode = tbl_Business.ncode And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";
        }



        private void Make_Base_Query_(ref string Tsql)
        {
            string T_Mbid = mtxtMbid.Text;
            string Mbid = ""; int Mbid2 = 0;
            Data_Set_Form_TF = 1;
            cls_Search_DB csb = new cls_Search_DB();
            csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

            string strSql = " Where A.Mbid = '" + Mbid + "'";
            strSql = strSql + " And A.Mbid2 = " + Mbid2;
            strSql = strSql + " And A.Save_Nomin_SW = 'Sav' ";

            Tsql = Tsql + strSql;
            Tsql = Tsql + " Order by A.recordtime Asc,A.Mbid,A.Mbid2 ";
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

            cgb.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb.db_grid_Obj_Data_Put();
        }



        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 11;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            //cg_sub.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원_번호"  , "성명"   , "전_상위번호"  , "전_상위성명"   , "후_상위번호"        
                                , "후_상위성명"   , "변경자"    , "변경일"  , "구분"  , "비고" 
                                , "센타"
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 85, 90, 100, 100, 100  
                             ,100, 90 , 150 , 75 , 150 
                             ,100
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true
                                    ,true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //5
                               
                               ,DataGridViewContentAlignment.MiddleCenter                              
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter             
                               ,DataGridViewContentAlignment.MiddleCenter             
                               ,DataGridViewContentAlignment.MiddleCenter //10
 
                               ,DataGridViewContentAlignment.MiddleCenter  
                              };
            cgb.grid_col_alignment = g_Alignment;
        }


        private void Set_gr_dic(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {

            string str_8 = "";

            cls_form_Meth cm = new cls_form_Meth();
            str_8 = cm._chang_base_caption_search(ds.Tables[base_db_name].Rows[fi_cnt][8].ToString());


            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][3]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][4]
 
                                ,ds.Tables[base_db_name].Rows[fi_cnt][5]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][6]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][7]                                             
                                ,str_8
                                ,ds.Tables[base_db_name].Rows[fi_cnt][9]  

                                ,ds.Tables[base_db_name].Rows[fi_cnt][10]  
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }




    }
}
