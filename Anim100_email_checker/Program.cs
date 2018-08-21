using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Anim100_email_checker
{

    //select* FROM TRANSCRIPTDETAIL
    //WHERE ACADEMIC_YEAR = '2018' AND ACADEMIC_TERM = 'FALL' AND EVENT_ID = 'ANIM 100' AND ADD_DROP_WAIT = 'A' AND PEOPLE_CODE_ID NOT IN(SELECT PEOPLE_CODE_ID FROM TRANSCRIPTDETAIL WHERE ACADEMIC_YEAR = '2018' AND ACADEMIC_TERM = 'FALL' AND EVENT_ID = 'ANIM 100' AND ADD_DROP_WAIT = 'D')
    //order by PEOPLE_CODE_ID

    class Program
    {

        static void Main(string[] args)
        {
            List<string> list_email = new List<string>();

            StreamWriter writer = new StreamWriter(".\\output.txt");

            SqlConnection conn = new SqlConnection("Data Source=budb01;Initial Catalog=Campus8;Integrated Security=True");
            conn.Open();

            StreamReader file_rdr = new StreamReader(".\\anim100.csv");
            string line = file_rdr.ReadLine();
            while (line != null)
            {
                string[] substrings = line.Split(',');

                string people_code_id = substrings[2].Trim();

                string query_email = string.Format("SELECT EMAIL_ADDRESS FROM ADDRESSSCHEDULE WHERE PEOPLE_ORG_CODE_ID='{0}'", people_code_id);
                SqlCommand command_email = new SqlCommand(query_email, conn);
                SqlDataReader dr_email = command_email.ExecuteReader();

                list_email.Clear();
                while (dr_email.Read())
                {
                    string email_str = "";
                    try
                    {
                        email_str = dr_email.GetString(0);
                    }
                    catch (System.Data.SqlTypes.SqlNullValueException ex)
                    {
                        email_str = "";
                    }

                    if (email_str.Length > 0)
                        list_email.Add(email_str);
                }
                dr_email.Close();

                if (list_email.Count == 0)
                {
                    writer.WriteLine("PEOPLE_CODE_ID = {0} COULD NOT FIND email", people_code_id);
                }
                else if (list_email.Count >= 2)
                {
                    writer.WriteLine("PEOPLE_CODE_ID = {0} MORE THAN 1 email count = {1}", people_code_id, list_email.Count);
                    writer.WriteLine("************************************************");
                    for (int ii=0; ii < list_email.Count; ii++)
                    {
                        writer.WriteLine("{0}", list_email[ii]);
                    }
                }
                else
                {
                    if (list_email[0].ToLower().Contains("@woodbury.edu"))
                        writer.WriteLine("*****  GOOD  PEOPLE_CODE_ID = {0}  email={1}", people_code_id, list_email[0]);
                    else
                        writer.WriteLine("*****  NON_WOODBURY_EMAIL  PEOPLE_CODE_ID = {0} NOT woodbury.edu email", people_code_id);
                }

                line = file_rdr.ReadLine();
            }//while

            file_rdr.Close();
            writer.Close();

            Console.WriteLine("DONE");
        }//main
    }//program
}//namespace


