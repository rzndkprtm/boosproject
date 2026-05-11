Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Unshipment
    Inherits Page

    Dim unshipmentClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Email(False, String.Empty)
            BindCompany()
            BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)

            btnPreview.OnClientClick = "window.open('view?action=unshipment&search=" & Server.UrlEncode(txtSearch.Text.Trim()) & "&company=" & Server.UrlEncode(ddlCompany.SelectedValue) & "','_blank'); return false;"
        End If
    End Sub

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs)
        Try
            Dim unshipmentClass As New UnshipmentClass
            Dim pdfBytes As Byte() = unshipmentClass.BindContent(txtSearch.Text, ddlCompany.SelectedValue)

            Dim fileName As String = String.Format("ORDER {0} {1}.pdf", String.Empty, String.Empty)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=UNSHIPMENT ORDER.pdf")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs)
        MessageError_Email(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showSendEmail(); };"
        Try
            If String.IsNullOrEmpty(txtSendEmailTo.Text) Then
                MessageError_Email(True, "EMAIL TO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendEmail", thisScript, True)
                Exit Sub
            End If

            Dim toEmail As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendEmailTo.Text) Then
                Dim raw As String = txtSendEmailTo.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                toEmail = String.Join(";", cleanedEmails)
            End If

            Dim ccEmail As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendEmailCC.Text) Then
                Dim raw As String = txtSendEmailCC.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccEmail = String.Join(";", cleanedEmails)
            End If

            If msgErrorSendEmail.InnerText = "" Then
                Dim mailingClass As New MailingClass
                mailingClass.SentUnshipment(toEmail, ccEmail, txtSearch.Text.Trim(), ddlCompany.SelectedValue)
            End If
        Catch ex As Exception
            MessageError_Email(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Email(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showSendEmail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/order/detail?orderid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub BindDataOrder(searchText As String, companyId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", searchText.Trim()),
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId))
            }

            Dim thisData As DataTable = unshipmentClass.GetDataTableSP("sp_OrderUnshipment", params)
            gvList.DataSource = thisData
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID")

            aEmail.Visible = PageAction("Email")

            Dim mailingString As String = "SELECT * FROM Mailings WHERE Name='Unshipment Order' AND Active=1"
            Dim dataToMailing As DataTable = unshipmentClass.GetDataTable(mailingString)
            If dataToMailing.Rows.Count > 0 Then
                Dim listTo As New List(Of String)
                Dim listCc As New List(Of String)

                For Each row As DataRow In dataToMailing.Rows
                    Dim emailsTo As String() = row("To").ToString().Split(";"c)
                    Dim emailsCc As String() = row("Cc").ToString().Split(";"c)

                    For Each email As String In emailsTo
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listTo.Add(email.Trim())
                        End If
                    Next

                    For Each email As String In emailsCc
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listCc.Add(email.Trim())
                        End If
                    Next
                Next

                txtSendEmailTo.Text = String.Join(vbCrLf, listTo)
                txtSendEmailCC.Text = String.Join(vbCrLf, listCc)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = unshipmentClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            ddlCompany.Items.Insert(0, New ListItem("All", ""))

        Catch ex As Exception
            ddlCompany.Items.Clear()
            ddlCompany.Items.Add(New ListItem("All", ""))
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Email(visible As Boolean, message As String)
        divErrorSendEmail.Visible = visible : msgErrorSendEmail.InnerText = message
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
