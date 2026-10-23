<%@ WebHandler Language="VB" Class="DownloadTicketFile" %>

Imports System
Imports System.Web
Imports System.IO
Imports System.Data

Public Class DownloadTicketFile
    Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            If Not IsNumeric(context.Request.QueryString("id")) Then
                Throw New Exception("Invalid file ID.")
            End If

            Dim messageId As Integer = Convert.ToInt32(context.Request.QueryString("id"))
            Dim settingClass As New SettingClass
            Dim dt As DataTable = settingClass.GetDataTable("SELECT FileName, FilePath, ContentType FROM TicketFiles WHERE MessageId=" & messageId)

            If dt.Rows.Count = 0 Then
                Throw New Exception("File not found.")
            End If

            Dim fileName As String = Path.GetFileName(dt.Rows(0)("FileName").ToString())
            Dim filePath As String = context.Server.MapPath("~" & dt.Rows(0)("FilePath").ToString())
            Dim contentType As String = If(IsDBNull(dt.Rows(0)("ContentType")), "application/octet-stream", dt.Rows(0)("ContentType").ToString())

            If Not File.Exists(filePath) Then
                Throw New Exception("File not found.")
            End If

            context.Response.Clear()
            context.Response.ContentType = contentType
            context.Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName.Replace("""", "") & """")
            context.Response.AddHeader("Content-Length", New FileInfo(filePath).Length.ToString())
            context.Response.TransmitFile(filePath)
            context.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            context.Response.Clear()
            context.Response.StatusCode = 404
            context.Response.ContentType = "text/plain"
            context.Response.Write("File not found.")
            context.ApplicationInstance.CompleteRequest()
        End Try
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class