﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Printing;
using System.Reflection;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MLM_Program
{
    public partial class frmMember_UpdateSelect : Form
    {
        cls_Grid_Base cgb = new cls_Grid_Base();

        cls_Grid_Base cg_Up_S = new cls_Grid_Base();
        cls_Grid_Base cgb_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Cacu = new cls_Grid_Base();
        cls_Grid_Base cgb_Rece = new cls_Grid_Base();


        cls_Grid_Base cgb_Auto = new cls_Grid_Base();


        private Dictionary<int, cls_Sell_Item> SalesItemDetail = new Dictionary<int, cls_Sell_Item>();
        private Dictionary<int, cls_Sell_Rece> Sales_Rece = new Dictionary<int, cls_Sell_Rece>();
        private Dictionary<int, cls_Sell_Cacu> Sales_Cacu = new Dictionary<int, cls_Sell_Cacu>();


        public delegate void Take_NumberDele(ref string Send_Number, ref string Send_Name);
        public event Take_NumberDele Take_Mem_Number;

        private const string base_db_name = "tbl_Memberinfo";
        private int Data_Set_Form_TF;
        private string idx_Mbid = "", idx_Password = "";
        private int idx_Mbid2 = 0;


        Series series_Item = new Series();


        public frmMember_UpdateSelect()
        {
            InitializeComponent();


            DoubleBuffered = true;

            typeof(Form).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
          | BindingFlags.Instance | BindingFlags.SetProperty, null, this, new object[] { true });


            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, dGridView_inf, new object[] { true });

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, dGridView_Pay, new object[] { true });

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, dGridView_Down_S2, new object[] { true });

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, dGridView_Down_N2, new object[] { true });

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, panel13, new object[] { true });

            typeof(TabControl ).InvokeMember("DoubleBuffered", BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.SetProperty, null, tabC_Mem, new object[] { true });            
        }
                

        private void frmBase_From_Load(object sender, EventArgs e)
        {

           
            Data_Set_Form_TF = 0;

            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb.d_Grid_view_Header_Reset();
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //dGridView_Line_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cg_Li.d_Grid_view_Header_Reset();
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<




            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            
            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;            
            mtxtSn.Mask = "999999-9999999"; //기본 셋팅은 주민번호이다. 
            mtxtSn_C.Mask = "999999-9999999"; //기본 셋팅은 주민번호이다. 

            mtxtTel1.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtTel2.Mask = cls_app_static_var.Tel_Number_Fromat;
            mtxtZip1.Mask = cls_app_static_var.ZipCode_Number_Fromat;
            mtxtZip2.Mask = cls_app_static_var.ZipCode_Number_Fromat;

            mtxtBrithDay.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtRegDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtEdDate.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtRBODate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtVisaDay.Mask = cls_app_static_var.Date_Number_Fromat;

            mtxtBrithDayC.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtTel2_C.Mask = cls_app_static_var.Tel_Number_Fromat;

            txtB1.Text = "0"; 
            //Reset_Chart_Total();

            if (cls_app_static_var.save_uging_Pr_Flag == 0) //후원인 기능 사용하지 마라.
            {       
                tabC_Up.TabPages.Remove(tabP_S);
                tabC_Up.TabPages.Remove(tabP_S_D);
                tabC_Mem.TabPages.Remove(tab_Down_Save);
                tbl_save.Visible = false;                             
            }

            if (cls_app_static_var.nom_uging_Pr_Flag == 0)  //추천인 기능 사용하지 마라
            {
                tabC_Up.TabPages.Remove(tabP_N);
                tabC_Up.TabPages.Remove(tabP_N_D);
                tabC_Mem.TabPages.Remove(tab_Down_Nom);
                
                tbl_nom.Visible = false;                
            }

            txtMbid_n.BackColor = cls_app_static_var.txt_Enable_Color;
            txtName_n.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSN_n.BackColor = cls_app_static_var.txt_Enable_Color;

            txtMbid_s.BackColor = cls_app_static_var.txt_Enable_Color;
            txtName_s.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSN_s.BackColor = cls_app_static_var.txt_Enable_Color;

            txtMbid_n2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtName_n2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSN_n2.BackColor = cls_app_static_var.txt_Enable_Color;

            txtMbid_s2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtName_s2.BackColor = cls_app_static_var.txt_Enable_Color;
            txtSN_s2.BackColor = cls_app_static_var.txt_Enable_Color;


            txtLineCnt.BackColor = cls_app_static_var.txt_Enable_Color; 
            mtxtSn.BackColor = cls_app_static_var.txt_Enable_Color;
            txtLeaveDate.BackColor = cls_app_static_var.txt_Enable_Color;
            txtLineDate.BackColor = cls_app_static_var.txt_Enable_Color;
            txtGrade.BackColor = cls_app_static_var.txt_Enable_Color;
            txt_Us.BackColor = cls_app_static_var.txt_Enable_Color;
            txtGradeP.BackColor = cls_app_static_var.txt_Enable_Color;

            if (tab_Nation_2.Visible == true)
            {
                cls_Pro_Base_Function cpbf = new cls_Pro_Base_Function();
                cpbf.Put_NaCode_ComboBox(combo_Se, combo_Se_Code);
                cpbf.Put_NaCode_ComboBox(combo_Se_2, combo_Se_Code_2);
            }

            tabC_Mem.SelectedIndex = 0;

            cls_Grid_Base_info_Put cgbp11 = new cls_Grid_Base_info_Put();
            cgbp11.dGridView_Put_baseinfo(dGridView_Talk, "talk");


            //tabC_Mem.TabPages.Remove(tab_Auto);
            //tabC_Mem.TabPages.Remove(tab_Auto_Select);
            
            //20190405 구현호 유지보수
            //tabC_Mem.TabPages.Remove(tabP_Sell);
            tabC_Mem.TabPages.Remove(tabP_info);
            tabC_Mem.TabPages.Remove(tabP_Up);
            tabC_Mem.TabPages.Remove(tabP_Pay);
            tabC_Mem.TabPages.Remove(tabP_Add);

            //20190405 구현호 유지보수
            //tabC_Mem.TabPages.Remove(tab_Down_Save);

            //20190405 구현호 유지보수
            //tabC_Mem.TabPages.Remove(tab_Down_Nom);
            tabC_Mem.TabPages.Remove(tab_Hide);

            if (cls_User.gid_CC_Save_TF == 0)  //공동신청인 권한이 없는 사람은 보이지 않게 한다.
                panel_CC.Enabled = false; 
            else
                panel_CC.Enabled = true ;

            radioB_RBO.Checked = true;
            radioB_G8.Checked = true;

            InitComboZipCode_TH();
            // 태국버전 인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                pnlDistrict_TH.Visible = true;
                pnlProvince_TH.Visible = true;
                pnlSubDistrict_TH.Visible = true;
                pnlZipCode_TH.Visible = true;
                pnlZipCode_KR.Visible = false;
                txtAddress2.ReadOnly = true;
                cbSubDistrict_TH_SelectedIndexChanged(this, null);
                //combo_Se_Code_2.Text = "TH";
            }
            // 태국 이외 버전 인 경우
            else
            {
                pnlDistrict_TH.Visible = false;
                pnlProvince_TH.Visible = false;
                pnlSubDistrict_TH.Visible = false;
                pnlZipCode_TH.Visible = false;
                pnlZipCode_KR.Visible = true;
                txtAddress2.ReadOnly = false;
                txtAddress2.Clear();
            }

            combo_Se_Code.Text = cls_User.gid_CountryCode;
            combo_Se_Code_2.Text = cls_User.gid_CountryCode;

            mtxtMbid.Focus();
        }

        private void InitComboZipCode_TH()
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_State_TH() ORDER BY ZIPCODE_SORT ");
            sb.AppendLine("SELECT * FROM ufn_Get_ZipCode_Province_TH() ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbProvince_TH.DataBindings.Clear();
            cbProvince_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbProvince_TH.DisplayMember = "ZipCode_NM";
            cbProvince_TH.ValueMember = "ProvinceCode";

        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            button_exigo.Left = butt_Save.Left + butt_Save.Width + 2;
            //butt_Delete.Left = butt_Excel.Left + butt_Excel.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;


            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);
            cfm.button_flat_change(butt_AddCode);
            cfm.button_flat_change(butt_AddCode2);
            cfm.button_flat_change(butt_AddCodeT1);
            cfm.button_flat_change(butt_Talk);

            cfm.button_flat_change(button_exigo);
            cfm.button_flat_change(button_Acc_Reg);
            

            
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

            ////그리드일 경우에는 DEL키로 행을 삭제하는걸 막는다.
            //if (sender is DataGridView)
            //{
            //    if (e.KeyValue == 46)
            //    {
            //        e.Handled = true;
            //    } // end if                
            //}

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


        private void frmMember_Activated(object sender, EventArgs e)
        {
            //this.Refresh ();
            string Send_Number = ""; string Send_Name = "";
            Take_Mem_Number(ref Send_Number, ref Send_Name);

            if (Send_Number != "")
            {
                mtxtMbid.Text = Send_Number;
                Set_Form_Date(mtxtMbid.Text, "m");
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
                            if (Input_Error_Check(mtb, "m") == true)
                                Set_Form_Date(mtb.Text, "m");
                            //SendKeys.Send("{TAB}");
                        }                   
                    }

                    else if (reCnt > 1)  //회원번호 비슷한 사람들이 많은 경우
                    {
                        string Mbid = "";
                        int Mbid2 = 0;
                        cds.Member_Nmumber_Split(mtb.Text, ref Mbid, ref Mbid2);

                        frmBase_Member_Search e_f = new frmBase_Member_Search();
                        
                        if (mtb.Name == "mtxtMbid")
                        {
                            e_f.Send_Mem_Number += new frmBase_Member_Search.SendNumberDele(e_f_Send_Mem_Number);
                            e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemNumber_Info);
                        }                                            

                        e_f.ShowDialog();

                        SendKeys.Send("{TAB}");
                    }
                    else if (reCnt <= 0)  //동일 회원번호로 사람이 없는 경우에
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Mbid_Not_Exist")
                         + "\n" +
                         cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
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
            if (Input_Error_Check(mtxtMbid, "m") == true)
                Set_Form_Date(mtxtMbid.Text, "m");
        }


        private void mtxtMbid_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Text.Replace("_", "").Replace("-", "").Replace(" ", "") == "")
            {                       
                if (mtb.Name == "mtxtMbid")
                {
                    _From_Data_Clear();                
                }
                
            }
        }


        //회원번호 클릿햇을때. 관련 정보들 다 리셋 시킨다.
        //추후 번호만 변경하고 엔터 안누눌러서.. 데이타가 엉키는 것을 방지하기 위함.
        private void mtxtMbid_Click(object sender, EventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)sender;
                        
            if (mtb.Name == "mtxtMbid")
            {
                _From_Data_Clear();
                combo_Se_Code.Text = cls_User.gid_CountryCode;
                combo_Se_Code_2.Text = cls_User.gid_CountryCode;
            }

            

            //마스크텍스트 박스에 입력한 내용이 있으면 그곳 다음으로 커서가 가게 한다.
            if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                mtb.SelectionStart = mtb.Text.Replace("-", "").Replace("_", "").Trim().Length + 1;

        }




        private void MtxtData_Sn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                MaskedTextBox mtb = (MaskedTextBox)sender;

                if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                {
                    string Sn = mtb.Text.Replace("-", "").Replace("_", "").Trim();
                    if (Sn_Number_(Sn, mtb) == true)
                    {                        
                            txtName_E_1_C.Focus();
                    }
                }
                else
                {                    
                        txtName_E_1_C.Focus();
                }

            }
        }



        private bool Sn_Number_(string Sn, MaskedTextBox mtb, int Check_Multi_TF = 0 )
        {
            if (Sn != "")
            {
                string sort_TF = "";
                bool check_b = false;
                cls_Sn_Check csn_C = new cls_Sn_Check();

                if (mtb.Name == "mtxtSn")
                {
                    if (raButt_IN_1.Checked == true) //내국인인 구분자
                        sort_TF = "in";

                    if (raButt_IN_2.Checked == true) //외국인 구분자
                        sort_TF = "fo";

                    if (raButt_IN_3.Checked == true) //사업자 구분자.
                        sort_TF = "biz";
                }
                else
                {
                    if (raButt_IN_1_C.Checked == true) //내국인인 구분자
                        sort_TF = "in";

                    if (raButt_IN_2_C.Checked == true) //외국인 구분자
                        sort_TF = "fo";

                }
                check_b = csn_C.Sn_Number_Check(Sn, sort_TF);

                if (check_b == false && sort_TF != "fo")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_Error")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    mtb.Focus(); return false;
                }
                else
                {

                    if (cls_app_static_var.Member_Reg_Multi_TF == 0 && Check_Multi_TF == 0 ) //다구좌 불가능으로 해서 체크되어 잇는 경우
                    {//동일 주민번호로 해서 가입한 사람이 있는지를 체크한다.
                        cls_Search_DB csb = new cls_Search_DB();
                        if (csb.Member_Multi_Sn_Search(Sn) == false) //주민번호 오류는 위에서 체크를 함.
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_Same")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            mtb.Focus(); return false;
                        }
                    }
                }


                if (mtb.Name == "mtxtSn")
                {
                    if (raButt_IN_1.Checked == true && check_b == true) //내국인인 경우에는 주민번호 체크한다.
                    {
                        string BirthDay2 = "";
                        if (csn_C.check_19_nai(Sn, ref BirthDay2) == false) //한국같은 경우에는 미성년자 필히 체크한다.
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_19")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            mtb.Focus(); return false;
                        }
                    }
                }

                if (mtb.Name == "mtxtSn_C")
                {
                    if (raButt_IN_1_C.Checked == true && check_b == true) //내국인인 경우에는 주민번호 체크한다.
                    {
                        string BirthDay2 = "";
                        if (csn_C.check_19_nai(Sn, ref BirthDay2) == false) //한국같은 경우에는 미성년자 필히 체크한다.
                        {
                            MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_19")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            mtb.Focus(); return false;
                        }
                    }
                }
            }
            else
            {
                if (cls_app_static_var.Member_Cpno_Put_TF == 1) //주민번호 관련 필수입력인데 입력 안햇다.
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_Put")
                            + "\n" +
                            cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    mtb.Focus(); return false;
                }
            }

            return true;
        }






        private void  Set_Form_Date(string T_Mbid, string T_sort )
        {
            _From_Data_Clear();   
            //idx_Mbid = ""; idx_Mbid2 = 0;
            string Mbid = ""; int Mbid2 = 0;
            Data_Set_Form_TF = 1;
            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
            {
                string Tsql = "";
                
                Tsql = "Select  ";
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                else
                    Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";

                Tsql = Tsql + " ,tbl_Memberinfo.mbid ";
                Tsql = Tsql + " ,tbl_Memberinfo.mbid2 ";
                Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";
                Tsql = Tsql + " ,tbl_Memberinfo.E_name ";
                Tsql = Tsql + " ,tbl_Memberinfo.E_name_Last ";

                Tsql = Tsql + " , tbl_Memberinfo.Email  AS Email ";
                Tsql = Tsql + ", tbl_Memberinfo.Cpno AS Cpno ";

                Tsql = Tsql + " , tbl_Memberinfo.LineCnt ";
                
                Tsql = Tsql + " , tbl_Memberinfo.RegTime ";
                Tsql = Tsql + " , tbl_Memberinfo.hptel   AS hptel";
                Tsql = Tsql + " , tbl_Memberinfo.Addcode1  AS Addcode1 ";
                Tsql = Tsql + " , tbl_Memberinfo.address1  AS address1 ";
                Tsql = Tsql + " , tbl_Memberinfo.address2   AS address2";

                Tsql = Tsql + " , tbl_Memberinfo.hometel   AS hometel";
                //Tsql = Tsql + " , tbl_Memberinfo.hptel )  AS hptel";
                Tsql = Tsql + " , tbl_Memberinfo.businesscode ";
                Tsql = Tsql + " ,Isnull(tbl_Business.Name,'') as B_Name";

                Tsql = Tsql + " , tbl_Memberinfo.BankCode ";
                Tsql = Tsql + " ,Isnull(tbl_Bank.bankName,'') as Bank_Name";
                Tsql = Tsql + " , tbl_Memberinfo.bankowner ";
                Tsql = Tsql + " , tbl_Memberinfo.bankaccnt  AS bankaccnt ";
                Tsql = Tsql + " , tbl_Memberinfo.Reg_bankaccnt  AS Reg_bankaccnt ";
                

                Tsql = Tsql + " , tbl_Memberinfo.Remarks ";

                Tsql = Tsql + " , tbl_Memberinfo.BirthDay ";
                Tsql = Tsql + " , tbl_Memberinfo.BirthDay_M ";
                Tsql = Tsql + " , tbl_Memberinfo.BirthDay_D ";
                Tsql = Tsql + " , tbl_Memberinfo.BirthDayTF ";

                Tsql = Tsql + " , tbl_Memberinfo.CpnoDocument ";
                Tsql = Tsql + " , tbl_Memberinfo.BankDocument ";
                
                Tsql = Tsql + " , tbl_Memberinfo.LeaveDate ";
                Tsql = Tsql + " , tbl_Memberinfo.LineUserDate ";
                Tsql = Tsql + " , tbl_Memberinfo.WebID ";
                Tsql = Tsql + " , tbl_Memberinfo.WebPassWord ";
                Tsql = Tsql + " , tbl_Memberinfo.Ed_Date ";
                Tsql = Tsql + " , tbl_Memberinfo.PayStop_Date ";

                Tsql = Tsql + " , tbl_Memberinfo.For_Kind_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.Sell_Mem_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.Add_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.GiBu_ ";
                Tsql = Tsql + " , tbl_Memberinfo.Myoffice_TF ";

                Tsql = Tsql + " , tbl_Memberinfo.VisaDate ";
                Tsql = Tsql + " , tbl_Memberinfo.RBO_S_Date ";
                

                Tsql = Tsql + " , tbl_Memberinfo.C_M_Name ";
                Tsql = Tsql + " , tbl_Memberinfo.C_For_Kind_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.C_cpno ";
                Tsql = Tsql + " , tbl_Memberinfo.C_E_name ";
                Tsql = Tsql + " , tbl_Memberinfo.C_E_name_Last ";
                
                Tsql = Tsql + " , tbl_Memberinfo.C_BirthDay ";
                Tsql = Tsql + " , tbl_Memberinfo.C_BirthDay_M ";
                Tsql = Tsql + " , tbl_Memberinfo.C_BirthDay_D ";
                Tsql = Tsql + " , tbl_Memberinfo.C_hptel ";
                Tsql = Tsql + " , tbl_Memberinfo.C_email ";

                Tsql = Tsql + " , tbl_Memberinfo.RBO_Mem_TF ";
                Tsql = Tsql + " , tbl_Memberinfo.G8_TF ";

                Tsql = Tsql + " , tbl_Memberinfo.Sex_FLAG";
                Tsql = Tsql + " , tbl_Memberinfo.AgreeSMS";
                Tsql = Tsql + " , tbl_Memberinfo.AgreeEmail";
                //Tsql = Tsql + " , tbl_Memberinfo.ipin_ci"; //휴대폰인증은 명의변경쪽에서 진행해야함
                //Tsql = Tsql + " , tbl_Memberinfo.ipin_di";
                
                
                
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " ,tbl_Memberinfo.Saveid + '-' + Convert(Varchar,tbl_Memberinfo.Saveid2) AS T_Saveid ";
                else
                    Tsql = Tsql + " ,Sav.sponsoralkynumber AS T_Saveid ";

                Tsql = Tsql + " , Isnull(Sav2.alphaname,'') AS Save_Name ";
                Tsql = Tsql + " , tbl_Memberinfo.Saveid ";
                //Tsql = Tsql + ",  Sav.Cpno  AS Save_Cpno ";
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<  

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " ,tbl_Memberinfo.Nominid + '-' + Convert(Varchar,tbl_Memberinfo.Nominid2) AS T_Nominid ";
                else
                    Tsql = Tsql + " ,Nom.enrolleralkynumber AS T_Nominid ";

                Tsql = Tsql + " , Isnull(Nom2.alphaname,'') AS Nomin_Name ";
                Tsql = Tsql + " , tbl_Memberinfo.Nominid ";

                //Tsql = Tsql + ",  Nom.Cpno AS Nom_Cpno ";

                //if (cls_app_static_var.Member_Cpno_Visible_TF == 1)
                //    Tsql = Tsql + ", Case When  Nom.Cpno <> '' Then LEFT(Nom.Cpno,6) +'-' + RIGHT(Nom.Cpno,7)  ELSE '' End AS Nom_Cpno";
                //else
                //    Tsql = Tsql + ", Case When  Nom.Cpno <> '' Then LEFT(Nom.Cpno,6) +'-' + '*******'  ELSE '' End  AS Nom_Cpno";
                //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<  

                Tsql = Tsql + ", Isnull(ETC_Addcode1,'') ETC_Addcode1 ";
                Tsql = Tsql + ", Isnull(ETC_Address1,'') ETC_Address1  ";
                Tsql = Tsql + ", Isnull(ETC_Address2,'') ETC_Address2  ";

                //Tsql = Tsql + ", Isnull(nationNameEng,'') nationNameEng , tbl_Memberinfo.Na_code ";
                Tsql = Tsql + ", Isnull(nationNameEng,'') nationNameEng , Isnull(nationNameKo,'') nationNameKo , tbl_Memberinfo.Na_code ";

                Tsql = Tsql + ", ISNULL(CP.Grade_Name,'')  G_Name ";
                Tsql = Tsql + " , ISNULL(CP.Grade_Name,'')  G_NameP  ";                
                
                Tsql = Tsql + ", isnull(MAuto.A_CardCode,'') A_CardCode ";
                Tsql = Tsql + ", isnull(MAuto.A_CardNumber,'') A_CardNumber ";
                Tsql = Tsql + ", isnull(MAuto.A_Period1,'') A_Period1 ";
                Tsql = Tsql + ", isnull(MAuto.A_Period2,'') A_Period2 ";
                Tsql = Tsql + ", isnull(MAuto.A_Card_Name_Number,'') A_Card_Name_Number ";
                Tsql = Tsql + ", isnull(MAuto.A_Start_Date,'') A_Start_Date ";
                Tsql = Tsql + ", isnull(MAuto.A_Month_Date,'') A_Month_Date ";
                Tsql = Tsql + ", isnull(MAuto.A_Stop_Date,'') A_Stop_Date ";

                Tsql = Tsql + ", isnull(MAuto.A_Rec_Name,'') A_Rec_Name ";
                Tsql = Tsql + ", isnull(MAuto.A_hptel,'') A_hptel ";
                Tsql = Tsql + ", isnull(MAuto.A_Addcode1,'') A_Addcode1 ";
                Tsql = Tsql + ", isnull(MAuto.A_Address1,'') A_Address1 ";
                Tsql = Tsql + ", isnull(MAuto.A_Address2,'') A_Address2 ";
                Tsql = Tsql + ", isnull(MAuto.A_ETC,'') A_ETC ";

                Tsql = Tsql + ", isnull(MAuto.A_ProcDay,'') A_ProcDay ";
                Tsql = Tsql + ", isnull(MAuto.A_ProcAmt,0) A_ProcAmt "; 

                Tsql = Tsql + ", isnull(tbl_Card.cardname,'') Card_Name";

                Tsql = Tsql + ", isnull(MAuto.Mbid2,0) A_Mbid2 ";


                Tsql = Tsql + ", isnull(tbl_Memberinfo.US_Num,0) US_Num ";
                Tsql = Tsql + ", isnull(tbl_Memberinfo.Third_Person_Agree,0)AS Third_Person_Agree ";
                Tsql = Tsql + ", isnull(tbl_Memberinfo.AgreeMarketing, 'N')AS AgreeMarketing ";

                Tsql = Tsql + ", tbl_Memberinfo.city ";
                Tsql = Tsql + ", tbl_Memberinfo.state ";

                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                Tsql = Tsql + " LEFT JOIN [mannaSync].[dbo].customer Sav (nolock) ON  Sav.[accountnumber] = Convert(varchar, tbl_Memberinfo.mbid2)  ";
                Tsql = Tsql + " LEFT JOIN [mannaSync].[dbo].customer Nom (nolock) ON  Nom.[accountnumber] =  Convert(varchar,tbl_Memberinfo.mbid2)  ";
                Tsql = Tsql + " LEFT JOIN [mannaSync].[dbo].customer Sav2 (nolock) ON  Sav.[sponsoralkynumber] = Sav2.[accountnumber]  ";
                Tsql = Tsql + " LEFT JOIN [mannaSync].[dbo].customer Nom2 (nolock) ON  Nom.[enrolleralkynumber] =  Nom2.[accountnumber]  ";

                Tsql = Tsql + " LEFT JOIN tbl_Memberinfo_Address MAdd (nolock) ON MAdd.Mbid = tbl_Memberinfo.Mbid And MAdd.Mbid2 = tbl_Memberinfo.Mbid2 And Sort_Add = 'R' ";

                Tsql = Tsql + " LEFT JOIN tbl_Memberinfo_A MAuto (nolock) ON MAuto.Mbid = tbl_Memberinfo.Mbid And MAuto.Mbid2 = tbl_Memberinfo.Mbid2 ";
                Tsql = Tsql + " LEFT JOIN tbl_Card (nolock) ON tbl_Card.Ncode = MAuto.A_CardCode "; 

                Tsql = Tsql + " LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";
                //Tsql = Tsql + " Left Join tbl_Bank (nolock) On tbl_Memberinfo.bankcode=tbl_Bank.ncode And tbl_Memberinfo.Na_code = tbl_Bank.Na_code ";
                Tsql = Tsql + " Left Join tbl_Bank (nolock) On tbl_Memberinfo.bankcode=tbl_Bank.ncode ";
                cls_NationService.SQL_BankNationCode(ref Tsql);
                Tsql = Tsql + " LEFT JOIN  tbl_Nation  (nolock) ON tbl_Nation.nationCode = tbl_Memberinfo.Na_Code  ";
                Tsql = Tsql + " Left Join tbl_Class CP On tbl_Memberinfo.CurGrade = CP.Grade_Cnt ";
                //Tsql = Tsql + " Left Join ufn_Mem_CurGrade_Mbid_Search ('" + Mbid + "'," + Mbid2.ToString() + ") AS CC_A On CC_A.Mbid = tbl_Memberinfo.Mbid And  CC_A.Mbid2 = tbl_Memberinfo.Mbid2 ";            

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                }

                //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
                Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";


                if (tab_Nation.Visible == true)
                {
                    if (combo_Se_Code.Text != "")
                    {
                        Tsql = Tsql + " And tbl_Memberinfo.Na_Code = '" + combo_Se_Code.Text + "'";
                    }
                }


                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text ) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                //++++++++++++++++++++++++++++++++
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                progress.Visible = true;
                progress.Maximum = 90; progress.Value = 10; progress.Refresh();
                Set_Form_Date(ds);
                progress.Value = progress.Value + 10; progress.Refresh();

                Set_Form_Date_Up(1);    //추천인 상선을 뿌려줌
                progress.Value = progress.Value + 10; progress.Refresh();

                Set_Form_Date_Up("S");  //후원인 상선을 뿌려줌
                progress.Value = progress.Value + 10; progress.Refresh();

                Set_Form_Date_Up(2);    //직추천한 사람들을 뿌려줌
                progress.Value = progress.Value + 10; progress.Refresh();

                Set_Form_Date_Up("S2");  //직후원한 사람들을 뿌려줌.
                progress.Value = progress.Value + 10; progress.Refresh();

                Base_Grid_Set();




                Set_Form_Date_Info(); //회원 매출 관련 뿌려줌   , 변경 정보, 수당 발생 내역 , 후원인 추천인 변경 내역 뿌려줌   
                progress.Value = progress.Value + 10; progress.Refresh();

                //chart_Item.Series.Clear();
                //Save_Nom_Line_Chart();
                //Set_SalesItemDetail(Mbid, Mbid2); //상품 관련 집계 도표를 뿌려준다.
                //Set_Form_Date_Talk(); //상담내역을 뿌려준다.
                progress.Value = progress.Value + 10; progress.Refresh();

                //Set_SalesDetail_Chart(Mbid, Mbid2); //pie 도표를 뿌려준다.
                progress.Value = progress.Value + 10; progress.Refresh();

                progress.Visible =false ;
                this.Cursor = System.Windows.Forms.Cursors.Default ;           
                            
                mtxtMbid.Focus();                
            }
            
            Data_Set_Form_TF = 0;            
        }

        private void Set_Form_Date(DataSet ds)
        {
            StringEncrypter decrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

            idx_Mbid =  ds.Tables[base_db_name].Rows[0]["Mbid"].ToString();
            idx_Mbid2 = int.Parse(ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString());
            
            mtxtMbid.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
            txtName.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
            mtxtSn.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Cpno"].ToString(), "Cpno");
            txtName_E_1.Text = ds.Tables[base_db_name].Rows[0]["E_name"].ToString();
            txtName_E_2.Text = ds.Tables[base_db_name].Rows[0]["E_name_Last"].ToString();
            txtLineCnt.Text = ds.Tables[base_db_name].Rows[0]["LineCnt"].ToString();
        
            txtMbid_s.Text = ds.Tables[base_db_name].Rows[0]["T_Saveid"].ToString();
            txtName_s.Text = ds.Tables[base_db_name].Rows[0]["Save_Name"].ToString();
            //txtSN_s.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Save_Cpno"].ToString(), "Cpno");

            txtMbid_n.Text = ds.Tables[base_db_name].Rows[0]["T_Nominid"].ToString();
            txtName_n.Text = ds.Tables[base_db_name].Rows[0]["Nomin_Name"].ToString();
           // txtSN_n.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Nom_Cpno"].ToString(), "Cpno");

            txtMbid_s2.Text = ds.Tables[base_db_name].Rows[0]["T_Saveid"].ToString();
            txtName_s2.Text = ds.Tables[base_db_name].Rows[0]["Save_Name"].ToString();
           // txtSN_s2.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Save_Cpno"].ToString(), "Cpno");

            txtMbid_n2.Text = ds.Tables[base_db_name].Rows[0]["T_Nominid"].ToString();
            txtName_n2.Text = ds.Tables[base_db_name].Rows[0]["Nomin_Name"].ToString();
            //txtSN_n2.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Nom_Cpno"].ToString(), "Cpno");
            
            txtGrade.Text = ds.Tables[base_db_name].Rows[0]["G_Name"].ToString();
            txtGradeP.Text = ds.Tables[base_db_name].Rows[0]["G_NameP"].ToString();

            if (ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString().Replace("-", "").Trim() != "")            
                txtLeaveDate.Text = string.Format("{0:####-##-##}", int.Parse(ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString()));//ds.Tables[base_db_name].Rows[0]["LeaveDate"].ToString();

            if (ds.Tables[base_db_name].Rows[0]["LineUserDate"].ToString().Replace("-", "").Trim() != "")            
                txtLineDate.Text = string.Format("{0:####-##-##}", int.Parse(ds.Tables[base_db_name].Rows[0]["LineUserDate"].ToString()));//ds.Tables[base_db_name].Rows[0]["LineUserDate"].ToString();

            if (ds.Tables[base_db_name].Rows[0]["PayStop_Date"].ToString().Replace("-", "").Trim() != "")            
                txtS.Text = string.Format("{0:####-##-##}", int.Parse(ds.Tables[base_db_name].Rows[0]["PayStop_Date"].ToString()));  //ds.Tables[base_db_name].Rows[0]["PayStop_Date"].ToString();

            if (ds.Tables[base_db_name].Rows[0]["Regtime"].ToString().Replace("-", "").Trim() != "")     
                mtxtRegDate.Text =  string.Format("{0:####-##-##}", int.Parse(ds.Tables[base_db_name].Rows[0]["Regtime"].ToString())); //ds.Tables[base_db_name].Rows[0]["Regtime"].ToString();

            if (ds.Tables[base_db_name].Rows[0]["Ed_Date"].ToString().Replace("-", "").Trim() != "")         
                mtxtEdDate.Text = string.Format("{0:####-##-##}", int.Parse(ds.Tables[base_db_name].Rows[0]["Ed_Date"].ToString()));  // ds.Tables[base_db_name].Rows[0]["Ed_Date"].ToString();

            if (ds.Tables[base_db_name].Rows[0]["state"].ToString().Replace("-", "").Trim() != "")
            {
                cbProvince_TH.Text = ds.Tables[base_db_name].Rows[0]["state"].ToString();
            }

            if (ds.Tables[base_db_name].Rows[0]["city"].ToString().Replace("-", "").Trim() != "")
            {
                cbDistrict_TH.Text = ds.Tables[base_db_name].Rows[0]["city"].ToString();
            }

            if (ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Replace ("-","").Trim () != "")
            {
                // 태국 국가코드인 경우
                if (ds.Tables[base_db_name].Rows[0]["Na_code"].ToString().Replace("-", "").Trim() == "TH")
                {
                    //cbZipCode_TH.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString();
                    try
                    {
                        cbProvince_TH.Text = ds.Tables[base_db_name].Rows[0]["Address2"].ToString().Split(' ')[2];
                        cbDistrict_TH.Text = ds.Tables[base_db_name].Rows[0]["Address2"].ToString().Split(' ')[1];
                        cbSubDistrict_TH.Text = ds.Tables[base_db_name].Rows[0]["Address2"].ToString().Split(' ')[0];
                    }
                    catch (Exception)
                    {
                        cbProvince_TH.Text = "";
                        cbDistrict_TH.Text = "";
                        cbSubDistrict_TH.Text = "";
                    }

                    txtZipCode_TH.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString();
                    Update();
                }
                else
                {
                    //txtAddCode1.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Substring(0, 3);
                    //txtAddCode2.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Substring(3, 3);                
                    mtxtZip1.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString();
                }
                ////txtAddCode1.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Substring(0, 3);
                ////txtAddCode2.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Substring(3, 3);                
                //mtxtZip1.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString(); 
            }
            txtAddress1.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Address1"].ToString());
            txtAddress2.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Address2"].ToString());


            string T_tel = "";
            if (decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hometel"].ToString()).Replace("-", "").Trim() != "")
            {
                //string[] tel = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hometel"].ToString()).Split('-');
                //txtTel_1.Text = tel[0].ToString ();
                //txtTel_2.Text = tel[1].ToString();
                //txtTel_3.Text = tel[2].ToString();
                new cls_form_Meth().Home_Number_Setting(ds.Tables[base_db_name].Rows[0]["hometel"].ToString(), mtxtTel1);

                // T_tel = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hometel"].ToString());
            }

            if (decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hptel"].ToString()).Replace("-", "").Trim() != "")
            {
                //string[] tel = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hptel"].ToString()).Split('-');
                //txtTel2_1.Text = tel[0].ToString();
                //txtTel2_2.Text = tel[1].ToString();
                //txtTel2_3.Text = tel[2].ToString();

                mtxtTel2.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["hptel"].ToString());
            }


            txtCenter.Text = ds.Tables[base_db_name].Rows[0]["B_Name"].ToString();
            txtCenter_Code.Text = ds.Tables[base_db_name].Rows[0]["businesscode"].ToString();

            txtBank.Text = ds.Tables[base_db_name].Rows[0]["bank_Name"].ToString();
            txtBank_Code.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["bankcode"].ToString());
            txtAccount.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["bankaccnt"].ToString());                       
            txtName_Accnt.Text = ds.Tables[base_db_name].Rows[0]["bankowner"].ToString();

            txtAccount_Reg.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Reg_bankaccnt"].ToString());


            txtWebID.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["webid"].ToString());
            txtPassword.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["webpassword"].ToString());
            idx_Password = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["webpassword"].ToString());

            txtEmail.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["Email"].ToString());
            txtRemark.Text = ds.Tables[base_db_name].Rows[0]["Remarks"].ToString();

            txtB1.Text = ds.Tables[base_db_name].Rows[0]["GiBu_"].ToString();

            mtxtVisaDay.Text = ds.Tables[base_db_name].Rows[0]["VisaDate"].ToString();
           




            string BirthDay = ds.Tables[base_db_name].Rows[0]["BirthDay"].ToString();
            if (BirthDay != "")
            {
                BirthDay = BirthDay + "-" +  ds.Tables[base_db_name].Rows[0]["BirthDay_M"].ToString();
                BirthDay = BirthDay + "-" +  ds.Tables[base_db_name].Rows[0]["BirthDay_D"].ToString();

                mtxtBrithDay.Text = BirthDay;
            }

            //소비자는 1 판매원은 기본 0
            if (ds.Tables[base_db_name].Rows[0]["Sell_Mem_TF"].ToString() == "1")
                opt_sell_3.Checked = true;
            else
                opt_sell_2.Checked = true;

            // 내국인은 0 외국인은 1  사업자는 2
            if (ds.Tables[base_db_name].Rows[0]["For_Kind_TF"].ToString() == "0")
                raButt_IN_1.Checked = true;
            else if (ds.Tables[base_db_name].Rows[0]["For_Kind_TF"].ToString() == "1")
                raButt_IN_2.Checked = true;
            else
                raButt_IN_3.Checked = true;

            //양력은 1  음력은 2
            if (ds.Tables[base_db_name].Rows[0]["BirthDayTF"].ToString() == "1")
                opt_Bir_TF_1.Checked = true;
            else
                opt_Bir_TF_2.Checked = true;


            if (ds.Tables[base_db_name].Rows[0]["RBO_Mem_TF"].ToString() == "0")
                radioB_RBO.Checked = true;
            else
                radioB_Begin.Checked = true;
            mtxtRBODate.Text = ds.Tables[base_db_name].Rows[0]["RBO_S_Date"].ToString();   


            if (ds.Tables[base_db_name].Rows[0]["G8_TF"].ToString() == "8")
                radioB_G8.Checked = true;
            else
                radioB_G4.Checked = true;

            

            check_MyOffice.Checked = false;
            if (ds.Tables[base_db_name].Rows[0]["Myoffice_TF"].ToString() == "1")
                check_MyOffice.Checked = true;
            //제3자동의
            if (ds.Tables[base_db_name].Rows[0]["Third_Person_Agree"].ToString() == "1")
                checkB_Third_Person_Agree.Checked = true;
            else
                checkB_Third_Person_Agree.Checked = false;
            //마케팅수신동의
            if (ds.Tables[base_db_name].Rows[0]["AgreeMarketing"].ToString() == "Y")
                checkB_AgreeMarketing.Checked = true;
            else
                checkB_AgreeMarketing.Checked = false;


            if (ds.Tables[base_db_name].Rows[0]["Saveid"].ToString() != "")
            {
                if (ds.Tables[base_db_name].Rows[0]["Saveid"].ToString().Substring(0, 1) == "*")
                    chk_S.Checked = true;
            }

            if (ds.Tables[base_db_name].Rows[0]["Nominid"].ToString() != "")
            {
                if (ds.Tables[base_db_name].Rows[0]["Nominid"].ToString().Substring(0, 1) == "*")
                    chk_N.Checked = true;
            }

            if (int.Parse(ds.Tables[base_db_name].Rows[0]["Add_TF"].ToString()) == 1)
                opt_B_1.Checked = true;
            else if (int.Parse(ds.Tables[base_db_name].Rows[0]["Add_TF"].ToString()) == 2)
                opt_B_2.Checked = true;
            else if (int.Parse(ds.Tables[base_db_name].Rows[0]["Add_TF"].ToString()) == 3)
                opt_B_3.Checked = true;
            else
            {
                opt_B_1.Checked = false; opt_B_2.Checked = false; opt_B_3.Checked = false;
            }

            if (int.Parse(ds.Tables[base_db_name].Rows[0]["BankDocument"].ToString()) == 1)
                check_BankDocument.Checked = true;

            if (int.Parse(ds.Tables[base_db_name].Rows[0]["CpnoDocument"].ToString()) == 1)
                check_CpnoDocument.Checked = true;

            radioB_Sex_X.Checked = false;
            radioB_Sex_Y.Checked = false;
            if (ds.Tables[base_db_name].Rows[0]["Sex_FLAG"].ToString() == "X")
                radioB_Sex_X.Checked = true;

            if (ds.Tables[base_db_name].Rows[0]["Sex_FLAG"].ToString() == "Y")
                radioB_Sex_Y.Checked = true;


            if (ds.Tables[base_db_name].Rows[0]["AgreeSMS"].ToString() == "Y")
                checkB_SMS_FLAG.Checked = true;
            else
                checkB_SMS_FLAG.Checked = false;


            if (ds.Tables[base_db_name].Rows[0]["AgreeEmail"].ToString() == "Y")
                checkB_EMail_FLAG.Checked = true;
            else
                checkB_EMail_FLAG.Checked = false;


            if (ds.Tables[base_db_name].Rows[0]["ETC_Addcode1"].ToString().Replace("-", "").Trim() != "")
            {
                //txtAddCode1.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Substring(0, 3);
                //txtAddCode2.Text = ds.Tables[base_db_name].Rows[0]["Addcode1"].ToString().Substring(3, 3);                
                mtxtZip2.Text = ds.Tables[base_db_name].Rows[0]["ETC_Addcode1"].ToString();
            }
            txtAddress3.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["ETC_Address1"].ToString());
            txtAddress4.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["ETC_Address2"].ToString());

            combo_Se.Text = ds.Tables[base_db_name].Rows[0]["nationNameEng"].ToString();
            combo_Se_Code.Text = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();

            // 접속 id가 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                combo_Se_2.Text = ds.Tables[base_db_name].Rows[0]["nationNameEng"].ToString();
            }
            // 그 외 국가인 경우
            else
            {
                combo_Se_2.Text = ds.Tables[base_db_name].Rows[0]["nationNameKo"].ToString();
            }
            combo_Se_Code_2.Text = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();

            radioB_Sex_X.Checked = ds.Tables[base_db_name].Rows[0]["Sex_FLAG"].ToString() == "X";
            radioB_Sex_Y.Checked = ds.Tables[base_db_name].Rows[0]["Sex_FLAG"].ToString() == "Y";
            checkB_SMS_FLAG.Checked = ds.Tables[base_db_name].Rows[0]["AgreeSMS"].ToString() == "Y";
            checkB_EMail_FLAG.Checked = ds.Tables[base_db_name].Rows[0]["AgreeEmail"].ToString() == "Y";


            if (ds.Tables[base_db_name].Rows[0]["C_M_Name"].ToString() != "")
            {
                check_CC.Checked = true;
                txtName_C.Text = ds.Tables[base_db_name].Rows[0]["C_M_Name"].ToString();
                mtxtSn_C.Text = decrypter.Decrypt(ds.Tables[base_db_name].Rows[0]["C_cpno"].ToString());

                txtName_E_1_C.Text = ds.Tables[base_db_name].Rows[0]["C_E_name"].ToString();
                txtName_E_2_C.Text = ds.Tables[base_db_name].Rows[0]["C_E_name_Last"].ToString();
                                
                BirthDay = ds.Tables[base_db_name].Rows[0]["C_BirthDay"].ToString();
                if (BirthDay != "")
                {
                    BirthDay = BirthDay + "-" + ds.Tables[base_db_name].Rows[0]["C_BirthDay_M"].ToString();
                    BirthDay = BirthDay + "-" + ds.Tables[base_db_name].Rows[0]["C_BirthDay_D"].ToString();                                        
                    mtxtBrithDayC.Text = BirthDay;                    
                }

                // 내국인은 0 외국인은 1  사업자는 2
                if (ds.Tables[base_db_name].Rows[0]["C_For_Kind_TF"].ToString() == "0")
                    raButt_IN_1_C.Checked = true;
                else if (ds.Tables[base_db_name].Rows[0]["C_For_Kind_TF"].ToString() == "1")
                    raButt_IN_2_C.Checked = true;

                mtxtTel2_C.Text = ds.Tables[0].Rows[0]["C_hptel"].ToString();
                txtEmail_C.Text = ds.Tables[0].Rows[0]["C_Email"].ToString();
            }


            button_exigo.Visible = false;

            //if (int.Parse (ds.Tables[base_db_name].Rows[0]["US_Num"].ToString()) == 0 )
            //    button_exigo.Visible = true;


            txt_Us.Text = ds.Tables[base_db_name].Rows[0]["US_Num"].ToString(); 


            txtName.ReadOnly = true;
            txtName.BackColor = cls_app_static_var.txt_Enable_Color; 
            txtName.BorderStyle = BorderStyle.FixedSingle;
        }

        private void Set_Form_Date_Up(int intTemp) //추천 관련.
        {
            if (intTemp ==1 ) //추천상위
                dGridView_Up_S_Header_Reset(dGridView_Up_N); //디비그리드 헤더와 기본 셋팅을 한다.
            else
                dGridView_Up_S_Header_Reset(dGridView_Down_N); //디비그리드 헤더와 기본 셋팅을 한다.

            cg_Up_S.d_Grid_view_Header_Reset();

            if (intTemp == 1) //추천상위
            {
                if (chk_N.Checked == true) return; //최상위 이면 상선 내역을 보여줄 필요가 없다.            
                Base_Grid_Set(" ufn_Up_Search_Nomin ");
            }
            else
            {
                Base_Grid_Down_Set("N");
            }
        }


        private void Set_Form_Date_Up(string strTemp)
        {
            if (strTemp == "S")
                dGridView_Up_S_Header_Reset(dGridView_Up_S); //디비그리드 헤더와 기본 셋팅을 한다.
            else
                dGridView_Up_S_Header_Reset(dGridView_Down_S); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Up_S.d_Grid_view_Header_Reset();

            if (strTemp == "S")
            {
                if (chk_S.Checked == true) return;     //최상위 이면 상선 내역을 보여줄 필요가 없다.   
                Base_Grid_Set(" ufn_Up_Search_Save ");
            }
            else
            {
                Base_Grid_Down_Set("S");
            }
        }

        private void Set_Form_Date_Info()
        {
            cls_Grid_Base_info_Put cgbp = new cls_Grid_Base_info_Put();
            cgbp.dGridView_Put_baseinfo(this, dGridView_Sell, "sell", mtxtMbid.Text);

            cls_Grid_Base_info_Put cgbp2 = new cls_Grid_Base_info_Put();
            cgbp2.dGridView_Put_baseinfo(this, dGridView_inf, "memc", mtxtMbid.Text);

            cls_Grid_Base_info_Put cgbp3 = new cls_Grid_Base_info_Put();
            cgbp3.dGridView_Put_baseinfo(this, dGridView_Up, "memupc", mtxtMbid.Text);

            cls_Grid_Base_info_Put cgbp4 = new cls_Grid_Base_info_Put();
            cgbp4.dGridView_Put_baseinfo(this, dGridView_Add, "memadd", mtxtMbid.Text);

            cls_Grid_Base_info_Put cgbp5 = new cls_Grid_Base_info_Put();
            cgbp5.dGridView_Put_baseinfo(this, dGridView_Talk, "talk", mtxtMbid.Text);


            cg_Up_S.d_Grid_view_Header_Reset();

            Base_Grid_info_Set(5);

            //dGridView_Info_Header_Reset(dGridView_inf, 2);
            //cg_Up_S.d_Grid_view_Header_Reset();

            //Base_Grid_info_Set(2);

            //dGridView_Info_Header_Reset(dGridView_Up, 3);
            //cg_Up_S.d_Grid_view_Header_Reset();

            //Base_Grid_info_Set(3);


            //dGridView_Info_Header_Reset(dGridView_Add, 4);
            //cg_Up_S.d_Grid_view_Header_Reset();

            //Base_Grid_info_Set(4);
        }


       
       

        private void Base_Grid_Set(string Ufn_Name  )
        {            
            string T_Mbid   = "" ;            
            T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;            
            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) != 1) return;
            
            string Tsql = "";

            Tsql = "Select  ";
            
            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + " T_up.mbid + '-' + Convert(Varchar,T_up.mbid2) ";
            else
                Tsql = Tsql + " T_up.mbid2 ";

            Tsql = Tsql + " ,T_up.M_Name ";
            Tsql = Tsql + " ,T_up.curP ";

            Tsql = Tsql + " From " + Ufn_Name ;
            Tsql = Tsql + " ('" + Mbid + "'," + Mbid2.ToString () + ") AS T_up";
            
            Tsql = Tsql + " Where    lvl > 0 ";
            Tsql = Tsql + " Order BY lvl Desc ";

            //당일 등록된 회원을 불러온다.

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
                Set_gr_dic_Line(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cg_Up_S.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cg_Up_S.db_grid_Obj_Data_Put();            
        }



        private void Base_Grid_Down_Set(string tSort)
        {
            string T_Mbid = "";
            T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            cls_Search_DB csb = new cls_Search_DB();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) != 1) return;

            string Tsql = "";

            Tsql = "Select  ";

            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) ";
            else
                Tsql = Tsql + " tbl_Memberinfo.mbid2 ";

            Tsql = Tsql + " ,tbl_Memberinfo.M_Name ";

            if (tSort == "S")
            {
                Tsql = Tsql + " ,tbl_Memberinfo.LineCnt ";
                Tsql = Tsql + " From tbl_Memberinfo " ;
                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where Saveid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where Saveid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   Saveid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " Order By LineCnt ASC ";
            }
            else
            {
                Tsql = Tsql + " ,tbl_Memberinfo.N_LineCnt ";
                Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where Nominid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where Nominid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   Nominid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " Order By N_LineCnt ASC ";
            }

            //당일 등록된 회원을 불러온다.

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
                Set_gr_dic_Line(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cg_Up_S.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cg_Up_S.db_grid_Obj_Data_Put();
        }



        private void Set_gr_dic_Line(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]  
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                //,ds.Tables[base_db_name].Rows[fi_cnt][3]
                                //,ds.Tables[base_db_name].Rows[fi_cnt][4]                                                               
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }



        private void dGridView_Up_S_Header_Reset(DataGridView t_Dgv)
        {
            cg_Up_S.Grid_Base_Arr_Clear();

            cg_Up_S.grid_col_Count = 5;
            cg_Up_S.basegrid = t_Dgv; //dGridView_Up_S;
            cg_Up_S.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cg_Up_S.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원_번호"  , "성명"   , "위치"  , ""   , ""        
                                    };
            cg_Up_S.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 60, 70, 30, 0, 0                               
                            };
            cg_Up_S.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                                                                                   
                                   };
            cg_Up_S.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter  //5      
                              };
            cg_Up_S.grid_col_alignment = g_Alignment;
            cg_Up_S.basegrid.RowHeadersWidth = 25;

            cg_Up_S.basegrid.ColumnHeadersDefaultCellStyle.Font =
            new Font(cg_Up_S.basegrid.Font.FontFamily, 8);
        }













        private void Base_Grid_info_Set(int intTemp)
        {
            string T_Mbid = "";
            T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            cls_Search_DB csb = new cls_Search_DB();
            cls_form_Meth cm = new cls_form_Meth();
            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) != 1) return;

            string Tsql = "";

            Tsql = "Select  ";
            if (intTemp == 1)
            {                            

                Tsql = Tsql + " SellDate ";
                Tsql = Tsql + " ,OrderNumber ";
                Tsql = Tsql + " ,SellTypeName ";
                //Tsql = Tsql + " ,Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " Ch_Detail ";
                Tsql = Tsql + " ,Case When ReturnTF = 1 Then '" + cm._chang_base_caption_search("정상") + "'";
                Tsql = Tsql + "  When ReturnTF = 2 Then '" + cm._chang_base_caption_search("반품") + "'";
                Tsql = Tsql + "  When ReturnTF = 4 Then '" + cm._chang_base_caption_search("교환") + "'";
                Tsql = Tsql + "  When ReturnTF = 3 Then '" + cm._chang_base_caption_search("부분반품") + "'";
                Tsql = Tsql + "  When ReturnTF = 5 Then '" + cm._chang_base_caption_search("취소") + "'";
                Tsql = Tsql + " END ";

                Tsql = Tsql + " ,TotalPrice ";
                Tsql = Tsql + " ,TotalInputPrice ";
                Tsql = Tsql + " ,TotalPV ";


                Tsql = Tsql + " ,InputCash ";
                Tsql = Tsql + " ,InputCard ";
                Tsql = Tsql + " ,InputPassbook ";
                Tsql = Tsql + " ,Etc1 ";

                Tsql = Tsql + " From tbl_SalesDetail (nolock) ";
                Tsql = Tsql + " Left Join tbl_SellType (nolock) On tbl_SellType.SellCode =tbl_SalesDetail.SellCode ";
                Tsql = Tsql + " LEFT JOIN tbl_Base_Change_Detail Ch_T (nolock) ON Ch_T.M_Detail_S = 'tbl_SalesDetail' And  Ch_T.M_Detail = Convert(Varchar,tbl_SalesDetail.ReturnTF ) ";
                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " Order By OrderNumber ASC ";
            }
            else if (intTemp == 2)
            {

                Tsql = Tsql + " A.ModRecordtime ";
                Tsql = Tsql + " ,Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " Ch_Detail ";
                Tsql = Tsql + " ,BeforeDetail ";
                Tsql = Tsql + " ,AfterDetail ";
                Tsql = Tsql + " ,A.ModRecordid ";

                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";

                Tsql = Tsql + " FROM tbl_Memberinfo_Mod AS A (nolock) " ;
                Tsql = Tsql + " LEFT JOIN tbl_Memberinfo_Mod_Detail Ch_T  (nolock) ON Ch_T.M_Detail = A.ChangeDetail";
                Tsql = Tsql + " LEFT JOIN tbl_Memberinfo AS B  (nolock) ON A.Mbid = B.Mbid And A.Mbid2 = B.Mbid2 ";
                Tsql = Tsql + " LEFT JOIN tbl_Business         (nolock) ON B.BusinessCode = tbl_Business.ncode  And B.Na_code = tbl_Business.Na_code ";

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where B.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where b.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   B.Mbid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " And Ch_T." + cls_app_static_var.Base_M_Detail_Ex + " IS NOT NULL ";
                Tsql = Tsql + " Order By Modrecordtime DESC ";
            }

            else if (intTemp == 3)
            {
                Tsql = Tsql + " tbl_Memberinfo_Save_Nomin_Change.recordtime ";

                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + ", tbl_Memberinfo_Save_Nomin_Change.Old_mbid + '-' + Convert(Varchar,tbl_Memberinfo_Save_Nomin_Change.Old_mbid2) ";
                else
                    Tsql = Tsql + ", tbl_Memberinfo_Save_Nomin_Change.Old_mbid2 ";
                Tsql = Tsql + " ,A.M_name AS oldname ";

                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + ", tbl_Memberinfo_Save_Nomin_Change.New_mbid + '-' + Convert(Varchar,tbl_Memberinfo_Save_Nomin_Change.New_mbid2) ";
                else
                    Tsql = Tsql + ", tbl_Memberinfo_Save_Nomin_Change.New_mbid2 ";
                Tsql = Tsql + " ,B.M_name AS Newname";

                Tsql = Tsql + " , Ch_T." + cls_app_static_var.Base_M_Detail_Ex + "  Ch_Detail ";
                Tsql = Tsql + " ,tbl_Memberinfo_Save_Nomin_Change.Recordid ";

                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";


                Tsql = Tsql + " FROM      tbl_Memberinfo_Save_Nomin_Change  (nolock) ";

                Tsql = Tsql + " Left JOIN tbl_Memberinfo A (nolock)  ON";
                Tsql = Tsql + " tbl_Memberinfo_Save_Nomin_Change.Old_mbid = A.mbid ";
                Tsql = Tsql + " And tbl_Memberinfo_Save_Nomin_Change.Old_mbid2 = A.mbid2 ";

                Tsql = Tsql + " Left Join tbl_Memberinfo B (nolock) ON ";
                Tsql = Tsql + " tbl_Memberinfo_Save_Nomin_Change.New_mbid = B.Mbid";
                Tsql = Tsql + " And tbl_Memberinfo_Save_Nomin_Change.New_mbid2 = B.Mbid2";

                Tsql = Tsql + " LEFT JOIN tbl_Base_Change_Detail Ch_T (nolock) ON Ch_T.M_Detail_S = 'tbl_Memberinfo_Save_Nomin_Change' And  Ch_T.M_Detail = tbl_Memberinfo_Save_Nomin_Change.Save_Nomin_SW ";

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo_Save_Nomin_Change.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo_Save_Nomin_Change.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo_Save_Nomin_Change.Mbid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " Order By tbl_Memberinfo_Save_Nomin_Change.recordtime DESC  ";
            }

            else if (intTemp == 4)
            {
              
                Tsql = Tsql + " Case When Sort_Add = 'C' Then '" + cm._chang_base_caption_search("직장") + "'";
                Tsql = Tsql + "  When Sort_Add = 'R' Then '" + cm._chang_base_caption_search("기본배송지") + "'";
                Tsql = Tsql + " END ";

                Tsql = Tsql + " ,ETC_Addcode1   ";
                Tsql = Tsql + " ,ETC_Address1 ";
                Tsql = Tsql + " ,ETC_Address2 ";

                Tsql = Tsql + " ,ETC_Tel_1 ";
                Tsql = Tsql + " ,ETC_Tel_2 ";
                Tsql = Tsql + " ,ETC_Name ";


                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";

                Tsql = Tsql + " From tbl_Memberinfo_Address (nolock) ";

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " Order By Sort_Add ASC ";
            }

            else if (intTemp == 5)
            {

                Tsql = Tsql + " OrderDate ";
                Tsql = Tsql + " ,Gid   ";
                Tsql = Tsql + " ,Case When Send_Result =1 then '성공' ELSE '실패' End Send_Result ";
                Tsql = Tsql + ", Send_Error ";
                Tsql = Tsql + " ,RecordTime ";


                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";
                Tsql = Tsql + " ,'' ";

                Tsql = Tsql + " ,'' ";

                Tsql = Tsql + " From tbl_Memberinfo_Ca_A_Monthly_Mod (nolock) ";

                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " Order By Base_Index dESC ";
            }

            //당일 등록된 회원을 불러온다.

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
            int T_cnt = 0;
            double S_cnt4 = 0;    double S_cnt5 = 0;    double S_cnt6 = 0;    double S_cnt7 = 0;   double S_cnt8 = 0;   double S_cnt9 = 0;
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_Info(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
                T_cnt = fi_cnt;
                if (intTemp == 1)
                {
                    S_cnt4 = S_cnt4 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt][4].ToString() );
                    S_cnt5 = S_cnt5 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt][5].ToString());
                    S_cnt6 = S_cnt6 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt][6].ToString());
                    S_cnt7 = S_cnt7 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt][7].ToString());
                    S_cnt8 = S_cnt8 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt][8].ToString());
                    S_cnt9 = S_cnt9 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt][9].ToString());
                }
            }


            if (intTemp == 1)
            {
                object[] row0 = { ""
                                    ,"<< " + cm._chang_base_caption_search("합계") + " >>"
                                    ,""
                                    ,""
                                    ,S_cnt4

                                    ,S_cnt5
                                    ,S_cnt6
                                    ,S_cnt7
                                    ,S_cnt8
                                    ,S_cnt9

                                    ,""
                                     };

                gr_dic_text[T_cnt + 2] = row0;
            }


            cg_Up_S.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cg_Up_S.db_grid_Obj_Data_Put();
        }




        private void Set_gr_dic_Info(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
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
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }

        private void Make_Base_Query(ref string Tsql)
        {
            Tsql = " SELECT ";
            Tsql += Environment.NewLine + "   tbl_Memberinfo_AutoShip.Auto_Seq ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo_AutoShip.mbid2 ";
            Tsql += Environment.NewLine + " , tbl_Memberinfo.M_Name ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_Date <> '' THEN ";
            Tsql += Environment.NewLine + " LEFT(tbl_Memberinfo_AutoShip.Req_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Req_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Req_Date, 7, 2) ELSE '' END Req_Date ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '10' THEN CASE WHEN CONVERT(VARCHAR, tbl_Memberinfo_AutoShip.Proc_Cnt) = '0' THEN '' ELSE CONVERT(VARCHAR, tbl_Memberinfo_AutoShip.Proc_Cnt) END ELSE '' END Last_Cnt ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '10' THEN ";
            Tsql += Environment.NewLine + " CASE WHEN ISNULL(A.SellDate , '') <> '' THEN LEFT(A.SellDate, 4) + '-' + SUBSTRING(A.SellDate, 5, 2) + '-' + SUBSTRING(A.SellDate, 7, 2)  ELSE '' END ";
            Tsql += Environment.NewLine + " ELSE '' END Last_Date ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN CONVERT(VARCHAR, tbl_Memberinfo_AutoShip.Proc_Cnt) ELSE '' END AS Next_Cnt ";
            Tsql += Environment.NewLine + " , CASE WHEN tbl_Memberinfo_AutoShip.Req_State <> '99' THEN CONVERT(VARCHAR, CAST(PROC_DATE AS DATETIME), 23)  ELSE '' END AS Next_Date";
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
           

            //if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() == ""))
            //    Tsql += Environment.NewLine + " And tbl_Memberinfo_AutoShip.Req_Date = '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";

            ////가입일자로 검색 -2
            //if ((mtxtRegDate1.Text.Replace("-", "").Trim() != "") && (mtxtRegDate2.Text.Replace("-", "").Trim() != ""))
            //{
            //    Tsql += Environment.NewLine + " And tbl_Memberinfo_AutoShip.Req_Date >= '" + mtxtRegDate1.Text.Replace("-", "").Trim() + "'";
            //    Tsql += Environment.NewLine + " And tbl_Memberinfo_AutoShip.Req_Date <= '" + mtxtRegDate2.Text.Replace("-", "").Trim() + "'";
            //}

            Tsql += Environment.NewLine + " ORDER BY tbl_Memberinfo_AutoShip.Auto_Seq DESC ";


        }

        private void Base_Grid_Set()
        {

            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_Auto.d_Grid_view_Header_Reset(1);

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
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
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
            cgb_Auto.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Auto.db_grid_Obj_Data_Put();
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

                            };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        private void dGridView_Base_Header_Reset()
        {
            cgb_Auto.grid_col_Count = 16;
            cgb_Auto.basegrid = dGridView_Base;
            cgb_Auto.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
        

            string[] g_HeaderText = { "자동주문번호" , "회원번호", "회원명" , "신청일자"
                                    , "마지막주문회차", "마지막주문일자", "다음주문회차", "다음주문일자", "상태"
                                    , "해지사유", "해지일자", "총 금액", "총 PV", "총 CV"
                                    , "_상태","기록자"
                                    };

            string[] g_ColsName = {"auto_seq" , "mbid2", "m_name" , "req_date"
                                    , "LastCnt", "Lastdate", "NextCnt", "NextDate", "status"
                                    , "EndETC", "EndDate", "PR", "PV", "BV"
                                    , "_status" , "RecordName"
                                    };
            cgb_Auto.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 120, 100, 100, 100
                            , 100, 120, 120, 100,120
                            , 100, 80, 120, 80, 80
                            , 0, 100
                            };
            cgb_Auto.grid_col_w = g_Width;


            Boolean[] g_ReadOnly = {true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                    ,true , true
                                   };
            cgb_Auto.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {
                                DataGridViewContentAlignment.MiddleLeft
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

                              };
            cgb_Auto.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[12 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[13 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[14 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_Auto.grid_cell_format = gr_dic_cell_format;
        }


        private void dGridView_Info_Header_Reset(DataGridView t_Dgv, int intTemp)
        {
            cg_Up_S.Grid_Base_Arr_Clear();
            cg_Up_S.basegrid = t_Dgv; 
            cg_Up_S.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;            
            cg_Up_S.grid_col_Count = 11;

            //cg_sub.grid_Frozen_End_Count = 2;
            cg_Up_S.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            

            if (intTemp == 1)
            {
                string[] g_HeaderText = {"매출_일자" ,  "주문번호" ,  "주문_종류"   , "상태"  , "매출액"  
                                        , "입급액"  ,"매출PV"  , "현금"  , "카드" , "무통장" 
                                        , "비고"
                                        };

                Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
                gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;

                cg_Up_S.grid_cell_format = gr_dic_cell_format;

                int[] g_Width = { 100, 90, 70, 80, 80
                                 ,80 , 80 , 80 , 80 , 80
                                 ,100
                                };

                DataGridViewContentAlignment[] g_Alignment =
                                  {DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleRight  //5    
  
                                   ,DataGridViewContentAlignment.MiddleRight 
                                   ,DataGridViewContentAlignment.MiddleRight  
                                   ,DataGridViewContentAlignment.MiddleRight
                                   ,DataGridViewContentAlignment.MiddleRight  //10

                                   ,DataGridViewContentAlignment.MiddleCenter  //10
                                  };

                cg_Up_S.grid_col_header_text = g_HeaderText;
                cg_Up_S.grid_col_w = g_Width;
                cg_Up_S.grid_col_alignment = g_Alignment;
            }
            else if (intTemp == 2)
            {
                string[] g_HeaderText = {"변경일"  , "변경내역"   , "전_내역"  , "후_내역"   , "변경자"        
                                    , ""   , ""    , ""  , "" , ""
                                    ,""
                                    };

                int[] g_Width = { 120, 100, 100, 100, 80
                                 ,0 , 0 , 0 , 0 , 0
                                 ,0
                                };

                DataGridViewContentAlignment[] g_Alignment =
                                  {DataGridViewContentAlignment.MiddleLeft
                                   ,DataGridViewContentAlignment.MiddleLeft 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleCenter  //5    
  
                                   ,DataGridViewContentAlignment.MiddleCenter 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleCenter  //10

                                   ,DataGridViewContentAlignment.MiddleCenter  //10
                                  };

                cg_Up_S.grid_col_header_text = g_HeaderText;
                cg_Up_S.grid_col_w = g_Width;
                cg_Up_S.grid_col_alignment = g_Alignment;
            }

            else if (intTemp == 3)
            {
                string[] g_HeaderText = {"변경일"  , "전_상위번호"   , "전_상위성명"  , "후_상위번호"   , "후_상위성명"        
                                    , "구분"   , "변경자"    , ""  , "" , ""
                                    ,""
                                    };

                int[] g_Width = { 120, 100, 100, 100, 100
                                 ,80 , 80 , 0 , 0 , 0
                                 ,0
                                };

                DataGridViewContentAlignment[] g_Alignment =
                                  {DataGridViewContentAlignment.MiddleLeft
                                   ,DataGridViewContentAlignment.MiddleCenter 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleCenter  //5    
  
                                   ,DataGridViewContentAlignment.MiddleCenter 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleCenter  //10

                                   ,DataGridViewContentAlignment.MiddleCenter  //10
                                  };

                cg_Up_S.grid_col_header_text = g_HeaderText;
                cg_Up_S.grid_col_w = g_Width;
                cg_Up_S.grid_col_alignment = g_Alignment;
            }

            else if (intTemp == 4)
            {
                string[] g_HeaderText = {"구분"  , "우편_번호"   , "주소1"  , "주소2"   , "연락처1"        
                                    , "연락처2"   , "수취인명"    , ""  , "" , ""
                                    ,""
                                    };

                int[] g_Width = { 120, 100, 100, 100, 100
                                 ,80 , 80 , 0 , 0 , 0
                                 ,0
                                };

                DataGridViewContentAlignment[] g_Alignment =
                                  {DataGridViewContentAlignment.MiddleLeft
                                   ,DataGridViewContentAlignment.MiddleCenter 
                                   ,DataGridViewContentAlignment.MiddleLeft  
                                   ,DataGridViewContentAlignment.MiddleLeft
                                   ,DataGridViewContentAlignment.MiddleLeft  //5    
  
                                   ,DataGridViewContentAlignment.MiddleLeft 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleCenter  //10

                                   ,DataGridViewContentAlignment.MiddleCenter  //10
                                  };

                cg_Up_S.grid_col_header_text = g_HeaderText;
                cg_Up_S.grid_col_w = g_Width;
                cg_Up_S.grid_col_alignment = g_Alignment;
            }



            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true                      
                                   };            
            cg_Up_S.grid_col_Lock = g_ReadOnly;
            
            cg_Up_S.basegrid.RowHeadersVisible = false;
        }



        private void dGridView_Info_Header_Reset(DataGridView t_Dgv, string intTemp)
        {
            cg_Up_S.Grid_Base_Arr_Clear();
            cg_Up_S.basegrid = t_Dgv;
            cg_Up_S.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cg_Up_S.grid_col_Count = 11;
            cg_Up_S.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            
            if (intTemp == "1")
            {
                string[] g_HeaderText = {"구분" ,  "마감일자" ,  "지급일자"   , "발생액"  , "소득세"  
                                        , "주민세"  ,"실지급액"  , ""  , "" , "" 
                                        , ""
                                        };

                Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
                gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
                
                cg_Up_S.grid_cell_format = gr_dic_cell_format;

                int[] g_Width = { 100, 90, 70, 80, 80
                                 ,80 , 80 , 0 , 0 , 0
                                 ,0
                                };

                DataGridViewContentAlignment[] g_Alignment =
                                  {DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter 
                                   ,DataGridViewContentAlignment.MiddleCenter  
                                   ,DataGridViewContentAlignment.MiddleCenter
                                   ,DataGridViewContentAlignment.MiddleRight  //5    
  
                                   ,DataGridViewContentAlignment.MiddleRight 
                                   ,DataGridViewContentAlignment.MiddleRight  
                                   ,DataGridViewContentAlignment.MiddleRight
                                   ,DataGridViewContentAlignment.MiddleRight  //10

                                   ,DataGridViewContentAlignment.MiddleCenter  //10
                                  };

                cg_Up_S.grid_col_header_text = g_HeaderText;
                cg_Up_S.grid_col_w = g_Width;
                cg_Up_S.grid_col_alignment = g_Alignment;
            }           
            
            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                    ,true , true,  true,  true ,true  
                                    ,true                      
                                   };
            cg_Up_S.grid_col_Lock = g_ReadOnly;
            cg_Up_S.basegrid.RowHeadersVisible = false;
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
                //쿼리문상 오류를 잡기 위함.
                if (T_R.Text_KeyChar_Check(e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if   
            }
            else if ((tb.Tag != null) && (tb.Tag.ToString() == "1"))
            {
                //숫자만 입력 가능
                if (T_R.Text_KeyChar_Check(e, 1) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

            else if ((tb.Tag != null) && (tb.Tag.ToString() == "-"))
            {
                //숫자와  - 만
                if (T_R.Text_KeyChar_Check(e, "-") == false)
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

            else if ((tb.Tag != null) &&  tb.Tag.ToString() == "name")  //회원 정보 관련해서 이름 검색을 필요로 하는 텍스트 박스이다.
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(tb, e) == false)
                {
                    e.Handled = true;
                    return;
                } // end if
            }

            else if ((tb.Tag != null) && (tb.Tag.ToString() == "."))
            {
                //쿼리문 오류관련 입력만 아니면 가능하다.
                if (T_R.Text_KeyChar_Check(e, 1, ".") == false)
                {
                    e.Handled = true;
                    return;
                } // end if
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

                    if (mtb.Name == "mtxtTel2")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                        //20191015구현호핸드폰으로 조회
                        if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                        {
                            int reCnt = 0;
                            cls_Search_DB cds = new cls_Search_DB();
                            string Search_Name = "";
                            string mbid2 = "";
                            reCnt = cds.Member_hptel_Search(mtb.Text, ref mbid2, ref Search_Name);

                            if (reCnt == 1)
                            {
                                if (mtb.Name == "mtxtTel2")
                                {
                                    mtxtMbid.Text = mbid2;
                                    txtName.Text = Search_Name;
                                    if (Input_Error_Check2(mtxtMbid, "m") == true)
                                        Set_Form_Date(mtxtMbid.Text, "m");
                                    //SendKeys.Send("{TAB}");
                                }
                            }
                            else if (reCnt > 1)  //회원번호 비슷한 사람들이 많은 경우
                            {
                                //string Mbid = "";
                                //int Mbid2 = 0;
                                //cds.Member_Nmumber_Split(mtb.Text, ref Mbid, ref Mbid2);

                                frmBase_Member_Search_hptel e_f = new frmBase_Member_Search_hptel();

                                if (mtb.Name == "mtxtTel2")
                                {
                                    e_f.Send_Mem_Number += new frmBase_Member_Search_hptel.SendNumberDele(e_f_Send_Mem_Number);
                                    e_f.Call_searchNumber_Info += new frmBase_Member_Search_hptel.Call_searchNumber_Info_Dele(e_f_Send_hptel_Info);
                                }

                                e_f.ShowDialog();

                                SendKeys.Send("{TAB}");
                            }
                            else if (reCnt <= 0)  //동일 회원번호로 사람이 없는 경우에
                            {
                                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Mbid_Not_Exist")
                                 + "\n" +
                                 cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            }

                        }

                    }

                    if (mtb.Name == "mtxtZip1")
                    {
                        if (Sn_Number_(Sn, mtb, "Zip") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtZip_Auto")
                    {
                        if (Sn_Number_(Sn, mtb, "Zip") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtTel_Auto")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }


                    if (mtb.Name == "mtxtBrithDay")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");

                        //20191016구현호 생년월일로 조회


                        if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                        {
                            int reCnt = 0;
                            cls_Search_DB cds = new cls_Search_DB();
                            string Search_Name = "";
                            string mbid2 = "";
                            reCnt = cds.Member_birthday_Search(mtb.Text, ref mbid2, ref Search_Name);

                            if (reCnt == 1)
                            {
                                if (mtb.Name == "mtxtBrithDay")
                                {
                                    mtxtMbid.Text = mbid2;
                                    txtName.Text = Search_Name;
                                    if (Input_Error_Check2(mtxtMbid, "m") == true)
                                        Set_Form_Date(mtxtMbid.Text, "m");
                                    //SendKeys.Send("{TAB}");
                                }
                            }

                            else if (reCnt > 1)  //회원번호 비슷한 사람들이 많은 경우
                            {
                                //string Mbid = "";
                                //int Mbid2 = 0;
                                //cds.Member_Nmumber_Split(mtb.Text, ref Mbid, ref Mbid2);

                                frmBase_Member_Search_birthday e_f = new frmBase_Member_Search_birthday();

                                if (mtb.Name == "mtxtBrithDay")
                                {
                                    e_f.Send_Mem_Number += new frmBase_Member_Search_birthday.SendNumberDele(e_f_Send_Mem_Number);
                                    e_f.Call_searchNumber_Info += new frmBase_Member_Search_birthday.Call_searchNumber_Info_Dele(e_f_Send_birthday_Info);
                                }

                                e_f.ShowDialog();

                                SendKeys.Send("{TAB}");
                            }
                            else if (reCnt <= 0)  //동일 회원번호로 사람이 없는 경우에
                            {
                                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Mbid_Not_Exist")
                                 + "\n" +
                                 cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                            }

                        }
                    }
                

                    if (mtb.Name == "mtxtVisaDay")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }

                    string R4_name = mtb.Name.Substring(mtb.Name.Length - 4, 4);
                    if (R4_name == "Date" || R4_name == "ate3" || R4_name == "ate1" || R4_name == "ate2" || R4_name == "ate4")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }




                  
                    

                }
                else
                    SendKeys.Send("{TAB}");


            }
        }
     
        void e_f_Send_hptel_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            searchMbid = ""; searchMbid2 = 0;
            seachName = mtxtTel2.Text.Trim();
        }

        void e_f_Send_birthday_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            searchMbid = ""; searchMbid2 = 0;
            seachName = mtxtBrithDay.Text.Trim();
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

        private bool Sn_Number_1(string Sn, TextBoxBase mtb, string sort_TF, int t_Sort2 = 0)
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
                        Ret = c_er.Input_Date_Err_Check((MaskedTextBox)mtb);

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
                if (sort_TF == "HpTel")
                {
                    MessageBox.Show("휴대폰"
                       + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }

                /* 2018-08-22 지성경 막음 
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

                if (sort_TF == "Email")
                {
                    MessageBox.Show("메일주소가 입력되지않았습니다."
                       + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                */
                mtb.Focus(); return false;
            }

            return true;
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
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtCenter_Code.Text = "";
                Data_Set_Form_TF = 0;
               
            }

            if (tb.Name == "txtBank")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtBank_Code.Text = "";
                Data_Set_Form_TF = 0;
                
            }
            
        }


        void T_R_Key_Enter_13_Name(string txt_tag, Control tb)
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
                        if (Input_Error_Check(mtxtMbid, "m") == true)
                            Set_Form_Date(mtxtMbid.Text, "m");
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



        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }


        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {
            if (tab_Nation.Visible == true)
            {
                combo_Se_Code.SelectedIndex = combo_Se.SelectedIndex;
                if (combo_Se_Code.Text == "")  //다국어 지원프로그램을 사용시 국가는 필히 선택을 해야 된다.
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                       + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Not_Na_Code")
                      + "\n" +
                      cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                    combo_Se.Focus(); return;
                }
            }

            if (tb.Name == "txtCenter")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtCenter_Code);
                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, txtCenter_Code, "");
                else
                    Ncod_Text_Set_Data(tb, txtCenter_Code);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtBank")
            {
                Data_Set_Form_TF = 1;
                Db_Grid_Popup(tb, txtBank_Code);
                //if (tb.Text.ToString() == "")
                //    Db_Grid_Popup(tb, txtBank_Code, "");
                //else
                //    Ncod_Text_Set_Data(tb, txtBank_Code);

                //SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }

          
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
                cgb_Pop.Next_Focus_Control = mtxtZip1;

            if (tb.Name == "txtCenter2")
                cgb_Pop.Next_Focus_Control = mtxtZip1;

            if (tb.Name == "txtBank")
                cgb_Pop.Next_Focus_Control = txtName_Accnt;

            if (tb.Name == "txtR_Id")
                cgb_Pop.Next_Focus_Control = txtName_Accnt;

            if (tb.Name == "txtChange")
                cgb_Pop.Next_Focus_Control = txtName_Accnt;

            if (tb.Name == "txtSellCode")
                cgb_Pop.Next_Focus_Control = txtName_Accnt;

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
                if (tb.Name == "txtCenter")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", strSql);
                    cgb_Pop.Next_Focus_Control = mtxtZip1;
                }

                if (tb.Name == "txtR_Id")
                    cgb_Pop.db_grid_Popup_Base(2, "사용자ID", "사용자명", "user_id", "U_Name", strSql);

                if (tb.Name == "txtBank")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "은행_코드", "은행명", "Ncode", "BankName", strSql);
                    cgb_Pop.Next_Focus_Control = txtName_Accnt;
                }
            }
            else
            {
             
                if (tb.Name == "txtCenter")
                {
                    string Tsql;
                    Tsql = "Select Ncode , Name  ";
                    Tsql = Tsql + " From tbl_Business (nolock) ";
                    Tsql = Tsql + " Where  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + combo_Se_Code.Text.Trim() + "') )";
                    if (combo_Se_Code.Text.Trim() != "" ) Tsql = Tsql + " And  Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
                    Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                    Tsql = Tsql + " And ShowMemberCenter =  'Y' "; // 2019-04-15 구현호 센터코드 등록 회원정보 보기에서 보기옵션
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "센타_코드", "센타명", "Ncode", "Name", Tsql);
                    cgb_Pop.Next_Focus_Control = mtxtZip1;
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
                    if (combo_Se_Code.Text.Trim() != "") Tsql = Tsql + " Where  Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
                    Tsql = Tsql + " Order by Ncode ";

                    cgb_Pop.db_grid_Popup_Base(2, "은행_코드", "은행명", "Ncode", "BankName", Tsql);
                    cgb_Pop.Next_Focus_Control = txtName_Accnt;
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
                Tsql = Tsql + " And  Ncode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + combo_Se_Code.Text.Trim() + "') )";
                if (combo_Se_Code.Text.Trim() != "") Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
                Tsql = Tsql + " And  U_TF = 0 "; //사용센타만 보이게 한다 
                Tsql = Tsql + " And ShowMemberCenter = 'Y' ";
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
                if (combo_Se_Code.Text.Trim() != "") Tsql = Tsql + " And   Na_Code = '" + combo_Se_Code.Text.Trim() + "'"; 
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







        private Boolean Input_Error_Check(MaskedTextBox m_tb, string s_Kind)
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
            Tsql = "Select Mbid , Mbid2, M_Name , Sell_Mem_TF  ";
            Tsql = Tsql + " , LineCnt , N_LineCnt  ";
            Tsql = Tsql + " , LeaveDate , LineUserDate  ";
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
                m_tb.Focus(); return false;
            }
            //++++++++++++++++++++++++++++++++            

            return true;
        }

        //20191016 구현호 전화번호체크
        private Boolean Input_Error_Check2(MaskedTextBox m_tb, string s_Kind)
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
            Tsql = "Select Mbid , Mbid2, M_Name , Sell_Mem_TF  ";
            Tsql = Tsql + " , LineCnt , N_LineCnt  ";
            Tsql = Tsql + " , LeaveDate , LineUserDate  ";
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
            Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
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



















        private void _From_Data_Clear()
        {
            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb.d_Grid_view_Header_Reset();
            //Base_Grid_Set(); //당일등록 회원을 불러온다.
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            ////>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Up_S_Header_Reset(dGridView_Up_N); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Up_S.d_Grid_view_Header_Reset(1);

            dGridView_Up_S_Header_Reset(dGridView_Up_S); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Up_S.d_Grid_view_Header_Reset(1);

            dGridView_Up_S_Header_Reset(dGridView_Down_N); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Up_S.d_Grid_view_Header_Reset(1);

            dGridView_Up_S_Header_Reset(dGridView_Down_S); //디비그리드 헤더와 기본 셋팅을 한다.
            cg_Up_S.d_Grid_view_Header_Reset(1);

            cls_Grid_Base_info_Put cgbp = new cls_Grid_Base_info_Put();
            cgbp.dGridView_Put_baseinfo( dGridView_Sell, "sell");

            cls_Grid_Base_info_Put cgbp2 = new cls_Grid_Base_info_Put();
            cgbp2.dGridView_Put_baseinfo( dGridView_inf, "memc");

            cls_Grid_Base_info_Put cgbp3 = new cls_Grid_Base_info_Put();
            cgbp3.dGridView_Put_baseinfo( dGridView_Up, "memupc");

            cls_Grid_Base_info_Put cgbp4 = new cls_Grid_Base_info_Put();
            cgbp4.dGridView_Put_baseinfo( dGridView_Add, "memadd");




            //dGridView_Info_Header_Reset(dGridView_Pay, "1");
            //cg_Up_S.d_Grid_view_Header_Reset(1);



            cls_Grid_Base_info_Put cgbp5 = new cls_Grid_Base_info_Put();
            cgbp5.dGridView_Put_baseinfo(dGridView_Sell_Item, "item");

            cls_Grid_Base_info_Put cgbp6 = new cls_Grid_Base_info_Put();
            cgbp6.dGridView_Put_baseinfo(dGridView_Sell_Cacu, "cacu");

            cls_Grid_Base_info_Put cgbp7 = new cls_Grid_Base_info_Put();
            cgbp7.dGridView_Put_baseinfo(dGridView_Sell_Rece, "rece");


            cls_Grid_Base_info_Put cgbp8 = new cls_Grid_Base_info_Put();
            cgbp8.dGridView_Put_baseinfo(dGridView_Pay, "pay");

            cls_Grid_Base_info_Put cgbp9 = new cls_Grid_Base_info_Put();
            cgbp9.dGridView_Put_baseinfo( dGridView_Down_N2, "nomindown");

            cls_Grid_Base_info_Put cgbp10 = new cls_Grid_Base_info_Put();
            cgbp10.dGridView_Put_baseinfo( dGridView_Down_S2, "savedown");

            cls_Grid_Base_info_Put cgbp11 = new cls_Grid_Base_info_Put();
            cgbp11.dGridView_Put_baseinfo(dGridView_Talk, "talk");
            //dGridView_Sell_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Item.d_Grid_view_Header_Reset(1);

            //dGridView_Sell_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu.d_Grid_view_Header_Reset(1);

            //dGridView_Sell_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece.d_Grid_view_Header_Reset(1);

            
            tabC_Up.SelectedIndex = 0;            
            tabC_Mem.SelectedIndex = 0;
            tabC_1.SelectedIndex = 0;
            ////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< 

            txtName.ReadOnly =false ;
            txtName.BackColor = SystemColors.Window;
            txtName.BorderStyle = BorderStyle.Fixed3D; 

            //txtName.BackColor = Color.FromArgb(236, 241, 220); 
            //txtName.BorderStyle = BorderStyle.Fixed3D; 

            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this, mtxtMbid);

            opt_sell_2.Checked = true; opt_Bir_TF_1.Checked = true;
            raButt_IN_1.Checked = true;
            check_BankDocument.Checked = false;
            check_CpnoDocument.Checked = false;

            opt_B_1.Checked = false; opt_B_2.Checked = false; opt_B_3.Checked = false; 

            chk_N.Checked = false; chk_S.Checked = false;
            mtxtSn.Mask = "999999-9999999";
            idx_Mbid = ""; idx_Mbid2 = 0;
            idx_Password = "";
            txtB1.Text = "0";
            button_exigo.Enabled = true;
            button_exigo.Visible = false; 

            Reset_Chart_Total();

            combo_Se.Text = ""; combo_Se_Code.Text = "";
            radioB_RBO.Checked = true;
            radioB_G8.Checked = true; 
            
            mtxtMbid.Focus();
        }
        private DataGridView e_f_Send_Export_Excel_Info(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "후원인엑셀";
            Excel_Export_From_Name = this.Name;
            return dGridView_Down_S2;
        }
        private DataGridView e_f_Send_Export_Excel_Info_2(ref string Excel_Export_From_Name, ref string Excel_Export_File_Name)
        {
            Excel_Export_File_Name = this.Text; // "추천인엑셀";
            Excel_Export_From_Name = this.Name;
            return dGridView_Down_N2;
        }


        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;


            if (bt.Name == "butt_Clear")
            {                
                _From_Data_Clear();

                combo_Se_Code.Text = cls_User.gid_CountryCode;
                combo_Se_Code_2.Text = cls_User.gid_CountryCode;
            }
        

            else if (bt.Name == "butt_Excel")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info);
                e_f.ShowDialog();
            }
            else if (bt.Name == "butt_Excel_2")
            {
                frmBase_Excel e_f = new frmBase_Excel();
                e_f.Send_Export_Excel_Info += new frmBase_Excel.Send_Export_Excel_Info_Dele(e_f_Send_Export_Excel_Info_2);
                e_f.ShowDialog();
            }
            else if (bt.Name == "butt_Save")
            {
                int Save_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Save_Base_Data(ref Save_Error_Check);

                if (Save_Error_Check > 0)               
                    _From_Data_Clear();     
                                
                
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            
            else if (bt.Name == "butt_Exit")
            {
                this.Close();
            }

            else if (bt.Name == "butt_AddCode")
            {
                frmBase_AddCode e_f = new frmBase_AddCode();
                e_f.Send_Address_Info += new frmBase_AddCode.SendAddressDele(e_f_Send_Address_Info);
                e_f.ShowDialog();

                txtAddress2.Focus();
            }

            else if (bt.Name == "butt_AddCode2")
            {
                frmBase_AddCode e_f = new frmBase_AddCode();
                e_f.Send_Address_Info += new frmBase_AddCode.SendAddressDele(e_f_Send_Address_Info2);
                e_f.ShowDialog();

                txtAddress4.Focus();
            }

            else if (bt.Name == "butt_AddCodeT1")
            {
                txtAddress3.Text = txtAddress1.Text;
                txtAddress4.Text = txtAddress2.Text;
                mtxtZip2.Text = mtxtZip1.Text;

                txtAddress4.Focus();
            }

          

        }

        private void e_f_Send_Address_Info(string AddCode1, string AddCode2, string Address1, string Address2, string Address3)
        {
            mtxtZip1.Text = AddCode1 + "-" + AddCode2; 
            txtAddress1.Text = Address1; txtAddress2.Text = Address2;
                        
        }

        private void e_f_Send_Address_Info2(string AddCode1, string AddCode2, string Address1, string Address2, string Address3)
        {
            mtxtZip2.Text = AddCode1 + "-" + AddCode2;
            txtAddress3.Text = Address1; txtAddress4.Text = Address2;

        }


        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
            //SendKeys.Send("{TAB}");
        }

   
  


        private bool  Check_TextBox_Error_Date()
        {
            cls_Check_Input_Error c_er = new cls_Check_Input_Error();
            if (mtxtRegDate.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_1(mtxtRegDate.Text, mtxtRegDate, "Date") == false)
                {
                    mtxtRegDate.Focus();
                    return false;
                }
            }

            if (mtxtBrithDay.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_1(mtxtBrithDay.Text, mtxtBrithDay, "Date") == false)
                {
                    mtxtBrithDay.Focus();
                    return false;
                }
            }

            if (mtxtVisaDay.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_1(mtxtVisaDay.Text, mtxtVisaDay, "Date") == false)
                {
                    mtxtVisaDay.Focus();
                    return false;
                }
            }

            if (mtxtEdDate.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_1(mtxtEdDate.Text, mtxtEdDate, "Date") == false)
                {
                    mtxtEdDate.Focus();
                    return false;
                }
            }

            if (mtxtRBODate.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_1(mtxtRBODate.Text, mtxtRBODate, "Date") == false)
                {
                    mtxtRBODate.Focus();
                    return false;
                }
            }

            return true;
        }


        
        private Boolean Check_TextBox_Error()
        {
            
            if (Input_Error_Check(mtxtMbid, "m") == false) return false; //회원번호 관련 관련 오류 체크

            
           
           
            cls_Check_Text T_R = new cls_Check_Text();
            string me = "";
            
            me = T_R.Text_Null_Check(txtName, "Msg_Sort_M_Name"); //성명을 필히 넣어야 합니다.
            if (me != "")
            {
                MessageBox.Show(me);
                return false;
            }

            if (mtxtRegDate.Text =="") //등록일자가 빈칸으로 되어 잇으면 당일을 셋팅한다.
                mtxtRegDate.Text = DateTime.Now.ToString("yyyy-MM-dd");




            string Sn = string.Empty;
            //Sn = mtxtTel1.Text.Replace("-", "").Replace("_", "").Trim();
            //if (Sn_Number_1(Sn, mtxtTel1, "Tel") == false)
            //{
            //    mtxtTel1.Focus();
            //    return false;
            //}

            Sn = mtxtTel2.Text.Replace("-", "").Replace("_", "").Trim();
            if (Sn_Number_1(Sn, mtxtTel2, "HpTel") == false)
            {
                mtxtTel2.Focus();
                return false;
            }

            Sn = mtxtZip1.Text.Replace("-", "").Replace("_", "").Trim();
            if (Sn_Number_1(Sn, mtxtZip1, "Zip") == false)
            {
                mtxtZip1.Focus();
                return false;
            }


            /* 2018-08-22 지성경 일단막자....
            if (txtAccount.Text == "")
            {
                me = "계좌번호를 필히 입력해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                txtAccount.Focus();
                return false;
            }

            if (txtName_Accnt.Text == "")
            {
                me = "예금주를 필히 입력해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                txtName_Accnt.Focus();
                return false;
            }


            if (txtBank_Code.Text == "")
            {
                me = "은행을 필히 선택해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                txtBank.Focus();
                return false;
            }
            */


            //날짜 관련 텍스트 파일들에 대해서 날짜 오류를 체크한다
            if (Check_TextBox_Error_Date() == false) return false;

            return true;
        }





        private bool Check_TextBox_CC_Error()
        {
            cls_Check_Text T_R = new cls_Check_Text();
            string me = "";

            me = T_R.Text_Null_Check(txtName_C, "Msg_Sort_M_Name"); //성명을 필히 넣어야 합니다.
            if (me != "")
            {
                MessageBox.Show(me);
                txtName_C.Focus();
                return false;
            }

            /* 2018-08-05 지성경 현재 부부사업자는 주민번호 체크하지아니함 
            if (mtxtSn_C.Text.Replace("-", "") == "")
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_Error")
                           + "\n" +
                           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                mtxtSn_C.Focus(); return false;
            }
            
            string Sn = mtxtSn_C.Text.Replace("-", "").Replace("_", "").Trim();
            if (Sn_Number_(Sn, mtxtSn_C) == false) return false;   //주민번호 입력 사항에 대해서 체크를 한다.                     
            */

            cls_Check_Input_Error c_er = new cls_Check_Input_Error();

            if (mtxtBrithDayC.Text.Replace("-", "").Trim() != "")
            {
                if (Sn_Number_1(mtxtBrithDayC.Text, mtxtBrithDayC, "Date") == false)
                {
                    mtxtBrithDayC.Focus();
                    return false;
                }
            }

            return true;
        }

        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;


            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (txtB1.Text.Trim() == "") txtB1.Text = "0"; 
            if (Check_TextBox_Error() == false) return;

            if (check_CC.Checked == true)
                if (Check_TextBox_CC_Error() == false) return;  //오토쉽 등록 관련 오류를 체크한다.

            cls_Search_DB csd = new cls_Search_DB();       

            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

            int S_RBO_Mem_TF = 0;
            string RBO_S_Date = ""; 
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string StrSql = "Select  Mbid, Mbid2 , RBO_Mem_TF , RBO_S_Date  ";
            StrSql = StrSql + " From tbl_Memberinfo  (nolock)  ";
            StrSql = StrSql + " Where mbid = '" + Mbid + "'";
            StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            Temp_Connect.Open_Data_Set(StrSql, "tbl_Memberinfo", ds);
            {
                if (Temp_Connect.DataSet_ReCount > 0)//오토쉽이 체크되어 잇는데 체크를 풀엇다. 그럼 삭제하라는 의미로 받아들인다.
                {
                    S_RBO_Mem_TF = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0]["RBO_Mem_TF"].ToString());
                    RBO_S_Date = ds.Tables["tbl_Memberinfo"].Rows[0]["RBO_S_Date"].ToString();

                    if (radioB_RBO.Checked == true && S_RBO_Mem_TF == 1 && mtxtRBODate.Text.Replace("-", "").Trim() == "")
                    {
                        MessageBox.Show("비긴즈에서 RBO 전환시에 날짜를 필히 입력 해야 합니다.."
                     + "\n" +
                     cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                        mtxtRBODate.Focus();
                        return ;
                    }
                }
            }


            csd.Member_Mod_BackUp(mtxtMbid.Text.Trim(), "tbl_Memberinfo");

            cls_Search_DB csd_R = new cls_Search_DB();
            csd_R.Member_Mod_BackUp(mtxtMbid.Text.Trim(), "tbl_Memberinfo_Address", " And Sort_Add = 'R' ");




            //cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

            try
            {
                string hometel = ""; string hptel = ""; 
                string BirthDay = "";  string BirthDay_M ="";  string BirthDay_D=""  ; int BirthDayTF = 0 ;
                string Sex_FLAG = "";
                string AgreeSMS   = "N";
                string AgreeEmail = "N";
                int Sell_Mem_TF = 0; int Add_TF = 0, Myoffice_TF = 0, RBO_Mem_TF = 0, G8_TF = 0;
                int BankDocument = 0, CpnoDocument = 0;
                int For_Kind_TF = 0;

                if (check_BankDocument.Checked == true) BankDocument = 1;
                if (check_CpnoDocument.Checked == true) CpnoDocument = 1;

                if (mtxtTel1.Text.Replace("-", "").Trim() != "") hometel = mtxtTel1.Text;
                if (mtxtTel2.Text.Replace("-", "").Trim() != "") hptel = mtxtTel2.Text;        
                
                if (opt_sell_3.Checked == true) Sell_Mem_TF = 1; //소비자는 1 판매원은 기본 0
                
                if (opt_Bir_TF_1.Checked ==true)  BirthDayTF =1 ; //양력은 1  음력은 2
                if (opt_Bir_TF_2.Checked ==true)  BirthDayTF =2 ;

                if (opt_B_1.Checked == true) Add_TF = 1;  //기본주소가 
                if (opt_B_2.Checked == true) Add_TF = 2; //회사 주소가
                if (opt_B_3.Checked == true) Add_TF = 3; //기본배송지 주소가


                if (radioB_RBO.Checked == true) RBO_Mem_TF = 0;// RBO 0 비긴즈 1
                if (radioB_Begin.Checked == true) RBO_Mem_TF = 1;

                if (radioB_G8.Checked == true) G8_TF = 8;// RBO 0 비긴즈 1
                if (radioB_G4.Checked == true) G8_TF = 4;

                if (check_MyOffice.Checked == true) Myoffice_TF = 1; 

                if (mtxtBrithDay.Text.Replace("-", "").Trim() != "")
                {
                    string[] Sn_t = mtxtBrithDay.Text.Split('-');

                    BirthDay = Sn_t[0];  //생년월일을 년월일로 해서 쪼갠다
                    BirthDay_M = Sn_t[1]; //웹쪽 관련해서 이렇게 받아들이는데가 많아서
                    BirthDay_D = Sn_t[2]; //웹쪽 기준에 맞춘거임.
                }                   
                
                if (radioB_Sex_Y.Checked == true) Sex_FLAG = "Y";
                if (radioB_Sex_X.Checked == true) Sex_FLAG = "X";

                if (checkB_SMS_FLAG.Checked == true)    AgreeSMS    = "Y";
                if (checkB_EMail_FLAG.Checked == true)  AgreeEmail  = "Y";


                if (raButt_IN_2.Checked == true) For_Kind_TF = 1;// 내국인은 0 외국인은 1  사업자는 2
                if (raButt_IN_3.Checked == true) For_Kind_TF = 2;

                StrSql = "";                
                StrSql = "Update tbl_Memberinfo Set ";

                StrSql = StrSql + " E_name = '" + txtName_E_1.Text.Trim() + "'";
                StrSql = StrSql + " ,E_name_Last = '" + txtName_E_2.Text.Trim() + "'";
                StrSql = StrSql + " ,Email = '" +  txtEmail.Text.Trim() + "'";

                //StrSql = StrSql + " ,Email = '" + txtEmail.Text.Trim() + "'";
                StrSql = StrSql + " ,Ed_Date = '" + mtxtEdDate.Text.Replace("-", "").Trim() + "'";
                StrSql = StrSql + " ,Remarks = '" + txtRemark.Text.Trim() + "'";
                StrSql = StrSql + " ,Regtime = '" + mtxtRegDate.Text.Replace("-", "").Trim() + "'";

                StrSql = StrSql + " ,RBO_S_Date = '" + mtxtRBODate.Text.Replace("-", "").Trim() + "'";
               

                StrSql = StrSql + " ,VisaDate = '" + mtxtVisaDay.Text.Replace("-", "").Trim() + "'";                

                StrSql = StrSql + " ,Addcode1 = '" + mtxtZip1.Text.Trim().Replace ("-","") + "'";
                StrSql = StrSql + " ,Address1 = '" + txtAddress1.Text.Trim() + "'";
                StrSql = StrSql + " ,Address2 = '" + txtAddress2.Text.Trim() + "'";
                StrSql = StrSql + " ,hometel = '" + hometel + "'";
                StrSql = StrSql + " ,hptel = '" + hptel + "'";

                StrSql = StrSql + " ,BirthDay = '" + BirthDay + "'";
                StrSql = StrSql + " ,BirthDay_M = '" + BirthDay_M + "'";
                StrSql = StrSql + " ,BirthDay_D = '" + BirthDay_D + "'";

                StrSql = StrSql + " ,BankCode = '" + txtBank_Code.Text.Trim() + "'";
                StrSql = StrSql + " ,bankowner = '" + txtName_Accnt.Text.Trim() + "'";
                StrSql = StrSql + " ,bankaccnt = dbo.ENCRYPT_AES256('" + txtAccount.Text.Trim() + "')";
                StrSql = StrSql + " ,Reg_bankaccnt = dbo.ENCRYPT_AES256('" + txtAccount_Reg.Text.Trim() + "')";
                
                StrSql = StrSql + " ,BusinessCode = '" + txtCenter_Code.Text.Trim () + "'";
                StrSql = StrSql + " ,For_Kind_TF = " + For_Kind_TF; 

                if (txtPassword.Text.Equals(idx_Password) == false)
                    StrSql = StrSql + " ,WebPassWord = '" + EncryptSHA256_EUCKR(txtPassword.Text.Trim()) + "'";

                if (check_CC.Checked == true)
                {
                    if (mtxtBrithDayC.Text.Replace("-", "").Trim() != "")
                    {
                        string[] Sn_t = mtxtBrithDayC.Text.Split('-');

                        BirthDay = Sn_t[0];  //생년월일을 년월일로 해서 쪼갠다
                        BirthDay_M = Sn_t[1]; //웹쪽 관련해서 이렇게 받아들이는데가 많아서
                        BirthDay_D = Sn_t[2]; //웹쪽 기준에 맞춘거임.
                    }

                    if (raButt_IN_2_C.Checked == true) For_Kind_TF = 1;// 내국인은 0 외국인은 1  사업자는 2

                    StrSql = StrSql + " ,C_M_Name = '" + txtName_C.Text.Trim() + "'";
                    StrSql = StrSql + " ,C_For_Kind_TF = " + For_Kind_TF;
                    //StrSql = StrSql + " ,C_cpno = '" + encrypter.Encrypt(mtxtSn_C.Text.Replace("-", "").Trim()) + "'";
                    //StrSql = StrSql + " ,C_E_name = '" + txtName_E_1_C.Text.Trim() + "'";
                    //StrSql = StrSql + " ,C_E_name_Last = '" + txtName_E_2_C.Text.Trim() + "'";
                    StrSql = StrSql + " ,C_BirthDay = '" + BirthDay + "'";
                    StrSql = StrSql + " ,C_BirthDay_M = '" + BirthDay_M + "'";
                    StrSql = StrSql + " ,C_BirthDay_D = '" + BirthDay_D + "'";
                    StrSql = StrSql + " ,C_hptel = '" + mtxtTel2_C.Text + "'";
                    StrSql = StrSql + " ,C_Email = '" + txtEmail_C.Text + "'";
                }
                else
                {
                    StrSql = StrSql + " ,C_M_Name = ''";
                    StrSql = StrSql + " , C_For_Kind_TF = 0 ";
                    //StrSql = StrSql + " ,C_cpno = ''";
                    //StrSql = StrSql + " ,C_E_name = ''";
                    //StrSql = StrSql + " ,C_E_name_Last = ''";
                    StrSql = StrSql + " ,C_BirthDay = '' ";
                    StrSql = StrSql + " ,C_BirthDay_M = '' ";
                    StrSql = StrSql + " ,C_BirthDay_D = '' ";
                    StrSql = StrSql + " ,C_hptel = '" + mtxtTel2_C.Text + "'";
                    StrSql = StrSql + " ,C_Email = '" + txtEmail_C.Text + "'";


                }
                                   
                StrSql = StrSql + " ,BirthDayTF = " + BirthDayTF.ToString();
                StrSql = StrSql + " ,Sell_Mem_TF = " + Sell_Mem_TF.ToString();

                StrSql = StrSql + " ,G8_TF = " + G8_TF.ToString();
                StrSql = StrSql + " ,RBO_Mem_TF = " + RBO_Mem_TF.ToString();

                StrSql = StrSql + " ,BankDocument = " + BankDocument.ToString();
                StrSql = StrSql + " ,CpnoDocument = " + CpnoDocument.ToString();

                StrSql = StrSql + " ,Add_TF = " + Add_TF.ToString();

                StrSql = StrSql + " ,Myoffice_TF = " + Myoffice_TF.ToString();
                StrSql = StrSql + " ,Sex_Flag = '" + Sex_FLAG + "'";
                StrSql = StrSql + " ,AgreeSMS = '" + AgreeSMS + "'";
                StrSql = StrSql + " ,AgreeEmail = '" + AgreeEmail + "'";
                 

                //StrSql = StrSql + " ,GiBu_ = " + double.Parse (txtB1.Text.Trim ().ToString());

                if (Mbid.Length == 0)
                    StrSql = StrSql + " Where Mbid2 = " + Mbid2.ToString();
                else
                {
                    StrSql = StrSql + " Where Mbid = '" + Mbid + "' ";
                    StrSql = StrSql + " And   Mbid2 = " + Mbid2.ToString();
                }

                Temp_Connect.Update_Data (StrSql, Conn, tran, this.Name, this.Text);


                Chang_Mem_Address_R(Mbid, Mbid2, Temp_Connect, Conn, tran);

              
                
                tran.Commit();
                Save_Error_Check = 1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));

                csd.tbl_Memberinfo_Mod(mtxtMbid.Text.Trim());
                csd_R.tbl_Memberinfo_Mod(mtxtMbid.Text.Trim(), "R", "tbl_Memberinfo_Address", " And Sort_Add = 'R' ");
                
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






        private void Chang_Mem_Address_R(string Mbid, int Mbid2, cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran)
        {
           // string ETC_Tel_1 = ""; string ETC_Tel_2 = "";
            string StrSql = "";

            StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);


            StrSql = "Select Sort_Add , Mbid, Mbid2 ";
            StrSql = StrSql + " From tbl_Memberinfo_Address  (nolock)  ";
            StrSql = StrSql + " Where mbid = '" + Mbid + "'";
            StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();
            StrSql = StrSql + " And Sort_Add = 'R' ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, "tbl_Memberinfo_Address", ds) == true)
            {
                //if (txtTel_R_1.Text != "") ETC_Tel_1 = txtTel_R_1.Text.Trim() + '-' + txtTel_R_2.Text.Trim() + '-' + txtTel_R_3.Text.Trim();
                //if (txtTel2_R_1.Text != "") ETC_Tel_2 = txtTel2_R_1.Text.Trim() + '-' + txtTel2_R_2.Text.Trim() + '-' + txtTel2_R_3.Text.Trim();

                if (Temp_Connect.DataSet_ReCount == 0)//동일한 이름으로 코드가 있다 그럼.이거 저장하면 안되요
                {


                    StrSql = "Insert into tbl_Memberinfo_Address ( ";
                    StrSql = StrSql + " Sort_Add ";
                    StrSql = StrSql + " ,Mbid ";
                    StrSql = StrSql + " ,Mbid2 ";
                    StrSql = StrSql + " ,ETC_Addcode1 ";
                    StrSql = StrSql + " ,ETC_Address1 ";
                    StrSql = StrSql + " ,ETC_Address2 ";
                    StrSql = StrSql + " ,ETC_Address3 ";
                    StrSql = StrSql + " ,ETC_Tel_1 ";
                    StrSql = StrSql + " ,ETC_Tel_2 ";
                    StrSql = StrSql + " ,ETC_Name ";
                    StrSql = StrSql + " ,Recordid ";
                    StrSql = StrSql + " ,Recordtime ";
                    StrSql = StrSql + " ) ";
                    StrSql = StrSql + " Values ( ";

                    StrSql = StrSql + " 'R' ";
                    StrSql = StrSql + ",'" + Mbid + "'";
                    StrSql = StrSql + "," + Mbid2.ToString();
                    StrSql = StrSql + ", '" + mtxtZip2.Text.Trim().Replace ("-","") + "'";
                    StrSql = StrSql + ", '" + encrypter.Encrypt(txtAddress3.Text.Trim()) + "'";
                    StrSql = StrSql + ", '" + encrypter.Encrypt(txtAddress4.Text.Trim()) + "'";
                    StrSql = StrSql + ", '' ";

                    StrSql = StrSql + ", '' ";
                    StrSql = StrSql + ", '' ";
                    StrSql = StrSql + ", '' ";
                    //StrSql = StrSql + ", '" + encrypter.Encrypt(ETC_Tel_1) + "'";
                    //StrSql = StrSql + ", '" + encrypter.Encrypt(ETC_Tel_2) + "'";
                    //StrSql = StrSql + ", '" + encrypter.Encrypt(txtName_R.Text.Trim()) + "'";
                    StrSql = StrSql + ",'" + cls_User.gid + "'";
                    StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                    StrSql = StrSql + " ) ";

                    Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_Address", Conn, tran);
                }
                else
                {
                    StrSql = "Update tbl_Memberinfo_Address Set ";
                    StrSql = StrSql + "  ETC_Addcode1 = '" + mtxtZip2.Text.Trim().Replace("-", "") + "'";
                    StrSql = StrSql + " ,ETC_Address1 = '" + encrypter.Encrypt(txtAddress3.Text.Trim()) + "'";
                    StrSql = StrSql + " ,ETC_Address2 = '" + encrypter.Encrypt(txtAddress4.Text.Trim()) + "'";
                    StrSql = StrSql + " ,ETC_Address3 = ''";
                    //StrSql = StrSql + " ,ETC_Tel_1 = '" + encrypter.Encrypt(ETC_Tel_1) + "'";
                    //StrSql = StrSql + " ,ETC_Tel_2 = '" + encrypter.Encrypt(ETC_Tel_2) + "'";
                    //StrSql = StrSql + " ,ETC_Name = '" + encrypter.Encrypt(txtName_R.Text.Trim()) + "'";
                    StrSql = StrSql + " Where mbid = '" + Mbid + "'";
                    StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();
                    StrSql = StrSql + " And Sort_Add = 'R' ";

                    Temp_Connect.Update_Data(StrSql, Conn, tran);

                }
            }
        }


        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            //int rowcnt = (sender as DataGridView).CurrentCell.RowIndex;  
            if ((sender as DataGridView).CurrentRow.Cells[0].Value != null)
            {
                mtxtMbid.Text = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();

                int reCnt = 0;
                cls_Search_DB cds = new cls_Search_DB();
                string Search_Name = "";
                reCnt = cds.Member_Name_Search(mtxtMbid.Text, ref Search_Name);

                if (reCnt == 1)
                {
                    txtName.Text = Search_Name;
                    if (Input_Error_Check(mtxtMbid, "m") == true)
                        Set_Form_Date(mtxtMbid.Text, "m");
                 
                }
            }

        }


        private void dGridView_Base_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            //dGridView_Sell_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Item.d_Grid_view_Header_Reset();

            //dGridView_Sell_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Cacu.d_Grid_view_Header_Reset();

            //dGridView_Sell_Rece_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            //cgb_Rece.d_Grid_view_Header_Reset();
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[2].Value != null))
            {

                tabC_1.SelectedIndex = 0;

                string T_OrderNumber = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                //string M_Nubmer = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();

                //Put_OrderNumber_SellDate(T_OrderNumber);           

                cls_Grid_Base_info_Put cgbp5 = new cls_Grid_Base_info_Put();
                cgbp5.dGridView_Put_baseinfo(this, dGridView_Sell_Item, "item", "", T_OrderNumber);

                cls_Grid_Base_info_Put cgbp6 = new cls_Grid_Base_info_Put();
                cgbp6.dGridView_Put_baseinfo(this, dGridView_Sell_Cacu, "cacu", "", T_OrderNumber);

                cls_Grid_Base_info_Put cgbp7 = new cls_Grid_Base_info_Put();
                cgbp7.dGridView_Put_baseinfo(this, dGridView_Sell_Rece, "rece", "", T_OrderNumber);
            }
           
        }























        private void Set_SalesItemDetail(string Mbid, int Mbid2)
        {
            cls_form_Meth cm = new cls_form_Meth();
            string strSql = "";

            strSql = "Select Isnull(Sum(tbl_SalesitemDetail.ItemCount), 0 )   ";
            strSql = strSql + " , tbl_Goods.Name Item_Name ";          
            strSql = strSql + " From tbl_SalesitemDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_Goods (nolock) ON tbl_Goods.Ncode = tbl_SalesitemDetail.ItemCode ";
            strSql = strSql + " LEFT JOIN tbl_SalesDetail (nolock) ON tbl_SalesDetail.OrderNumber = tbl_SalesitemDetail.OrderNumber ";
            strSql = strSql + " Where tbl_SalesDetail.Mbid = '" + Mbid.ToString() + "'";
            strSql = strSql + " And   tbl_SalesDetail.Mbid2 = " + Mbid2;
            strSql = strSql + " And   ItemCount > 0 ";
            strSql = strSql + " Group By tbl_Goods.Name ";
            strSql = strSql + " Order By tbl_Goods.Name ASC ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++
            //Dictionary<string, int> T_SalesitemDetail = new Dictionary<string, int>();
            int ItemCnt = 0; string ItemCode = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                ItemCode = ds.Tables[base_db_name].Rows[fi_cnt]["Item_Name"].ToString();
                ItemCnt = int.Parse(ds.Tables[base_db_name].Rows[fi_cnt][0].ToString());
                Push_data(series_Item, ItemCode.Replace(" ", "").Substring(0, 5), ItemCnt);
            }


        }





        private void Push_data(Series series, string p, int p_3)
        {
            DataPoint dp = new DataPoint();
            dp.SetValueXY(p, p_3);
            dp.Label = string.Format(cls_app_static_var.str_Currency_Type, p_3); // p_3.ToString(); //p_3.ToString();
            series.Points.Add(dp);
        }

        //Push_data(series_Item, nodeKey.ToString() + "Line", Save_Cnt[nodeKey]);
        private void Save_Nom_Line_Chart()
        {
            cls_form_Meth cm = new cls_form_Meth();
            //series_Item.Name = cm._chang_base_caption_search("상품별");            
            chart_Item.Series.Clear();
            series_Item.Points.Clear();
            series_Item["DrawingStyle"] = "Emboss";
            series_Item["PointWidth"] = "0.5";
            series_Item.Name = cm._chang_base_caption_search("수량");
            series_Item.ChartType = SeriesChartType.Column ;
            series_Item.Legend = "Legend1";
            chart_Item.Series.Add(series_Item);

            chart_Item.ChartAreas[0].AxisX.Interval = 1;
            chart_Item.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("맑은고딕", 9);
            chart_Item.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 7;
        }





        private void Set_SalesDetail_Chart(string Mbid, int Mbid2)
        {
            cls_form_Meth cm = new cls_form_Meth();
            string strSql = "";

            strSql = "Select SellTypeName AS SellCodeName , InputCash,  InputCard , InputPassbook , TotalPrice ";
            strSql = strSql + ", tbl_SalesDetail.recordid , tbl_SalesDetail.Sellcode , InputMile ";            
            strSql = strSql + " From tbl_SalesDetail (nolock) ";
            strSql = strSql + " LEFT JOIN tbl_SellType (nolock) ON tbl_SellType.SellCode = tbl_SalesDetail.SellCode ";            
            strSql = strSql + " Where tbl_SalesDetail.Mbid = '" + Mbid.ToString() + "'";
            strSql = strSql + " And   tbl_SalesDetail.Mbid2 = " + Mbid2;
            strSql = strSql + " And   TotalPV >= 0 ";            
            strSql = strSql + " Order By OrderNumber ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(strSql, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++            
            
            Dictionary<string, double> SelType_1 = new Dictionary<string, double>();

            double Sum_13 = 0; double Sum_14 = 0; double Sum_15 = 0; double Sum_16 = 0;
            double Sell_Cnt_1 = 0; double Sell_Cnt_2 = 0;

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
            
                Sum_13 = Sum_13 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputCash"].ToString());
                Sum_14 = Sum_14 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputCard"].ToString());
                Sum_15 = Sum_15 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputPassbook"].ToString());
                Sum_16 = Sum_16 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["InputMile"].ToString());

                string T_ver = ds.Tables[base_db_name].Rows[fi_cnt]["SellCodeName"].ToString();
                if (SelType_1.ContainsKey(T_ver) == true)
                {
                    SelType_1[T_ver] = SelType_1[T_ver] + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalPrice"].ToString());                 
                }
                else
                {
                    SelType_1[T_ver] = double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalPrice"].ToString());
                }

                T_ver =  ds.Tables[base_db_name].Rows[fi_cnt]["recordid"].ToString();
                if (T_ver.Contains("WEB") != true)
                {
                    Sell_Cnt_1 = Sell_Cnt_1 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalPrice"].ToString());
                }
                else
                {
                    Sell_Cnt_2 = Sell_Cnt_2 + double.Parse(ds.Tables[base_db_name].Rows[fi_cnt]["TotalPrice"].ToString());
                }
            }

            Reset_Chart_Total(Sum_13, Sum_14, Sum_15, Sum_16);
            Reset_Chart_Total(ref SelType_1);
            Reset_Chart_Total(Sell_Cnt_1, Sell_Cnt_2);            

        }


        private void Reset_Chart_Total()
        {
            //chart_Mem.Series.Clear();
            cls_form_Meth cm = new cls_form_Meth();

            if (cls_app_static_var.Using_Mileage_TF == 1)
            {
                double[] yValues = { 0, 0, 0 , 0  };
                string[] xValues = { cm._chang_base_caption_search("현금"), cm._chang_base_caption_search("카드"), cm._chang_base_caption_search("무통장"), cm._chang_base_caption_search("마일리지") };
                chart_Mem.Series["Series1"].Points.DataBindXY(xValues, yValues);
            }
            else
            {
                double[] yValues = { 0, 0, 0 };
                string[] xValues = { cm._chang_base_caption_search("현금"), cm._chang_base_caption_search("카드"), cm._chang_base_caption_search("무통장") };
                chart_Mem.Series["Series1"].Points.DataBindXY(xValues, yValues);
            }

            chart_Mem.Series["Series1"].ChartType = SeriesChartType.Pie;
            chart_Mem.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_Mem.Legends[0].Enabled = true;

            string Tsql = "Select SellCode , SellTypeName ";
            Tsql = Tsql + " From tbl_SellType ";
            Tsql = Tsql + " Order BY SellCode  ";
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();

            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_SellType", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt != 0)
            {
                double[] yValues_2 = new double[ReCnt];
                string[] xValues_2 = new string[ReCnt]; // { cm._chang_base_caption_search(""), cm._chang_base_caption_search("탈퇴") }; 

                for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
                {
                    yValues_2[fi_cnt] = 0;
                    xValues_2[fi_cnt] = ds.Tables["tbl_SellType"].Rows[fi_cnt]["SellTypeName"].ToString();
                }

                chart_Leave.Series["Series1"].Points.DataBindXY(xValues_2, yValues_2);

                chart_Leave.Series["Series1"].ChartType = SeriesChartType.Pie;
                chart_Leave.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                chart_Leave.Legends[0].Enabled = true;
            }



            double[] yValues_3 = { 0, 0 };
            string[] xValues_3 = { cm._chang_base_caption_search("일반"), cm._chang_base_caption_search("WEB") };
            chart_edu.Series["Series1"].Points.DataBindXY(xValues_3, yValues_3);
            chart_edu.Series["Series1"].ChartType = SeriesChartType.Pie;
            chart_edu.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_edu.Legends[0].Enabled = true;

            chart_Item.Series.Clear();
            series_Item.Points.Clear();
        }



        private void Reset_Chart_Total(double SellCnt_1, double SellCnt_2, double SellCnt_3, double SellCnt_4)
        {
            //chart_Mem.Series.Clear();
            cls_form_Meth cm = new cls_form_Meth();
            Series series_Save = new Series();

            chart_Mem.Series.Clear();
            chart_Mem.Series.Add(series_Save);

            DataPoint dp = new DataPoint();
            series_Save.ChartType = SeriesChartType.Pie;
            dp.SetValueXY(cm._chang_base_caption_search("현금"), SellCnt_1);
            dp.Label = string.Format(cls_app_static_var.str_Currency_Type, SellCnt_1);
            dp.LabelForeColor = Color.Black;
            dp.LegendText = cm._chang_base_caption_search("현금");
            series_Save.Points.Add(dp);

            DataPoint dp2 = new DataPoint();

            dp2.SetValueXY(cm._chang_base_caption_search("카드"), SellCnt_2);
            dp2.Label = string.Format(cls_app_static_var.str_Currency_Type, SellCnt_2);
            dp2.LabelForeColor = Color.Black;
            dp2.LegendText = cm._chang_base_caption_search("카드");
            series_Save.Points.Add(dp2);


            DataPoint dp3 = new DataPoint();

            dp3.SetValueXY(cm._chang_base_caption_search("무통장"), SellCnt_3);
            dp3.Label = string.Format(cls_app_static_var.str_Currency_Type, SellCnt_3);
            dp3.LabelForeColor = Color.Black;
            dp3.LegendText = cm._chang_base_caption_search("무통장");
            series_Save.Points.Add(dp3);

            if (cls_app_static_var.Using_Mileage_TF == 1)
            {
                DataPoint dp4 = new DataPoint();

                dp4.SetValueXY(cm._chang_base_caption_search("마일리지"), SellCnt_4);
                dp4.Label = string.Format(cls_app_static_var.str_Currency_Type, SellCnt_3);
                dp4.LabelForeColor = Color.Black;
                dp4.LegendText = cm._chang_base_caption_search("마일리지");
                series_Save.Points.Add(dp4);
            }



        }

        private void Reset_Chart_Total(ref Dictionary<string, double> SelType_1)
        {

            cls_form_Meth cm = new cls_form_Meth();
            Series series_Save = new Series();

            chart_Leave.Series.Clear();
            chart_Leave.Series.Add(series_Save);
            int forCnt = 0;
            foreach (string tkey in SelType_1.Keys)
            {
                DataPoint dp = new DataPoint();
                series_Save.ChartType = SeriesChartType.Pie;
                dp.SetValueXY(tkey, SelType_1[tkey]);
                dp.Label = string.Format(cls_app_static_var.str_Currency_Type, SelType_1[tkey]);
                dp.LabelForeColor = Color.Black;
                dp.LegendText = tkey;
                series_Save.Points.Add(dp);
                forCnt++;
            }

            chart_Leave.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_Leave.Legends[0].Enabled = true;
        }


        private void Reset_Chart_Total(double SellCnt_1, double SellCnt_2)
        {
            //chart_edu.Series.Clear();
            cls_form_Meth cm = new cls_form_Meth();
            Series series_Save = new Series();

            chart_edu.Series.Clear();
            chart_edu.Series.Add(series_Save);

            DataPoint dp = new DataPoint();
            series_Save.ChartType = SeriesChartType.Pie;
            dp.SetValueXY(cm._chang_base_caption_search("일반"), SellCnt_1);
            dp.Label = string.Format(cls_app_static_var.str_Currency_Type, SellCnt_1);
            dp.LabelForeColor = Color.Black;
            dp.LegendText = cm._chang_base_caption_search("일반");
            series_Save.Points.Add(dp);

            DataPoint dp2 = new DataPoint();

            dp2.SetValueXY(cm._chang_base_caption_search("WEB"), SellCnt_2);
            dp2.Label = string.Format(cls_app_static_var.str_Currency_Type, SellCnt_2);
            dp2.LabelForeColor = Color.Black;
            dp2.LegendText = cm._chang_base_caption_search("WEB");
            series_Save.Points.Add(dp2);


            chart_edu.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            chart_edu.Legends[0].Enabled = true;
        }




        private void tabC_Mem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (idx_Mbid2 <= 0)
                return;

            if (tabC_Mem.SelectedTab.Name  == "tabP_Pay")  //
            {
                if (dGridView_Pay.RowCount == 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    cls_Grid_Base_info_Put cgbp = new cls_Grid_Base_info_Put();
                    cgbp.dGridView_Put_baseinfo(this, dGridView_Pay, "pay", mtxtMbid.Text.Trim());
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }

            if (tabC_Mem.SelectedTab.Name == "tab_Down_Save") // tabP_Down_Save
            {
                if (cls_app_static_var.save_uging_Pr_Flag == 0) //후원인 기능 사용하지 마라.
                    return;

                if (dGridView_Down_S2.RowCount == 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    cls_Grid_Base_info_Put cgbp = new cls_Grid_Base_info_Put();
                    cgbp.dGridView_Put_baseinfo(this, dGridView_Down_S2, "savedown", mtxtMbid.Text.Trim());
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }

            if (tabC_Mem.SelectedTab.Name  == "tab_Down_Nom") //tabP_Down_Nom
            {
                if (cls_app_static_var.nom_uging_Pr_Flag == 0) //추천인 기능 사용하지 마라.
                    return;

                if (dGridView_Down_N2.RowCount == 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    cls_Grid_Base_info_Put cgbp = new cls_Grid_Base_info_Put();
                    cgbp.dGridView_Put_baseinfo(this, dGridView_Down_N2, "nomindown", mtxtMbid.Text.Trim());
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }


        }

        private void butt_Talk_Click(object sender, EventArgs e)
        {

            if (txtSeq.Text == "")
            {
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            else
            {
                if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            if (Input_Error_Check(mtxtMbid, "m") == false) return ; //회원번호 관련 관련 오류 체크
            
            cls_Check_Text T_R = new cls_Check_Text();
            string me = "";

            me = T_R.Text_Null_Check(txtName, "Msg_Sort_M_Name"); //성명을 필히 넣어야 합니다.
            if (me != "")
            {
                MessageBox.Show(me);
                return ;
            }

            me = T_R.Text_Null_Check(txtTalk, "Msg_Sort_Talk"); //상담내역을 필히 넣어야 합니다.
            if (me != "")
            {
                MessageBox.Show(me);
                txtTalk.Focus();
                return ;
            }
            


            cls_Search_DB csd = new cls_Search_DB();
     
            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);
            
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

            try
            {
                string StrSql = "";

                if (txtSeq.Text == "")
                {
                    
                    StrSql = "Insert into tbl_Memberinfo_Talk ";
                    StrSql = StrSql + " (";
                    StrSql = StrSql + " Mbid  ";
                    StrSql = StrSql + " , Mbid2 ";
                    StrSql = StrSql + " , TalkContent ";
                    StrSql = StrSql + " , Recordid ";
                    StrSql = StrSql + " , Recordtime ";

                    StrSql = StrSql + ") Values ( ";
                    StrSql = StrSql + "'" + Mbid + "'";
                    StrSql = StrSql + "," + Mbid2;
                    StrSql = StrSql + ",'" + txtTalk.Text.Trim() + "'";
                    StrSql = StrSql + ",'" + cls_User.gid + "'";
                    StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                    StrSql = StrSql + ")";

                    Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran, this.Name, this.Text);
                }
                else
                {
                    StrSql = "Insert into tbl_Memberinfo_Talk_Mod Select * ,'" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21)  From tbl_Memberinfo_Talk  ";
                    StrSql = StrSql + " Where Seq = " + txtSeq.Text ;

                    Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran, this.Name, this.Text);


                    StrSql = "Update tbl_Memberinfo_Talk Set ";
                    StrSql = StrSql + " TalkContent = '" + txtTalk.Text.Trim() + "'";
                    StrSql = StrSql + " Where Seq = " + txtSeq.Text;
                    

                    Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);
                }
                tran.Commit();
                if (txtSeq.Text == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                }
                else
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
                }
            }
            catch (Exception)
            {
                tran.Rollback();

                if (txtSeq.Text == "")
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                }
                else
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));
                }
                
            }

            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();

                cls_Grid_Base_info_Put cgbp5 = new cls_Grid_Base_info_Put();
                cgbp5.dGridView_Put_baseinfo(this, dGridView_Talk, "talk", mtxtMbid.Text);
            }

        }

        private void dGridView_Talk_DoubleClick(object sender, EventArgs e)
        {
            txtTalk.Text = ""; txtSeq.Text = "";
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[3].Value != null))
            {
                string TalkContent = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                string Seq = (sender as DataGridView).CurrentRow.Cells[3].Value.ToString();

                txtTalk.Text = TalkContent; txtSeq.Text = Seq;                 
            }
        }


        private void button_Acc_Reg_Click(object sender, EventArgs e)
        {
            Reg_Bank_Account();
        }


        private void Reg_Bank_Account()
        {
            txtAccount_Reg.Text = "";

            lbl_ACC.Text = "미인증";

            string Sn = mtxtSn.Text.Replace("-", "").Replace("_", "").Trim();

            cls_Sn_Check csn_C = new cls_Sn_Check();
            string sort_TF = "";
            bool check_b = false;
            if (raButt_IN_1.Checked == true) //내국인인 구분자
                sort_TF = "in";

            if (raButt_IN_2.Checked == true) //외국인 구분자
                sort_TF = "fo";

            if (raButt_IN_3.Checked == true) //사업자 구분자.
                sort_TF = "biz";

            check_b = csn_C.Sn_Number_Check(Sn, sort_TF);

            Data_Set_Form_TF = 0;

            //if (check_b == false)
            //{
            //    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_SN_Number_Error")
            //           + "\n" +
            //           cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
            //    mtxtSn.Focus(); return;
            //}


            string me = "";

            if (txtAccount.Text == "")
            {
                me = "계좌번호를 필히 입력해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                txtAccount.Focus();
                return;
            }

            if (txtName_Accnt.Text == "")
            {
                me = "예금주를 필히 입력해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                txtName_Accnt.Focus();
                return;
            }


            if (txtBank_Code.Text == "")
            {
                me = "은행을 필히 선택해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                txtBank.Focus();
                return;
            }

            if (mtxtBrithDay.Text.Replace("-", "").Trim() == "")
            {
                me = "생년월일을 필히 선택해 주십시요." + "\n" +
                 cls_app_static_var.app_msg_rm.GetString("Msg_txt_Not_Data");

                MessageBox.Show(me);
                mtxtBrithDay.Focus();
                return;
            }


            cls_Sn_Check csc = new cls_Sn_Check();

            string successYN = "";

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                //successYN = csc.Bank_Acount_Check(txtName_Accnt.Text, mtxtSn.Text.Substring(0, 6), txtBank_Code.Text, txtAccount.Text);
                successYN = csc.Bank_Acount_Check(txtName_Accnt.Text, mtxtBrithDay.Text.Replace("-", "").Substring(2, 6), txtBank_Code.Text, txtAccount.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("계좌인증 오류");
                //MessageBox.Show(ee.ToString ());
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;

            if (successYN == "Y")
            {
                txtAccount_Reg.Text = txtAccount.Text;
                lbl_ACC.Text = "Success";
                me = "올바른 계좌 정보 입니다. 계좌인증 성공.";
                MessageBox.Show(me);
                txtName_E_1.Focus();
            }
            else
            {
                txtAccount_Reg.Text = "";
                lbl_ACC.Text = "Fail";
                me = "올바른 계좌 정보가 아닙니다. 확인후 다시 시도해 주십시요. 계좌인증 실패.";
                MessageBox.Show(me);
                txtAccount.Focus();
            }



        }

        private string EncryptSHA256_EUCKR(string phrase)
        {
            /*
            SHA256 sha = new SHA256Managed();

            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(phrase));

            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            return stringBuilder.ToString();
            */
            if (string.IsNullOrEmpty(phrase) == true)
            {
                return "";
            }
            else
            {
                Encoding encoding = Encoding.Unicode;

                SHA256 sha = new SHA256Managed();
                byte[] data = sha.ComputeHash(encoding.GetBytes(phrase));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary> 비밀번호 초기화를 원할시 ever + 생년월일로 비밀번호를 설정해줍시다. </summary>
        private void btnWebPasswordDefault_Click(object sender, EventArgs e)
        {
            if (idx_Mbid2 <= 0 || idx_Mbid2 == null)
                return; 
            if (mtxtBrithDay.Text.Replace("-", "").Length != 10)
            {
                MessageBox.Show("생년월일을 확인해주십시오.");
                mtxtBrithDay.Focus();
                return;
            }

            txtPassword.Text = "ever" + mtxtBrithDay.Text.Replace("-", "").Trim().Substring(2, 6);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Update Tbl_Memberinfo");
            sb.AppendLine("SET WebPassword = '"+ EncryptSHA256_EUCKR(txtPassword.Text.Trim()) + "'");
            sb.AppendLine("WHERE mbid2 = '" + idx_Mbid2 + "'");
            
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            
            Temp_Connect.Update_Data(sb.ToString(), this.Name, this.Text);
            MessageBox.Show("비밀번호가 정상적으로 변경 되었습니다.");
        }

        private void cbProvince_TH_SelectedIndexChanged(object sender, EventArgs e)
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_City_TH('" + cbProvince_TH.Text + "') ORDER BY ZIPCODE_SORT ");
            sb.AppendLine("SELECT ZIPCODE_NM FROM ufn_Get_ZipCode_District_TH('" + cbProvince_TH.Text + "') ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbDistrict_TH.DataBindings.Clear();
            cbDistrict_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbDistrict_TH.DisplayMember = "ZipCode_NM";
        }



        private void cbDistrict_TH_SelectedIndexChanged(object sender, EventArgs e)
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT * FROM dbo.ufn_Get_ZipCode_TH('" + cbDistrict_TH.Text + "') ");
            sb.AppendLine("SELECT ZIPCODE_NM FROM dbo.ufn_Get_ZipCode_SubDistrict_TH('" + cbDistrict_TH.Text + "') ORDER BY MinSubDistrictID ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            cbSubDistrict_TH.DataBindings.Clear();
            cbSubDistrict_TH.DataSource = ds.Tables["ZipCode_NM"];
            cbSubDistrict_TH.DisplayMember = "ZipCode_NM";
        }

        private void cbSubDistrict_TH_SelectedIndexChanged(object sender, EventArgs e)
        {
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT [ZIPCODE_NM] = PostCode FROM TLS_ZIPCODE_CS WITH(NOLOCK) WHERE SubDistrictThaiShort = '" + cbSubDistrict_TH.Text + "' ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "ZipCode_NM", ds) == false) return;

            if (Temp_conn.DataSet_ReCount <= 0) return;

            txtZipCode_TH.Text = "";
            txtZipCode_TH.Text = ds.Tables["ZipCode_NM"].Rows[0][0].ToString();

            //txtAddress2.Text = cbProvince_TH.Text + " " + cbDistrict_TH.Text + " " + cbProvince_TH.SelectedValue.ToString();
            txtAddress2.Text = cbSubDistrict_TH.Text + " " + cbDistrict_TH.Text + " " + cbProvince_TH.Text;


            //cbDistrict_TH.DataBindings.Clear();
            //cbDistrict_TH.DataSource = ds.Tables["ZipCode_NM"];
            //cbDistrict_TH.DisplayMember = "ZipCode_NM";
        }

        private void combo_Se_Code_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            combo_Se_2.SelectedIndex = combo_Se_Code_2.SelectedIndex;

            // 태국버전 인 경우
            if (combo_Se_Code_2.Text == "TH")
            {
                pnlDistrict_TH.Visible = true;
                pnlProvince_TH.Visible = true;
                pnlSubDistrict_TH.Visible = true;
                pnlZipCode_TH.Visible = true;
                pnlZipCode_KR.Visible = false;
                txtAddress2.ReadOnly = true;
                cbSubDistrict_TH_SelectedIndexChanged(this, null);
            }
            // 태국 이외 버전 인 경우
            else
            {
                pnlDistrict_TH.Visible = false;
                pnlProvince_TH.Visible = false;
                pnlSubDistrict_TH.Visible = false;
                pnlZipCode_TH.Visible = false;
                pnlZipCode_KR.Visible = true;
                txtAddress2.ReadOnly = false;
                txtAddress2.Clear();
            }
        }

        private void combo_Se_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            combo_Se_Code_2.SelectedIndex = combo_Se_2.SelectedIndex;

            // 태국버전 인 경우
            if (combo_Se_Code_2.Text == "TH")
            {
                pnlDistrict_TH.Visible = true;
                pnlProvince_TH.Visible = true;
                pnlSubDistrict_TH.Visible = true;
                pnlZipCode_TH.Visible = true;
                pnlZipCode_KR.Visible = false;
                txtAddress2.ReadOnly = true;
                cbSubDistrict_TH_SelectedIndexChanged(this, null);
            }
            // 태국 이외 버전 인 경우
            else
            {
                pnlDistrict_TH.Visible = false;
                pnlProvince_TH.Visible = false;
                pnlSubDistrict_TH.Visible = false;
                pnlZipCode_TH.Visible = false;
                pnlZipCode_KR.Visible = true;
                txtAddress2.ReadOnly = false;
                txtAddress2.Clear();
            }
        }


















    }
}
