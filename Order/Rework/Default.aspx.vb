Partial Class Order_Rework_Default
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvList_PageIndexChanging", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/order/rework/detail?reworkid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetail_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = orderClass.GetDataTable("SELECT * FROM Logs WHERE Type='OrderReworks' AND DataId='" & dataId & "' ORDER BY ActionDate ASC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError_Log(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLog_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            ElseIf e.CommandName = "ToOrder" Then
                MessageError(False, String.Empty)
                Try
                    Session("headerId") = dataId
                    Response.Redirect("~/order/detail", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkToOrder_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub BindDataOrder(search As String, status As String, active As String)
        Session("reworkId") = String.Empty
        Session("headerId") = String.Empty
        Try
            Dim byActive As String = " WHERE OrderReworks.Active=1"
            If active = "0" Then byActive = "WHERE OrderReworks.Active=0"

            Dim byRole As String = String.Empty

            Dim byStatus As String = String.Empty

            Dim byText As String = String.Empty
            Dim byOrder As String = " ORDER BY OrderReworks.Id DESC"

            If Not search = "" Then
                byText = " AND (OrderReworks.Id LIKE '%" & search.Trim() & "%' OR OrderHeaders.OrderId LIKE '%" & search.Trim() & "%' OR OrderHeaders.OrderNumber LIKE '%" & search.Trim() & "%' OR OrderHeaders.OrderName LIKE '%" & search.Trim() & "%' OR OrderHeaders.CustomerId LIKE '%" & search.Trim() & "%' OR Customers.Name LIKE '%" & search.Trim() & "%')"
            End If

            If Session("RoleName") = "Developer" Then
                byRole = String.Empty
                byStatus = String.Empty
                If Not status = "" Then
                    byStatus = " AND OrderReworks.Status='" & status & "'"
                End If
                byOrder = " ORDER BY OrderReworks.Id, CASE WHEN OrderReworks.Status='Pending Approval' THEN 1 WHEN OrderReworks.Status='Approved' THEN 2 WHEN OrderReworks.Status='Rejected' THEN 3 WHEN OrderReworks.Status='Unsubmitted' THEN 4 END DESC"
            End If

            If Session("RoleName") = "Customer" Then
                byRole = "AND OrderHeaders.CustomerId='" & Session("CustomerId").ToString() & "'"
                byStatus = String.Empty
                If Not status = "" Then
                    byStatus = " AND OrderReworks.Status='" & status & "'"
                End If
                byOrder = " ORDER BY OrderReworks.Id, CASE WHEN OrderReworks.Status='Pending Approval' THEN 1 WHEN OrderReworks.Status='Approved' THEN 2 WHEN OrderReworks.Status='Rejected' THEN 3 WHEN OrderReworks.Status='Unsubmitted' THEN 4 END DESC"
            End If

            Dim thisQuery As String = String.Format("SELECT OrderReworks.*, OrderHeaders.OrderNumber AS OrderNumber, OrderHeaders.OrderName AS OrderName, Customers.Name AS CustomerName FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id {0} {1} {2} {3} {4}", byActive, byRole, byStatus, byText, byOrder)

            gvList.DataSource = orderClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            divActive.Visible = PageAction("Active")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataOrder", ex.ToString()}
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

    Protected Function GetOrderId(headerId As String) As String
        If Not String.IsNullOrEmpty(headerId) Then
            Return orderClass.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id='" & headerId & "'")
        End If
        Return String.Empty
    End Function

    Protected Function BindTextLog(logId As String) As String
        Return orderClass.GetTextLog(logId)
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
