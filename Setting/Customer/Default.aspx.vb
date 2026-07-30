Imports System.Data

Partial Class Setting_Customer_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Customers As Integer
    Protected CustomerContacts As Integer
    Protected CustomerAddress As Integer
    Protected CustomerBusiness As Integer
    Protected CustomerLogins As Integer
    Protected CustomerMarkups As Integer
    Protected CustomerDiscounts As Integer
    Protected CustomerPromos As Integer
    Protected CustomerProductAccess As Integer
    Protected CustomerQuotes As Integer
    Protected CustomerCustomPricings As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_Customer", Nothing)

            If dt.Rows.Count > 0 Then
                Customers = CInt(dt.Rows(0)("Customers"))
                CustomerContacts = CInt(dt.Rows(0)("CustomerContacts"))
                CustomerAddress = CInt(dt.Rows(0)("CustomerAddress"))
                CustomerBusiness = CInt(dt.Rows(0)("CustomerBusiness"))
                CustomerLogins = CInt(dt.Rows(0)("CustomerLogins"))
                CustomerMarkups = CInt(dt.Rows(0)("CustomerMarkups"))
                CustomerDiscounts = CInt(dt.Rows(0)("CustomerDiscounts"))
                CustomerPromos = CInt(dt.Rows(0)("CustomerPromos"))
                CustomerProductAccess = CInt(dt.Rows(0)("CustomerProductAccess"))
                CustomerQuotes = CInt(dt.Rows(0)("CustomerQuotes"))
                CustomerCustomPricings = CInt(dt.Rows(0)("CustomerCustomPricings"))
            End If
        End If
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
