Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class ShutterOceanClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Async Function SendOrderAsync(orderId As String) As Task(Of String)
        Dim order As OrderData = GetOrderFromDB(orderId)
        Dim jsonData As String = JsonConvert.SerializeObject(order, Formatting.Indented)

        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromSeconds(60)
            client.DefaultRequestHeaders.Clear()
            client.DefaultRequestHeaders.Add("X-API-KEY", "hidupjokowi")

            Using content As New StringContent(jsonData, Encoding.UTF8, "application/json")
                Dim resp As HttpResponseMessage = Await client.PostAsync("https://onlineorder.au/handler/Json.ashx", content)
                Return Await resp.Content.ReadAsStringAsync()
            End Using
        End Using
    End Function

    Public Shared Function GetOrderFromDB(orderId As String) As OrderData
        Dim order As New OrderData()
        order.Details = New List(Of OrderDetail)()

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim orderClass As New OrderClass

        Dim orderHeader As DataRow = orderClass.GetDataRow("SELECT * FROM OrderHeaders WHERE Id='" & orderId & "'")
        order.OrderNumber = orderHeader("OrderNumber").ToString()
        order.OrderName = orderHeader("OrderName").ToString()

        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()

            Using cmd As New SqlCommand("Select OrderDetails.*, Blinds.Name AS BlindName, ProductColours.Name AS ColourName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id LEFT JOIN ProductColours ON Products.ColourType = ProductColours.Id WHERE OrderDetails.HeaderId=@Id AND Designs.Name='Skyline Shutter Ocean'", thisConn)
                cmd.Parameters.AddWithValue("@Id", orderId)

                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim detail As New OrderDetail() With {
                            .BlindName = rdr("BlindName").ToString(),
                            .Colour = rdr("ColourName").ToString(),
                            .Qty = rdr("Qty").ToString(),
                            .Room = rdr("Room").ToString(),
                            .Mounting = rdr("Mounting").ToString(),
                            .Width = rdr("Width").ToString(),
                            .Drop = rdr("Drop").ToString(),
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
                            .TiltrodSplit = rdr("TiltrodType").ToString(),
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
                        }

                        order.Details.Add(detail)
                    End While
                End Using
            End Using
        End Using

        Return order
    End Function
End Class

Public Class OrderData
    Public Property OrderNumber As String
    Public Property OrderName As String
    Public Property Details As List(Of OrderDetail)
End Class

Public Class OrderDetail
    Public Property BlindName As String
    Public Property Colour As String
    Public Property Qty As String
    Public Property Room As String
    Public Property Mounting As String
    Public Property Width As String
    Public Property Drop As String
    Public Property TrackLength As String
    Public Property TrackQty As String
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