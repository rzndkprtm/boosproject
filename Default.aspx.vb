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
            secDefault.Visible = True

            Dim companyId As String = String.Empty
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "Factory Office" Then
                companyId = "2"
            End If

            If Session("CompanyId") = "2" Then companyId = "2"

            If Not String.IsNullOrEmpty(companyId) Then
                Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Newsletters WHERE CompanyId='2' AND Active=1")
                If thisData Is Nothing Then
                    Exit Sub
                End If
                imgNewsletter.ImageUrl = thisData("Link").ToString()
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class