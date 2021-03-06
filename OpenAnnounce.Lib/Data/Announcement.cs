﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace OpenAnnounce.Data
{
    public class Announcement
    {
        private DatabaseManager _manager;

        private int _id = -1;
        private string _title = "", _body = "", _statusMessage = "";
        private int _importance = 0;
        private DateTime _startDate = DateTime.Today, _endDate = DateTime.Today;
        private DateTime _createTime = DateTime.Now, _editTime = DateTime.Now, _statusTime = DateTime.Now;
        private int _createUser = 0, _editUser = -1, _statusUser = -1;
        private int _status = 0, _scope = 0;

        public int Id { get { return _id; } }
        public string Title { get { return _title; } set { _title = value; } }
        public string Body { get { return _body; } set { _body = value; } }
        public int Importance { get { return _importance; } set { _importance = value; } }

        public DateTime StartDate { get { return _startDate; } set { _startDate = value.Date; } }
        public DateTime EndDate { get { return _endDate; } set { _endDate = value.Date; } }

        public DateTime CreateTime { get { return _createTime; } set { _createTime = value; } }
        public int CreatorId { get { return _createUser; } set { _createUser = value; } }
        public string CreatorDisplayName { get { return UserProfile.FromDatabase(_manager, _createUser).DisplayName; } }

        public DateTime EditTime { get { return _editTime; } set { _editTime = value; } }
        public int EditorId { get { return _editUser; } set { _editUser = value; } }
        public string EditorDisplayName { get { return UserProfile.FromDatabase(_manager, _editUser).DisplayName; } }

        public DateTime StatusTime { get { return _createTime; } set { _createTime = value; } }
        public string StatusMessage { get { return _statusMessage; } set { _statusMessage = value; } }
        public int StatusUserId { get { return _statusUser; } set { _statusUser = value; } }
        public string StatusUserDisplayName { get { return UserProfile.FromDatabase(_manager, _statusUser).DisplayName; } }

        public AnnouncementStatus Status { get { return (AnnouncementStatus)_status; } set { _status = (int)value; } }
        public int ScopeId { get { return _scope; } set { _scope = value; } }
        public Scope Scope { get { return Scope.FromDatabase(_manager, _scope); } }

        public Announcement(DatabaseManager manager)
        {
            _manager = manager;
        }

        public Announcement(DatabaseManager manager, int id)
        {
            _manager = manager;
            _id = id;
        }

        public Announcement(DatabaseManager manager, SqlDataReader r)
        {
            _manager = manager;

            _id = (int)r["Id"];
            _title = (string)r["Title"];
            _body = (string)r["Body"];
            _importance = (int)r["Importance"];

            _startDate = (DateTime)r["StartDate"];
            _endDate = (DateTime)r["EndDate"];

            _createTime = (DateTime)r["CreateTime"];
            _createUser = (int)r["CreateUser"];

            _editTime = (DateTime)r["EditTime"];
            _editUser = (r["EditUser"] is DBNull) ? -1 : (int)r["EditUser"];

            _statusTime = (DateTime)r["StatusTime"];
            _statusUser = (r["StatusUser"] is DBNull) ? -1 : (int)r["StatusUser"];
            _statusMessage = (r["StatusMessage"] is DBNull) ? "" : (string)r["StatusMessage"];

            _status = (int)r["Status"];
            _scope = (r["Scope"] is DBNull) ? 0 : (int)r["Scope"];
        }

        public void Clean()
        {
            if (_startDate > _endDate)
            {
                DateTime _temp = _startDate;
                _startDate = _endDate;
                _endDate = _temp;
            }
        }

        public void Insert()
        {
            Clean();

            using (SqlCommand cmd = _manager.CreateCommand("INSERT INTO Announcements (Title, Body, Importance, StartDate, EndDate, CreateTime, CreateUser, EditTime, EditUser, StatusTime, StatusUser, StatusMessage, Status, Scope) VALUES (@title, @body, @importance, @startDate, @endDate, @createTime, @createUser, @editTime, @editUser, @statusTime, @statusUser, @statusMessage, @status, @scope)"))
            {
                cmd.Parameters.AddWithValue("@title", _title);
                cmd.Parameters.AddWithValue("@body", _body);
                cmd.Parameters.AddWithValue("@importance", _importance);
                cmd.Parameters.AddWithValue("@startDate", _startDate);
                cmd.Parameters.AddWithValue("@endDate", _endDate);
                cmd.Parameters.AddWithValue("@createTime", _createTime);
                cmd.Parameters.AddWithValue("@createUser", _createUser);
                cmd.Parameters.AddWithValue("@editTime", _editTime);
                cmd.Parameters.AddWithValue("@editUser", (_editUser <= 0) ? DBNull.Value : (object)_editUser);
                cmd.Parameters.AddWithValue("@statusTime", _statusTime);
                cmd.Parameters.AddWithValue("@statusUser", (_statusUser <= 0) ? DBNull.Value : (object)_statusUser);
                cmd.Parameters.AddWithValue("@statusMessage", _statusMessage);
                cmd.Parameters.AddWithValue("@status", _status);
                cmd.Parameters.AddWithValue("@scope", (_scope == 0) ? DBNull.Value : (object)_scope);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update()
        {
            Clean();

            using (SqlCommand cmd = _manager.CreateCommand("UPDATE Announcements SET Title=@title, Body=@body, Importance=@importance, StartDate=@startDate, EndDate=@endDate, CreateTime=@createTime, CreateUser=@createUser, EditTime=@editTime, EditUser=@editUser, StatusTime=@statusTime, StatusUser=@statusUser, StatusMessage=@statusMessage, Status=@status, Scope=@scope WHERE id=@id"))
            {
                cmd.Parameters.AddWithValue("@id", _id);
                cmd.Parameters.AddWithValue("@title", _title);
                cmd.Parameters.AddWithValue("@body", _body);
                cmd.Parameters.AddWithValue("@importance", _importance);
                cmd.Parameters.AddWithValue("@startDate", _startDate);
                cmd.Parameters.AddWithValue("@endDate", _endDate);
                cmd.Parameters.AddWithValue("@createTime", _createTime);
                cmd.Parameters.AddWithValue("@createUser", _createUser);
                cmd.Parameters.AddWithValue("@editTime", _editTime);
                cmd.Parameters.AddWithValue("@editUser", (_editUser <= 0) ? DBNull.Value : (object)_editUser);
                cmd.Parameters.AddWithValue("@statusTime", _statusTime);
                cmd.Parameters.AddWithValue("@statusUser", (_statusUser <= 0) ? DBNull.Value : (object)_statusUser);
                cmd.Parameters.AddWithValue("@statusMessage", _statusMessage);
                cmd.Parameters.AddWithValue("@status", _status);
                cmd.Parameters.AddWithValue("@scope", (_scope == 0) ? DBNull.Value : (object)_scope);

                cmd.ExecuteNonQuery();
            }
        }

        public static void PopulatePageNumber(DatabaseManager manager, UserProfile settings, Label currentPageLabel, Label maxPageLabel, string mode, bool viewExpired, int currentPage, int numPerPage)
        {
            SqlCommand cmd;
            if (mode == "ViewAll" || mode == "Approval")
            {
                cmd = manager.CreateCommand("SELECT COUNT(*) FROM Announcements WHERE Status<>3");
            }
            else
            {
                cmd = manager.CreateCommand("SELECT COUNT(*) FROM Announcements WHERE Status<>3 AND CreateUser=@createUser");
            }

            try
            {
                if (!viewExpired)
                    cmd.CommandText += " AND EndDate>=@today";

                cmd.Parameters.AddWithValue("@createUser", settings.Id);
                cmd.Parameters.AddWithValue("@today", DateTime.Today);

                maxPageLabel.Text = Math.Max(1, Math.Ceiling((((int)cmd.ExecuteScalar()) / (double)numPerPage))).ToString();
                currentPageLabel.Text = currentPage.ToString();
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public static void PopulateAnnouncementTable(DatabaseManager manager, UserProfile settings, CompiledSecurityInfo level, HtmlTable table, int offset, int rows, Dictionary<int, CheckBox> checkBoxes)
        {
            string mode;

            if (level["CanApproveAnnouncement"] && level["CanViewAllAnnouncement"])
                mode = "Approval";
            else if (level["CanViewAllAnnouncement"])
                mode = "ViewAll";
            else
                mode = "Submission";

            PopulateAnnouncementTable(manager, settings, mode, false, false, table, offset, rows, checkBoxes);
        }

        public static void PopulateAnnouncementTable(DatabaseManager manager, UserProfile settings, string mode, bool viewExpired, bool viewDeleted, HtmlTable table, int offset, int rows, Dictionary<int, CheckBox> checkBoxes)
        {
            SqlCommand cmd;

            if (mode == "ViewAll" || mode == "Approval")
            {
                cmd = manager.CreateCommand("SELECT * FROM Announcements WHERE 1=1");
            }
            else
            {
                cmd = manager.CreateCommand("SELECT * FROM Announcements WHERE CreateUser=@createUser");
            }

            try
            {
                if (!viewExpired)
                    cmd.CommandText += " AND EndDate>=@today";

                if (!viewDeleted)
                    cmd.CommandText += " AND Status<>3";

                if (mode == "Approval")
                    cmd.CommandText += " ORDER BY (CASE WHEN Status = 0 THEN 1 ELSE 0 END) DESC, ";
                else if (mode == "Submission")
                    cmd.CommandText += " ORDER BY (CASE WHEN Status = 2 THEN 1 ELSE 0 END) DESC, ";
                else
                    cmd.CommandText += " ORDER BY ";

                cmd.CommandText += "Importance DESC, StartDate DESC OFFSET " + offset + " ROWS FETCH NEXT " + rows + " ROWS ONLY";

                cmd.Parameters.AddWithValue("@createUser", settings.Id);
                cmd.Parameters.AddWithValue("@today", DateTime.Today);

                List<Announcement> announcements = new List<Announcement>();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            announcements.Add(new Announcement(manager, r));
                        }
                    }
                    else
                    {
                        HtmlTableRow row = new HtmlTableRow();
                        HtmlTableCell cell;
                        row.Cells.Add(cell = new HtmlTableCell()
                        {
                            ColSpan = (checkBoxes == null) ? 6 : 7,
                            InnerHtml = "<em>There are currently no announcements requiring attention</em>",
                        });
                        cell.Style.Add("padding-left", "5px");
                        table.Rows.Add(row);
                        return;
                    }
                }

                foreach (Announcement a in announcements)
                {
                    HtmlTableRow row = new HtmlTableRow();

                    if (a.EndDate < DateTime.Today || a.Status == AnnouncementStatus.Deleted)
                    {
                        row.Style.Add("background", "#f3f3f3");
                    }
                    else if (mode == "Approval" && a.Status == Announcement.AnnouncementStatus.Pending)
                    {
                        row.Style.Add("background", "#ffa4a4");
                    }
                    else if (mode == "Submission" && a.Status == Announcement.AnnouncementStatus.Denied)
                    {
                        row.Style.Add("background", "#ffa4a4");
                    }
                    HtmlTableCell checkCell;
                    CheckBox chk;

                    if (checkBoxes != null)
                    {
                        row.Cells.Add(checkCell = new HtmlTableCell());
                        checkCell.Style.Add("text-align", "center");
                        checkCell.Controls.Add(chk = new CheckBox()
                        {
                            ID = "chk_announcement_" + a.Id
                        });
                        if (checkBoxes.ContainsKey(a.Id))
                            checkBoxes[a.Id] = chk;
                        else
                            checkBoxes.Add(a.Id, chk);
                    }
                    row.Cells.Add(new HtmlTableCell() { InnerHtml = a.Title });
                    row.Cells.Add(new HtmlTableCell() { InnerHtml = a.CreatorDisplayName });
                    row.Cells.Add(new HtmlTableCell() { InnerHtml = a.StartDate.ToShortDateString() + " - " + a.EndDate.ToShortDateString() });
                    row.Cells.Add(new HtmlTableCell() { InnerHtml = a.Scope.Name });
                    if (a.EndDate < DateTime.Today)
                    {
                        row.Cells.Add(new HtmlTableCell() { InnerHtml = "Expired" });
                    }
                    else
                    {
                        switch (a.Status)
                        {
                            case Announcement.AnnouncementStatus.Pending:
                                row.Cells.Add(new HtmlTableCell() { InnerHtml = "Pending" });
                                break;
                            case Announcement.AnnouncementStatus.Approved:
                                row.Cells.Add(new HtmlTableCell() { InnerHtml = "Approved" });
                                break;
                            case Announcement.AnnouncementStatus.Denied:
                                row.Cells.Add(new HtmlTableCell() { InnerHtml = "Denied" });
                                break;
                            case Announcement.AnnouncementStatus.Deleted:
                                row.Cells.Add(new HtmlTableCell() { InnerHtml = "Deleted" });
                                break;
                            default:
                                row.Cells.Add(new HtmlTableCell() { InnerHtml = "(Unknown)" });
                                break;
                        }
                    }
                    row.Cells.Add(new HtmlTableCell() { InnerHtml = "<a href=\"AnnouncementEdit.aspx?id=" + a.Id + "\" class=\"linkbutton-small\" style=\"padding-left: 8px; padding-right: 8px;\">Edit</a>" });

                    table.Rows.Add(row);
                }
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public static Announcement FromDatabase(DatabaseManager manager, int id)
        {
            using (SqlCommand cmd = manager.CreateCommand("SELECT * FROM Announcements WHERE Id=@id"))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return new Announcement(manager, r);
                    else
                        return null;
                }
            }
        }

        public enum AnnouncementStatus : int
        {
            Pending = 0,
            Approved = 1,
            Denied = 2,
            Deleted = 3
        }
    }
}