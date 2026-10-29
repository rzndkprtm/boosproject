Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Invoice
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim pageAccess As Boolean = LoginAccess("Load")
        'If pageAccess = False Then
        '    Response.Redirect("~/order", False)
        '    Exit Sub
        'End If

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
            Dim selectedIds As New List(Of String)

            For Each row As GridViewRow In gvList.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim chkSelect As CheckBox = CType(row.FindControl("chkSelect"), CheckBox)

                    If chkSelect IsNot Nothing AndAlso chkSelect.Checked Then
                        Dim id As String = gvList.DataKeys(row.RowIndex).Value.ToString()
                        selectedIds.Add(id)
                    End If
                End If
            Next

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

    Protected Sub BindData(headerId As String)
        Try
            Dim headerData As DataRow = orderClass.GetDataRow("SELECT OrderHeaders.InvoiceNumber, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            lblCompanyId.Text = headerData("CompanyId").ToString()
            lblCustomerName.Text = headerData("CustomerName").ToString()
            txtInvoiceNumber.Text = headerData("InvoiceNumber").ToString()

            Dim detailParams As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", Convert.ToInt32(headerId))
            }
            gvList.DataSource = orderClass.GetDataTableSP("sp_OrderDetails_List_ByHeaderId", detailParams)
            gvList.DataBind()

            gvList.Columns(2).Visible = LoginAccess("Visible ID")
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
