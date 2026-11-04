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

            groups.Add(SearchPages(keyword))
            groups.Add(SearchCustomer(keyword))
            groups.Add(SearchDesignType(keyword))
            groups.Add(SearchBlindType(keyword))
            groups.Add(SearchProduct(keyword))
            groups.Add(SearchFabric(keyword))
            groups.Add(SearchFabricColour(keyword))
            groups.Add(SearchOrders(keyword))

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

    Private Function ExecuteSearch(category As String, icon As String, sql As String, keyword As String, url As String) As SearchGroup
        Dim items As New List(Of SearchItem)
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(sql, thisConn)
                thisCmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value = "%" & keyword & "%"

                thisConn.Open()
                Using reader As SqlDataReader = thisCmd.ExecuteReader()
                    While reader.Read()
                        Dim resultId As String = Convert.ToString(reader("ResultId"))
                        items.Add(New SearchItem With {
                            .Title = Convert.ToString(reader("Title")),
                            .Description = Convert.ToString(reader("Description")),
                            .Url = ResolveUrl(url & resultId)
                        })
                    End While
                End Using
            End Using
        End Using

        Return New SearchGroup With {.Category = category, .Icon = icon, .Count = items.Count, .Items = items}
    End Function

    Private Function SearchPages(keyword As String) As SearchGroup
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

            Return New SearchGroup With {.Category = "Pages", .Icon = "bi bi-file-earmark-text-fill", .Count = items.Count, .Items = items}
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Pages", .Icon = "bi bi-file-earmark-text-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchCustomer(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(Id AS NVARCHAR(50)) AS ResultId, ISNULL(Name,'') AS Title, 'Debtor Code : ' + ISNULL(DebtorCode,'') AS Description FROM Customers WHERE Name LIKE @Keyword ORDER BY Name ASC"

            Return ExecuteSearch("Customer", "bi bi-gear-fill", sql, keyword, "~/setting/customer/detail?customerid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Design Type", .Icon = "bi bi-gear-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchDesignType(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(D.Id AS NVARCHAR(50)) AS ResultId, ISNULL(D.Name,'') AS Title, ISNULL(C.CompanyAlias, '') AS Description FROM Designs D OUTER APPLY (SELECT STRING_AGG(CO.Alias, ', ') AS CompanyAlias FROM STRING_SPLIT(D.CompanyId, ',') S INNER JOIN Companys CO ON CO.Id = TRY_CAST(LTRIM(RTRIM(S.value)) AS INT)) C WHERE D.Name LIKE @Keyword ORDER BY D.Name ASC"

            Return ExecuteSearch("Design Type", "bi bi-gear-fill", sql, keyword, "~/setting/specification/design/edit?designid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Design Type", .Icon = "bi bi-gear-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchBlindType(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(B.Id AS NVARCHAR(50)) AS ResultId, ISNULL(B.Name,'') AS Title, ISNULL(CD.CompanyDetailName, '') AS Description FROM Blinds B OUTER APPLY (SELECT STRING_AGG(CD.Name, ', ') AS CompanyDetailName FROM STRING_SPLIT(B.CompanyDetailId, ',') S INNER JOIN CompanyDetails CD ON CD.Id = TRY_CAST(LTRIM(RTRIM(S.value)) AS INT)) CD WHERE B.Name LIKE @Keyword ORDER BY B.Name ASC"

            Return ExecuteSearch("Blind Type", "bi bi-gear-fill", sql, keyword, "~/setting/specification/blind/edit?blindid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Design Type", .Icon = "bi bi-gear-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchProduct(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(Products.Id AS NVARCHAR(50)) AS ResultId, ISNULL(Products.Name,'') AS Title, 'Design Type : ' + ISNULL(Designs.Name,'') + CHAR(10) + 'Blind Type : ' + ISNULL(Blinds.Name,'') + CHAR(10) + 'Status : ' + ISNULL(Products.Status,'') AS Description FROM Products LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE Products.Name LIKE @Keyword ORDER BY Products.Name ASC"

            Return ExecuteSearch("Product", "bi bi-gear-fill", sql, keyword, "~/setting/specification/product/detail?productid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Product", .Icon = "bi bi-gear-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchFabric(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(Id AS NVARCHAR(50)) AS ResultId, ISNULL(Name,'') AS Title, 'Type : ' + ISNULL(Type,'') + CHAR(10) + 'Status : ' + ISNULL(Status,'') AS Description FROM Fabrics WHERE Name LIKE @Keyword ORDER BY Name ASC"

            Return ExecuteSearch("Fabric Type", "bi bi-gear-fill", sql, keyword, "~/setting/specification/fabric/colour/edit?fabriccolourid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Fabric Type", .Icon = "bi bi-gear-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchFabricColour(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(Id AS NVARCHAR(50)) AS ResultId, ISNULL(Name,'') AS Title, 'BOE ID : ' + ISNULL(CAST(BoeId AS NVARCHAR(50)),'') + CHAR(10) + 'Status : ' + ISNULL(CAST(Status AS NVARCHAR(50)),'') AS Description FROM FabricColours WHERE Colour LIKE @Keyword ORDER BY Name ASC"

            Return ExecuteSearch("Fabric Colour", "bi bi-gear-fill", sql, keyword, "~/setting/specification/fabric/detail?fabricid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Fabric Colour", .Icon = "bi bi-gear-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function

    Private Function SearchOrders(keyword As String) As SearchGroup
        Try
            Dim sql As String = "SELECT TOP 20 CAST(OrderHeaders.Id AS NVARCHAR(50)) AS ResultId, ISNULL(OrderHeaders.OrderId,'') AS Title, 'Customer Name : ' + ISNULL(Customers.Name,'') + CHAR(10) + 'Order Number : ' + ISNULL(OrderHeaders.OrderNumber,'') + CHAR(10) + 'Order Name : ' + ISNULL(OrderHeaders.OrderName,'') AS Description FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.OrderNumber LIKE @Keyword OR Customers.Name LIKE @Keyword ORDER BY OrderHeaders.Id DESC"

            Return ExecuteSearch("Orders", "bi bi-basket-fill", sql, keyword, "~/order/detail?orderid=")
        Catch ex As Exception
            Return New SearchGroup With {.Category = "Orders", .Icon = "bi bi-basket-fill", .Count = 0, .Items = New List(Of SearchItem)}
        End Try
    End Function
End Class
