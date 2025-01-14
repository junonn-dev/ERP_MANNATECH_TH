﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace MLM_Program
{
    public partial class frmBase_AddCode : Form
    {

        public delegate void SendAddressDele(string AddCode1, string AddCode2, string Address1, string Address2, string Address3);
        public event SendAddressDele Send_Address_Info;

        cls_Grid_Base cgb = new cls_Grid_Base();
        private const string base_db_name = "tbl_zipcode";
        private string t_AddCode1;
        private string t_AddCode2;
        private int FormLoad_TF = 0;
        private int Data_Set_Form_TF = 0;
        private int Data_Set_Form_TF2 = 0;

        public frmBase_AddCode()
        {
            InitializeComponent();
        }


        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            Data_Set_Form_TF = 0;
            Data_Set_Form_TF2 = 0;
            t_AddCode1 = ""; t_AddCode2 = "";

            FormLoad_TF = 1;
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            FormLoad_TF = 0;

            string t_url = "https://www.koreavisi.com/common/cs/address/search.do";//cls_app_static_var.AddressURL;

            webBrowser1.Navigate(t_url);
            webBrowser1.ScriptErrorsSuppressed = true;
            timer1.Enabled = true;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.DocumentText.Contains("successYN") == true)
            {
                timer1.Enabled = false;
                string Getstring = "";
                Getstring = webBrowser1.DocumentText;

                JObject ReturnData = JObject.Parse(Getstring);

                string SuccessYN = "";
                t_AddCode1 = "";
                t_AddCode2 = "";
                string Add_1 = "";

                SuccessYN = ReturnData["successYN"].ToString();

                if (SuccessYN == "Y")
                {
                    t_AddCode1 = ReturnData["zipcode"].ToString();
                    t_AddCode2 = "";
                    Add_1 = ReturnData["addr"].ToString();
                }

                Send_Address_Info(t_AddCode1, t_AddCode2, Add_1, "", "");

                this.Close();
            }
        }







    }
}
