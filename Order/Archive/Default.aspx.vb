Partial Class Order_Archive_Default
    Inherits Page

    Dim archiveClass As New ArchiveClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

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

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
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
                    'Session("OrderSearch") = txtSearch.Text
                    'Session("OrderStatus") = ddlStatus.SelectedValue
                    'Session("OrderActive") = ddlActive.SelectedValue

                    url = String.Format("~/order/archive/detail?orderid={0}", dataId)

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
            End If
        End If
    End Sub

    Protected Sub BindDataOrder(search As String, status As String, active As String)
        Try
            Dim byActive As String = "WHERE Order_Header.Active=1"
            Dim byRole As String = String.Empty
            Dim byInstaller As String = String.Empty
            Dim byStatus As String = String.Empty
            Dim byText As String = String.Empty
            Dim byOrder As String = "ORDER BY Order_Header.OrdID DESC"

            If active = "0" Then byActive = "WHERE Order_Header.Active=0"

            If Not search = "" Then
                byText = "AND (Order_Header.OrdID LIKE '%" + search + "%' OR Order_Header.StoreOrderNo LIKE '%" + search + "%' OR Order_Header.StoreCustomer LIKE '%" + search + "%' OR Stores.Name LIKE '%" + search + "%' OR Order_Header.UserLogin LIKE '%" + search + "%')"
            End If

            If Session("RoleName") = "Developer" Or Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                byRole = String.Empty
                byStatus = String.Empty
                If Not status = "" Then
                    byStatus = "AND Order_Header.Status='" & status & "'"
                End If
            End If

            If Session("RoleName") = "Customer" Then
                byRole = "AND Order_Header.UserLogin='" & Session("UserName") & "'"
                If Session("RoleName") = "Customer" AndAlso Session("CustomerId") = "1536" Then
                    byRole = "AND Order_Header.UserLogin='duasaudara'"
                End If
                byStatus = String.Empty
                If Not status = "" Then
                    byStatus = "AND Order_Header.Status='" & status & "'"
                End If
            End If

            If Session("RoleName") = "Sales" Then
                byRole = "AND Users.CompanyGroup='JPMD'"
                If Session("LevelName") = "Member" Then
                    byRole = "AND (Left(Users.DebtorCode,1) = '1' OR Left(Users.DebtorCode,2) = '31' Or Users.DebtorCode = '2P-012')"
                End If
                byStatus = String.Empty
                If Not status = "" Then
                    byStatus = "AND Order_Header.Status='" & status & "'"
                End If
            End If

            If Session("RoleName") = "Account" Then
                byRole = "AND Users.CompanyGroup='JPMD'"
                byStatus = String.Empty
                If Not status = "" Then
                    byStatus = "AND Order_Header.Status='" & status & "'"
                End If
            End If

            Dim thisQuery As String = String.Format("SELECT DISTINCT Order_Header.*, Users.CompanyGroup AS CompanyGroup, BOEOrder_Header.Name_courier AS Courier, BOEOrder_Header.ContainerNo AS ConNote, BOEOrder_Header.ShipmentNo AS ShipmentNo, Stores.Name AS StoreName FROM Order_Header LEFT JOIN BOEOrder_Header ON Order_Header.IDBOEH = BOEOrder_Header.IDH LEFT JOIN Users INNER JOIN Stores ON Users.DebtorCode=Stores.Id ON Users.UserName = Order_Header.UserLogin {0} {1} {2} {3} {4} {5}", byActive, byRole, byInstaller, byStatus, byText, byOrder)

            gvList.DataSource = archiveClass.GetDataTable(thisQuery)
            gvList.DataBind()

            divActive.Visible = PageAction("Visible Active")
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
