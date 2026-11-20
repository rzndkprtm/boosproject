Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class Ticket_Add
    Inherits Page

    Private ReadOnly myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Private ticketClass As New TicketClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/ticket", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            ' VALIDATION
            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(txtSubject.Text) Then
                MessageError(True, "SUBJECT IS REQUIRED !")
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(txtMessage.Text) Then
                MessageError(True, "MESSAGE IS REQUIRED !")
                Exit Sub
            End If

            ' VALIDATE FILE SIZE
            If fileTicketUpload.HasFiles Then
                For Each uploadedFile As HttpPostedFile In fileTicketUpload.PostedFiles
                    If uploadedFile.ContentLength > 5 * 1024 * 1024 Then
                        MessageError(True, "MAXIMUM FILE SIZE IS 5 MB PER FILE !")
                        Exit Sub
                    End If
                Next
            End If

            ' GENERATE TICKET ID
            Dim ticketId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM Tickets ORDER BY Id DESC")

            ' GENERATE FIRST MESSAGE ID
            Dim messageId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketMessages ORDER BY Id DESC")

            Dim success As Boolean = False
            Dim retry As Integer = 0
            Dim maxRetry As Integer = 100
            Dim ticketNo As String = String.Empty

            ' CREATE TICKET
            Do While Not success
                retry += 1

                If retry > maxRetry Then
                    Throw New Exception("FAILED TO GENERATE UNIQUE TICKET NUMBER !")
                End If

                ticketNo = "BOOS" & ticketClass.GenerateTicketNo()
                Try
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("INSERT INTO Tickets VALUES (@Id, @TicketNo, @LoginId, @Type, @Subject, 'Open', 'Normal', GETDATE(), NULL, NULL); INSERT INTO TicketMessages VALUES (@ChatMessageId, @Id, 'Customer', @LoginId, @Message, GETDATE(), 0);", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", ticketId)
                            thisCmd.Parameters.AddWithValue("@TicketNo", ticketNo)
                            thisCmd.Parameters.AddWithValue("@LoginId", Session("LoginId").ToString())
                            thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            thisCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())
                            thisCmd.Parameters.AddWithValue("@ChatMessageId", messageId)
                            thisCmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = txtMessage.Text.Trim()
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    success = True
                Catch exSql As SqlException
                    If exSql.Number = 2601 OrElse exSql.Number = 2627 Then
                        success = False
                    Else
                        Throw
                    End If
                End Try
            Loop

            ' CREATE TICKET DIRECTORY
            Dim directoryTicket As String = Server.MapPath(String.Format("~/File/Ticket/{0}/", ticketNo))
            If Not Directory.Exists(directoryTicket) Then
                Directory.CreateDirectory(directoryTicket)
            End If

            ' PROCESS ATTACHMENTS
            If fileTicketUpload.HasFiles Then
                ' GENERATE SECOND MESSAGE ID
                ' This message is created specifically for attachments.
                Dim attachmentMessageId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketMessages ORDER BY Id DESC")

                ' INSERT SECOND MESSAGE
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("INSERT INTO TicketMessages VALUES (@Id, @TicketId, 'Customer', @SenderId, @Message, GETDATE(), 0)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", attachmentMessageId)
                        thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                        thisCmd.Parameters.AddWithValue("@SenderId", Session("LoginId").ToString())
                        thisCmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = txtMessage.Text.Trim()
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                ' SAVE EACH FILE
                For Each uploadedFile As HttpPostedFile In fileTicketUpload.PostedFiles
                    If uploadedFile.ContentLength <= 0 Then Continue For

                    ' GENERATE FILE ID
                    Dim fileId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketFiles ORDER BY Id DESC")

                    Dim originalFileName As String = Path.GetFileName(uploadedFile.FileName)
                    Dim extension As String = Path.GetExtension(originalFileName)
                    Dim storedFileName As String = Guid.NewGuid().ToString("N") & extension

                    Dim physicalPath As String = Path.Combine(directoryTicket, storedFileName)
                    Dim relativePath As String = String.Format("~/File/Ticket/{0}/{1}", ticketNo, storedFileName)

                    ' SAVE PHYSICAL FILE
                    uploadedFile.SaveAs(physicalPath)

                    ' INSERT TICKET FILE
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("INSERT INTO TicketFiles (Id, MessageId, FileName, FilePath, FileSize, ContentType) VALUES (@Id, @MessageId, @FileName, @FilePath, @FileSize, @ContentType)", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", fileId)
                            thisCmd.Parameters.AddWithValue("@MessageId", attachmentMessageId)
                            thisCmd.Parameters.AddWithValue("@FileName", originalFileName)
                            thisCmd.Parameters.AddWithValue("@FilePath", relativePath)
                            thisCmd.Parameters.AddWithValue("@FileSize", uploadedFile.ContentLength)
                            thisCmd.Parameters.AddWithValue("@ContentType", uploadedFile.ContentType)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Next
            End If

            Dim mailingClass As New MailingClass
            mailingClass.NewTicket(ticketId)

            ' SUCCESS MODAL
            ClientScript.RegisterStartupScript(Me.GetType(), "ShowSuccessModal", "showSuccessModal('" & ticketNo.Replace("'", "\'") & "', '" & ticketId & "');", True)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName") = "Customer" Then
                MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/ticket", False)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible
        msgError.InnerText = message
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