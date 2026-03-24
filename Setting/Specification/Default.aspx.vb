Partial Class Setting_Specification_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        divDesignType.Attributes("onclick") = "location.href='designtype'"
        divBlindType.Attributes("onclick") = "location.href='blindtype'"
        divProduct.Attributes("onclick") = "location.href='product'"
        divFabric.Attributes("onclick") = "location.href='fabric'"
        divChain.Attributes("onclick") = "location.href='chain'"
        divRemote.Attributes("onclick") = "location.href='remote'"
        divBottom.Attributes("onclick") = "location.href='bottom'"
        divMounting.Attributes("onclick") = "location.href='mounting'"
    End Sub

    Protected Function GetSumData(params As String) As String
        Try
            If Not String.IsNullOrEmpty(params) Then
                Dim thisQuery As String = String.Format("SELECT COUNT(*) FROM {0}", params)
                If params = "Chains" Then
                    thisQuery = "SELECT COUNT(*) FROM Chains WHERE ControlTypeId='1'"
                End If
                If params = "Remotes" Then
                    thisQuery = "SELECT COUNT(DISTINCT Chains.Id) FROM Chains CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray WHERE controlArray.VALUE<>'1'"
                End If
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
