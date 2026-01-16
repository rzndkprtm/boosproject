Imports System.Data.SqlClient

Partial Class Setting_Customer_Default
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomer")

            BindCompany()
            ddlCompany.SelectedValue = Session("CompanyCustomer")
            BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchCustomer") = txtSearch.Text
        Session("CompanyCustomer") = ddlCompany.SelectedValue
        Response.Redirect("~/setting/customer/add", False)
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvList_PageIndexChanging", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            Session("SearchCustomer") = txtSearch.Text
            Session("CompanyCustomer") = ddlCompany.SelectedValue

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/setting/customer/detail?customerid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetail_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId = '" + dataId + "' ORDER BY ActionDate ASC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLog_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Active=0 WHERE Id=@Id UPDATE CustomerLogins SET Active=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", dataId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchCustomer") = txtSearch.Text
            Session("CompanyCustomer") = ddlCompany.SelectedValue
            Response.Redirect("~/setting/customer", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDelete_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, companyText As String, activeText As String)
        Session("SearchCustomer") = String.Empty
        Session("CompanyCustomer") = String.Empty
        Try
            Dim activeString As String = "WHERE Customers.Active='" & ddlActive.SelectedValue & "'"
            Dim searchString As String = String.Empty
            Dim companyString As String = String.Empty

            Dim roleString As String = String.Empty
            Dim orderString As String = "ORDER BY Customers.Name ASC"

            If Not searchText = "" Then
                searchString = "AND (Customers.Id LIKE '%" & searchText.Trim() & "%' OR Customers.DebtorCode LIKE '%" & searchText.Trim() & "%' OR Customers.Name LIKE '%" & searchText.Trim() & "%' OR Customers.Area LIKE '%" & searchText.Trim() & "%' OR Companys.Alias LIKE '%" & searchText.Trim() & "%')"
            End If

            If Not String.IsNullOrEmpty(companyText) Then
                companyString = "AND Customers.CompanyId='" & companyText & "'"
            End If

            If Session("RoleName") = "Sales" Then
                roleString = "AND Customers.CompanyId='" & Session("CompanyId").ToString() & "'"
                If Session("LevelName") = "Member" Then
                    roleString = "AND (Customers.Operator='" & Session("LoginId").ToString() & "' OR Customers.Id='" & Session("CustomerId") & "')"
                End If
                orderString = "ORDER BY CASE WHEN Customers.Id='3' THEN 0 ELSE 1 END ASC, Customers.Name ASC"
            End If

            If Session("RoleName") = "Account" Then
                roleString = "AND Customers.CompanyId='" & Session("CompanyId").ToString() & "'"
            End If

            If Session("RoleName") = "Customer Service" Then
                roleString = "AND Customers.CompanyId='" & Session("CompanyId").ToString() & "'"
            End If

            If Session("RoleName") = "Data Entry" Then
                roleString = "AND Customers.CompanyId<>'" & Session("CompanyId").ToString() & "'"
            End If

            Dim thisQuery As String = String.Format("SELECT Customers.*, CustomerLogins.FullName AS OperatorName, CASE WHEN Customers.CashSale=1 THEN 'Yes' WHEN Customers.CashSale=0 THEN 'No' ELSE 'Error' END AS CustomerCashSale, CASE WHEN Customers.OnStop=1 THEN 'Yes' WHEN Customers.OnStop=0 THEN 'No' ELSE 'Error' END AS CustomerOnStop, CASE WHEN Customers.MinSurcharge=1 THEN 'Yes' WHEN Customers.MinSurcharge=0 THEN 'No' ELSE 'Error' END AS CustomerMinSurcharge, CASE WHEN Customers.Active=1 THEN 'Yes' WHEN Customers.Active=0 THEN 'No' ELSE 'Error' END AS DataActive, Companys.Alias AS CompanyAlias, CompanyDetails.Name AS CompanyDetailName FROM Customers LEFT JOIN Companys ON Customers.CompanyId=Companys.Id LEFT JOIN CompanyDetails ON Customers.CompanyDetailId=CompanyDetails.Id LEFT JOIN CustomerLogins ON Customers.Operator=CustomerLogins.Id {0} {1} {2} {3} {4}", activeString, companyString, roleString, searchString, orderString)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID") ' ID
            gvList.Columns(2).Visible = PageAction("Visible Debtor Code") ' DEBTOR CODE
            gvList.Columns(4).Visible = PageAction("Visible Company") ' COMPANY
            gvList.Columns(5).Visible = PageAction("Visible Company Detail") ' COMPANY DETAIL
            gvList.Columns(6).Visible = PageAction("Visible Area") ' AREA
            gvList.Columns(7).Visible = PageAction("Visible Operator") ' OPERATOR
            gvList.Columns(8).Visible = PageAction("Visible Cash Sale") ' CASH SALE
            gvList.Columns(9).Visible = PageAction("Visible On Stop") ' ON STOP
            gvList.Columns(10).Visible = PageAction("Visible Active") ' ACTIVE

            btnAdd.Visible = PageAction("Add")
            ddlActive.Visible = PageAction("Active")
            divCompany.Visible = PageAction("Filter Company")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Companys WHERE Id<>'1' ORDER BY Id ASC"
            If Session("RoleName") = "Developer" Then
                thisQuery = "SELECT * FROM Companys ORDER BY Id ASC"
            End If

            ddlCompany.DataSource = settingClass.GetDataTable(thisQuery)
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindCompany", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
    End Sub

    Protected Function BindTextLog(logId As String) As String
        Return settingClass.getTextLog(logId)
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
