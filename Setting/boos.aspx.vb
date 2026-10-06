Imports System.Data.SqlClient

Partial Class Setting_boos
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("action")) Then
            Exit Sub
        End If

        Dim thisAction As String = Request.QueryString("action").ToString()
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
End Class
