using System;
using System.Collections.Generic;

using System.Web;
using System.Security.Cryptography;
using System.Text;
using Jayrock.Json;


using System.Net;
using System.Net.Mail;



/// <summary>
/// Summary description for User
/// </summary>
public class User
{

    public static readonly string SystemMailAccount = System.Configuration.ConfigurationManager.AppSettings["SystemMailAccount"];
    public static readonly string SystemMailDisplayName = System.Configuration.ConfigurationManager.AppSettings["SystemMailDisplayName"];
    public static readonly string SystemMailPassword = System.Configuration.ConfigurationManager.AppSettings["SystemMailPassword"];
    public static string SupervisorMailAccount = "";
    public static string SupervisorMailDisplayName = "";


    //object for DB access
    private DatabaseAccess dbAccess = new DatabaseAccess();

    #region Auto Login

    public UserInfo login(UserInfo useInfo)
    {
        //create database access object 
        string SID = useInfo.SID;
        String sql = string.Format("SELECT * FROM [User] where SID='{0}' ",
                                    SID);

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows.Count > 0)
            {
                useInfo.ID = Convert.ToInt16(dt.Rows[0]["ID"]);
                useInfo.UserGroup = dt.Rows[0]["UserGroup"].ToString();
                useInfo.LastLoginDateTime = Convert.ToDateTime(dt.Rows[0]["LoginDateTime"]);


                sql = "UPDATE [User] "
                                    + "SET [SID] = @SID "
                                    + ",[Name] = @Name "
                                    + ",[FullName] = @FullName "
                                    + ",[Email] = @Email "
                                    + ",[LoginDateTime] = @LoginDateTime "
                                    + ",[LastLoginDateTime] = @LastLoginDateTime "
                                    + "WHERE ID = @ID ";
                dict.Clear();
                dict.Add("@SID", useInfo.SID);
                dict.Add("@Name", useInfo.Name);
                dict.Add("@FullName", useInfo.FullName);
                dict.Add("@Email", useInfo.Email);
                dict.Add("@LoginDateTime", useInfo.LoginDateTime);
                dict.Add("@LastLoginDateTime", useInfo.LastLoginDateTime);
                dict.Add("@ID", useInfo.ID);

                dbAccess.update(sql, dict);

            }
            else
            {
                useInfo.UserGroup = GlobalSetting.SystemRoles.Normal;
                sql = "INSERT INTO [User] "
                                    + "([SID] ,[Name] ,[FullName] ,[Email] ,[UserGroup] ,[LoginDateTime]) "
                                    + "VALUES "
                                    + "( @SID, @Name, @FullName, @Email, @UserGroup, @LoginDateTime );select SCOPE_IDENTITY();";

                dict.Clear();
                dict.Add("@SID", useInfo.SID);
                dict.Add("@Name", useInfo.Name);
                dict.Add("@FullName", useInfo.FullName);
                dict.Add("@Email", useInfo.Email);
                dict.Add("@UserGroup", useInfo.UserGroup);
                dict.Add("@LoginDateTime", useInfo.LoginDateTime); 
                 
                useInfo.ID = Convert.ToInt32(dbAccess.select(sql, dict).Rows[0][0]);

                if(useInfo.ID == 1)
                {
                    useInfo.UserGroup = GlobalSetting.SystemRoles.Super;
                    sql = "update [User] set UserGroup = @UserGroup "; 
                    dict.Clear();
                    dict.Add("@UserGroup", useInfo.UserGroup); 
                    dbAccess.update(sql, dict); 
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

        return useInfo;
    }

    #endregion

    #region Normal Login

    //register new user
    public Boolean register(DetailUserInfo userInfo)
    {

        dbAccess.open();


        try
        {

            String sql = string.Format("select count(*) from [User] where name = '{0}'", userInfo.Name);
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows[0][0].ToString() != "0")
            {
                return false;
            }

            //encrypt the password before save to database
            using (MD5 md5Hash = MD5.Create())
            {
                userInfo.Password = GetMd5Hash(md5Hash, userInfo.Password);
            }

            //create database access object 
            sql = string.Format("INSERT INTO [DetailUser] ([Name] ,[Password] ,[Age] ,[Sex] ,[Post] ,[Department], [Status], [RegisterDate],[UserGroup]) "
                                     + "VALUES ('{0}','{1}',{2},{3},'{4}','{5}', '{6}',getDate(),'{7}') ",
                                        userInfo.Name,
                                        userInfo.Password,
                                        userInfo.Age,
                                        userInfo.Sex,
                                        userInfo.Post,
                                        userInfo.Department,
                                        DetailUserInfo.UserStatus.Pending,
                                        userInfo.UserGroup);
            dbAccess.update(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

        return true;
    }

    //verify the user login
    public DetailUserInfo login(string userName, string password)
    {
        //encrypt the password before save to database
        using (MD5 md5Hash = MD5.Create())
        {
            password = GetMd5Hash(md5Hash, password);
        }

        //create database access object 
        String sql = string.Format("SELECT * FROM [DetailUser] where Name='{0}' and password = '{1}' and Status = 'Approved' ",
                                    userName,
                                    password);
        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows.Count > 0)
            {
                DetailUserInfo useInfo = new DetailUserInfo();
                useInfo.ID = Convert.ToInt16(dt.Rows[0]["ID"]);
                useInfo.Name = dt.Rows[0]["Name"].ToString();
                useInfo.Password = dt.Rows[0]["Password"].ToString();
                useInfo.Age = dt.Rows[0]["Age"] == DBNull.Value ? (short)0 : Convert.ToInt16(dt.Rows[0]["Age"]);
                useInfo.Sex = Convert.ToInt16(dt.Rows[0]["Sex"]);
                useInfo.Post = dt.Rows[0]["Post"].ToString();
                useInfo.Department = dt.Rows[0]["Department"].ToString();
                useInfo.UserGroup = dt.Rows[0]["UserGroup"].ToString();

                return useInfo;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

        return null;
    }

    public Jayrock.Json.JsonArray getAllUsers()
    {
        JsonArray users = new JsonArray();
        JsonObject tmpUser;

        //create database access object 
        String sql = "SELECT  [ID] "
                   + ",[SID]"
                   + ",[Name] "
                   + ",[FullName] "
                   + ",[Email] "
                   + ",[UserGroup] "
                   + ",[LoginDateTime] "
                   + ",[LastLoginDateTime] FROM [User] "
                   + "where UserGroup != '" + GlobalSetting.SystemRoles.Super + "' "
                   + "  order by UserGroup, Name ";

        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    tmpUser = new JsonObject();
                    foreach (System.Data.DataColumn col in dt.Columns)
                    {
						if(row[col.ColumnName] == DBNull.Value)
                        {
							tmpUser.Accumulate(col.ColumnName.ToLower(), "");
                        }
                        else if (col.DataType == System.Type.GetType("System.DateTime"))
                        {
                            tmpUser.Accumulate(col.ColumnName.ToLower(), Convert.ToDateTime(row[col.ColumnName]).ToString(GlobalSetting.DateTimeFormat));
                        }else if (col.ColumnName == "UserGroup")
                        {
                            tmpUser.Accumulate(col.ColumnName.ToLower(), GlobalSetting.SystemRoles.getDesc(row[col.ColumnName].ToString())); 
                        } 
                        else
                        {
                            tmpUser.Accumulate(col.ColumnName.ToLower(), row[col.ColumnName]);
                        }



                        
                    }

                    users.Add(tmpUser);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

        return users;
    }
    public Jayrock.Json.JsonObject getUserStatus()
    {
        JsonObject status = new JsonObject();

        //create database access object 
        String sql = "SELECT distinct [Status] FROM [DetailUser] where UserGroup <> '" + GlobalSetting.SystemRoles.Admin + "' ";
        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    foreach (System.Data.DataColumn col in dt.Columns)
                    {
                        status.Accumulate(row[col.ColumnName].ToString(), row[col.ColumnName].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

        return status;
    }
    public Jayrock.Json.JsonArray getPendingUsers()
    {
        JsonArray users = new JsonArray();
        JsonObject tmpUser;

        //create database access object 
        String sql = "SELECT [ID] ,[Name] ,[Age] ,[Sex] ,[Post] ,[Department],[RegisterDate] FROM [DetailUser] "
                   + "where Status = 'Approved' and UserGroup <> '" + GlobalSetting.SystemRoles.Admin + "' ";
        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    tmpUser = new JsonObject();
                    foreach (System.Data.DataColumn col in dt.Columns)
                    {
                        tmpUser.Accumulate(col.ColumnName.ToLower(), row[col.ColumnName]);
                    }

                    users.Add(tmpUser);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

        return users;
    }

    public void updateStatus(string userIDs, string loginID, string newStatus)
    {

        //create database access object 
        String sql = string.Format("UPDATE [DetailUser] SET [Status] = '{2}' ,[StatusUpdateBy] = {0} ,[StatusUpdateDate] = getDate() where ID in ({1}) ",
                                    loginID,
                                    userIDs,
                                    newStatus);
        dbAccess.open();
        try
        {
            dbAccess.update(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }
    }

    public void updateUserRole(string userIDs, string role)
    {

        //create database access object 
        String sql = string.Format("UPDATE [User] SET [UserGroup] = '{1}' where ID in ({0}) ",
                                    userIDs,
                                    role);
        dbAccess.open();
        try
        {
            dbAccess.update(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }
    }

    //static function to convert the password to md5 string
    static string GetMd5Hash(MD5 md5Hash, string input)
    {

        // Convert the input string to a byte array and compute the hash. 
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes 
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string. 
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string. 
        return sBuilder.ToString();
    }

    #endregion

    public void logActivity(string userID, string activityID, string category, string action)
    {

        //create database access object 
        String sql = " Delete from [ActivityLog]"
                    + " where UserID = @UserID and ActivityID = @ActivityID and Category = @Category; "
                    + " INSERT INTO [ActivityLog] ([UserID] ,[ActivityID] ,[Category] ,[UserAction] ,[ActionDate]) "
                    + " VALUES (@UserID, @ActivityID, @Category, @UserAction, getDate()) ";
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dbAccess.open();
        try
        {
            dict.Add("@UserID", Convert.ToInt32(userID));
            dict.Add("@ActivityID", activityID);
            dict.Add("@Category", category);
            dict.Add("@UserAction", action);
            dbAccess.update(sql, dict);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

    }

    public void logActivity(string userID, string activityID, string category, string action, string[] schedules)
    {

        //create database access object 
        String sql_delete = " Delete from [ActivityLog]"
                    + " where UserID = @UserID and ActivityID = @ActivityID and Category = @Category ";
        //create database access object 
        String sql = " INSERT INTO [ActivityLog] ([UserID] ,[ActivityID] ,[Category],[ExtField] ,[UserAction] ,[ActionDate]) "
                    + " VALUES (@UserID, @ActivityID, @Category, @ExtField, @UserAction, getDate()) ";
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dbAccess.open();

        dbAccess.BeginTransaction();
        try
        {

            dict.Clear();
            dict.Add("@UserID", Convert.ToInt32(userID));
            dict.Add("@ActivityID", activityID);
            dict.Add("@Category", category);
            dbAccess.update(sql_delete, dict);

            string tmp;
            for (int i = 0; i < schedules.Length; i++)
            {
                tmp = schedules[i];
                dict.Clear();
                dict.Add("@UserID", Convert.ToInt32(userID));
                dict.Add("@ActivityID", activityID);
                dict.Add("@Category", category);
                dict.Add("@UserAction", action);
                dict.Add("@ExtField", Convert.ToInt32(tmp));
                dbAccess.update(sql, dict);
            }

            dbAccess.Commit();
        }
        catch (Exception ex)
        {
            dbAccess.rollback();
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }

    }


    public void getReceiverMailAndName()
    {
        string sql;
        dbAccess.open();
        try
        {
            sql = "select [Description] from [SystemPara] where Category = 'ReceiverEmailAddress' and SubSequence = 1";
            System.Data.DataTable dt = dbAccess.select(sql);
            SupervisorMailAccount = dt.Rows[0][0].ToString();
            sql = "select [Description] from [SystemPara] where Category = 'ReceiverDisplayName' and SubSequence = 1";
            dt = dbAccess.select(sql);
            SupervisorMailDisplayName = dt.Rows[0][0].ToString();
        }
        catch
        {
            throw;
        }
        finally
        {
            dbAccess.close();
        }
    }
    public void commitSuggestion(SuggestionInfo info)
    {
        getReceiverMailAndName();

        //transfer the line-break to html line-break
        info.Suggestion = info.Suggestion.Replace("\n", "<br>");
        //create database access object 
        string sql  = " INSERT INTO [Suggestion] "
                   + "([UserName] ,[Email] ,[Type] ,[PhoneNumber] ,[OtherEmail],[Suggestion],[CreateBy],[CreateDate]) "
                   + " VALUES (@UserName, @Email, @Type, @PhoneNumber, @OtherEmail, @Suggestion, @CreateBy, getDate()) ";
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dbAccess.open();
        try
        {
            dict.Add("@UserName", info.UserName);
            dict.Add("@Email", info.Email);
            dict.Add("@Type", info.Type);
            dict.Add("@PhoneNumber", info.PhoneNumber);
            dict.Add("@OtherEmail", info.OtherEmail);
            dict.Add("@Suggestion", info.Suggestion);
            dict.Add("@CreateBy", info.CreateBy);

            dbAccess.update(sql, dict);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }
        sendMail(info);

    }

    public void sendMail(SuggestionInfo info)
    {
        MailAddress fromAddress = new MailAddress(SystemMailAccount, SystemMailDisplayName);
        MailAddress toAddress = new MailAddress(SupervisorMailAccount, SupervisorMailDisplayName);
        const string subject = "Blue Cross Web Portal Suggestion";
        string body = "Dear " + SupervisorMailDisplayName + ", <br/><br/>Please find the suggestion below:<br/><br/>";
        body += info.Suggestion + "<br/><br/>";
       // body += "Submitted by " + info.UserName + "<br/>";
       // body += "Email : " + info.Email + "<br/>";
        //body += "Tel. : " + (string.IsNullOrEmpty(info.PhoneNumber) ? "N/A" : info.PhoneNumber) + "<span style='padding-left:50px;'>Other Email : </span>" + (string.IsNullOrEmpty(info.OtherEmail) ? "N/A" : info.OtherEmail) + "<br/>";


        //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        SmtpClient smtp = new SmtpClient("smtp.bluecross.com.hk", 25);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.UseDefaultCredentials = false;
        //smtp.Credentials = new NetworkCredential(fromAddress.Address, SystemMailPassword);
        smtp.Credentials = new NetworkCredential(fromAddress.Address, "");
        //smtp.Credentials = System.Net.CredentialCache.DefaultCredentials;
        //smtp.EnableSsl = true;


        MailMessage message = new MailMessage(fromAddress, toAddress);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        using (message)
        {
            smtp.Send(message);
        }
    }


}
