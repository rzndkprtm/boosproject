Imports System.Data

Partial Class Order_View
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim action As String = Request("action")

            Dim headerId As String = Request("boosid")
            If String.IsNullOrEmpty(headerId) OrElse String.IsNullOrEmpty(action) Then Exit Sub

            If action = "jobsheet" Then JobSheet(headerId)
            If action = "joborder" Then JobOrder(headerId)
            If action = "invoice" Then Invoice(headerId)
            If action = "quote" Then Quote(headerId)
            If action = "quotebuilder" Then QuoteBuilder(headerId)
            If action = "quotes" Then QuoteCustomer(headerId)
            If action = "suratjalan" Then SuratJalan(headerId)
        End If
    End Sub

    Protected Sub JobSheet(headerId As String)
        Try
            Dim previewClass As New PreviewClass
            Dim pdfBytes As Byte() = previewClass.BindContent(headerId)

            Dim orderId As String = previewClass.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id='" & headerId & "'")
            Dim customerName As String = previewClass.GetItemData("SELECT Customers.Name FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            Dim fileName As String = String.Format("ORDER {0} {1}.pdf", orderId, customerName.ToUpper())

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Invoice(headerId As String)
        Try
            Dim invoiceClass As New InvoiceClass

            Dim invoiceNumber As String = String.Empty
            Dim customerName As String = String.Empty
            Dim companyId As String = String.Empty

            Dim orderData As DataRow = invoiceClass.GetDataRow("SELECT Customers.CompanyId AS CompanyId, OrderHeaders.InvoiceNumber AS InvoiceNumber, Customers.Name AS CustomerName FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            If Not orderData Is Nothing Then
                invoiceNumber = orderData("InvoiceNumber").ToString().ToUpper()
                customerName = orderData("CustomerName").ToString().ToUpper()
                companyId = orderData("CompanyId").ToString()
            End If
            Dim fileName As String = String.Format("INVOICE {0} {1}.pdf", invoiceNumber, customerName)

            Dim pdfBytes As Byte() = invoiceClass.BindContent(headerId)
            If companyId = "3" Then
                pdfBytes = invoiceClass.BindContent_Local(headerId)
            End If

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Quote(headerId As String)
        Try
            Dim quoteClass As New QuoteClass
            Dim pdfBytes As Byte() = quoteClass.BindContent(headerId)

            Dim orderId As String = String.Empty
            Dim customerName As String = String.Empty

            Dim orderData As DataRow = quoteClass.GetDataRow("SELECT OrderHeaders.OrderId AS OrderId, Customers.Name AS CustomerName FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            If Not orderData Is Nothing Then
                orderId = orderData("OrderId").ToString().ToUpper()
                customerName = orderData("CustomerName").ToString().ToUpper()
            End If
            Dim fileName As String = String.Format("QUOTE {0} {1}.pdf", orderId, customerName)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub QuoteBuilder(headerId As String)
        Try
            Dim quoteClass As New QuoteClass
            Dim pdfBytes As Byte() = quoteClass.BindContentBuilder(headerId)

            Dim orderData As DataRow = quoteClass.GetDataRow("SELECT * FROM OrderHeaders WHERE Id='" & headerId & "'")
            Dim orderNumber As String = String.Empty
            Dim orderName As String = String.Empty
            If Not orderData Is Nothing Then
                orderNumber = orderData("OrderNumber")
                orderName = orderData("OrderName")
            End If
            Dim fileName As String = String.Format("QUOTE-{0}-{1}.pdf", orderNumber, orderName)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub SuratJalan(headerId As String)
        Try
            Dim suratClass As New SuratClass
            Dim pdfBytes As Byte() = suratClass.BindContent(headerId)

            Dim orderId As String = suratClass.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id='" & headerId & "'")
            Dim fileName As String = String.Format("SURAT JALAN {0}.pdf", orderId)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub QuoteCustomer(headerId As String)
        Try
            Dim quoteClass As New QuoteClass
            Dim pdfBytes As Byte() = quoteClass.BindContentCustomer(headerId)

            Dim orderData As DataRow = quoteClass.GetDataRow("SELECT * FROM OrderHeaders WHERE Id='" & headerId & "'")
            Dim orderNumber As String = String.Empty
            Dim orderName As String = String.Empty
            If Not orderData Is Nothing Then
                orderNumber = orderData("OrderNumber")
                orderName = orderData("OrderName")
            End If
            Dim fileName As String = String.Format("QUOTE-{0}-{1}.pdf", orderNumber, orderName)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub JobOrder(headerId As String)
        Try
            Dim jobClass As New JobClass
            Dim pdfBytes As Byte() = jobClass.BindContent(headerId)

            Dim orderId As String = jobClass.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id='" & headerId & "'")
            Dim customerName As String = jobClass.GetItemData("SELECT Customers.Name FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            Dim fileName As String = String.Format("JOB ORDER {0} {1}.pdf", orderId, customerName.ToUpper())

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub
End Class
