Imports System.Data

Partial Class Setting_Specification_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Designs As Integer
    Protected Blinds As Integer
    Protected Products As Integer
    Protected Fabrics As Integer
    Protected Chains As Integer
    Protected Remotes As Integer
    Protected Bottoms As Integer
    Protected Mountings As Integer
    Protected ProductTubes As Integer
    Protected ProductControls As Integer
    Protected ProductColours As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            divAdditional.Visible = False : divHr.Visible = False
            If Session("RoleName") = "Developer" Then divAdditional.Visible = True : divHr.Visible = True

            If Not IsPostBack Then
                Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_Specification", Nothing)

                If dt.Rows.Count > 0 Then
                    Designs = CInt(dt.Rows(0)("Designs"))
                    Blinds = CInt(dt.Rows(0)("Blinds"))
                    Products = CInt(dt.Rows(0)("Products"))
                    Fabrics = CInt(dt.Rows(0)("Fabrics"))
                    Chains = CInt(dt.Rows(0)("Chains"))
                    Remotes = CInt(dt.Rows(0)("Remotes"))
                    Bottoms = CInt(dt.Rows(0)("Bottoms"))
                    Mountings = CInt(dt.Rows(0)("Mountings"))
                    ProductTubes = CInt(dt.Rows(0)("ProductTubes"))
                    ProductControls = CInt(dt.Rows(0)("ProductControls"))
                    ProductColours = CInt(dt.Rows(0)("ProductColours"))
                End If
            End If
        End If
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
