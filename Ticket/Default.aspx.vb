
Partial Class Ticket_Default
    Inherits Page

    Dim ticketClass As New TicketClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)

    End Sub

    Protected Sub btnTopics_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/ticket/topic", False)
    End Sub

    Protected Sub BindData()
        Try
            rptChatList.DataSource = ticketClass.GetDataTable("SELECT Tickets.*, TicketTopics.Name AS TopicName, CustomerLogins.FullName AS LoginName FROM Tickets LEFT JOIN TicketTopics ON Tickets.TopicId=TicketTopics.Id LEFT JOIN CustomerLogins ON Tickets.LoginId=CustomerLogins.Id")
            rptChatList.DataBind()

            btnTopics.Visible = PageAction("Setting Topic")
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
