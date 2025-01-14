﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Security.Cryptography;



namespace MLM_Program
{
    //public static  sqlconnection Conn;

    

    class cls_Connect_DB
    {
        public static string  Conn_Str;
        public static string  SMS_Conn_Str;
        public static string  AddCode_Conn_Str;
        public static string  Base_Conn_Str;
        public static string  Return_Conn_Str;
        public static string AddCode_Daum_Conn_Str;
        public static string Ga_Close_Conn_Str;

        /// <summary>
        /// 운영 여부(운영 외에는 false[program.cs에서 처리)
        /// <para>개발기인경우 로그인 화면 전/후 메시지 출력, 메인화면의 백그라운드 이미지/메인 Form Title에 [개발기프로그램] 문구 추가 처리</para>
        /// </summary>
        public static bool LiveFlag = true;

        private   SqlConnection Conn;
        private   SqlConnection AddCode_Conn;
        private SqlConnection AddCode_Daum_Conn;
        private   SqlConnection SMS_Conn;
        private   SqlConnection Base_Conn;
        private SqlConnection Return_Conn;
        private SqlConnection Ga_Close_Conn;

        public int DataSet_ReCount;

        public SqlConnection Conn_Conn()
        {
            return Conn;
        }


        public SqlConnection Conn_Conn_Return()
        {
            return Return_Conn;
        }

        public SqlConnection Conn_Ga_Close()
        {
            return Ga_Close_Conn;
        }


        //DB연결 관련해서 수당 부분 때문에 외부에서도 연결 할수 있도록 공개함.
        internal  void Connect_DB()
        {
            Conn = new SqlConnection(Conn_Str);            
            try
            {
                Conn.Open();               
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }


        internal void Connect_Return_DB()
        {
            Return_Conn = new SqlConnection(Return_Conn_Str);
            try
            {
                Return_Conn.Open();
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }


        internal void Connect_Ga_Close_DB()
        {
            Ga_Close_Conn = new SqlConnection(Ga_Close_Conn_Str);
            try
            {
                Ga_Close_Conn.Open();
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }



        internal void Connect_AddCode_DB()
        {
            AddCode_Conn = new SqlConnection(AddCode_Conn_Str);
            try
            {
                AddCode_Conn.Open();
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }

        internal void Connect_SMS_DB()
        {
            SMS_Conn = new SqlConnection(SMS_Conn_Str);
            try
            {
                SMS_Conn.Open();
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }

        internal void Connect_Base_DB()
        {
            Base_Conn = new SqlConnection(Base_Conn_Str);
            try
            {
                Base_Conn.Open();
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }

        internal void Connect_AddCode_Daum_DB()
        {
            AddCode_Daum_Conn = new SqlConnection(AddCode_Daum_Conn_Str);
            try
            {
                AddCode_Daum_Conn.Open();
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }
        }


        internal void Close_DB()
        {            
           if (Conn.State ==ConnectionState.Open )
           {
               Conn.Close();
            }//end if            
        }

        internal void Close_Ga_DB()
        {
            if (Ga_Close_Conn.State == ConnectionState.Open)
            {
                Ga_Close_Conn.Close();
            }//end if            
        }

        internal void Close_Return_DB()
        {
            if (Return_Conn.State == ConnectionState.Open)
            {
                Return_Conn.Close();
            }//end if            
        }



        internal void Close_Ga_Close_DB()
        {
            if (Ga_Close_Conn.State == ConnectionState.Open)
            {
                Ga_Close_Conn.Close();
            }//end if            
        }

        internal void Close_SMS_DB()
        {
            if (SMS_Conn.State == ConnectionState.Open)
            {
                SMS_Conn.Close();
            }//end if            
        }
        
        

        internal void Close_AddCode_DB()
        {
            if (AddCode_Conn.State == ConnectionState.Open)
            {
                AddCode_Conn.Close();
            }//end if            
        }

        internal void Close_Base_DB()
        {
            if (Base_Conn.State == ConnectionState.Open)
            {
                Base_Conn.Close();
            }//end if            
        }

        internal void Close_AddCode_Daum_DB()
        {
            if (AddCode_Daum_Conn.State == ConnectionState.Open)
            {
                AddCode_Daum_Conn.Close();
            }//end if            
        }


        public Boolean Open_SMSData_Set(string T_query, string T_table_name, DataSet ds)
        {
            Connect_SMS_DB(); //DB를 연결하고

            try
            {
                //Chang_T_query_N(ref T_query);

                SqlDataAdapter adapter = new SqlDataAdapter(T_query, SMS_Conn);
                adapter.SelectCommand.CommandTimeout = 0;
                DataSet_ReCount = adapter.Fill(ds, T_table_name);
                return true;
            }

            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Select_Err"));
                return false;
            }


            finally
            {
                Close_SMS_DB(); // DB와 연결을 끊는다
            }
        }




        public  Boolean  Open_Data_Set(string T_query, string T_table_name, DataSet ds)
        {
            Connect_DB(); //DB를 연결하고

            try
            {
                Chang_T_query_N(ref T_query);

                SqlDataAdapter adapter = new SqlDataAdapter(T_query, Conn);
                adapter.SelectCommand.CommandTimeout = 0;
                DataSet_ReCount = adapter.Fill(ds, T_table_name);
                return true;
            }

            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Select_Err"));
                return false;
            }


            finally
            {
                Close_DB(); // DB와 연결을 끊는다
            }    	                          
        }

        public Boolean Open_Data_Set(string T_query, string T_table_name, DataSet ds, string t_form_name = "", string t_from_text = "", int N_Chang = 0)
        {
            Connect_DB(); //DB를 연결하고

            try
            {
                if (N_Chang == 0 )
                    Chang_T_query_N(ref T_query);

                SqlDataAdapter adapter = new SqlDataAdapter(T_query, Conn);
                adapter.SelectCommand.CommandTimeout = 0;
                
                DataSet_ReCount = adapter.Fill(ds, T_table_name);

                if (t_form_name != "")
                {
                    Put_User_Log(T_query, t_form_name, t_from_text);
                }

                return true;
            }

            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Select_Err"));
                return false;
            }


            finally
            {
                Close_DB(); // DB와 연결을 끊는다
            }
        }


        public Boolean Open_Data_Set(string T_query, string T_table_name, SqlConnection T_Conn, DataSet ds, string t_form_name = "", string t_from_text = "")
        {
            SqlDataAdapter adapter = new SqlDataAdapter(T_query, T_Conn);
            adapter.SelectCommand.CommandTimeout = 0;
            DataSet_ReCount = adapter.Fill(ds, T_table_name);
            
            


            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn);
            }

            return true;           
        }


        public Boolean Open_Data_Set(string T_query, string T_table_name, SqlConnection T_Conn, SqlTransaction tran, DataSet ds, string t_form_name = "", string t_from_text = "")
        {
            SqlDataAdapter adapter = new SqlDataAdapter(T_query, T_Conn);
            adapter.SelectCommand.CommandTimeout = 0;
            
            DataSet_ReCount = adapter.Fill(ds, T_table_name);


            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn);
            }

            return true;
        }

        public Boolean Open_Data_Set_2(string T_query, string T_table_name, SqlConnection T_Conn, DataSet ds, string t_form_name = "", string t_from_text = "")
        {            

            SqlDataAdapter adapter = new SqlDataAdapter(T_query, T_Conn);
            adapter.SelectCommand.CommandTimeout = 0;
            DataSet_ReCount = adapter.Fill(ds, T_table_name);
            Chang_T_query_N(ref T_query);

            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn);
            }

            return true;
        }
          


        public Boolean Open_Data_Set(string T_query, SqlConnection T_Conn, SqlTransaction tran,ref SqlDataReader Sr , string t_form_name = "", string t_from_text = "")
        {
            
                Chang_T_query_N(ref T_query);


                SqlDataReader SR_2 = null;
                SqlCommand Select_comm_2 = new SqlCommand(T_query, T_Conn, tran);
                SR_2 = Select_comm_2.ExecuteReader();
                
                
                           
                //DataSet_ReCount = Sr.Cast<object>().Count(); 
                if (SR_2.HasRows == true)
                {
                    DataSet_ReCount = SR_2.Cast<object>().Count();
                    SR_2.Close(); SR_2.Dispose();

                    SqlCommand Select_comm = new SqlCommand(T_query, T_Conn, tran);
                    Sr = Select_comm.ExecuteReader();

                    

                }
                else
                {
                    SR_2.Close(); SR_2.Dispose();
                    SqlCommand Select_comm = new SqlCommand(T_query, T_Conn, tran);
                    Sr = Select_comm.ExecuteReader();
                    DataSet_ReCount = 0;
                }

                //DataTable dt = new DataTable();
                //dt.Load(Sr);
                //DataSet_ReCount = dt.Rows.Count;
                //dt.Clear();
                //dt.Dispose();
                //SR_2.Close(); SR_2.Dispose();

                if (t_form_name != "")
                {
                    Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
                }
                return true;
           
        }

        public Boolean Open_Data_Set(string T_query, SqlConnection T_Conn, SqlTransaction tran, ref DataTable DT, string t_form_name = "", string t_from_text = "")
        {

            Chang_T_query_N(ref T_query);
            SqlDataReader Sr = null; 

            SqlCommand Select_comm = new SqlCommand(T_query, T_Conn, tran);
            Sr = Select_comm.ExecuteReader();

            //SqlDataReader SR_2 = Sr;

            //DataTable dt = new DataTable();
            DT.Load(Sr);
            DataSet_ReCount = DT.Rows.Count;
            //dt.Clear();
            //dt.Dispose();
            //SR_2.Close(); SR_2.Dispose();

            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
            }
            return true;

        }


        //우편번호 관련 DB로 접속을 하고.. 데이타를 가져온다.
        public Boolean Open_Data_Set_Base(string T_query, string T_table_name, DataSet ds)
        {
            Connect_Base_DB(); //DB를 연결하고

            try
            {
                Chang_T_query_N(ref T_query);

                SqlDataAdapter adapter = new SqlDataAdapter(T_query, Base_Conn);
                adapter.SelectCommand.CommandTimeout = 0;
                DataSet_ReCount = adapter.Fill(ds, T_table_name);
                return true;
            }

            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Select_Err"));
                return false;
            }


            finally
            {
                Close_Base_DB(); // DB와 연결을 끊는다
            }
        }





        //우편번호 관련 DB로 접속을 하고.. 데이타를 가져온다.
        public Boolean Open_Data_Set_AddCode(string T_query, string T_table_name, DataSet ds)
        {
            Connect_AddCode_DB(); //DB를 연결하고

            try
            {
                Chang_T_query_N(ref T_query);

                SqlDataAdapter adapter = new SqlDataAdapter(T_query, AddCode_Conn);
                adapter.SelectCommand.CommandTimeout = 0;
                DataSet_ReCount = adapter.Fill(ds, T_table_name);
                return true;
            }

            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Select_Err"));
                return false;
            }


            finally
            {
                Close_AddCode_DB(); // DB와 연결을 끊는다
            }
        }




        //우편번호 관련 DB로 접속을 하고.. 데이타를 가져온다.
        public Boolean Open_Data_Set_AddCode_Daum(string T_query, string T_table_name, DataSet ds)
        {
            Connect_AddCode_Daum_DB(); //DB를 연결하고

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(T_query, AddCode_Daum_Conn);
                adapter.SelectCommand.CommandTimeout = 0;
                DataSet_ReCount = adapter.Fill(ds, T_table_name);
                return true;
            }

            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Select_Err"));
                return false;
            }


            finally
            {
                Close_AddCode_Daum_DB(); // DB와 연결을 끊는다
            }
        }


        public Boolean Query_Exec_God_Daum(string T_query, string t_form_name = "", string t_from_text = "", int nvarch_TF = 0)
        {
            Connect_AddCode_Daum_DB(); //DB를 연결하고

            SqlCommand up_comm = new SqlCommand(T_query, AddCode_Daum_Conn);
            up_comm.ExecuteNonQuery();

            Close_AddCode_Daum_DB(); // DB와 연결을 끊는다
            return true;
        }




        public Boolean Update_Data(DataSet ds, Dictionary<string, string> T_dic, string T_query, string T_table_name)
        {
            
            Connect_DB(); //DB를 연결하고

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.UpdateCommand = new SqlCommand(T_query, Conn);

                Dictionary<string, string>.KeyCollection.Enumerator ke =
                    T_dic.Keys.GetEnumerator();

                string filevalue = "";
                string fieldName = "";

                if (T_dic.Count <= 0) return true;

                while (ke.MoveNext())
                {
                    fieldName = ke.Current.ToString();
                    filevalue = T_dic[fieldName];
                    adapter.UpdateCommand.Parameters.AddWithValue(fieldName, filevalue);
                }
                adapter.Update(ds, T_table_name);

                return true; 
            }
            catch (Exception ec )
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));
                return false; 
            }
                
            finally
            {
                Close_DB(); // DB와 연결을 끊는다
            } 
        }



        public Boolean Update_Data(string T_query, string t_form_name = "", string t_from_text = "", int nvarch_TF = 0)
        {

            if (cls_User.gid == cls_User.SuperUserID)
                return Query_Exec_God(T_query,t_form_name,t_from_text,nvarch_TF) ;
            else
                return Query_Exec(T_query, t_form_name, t_from_text, nvarch_TF);
        }


        private Boolean Query_Exec_God(string T_query, string t_form_name = "", string t_from_text = "", int nvarch_TF = 0)
        {
            Connect_DB(); //DB를 연결하고
                                  
            SqlCommand up_comm = new SqlCommand(T_query, Conn);
            up_comm.ExecuteNonQuery();                       
            
            Close_DB(); // DB와 연결을 끊는다
            return true;
        }


        private Boolean  Query_Exec(string T_query, string t_form_name = "", string t_from_text = "", int nvarch_TF = 0)
        {
            Connect_DB(); //DB를 연결하고

            try
            {
                if (nvarch_TF == 0)
                    Chang_T_query_N(ref T_query);

                SqlCommand up_comm = new SqlCommand(T_query, Conn);
                up_comm.ExecuteNonQuery();

                if (t_form_name != "")
                {
                    Put_User_Log(T_query, t_form_name, t_from_text);
                }

                Close_DB(); // DB와 연결을 끊는다
                return true;
            }
            catch (Exception ec)
            {
                Close_DB(); // DB와 연결을 끊는다

                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Edit_Err"));
                return false;
            }           
        }


        public Boolean Update_Data(string T_query, SqlConnection T_Conn,SqlTransaction  tran, string t_form_name = "", string t_from_text = "")
        {              
            Chang_T_query_N(ref T_query);

            SqlCommand up_comm = new SqlCommand(T_query, T_Conn, tran);
            up_comm.ExecuteNonQuery();

            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn,tran);
            }

            return true;                   
        }



        public Boolean   Insert_Data( Dictionary<string, string> T_dic,Dictionary<string, SqlDbType > T_dic_2, string T_query, string T_table_name)
        {
            Connect_DB(); //DB를 연결하고
            int while_cnt , while_cnt_2 ;

            try
            {
                SqlCommand Comm = new SqlCommand(T_query, Conn);

                Dictionary<string, string>.KeyCollection.Enumerator ke =
                    T_dic.Keys.GetEnumerator();

                string filevalue = "";
                string fieldName = "";
                SqlDbType Sty = new SqlDbType();

                if (T_dic.Count <= 0) return true;

                while_cnt = 0;
                while (ke.MoveNext())
                {
                    fieldName = ke.Current.ToString();
                    Sty = T_dic_2[fieldName];
                    Comm.Parameters.Add(fieldName, Sty);
                    while_cnt++;
                }


                while_cnt_2 = 0;
                foreach (string T_key in T_dic.Keys)
                {
                    filevalue = T_dic[T_key];
                    Comm.Parameters[T_key].Value = filevalue;
                    while_cnt_2++;
                }

                if (while_cnt == while_cnt_2)
                {
                    Comm.ExecuteNonQuery();
                }
                else
                {
                    return false;
                }
              
                return true;
            }
            catch (Exception ec )
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                return false;
            }

            finally
            {
                Close_DB(); // DB와 연결을 끊는다
            }
        }


        public Boolean Insert_Data( string T_query, string T_table_name, string t_form_name = "", string t_from_text ="", int nvarch_TF = 0, int db_Close_TF =0)
        {
            Connect_DB(); //DB를 연결하고           

            try
            {
                if (nvarch_TF == 0)
                    Chang_T_query_N(ref T_query);
                
                SqlCommand Comm = new SqlCommand(T_query, Conn);              
                
                Comm.ExecuteNonQuery();

                if (t_form_name != "")
                {
                    Put_User_Log(T_query, t_form_name, t_from_text);
                }


                return true;
            }
            catch (Exception ec)
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Save_Err"));
                return false;
            }

            finally
            {
                if (db_Close_TF ==0 )
                    Close_DB(); // DB와 연결을 끊는다
            }
        }


        public Boolean Insert_Data(string T_query,  SqlConnection T_Conn, SqlTransaction tran)
        {
            Chang_T_query_N(ref T_query);

            SqlCommand Comm = new SqlCommand(T_query, T_Conn, tran);
            Comm.CommandTimeout = 0;
            Comm.ExecuteNonQuery();          

            return true;
        }








        public Boolean Insert_Data(string T_query, string T_table_name, SqlConnection T_Conn, SqlTransaction tran, string t_form_name = "", string t_from_text = "")
        {    
            Chang_T_query_N(ref T_query);

            SqlCommand Comm = new SqlCommand(T_query, T_Conn, tran);

            Comm.ExecuteNonQuery();

            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
            }


            return true;                
        }


        public Boolean Insert_Data(string T_query, string T_table_name, SqlConnection T_Conn)
        {
            
            SqlCommand Comm = new SqlCommand(T_query, T_Conn);

            Comm.ExecuteNonQuery();

            //if (t_form_name != "")
            //{
            //    Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
            //}
            return true;
        }


        public Boolean Update_Data(string T_query, string T_table_name, SqlConnection T_Conn)
        {

            SqlCommand Comm = new SqlCommand(T_query, T_Conn);

            Comm.ExecuteNonQuery();

            //if (t_form_name != "")
            //{
            //    Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
            //}
            return true;
        }


        public Boolean Delete_Data(string T_query, string T_table_name, string t_form_name = "", string t_from_text = "")
        {
            Connect_DB(); //DB를 연결하고           

            try
            {

                Chang_T_query_N(ref T_query);

                SqlCommand Comm = new SqlCommand(T_query, Conn);

                Comm.ExecuteNonQuery();

                if (t_form_name != "")
                {
                    Put_User_Log(T_query, t_form_name, t_from_text);
                }


                return true;
            }
            catch (Exception ec )
            {
                if (cls_User.gid == cls_User.SuperUserID)
                    MessageBox.Show(ec.Message);
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Base_Del_Err"));
                return false;
            }

            finally
            {
                Close_DB(); // DB와 연결을 끊는다
            }
        }


        public Boolean Delete_Data(string T_query, string T_table_name, SqlConnection T_Conn, SqlTransaction tran, string t_form_name = "", string t_from_text = "")
        {            
            Chang_T_query_N(ref T_query);

            SqlCommand Comm = new SqlCommand(T_query, T_Conn, tran);

            Comm.ExecuteNonQuery();

            if (t_form_name != "")
            {
                Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
            }

            return true; 
        }

        public Boolean Delete_Data(string T_query, string T_table_name, SqlConnection T_Conn)
        {

            try
            {
                SqlCommand Comm = new SqlCommand(T_query, T_Conn);

                Comm.ExecuteNonQuery();
                return true; 
            }
            catch (Exception )
            {
                return true; 
            }
            //if (t_form_name != "")
            //{
            //    Put_User_Log(T_query, t_form_name, t_from_text, T_Conn, tran);
            //}
            //return true;
        }


        private void Put_User_Log(string T_query, string t_form_name, string  t_form_text )
        {

            string T_Job_ETC = T_query.Replace("'", "\"\"");            
            cls_WATCrypt m_crypt = new cls_WATCrypt("12345678");           

            T_Job_ETC = m_crypt.Encrypt(T_Job_ETC);
            //T_Job_ETC = m_crypt.Decrypt (T_Job_ETC);  

            string T_Sql = "INSERT INTO tbl_User_Log (T_U_ID,Form_Name , Form_Name_Caption ,Job_Time,Job_Code,Job_Suc_TF,Job_ETC) " ;
            T_Sql = T_Sql + " Values ";
            T_Sql = T_Sql + " (";
            T_Sql = T_Sql + "'" + cls_User.gid + "',";
            T_Sql = T_Sql + "'" + t_form_name + "',";
            T_Sql = T_Sql + "'" + t_form_text + "',Convert(Varchar(25),GetDate(),21) ,"; 
            T_Sql = T_Sql + "'',0,'" + T_Job_ETC + "'" ;
            T_Sql = T_Sql + ")";
            
            SqlCommand Comm = new SqlCommand(T_Sql, Conn);

            Comm.ExecuteNonQuery();
        }


        private void Put_User_Log(string T_query, string t_form_name, string t_form_text, SqlConnection T_Conn, SqlTransaction tran)
        {
            
            
            string T_Job_ETC = T_query.Replace("'", "\"\"");

            cls_WATCrypt m_crypt = new cls_WATCrypt("12345678");
            T_Job_ETC = m_crypt.Encrypt(T_Job_ETC);

            string T_Sql = "INSERT INTO tbl_User_Log (T_U_ID,Form_Name , Form_Name_Caption ,Job_Time,Job_Code,Job_Suc_TF,Job_ETC) ";
            T_Sql = T_Sql + " Values ";
            T_Sql = T_Sql + " (";
            T_Sql = T_Sql + "'" + cls_User.gid + "',";
            T_Sql = T_Sql + "'" + t_form_name + "',";
            T_Sql = T_Sql + "'" + t_form_text + "',Convert(Varchar(25),GetDate(),21) ,";
            T_Sql = T_Sql + "'',0,'" + T_Job_ETC + "'";
            T_Sql = T_Sql + ")";

            SqlCommand Comm = new SqlCommand(T_Sql, T_Conn, tran);

            Comm.ExecuteNonQuery();
        }


        private void Put_User_Log(string T_query, string t_form_name, string t_form_text, SqlConnection T_Conn)
        {


            string T_Job_ETC = T_query.Replace("'", "\"\"");

            cls_WATCrypt m_crypt = new cls_WATCrypt("12345678");
            T_Job_ETC = m_crypt.Encrypt(T_Job_ETC);

            string T_Sql = "INSERT INTO tbl_User_Log (T_U_ID,Form_Name , Form_Name_Caption ,Job_Time,Job_Code,Job_Suc_TF,Job_ETC) ";
            T_Sql = T_Sql + " Values ";
            T_Sql = T_Sql + " (";
            T_Sql = T_Sql + "'" + cls_User.gid + "',";
            T_Sql = T_Sql + "'" + t_form_name + "',";
            T_Sql = T_Sql + "'" + t_form_text + "',Convert(Varchar(25),GetDate(),21) ,";
            T_Sql = T_Sql + "'',0,'" + T_Job_ETC + "'";
            T_Sql = T_Sql + ")";

            SqlCommand Comm = new SqlCommand(T_Sql, T_Conn);

            Comm.ExecuteNonQuery();
        }
         


        private void Chang_T_query_N (ref string  T_query )
        {
            // 태국이나 베트남 같은 언어의 경우에는 nvarchar저장시에 N을 붙여줘야 하므로
                // 저장되는 쿼리에서 문자 기준으로 쪼개서 N을 붙여줌.
                if( cls_app_static_var.app_multi_lang_query  ==1 )
                {                    
                    string[] query = T_query.Split(Convert.ToChar ("'"));

                    int i = 1;
                    
                    T_query = "";
                                       
                    foreach (string t_wi in query) 
                    {
                        if ((i % 2 == 1) && (i <= query.Length-1 )
                            && (query[i] != "-")
                            && (query[i] != "")
                            )
                        {
                            if (query.Length != i) 
                                T_query += t_wi + "N'";
                            else
                                T_query += t_wi ;
                        }
                        else
                        {
                            if (query.Length != i)
                                T_query += t_wi + "'";
                            else
                                T_query += t_wi;
                        }
                        i++;
                    }
                }
        }       
        
    } // end cls_Connect_DB



    class cls_Search_DB
    {

        private Dictionary<int, string> Be_Memberinfo = new Dictionary<int, string>() ;

        public int Member_Name_Search(string Search_Member,ref string  Search_Name )
        {
            Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim (), ref Mbid, ref Mbid2) < 0) return -1;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length  == 0 )
                Tsql = Tsql + " Where Convert(Varchar,Mbid2) like '%" + Mbid2.ToString () + "%' ";
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And  Mbid2 = " + Mbid2.ToString();
                //Tsql = Tsql + " And   Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            }
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";

            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
                
            return ReCnt;            
            //++++++++++++++++++++++++++++++++
        }



        //20191015 구현호 핸드폰으로 조회
        public int Member_hptel_Search(string Search_Member, ref string Mbid2, ref string Search_Name)
        {
            Search_Name = "";
            string hptel = "";  Mbid2 = "";

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            //if (Member_Nmumber_Split(Search_Member.Trim(), ref hptel, ref Mbid2) < 0) return -1;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid2, M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
         
            Tsql = Tsql + " Where hptel like '%" + Search_Member.ToString() + "%' ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
            {
                Search_Name = "";
                return ReCnt;
            }
            else if (ReCnt == 1)
            {
                Mbid2 = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();
            }
            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }



        //20191015 구현호 생일로 조회
        public int Member_birthday_Search(string Search_Member, ref string Mbid2, ref string Search_Name)
        {
            Search_Name = "";
            string hptel = ""; Mbid2 = "";

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            //if (Member_Nmumber_Split(Search_Member.Trim(), ref hptel, ref Mbid2) < 0) return -1;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid2, M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";

            Tsql = Tsql + " Where Birthday + '-' + Birthday_M + '-' + Birthday_D ='" + Search_Member.ToString() + "' ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
            {
                Search_Name = "";
                return ReCnt;
            }
            else if (ReCnt == 1)
            {
                Mbid2 = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();
            }
            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }

        public int Member_Name_Search_S_N(string Search_Member, ref string Search_Name)
        {
            Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return -1;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And  Mbid2 = " + Mbid2.ToString();
                //Tsql = Tsql + " And   Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            }
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            //Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
            //Tsql += " AND US_Num <> 0 ";
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }
    
        public int Member_Name_Search_S_N_tbl_memberinfo(string Search_Member, ref string Search_Name)
        {
            Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return -1;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            //Tsql = "Select M_Name  ";
            //Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            //if (Mbid.Length == 0)
            //    Tsql = Tsql + " Where Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            //else
            //{
            //    Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
            //    Tsql = Tsql + " And  Mbid2 = " + Mbid2.ToString();
            //    //Tsql = Tsql + " And   Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            //}
            ////// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            ////Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
            ////Tsql += " AND US_Num <> 0 ";
            //Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";
          
            Tsql = "exec [Usp_JDE_SELECT_NOM_SAVE_like] "+ Mbid2 + "";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }

        public int Member_Name_Search_S_N_tbl_memberinfo_Fo(string Search_Member, ref string Search_Name)
        {
            Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return -1;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            //Tsql = "Select M_Name  ";
            //Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            //if (Mbid.Length == 0)
            //    Tsql = Tsql + " Where Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            //else
            //{
            //    Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
            //    Tsql = Tsql + " And  Mbid2 = " + Mbid2.ToString();
            //    //Tsql = Tsql + " And   Convert(Varchar,Mbid2) like '%" + Mbid2.ToString() + "%' ";
            //}
            ////// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            ////Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
            ////Tsql += " AND US_Num <> 0 ";
            //Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            Tsql = "exec [Usp_JDE_SELECT_NOM_SAVE_like_Fo] " + Mbid2 + "";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }



        public string Auto_Member_Number_Search(string Search_Member)
        {
            string Auto_Number = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2);


            if (Mbid == "")
                Mbid = cls_app_static_var.Mem_Number_Auto_Base_Mbid;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "EXEC Usp_Insert_tbl_Memberinfo_Number '',0,'" + Mbid + "',0 ";
            
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return Auto_Number;
            int ReCnt = Temp_Connect.DataSet_ReCount;
    
            Auto_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
            return Auto_Number;
            //++++++++++++++++++++++++++++++++
        }


        public string Auto_Member_Number_Search_Hand(string Search_Member)
        {
            string Auto_Number = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return Auto_Number;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();

            string Tsql;
            Tsql = "EXEC Usp_Insert_tbl_Memberinfo_Number '" + Mbid + "'," + Mbid2  + ",'''',0 ";
           
            DataSet ds = new DataSet();

            try
            {
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set_2(Tsql, "tbl_Memberinfo", Conn, ds) == false) return Auto_Number;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                Auto_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
                return Auto_Number;
            }

            catch (Exception )
            {
                Auto_Number = "";
                return Auto_Number;
            }
        }




        public string Auto_Member_Number_Search_Random(string Search_Member)
        {
            string Auto_Number = "";
            string Mbid = ""; int Mbid2 = 0;
            int Base_Num = 1;


            //for (int i = 1; i <= cls_app_static_var.Member_Number_2 - 1; i++)
            //{
            //    Base_Num = Base_Num * 10;
            //}

            for (int i = 1; i <= 5; i++)
            {
                Base_Num = Base_Num * 10;
            }


            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.

            Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2);

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();

            string Tsql;

            if (Mbid == "")
                Mbid = cls_app_static_var.Mem_Number_Auto_Base_Mbid;


            Tsql = "EXEC Usp_Insert_tbl_Memberinfo_Number_Ran_QSciences '''',0,'" + Mbid + "',0, " + Base_Num;
            //Tsql = "EXEC Usp_Insert_tbl_Memberinfo_Number_Ran '''',0,'" + Mbid + "',0, " + Base_Num ;

            DataSet ds = new DataSet();

            try
            {
                //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
                if (Temp_Connect.Open_Data_Set_2(Tsql, "tbl_Memberinfo", Conn, ds) == false) return Auto_Number;
                int ReCnt = Temp_Connect.DataSet_ReCount;

                Auto_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
                return Auto_Number;
            }

            catch (Exception)
            {
                Auto_Number = "";
                return Auto_Number;
            }
        }




        public int Member_Name_Search(ref string Member_Number,string Search_Name )
        {                     
           
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid, Mbid2  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            Tsql = Tsql + " Where tbl_Memberinfo.M_name like '%" + Search_Name + "%' ";
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Member_Number = "";
            else if (ReCnt == 1)
                if (cls_app_static_var.Member_Number_1 > 0 )
                    Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString() + "-" + 
                                    ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString()  ;
                else
                    Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }

        public int Member_Webid_Search(ref string Member_Number, string Search_Name)
        {

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid, Mbid2  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            Tsql = Tsql + " Where tbl_Memberinfo.Webid like '%" + Search_Name + "%' ";
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Member_Number = "";
            else if (ReCnt == 1)
                if (cls_app_static_var.Member_Number_1 > 0)
                    Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString() + "-" +
                                    ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();
                else
                    Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }

        public int Member_Name_Search_S_N(ref string Member_Number, string Search_Name)
        {

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid, Mbid2  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            Tsql = Tsql + " Where tbl_Memberinfo.M_name like '%" + Search_Name + "%' ";
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
           // Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode + "') )";
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Member_Number = "";
            else if (ReCnt == 1)
                if (cls_app_static_var.Member_Number_1 > 0)
                    Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString() + "-" +
                                    ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();
                else
                    Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }




        public string Member_Name_Search(string Search_Member)        
        {
            string Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return "-1";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }
            //// Tsql = Tsql + " And  tbl_Memberinfo.Full_Save_TF  = 1 ";
            Tsql = Tsql + " And BusinessCode in ( Select Center_Code From ufn_User_In_Center ('" + cls_User.gid_CenterCode + "','" + cls_User.gid_CountryCode  + "') )";
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return "-2";
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return Search_Name;
            //++++++++++++++++++++++++++++++++
        }


        public string Member_Search_Base(string Search_Member)
        {
            string Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return "-1";

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }            
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return "-2";
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return Search_Name;
            //++++++++++++++++++++++++++++++++
        }




        public int LineCnt_Search_Save(string Mbid, int Mbid2)
        {
            Dictionary<int, int> S_Line = new Dictionary<int, int>();


            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select LineCnt  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Saveid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Saveid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Saveid2 = " + Mbid2.ToString();
            }

            Tsql = Tsql + " And LineCnt > 0 ";
            Tsql = Tsql + " Order by LineCnt ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -1;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)                return 1;

            int T_LineCnt  = 0 ;
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                T_LineCnt = int.Parse (ds.Tables["tbl_Memberinfo"].Rows[fi_cnt][0].ToString() );
                S_Line[T_LineCnt] = T_LineCnt;
            }

            int Cnt = 0;
            for (Cnt = 1; Cnt <= cls_app_static_var.Member_Down_Cnt; Cnt++)
            {
                if (S_Line.ContainsKey(Cnt) == false) break;                
            }

            return Cnt;
            //++++++++++++++++++++++++++++++++
        }


        public int LineCnt_Search_Save_Check(string Mbid, int Mbid2 , int Select_Line)
        {
            Dictionary<int, int> S_Line = new Dictionary<int, int>();


            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select LineCnt  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Saveid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Saveid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Saveid2 = " + Mbid2.ToString();
            }

            Tsql = Tsql + " And LineCnt > 0 ";
            Tsql = Tsql + " Order by LineCnt ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -1;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return Select_Line;

            int T_LineCnt = 0;
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                T_LineCnt = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[fi_cnt][0].ToString());
                S_Line[T_LineCnt] = T_LineCnt;
            }

            if (S_Line.ContainsKey(Select_Line) == false)  //선택한 라인이 비어있으면  그라인을 그대로 보낸다/
            {
                return Select_Line;
            }
            else
            {

                int Cnt = 0;
                for (Cnt = 1; Cnt <= cls_app_static_var.Member_Down_Cnt; Cnt++)
                {
                    if (S_Line.ContainsKey(Cnt) == false) break;
                }

                return Cnt;
            }
            //++++++++++++++++++++++++++++++++
        }


        public int N_LineCnt_Search_Nom(string Mbid, int Mbid2)
        {
            Dictionary<int, int> S_Line = new Dictionary<int, int>();


            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select N_LineCnt  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Nominid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Nominid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Nominid2 = " + Mbid2.ToString();
            }

            Tsql = Tsql + " And N_LineCnt > 0 ";
            Tsql = Tsql + " Order by N_LineCnt ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -1;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return 1;

            int T_LineCnt = 0;
            for (int fi_cnt = 0; fi_cnt <= ReCnt - 1; fi_cnt++)
            {
                T_LineCnt = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[fi_cnt][0].ToString());
                S_Line[T_LineCnt] = T_LineCnt;
            }

            int Cnt = 0;
            for (Cnt = 1; Cnt <= 3000; Cnt++)
            {
                if (S_Line.ContainsKey(Cnt) == false) break;
            }

            return Cnt;
            //++++++++++++++++++++++++++++++++
        }


        //텍스트 박스에 입력된 회원번호 내역이 올바른지를 체크하는 메소드
        public int Member_Nmumber_Split(string Search_Member_Num, ref string Mbid, ref int Mbid2)
        {
            if (Search_Member_Num.Trim() == "")
                return -1;

            Mbid = ""; Mbid2 = 0;
            string[] t_Mbid;
            t_Mbid = Search_Member_Num.Trim().Split('-');

            //회원번호 체게상 앞자리를 사용한다고 햇는데 나온거는 한자리 이면 오류임 2자리 이상은 나와야함.
            if (cls_app_static_var.Member_Number_1 > 0 && t_Mbid.Length <= 1)               return -1;
            

            if (t_Mbid.Length == 2)
            {
                if (t_Mbid[0] == "" || t_Mbid[1] == "") return -1;
            }

            if (t_Mbid.Length == 1)
                Mbid2 = int.Parse(t_Mbid[0]);
            else
            {
                Mbid = t_Mbid[0].Replace(" ","");
                Mbid2 = int.Parse(t_Mbid[1].Replace(" ", ""));
            }

            if (Mbid2 == 0 )
                return -1;

            return 1;
        }


        //다단계 같은 경우 마케팅상 하선의 후원인은 제한적이다.
        // 그 이상을 벗어나서 등록 시킬수 없다. 그래서.. 그 제한을 체크하는 메소드
        //일반적으로 방판은 제한이 없기 때문에 방판에서는 거의 2천 가까이 열어서 무한으로 들어오게 함.
        public Boolean  Member_Down_Save_TF(string Search_Member)
        {            
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false ;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Saveid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Saveid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Saveid2 = " + Mbid2.ToString();
            }

            Tsql = Tsql + " And LineCnt<>0 ";            
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false ;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt < cls_app_static_var.Member_Down_Cnt)
                return true;
            else
                return false ;

            
            //++++++++++++++++++++++++++++++++
        }

        public Boolean Member_Down_Save_TF(string Search_Member, string Nom_Mbid)
        {
            string Mbid = ""; int Mbid2 = 0;  //등록한 후원인             
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false;


            string N_Mbid = ""; int N_Mbid2 = 0;            //등록한 추천인번호
            if (Member_Nmumber_Split(Nom_Mbid.Trim(), ref N_Mbid, ref N_Mbid2) < 0) return false;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid, Mbid2  ";
            Tsql = Tsql + " From ufn_GetSubTree_MemGroup_Mannatech_Nom_Down_Check  ('" + N_Mbid + "'," + N_Mbid2 + ")  ";            //입력 추천인 하부후원조직상 .. 입력한 후원인이 있어야 한다.
            Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
            Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
                return true;
            else
                return false;


            //++++++++++++++++++++++++++++++++
        }
        public Boolean Member_Down_Save_TF_Fo(string Search_Member, string Nom_Mbid)
        {
            string Mbid = ""; int Mbid2 = 0;  //등록한 후원인             
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false;


            string N_Mbid = ""; int N_Mbid2 = 0;            //등록한 추천인번호
            if (Member_Nmumber_Split(Nom_Mbid.Trim(), ref N_Mbid, ref N_Mbid2) < 0) return false;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid, Mbid2  ";
            Tsql = Tsql + " From ufn_GetSubTree_MemGroup_Mannatech_Nom_Down_Check_Fo  ('" + N_Mbid + "'," + N_Mbid2 + ")  ";            //입력 추천인 하부후원조직상 .. 입력한 후원인이 있어야 한다.
            Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
            Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
                return true;
            else
                return false;


            //++++++++++++++++++++++++++++++++
        }
        public Boolean Member_Down_Save_TF(string Search_Member , string Self_Member, int LineCnt, ref string Org_Mbid , ref int Org_Mbid2 )
        {
            string Mbid = ""; int Mbid2 = 0;
            string Se_Mbid = ""; int Se_Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false;

            if (Member_Nmumber_Split(Self_Member.Trim(), ref Se_Mbid, ref Se_Mbid2) < 0) return false;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid , Mbid2 ,  M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Saveid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Saveid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Saveid2 = " + Mbid2.ToString();
            }

            Tsql = Tsql + " And LineCnt = " + LineCnt.ToString()  ;
            Tsql = Tsql + " And Mbid +'-'+ Convert(Varchar, Mbid2 ) <> '" + Se_Mbid + '-' + Se_Mbid2.ToString() + "'";
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt <= 0)
                return true;
            else
            {
                Org_Mbid = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString() ;
                Org_Mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][1].ToString());
                return false;
            }

            //++++++++++++++++++++++++++++++++
        }



        public Boolean Close_Check_SellDate(string table_Name, string SearchDate_T, int Msg_SF = 0)
        {
            string SearchDate = SearchDate_T.Replace("-", "");
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select ToEndDate  ";
            Tsql = Tsql + " From " + table_Name + "  (nolock)  ";
            Tsql = Tsql + "Where ToEndDate >=  '" + SearchDate + "'";
            
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                if (Msg_SF == 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Close_Date")
                                   + "\n" +
                                   cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                }
                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }

        public Boolean Check_Stock_INPut(string OrderNumber, ref string Center_N, ref string Center_Code, ref string U_ID, ref string in_Date)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT Top 1 tbl_StockInput.In_Index , tbl_StockInput.In_Name , tbl_StockInput.In_Date , tbl_StockInput.In_C_Code   ";
            Tsql = Tsql + " ,Isnull(St_Bus.Name,'') as St_B_Name";
            Tsql = Tsql + " From tbl_StockInput (nolock)  ";
            Tsql = Tsql + " LEFT JOIN tbl_Business St_Bus (nolock) ON tbl_StockInput.In_C_Code = St_Bus.NCode ";
            Tsql = Tsql + " Where tbl_StockInput.OrderNumber ='" + OrderNumber + "'";

            Tsql = Tsql + " Order by tbl_StockInput.In_Index DESC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                Center_N = ds.Tables["tbl_StockOutput"].Rows[0]["St_B_Name"].ToString();
                Center_Code = ds.Tables["tbl_StockOutput"].Rows[0]["In_C_Code"].ToString();
                U_ID = ds.Tables["tbl_StockOutput"].Rows[0]["In_Name"].ToString();
                in_Date = ds.Tables["tbl_StockOutput"].Rows[0]["In_Date"].ToString();
                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }


        public Boolean Check_Stock_Output_Rece(string OrderNumber, int RecIndex)
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql = "";
            Tsql = " Select ISNULL(SUM(ISNULL(tbl_StockOutput.ItemCount, 0)), 0) Cnt From tbl_Sales_Rece (nolock) ";
            Tsql = Tsql + " Left Outer Join tbl_StockOutput (nolock) on tbl_Sales_Rece.OrderNumber = tbl_StockOutput.OrderNumber And tbl_Sales_Rece.SalesItemIndex = tbl_StockOutput.Salesitemindex ";
            Tsql = Tsql + " Where tbl_Sales_Rece.OrderNumber = '" + OrderNumber + "' And tbl_Sales_Rece.RecIndex = " + RecIndex;
            Tsql = Tsql + " And tbl_StockOutput.Out_Index not in (Select Out_Index From tbl_StockOutput_Not_Union (nolock) ) ";

            DataSet ds = new DataSet();
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {
                if (double.Parse(ds.Tables["tbl_StockOutput"].Rows[0]["Cnt"].ToString()) > 0)
                {
                    MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Stock_Out")
                                + "\n" +
                                cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }


        public Boolean Check_Stock_OutPut(string OrderNumber, int SalesItemIndex )
        {
            

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT Out_index  From tbl_StockOutput (nolock)  ";
            Tsql = Tsql + " Where OrderNumber ='" + OrderNumber +  "'" ;
            Tsql = Tsql + " And   SalesItemIndex =" + SalesItemIndex  ;
            Tsql = Tsql + " And tbl_StockOutput.Out_Index not in (Select Out_Index From tbl_StockOutput_Not_Union (nolock) )  ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Stock_Out")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }

        public Boolean Check_Stock_RealCnt(string CenterCode, string ItemCode, int SellCnt)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = string.Format("EXEC Usp_Jde_StockCheck '{0}', '{1}', {2}, ''", CenterCode, ItemCode, SellCnt)
;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                bool bNonExistsStockCount = false;
                string ItemCodes = string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if(row["StockYN"].ToString().Equals("N"))
                    {

                        bNonExistsStockCount = true;
                        if (ItemCodes == string.Empty)
                            ItemCodes = row["ItemCode"].ToString();
                        else
                            ItemCodes += "," + row["ItemCode"].ToString();

                    }

                }
                if (bNonExistsStockCount)
                {
                    MessageBox.Show("상품코드 : " + ItemCodes + Environment.NewLine +
                        "ProductInventory Table 내 재고가 없다고 확인됩니다." + Environment.NewLine +
                        "해당상품을 추가할 수 없습니다.");
                    return false;
                }

                //재고있음
                return true;
            }
            else
            {
                MessageBox.Show("ProductInventory Table 내 ItemCode가 조회되지않습니다.");
                return false;  //한명도 없으면 true
            }//++++++++++++++++++++++++++++++++
        }


        public Boolean Check_Stock_OutPut(string OrderNumber)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT Out_index  From tbl_StockOutput (nolock)  ";
            Tsql = Tsql + " Where OrderNumber ='" + OrderNumber + "'";
            Tsql = Tsql + " And ItemCount >  0 ";
            Tsql = Tsql + " And tbl_StockOutput.Out_Index Not in (Select Out_Index From tbl_StockOutput_Not_Union (nolock) ) ";
            
            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Stock_Out")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }



        public Boolean Check_Stock_OutPut(string OrderNumber, int SalesItemIndex, int TT)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT Out_index  From tbl_StockOutput (nolock)  ";
            Tsql = Tsql + " Where OrderNumber ='" + OrderNumber + "'";
            Tsql = Tsql + " And   SalesItemIndex =" + SalesItemIndex;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }



        public Boolean Check_Stock_INPut(string OrderNumber, int SalesItemIndex)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT In_Index  From tbl_StockInput (nolock)  ";
            Tsql = Tsql + " Where OrderNumber ='" + OrderNumber + "'";
            Tsql = Tsql + " And   SalesItemIndex =" + SalesItemIndex;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Stock_IN")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }


        public Boolean Check_Stock_INPut(string OrderNumber)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT In_Index  From tbl_StockInput (nolock)  ";
            Tsql = Tsql + " Where OrderNumber ='" + OrderNumber + "'";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_StockOutput", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Stock_IN")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }




        public Boolean Check_Stock_Close(string CenterCode, string StockDay)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT StockDay , CenterCode From DayStock (nolock)  ";
            Tsql = Tsql + " Where   CenterCode = '" + CenterCode + "'";
            Tsql = Tsql + " And replace(StockDay,'-','') >= '" + StockDay + "'";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "DayStock", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Not_Close_Date")
                              + "\n" +
                              cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));

                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }

        public Boolean Check_Stock_Close(string CenterCode, string StockDay, int i)
        {
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "SELECT StockDay , CenterCode From DayStock (nolock)  ";
            Tsql = Tsql + " Where   CenterCode = '" + CenterCode + "'";
            Tsql = Tsql + " And replace(StockDay,'-','') >= '" + StockDay + "'";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "DayStock", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)  //한건이라도 있으면 마감이 돌았음 그럼 안됨
            {
                return false; //한건이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }


        public Boolean Member_Multi_Sn_Search(string Search_Member_Sn)
        {
            //StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

            //string t_Sn = Search_Member_Sn.ToString().Replace("-","");
            //t_Sn = t_Sn.Replace(" ", "").Replace("_", "");

            //if (t_Sn == "") return true; //주민번호가 없는 경우네는 걍 한명도 없는 걸로 보고 True를 던짐.

            // //++++++++++++++++++++++++++++++++
            //cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            //string Tsql;
            //Tsql = "Select Mbid + '-' + Convert(Varchar, Mbid2 ) AS M_Mbid , M_Name  ";
            //Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            //Tsql = Tsql + " Where Cpno = '" + encrypter.Encrypt (t_Sn) + "'";
            //Tsql = Tsql + " And LeaveDate = '' "; //탈퇴지되지 않은 사람들 중에서 동일 주민번호가 있는지를 체크한다.
            //Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            //DataSet ds = new DataSet();
            ////테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            //if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            //int ReCnt = Temp_Connect.DataSet_ReCount;

            //if (ReCnt > 0)
            //    return false; //한명이라도 있으면 False
            //else
            //    return true;  //한명도 없으면 true
            ////++++++++++++++++++++++++++++++++




            StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);
            string t_Sn = Search_Member_Sn.ToString().Replace("-", "");
            t_Sn = t_Sn.Replace(" ", "").Replace("_", "");
            Boolean TSW = true;
            string LeaveDate = "";

            if (t_Sn == "") return true; //주민번호가 없는 경우네는 걍 한명도 없는 걸로 보고 True를 던짐.

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Top 1 Mbid + '-' + Convert(Varchar, Mbid2 ) AS M_Mbid , M_Name , LeaveDate, LeaveCheck   ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            Tsql = Tsql + " Where Cpno = '" + encrypter.Encrypt(t_Sn) + "'";
            Tsql = Tsql + " Order by RecordTime DESC  ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {
                TSW = false; //탈퇴일자가 없는 회원이면.. 당연히 존재하기 때문에  가입 못하게 한ㄴ다.

                if (ds.Tables["tbl_Memberinfo"].Rows[0]["LeaveCheck"].ToString() != "1")
                {
                    LeaveDate = ds.Tables["tbl_Memberinfo"].Rows[0]["LeaveDate"].ToString();
                    int LeaveCheck = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0]["LeaveCheck"].ToString());

                    string PayDate = LeaveDate.Substring(0, 4) + '-' + LeaveDate.Substring(4, 2) + '-' + LeaveDate.Substring(6, 2);
                    DateTime TodayDate = new DateTime();
                    TodayDate = DateTime.Parse(PayDate);
                    PayDate = TodayDate.AddMonths(6).ToString("yyyy-MM-dd").Replace("-", "").Replace("/", "");

                    //if (int.Parse(cls_User.gid_date_time) > int.Parse(PayDate) || LeaveCheck != 0)

                    //admin 은 탈퇴후 6개월이랑 상관없이 가입가능하게 설정 변경 2017-03-15 김종국 이사 요청에 의해서 
                    if (int.Parse(cls_User.gid_date_time) > int.Parse(PayDate)  || cls_User.gid == "admin" )
                        TSW = true; //6개월 이후에나 가입이 가능하다.
                    else
                    {
                        TSW = false;

                        MessageBox.Show(cls_app_static_var.app_msg_rm.GetString("Msg_Leave_6")
                               + "\n" +
                               cls_app_static_var.app_msg_rm.GetString("Msg_Re_Action"));
                    }
                }

                return TSW; //한명이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }


        public Boolean Member_Multi_Sn_Search(string Search_Member_Sn, string  Mbid ,int Mbid2)
        {
            StringEncrypter encrypter = new StringEncrypter(cls_User.con_EncryptKey, cls_User.con_EncryptKeyIV);

            string t_Sn = Search_Member_Sn.ToString().Replace("-", "");
            t_Sn = t_Sn.Replace(" ", "").Replace("_", "");

            if (t_Sn == "") return true; //주민번호가 없는 경우네는 걍 한명도 없는 걸로 보고 True를 던짐.

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select Mbid + '-' + Convert(Varchar, Mbid2 ) AS M_Mbid , M_Name  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            Tsql = Tsql + " Where Cpno = '" + encrypter.Encrypt (t_Sn) + "'";
            //if (Mbid.Length == 0)
            //    Tsql = Tsql + " And Mbid2 = " + Mbid2.ToString();
            //else
            //{
            //    Tsql = Tsql + " And Mbid = '" + Mbid + "' ";
            //    Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            //}

            Tsql = Tsql + " And LeaveDate = '' "; //탈퇴지되지 않은 사람들 중에서 동일 주민번호가 있는지를 체크한다.
            Tsql = Tsql + " Order by Mbid, Mbid2 ASC ";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {
                string T_Mbid = "";
                T_Mbid = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();
                if (T_Mbid == (Mbid + "-" + Mbid2.ToString()))
                    return true;
                else
                    return false; //한명이라도 있으면 False
            }
            else
                return true;  //한명도 없으면 true
            //++++++++++++++++++++++++++++++++
        }





        public Boolean SalesDetail_Mod_BackUp(string OrderNumber, string baseTableName, string Where_sql = "")
        {

            Be_Memberinfo.Clear();

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            Tsql = Tsql + " Where OrderNumber = '" + OrderNumber + "' ";
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;

            for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
            {
                Be_Memberinfo[i] = ds.Tables[baseTableName].Rows[0][i].ToString();
            }

            return true;
        }




        public Boolean SalesDetail_Mod(SqlConnection Conn, SqlTransaction tran , string OrderNumber, string baseTableName, string Where_sql = "")
        {
            if (Be_Memberinfo.Count <= 0) return true;
           
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            Tsql = Tsql + " Where OrderNumber = '" + OrderNumber + "' ";            
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";

            string newStr = "";
            for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
            {
                newStr = ds.Tables[baseTableName].Rows[0][i].ToString();
                if (Be_Memberinfo[i] != newStr)
                {
                    Fild_Name =  ds.Tables[baseTableName].Columns[i].ColumnName;
                    string AfterDetail = ds.Tables[baseTableName].Rows[0][i].ToString();
                    string StrSql = "";
                    StrSql = "insert into tbl_SalesDetail_Change   ";
                    StrSql = StrSql + " (";
                    StrSql = StrSql + " OrderNumber ";
                    StrSql = StrSql + ", ChangeDetail ";
                    StrSql = StrSql + ", BeforeDetail ";
                    StrSql = StrSql + ", AfterDetail ";
                    StrSql = StrSql + ", ModRecordid ";
                    StrSql = StrSql + ", ModRecordtime ";
                    StrSql = StrSql + " ) Values (";
                    StrSql = StrSql + "'" + OrderNumber + "'";                    
                    StrSql = StrSql + ", '" + Fild_Name + "'";
                    StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                    StrSql = StrSql + ", '" + AfterDetail + "'";
                    StrSql = StrSql + ",'" + cls_User.gid + "'";
                    StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                    StrSql = StrSql + ")";

                    Temp_Connect.Insert_Data(StrSql, baseTableName, Conn, tran);
                }
            }

              return true;
        }


        public Boolean SalesDetail_Mod( string OrderNumber, string baseTableName, string Where_sql = "")
        {
            if (Be_Memberinfo.Count <= 0) return true;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            Tsql = Tsql + " Where OrderNumber = '" + OrderNumber + "' ";
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";

            string newStr = "";
            for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
            {
                newStr = ds.Tables[baseTableName].Rows[0][i].ToString();
                if (Be_Memberinfo[i] != newStr)
                {
                    Fild_Name = ds.Tables[baseTableName].Columns[i].ColumnName;
                    string AfterDetail = ds.Tables[baseTableName].Rows[0][i].ToString();
                    string StrSql = "";
                    StrSql = "insert into tbl_SalesDetail_Change   ";
                    StrSql = StrSql + " (";
                    StrSql = StrSql + " OrderNumber ";
                    StrSql = StrSql + ", ChangeDetail ";
                    StrSql = StrSql + ", BeforeDetail ";
                    StrSql = StrSql + ", AfterDetail ";
                    StrSql = StrSql + ", ModRecordid ";
                    StrSql = StrSql + ", ModRecordtime ";
                    StrSql = StrSql + " ) Values (";
                    StrSql = StrSql + "'" + OrderNumber + "'";
                    StrSql = StrSql + ", '" + Fild_Name + "'";
                    StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                    StrSql = StrSql + ", '" + AfterDetail + "'";
                    StrSql = StrSql + ",'" + cls_User.gid + "'";
                    StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                    StrSql = StrSql + ")";

                    Temp_Connect.Insert_Data(StrSql, baseTableName);
                }
            }

            return true;
        }

        public Boolean tbl_SalesDetail_Total_Change(SqlConnection Conn, SqlTransaction tran, string OrderNumber, int Sales_T_Index, string baseTableName, string Where_sql = "")
        {
            if (Be_Memberinfo.Count <= 0) return true;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();

            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            Tsql = Tsql + " Where OrderNumber = '" + OrderNumber + "' ";
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";

            string newStr = "";
            for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
            {
                newStr = ds.Tables[baseTableName].Rows[0][i].ToString();
                if (Be_Memberinfo[i] != newStr)
                {
                    Fild_Name = ds.Tables[baseTableName].Columns[i].ColumnName;
                    string AfterDetail = ds.Tables[baseTableName].Rows[0][i].ToString();
                    string StrSql = "";
                    StrSql = "insert into tbl_SalesDetail_Total_Change   ";
                    StrSql = StrSql + " (";
                    StrSql = StrSql + " OrderNumber ";
                    StrSql = StrSql + ",Sales_T_Index ";
                    StrSql = StrSql + ",Kind_TF ";                    
                    StrSql = StrSql + ", ChangeDetail ";
                    StrSql = StrSql + ", BeforeDetail ";
                    StrSql = StrSql + ", AfterDetail ";
                    StrSql = StrSql + ", ModRecordid ";
                    StrSql = StrSql + ", ModRecordtime ";
                    StrSql = StrSql + " ) Values (";
                    StrSql = StrSql + "'" + OrderNumber + "'";
                    StrSql = StrSql + "," + Sales_T_Index;

                    if (baseTableName == "tbl_Sales_Cacu")
                        StrSql = StrSql + ", 2 ";
                    else if (baseTableName == "tbl_Sales_Rece")
                        StrSql = StrSql + ", 3 ";
                    else if (baseTableName == "tbl_SalesItemDetail")
                        StrSql = StrSql + ", 1 ";
                    else
                        StrSql = StrSql + ", 1 ";

                    StrSql = StrSql + ", '" + Fild_Name + "'";
                    StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                    StrSql = StrSql + ", '" + AfterDetail + "'";
                    StrSql = StrSql + ",'" + cls_User.gid + "'";
                    StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                    StrSql = StrSql + ")";

                    Temp_Connect.Insert_Data(StrSql, baseTableName, Conn, tran);
                }
            }

            return true;
        }



        public Boolean Member_Mod_BackUp (string Search_Member, string baseTableName , string Where_sql ="")
        {
            string Mbid = ""; int Mbid2 = 0;
            Be_Memberinfo.Clear();

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false ;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false ;

            for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
            {
                Be_Memberinfo[i] = ds.Tables[baseTableName].Rows[0][i].ToString();
            }

            return true;
        }



        public Boolean Member_Mod_BackUp(string Mbid, int Mbid2, string baseTableName, string Where_sql = "")
        {
            
            Be_Memberinfo.Clear();
            
            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;

            for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
            {
                Be_Memberinfo[i] = ds.Tables[baseTableName].Rows[0][i].ToString();
            }

            return true;
        }



        public Boolean tbl_Memberinfo_Mod(string Search_Member, string T_ETC = "")
        {
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";
            int SSW = 0; int NSW = 0;

            
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string newStr = "";
                for (int i = 0; i < ds.Tables["tbl_Memberinfo"].Columns.Count; i++)
                {
                    newStr = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                    if (Be_Memberinfo[i] !=newStr )
                    {
                        Fild_Name = ds.Tables["tbl_Memberinfo"].Columns[i].ColumnName;
                        string AfterDetail = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                        string StrSql = "";
                        if (Fild_Name.ToUpper() == "bankaccnt")
                        {
                            StrSql = "insert into tbl_Memberinfo_Mod ";
                            StrSql = StrSql + " (";
                            StrSql = StrSql + " mbid, mbid2 ";
                            StrSql = StrSql + ", ChangeDetail ";
                            StrSql = StrSql + ", BeforeDetail ";
                            StrSql = StrSql + ", AfterDetail ";
                            StrSql = StrSql + ", ModRecordid ";
                            StrSql = StrSql + ", ModRecordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "'";
                            StrSql = StrSql + "," + Mbid2.ToString();
                            StrSql = StrSql + ", '" + Fild_Name + "'";
                            StrSql = StrSql + ", dbo.DECRYPT_AES256('" + Be_Memberinfo[i] + "')";
                            StrSql = StrSql + ", dbo.DECRYPT_AES256('" + AfterDetail + "')";
                            StrSql = StrSql + ",'" + cls_User.gid + "'";
                            StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                            StrSql = StrSql + ")";
                        }
                       
                        StrSql = "insert into tbl_Memberinfo_Mod ";
                        StrSql = StrSql + " (";
                        StrSql = StrSql + " mbid, mbid2 ";
                        StrSql = StrSql + ", ChangeDetail ";
                        StrSql = StrSql + ", BeforeDetail ";
                        StrSql = StrSql + ", AfterDetail ";
                        StrSql = StrSql + ", ModRecordid ";
                        StrSql = StrSql + ", ModRecordtime ";
                        StrSql = StrSql + " ) Values (";
                        StrSql = StrSql + "'" + Mbid + "'";
                        StrSql = StrSql + "," + Mbid2.ToString();
                        StrSql = StrSql + ", '" + Fild_Name + "'";
                        StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                        StrSql = StrSql + ", '" + AfterDetail + "'";
                        StrSql = StrSql + ",'" + cls_User.gid + "'";
                        StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                        StrSql = StrSql + ")";

                        Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);
                        //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;

                        if (((Fild_Name.ToUpper() == "SAVEID") || (Fild_Name.ToUpper() == "SAVEID2")) && (SSW == 0))
                        {
                            string Old_mbid = ""; int Old_mbid2 = 0;
                            string New_mbid = ""; int New_mbid2 = 0;

                            if (Fild_Name.ToUpper() == "SAVEID")
                            {
                                Old_mbid = Be_Memberinfo[i];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i + 1]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i + 1].ToString());
                            }
                            else
                            {
                                Old_mbid = Be_Memberinfo[i - 1];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i - 1].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString());
                            }

                            StrSql = "";
                            StrSql = "INSERT INTO tbl_Memberinfo_Save_Nomin_Change ";
                            StrSql = StrSql + " (mbid, mbid2, Old_mbid, Old_mbid2, New_mbid, New_mbid2,Save_Nomin_SW,Remark,Recordid, Recordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "', " + Mbid2 + ", '";
                            StrSql = StrSql + Old_mbid + "'," + Old_mbid2 + ",'" + New_mbid + "',";
                            StrSql = StrSql + New_mbid2 + ",'SAV','" + T_ETC + "','" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21))";

                            //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;
                            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);

                            SSW = 1;
                        }


                        if (((Fild_Name.ToUpper() == "NOMINID") || (Fild_Name.ToUpper() == "NOMINID2")) && (NSW == 0))
                        {
                            string Old_mbid = ""; int Old_mbid2 = 0;
                            string New_mbid = ""; int New_mbid2 = 0;

                            if (Fild_Name.ToUpper() == "NOMINID")
                            {
                                Old_mbid = Be_Memberinfo[i];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i + 1]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i + 1].ToString());
                            }
                            else
                            {
                                Old_mbid = Be_Memberinfo[i - 1];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i - 1].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString());
                            }

                            StrSql = "";
                            StrSql = "INSERT INTO tbl_Memberinfo_Save_Nomin_Change ";
                            StrSql = StrSql + " (mbid, mbid2, Old_mbid, Old_mbid2, New_mbid, New_mbid2,Save_Nomin_SW,Remark,Recordid, Recordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "', " + Mbid2 + ", '";
                            StrSql = StrSql + Old_mbid + "'," + Old_mbid2 + ",'" + New_mbid + "',";
                            StrSql = StrSql + New_mbid2 + ",'NOM','" + T_ETC + "','" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21))";

                            //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;
                            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);

                            NSW = 1;
                        }

                    }
                }


                tran.Commit();
                return true;

            }
            catch (Exception)
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


        public Boolean tbl_Memberinfo_Mod_JDE(string Search_Member, string T_ETC = "")
        {
            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;


            cls_Connect_DB Temp_Connect2 = new cls_Connect_DB();


            string Tsql2;
            Tsql2 = "Select MAX(IDX_MEMBER)  ";
            Tsql2 = Tsql2 + " From tbl_Memberinfo_Mod_JDE  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql2 = Tsql2 + " Where Mbid2 = " + Mbid2.ToString();
         

            DataSet ds2 = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql2, "tbl_Memberinfo_Mod_JDE", ds2) == false) return false;
            String idx_member = ds2.Tables["tbl_Memberinfo_Mod_JDE"].Rows[0][0].ToString();


            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";
            string Fild_Name_JDE = "";
            int SSW = 0; int NSW = 0;


            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string newStr = "";
                for (int i = 0; i < ds.Tables["tbl_Memberinfo"].Columns.Count; i++)
                {
                    newStr = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                    if (Be_Memberinfo[i] != newStr)
                    {

                        Fild_Name_JDE = ds.Tables["tbl_Memberinfo"].Columns[i].ColumnName;
                        string AfterDetail_JDE = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                        string StrSql_JDE = "";
                        StrSql_JDE = "update tbl_Memberinfo_Mod_JDE ";
                        StrSql_JDE = StrSql_JDE + "SET " + Fild_Name_JDE + " = '" + AfterDetail_JDE + "'";
                        StrSql_JDE = StrSql_JDE + "WHERE Mbid2 = '" + Mbid2 + "'";
                        StrSql_JDE = StrSql_JDE + "and  idx_member = " + idx_member + "";


                        Temp_Connect.Insert_Data(StrSql_JDE, "tbl_Memberinfo_Mod_JDE", Conn, tran);


                        Fild_Name = ds.Tables["tbl_Memberinfo"].Columns[i].ColumnName;
                        string AfterDetail = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                        string StrSql = "";
                        StrSql = "insert into tbl_Memberinfo_Mod ";
                        StrSql = StrSql + " (";
                        StrSql = StrSql + " mbid, mbid2 ";
                        StrSql = StrSql + ", ChangeDetail ";
                        StrSql = StrSql + ", BeforeDetail ";
                        StrSql = StrSql + ", AfterDetail ";
                        StrSql = StrSql + ", ModRecordid ";
                        StrSql = StrSql + ", ModRecordtime ";
                        StrSql = StrSql + " ) Values (";
                        StrSql = StrSql + "'" + Mbid + "'";
                        StrSql = StrSql + "," + Mbid2.ToString();
                        StrSql = StrSql + ", '" + Fild_Name + "'";
                        StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                        StrSql = StrSql + ", '" + AfterDetail + "'";
                        StrSql = StrSql + ",'" + cls_User.gid + "'";
                        StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                        StrSql = StrSql + ")";

                        Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);
                        //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;

                        if (((Fild_Name.ToUpper() == "SAVEID") || (Fild_Name.ToUpper() == "SAVEID2")) && (SSW == 0))
                        {
                            string Old_mbid = ""; int Old_mbid2 = 0;
                            string New_mbid = ""; int New_mbid2 = 0;

                            if (Fild_Name.ToUpper() == "SAVEID")
                            {
                                Old_mbid = Be_Memberinfo[i];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i + 1]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i + 1].ToString());
                            }
                            else
                            {
                                Old_mbid = Be_Memberinfo[i - 1];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i - 1].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString());
                            }

                            StrSql = "";
                            StrSql = "INSERT INTO tbl_Memberinfo_Save_Nomin_Change ";
                            StrSql = StrSql + " (mbid, mbid2, Old_mbid, Old_mbid2, New_mbid, New_mbid2,Save_Nomin_SW,Remark,Recordid, Recordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "', " + Mbid2 + ", '";
                            StrSql = StrSql + Old_mbid + "'," + Old_mbid2 + ",'" + New_mbid + "',";
                            StrSql = StrSql + New_mbid2 + ",'SAV','" + T_ETC + "','" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21))";

                            //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;
                            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);

                            SSW = 1;
                        }


                        if (((Fild_Name.ToUpper() == "NOMINID") || (Fild_Name.ToUpper() == "NOMINID2")) && (NSW == 0))
                        {
                            string Old_mbid = ""; int Old_mbid2 = 0;
                            string New_mbid = ""; int New_mbid2 = 0;

                            if (Fild_Name.ToUpper() == "NOMINID")
                            {
                                Old_mbid = Be_Memberinfo[i];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i + 1]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i + 1].ToString());
                            }
                            else
                            {
                                Old_mbid = Be_Memberinfo[i - 1];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i - 1].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString());
                            }

                            StrSql = "";
                            StrSql = "INSERT INTO tbl_Memberinfo_Save_Nomin_Change ";
                            StrSql = StrSql + " (mbid, mbid2, Old_mbid, Old_mbid2, New_mbid, New_mbid2,Save_Nomin_SW,Remark,Recordid, Recordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "', " + Mbid2 + ", '";
                            StrSql = StrSql + Old_mbid + "'," + Old_mbid2 + ",'" + New_mbid + "',";
                            StrSql = StrSql + New_mbid2 + ",'NOM','" + T_ETC + "','" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21))";

                            //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;
                            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);

                            NSW = 1;
                        }

                    }
                }
                //string StrSql_JDE_PROCEDUER = "";
                //StrSql_JDE_PROCEDUER = "EXEC  Usp_Insert_Tbl_Memberinfo_JDE '" + Mbid2 + "','U','" + idx_member + "'";
                //Temp_Connect.Insert_Data(StrSql_JDE_PROCEDUER, "tbl_Memberinfo_Mod_JDE", Conn, tran);

                tran.Commit();
                return true;

            }
            catch (Exception)
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

        public Boolean tbl_Memberinfo_Mod(string Mbid, int Mbid2, string T_ETC = "")
        {
          //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From tbl_Memberinfo  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";
            int SSW = 0; int NSW = 0;


            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string newStr = "";
                for (int i = 0; i < ds.Tables["tbl_Memberinfo"].Columns.Count; i++)
                {
                    newStr = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                    if (Be_Memberinfo[i] != newStr)
                    {
                        Fild_Name = ds.Tables["tbl_Memberinfo"].Columns[i].ColumnName;
                        string AfterDetail = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                        string StrSql = "";
                        StrSql = "insert into tbl_Memberinfo_Mod ";
                        StrSql = StrSql + " (";
                        StrSql = StrSql + " mbid, mbid2 ";
                        StrSql = StrSql + ", ChangeDetail ";
                        StrSql = StrSql + ", BeforeDetail ";
                        StrSql = StrSql + ", AfterDetail ";
                        StrSql = StrSql + ", ModRecordid ";
                        StrSql = StrSql + ", ModRecordtime ";
                        StrSql = StrSql + " ) Values (";
                        StrSql = StrSql + "'" + Mbid + "'";
                        StrSql = StrSql + "," + Mbid2.ToString();
                        StrSql = StrSql + ", '" + Fild_Name + "'";
                        StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                        StrSql = StrSql + ", '" + AfterDetail + "'";
                        StrSql = StrSql + ",'" + cls_User.gid + "'";
                        StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                        StrSql = StrSql + ")";

                        Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);
                        //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;

                        if (((Fild_Name.ToUpper() == "SAVEID") || (Fild_Name.ToUpper() == "SAVEID2")) && (SSW == 0))
                        {
                            string Old_mbid = ""; int Old_mbid2 = 0;
                            string New_mbid = ""; int New_mbid2 = 0;

                            if (Fild_Name.ToUpper() == "SAVEID")
                            {
                                Old_mbid = Be_Memberinfo[i];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i + 1]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i + 1].ToString());
                            }
                            else
                            {
                                Old_mbid = Be_Memberinfo[i - 1];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i - 1].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString());
                            }

                            StrSql = "";
                            StrSql = "INSERT INTO tbl_Memberinfo_Save_Nomin_Change ";
                            StrSql = StrSql + " (mbid, mbid2, Old_mbid, Old_mbid2, New_mbid, New_mbid2,Save_Nomin_SW,Remark,Recordid, Recordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "', " + Mbid2 + ", '";
                            StrSql = StrSql + Old_mbid + "'," + Old_mbid2 + ",'" + New_mbid + "',";
                            StrSql = StrSql + New_mbid2 + ",'SAV','" + T_ETC + "','" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21))";

                            //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;
                            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);

                            SSW = 1;
                        }


                        if (((Fild_Name.ToUpper() == "NOMINID") || (Fild_Name.ToUpper() == "NOMINID2")) && (NSW == 0))
                        {
                            string Old_mbid = ""; int Old_mbid2 = 0;
                            string New_mbid = ""; int New_mbid2 = 0;

                            if (Fild_Name.ToUpper() == "NOMINID")
                            {
                                Old_mbid = Be_Memberinfo[i];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i + 1]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i + 1].ToString());
                            }
                            else
                            {
                                Old_mbid = Be_Memberinfo[i - 1];
                                Old_mbid2 = int.Parse(Be_Memberinfo[i]);
                                New_mbid = ds.Tables["tbl_Memberinfo"].Rows[0][i - 1].ToString();
                                New_mbid2 = int.Parse(ds.Tables["tbl_Memberinfo"].Rows[0][i].ToString());
                            }

                            StrSql = "";
                            StrSql = "INSERT INTO tbl_Memberinfo_Save_Nomin_Change ";
                            StrSql = StrSql + " (mbid, mbid2, Old_mbid, Old_mbid2, New_mbid, New_mbid2,Save_Nomin_SW,Remark,Recordid, Recordtime ";
                            StrSql = StrSql + " ) Values (";
                            StrSql = StrSql + "'" + Mbid + "', " + Mbid2 + ", '";
                            StrSql = StrSql + Old_mbid + "'," + Old_mbid2 + ",'" + New_mbid + "',";
                            StrSql = StrSql + New_mbid2 + ",'NOM','" + T_ETC + "','" + cls_User.gid + "', Convert(Varchar(25),GetDate(),21))";

                            //if (Temp_Connect.Insert_Data(Tsql, "tbl_Memberinfo") == false) return false;
                            Temp_Connect.Insert_Data(StrSql, "tbl_Memberinfo", Conn, tran);

                            NSW = 1;
                        }

                    }
                }


                tran.Commit();
                return true;

            }
            catch (Exception)
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


        public Boolean tbl_Memberinfo_Mod(string Search_Member, string Sort_Add, string baseTableName, string Where_sql = "")
        {
            if (Be_Memberinfo.Count <= 0) return true;

            string Mbid = ""; int Mbid2 = 0;

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return false;

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();


            string Tsql;
            Tsql = "Select *  ";
            Tsql = Tsql + " From " + baseTableName + "  (nolock)  ";
            if (Mbid.Length == 0)
                Tsql = Tsql + " Where Mbid2 = " + Mbid2.ToString();
            else
            {
                Tsql = Tsql + " Where Mbid = '" + Mbid + "' ";
                Tsql = Tsql + " And   Mbid2 = " + Mbid2.ToString();
            }
            Tsql = Tsql + Where_sql;

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, baseTableName, ds) == false) return false;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0) return false;
            string Fild_Name = "";
            
            Temp_Connect.Connect_DB();
            SqlConnection Conn = Temp_Connect.Conn_Conn();
            SqlTransaction tran = Conn.BeginTransaction();

            try
            {
                string newStr = "";
                for (int i = 0; i < ds.Tables[baseTableName].Columns.Count; i++)
                {
                    newStr = ds.Tables[baseTableName].Rows[0][i].ToString();
                    if (Be_Memberinfo[i] != newStr)
                    {
                        Fild_Name = Sort_Add + '-' + ds.Tables[baseTableName].Columns[i].ColumnName;
                        string AfterDetail = ds.Tables[baseTableName].Rows[0][i].ToString();
                        string StrSql = "";
                        StrSql = "insert into tbl_Memberinfo_Mod ";
                        StrSql = StrSql + " (";
                        StrSql = StrSql + " mbid, mbid2 ";
                        StrSql = StrSql + ", ChangeDetail ";
                        StrSql = StrSql + ", BeforeDetail ";
                        StrSql = StrSql + ", AfterDetail ";
                        StrSql = StrSql + ", ModRecordid ";
                        StrSql = StrSql + ", ModRecordtime ";
                        StrSql = StrSql + " ) Values (";
                        StrSql = StrSql + "'" + Mbid + "'";
                        StrSql = StrSql + "," + Mbid2.ToString();
                        StrSql = StrSql + ", '" + Fild_Name + "'";
                        StrSql = StrSql + ", '" + Be_Memberinfo[i] + "'";
                        StrSql = StrSql + ", '" + AfterDetail + "'";
                        StrSql = StrSql + ",'" + cls_User.gid + "'";
                        StrSql = StrSql + ", Convert(Varchar(25),GetDate(),21) ";
                        StrSql = StrSql + ")";

                        Temp_Connect.Insert_Data(StrSql, baseTableName, Conn, tran);
                    }
                }


                tran.Commit();
                return true;

            }
            catch (Exception)
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




        public int Member_Name_Search_Mem(string Search_Member, ref string Search_Name)
        {
            Search_Name = "";
            string Mbid = ""; int Mbid2 = 0;

            if (Search_Member.Trim() == "") return -1;
            Mbid = Search_Member.Trim();

            //회원번호 불리시 잘못된 거면 오류로 해서 팅겨 버린다. -1을 리턴한다.
            //if (Member_Nmumber_Split(Search_Member.Trim(), ref Mbid, ref Mbid2) < 0) return -1;


            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            Tsql = "Select U_Name AS N_Name  ";
            Tsql = Tsql + " From tbl_User  (nolock)  ";
            Tsql = Tsql + " Where User_Ncode like '%" + Mbid.ToString() + "%' ";

            Tsql = Tsql + " And tbl_User.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";

            Tsql = Tsql + " Order by User_Ncode ";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Search_Name = "";
            else if (ReCnt == 1)
                Search_Name = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }


        public int Member_Name_Search_Mem(ref string Member_Number, string Search_Name)
        {

            //++++++++++++++++++++++++++++++++
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;

            Tsql = "Select tbl_User.User_Ncode  Ncode  ";
            Tsql = Tsql + " From tbl_User  (nolock)  ";
            Tsql = Tsql + " Where tbl_User.U_Name like '%" + Search_Name + "%' ";
            Tsql = Tsql + " And tbl_User.Na_Code in ( Select Na_Code From ufn_User_In_Na_Code ('" + cls_User.gid_CountryCode + "') )";


            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "tbl_Memberinfo", ds) == false) return -2;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt == 0)
                Member_Number = "";
            else if (ReCnt == 1)

                Member_Number = ds.Tables["tbl_Memberinfo"].Rows[0][0].ToString();

            return ReCnt;
            //++++++++++++++++++++++++++++++++
        }

        public string Select_Today(string Format)
        {
            cls_Connect_DB Temp_Connect = new cls_Connect_DB();
            string Tsql;
            string Today = "";

            Tsql = "SELECT CONVERT(VARCHAR(10), GETDATE(), 121)";

            DataSet ds = new DataSet();
            //테이블에 맞게  DataSet에 내역을 넣고 제대로되었으면 true가 오고 아니면 걍 튀어나간다.
            if (Temp_Connect.Open_Data_Set(Tsql, "Today", ds) == false) return Today;
            int ReCnt = Temp_Connect.DataSet_ReCount;

            if (ReCnt > 0)
            {
                DateTime DT = new DateTime();
                DT = DateTime.Parse(ds.Tables["Today"].Rows[0][0].ToString());

                Today = DT.ToString(Format);
            }

            return Today;
        }



    }// end cls_Search_DB

}
