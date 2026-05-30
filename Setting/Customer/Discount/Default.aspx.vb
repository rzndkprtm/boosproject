Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web.Services

Partial Class Setting_Customer_Discount_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    <WebMethod()>
    Public Shared Function GetCustomerDiscount(customerId As String) As Object
        Dim settingClass As New SettingClass
        Dim dt As DataTable = settingClass.GetDataTable("SELECT Type, DataId, Discount FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END, DataId ASC")

        Dim result As New List(Of Object)
        For Each r As DataRow In dt.Rows
            Dim typeName As String = r("Type").ToString()
            Dim dataId As String = r("DataId").ToString()
            Dim discount As Decimal = Convert.ToDecimal(r("Discount"))
            Dim title As String = GetDiscountTitle(typeName, dataId)
            Dim value As String = If(discount > 0, discount.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%", "-")
            result.Add(New With {.Type = typeName, .Product = title, .Discount = value})
        Next
        Return result
    End Function

    Private Shared Function GetDiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Dim settingClass As New SettingClass
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerDiscount")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerDiscount") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerDiscounts", params)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function DiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function DiscountValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
