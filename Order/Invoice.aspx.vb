Imports System.Data
Imports System.Data.SqlClient
Partial Class Order_Invoice
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("orderid")) Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        lblHeaderId.Text = Request.QueryString("orderid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblHeaderId.Text)
        End If
    End Sub

    Protected Sub btnDownloadInvoice_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim selectedIds As List(Of String) = GetSelectedItemIds()

            If selectedIds.Count = 0 Then
                MessageError(True, "PLEASE SELECT AT LEAST ONE ITEM!")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET InvoiceNumber=@InvoiceNumber WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    thisCmd.Parameters.AddWithValue("@InvoiceNumber", txtInvoiceNumber.Text.Trim())
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim itemIds As String = String.Join(",", selectedIds)

            Dim invoiceClass As New InvoiceClass
            Dim pdfBytes As Byte() = invoiceClass.BindContent("Custom", lblHeaderId.Text, itemIds)
            If lblCompanyId.Text = "3" Then
                pdfBytes = invoiceClass.BindContent_Local(lblHeaderId.Text)
            End If

            Dim fileName As String = String.Format("INVOICE {0} {1}.pdf", txtInvoiceNumber.Text.ToUpper(), lblCustomerName.Text.ToUpper())

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & "")
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

    Protected Sub btnPreviewInvoice_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim selectedIds As List(Of String) = GetSelectedItemIds()

            If selectedIds.Count = 0 Then
                MessageError(True, "PLEASE SELECT AT LEAST ONE ITEM!")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderHeaders SET InvoiceNumber=@InvoiceNumber WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    thisCmd.Parameters.AddWithValue("@InvoiceNumber", txtInvoiceNumber.Text.Trim())
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim itemIds As String = String.Join(",", selectedIds)

            Dim previewUrl As String = ResolveUrl("~/order/view") & "?action=custominvoice&boosid=" & Server.UrlEncode(lblHeaderId.Text) & "&itemid=" & Server.UrlEncode(itemIds)

            Response.Redirect(previewUrl, False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSendInvoice_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim selectedIds As List(Of String) = GetSelectedItemIds()

            If selectedIds.Count = 0 Then
                MessageError(True, "PLEASE SELECT AT LEAST ONE ITEM!")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderHeaders SET InvoiceNumber=@InvoiceNumber WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    thisCmd.Parameters.AddWithValue("@InvoiceNumber", txtInvoiceNumber.Text.Trim())
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim itemIds As String = String.Join(",", selectedIds)

            Dim isValidEmail As Boolean = orderClass.IsValidEmail(txtSendInvoiceTo.Text)
            If IsValid = False Then
                MessageError(True, "PLEASE CHECK YOUR CUSTOMER EMAIL TO !")
                Exit Sub
            End If

            Dim ccCustomer As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendInvoiceCCCustomer.Text) Then
                Dim raw As String = txtSendInvoiceCCCustomer.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccCustomer = String.Join(";", cleanedEmails)
            End If

            Dim ccStaff As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendInvoiceCCStaff.Text) Then
                Dim raw As String = txtSendInvoiceCCStaff.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccStaff = String.Join(";", cleanedEmails)
            End If

            'Using thisConn As New SqlConnection(myConn)
            '    If lblOrderStatus.Text = "Waiting Proforma" OrElse lblOrderStatus.Text = "Pending Payment" Then
            '        Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Proforma Sent', Collector=@Collector, InvoiceDate=GETDATE(), DueDate=DATEADD(DAY, 14, GETDATE()) WHERE Id=@Id;", thisConn)
            '            thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
            '            thisCmd.Parameters.AddWithValue("@Collector", Session("LoginId").ToString())
            '            thisConn.Open()
            '            thisCmd.ExecuteNonQuery()
            '        End Using
            '    End If
            'End Using

            Dim mailingClass As New MailingClass
            mailingClass.SendInvoice(lblHeaderId.Text, Session("LoginId").ToString(), txtSendInvoiceTo.Text, ccCustomer, ccStaff, "Custom", itemIds)

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Send Invoice"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(headerId As String)
        Try
            Dim headerData As DataRow = orderClass.GetDataRow("SELECT OrderHeaders.CustomerId, OrderHeaders.InvoiceNumber, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, CASE WHEN Payment = 1 THEN 'Paid' ELSE '' END AS OrderPaid, FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Dim orderPaid As String = headerData("OrderPaid").ToString()
            If orderPaid = "Paid" Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            lblCustomerId.Text = headerData("CustomerId").ToString()
            lblCompanyId.Text = headerData("CompanyId").ToString()
            lblCustomerName.Text = headerData("CustomerName").ToString()
            txtInvoiceNumber.Text = headerData("InvoiceNumber").ToString()

            Dim detailParams As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", Convert.ToInt32(headerId))
            }
            gvList.DataSource = orderClass.GetDataTableSP("sp_OrderDetails_List_ByHeaderId", detailParams)
            gvList.DataBind()

            gvList.Columns(2).Visible = LoginAccess("Visible ID")

            txtSendInvoiceTo.Text = orderClass.GetCustomerPrimaryEmail(lblCustomerId.Text)

            Dim dataEmailCustomer As DataTable = orderClass.GetDataTable("SELECT Email FROM CustomerContacts CROSS APPLY STRING_SPLIT(Tags, ',') AS thisArray WHERE CustomerId='" & lblCustomerId.Text & "' AND LTRIM(RTRIM(Email)) <> '' AND Email IS NOT NULL AND thisArray.VALUE='Invoicing' AND [Primary]=0")
            If dataEmailCustomer.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataEmailCustomer.Rows
                    listEmail.Add(row("Email").ToString())
                Next

                txtSendInvoiceCCCustomer.Text = String.Join(vbCrLf, listEmail)
            End If

            Dim dataCCMailing As DataTable = orderClass.GetDataTable("SELECT * FROM Mailings WHERE CompanyId='" & lblCompanyId.Text & "' AND Name='Send Invoice' AND Active=1")
            If dataCCMailing.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataCCMailing.Rows
                    Dim emails As String() = row("Cc").ToString().Split(";"c)

                    For Each email As String In emails
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listEmail.Add(email.Trim())
                        End If
                    Next
                Next

                txtSendInvoiceCCStaff.Text = String.Join(vbCrLf, listEmail)
            End If

            Dim operatorEmail As String = orderClass.GetItemData("SELECT ISNULL(STRING_AGG(Logins.Email, ';'), '') FROM Customers OUTER APPLY STRING_SPLIT(Customers.Operator, ',') operatorArray LEFT JOIN Logins ON Logins.Id = TRY_CAST(operatorArray.value AS INT) WHERE Customers.Id='" & lblCustomerId.Text & "';")
            If Not String.IsNullOrEmpty(operatorEmail) Then
                Dim listEmail As New List(Of String)
                If Not String.IsNullOrWhiteSpace(txtSendInvoiceCCStaff.Text) Then
                    listEmail.AddRange(txtSendInvoiceCCStaff.Text.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries))
                End If
                Dim operatorEmails = operatorEmail.Split(";"c)
                For Each email As String In operatorEmails
                    If Not String.IsNullOrWhiteSpace(email) Then
                        listEmail.Add(email.Trim())
                    End If
                Next
                listEmail = listEmail.Distinct().ToList()
                txtSendInvoiceCCStaff.Text = String.Join(vbCrLf, listEmail)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function BindProductDescription(itemId As String) As String
        Return orderClass.GetProductDescription(itemId, False)
    End Function

    Protected Function GetSelectedItemIds() As List(Of String)
        Dim selectedIds As New List(Of String)

        For Each row As GridViewRow In gvList.Rows
            If row.RowType = DataControlRowType.DataRow Then
                Dim chkSelect As CheckBox = CType(row.FindControl("chkSelect"), CheckBox)

                If chkSelect IsNot Nothing AndAlso chkSelect.Checked Then
                    selectedIds.Add(gvList.DataKeys(row.RowIndex).Value.ToString())
                End If
            End If
        Next

        Return selectedIds
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerHtml = message
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
