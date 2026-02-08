Imports System.Data
Imports System.Data.SqlClient

Partial Class Ticket_Default
    Inherits Page
    
    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            txtSearch.Text = Session("TicketSearch")
            BindDataTicket(txtSearch.Text)
        End If
    End Sub

    Protected Sub tmrTicket_Tick(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataTicket(txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataTicket(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("TicketSearch") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            Dim personalEmail As String = settingClass.GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & Session("LoginId").ToString() & "'")
            If String.IsNullOrEmpty(personalEmail) Then
                personalEmail = settingClass.GetItemData("SELECT CustomerContacts.Email FROM CustomerContacts LEFT JOIN CustomerLogins ON CustomerContacts.CustomerId=CustomerLogins.CustomerId WHERE CustomerLogins.Id='" & Session("LoginId").ToString() & "' AND CustomerContacts.[Primary]=1")
            End If

            txtCreatedEmail.Text = personalEmail
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If fuAttachment.HasFiles Then
                For Each file As HttpPostedFile In fuAttachment.PostedFiles
                    If file.ContentLength > (2 * 1024 * 1024) Then
                        MessageError_Process(True, "MAXIMUM FILE UPLOAD IS 2MB !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                        Exit Sub
                    End If
                Next
            End If
            If msgErrorProcess.InnerText = "" Then
                Dim ticketId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Tickets ORDER BY Id DESC")
                Dim ticketCode As String = settingClass.GenerateTicketId(10)
                Dim ticketDetailId As String = settingClass.CreateId("SELECT TOP 1 Id FROM TicketDetails ORDER BY Id DESC")

                Dim uploadFolder As String = Server.MapPath(String.Format("~/File/Ticket/{0}", ticketId))

                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Tickets VALUES (@Id, @TicketCode, @CreatedBy, GETDATE(), @Issue, @Subject, @Message, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ticketId)
                        myCmd.Parameters.AddWithValue("@TicketCode", ticketCode)
                        myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                        myCmd.Parameters.AddWithValue("@Issue", ddlIssue.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Message", txtMessage.Text)

                        myCmd.ExecuteNonQuery()
                    End Using

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

                mailingClass.Ticket(ticketId)

                url = String.Format("~/ticket/detail?id={0}&ticketcode={1}", ticketId, ticketCode)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError_Process(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProcess_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataTicket(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvList_PageIndexChanging", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                Dim ticketCode As String = settingClass.GetItemData("SELECT TicketCode FROM Tickets WHERE Id='" & dataId & "'")
                url = String.Format("~/ticket/detail?id={0}&ticketcode={1}", dataId, ticketCode)
                Response.Redirect(url, False)
            End If
        End If
    End Sub

    Protected Sub BindDataTicket(searchText As String)
        Session("TicketSearch") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@CompanyId", Session("CompanyId")),
                New SqlParameter("@LoginId", Session("LoginId")),
                New SqlParameter("@LevelName", Session("LevelName"))
            }

            Dim thisData As DataTable = settingClass.GetDataTableSP("sp_TicketList", params)

            gvList.DataSource = thisData
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataTicket", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
