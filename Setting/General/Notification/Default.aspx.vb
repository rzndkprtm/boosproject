Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Setting_General_Notification_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    <WebMethod()>
    Public Shared Sub SavePopupLog(popupId As String)
        Dim loginId As String = HttpContext.Current.Session("LoginId").ToString()

        Dim query As String = "IF NOT EXISTS ( SELECT 1 FROM NotificationLogs WHERE LoginId = '" & loginId & "' AND NotificationId = '" & popupId & "' ) BEGIN INSERT INTO NotificationLogs VALUES (NEWID(), '" & popupId & "', '" & loginId & "', GETDATE()) END"

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand(query, thisConn)
                thisConn.Open()
                myCmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
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

                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchNotification") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Title LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Notifications {0} ORDER BY Title ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
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

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
