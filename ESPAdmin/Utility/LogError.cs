using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Configuration;
using System.Web;

namespace ESPAdmin.Utility
{
    public class LogError
    {
        public string ErrorLog(string RecordID, string RecordID2, string RecordID3, string Section, string Section2, string Section3, string Description, string Verbose, int Status)
        {
            try
            {
                int iS1 = Section.Length;
                int iS2 = Section2.Length;
                int iS3 = Section3.Length;
                int iD = Description.Length;
                int iV = Verbose.Length;

                if (iS1 > 300) { iS1 = 300; }
                if (iS2 > 50) { iS2 = 50; }
                if (iS3 > 50) { iS3 = 50; }
                if (iD > 300) { iD = 300; }
                if (iV > 300) { iV = 300; }

                string retVal = "";
                try
                {
                    //call logerror API here
                    Employer.Employer employer = new Employer.Employer();
                    retVal = employer.LogError(RecordID, RecordID2, RecordID3,
                                           Section.Substring(0, iS1),
                                           Section2.Substring(0, iS2),
                                           Section3.Substring(0, iS3),
                                           Description.Substring(0, iD),
                                           Verbose.Substring(0, iV),
                                           Status);
                }
                catch(Exception ex)
                {
                    LogWorker logworker = new LogWorker("ErrorLog", "ErrorLog", ex.Message.ToString());
                }

                string[] retValParser;
                retValParser = retVal.Split('~');
                if (retValParser.GetUpperBound(0) > 0)
                {
                    if (retValParser[0] != "0") { return retValParser[1]; }
                    else
                    {
                        //UserID | ClientID | EntityID
                        this.WriteToLog(RecordID + "|" + RecordID2 + "|" + RecordID3 + "\t" + Section + "\t" + Section2 + "\t" + Section3 + "\t" + Description + "\t" + Verbose);
                        return retValParser[1];
                    }
                }
                else { return retVal; }
            }
            catch (Exception ex)
            {
                this.WriteToLog(RecordID + "\t" + Section + "\t" + Section2 + "\t" + Section3 + "\t" + Description + "\t" + Verbose);
                LogWorker logworker = new LogWorker("LogError", "ErrorLog", ex.Message.ToString());
            }
            return "";
        }

        public void WriteToLog(string msg)
        {
            string rootPath = HttpContext.Current.Session["ApplicationServerMapPath"].ToString();
            if (rootPath.Trim() == "") { return; }

            string _stamp = DateTime.Now.ToString("yyyyMMdd");
            string logPath = rootPath + "\\App_Log";
            string logFile = "\\log_" + _stamp + ".txt";

            try
            {
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }

                //check the file
                FileStream fs = new FileStream(logPath + logFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter s = new StreamWriter(fs);
                s.Close();
                fs.Close();

                //log it
                FileStream fs1 = new FileStream(logPath + logFile, FileMode.Append, FileAccess.Write);
                StreamWriter s1 = new StreamWriter(fs1);
                s1.Write(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + "\t" + msg + System.Environment.NewLine);
                s1.Close();
                fs1.Close();
            }
            catch (Exception ex)
            {
                LogWorker logworker = new LogWorker("LogError", "WriteToLog", ex.Message.ToString());
            }
        }
    }
}