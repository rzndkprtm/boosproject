Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class Search
    Inherits Page

    Private ReadOnly myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class SearchItem
        Public Property Title As String
        Public Property Description As String
        Public Property Url As String
    End Class

    Public Class SearchGroup
        Public Property Category As String
        Public Property Icon As String
        Public Property Count As Integer
        Public Property Items As List(Of SearchItem)
    End Class

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim keyword As String = Request.QueryString("q")

            If Not String.IsNullOrWhiteSpace(keyword) Then
                txtSearch.Text = keyword
                PerformSearch(keyword)
            End If
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Dim keyword As String = txtSearch.Text.Trim()

        If String.IsNullOrEmpty(keyword) Then
            pnlResults.Visible = False
            lblSearchInfo.Text = String.Empty
            lblError.Text = "PLEASE ENTER A SEARCH KEYWORD !"
            Exit Sub
        End If

        Response.Redirect("~/search?q=" & Server.UrlEncode(keyword), False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Sub PerformSearch(keyword As String)
        Try
            Dim groups As New List(Of SearchGroup)

            lblError.Text = String.Empty

            groups.Add(SearchPage(keyword))
            groups.Add(SearchCustomer(keyword))
            groups.Add(SearchDesignType(keyword))
            groups.Add(SearchBlindType(keyword))
            groups.Add(SearchProduct(keyword))
            groups.Add(SearchFabric(keyword))
            groups.Add(SearchFabricColour(keyword))
            groups.Add(SearchBottomType(keyword))
            groups.Add(SearchBottomColour(keyword))
            groups.Add(SearchOrder(keyword))
            groups.Add(SearchOrderDetail(keyword))
            groups.Add(SearchLogin(keyword))
            groups.Add(SearchMailing(keyword))
            groups.Add(SearchTicket(keyword))

            Dim resultGroups = groups.Where(Function(x) x.Count > 0).ToList()

            rptCategories.DataSource = resultGroups
            rptCategories.DataBind()

            pnlResults.Visible = True
            pnlNoResults.Visible = (resultGroups.Count = 0)

            Dim totalResults As Integer = groups.Sum(Function(x) x.Count)

            lblSearchInfo.Text = totalResults.ToString() & " results found for '" & Server.HtmlEncode(keyword) & "'"
        Catch ex As Exception
            lblError.Text = "UNABLE TO PERFORM SEARCH. PLEASE TRY AGAIN !"
        End Try
    End Sub

    Private Function ExecuteSearch(type As String, category As String, icon As String, keyword As String, url As String) As SearchGroup
        Try
            Dim items As New List(Of SearchItem)

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Using cmd As New SqlCommand("sp_GlobalSearch", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = type
                    cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 255).Value = keyword

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            items.Add(New SearchItem With {
                            .Title = reader("Title").ToString(),
                            .Description = reader("Description").ToString(),
                            .Url = ResolveUrl(url & reader("ResultId").ToString())
                        })
                        End While
                    End Using
                End Using
            End Using

            Return New SearchGroup With {
                .Category = category,
                .Icon = icon,
                .Count = items.Count,
                .Items = items
            }

        Catch ex As Exception
            Return New SearchGroup With {
                .Category = category,
                .Icon = icon,
                .Count = 0,
                .Items = New List(Of SearchItem)
            }
        End Try
    End Function

    Private Function SearchPage(keyword As String) As SearchGroup
        Try
            Dim items As New List(Of SearchItem)
            Dim rootPath As String = Server.MapPath("~/")

            Dim files As String() = Directory.GetFiles(rootPath, "*.aspx", SearchOption.AllDirectories)

            For Each file As String In files
                Try
                    Dim relativePath As String = file.Substring(rootPath.Length).Replace("\", "/")

                    If relativePath.StartsWith("bin/", StringComparison.OrdinalIgnoreCase) OrElse relativePath.StartsWith("obj/", StringComparison.OrdinalIgnoreCase) OrElse relativePath.StartsWith("packages/", StringComparison.OrdinalIgnoreCase) Then
                        Continue For
                    End If

                    If relativePath.Equals("Search.aspx", StringComparison.OrdinalIgnoreCase) Then
                        Continue For
                    End If

                    Dim pageName As String = Path.GetFileNameWithoutExtension(file)
                    Dim pageTitle As String = pageName

                    If pageName.Equals("Default", StringComparison.OrdinalIgnoreCase) Then
                        pageTitle = New DirectoryInfo(Path.GetDirectoryName(file)).Name
                    End If

                    Dim content As String = System.IO.File.ReadAllText(file)
                    Dim match As Match = Regex.Match(content, "Title\s*=\s*""([^""]*)""", RegexOptions.IgnoreCase)

                    If match.Success AndAlso Not String.IsNullOrWhiteSpace(match.Groups(1).Value) Then
                        pageTitle = match.Groups(1).Value

                        If pageName.Equals("Default", StringComparison.OrdinalIgnoreCase) Then
                            pageTitle = New DirectoryInfo(Path.GetDirectoryName(file)).Name
                        End If
                    End If

                    If pageName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 OrElse pageTitle.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 OrElse relativePath.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                        Dim pageUrl As String = relativePath.Substring(0, relativePath.Length - 5).ToLowerInvariant()
                        If pageName.Equals("Default", StringComparison.OrdinalIgnoreCase) Then
                            Dim lastSlash As Integer = pageUrl.LastIndexOf("/")

                            If lastSlash >= 0 Then
                                pageUrl = pageUrl.Substring(0, lastSlash)
                            End If
                        End If

                        items.Add(New SearchItem With {.Title = pageTitle, .Description = "Page : " & relativePath, .Url = ResolveUrl("~/" & pageUrl)})
                    End If
                Catch ex As Exception
                    Continue For
                End Try
            Next

            items = items.OrderBy(Function(x) x.Title).Take(20).ToList()

            Return New SearchGroup With {.Category = "Pages", .Icon = "bi bi-search", .Count = items.Count, .Items = items}
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Pages", .Icon = "bi bi-search", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchCustomer(keyword As String) As SearchGroup
        Return ExecuteSearch("Customer", "Customer", "bi bi-search", keyword, "~/setting/customer/detail?customerid=")
    End Function

    Private Function SearchDesignType(keyword As String) As SearchGroup
        Return ExecuteSearch("DesignType", "Design Type", "bi bi-search", keyword, "~/setting/specification/design/edit?designid=")
    End Function

    Private Function SearchBlindType(keyword As String) As SearchGroup
        Return ExecuteSearch("BlindType", "Blind Type", "bi bi-search", keyword, "~/setting/specification/blind/edit?blindid=")
    End Function

    Private Function SearchProduct(keyword As String) As SearchGroup
        Return ExecuteSearch("Product", "Product", "bi bi-search", keyword, "~/setting/specification/product/detail?productid=")
    End Function

    Private Function SearchFabric(keyword As String) As SearchGroup
        Return ExecuteSearch("Fabric", "Fabric Type", "bi bi-search", keyword, "~/setting/specification/fabric/colour/edit?fabriccolourid=")
    End Function

    Private Function SearchFabricColour(keyword As String) As SearchGroup
        Return ExecuteSearch("FabricColour", "Fabric Colour", "bi bi-search", keyword, "~/setting/specification/fabric/detail?fabricid=")
    End Function

    Private Function SearchBottomType(keyword As String) As SearchGroup
        Return ExecuteSearch("BottomType", "Bottom Type", "bi bi-search", keyword, "~/setting/specification/bottom/detail?bottomid=")
    End Function

    Private Function SearchBottomColour(keyword As String) As SearchGroup
        Return ExecuteSearch("BottomColour", "Bottom Colour", "bi bi-search", keyword, "~/setting/specification/bottom/detail?bottomid=")
    End Function

    Private Function SearchOrder(keyword As String) As SearchGroup
        Return ExecuteSearch("Order", "Orders", "bi bi-search", keyword, "~/order/detail?orderid=")
    End Function

    Private Function SearchOrderDetail(keyword As String) As SearchGroup
        Return ExecuteSearch("OrderDetail", "Order Detail", "bi bi-search", keyword, "~/order/detail?orderid=")
    End Function

    Private Function SearchLogin(keyword As String) As SearchGroup
        Return ExecuteSearch("Login", "User Login", "bi bi-search", keyword, "~/setting/login/user/edit?loginid=")
    End Function

    Private Function SearchMailing(keyword As String) As SearchGroup
        Return ExecuteSearch("Mailing", "Mailing", "bi bi-search", keyword, "~/setting/general/mailing/edit?mailid=")
    End Function

    Private Function SearchTicket(keyword As String) As SearchGroup
        Return ExecuteSearch("Ticket", "Ticket", "bi bi-search", keyword, "~/ticket/?id=")
    End Function

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
