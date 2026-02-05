Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class ShutterOceanService
    Public Async Function SendOrderAsync(headerId As String) As Task

        Dim payload As New DataHeader()
        payload.Details = New List(Of DataDetail)()

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
            Await conn.OpenAsync()

            Dim cmdHeader As New SqlCommand("SELECT * FROM OrderHeaders WHERE Id=@Id", conn)
            cmdHeader.Parameters.AddWithValue("@Id", headerId)

            Using rd = Await cmdHeader.ExecuteReaderAsync()
                If Await rd.ReadAsync() Then
                    payload.Id = rd("Id")
                    payload.OrderId = rd("OrderId").ToString()
                    payload.JobId = String.Empty
                    payload.CustomerId = "LS-A224"
                    payload.OrderNumber = rd("OrderNumber").ToString()
                    payload.OrderName = rd("OrderName").ToString()
                    payload.OrderNote = rd("OrderNote").ToString()
                    payload.OrderType = rd("OrderType").ToString()
                    payload.Status = rd("Status").ToString()
                    payload.CreatedBy = rd("CreatedBy").ToString()
                End If
            End Using

            Dim cmdDetail As New SqlCommand("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductColours.Name AS ColourName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId=@HeaderId AND OrderDetails.Active=1 AND Products.DesignId='15'", conn)
            cmdDetail.Parameters.AddWithValue("@HeaderId", headerId)

            Using rdr = Await cmdDetail.ExecuteReaderAsync()
                While Await rdr.ReadAsync()
                    payload.Details.Add(New DataDetail With {
                    .BlindName = rdr("BlindName").ToString(),
                    .Colour = rdr("ColourName").ToString(),
                    .Qty = rdr("Qty").ToString(),
                    .Room = rdr("Room").ToString(),
                    .Mounting = rdr("Mounting").ToString(),
                    .Width = If(IsDBNull(rdr("Width")), 0, rdr("Width")),
                    .Drop = If(IsDBNull(rdr("Drop")), 0, rdr("Drop")),
                    .TrackLength = If(IsDBNull(rdr("TrackLength")), 0, rdr("TrackLength")),
                    .TrackQty = If(IsDBNull(rdr("TrackQty")), 0, rdr("TrackQty")),
                    .Layout = rdr("LayoutCode").ToString(),
                    .LayoutSpecial = rdr("LayoutCodeCustom").ToString(),
                    .PanelQty = If(IsDBNull(rdr("PanelQty")), 0, rdr("PanelQty")),
                    .CustomHeaderLength = If(IsDBNull(rdr("CustomHeaderLength")), 0, rdr("CustomHeaderLength")),
                    .SemiInsideMount = rdr("SemiInsideMount").ToString(),
                    .BottomTrackType = rdr("BottomTrackType").ToString(),
                    .BottomTrackRecess = rdr("BottomTrackRecess").ToString(),
                    .LouvreSize = If(IsDBNull(rdr("LouvreSize")), 0, rdr("LouvreSize")),
                    .LouvrePosition = rdr("LouvrePosition").ToString(),
                    .HingeColour = rdr("HingeColour").ToString(),
                    .HingeQtyPerPanel = If(IsDBNull(rdr("HingeQtyPerPanel")), 0, rdr("HingeQtyPerPanel")),
                    .PanelQtyWithHinge = If(IsDBNull(rdr("PanelQtyWithHinge")), 0, rdr("PanelQtyWithHinge")),
                    .MidrailHeight1 = If(IsDBNull(rdr("MidrailHeight1")), 0, rdr("MidrailHeight1")),
                    .MidrailHeight2 = If(IsDBNull(rdr("MidrailHeight2")), 0, rdr("MidrailHeight2")),
                    .MidrailCritical = rdr("MidrailCritical").ToString(),
                    .FrameType = rdr("FrameType").ToString(),
                    .FrameLeft = rdr("FrameLeft").ToString(),
                    .FrameRight = rdr("FrameRight").ToString(),
                    .FrameTop = rdr("FrameTop").ToString(),
                    .FrameBottom = rdr("FrameBottom").ToString(),
                    .Buildout = rdr("Buildout").ToString(),
                    .BuildoutPosition = rdr("BuildoutPosition").ToString(),
                    .LocationTPost1 = If(IsDBNull(rdr("Gap1")), 0, rdr("Gap1")),
                    .LocationTPost2 = If(IsDBNull(rdr("Gap2")), 0, rdr("Gap2")),
                    .LocationTPost3 = If(IsDBNull(rdr("Gap3")), 0, rdr("Gap3")),
                    .LocationTPost4 = If(IsDBNull(rdr("Gap4")), 0, rdr("Gap4")),
                    .LocationTPost5 = If(IsDBNull(rdr("Gap5")), 0, rdr("Gap5")),
                    .HorizontalTPost = rdr("HorizontalTPost").ToString(),
                    .HorizontalTPostHeight = If(IsDBNull(rdr("HorizontalTPostHeight")), 0, rdr("HorizontalTPostHeight")),
                    .JoinedPanels = rdr("JoinedPanels").ToString(),
                    .TiltrodType = rdr("TiltrodType").ToString(),
                    .TiltrodSplit = rdr("TiltrodSplit").ToString(),
                    .SplitHeight1 = If(IsDBNull(rdr("SplitHeight1")), 0, rdr("SplitHeight1")),
                    .SplitHeight2 = If(IsDBNull(rdr("SplitHeight2")), 0, rdr("SplitHeight2")),
                    .ReverseHinged = rdr("ReverseHinged").ToString(),
                    .PelmetFlat = rdr("PelmetFlat").ToString(),
                    .ExtraFascia = rdr("ExtraFascia").ToString(),
                    .HingesLoose = rdr("HingesLoose").ToString(),
                    .DoorCutOut = rdr("DoorCutOut").ToString(),
                    .SpecialShape = rdr("SpecialShape").ToString(),
                    .TemplateProvided = rdr("TemplateProvided").ToString(),
                    .LinearMetre = Convert.ToDecimal(rdr("LinearMetre")),
                    .SquareMetre = Convert.ToDecimal(rdr("SquareMetre"))
                })
                End While
            End Using
        End Using

        Dim jsonBody As String = JsonConvert.SerializeObject(payload)

        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromSeconds(60)

            client.DefaultRequestHeaders.Add("X-API-KEY", "hidupjokowi")

            Dim content As New StringContent(jsonBody, Encoding.UTF8, "application/json")
            Await client.PostAsync("https://onlineorder.au/handler/Json.ashx", content)
        End Using
    End Function

End Class

Public Class DataHeader
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
    Public Property Details As List(Of DataDetail)
End Class

Public Class DataDetail
    Public Property BlindName As String
    Public Property Colour As String
    Public Property Qty As String
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
    Public Property LouvreSize As Integer
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
    Public Property LocationTPost1 As Integer
    Public Property LocationTPost2 As Integer
    Public Property LocationTPost3 As Integer
    Public Property LocationTPost4 As Integer
    Public Property LocationTPost5 As Integer
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
End Class
