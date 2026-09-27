Imports System.Web.Services

Partial Class Setting_UpdateSession
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session("KeepAlive") = 1
    End Sub

    <WebMethod()>
    Public Shared Function UpdateCurrentPage(page As String) As String
        Dim context As HttpContext = HttpContext.Current

        If context.Session("LoginId") Is Nothing Then
            Return "NO_LOGIN"
        End If

        Dim loginId As String = context.Session("LoginId").ToString()

        context.Application("UserPage_" & loginId) = page
        context.Application("UserPageTime_" & loginId) = DateTime.Now

        Return "OK"
    End Function

    <WebMethod()>
    Public Shared Function GetCurrentPage(loginId As String) As String
        Dim context As HttpContext = HttpContext.Current

        Dim page As String = TryCast(context.Application("UserPage_" & loginId), String)

        Dim lastActivity As DateTime = DateTime.MinValue

        If context.Application("UserPageTime_" & loginId) IsNot Nothing Then
            lastActivity = CType(context.Application("UserPageTime_" & loginId), DateTime)
        End If


        Dim isOnline As Boolean = page IsNot Nothing AndAlso lastActivity <> DateTime.MinValue AndAlso DateTime.Now.Subtract(lastActivity).TotalSeconds <= 10

        If Not isOnline Then
            Return "<div class='text-danger fw-bold'>OFFLINE</div>" &
                   "<div class='mt-2'>User is not currently active.</div>"
        End If

        Return "<div class='text-success fw-bold'>ONLINE</div>" &
               "<div class='mt-3'>" &
               "<strong>Current Page:</strong><br>" &
               HttpUtility.HtmlEncode(page) &
               "</div>" &
               "<div class='mt-3'>" &
               "<strong>Last Activity:</strong> " &
               lastActivity.ToString("HH:mm:ss") &
               "</div>"
    End Function
End Class
