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
            context.Response.StatusCode = 401 ' Unauthorized
            context.Response.Write("{""status"": ""error"", ""message"": ""Invalid or missing API key""}")
            Return
        End If

        If context.Request.HttpMethod <> "POST" Then
            context.Response.StatusCode = 405 ' Method Not Allowed
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

                    '#OrderHeader
                    Using thisCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, OrderFactory, OrderContact, OrderAddress, OrderContainer, Status, CreatedBy, CreatedDate, SubmittedDate, Payment, Amount, Download, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'BIG', '', '', 'NSW', 'New Order', 1568, GETDATE(), GETDATE(), 0, 0, 'No', 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn, transaction)
                        thisCmd.Parameters.AddWithValue("@Id", headerId)
                        thisCmd.Parameters.AddWithValue("@OrderId", orderData.OrderId)
                        thisCmd.Parameters.AddWithValue("@CustomerId", "1538")
                        thisCmd.Parameters.AddWithValue("@OrderNumber", orderData.OrderNumber)
                        thisCmd.Parameters.AddWithValue("@OrderName", orderData.OrderName)
                        thisCmd.Parameters.AddWithValue("@OrderNote", orderData.OrderNote)

                        thisCmd.ExecuteNonQuery()
                    End Using

                    If orderData.Details IsNot Nothing AndAlso orderData.Details.Count > 0 Then

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
    Public Property BlindName As String
    Public Property Colour As String
    Public Property ExactId As String
    Public Property ProductPriceGroupId As String
    Public Property Qty As Integer
    Public Property Room As String
    Public Property Mounting As String
    Public Property Width As Integer
    Public Property Drop As Integer
    Public Property TrackLength As Integer
    Public Property TrackQty As Integer
    Public Property Layout As String
    Public Property LayoutSpecial As String
    Public Property PanelQty As Integer
    Public Property CustomHeaderLength As Integer
    Public Property SemiInsideMount As String
    Public Property BottomTrackType As String
    Public Property BottomTrackRecess As String
    Public Property LouvreSize As String
    Public Property LouvrePosition As String
    Public Property HingeColour As String
    Public Property HingeQtyPerPanel As Integer
    Public Property PanelQtyWithHinge As Integer
    Public Property MidrailHeight1 As Integer
    Public Property MidrailHeight2 As Integer
    Public Property MidrailCritical As String
    Public Property FrameType As String
    Public Property FrameLeft As String
    Public Property FrameRight As String
    Public Property FrameTop As String
    Public Property FrameBottom As String
    Public Property Buildout As String
    Public Property BuildoutPosition As String
    Public Property LocationTPost1 As String
    Public Property LocationTPost2 As String
    Public Property LocationTPost3 As String
    Public Property LocationTPost4 As String
    Public Property LocationTPost5 As String
    Public Property HorizontalTPost As String
    Public Property HorizontalTPostHeight As Integer
    Public Property JoinedPanels As String
    Public Property TiltrodType As String
    Public Property TiltrodSplit As String
    Public Property SplitHeight1 As Integer
    Public Property SplitHeight2 As Integer
    Public Property ReverseHinged As String
    Public Property PelmetFlat As String
    Public Property ExtraFascia As String
    Public Property HingesLoose As String
    Public Property DoorCutOut As String
    Public Property SpecialShape As String
    Public Property TemplateProvided As String
    Public Property LinearMetre As Decimal
    Public Property SquareMetre As Decimal
    Public Property Notes As String
    Public Property Cost As Decimal
    Public Property CostOverride As Decimal
    Public Property Discount As Decimal
    Public Property FinalCost As Decimal
    Public Property MarkUp As Decimal
    Public Property TotalBlinds As Integer
    Public Property Production As String
    Public Property Paid As Integer
End Class