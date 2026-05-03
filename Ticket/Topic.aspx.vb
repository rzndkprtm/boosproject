Imports System.Data
Imports System.Data.SqlClient

Partial Class Ticket_Topic
    Inherits Page

    Dim ticketClass As New TicketClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/ticket", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim stringSearch As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                stringSearch = "WHERE TicketTopics.Id LIKE '%" & searchText & "%' OR TicketTopics.Name LIKE '%" & searchText & "%'"
            End If
            Dim stringQuery As String = String.Format("SELECT TicketTopics.*, LoginRoles.Name AS RoleName CASE WHEN TicketTopics.Active=1 THEN 'Yes' WHEN TicketTopics.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM TicketTopics LEFT JOIN LoginRoles ON TicketTopics.RoleId=LoginRoles.Id {0} ORDER BY TicketTopics.Name ASC", stringSearch)
            gvList.DataSource = ticketClass.GetDataTable(stringQuery)
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

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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
