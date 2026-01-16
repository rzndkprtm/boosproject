Imports System.Data
Imports System.Data.SqlClient

Partial Class Ticket_Detail
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataMailing As Object() = Nothing

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Dim ticketId As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/ticket", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("id")) OrElse String.IsNullOrEmpty(Request.QueryString("ticketcode")) Then
            Response.Redirect("~/ticket", False)
            Exit Sub
        End If

        ticketId = Request.QueryString("id").ToString()
        txtTicketCode.Text = Request.QueryString("ticketcode").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Reply(False, String.Empty)
            BindData(ticketId)
        End If
    End Sub

    Protected Sub tmrTicketDetail_Tick(sender As Object, e As EventArgs)
        Dim ticketId As String = Request.QueryString("id")
        BindData(ticketId)
    End Sub

    Protected Sub btnReply_Click(sender As Object, e As EventArgs)
        MessageError_Reply(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showReply(); };"
        Try
            If fuAttachment.HasFiles Then
                For Each file As HttpPostedFile In fuAttachment.PostedFiles
                    If file.ContentLength > (2 * 1024 * 1024) Then
                        MessageError_Reply(True, "MAXIMUM FILE UPLOAD IS 2MB !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showReply", thisScript, True)
                        Exit Sub
                    End If
                Next
            End If

            If msgErrorReply.InnerText = "" Then
                Dim ticketDetailId As String = settingClass.CreateId("SELECT TOP 1 Id FROM TicketDetails ORDER BY Id DESC")

                Dim uploadFolder As String = Server.MapPath(String.Format("~/File/Ticket/{0}", ticketId))

                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO TicketDetails VALUES (@Id, @TicketId, @ReplyBy, @Message, GETDATE())", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ticketDetailId)
                        myCmd.Parameters.AddWithValue("@TicketId", ticketId)
                        myCmd.Parameters.AddWithValue("@ReplyBy", Session("LoginId").ToString())
                        myCmd.Parameters.AddWithValue("@Message", txtMessage.Text)

                        myCmd.ExecuteNonQuery()
                    End Using

                    If fuAttachment.HasFiles Then
                        If Not IO.Directory.Exists(uploadFolder) Then
                            IO.Directory.CreateDirectory(uploadFolder)
                        End If

                        For Each file As HttpPostedFile In fuAttachment.PostedFiles
                            Dim originalName As String = IO.Path.GetFileName(file.FileName)
                            Dim savePath As String = IO.Path.Combine(uploadFolder, originalName)
                            Dim fileNameOnly As String = IO.Path.GetFileNameWithoutExtension(originalName)
                            Dim ext As String = IO.Path.GetExtension(originalName)
                            Dim counter As Integer = 1

                            While IO.File.Exists(savePath)
                                Dim newFileName As String = String.Format("{0}_{1}{2}", fileNameOnly, counter, ext)
                                savePath = IO.Path.Combine(uploadFolder, newFileName)
                                counter += 1
                            End While

                            file.SaveAs(savePath)

                            Dim relativePath As String = String.Format("~/File/Ticket/{0}/{1}", ticketId, IO.Path.GetFileName(savePath))

                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO TicketAttachments VALUES (NEWID(), @TicketDetailId, @FileName)", thisConn)
                                myCmd.Parameters.AddWithValue("@TicketDetailId", ticketDetailId)
                                myCmd.Parameters.AddWithValue("@FileName", IO.Path.GetFileName(savePath))

                                myCmd.ExecuteNonQuery()
                            End Using
                        Next
                    End If

                    thisConn.Close()
                End Using

                url = String.Format("~/ticket/detail?id={0}&ticketcode={1}", ticketId, txtTicketCode.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Reply(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Reply(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError_Reply(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnReply_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showReply", thisScript, True)
        End Try
    End Sub

    Protected Sub rptDetail_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)

            Dim desc As HtmlGenericControl = CType(e.Item.FindControl("pMessage"), HtmlGenericControl)
            Dim titleAttachment As HtmlGenericControl = CType(e.Item.FindControl("titleAttachment"), HtmlGenericControl)
            Dim rptAttachments As Repeater = CType(e.Item.FindControl("rptAttachment"), Repeater)

            Dim ticketDetailId As Integer = row("Id").ToString()
            Dim rawText As String = row("Message").ToString()

            rawText = rawText.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")
            desc.InnerHtml = rawText

            Dim attachmentData As DataSet = settingClass.GetListData("SELECT FileName FROM TicketAttachments WHERE TicketDetailId='" & ticketDetailId & "'")


            If attachmentData IsNot Nothing AndAlso attachmentData.Tables.Count > 0 AndAlso attachmentData.Tables(0).Rows.Count > 0 Then
                rptAttachments.DataSource = attachmentData.Tables(0)
                rptAttachments.DataBind()
                titleAttachment.Visible = True
            Else
                rptAttachments.Visible = False
                titleAttachment.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptAllTicket_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "OpenTicket" Then
            Dim thisId As String = e.CommandArgument.ToString()
            Dim ticketCode As String = settingClass.GetItemData("SELECT TicketCode FROM Tickets WHERE Id='" & thisId & "'")

            url = String.Format("~/ticket/detail?id={0}&ticketcode={1}", thisId, ticketCode)
            Response.Redirect(url, False)
        End If
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Tickets SET Status=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", ticketId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/ticket/detail?id={0}&ticketcode={1}", ticketId, txtTicketCode.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnClose_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindData(ticketId As String)
        Try
            If String.IsNullOrEmpty(ticketId) Then
                Response.Redirect("~/ticket", False)
                Exit Sub
            End If

            Dim thisQuery As String = "SELECT Tickets.*, CustomerLogins.FullName AS CreatedName FROM Tickets LEFT JOIN CustomerLogins ON Tickets.CreatedBy=CustomerLogins.Id WHERE Tickets.Id='" & ticketId & "'"
            If Session("RoleName") = "Customer" Then
                thisQuery = "SELECT Tickets.*, CustomerLogins.FullName AS CreatedName FROM Tickets LEFT JOIN CustomerLogins ON Tickets.CreatedBy=CustomerLogins.Id WHERE Tickets.Id='" & ticketId & "' AND Tickets.CreatedBy='" & Session("LoginId").ToString() & "'"
            End If
            Dim thisData As DataSet = settingClass.GetListData(thisQuery)
            If thisData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/ticket", False)
                Exit Sub
            End If

            txtIssue.Text = thisData.Tables(0).Rows(0).Item("Issue").ToString()
            txtSubject.Text = thisData.Tables(0).Rows(0).Item("Subject").ToString()
            txtCreatedBy.Text = thisData.Tables(0).Rows(0).Item("CreatedName").ToString()
            txtCreatedDate.Text = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")

            Dim status As Boolean = thisData.Tables(0).Rows(0).Item("Status")

            rptDetail.DataSource = settingClass.GetListData("SELECT TicketDetails.*, CustomerLogins.FullName AS ReplyName, CustomerLoginRoles.Name AS ReplyRole FROM TicketDetails LEFT JOIN CustomerLogins ON TicketDetails.ReplyBy=CustomerLogins.Id LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId=CustomerLoginRoles.Id WHERE TicketDetails.TicketId='" & ticketId & "' ORDER BY CreatedDate DESC")
            rptDetail.DataBind()

            aReply.Visible = False : aClose.Visible = False
            If status = True Then aReply.Visible = True : aClose.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Reply(visible As Boolean, message As String)
        divErrorReply.Visible = visible : msgErrorReply.InnerText = message
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
