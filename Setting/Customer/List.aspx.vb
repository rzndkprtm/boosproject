Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_List
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomer")

            BindCompany()
            BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        gvList.PageIndex = 0
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        gvList.PageIndex = 0
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        gvList.PageIndex = 0
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchCustomer") = txtSearch.Text
        Session("CompanyCustomer") = ddlCompany.SelectedValue
        Response.Redirect("~/setting/customer/add", False)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Page" Then
                gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
                BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        Try
            BuildPager()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Session("SearchCustomer") = txtSearch.Text
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                url = String.Format("~/setting/customer/detail?customerid={0}", dataId)
                Response.Redirect(url, False)
            End If
            If e.CommandName = "Ubah" Then
                url = String.Format("~/setting/customer/edit?customerid={0}", dataId)
                Response.Redirect(url, False)
            End If
        End If
    End Sub

    Protected Sub btnCashSale_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdCashSale.Text
            Dim newData As String = ddlNewCashSale.SelectedValue
            Dim oldData As String = txtOldCashSale.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET CashSale=@CashSale WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@CashSale", newData)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Cash Sale : {0}", newData)
            dataLog = {"Customers", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Session("SearchCustomer") = txtSearch.Text
            Response.Redirect("~/setting/customer/list", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnOnStop_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdOnStop.Text
            Dim newData As String = ddlNewOnStop.SelectedValue
            Dim oldData As String = txtOldOnStop.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET OnStop=@OnStop WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@OnStop", newData)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change On Stop : {0}", newData)
            dataLog = {"Customers", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Session("SearchCustomer") = txtSearch.Text
            Response.Redirect("~/setting/customer/list", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Active=0 WHERE Id=@Id UPDATE Logins SET Active=0 WHERE CustomerId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", dataId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchCustomer") = txtSearch.Text
            Response.Redirect("~/setting/customer/list", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, companyText As String, activeText As String)
        Session("SearchCustomer") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Active", ddlActive.SelectedValue),
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyText), CType(DBNull.Value, Object), companyText)),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LoginId", Session("LoginId")),
                New SqlParameter("@CustomerId", Session("CustomerId")),
                New SqlParameter("@LevelName", Session("LevelName"))
            }

            Dim thisData As DataTable = settingClass.GetDataTableSP("sp_CustomerList", params)

            gvList.DataSource = thisData
            gvList.DataBind()

            gvList.Columns(1).Visible = LoginAccess("Visible ID") ' ID
            gvList.Columns(2).Visible = LoginAccess("Visible Debtor Code") ' DEBTOR CODE
            gvList.Columns(4).Visible = LoginAccess("Visible Company") ' COMPANY
            gvList.Columns(5).Visible = LoginAccess("Visible Company Detail") ' COMPANY DETAIL
            gvList.Columns(6).Visible = LoginAccess("Visible Area") ' AREA
            gvList.Columns(7).Visible = LoginAccess("Visible Sales") ' OPERATOR
            gvList.Columns(8).Visible = LoginAccess("Visible Cash Sale") ' CASH SALE
            gvList.Columns(9).Visible = LoginAccess("Visible On Stop") ' ON STOP
            gvList.Columns(10).Visible = LoginAccess("Visible Active") ' ACTIVE

            btnAdd.Visible = LoginAccess("Add")
            aExport.Visible = LoginAccess("Export")
            ddlActive.Visible = LoginAccess("Active")
            divCompany.Visible = LoginAccess("Filter Company")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Companys ORDER BY Id ASC"
            If Session("RoleName") = "Developer" Then
                thisQuery = "SELECT * FROM Companys WHERE Active=1 ORDER BY Id ASC"
            End If

            ddlCompany.DataSource = settingClass.GetDataTable(thisQuery)
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCompany.SelectedValue = Session("CompanyId").ToString()

            ddlExportCompany.DataSource = settingClass.GetDataTable(thisQuery)
            ddlExportCompany.DataTextField = "Alias"
            ddlExportCompany.DataValueField = "Id"
            ddlExportCompany.DataBind()

            If ddlExportCompany.Items.Count > 1 Then
                ddlExportCompany.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCompany.SelectedValue = Session("CompanyId").ToString()
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Private Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Exit Sub
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
