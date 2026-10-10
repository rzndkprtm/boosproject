Imports System.Data
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
    Public Shared Function GetTickets(type As String) As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()
            Dim result As New List(Of Object)

            If loginId <= 0 Then
                Return New With {.success = False, .message = "Session expired. Please log in again."}
            End If

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "SELECT T.Id, T.TicketNo, T.LoginId, L.FullName, C.Name AS CustomerName, T.Type, T.Subject, T.Status, T.Priority, T.CreatedDate, M.LastMessageDate, M.LastMessage, ISNULL(U.UnreadCount, 0) AS UnreadCount FROM Tickets T LEFT JOIN Logins L ON T.LoginId = L.Id LEFT JOIN Customers C ON L.CustomerId = C.Id OUTER APPLY (SELECT TOP 1 X.CreatedDate AS LastMessageDate, X.LastMessage FROM (SELECT CM.CreatedDate, CM.Message AS LastMessage, CM.Id, 1 AS SortType FROM TicketMessages CM WHERE CM.TicketId = T.Id UNION ALL SELECT F.CreatedDate, F.FileName AS LastMessage, F.Id, 2 AS SortType FROM TicketFiles F WHERE F.TicketId = T.Id) X ORDER BY X.CreatedDate DESC, X.SortType DESC, X.Id DESC) M OUTER APPLY (SELECT COUNT(*) AS UnreadCount FROM TicketMessages CM2 WHERE CM2.TicketId = T.Id AND CM2.SenderType = @SenderType AND NOT EXISTS (SELECT 1 FROM STRING_SPLIT(ISNULL(CM2.ReadBy, ''), ',') S WHERE LTRIM(RTRIM(S.value)) = CAST(@LoginId AS NVARCHAR(20)))) U WHERE (@Type = '' OR T.Type = @Type)"

                If Not isInternal Then
                    thisSql &= " AND T.LoginId = @LoginId"
                End If

                thisSql &= " ORDER BY ISNULL(M.LastMessageDate, T.CreatedDate) DESC, T.Id DESC"

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
                    thisCmd.Parameters.Add("@SenderType", SqlDbType.NVarChar, 20).Value = If(isInternal, "Customer", "Internal")
                    thisCmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = If(type, "").Trim()

                    thisConn.Open()

                    Using dr As SqlDataReader = thisCmd.ExecuteReader()
                        While dr.Read()
                            result.Add(New With {
                            .Id = Convert.ToInt32(dr("Id")),
                            .TicketNo = dr("TicketNo").ToString(),
                            .LoginId = Convert.ToInt32(dr("LoginId")),
                            .FullName = If(IsDBNull(dr("FullName")), "", dr("FullName").ToString()),
                            .CustomerName = If(IsDBNull(dr("CustomerName")), "", dr("CustomerName").ToString()),
                            .Type = dr("Type").ToString(),
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


    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetMessages(ticketId As Integer) As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()
            Dim result As New List(Of Object)

            If loginId <= 0 Then
                Return New With {.success = False, .message = "Session expired. Please log in again."}
            End If

            If ticketId <= 0 Then
                Return New With {.success = False, .message = "Invalid ticket ID."}
            End If

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim ticketSql As String = "SELECT Id, LoginId, Status FROM Tickets WHERE Id = @TicketId"

                Using ticketCmd As New SqlCommand(ticketSql, thisConn)
                    ticketCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    Using ticketDr As SqlDataReader = ticketCmd.ExecuteReader()
                        If Not ticketDr.Read() Then
                            Return New With {.success = False, .message = "Ticket not found."}
                        End If

                        Dim ticketLoginId As Integer = Convert.ToInt32(ticketDr("LoginId"))

                        If Not isInternal AndAlso ticketLoginId <> loginId Then
                            Return New With {.success = False, .message = "You do not have permission to access this ticket."}
                        End If
                    End Using
                End Using

                Dim thisSql As String = "SELECT CM.Id, CM.TicketId, CM.SenderType, CM.SenderId, CM.Message, CM.CreatedDate, CM.ReadBy, CASE WHEN F.Id IS NOT NULL THEN 'File' ELSE 'Text' END AS MessageType, F.Id AS FileId, F.FileName, F.FilePath, F.FileSize, F.ContentType, L.FullName AS SenderName FROM TicketMessages CM LEFT JOIN TicketFiles F ON F.MessageId = CM.Id LEFT JOIN Logins L ON L.Id = CM.SenderId WHERE CM.TicketId = @TicketId ORDER BY CM.Id ASC"

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    Using dr As SqlDataReader = thisCmd.ExecuteReader()
                        While dr.Read()
                            Dim senderType As String = Convert.ToString(dr("SenderType"))
                            Dim senderId As Integer = If(IsDBNull(dr("SenderId")), 0, Convert.ToInt32(dr("SenderId")))
                            Dim isMine As Boolean = (senderId = loginId)

                            Dim messageType As String = Convert.ToString(dr("MessageType"))
                            Dim fileName As String = If(IsDBNull(dr("FileName")), "", Convert.ToString(dr("FileName")))
                            Dim filePath As String = If(IsDBNull(dr("FilePath")), "", Convert.ToString(dr("FilePath")))
                            Dim fileSize As Long = If(IsDBNull(dr("FileSize")), 0L, Convert.ToInt64(dr("FileSize")))
                            Dim contentType As String = If(IsDBNull(dr("ContentType")), "", Convert.ToString(dr("ContentType")))

                            Dim senderName As String = If(IsDBNull(dr("SenderName")), senderType, Convert.ToString(dr("SenderName")))

                            result.Add(New With {
                            .Id = Convert.ToInt32(dr("Id")),
                            .TicketId = Convert.ToInt32(dr("TicketId")),
                            .SenderType = senderType,
                            .SenderId = senderId,
                            .SenderName = senderName,
                            .Message = If(IsDBNull(dr("Message")), "", Convert.ToString(dr("Message"))),
                            .CreatedDate = Convert.ToDateTime(dr("CreatedDate")).ToString("yyyy-MM-dd HH:mm:ss"),
                            .MessageType = messageType,
                            .FileId = If(IsDBNull(dr("FileId")), CType(Nothing, Integer?), Convert.ToInt32(dr("FileId"))),
                            .FileName = fileName,
                            .FilePath = filePath,
                            .FileSize = fileSize,
                            .ContentType = contentType,
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


    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SendMessage(ticketId As Integer, message As String) As Object
        Try
            If String.IsNullOrWhiteSpace(message) Then
                Return New With {.success = False, .message = "Message cannot be empty."}
            End If

            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            If loginId <= 0 Then
                Return New With {.success = False, .message = "Session expired. Please log in again."}
            End If

            If ticketId <= 0 Then
                Return New With {.success = False, .message = "Invalid ticket ID."}
            End If

            Dim senderType As String = If(isInternal, "Internal", "Customer")
            Dim senderId As Integer = loginId

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                ' VALIDASI TICKET DAN STATUS
                Dim checkSql As String = "SELECT Status FROM Tickets WHERE Id = @TicketId"

                If Not isInternal Then
                    checkSql &= " AND LoginId = @LoginId"
                End If

                Dim ticketStatus As String = ""

                Using thisCmd As New SqlCommand(checkSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    If Not isInternal Then
                        thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
                    End If

                    Dim statusResult As Object = thisCmd.ExecuteScalar()

                    If statusResult Is Nothing OrElse IsDBNull(statusResult) Then
                        Return New With {.success = False, .message = "Ticket not found or access denied."}
                    End If

                    ticketStatus = Convert.ToString(statusResult)
                End Using

                ' CUSTOMER TIDAK BOLEH MENGIRIM PESAN JIKA CLOSED
                If Not isInternal AndAlso String.Equals(ticketStatus, "Closed", StringComparison.OrdinalIgnoreCase) Then
                    Return New With {.success = False, .message = "This ticket is closed. You can no longer send messages."}
                End If

                Dim ticketClass As New TicketClass()
                Dim messageId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketMessages ORDER BY Id DESC")

                Dim insertSql As String = "INSERT INTO TicketMessages (Id, TicketId, SenderType, SenderId, Message, CreatedDate, ReadBy) VALUES (@Id, @TicketId, @SenderType, @SenderId, @Message, GETDATE(), NULL)"

                Using thisCmd As New SqlCommand(insertSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", messageId)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                    thisCmd.Parameters.Add("@SenderType", SqlDbType.NVarChar, 20).Value = senderType
                    thisCmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = senderId
                    thisCmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = message.Trim()
                    thisCmd.ExecuteNonQuery()
                End Using

                ' INTERNAL BOLEH MEMBUKA KEMBALI TICKET CLOSED
                ' CUSTOMER TIDAK AKAN MENGUBAH STATUS CLOSED MENJADI OPEN
                If isInternal Then
                    Dim updateSql As String = "UPDATE Tickets SET Status = CASE WHEN Status = 'Closed' THEN 'Open' ELSE Status END WHERE Id = @TicketId"

                    Using thisCmd As New SqlCommand(updateSql, thisConn)
                        thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                        thisCmd.ExecuteNonQuery()
                    End Using
                End If
            End Using

            Return New With {.success = True}

        Catch ex As Exception
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function


    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CloseTicket(ticketId As Integer) As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            If loginId <= 0 Then
                Return New With {.success = False, .message = "Session expired. Please log in again."}
            End If

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "UPDATE Tickets SET Status = 'Closed', ClosedDate = GETDATE() WHERE Id = @TicketId"

                If Not isInternal Then
                    thisSql &= " AND LoginId = @LoginId"
                End If

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    If Not isInternal Then
                        thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
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

    Private Shared Sub MarkMessagesAsRead(ticketId As Integer)
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            If loginId <= 0 Then Exit Sub

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "UPDATE CM SET ReadBy = CASE WHEN NULLIF(LTRIM(RTRIM(CM.ReadBy)), '') IS NULL THEN CAST(@LoginId AS NVARCHAR(20)) ELSE CM.ReadBy + ',' + CAST(@LoginId AS NVARCHAR(20)) END FROM TicketMessages CM INNER JOIN Tickets T ON T.Id = CM.TicketId WHERE CM.TicketId = @TicketId AND (CM.ReadBy IS NULL OR NOT EXISTS (SELECT 1 FROM STRING_SPLIT(CM.ReadBy, ',') S WHERE LTRIM(RTRIM(S.value)) = CAST(@LoginId AS NVARCHAR(20))))"

                If isInternal Then
                    thisSql &= " AND CM.SenderType = 'Customer'"
                Else
                    thisSql &= " AND T.LoginId = @LoginId AND CM.SenderType = 'Internal'"
                End If

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                    thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId

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