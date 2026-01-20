Imports System.Data

Partial Class _Default
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Protected Sub BindData()
        Try

            If Session("CompanyId") = "3" OrElse Session("CompanyId") = "5" Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Newsletters WHERE CompanyId='" & Session("CompanyId") & "' Active=1")
            If thisData Is Nothing Then
                Exit Sub
            End If

            imgNewsletter.ImageUrl = thisData("Link").ToString()
        Catch ex As Exception
        End Try
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class