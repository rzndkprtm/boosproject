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
    Public Shared Function GetTickets() As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()
            Dim result As New List(Of Object)

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "SELECT T.Id, T.TicketNo, T.LoginId, T.Subject, T.Status, T.Priority, T.CreatedDate, M.LastMessageDate, M.LastMessage, ISNULL(U.UnreadCount, 0) AS UnreadCount FROM ChatTickets T OUTER APPLY (SELECT TOP 1 CM.CreatedDate AS LastMessageDate, CM.Message AS LastMessage FROM ChatTicketMessages CM WHERE CM.TicketId = T.Id ORDER BY CM.CreatedDate DESC, CM.Id DESC) M OUTER APPLY (SELECT COUNT(*) AS UnreadCount FROM ChatTicketMessages CM2 WHERE CM2.TicketId = T.Id AND CM2.SenderType = @SenderType AND NOT EXISTS (SELECT 1 FROM STRING_SPLIT(ISNULL(CM2.ReadBy, ''), ',') S WHERE LTRIM(RTRIM(S.value)) = CAST(@LoginId AS NVARCHAR(20)))) U"

                If Not isInternal Then
                    thisSql &= " WHERE T.LoginId = @LoginId"
                End If

                thisSql &= " ORDER BY ISNULL(M.LastMessageDate, T.CreatedDate) DESC, T.Id DESC"

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
                    thisCmd.Parameters.Add("@SenderType", SqlDbType.NVarChar, 20).Value = If(isInternal, "Customer", "Internal")

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


    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetMessages(ticketId As Integer) As Object
        Try
            Dim loginId As Integer = GetLoginId()
            Dim isInternal As Boolean = IsInternalUser()

            If loginId <= 0 Then
                Return New With {.success = False, .message = "Session expired. Please log in again."}
            End If

            Dim result As New List(Of Object)

            Using thisConn As New SqlConnection(myConn)
                Dim thisSql As String = "SELECT CM.Id, CM.TicketId, CM.SenderType, CM.SenderId, CM.Message, CM.CreatedDate, CM.ReadBy, CASE WHEN CM.SenderType = 'Customer' THEN L.FullName ELSE LR.Name + ' - ' + L.FullName END AS SenderName FROM ChatTicketMessages CM INNER JOIN ChatTickets T ON T.Id = CM.TicketId INNER JOIN Logins L ON CM.SenderId = L.Id INNER JOIN LoginRoles LR ON L.RoleId = LR.Id WHERE CM.TicketId = @TicketId"

                If Not isInternal Then
                    thisSql &= " AND T.LoginId = @LoginId"
                End If

                thisSql &= " ORDER BY CM.CreatedDate ASC, CM.Id ASC"

                Using thisCmd As New SqlCommand(thisSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    If Not isInternal Then
                        thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
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

                            Dim readBy As String = If(IsDBNull(dr("ReadBy")), "", dr("ReadBy").ToString())
                            Dim isRead As Boolean = readBy.Split(","c).Any(Function(x) x.Trim() = loginId.ToString())

                            result.Add(New With {
                            .Id = Convert.ToInt32(dr("Id")),
                            .TicketId = Convert.ToInt32(dr("TicketId")),
                            .SenderType = senderType,
                            .SenderId = senderId,
                            .SenderName = dr("SenderName").ToString(),
                            .Message = dr("Message").ToString(),
                            .CreatedDate = Convert.ToDateTime(dr("CreatedDate")).ToString("yyyy-MM-dd HH:mm:ss"),
                            .IsRead = isRead,
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

            Dim senderType As String = If(isInternal, "Internal", "Customer")
            Dim senderId As Integer = loginId

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim checkSql As String = "SELECT COUNT(*) FROM ChatTickets WHERE Id = @TicketId"

                If Not isInternal Then
                    checkSql &= " AND LoginId = @LoginId"
                End If

                Using thisCmd As New SqlCommand(checkSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    If Not isInternal Then
                        thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
                    End If

                    Dim exists As Integer = Convert.ToInt32(thisCmd.ExecuteScalar())

                    If exists = 0 Then
                        Return New With {.success = False, .message = "Ticket not found or access denied."}
                    End If
                End Using

                Dim ticketClass As New TicketClass()
                Dim messageId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM ChatTicketMessages ORDER BY Id DESC")

                Dim insertSql As String = "INSERT INTO ChatTicketMessages (Id, TicketId, SenderType, SenderId, Message, CreatedDate, ReadBy) VALUES (@Id, @TicketId, @SenderType, @SenderId, @Message, GETDATE(), NULL)"

                Using thisCmd As New SqlCommand(insertSql, thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", messageId)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                    thisCmd.Parameters.Add("@SenderType", SqlDbType.NVarChar, 20).Value = senderType
                    thisCmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = senderId
                    thisCmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = message.Trim()
                    thisCmd.ExecuteNonQuery()
                End Using

                Dim updateSql As String = "UPDATE ChatTickets SET Status = CASE WHEN Status = 'Closed' THEN 'Open' ELSE Status END WHERE Id = @TicketId"

                Using thisCmd As New SqlCommand(updateSql, thisConn)
                    thisCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                    thisCmd.ExecuteNonQuery()
                End Using
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
                Dim thisSql As String = "UPDATE ChatTickets SET Status = 'Closed', ClosedDate = GETDATE() WHERE Id = @TicketId"

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
                Dim thisSql As String = "UPDATE CM SET ReadBy = CASE WHEN NULLIF(LTRIM(RTRIM(CM.ReadBy)), '') IS NULL THEN CAST(@LoginId AS NVARCHAR(20)) ELSE CM.ReadBy + ',' + CAST(@LoginId AS NVARCHAR(20)) END FROM ChatTicketMessages CM INNER JOIN ChatTickets T ON T.Id = CM.TicketId WHERE CM.TicketId = @TicketId AND (CM.ReadBy IS NULL OR NOT EXISTS (SELECT 1 FROM STRING_SPLIT(CM.ReadBy, ',') S WHERE LTRIM(RTRIM(S.value)) = CAST(@LoginId AS NVARCHAR(20))))"

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