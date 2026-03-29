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
        If thisAction = "resetcashsaleorder" Then
            ResetDataCashSaleOrder()
        End If
        If thisAction = "unshipment" Then
            UnshipmentOrder()
        End If
        If thisAction = "deleteorderactioncontext" Then
            DeleteContext()
        End If
        If thisAction = "login" Then
            If String.IsNullOrEmpty(Request.QueryString("user")) Then
                Response.Redirect("~/", False)
                Exit Sub
            End If
            GetTemporary(Request.QueryString("user").ToString())
        End If
        If thisAction = "resetpassword" Then
            ResetPassword()
        End If
        If thisAction = "clearsession" Then
            ClearSession()
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

    Protected Sub UpdateShipment(id As String, status As String, shipNumber As String, shipDate As Date, conNumber As String, courier As String, invNumber As String)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderShipments SET ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, Courier=@Courier WHERE Id=@Id; UPDATE OrderHeaders SET Status=@Status WHERE Id=@Id; UPDATE OrderInvoices SET InvoiceNumber=@InvoiceNumber WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Parameters.AddWithValue("@ShipmentNumber", shipNumber)
                    myCmd.Parameters.AddWithValue("@ShipmentDate", shipDate)
                    myCmd.Parameters.AddWithValue("@ContainerNumber", conNumber)
                    myCmd.Parameters.AddWithValue("@Courier", courier)
                    myCmd.Parameters.AddWithValue("@Status", status)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", invNumber)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub CreateSales()
        Try
            Dim salesClass As New SalesClass
            Dim companyData As DataTable = salesClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Id ASC")
            If companyData.Rows.Count > 0 Then
                For i As Integer = 0 To companyData.Rows.Count - 1
                    Dim companyId As String = companyData.Rows(i)("Id").ToString()

                    Dim salesData As Integer = salesClass.GetItemData_Integer("SELECT COUNT(*) FROM Sales WHERE SummaryDate=GETDATE() AND CompanyId='" & companyId & "'")

                    If salesData = 0 Then
                        Dim thisId As String = salesClass.CreateId("SELECT TOP 1 Id FROM Sales ORDER BY Id DESC")

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Sales(Id, CompanyId, SummaryDate, TotalCostPrice, TotalSellingPrice, TotalPaidAmount) VALUES(@Id, @CompanyId, GETDATE(), 0, 0, 0)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", thisId)
                                myCmd.Parameters.AddWithValue("@CompanyId", companyId)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ResetDataCashSaleOrder()
        Try
            Dim orderClass As New OrderClass
            Dim thisData As DataTable = orderClass.GetDataTable("SELECT OrderHeaders.* FROM OrderHeaders LEFT JOIN OrderInvoices ON OrderHeaders.Id=OrderInvoices.Id WHERE OrderHeaders.Status='Proforma Sent' AND OrderInvoices.DueDate=CAST(GETDATE() AS DATE)")

            If thisData.Rows.Count > 0 Then
                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim thisId As String = thisData.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        thisConn.Open()

                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Unsubmitted', SubmittedDate=NULL WHERE Id=@Id; DELETE FROM OrderInvoices WHERE Id=@Id;", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)

                            myCmd.ExecuteNonQuery()
                        End Using

                        Dim serviceData As DataTable = orderClass.GetDataTable("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & thisId & "' AND Products.DesignId='16'")
                        If serviceData.Rows.Count > 0 Then
                            For iDetail As Integer = 0 To serviceData.Rows.Count - 1
                                Dim serviceId As String = serviceData.Rows(iDetail)("Id").ToString()

                                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@ItemId; DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                                    myCmd.Parameters.AddWithValue("@ItemId", serviceId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", thisId)

                                    myCmd.ExecuteNonQuery()
                                End Using
                            Next
                        End If

                        thisConn.Close()
                    End Using
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub UnshipmentOrder()
        Try
            If Now.DayOfWeek >= DayOfWeek.Monday AndAlso Now.DayOfWeek <= DayOfWeek.Friday Then
                Dim mailClass As New MailingClass
                Dim unshipmentClass As New UnshipmentClass

                Dim fileName As String = Trim("Unshipment - In Production Order " & Now.ToString("dd MMm yyyy") & ".pdf")

                Dim pdfFilePath As String = Server.MapPath("~/File/Report/" & fileName)
                unshipmentClass.BindContent(pdfFilePath)

                mailClass.MailUnshipment(pdfFilePath)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub DeleteContext()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderActionContext", thisConn)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub GetTemporary(user As String)
        Try
            Dim thisId As String = String.Empty

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Temporarys OUTPUT INSERTED.Id VALUES (NEWID(), @UserName)", thisConn)
                    myCmd.Parameters.AddWithValue("@UserName", user)

                    thisConn.Open()
                    thisId = myCmd.ExecuteScalar().ToString()
                End Using
            End Using
            Dim url As String = String.Format("~/information?uid={0}", thisId)
            Response.Redirect(url, False)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ResetPassword()
        Dim loginData As DataTable = settingClass.GetDataTable("SELECT CustomerLogins.* FROM CustomerLogins LEFT JOIN Customers ON CustomerLogins.CustomerId=Customers.Id WHERE Customers.CompanyId='2'")
        If loginData.Rows.Count > 0 Then
            For iLogin As Integer = 0 To loginData.Rows.Count - 1
                Dim loginId As String = loginData.Rows(iLogin)("Id").ToString()
                Dim userName As String = loginData.Rows(iLogin)("UserName").ToString()

                Dim newPassword As String = settingClass.Encrypt(userName)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Password=@Password, FailedCount=0, LastLogin=NULL, ResetLogin=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", loginId)
                        myCmd.Parameters.AddWithValue("@Password", newPassword)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            Next
        End If
    End Sub

    Protected Sub ClearSession()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Sessions", thisConn)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub
End Class
