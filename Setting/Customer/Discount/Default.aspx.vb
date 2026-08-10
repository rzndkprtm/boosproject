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
        Dim dt As DataTable = settingClass.GetDataTable("SELECT Id, Type, DataId, Discount FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END, DataId ASC")

        Dim result As New List(Of Object)
        For Each r As DataRow In dt.Rows
            Dim typeName As String = r("Type").ToString()
            Dim dataId As String = r("DataId").ToString()
            Dim discount As Decimal = Convert.ToDecimal(r("Discount"))
            Dim title As String = GetDiscountTitle(typeName, dataId)
            Dim value As String = If(discount > 0, discount.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%", "-")
            result.Add(New With {.Id = r("Id").ToString(), .Type = typeName, .Product = title, .Discount = value})
        Next
        Return result
    End Function

    Private Shared Function GetDiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Dim settingClass As New SettingClass

        Dim dataName As String = String.Empty
        If type = "Designs" Then
            dataName = settingClass.GetItemData("SELECT Name FROM Designs WHERE Id='" & dataId & "'")
        End If
        If type = "PriceProductGroups" Then
            dataName = settingClass.GetItemData("SELECT CASE WHEN Status='Active' THEN Name ELSE Name + ' [' + UPPER(Status) + ']' END FROM PriceProductGroups WHERE Id='" & dataId & "'")
        End If
        If type = "RollerFabrics" OrElse type = "RomanFabrics" OrElse type = "PanelGlideFabrics" Then
            dataName = settingClass.GetItemData("SELECT Name FROM Fabrics WHERE Id='" & dataId & "'")
        End If
        If type = "RollerFabricColours" OrElse type = "RomanFabricColours" OrElse type = "PanelGlideFabricColours" Then
            dataName = settingClass.GetItemData("SELECT Name FROM FabricColours WHERE Id='" & dataId & "'")
        End If
        Return dataName
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
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchCustomerDiscount") = txtSearch.Text
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnAddDiscountA_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=Designs", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountB_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=PriceProductGroups", txtCustomerId.Text)
        Response.Redirect(Url, False)
    End Sub

    Protected Sub btnAddDiscountC_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=RollerFabrics", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountD_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=RollerFabricColours", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountE_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=RomanFabrics", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountF_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=RomanFabricColours", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountG_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=PanelGlideFabrics", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountH_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/discount/add?custid={0}&type=PanelGlideFabricColours", txtCustomerId.Text)
        Response.Redirect(url, False)
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

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerDiscounts_List", params)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {.Text = "Previous", .PageIndex = currentPage - 1, .CssClass = ""})
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {.Text = (i + 1).ToString(), .PageIndex = i, .CssClass = If(i = currentPage, "active", "")})
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {.Text = "Next", .PageIndex = currentPage + 1, .CssClass = ""})
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

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
