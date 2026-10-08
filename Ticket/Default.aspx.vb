Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Script.Services

Partial Class Ticket_Default
    Inherits Page

    Private Shared ReadOnly ConnString As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetTickets() As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()
            Dim result As New List(Of Object)

            Using conn As New SqlConnection(ConnString)
                Dim sql As String = "SELECT T.Id, T.TicketNo, T.LoginId, T.Subject, T.Status, T.Priority, T.CreatedDate, M.LastMessageDate, M.LastMessage, ISNULL(U.UnreadCount, 0) AS UnreadCount FROM ChatTickets T OUTER APPLY (SELECT TOP 1 CM.CreatedDate AS LastMessageDate, CM.Message AS LastMessage FROM ChatTicketMessages CM WHERE CM.TicketId = T.Id ORDER BY CM.CreatedDate DESC, CM.Id DESC) M OUTER APPLY (SELECT COUNT(*) AS UnreadCount FROM ChatTicketMessages CM2 WHERE CM2.TicketId = T.Id AND CM2.IsRead = 0 AND CM2.SenderType = " & If(isInternal, "'Customer'", "'Internal'") & ") U"

                If Not isInternal Then
                    sql &= " WHERE T.LoginId = @LoginId"
                End If

                sql &= " ORDER BY ISNULL(M.LastMessageDate, T.CreatedDate) DESC, T.Id DESC"

                Using cmd As New SqlCommand(sql, conn)
                    If Not isInternal Then
                        cmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    conn.Open()

                    Using dr As SqlDataReader = cmd.ExecuteReader()
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

            Using conn As New SqlConnection(ConnString)
                Dim sql As String = "SELECT CM.Id, CM.TicketId, CM.SenderType, CM.SenderId, CM.Message, CM.CreatedDate, CM.IsRead FROM ChatTicketMessages CM INNER JOIN ChatTickets T ON T.Id = CM.TicketId WHERE CM.TicketId = @TicketId"

                If Not isInternal Then
                    sql &= " AND T.LoginId = @LoginId"
                End If

                sql &= " ORDER BY CM.CreatedDate ASC, CM.Id ASC"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@TicketId", ticketId)
                    If Not isInternal Then
                        cmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    conn.Open()

                    Using dr As SqlDataReader = cmd.ExecuteReader()
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

            Using conn As New SqlConnection(ConnString)
                conn.Open()

                Dim checkSql As String = "SELECT COUNT(*) FROM ChatTickets WHERE Id = @TicketId"

                If Not isInternal Then
                    checkSql &= " AND LoginId = @LoginId"
                End If

                Using checkCmd As New SqlCommand(checkSql, conn)
                    checkCmd.Parameters.AddWithValue("@TicketId", ticketId)

                    If Not isInternal Then
                        checkCmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    Dim exists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                    If exists = 0 Then
                        Return New With {.success = False, .message = "Ticket not found or access denied."}
                    End If
                End Using

                Dim messageId As String = CreateMessageId()

                Using cmd As New SqlCommand("INSERT INTO ChatTicketMessages (Id, TicketId, SenderType, SenderId, Message, CreatedDate, IsRead) VALUES (@Id, @TicketId, @SenderType, @SenderId, @Message, GETDATE(), 0)", conn)
                    cmd.Parameters.AddWithValue("@Id", messageId)
                    cmd.Parameters.AddWithValue("@TicketId", ticketId)
                    cmd.Parameters.AddWithValue("@SenderType", senderType)
                    cmd.Parameters.AddWithValue("@SenderId", senderId)
                    cmd.Parameters.AddWithValue("@Message", message.Trim())
                    cmd.ExecuteNonQuery()
                End Using

                Dim updateSql As String = "UPDATE ChatTickets SET Status = CASE WHEN Status = 'Closed' THEN 'Open' ELSE Status END WHERE Id = @TicketId"

                Using cmd As New SqlCommand(updateSql, conn)
                    cmd.Parameters.AddWithValue("@TicketId", ticketId)
                    cmd.ExecuteNonQuery()
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

            Using conn As New SqlConnection(ConnString)
                Dim sql As String = "UPDATE ChatTickets SET Status = 'Closed', ClosedDate = GETDATE() WHERE Id = @TicketId"

                If Not isInternal Then
                    sql &= " AND LoginId = @LoginId"
                End If

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@TicketId", ticketId)

                    If Not isInternal Then
                        cmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    conn.Open()

                    Dim affected As Integer = cmd.ExecuteNonQuery()

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

            Using conn As New SqlConnection(ConnString)
                Dim sql As String = "UPDATE CM SET IsRead = 1 FROM ChatTicketMessages CM INNER JOIN ChatTickets T ON T.Id = CM.TicketId WHERE CM.TicketId = @TicketId AND CM.IsRead = 0"

                If isInternal Then
                    sql &= " AND CM.SenderType = 'Customer'"
                Else
                    sql &= " AND T.LoginId = @LoginId AND CM.SenderType = 'Internal'"
                End If

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@TicketId", ticketId)

                    If Not isInternal Then
                        cmd.Parameters.AddWithValue("@LoginId", loginId)
                    End If

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch
            ' Ignore read error
        End Try
    End Sub

    ' GET LOGIN ID
    Private Shared Function GetLoginId() As Integer
        If HttpContext.Current.Session("LoginId") Is Nothing Then
            Return 0
        End If

        Return Convert.ToInt32(HttpContext.Current.Session("LoginId"))
    End Function

    ' CHECK INTERNAL
    Private Shared Function IsInternalUser() As Boolean
        Dim roleName As String = Convert.ToString(HttpContext.Current.Session("RoleName"))
        Return Not String.Equals(roleName, "Customer", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function CreateMessageId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(ConnString)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM ChatTicketMessages ORDER BY Id DESC", thisConn)
                    thisConn.Open()
                    Dim lastId As Object = thisCmd.ExecuteScalar()
                    If lastId IsNot Nothing AndAlso Not IsDBNull(lastId) Then
                        result = (CInt(lastId) + 1).ToString()
                    Else
                        result = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function
End Class