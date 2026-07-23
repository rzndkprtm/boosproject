Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Product_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchProductGroup")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchProductGroup") = txtSearch.Text
        Response.Redirect("~/setting/price/product/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrWhiteSpace(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@RoleName", Session("RoleName").ToString())
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_PriceProductGroups_List", params)
            gvList.DataBind()

            gvList.Columns(1).Visible = LoginAccess("Visible ID")
            btnAdd.Visible = LoginAccess("Add")
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

    Protected Function GetCompanyName(dataId As String) As String
        If Not String.IsNullOrEmpty(dataId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceProductGroups.CompanyDetailId, ',') AS companyArray LEFT JOIN CompanyDetails ON companyArray.VALUE=CompanyDetails.Id WHERE PriceProductGroups.Id='" & dataId & "' ORDER BY CompanyDetails.Id ASC")
            Dim hasil As String = String.Empty
            If myData.Rows.Count > 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += designName & ","
                Next
                Return hasil.Remove(hasil.Length - 1).ToString()
            Else
                Return String.Empty
            End If
        End If
        Return "Error"
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
