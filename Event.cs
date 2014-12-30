using System;
using System.Collections.Generic;

using System.Web;
using Jayrock.Json;

/// <summary>
/// Summary description for Event
/// </summary>
public class Event
{
    //object for DB access
    private DatabaseAccess dbAccess = new DatabaseAccess();

    public void save(EventInfo eventinfo, List<ImageInfo> imageList)
    {
        string sql = " INSERT INTO [Event] "
                   + " ([SerialNo] ,[Type] ,[Name] ,[StartTime] ,[EndTime] ,[Deadline] ,[Location] ,[ContactPerson] ,"
                   + " [PhoneNumber] ,[Department] ,[EventDetails], [Status] ,[CreateDate] ,[CreateBy]) "
                   + " VALUES "
                   + " (@SerialNo, @Type, @Name, @StartTime,@EndTime, @Deadline, @Location,@ContactPerson,@PhoneNumber, "
                   + " @Department, @EventDetails, @Status, getDate(), @CreateBy);select SCOPE_IDENTITY();";

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("@SerialNo", eventinfo.SerialNo);
        dict.Add("@Type", eventinfo.Type);
        dict.Add("@Name", eventinfo.Name);
        dict.Add("@StartTime", eventinfo.StartTime);
        dict.Add("@EndTime", eventinfo.EndTime);
        dict.Add("@Deadline", eventinfo.Deadline);
        dict.Add("@Location", eventinfo.Location);
        dict.Add("@ContactPerson", eventinfo.ContactPerson);
        dict.Add("@PhoneNumber", eventinfo.PhoneNumber);
        dict.Add("@Department", eventinfo.Department);
        dict.Add("@EventDetails", eventinfo.EventDetails);
        dict.Add("@Status", eventinfo.Status);
        dict.Add("@CreateBy", eventinfo.CreateBy);

        //insert the new images 
        string sql_Image = " INSERT INTO [Image] ([Category] ,[ParentID] ,[FileName],[Extension],[Type],[ContentLength],[Content]) "
                     + " VALUES (@Category, @ParentID, @FileName, @Extension, @Type ,@ContentLength,@Content)";
        Dictionary<string, object> dictImage = new Dictionary<string, object>();

        this.dbAccess.open();

        this.dbAccess.BeginTransaction();
        try
        {
            string lastID = this.dbAccess.select(sql, dict).Rows[0][0].ToString();

            //insert the images
            for (int i = 0; i < imageList.Count; i++)
            {
                dictImage.Clear();
                dictImage.Add("@Category", "Event");
                dictImage.Add("@ParentID", lastID);
                dictImage.Add("@FileName", imageList[i].FileName);
                dictImage.Add("@Extension", imageList[i].Extension);
                dictImage.Add("@Type", imageList[i].Type);
                dictImage.Add("@ContentLength", imageList[i].ContentLength);
                dictImage.Add("@Content", imageList[i].Content);
                this.dbAccess.update(sql_Image, dictImage);
            }

            this.dbAccess.Commit();

        }
        catch (Exception ex)
        {
            this.dbAccess.rollback();
            throw ex;
        }
        finally
        {
            this.dbAccess.close();
        }

    }

    public void update(EventInfo eventinfo, List<ImageInfo> imageList, bool resetAttendance)
    {
        string sql = "  UPDATE [Event]"
                    + " SET [SerialNo] = @SerialNo"
                    + " ,[VersionNo] = @VersionNo"
                    + " ,[Type] = @Type" 
                    + " ,[Name] = @Name"
                    + " ,[StartTime] = @StartTime"
                    + " ,[EndTime] = @EndTime"
                    + " ,[Deadline] = @Deadline"
                    + " ,[Location] = @Location"
                    + " ,[ContactPerson] = @ContactPerson"
                    + " ,[PhoneNumber] = @PhoneNumber"
                    + " ,[Department] = @Department"
                    + " ,[EventDetails] = @EventDetails  "
                    + " ,[UpdateDate] = getDate() "
                    + " ,[UpdateBy] = @UpdateBy "
                    + " WHERE ID= @ID";



        string sqlResetSche = "delete "
                        + "FROM [ActivityLog] "
                        + "where ActivityID = @ActivityID";



        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("@SerialNo", eventinfo.SerialNo);
        dict.Add("@VersionNo", eventinfo.VersionNo);
        dict.Add("@Type", eventinfo.Type);
        dict.Add("@Name", eventinfo.Name);
        dict.Add("@StartTime", eventinfo.StartTime);
        dict.Add("@EndTime", eventinfo.EndTime);
        dict.Add("@Deadline", eventinfo.Deadline);
        dict.Add("@Location", eventinfo.Location);
        dict.Add("@ContactPerson", eventinfo.ContactPerson);
        dict.Add("@PhoneNumber", eventinfo.PhoneNumber);
        dict.Add("@Department", eventinfo.Department);
        dict.Add("@EventDetails", eventinfo.EventDetails);
        dict.Add("@UpdateBy", eventinfo.UpdateBy);

        dict.Add("@ID", eventinfo.ID);


        //insert the new images 
        string sql_Image = " Update [Image] set [FileName] = @FileName, [Extension] = @Extension, "
                  + " [Type] = @Type,[ContentLength] = @ContentLength,[Content] = @Content where ID = @ID";
        Dictionary<string, object> dictImage = new Dictionary<string, object>();


        this.dbAccess.open();

        try
        {
            this.dbAccess.BeginTransaction();
            this.dbAccess.update(sql, dict);


            //insert the images
            for (int i = 0; i < imageList.Count; i++)
            { 
                dictImage.Clear();
                dictImage.Add("@FileName", imageList[i].FileName);
                dictImage.Add("@Extension", imageList[i].Extension);
                dictImage.Add("@Type", imageList[i].Type);
                dictImage.Add("@ContentLength", imageList[i].ContentLength);
                dictImage.Add("@Content", imageList[i].Content);
                dictImage.Add("@ID", imageList[i].ID);
                this.dbAccess.update(sql_Image, dictImage);
            }


            //reset attendance
            if (resetAttendance)
            {
                dictImage.Clear();
                dictImage.Add("@ActivityID", eventinfo.ID);
                this.dbAccess.update(sqlResetSche, dictImage);

            }

            this.dbAccess.Commit();
        }
        catch (Exception ex)
        {
            this.dbAccess.rollback();
            throw ex;
        }
        finally
        {
            this.dbAccess.close();
        }

    }

    public Jayrock.Json.JsonArray getAllEvent()
    {
        JsonArray events = new JsonArray();
        JsonObject tmpEvent;
         
        //create database access object 
        String sql = " SELECT [Event].[ID], [SerialNo],[Name] Title, Status, 'Event' Category, SystemPara.Description Type,  left(CONVERT(VARCHAR, [StartTime], " + GlobalSetting.DatabaseDateTimeFormat + "),10) EffectiveDate FROM [Event] "
                   + " left outer join SystemPara on category = 'EventType' and [Event].type = SystemPara.ID"
                   + " where status <> 'Trashed' order by status, StartTime desc, SerialNo desc ";
         
        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    tmpEvent = new JsonObject();
                    foreach (System.Data.DataColumn col in dt.Columns)
                    {
                        tmpEvent.Accumulate(col.ColumnName.ToLower(), row[col.ColumnName]);
                    }

                    events.Add(tmpEvent);
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

        return events;
    }

    public EventInfo getEvent(int id)
    {
        EventInfo eventinfo = new EventInfo();

        String sql = string.Format("SELECT  [ID] ,[VersionNo],[SerialNo] ,[Type] ,[Name] ,[StartTime] ,[EndTime] ,[Deadline] ,[Location] ,[ContactPerson] ,[PhoneNumber] ,[Department] ,[EventDetails] ,[Status] ,[CreateDate] ,[CreateBy] " +
                    "FROM [Event] where [ID] = {0}",
                    id);
         
        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);

            eventinfo.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            eventinfo.SerialNo = dt.Rows[0]["SerialNo"].ToString();
            eventinfo.VersionNo = Convert.ToSingle(dt.Rows[0]["VersionNo"]);
            eventinfo.Type = Convert.ToInt32(dt.Rows[0]["Type"].ToString());
            eventinfo.Name = dt.Rows[0]["Name"].ToString();
            eventinfo.Location = dt.Rows[0]["Location"].ToString();
            eventinfo.PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
            eventinfo.StartTime = Convert.ToDateTime(dt.Rows[0]["StartTime"]);
            eventinfo.EndTime = Convert.ToDateTime(dt.Rows[0]["EndTime"]);
            eventinfo.Deadline = Convert.ToDateTime(dt.Rows[0]["Deadline"]);
            eventinfo.ContactPerson = dt.Rows[0]["ContactPerson"].ToString();
            eventinfo.CreateBy = Convert.ToInt32(dt.Rows[0]["CreateBy"]);
            eventinfo.CreateDate = Convert.ToDateTime(dt.Rows[0]["CreateDate"]);
            eventinfo.Department = dt.Rows[0]["Department"].ToString();
            eventinfo.EventDetails = dt.Rows[0]["EventDetails"].ToString();
            eventinfo.Status = dt.Rows[0]["Status"].ToString(); 
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbAccess.close();
        }
         
        return eventinfo;
    }

    public Jayrock.Json.JsonArray getEventByMonth(DateTime dateOfMonth)
    {

        JsonArray events = new JsonArray();
        JsonObject tmpEvent;

        DateTime startime = new DateTime(dateOfMonth.Year, dateOfMonth.Month, 1).AddMonths(0);
        DateTime endtime = new DateTime(dateOfMonth.Year, dateOfMonth.Month, 1).AddMonths(1);

        //create database access object 
        String sql = " SELECT [ID] ,[Name] ,CONVERT(VARCHAR, [StartTime], 120) [StartTime] ,CONVERT(VARCHAR, [EndTime], 120) [EndTime], 'Event' Type FROM [Event] "
                   + " where (([StartTime] >= @StartTime and [StartTime] <= @EndTime) "
                   + " or ([EndTime] >= @StartTime and [EndTime] <= @EndTime)) and Status = 'Published' "
                   + " union all "
                   + " SELECT distinct [Training].[ID] ,[Name] ,CONVERT(VARCHAR, StartTime, 120) [StartTime] , "
                   + " CONVERT(VARCHAR, [EndTime], 120) [EndTime], 'Training' Type  "
                   + " FROM [Training] "
                   + " join TrainingSche on Training.ID =  TrainingSche.TrainingID  "
                   + " where (([StartTime] >= @StartTime  "
                   + " and [StartTime] <= @EndTime)   "
                   + " or ([EndTime] >= @StartTime and [EndTime] <= @EndTime))  "
                   + " and  Status = 'Published'  order by StartTime ";

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("@StartTime", startime);
        dict.Add("@EndTime", endtime);

        dbAccess.open();
        try
        {
            System.Data.DataTable dt = dbAccess.select(sql, dict);
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    tmpEvent = new JsonObject();
                    foreach (System.Data.DataColumn col in dt.Columns)
                    {
                        tmpEvent.Accumulate(col.ColumnName.ToLower(), row[col.ColumnName]);
                    }

                    events.Add(tmpEvent);
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

        return events;
    }

    public void updateStatus(string articleIDs, string newStatus)
    {

        //create database access object 
        String sql = string.Format("UPDATE [Event] SET [Status] = '{0}' where ID in ({1}) ",
                                    newStatus,
                                    articleIDs);
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

    public string getEventDecision(int ID, int loginID)
    {


        string decision = "";

        string sql = "select "
                    + "case when a.UserAction = 'NotAttend' then 'Not Attend' else a.UserAction END 'Decision' "  
                    + "FROM ActivityLog a " 
                    + "join Event e on a.ActivityID = e.ID "
                    + "where a.Category='Event' and e.ID = @ID and a.UserID = @UserID  ";

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("@ID", ID);
        dict.Add("@UserID", loginID);

        dbAccess.open();

        try
        {
            System.Data.DataTable dt = dbAccess.select(sql, dict);

            foreach (System.Data.DataRow row in dt.Rows)
            { 
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    decision = row[col.ColumnName].ToString();
                }
                 
            }

        }
        catch
        {
            throw;
        }
        finally
        {
            dbAccess.close();
        }

        return decision;

    }


    public List<EventInfo> getLatestEvent(int total)
    {
        List<EventInfo> list = new List<EventInfo>();
        EventInfo eventInfo;

        dbAccess.open();
        try
        {
            string sql = string.Format(" SELECT top {0} [Event].[ID], [SerialNo],[Name] Title, Status, 'Event' Category, SystemPara.Description Type,  [StartTime] EffectiveDate FROM [Event] "
                   + " left outer join SystemPara on category = 'EventType' and [Event].type = SystemPara.ID"
                   + " where status = '{1}' order by StartTime desc, SerialNo desc ",
                                        total,
                                        GlobalSetting.ArticleStatus.Published);

            if(total == 0)
            {
                sql = string.Format(" SELECT [Event].[ID], [SerialNo],[Name] Title, Status, 'Event' Category, SystemPara.Description Type,  [StartTime] EffectiveDate FROM [Event] "
                   + " left outer join SystemPara on category = 'EventType' and [Event].type = SystemPara.ID"
                   + " where status = '{0}' order by StartTime desc, SerialNo desc ", 
                                        GlobalSetting.ArticleStatus.Published);
            }
             
            System.Data.DataTable dt = dbAccess.select(sql);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                eventInfo = new EventInfo();
                eventInfo.ID = Convert.ToInt32(row["ID"]);
                eventInfo.Name = Convert.ToString(row["Title"]);
                eventInfo.StartTime = Convert.ToDateTime(row["EffectiveDate"]); 
                 
                list.Add(eventInfo);
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

        return list;
    }

}
