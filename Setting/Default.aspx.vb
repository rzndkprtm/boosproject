Partial Class Setting_Default
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        divGeneral.Attributes("onclick") = "location.href='general'"
        divCustomer.Attributes("onclick") = "location.href='customer'"
        divSpecification.Attributes("onclick") = "location.href='specification'"
        divPrice.Attributes("onclick") = "location.href='price'"
        divAdditional.Attributes("onclick") = "location.href='additional'"
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
