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
    public partial class frmMember_Auto : Form
    {
        StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_cacu = new cls_Grid_Base();
        cls_Grid_Base cgb_Req = new cls_Grid_Base();
        cls_Grid_Base cgb_Req_Item = new cls_Grid_Base();
        cls_Grid_Base cgb_Req_Cacu = new cls_Grid_Base();
        cls_Grid_Base cgb_Req_Rece = new cls_Grid_Base();

        private const string base_db_name = "tbl_Memberinfo_AutoShip";
        private int Data_Set_Form_TF;

        private Dictionary<int, cls_AutoShip_Item> AutoShip_Item = new Dictionary<int, cls_AutoShip_Item>();
        private Dictionary<int, cls_AutoShip_Cacu> AutoShip_Cacu = new Dictionary<int,cls_AutoShip_Cacu>();
        private Dictionary<int, cls_AutoShip_Rece> AutoShip_Rece = new Dictionary<int, cls_AutoShip_Rece>();
        
        private Dictionary<string, TextBox> Ncode_dic = new Dictionary<string, TextBox>();

        private string idx_Mbid = "";
        private int idx_Mbid2 = 0;
        private string idx_Na_Code = "";
        private int idx_Sell_Mem_TF = 0;
        private int idx_Grade = 0;
        private string idx_CustomerGroupKey = "";

        //20190319 구현호 신청자인지 아닌지를 반환하는 전역변수
        private bool newbool;



        public frmMember_Auto()
        {
            InitializeComponent();
        }


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            System.DateTime.Now.ToString("yyyy");
            DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            Data_Set_Form_TF = 1;
            InitCombo();
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset(1);

            dGridView_Base_Cacu_Header_Reset();
            cgb_cacu.d_Grid_view_Header_Reset(1);

            dGridView_Base_Req__All_Header_Reset();
            cgb_Req_Item.d_Grid_view_Header_Reset(1);
            cgb_Req_Cacu.d_Grid_view_Header_Reset(1);
            cgb_Req_Rece.d_Grid_view_Header_Reset(1);

            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            cls_form_Meth cm = new cls_form_Meth();
            
            cm.from_control_text_base_chang(this);

            
            mtxtMbid.Mask = cls_app_static_var.Member_Number_Fromat;
            mtxtZip1.Mask = cls_app_static_var.ZipCode_Number_Fromat;
            mtxtProcDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtStartDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtReqDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtExtendDate.Mask = cls_app_static_var.Date_Number_Fromat;
            mtxtRecTel.Mask = cls_app_static_var.Tel_Number_Fromat;

            string[] data_Y = { ""
                              , int.Parse (cls_User.gid_date_time.Substring (0,4)).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 1 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 2 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 3 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 4 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 5 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 6 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 7 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 8 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 9 ).ToString()
                              , (int.Parse (cls_User.gid_date_time.Substring (0,4)) + 10 ).ToString()
                              };

            string[] data_M = { "","01", "02", "03", "04", "05" 
                               , "06", "07", "08", "09", "10" 
                               , "11", "12" 
                              };

            string[] data_P = {  "일시불"
                               , "2", "3", "4", "5"
                               , "6", "7", "8", "9", "10"
                               , "11", "12"
                              };

            for (int i = 0; i < 100; i++)
                cboProc_Cnt.Items.Add(i);

            // 각 콤보박스에 데이타를 초기화
            combo_Card_Year.Items.AddRange(data_Y);
            combo_Card_Month.Items.AddRange(data_M);
            combo_Card_Per.Items.AddRange(data_P);

            combo_Card_Year.SelectedIndex = 0;
            combo_Card_Month.SelectedIndex = 0;
            combo_Card_Per.SelectedIndex = 0;

            mtxtReqDate.Text = cls_User.gid_date_time;

            /*일단 포인트 탭은 제거*/
            tab_Cacu.TabPages.Remove(tab_Mile);

            InitComboZipCode_TH();
            // 태국버전 인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                pnl_Installment.Visible = false;    // 할부 개월
                pnl_Card_CVC.Visible = true;        // 카드보안코드
                pnlBirthInfo.Visible = false;       // 생년월일 6자리 or 사업자번호

                pnlDistrict_TH.Visible = true;
                pnlProvince_TH.Visible = true;
                pnlSubDistrict_TH.Visible = true;
                pnlZipCode_TH.Visible = true;
                mtxtZip1.Visible = false;
                txt_RecAddress1.ReadOnly = false;
                txt_RecAddress2.ReadOnly = true;
                //cbSubDistrict_TH_SelectedIndexChanged(this, null);
            }
            // 태국 이외 버전 인 경우
            else
            {
                pnl_Installment.Visible = true;     // 할부 개월
                pnl_Card_CVC.Visible = false;       // 카드보안코드
                pnlBirthInfo.Visible = true;        // 생년월일 6자리 or 사업자번호

                pnlDistrict_TH.Visible = false;
                pnlProvince_TH.Visible = false;
                pnlSubDistrict_TH.Visible = false;
                pnlZipCode_TH.Visible = false;
                mtxtZip1.Visible = true;
                txt_RecAddress2.ReadOnly = false;
                txt_RecAddress2.Clear();
            }

            cbProvince_TH.SelectedIndex = -1;
            cbDistrict_TH.SelectedIndex = -1;
            cbSubDistrict_TH.SelectedIndex = -1;
            txtZipCode_TH.Text = "";
            txt_RecAddress2.Text = "";

            mtxtMbid.Focus();

            Data_Set_Form_TF = 0;
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

        private void frmMember_Auto_Activated(object sender, EventArgs e)
        {
           //19-03-11 깜빡임제거 this.Refresh();
            mtxtMbid.Focus();
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {
            butt_Clear.Left = 0;
            butt_Delete.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Save.Left = butt_Clear.Left + butt_Clear.Width + 2;
            butt_Exit.Left = this.Width - butt_Exit.Width - 17;

            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Clear);
            cfm.button_flat_change(butt_Save);
            cfm.button_flat_change(butt_Delete);
            cfm.button_flat_change(butt_Excel);
            cfm.button_flat_change(butt_Exit);
            cfm.button_flat_change(butt_Member_Address);
            cfm.button_flat_change(butt_AddCode);
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

        private void txtData_Enter(object sender, EventArgs e)
        {
            //this.Refresh();
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

                if (tb.Tag != null)
                {
                    if (tb.Tag.ToString() == "2" && tb.Text != "")
                    {
                        Data_Set_Form_TF = 1;
                        double T_p = double.Parse(tb.Text.Replace(",", "").ToString());
                        tb.Text = string.Format(cls_app_static_var.str_Currency_Type, T_p);
                        Data_Set_Form_TF = 0;
                    }
                }
            }

            if (sender is MaskedTextBox)
            {
                MaskedTextBox tb = (MaskedTextBox)sender;
                if (tb.ReadOnly == false)
                    tb.BackColor = Color.White;

                //if (tb.Name == "mtxtReqDate" || tb.Name == "mtxtStartDate")
                //{
                //    Check_StartDate_WeekDay(tb);
                //}
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
                    }

                    if (mtb.Name == "mtxtTel3")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }
                    if (mtb.Name == "mtxtTel4")
                    {
                        if (Sn_Number_(Sn, mtb, "Tel") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtZip1")
                    {
                        if (Sn_Number_(Sn, mtb, "Zip") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtZip2")
                    {
                        if (Sn_Number_(Sn, mtb, "Zip") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtBrithDay" || mtb.Name == "mtxtBrithDayC")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }

                    if (mtb.Name == "mtxtRecTel")
                    {
                        mtxtZip1.Focus();
                    }
                    
                    string R4_name = mtb.Name.Substring(mtb.Name.Length - 4, 4);
                    if (R4_name == "Date" || R4_name == "ate3" || R4_name == "ate1" || R4_name == "ate2" || R4_name == "ate4")
                    {
                        if (Sn_Number_(Sn, mtb, "Date") == true)
                            SendKeys.Send("{TAB}");
                    }


                }
                else
                {
                    if (mtb.Name == "mtxtZip1")
                    {
                        frmBase_AddCode e_f = new frmBase_AddCode();
                        e_f.Send_Address_Info += new frmBase_AddCode.SendAddressDele(e_f_Send_Address_Info);
                        e_f.ShowDialog();
                        txt_RecAddress2.Focus();
                        SendKeys.Send("{TAB}");
                    }
                }


            }
        }

        

        void e_f_Send_MemName_Info(ref string searchMbid, ref int searchMbid2, ref string seachName)
        {
            seachName = txtName.Text.Trim();
            cls_Search_DB csb = new cls_Search_DB();
            csb.Member_Nmumber_Split(mtxtMbid.Text.Trim(), ref searchMbid, ref searchMbid2);
        }


        //변경할려는 대상자에 대한 회원번호에서 회원 검색창을 뛰엇을 경우에
        void e_f_Send_Mem_Number(string Send_Number, string Send_Name)
        {
            mtxtMbid.Text = Send_Number; txtName.Text = Send_Name;
            if (Input_Error_Check(mtxtMbid, "m") == true)
                Set_Form_Date(Send_Number);

            mtxtReqDate.Focus();
        }


        private Boolean Input_Error_Check(MaskedTextBox m_tb)
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

            if (Mbid == "" && Mbid2 == 0) //올바르게 회원번호 양식에 맞춰서 입력햇는가.
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Input_Err")
                        + "-" + cls_app_static_var.app_msg_rm.GetString("Msg_Sort_MemNumber")
                       + "\n" +
                       cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                m_tb.Focus(); return false;
            }

            if (idx_Mbid2 == 0)
                idx_Mbid2 = Mbid2;

            string Tsql = "";
            Tsql = "Select Mbid , Mbid2, M_Name , Sell_Mem_TF  ";
            Tsql = Tsql + " , LineCnt , N_LineCnt  ";
            Tsql = Tsql + " , LeaveDate , LineUserDate  ";
            Tsql = Tsql + " , Saveid  , Saveid2  ";
            Tsql = Tsql + " , Nominid , Nominid2 , Na_Code ";
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
            else
            {
                if (idx_Na_Code == "")
                    idx_Na_Code = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();
            }
            //++++++++++++++++++++++++++++++++            

            return true;
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
                            if (Input_Error_Check(mtb) == true)
                            {
                                Set_Form_Date(mtb.Text);
                                mtxtReqDate.Focus();
                            }
                            else
                            {
                                Form_Clear_();
                                mtxtMbid.Focus();
                            }
    
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
                            e_f.Call_searchNumber_Info += new frmBase_Member_Search.Call_searchNumber_Info_Dele(e_f_Send_MemName_Info);
                        }

                        e_f.ShowDialog();

                        SendKeys.Send("{TAB}");
                    }
                }
                else
                    SendKeys.Send("{TAB}");
            }

        }


        //회원번호 클릿햇을때. 관련 정보들 다 리셋 시킨다.
        //추후 번호만 변경하고 엔터 안누눌러서.. 데이타가 엉키는 것을 방지하기 위함.
        private void mtxtMbid_Click(object sender, EventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Name == "mtxtMbid")
            {
                Form_Clear_();
            }

            Data_Set_Form_TF = 1;

            //마스크텍스트 박스에 입력한 내용이 있으면 그곳 다음으로 커서가 가게 한다.
            if (mtb.Text.Replace("-", "").Replace("_", "").Trim() != "")
                mtb.SelectionStart = mtb.Text.Replace("-", "").Replace("_", "").Trim().Length + 1;

            Data_Set_Form_TF = 0;
        }

        private void MtxtData_Sn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                MaskedTextBox mtb = (MaskedTextBox)sender;
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


        private void txtData_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            int Sw_Tab = 0;

            if ((sender is TextBox) == false) return;

            TextBox tb = (TextBox)sender;

            if (tb.Name == "txt_Card_Name")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                {
                    txt_Card_Code.Text = "";
                }
                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txt_RecName" || tb.Name == "txt_RecAddress1" || tb.Name == "txt_RecAddress2")
            {
                if (AutoShip_Rece != null)
                {
                    foreach (int t_key in AutoShip_Rece.Keys)
                    {
                        AutoShip_Rece[t_key].Del_TF = "U";
                    }
                }
            }

            if (tb.Name == "txtMilePartner")
            {
                Data_Set_Form_TF = 1;
                if (tb.Text.Trim() == "")
                    txtMilePartner_Code.Text = "";
                Data_Set_Form_TF = 0;
            }

        }

        private void mtxtData_TextChanged(object sender, EventArgs e)
        {
            if ((sender is MaskedTextBox) == false) return;

            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Name == "mtxtRecTel" || mtb.Name == "mtxtZip1")
            {
                if (AutoShip_Rece != null)
                {
                    foreach (int t_key in AutoShip_Rece.Keys)
                    {
                        AutoShip_Rece[t_key].Del_TF = "U";
                    }
                }
            }

        }


        private void txtBox_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TextBox) == false) return;

            TextBox tb = (TextBox)sender;

            if (tb.Name == "txt_Card_Name")
            {
                if (Ncode_dic != null)
                    Ncode_dic.Clear();
                Ncode_dic["ncode"] = txt_Card_Code;
                Ncode_dic["cardname"] = tb;

                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, "");
                else
                    Ncod_Text_Set_Data(tb);

                SendKeys.Send("{TAB}");
                Data_Set_Form_TF = 0;
            }


        }
        private void Db_Grid_Popup(TextBox tb, string strSql)
        {

            cls_Grid_Base_Popup cgb_Pop = new cls_Grid_Base_Popup();
            DataGridView Popup_gr = new DataGridView();
            Popup_gr.Name = "Popup_gr";
            this.Controls.Add(Popup_gr);
            cgb_Pop.basegrid = Popup_gr;
            cgb_Pop.Base_fr = this;
            cgb_Pop.Base_text_dic = Ncode_dic;
            cgb_Pop.Base_tb_2 = tb;    //2번은 명임
            cgb_Pop.Base_Location_obj = tb;

            if (strSql != "")
            {
                if (tb.Name == "txt_Card_Name")
                {
                    cgb_Pop.db_grid_Popup_Base(2, "카드_코드", "카드명"
                                                , "ncode", "CardName"
                                                , strSql);
                    cgb_Pop.Next_Focus_Control = txt_CardOwner;
                }
            }
            else
            {
                if (tb.Name == "txt_Card_Name")
                {
                    string Tsql;
                    Tsql = "Select  Ncode, cardname   ";
                    Tsql = Tsql + " From tbl_Card (nolock) ";
                    Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                    Tsql = Tsql + " OR    cardname like '%" + tb.Text.Trim() + "%')";
                    if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";

                    cgb_Pop.db_grid_Popup_Base(2, "카드_코드", "카드명"
                                                , "ncode", "CardName"
                                                , Tsql);

                    cgb_Pop.Next_Focus_Control = txt_CardOwner;

                }
                if (tb.Name == "txtMilePartner")
                {
                    string Tsql;

                    Tsql = " SELECT PartnerCode, PartnerName  ";
                    Tsql = Tsql + " FROM tbl_Member_Mileage_Partner (NOLOCK)  ";
                    Tsql = Tsql + " WHERE PartnerCode Like '%" + tb.Text.Trim() + "%'  ";
                    Tsql = Tsql + " OR PartnerName Like '%" + tb.Text.Trim() + "%'  ";
                    Tsql = Tsql + " ORDER BY SortNumber ASC   ";

                    cgb_Pop.db_grid_Popup_Base(2, "제휴사코드", "제휴사명"
                                                , "PartnerCode", "PartnerName"
                                                , Tsql);

                    cgb_Pop.Next_Focus_Control = txt_Mile_Price;
                }
            }
            
        }

        private void Ncod_Text_Set_Data(TextBox tb)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            if (tb.Name == "txt_Card_Name")
            {
                Tsql = "Select  Ncode, cardname   ";
                Tsql = Tsql + " From tbl_Card (nolock) ";
                Tsql = Tsql + " Where ( Ncode like '%" + tb.Text.Trim() + "%'";
                Tsql = Tsql + " OR    cardname like '%" + tb.Text.Trim() + "%')";
                if (idx_Na_Code != "") Tsql = Tsql + " And   Na_Code = '" + idx_Na_Code + "'";
            }

            if (tb.Name == "txtMilePartner")
            {
                Tsql = " SELECT PartnerCode, PartnerName ";
                Tsql = Tsql + " FROM tbl_Member_Mileage_Partner (NOLOCK) ";
                Tsql = Tsql + " WHERE PartnerCode Like '%" + tb.Text.Trim() + "%' ";
                Tsql = Tsql + " OR PartnerName Like '%" + tb.Text.Trim() + "%' ";
                Tsql = Tsql + " ORDER BY SortNumber ASC ";
            }


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "t_P_table", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 1)
            {
                int fCnt = 0;
                foreach (string t_key in Ncode_dic.Keys)
                {
                    Ncode_dic[t_key].Text = ds.Tables["t_P_table"].Rows[0][fCnt].ToString();
                    fCnt++;
                }
            }

            if ((ReCnt > 1) || (ReCnt == 0)) Db_Grid_Popup(tb, Tsql);
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
                        if (Input_Error_Check(mtxtMbid, "m") == true)
                            Set_Form_Date(mtxtMbid.Text);
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


        void T_R_Key_Enter_13()
        {
            SendKeys.Send("{TAB}");
        }


        void T_R_Key_Enter_13_Ncode(string txt_tag, TextBox tb)
        {
            if (tb.Name == "txt_Card_Name")
            {
                if (Ncode_dic != null)
                    Ncode_dic.Clear();
                Ncode_dic["ncode"] = txt_Card_Code;
                Ncode_dic["cardname"] = tb;

                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, "");
                else
                    Ncod_Text_Set_Data(tb);
                SendKeys.Send("{TAB}");

                Data_Set_Form_TF = 0;
            }

            if (tb.Name == "txtMilePartner")
            {
                if (Ncode_dic != null)
                    Ncode_dic.Clear();
                Ncode_dic["PartnerCode"] = txtMilePartner_Code;
                Ncode_dic["PartnerName"] = tb;

                if (tb.Text.ToString() == "")
                    Db_Grid_Popup(tb, "");
                else
                    Ncod_Text_Set_Data(tb);
                SendKeys.Send("{TAB}");

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
                cgb_Pop.Next_Focus_Control = butt_AddCode;
            if (tb.Name == "txtMilePartner")
                cgb_Pop.Next_Focus_Control = txt_Mile_Price;
            

            cgb_Pop.Db_Grid_Popup_Make_Sql(tb, tb1_Code, idx_Na_Code);
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
        }


        private void Form_Clear_()
        {

            Data_Set_Form_TF = 1;
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>                
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            dGridView_Base_Req__All_Header_Reset();
            cgb_Req_Item.d_Grid_view_Header_Reset(1);
            cgb_Req_Cacu.d_Grid_view_Header_Reset(1);
            cgb_Req_Rece.d_Grid_view_Header_Reset(1);

            cls_form_Meth ct = new cls_form_Meth();
            ct.from_control_clear(this);

            idx_Mbid = "";
            idx_Mbid2 = 0;
            idx_Na_Code = "";
            combo_Card_Per.Text = "";
            if (AutoShip_Item != null)
                AutoShip_Item.Clear();
            if (AutoShip_Cacu != null)
                AutoShip_Cacu.Clear();
            if (AutoShip_Rece != null)
                AutoShip_Rece.Clear();

            Base_Sub_Clear("item");
            Base_Sub_Clear("Rece");
            Base_Sub_Clear("Cacu");

            mtxtReqDate.Text = cls_User.gid_date_time;
            txtName.ReadOnly = false;
            cboProc_Cnt.SelectedIndex = -1;

            cbProvince_TH.SelectedIndex = -1;
            cbDistrict_TH.SelectedIndex = -1;
            cbSubDistrict_TH.SelectedIndex = -1;

            Data_Set_Form_TF = 0;

            mtxtMbid.Focus();
            chK_PV_CV_Check.Checked = true;
        }


        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
                      
            if (bt.Name == "butt_Save")
            {
                int Save_Error_Check = 0;       Data_Set_Form_TF = 1;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                //저장이 이루어진다.
                Save_Base_Data(ref Save_Error_Check);  //저장이 이루어진다

                               
                Data_Set_Form_TF = 0;
                if (Save_Error_Check > 0)
                {
                    if (txtAutoSEQ.Text == "")
                    {
                        cls_Search_DB csd = new cls_Search_DB();
                        string T_Mbid = mtxtMbid.Text.Trim();
                        //에러 없이 막 등록한 오토쉽 주문번호 텍스트 컨트롤에서 텍스트를 가져온다.
                        string ordernumber = txtAutoSEQMessage.Text;
                        string Mbid = ""; int Mbid2 = 0;
                        csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

                        

                        int Save_Error_Check2 = 0; 

                        //오토쉽 1회차가 결제 되게 됨 2020-09-16 김팀이 추가함..
                        Save_Base_Data_Auto_First(ref Save_Error_Check2, txtAutoSEQMessage.Text, Mbid2, txtName.Text);
                        //오토쉽 1회차가 결제 되게 됨 2020-09-16 김팀이 추가함..


                        






                        ////오토쉽등록시 문자 ㄱㄱ
                        //Send_SMS_Message_Congratulations_membership(Mbid.ToString(), Mbid2.ToString(), ordernumber);

                        //에러 없이 막 등록한 오토쉽 주문번호 텍스트 컨트롤의 텍스트를 비워준다.
                        txtAutoSEQMessage.Text = "";
                    }
                    Form_Clear_();
                    mtxtMbid.Focus();
                   
                }
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

            else if (bt.Name == "butt_Delete")
            {
                int Delete_Error_Check = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Delete_Base_Data(ref Delete_Error_Check);

                if (Delete_Error_Check > 0)
                    Form_Clear_();

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }


            else if (bt.Name == "butt_Clear")
            {
                Form_Clear_();
                mtxtMbid.Focus();

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

                txt_RecAddress2.Focus();
            }

            else if (bt.Name == "butt_Member_Address")
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Member_Address_Search();
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }

        }
        //private void Send_SMS_Message_Congratulations_membership(string mbid, string mbid2, string ordernumber)
        //{
        //    cls_Connect_DB Temp_Connect = new cls_Connect_DB();
        //    string Tsql;
        //    Tsql = "EXEC Usp_Insert_SMS '30', '', " + mbid2.ToString() + ", '" + ordernumber.ToString() + "', ''";

        //    DataSet ds = new DataSet();

        //    //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
        //    if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false)
        //        return;

        //}
        private void Member_Address_Search()
        {
            if (idx_Mbid2 == 0)
                return;


            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            Tsql = " SELECT ";
            Tsql = Tsql + " M_Name ";
            Tsql = Tsql + " , REPLACE(hptel, '-', '') AS hptel ";
            Tsql = Tsql + " , Addcode1 ";
            Tsql = Tsql + " , Address1 ";
            Tsql = Tsql + " , Address2 ";
            Tsql = Tsql + " , city ";
            Tsql = Tsql + " , state ";
            Tsql = Tsql + " FROM tbl_Memberinfo (NOLOCK) ";
            Tsql = Tsql + " WHERE mbid = '" + idx_Mbid + "' ";
            Tsql = Tsql + " AND mbid2 = " + idx_Mbid2;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "Address", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {
                txt_RecName.Text = ds.Tables["Address"].Rows[0]["M_Name"].ToString();
                mtxtRecTel.Text = ds.Tables["Address"].Rows[0]["hptel"].ToString();
                
                txt_RecAddress1.Text = ds.Tables["Address"].Rows[0]["Address1"].ToString();
                txt_RecAddress2.Text = ds.Tables["Address"].Rows[0]["Address2"].ToString();

                // 태국인 경우
                if (cls_User.gid_CountryCode == "TH")
                {
                    try
                    {
                        cbProvince_TH.Text = ds.Tables["Address"].Rows[0]["Address2"].ToString().Split(' ')[2];
                        cbDistrict_TH.Text = ds.Tables["Address"].Rows[0]["Address2"].ToString().Split(' ')[1];
                        cbSubDistrict_TH.Text = ds.Tables["Address"].Rows[0]["Address2"].ToString().Split(' ')[0];
                    }
                    catch (Exception)
                    {
                        cbProvince_TH.Text = "";
                        cbDistrict_TH.Text = "";
                        cbSubDistrict_TH.Text = "";
                    }

                    txtZipCode_TH.Text = ds.Tables["Address"].Rows[0]["Addcode1"].ToString();
                }
                // 한국인 경우
                else
                {
                    mtxtZip1.Text = ds.Tables["Address"].Rows[0]["Addcode1"].ToString();
                }
            }
        }

        private void Delete_Base_Data(ref int Delete_Error_Check)
        {
            Delete_Error_Check = 0;
            if (txtAutoSEQ.Text == "") return;
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            cls_Search_DB csd = new cls_Search_DB();
            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            string Auto_Seq = "";
            Auto_Seq = txtAutoSEQ.Text;

            try
            {
                string StrSql = "";

                StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Mod_Del ";
                StrSql = StrSql + " SELECT *, 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '', '' ";
                StrSql = StrSql + " FROM tbl_Memberinfo_AutoShip (nolock) ";
                StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
                if (Mbid.Length ==0)
                    StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();
                else
                {
                    StrSql = StrSql + " And mbid = '" + Mbid + "' ";
                    StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();
                }
                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);

                StrSql = " DELETE FROM tbl_Memberinfo_AutoShip Where Auto_Seq = '" + Auto_Seq + "'";
                if (Mbid.Length ==0)
                    StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();
                else
                {
                    StrSql = StrSql + " And mbid = '" + Mbid + "' ";
                    StrSql = StrSql + " And mbid2 = " + Mbid2.ToString();
                }
                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);


                StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Item_Mod_Del ";
                StrSql = StrSql + " SELECT *, 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '' ";
                StrSql = StrSql + " FROM tbl_Memberinfo_AutoShip_Item (nolock) ";
                StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);

                StrSql = " DELETE FROM tbl_Memberinfo_AutoShip_Item Where Auto_Seq = '" + Auto_Seq + "'";
                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);

                StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Cacu_Mod_Del ";
                StrSql = StrSql + " SELECT *, 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '', '' ";
                StrSql = StrSql + " FROM tbl_Memberinfo_AutoShip_Cacu (nolock) ";
                StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);

                StrSql = " DELETE FROM tbl_Memberinfo_AutoShip_Cacu Where Auto_Seq = '" + Auto_Seq + "'";
                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);

                StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Rece_Mod_Del ";
                StrSql = StrSql + " SELECT *, 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '' ";
                StrSql = StrSql + " FROM tbl_Memberinfo_AutoShip_Rece (nolock) ";
                StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
                Temp_Connect.Insert_Data(StrSql, base_db_name, Conn, tran);

                StrSql = " DELETE FROM tbl_Memberinfo_AutoShip_Rece Where Auto_Seq = '" + Auto_Seq + "'";
                Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name, this.Text);

                tran.Commit();
                Delete_Error_Check = 1;
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del"));

                csd.tbl_Memberinfo_Mod(mtxtMbid.Text.Trim());

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

        private void e_f_Send_Address_Info(string AddCode1, string AddCode2, string Address1, string Address2, string Address3)
        {
            mtxtZip1.Text = AddCode1 + "-" + AddCode2;
            txt_RecAddress1.Text = Address1; txt_RecAddress2.Text = Address2;            
        }

        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
          
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);

        }

        private void DTP_Base_CloseUp1(object sender, EventArgs e)
        {

            //if (txt_ReqState.Text != "연장")
            //{
            //    MessageBox.Show("연장건만 자동주문실행일자 수동변경이 가능합니다.");
            //    txt_RecName.Focus();
            //    return;
            //}
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);

        }

        private void DTP_Base_CloseUp2(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
            if (sender == DTP_StartDate)
            {
                mtxtProcDate.Text = mtxtStartDate.Text;
                string day = "05";
                if (mtxtStartDate.Text.Replace("-", "").Length == 8)
                    day = mtxtStartDate.Text.Substring(mtxtStartDate.SelectionLength - 2, 2);

                if (day == "05")
                {
                    RadioFiveDay.Checked = true;
                    RadioTenDay.Checked = false;
                    RadioFifteenDay.Checked = false;
                    RadioTwentyDay.Checked = false;
                    RadioTwentyfiveDay.Checked = false;
                }
                else if (day == "10")
                {
                    RadioFiveDay.Checked = false;
                    RadioTenDay.Checked = true;
                    RadioFifteenDay.Checked = false;
                    RadioTwentyDay.Checked = false;
                    RadioTwentyfiveDay.Checked = false;
                }
                else if (day == "15")
                {
                    RadioFiveDay.Checked = false;
                    RadioTenDay.Checked = false;
                    RadioFifteenDay.Checked = true;
                    RadioTwentyDay.Checked = false;
                    RadioTwentyfiveDay.Checked = false;
                }
                else if (day == "20")
                {
                    RadioFiveDay.Checked = false;
                    RadioTenDay.Checked = false;
                    RadioFifteenDay.Checked = false;
                    RadioTwentyDay.Checked = true;
                    RadioTwentyfiveDay.Checked = false;
                }
                else if (day == "25")
                {
                    RadioFiveDay.Checked = false;
                    RadioTenDay.Checked = false;
                    RadioFifteenDay.Checked = false;
                    RadioTwentyDay.Checked = false;
                    RadioTwentyfiveDay.Checked = true;
                }
            }


        }

        private void Base_Grid_Set()
        {           
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< 
            string Tsql = "";

            string TodayDate = DateTime.Now.ToString("yyyyMMdd");
             
            Tsql = "Select 0, Name , NCode ,price4,price5 ,price2    ";
            Tsql = Tsql + " , '', '' ,'' ,'' ,'' ";
            Tsql = Tsql + " From UFN_GOOD_SEARCH_WEB_SELL('" + cls_User.gid_date_time + "', '" + cls_User.gid_CountryCode + "' , 9) ";
            Tsql = Tsql + " Where AutoShipYN = 'Y' ";
            Tsql = Tsql + " Order by Ncode ";

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

        private void Base_Grid_Set_Update(string Auto_seq)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< 
            string Tsql = "";

            string TodayDate = DateTime.Now.ToString("yyyyMMdd");

            Tsql = "Select 0, Name , NCode ,price4,price5 ,price2    ";
            Tsql = Tsql + " , '', '' ,'' ,'' ,'' ";
            Tsql = Tsql + " From UFN_GOOD_SEARCH_WEB_SELL('" + cls_User.gid_date_time + "', '" + cls_User.gid_CountryCode +"' , 9) ";
            Tsql = Tsql + " Where AutoShipYN = 'Y'   ";
            Tsql = Tsql + " or ncode in (select itemcode from  tbl_Memberinfo_AutoShip_Item where Auto_Seq = '"+ Auto_seq  + "' )  ";
            Tsql = Tsql + " Order by Ncode ";

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
            int Col_Cnt = 0;
            object[] row0 = new object[cgb.grid_col_Count];

            while (Col_Cnt < cgb.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }

            gr_dic_text[fi_cnt + 1] = row0;
        }

        private void dGridView_Base_Header_Reset()
        {
            cgb.grid_col_Count = 10;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.CellSelect;
            cgb.grid_Frozen_End_Count = 2;
            //cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"수량"  , "상품명"   , "상품코드"  , "PV"   , "CV"        
                                , "회원가"   , ""    , ""   , ""    , ""                                
                                    };
            string[] g_Cols = {"ItemCount"  , "ItemName"   , "ItemCode"  , "ItemPV"   , "ItemCV"
                                , "ItemPrice"   , "T2"    , "T3"   , "T4"    , "T5"                                
                                    };

            cgb.grid_col_header_text = g_HeaderText;
            cgb.grid_col_name = g_Cols;

            int[] g_Width = { 80, 130, 100, 70, 70                             
                             ,70 , 0 ,  0 , 0 ,  0                             
                            };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { false , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                                                         
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleRight 
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight  //5
                               
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleCenter //10                                                           
                              };
            cgb.grid_col_alignment = g_Alignment;


            DataGridViewColumnSortMode[] g_SortM =
                              {DataGridViewColumnSortMode.Automatic  
                               ,DataGridViewColumnSortMode.Automatic  
                               ,DataGridViewColumnSortMode.Automatic  
                               ,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic  //5
                               
                               ,DataGridViewColumnSortMode.Automatic                              
                               ,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic
                               ,DataGridViewColumnSortMode.Automatic 
                               ,DataGridViewColumnSortMode.Automatic //10                                                           
                              };
            cgb.grid_col_SortMode = g_SortM;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb.grid_cell_format = gr_dic_cell_format;

            cgb.basegrid.RowHeadersVisible = false;
        }


        private void Set_gr_dic_Cacu(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
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
        
        private void dGridView_Base_Cacu_Header_Reset()
        {
            cgb_cacu.grid_col_Count = 10;
            cgb_cacu.basegrid = dGridView_Base_Cacu;
            cgb_cacu.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_cacu.grid_Frozen_End_Count = 4;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"Cacu_Index", "Card_Code"  , "제휴사명"   , "카드번호"  , "유효기간(년)"   
                                    , "유효기간(월)", "할부개월", "카드 소유자명", "결제금액", "카드보안코드"
                                    };
            cgb_cacu.grid_col_header_text = g_HeaderText;

            int[] g_Width;
            // 태국 인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                g_Width = new int[] { 0, 0, 0, 100, 100
                             ,100, 100, 80, 100, 100
                            };
            }
            // 한국 인 경우
            else
            {
                g_Width = new int[] { 0, 0, 0, 100, 100
                             ,100, 0, 80, 100, 0
                            };
            }

            cgb_cacu.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true, true, true
                                   };
            cgb_cacu.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {
                                DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleCenter 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleCenter
                              };
            cgb_cacu.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;           
            cgb_cacu.grid_cell_format = gr_dic_cell_format;
        }


        private bool  Check_TextBox_Error_Date()
        {
            cls_Check_Input_Error c_er = new cls_Check_Input_Error();
            if (mtxtReqDate.Text.Replace("-", "").Replace("/", "").Trim() != "")
            {
                if (Sn_Number_(mtxtReqDate.Text, mtxtReqDate, "Date") == false)
                {
                    mtxtReqDate.Focus();
                    return false;
                }
            }

            return true;
        }


        private Boolean Check_TextBox_ETC_Error()
        {   
            cls_Check_Text T_R = new cls_Check_Text();
            string me = "";

            me = T_R.Text_Null_Check(mtxtMbid, "Msg_Sort_MemNumber"); //성명을 필히 넣어야 합니다.
            if (me != "")
            {
                MessageBox.Show(me);
                return false;
            }

            me = T_R.Text_Null_Check(txtName, "Msg_Sort_M_Name"); //성명을 필히 넣어야 합니다.
            if (me != "")
            {
                MessageBox.Show(me);
                return false;
            }

            return true;
        }
        private Boolean Check_mtxtStardendtDate_Error()
        {
            cls_Check_Text T_R = new cls_Check_Text();
            string me = "";
            int a = mtxtStartDate.Text.Length;
            int b = mtxtProcDate.Text.Length;


            if (a != 10 || b != 10)
            {
                me = "오토쉽 시작일자와 다음 ADS시작일자의 날짜를 정확히 넣어주세요. " + "\n";
                MessageBox.Show(me);
                return false;
            }
            return true;
        }
        //private Boolean Check_One_Error()
        //{
        //    cls_Check_Text T_R = new cls_Check_Text();
        //    string me = "";
        //    int a = mtxtStartDate.Text.Length;
        //    int b = mtxtProcDate.Text.Length;


        //    if (a != 10 || b != 10)
        //    {
        //        me = "오토쉽이 이미 존재합니다 기존 오토쉽을 수정해주세요. " + "\n";
        //        MessageBox.Show(me);
        //        return false;
        //    }
        //    return true;
        //}
        private Boolean Check_Save_Error()
        {
            //if (rb_ReqMonth_3.Checked == false)
            //{
            //    MessageBox.Show("신청개월을 선택하시기 바랍니다.");
            //    return false;
            //}

            if (txt_RecName.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please enter the recipient.");
                }
                else
                {
                    MessageBox.Show("수령인을 입력하시기 바랍니다.");
                }
                txt_RecName.Focus();
                return false;
            }

            if (txt_RecAddress1.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please select an address.");
                }
                else
                {
                    MessageBox.Show("주소를 선택하시기 바랍니다.");
                }
                butt_AddCode.Focus();
                return false;
            }

            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                if (txtZipCode_TH.Text.Replace("-", "").Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Please select your zip code.");
                    }
                    else
                    {
                        MessageBox.Show("우편번호를 선택하시기 바랍니다.");
                    }
                    butt_AddCode.Focus();
                    return false;
                }
                else if (cbProvince_TH.Text == "")
                {
                    MessageBox.Show("Please select province.");
                    return false;
                }
                else if (cbDistrict_TH.Text == "")
                {
                    MessageBox.Show("Please select district.");
                    return false;
                }
                else if (cbSubDistrict_TH.Text == "")
                {
                    MessageBox.Show("Please select subdistrict.");
                    return false;
                }
            }
            // 한국인 경우
            else
            {
                if (mtxtZip1.Text.Replace("-", "").Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Please select your zip code.");
                    }
                    else
                    {

                        MessageBox.Show("우편번호를 선택하시기 바랍니다.");
                    }
                    butt_AddCode.Focus();
                    return false;
                }
            }



            //if (chk_AutoExtend.Checked == false)
            //{
            //    if (MessageBox.Show("자동연장이 선택되어 있지 않습니다.\n계속 진행하시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No)
            //    {
            //        return false;
            //    }
            //}




            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string StrSql = "";
            StrSql = " SELECT Sell_Mem_TF From tbl_Memberinfo (nolock) ";
            StrSql = StrSql + " Where mbid = '" + idx_Mbid + "' AND mbid2 = " + idx_Mbid2;

            DataSet ds = new DataSet();
            if (Temp_Connect.Open_Data_Set(StrSql, "Sell_Mem_TF", ds) == false) return false;

            //2018-09-29 요청에 의해서 우대회원도 가능하게 품 김영수 팀장.
            //if (ds.Tables["Sell_Mem_TF"].Rows[0][0].ToString() == "1")
            //{
            //    MessageBox.Show("소비자 회원은 ADS를 신청할 수 없습니다.");
            //    return false;
            //}

            //결제할 금액하고 총액이 다르면 저장 안되게
            double SumAmt = 0, DeliveryAmt = 0, Payment = 0;

            if (AutoShip_Cacu != null)
            {
                
                foreach (int t_key in AutoShip_Cacu.Keys)
                {
                    if (AutoShip_Cacu[t_key].Del_TF != "D")
                    {
                        Payment = Payment + AutoShip_Cacu[t_key].Payment_Amt;
                    }
                }

                
            }

            if (txt_TotalItemPrice.Text.Replace(",", "").Trim() == "") 
                SumAmt = 0;
            else
                SumAmt = double.Parse(txt_TotalItemPrice.Text.Replace(",", ""));

            if (txtDeliverPrice.Text.Replace(",", "").Trim() == "")
                DeliveryAmt = 0;
            else
                DeliveryAmt = double.Parse(txtDeliverPrice.Text.Replace(",", "").Trim());

            if (Payment != SumAmt + DeliveryAmt)
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Payment amounts do not match.");
                }
                else
                {
                    MessageBox.Show("결제 금액이 일치하지 않습니다.");
                }
                return false;
            }

            

            return true;
        }


        /// <summary> 2018-07-02 지성경 : 이게 왜있는걸까? 무조건 return true 처리함</summary>
        private Boolean Check_Update_Error(string AutoSeq)
        {
            /*
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            string Tsql = "";

            Tsql = " SELECT ";
            Tsql = Tsql + " CASE WHEN Proc_Cnt / Req_Month = 0 THEN 'Y' ELSE 'N' END AS UpdateChk ";
            Tsql = Tsql + " FROM tbl_Memberinfo_AutoShip (NOLOCK) ";
            Tsql = Tsql + " WHERE Auto_Seq = '" + AutoSeq + "' ";

            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo_AutoShip", ds, this.Name, this.Text) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ds.Tables["tbl_Memberinfo_AutoShip"].Rows[0]["UpdateChk"].ToString() != "Y")
                return false;
            else
             * */
                return true;
        }


        private Boolean Check_Save_Item_Error()
        {
            string ItemChk = "N";

            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() != "0")
                {
                    ItemChk = "Y";
                    break;
                }
            }

            if (ItemChk == "N")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please select an item.");
                }
                else
                {
                    MessageBox.Show("품목을 선택하시기 바랍니다.");
                }
                return false;
            }

            int ItemCount = 0;
            int SumItemCount = 0;
            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                if(int.TryParse(dGridView_Base.Rows[i].Cells[0].Value.ToString(), out ItemCount))
                {
                    SumItemCount += ItemCount;
                }
                
            }

            if(SumItemCount == 0)
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please check the number of items.");
                }
                else
                {
                    MessageBox.Show("품목을 개수 확인 바랍니다.");
                }
                return false;
            }

            return true;
        }


        private Boolean Check_Save_Card_Error()
        {
            int CardCnt = 0;

            if (AutoShip_Cacu != null)
            {
                foreach (int t_key in AutoShip_Cacu.Keys)
                {
                    if (AutoShip_Cacu[t_key].Del_TF != "D")
                    {
                        CardCnt = CardCnt + 1;
                    }
                }
            }

            if (CardCnt == 0)
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please register your card.");
                }
                else
                {

                    MessageBox.Show("카드를 등록하시기 바랍니다.");
                }
                return false;
            }
            else if (CardCnt > 1)
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Only one card can be registered.");
                }
                else
                {

                    MessageBox.Show("카드는 하나만 등록 가능합니다.");
                }
                return false;
            }

            return true;
        }

        //저장 버튼을 눌럿을때 실행되는 메소드 실질적인 변경 작업이 이루어진다.
        private void Save_Base_Data(ref int Save_Error_Check)
        {
            Save_Error_Check = 0;
            if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                                    
            if (Check_TextBox_ETC_Error() == false) return;  //전화번호 웹아이디 주민번호 같은 부가적인 입력 사항에 대한 오류를 체크한다.
            if (Check_TextBox_Error_Date() == false) return; //날짜 관련 텍스트 파일들에 대해서 날짜 오류를 체크한다
            if (Check_Save_Error() == false) return;
            if (Check_Save_Item_Error() == false) return;
            if (Check_Save_Card_Error() == false) return;
            //if (Check_mtxtStardendtDate_Error() == false) return;
            //if (Check_One_Error() == false) return;

            if (txtAutoSEQ.Text.Trim() != "")
            {
                if (Check_Update_Error(txtAutoSEQ.Text.Trim()) == false) return;
            }

            cls_Search_DB csd = new cls_Search_DB();
            
            string T_Mbid = mtxtMbid.Text.Trim();
            string Mbid = ""; int Mbid2 = 0;
            csd.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2);

            if (Mbid == "" && Mbid2 == 0)
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Mem_Mbid_Not")
                          + "\n" +
                          cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                return;
            }

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();
            string AutoSeq = "";

            try
            {
                if (txtAutoSEQ.Text.Trim() == "")
                {
                    //저장할 것에 대한 주문번호를 따온다          
                    DB_Save_tbl_AutoShip_Number(Temp_Connect, Conn, tran, ref AutoSeq);
                    //에러 없이 막 등록한 오토쉽 주문번호를 텍스트 컨트롤에 넣는다.(메세지발송용)
                    txtAutoSEQMessage.Text = AutoSeq;
                    if (AutoSeq == "") //주문번호 미발급시 오류로 해서 되돌린다.  
                    {
                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));

                        tran.Dispose();
                        Temp_Connect.Close_DB();

                        return;
                    }
                }
                else
                    AutoSeq = txtAutoSEQ.Text;

                DB_Save_tbl_AutoShip(Temp_Connect, Conn, tran, AutoSeq);   //오토쉽 헤더
                DB_Save_tbl_AutoShip_Item(Temp_Connect, Conn, tran, AutoSeq);   //오토쉽 품목
                DB_Save_tbl_AutoShip_Cacu(Temp_Connect, Conn, tran, AutoSeq);   //오토쉽 결제
                DB_Save_tbl_AutoShip_Rece(Temp_Connect, Conn, tran, AutoSeq);   //오토쉽 배송

                cls_form_Meth cm = new cls_form_Meth();     
                            
                tran.Commit();
                Save_Error_Check = 1;

                if (txtAutoSEQ.Text == "")
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save"));
                else
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit"));
            }
             catch (Exception ee)
            {
                tran.Rollback();
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
            }
            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }

        }


        private void DB_Save_tbl_AutoShip_Number(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran, ref string AutoSeq)
        {
            string IndexTime = "";
            IndexTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");               

            string StrSql = "";
            StrSql = "INSERT INTO tbl_Autoship_OrdNumber ";
            StrSql = StrSql + " (Auto_Seq , Mbid , Mbid2 ";
            StrSql = StrSql + " , ReqDate , IndexTime ";
            StrSql = StrSql + " , User_TF)";
            StrSql = StrSql + " Select ";
            StrSql = StrSql + " '" + mtxtReqDate.Text.Replace("-", "").Trim().Substring(2, 6) + "'";

            StrSql = StrSql + " + Right('00000' + convert(varchar(5),convert(Float,isnull(Max(Right(Auto_Seq,5)),0)) + 1),5) ";
            //StrSql = StrSql + " + Right(convert(varchar(5),convert(Float,isnull(Max(Right(Auto_Seq,5)),0)) + 1),5) ";


            StrSql = StrSql + ",'" + idx_Mbid + "'," + idx_Mbid2 + ",";
            StrSql = StrSql + "'" + mtxtReqDate.Text.Replace("-", "").Trim() + "',";
            StrSql = StrSql + "'" + IndexTime + "',1";

            StrSql = StrSql + " From tbl_Autoship_OrdNumber (nolock) ";
            // StrSql = StrSql + " Where LEFT(OrderNumber,8) = '" + mtxtSellDate.Text.Replace("-", "").Trim() + "'";
            StrSql = StrSql + " Where ReqDate = '" + mtxtReqDate.Text.Replace("-", "").Trim() + "'";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Autoship_OrdNumber", Conn, tran, this.Name.ToString(), this.Text) == false) return;



            //++++++++++++++++++++++++++++++++                
            StrSql = "Select Auto_Seq  ";
            StrSql = StrSql + " From tbl_Autoship_OrdNumber (nolock) ";
            StrSql = StrSql + " Where Mbid = '" + idx_Mbid + "'";
            StrSql = StrSql + " And Mbid2 = " + idx_Mbid2;
            StrSql = StrSql + " And ReqDate = '" + mtxtReqDate.Text.Replace("-", "").Trim() + "'";
            StrSql = StrSql + " And IndexTime = '" + IndexTime + "'";

                

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(StrSql, "tbl_Autoship_OrdNumber", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            AutoSeq = ds.Tables["tbl_Autoship_OrdNumber"].Rows[0]["Auto_Seq"].ToString();
            //++++++++++++++++++++++++++++++++
            



        }

        private void DB_Save_tbl_AutoShip(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran, string Auto_Seq)
        {
            string StrSql = "";

            double TotalPrice = 0, TotalPV = 0, TotalCV = 0, Delivery = 0;
            string Month_Date = string.Empty;
            if (txt_TotalItemPrice.Text == "")
                TotalPrice = 0;
            else
                TotalPrice = double.Parse(txt_TotalItemPrice.Text.Trim());
            if (txt_TotalPV.Text == "")
                TotalPV = 0;
            else
                TotalPV = double.Parse(txt_TotalPV.Text.Trim());
            if (txt_TotalCV.Text == "")
                TotalCV = 0;
            else
                TotalCV = double.Parse(txt_TotalCV.Text.Trim());

            if (RadioFiveDay.Checked)
                Month_Date = "05";
            if (RadioTenDay.Checked)
                Month_Date = "10";
            if (RadioFifteenDay.Checked)
                Month_Date = "15";
            if (RadioTwentyDay.Checked)
                Month_Date = "20";
            if (RadioTwentyfiveDay.Checked)
                Month_Date = "25";
            string ReqType = "";
            ReqType = "BC";

            int PV_CV_Check = 0;
            if( chK_PV_CV_Check.Checked == true)
            {
                PV_CV_Check = 1;
            }

            if (txtDeliverPrice.Text.Replace(",", "").Trim() == "")
                Delivery = 0;
            else
                Delivery = double.Parse(txtDeliverPrice.Text.Replace(",", "").Trim());

            string Proc_Cnt = cboProc_Cnt.Text;
            if(Proc_Cnt =="")
            {
                Proc_Cnt = "0";
            }
            if (txtAutoSEQ.Text.Trim() == "")
            {
                StrSql = "";
                StrSql = " INSERT INTO tbl_Memberinfo_AutoShip ( Auto_Seq, ";
                StrSql = StrSql + " mbid, mbid2, Req_Type, Req_State, Req_Date ";
                StrSql = StrSql + " , Start_Date, Proc_Date, Month_Date, TotalPrice, TotalPV, TotalCV ";
                StrSql = StrSql + " , Etc, Proc_Cnt, DeliveryCharge, RecordID, RecordTime ,PV_CV_Check";
                StrSql = StrSql + " ) VALUES ( ";
                StrSql = StrSql + " '" + Auto_Seq + "' ";
                StrSql = StrSql + " , '" + idx_Mbid + "' ";
                StrSql = StrSql + " , " + idx_Mbid2;
                StrSql = StrSql + " , '" + ReqType + "' "; //ADS신청종류
                StrSql = StrSql + " , '10' ";   //ADS신청상태
                StrSql = StrSql + " , '" + mtxtReqDate.Text.Replace("-", "").Trim() +"' "; //신청날짜
                StrSql = StrSql + " , '" + mtxtReqDate.Text.Replace("-", "").Trim() +"' "; //신청날짜
                StrSql = StrSql + " , '" + mtxtReqDate.Text.Replace("-", "").Trim() +"' "; //신청날짜
                //StrSql = StrSql + " , '" + mtxtStartDate.Text.Replace("-", "").Trim() + "' "; //첫 실행날짜
                //StrSql = StrSql + " , '" + mtxtProcDate.Text.Replace("-", "").Trim() + "' "; //다음 실행날짜
                StrSql = StrSql + " , '" + Month_Date+ "'";
                StrSql = StrSql + " , " + TotalPrice;
                StrSql = StrSql + " , " + TotalPV;
                StrSql = StrSql + " , " + TotalCV;
                StrSql = StrSql + " , '' "; //비고
                StrSql = StrSql + " , " + Proc_Cnt;
                StrSql = StrSql + " , " + Delivery;  //배송비
                StrSql = StrSql + " , '" + cls_User.gid + "' ";
                StrSql = StrSql + " , Convert(Varchar(25),GetDate(),21) ";
                StrSql = StrSql + " , " + PV_CV_Check;
                StrSql = StrSql + " ) ";

                if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip", Conn, tran, this.Name.ToString(), this.Text) == false) return;
               
            }
            else
            {
                //insert 백업, update
                StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Mod_Del ";
                StrSql = StrSql + " Select *, 0, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '', '' ";
                StrSql = StrSql + " From tbl_Memberinfo_AutoShip (nolock) ";
                StrSql = StrSql + " Where Auto_Seq = '" + txtAutoSEQ.Text.Trim() + "'";

                if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;

                StrSql = " UPDATE tbl_Memberinfo_AutoShip SET ";
                //StrSql = StrSql + " Req_Date = '" + mtxtReqDate.Text.Replace("-", "").Trim() + "' ";
                //StrSql = StrSql + "  Start_Date = '" + mtxtStartDate.Text.Replace("-", "").Trim() + "' ";
                //20201007 구현호  김대리 요청 수동으로 자동주문실행일자 변경가능해야함

                if (mtxtProcDate.Text.Replace("-", "").Trim() == "")
                {
              
                }
                else
                {
                    StrSql = StrSql + "  Proc_Date = '" + mtxtProcDate.Text.Replace("-", "").Trim() + "' ";
                }
            
                //
                StrSql = StrSql + " , Month_Date = '" + Month_Date + "'";
                StrSql = StrSql + " , TotalPrice = " + TotalPrice;
                StrSql = StrSql + " , TotalPV = " + TotalPV;
                StrSql = StrSql + " , TotalCV = " + TotalCV;
                StrSql = StrSql + " , Proc_Cnt = " + Proc_Cnt;
                StrSql = StrSql + " , Etc = '' ";   //비고
                StrSql = StrSql + " , DeliveryCharge = " + Delivery;    //배송비
                StrSql = StrSql + " , PV_CV_Check = " + PV_CV_Check;    //가격표시여부
                //StrSql = StrSql + " , Req_Month = " + ReqMonth; //신청개월수
                //StrSql = StrSql + " , AutoExtend = '" + AutoExtend + "' ";  //자동연장여부
                StrSql = StrSql + " Where Auto_Seq = '" + txtAutoSEQ.Text.Trim() + "'";

                if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;
            }
        }

        private void Set_AutoShip_Rece_ReBind()
        {
            int NewReceIndex = 0;

            if (AutoShip_Rece != null)
            {
                foreach (int t_key in AutoShip_Rece.Keys)
                {
                    if (NewReceIndex < t_key)
                        NewReceIndex = t_key;
                }
            }
            NewReceIndex = NewReceIndex + 1;

            if (txtAutoSEQ.Text.Trim() == "")
            {
                cls_AutoShip_Rece t_c_rece = new cls_AutoShip_Rece();

                t_c_rece.RecIndex = NewReceIndex;
                t_c_rece.Rec_Name = txt_RecName.Text.Trim();
                t_c_rece.Rec_Tel = mtxtRecTel.Text.Trim();
                // t_c_rece.Rec_AddCode = mtxtZip1.Text.Trim();
                t_c_rece.Rec_Address1 = txt_RecAddress1.Text.Trim();
                t_c_rece.Rec_Address2 = txt_RecAddress2.Text.Trim();
                // 태국 인 경우
                if (cls_User.gid_CountryCode == "TH")
                {
                    t_c_rece.Rec_AddCode = txtZipCode_TH.Text.Trim();
                    t_c_rece.Rec_city = cbDistrict_TH.Text;
                    t_c_rece.Rec_state = cbProvince_TH.SelectedValue.ToString();
                }
                // 한국 인 경우
                else
                {
                    t_c_rece.Rec_AddCode = mtxtZip1.Text.Trim();
                    t_c_rece.Rec_city = "";
                    t_c_rece.Rec_state = "";
                }
                t_c_rece.RecordID = cls_User.gid;
                t_c_rece.RecordTime = "";

                t_c_rece.Del_TF = "S";

                AutoShip_Rece[NewReceIndex] = t_c_rece;


            }
            else
            {
                if (txtReceIndex.Text == "")
                {
                    cls_AutoShip_Rece t_c_rece = new cls_AutoShip_Rece();

                    t_c_rece.RecIndex = NewReceIndex;
                    t_c_rece.Rec_Name = txt_RecName.Text.Trim();
                    t_c_rece.Rec_Tel = mtxtRecTel.Text.Trim();
                    //t_c_rece.Rec_AddCode = mtxtZip1.Text.Trim();
                    t_c_rece.Rec_Address1 = txt_RecAddress1.Text.Trim();
                    t_c_rece.Rec_Address2 = txt_RecAddress2.Text.Trim();
                    // 태국 인 경우
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        t_c_rece.Rec_AddCode = txtZipCode_TH.Text.Trim();
                        t_c_rece.Rec_city = cbDistrict_TH.Text;
                        t_c_rece.Rec_state = cbProvince_TH.SelectedValue.ToString();
                    }
                    // 한국 인 경우
                    else
                    {
                        t_c_rece.Rec_AddCode = mtxtZip1.Text.Trim();
                        t_c_rece.Rec_city = "";
                        t_c_rece.Rec_state = "";
                    }
                    t_c_rece.RecordID = cls_User.gid;
                    t_c_rece.RecordTime = "";

                    t_c_rece.Del_TF = "S";

                    AutoShip_Rece[NewReceIndex] = t_c_rece;
                }
                else
                {
                    AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_Name = txt_RecName.Text.Trim();
                    AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_Tel = mtxtRecTel.Text.Trim();
                    //AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_AddCode = mtxtZip1.Text.Trim();
                    AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_Address1 = txt_RecAddress1.Text.Trim();
                    AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_Address2 = txt_RecAddress2.Text.Trim();
                    // 태국 인 경우
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_AddCode = txtZipCode_TH.Text.Trim();
                        AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_city = cbDistrict_TH.Text;
                        AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_state = cbProvince_TH.SelectedValue.ToString();
                    }
                    // 한국 인 경우
                    else
                    {
                        AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_AddCode = mtxtZip1.Text.Trim();
                        AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_city = "";
                        AutoShip_Rece[int.Parse(txtReceIndex.Text)].Rec_state = "";
                    }
                }
            }
        }

        private void DB_Save_tbl_AutoShip_Rece(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran, string Auto_Seq)
        {
            Set_AutoShip_Rece_ReBind();

            foreach (int t_key in AutoShip_Rece.Keys)
            {
                if (AutoShip_Rece[t_key].Del_TF == "D") //삭제이다
                {
                    DB_Save_tbl_AutoShip_Rece____D(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
                else if (AutoShip_Rece[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_AutoShip_Rece____U(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
                else if (AutoShip_Rece[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_AutoShip_Rece____S(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
            }
        }

        private void DB_Save_tbl_AutoShip_Cacu(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran, string Auto_Seq)
        {
            foreach (int t_key in AutoShip_Cacu.Keys)
            {
                if (AutoShip_Cacu[t_key].Del_TF == "D") //삭제이다
                {
                    DB_Save_tbl_AutoShip_Cacu____D(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
                else if (AutoShip_Cacu[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_AutoShip_Cacu____U(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
                else if (AutoShip_Cacu[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_AutoShip_Cacu____S(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
            }
        }


        private void DB_Save_tbl_AutoShip_Item(cls_Connect_DB Temp_Connect,
                                             SqlConnection Conn, SqlTransaction tran, string Auto_Seq)
        {
            Set_AutoShip_Item_ReBind(Auto_Seq);

            foreach (int t_key in AutoShip_Item.Keys)
            {
                if (AutoShip_Item[t_key].Del_TF == "D") //삭제이다
                {
                    DB_Save_tbl_AutoShip_Item____D(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
                else if (AutoShip_Item[t_key].Del_TF == "U") //업데이트다 
                {
                    DB_Save_tbl_AutoShip_Item____U(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
                else if (AutoShip_Item[t_key].Del_TF == "S")  //새로운 저장이다
                {
                    DB_Save_tbl_AutoShip_Item____S(Temp_Connect, Conn, tran, Auto_Seq, t_key);
                }
            }
        }

        private void Set_AutoShip_Item_ReBind(string Auto_Seq)
        {
            int NewItemIndex = 0;
            if (AutoShip_Item != null)
            {
                foreach (int t_key in AutoShip_Item.Keys)
                {
                    if (NewItemIndex < t_key)
                        NewItemIndex = t_key;
                }
            }
            NewItemIndex = NewItemIndex + 1;

            //기존에 있던 건의 수량이 0이다...그럼 삭제
            foreach (int t_key in AutoShip_Item.Keys)
            {
                for (int i = 0; i < dGridView_Base.Rows.Count; i++)
                {
                    if (dGridView_Base.Rows[i].Cells["ItemCode"].Value.ToString() == AutoShip_Item[t_key].ItemCode 
                            && (dGridView_Base.Rows[i].Cells[0].Value.ToString() == "0" || dGridView_Base.Rows[i].Cells[0].Value.ToString() == "")
                            )
                    {
                        AutoShip_Item[t_key].Del_TF = "D";
                    }
                }
            }

            //업데이트 되거나 새로 입력이 된 건들...
            string Chk_New = "";
            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                Chk_New = "Y";
                if (dGridView_Base.Rows[i].Cells[0].Value.ToString() != "0" && dGridView_Base.Rows[i].Cells[0].Value.ToString() != "")
                {
                    foreach (int t_key in AutoShip_Item.Keys)
                    {
                        if (dGridView_Base.Rows[i].Cells["ItemCode"].Value.ToString() == AutoShip_Item[t_key].ItemCode)
                        {
                            Chk_New = "N";
                            AutoShip_Item[t_key].ItemCount = int.Parse(dGridView_Base.Rows[i].Cells["ItemCount"].Value.ToString());
                            AutoShip_Item[t_key].ItemPrice = double.Parse(dGridView_Base.Rows[i].Cells["ItemPrice"].Value.ToString());
                            AutoShip_Item[t_key].ItemPV = double.Parse(dGridView_Base.Rows[i].Cells["ItemPV"].Value.ToString());
                            AutoShip_Item[t_key].ItemCV = double.Parse(dGridView_Base.Rows[i].Cells["ItemCV"].Value.ToString());
                            AutoShip_Item[t_key].ItemTotalPrice = AutoShip_Item[t_key].ItemCount * AutoShip_Item[t_key].ItemPrice;
                            AutoShip_Item[t_key].ItemTotalPV = AutoShip_Item[t_key].ItemCount * AutoShip_Item[t_key].ItemPV;
                            AutoShip_Item[t_key].ItemTotalCV = AutoShip_Item[t_key].ItemCount * AutoShip_Item[t_key].ItemCV;

                            AutoShip_Item[t_key].Del_TF = "U";
                        }
                    }

                    if (Chk_New == "Y") //새로 입력된 건
                    {
                        cls_AutoShip_Item t_c_item = new cls_AutoShip_Item();

                        t_c_item.Auto_Seq = Auto_Seq;
                        t_c_item.ItemIndex = NewItemIndex;
                        t_c_item.ItemCount = int.Parse(dGridView_Base.Rows[i].Cells["ItemCount"].Value.ToString());
                        t_c_item.ItemCode = dGridView_Base.Rows[i].Cells["ItemCode"].Value.ToString();
                        t_c_item.ItemName = dGridView_Base.Rows[i].Cells["ItemName"].Value.ToString();
                        t_c_item.ItemPrice = double.Parse(dGridView_Base.Rows[i].Cells["ItemPrice"].Value.ToString());
                        t_c_item.ItemPV = double.Parse(dGridView_Base.Rows[i].Cells["ItemPV"].Value.ToString());
                        t_c_item.ItemCV = double.Parse(dGridView_Base.Rows[i].Cells["ItemCV"].Value.ToString());
                        t_c_item.ItemTotalPrice = t_c_item.ItemCount * t_c_item.ItemPrice;
                        t_c_item.ItemTotalPV = t_c_item.ItemCount * t_c_item.ItemPV;
                        t_c_item.ItemTotalCV = t_c_item.ItemCount * t_c_item.ItemCV;

                        t_c_item.RecordID = cls_User.gid;
                        t_c_item.RecordTime = "";
                        t_c_item.Del_TF = "S";

                        AutoShip_Item[NewItemIndex] = t_c_item;
                        NewItemIndex = NewItemIndex + 1;
                    }
                }
            }
            

        }

        

        private void DB_Save_tbl_AutoShip_Item____D(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string AutoSeq, int ItemIndex)
        {
            string StrSql = "";
            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Item_Mod_Del ";
            StrSql = StrSql + " Select *, 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), ''  from tbl_Memberinfo_AutoShip_Item (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Item[ItemIndex].Auto_Seq + "'";
            StrSql = StrSql + " And ItemIndex = " + AutoShip_Item[ItemIndex].ItemIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Item", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = " DELETE FROM tbl_Memberinfo_AutoShip_Item ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Item[ItemIndex].Auto_Seq + "'";
            StrSql = StrSql + " And ItemIndex = " + AutoShip_Item[ItemIndex].ItemIndex;

            if (Temp_Connect.Delete_Data(StrSql, "tbl_Memberinfo_AutoShip_Item", Conn, tran, this.Name.ToString(), this.Text) == false) return;

        }



        private void DB_Save_tbl_AutoShip_Item____U(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string AutoSeq, int ItemIndex)
        {
            string StrSql = "";
            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Item_Mod_Del ";
            StrSql = StrSql + " Select *, 0, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), ''  from tbl_Memberinfo_AutoShip_Item (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Item[ItemIndex].Auto_Seq + "'";
            StrSql = StrSql + " And ItemIndex = " + AutoShip_Item[ItemIndex].ItemIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Item_Mod_Del", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = " UPDATE tbl_Memberinfo_AutoShip_Item SET ";
            StrSql = StrSql + " ItemCount = " + AutoShip_Item[ItemIndex].ItemCount;
            StrSql = StrSql + " , ItemPrice = " + AutoShip_Item[ItemIndex].ItemPrice;
            StrSql = StrSql + " , ItemPV = " + AutoShip_Item[ItemIndex].ItemPV;
            StrSql = StrSql + " , ItemCV = " + AutoShip_Item[ItemIndex].ItemCV;
            StrSql = StrSql + " , ItemTotalPrice = " + AutoShip_Item[ItemIndex].ItemTotalPrice;
            StrSql = StrSql + " , ItemTotalPV = " + AutoShip_Item[ItemIndex].ItemTotalPV;
            StrSql = StrSql + " , ItemTotalCV = " + AutoShip_Item[ItemIndex].ItemTotalCV;
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Item[ItemIndex].Auto_Seq + "'";
            StrSql = StrSql + " And ItemIndex = " + AutoShip_Item[ItemIndex].ItemIndex;

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }

        private void DB_Save_tbl_AutoShip_Item____S(
                    cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,
                    string AutoSeq, int ItemIndex)
        {
            string StrSql = "";

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Item ";
            StrSql = StrSql + " ( Auto_Seq, ItemIndex, ItemCode, ItemName, ItemCount ";
            StrSql = StrSql + " , ItemPrice, ItemPV, ItemCV, ItemTotalPrice, ItemTotalPV ";
            StrSql = StrSql + " , ItemTotalCV, RecordId, RecordTime ) ";
            StrSql = StrSql + " VALUES ( ";

            if (txtAutoSEQ.Text == "")
                StrSql = StrSql + "'" + AutoSeq + "'";
            else
                StrSql = StrSql + "'" + txtAutoSEQ.Text.Trim() + "'";

            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemIndex;
            StrSql = StrSql + " , '" + AutoShip_Item[ItemIndex].ItemCode + "' ";
            StrSql = StrSql + " , '" + AutoShip_Item[ItemIndex].ItemName + "' ";
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemCount;
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemPrice;
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemPV;
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemCV;
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemTotalPrice;
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemTotalPV;
            StrSql = StrSql + " , " + AutoShip_Item[ItemIndex].ItemTotalCV;
            StrSql = StrSql + " , '" + cls_User.gid + "' ";
            StrSql = StrSql + " , Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Item", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }


        private void DB_Save_tbl_AutoShip_Cacu____D(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran,string AutoSeq, int CacuIndex)
        {
            string StrSql = "";
            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Cacu_Mod_Del ";
            StrSql = StrSql + " Select *, 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '', '' FROM tbl_Memberinfo_AutoShip_Cacu (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Cacu[CacuIndex].Auto_Seq + "'";
            StrSql = StrSql + " And CacuIndex = " + AutoShip_Cacu[CacuIndex].CacuIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Cacu_Mod_Del", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = " DELETE FROM tbl_Memberinfo_AutoShip_Cacu ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Cacu[CacuIndex].Auto_Seq + "'";
            StrSql = StrSql + " And CacuIndex = " + AutoShip_Cacu[CacuIndex].CacuIndex;

            if (Temp_Connect.Delete_Data(StrSql, "tbl_Memberinfo_AutoShip_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }



        private void DB_Save_tbl_AutoShip_Cacu____U(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string AutoSeq, int CacuIndex)
        {
            string StrSql = "";

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Cacu_Mod_Del ";
            StrSql = StrSql + " Select *, 0, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '', '' FROM tbl_Memberinfo_AutoShip_Cacu (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Cacu[CacuIndex].Auto_Seq + "'";
            StrSql = StrSql + " And CacuIndex = " + AutoShip_Cacu[CacuIndex].CacuIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Cacu_Mod_Del", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = " UPDATE tbl_Memberinfo_AutoShip_Cacu SET ";
            StrSql = StrSql + " Cacu_Type = " + AutoShip_Cacu[CacuIndex].Cacu_Type;
            StrSql = StrSql + " , CardCode = '" + AutoShip_Cacu[CacuIndex].CardCode + "' ";
            StrSql = StrSql + " , CardName = '" + AutoShip_Cacu[CacuIndex].CardName + "' ";
            StrSql = StrSql + " , CardNumber = '" + encrypter.Encrypt(AutoShip_Cacu[CacuIndex].CardNumber) + "' ";
            StrSql = StrSql + " , Period1 = '" + AutoShip_Cacu[CacuIndex].Period1 + "' ";
            StrSql = StrSql + " , Period2 = '" + AutoShip_Cacu[CacuIndex].Period2 + "' ";
            StrSql = StrSql + " , Card_OwnerName = '" + AutoShip_Cacu[CacuIndex].Card_OwnerName + "' ";
            StrSql = StrSql + " , Payment_Amt = " + AutoShip_Cacu[CacuIndex].Payment_Amt;
            StrSql = StrSql + " , Installment_Period = '" + AutoShip_Cacu[CacuIndex].Installment_Period + "' ";
            //StrSql = StrSql + " , C_P_Number = '" +  encrypter.Encrypt(AutoShip_Cacu[CacuIndex].C_P_Number) + "' ";
            StrSql = StrSql + " , C_B_Number = '" + AutoShip_Cacu[CacuIndex].C_B_Number + "' ";
            StrSql = StrSql + " , C_CardType = '" + AutoShip_Cacu[CacuIndex].C_CardType + "' ";
            StrSql = StrSql + " , C_CVC = '" + AutoShip_Cacu[CacuIndex].C_CVC + "' ";
            //StrSql = StrSql + " , AuthNumber = '" + AutoShip_Cacu[CacuIndex].AuthNumber + "' ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Cacu[CacuIndex].Auto_Seq + "'";
            StrSql = StrSql + " And CacuIndex = " + AutoShip_Cacu[CacuIndex].CacuIndex;

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }



        private void DB_Save_tbl_AutoShip_Cacu____S(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string AutoSeq, int CacuIndex)
        {
            string StrSql = "";
            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Cacu ";
            StrSql = StrSql + " ( Auto_Seq, CacuIndex, Cacu_Type, CardCode, CardName, CardNumber ";
            StrSql = StrSql + " , Period1, Period2, Card_OwnerName, Payment_Amt, Installment_Period";
            //StrSql = StrSql + " , C_P_Number, C_B_Number, C_CardType, RecordId, RecordTime)";//, AuthNumber ) ";
            StrSql = StrSql + " , C_B_Number, C_CardType, C_CVC, RecordId, RecordTime)";//, AuthNumber ) ";
            StrSql = StrSql + " VALUES ( ";

            if (txtAutoSEQ.Text.Trim() == "")
                StrSql = StrSql + "'" + AutoSeq + "'";
            else
                StrSql = StrSql + "'" + txtAutoSEQ.Text.Trim() + "'";

            StrSql = StrSql + " , " + AutoShip_Cacu[CacuIndex].CacuIndex;
            StrSql = StrSql + " , " + AutoShip_Cacu[CacuIndex].Cacu_Type;
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].CardCode + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].CardName + "' ";
            StrSql = StrSql + " , '" + encrypter.Encrypt(AutoShip_Cacu[CacuIndex].CardNumber) + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].Period1 + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].Period2 + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].Card_OwnerName + "' ";
            StrSql = StrSql + " , " + AutoShip_Cacu[CacuIndex].Payment_Amt;
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].Installment_Period + "' ";
            //StrSql = StrSql + " , '" + encrypter.Encrypt(AutoShip_Cacu[CacuIndex].C_P_Number) + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].C_B_Number + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].C_CardType + "' ";
            StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].C_CVC + "' ";
            StrSql = StrSql + " , '" + cls_User.gid + "' ";
            StrSql = StrSql + " , Convert(Varchar(25),GetDate(),21) ";
            //StrSql = StrSql + " , '" + AutoShip_Cacu[CacuIndex].AuthNumber + "' ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Cacu", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }


        private void DB_Save_tbl_AutoShip_Rece____D(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string AutoSeq, int RecIndex)
        {
            string StrSql = "";

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Rece_Mod_Del ";
            StrSql = StrSql + " Select * , 3, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '' From tbl_Memberinfo_AutoShip_Rece (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Rece[RecIndex].Auto_Seq + "'";
            StrSql = StrSql + " And RecIndex = " + AutoShip_Rece[RecIndex].RecIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Rece_Mod_Del", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = " DELETE FROM tbl_Memberinfo_AutoShip_Rece ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Rece[RecIndex].Auto_Seq + "'";
            StrSql = StrSql + " And RecIndex = " + AutoShip_Rece[RecIndex].RecIndex;

            if (Temp_Connect.Delete_Data(StrSql, "tbl_Memberinfo_AutoShip_Rece", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }
        
        private void DB_Save_tbl_AutoShip_Rece____U(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string AutoSeq, int RecIndex)
        {
            string StrSql = "";

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Rece_Mod_Del ";
            StrSql = StrSql + " Select *, 0, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21), '' From tbl_Memberinfo_AutoShip_Rece (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Rece[RecIndex].Auto_Seq + "'";
            StrSql = StrSql + " And RecIndex = " + AutoShip_Rece[RecIndex].RecIndex;

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Rece_Mod_Del", Conn, tran, this.Name.ToString(), this.Text) == false) return;

            StrSql = " UPDATE tbl_Memberinfo_AutoShip_Rece SET";
            StrSql = StrSql + " Rec_Name = '" + AutoShip_Rece[RecIndex].Rec_Name + "' ";
            StrSql = StrSql + " , Rec_Tel = '" + AutoShip_Rece[RecIndex].Rec_Tel + "' ";
            StrSql = StrSql + " , Rec_Addcode = '" + AutoShip_Rece[RecIndex].Rec_AddCode + "' ";
            StrSql = StrSql + " , Rec_Address1 = '" + AutoShip_Rece[RecIndex].Rec_Address1 + "' ";
            StrSql = StrSql + " , Rec_Address2 = '" + AutoShip_Rece[RecIndex].Rec_Address2 + "' ";
            // 태국 인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                StrSql = StrSql + " , Rec_city = '" + AutoShip_Rece[RecIndex].Rec_city + "' ";
                StrSql = StrSql + " , Rec_state = '" + AutoShip_Rece[RecIndex].Rec_state + "' ";
            }
            StrSql = StrSql + " Where Auto_Seq = '" + AutoShip_Rece[RecIndex].Auto_Seq + "'";
            StrSql = StrSql + " And RecIndex = " + AutoShip_Rece[RecIndex].RecIndex;

            if (Temp_Connect.Update_Data(StrSql, Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }

        private void DB_Save_tbl_AutoShip_Rece____S(cls_Connect_DB Temp_Connect, SqlConnection Conn, SqlTransaction tran, string AutoSeq, int RecIndex)
        {
            string StrSql = "";

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Rece ";
            StrSql = StrSql + " ( Auto_Seq, RecIndex, Rec_Name, Rec_Tel, Rec_Addcode ";
            StrSql = StrSql + " , Rec_Address1, Rec_Address2, Rec_city, Rec_state, RecordId, RecordTime ) ";
            StrSql = StrSql + " VALUES ( ";

            if (txtAutoSEQ.Text.Trim() == "")
                StrSql = StrSql + "'" + AutoSeq + "'";
            else
                StrSql = StrSql + "'" + txtAutoSEQ.Text.Trim() + "'";

            StrSql = StrSql + " , " + AutoShip_Rece[RecIndex].RecIndex;
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_Name + "' ";
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_Tel + "' ";
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_AddCode + "' ";
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_Address1 + "' ";
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_Address2 + "' ";
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_city + "' ";
            StrSql = StrSql + " , '" + AutoShip_Rece[RecIndex].Rec_state + "' ";
            StrSql = StrSql + " , '" + cls_User.gid + "' ";
            StrSql = StrSql + " , Convert(Varchar(25),GetDate(),21) ";
            StrSql = StrSql + " ) ";

            if (Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Rece", Conn, tran, this.Name.ToString(), this.Text) == false) return;
        }

        private void Set_Form_Date(string T_Mbid, bool NewAutoSeq = false, string ExistsAutosSeq = "")
        {
            combo_Card_Per.Text = "";
            Form_Clear_();
            string Mbid = ""; int Mbid2 = 0; idx_Na_Code = "";
            string Chk_New = "U";
            newbool = true;
            Data_Set_Form_TF = 1;
            cls_Search_DB csb = new cls_Search_DB();

            if (csb.Member_Nmumber_Split(T_Mbid, ref Mbid, ref Mbid2) == 1)
            {
                string Tsql = "";

                Tsql = " Select ";
                if (cls_app_static_var.Member_Number_1 > 0)
                    Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                else
                    Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";

                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Month_Date ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Auto_Seq ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.mbid ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.mbid2 ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Req_Type ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Req_State ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Req_Date ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Start_Date ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.End_Date ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Extend_Date ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Proc_Date ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.TotalPrice ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.TotalPV ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.TotalCV ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Etc ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.End_Reason ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.DeliveryCharge ";
                Tsql = Tsql + " , tbl_Memberinfo.M_Name ";
                Tsql = Tsql + " , tbl_Memberinfo.Na_Code ";
                Tsql = Tsql + " , MC1.FlagName AS ReqState_Name ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Proc_Cnt ";
                Tsql = Tsql + " , tbl_Memberinfo_AutoShip.PV_CV_Check   ";
                Tsql = Tsql + " From tbl_Memberinfo_AutoShip (nolock) ";
                Tsql = Tsql + " LEFT OUTER JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo_AutoShip.mbid = tbl_Memberinfo.mbid AND tbl_Memberinfo_AutoShip.mbid2 = tbl_Memberinfo.mbid2 ";
                Tsql = Tsql + " LEFT OUTER JOIN tbl_MasterCode MC1 (nolock) ON tbl_Memberinfo_AutoShip.Req_State = MC1.FlagCode AND MC1.ClassCode = '001' AND MC1.ModuleCode = 'AutoShip' ";
                if (Mbid.Length == 0)
                    Tsql = Tsql + " Where tbl_Memberinfo_AutoShip.Mbid2 = " + Mbid2.ToString();
                else
                {
                    Tsql = Tsql + " Where tbl_Memberinfo_AutoShip.Mbid = '" + Mbid + "' ";
                    Tsql = Tsql + " And   tbl_Memberinfo_AutoShip.Mbid2 = " + Mbid2.ToString();
                }
                Tsql = Tsql + " And tbl_Memberinfo_AutoShip.Req_State <> '99' ";
                Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
                Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";
                if(ExistsAutosSeq != "")
                {
                    Tsql = Tsql + " And tbl_Memberinfo_AutoShip.Auto_Seq = '" + ExistsAutosSeq + "'";
                }

                //++++++++++++++++++++++++++++++++
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0 || NewAutoSeq)
                {
                    ds.Clear();

                    Tsql = "Select  ";
                    if (cls_app_static_var.Member_Number_1 > 0)
                        Tsql = Tsql + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
                    else
                        Tsql = Tsql + " tbl_Memberinfo.mbid2 AS M_Mbid ";

                    //Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Month_Date ";
                    Tsql = Tsql + " ,tbl_Memberinfo.mbid ";
                    Tsql = Tsql + " ,tbl_Memberinfo.mbid2 ";
                    Tsql = Tsql + " , tbl_Memberinfo.M_Name ";
                    Tsql = Tsql + " , tbl_Memberinfo.Na_Code ";

                    Tsql = Tsql + " From tbl_Memberinfo (nolock) ";
                    if (Mbid.Length == 0)
                        Tsql = Tsql + " Where tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                    else
                    {
                        Tsql = Tsql + " Where tbl_Memberinfo.Mbid = '" + Mbid + "' ";
                        Tsql = Tsql + " And   tbl_Memberinfo.Mbid2 = " + Mbid2.ToString();
                    }
                    Tsql = Tsql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
                    Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

                    if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
                    ReCnt = Temp_Connect.DataSet_ReCount;

                    if (ReCnt == 0) return;

                    Chk_New = "I";
                    //20190320 구현호 신청자가 신규자이기 때문에 여기서 신청자 전역변수를 바꾼다. (시작일자 일자버튼 클릭시 반영)
                    newbool = false;
                 
                    /*
                    btnFive.Enabled = true;
                    btnTen.Enabled = true;
                    btnFifteen.Enabled = true;
                    btnTwenty.Enabled = true;

                    btnFive1.Enabled = false;
                    btnTen1.Enabled = false;
                    btnFifteen1.Enabled = false;
                    btnTwenty1.Enabled = false;
                    */
                }

                Set_Form_Date(ds, Chk_New);

                if (Chk_New == "U")
                {
                    string Auto_Seq = ds.Tables[base_db_name].Rows[0]["Auto_Seq"].ToString();
                    txtAutoSEQ.Text = Auto_Seq;

                    string ReqState = ds.Tables[base_db_name].Rows[0]["Req_State"].ToString();

                    
                    /*
                    if (ReqState == "10") //신청상태 
                    {
                        btnFive.Enabled = true;
                        btnTen.Enabled = true;
                        btnFifteen.Enabled = true;
                        btnTwenty.Enabled = true;


                        btnFive1.Enabled = true;
                        btnTen1.Enabled = true;
                        btnFifteen1.Enabled = true;
                        btnTwenty1.Enabled = true;


                        mtxtStartDate.Enabled = true;
                        DTP_StartDate.Enabled = true;


                    }
                    else
                    {
                        
                        DTP_StartDate.Enabled = false;
                        mtxtStartDate.Enabled = false;
                        mtxtProcDate.Enabled = false;
                        //cboProcDay.Enabled = true;
                    }
                    */

                    if (Auto_Seq != "")
                    {
                        Set_Form_Item(Auto_Seq);
                        Set_Form_Cacu(Auto_Seq);
                        Set_Form_Rece(Auto_Seq);
                    }
                }
                else if (Chk_New == "I")
                {
                    /*
                    mtxtReqDate.Enabled = true;
                    DTP_ReqDate.Enabled = true;
                    mtxtStartDate.Enabled = true;
                    DTP_StartDate.Enabled = true;
                    mtxtProcDate.Enabled = false;
                    */
                }

                
                
                mtxtMbid.Focus();
            }
            tab_Auto.SelectedIndex = 0;
            Data_Set_Form_TF = 0;
        }

        private void Set_Form_Rece(string AutoSEQ)
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            string Tsql = "";

            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip_Rece.Auto_Seq ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.RecIndex ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Name ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_tel ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Addcode ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Address1 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Address2 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_city ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_state ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.RecordID ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.RecordTime ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip_Rece (nolock) ";
            Tsql = Tsql + " Where tbl_Memberinfo_AutoShip_Rece.Auto_Seq = '" + AutoSEQ + "'";

            if (Temp_Connect.Open_Data_Set(Tsql, "AutoShip_Rece", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {
                txtReceIndex.Text = ds.Tables["AutoShip_Rece"].Rows[0]["RecIndex"].ToString();
                txt_RecName.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Name"].ToString();
                txt_RecAddress1.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Address1"].ToString();
                txt_RecAddress2.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Address2"].ToString();

                // 태국 인 경우
                if (cls_User.gid_CountryCode == "TH")
                {
                    try
                    {
                        cbProvince_TH.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Address2"].ToString().Split(' ')[2];
                        cbDistrict_TH.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Address2"].ToString().Split(' ')[1];
                        cbSubDistrict_TH.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Address2"].ToString().Split(' ')[0];
                    }
                    catch (Exception)
                    {
                        cbProvince_TH.Text = "";
                        cbDistrict_TH.Text = "";
                        cbSubDistrict_TH.Text = "";
                    }

                    txtZipCode_TH.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Addcode"].ToString();
                }
                // 그 외 국가코드인 경우
                else
                {
                    mtxtZip1.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Addcode"].ToString();
                }

                //cbProvince_TH.Text = 
                mtxtRecTel.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_Tel"].ToString();
                //mtxtZip1.Text = ds.Tables["AutoShip_Rece"].Rows[0]["Rec_AddCode"].ToString();
            }

            Dictionary<int, cls_AutoShip_Rece> T_AutoShip_Rece = new Dictionary<int, cls_AutoShip_Rece>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_AutoShip_Rece t_c_rece = new cls_AutoShip_Rece();

                t_c_rece.Auto_Seq = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Auto_Seq"].ToString();
                t_c_rece.RecIndex = int.Parse(ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["RecIndex"].ToString());
                t_c_rece.Rec_Name = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_Name"].ToString();
                t_c_rece.Rec_Tel = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_Tel"].ToString();
                t_c_rece.Rec_AddCode = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_Addcode"].ToString();
                t_c_rece.Rec_Address1 = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_Address1"].ToString();
                t_c_rece.Rec_Address2 = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_Address2"].ToString();
                // 태국 인 경우
                if (cls_User.gid_CountryCode == "TH")
                {
                    t_c_rece.Rec_city = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_city"].ToString();
                    t_c_rece.Rec_state = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["Rec_state"].ToString();
                }
                // 한국 인 경우
                else
                {
                    t_c_rece.Rec_city = "";
                    t_c_rece.Rec_state = "";
                }

                t_c_rece.RecordID = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["RecordID"].ToString();
                t_c_rece.RecordTime = ds.Tables["AutoShip_Rece"].Rows[fi_cnt]["RecordTime"].ToString();

                t_c_rece.Del_TF = "";
                T_AutoShip_Rece[t_c_rece.RecIndex] = t_c_rece;
            }
            AutoShip_Rece = T_AutoShip_Rece;
        }


        private void Set_Form_Cacu(string AutoSEQ)
        {
           
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            string Tsql = "";
			
            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip_Cacu.Auto_Seq ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CacuIndex ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Cacu_Type ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CardCode ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CardName ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CardNumber CardNumber "; //2018-10-22 지성경 여기에서 CardNumber 랑 C_P_Number 코드상으로 Decrypt해줘야함
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Period1 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Period2 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Card_OwnerName ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.C_P_Number C_P_Number ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.C_B_Number ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.C_CardType ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.C_CVC ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Payment_Amt ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Installment_Period ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.RecordID ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.RecordTime ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip_Cacu (nolock) ";
            Tsql = Tsql + " LEFT OUTER JOIN tbl_Card (nolock) ON tbl_Memberinfo_AutoShip_Cacu.CardCode = tbl_Card.ncode ";
            Tsql = Tsql + " WHERE tbl_Memberinfo_AutoShip_Cacu.Auto_Seq = '" + AutoSEQ + "'";
            Tsql = Tsql + " ORDER BY tbl_Memberinfo_AutoShip_Cacu.CacuIndex ASC ";

            if (Temp_Connect.Open_Data_Set(Tsql, "AutoShip_Cacu", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            Dictionary<int, cls_AutoShip_Cacu> T_AutoShip_Cacu = new Dictionary<int, cls_AutoShip_Cacu>();
            foreach (DataRow row in ds.Tables["AutoShip_Cacu"].Rows)
            {
                row["CardNumber"] = encrypter.Decrypt(row["CardNumber"].ToString());
                row["C_P_Number"] = encrypter.Decrypt(row["C_P_Number"].ToString());

            }
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_AutoShip_Cacu t_c_cacu = new cls_AutoShip_Cacu();
          
                t_c_cacu.Auto_Seq = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Auto_Seq"].ToString();
                t_c_cacu.CacuIndex = int.Parse(ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["CacuIndex"].ToString());
                t_c_cacu.CardCode = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["CardCode"].ToString();
                t_c_cacu.Cacu_Type = int.Parse(ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Cacu_Type"].ToString());
                t_c_cacu.CardName = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["CardName"].ToString();
                t_c_cacu.CardNumber = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["CardNumber"].ToString();
                t_c_cacu.Period1 = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Period1"].ToString();
                t_c_cacu.Period2 = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Period2"].ToString();
                t_c_cacu.Card_OwnerName = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Card_OwnerName"].ToString();
                t_c_cacu.C_P_Number = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["C_P_Number"].ToString();
                t_c_cacu.C_B_Number = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["C_B_Number"].ToString();
                t_c_cacu.C_CardType = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["C_CardType"].ToString();
                if (cls_User.gid_CountryCode == "TH")
                {
                    t_c_cacu.C_CVC = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["C_CVC"].ToString();
                }
                else
                {
                    t_c_cacu.C_CVC = "";
                }
                t_c_cacu.Payment_Amt = double.Parse(ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Payment_Amt"].ToString());
                t_c_cacu.Installment_Period = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["Installment_Period"].ToString();
                t_c_cacu.RecordID = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["RecordID"].ToString();
                t_c_cacu.RecordTime = ds.Tables["AutoShip_Cacu"].Rows[fi_cnt]["RecordTime"].ToString();

                t_c_cacu.Del_TF = "";
                T_AutoShip_Cacu[t_c_cacu.CacuIndex] = t_c_cacu;
            }
            AutoShip_Cacu = T_AutoShip_Cacu;

            txt_Card_Number.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["CardNumber"].ToString();
            combo_Card_Year.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["Period1"].ToString();
            combo_Card_Month.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["Period2"].ToString();
            txt_CardOwner.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["Card_OwnerName"].ToString();
            txt_C_B_Number.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["C_B_Number"].ToString();
            if (cls_User.gid_CountryCode == "TH")
            {
                txt_C_CVC.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["C_CVC"].ToString();
            }
            
            combo_Card_Per.Text =  "";


            //if (ds.Tables["AutoShip_Cacu"].Rows[0]["C_B_Number"].ToString().Length == 1)
            //{
            //    combo_Card_Per.Text = "0" + ds.Tables["AutoShip_Cacu"].Rows[0]["C_B_Number"].ToString();

            //}
            if(ds.Tables["AutoShip_Cacu"].Rows[0]["Installment_Period"].ToString() == "일시불")
            {
                combo_Card_Per.Text = ds.Tables["AutoShip_Cacu"].Rows[0]["Installment_Period"].ToString();
            }
            else
            {
                combo_Card_Per.Text = Convert.ToInt32(AutoShip_Cacu[1].Installment_Period).ToString();
            }
            

            ds.Tables["AutoShip_Cacu"].Rows[0]["C_B_Number"].ToString();


            Cacu_Grid_Set();

        }

        private void Set_Form_Item(string AutoSEQ)
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            string Tsql = "";

            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip_Item.Auto_Seq ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemIndex ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemCode ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemName ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemCount ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemPrice ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemPV ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemCV ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemTotalPrice ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemTotalPV ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemTotalCV ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.RecordID ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.RecordTime ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip_Item (nolock) ";
            Tsql = Tsql + " Where tbl_Memberinfo_AutoShip_Item.Auto_Seq = '" + AutoSEQ + "'";

            if (Temp_Connect.Open_Data_Set(Tsql, "AutoShip_Item", ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            string StrItemCode = "";
            int ItemCount = 0;

            for (int i = 0; i < ds.Tables["AutoShip_Item"].Rows.Count; i++)
            {
                StrItemCode = ds.Tables["AutoShip_Item"].Rows[i]["ItemCode"].ToString();
                ItemCount = int.Parse(ds.Tables["AutoShip_Item"].Rows[i]["ItemCount"].ToString());
                for (int j = 0; j < dGridView_Base.Rows.Count; j++)
                {
                    if (dGridView_Base.Rows[j].Cells["ItemCode"].Value.ToString() == StrItemCode)
                    {
                        dGridView_Base.Rows[j].Cells["ItemCount"].Value = ItemCount;
                    }
                }
            }

            Dictionary<int, cls_AutoShip_Item> T_AutoShip_Item = new Dictionary<int, cls_AutoShip_Item>();
            double SumAmt = 0, SumPV = 0, SumCV = 0;

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                cls_AutoShip_Item t_c_item = new cls_AutoShip_Item();

                t_c_item.Auto_Seq = ds.Tables["AutoShip_Item"].Rows[fi_cnt]["Auto_Seq"].ToString();
                t_c_item.ItemIndex = int.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemIndex"].ToString());
                t_c_item.ItemCode = ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemCode"].ToString();
                t_c_item.ItemName = ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemName"].ToString();
                t_c_item.ItemCount = int.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemCount"].ToString());
                t_c_item.ItemPrice = double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemPrice"].ToString());
                t_c_item.ItemPV = double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemPV"].ToString());
                t_c_item.ItemCV = double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemCV"].ToString());
                t_c_item.ItemTotalPrice = double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemTotalPrice"].ToString());
                t_c_item.ItemTotalPV = double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemTotalPV"].ToString());
                t_c_item.ItemTotalCV = double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemTotalCV"].ToString());
                t_c_item.RecordID = ds.Tables["AutoShip_Item"].Rows[fi_cnt]["RecordID"].ToString();
                t_c_item.RecordTime = ds.Tables["AutoShip_Item"].Rows[fi_cnt]["RecordTime"].ToString();

                t_c_item.Del_TF = "";
                T_AutoShip_Item[t_c_item.ItemIndex] = t_c_item;

                SumAmt = SumAmt + double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemTotalPrice"].ToString());
                SumPV = SumPV + double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemTotalPV"].ToString());
                SumCV = SumCV + double.Parse(ds.Tables["AutoShip_Item"].Rows[fi_cnt]["ItemTotalCV"].ToString());
            }
            AutoShip_Item = T_AutoShip_Item;

            txt_TotalItemPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, SumAmt);
            txt_TotalPV.Text = string.Format(cls_app_static_var.str_Currency_Type, SumPV);
            txt_TotalCV.Text = string.Format(cls_app_static_var.str_Currency_Type, SumCV);
        }


        private void Set_Form_Date(DataSet ds, string Chk)
        {
            cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();
            DataSet ds2 = new DataSet();
            string Tsql2 = "";

            Tsql2 = " Select ";
            Tsql2 = Tsql2 + " tbl_Memberinfo_AutoShip.Auto_seq ";
            Tsql2 = Tsql2 + " From tbl_Memberinfo_AutoShip (nolock) ";
            Tsql2 = Tsql2 + " Where tbl_Memberinfo_AutoShip.mbid2 = '" + ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString() + "'";

            if (Temp_Connect2.Open_Data_Set(Tsql2, "tbl_Memberinfo_AutoShip", ds2, this.Name, this.Text) == false) return;
            int ReCnt2 = Temp_Connect2.DataSet_ReCount;

            if (ReCnt2 == 0) 
            {
                Base_Grid_Set();
            }
            else
            {
                string Auto_seq = ds.Tables[base_db_name].Rows[0]["Auto_seq"].ToString();
                Base_Grid_Set_Update(Auto_seq);
            }

          
            idx_Mbid = ds.Tables[base_db_name].Rows[0]["Mbid"].ToString();
            idx_Mbid2 = int.Parse(ds.Tables[base_db_name].Rows[0]["Mbid2"].ToString());
            idx_Na_Code = ds.Tables[base_db_name].Rows[0]["Na_Code"].ToString();
            mtxtMbid.Text = ds.Tables[base_db_name].Rows[0]["M_Mbid"].ToString();
            txtName.Text = ds.Tables[base_db_name].Rows[0]["M_Name"].ToString();
            cboProc_Cnt.Text = ds.Tables[base_db_name].Rows[0]["Proc_Cnt"].ToString();



            if (Chk == "U")
            {
                mtxtReqDate.Text = ds.Tables[base_db_name].Rows[0]["Req_Date"].ToString();
                string Start_Date = ds.Tables[base_db_name].Rows[0]["Start_Date"].ToString();
                if (Start_Date.Length == 8)
                {
                    Start_Date = string.Format("{0}-{1}-{2}", Start_Date.Substring(0, 4), Start_Date.Substring(4, 2), Start_Date.Substring(6, 2));
                }
                mtxtStartDate.Text = Start_Date;

                mtxtExtendDate.Text = ds.Tables[base_db_name].Rows[0]["Extend_Date"].ToString();

                string Proc_Date = ds.Tables[base_db_name].Rows[0]["Proc_Date"].ToString();
                if (Proc_Date.Length == 8)
                {
                    Proc_Date = string.Format("{0}-{1}-{2}", Proc_Date.Substring(0, 4), Proc_Date.Substring(4, 2), Proc_Date.Substring(6, 2));
                }
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd");
                mtxtProcDate.Text = Proc_Date;


                string Month_Date = ds.Tables[base_db_name].Rows[0]["Month_Date"].ToString();
                if (Month_Date == "05")
                {
                    RadioFiveDay.Checked = true;
                }
                else if (Month_Date == "10")
                {
                    RadioTenDay.Checked = true;
                }
                else if (Month_Date == "15")
                {
                    RadioFifteenDay.Checked = true;
                }
                else if (Month_Date == "20")
                {
                    RadioTwentyDay.Checked = true;
                }
                else if (Month_Date == "25")
                {
                    RadioTwentyfiveDay.Checked = true;
                }
                string PV_CV_Check = string.Format(ds.Tables[base_db_name].Rows[0]["PV_CV_Check"].ToString());
                if (PV_CV_Check == "1")
                {
                    chK_PV_CV_Check.Checked = true;
                }
                else 
                {
                    chK_PV_CV_Check.Checked = false;
                }
                txt_TotalItemPrice.Text = string.Format(ds.Tables[base_db_name].Rows[0]["TotalPrice"].ToString());
                txt_TotalPV.Text = string.Format(ds.Tables[base_db_name].Rows[0]["TotalPV"].ToString());
                txt_TotalCV.Text = string.Format(ds.Tables[base_db_name].Rows[0]["TotalCV"].ToString());
                txt_ReqState.Text = ds.Tables[base_db_name].Rows[0]["ReqState_Name"].ToString();
                txtDeliverPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, ds.Tables[base_db_name].Rows[0]["DeliveryCharge"].ToString());
                cboProc_Cnt.Text = ds.Tables[base_db_name].Rows[0]["Proc_Cnt"].ToString();
            }
            else
            {
                mtxtReqDate.Text = cls_User.gid_date_time;
            }
            

            txtName.ReadOnly = true;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.BackColor = cls_app_static_var.txt_Enable_Color;





        }

        private void mtxtMbid_TextChanged(object sender, EventArgs e)
        {
            if (Data_Set_Form_TF == 1) return;
            MaskedTextBox mtb = (MaskedTextBox)sender;

            if (mtb.Text.Replace("_", "").Replace("-", "").Replace(" ", "") == "")
            {
                if (mtb.Name == "mtxtMbid")
                {
                    Form_Clear_();
                }

            }
        }

        private void dGridView_Base_Sub_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Name == "dGridView_Base_Cacu")
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0].Value != null)
                {
                    cls_form_Meth ct = new cls_form_Meth();
                    int CacuIndex = 0;
                    string CacuType = "";

                    txtCacuIndex.Text = dgv.CurrentRow.Cells[0].Value.ToString();
                    CacuIndex = int.Parse(dgv.CurrentRow.Cells[0].Value.ToString());
                    CacuType = AutoShip_Cacu[CacuIndex].Cacu_Type.ToString();

                    if (CacuType == "3")        //카드
                    {
                        txt_Card_Name.Text = AutoShip_Cacu[CacuIndex].CardName.ToString();
                        txt_Card_Code.Text = AutoShip_Cacu[CacuIndex].CardCode.ToString();
                        txt_CardOwner.Text = AutoShip_Cacu[CacuIndex].Card_OwnerName.ToString();
                        txt_Card_Number.Text = AutoShip_Cacu[CacuIndex].CardNumber.ToString();
                        combo_Card_Per.Text = AutoShip_Cacu[CacuIndex].Installment_Period.ToString();       
                        txt_Card_Price.Text = string.Format(cls_app_static_var.str_Currency_Type, AutoShip_Cacu[CacuIndex].Payment_Amt);
                        //txt_C_P_Number.Text = AutoShip_Cacu[CacuIndex].C_P_Number.ToString();
                        txt_C_B_Number.Text = AutoShip_Cacu[CacuIndex].C_B_Number.ToString();

                        if (AutoShip_Cacu[CacuIndex].C_CardType == "0")
                        {
                            rb_CardType_0.Checked = true;
                            rb_CardType_1.Checked = false;
                        }
                        else if (AutoShip_Cacu[CacuIndex].C_CardType == "1")
                        {
                            rb_CardType_0.Checked = false;
                            rb_CardType_1.Checked = true;
                        }

                        combo_Card_Year.Text = AutoShip_Cacu[CacuIndex].Period1.ToString();
                        combo_Card_Month.Text = AutoShip_Cacu[CacuIndex].Period2.ToString();
                        if (AutoShip_Cacu[CacuIndex].Installment_Period == "일시불" || AutoShip_Cacu[CacuIndex].Installment_Period == "")
                        {
                            combo_Card_Per.Text = "일시불";
                        }
                        else
                        {
                            combo_Card_Per.Text = Convert.ToInt32(AutoShip_Cacu[CacuIndex].Installment_Period).ToString();
                        }

                        tab_Cacu.SelectedIndex = 0;

                        //포인트 탭 초기화
                        txtMilePartner.Text = "";
                        txtMilePartner_Code.Text = "";
                        txt_Mile_Price.Text = "";
                    }
                    else if (CacuType == "4")       //포인트
                    {
                        txtMilePartner.Text = AutoShip_Cacu[CacuIndex].CardName.ToString();
                        txtMilePartner_Code.Text = AutoShip_Cacu[CacuIndex].CardCode.ToString();
                        txt_Mile_Price.Text = string.Format(cls_app_static_var.str_Currency_Type, AutoShip_Cacu[CacuIndex].Payment_Amt);

                        tab_Cacu.SelectedIndex = 1;

                        //카드 탭 초기화
                        txt_Card_Name.Text = "";
                        txt_Card_Code.Text = "";
                        txt_CardOwner.Text = "";
                        txt_Card_Number.Text = "";
                        combo_Card_Per.Text = "";
                        txt_Card_Price.Text = "";
                        //txt_C_P_Number.Text = "";
                        txt_C_B_Number.Text = "";

                        rb_CardType_0.Checked = false;
                        rb_CardType_1.Checked = false;

                        combo_Card_Year.Text = "";
                        combo_Card_Month.Text = "";
                        combo_Card_Per.Text = "";
                    }

                    //butt_cacu_del.Visible = false;
                    //butt_cacu_add.Text = ct._chang_base_caption_search("수정");
                }
            }
        }



        private Boolean Card_Add_Check()
        {
            //if (txt_Card_Name.Text.Trim() == "" || txt_Card_Code.Text == "")
            //{
            //    MessageBox.Show("카드를 선택해주시기 바랍니다.");
            //    txt_Card_Name.Focus();
            //    return false;
            //}
            //if (rb_CardType_0.Checked == false && rb_CardType_1.Checked == false)
            //{
            //    MessageBox.Show("카드구분을 선택하시기 바랍니다.");
            //    rb_CardType_0.Focus();
            //    return false;
            //}
            if (txt_Card_Price.Text.Replace(",", "").Trim() == "" || txt_Card_Price.Text.Replace(",", "").Trim() == "0")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please enter the approved amount.");
                }
                else
                {
                    MessageBox.Show("승인금액을 입력하시기 바랍니다.");
                }
                txt_Card_Price.Focus();
                return false;
            }
            if (txt_Card_Number.Text.Replace("-", "").Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please enter your card number.");
                }
                else
                {

                    MessageBox.Show("카드번호를 입력하시기 바랍니다.");
                }
                txt_Card_Number.Focus();
                return false;
            }
            //if (txt_C_P_Number.Text.Trim() == "")
            //{
            //    MessageBox.Show("카드 비밀번호 앞 2자리를 입력하시기 바랍니다.");
            //    txt_C_P_Number.Focus();
            //    return false;
            //}

            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                if (txt_C_CVC.Text.Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Please enter your card security code number.");
                    }
                    else
                    {
                        MessageBox.Show("카드보안코드를 입력하시기 바랍니다.");
                    }
                    txt_C_CVC.Focus();
                    return false;
                }
            }
            // 한국인 경우
            else
            {
                if (txt_C_B_Number.Text.Trim() == "")
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("Please enter your date of birth or business number.");
                    }
                    else
                    {

                        MessageBox.Show("생년월일 또는 사업자번호를 입력하시기 바랍니다.");
                    }
                    txt_C_B_Number.Focus();
                    return false;
                }
            }
            

            if (combo_Card_Year.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please select the validity period (years).");
                }
                else
                {

                    MessageBox.Show("유효기간(년)을 선택하시기 바랍니다.");
                }
                combo_Card_Year.Focus();
                return false;
            }
            if (combo_Card_Month.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please select the validity period (months).");
                }
                else
                {

                    MessageBox.Show("유효기간(월)을 선택하시기 바랍니다.");
                }
                combo_Card_Month.Focus();
                return false;
            }
            if(txt_CardOwner.Text.Trim() == "")
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please enter the owner's name.");
                }
                else
                {

                    MessageBox.Show("소유자명을 입력하시기 바랍니다.");
                }
                txt_CardOwner.Focus();
                return false;
            }
            //if (combo_Card_Per.Text.Trim() == "")
            //{
            //    MessageBox.Show("할부개월을 선택하시기 바랍니다.");
            //    combo_Card_Per.Focus();
            //    return false;
            //}

            return true;
        }
        /*
        private Boolean Card_Certify_Check()
        {
            string CardNo = txt_Card_Number.Text.Replace("-", "");
            string CardPeriod = combo_Card_Year.Text.Substring(2) + combo_Card_Month.Text;
            string CardType = "";
            if (rb_CardType_0.Checked == true)
                CardType = "0";
            if (rb_CardType_1.Checked == true)
                CardType = "1";
            string Password = txt_C_P_Number.Text;
            string AuthValue = txt_C_B_Number.Text.Replace("-", "");

            cls_Socket csg = new cls_Socket();
            if (csg.Card_Certify(CardNo, CardPeriod, CardType, Password, AuthValue) != "Y")
            {
                MessageBox.Show("신용카드 인증에 실패했습니다.");
                return false;
            }

            return true;
        }
        */
        private void Button_Sub_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_cacu_add")
            {
            
                if (Card_Add_Check() == false) return;

                /*신용카드 인증기능 제외*/
                //if (Card_Certify_Check() == false) return;

                if (txtCacuIndex.Text == "")    //새로운 등록
                {
                    if (AutoShip_Cacu != null)
                    {
                        foreach (int t_key in AutoShip_Cacu.Keys)
                        {
                            if (AutoShip_Cacu[t_key].Del_TF != "D")
                            {
                                if (cls_User.gid_CountryCode == "TH")
                                {
                                    MessageBox.Show("Only one card can be registered.");
                                }
                                else
                                {

                                    MessageBox.Show("한 개의 카드만 등록가능합니다.");
                                }
                                return;
                            }
                        }
                    }
                    
                    Base_Sub_Save_Cacu(3);
                    Base_Sub_Clear("Cacu");
                }
                else
                {
                    Base_Sub_Edit_Cacu(3);
                    Base_Sub_Clear("Cacu");
                }
            }
            else if (bt.Name == "butt_cacu_del")
            {
                if (txtCacuIndex.Text == "") return;

                AutoShip_Cacu[int.Parse(txtCacuIndex.Text)].Del_TF = "D";
                if (AutoShip_Cacu != null)
                    Cacu_Grid_Set();
            }

        }

        private void Base_Sub_Save_Cacu(int C_SF)
        {
            cls_form_Meth ct = new cls_form_Meth();
            int New_C_index = 0;
            if (AutoShip_Cacu != null)
            {
                foreach (int t_key in AutoShip_Cacu.Keys)
                {
                    if (New_C_index < t_key)
                        New_C_index = t_key;
                }
            }
            New_C_index = New_C_index + 1;

            cls_AutoShip_Cacu t_c_cacu = new cls_AutoShip_Cacu();
            
            
            t_c_cacu.CacuIndex = New_C_index;
            
            if (C_SF == 3)
            {
                t_c_cacu.Cacu_Type = 3;
                t_c_cacu.CardCode = txt_Card_Code.Text.Trim();
                t_c_cacu.CardName = txt_Card_Name.Text.Trim();
                t_c_cacu.CardNumber = txt_Card_Number.Text.Trim();
                t_c_cacu.Period1 = combo_Card_Year.Text.Trim();
                t_c_cacu.Period2 = combo_Card_Month.Text.Trim();
                t_c_cacu.Installment_Period = combo_Card_Per.Text.Trim();
                if(t_c_cacu.Installment_Period == null)
                    t_c_cacu.Installment_Period = "일시불";
                if (t_c_cacu.Installment_Period.Length == 1)
                    t_c_cacu.Installment_Period = "0" + t_c_cacu.Installment_Period;

                t_c_cacu.Card_OwnerName = txt_CardOwner.Text.Trim();
                //t_c_cacu.C_P_Number = txt_C_P_Number.Text.Trim();
                t_c_cacu.C_B_Number = txt_C_B_Number.Text.Trim();
                t_c_cacu.Payment_Amt = double.Parse(txt_Card_Price.Text.Trim());
                t_c_cacu.C_CardType = "0";
                t_c_cacu.AuthNumber = txt_AuthNumber.Text.Trim();
                t_c_cacu.C_CVC = txt_C_CVC.Text.Trim();

                if (rb_CardType_1.Checked == true)
                    t_c_cacu.C_CardType = "1";

            }

            t_c_cacu.RecordID = cls_User.gid;
            t_c_cacu.RecordTime = "";

            t_c_cacu.Del_TF = "S";
            AutoShip_Cacu[New_C_index] = t_c_cacu;
        }


        private void Base_Sub_Clear(string s_Tf)
        {
            cls_form_Meth ct = new cls_form_Meth();

            if (s_Tf == "Cacu")
            {
                txtCacuIndex.Text = "";

                dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_cacu.d_Grid_view_Header_Reset(1);

                txt_Card_Code.Text = "";
                txt_Card_Name.Text = "";
                txt_Card_Number.Text = "";
                combo_Card_Per.Text = "";
                txt_Card_Price.Text = "0";
                txt_CardOwner.Text = "";
                //txt_C_P_Number.Text = "";
                txt_C_B_Number.Text = "";
                rb_CardType_0.Checked = true;
                rb_CardType_1.Checked = false;
                txt_C_CVC.Text = "";

                if (combo_Card_Year.SelectedIndex >= 0)
                    combo_Card_Year.SelectedIndex = 0;
                if (combo_Card_Month.SelectedIndex >= 0)
                    combo_Card_Month.SelectedIndex = 0;
                if (combo_Card_Per.SelectedIndex >= 0)
                    combo_Card_Per.SelectedIndex = 0;

                //butt_cacu_add.Visible = false;
                //butt_cacu_del.Visible = false;
                butt_cacu_add.Text = ct._chang_base_caption_search("추가");
                
                if (AutoShip_Cacu != null)
                    Cacu_Grid_Set(); //배송 그리드

            }
        }

        private void Base_Sub_Edit_Cacu(int C_TF)
        {
            cls_form_Meth ct = new cls_form_Meth();
            int C_index = int.Parse(txtCacuIndex.Text);
            
            AutoShip_Cacu[C_index].Cacu_Type = 0;
            AutoShip_Cacu[C_index].CardCode = "";
            AutoShip_Cacu[C_index].CardName = "";
            AutoShip_Cacu[C_index].CardNumber = "";
            AutoShip_Cacu[C_index].Card_OwnerName = "";
            AutoShip_Cacu[C_index].Period1 = "";
            AutoShip_Cacu[C_index].Period2 = "";
            AutoShip_Cacu[C_index].Payment_Amt = 0;
            AutoShip_Cacu[C_index].Installment_Period = "";
            AutoShip_Cacu[C_index].C_P_Number = "";
            AutoShip_Cacu[C_index].C_B_Number = "";
            AutoShip_Cacu[C_index].C_CardType = "";
            AutoShip_Cacu[C_index].AuthNumber = "";
            AutoShip_Cacu[C_index].C_CVC = "";

            if (C_TF == 3)
            {
                AutoShip_Cacu[C_index].Cacu_Type = 3;
                AutoShip_Cacu[C_index].CardCode = txt_Card_Code.Text.Trim();
                AutoShip_Cacu[C_index].CardName = txt_Card_Name.Text.Trim();
                AutoShip_Cacu[C_index].CardNumber = txt_Card_Number.Text.Trim();
                AutoShip_Cacu[C_index].Card_OwnerName = txt_CardOwner.Text.Trim();
                AutoShip_Cacu[C_index].Period1 = combo_Card_Year.Text.Trim();
                AutoShip_Cacu[C_index].Period2 = combo_Card_Month.Text.Trim();
                AutoShip_Cacu[C_index].Installment_Period = combo_Card_Per.Text.Trim();
                if (AutoShip_Cacu[C_index].Installment_Period == "")
                    AutoShip_Cacu[C_index].Installment_Period = "일시불";

                if (AutoShip_Cacu[C_index].Installment_Period.Length == 1)
                    AutoShip_Cacu[C_index].Installment_Period = "0" + AutoShip_Cacu[C_index].Installment_Period;


                AutoShip_Cacu[C_index].Payment_Amt = double.Parse(txt_Card_Price.Text.Trim());
                //AutoShip_Cacu[C_index].C_P_Number = txt_C_P_Number.Text.Trim();
                AutoShip_Cacu[C_index].C_B_Number = txt_C_B_Number.Text.Trim();
                AutoShip_Cacu[C_index].AuthNumber = txt_AuthNumber.Text.Trim();
                AutoShip_Cacu[C_index].C_CVC = txt_C_CVC.Text.Trim();

                if (rb_CardType_0.Checked == true)
                    AutoShip_Cacu[C_index].C_CardType = "0";
                if (rb_CardType_1.Checked == true)
                    AutoShip_Cacu[C_index].C_CardType = "1";
            }

            if (AutoShip_Cacu[C_index].Del_TF == "")
                AutoShip_Cacu[C_index].Del_TF = "U";

            txtCacuIndex.Text = "";
        }

        private void Cacu_Grid_Set()
        {
            dGridView_Base_Cacu_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_cacu.d_Grid_view_Header_Reset();

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            int fi_cnt = 0;
            foreach (int t_key in AutoShip_Cacu.Keys)
            {
                if (AutoShip_Cacu[t_key].Del_TF != "D")
                {
                    Set_gr_dic_Cacu(ref gr_dic_text, t_key, fi_cnt);  //데이타를 배열에 넣는다.
                }
                fi_cnt++;
            }

            cgb_cacu.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_cacu.db_grid_Obj_Data_Put();
        }

        private void Set_gr_dic_Cacu(ref Dictionary<int, object[]> gr_dic_text, int t_key, int fi_cnt)
        {
            object[] row0 = { AutoShip_Cacu[t_key].CacuIndex
                                ,AutoShip_Cacu[t_key].CardCode   
                                ,AutoShip_Cacu[t_key].CardName     
                                ,AutoShip_Cacu[t_key].CardNumber
                                ,AutoShip_Cacu[t_key].Period1    

                                ,AutoShip_Cacu[t_key].Period2
                                ,AutoShip_Cacu[t_key].Installment_Period
                                ,AutoShip_Cacu[t_key].Card_OwnerName
                                ,AutoShip_Cacu[t_key].Payment_Amt
                                ,AutoShip_Cacu[t_key].C_CVC
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }

        private Boolean Check_Date()
        {

            if (int.Parse(mtxtReqDate.Text.Replace("-", "").Trim()) < int.Parse(mtxtStartDate.Text.Replace("-", "").Trim()))
            {
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("Please check the application date and execution start date.");
                }
                else
                {

                    MessageBox.Show("신청일과 실행시작일을 확인하시가 바랍니다.");
                }
                return false;
            }

            return true;
        }

        /*
        private Boolean Check_StartDate_WeekDay(MaskedTextBox mtb)
        {
            string dt = "";
            string Tsql = "";
             
            dt = mtb.Text.Replace("-", "").Trim();
            if (dt == "") return false;

            Tsql = " SELECT T.DT, T.WEEKDAY FROM ( ";
            Tsql = Tsql + " SELECT CONVERT(VARCHAR(8), DATEADD(DD, number, '" + dt + "'),112) AS DT ";
            Tsql = Tsql + " , DATEPART(WEEKDAY, CONVERT(VARCHAR(8),DATEADD(DD, number, '" + dt + "'),112)) AS WEEKDAY ";
            Tsql = Tsql + " FROM master.dbo.spt_values ";
            Tsql = Tsql + " WHERE type = 'P' and number <= DATEDIFF(DD , '" + dt + "', DATEADD(DD, 7, '" + dt + "')) ";
            Tsql = Tsql + " ) T ";
            Tsql = Tsql + " WHERE T.DT > '" + dt + "' ";
            Tsql = Tsql + " AND T.WEEKDAY = 4 ";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();  
            if (Temp_Connect.Open_Data_Set(Tsql, "CheckWeek", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
            {
                MessageBox.Show("등록되어 있지 않은 날짜입니다.\n확인하시기 바랍니다.");
                mtb.Text = "";
                return false;
            }

            mtxtStartDate.Text = ds.Tables["CheckWeek"].Rows[0]["DT"].ToString();
            mtxtProcDate.Text = ds.Tables["CheckWeek"].Rows[0]["DT"].ToString();
            return true;
        }
        */

        private void dGridView_Base_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            DataGridView dgv = (DataGridView)sender;

            // 현재 Cell이 change 된 경우 + 선택한 Cell Column이 0인 경우.
            if (dgv.IsCurrentCellDirty && e.ColumnIndex == 0)
            {
                int inputValue = 0;
                bool inputCheck = int.TryParse(e.FormattedValue.ToString(), out inputValue);
                if (inputCheck == false || inputValue < 0)
                {
                    if (cls_User.gid_CountryCode == "TH")
                    {
                        MessageBox.Show("The quantity entered is not a positive number. Please check again.");
                    }
                    else
                    {
                        MessageBox.Show("입력한 수량이 양수가 아닙니다. 다시 확인해 주시기 바랍니다.");
                    }
                    e.Cancel = true;
                    dgv.EditMode = DataGridViewEditMode.EditOnEnter;
                }
                //else
                //{
                //    dgv[e.ColumnIndex, e.RowIndex].Value = inputValue;
                //}
            }
        }

        private void dGridView_Base_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (mtxtMbid.Text == "") return;
            DataGridView dgv = (DataGridView)sender;
            double Sum_Amt = 0, Sum_PV = 0, Sum_CV = 0; ;

            // Cell filterling시 Error가 나지 않도록 int 값으로 저장
            // CellValidating에서 수정시 Error 발생으로 양수인지 판단은 CellValidating에서, 값 수정은 CellValueChanged에서 진행.
            if (dgv.IsCurrentCellDirty && e.ColumnIndex == 0)
            {
                dgv[e.ColumnIndex, e.RowIndex].Value = int.Parse(dgv[e.ColumnIndex, e.RowIndex].Value.ToString());
            }

            for (int i = 0; i < dGridView_Base.Rows.Count; i++)
            {
                string strItemCount = dGridView_Base.Rows[i].Cells["ItemCount"].Value.ToString();
                int nItemCount = 0;

                if (int.TryParse(strItemCount, out nItemCount))
                {
                    Sum_Amt = Sum_Amt + (int.Parse(dGridView_Base.Rows[i].Cells["ItemPrice"].Value.ToString()) * nItemCount);
                    Sum_PV = Sum_PV + (int.Parse(dGridView_Base.Rows[i].Cells["ItemPV"].Value.ToString()) * nItemCount);
                    Sum_CV = Sum_CV + (int.Parse(dGridView_Base.Rows[i].Cells["ItemCV"].Value.ToString()) * nItemCount);
                }
                if(nItemCount ==  0 )
                {
                    dGridView_Base.Rows[i].Cells["ItemCount"].Value = 0;
                }

            }
            txt_TotalItemPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_Amt);
            txt_TotalPV.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_PV);
            txt_TotalCV.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_CV);

            if (cls_User.gid_CountryCode == "TH")
            {
                if (Sum_Amt < cls_app_static_var.Delivery_Standard_TH && Sum_Amt != 0)
                {
                    txtDeliverPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge_TH);
                    txtTotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge_TH + Sum_Amt);
                }
                else
                {
                    txtTotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_Amt);
                    txtDeliverPrice.Text = "0";
                }
            }
            else
            {
                if (Sum_Amt < cls_app_static_var.Delivery_Standard && Sum_Amt != 0)
                {
                    txtDeliverPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge);
                    txtTotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, cls_app_static_var.Delivery_Charge + Sum_Amt);
                }
                else
                {
                    txtTotalPrice.Text = string.Format(cls_app_static_var.str_Currency_Type, Sum_Amt);
                    txtDeliverPrice.Text = "0";
                }
            }

        }

        private void tab_Auto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tab_Auto.SelectedTab.Name == "tab_Req")
            {
                Base_Req_Grid_Set();
            }

        }

        private void Base_Req_Grid_Set()
        {
            dGridView_Base_Req_Header_Reset();
            cgb_Req.d_Grid_view_Header_Reset();
            string Tsql = "";

            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip.Auto_Seq ";
            if (cls_app_static_var.Member_Number_1 > 0)
                Tsql = Tsql + " , tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2) AS M_Mbid ";
            else
                Tsql = Tsql + " , tbl_Memberinfo.mbid2 AS M_Mbid ";
            
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Req_State ";
            Tsql = Tsql + " , MC1.FlagName NM_ReqState ";
            Tsql = Tsql + " , CASE WHEN ISNULL(tbl_Memberinfo_AutoShip.Req_Date, '') <> '' THEN LEFT(tbl_Memberinfo_AutoShip.Req_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Req_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Req_Date, 7, 2) ELSE '' END AS Req_Date ";
            Tsql = Tsql + " , CASE WHEN ISNULL(tbl_Memberinfo_AutoShip.Start_Date, '') <> '' THEN LEFT(tbl_Memberinfo_AutoShip.Start_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Start_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Start_Date, 7, 2) ELSE '' END AS Start_Date ";
            Tsql = Tsql + " , CASE WHEN ISNULL(tbl_Memberinfo_AutoShip.Proc_Date, '') <> '' THEN LEFT(tbl_Memberinfo_AutoShip.Proc_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Proc_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Proc_Date, 7, 2) ELSE '' END AS Proc_Date ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Proc_Cnt ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.TotalPrice ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.TotalPV ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.TotalCV ";
            Tsql = Tsql + " , CASE WHEN ISNULL(tbl_Memberinfo_AutoShip.Extend_Date, '') <> '' THEN LEFT(tbl_Memberinfo_AutoShip.Extend_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Extend_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.Extend_Date, 7, 2) ELSE '' END AS Extend_Date ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Etc ";
            Tsql = Tsql + ", CASE WHEN ISNULL(tbl_Memberinfo_AutoShip.End_Date, '') <> '' THEN LEFT(tbl_Memberinfo_AutoShip.End_Date, 4) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.End_Date, 5, 2) + '-' + SUBSTRING(tbl_Memberinfo_AutoShip.End_Date, 7, 2) ELSE '' END AS End_Date ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.End_Reason ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Proc_25_Cnt ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip.Proc_25_Won_Sum ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip (nolock) ";
            Tsql = Tsql + " Left Outer Join tbl_Memberinfo (nolock) ON tbl_Memberinfo_AutoShip.mbid = tbl_Memberinfo.mbid And tbl_Memberinfo_AutoShip.mbid2 = tbl_Memberinfo.mbid2 ";
            Tsql = Tsql + " Left Outer Join tbl_MasterCode (nolock) MC1 ON tbl_Memberinfo_AutoShip.Req_State = MC1.FlagCode And MC1.ClassCode = '001' and MC1.ModuleCode = 'Autoship' ";
            Tsql = Tsql + " Where tbl_Memberinfo_AutoShip.mbid = '" + idx_Mbid + "' ";
            Tsql = Tsql + " And tbl_Memberinfo_AutoShip.mbid2 = " + idx_Mbid2;
            Tsql = Tsql + " Order By tbl_Memberinfo_AutoShip.Auto_Seq DESC ";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo_AutoShip", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_dic_Req(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cgb_Req.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_Req.db_grid_Obj_Data_Put();
        }


        private void Set_dic_Req(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;
            object[] row0 = new object[cgb_Req.grid_col_Count];

            while (Col_Cnt < cgb_Req.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }

            gr_dic_text[fi_cnt + 1] = row0;
            gr_dic_text[fi_cnt + 1] = row0;
        }

        private void dGridView_Base_Req_Header_Reset()
        {
            cgb_Req.grid_col_Count = 17;
            cgb_Req.basegrid = dataGridView_Req;
            cgb_Req.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Req.grid_Frozen_End_Count = 4;
            cgb_Req.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"자동주문번호", "회원번호"  , "_ReqState"   , "신청상태"  , "신청일자"   
                                    , "시작일자", "다음자동주문예정일", "최근자동주문결제회차", "신청금액", "신청PV"
                                    , "신청CV", "연장일자", "비고", "종료일자", "종료이유"
                                       , "25만원이상연속회차","25만원이상연속누적금액"
                                    };

            string[] g_Cols = {"Auto_Seq", "mbid2"  , "ReqState"   , "ReqStateName"  , "Req_Date"
                                    , "Start_Date", "Proc_Date", "Proc_cnt", "TotalPrice", "TotalPV"
                                    , "TotalCV", "Extend_Date", "ETC", "End_Date", "End_Reason"
                                          ,"Proc_25_Cnt","Proc_25_Won_Sum"
                                    };
            cgb_Req.grid_col_name = g_Cols;
            cgb_Req.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 100, 100, 0, 100, 100  
                             , 100, 100, 100, 100, 100
                             , 100, 0, 100, 100, 100
                                 ,150,150
                            };
            cgb_Req.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true, true, true, true, true                                     
                                    ,true, true, true, true, true
                                    ,true, true, true, true, true
                                            , true, true
                                   };
            cgb_Req.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight

                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_Req.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[16 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[17 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb_Req.grid_cell_format = gr_dic_cell_format;
        }


        private void dGridView_Base_Req__All_Header_Reset()
        {
            /******************************************************************/
            /*******************************품목*******************************/
            /******************************************************************/
            cgb_Req_Item.grid_col_Count = 8;
            cgb_Req_Item.basegrid = dataGridView_Req_Item;
            cgb_Req_Item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Req_Item.grid_Frozen_End_Count = 4;
            cgb_Req_Item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText_Item = {"_Auto_Seq", "_ItemIndex"  , "품목코드"   , "품목명"  , "신청수량"   
                                    , "금액", "PV", "CV"
                                    };
            string[] g_Cols_Item = {"_Auto_Seq", "_ItemIndex"  , "ItemCode"   , "ItemName"  , "ItemCount"   
                                    , "ItemPrice", "PV", "BV"
                                    };
            cgb_Req_Item.grid_col_header_text = g_HeaderText_Item;
            cgb_Req_Item.grid_col_name = g_Cols_Item;

            int[] g_Width_Item = { 0, 0, 100, 100, 100  
                             , 100, 100, 100
                            };
            cgb_Req_Item.grid_col_w = g_Width_Item;

            Boolean[] g_ReadOnly_Item = { true, true, true, true, true                                     
                                    ,true, true, true
                                   };
            cgb_Req_Item.grid_col_Lock = g_ReadOnly_Item;

            DataGridViewContentAlignment[] g_Alignment_Item =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight //5
                               
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                              };
            cgb_Req_Item.grid_col_alignment = g_Alignment_Item;

            Dictionary<int, string> gr_dic_cell_format_Item = new Dictionary<int, string>();
            gr_dic_cell_format_Item[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format_Item[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format_Item[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format_Item[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb_Req_Item.grid_cell_format = gr_dic_cell_format_Item;

            /******************************************************************/
            /*******************************결제*******************************/
            /******************************************************************/
            cgb_Req_Cacu.grid_col_Count = 10;
            cgb_Req_Cacu.basegrid = dataGridView_Req_Cacu;
            cgb_Req_Cacu.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Req_Cacu.grid_Frozen_End_Count = 4;
            cgb_Req_Cacu.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText_Cacu = {"_Auto_Seq", "_ItemIndex"  , "_cardcode"   , "카드명"  , "카드번호"   
                                    , "유효기간(년)", "유효기간(월)", "카드소유주명", "결제금액", "카드보안코드"
                                    };
            string[] g_Cols_Cacu = {"Auto_Seq", "ItemIndex"  , "cardcode"   , "cardName"  , "cardNumber"   
                                    , "유효기간(년)", "유효기간(월)", "카드소유주명", "결제금액", "카드보안코드"
                                    };
            cgb_Req_Cacu.grid_col_header_text = g_HeaderText_Cacu;
            cgb_Req_Cacu.grid_col_name = g_Cols_Cacu;

            int[] g_Width_Cacu;
            // 태국인 경우
            if (cls_User.gid_CountryCode == "TH")
            {
                g_Width_Cacu = new int[] 
                            {
                                0, 0, 0, 100, 100
                             , 100, 100, 100, 100, 100
                            };
            }
            // 한국 인 경우
            else
            {
                g_Width_Cacu = new int[]
                            {
                                0, 0, 0, 100, 100
                                , 100, 100, 100, 100, 0
                            };
            }

            cgb_Req_Cacu.grid_col_w = g_Width_Cacu;

            Boolean[] g_ReadOnly_Cacu = { true, true, true, true, true                                     
                                    ,true, true, true, true, true
                                   };
            cgb_Req_Cacu.grid_col_Lock = g_ReadOnly_Cacu;

            DataGridViewContentAlignment[] g_Alignment_Cacu =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_Req_Cacu.grid_col_alignment = g_Alignment_Cacu;

            Dictionary<int, string> gr_dic_cell_format_Cacu = new Dictionary<int, string>();
            gr_dic_cell_format_Cacu[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            cgb_Req_Cacu.grid_cell_format = gr_dic_cell_format_Cacu;

            /******************************************************************/
            /*******************************배송*******************************/
            /******************************************************************/
            cgb_Req_Rece.grid_col_Count = 7;
            cgb_Req_Rece.basegrid = dataGridView_Req_Rece;
            cgb_Req_Rece.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_Req_Rece.grid_Frozen_End_Count = 4;
            cgb_Req_Rece.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText_Rece = {"_Auto_Seq", "_ReceIndex"  , "수령인"   , "수령인연락처"  , "우편번호"   
                                    , "주소", "상세주소"
                                    }; 
            string[] g_Cols_Rece = {"Auto_Seq", "ReceIndex"  , "수령인"   , "수령인연락처"  , "우편번호"   
                                    , "주소", "상세주소"
                                    };
            cgb_Req_Rece.grid_col_header_text = g_HeaderText_Rece;
            cgb_Req_Rece.grid_col_name = g_Cols_Rece;

            int[] g_Width_Rece = { 0, 0, 100, 100, 100  
                             , 100, 100
                            };
            cgb_Req_Rece.grid_col_w = g_Width_Rece;

            Boolean[] g_ReadOnly_Rece = { true, true, true, true, true                                     
                                    ,true, true
                                   };
            cgb_Req_Rece.grid_col_Lock = g_ReadOnly_Rece;

            DataGridViewContentAlignment[] g_Alignment_Rece =
                              {DataGridViewContentAlignment.MiddleCenter  
                               ,DataGridViewContentAlignment.MiddleLeft 
                               ,DataGridViewContentAlignment.MiddleLeft  
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft //5
                               
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_Req_Rece.grid_col_alignment = g_Alignment_Rece;
        }

        private void Set_dic_Req_Item(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text_item, int fi_cnt)
        {
            object[] row0 = { ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][0]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][1]  
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][2]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][3]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][4]
                            
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][5]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][6]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Item"].Rows[fi_cnt][7]
                            };

            gr_dic_text_item[fi_cnt + 1] = row0;
        }

        private void Set_dic_Req_Cacu(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text_cacu, int fi_cnt)
        {
            object[] row0 = { ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][0]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][1]  
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][2]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][3]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][4]

                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][5]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][6]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][7]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][8]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows[fi_cnt][9]
                            };

            gr_dic_text_cacu[fi_cnt + 1] = row0;
        }

        private void Set_dic_Req_Rece(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text_rece, int fi_cnt)
        {
            object[] row0 = { ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][0]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][1]  
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][2]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][3]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][4]
                            
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][5]
                            ,ds.Tables["tbl_Memberinfo_AutoShip_Rece"].Rows[fi_cnt][6]
                            };

            gr_dic_text_rece[fi_cnt + 1] = row0;
        }


        private void Base_Grid_Req_All(long AutoSeq)
        {
            dGridView_Base_Req__All_Header_Reset();
            cgb_Req_Item.d_Grid_view_Header_Reset();
            cgb_Req_Cacu.d_Grid_view_Header_Reset();
            cgb_Req_Rece.d_Grid_view_Header_Reset();

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            DataSet ds = new DataSet();
            string Tsql = "";
            int ReCnt = 0;

            //품목
            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip_Item.Auto_Seq ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemIndex ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemCode ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemName ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemCount ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemTotalPrice ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemTotalPV ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Item.ItemTotalCV ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip_Item (nolock) ";
            Tsql = Tsql + " Where tbl_Memberinfo_AutoShip_Item.Auto_Seq = '" + AutoSeq + "'";
            Tsql = Tsql + " Order By tbl_Memberinfo_AutoShip_Item.ItemIndex ASC ";

            ds.Clear();
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo_AutoShip_Item", ds) == false) return;
            ReCnt = Temp_Connect.DataSet_ReCount;

            Dictionary<int, object[]> gr_dic_text_item = new Dictionary<int, object[]>();
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_dic_Req_Item(ref ds, ref gr_dic_text_item, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cgb_Req_Item.grid_name_obj = gr_dic_text_item;  //배열을 클래스로 보낸다.
            cgb_Req_Item.db_grid_Obj_Data_Put();

            //결제
            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip_Cacu.Auto_Seq ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CacuIndex ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CardCode ";
            Tsql = Tsql + " , tbl_Card.cardname ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.CardNumber "; //2018-10-22 DECRYPT 해줘야함 
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Period1 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Period2 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Card_OwnerName ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.Payment_Amt ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Cacu.C_CVC ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip_Cacu (nolock) ";
            Tsql = Tsql + " Left Outer Join tbl_Card (nolock) On tbl_Memberinfo_AutoShip_Cacu.CardCode = tbl_Card.ncode ";
            Tsql = Tsql + " Where tbl_Memberinfo_AutoShip_Cacu.Auto_Seq = '" + AutoSeq + "'";
            Tsql = Tsql + " Order By tbl_Memberinfo_AutoShip_Cacu.CacuIndex ASC ";

            ds.Clear();
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo_AutoShip_Cacu", ds) == false) return;
            ReCnt = Temp_Connect.DataSet_ReCount;

            Dictionary<int, object[]> gr_dic_text_cacu = new Dictionary<int, object[]>();
            foreach(DataRow row in ds.Tables["tbl_Memberinfo_AutoShip_Cacu"].Rows)
            {
                row["CardNumber"] = encrypter.Decrypt(row["CardNumber"].ToString());
            }
            
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_dic_Req_Cacu(ref ds, ref gr_dic_text_cacu, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cgb_Req_Cacu.grid_name_obj = gr_dic_text_cacu;  //배열을 클래스로 보낸다.
            cgb_Req_Cacu.db_grid_Obj_Data_Put();

            //배송
            Tsql = " Select ";
            Tsql = Tsql + " tbl_Memberinfo_AutoShip_Rece.Auto_Seq ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.RecIndex ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Name ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Tel ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Addcode ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Address1 ";
            Tsql = Tsql + " , tbl_Memberinfo_AutoShip_Rece.Rec_Address2 ";
            Tsql = Tsql + " From tbl_Memberinfo_AutoShip_Rece (nolock) ";
            Tsql = Tsql + " Where tbl_Memberinfo_AutoShip_Rece.Auto_Seq = '" + AutoSeq + "'";
            Tsql = Tsql + " Order By tbl_Memberinfo_AutoShip_Rece.RecIndex ASC ";

            ds.Clear();
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo_AutoShip_Rece", ds) == false) return;
            ReCnt = Temp_Connect.DataSet_ReCount;

            Dictionary<int, object[]> gr_dic_text_rece = new Dictionary<int, object[]>();
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_dic_Req_Rece(ref ds, ref gr_dic_text_rece, fi_cnt);  //데이타를 배열에 넣는다.
            }
            cgb_Req_Rece.grid_name_obj = gr_dic_text_rece;  //배열을 클래스로 보낸다.
            cgb_Req_Rece.db_grid_Obj_Data_Put();
        }

        private void dataGridView_Req_DoubleClick(object sender, EventArgs e)
        {
            /*
     string[] g_Cols = {"Auto_Seq", "mbid2"  , "ReqState"   , "ReqStateName"  , "Req_Date"
                                    , "Start_Date", "Proc_Date", "Proc_cnt", "TotalPrice", "TotalPV"
                                    , "TotalCV", "Extend_Date", "ETC", "End_Date", "End_Reason"
                                    };
             */
            if (((sender as DataGridView).CurrentRow != null)
                && ((sender as DataGridView).CurrentRow.Cells["mbid2"].Value != null) 
                && ((sender as DataGridView).CurrentRow.Cells["ReqState"].Value != null))
            {
                if (((sender as DataGridView).CurrentRow.Cells["ReqState"].Value.ToString().Equals("99")))
                    return;

                long AutoSeq = long.Parse((sender as DataGridView).CurrentRow.Cells["Auto_Seq"].Value.ToString());

                //Base_Grid_Req_All(AutoSeq);

                Set_Form_Date(idx_Mbid2.ToString(), false, AutoSeq.ToString());

            }
        }

        private void dataGridView_Req_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[1].Value != null))
            {
                long AutoSeq = long.Parse((sender as DataGridView).CurrentRow.Cells["Auto_Seq"].Value.ToString());
                Base_Grid_Req_All(AutoSeq);
            }
        }

        private void InitCombo()
        {
            /*
            cls_Connect_DB Temp_conn = new cls_Connect_DB();
            DataSet ds = new DataSet();
           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT  TOP 30 CONVERT(VARCHAR, DATEADD(dd, 1, STARTDATE), 23) AS [자동결제날짜]");
            sb.AppendLine("FROM tbl_WeekCount ");
            sb.AppendLine("WHERE STARTDATE BETWEEN  CONVERT(VARCHAR, GETDATE(), 112)  AND CONVERT(VARCHAR, DATEADD(YY, +1, GETDATE()), 112) ");

            if (Temp_conn.Open_Data_Set(sb.ToString(), "Autoship_paymentday", ds) == false) return;

            cboProcDay.Items.Clear();
            foreach(DataRow row in ds.Tables["Autoship_paymentday"].Rows)
            {
                cboStartDay.Items.Add(row["자동결제날짜"].ToString());
                cboProcDay.Items.Add(row["자동결제날짜"].ToString());
            }
            */

            //sb.Clear();
            //ds = new DataSet();
            //sb.AppendLine("SELECT  TOP 30 CONVERT(VARCHAR, DATEADD(dd, 1, STARTDATE), 23) AS [자동결제날짜]");
            //sb.AppendLine("FROM tbl_WeekCount ");
            //
            //if (Temp_conn.Open_Data_Set(sb.ToString(), "Autoship_paymentday", ds) == false) return;
            //cboStartDay.Items.Clear();
            //foreach (DataRow row in ds.Tables["Autoship_paymentday"].Rows)
            //{
            //    cboStartDay.Items.Add(row["자동결제날짜"].ToString());
            //}


        }
       
        private void btnFive_Click(object sender, EventArgs e)
        {
            //int Month_Day = Convert.ToInt32(DateTime.Now.ToString("dd"));
            //int today1 = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            //int month1 = Convert.ToInt32(DateTime.Now.ToString("MM"));
            //int year1 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
           // string month = Left(mtxtReqDate.Text.Substring(0, 9).Replace("-", ""),2);


            int Month_Day =Convert.ToInt32(mtxtReqDate.Text.Substring(8,2).Replace("-",""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));



            string monthstring;
            if (Month_Day >=  5) //오늘이 5일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtStartDate.Text = year1 + "-" + monthstring + "-" + "05";
            if(newbool == false)
            {
                mtxtProcDate.Text = mtxtStartDate.Text;
                RadioFiveDay.Checked = true;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = false;
            }
            //string ReqState = ds.Tables[base_db_name].Rows[0]["Req_State"].ToString();

            //if (ReqState == "10") //신청상태 
            //{
            //    //cboStartDay.Enabled = true;
            //    mtxtProcDate.Enabled = false;
            //}
            //else
            //{
            //    mtxtStartDate.Enabled = false;
            //    mtxtProcDate.Text = mtxtStartDate.Text;
            //    //cboProcDay.Enabled = true;
            //}
        }
        private void btnTen_Click(object sender, EventArgs e)
        {
            //    int Month_Day = Convert.ToInt32(DateTime.Now.ToString("dd"));
            //    int today1 = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            //    int month1 = Convert.ToInt32(DateTime.Now.ToString("MM"));
            //    int year1 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));


            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day >= 10) //오늘이 10일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtStartDate.Text = year1 + "-" + monthstring + "-" + "10";
            if (newbool == false)
            {
                mtxtProcDate.Text = mtxtStartDate.Text;
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = true;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = false;
            }
        }
        private void btnFifteen_Click(object sender, EventArgs e)
        {
        //    int Month_Day = Convert.ToInt32(DateTime.Now.ToString("dd"));
        //    int today1 = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
        //    int month1 = Convert.ToInt32(DateTime.Now.ToString("MM"));
        //    int year1 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));


            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day >= 15) //오늘이 15일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1 ;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtStartDate.Text = year1 + "-" + monthstring + "-" + "15";
            if (newbool == false)
            {
                mtxtProcDate.Text = mtxtStartDate.Text;
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = true;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = false;
            }
        }

        private void btnTwenty_Click(object sender, EventArgs e)
        {

            //int Month_Day = Convert.ToInt32(DateTime.Now.ToString("dd"));
            //int today1 = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            //int month1 = Convert.ToInt32(DateTime.Now.ToString("MM"));
            //int year1 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day >= 20) //오늘이 20일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1 ;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtStartDate.Text = year1 + "-" + monthstring + "-" + "20";
            if (newbool == false)
            {
                mtxtProcDate.Text = mtxtStartDate.Text;
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = true;
                RadioTwentyfiveDay.Checked = false;
            }

        }

        private void btnFive1_Click(object sender, EventArgs e)
        {
            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day > 5) //오늘이 15일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtProcDate.Text = year1 + "-" + monthstring + "-" + "05";
            if (newbool == true)
            {
                RadioFiveDay.Checked = true;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = false;
            }
        }

        private void btnTen1_Click(object sender, EventArgs e)
        {
            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day > 10) //오늘이 15일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtProcDate.Text = year1 + "-" + monthstring + "-" + "10";
            if (newbool == true)
            {
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = true;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = false;
            }
        }

        private void btnFifteen1_Click(object sender, EventArgs e)
        {

            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day > 15) //오늘이 15일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtProcDate.Text = year1 + "-" + monthstring + "-" + "15";
            if (newbool == true)
            {
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = true;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = false;
            }
        }

        private void btnTwenty1_Click(object sender, EventArgs e)
        {
            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day > 20) //오늘이 20일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtProcDate.Text = year1 + "-" + monthstring + "-" + "20";
            if (newbool == true)
            {
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = true;
                RadioTwentyfiveDay.Checked = false;
            }
        }


        private void btnTwentyfive1_Click(object sender, EventArgs e)
        {
            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day > 25) //오늘이 25일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtProcDate.Text = year1 + "-" + monthstring + "-" + "25";
            if (newbool == true)
            {
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = true;
            }
        }

        private void btnTwentyfive_Click(object sender, EventArgs e)
        {

            int Month_Day = Convert.ToInt32(mtxtReqDate.Text.Substring(8, 2).Replace("-", ""));
            int month1 = Convert.ToInt32(mtxtReqDate.Text.Substring(5, 2).Replace("-", ""));
            int year1 = Convert.ToInt32(mtxtReqDate.Text.Substring(0, 4).Replace("-", ""));

            string monthstring;
            if (Month_Day >= 25) //오늘이 25일보다 적으면
            {
                month1 += 1;
            }

            int monthint = month1;
            if (monthint < 10)
            {
                monthstring = "0" + monthint.ToString();
            }
            else
            {
                monthstring = monthint.ToString();
            }
            mtxtStartDate.Text = year1 + "-" + monthstring + "-" + "25";
            if (newbool == false)
            {
                mtxtProcDate.Text = mtxtStartDate.Text;
                RadioFiveDay.Checked = false;
                RadioTenDay.Checked = false;
                RadioFifteenDay.Checked = false;
                RadioTwentyDay.Checked = false;
                RadioTwentyfiveDay.Checked = true;
            }
        }


        private void txtTotalPrice_TextChanged(object sender, EventArgs e)
        {
            //txt_Card_Price.Text = txt_TotalItemPrice.Text;
            //20210701 배송비도 있어서 총금액으로.
            txt_Card_Price.Text = txtTotalPrice.Text;
            
        }

        private void butt_NewAutoship_Click(object sender, EventArgs e)
        {
            Set_Form_Date(idx_Mbid2.ToString(), true);
        }

        private void mtxtStartDate_TextChanged(object sender, EventArgs e)
        {
            string strStartDate = mtxtStartDate.Text.Replace("-", "").Trim();
            string strReqDate = mtxtReqDate.Text.Replace("-", "").Trim();

            if (strStartDate.Length == 8 && strReqDate.Length == 8 &&
                (txtAutoSEQ.Text == string.Empty || txt_ReqState.Text =="신청"))
            {
                int StartDate = Convert.ToInt32(strStartDate);

                int StartYear = Convert.ToInt32(strStartDate.Substring(0, 4));
                int StartMonth = Convert.ToInt32(strStartDate.Substring(4, 2));
                int StartDay = Convert.ToInt32(strStartDate.Substring(6, 2));

                int ReqDate = Convert.ToInt32(strReqDate);

                int ReqYear = Convert.ToInt32(strReqDate.Substring(0, 4));
                int ReqMonth = Convert.ToInt32(strReqDate.Substring(4, 2));
                int ReqDay = Convert.ToInt32(strReqDate.Substring(6, 2));

                if ((StartDay.Equals(5) ||
                    StartDay.Equals(10) ||
                    StartDay.Equals(15) ||
                    StartDay.Equals(20) ||
                    StartDay.Equals(25)) && ReqDate <= StartDate
                    )
                    mtxtProcDate.Text = mtxtStartDate.Text;
                else
                {

                    mtxtStartDate.TextChanged -= mtxtStartDate_TextChanged;
                    mtxtStartDate.Text = string.Empty;
                    mtxtStartDate.Focus();
                    mtxtStartDate.TextChanged += mtxtStartDate_TextChanged;

                }


            }
        }

        //private void mtxtProcDate_TextChanged(object sender, EventArgs e)
        //{

        //    string strProcDate = mtxtProcDate.Text.Replace("-", "").Trim();
        //    string strReqDate = mtxtReqDate.Text.Replace("-", "").Trim();

        //    if (strProcDate.Length == 8 && strReqDate.Length == 8)
        //    {
        //        int ProcDate = Convert.ToInt32(strProcDate);

        //        int ProcYear = Convert.ToInt32(strProcDate.Substring(0, 4));
        //        int ProcMonth = Convert.ToInt32(strProcDate.Substring(4, 2));
        //        int ProcDay = Convert.ToInt32(strProcDate.Substring(6, 2));

        //        int ReqDate = Convert.ToInt32(strReqDate);

        //        int ReqYear = Convert.ToInt32(strReqDate.Substring(0, 4));
        //        int ReqMonth = Convert.ToInt32(strReqDate.Substring(4, 2));
        //        int ReqDay = Convert.ToInt32(strReqDate.Substring(6, 2));

        //        if ((ProcDay.Equals(5) ||
        //            ProcDay.Equals(10) ||
        //            ProcDay.Equals(15) ||
        //            ProcDay.Equals(20) ||
        //            ProcDay.Equals(25)) && ReqDate <= ProcDate)
        //            { }
        //        else
        //        {

        //            mtxtProcDate.TextChanged -= mtxtProcDate_TextChanged;
        //            mtxtProcDate.Text = string.Empty;
        //            mtxtProcDate.Focus();
        //            mtxtProcDate.TextChanged += mtxtProcDate_TextChanged;

        //        }


        //    }
        //}













        private void Save_Base_Data_Auto_First(ref int Save_Error_Check,  string Auto_Seq,  int idx_Mbid2,  string M_Name )
        {
            Save_Error_Check = 0;
            //if (MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Q"), "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string StrSql = "";
            int RowCount = 0;
         
            
            string SellDate = "", OrderNumber = "",   SellDate_Auto = "";
            string Procedure = "";
                        
                
            try
            {
                /*변수초기화*/
                Procedure = "";                
                SellDate = "";
                OrderNumber = "";                
               

                cls_Search_DB csd = new cls_Search_DB();

                SellDate = csd.Select_Today("yyyyMMdd");
                SellDate_Auto = SellDate;                              
                cls_Web Cls_Web = new cls_Web();

                string ItemCount_Chk = "Y";

                string SuccessYN = ""; 
                if (Base_Sell_Table_Make(Auto_Seq, SellDate, idx_Mbid2, M_Name, ItemCount_Chk, ref OrderNumber) == true)
                {
                    if (OrderNumber != "")
                    {

                        string TT_Ret = "";
                        SuccessYN = "";
                        string SuccessYN_Card = "N";
                        int ReCnt_Card = 0;

                        DataSet ds_Card = new DataSet();

                        //재고가 없으면 어차피 실패니까 결제를 태우지 않음
                        if (ItemCount_Chk == "Y")
                        {
                            
                            /*카드로 결제할 건이 있으면 카드 결제 로직을 태운다*/

                            StrSql = " SELECT CacuIndex FROM tbl_Memberinfo_AutoShip_Cacu (NOLOCK) WHERE Auto_Seq = '" + Auto_Seq + "' AND Cacu_Type = 3 ";

                            Temp_Connect.Open_Data_Set(StrSql, "CardSearch", ds_Card);
                            ReCnt_Card = Temp_Connect.DataSet_ReCount;

                            string ErrMessage = "";
                            if (ReCnt_Card > 0)
                            {
                                /*카드결제*/
                                for (int i_Card = 0; i_Card < ReCnt_Card; i_Card++)
                                {
                                    if (SuccessYN_Card == "N")
                                    {

                                        //SuccessYN_Card = Cls_Web.Dir_Card_AutoShip_OK(OrderNumber, int.Parse(ds_Card.Tables["CardSearch"].Rows[i_Card]["CacuIndex"].ToString()), ref ErrMessage);

                                        // 태국인경우 바로 태국전용 Function 호출 
                                        if (cls_User.gid_CountryCode == "TH")
                                        {
                                            SuccessYN_Card = Cls_Web.Dir_Card_AutoShip_OK_TH(OrderNumber, int.Parse(ds_Card.Tables["CardSearch"].Rows[i_Card]["CacuIndex"].ToString()), ref ErrMessage);
                                        }
                                        // 한국인 경우
                                        else
                                        {
                                            SuccessYN_Card = Cls_Web.Dir_Card_AutoShip_OK(OrderNumber, int.Parse(ds_Card.Tables["CardSearch"].Rows[i_Card]["CacuIndex"].ToString()), ref ErrMessage);
                                        }

                                    }
                                }
                            }

                            /*결제 성공 유무 확인*/
                            if (SuccessYN_Card == "Y" && ItemCount_Chk == "Y")
                                SuccessYN = "Y";
                            else
                                SuccessYN = "N";

                            if (SuccessYN_Card == "N" && ReCnt_Card > 0)
                                ErrMessage = ErrMessage + " 카드에러";
                            if (ItemCount_Chk == "N")
                                ErrMessage = ErrMessage + " 재고 부족";

                            if (SuccessYN == "Y")
                            {
                                Chang_Sucess(Auto_Seq, OrderNumber, Temp_Connect);

                                StrSql = "EXEC Usp_Sell_Cacu_ReCul_AutoShip_CS '" + OrderNumber + "','Y', '" + cls_User.gid + "'";
                                Temp_Connect.Update_Data(StrSql, this.Name, this.Text);

                               
                                StrSql = " Update tbl_Memberinfo_AutoShip SET ";
                                StrSql += Environment.NewLine + " Proc_Date =  (select LEFT(CONVERT(varchar, DATEADD(MONTH, 1, getdate()), 112),6) + MONTH_DATE)";
                                StrSql += Environment.NewLine + " , Req_State = '20' ";
                                StrSql += Environment.NewLine + " , Proc_Cnt = Proc_Cnt + 1 ";
                                StrSql += Environment.NewLine + " , End_Reason = ''";
                                StrSql += Environment.NewLine + " , Extend_Date = ''";//CASE WHEN Proc_Cnt <> 0 THEN CASE WHEN Proc_Cnt % 13 = 0 THEN '" + SellDate_Auto + "' ELSE Extend_Date END ELSE Extend_Date END ";
                                StrSql += Environment.NewLine + " Where Auto_Seq = '" + Auto_Seq + "'";

                                Temp_Connect.Update_Data(StrSql, this.Name, this.Text);


                                //오토쉽 랭크업보너스 - 메뉴얼결제 최후로 돌린다. 3달단위 결제완료시 돌아간다.
                                StrSql = "EXEC Usp_Insert_Memberinfo_Autoship_Promotion '" + OrderNumber + "','" + idx_Mbid2 + "'";
                                Temp_Connect.Update_Data(StrSql, this.Name, this.Text);

                                //거래 완전성공한 건을 메나싱크로 보낸다
                                StrSql = "EXEC Usp_JDE_Insert_MK_Ord '" + OrderNumber + "'";
                                Temp_Connect.Update_Data(StrSql, this.Name, this.Text);

                                System.Threading.Thread.Sleep(500);
                                Sell_Ac_insurancenumber(OrderNumber);//직판 관련 승인 번호를 받아온다.            
                            }
                            else
                            {
                                Chang_Fail(Auto_Seq, OrderNumber, Temp_Connect, TT_Ret);

                                StrSql = "EXEC Usp_Sell_Cacu_ReCul_AutoShip_CS '" + OrderNumber + "','N' , '" + cls_User.gid + "'";
                                Temp_Connect.Update_Data(StrSql, this.Name, this.Text);

                                //결제실패 데이터 업데이트
                                StrSql = StrSql + " Update tbl_Memberinfo_AutoShip_Mod_Del Set ";
                                StrSql = StrSql + " Etc = '" + ErrMessage + "' ";
                                StrSql = StrSql + " WHERE Auto_Seq = '" + Auto_Seq + "'";
                                StrSql = StrSql + " And Proc_Date = '" + SellDate_Auto + "' ";
                                StrSql = StrSql + " And OrderNumber = '" + OrderNumber + "' ";
                                StrSql = StrSql + " And Del_TF = 2 ";

                                

                                Temp_Connect.Update_Data(StrSql, this.Name, this.Text);
                            }
                            
                        }
                    }
                }




                if (cls_User.gid_CountryCode == "TH")
                {
                    StrSql = "EXEC [Usp_TH_SMS]   " + idx_Mbid2 + ",'','"+Auto_Seq+"','1'";  
                    Temp_Connect.Update_Data(StrSql, "", "");

                    // Mail 호출 - 오토십
                    new cls_Web().SendMail_TH(idx_Mbid2, string.Empty, Auto_Seq, string.Empty, ESendMailType_TH.autoshipMail);

                    if (SuccessYN == "Y")
                    {
                        StrSql = "EXEC [Usp_TH_SMS]   " + idx_Mbid2 + ",'" + OrderNumber + "','','4'";
                        Temp_Connect.Update_Data(StrSql, "", "");
                        // Mail 호출 - 주문완료
                        new cls_Web().SendMail_TH(idx_Mbid2, OrderNumber, string.Empty, string.Empty, ESendMailType_TH.orderCompleteMail);
                    }
                    else
                    {
                        StrSql = "EXEC [Usp_TH_SMS]   " + idx_Mbid2 + ",'','','2'";   
                        Temp_Connect.Update_Data(StrSql, "", "");
                    }

                }
                else
                {
                    StrSql = "EXEC Usp_Insert_SMS_New  '22',''," + idx_Mbid2 + ",'" + Auto_Seq + "', ''";  //오토쉽 신
                                                                                                           //StrSql = "EXEC Usp_Insert_SMS '22',''," + idx_Mbid2 + ",'" + Auto_Seq + "', ''";  //오토쉽 신
                    Temp_Connect.Update_Data(StrSql, "", "");

                    if (SuccessYN == "Y")
                    {
                        StrSql = "EXEC Usp_Insert_SMS_New  '20',''," + idx_Mbid2 + ",'" + OrderNumber + "', ''";  //매출 결제 완료
                                                                                                                  //StrSql = "EXEC Usp_Insert_SMS '20',''," + idx_Mbid2 + ",'" + OrderNumber + "', ''";  //매출 결제 완료
                        Temp_Connect.Update_Data(StrSql, "", "");
                    }
                    else
                    {
                        StrSql = "EXEC Usp_Insert_SMS_New  '24',''," + idx_Mbid2 + ",'" + Auto_Seq + "', ''";  //오토쉽 결제 실패
                                                                                                               //StrSql = "EXEC Usp_Insert_SMS '24',''," + idx_Mbid2 + ",'" + Auto_Seq + "', ''";  //오토쉽 결제 실패
                        Temp_Connect.Update_Data(StrSql, "", "");
                    }

                }

            }
            catch (Exception ee)
            {
                //StrSql = " Update tbl_AutoShip_Log SET ";
                //StrSql = StrSql + " CloseTF = 'E' ";
                //StrSql = StrSql + " , EndDate = Convert(Varchar(25),GetDate(),21) ";
                //StrSql = StrSql + " Where CloseTF = 'F' ";

                //Temp_Connect.Update_Data(StrSql, "", "");

                ////MessageBox.Show("결제 진행 중 에러가 발생했습니다.");
            }
            finally
            {

                
                

                Temp_Connect.Close_DB();
            }



                
                
            

            //try
            //{

            //    StrSql = " Update tbl_AutoShip_Log SET ";
            //    StrSql = StrSql + " CloseTF = 'T' ";
            //    StrSql = StrSql + " , EndDate = Convert(Varchar(25),GetDate(),21) ";
            //    StrSql = StrSql + " Where CloseTF = 'F' ";
            //    Temp_Connect.Update_Data(StrSql, this.Name, this.Text);

            //    Save_Error_Check = 1;

            //}
            //catch (Exception ee)
            //{
            //    StrSql = " Update tbl_AutoShip_Log SET ";
            //    StrSql = StrSql + " CloseTF = 'E' ";
            //    StrSql = StrSql + " , EndDate = Convert(Varchar(25),GetDate(),21) ";
            //    StrSql = StrSql + " Where CloseTF = 'F' ";

            //    Temp_Connect.Update_Data(StrSql, "", "");

            //    MessageBox.Show("결제 진행 중 에러가 발생했습니다.");
            //}
            //finally
            //{
            //    Temp_Connect.Close_DB();
            //}


        }



        private void Sell_Ac_insurancenumber(string T_ord_N)
        {
            string Req = "";

            cls_Socket csg = new cls_Socket();
            Req = csg.Dir_Connect_Send(T_ord_N);

            if (Req != "Y")
            {

                if (Req == "-10000")
                    return;

                string MessageInsurance = string.Format("공제조합 발급이 실패되었습니다. 에러코드:{0}" + Environment.NewLine +
                    "https://www.macco.or.kr/it/selectListSocketErrorCode.do 접속해서 에러코드 확인후에" + Environment.NewLine +
                    "메나테크㈜ 전산담당자에게 문의하시기 바랍니다.", Req);

                MessageBox.Show(MessageInsurance);
            }
            else
            {
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();
                string Tsql = "";

                Tsql = "Select  InsuranceNumber  From tbl_SalesDetail  (nolock) ";
                Tsql = Tsql + " Where OrderNumber = '" + T_ord_N + "'";
                //++++++++++++++++++++++++++++++++               

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, "tbl_SalesDetail", ds) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;


                string inus_ = "";
                if (ReCnt > 0)
                    inus_  = ds.Tables["tbl_SalesDetail"].Rows[0]["InsuranceNumber"].ToString();
                if (cls_User.gid_CountryCode == "TH")
                {
                    MessageBox.Show("The deduction number was issued normally. [Deduction number: " + inus_ + "]");
                }
                else
                {

                    MessageBox.Show("공제번호가 정상적으로 발급 되었습니다. [공제번호 : " + inus_ + "]");
                }
                //Button T_bt = butt_Print; EventArgs ee1 = null;

            }
        }

        private Boolean Save_Check()
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";

            Tsql = " Select T_Index From tbl_AutoShip_Log ";
            Tsql = Tsql + " Where CloseTF = 'F' ";
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {              
            }

            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                Tsql = "INSERT INTO tbl_AutoShip_Log (CloseTF, StartDate, RecordID) VALUES (";
                Tsql = Tsql + " 'F', Convert(Varchar(25),GetDate(),21), '" + cls_User.gid + "' ";
                Tsql = Tsql + " )";

                Temp_Connect.Insert_Data(Tsql, "tbl_AutoShip_Log", this.Name, this.Text);

                tran.Commit();
                return true;
            }
            catch (Exception ee)
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                tran.Dispose();
                Temp_Connect.Close_DB();
            }
        }

        private Boolean Base_Sell_Table_Make(string Auto_Seq, string SellDate, int idx_Mbid2, string M_Name, string ItemCount_Chk, ref string OrderNumber)
        {

            string ToEndDate = cls_User.gid_date_time;
            string SellSort = "";
            string StrSql = "", T_index = "", T_CenterCode = "";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();



            SellSort = "BC";


            try
            {
                StrSql = "EXEC Usp_Insert_Tbl_Sales_OrderNumber_CS '', " + idx_Mbid2 + ", '" + SellDate + "', '" + T_CenterCode + "'";

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                Temp_Connect.Open_Data_Set(StrSql, "tbl_Sales_OrdNumber", ds);
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt > 0)
                {
                    OrderNumber = ds.Tables["tbl_Sales_OrdNumber"].Rows[0]["OrderNumber"].ToString();
                   
                    StrSql = " EXEC Usp_Insert_AutoShip_SalesTable '" + OrderNumber + "', '" + Auto_Seq + "', '" + SellDate + "' ";                    
                    Temp_Connect.Insert_Data(StrSql, "tbl_SalesDetail", this.Name.ToString(), this.Text);


                    StrSql = "Update tbl_SalesDetail Set Recordid = '" + cls_User.gid + "' Where Ordernumber = '" + OrderNumber + "' ";
                    Temp_Connect.Update_Data(StrSql);

                    StrSql = "Update tbl_Sales_Cacu Set Recordid = '" + cls_User.gid + "'  Where Ordernumber = '" + OrderNumber + "' ";
                    Temp_Connect.Update_Data(StrSql);

                    StrSql = "Update tbl_Sales_Rece Set Recordid = '" + cls_User.gid + "'  Where Ordernumber = '" + OrderNumber + "' ";
                    Temp_Connect.Update_Data(StrSql);

                }


                return true;
            }
            catch (Exception ee)
            {                
                return false;
            }
        }







        private void Chang_Sucess(string Auto_Seq, string OrderNumber, cls_Connect_DB Temp_Connect)
        {
            string StrSql = "";
            //정상결제되면 Del_TF = 1, 결제실패면 = 2, 수정이면 = 0, 삭제면 = 3

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Mod_Del ";
            StrSql = StrSql + " Select *, 1, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' , '' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Mod_Del", this.Name, this.Text);

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Item_Mod_Del ";
            StrSql = StrSql + " Select *, 1, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip_Item (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            StrSql = StrSql + " Order by ItemIndex ASC ";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Item_Mod_Del", this.Name, this.Text);

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Cacu_Mod_Del ";
            StrSql = StrSql + " Select *, 1, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' , '' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip_Cacu (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            StrSql = StrSql + " Order By CacuIndex ASC ";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Cacu_Mod_Del", this.Name, this.Text);

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Rece_Mod_Del ";
            StrSql = StrSql + " Select *, 1, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip_Rece (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            StrSql = StrSql + " Order By RecIndex ASC ";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Rece_Mod_Del", this.Name, this.Text);
        }

        private void Chang_Fail(string Auto_Seq, string OrderNumber, cls_Connect_DB Temp_Connect, string TT_Ret)
        {

            string StrSql = "";
            //정상결제되면 Del_TF = 1, 결제실패면 = 2, 수정이면 = 0, 삭제면 = 3

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Mod_Del ";
            StrSql = StrSql + " Select *, 2, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "', '" + TT_Ret + "' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Mod_Del", this.Name, this.Text);

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Item_Mod_Del ";
            StrSql = StrSql + " Select *, 2, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip_Item (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            StrSql = StrSql + " Order by ItemIndex ASC ";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Item_Mod_Del", this.Name, this.Text);

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Cacu_Mod_Del ";
            StrSql = StrSql + " Select *, 2, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' , '" + TT_Ret + "' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip_Cacu (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            StrSql = StrSql + " Order By CacuIndex ASC ";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Cacu_Mod_Del", this.Name, this.Text);

            StrSql = " INSERT INTO tbl_Memberinfo_AutoShip_Rece_Mod_Del ";
            StrSql = StrSql + " Select *, 2, '" + cls_User.gid + "', Convert(Varchar(25),GetDate(),120), '" + OrderNumber + "' ";
            StrSql = StrSql + " From tbl_Memberinfo_AutoShip_Rece (nolock) ";
            StrSql = StrSql + " Where Auto_Seq = '" + Auto_Seq + "'";
            StrSql = StrSql + " Order By RecIndex ASC ";
            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo_AutoShip_Rece_Mod_Del", this.Name, this.Text);
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

            txt_RecAddress2.Text = cbSubDistrict_TH.Text + " " + cbDistrict_TH.Text + " " + cbProvince_TH.Text;
        }
    }

}
