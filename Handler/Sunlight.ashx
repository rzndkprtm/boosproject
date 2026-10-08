<%@ WebHandler Language="VB" Class="Sunlight" %>

Imports System.IO
Imports System.Web.Script.Serialization
Imports System.Data.SqlClient
Imports System.Threading.Tasks

Public Class Sunlight : Implements IHttpHandler

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"

        Dim apiKeyHeader As String = context.Request.Headers("X-API-KEY")
        Dim validApiKey As String = System.Configuration.ConfigurationManager.AppSettings("ApiKey")

        If String.IsNullOrEmpty(apiKeyHeader) OrElse apiKeyHeader <> validApiKey Then
            context.Response.StatusCode = 401
            context.Response.Write("{""status"": ""error"", ""message"": ""Invalid or missing API key""}")
            Return
        End If

        If context.Request.HttpMethod <> "POST" Then
            context.Response.StatusCode = 405
            context.Response.Write("{""status"": ""error"", ""message"": ""Method not allowed""}")
            Return
        End If

        Dim inputStream As Stream = context.Request.InputStream
        Dim reader As New StreamReader(inputStream)
        Dim jsonString As String = reader.ReadToEnd()

        If String.IsNullOrEmpty(jsonString) Then
            context.Response.StatusCode = 400
            context.Response.Write("{""status"": ""error"", ""message"": ""Request body is empty""}")
            Return
        End If

        Dim serializer As New JavaScriptSerializer()

        Try
            Dim orderData As OrderData = serializer.Deserialize(Of OrderData)(jsonString)
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Dim transaction As SqlTransaction = thisConn.BeginTransaction()
                Try
                    Dim headerId As String = orderClass.GetNewOrderHeaderId()

                    ' ORDER HEADERS
                    Using thisCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, OrderFactory, OrderContact, OrderAddress, OrderContainer, Status, CreatedBy, CreatedDate, SubmittedDate, Payment, Amount, Download, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'BIG', '', '', 'NSW', 'New Order', 1568, GETDATE(), GETDATE(), 0, 0, 'No', 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn, transaction)
                        thisCmd.Parameters.AddWithValue("@Id", headerId)
                        thisCmd.Parameters.AddWithValue("@OrderId", orderData.OrderId)
                        thisCmd.Parameters.AddWithValue("@CustomerId", "1538")
                        thisCmd.Parameters.AddWithValue("@OrderNumber", orderData.OrderNumber)
                        thisCmd.Parameters.AddWithValue("@OrderName", orderData.OrderName)
                        thisCmd.Parameters.AddWithValue("@OrderNote", orderData.OrderNote)

                        thisCmd.ExecuteNonQuery()
                    End Using

                    ' ORDER DETAILS
                    If orderData.Details IsNot Nothing AndAlso orderData.Details.Count > 0 Then
                        For Each detail As OrderDetail In orderData.Details
                            Select Case detail.DesignName.Trim().ToLower()
                                Case "Aluminium Blinds"
                                    InsertAluminium(thisConn, transaction, headerId, detail)
                                Case "Venetian Blinds"
                                    InsertAluminium(thisConn, transaction, headerId, detail)
                                Case Else
                                    Throw New Exception("DesignName tidak dikenali: " & detail.DesignName)
                            End Select
                        Next
                    End If

                    transaction.Commit()
                    context.Response.StatusCode = 200
                    context.Response.Write("{""status"":""success"",""message"":""Data diterima dan disimpan dengan sukses""}")
                    context.ApplicationInstance.CompleteRequest()
                Catch ex As Exception
                    transaction.Rollback()
                    context.Response.StatusCode = 500
                    context.Response.Write("{""status"":""error"",""message"":""Database operation failed: " & ex.Message.Replace("""", "\""") & """}")
                    context.ApplicationInstance.CompleteRequest()
                End Try
                thisConn.Close()
            End Using
        Catch ex As Exception
            context.Response.StatusCode = 400
            context.Response.Write("{""status"": ""error"", ""message"": ""Invalid JSON format"", ""details"": """ & ex.Message & """}")
            context.ApplicationInstance.CompleteRequest()
        End Try
    End Sub

    Private Sub InsertAluminium(koneksi As SqlConnection, trans As SqlTransaction, headerId As String, detail As OrderDetail)

        detail.BlindName = "0.21mm"
        Dim designId As String = "1"
        Dim blindId As String = "2"
        Dim priceGroupId As String = "14"

        Dim productColourId As String = "SELECT Id FROM ProductColours WHERE Name='" & detail.ColourType & "'"

        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS subCompanyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='9' AND ControlType='17' AND ColourType='" & productColourId & "' AND subCompanyArray.VALUE='7'")

        Dim productGroupName As String = String.Format("{0} - {1}", detail.DesignName, detail.BlindName)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productGroupName, designId, priceGroupId)

        Dim controlLength As String = "Custom"
        Dim controlLengthValue As Integer = detail.ChainLength

        If detail.ChainLength = 0 Then
            controlLength = "Standard"
            controlLengthValue = Math.Ceiling(detail.Drop * 2 / 3)
            If controlLengthValue < 450 Then controlLengthValue = 450
        End If

        Dim linearMetre As Decimal = detail.Width / 1000
        Dim squareMetre As Decimal = detail.Width * detail.Drop / 1000000

        For i As Integer = 1 To detail.Qty
            Dim itemId As String = orderClass.GetNewOrderItemId()

            Using thisCmd As New SqlCommand("sp_OrderDetails_Insert_Aluminium", koneksi, trans)
                thisCmd.Parameters.AddWithValue("@Id", itemId)
                thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                thisCmd.Parameters.AddWithValue("@ProductId", productId)
                thisCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                thisCmd.Parameters.AddWithValue("@PriceProductGroupIdB", CType(DBNull.Value, Object))
                thisCmd.Parameters.AddWithValue("@Room", detail.Room)
                thisCmd.Parameters.AddWithValue("@Mounting", detail.Mounting)
                thisCmd.Parameters.AddWithValue("@SubType", "Single")
                thisCmd.Parameters.AddWithValue("@ControlPosition", detail.ControlPosition)
                thisCmd.Parameters.AddWithValue("@TilterPosition", detail.TilterPosition)
                thisCmd.Parameters.AddWithValue("@Width", detail.Width)
                thisCmd.Parameters.AddWithValue("@Drop", detail.Drop)
                thisCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                thisCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                thisCmd.Parameters.AddWithValue("@WandLength", controlLength)
                thisCmd.Parameters.AddWithValue("@WandLengthValue", controlLengthValue)
                thisCmd.Parameters.AddWithValue("@ControlPositionB", String.Empty)
                thisCmd.Parameters.AddWithValue("@TilterPositionB", String.Empty)
                thisCmd.Parameters.AddWithValue("@WidthB", 0)
                thisCmd.Parameters.AddWithValue("@DropB", 0)
                thisCmd.Parameters.AddWithValue("@ControlLengthB", String.Empty)
                thisCmd.Parameters.AddWithValue("@ControlLengthValueB", 0)
                thisCmd.Parameters.AddWithValue("@WandLengthB", String.Empty)
                thisCmd.Parameters.AddWithValue("@WandLengthValueB", 0)
                thisCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                thisCmd.Parameters.AddWithValue("@LinearMetreB", 0)
                thisCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                thisCmd.Parameters.AddWithValue("@SquareMetreB", 0)
                thisCmd.Parameters.AddWithValue("@Supply", detail.BottomHoldDown)
                thisCmd.Parameters.AddWithValue("@TotalItems", 1)
                thisCmd.Parameters.AddWithValue("@Notes", detail.Notes)
                thisCmd.Parameters.AddWithValue("@MarkUp", 0)

                thisCmd.ExecuteNonQuery()
            End Using

            orderClass.ResetPriceDetail(headerId, itemId)
            orderClass.CalculatePrice(headerId, itemId)
            orderClass.FinalCostItem(headerId, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, 2, "Order Item Added"}
            orderClass.Logs(dataLog)
        Next
        orderClass.UpdateOrderFactory(headerId)
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class

<Serializable()>
Public Class OrderData
    Public Property Id As Integer
    Public Property OrderId As String
    Public Property JobId As String
    Public Property CustomerId As String
    Public Property OrderNumber As String
    Public Property OrderName As String
    Public Property OrderNote As String
    Public Property OrderType As String
    Public Property Status As String
    Public Property CreatedBy As String
    Public Property Details As List(Of OrderDetail)
End Class

<Serializable()>
Public Class OrderDetail
    Public Property Id As Integer
    Public Property Number As Integer
    Public Property HeaderId As Integer
    Public Property ProductId As String
    Public Property DesignName As String
    Public Property BlindName As String
    Public Property ColourType As String
    Public Property Qty As Integer
    Public Property Room As String
    Public Property Mounting As String
    Public Property Width As Integer
    Public Property Drop As Integer
    Public Property ControlPosition As String
    Public Property TilterPosition As String
    Public Property BottomHoldDown As String ' Hold Down Clip
    Public Property ChainLength As Integer ' Cord Length
    Public Property Notes As String
End Class