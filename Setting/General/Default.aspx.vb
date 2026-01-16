Partial Class Setting_General_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        divCompany.Attributes("onclick") = "location.href='company'"
        divMailing.Attributes("onclick") = "location.href='mailing'"
        divRoleAccess.Attributes("onclick") = "location.href='role'"
        divLevelAccess.Attributes("onclick") = "location.href='level'"
        divNewsletter.Attributes("onclick") = "location.href='newsletter'"
        divTutorial.Attributes("onclick") = "location.href='tutorial'"
        divActionAccess.Attributes("onclick") = "location.href='action'"
    End Sub

    Protected Function GetSumData(params As String) As String
        Try
            If Not String.IsNullOrEmpty(params) Then
                Dim thisQuery As String = String.Format("SELECT COUNT(*) FROM {0}", params)
                Dim sumData As Integer = settingClass.GetItemData_Integer(thisQuery)
                Return sumData & " Data"
            End If
            Return String.Empty
        Catch ex As Exception
            Return ex.ToString()
        End Try
    End Function

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
