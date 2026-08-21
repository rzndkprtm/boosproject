Imports System.Data

Partial Class Setting_Price_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected PriceGroups As Integer
    Protected PriceProductGroups As Integer
    Protected PriceBases As Integer
    Protected PriceSurcharges As Integer
    Protected PriceServices As Integer
    Protected Promos As Integer
    Protected Calculations As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_Price", Nothing)

            If dt.Rows.Count > 0 Then
                PriceGroups = CInt(dt.Rows(0)("PriceGroups"))
                PriceProductGroups = CInt(dt.Rows(0)("PriceProductGroups"))
                PriceBases = CInt(dt.Rows(0)("PriceBases"))
                PriceSurcharges = CInt(dt.Rows(0)("PriceSurcharges"))
                PriceServices = CInt(dt.Rows(0)("PriceServices"))
                Promos = CInt(dt.Rows(0)("Promos"))
                Calculations = CInt(dt.Rows(0)("Calculations"))
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
