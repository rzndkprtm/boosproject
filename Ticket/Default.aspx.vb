Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Script.Services

Partial Class Ticket_Default
    Inherits Page

    Private Shared ReadOnly myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If
        If Not IsPostBack Then
            btnAdd.Visible = LoginAccess("Add")
        End If
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

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/ticket/add", False)
    End Sub


    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetTickets() As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()
            Dim result As New List(Of Object)

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "SELECT T.Id, T.TicketNo, T.LoginId, T.Subject, T.Status, T.Priority, T.CreatedDate, M.LastMessageDate, M.LastMessage, ISNULL(U.UnreadCount, 0) AS UnreadCount FROM ChatTickets T OUTER APPLY (SELECT TOP 1 CM.CreatedDate AS LastMessageDate, CM.Message AS LastMessage FROM ChatTicketMessages CM WHERE CM.TicketId = T.Id ORDER BY CM.CreatedDate DESC, CM.Id DESC) M OUTER APPLY (SELECT COUNT(*) AS UnreadCount FROM ChatTicketMessages CM2 WHERE CM2.TicketId=T.Id AND CM2.IsRead=0 AND CM2.SenderType=" & If(isInternal, "'Customer'", "'Internal'") & ") U"

                If Not isInternal Then
                    thisSql &= " WHERE T.LoginId=@LoginId"
                End If

                thisSql &= " ORDER BY ISNULL(M.LastMessageDate, T.CreatedDate) DESC, T.Id DESC"

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    If Not isInternal Then
                        thisCmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    thisConn.Open()

                    Using dr As SqlDataReader = thisCmd.ExecuteReader()
                        While dr.Read()
                            result.Add(New With {
                                .Id = Convert.ToInt32(dr("Id")),
                                .TicketNo = dr("TicketNo").ToString(),
                                .LoginId = Convert.ToInt32(dr("LoginId")),
                                .Subject = dr("Subject").ToString(),
                                .Status = dr("Status").ToString(),
                                .Priority = dr("Priority").ToString(),
                                .CreatedDate = Convert.ToDateTime(dr("CreatedDate")).ToString("yyyy-MM-dd HH:mm:ss"),
                                .LastMessageDate = If(IsDBNull(dr("LastMessageDate")), Nothing, Convert.ToDateTime(dr("LastMessageDate")).ToString("yyyy-MM-dd HH:mm:ss")),
                                .LastMessage = If(IsDBNull(dr("LastMessage")), "", dr("LastMessage").ToString()),
                                .UnreadCount = Convert.ToInt32(dr("UnreadCount"))
                            })
                        End While
                    End Using
                End Using
            End Using

            Return New With {.success = True, .data = result}
        Catch ex As Exception
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

    ' GET MESSAGES
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetMessages(ticketId As Integer) As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()
            Dim result As New List(Of Object)

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "SELECT CM.Id, CM.TicketId, CM.SenderType, CM.SenderId, CM.Message, CM.CreatedDate, CM.IsRead, L.FullName FROM ChatTicketMessages CM INNER JOIN ChatTickets T ON T.Id=CM.TicketId INNER JOIN Logins L ON CM.SenderId=L.Id WHERE CM.TicketId=@TicketId"

                If Not isInternal Then
                    thisSql &= " AND T.LoginId=@LoginId"
                End If

                thisSql &= " ORDER BY CM.CreatedDate ASC, CM.Id ASC"

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                    If Not isInternal Then
                        thisCmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    thisConn.Open()

                    Using dr As SqlDataReader = thisCmd.ExecuteReader()
                        While dr.Read()
                            Dim senderType As String = dr("SenderType").ToString()
                            Dim senderId As Integer = Convert.ToInt32(dr("SenderId"))
                            Dim isMine As Boolean

                            If isInternal Then
                                isMine = senderType = "Internal" AndAlso senderId = loginId
                            Else
                                isMine = senderType = "Customer" AndAlso senderId = loginId
                            End If

                            result.Add(New With {
                                .Id = Convert.ToInt32(dr("Id")),
                                .TicketId = Convert.ToInt32(dr("TicketId")),
                                .SenderType = senderType,
                                .SenderId = senderId,
                                .SenderName = dr("FullName").ToString(),
                                .Message = dr("Message").ToString(),
                                .CreatedDate = Convert.ToDateTime(dr("CreatedDate")).ToString("yyyy-MM-dd HH:mm:ss"),
                                .IsRead = Convert.ToBoolean(dr("IsRead")),
                                .IsMine = isMine
                            })
                        End While
                    End Using
                End Using
            End Using

            MarkMessagesAsRead(ticketId)

            Return New With {.success = True, .data = result}
        Catch ex As Exception
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

    ' SEND MESSAGE
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SendMessage(ticketId As Integer, message As String) As Object
        Try
            If String.IsNullOrWhiteSpace(message) Then
                Return New With {.success = False, .message = "Message cannot be empty."}
            End If

            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            Dim senderType As String = If(isInternal, "Internal", "Customer")
            Dim senderId As Integer = If(isInternal, loginId, loginId)

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim checkSql As String = "SELECT COUNT(*) FROM ChatTickets WHERE Id=@TicketId"
                If Not isInternal Then
                    checkSql &= " AND LoginId=@LoginId"
                End If

                Using thisCmd As New SqlCommand(checkSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                    If Not isInternal Then
                        thisCmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    Dim exists As Integer = Convert.ToInt32(thisCmd.ExecuteScalar())
                    If exists = 0 Then
                        Return New With {.success = False, .message = "Ticket not found or access denied."}
                    End If
                End Using

                Dim ticketClass As New TicketClass

                Dim messageId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM ChatTicketMessages ORDER BY Id DESC")

                Using thisCmd As New SqlCommand("INSERT INTO ChatTicketMessages (Id, TicketId, SenderType, SenderId, Message, CreatedDate, IsRead) VALUES (@Id, @TicketId, @SenderType, @SenderId, @Message, GETDATE(), 0)", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", messageId)
                    thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                    thisCmd.Parameters.AddWithValue("@SenderType", senderType)
                    thisCmd.Parameters.AddWithValue("@SenderId", senderId)
                    thisCmd.Parameters.AddWithValue("@Message", message.Trim())
                    thisCmd.ExecuteNonQuery()
                End Using

                Dim updateSql As String = "UPDATE ChatTickets SET Status=CASE WHEN Status = 'Closed' THEN 'Open' ELSE Status END WHERE Id=@TicketId"

                Using thisCmd As New SqlCommand(updateSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Return New With {.success = True}
        Catch ex As Exception
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

    ' CLOSE TICKET
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CloseTicket(ticketId As Integer) As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "UPDATE ChatTickets SET Status='Closed', ClosedDate=GETDATE() WHERE Id=@TicketId"

                If Not isInternal Then
                    thisSql &= " AND LoginId=@LoginId"
                End If

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                    If Not isInternal Then
                        thisCmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    thisConn.Open()

                    Dim affected As Integer = thisCmd.ExecuteNonQuery()
                    If affected = 0 Then
                        Return New With {.success = False, .message = "Ticket not found or access denied."}
                    End If
                End Using
            End Using

            Return New With {.success = True}
        Catch ex As Exception
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

    ' MARK AS READ
    Private Shared Sub MarkMessagesAsRead(ticketId As Integer)
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "UPDATE CM SET IsRead=1 FROM ChatTicketMessages CM INNER JOIN ChatTickets T ON T.Id=CM.TicketId WHERE CM.TicketId=@TicketId AND CM.IsRead=0"

                If isInternal Then
                    thisSql &= " AND CM.SenderType='Customer'"
                Else
                    thisSql &= " AND T.LoginId=@LoginId AND CM.SenderType='Internal'"
                End If

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@TicketId", ticketId)
                    If Not isInternal Then
                        thisCmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch
        End Try
    End Sub

    ' GET LOGIN ID
    Private Shared Function GetLoginId() As Integer
        If HttpContext.Current.Session("LoginId") Is Nothing Then Return 0
        Return Convert.ToInt32(HttpContext.Current.Session("LoginId"))
    End Function

    ' CHECK INTERNAL
    Private Shared Function IsInternalUser() As Boolean
        Dim roleName As String = Convert.ToString(HttpContext.Current.Session("RoleName"))
        Return Not String.Equals(roleName, "Customer", StringComparison.OrdinalIgnoreCase)
    End Function
End Class