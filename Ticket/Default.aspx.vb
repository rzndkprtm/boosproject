Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports OfficeOpenXml.LoadFunctions

Partial Class Ticket_Default
    Inherits Page

    Dim ticketClass As New TicketClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("tid")) Then
            lblId.Text = Request.QueryString("tid").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindListTicket(txtSearch.Text)
            BindTicketDetail(lblId.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            BindTopic()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnTopic_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/ticket/topic", False)
    End Sub

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/ticket", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindListTicket(txtSearch.Text)
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If msgErrorProcess.InnerText = "" Then
                Dim thisId As String = String.Empty

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Tickets OUTPUT INSERTED.Id VALUES (NEWID(), @LoginId, @TopicId, @Subject, GETDATE(), 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@LoginId", Session("LoginId").ToString())
                        myCmd.Parameters.AddWithValue("@TopicId", ddlTopic.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())

                        thisConn.Open()
                        thisId = myCmd.ExecuteScalar().ToString()
                    End Using
                End Using

                Dim directoryTicket As String = Server.MapPath(String.Format("~/File/Ticket/{0}/", thisId.ToUpper()))
                If Not Directory.Exists(directoryTicket) Then
                    Directory.CreateDirectory(directoryTicket)
                End If

                url = String.Format("~/ticket?tid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSend_Click(sender As Object, e As EventArgs)
        Try
            If Not String.IsNullOrEmpty(txtMessage.Text) Then
                Dim messageText As String = txtMessage.Text.Trim()
                If txtMessage.Text.Contains("[Translate]") Then

                End If
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO TicketDetails VALUES (NEWID(), @TicketId, @SenderId, @MessageText, GETDATE())", thisConn)
                        myCmd.Parameters.AddWithValue("@TicketId", lblId.Text)
                        myCmd.Parameters.AddWithValue("@SenderId", Session("LoginId").ToString())
                        myCmd.Parameters.AddWithValue("@MessageText", txtMessage.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim url As String = "~/ticket"
                If Not String.IsNullOrEmpty(lblId.Text) Then
                    url = String.Format("~/ticket?tid={0}", lblId.Text)
                End If

                Response.Redirect(url, False)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        If fuMessage.HasFile Then

            Dim fileName As String = Path.GetFileName(fuMessage.FileName)
            Dim thisPath As String = String.Format("~/File/Ticket/{0}/", lblId.Text.ToUpper())

            Dim folderPath As String = Server.MapPath(thisPath)

            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If

            Dim savePath As String = Path.Combine(folderPath, fileName)

            fuMessage.SaveAs(savePath)

        End If
    End Sub

    Protected Sub BindListTicket(searchText As String)
        Try
            Dim stringSearch As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                stringSearch = "WHERE Tickets.Subject LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT Tickets.*, CASE WHEN Tickets.Status=1 THEN 'Open' WHEN Tickets.Status=0 THEN 'Close' ELSE 'Error' END AS DataStatus, TicketTopics.Name AS TopicName, CustomerLogins.FullName AS LoginName, CASE WHEN LEN(LastMsg.MessageText) > 50  THEN LEFT(LastMsg.MessageText, 50) + ' .....' ELSE LastMsg.MessageText END AS LastMessage, LastMsg.CreatedDate AS LastMessageDate FROM Tickets LEFT JOIN TicketTopics ON Tickets.TopicId = TicketTopics.Id LEFT JOIN CustomerLogins ON Tickets.LoginId = CustomerLogins.Id OUTER APPLY (SELECT TOP 1 MessageText, CreatedDate FROM TicketDetails WHERE TicketDetails.TicketId=Tickets.Id ORDER BY CreatedDate DESC) LastMsg {0}", stringSearch)
            rptChatList.DataSource = ticketClass.GetDataTable(thisString)
            rptChatList.DataBind()

            btnTopic.Visible = PageAction("Setting Topic")
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindTicketDetail(ticketId As String)
        Try
            If Not String.IsNullOrEmpty(ticketId) Then

                Dim ticketData As DataRow = ticketClass.GetDataRow("SELECT Subject FROM Tickets WHERE Id='" & ticketId & "'")
                If ticketData Is Nothing Then
                    Response.Redirect("~/ticket", False)
                    Exit Sub
                End If
                hSubject.InnerText = "Subject : " & ticketData("Subject").ToString()

                rptTicketDetail.DataSource = ticketClass.GetDataTable("SELECT TicketDetails.*, CustomerLogins.FullName AS LoginName FROM TicketDetails LEFT JOIN CustomerLogins ON TicketDetails.SenderId=CustomerLogins.Id WHERE TicketDetails.TicketId='" & ticketId.ToUpper() & "' ORDER BY TicketDetails.CreatedDate ASC")
                rptTicketDetail.DataBind()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindTopic()
        ddlTopic.Items.Clear()
        Try
            ddlTopic.DataSource = ticketClass.GetDataTable("SELECT * FROM TicketTopics WHERE Active=1 ORDER BY Name ASC")
            ddlTopic.DataTextField = "Name"
            ddlTopic.DataValueField = "Id"
            ddlTopic.DataBind()

            If ddlTopic.Items.Count > 0 Then
                ddlTopic.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
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
