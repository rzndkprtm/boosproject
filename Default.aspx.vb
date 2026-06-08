Imports System.Data

Partial Class _Default
    Inherits Page
    
    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Protected Sub BindData()
        Try
            If Session("CompanyId") = "3" Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            secDefault.Visible = True

            Dim companyId As String = If(Session("CompanyId"), "").ToString()
            If String.IsNullOrEmpty(companyId) OrElse companyId = "2" Then
                Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Newsletters WHERE CompanyId='2' AND Active=1")
                If thisData Is Nothing Then
                    Exit Sub
                End If
                imgNewsletter.ImageUrl = thisData("Link").ToString()
                If Session("CustomerId") = "127" Then
                    Response.Redirect("~/order", False)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class