Partial Class Contact
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MessageError(False, String.Empty)

            divSales.Visible = False : divCSJPMD.Visible = False
            If Session("RoleName") = "Customer" Then
                If Session("CompanyId") = "2" Then
                    divSales.Visible = True : divCSJPMD.Visible = True
                End If
            End If
        End If
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub
End Class