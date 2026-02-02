<%@ WebHandler Language="VB" Class="Download" %>

Imports System
Imports System.Web
Imports System.IO

Public Class Download : Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim folder As String = context.Request("folder")
        Dim file As String = context.Request("file")

        If String.IsNullOrEmpty(folder) OrElse String.IsNullOrEmpty(file) Then
            context.Response.StatusCode = 400
            context.Response.End()
            Return
        End If

        Dim fullPath As String = context.Server.MapPath("~/File/Order/" & folder & "/" & file)

        If Not IO.File.Exists(fullPath) Then
            context.Response.StatusCode = 404
            context.Response.End()
            Return
        End If

        Dim ext = Path.GetExtension(fullPath).ToLower()
        Dim contentType As String = "application/octet-stream"

        Select Case ext
            Case ".jpg", ".jpeg" : contentType = "image/jpeg"
            Case ".png" : contentType = "image/png"
            Case ".pdf" : contentType = "application/pdf"
            Case ".docx" : contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Case ".xlsx" : contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case ".zip" : contentType = "application/zip"
        End Select

        context.Response.Clear()
        context.Response.ContentType = contentType
        context.Response.AddHeader("Content-Disposition", "attachment; filename=""" & file & """")
        context.Response.TransmitFile(fullPath)
        context.Response.End()
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class