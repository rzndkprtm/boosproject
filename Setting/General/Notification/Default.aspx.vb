Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Setting_General_Notification_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    <WebMethod()>
    Public Shared Function GetNotification() As Object
        Dim result As New List(Of Object)

        Try
            Dim settingClass As New SettingClass
            Dim loginId As String = HttpContext.Current.Session("LoginId").ToString()

            Dim dt As DataTable = settingClass.GetDataTable("SELECT N.* FROM Notifications N CROSS APPLY STRING_SPLIT(N.LoginId, ',') A WHERE A.VALUE='" & loginId & "' AND N.Active=1 AND CAST(GETDATE() AS DATE) BETWEEN CAST(N.StartDate AS DATE) AND CAST(N.EndDate AS DATE) AND NOT EXISTS (SELECT 1 FROM NotificationLogs L WHERE L.NotificationId=N.Id AND L.LoginId='" & loginId & "' ) ORDER BY N.Id")

            For Each row As DataRow In dt.Rows
                Dim fullName As String = "<strong>" & HttpContext.Current.Session("FullName").ToString() & "</strong>"

                Dim thisMsg As String = row("Message").ToString()

                thisMsg = thisMsg.Replace("[FullName]", fullName)
                thisMsg = "Hi " & fullName & ",<br><br>" & thisMsg

                result.Add(New With {.title = row("Title").ToString(), .message = thisMsg, .popupId = row("Id").ToString()})
            Next
        Catch ex As Exception
        End Try
        Return result
    End Function

    <WebMethod()>
    Public Shared Sub SavePopupLog(popupId As String)
        Dim loginId As String = HttpContext.Current.Session("LoginId").ToString()

        Dim query As String = "IF NOT EXISTS (SELECT 1 FROM NotificationLogs WHERE LoginId='" & loginId & "' AND NotificationId='" & popupId & "') BEGIN INSERT INTO NotificationLogs VALUES (NEWID(), '" & popupId & "', '" & loginId & "', GETDATE()) END"

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using thisCmd As SqlCommand = New SqlCommand(query, thisConn)
                thisConn.Open()
                thisCmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchNotification")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchNotification") = txtSearch.Text
        Response.Redirect("~/setting/general/notification/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchNotification") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Dim url As String = String.Format("~/setting/general/notification/edit?notifid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Notifications WHERE Id=@Id; DELETE FROM NotificationLogs WHERE NotificationId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchNotification") = txtSearch.Text
            Response.Redirect("~/setting/general/notification", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchNotification") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Notifications.Title LIKE '%" & searchText.Trim() & "%' OR LoginRoles.Name LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT Notifications.*, LoginRoles.Name AS RoleName, CASE WHEN Notifications.Active=1 THEN 'Yes' WHEN Notifications.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Notifications LEFT JOIN LoginRoles ON Notifications.RoleId=LoginRoles.Id {0} ORDER BY Notifications.Title ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
