﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MLM_Program
{
    public partial class frmBase_AV_Accnt_Danal : Form
    {
        public delegate void SendAccntDele(string SuccessYN, string Message);
        public event SendAccntDele Send_Accnt_Info;

        public delegate void Call_AV_Accnt_Info_Dele(ref string OrderNumber, ref int C_Index);
        public event Call_AV_Accnt_Info_Dele Call_AV_Accnt_Info;

        cls_Grid_Base cgb = new cls_Grid_Base();
        private int FormLoad_TF = 0;

        private string R_SuccessYN = "";
        private string R_Message = "";
        private string R_Tid = "";
        private double R_Amount = 0;
        private string R_virtualAccount = "";
        private string R_BankCode = "";
        private string R_BankName = "";
        private string R_UserName = "";
        private string R_IsCashReceipt = "";


        private int Seq_No = 0;
        private string str_sendvalue = "";

        private string OrderNumber = "";
        private int C_Index = 0;

        private int idx_mbid2 = 0;

        public string Temp_OrderNumber = "";
        public int Temp_C_Index = 0;

        public frmBase_AV_Accnt_Danal()
        {
            InitializeComponent();
        }

        public void GetSuccessData(ref string SuccessYn, ref string ResultMessage)
        {
            SuccessYn = this.R_SuccessYN;
            ResultMessage = this.R_Message;
        }

        private void VR_Account_OK_Data(string OrderNumber, int C_Index, ref string str_sendvalue, ref int Seq_No)
        {
            string Tsql = "";

            Tsql = " Select ";
            Tsql = Tsql + " C_Price2 ";
            Tsql = Tsql + " , C_Code BankCode ";
            Tsql = Tsql + " , CASE WHEN C_Cash_Send_TF IN (1,2) THEN '1' ELSE '0' END ReceiptYN ";
            Tsql = Tsql + " , CASE WHEN C_Cash_Bus_TF = 0 THEN '01' WHEN C_Cash_Bus_TF = 1 THEN '02' ELSE '' END Receipt_Gubun1 ";
            Tsql = Tsql + " , CASE WHEN C_Cash_Bus_TF = 0 THEN '3' WHEN C_Cash_Bus_TF = 1 THEN '4' ELSE '' END Receipt_Gubun2 ";
            Tsql = Tsql + " , C_Cash_Send_Nu Receipt_Num"; //현금영수증신청번호
            Tsql = Tsql + " , tbl_Sales_Cacu.OrderNumber ";
            Tsql = Tsql + " , tbl_SalesDetail.mbid2 ";
            Tsql = Tsql + " , tbl_SalesDetail.M_Name ";
            Tsql = Tsql + " , ISNULL(tbl_Memberinfo.Email, '') Email";
            Tsql = Tsql + " , ISNULL(tbl_Memberinfo.hometel, '') HomeTel";
            Tsql = Tsql + " , ISNULL(tbl_Memberinfo.hptel, '') HpTel ";
            Tsql = Tsql + " , ISNULL(tbl_Memberinfo.Address1, '') + ' ' + ISNULL(tbl_Memberinfo.Address2, '') AS Address ";
            Tsql = Tsql + " , CONVERT(VARCHAR(8), GETDATE(), 112) DT ";
            Tsql = Tsql + " , (SELECT TOP 1 ItemName FROM TBL_SALESITEMDETAIL (NOLOCK) WHERE ORDERNUMBER = tbl_SalesDetail.ORDERNUMBER) +  ";
            Tsql = Tsql + "   (SELECT CASE WHEN COUNT(ORDERNUMBER) < 2 THEN '' ELSE '외' + CONVERT(VARCHAR, COUNT(ORDERNUMBER) - 1) + '개' END FROM TBL_SALESITEMDETAIL (NOLOCK) WHERE ORDERNUMBER = tbl_SalesDetail.ORDERNUMBER AND SellState <> 'C_1') AS ItemName   ";
            Tsql = Tsql + " From tbl_Sales_Cacu (NOLOCK) ";
            Tsql = Tsql + " INNER JOIN tbl_SalesDetail (NOLOCK) ON tbl_Sales_Cacu.OrderNumber = tbl_SalesDetail.OrderNumber ";
            Tsql = Tsql + " INNER JOIN tbl_Memberinfo (NOLOCK) ON tbl_SalesDetail.mbid = tbl_Memberinfo.mbid AND tbl_SalesDetail.mbid2 = tbl_Memberinfo.mbid2 ";
            Tsql = Tsql + " WHERE tbl_Sales_Cacu.OrderNumber = '" + OrderNumber + "' ";
            Tsql = Tsql + " And tbl_Sales_Cacu.C_TF = 5 ";
            Tsql = Tsql + " And tbl_Sales_Cacu.C_Number3 = '' ";
            Tsql = Tsql + " And tbl_Sales_Cacu.C_Number1 = '' ";
            Tsql = Tsql + " And tbl_Sales_Cacu.C_Price2 > 0 ";
            Tsql = Tsql + " And tbl_Sales_Cacu.C_Index = " + C_Index;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "AV_ACCOUNT", ds) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++
            string AccountHolder = "메나테크";      //예금주명(기본값)
            double Amount = 0;                                  //결제금액(필수, 최소금액1원)
            string ItemName = "";                               //상품명(필수)
            string ExpireDate = "";                             //입금마감기한(기본 요청일 + 2일)
            string OrderID = "";                                //가맹점 주문번호
            string BankCodeBase = "";                           //주문자가 입금할 은행 코드
            string ByPassValue = "";                            //추가필드값
            string IsCashReceiptUI = "";                        //현금영수증 UI표시 설정 유무(기본값:Y)
            string UserID = "";                                 //주문자ID(필수)
            string UserName = "";                               //주문자이름
            string UserEmail = "";                              //주문자 이메일
            string UserAgent = "PC";                            //사용자환경
            string TxType = "AUTH";                             //트랜잭션 타입(고정값)
            string ServiceType = "DANALVACCOUNT";               //서비스 타입(고정값)


            Amount = double.Parse(ds.Tables["AV_ACCOUNT"].Rows[0]["C_Price2"].ToString());
            ItemName = ds.Tables["AV_ACCOUNT"].Rows[0]["ItemName"].ToString();
            OrderID = OrderNumber;
            BankCodeBase = ds.Tables["AV_ACCOUNT"].Rows[0]["BankCode"].ToString();
            IsCashReceiptUI = "Y";
            UserID = ds.Tables["AV_ACCOUNT"].Rows[0]["mbid2"].ToString();
            UserName = ds.Tables["AV_ACCOUNT"].Rows[0]["M_Name"].ToString();
            UserEmail = ds.Tables["AV_ACCOUNT"].Rows[0]["Email"].ToString();


            str_sendvalue = str_sendvalue + "accountHolder=" + AccountHolder;
            str_sendvalue = str_sendvalue + "&amount=" + Amount;
            str_sendvalue = str_sendvalue + "&itemName=" + ItemName;
            str_sendvalue = str_sendvalue + "&expireDate=" + ExpireDate;
            str_sendvalue = str_sendvalue + "&orderId=" + OrderID;
            str_sendvalue = str_sendvalue + "&bankCodeBase=" + BankCodeBase;
            str_sendvalue = str_sendvalue + "&byPassValue=" + ByPassValue;
            str_sendvalue = str_sendvalue + "&isCashReceiptUi=" + IsCashReceiptUI;
            str_sendvalue = str_sendvalue + "&userId=" + UserID;
            str_sendvalue = str_sendvalue + "&userName=" + UserName;
            str_sendvalue = str_sendvalue + "&userEmail=" + UserEmail;
            str_sendvalue = str_sendvalue + "&userAgent=" + UserAgent;
            str_sendvalue = str_sendvalue + "&txType=" + TxType;
            str_sendvalue = str_sendvalue + "&serviceType=" + ServiceType;

            string StrSql = "";
            StrSql = "EXEC Usp_Insert_tbl_Sales_Cacu_Card " + C_Index + " ,'" + OrderNumber + "','','가상계좌','A','','" + cls_User.gid + "'";
            Temp_Connect.Open_Data_Set(StrSql, "Cacu_Card", ds);

            Seq_No = int.Parse(ds.Tables["Cacu_Card"].Rows[0][0].ToString());

            idx_mbid2 = int.Parse(UserID);
        }

        private void frmBase_From_Load(object sender, EventArgs e)
        {
           
            OrderNumber = Temp_OrderNumber;
            C_Index = Temp_C_Index;

            Call_AV_Accnt_Info(ref OrderNumber, ref C_Index);

            VR_Account_OK_Data(OrderNumber, C_Index, ref str_sendvalue, ref Seq_No);

            FormLoad_TF = 1;
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);
            FormLoad_TF = 0;

            string strUrl = cls_app_static_var.ApproveAccountURL;
            byte[] postData = Encoding.UTF8.GetBytes(str_sendvalue);
            webBrowser1.Navigate(strUrl, null, postData, "Content-Type: application/x-www-form-urlencoded");


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
                }

            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.DocumentText.Contains("successYN") == true)
            {
                timer1.Enabled = false;
                string Getstring = "";

                Getstring = webBrowser1.DocumentText.ToString();
                JObject ReturnData = JObject.Parse(Getstring.Replace("\r", "").Replace("\n", ""));

                R_SuccessYN = ReturnData["successYN"].ToString();

                if (R_SuccessYN == "Y")
                {
                    R_Tid = ReturnData["tid"].ToString();
                    R_Amount = double.Parse(ReturnData["amount"].ToString());
                    R_virtualAccount = ReturnData["virtualAccount"].ToString();
                    R_BankCode = ReturnData["bankCode"].ToString();
                    R_BankName = ReturnData["bankName"].ToString();
                    R_UserName = ReturnData["userName"].ToString();
                    R_IsCashReceipt = ReturnData["isCashReceipt"].ToString();

                    int C_CashReceipt = -1;
                    if (R_IsCashReceipt == "Y")
                        C_CashReceipt = 1;

                    Cacu_Update(R_virtualAccount, R_Tid, R_UserName, C_CashReceipt, R_SuccessYN, R_BankCode, R_Amount, "", idx_mbid2);

                }
                else
                {
                    R_Message = ReturnData["message"].ToString();

                    Cacu_Update(R_virtualAccount, R_Tid, R_UserName, 0, R_SuccessYN, R_BankCode, R_Amount, R_Message, idx_mbid2);
                }

                Send_Accnt_Info(R_SuccessYN, R_Message);
                this.Close();
            }
        }


        private void Cacu_Update(string C_Number1, string C_Number3, string C_Name1, int C_Cash_Sort_TF, string SuccessYN, string BankCode, double Price, string ErrMessage, int mbid2)
        {
            string StrSql = "";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            if (SuccessYN == "Y")
            {
                StrSql = " UPDATE tbl_Sales_Cacu SET ";
                StrSql = StrSql + " C_Number1 = dbo.ENCRYPT_AES256('" + C_Number1 + "') ";
                StrSql = StrSql + " , C_Number3 = '" + C_Number3 + "' ";
                StrSql = StrSql + " , C_Name1 = '" + C_Name1 + "' ";
                StrSql = StrSql + " , C_Cash_Sort_TF = " + C_Cash_Sort_TF;
                StrSql = StrSql + " WHERE OrderNumber = '" + OrderNumber + "' ";
                StrSql = StrSql + " AND C_index = " + C_Index;

                Temp_Connect.Update_Data(StrSql);


                StrSql = "Update tbl_Sales_Cacu SET ";
                StrSql = StrSql + " C_Number3  = '" + C_Number3 + "'"; //거래번호
                StrSql = StrSql + " , C_Number1 = dbo.ENCRYPT_AES256('" + C_Number1 + "')"; //가상계좌번호
                StrSql = StrSql + " ,C_CancelTF = 0 ";
                StrSql = StrSql + " ,C_CancelDate = '' ";
                StrSql = StrSql + " ,C_CancelPrice = 0 ";
                StrSql = StrSql + " Where OrderNumber ='" + OrderNumber + "'";
                StrSql = StrSql + " And   C_index = " + C_Index;

                Temp_Connect.Update_Data(StrSql);



                StrSql = "UPDATE tbl_Sales_Cacu ";
                StrSql = StrSql + "SET C_CODE = '"+ BankCode + "'";
                StrSql = StrSql + ", C_CodeName = (SELECT top  1 bankname FROM tbl_Bank WHERE ncode ='" + BankCode + "')";
                StrSql = StrSql + "where OrderNumber = '" + OrderNumber + "' ";
                StrSql = StrSql + "and C_index = " + C_Index;

                Temp_Connect.Update_Data(StrSql);


                StrSql = "DELETE FROM tbl_Sales_Cacu_ACC   ";
                StrSql = StrSql + "where OrderNumber = '" + OrderNumber + "' ";
                StrSql = StrSql + "and C_index = " + C_Index;

                Temp_Connect.Update_Data(StrSql);


                StrSql = "Insert into tbl_Sales_Cacu_ACC   ";
                StrSql = StrSql + " (OrderNumber ,C_index ,C_Cash_Receipt_TF , Bank_Code , Bank_ACC_Account , C_Price2, mbid, mbid2 ,expire_date , Cul_Send_TF , Exi_TF_OrderNumber ) ";
                StrSql = StrSql + " Values ('" + OrderNumber + "'";
                StrSql = StrSql + "," + C_Index;
                StrSql = StrSql + ",0";
                StrSql = StrSql + ",'" + BankCode + "'";
                StrSql = StrSql + ",'" + C_Number1 + "'";
                StrSql = StrSql + "," + Price;
                StrSql = StrSql + ",'' ";
                StrSql = StrSql + "," + mbid2;
                StrSql = StrSql + ",'',0 , '' ) ";

                Temp_Connect.Update_Data(StrSql);
            }
            else if (SuccessYN == "N")
            {

                C_Number3 = "";
                StrSql = "Update tbl_Sales_Cacu SET ";
                StrSql = StrSql + " C_Price1  = 0 ";
                StrSql = StrSql + " , C_Etc = C_Etc+ '" + ErrMessage + "'";  //승인 오류시 비고칸에 내역을 넣도록 한다.
                StrSql = StrSql + " Where OrderNumber ='" + OrderNumber + "'";
                StrSql = StrSql + " And   C_index = " + C_Index;

                Temp_Connect.Update_Data(StrSql);
            }


            StrSql = "Update tbl_Sales_Cacu_Card SET ";
            StrSql = StrSql + " Card_No = '" + C_Number1 + "'";
            StrSql = StrSql + " ,C_Number3 = '" + C_Number3 + "'";
            StrSql = StrSql + " ,rStatus = '" + SuccessYN + "' ";
            StrSql = StrSql + " ,Return_Date = Convert(Varchar(25),GetDate(),21)";
            StrSql = StrSql + " Where Seqno  =" + Seq_No;

            Temp_Connect.Update_Data(StrSql, "", "", 1);


        }

    }
}
