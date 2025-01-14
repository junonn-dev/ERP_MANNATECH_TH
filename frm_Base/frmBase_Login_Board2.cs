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




namespace MLM_Program
{
    public partial class frmBase_Login_Board2 : Form
    {
        cls_Grid_Base cgb = new cls_Grid_Base();
        cls_Grid_Base cgb_sale = new cls_Grid_Base();
        cls_Grid_Base cgb_item = new cls_Grid_Base();
        cls_Grid_Base cgb_salesdetail = new cls_Grid_Base();
        cls_Grid_Base cgb_sales_date = new cls_Grid_Base();
        cls_Grid_Base cgb_sales_type = new cls_Grid_Base();
        cls_Grid_Base cgb_sales_center = new cls_Grid_Base();
        cls_Grid_Base cgb_sales_salesitemdetail = new cls_Grid_Base();
        cls_Grid_Base cgb_search_item = new cls_Grid_Base();
        cls_Grid_Base cgb_search_item_count = new cls_Grid_Base();
        cls_Grid_Base cgb_search_method = new cls_Grid_Base();
        cls_Grid_Base cgb_search_class = new cls_Grid_Base();

        cls_Grid_Base cgb_members_center = new cls_Grid_Base();
        cls_Grid_Base cgb_members_grade = new cls_Grid_Base();
        cls_Grid_Base cgb_members_date = new cls_Grid_Base();
        cls_Grid_Base cgb_members_date_center = new cls_Grid_Base();

        cls_Grid_Base cgb_total_date = new cls_Grid_Base();
        cls_Grid_Base cgb_total_month = new cls_Grid_Base();
        cls_Grid_Base cgb_total_year = new cls_Grid_Base();


        private const string base_db_name = "tbl_Memberinfo";

        public delegate void SendNumberDele(string Send_Number, string Send_Name, string Send_OrderNumber);
        public event SendNumberDele Send_Sell_Number;

        public delegate void Send_Mem_NumberDele(string Send_Number, string Send_Name);
        public event Send_Mem_NumberDele Send_Mem_Number;
        

        public frmBase_Login_Board2()
        {
            InitializeComponent();
        }


        private void frmBase_Login_Board_Load(object sender, EventArgs e)
        {
           
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();

            dGridView_Sale_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_sale.d_Grid_view_Header_Reset();

            dGridView_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_item.d_Grid_view_Header_Reset();

            dGridView_Base_Header_Reset_serach_item_count();
            cgb_search_item_count.d_Grid_view_Header_Reset();

            dGridView_Base_Header_Reset_serach_method();
            cgb_search_method.d_Grid_view_Header_Reset();

            dGridView_Base_Header_Reset_serach_class();
            cgb_search_class.d_Grid_view_Header_Reset();

            //월별매출총액
            dGridView_Base_total_month();
            cgb_total_month.d_Grid_view_Header_Reset();
            //년도별매출총액
            dGridView_Base_total_year();
            cgb_total_year.d_Grid_view_Header_Reset();

            //-- 년간매출비교
            dGridView_Base_Header_Reset_salesdetail();
            cgb_salesdetail.d_Grid_view_Header_Reset();
            //--당월매출분석 일별수익
            dGridView_Base_Header_Reset_sales_date();
            cgb_sales_date.d_Grid_view_Header_Reset();
            //--당월매출분석 결제유형별
            dGridView_Base_Header_Reset_sales_type();
            cgb_sales_type.d_Grid_view_Header_Reset();
            //--당월매출분석 센터별
            dGridView_Base_Header_Reset_sales_center();
            cgb_sales_center.d_Grid_view_Header_Reset();
            //--당월매출분석 아이템별
            dGridView_Base_Header_Reset_sales_salesitemdetail();
            cgb_sales_salesitemdetail.d_Grid_view_Header_Reset();


            //회원수 센터별 일별
            dGridView_Base_Header_Reset_members_center();
            cgb_members_center.d_Grid_view_Header_Reset();
            //회원수 직급별 인원
            dGridView_Base_Header_Reset_members_grade();
            cgb_members_grade.d_Grid_view_Header_Reset();
            //회원수 당월일별 인원
            dGridView_Base_Header_Reset_members_date();
            cgb_members_date.d_Grid_view_Header_Reset();
            //회원수 당월센터별 인원
            dGridView_Base_Header_Reset_members_date_center();
            cgb_members_date_center.d_Grid_view_Header_Reset();

         
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            cls_form_Meth cm = new cls_form_Meth();
            cm.from_control_text_base_chang(this);

            string[] data_P = { cm._chang_base_caption_search("일별")
                               ,cm._chang_base_caption_search("월별")
                               ,cm._chang_base_caption_search("년별")
                              };

            // 각 콤보박스에 데이타를 초기화
            combo_Sort.Items.AddRange(data_P);
            combo_Sort.SelectedIndex = 0;

            //butt_Exit
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
    

            if (T_bt.Visible == true)
            {
                EventArgs ee1 = null;
                if (e.KeyValue == 123 )
                    Base_Button_Click(T_bt, ee1);
            }
        }

        private void frmBase_Resize(object sender, EventArgs e)
        {

            butt_Exit.Left = this.Width - butt_Exit.Width - 17;

            cls_form_Meth cfm = new cls_form_Meth();
            cfm.button_flat_change(butt_Exit);
        }

        private void frmBase_Login_Board_Activated(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void frmBase_Login_Board_Shown(object sender, EventArgs e)
        {
            DataSearch();
        }


        private void Base_Button_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt.Name == "butt_Exit")
            {
                this.Close();
            }
        }

        private void DataSearch()
        {
            dGridView_Base_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb.d_Grid_view_Header_Reset();

            dGridView_Sale_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_sale.d_Grid_view_Header_Reset();

            //월별매출총액
            dGridView_Base_total_month();
            cgb_total_month.d_Grid_view_Header_Reset();
            //년도별매출총액
            dGridView_Base_total_year();
            cgb_total_year.d_Grid_view_Header_Reset();

            //년간매출비교
            dGridView_Base_Header_Reset_salesdetail();
            cgb_salesdetail.d_Grid_view_Header_Reset();
            //--당월매출분석 일별수익
            dGridView_Base_Header_Reset_sales_date();
            cgb_sales_date.d_Grid_view_Header_Reset();
            //--당월매출분석 결제유형별
            dGridView_Base_Header_Reset_sales_type();
            cgb_sales_type.d_Grid_view_Header_Reset();
            //--당월매출분석 아이템별
            dGridView_Base_Header_Reset_sales_salesitemdetail();
            cgb_sales_salesitemdetail.d_Grid_view_Header_Reset();


            dGridView_Base_Header_Reset_serach_item_count();
            cgb_search_item_count.d_Grid_view_Header_Reset();

            //회원수 당월 일별
            dGridView_Base_Header_Reset_members_center();
            cgb_members_center.d_Grid_view_Header_Reset();
            //회원수 센터별 인원
            dGridView_Base_Header_Reset_members_grade();
            cgb_members_grade.d_Grid_view_Header_Reset();
            //회원수 날짜별 인원
            dGridView_Base_Header_Reset_members_date();
            cgb_members_date.d_Grid_view_Header_Reset();
            //회원수 당월센터별 인원
            dGridView_Base_Header_Reset_members_date_center();
            cgb_members_date_center.d_Grid_view_Header_Reset();

            dGridView_Base_Header_Reset_serach_class();
            cgb_search_class.d_Grid_view_Header_Reset();

            Member_Join_Today_Grid();           //당일회원가입현황 그리드
            Member_Num_Chart();                 //직급별, 센터별 회원수 차트
            Sum_Price_Chart();                  //전전월, 전월, 당월 매출비교
            Sale_This_Month_Chart();            //당월 매출(일별, 결제유형별)
            Sale_Today_Info_Grid();             //당일 매출정보

            total_grid(); // 일별,월별,년도별 매출총액

            
     
        }

        private void total_grid()
        {
            //월별매출총액
            string Str_Query = "";
            Str_Query = Str_Query + @" Select  LEFT(tbl_SalesDetail.SellDate,6) SellDate   ,Sum(tbl_SalesDetail.TotalPv)   ,Sum(tbl_SalesDetail.TotalPv)   ,Sum(tbl_SalesDetail.TotalCv)  
						,Sum(tbl_SalesDetail.InputCash)   ,Sum(tbl_SalesDetail.InputCard)   ,Sum(tbl_SalesDetail.InputPassbook + tbl_SalesDetail.InputPassbook_2)
						,Sum(tbl_SalesDetail.InputMile)   , Sum(tbl_SalesDetail.UnaccMoney) ,  Sum(tbl_SalesDetail.TotalInputPrice) , Sum(tbl_SalesDetail.InputPass_Pay)
						,0 Re_InputPass_Pay From tbl_SalesDetail(nolock)  LEFT JOIN tbl_SalesDetail(nolock) Re_S
                         ON tbl_SalesDetail.Re_BaseOrderNumber = Re_S.OrderNumber And Re_S.InputPass_Pay > 0
                         LEFT Join tbl_Memberinfo(nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2
                         Left Join tbl_SellType(nolock) On tbl_SellType.SellCode = tbl_SalesDetail.SellCode
                         Left JOIN tbl_Business(nolock) ON tbl_SalesDetail.BusCode = tbl_Business.ncode  And tbl_SalesDetail.Na_code = tbl_Business.Na_code
                         Where tbl_SalesDetail.Mbid2 >= 0 And tbl_SalesDetail.Ga_Order = 0  And tbl_SalesDetail.Mbid2 >= 0   And tbl_SalesDetail.BusCode
				     	 in (Select Center_Code From ufn_User_In_Center('', '') ) And tbl_Memberinfo.Na_Code in (Select Na_Code From ufn_User_In_Na_Code('') ) And tbl_SalesDetail.ReturnTF = 1
                         Group By LEFT(tbl_SalesDetail.SellDate, 6)  Order By LEFT(tbl_SalesDetail.SellDate, 6)";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();

            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date = new Dictionary<string, string>();

            string Base_Date = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_total(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date = ds.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date) == false)
                    dic_Date[Base_Date] = Base_Date;
            }

            cgb_total_month.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_total_month.db_grid_Obj_Data_Put();

            Str_Query = "";
            //년도별매출총액
            Str_Query = Str_Query + @"
                          Select  LEFT(tbl_SalesDetail.SellDate,4) SellDate   ,Sum(tbl_SalesDetail.TotalPv)   ,Sum(tbl_SalesDetail.TotalPv)   ,Sum(tbl_SalesDetail.TotalCv)   
						  ,Sum(tbl_SalesDetail.InputCash)   ,Sum(tbl_SalesDetail.InputCard)   ,Sum(tbl_SalesDetail.InputPassbook + tbl_SalesDetail.InputPassbook_2)   
						  ,Sum(tbl_SalesDetail.InputMile)   , Sum(tbl_SalesDetail.UnaccMoney) ,  Sum(tbl_SalesDetail.TotalInputPrice) , Sum(tbl_SalesDetail.InputPass_Pay)  
						  ,0 Re_InputPass_Pay From tbl_SalesDetail  (nolock)  LEFT JOIN tbl_SalesDetail 
						  (nolock) Re_S ON tbl_SalesDetail.Re_BaseOrderNumber = Re_S.OrderNumber And Re_S.InputPass_Pay > 0  
						  LEFT Join tbl_Memberinfo  (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 
						  Left Join tbl_SellType  (nolock) On tbl_SellType.SellCode=tbl_SalesDetail.SellCode   
						  Left JOIN tbl_Business (nolock) ON  tbl_SalesDetail.BusCode = tbl_Business.ncode  And tbl_SalesDetail.Na_code = tbl_Business.Na_code  
						  Where tbl_SalesDetail.Mbid2 >= 0 And  tbl_SalesDetail.Ga_Order = 0  And tbl_SalesDetail.Mbid2 >= 0   
						  And tbl_SalesDetail.BusCode in ( Select Center_Code From ufn_User_In_Center ('','') ) And tbl_Memberinfo.Na_Code 
						  in ( Select Na_Code From ufn_User_In_Na_Code ('') ) And tbl_SalesDetail.ReturnTF = 1   Group By LEFT(tbl_SalesDetail.SellDate,4)  Order By LEFT(tbl_SalesDetail.SellDate,4) ";
            StringBuilder sb = new StringBuilder();
            cls_Connect_DB temp_connect1 = new cls_Connect_DB();

            DataSet ds1 = new DataSet();

            if (temp_connect1.Open_Data_Set(Str_Query, base_db_name, ds1, this.Name, this.Text) == false) return;
            int ReCnt1 = temp_connect1.DataSet_ReCount;

            if (ReCnt1 == 0) return;

            Dictionary<int, object[]> gr_dic_text1 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date1 = new Dictionary<string, string>();

            string Base_Date1 = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt1 - 1; fi_cnt++)
            {
                Set_gr_dic_total(ref ds1, ref gr_dic_text1, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date1 = ds1.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date1.ContainsKey(Base_Date1) == false)
                    dic_Date1[Base_Date1] = Base_Date1;
            }

            cgb_total_year.grid_name_obj = gr_dic_text1;  //배열을 클래스로 보낸다.
            cgb_total_year.db_grid_Obj_Data_Put();


            Str_Query = "";
            Str_Query = Str_Query + @" SELECT tbl_Memberinfo.mbid2,tbl_Memberinfo.M_Name,isnull(tbl_Class.Grade_Name  , '등급없음') as 'class'
								
									, case when tbl_Class.Grade_Name IS NULL then  sum(TotalPv) else '' end as 'NoClass'
									, case when tbl_Class.Grade_Name = '회원' then  sum(TotalPv) else '' end as 'Member'
									, case when tbl_Class.Grade_Name = 'RCW' then  sum(TotalPv) else '' end as 'RCW'
									, case when tbl_Class.Grade_Name = 'CW' then  sum(TotalPv) else '' end as 'CW'
									, case when tbl_Class.Grade_Name = 'DD' then  sum(TotalPv) else '' end as 'DD'
									, case when tbl_Class.Grade_Name = 'RDM' then  sum(TotalPv) else '' end as 'RDM'
									, case when tbl_Class.Grade_Name = 'DM' then  sum(TotalPv) else '' end as 'DM'
									, case when tbl_Class.Grade_Name = 'EM' then  sum(TotalPv) else '' end as 'EM'
								    , case when tbl_Class.Grade_Name = 'RB' then  sum(TotalPv) else '' end as 'RB'
									, case when tbl_Class.Grade_Name = 'PT' then  sum(TotalPv) else '' end as 'PT'
									, case when tbl_Class.Grade_Name = 'GD' then  sum(TotalPv) else '' end as 'GD'
									, case when tbl_Class.Grade_Name = 'SV' then  sum(TotalPv) else '' end as 'SV'
									, case when tbl_Class.Grade_Name = 'BR' then  sum(TotalPv) else '' end as 'BR'
									, case when tbl_Class.Grade_Name = 'ST' then  sum(TotalPv) else '' end as 'ST'
								
								
								from tbl_SalesDetail (nolock) 
                               LEFT Join tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2
							    Left Join tbl_Class (nolock) ON tbl_Memberinfo.CurGrade = tbl_Class.Grade_Cnt 

								group by tbl_Memberinfo.mbid2,tbl_Memberinfo.M_Name,tbl_Class.Grade_Name 
								order by  M_Name";
            cls_Connect_DB Temp_Connect3 = new cls_Connect_DB();

            DataSet ds3 = new DataSet();

            if (Temp_Connect3.Open_Data_Set(Str_Query, base_db_name, ds3, this.Name, this.Text) == false) return;
            int Recnt3 = Temp_Connect3.DataSet_ReCount;

            if (Recnt3 == 0) return;

            Dictionary<int, object[]> gr_dic_text3 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date3 = new Dictionary<string, string>();

            string Base_Date3 = "";

            for (int fi_cnt = 0; fi_cnt <= Recnt3 - 1; fi_cnt++)
            {
                Set_gr_dic_serach_class(ref ds3, ref gr_dic_text3, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date3 = ds3.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date3.ContainsKey(Base_Date3) == false)
                    dic_Date3[Base_Date3] = Base_Date3;
            }

            cgb_search_class.grid_name_obj = gr_dic_text3;  //배열을 클래스로 보낸다.
            cgb_search_class.db_grid_Obj_Data_Put();

        }

        private void Sale_Today_Info_Grid()
        {
            string Str_Query = "";
            Str_Query = " Select tbl_SalesDetail.OrderNumber,	";
            if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 > 0)
                Str_Query = Str_Query + " tbl_SalesDetail.mbid + '-' + Convert(Varchar,tbl_SalesDetail.mbid2)  ";
            else if (cls_app_static_var.Member_Number_1 == 0 && cls_app_static_var.Member_Number_2 > 0)
                Str_Query = Str_Query + " tbl_SalesDetail.mbid2  ";
            else if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 == 0)
                Str_Query = Str_Query + " tbl_SalesDetail.mbid  ";

            Str_Query = Str_Query + @" , tbl_SalesDetail.M_Name,			
	                                    tbl_Memberinfo.hptel,			
	                                    tbl_SellType.SellTypeName,		
	                                    tbl_business1.name,				
	                                    tbl_business2.name,				
	                                    tbl_SalesDetail.TotalPv,		
	                                    ISNULL(Cacu.Price1, 0) Price1,	
	                                    ISNULL(Cacu.Price2, 0) Price2,	
	                                    ISNULL(Cacu.Price3, 0) Price3,	
	                                    ISNULL(Cacu.Price4, 0) Price4,	
	                                    ISNULL(Cacu.Price5, 0) Price5,	
	                                    tbl_SalesDetail.UnaccMoney		
                                    From tbl_SalesDetail (nolock)
                                    Left Outer Join tbl_Memberinfo (nolock) on tbl_SalesDetail.mbid = tbl_Memberinfo.mbid And tbl_SalesDetail.mbid2 = tbl_Memberinfo.mbid2
                                    Left Outer Join tbl_SellType on tbl_SalesDetail.SellCode = tbl_SellType.SellCode
                                    Left Outer Join tbl_Business tbl_business1 on tbl_Memberinfo.businesscode = tbl_business1.ncode
                                    Left Outer Join tbl_Business tbl_business2 on tbl_SalesDetail.BusCode = tbl_business2.ncode
                                    Left Outer Join (
					                                    Select
						                                    tbl_Sales_Cacu.OrderNumber, 
						                                    SUM(ISNULL(Case When tbl_Sales_Cacu.C_TF = '1' THEN tbl_Sales_Cacu.C_Price1 ELSE 0 END, 0)) Price1,
						                                    SUM(ISNULL(Case When tbl_Sales_Cacu.C_TF = '2' THEN tbl_Sales_Cacu.C_Price1 ELSE 0 END, 0)) Price2,
						                                    SUM(ISNULL(Case When tbl_Sales_Cacu.C_TF = '3' THEN tbl_Sales_Cacu.C_Price1 ELSE 0 END, 0)) Price3,
						                                    SUM(ISNULL(Case When tbl_Sales_Cacu.C_TF = '4' THEN tbl_Sales_Cacu.C_Price1 ELSE 0 END, 0)) Price4,
						                                    Sum(ISNULL(Case When tbl_Sales_Cacu.C_TF = '5' THEN tbl_Sales_Cacu.C_Price1 ELSE 0 END, 0)) Price5
					                                    From tbl_Sales_Cacu (nolock)
					                                    Left Outer Join tbl_SalesDetail (nolock) on tbl_Sales_Cacu.OrderNumber = tbl_SalesDetail.OrderNumber
					                                    Where tbl_SalesDetail.SellDate = CONVERT(nvarchar(8), GETDATE(), 112)
					                                    Group By tbl_Sales_Cacu.OrderNumber
				                                    ) Cacu on tbl_SalesDetail.OrderNumber = Cacu.OrderNumber
                                    Where tbl_SalesDetail.SellDate = CONVERT(nvarchar(8), GETDATE(), 112)
                                    ";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();

            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date = new Dictionary<string, string>();

            string Base_Date = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_Sale(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date = ds.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date) == false)
                    dic_Date[Base_Date] = Base_Date;
            }
            
            cgb_sale.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_sale.db_grid_Obj_Data_Put();

        }


        private void Sale_This_Month_Chart()
        {
            cls_form_Meth cm = new cls_form_Meth();
            string Str_Query = @"
                                Select
                                    RIGHT(CONVERT(nvarchar(8), DATEADD(D,-Ca.number,CONVERT(nvarchar(8), DATEADD(MONTH, 1, GETDATE()) - DAY(GETDATE()), 112)), 112), 2) DT, ISNULL(sale.TotalPv, 0) TotalPv
                                From master..spt_values Ca
                                Left Outer Join (
					                                Select
					                                tbl_SalesDetail.SellDate, SUM(tbl_SalesDetail.TotalPv) TotalPv
					                                From tbl_SalesDetail (nolock)
					                                Where SellDate Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6) + '%'
					                                Group By tbl_SalesDetail.SellDate
                                                    having count (SellDate) > 0
				                                ) sale on CONVERT(nvarchar(8), DATEADD(D,-Ca.number,CONVERT(nvarchar(8), DATEADD(MONTH, 1, GETDATE()) - DAY(GETDATE()), 112)), 112) = sale.SellDate
                                Where Ca.type = 'P' And Ca.number <= DATEDIFF(D, DATENAME(YEAR,GETDATE()) + DATENAME(month,GETDATE())+'01', CONVERT(nvarchar(8), DATEADD(MONTH, 1, GETDATE()) - DAY(GETDATE()), 112))
                                Order By DT ASC
                                ";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();

            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date = new Dictionary<string, string>();

            string Base_Date = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_sales_date(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date = ds.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date) == false)
                    dic_Date[Base_Date] = Base_Date;
            }

            cgb_sales_date.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_sales_date.db_grid_Obj_Data_Put();

            Str_Query =  "";


            Str_Query = Str_Query + " Select ";
            Str_Query = Str_Query + " CASE tbl_Sales_Cacu.C_TF  ";
            Str_Query = Str_Query + "   WHEN '1' THEN '" + cm._chang_base_caption_search("현금") + "'  ";
            Str_Query = Str_Query + "   WHEN '2' THEN '" + cm._chang_base_caption_search("무통장") + "'  ";
            Str_Query = Str_Query + "   WHEN '3' THEN '" + cm._chang_base_caption_search("카드") + "'  ";
            Str_Query = Str_Query + "   WHEN '4' THEN '" + cm._chang_base_caption_search("마일리지") + "'  ";
            Str_Query = Str_Query + "   WHEN '5' THEN '" + cm._chang_base_caption_search("가상계좌") + "'  ";
            Str_Query = Str_Query + " ELSE ''  ";
            Str_Query = Str_Query + "END NM_TYPE, ";
            Str_Query = Str_Query + "SUM(C_Price1) Price ";
            Str_Query = Str_Query + "From tbl_Sales_Cacu (nolock) ";
            Str_Query = Str_Query + "Where c_appdate1 Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6)+ '%' ";
            Str_Query = Str_Query + "Group By tbl_Sales_Cacu.C_TF ";

            cls_Connect_DB Temp_Connect1 = new cls_Connect_DB();

            DataSet ds1 = new DataSet();

            if (Temp_Connect1.Open_Data_Set(Str_Query, base_db_name, ds1, this.Name, this.Text) == false) return;
            int ReCnt1 = Temp_Connect1.DataSet_ReCount;

            if (ReCnt1 == 0) return;

            Dictionary<int, object[]> gr_dic_text1 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date1 = new Dictionary<string, string>();

            string Base_Date1 = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt1 - 1; fi_cnt++)
            {
                Set_gr_dic_sales_type(ref ds1, ref gr_dic_text1, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date1 = ds1.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date1) == false)
                    dic_Date[Base_Date1] = Base_Date1;
            }

            cgb_sales_type.grid_name_obj = gr_dic_text1;  //배열을 클래스로 보낸다.
            cgb_sales_type.db_grid_Obj_Data_Put();








            Str_Query = "";
            Str_Query = Str_Query + @"
                                Select
	                                tbl_Business.ncode, tbl_Business.name, SUM(tbl_Salesdetail.TotalPv) TotalPv
                                From tbl_SalesDetail (nolock)
                                Left Outer Join tbl_Memberinfo (nolock) on tbl_SalesDetail.mbid = tbl_Memberinfo.mbid And tbl_SalesDetail.mbid2 = tbl_Memberinfo.mbid2
                                Left Outer Join tbl_Business on tbl_Memberinfo.businesscode = tbl_Business.ncode
                                Where tbl_SalesDetail.SellDate Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6) + '%'
                                And ISNULL(tbl_business.ncode, '') <> ''
                                Group by tbl_Business.ncode, tbl_Business.name
                                Order by tbl_Business.ncode
                                ";

            cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();

            DataSet ds2 = new DataSet();

            if (Temp_Connect2.Open_Data_Set(Str_Query, base_db_name, ds2, this.Name, this.Text) == false) return;
            int ReCnt2 = Temp_Connect2.DataSet_ReCount;

            if (ReCnt2 == 0) return;

            Dictionary<int, object[]> gr_dic_text2 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date2 = new Dictionary<string, string>();

            string Base_Date2 = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt2 - 1; fi_cnt++)
            {
                Set_gr_dic_sales_center(ref ds2, ref gr_dic_text2, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date2 = ds2.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date2) == false)
                    dic_Date[Base_Date2] = Base_Date2;
            }

            cgb_sales_center.grid_name_obj = gr_dic_text2;  //배열을 클래스로 보낸다.
            cgb_sales_center.db_grid_Obj_Data_Put();

            Str_Query = "";
            Str_Query = Str_Query + @"
                                Select Top 1 ItemCode From tbl_MakeITemCode2 (nolock) ";

            Str_Query = "";
            Str_Query = Str_Query + @"
                                Select
                                tbl_MakeItemCode2.UpitemCode + tbl_MakeItemCode2.ItemCode UpItemCode, tbl_MakeItemCode2.ItemName, ISNULL(Item.ItemTotalPv, 0) ItemTotalPv
                                From tbl_MakeItemCode2 (nolock)
                                Left Outer Join (
					                                Select
					                                tbl_Goods.Up_itemCode, SUM(tbl_SalesItemDetail.ItemTotalPv) ItemTotalPv
					                                From tbl_SalesItemDetail (nolock)
					                                Inner Join tbl_SalesDetail (nolock) on tbl_SalesItemDetail.OrderNumber = tbl_SalesDetail.OrderNumber
					                                Left Outer Join tbl_Goods (nolock) on tbl_SalesItemDetail.ItemCode = tbl_Goods.ncode
					                                Where tbl_SalesDetail.SellDate Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6) + '%'
					                                Group By tbl_Goods.Up_itemCode
				                                ) Item on tbl_MakeItemCode2.UpitemCode + tbl_MakeItemCode2.ItemCode = Item.Up_itemCode
                                Order By tbl_MakeItemCode2.UpitemCode + tbl_MakeItemCode2.ItemCode ASC";
          

            Str_Query = "";
            Str_Query = Str_Query + @"

            Select tbl_MakeItemCode1.ItemCode, tbl_MakeItemCode1.ItemName, ISNULL(Item.ItemTotalPv, 0) ItemTotalPv
                                From tbl_MakeItemCode1 (nolock)
                                Left Outer Join (
					                                Select
						                                tbl_Goods.Up_itemCode, SUM(tbl_SalesItemDetail.ItemTotalPv) ItemTotalPv
                                                    From tbl_SalesItemDetail (nolock)
                                                    Inner Join tbl_SalesDetail (nolock) on tbl_SalesItemDetail.OrderNumber = tbl_SalesDetail.OrderNumber
                                                    Left Outer Join tbl_Goods (nolock) on tbl_SalesItemDetail.ItemCode = tbl_Goods.ncode
                                                    Where tbl_SalesDetail.SellDate Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6) + '%'
                                                    Group By tbl_Goods.Up_itemCode
				                                ) Item on tbl_MakeItemCode1.ItemCode = Item.Up_itemCode
                                Order By tbl_MakeItemCode1.ItemCode


                                ";
            cls_Connect_DB Temp_connect3 = new cls_Connect_DB();

            DataSet ds3 = new DataSet();

            if (Temp_connect3.Open_Data_Set(Str_Query, base_db_name, ds3, this.Name, this.Text) == false) return;
            int ReCnt3 = Temp_connect3.DataSet_ReCount;

            if (ReCnt3 == 0) return;

            Dictionary<int, object[]> gr_dic_text3 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date3 = new Dictionary<string, string>();

            string Base_Date3 = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt3 - 1; fi_cnt++)
            {
                Set_gr_dic_sales_salesitemdetail(ref ds3, ref gr_dic_text3, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date3 = ds3.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date3) == false)
                    dic_Date[Base_Date3] = Base_Date3;
            }

            cgb_sales_salesitemdetail.grid_name_obj = gr_dic_text3;  //배열을 클래스로 보낸다.
            cgb_sales_salesitemdetail.db_grid_Obj_Data_Put();

            //cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            //DataSet ds = new DataSet();

            //if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            //int ReCnt = Temp_Connect.DataSet_ReCount;

            //if (ReCnt == 0) return;

            ////상품중분류 코드가 등록이 되어 있으면 상품-중분류별로, 없으면 대분류로
            //if (ds.Tables[3].Rows.Count == 0)
            //{
            //    tab_sale.TabPages.Add("tabpage_item", "상품-대분류별");

            //    Chart chart_item;
            //    chart_item = new Chart();

            //    chart_item.ChartAreas.Add("Item");
            //    chart_item.Dock = DockStyle.Fill;
            //    chart_item.Name = "chart_item";
            //    tab_sale.TabPages["tabpage_item"].Controls.Add(chart_item);
            //    tab_sale.TabPages["tabpage_item"].BorderStyle = BorderStyle.FixedSingle;

            //    Series series_item;
            //    series_item = new Series();

            //    series_item.ChartArea = "Item";
            //    series_item.ChartType = SeriesChartType.Column;
            //    series_item.XValueMember = ds.Tables[5].Columns[1].ToString();
            //    series_item.YValueMembers = ds.Tables[5].Columns[2].ToString();
            //    series_item.Color = Color.FromArgb(89, 117, 156);

            //    chart_item.Series.Add(series_item);

            //    chart_item.ChartAreas["Item"].AxisX.Interval = 1;
            //    chart_item.ChartAreas["Item"].AxisX.TitleFont = new System.Drawing.Font("맑은고딕", 7);
            //    chart_item.ChartAreas["Item"].AxisX.LabelAutoFitMaxFontSize = 7;
            //    chart_item.ChartAreas["Item"].AxisY.TitleFont = new System.Drawing.Font("맑은고딕", 7);
            //    chart_item.ChartAreas["Item"].AxisY.LabelAutoFitMaxFontSize = 7;
            //    chart_item.ChartAreas["Item"].AxisY.LabelStyle.Format = cls_app_static_var.str_Currency_Type;
            //    chart_item.ChartAreas["Item"].AxisX.MajorGrid.Enabled = false;
            //    chart_item.ChartAreas["Item"].AxisY.MajorGrid.Enabled = true;
            //    chart_item.Series[0].IsVisibleInLegend = false;
            //    chart_item.Series[0]["PixelPointWidth"] = "15";

            //    chart_item.DataSource = ds.Tables[5];
            //    chart_item.DataBind();
            //    chart_item.GetToolTipText += Chart_GetToolTipText;
            //}
            //else
            //{
            //    tab_sale.TabPages.Add("tabpage_item", "상품-중분류별");

            //    Chart chart_item;
            //    chart_item = new Chart();

            //    chart_item.ChartAreas.Add("Item");
            //    chart_item.Dock = DockStyle.Fill;
            //    chart_item.Name = "chart_item";
            //    tab_sale.TabPages["tabpage_item"].Controls.Add(chart_item);
            //    tab_sale.TabPages["tabpage_item"].BorderStyle = BorderStyle.FixedSingle;

            //    Series series_item;
            //    series_item = new Series();

            //    series_item.ChartArea = "Item";
            //    series_item.ChartType = SeriesChartType.Column;
            //    series_item.XValueMember = ds.Tables[4].Columns[1].ToString();
            //    series_item.YValueMembers = ds.Tables[4].Columns[2].ToString();
            //    series_item.Color = Color.FromArgb(89, 117, 156);

            //    chart_item.Series.Add(series_item);

            //    chart_item.ChartAreas["Item"].AxisX.Interval = 1;
            //    chart_item.ChartAreas["Item"].AxisX.TitleFont = new System.Drawing.Font("맑은고딕", 7);
            //    chart_item.ChartAreas["Item"].AxisX.LabelAutoFitMaxFontSize = 7;
            //    chart_item.ChartAreas["Item"].AxisY.TitleFont = new System.Drawing.Font("맑은고딕", 7);
            //    chart_item.ChartAreas["Item"].AxisY.LabelAutoFitMaxFontSize = 7;
            //    chart_item.ChartAreas["Item"].AxisY.LabelStyle.Format = cls_app_static_var.str_Currency_Type;
            //    chart_item.ChartAreas["Item"].AxisX.MajorGrid.Enabled = false;
            //    chart_item.ChartAreas["Item"].AxisY.MajorGrid.Enabled = true;
            //    chart_item.Series[0].IsVisibleInLegend = false;
            //    chart_item.Series[0]["PixelPointWidth"] = "15";

            //    chart_item.DataSource = ds.Tables[4];
            //    chart_item.DataBind();
            //    chart_item.GetToolTipText += Chart_GetToolTipText;

            //}

        }


        private void Sum_Price_Chart()
        {
            string Str_Query = @"Select 
	                                Left(CONVERT(nvarchar(8), DATEADD(D,-Ca.number,GETDATE()), 112), 6) AS DT_Group,
	                                Right(CONVERT(nvarchar(8), DATEADD(D,-Ca.number,GETDATE()), 112),2) As DT, Isnull(Sale.TotalPv, 0) TotalPv
                                From master..spt_values Ca
                                Left Outer Join (	Select
						                                tbl_SalesDetail.SellDate, SUM(tbl_SalesDetail.TotalPv) TotalPv
					                                From tbl_SalesDetail (nolock)
					                                Where tbl_SalesDetail.SellDate Between LEFT(CONVERT(nvarchar(8), DATEADD(month, -2, GETDATE()), 112), 6) +'01' and CONVERT(nvarchar(8), GETDATE(), 112)
					                                Group By tbl_SalesDetail.SellDate
					                                ) Sale on CONVERT(nvarchar(8), DATEADD(D, -Ca.number, GETDATE()), 112) = Sale.SellDate
                                Where Ca.type = 'P' And Ca.number <= DATEDIFF(D,LEFT(CONVERT(nvarchar(8), DATEADD(month, -2, GETDATE()), 112), 6) +'01', CONVERT(nvarchar(8), GETDATE(), 112))
                                Order By DT_Group ASC, DT ASC
                                ";

      
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();

            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            //++++++++++++++++++++++++++++++++


            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_salesdetail(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.
            }


            cgb_salesdetail.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_salesdetail.db_grid_Obj_Data_Put();
        }
        private void Set_gr_dic_total(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
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
        //회원수-센터별인원
        private void Set_gr_dic_members_grade(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                    ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //회원수-센터별인원
        private void Set_gr_dic_members_center(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                    ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //회원수-당월일별인원
        private void Set_gr_dic_members_date(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //회원수--당월센터별인원
        private void Set_gr_dic_members_date_center(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //매출비교
        private void Set_gr_dic_salesdetail(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //당월매출분석 일별수익
        private void Set_gr_dic_sales_date(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //당월매출분석 타입별
        private void Set_gr_dic_sales_type(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //당월매출분석 센터별
        private void Set_gr_dic_sales_center(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                    ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        //당월매출분석 아이템별
        private void Set_gr_dic_sales_salesitemdetail(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            object[] row0 = { ds.Tables[base_db_name].Rows[fi_cnt][0]
                                ,ds.Tables[base_db_name].Rows[fi_cnt][1]
                                    ,ds.Tables[base_db_name].Rows[fi_cnt][2]
                                 };

            gr_dic_text[fi_cnt + 1] = row0;
        }
        private void Member_Num_Chart()
        {
            string Str_Query = @"
                                    Select 
	                                    count(1) cnt, tbl_Memberinfo.CurGrade, tbl_Class.Grade_Name
                                    From tbl_Memberinfo (nolock)
                                    Left Outer Join (Select 0 Grade_Code, '회원' Grade_Name Union All Select Grade_Code, Grade_Name From tbl_Class) tbl_Class on tbl_Memberinfo.CurGrade = tbl_Class.Grade_Code
                                    Where ISNULL(tbl_Memberinfo.LeaveDate, '') = ''
                                    And tbl_Memberinfo.LeaveCheck <> '0'
                                    Group by tbl_Memberinfo.CurGrade, tbl_Class.Grade_Name
                                    Order by tbl_Memberinfo.CurGrade ";
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();

            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date = new Dictionary<string, string>();

            string Base_Date = "";

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic_members_grade(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date = ds.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date) == false)
                    dic_Date[Base_Date] = Base_Date;
            }

            cgb_members_grade.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
            cgb_members_grade.db_grid_Obj_Data_Put();



            Str_Query = "";

           Str_Query = @"
                                    Select 
	                                  count(1) cnt, tbl_Memberinfo.businesscode, tbl_Business.name
                                    From tbl_Memberinfo (nolock)
                                    Left Outer Join tbl_Business on tbl_Memberinfo.businesscode = tbl_Business.ncode
                                    Where tbl_Memberinfo.businesscode in (Select ncode From tbl_Business)
                                    And tbl_Business.U_TF = 0
                                    And ISNULL(tbl_Memberinfo.LeaveDate, '') = ''
                                    And tbl_Memberinfo.LeaveCheck <> '0'
                                   
                                    Group by tbl_Memberinfo.businesscode, tbl_Business.name
                                    Order by cnt DESC  ";
            cls_Connect_DB Temp_Connect1 = new cls_Connect_DB();

            DataSet ds1 = new DataSet();

            if (Temp_Connect1.Open_Data_Set(Str_Query, base_db_name, ds1, this.Name, this.Text) == false) return;
            int Recnt1 = Temp_Connect1.DataSet_ReCount;

            if (Recnt1 == 0) return;

            Dictionary<int, object[]> gr_dic_text1 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date1 = new Dictionary<string, string>();

            string Base_Date1 = "";

            for (int fi_cnt = 0; fi_cnt <= Recnt1 - 1; fi_cnt++)
            {
                Set_gr_dic_members_center(ref ds1, ref gr_dic_text1, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date1 = ds1.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date1.ContainsKey(Base_Date1) == false)
                    dic_Date1[Base_Date1] = Base_Date1;
            }

            cgb_members_center.grid_name_obj = gr_dic_text1;  //배열을 클래스로 보낸다.
            cgb_members_center.db_grid_Obj_Data_Put();



            Str_Query = "";

            Str_Query = @"

                                    Select
	                                    ISNULL(Member.CNT, 0) CNT, RIGHT(CONVERT(nvarchar(8), DATEADD(D,-Ca.number,CONVERT(nvarchar(8), DATEADD(MONTH, 1, GETDATE()) - DAY(GETDATE()), 112)), 112), 2) DT 
                                    From master..spt_values Ca
                                    Left Outer Join (
					                                    Select tbl_Memberinfo.Regtime, COUNT(1) CNT
					                                    From tbl_Memberinfo
					                                    Where tbl_Memberinfo.Regtime Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6) + '%'
					                                    Group by tbl_Memberinfo.Regtime
				                                    ) Member on CONVERT(nvarchar(8), DATEADD(D,-Ca.number,CONVERT(nvarchar(8), DATEADD(MONTH, 1, GETDATE()) - DAY(GETDATE()), 112)), 112) = Member.Regtime
                                    Where CNT<>0 and Ca.type = 'P' And Ca.number <= DATEDIFF(D, DATENAME(YEAR,GETDATE()) + DATENAME(month,GETDATE())+'01', CONVERT(nvarchar(8), DATEADD(MONTH, 1, GETDATE()) - DAY(GETDATE()), 112))
                                    Order By DT  ";

            cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();

            DataSet ds2 = new DataSet();

            if (Temp_Connect2.Open_Data_Set(Str_Query, base_db_name, ds2, this.Name, this.Text) == false) return;
            int Recnt2 = Temp_Connect2.DataSet_ReCount;

            if (Recnt2 == 0) return;

            Dictionary<int, object[]> gr_dic_text2 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_date2 = new Dictionary<string, string>();

            string Base_Date2 = "";

            for (int fi_cnt = 0; fi_cnt <= Recnt2 - 1; fi_cnt++)
            {
                Set_gr_dic_members_date(ref ds2, ref gr_dic_text2, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date2 = ds2.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_date2.ContainsKey(Base_Date2) == false)
                    dic_date2[Base_Date2] = Base_Date2;
            }

            cgb_members_date.grid_name_obj = gr_dic_text2;  //배열을 클래스로 보낸다.
            cgb_members_date.db_grid_Obj_Data_Put();

            Str_Query = "";
            Str_Query = @"
						Select  LEFT(tbl_SalesDetail.SellDate,6) SellDate   ,Sum(tbl_SalesDetail.TotalPv)   ,Sum(tbl_SalesDetail.TotalPv)   ,Sum(tbl_SalesDetail.TotalCv)  
						,Sum(tbl_SalesDetail.InputCash)   ,Sum(tbl_SalesDetail.InputCard)   ,Sum(tbl_SalesDetail.InputPassbook + tbl_SalesDetail.InputPassbook_2)   
						,Sum(tbl_SalesDetail.InputMile)   , Sum(tbl_SalesDetail.UnaccMoney) ,  Sum(tbl_SalesDetail.TotalInputPrice) , Sum(tbl_SalesDetail.InputPass_Pay)  
						,0 Re_InputPass_Pay From tbl_SalesDetail  (nolock)  LEFT JOIN tbl_SalesDetail (nolock) Re_S 
						ON tbl_SalesDetail.Re_BaseOrderNumber = Re_S.OrderNumber And Re_S.InputPass_Pay > 0 
						 LEFT Join tbl_Memberinfo  (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2  
						 Left Join tbl_SellType  (nolock) On tbl_SellType.SellCode=tbl_SalesDetail.SellCode  
						  Left JOIN tbl_Business (nolock) ON  tbl_SalesDetail.BusCode = tbl_Business.ncode  And tbl_SalesDetail.Na_code = tbl_Business.Na_code  
						  Where tbl_SalesDetail.Mbid2 >= 0 And  tbl_SalesDetail.Ga_Order = 0  And tbl_SalesDetail.Mbid2 >= 0   And tbl_SalesDetail.BusCode 
						  in ( Select Center_Code From ufn_User_In_Center ('','') ) And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('') ) And tbl_SalesDetail.ReturnTF = 1   
						  Group By LEFT(tbl_SalesDetail.SellDate,6)  Order By LEFT(tbl_SalesDetail.SellDate,6)";





            Str_Query = "";

            Str_Query = @"Select
                                    ISNULL(Member.CNT, 0) CNT, tbl_Business.ncode, tbl_Business.name
                                    From tbl_Business  (nolock)
                                    Left Outer Join (
					                                    Select
					                                    tbl_Memberinfo.businesscode, Count(1) CNT
					                                    From tbl_Memberinfo (nolock)
					                                    Where tbl_Memberinfo.Regtime Like LEFT(CONVERT(nvarchar(8), GETDATE(), 112), 6) + '%'
					                                    Group By tbl_Memberinfo.businesscode
				                                    ) Member on tbl_Business.ncode = Member.businesscode
				                    Order By tbl_Business.ncode DESC
                            ";
            cls_Connect_DB Temp_Connect3 = new cls_Connect_DB();

            DataSet ds3 = new DataSet();

            if (Temp_Connect3.Open_Data_Set(Str_Query, base_db_name, ds3, this.Name, this.Text) == false) return;
            int Recnt3 = Temp_Connect3.DataSet_ReCount;

            if (Recnt3 == 0) return;

            Dictionary<int, object[]> gr_dic_text3 = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_date3 = new Dictionary<string, string>();

            string Base_Date3 = "";

            for (int fi_cnt = 0; fi_cnt <= Recnt3 - 1; fi_cnt++)
            {
                Set_gr_dic_members_date_center(ref ds3, ref gr_dic_text3, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date3 = ds3.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_date3.ContainsKey(Base_Date3) == false)
                    dic_date3[Base_Date3] = Base_Date3;
            }

            cgb_members_date_center.grid_name_obj = gr_dic_text3;  //배열을 클래스로 보낸다.
            cgb_members_date_center.db_grid_Obj_Data_Put();

        }

        //CASE WHEN tbl_Memberinfo.Addcode1 <> '' THEN LEFT(tbl_Memberinfo.Addcode1, 3) + '-' + SUBSTRING(tbl_Memberinfo.Addcode1, 4, LEN(tbl_Memberinfo.Addcode1)-3) ELSE '' END Addcode1,

        private void Member_Join_Today_Grid()
        {
            string Str_Query = "";

            Str_Query = "Select ";
            if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 > 0)
                Str_Query = Str_Query + " tbl_Memberinfo.mbid + '-' + Convert(Varchar,tbl_Memberinfo.mbid2)  ";
            else if (cls_app_static_var.Member_Number_1 == 0 && cls_app_static_var.Member_Number_2 > 0)
                Str_Query = Str_Query + " tbl_Memberinfo.mbid2  ";
            else if (cls_app_static_var.Member_Number_1 > 0 && cls_app_static_var.Member_Number_2 == 0)
                Str_Query = Str_Query + " tbl_Memberinfo.mbid  ";
            Str_Query = Str_Query + @" , tbl_Memberinfo.M_Name, tbl_Memberinfo.hptel, tbl_Business.name,
	                                    

tbl_Memberinfo.Addcode1,
tbl_Memberinfo.Address1 + ' ' + tbl_Memberinfo.Address2
                                    From tbl_Memberinfo (nolock)
                                    Left Outer Join tbl_Business on tbl_Memberinfo.businesscode = tbl_Business.ncode
                                    Where tbl_Memberinfo.Regtime = CONVERT(nvarchar(8), GETDATE(), 112)
                                    ";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            
            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;
            
            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
            Dictionary<string, string> dic_Date = new Dictionary<string, string>();

            string Base_Date = ""; 

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_dic(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                Base_Date = ds.Tables[base_db_name].Rows[fi_cnt][0].ToString();

                if (dic_Date.ContainsKey(Base_Date) == false)
                    dic_Date[Base_Date] = Base_Date;
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


        private void Set_gr_dic_Sale(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;
            object[] row0 = new object[cgb_sale.grid_col_Count];

            while (Col_Cnt < cgb_sale.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }

            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Base_Header_Reset()
        {
            cgb.Grid_Base_Arr_Clear();
            cgb.grid_col_Count = 6;
            cgb.basegrid = dGridView_Base;
            cgb.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb.grid_Frozen_End_Count = 2;
            cgb.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원번호", "회원명", "연락처", "센터", "우편번호"
                                    , "주소"
                                    };
            cgb.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 80, 80, 80
                            ,150
                        };
            cgb.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true
                                   };
            cgb.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               
                               ,DataGridViewContentAlignment.MiddleLeft                           
                              };
            cgb.grid_col_alignment = g_Alignment;
            
        }


        private void dGridView_Sale_Header_Reset()
        {
            cgb_sale.Grid_Base_Arr_Clear();
            cgb_sale.grid_col_Count = 14;
            cgb_sale.basegrid = dGridView_Sale;
            cgb_sale.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_sale.grid_Frozen_End_Count = 2;
            cgb_sale.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"주문번호", "회원번호", "회원명", "회원연락처", "판매유형"
                                    , "회원센터", "판매센터", "총금액", "현금", "무통장"                                
                                    , "카드", "마일리지", "가상계좌", "미결제액"
                                    };
            cgb_sale.grid_col_header_text = g_HeaderText;

            if (cls_app_static_var.Using_Mileage_TF == 1)
            {
                int[] g_Width = { 90, 100, 80, 80, 80
                                ,150 , 80 , 80 , 80 , 80
                                ,80  , 0 , 80 , 80
                                };
                cgb_sale.grid_col_w = g_Width;
            }
            else
            {
                int[] g_Width = { 90, 100, 80, 80, 80
                                ,150 , 80 , 80 , 80 , 80
                                ,80  , 80 , 80 , 80
                                };
                cgb_sale.grid_col_w = g_Width;
            }
            
            Boolean[] g_ReadOnly = { true , true,  true,  true ,true                                     
                                    ,true , true,  true,  true ,true                                                                        
                                    ,true , true,  true,  true
                                   };
            cgb_sale.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight

                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                           
                              };
            cgb_sale.grid_col_alignment = g_Alignment;

            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[8 - 1] = "###,###,###,##0";
            gr_dic_cell_format[9 - 1] = "###,###,###,##0";
            gr_dic_cell_format[10 - 1] = "###,###,###,##0";
            gr_dic_cell_format[11 - 1] = "###,###,###,##0";
            gr_dic_cell_format[12 - 1] = "###,###,###,##0";
            gr_dic_cell_format[13 - 1] = "###,###,###,##0";
            gr_dic_cell_format[14 - 1] = "###,###,###,##0";

            cgb_sale.grid_cell_format = gr_dic_cell_format;
        }

        private void dGridView_Base_Header_Reset_members_grade()
        {
            cgb_members_grade.Grid_Base_Arr_Clear();
            cgb_members_grade.grid_col_Count = 3;
            cgb_members_grade.basegrid = dataGridView_members_grade;
            cgb_members_grade.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_members_grade.grid_Frozen_End_Count = 2;
            cgb_members_grade.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"인원수", "직급코드", "직급명"
                                    };
            cgb_members_grade.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 80
                        };
            cgb_members_grade.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true
                                   };
            cgb_members_grade.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_members_grade.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            gr_dic_cell_format[2] = "###,###,###,##0";
            cgb_members_grade.grid_cell_format = gr_dic_cell_format;
        }

        //매출비교
        private void dGridView_Base_Header_Reset_members_center()
        {
            cgb_members_center.Grid_Base_Arr_Clear();
            cgb_members_center.grid_col_Count = 3;
            cgb_members_center.basegrid = dataGridView_members_center;
            cgb_members_center.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_members_center.grid_Frozen_End_Count = 2;
            cgb_members_center.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"인원수", "센터코드", "센터명"
                                    };
            cgb_members_center.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 80
                        };
            cgb_members_center.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true
                                   };
            cgb_members_center.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_members_center.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            gr_dic_cell_format[2] = "###,###,###,##0";
            cgb_members_center.grid_cell_format = gr_dic_cell_format;
        }
        private void dGridView_Base_Header_Reset_members_date()
        {
            cgb_members_date.Grid_Base_Arr_Clear();
            cgb_members_date.grid_col_Count = 2;
            cgb_members_date.basegrid = dataGridView_members_date;
            cgb_members_date.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_members_date.grid_Frozen_End_Count = 2;
            cgb_members_date.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"인원수", "날짜"
                                    };
            cgb_members_date.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100
                        };
            cgb_members_date.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true
                                   };
            cgb_members_date.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_members_date.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            cgb_members_date.grid_cell_format = gr_dic_cell_format;
        }
        private void dGridView_Base_Header_Reset_members_date_center()
        {
            cgb_members_date_center.Grid_Base_Arr_Clear();
            cgb_members_date_center.grid_col_Count = 3;
            cgb_members_date_center.basegrid = dataGridView_members_date_center;
            cgb_members_date_center.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_members_date_center.grid_Frozen_End_Count = 2;
            cgb_members_date_center.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"인원수", "센터코드", "센터명"
                                    };
            cgb_members_date_center.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100,100
                        };
            cgb_members_date_center.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true, true
                                   };
            cgb_members_date_center.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                                   ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_members_date_center.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            gr_dic_cell_format[2] = "###,###,###,##0";
            cgb_members_date_center.grid_cell_format = gr_dic_cell_format;
        }

        //매출비교
        private void dGridView_Base_Header_Reset_salesdetail()
        {
            cgb_salesdetail.Grid_Base_Arr_Clear();
            cgb_salesdetail.grid_col_Count = 3;
            cgb_salesdetail.basegrid = dGridView_Base2;
            cgb_salesdetail.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_salesdetail.grid_Frozen_End_Count = 2;
            cgb_salesdetail.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"년도/월", "일", "총액"
                                    };
            cgb_salesdetail.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 80
                        };
            cgb_salesdetail.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true
                                   };
            cgb_salesdetail.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_salesdetail.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            gr_dic_cell_format[2] = "###,###,###,##0";

            cgb_salesdetail.grid_cell_format = gr_dic_cell_format;

        }
        //당월매출분석 일별수익
        private void dGridView_Base_Header_Reset_sales_date()
        {
            cgb_sales_date.Grid_Base_Arr_Clear();
            cgb_sales_date.grid_col_Count = 2;
            cgb_sales_date.basegrid = dataGridView_sales_date;
            cgb_sales_date.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_sales_date.grid_Frozen_End_Count = 2;
            cgb_sales_date.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = { "일자", "총액"
                                    };
            cgb_sales_date.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100
                        };
            cgb_sales_date.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true
                                   };
            cgb_sales_date.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_sales_date.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";

            cgb_sales_date.grid_cell_format = gr_dic_cell_format;

        }
        //--당월매출분석 결제유형별
        private void dGridView_Base_Header_Reset_sales_type()
        {
            cgb_sales_type.Grid_Base_Arr_Clear();
            cgb_sales_type.grid_col_Count = 2;
            cgb_sales_type.basegrid = dataGridView_sales_type;
            cgb_sales_type.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_sales_type.grid_Frozen_End_Count = 2;
            cgb_sales_type.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = { "결제유형", "총액"
                                    };
            cgb_sales_type.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100
                        };
            cgb_sales_type.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true
                                   };
            cgb_sales_type.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_sales_type.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";

            cgb_sales_type.grid_cell_format = gr_dic_cell_format;

        }
        //--당월매출분석 센터별
        private void dGridView_Base_Header_Reset_sales_center()
        {
            cgb_sales_center.Grid_Base_Arr_Clear();
            cgb_sales_center.grid_col_Count = 3;
            cgb_sales_center.basegrid = dataGridView_sales_center;
            cgb_sales_center.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_sales_center.grid_Frozen_End_Count = 2;
            cgb_sales_center.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = { "센터코드","센터명", "총액"
                                    };
            cgb_sales_center.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 100
                        };
            cgb_sales_center.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true, true
                                   };
            cgb_sales_center.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                              ,DataGridViewContentAlignment.MiddleLeft
                              ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_sales_center.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            gr_dic_cell_format[2] = "###,###,###,##0";

            cgb_sales_center.grid_cell_format = gr_dic_cell_format;

        }
        //--당월매출분석 품목별
        private void dGridView_Base_Header_Reset_sales_salesitemdetail()
        {
            cgb_sales_salesitemdetail.Grid_Base_Arr_Clear();
            cgb_sales_salesitemdetail.grid_col_Count = 3;
            cgb_sales_salesitemdetail.basegrid = dataGridView_sales_salesitemdetail;
            cgb_sales_salesitemdetail.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_sales_salesitemdetail.grid_Frozen_End_Count = 2;
            cgb_sales_salesitemdetail.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = { "품목코드", "품목명", "가격"
                                    };
            cgb_sales_salesitemdetail.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 100
                        };
            cgb_sales_salesitemdetail.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true , true
                                   };
            cgb_sales_salesitemdetail.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleLeft
                                   ,DataGridViewContentAlignment.MiddleLeft
                              };
            cgb_sales_salesitemdetail.grid_col_alignment = g_Alignment;
            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[0] = "###,###,###,##0";
            gr_dic_cell_format[1] = "###,###,###,##0";
            gr_dic_cell_format[2] = "###,###,###,##0";

            cgb_sales_salesitemdetail.grid_cell_format = gr_dic_cell_format;

        }

        private void Chart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                    var dataPoint = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                    e.Text = string.Format(cls_app_static_var.str_Currency_Type, dataPoint.YValues[0]);
                    break;
            }
        }

        private void dGridView_Sale_DoubleClick(object sender, EventArgs e)
        {
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string Send_Nubmer = ""; string Send_Name = ""; ; string Send_OrderNumber = "";

                Send_OrderNumber = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                Send_Nubmer = (sender as DataGridView).CurrentRow.Cells[1].Value.ToString();
                Send_Name = (sender as DataGridView).CurrentRow.Cells[2].Value.ToString();
                Send_Sell_Number(Send_Nubmer, Send_Name, Send_OrderNumber);
            }           
        }


        private void Item_Grid_Set(string OrderNum)
        {
            if (OrderNum == "") return;

            cls_form_Meth cm = new cls_form_Meth();
            string Str_Query = "";
            Str_Query = " Select tbl_SalesItemDetail.ItemCode, tbl_Goods.name ";
            Str_Query = Str_Query + " ,Case When ReturnTF = 1 Then '" + cm._chang_base_caption_search("정상") + "'";
            Str_Query = Str_Query + "  When ReturnTF = 2 Then '" + cm._chang_base_caption_search("반품") + "'";
            Str_Query = Str_Query + "  When ReturnTF = 4 Then '" + cm._chang_base_caption_search("교환") + "'";
            Str_Query = Str_Query + "  When ReturnTF = 3 Then '" + cm._chang_base_caption_search("부분반품") + "'";
            Str_Query = Str_Query + "  When ReturnTF = 5 Then '" + cm._chang_base_caption_search("취소") + "'";
            Str_Query = Str_Query + " END ReturnTFName ";
            Str_Query = Str_Query + " , tbl_SalesItemDetail.ItemCount, tbl_SalesItemDetail.ItemTotalPv ";
            Str_Query = Str_Query + " From tbl_SalesItemDetail (nolock) ";
            Str_Query = Str_Query + " Inner Join tbl_SalesDetail (nolock) on tbl_SalesItemDetail.OrderNumber = tbl_SalesDetail.OrderNumber ";
            Str_Query = Str_Query + " Left Outer Join tbl_Goods on tbl_SalesItemDetail.ItemCode = tbl_Goods.ncode ";
            Str_Query = Str_Query + " Where tbl_SalesItemDetail.OrderNumber = '" + OrderNum + "'";

            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Str_Query, base_db_name, ds, this.Name, this.Text) == false) return;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return;

            Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();

            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                Set_gr_Item(ref ds, ref gr_dic_text, fi_cnt);
            }

            cgb_item.grid_name_obj = gr_dic_text;
            cgb_item.db_grid_Obj_Data_Put();
        }


        private void Set_gr_Item(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;

            object[] row0 = new object[cgb_item.grid_col_Count];

            while (Col_Cnt < cgb_item.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }


            gr_dic_text[fi_cnt + 1] = row0;
        }


        private void dGridView_Item_Header_Reset()
        {
            cgb_item.Grid_Base_Arr_Clear();
            cgb_item.basegrid = dGridView_Item;
            cgb_item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_item.grid_col_Count = 5;
            cgb_item.grid_Frozen_End_Count = 2;
            cgb_item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"상품코드"  , "상품명"   , "구분"  , "판매수량"   , "총상품액"        
                                };

            int[] g_Width = { 80, 100, 80, 80, 100
                            };

            DataGridViewContentAlignment[] g_Alignment =
                                {DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleLeft
                                ,DataGridViewContentAlignment.MiddleRight
                                ,DataGridViewContentAlignment.MiddleRight
                                };


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;

            cgb_item.grid_col_header_text = g_HeaderText;
            cgb_item.grid_cell_format = gr_dic_cell_format;
            cgb_item.grid_col_w = g_Width;
            cgb_item.grid_col_alignment = g_Alignment;
            
            Boolean[] g_ReadOnly = { true , true,  true,  true ,true  
                                   };
            cgb_item.grid_col_Lock = g_ReadOnly;

        }

        private void dGridView_Base_total_month()
        {




            cgb_total_month.grid_col_Count = 12;
            cgb_total_month.basegrid = dataGridView_total_month;
            cgb_total_month.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_total_month.grid_Frozen_End_Count = 1;
            cgb_total_month.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"날짜"  , "구매액"   , "구매PV"  , "_구매AV"  , "현금"
                                       , "카드"   , "무통장"   , "마일리지"    , "미결제"   , "입금액"
                                       , "배송비", "_원거래 반품배송비"
                                    };
            cgb_total_month.grid_col_header_text = g_HeaderText;

            if (cls_app_static_var.Using_Mileage_TF == 1)
            {
                int[] g_Width = { 90, 100, 80, 0, 80
                                 ,80 , 80 , 80 , 10 , 10
                                  , 10  , 0
                                };
                cgb_total_month.grid_col_w = g_Width;
            }
            else
            {
                int[] g_Width = { 90, 100, 80, 0, 80
                                 ,80 , 80 , 0 , 10 , 10
                                 , 10   ,0
                                };
                cgb_total_month.grid_col_w = g_Width;
            }

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                     ,true , true
                                   };
            cgb_total_month.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight  //5
                               
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight //10

                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight


                              };
            cgb_total_month.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[2 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[12 - 1] = cls_app_static_var.str_Grid_Currency_Type;


            cgb_total_month.grid_cell_format = gr_dic_cell_format;
            cgb_total_month.basegrid.RowHeadersVisible = false;

        }

        private void dGridView_Base_total_year()
        {
            cgb_total_year.grid_col_Count = 12;
            cgb_total_year.basegrid = dataGridView_total_year;
            cgb_total_year.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_total_year.grid_Frozen_End_Count = 1;
            cgb_total_year.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"날짜"  , "구매액"   , "구매PV"  , "_구매AV"  , "현금"
                                       , "카드"   , "무통장"   , "마일리지"    , "미결제"   , "입금액"
                                       , "배송비", "_원거래 반품배송비"
                                    };
            cgb_total_year.grid_col_header_text = g_HeaderText;

            if (cls_app_static_var.Using_Mileage_TF == 1)
            {
                int[] g_Width = { 90, 100, 80, 0, 80
                                 ,80 , 80 , 80 , 10 , 10
                                  , 10  , 0
                                };
                cgb_total_year.grid_col_w = g_Width;
            }
            else
            {
                int[] g_Width = { 90, 100, 80, 0, 80
                                 ,80 , 80 , 0 , 10 , 10
                                 , 10   ,0
                                };
                cgb_total_year.grid_col_w = g_Width;
            }

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                     ,true , true
                                   };
            cgb_total_year.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight  //5
                               
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight //10

                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight


                              };
            cgb_total_year.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[2 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[3 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[4 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[5 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[6 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[7 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[8 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[9 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[10 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[11 - 1] = cls_app_static_var.str_Grid_Currency_Type;
            gr_dic_cell_format[12 - 1] = cls_app_static_var.str_Grid_Currency_Type;


            cgb_total_year.grid_cell_format = gr_dic_cell_format;
            cgb_total_year.basegrid.RowHeadersVisible = false;

        }

        private void dGridView_Sale_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dGridView_Item_Header_Reset(); //디비그리드 헤더와 기본 셋팅을 한다.
            cgb_item.d_Grid_view_Header_Reset();

            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string Str_OrderNum = "";
                Str_OrderNum = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                Item_Grid_Set(Str_OrderNum);
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }            
        }

        private void dGridView_Base_DoubleClick(object sender, EventArgs e)
        {
            if (((sender as DataGridView).CurrentRow != null) && ((sender as DataGridView).CurrentRow.Cells[0].Value != null))
            {
                string Send_Nubmer = ""; string Send_Name = "";
                Send_Nubmer = dGridView_Base.CurrentRow.Cells[0].Value.ToString();
                Send_Name = dGridView_Base.CurrentRow.Cells[1].Value.ToString();
                Send_Mem_Number(Send_Nubmer, Send_Name); 
            }     
        }

        private void DTP_Base_CloseUp(object sender, EventArgs e)
        {
            cls_form_Meth ct = new cls_form_Meth();
            ct.form_DateTimePicker_Search_TextBox(this, (DateTimePicker)sender);
        }

        private void butt_Clear_Click(object sender, EventArgs e)
        {

            string Tsql = "";
            if (check_Set.Checked == true)
            {

                dGridView_Base_Header_Reset_serach_item_count(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_search_item_count.d_Grid_view_Header_Reset();



                if (combo_Sort.SelectedIndex == 0)
                    Tsql = "Select  tbl_SalesDetail.SellDate   ";

                if (combo_Sort.SelectedIndex == 1)
                    Tsql = "Select  LEFT(tbl_SalesDetail.SellDate,6)  SellDate  ";

                if (combo_Sort.SelectedIndex == 2)
                    Tsql = "Select  LEFT(tbl_SalesDetail.SellDate,4)  SellDate ";
           
                Tsql = Tsql + " 				,Case When tbl_Goods.Set_TF  = 0 then  tbl_SalesItemDetail.ItemCode ELSE tbl_Goods_Set.Sub_Good_Code End  ItemCode  ";
                Tsql = Tsql + " 				,Case When tbl_Goods.Set_TF  = 0 then tbl_Goods.name  ELSE (Select name from tbl_Goods (nolock) G_11 where tbl_Goods_Set.Sub_Good_Code = G_11.ncode   ) END Item_Name   ";

                Tsql = Tsql + " 				 , Case When tbl_Goods.Set_TF  = 0 then  tbl_SalesItemDetail.ItemCount  ELSE tbl_Goods_Set.Sub_Good_Cnt * tbl_SalesItemDetail.ItemCount END ItemCount  ";

                Tsql = Tsql + "  From tbl_SalesItemDetail (nolock)  LEFT JOIN tbl_SalesDetail (nolock)  ON tbl_SalesItemDetail.OrderNumber = tbl_SalesDetail.OrderNumber  ";
                Tsql = Tsql + "  LEFT JOIN tbl_Sales_Rece (nolock)  ON tbl_SalesItemDetail.OrderNumber = tbl_Sales_Rece.OrderNumber And tbl_SalesItemDetail.Salesitemindex = tbl_Sales_Rece.Salesitemindex";
                Tsql = Tsql + "    LEFT JOIN tbl_Memberinfo (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2  ";
                Tsql = Tsql + "    LEFT JOIN tbl_Business (nolock) ON tbl_Memberinfo.BusinessCode = tbl_Business.NCode And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";
                Tsql = Tsql + "      LEFT JOIN tbl_Business S_Bus (nolock) ON tbl_SalesDetail.BusCode = S_Bus.NCode And tbl_SalesDetail.Na_code = S_Bus.Na_code  ";
                Tsql = Tsql + " 	  Left Join tbl_Class C1 On tbl_Memberinfo.CurGrade=C1.Grade_Cnt  ";
                Tsql = Tsql + " 	  LEFT JOIN tbl_Goods (nolock) ON tbl_Goods.Ncode = tbl_SalesitemDetail.ItemCode  And tbl_Goods.Set_TF = 0 ";
                Tsql = Tsql + " 	    left join   tbl_Goods_Set  (nolock)  on  tbl_Goods_Set.Good_Code  = tbl_SalesItemDetail.ItemCode";
                Tsql = Tsql + " 		  LEFT Join tbl_SellType ON tbl_SalesDetail.SellCode = tbl_SellType.SellCode";
                Tsql = Tsql + " Where   tbl_SalesDetail.Ga_Order = 0 And tbl_SalesDetail.Mbid2 >= 0  And (LEFT(tbl_SalesitemDetail.SellState,1)='N' OR LEFT(tbl_SalesitemDetail.SellState,1)='C' )   ";
                if ((mtxtInDate2.Text.Replace("-", "").Trim() != "") && (mtxtInDate3.Text.Replace("-", "").Trim() == ""))
                {


                    if (combo_Sort.SelectedIndex == 0)
                        Tsql = Tsql + " And tbl_SalesDetail.SellDate = '" + mtxtInDate2.Text.Replace("-", "").Trim() + "'";

                    if (combo_Sort.SelectedIndex == 1)
                        Tsql = Tsql + " And LEFT(tbl_SalesDetail.SellDate ,6) = '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 6) + "'";

                    if (combo_Sort.SelectedIndex == 2)
                        Tsql = Tsql + " And LEFT(tbl_SalesDetail.SellDate ,4) = '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                }

                //가입일자로 검색 -2
                if ((mtxtInDate2.Text.Replace("-", "").Trim() != "") && (mtxtInDate3.Text.Replace("-", "").Trim() != ""))
                {
                    if (combo_Sort.SelectedIndex == 0)
                    {
                        Tsql = Tsql + " And  tbl_SalesDetail.SellDate >= '" + mtxtInDate2.Text.Replace("-", "").Trim() + "'";
                        Tsql = Tsql + " And  tbl_SalesDetail.SellDate <= '" + mtxtInDate3.Text.Replace("-", "").Trim() + "'";
                    }

                    if (combo_Sort.SelectedIndex == 1)
                    {
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,6) >= '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 6) + "'";
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,6) <= '" + mtxtInDate3.Text.Replace("-", "").Trim().Substring(0, 6) + "'";
                    }

                    if (combo_Sort.SelectedIndex == 2)
                    {
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,4) >= '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,4) <= '" + mtxtInDate3.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                    }
                }
                Tsql = Tsql + " 			And tbl_SalesDetail.BusCode in ( Select Center_Code From ufn_User_In_Center ('','') ) ";
                Tsql = Tsql + " 			And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('') ) ";

                Tsql = Tsql + " 			Order By tbl_SalesDetail.SellDate DESC,ITEMCODE";
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
                for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
                {
                    Set_gr_dic_serach_item_count(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                }
                cgb_search_item_count.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
                cgb_search_item_count.db_grid_Obj_Data_Put();




                cls_Connect_DB Temp_Conn = new cls_Connect_DB();
                Temp_Conn.Update_Data(
                       string.Format("EXEC [Usp_Input_serach_item_count_temp_delete]"));

                foreach (DataGridViewRow row in dataGridView_serach_item_count.Rows)
                {

                    //row.Cells["RequisitionDate"].Value = RequisitionDate;
                    //row.Cells["DepositApproximateDate"].Value = DepositApproximateDate;
                    string SellDate = row.Cells["SellDate"].Value.ToString();
                    string ItemCode = row.Cells["ItemCode"].Value.ToString();
                    string Item_Name = row.Cells["Item_Name"].Value.ToString();
                    string ItemCount = row.Cells["ItemCount"].Value.ToString();


                    //임시테이블에 박아준다.
                    Temp_Conn.Update_Data(
                       string.Format("EXEC Usp_Input_serach_item_count_temp '{0}', '{1}', '{2}','{3}'"
                       , SellDate
                       , ItemCode
                       , Item_Name
                       , ItemCount
                       ));

                }
                
                Tsql = "";

                dGridView_Base_Header_Reset_serach_item_count(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_search_item_count.d_Grid_view_Header_Reset();

                Tsql = "select selldate,itemcode, item_name, sum(itemcount) from [serach_item_count_temp] group by  selldate,itemcode, item_name";
                cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();

                DataSet ds1 = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect2.Open_Data_Set(Tsql, base_db_name, ds1, this.Name, this.Text) == false) return;
                int recnt1 = Temp_Connect2.DataSet_ReCount;

                if (recnt1 == 0) return;
                Dictionary<int, object[]> gr_dic_text1 = new Dictionary<int, object[]>();
                for (int fi_cnt = 0; fi_cnt <= recnt1 - 1; fi_cnt++)
                {
                    Set_gr_dic_serach_item_count(ref ds1, ref gr_dic_text1, fi_cnt);  //데이타를 배열에 넣는다.

                }
                cgb_search_item_count.grid_name_obj = gr_dic_text1;  //배열을 클래스로 보낸다.
                cgb_search_item_count.db_grid_Obj_Data_Put();


            }
            else
            {

                dGridView_Base_Header_Reset_serach_item(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_search_item.d_Grid_view_Header_Reset();




                if (combo_Sort.SelectedIndex == 0)
                    Tsql = "Select  tbl_SalesDetail.SellDate   ";

                if (combo_Sort.SelectedIndex == 1)
                    Tsql = "Select  LEFT(tbl_SalesDetail.SellDate,6)  SellDate  ";

                if (combo_Sort.SelectedIndex == 2)
                    Tsql = "Select  LEFT(tbl_SalesDetail.SellDate,4)  SellDate ";


                Tsql = Tsql + " , tbl_SalesItemDetail.ItemCode ";
                Tsql = Tsql + " , Isnull(tbl_Goods.Name,'')   ";



                Tsql = Tsql + " ,Sum(ItemCount) ItemCount ";


                //Tsql = Tsql + " ,Sum(ItemCount) ItemCount ";
                Tsql = Tsql + " ,Sum(ItemTotalPv) TotalPv ";
                Tsql = Tsql + " ,Sum(ItemTotalPV) TotalPV ";
                Tsql = Tsql + " ,Sum(ItemTotalCV) TotalCV ";
                Tsql = Tsql + " , '', '' ,'' ";

                Tsql = Tsql + " From tbl_SalesitemDetail (nolock) ";
                Tsql = Tsql + " LEFT Join tbl_SalesDetail  (nolock) On tbl_SalesDetail.OrderNumber = tbl_SalesitemDetail.OrderNumber ";
                Tsql = Tsql + " LEFT Join tbl_Memberinfo  (nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2 ";
                Tsql = Tsql + " LEFT Join tbl_Goods  (nolock) ON tbl_Goods.Ncode = tbl_SalesitemDetail.itemCode";

                Tsql = Tsql + " Left Join tbl_SellType  (nolock) On tbl_SellType.SellCode=tbl_SalesDetail.SellCode  ";
                Tsql = Tsql + " Left JOIN tbl_Business (nolock) ON  tbl_Memberinfo.businesscode = tbl_Business.ncode  And tbl_Memberinfo.Na_code = tbl_Business.Na_code ";


                //가입일자로 검색 -1
                Tsql = Tsql + " Where   tbl_SalesDetail.Ga_Order = 0 And tbl_SalesDetail.Mbid2 >= 0  And (LEFT(tbl_SalesitemDetail.SellState,1)='N' OR LEFT(tbl_SalesitemDetail.SellState,1)='C' )   ";
                if ((mtxtInDate2.Text.Replace("-", "").Trim() != "") && (mtxtInDate3.Text.Replace("-", "").Trim() == ""))
                {


                    if (combo_Sort.SelectedIndex == 0)
                        Tsql = Tsql + " And tbl_SalesDetail.SellDate = '" + mtxtInDate2.Text.Replace("-", "").Trim() + "'";

                    if (combo_Sort.SelectedIndex == 1)
                        Tsql = Tsql + " And LEFT(tbl_SalesDetail.SellDate ,6) = '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 6) + "'";

                    if (combo_Sort.SelectedIndex == 2)
                        Tsql = Tsql + " And LEFT(tbl_SalesDetail.SellDate ,4) = '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                }

                //가입일자로 검색 -2
                if ((mtxtInDate2.Text.Replace("-", "").Trim() != "") && (mtxtInDate3.Text.Replace("-", "").Trim() != ""))
                {
                    if (combo_Sort.SelectedIndex == 0)
                    {
                        Tsql = Tsql + " And  tbl_SalesDetail.SellDate >= '" + mtxtInDate2.Text.Replace("-", "").Trim() + "'";
                        Tsql = Tsql + " And  tbl_SalesDetail.SellDate <= '" + mtxtInDate3.Text.Replace("-", "").Trim() + "'";
                    }

                    if (combo_Sort.SelectedIndex == 1)
                    {
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,6) >= '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 6) + "'";
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,6) <= '" + mtxtInDate3.Text.Replace("-", "").Trim().Substring(0, 6) + "'";
                    }

                    if (combo_Sort.SelectedIndex == 2)
                    {
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,4) >= '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,4) <= '" + mtxtInDate3.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                    }
                }

                //strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And tbl_SalesDetail.BusCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";

                Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";



                if (combo_Sort.SelectedIndex == 0)
                {
                    Tsql = Tsql + " Group By tbl_SalesDetail.SellDate , ItemCode , isnull(Tbl_Goods.Name,'') ";
                    Tsql = Tsql + " Order By tbl_SalesDetail.SellDate , ItemCode , isnull(Tbl_Goods.Name,'') ";
                }

                if (combo_Sort.SelectedIndex == 1)
                {
                    Tsql = Tsql + " Group By LEFT(tbl_SalesDetail.SellDate,6) , ItemCode , isnull(Tbl_Goods.Name,'') ";
                    Tsql = Tsql + " Order By LEFT(tbl_SalesDetail.SellDate,6) , ItemCode , isnull(Tbl_Goods.Name,'') ";
                }


                if (combo_Sort.SelectedIndex == 2)
                {
                    Tsql = Tsql + " Group By LEFT(tbl_SalesDetail.SellDate,4) , ItemCode , isnull(Tbl_Goods.Name,'') ";
                    Tsql = Tsql + " Order By LEFT(tbl_SalesDetail.SellDate,4) , ItemCode , isnull(Tbl_Goods.Name,'') ";
                }
                cls_Connect_DB Temp_Connect = new cls_Connect_DB();

                DataSet ds = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set(Tsql, base_db_name, ds, this.Name, this.Text) == false) return;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                if (ReCnt == 0) return;
                Dictionary<int, object[]> gr_dic_text = new Dictionary<int, object[]>();
                for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
                {
                    Set_gr_dic_serach_item(ref ds, ref gr_dic_text, fi_cnt);  //데이타를 배열에 넣는다.

                }
                cgb_search_item.grid_name_obj = gr_dic_text;  //배열을 클래스로 보낸다.
                cgb_search_item.db_grid_Obj_Data_Put();




                dGridView_Base_Header_Reset_serach_method(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_search_method.d_Grid_view_Header_Reset();


                Tsql = "";
                Tsql = Tsql + @"
                  Select
                          case when  tbl_SalesDetail.RecordID  = 'web' then '웹'
                            when tbl_SalesDetail.RecordID  = 'auto' then '오토쉽'
                            when tbl_SalesDetail.RecordID ='mobile' then '모바일'
                        when tbl_SalesDetail.RecordID not in( 'mobile','auto','web') then 'ERP' END RecordID
                             ,Sum(TotalPv) TotalPv  ,Sum(TotalPV) TotalPV
                            
                          from  tbl_SalesDetail(nolock) 
                               LEFT Join tbl_Memberinfo(nolock) ON tbl_Memberinfo.Mbid = tbl_SalesDetail.Mbid And tbl_Memberinfo.Mbid2 = tbl_SalesDetail.Mbid2
                
							        Left Join tbl_SellType(nolock) On tbl_SellType.SellCode = tbl_SalesDetail.SellCode";


                //가입일자로 검색 -1
                Tsql = Tsql + " Where   tbl_SalesDetail.Ga_Order = 0 And tbl_SalesDetail.Mbid2 >= 0    ";
                if ((mtxtInDate2.Text.Replace("-", "").Trim() != "") && (mtxtInDate3.Text.Replace("-", "").Trim() == ""))
                {


                    if (combo_Sort.SelectedIndex == 0)
                        Tsql = Tsql + " And tbl_SalesDetail.SellDate = '" + mtxtInDate2.Text.Replace("-", "").Trim() + "'";

                    if (combo_Sort.SelectedIndex == 1)
                        Tsql = Tsql + " And LEFT(tbl_SalesDetail.SellDate ,6) = '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 6) + "'";

                    if (combo_Sort.SelectedIndex == 2)
                        Tsql = Tsql + " And LEFT(tbl_SalesDetail.SellDate ,4) = '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                }

                //가입일자로 검색 -2
                if ((mtxtInDate2.Text.Replace("-", "").Trim() != "") && (mtxtInDate3.Text.Replace("-", "").Trim() != ""))
                {
                    if (combo_Sort.SelectedIndex == 0)
                    {
                        Tsql = Tsql + " And  tbl_SalesDetail.SellDate >= '" + mtxtInDate2.Text.Replace("-", "").Trim() + "'";
                        Tsql = Tsql + " And  tbl_SalesDetail.SellDate <= '" + mtxtInDate3.Text.Replace("-", "").Trim() + "'";
                    }

                    if (combo_Sort.SelectedIndex == 1)
                    {
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,6) >= '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 6) + "'";
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,6) <= '" + mtxtInDate3.Text.Replace("-", "").Trim().Substring(0, 6) + "'";
                    }

                    if (combo_Sort.SelectedIndex == 2)
                    {
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,4) >= '" + mtxtInDate2.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                        Tsql = Tsql + " And LEFT( tbl_SalesDetail.SellDate ,4) <= '" + mtxtInDate3.Text.Replace("-", "").Trim().Substring(0, 4) + "'";
                    }
                }

                //strSql = strSql + " And tbl_Memberinfo.BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
                Tsql = Tsql + " And tbl_SalesDetail.BusCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";

                Tsql = Tsql + " And tbl_Memberinfo.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

                    Tsql = Tsql + " Group By tbl_SalesDetail.RecordID   ";
            
                cls_Connect_DB temp_connect1 = new cls_Connect_DB();

                DataSet ds1 = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (temp_connect1.Open_Data_Set(Tsql, base_db_name, ds1, this.Name, this.Text) == false) return;
                int ReCnt1 = temp_connect1.DataSet_ReCount;

                if (ReCnt1 == 0) return;
                Dictionary<int, object[]> gr_dic_text1 = new Dictionary<int, object[]>();
                for (int fi_cnt = 0; fi_cnt <= ReCnt1 - 1; fi_cnt++)
                {
                    Set_gr_dic_serach_method(ref ds1, ref gr_dic_text1, fi_cnt);  //데이타를 배열에 넣는다.

                }
                cgb_search_method.grid_name_obj = gr_dic_text1;  //배열을 클래스로 보낸다.
                cgb_search_method.db_grid_Obj_Data_Put();



                cls_Connect_DB Temp_Conn = new cls_Connect_DB();
                Temp_Conn.Update_Data(
                       string.Format("EXEC [Usp_Input_serach_item_method_temp_delete]"));

                foreach (DataGridViewRow row in dataGridView_serach_method.Rows)
                {

                    //row.Cells["RequisitionDate"].Value = RequisitionDate;
                    //row.Cells["DepositApproximateDate"].Value = DepositApproximateDate;
                    string RecordID = row.Cells["RecordID"].Value.ToString();
                    string TotalPv = row.Cells["TotalPv"].Value.ToString().Replace(".0000","");
                    string TotalPV = row.Cells["TotalPV"].Value.ToString();


                    //임시테이블에 박아준다.
                    Temp_Conn.Update_Data(
                       string.Format("EXEC Usp_Input_serach_item_method_temp '{0}', '{1}', '{2}' "
                       , RecordID
                       , TotalPv
                       , TotalPV
                       ));

                }

                Tsql = "";

                dGridView_Base_Header_Reset_serach_method(); //디비그리드 헤더와 기본 셋팅을 한다.
                cgb_search_method.d_Grid_view_Header_Reset();

                Tsql = "select RecordID,  sum(TotalPv), sum(TotalPV) from [serach_item_method_temp] group by  RecordID";
                cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();

                DataSet ds2 = new DataSet();
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect2.Open_Data_Set(Tsql, base_db_name, ds2, this.Name, this.Text) == false) return;
                int recnt1 = Temp_Connect2.DataSet_ReCount;

                if (recnt1 == 0) return;
                Dictionary<int, object[]> gr_dic_text2 = new Dictionary<int, object[]>();
                for (int fi_cnt = 0; fi_cnt <= recnt1 - 1; fi_cnt++)
                {
                    Set_gr_dic_serach_method(ref ds2, ref gr_dic_text2, fi_cnt);  //데이타를 배열에 넣는다.

                }
                cgb_search_method.grid_name_obj = gr_dic_text2;  //배열을 클래스로 보낸다.
                cgb_search_method.db_grid_Obj_Data_Put();



            }




        }

        private void dGridView_Base_Header_Reset_serach_item()
        {

            cgb_search_item.grid_col_Count = 10;
            cgb_search_item.basegrid = dataGridView_serach_item;
            cgb_search_item.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_search_item.grid_Frozen_End_Count = 2;
            //cgb.grid_Merge = true;
            //cgb.grid_Merge_Col_Start_index = 0;
            //cgb.grid_Merge_Col_End_index = 1;
            //cgb.grid_Frozen_End_Count = 2;
            cgb_search_item.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"구매_일자"  , "상품코드"   , "상품명"  , "구매수량"   , "구매액"
                                , "구매PV"   , "구매AV"    , ""   , ""    , ""
                                    };

            cgb_search_item.grid_col_header_text = g_HeaderText;

            int[] g_Width = { 90, 100, 80, 80, 80
                             ,80 ,0 , 0 , 0 , 0
                            };
            cgb_search_item.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true ,true
                                    ,true , true,  true,  true ,true
                                   };
            cgb_search_item.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight  //5
                               
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleRight
                               ,DataGridViewContentAlignment.MiddleLeft
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight //10
                           
                              };
            cgb_search_item.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = "###,###,###,##0";
            gr_dic_cell_format[5 - 1] = "###,###,###,##0";
            gr_dic_cell_format[6 - 1] = "###,###,###,##0";
            gr_dic_cell_format[7 - 1] = "###,###,###,##0";

            cgb_search_item.grid_cell_format = gr_dic_cell_format;
            cgb_search_item.basegrid.RowHeadersVisible = false;

        }


        private void dGridView_Base_Header_Reset_serach_item_count()
        {

            cgb_search_item_count.grid_col_Count = 4;
            cgb_search_item_count.basegrid = dataGridView_serach_item_count;
            cgb_search_item_count.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_search_item_count.grid_Frozen_End_Count = 2;
            //cgb.grid_Merge = true;
            //cgb.grid_Merge_Col_Start_index = 0;
            //cgb.grid_Merge_Col_End_index = 1;
            //cgb.grid_Frozen_End_Count = 2;
            cgb_search_item_count.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"구매_일자"  , "상품코드"   , "상품명"  , "구매수량"
                                    };
            cgb_search_item_count.grid_col_header_text = g_HeaderText;
            string[] g_Cols = {   "SellDate"     , "ItemCode"     , "Item_Name" , "ItemCount"  };

            cgb_search_item_count.grid_col_name = g_Cols;

            int[] g_Width = { 90, 100, 80, 80
                            };
            cgb_search_item_count.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true,  true 
                                   };
            cgb_search_item_count.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleRight
                           
                              };
            cgb_search_item_count.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = "###,###,###,##0";

            cgb_search_item_count.grid_cell_format = gr_dic_cell_format;
            cgb_search_item_count.basegrid.RowHeadersVisible = false;

        }

        private void dGridView_Base_Header_Reset_serach_method()
        {

            cgb_search_method.grid_col_Count = 3;
            cgb_search_method.basegrid = dataGridView_serach_method;
            cgb_search_method.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_search_method.grid_Frozen_End_Count = 2;
            //cgb.grid_Merge = true;
            //cgb.grid_Merge_Col_Start_index = 0;
            //cgb.grid_Merge_Col_End_index = 1;
            //cgb.grid_Frozen_End_Count = 2;
            cgb_search_method.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"구매방법"    , "총금액"  , "총PV"
                                    };
            cgb_search_method.grid_col_header_text = g_HeaderText;
            string[] g_Cols = { "RecordID",  "TotalPv", "TotalPV" };

            cgb_search_method.grid_col_name = g_Cols;

            int[] g_Width = { 90, 80, 80
                            };
            cgb_search_method.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true
                                   };
            cgb_search_method.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter

                              };
            cgb_search_method.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[2 - 1] = "###,###,###,##0";
            gr_dic_cell_format[3 - 1] = "###,###,###,##0";

            cgb_search_method.grid_cell_format = gr_dic_cell_format;
            cgb_search_method.basegrid.RowHeadersVisible = false;

        }

        private void dGridView_Base_Header_Reset_serach_class()
        {

            cgb_search_class.grid_col_Count = 17;
            cgb_search_class.basegrid = dataGridView_serach_class;
            cgb_search_class.grid_select_mod = DataGridViewSelectionMode.FullRowSelect;
            cgb_search_class.grid_Frozen_End_Count = 2;
            //cgb.grid_Merge = true;
            //cgb.grid_Merge_Col_Start_index = 0;
            //cgb.grid_Merge_Col_End_index = 1;
            //cgb.grid_Frozen_End_Count = 2;
            cgb_search_class.basegrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] g_HeaderText = {"회원번호"    , "이름"  , "직급", "NoClass", "회원"
                                    ,"RCW"  ,"CW"  ,"DD","RDM","DM"
                                    ,"EM","RB","PT","GD","SV"
                                    ,"BR","ST"
                                    };
            cgb_search_class.grid_col_header_text = g_HeaderText;
            string[] g_Cols = {
                "mbid2"    , "M_Name"  , "class", "NoClass", "Member"
                                    ,"RCW"  ,"CW"  ,"DD","RDM","DM"
                                    ,"EM","RB","PT","GD","SV"
                                    ,"BR","ST"
            };

            cgb_search_class.grid_col_name = g_Cols;

            int[] g_Width = { 90, 80, 80, 80, 80,
                    90, 80, 80, 80, 80,
                    90, 80, 80, 80, 80,
                    90, 80
                            };
            cgb_search_class.grid_col_w = g_Width;

            Boolean[] g_ReadOnly = { true , true,  true, true,  true,
                 true , true,  true, true,  true,
                  true , true,  true, true,  true,
                   true , true
                                   };
            cgb_search_class.grid_col_Lock = g_ReadOnly;

            DataGridViewContentAlignment[] g_Alignment =
                              {DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter

                                ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                
                                ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter
                                ,DataGridViewContentAlignment.MiddleCenter

                                ,DataGridViewContentAlignment.MiddleCenter
                               ,DataGridViewContentAlignment.MiddleCenter

                              };
            cgb_search_class.grid_col_alignment = g_Alignment;


            Dictionary<int, string> gr_dic_cell_format = new Dictionary<int, string>();
            gr_dic_cell_format[4 - 1] = "###,###,###,##0";
            gr_dic_cell_format[5 - 1] = "###,###,###,##0";
            gr_dic_cell_format[6 - 1] = "###,###,###,##0";
            gr_dic_cell_format[7 - 1] = "###,###,###,##0";
            gr_dic_cell_format[8 - 1] = "###,###,###,##0";
            gr_dic_cell_format[9 - 1] = "###,###,###,##0";
            gr_dic_cell_format[10 - 1] = "###,###,###,##0";
            gr_dic_cell_format[11- 1] = "###,###,###,##0";
            gr_dic_cell_format[12- 1] = "###,###,###,##0";
            gr_dic_cell_format[13- 1] = "###,###,###,##0";
            gr_dic_cell_format[14- 1] = "###,###,###,##0";
            gr_dic_cell_format[15- 1] = "###,###,###,##0";
            gr_dic_cell_format[16- 1] = "###,###,###,##0";
            gr_dic_cell_format[17- 1] = "###,###,###,##0";

            cgb_search_class.grid_cell_format = gr_dic_cell_format;
            cgb_search_class.basegrid.RowHeadersVisible = false;

        }








        private void Set_gr_dic_serach_item(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;

            object[] row0 = new object[cgb_search_item.grid_col_Count];

            while (Col_Cnt < cgb_search_item.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }


            gr_dic_text[fi_cnt + 1] = row0;
        }

        private void Set_gr_dic_serach_item_count(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;

            object[] row0 = new object[cgb_search_item_count.grid_col_Count];

            while (Col_Cnt < cgb_search_item_count.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }


            gr_dic_text[fi_cnt + 1] = row0;
        }
        private void Set_gr_dic_serach_method(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;

            object[] row0 = new object[cgb_search_method.grid_col_Count];

            while (Col_Cnt < cgb_search_method.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }


            gr_dic_text[fi_cnt + 1] = row0;
        }
        private void Set_gr_dic_serach_class(ref DataSet ds, ref Dictionary<int, object[]> gr_dic_text, int fi_cnt)
        {
            int Col_Cnt = 0;

            object[] row0 = new object[cgb_search_class.grid_col_Count];

            while (Col_Cnt < cgb_search_class.grid_col_Count)
            {
                row0[Col_Cnt] = ds.Tables[base_db_name].Rows[fi_cnt][Col_Cnt];
                Col_Cnt++;
            }


            gr_dic_text[fi_cnt + 1] = row0;
        }
    }

}
