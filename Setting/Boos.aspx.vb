Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Boos
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("action")) Then
            Response.Redirect("~/additional", False)
            Exit Sub
        End If

        Dim thisAction As String = Request.QueryString("action").ToString()
        If thisAction = "createsales" Then
            CreateSales()
        End If
        If thisAction = "refreshsales" Then
            RefreshSales()
        End If
        If thisAction = "resetproformaorder" Then
            ResetProformaOrder()
        End If
        If thisAction = "deleteorderactioncontext" Then
            DeleteOrderActionContext()
        End If
        If thisAction = "deletenullsession" Then
            DeleteNullSession()
        End If
        If thisAction = "updatefactory" Then
            UpdateFactory()
        End If
        If thisAction = "downloadboe" Then
            UpdateDownloadBOE()
        End If
        If thisAction = "shipment" Then
            If String.IsNullOrEmpty(Request.QueryString("OrdID")) Then
                Exit Sub
            End If
            If String.IsNullOrEmpty(Request.QueryString("status")) Then
                Exit Sub
            End If
            Dim id As String = Request.QueryString("OrdID").ToString()
            Dim status As String = Request.QueryString("Status").ToString()
            Dim shipmentNumber As String = Request.QueryString("ShipmentNo").ToString()
            Dim containerNumber As String = Request.QueryString("ContainerNo").ToString()
            Dim courier As String = Request.QueryString("Courier").ToString()
            Dim invoiceNumber As String = Request.QueryString("InvoiceNo").ToString()

            Dim shipDateStr As String = Request.QueryString("ShipDate")
            Dim shipDate As DateTime

            If String.IsNullOrEmpty(shipDateStr) OrElse Not DateTime.TryParse(shipDateStr, shipDate) Then
                Exit Sub
            End If

            UpdateShipment(id, status, shipmentNumber, shipDate, containerNumber, courier, invoiceNumber)
        End If
    End Sub

    Protected Sub RefreshSales()
        Try
            Dim salesClass As New SalesClass

            Dim dataCompany As DataTable = salesClass.GetDataTable("SELECT Id FROM Companys")
            If dataCompany.Rows.Count > 0 Then
                For i As Integer = 0 To dataCompany.Rows.Count - 1
                    Dim companyId As String = dataCompany.Rows(i)("Id").ToString()
                    salesClass.RefreshData(companyId)
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub UpdateDownloadBOE()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("sp_OrderHeaders_Update_BOE", thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub UpdateShipment(id As String, status As String, shipNumber As String, shipDate As Date, conNumber As String, courier As String, invNumber As String)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status=@Status, ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, Courier=@Courier, InvoiceNumber=@InvoiceNumber WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", id)
                    thisCmd.Parameters.AddWithValue("@ShipmentNumber", shipNumber)
                    thisCmd.Parameters.AddWithValue("@ShipmentDate", shipDate)
                    thisCmd.Parameters.AddWithValue("@ContainerNumber", conNumber)
                    thisCmd.Parameters.AddWithValue("@Courier", courier)
                    thisCmd.Parameters.AddWithValue("@Status", status)
                    thisCmd.Parameters.AddWithValue("@InvoiceNumber", invNumber)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub UpdateFactory()
        Try
            Dim orderClass As New OrderClass
            Dim orderData As DataTable = orderClass.GetDataTable("SELECT Id FROM OrderHeaders WHERE Active=1 ORDER BY Id ASC")
            If orderData.Rows.Count > 0 Then
                For i As Integer = 0 To orderData.Rows.Count - 1
                    Dim headerId As String = orderData.Rows(i)("Id").ToString()
                    orderClass.UpdateOrderFactory(headerId)
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub CreateSales()
        Try
            Dim salesClass As New SalesClass
            Dim companyData As DataTable = salesClass.GetDataTable("SELECT Id FROM Companys WHERE Active=1 ORDER BY Id ASC")
            If companyData.Rows.Count > 0 Then
                For i As Integer = 0 To companyData.Rows.Count - 1
                    Dim companyId As String = companyData.Rows(i)("Id").ToString()

                    Dim salesData As Integer = salesClass.GetItemData_Integer("SELECT COUNT(*) FROM Sales WHERE SummaryDate=GETDATE() AND CompanyId='" & companyId & "'")
                    If salesData = 0 Then
                        Dim thisId As String = salesClass.CreateId("SELECT TOP 1 Id FROM Sales ORDER BY Id DESC")

                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Sales(Id, CompanyId, SummaryDate, TotalCostPrice, TotalSellingPrice, TotalPaidAmount) VALUES(@Id, @CompanyId, GETDATE(), 0, 0, 0)", thisConn)
                                thisCmd.Parameters.AddWithValue("@Id", thisId)
                                thisCmd.Parameters.AddWithValue("@CompanyId", companyId)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ResetProformaOrder()
        Try
            Dim orderClass As New OrderClass
            Dim thisData As DataTable = orderClass.GetDataTable("SELECT Id FROM OrderHeaders WHERE Status='Proforma Sent' AND DueDate=CAST(GETDATE() AS DATE)")
            If thisData.Rows.Count > 0 Then
                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim thisId As String = thisData.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Pending Payment' WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"OrderHeaders", thisId, 2, "Pending Payment Order"}
                    settingClass.Logs(dataLog)

                    Dim mailingClass As New MailingClass
                    mailingClass.ResetProformaOrder(thisId)
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub DeleteOrderActionContext()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM OrderActionContext", thisConn)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub DeleteNullSession()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Sessions WHERE LoginId IS NULL", thisConn)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub
End Class
