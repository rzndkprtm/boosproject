Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Partial Class Spam_ShutterLoop
    Inherits Page

    Private _jsonWritten As Boolean = False

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            RegisterAsyncTask(New PageAsyncTask(AddressOf ExecuteAsync))
        End If
    End Sub

    Private Async Function ExecuteAsync() As Task
        Dim orderId As String = Request.QueryString("orderid")

        Dim order As OrderData = GetOrderFromDB(orderId)

        Dim jsonData As String = JsonConvert.SerializeObject(order, Formatting.Indented)

        Response.Clear()
        Response.ContentType = "application/json"
        Response.Write(jsonData)
        _jsonWritten = True
        Context.ApplicationInstance.CompleteRequest()

        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromSeconds(60)
            client.DefaultRequestHeaders.Clear()
            client.DefaultRequestHeaders.Add("X-API-KEY", "hidupjokowi")

            Using content As New StringContent(jsonData, Encoding.UTF8, "application/json")
                Dim resp As HttpResponseMessage = Await client.PostAsync("https://shutters.onlineorder.au/handler/Json.ashx", content)
                Dim body As String = Await resp.Content.ReadAsStringAsync()

                Response.Clear()
                Response.StatusCode = CInt(resp.StatusCode)
                Response.ContentType = "application/json"
                Response.Write(body)

                Context.ApplicationInstance.CompleteRequest()
            End Using
        End Using
    End Function

    Private Function GetOrderFromDB(orderId As String) As OrderData
        Dim order As New OrderData()
        order.Details = New List(Of OrderDetail)()

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()

            Using thisCmd As New SqlCommand("SELECT * FROM OrderHeaders WHERE Id=@Id", thisConn)
                thisCmd.Parameters.AddWithValue("@Id", orderId)
                Using rdr As SqlDataReader = thisCmd.ExecuteReader()
                    If rdr.Read() Then
                        order.OrderNumber = rdr("OrderNumber").ToString()
                        order.OrderName = rdr("OrderName").ToString()
                    End If
                End Using
            End Using

            Using cmd As New SqlCommand("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductColours.Name AS ColourName FROM OrderDetails INNER JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId=@Id AND Designs.Name='Skyline Shutter Ocean'", thisConn)
                cmd.Parameters.AddWithValue("@Id", orderId)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim detail As New OrderDetail()
                        detail.BlindName = rdr("BlindName").ToString()
                        detail.Colour = rdr("ColourName").ToString()
                        detail.Qty = rdr("Qty")
                        detail.Room = rdr("Room").ToString()
                        detail.Mounting = rdr("Mounting").ToString()
                        detail.Width = rdr("Width")
                        detail.Drop = rdr("Drop")
                        detail.TrackLength = If(IsDBNull(rdr("TrackLength")), 0, rdr("TrackLength"))
                        detail.TrackQty = If(IsDBNull(rdr("TrackQty")), 0, rdr("TrackQty"))
                        detail.Layout = rdr("LayoutCode").ToString()
                        detail.LayoutSpecial = rdr("LayoutCodeCustom").ToString()
                        detail.PanelQty = If(IsDBNull(rdr("PanelQty")), 0, rdr("PanelQty"))
                        detail.CustomHeaderLength = If(IsDBNull(rdr("CustomHeaderLength")), 0, rdr("CustomHeaderLength"))
                        detail.SemiInsideMount = rdr("SemiInsideMount").ToString()
                        detail.BottomTrackType = rdr("BottomTrackType").ToString()
                        detail.BottomTrackRecess = rdr("BottomTrackRecess").ToString()
                        detail.LouvreSize = If(IsDBNull(rdr("LouvreSize")), 0, rdr("LouvreSize"))
                        detail.LouvrePosition = rdr("LouvrePosition").ToString()
                        detail.HingeColour = rdr("HingeColour").ToString()
                        detail.HingeQtyPerPanel = If(IsDBNull(rdr("HingeQtyPerPanel")), 0, rdr("HingeQtyPerPanel"))
                        detail.PanelQtyWithHinge = If(IsDBNull(rdr("PanelQtyWithHinge")), 0, rdr("PanelQtyWithHinge"))
                        detail.MidrailHeight1 = If(IsDBNull(rdr("MidrailHeight1")), 0, rdr("MidrailHeight1"))
                        detail.MidrailHeight2 = If(IsDBNull(rdr("MidrailHeight2")), 0, rdr("MidrailHeight2"))
                        detail.MidrailCritical = rdr("MidrailCritical").ToString()
                        detail.FrameType = rdr("FrameType").ToString()
                        detail.FrameLeft = rdr("FrameLeft").ToString()
                        detail.FrameRight = rdr("FrameRight").ToString()
                        detail.FrameTop = rdr("FrameTop").ToString()
                        detail.FrameBottom = rdr("FrameBottom").ToString()
                        detail.Buildout = rdr("Buildout").ToString()
                        detail.BuildoutPosition = rdr("BuildoutPosition").ToString()
                        detail.LocationTPost1 = If(IsDBNull(rdr("Gap1")), 0, rdr("Gap1"))
                        detail.LocationTPost2 = If(IsDBNull(rdr("Gap2")), 0, rdr("Gap2"))
                        detail.LocationTPost3 = If(IsDBNull(rdr("Gap3")), 0, rdr("Gap3"))
                        detail.LocationTPost4 = If(IsDBNull(rdr("Gap4")), 0, rdr("Gap4"))
                        detail.LocationTPost5 = If(IsDBNull(rdr("Gap5")), 0, rdr("Gap5"))
                        detail.HorizontalTPost = rdr("HorizontalTPost").ToString()
                        detail.HorizontalTPostHeight = If(IsDBNull(rdr("HorizontalTPostHeight")), 0, rdr("HorizontalTPostHeight"))
                        detail.JoinedPanels = rdr("JoinedPanels").ToString()
                        detail.TiltrodType = rdr("TiltrodType").ToString()
                        detail.TiltrodSplit = rdr("TiltrodType").ToString()
                        detail.SplitHeight1 = If(IsDBNull(rdr("SplitHeight1")), 0, rdr("SplitHeight1"))
                        detail.SplitHeight2 = If(IsDBNull(rdr("SplitHeight2")), 0, rdr("SplitHeight2"))
                        detail.ReverseHinged = rdr("ReverseHinged").ToString()
                        detail.PelmetFlat = rdr("PelmetFlat").ToString()
                        detail.ExtraFascia = rdr("ExtraFascia").ToString()
                        detail.HingesLoose = rdr("HingesLoose").ToString()
                        detail.DoorCutOut = rdr("DoorCutOut").ToString()
                        detail.SpecialShape = rdr("SpecialShape").ToString()
                        detail.TemplateProvided = rdr("TemplateProvided").ToString()
                        detail.LinearMetre = rdr("LinearMetre")
                        detail.SquareMetre = rdr("SquareMetre")
                        order.Details.Add(detail)
                    End While
                End Using
            End Using
        End Using

        Return order
    End Function

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If _jsonWritten Then
            Return
        End If
        MyBase.Render(writer)
    End Sub

    Private Sub WritePlain(msg As String)
        Response.Clear()
        Response.ContentType = "text/plain"
        Response.Write(msg)
        _jsonWritten = True
        Context.ApplicationInstance.CompleteRequest()
    End Sub

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
End Class
