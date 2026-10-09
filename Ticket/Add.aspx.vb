Imports System.Data.SqlClient
Imports System.IO

Partial Class Ticket_Add
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim ticketClass As New TicketClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If
            If txtSubject.Text = "" Then
                MessageError(True, "SUBJECT IS REQUIRED !")
                Exit Sub
            End If
            If txtMessage.Text = "" Then
                MessageError(True, "MESSAGE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim chatTicketId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM Tickets ORDER BY Id DESC")
                Dim chatTicketMsgId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketMessages ORDER BY Id DESC")

                Dim success As Boolean = False
                Dim retry As Integer = 0
                Dim maxRetry As Integer = 100
                Dim ticketNo As String = String.Empty

                Do While Not success
                    retry += 1
                    If retry > maxRetry Then
                        Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
                    End If

                    Dim randomCode As String = ticketClass.GenerateRandomCode()
                    ticketNo = String.Format("BOOS{0}", randomCode)
                    Try
                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand("INSERT INTO Tickets VALUES (@Id, @TicketNo, @LoginId, @Type, @Subject, 'Open', 'Normal', GETDATE(), NULL); INSERT INTO TicketMessages VALUES (@ChatMessageId, @Id, 'Customer', @LoginId, @Message, GETDATE(), 0);", thisConn)
                                thisCmd.Parameters.AddWithValue("@Id", chatTicketId)
                                thisCmd.Parameters.AddWithValue("@TicketNo", ticketNo)
                                thisCmd.Parameters.AddWithValue("@LoginId", Session("LoginId").ToString())
                                thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                                thisCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())
                                thisCmd.Parameters.AddWithValue("@ChatMessageId", chatTicketMsgId)
                                thisCmd.Parameters.AddWithValue("@Message", txtMessage.Text)

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

                Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Ticket/{0}/", ticketNo))
                If Not Directory.Exists(directoryOrder) Then
                    Directory.CreateDirectory(directoryOrder)
                End If

                Response.Redirect("~/ticket", False)
            End If
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
