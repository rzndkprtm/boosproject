Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web.Services
Imports System.Windows

Partial Class Setting_Customer_Markup_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    <WebMethod()>
    Public Shared Function GetCustomerMarkup(customerId As String) As Object
        Dim settingClass As New SettingClass
        Dim dt As DataTable = settingClass.GetDataTable("SELECT * FROM CustomerMarkups WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END, DataId ASC")

        Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & customerId & "'")

        Dim result As New List(Of Object)

        For Each r As DataRow In dt.Rows
            Dim type As String = r("Type").ToString()
            Dim method As String = r("Method").ToString()
            Dim dataId As String = r("DataId").ToString()
            Dim title As String = GetMarkupTitle(type, dataId)

            Dim markup1 As Decimal = If(IsDBNull(r("Markup1")), 0D, Convert.ToDecimal(r("Markup1")))
            Dim markup2 As Decimal = If(IsDBNull(r("Markup2")), 0D, Convert.ToDecimal(r("Markup2")))
            Dim markup3 As Decimal = If(IsDBNull(r("Markup3")), 0D, Convert.ToDecimal(r("Markup3")))
            Dim markup4 As Decimal = If(IsDBNull(r("Markup4")), 0D, Convert.ToDecimal(r("Markup4")))
            Dim markup5 As Decimal = If(IsDBNull(r("Markup5")), 0D, Convert.ToDecimal(r("Markup5")))

            Dim markupValue1 As String = "-"
            Dim markupValue2 As String = "-"
            Dim markupValue3 As String = "-"
            Dim markupValue4 As String = "-"
            Dim markupValue5 As String = "-"

            If method = "Percent" Then
                markupValue1 = markup1.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%"
                markupValue2 = markup2.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%"
                markupValue3 = markup3.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%"
                markupValue4 = markup4.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%"
                markupValue5 = markup5.ToString("G29", CultureInfo.GetCultureInfo("en-US")) & "%"
            End If

            If method = "Value" Then
                If companyId = "2" Then
                    markupValue1 = "$" & markup1.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue2 = "$" & markup2.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue3 = "$" & markup3.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue4 = "$" & markup4.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue5 = "$" & markup5.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                End If

                If companyId = "3" Then
                    markupValue1 = "Rp" & markup1.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue2 = "Rp" & markup2.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue3 = "Rp" & markup3.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue4 = "Rp" & markup4.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                    markupValue5 = "Rp" & markup5.ToString("G29", CultureInfo.GetCultureInfo("en-US"))
                End If
            End If

            result.Add(New With {
                .Id = r("Id").ToString(),
                .Product = title,
                .Markup1 = markupValue1,
                .Markup2 = markupValue2,
                .Markup3 = markupValue3,
                .Markup4 = markupValue4,
                .Markup5 = markupValue5
            })
        Next
        Return result
    End Function

    Private Shared Function GetMarkupTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Dim settingClass As New SettingClass

        Dim dataName As String = String.Empty
        If type = "Designs" Then
            dataName = settingClass.GetItemData("Select Name FROM Designs WHERE Id='" & dataId & "'")
        End If
        If type = "Blinds" Then
            dataName = settingClass.GetItemData("Select Name FROM Blinds WHERE Id='" & dataId & "'")
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
        If type = "RollerChains" Then
            dataName = settingClass.GetItemData("SELECT Name FROM Chains WHERE Id='" & dataId & "'")
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
            txtSearch.Text = Session("SearchCustomerMarkup")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchCustomerMarkup") = txtSearch.Text
        Response.Redirect("~/setting/customer/markup/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchCustomerMarkup") = txtSearch.Text
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

    Protected Sub btnAddMarkup_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/customer/markup/add?custid={0}", txtCustomerId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerMarkup") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerMarkups_List", params)
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
