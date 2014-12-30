using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for AccessControl
/// </summary>
public class AccessControl
{
	public AccessControl()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static Dictionary<string, string> getDisabledFunctionDict()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        string sql = "select FunctionName, AccessRole from AccessControl where Enable = 0 ";
        DatabaseAccess dbAccess = new DatabaseAccess();

        dbAccess.open();

        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                result.Add(row["FunctionName"].ToString().ToUpper(), row["AccessRole"].ToString().ToUpper());
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

        return result;
    }

    public static Dictionary<string, string> getAdminFunctionDict()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        string sql = "select FunctionName, AccessRole from AccessControl where Enable = 1 and AccessRole = '" + GlobalSetting.SystemRoles.Admin + "' ";
        DatabaseAccess dbAccess = new DatabaseAccess();

        dbAccess.open();

        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                result.Add(row["FunctionName"].ToString().ToUpper(), row["AccessRole"].ToString().ToUpper());
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

        return result;
    }

    public static Dictionary<string, string> getSuperFunctionDict()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        string sql = "select FunctionName, AccessRole from AccessControl where Enable = 1 and AccessRole in ('" + GlobalSetting.SystemRoles.Super + "') ";
        DatabaseAccess dbAccess = new DatabaseAccess();

        dbAccess.open();

        try
        {
            System.Data.DataTable dt = dbAccess.select(sql);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                result.Add(row["FunctionName"].ToString().ToUpper(), row["AccessRole"].ToString().ToUpper());
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

        return result;
    }
     
}
