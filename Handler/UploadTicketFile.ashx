<%@ WebHandler Language="VB" Class="UploadTicketFile" %>

Imports System
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web
Imports System.Web.Script.Serialization

Public Class UploadTicketFile : Implements IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Private ReadOnly myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Private Const MaxFileSize As Integer = 20971520
    Private Const UploadFolder As String = "~/File/Ticket/"

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"
        context.Response.ContentEncoding = System.Text.Encoding.UTF8

        Dim savedFilePath As String = Nothing
        Dim savedPhysicalPath As String = Nothing
        Dim serializer As New JavaScriptSerializer()
        Try
            If Not String.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) Then
                WriteResponse(context, New With {.success = False, .message = "Only POST requests are allowed."})
                Return
            End If

            Dim loginId As Integer = 0

            If context.Session("LoginId") Is Nothing OrElse Not Integer.TryParse(Convert.ToString(context.Session("LoginId")), loginId) OrElse loginId <= 0 Then
                WriteResponse(context, New With {.success = False, .message = "Session expired. Please log in again."})
                Return
            End If

            Dim roleName As String = Convert.ToString(context.Session("RoleName"))
            Dim isInternal As Boolean = Not String.Equals(roleName, "Customer", StringComparison.OrdinalIgnoreCase)

            Dim ticketId As Integer
            If Not Integer.TryParse(context.Request.Form("ticketId"), ticketId) OrElse ticketId <= 0 Then
                WriteResponse(context, New With {.success = False, .message = "Invalid ticket ID."})
                Return
            End If

            If context.Request.Files.Count = 0 Then
                WriteResponse(context, New With {.success = False, .message = "No file received. Please check the upload request."})
                Return
            End If

            Dim uploadedFile As HttpPostedFile = context.Request.Files(0)
            If uploadedFile Is Nothing OrElse uploadedFile.ContentLength <= 0 Then
                WriteResponse(context, New With {.success = False, .message = "The selected file is empty or was not received."})
                Return
            End If

            If uploadedFile.ContentLength > MaxFileSize Then
                WriteResponse(context, New With {.success = False, .message = "Maximum file size is 20 MB."})
                Return
            End If

            Dim originalFileName As String = Path.GetFileName(uploadedFile.FileName)

            If String.IsNullOrWhiteSpace(originalFileName) Then
                WriteResponse(context, New With {.success = False, .message = "Invalid file name."})
                Return
            End If

            Dim extension As String = Path.GetExtension(originalFileName)
            Dim storedFileName As String = Guid.NewGuid().ToString("N") & extension

            Dim ticketNo As String = ""

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim ticketSql As String = "SELECT TicketNo FROM Tickets WHERE Id = @TicketId"

                Using ticketCmd As New SqlCommand(ticketSql, thisConn)
                    ticketCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    Dim ticketResult As Object = ticketCmd.ExecuteScalar()

                    If ticketResult Is Nothing OrElse ticketResult Is DBNull.Value Then
                        WriteResponse(context, New With {.success = False, .message = "Ticket not found."})
                        Return
                    End If

                    ticketNo = Convert.ToString(ticketResult)
                End Using
            End Using

            If String.IsNullOrWhiteSpace(ticketNo) OrElse ticketNo.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 OrElse ticketNo.Contains("..") OrElse ticketNo.Contains("/") OrElse ticketNo.Contains("\") Then
                WriteResponse(context, New With {.success = False, .message = "Invalid ticket number."})
                Return
            End If

            Dim uploadVirtualFolder As String = UploadFolder & ticketNo & "/"
            Dim uploadPhysicalFolder As String = context.Server.MapPath(uploadVirtualFolder)

            If Not Directory.Exists(uploadPhysicalFolder) Then
                Directory.CreateDirectory(uploadPhysicalFolder)
            End If

            savedPhysicalPath = Path.Combine(uploadPhysicalFolder, storedFileName)
            savedFilePath = VirtualPathUtility.ToAbsolute(uploadVirtualFolder & storedFileName)

            Dim senderType As String = If(isInternal, "Internal", "Customer")
            Dim contentType As String = If(String.IsNullOrWhiteSpace(uploadedFile.ContentType), "application/octet-stream", uploadedFile.ContentType)
            Dim fileSize As Long = uploadedFile.ContentLength

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim ticketSql As String = "SELECT Id, LoginId, Status FROM Tickets WHERE Id = @TicketId"

                Using ticketCmd As New SqlCommand(ticketSql, thisConn)
                    ticketCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId

                    Using dr As SqlDataReader = ticketCmd.ExecuteReader()
                        If Not dr.Read() Then
                            WriteResponse(context, New With {.success = False, .message = "Ticket not found."})
                            Return
                        End If

                        Dim ticketLoginId As Integer = Convert.ToInt32(dr("LoginId"))
                        Dim ticketStatus As String = Convert.ToString(dr("Status"))

                        If Not isInternal AndAlso ticketLoginId <> loginId Then
                            WriteResponse(context, New With {.success = False, .message = "You do not have permission to access this ticket."})
                            Return
                        End If

                        If String.Equals(ticketStatus, "Closed", StringComparison.OrdinalIgnoreCase) Then
                            WriteResponse(context, New With {.success = False, .message = "This ticket is closed. You cannot upload files."})
                            Return
                        End If
                    End Using
                End Using
            End Using

            uploadedFile.SaveAs(savedPhysicalPath)

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using thisTrans As SqlTransaction = thisConn.BeginTransaction()
                    Try
                        Dim ticketClass As New TicketClass
                        Dim messageId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketMessages ORDER BY Id DESC")
                        Dim fileId As String = ticketClass.CreateId("SELECT TOP 1 Id FROM TicketFiles ORDER BY Id DESC")

                        Dim messageSql As String = "INSERT INTO TicketMessages VALUES (@Id, @TicketId, @SenderType, @SenderId, @Message, GETDATE(), NULL)"

                        Using messageCmd As New SqlCommand(messageSql, thisConn, thisTrans)
                            messageCmd.Parameters.Add("@Id", SqlDbType.Int).Value = messageId
                            messageCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                            messageCmd.Parameters.Add("@SenderType", SqlDbType.NVarChar, 20).Value = senderType
                            messageCmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = loginId
                            messageCmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = ""
                            messageCmd.Parameters.Add("@ReadBy", SqlDbType.NVarChar, -1).Value = loginId.ToString()

                            messageCmd.ExecuteNonQuery()
                        End Using

                        Dim fileSql As String = "INSERT INTO TicketFiles VALUES (@Id, @TicketId, @MessageId, @SenderType, @SenderId, @FileName, @FilePath, @FileSize, @ContentType, GETDATE())"

                        Using fileCmd As New SqlCommand(fileSql, thisConn, thisTrans)
                            fileCmd.Parameters.Add("@Id", SqlDbType.Int).Value = fileId
                            fileCmd.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                            fileCmd.Parameters.Add("@MessageId", SqlDbType.Int).Value = messageId
                            fileCmd.Parameters.Add("@SenderType", SqlDbType.NVarChar, 20).Value = senderType
                            fileCmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = loginId
                            fileCmd.Parameters.Add("@FileName", SqlDbType.NVarChar, 255).Value = originalFileName
                            fileCmd.Parameters.Add("@FilePath", SqlDbType.NVarChar, 500).Value = savedFilePath
                            fileCmd.Parameters.Add("@FileSize", SqlDbType.BigInt).Value = fileSize
                            fileCmd.Parameters.Add("@ContentType", SqlDbType.NVarChar, 100).Value = contentType

                            fileCmd.ExecuteNonQuery()
                        End Using

                        thisTrans.Commit()

                        WriteResponse(context, New With {
                            .success = True,
                            .message = "File uploaded successfully.",
                            .data = New With {
                                .Id = messageId,
                                .MessageId = messageId,
                                .FileId = fileId,
                                .TicketId = ticketId,
                                .SenderType = senderType,
                                .SenderId = loginId,
                                .MessageType = "File",
                                .FileName = originalFileName,
                                .FilePath = savedFilePath,
                                .FileSize = fileSize,
                                .ContentType = contentType,
                                .CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                .IsMine = True
                            }
                        })
                    Catch
                        Try
                            thisTrans.Rollback()
                        Catch
                        End Try
                        Throw
                    End Try
                End Using
            End Using
        Catch ex As Exception
            If Not String.IsNullOrWhiteSpace(savedPhysicalPath) AndAlso File.Exists(savedPhysicalPath) Then
                Try
                    File.Delete(savedPhysicalPath)
                Catch
                End Try
            End If
            WriteResponse(context, New With {.success = False, .message = "Upload failed: " & ex.Message})
        End Try
    End Sub

    Private Sub WriteResponse(context As HttpContext, data As Object)
        Dim serializer As New JavaScriptSerializer()
        context.Response.Write(serializer.Serialize(data))
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class