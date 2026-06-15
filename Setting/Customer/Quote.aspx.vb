Imports System.Data

Partial Class Setting_Customer_Quote
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerQuote")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
        Session("SearchCustomerQuote") = txtSearch.Text
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Page" Then
                gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
                BindData(txtSearch.Text)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        Try
            BuildPager()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerQuote") = String.Empty
        Try
            Dim search As String = String.Empty

            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Customers.Id LIKE '%" & searchText.Trim() & "%' OR Customers.Name LIKE '%" & searchText.Trim() & "%' OR Customers.DebtorCode LIKE '%" & searchText.Trim() & "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT CustomerQuotes.*, Customers.Name AS CustomerName FROM CustomerQuotes LEFT JOIN Customers ON CustomerQuotes.Id=Customers.Id {0} ORDER BY Customers.Id ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
                pages.Add(New With {
                    .Text = "Previous",
                    .PageIndex = currentPage - 1,
                    .CssClass = ""
                })
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {
                    .Text = (i + 1).ToString(),
                    .PageIndex = i,
                    .CssClass = If(i = currentPage, "active", "")
                })
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {
                    .Text = "Next",
                    .PageIndex = currentPage + 1,
                    .CssClass = ""
                })
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

    Protected Function BindAddress(customerId As String) As String
        Dim result As String = String.Empty

        If Not String.IsNullOrEmpty(customerId) Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")

            If thisData IsNot Nothing Then
                Dim parts As New List(Of String)

                Dim address As String = thisData("Address").ToString().Trim()
                Dim suburb As String = thisData("Suburb").ToString().Trim()
                Dim state As String = thisData("State").ToString().Trim()
                Dim postCode As String = thisData("PostCode").ToString().Trim()

                If Not String.IsNullOrEmpty(address) Then parts.Add(address)
                If Not String.IsNullOrEmpty(suburb) Then parts.Add(suburb)

                Dim statePostCode As String = (state & " " & postCode).Trim()
                If Not String.IsNullOrEmpty(statePostCode) Then parts.Add(statePostCode)

                result = String.Join(", ", parts)
            End If
        End If

        Return result
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
